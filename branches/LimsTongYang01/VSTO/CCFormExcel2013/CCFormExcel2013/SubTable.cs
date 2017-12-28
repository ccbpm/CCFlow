using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace CCFormExcel2013
{
	/// <summary>
	/// 
	/// 注意：
	/// Connection中仅记录当前Excel区域中存在的『原始数据行』的关联信息，不记录new/delete的情况，new/delete在Data["Idx"]中记录
	/// </summary>
	public class SubTable
	{
		#region 成员

		private string _name;
		private Excel.Range _range; //子表区域
		private int _Ro; //子表开始行数
		private int _rangeRows; //子表区域行数

		private DataTable _originData; //原始数据
		private DataTable _newData; //新数据

		private int _TableHeadHeight; //表头高度
		private Dictionary<int, string> _columns; //表头（列绑定）信息：col:KeyOfEn
		private string _PkColumnName; //主键列名
		private Dictionary<int, string> _rowsConnection; //行绑定关系：rowidInExcel:rowPK
		private Stack _operations; //操作序列

		#endregion

		#region 属性-Property

		/// <summary>
		/// 子表名称（子表区域命名）
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// 子表区域
		/// </summary>
		public Excel.Range Range
		{
			get { return _range; }
		}

		/// <summary>
		/// Excel中数据行数（实时）// 先TODO: 不准确，因为有可能出现子表区域有10行数据，但实际有效数据只有5行的情况，此时不进行“插入行”操作，直接在空行中填入数据，也是新建行，但是不会触发this.InsertRow()，也就不会修改RowsConnection
		/// </summary>
		public int RowsCount
		{
			get { return this._rowsConnection.Count; }
		}

		/// <summary>
		/// 表头所占高度
		/// </summary>
		public int TableHeadHeight
		{
			get { return _TableHeadHeight; }
		}

		/// <summary>
		/// 字段信息（Range.Column:KeyOfEn）
		/// </summary>
		public Dictionary<int, string> Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// 子表数据
		/// </summary>
		public DataTable Data
		{
			get { return _newData; }
			set { _newData = value; }
		}

		/// <summary>
		/// 原始子表数据
		/// </summary>
		public DataTable OriginData
		{
			get { return _originData; }
			set { _originData = value; }
		}


		public bool IsInsertRow
		{
			get { return this._rangeRows < this.Range.Rows.Count; }
		}
		public bool IsDeteteRow
		{
			get { return this._rangeRows > this.Range.Rows.Count; }
		}
		#endregion

		#region 方法

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="range"></param>
		/// <param name="data"></param>
		public SubTable(Excel.Range range, DataTable data, Dictionary<int, string> columns, int intTableHeadHeight, string strPkColumnName)
		{
			this._range = range;
			this._Ro = range.Row;
			this._rangeRows = range.Rows.Count;
			this._originData = data.Copy();
			this._newData = data.Copy();
			this._name = data.TableName;

			this._columns = columns;
			this._TableHeadHeight = intTableHeadHeight;
			this._PkColumnName = strPkColumnName;
			this._rowsConnection = new Dictionary<int, string>();

			this._operations = new Stack();
			if (!_newData.Columns.Contains("Idx"))
			{
				_newData.Columns.Add("Idx");
			}
			else
			{
				foreach (DataRow dr in _newData.Rows) //!若Excel表单数据与数据库数据不一致，则需修改此方法
				{
					if (!string.IsNullOrEmpty(dr["Idx"].ToString()))
						this.SetConnection(int.Parse(dr["Idx"].ToString()), _newData.Rows.IndexOf(dr));
				}
			}
		}

		/// <summary>
		/// 设置行关联（同时保存到this.Data和this.Connection中）（暂定：只允许在填充数据时执行）
		/// 注意：插入行时不要执行该方法！
		/// </summary>
		/// <param name="Idx"></param>
		/// <param name="rowInDatatable"></param>
		public void SetConnection(int Idx, int rowInDatatable)
		{
			if (_rowsConnection.ContainsKey(Idx))
			{
				_newData.Rows[rowInDatatable]["Idx"] = Idx;
				_rowsConnection[Idx] = _newData.Rows[rowInDatatable][_PkColumnName].ToString();
			}
			else
			{
				_newData.Rows[rowInDatatable]["Idx"] = Idx;
				_rowsConnection.Add(Idx, _newData.Rows[rowInDatatable][_PkColumnName].ToString());
			}
		}
		/*
		public void SetConnection(int Idx, string OID)
		{
			if (_rowsConnection.Contains(Idx))
			{
				_newData.Rows[rowInDatatable]["Idx"] = Idx;
				_rowsConnection[Idx] = _newData.Rows[rowInDatatable][_PkColumnName];
			}
			else
			{
				_newData.Rows[rowInDatatable]["Idx"] = Idx;
				_rowsConnection.Add(Idx, _newData.Rows[rowInDatatable][_PkColumnName]);
			}
		}*/

		/// <summary>
		/// 初始化行关联（用于表单数据已填充到Excel时执行）
		/// </summary>
		public void InitConnection()
		{
			if (!_newData.Columns.Contains("Idx"))
			{
				_newData.Columns.Add("Idx");
				return;
			}
			_rowsConnection = new Dictionary<int, string>();
			foreach (DataRow dr in _newData.Rows) //!若Excel表单数据与数据库数据不一致，则需修改此方法
			{
				if (!string.IsNullOrEmpty(dr["Idx"].ToString()))
					this.SetConnection(int.Parse(dr["Idx"].ToString()), _newData.Rows.IndexOf(dr));
			}
		}

		/// <summary>
		/// 刷新行关联（用于在子表前插入行时更新行关联）
		/// </summary>
		public void RefreshConnection()
		{
			if (this.Range.Row != this._Ro) //在子表前插入行时
			{
				var diff = this.Range.Row - this._Ro;
				this._rowsConnection = new Dictionary<int, string>();
				foreach (DataRow dr in this.Data.Rows)
				{
					if (!string.IsNullOrEmpty(dr["Idx"].ToString()))
					{
						dr["Idx"] = (int)dr["Idx"] + diff;
						_rowsConnection.Add((int)dr["Idx"], dr[_PkColumnName].ToString());
					}
				}
			}
			//else if (this._rangeRows != this.Range.Rows.Count) //在子表中插入行时 //无法判断是在第几行插入的，所以无法处理这种情况
			//{}

		}

		/// <summary>
		/// 获取Idx关联的OID
		/// </summary>
		/// <param name="Idx"></param>
		/// <returns></returns>
		public string GetOidByRowid(int Idx)
		{
			if (_rowsConnection.ContainsKey(Idx))
				return _rowsConnection[Idx].ToString();
			else
				return null;
		}

		/// <summary>
		/// 获取子表某一列在WorkSheet中的Column
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public int GetColumnCx(string column)
		{
			foreach (KeyValuePair<int, string> col in _columns)
			{
				if (col.Value == column)
					return col.Key;
			}
			return -1;
		}

		/// <summary>
		/// 获取子表中某一列绑定的字段名
		/// </summary>
		/// <param name="rangeColumn"></param>
		/// <returns></returns>
		public string GetColumnName(int rangeColumn)
		{
			if (_columns.ContainsKey(rangeColumn))
				return _columns[rangeColumn];
			else
				return null;
		}

		/// <summary>
		/// 获取子表中某一列绑定的字段名
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public string GetColumnName(Excel.Range range)
		{
			if (_columns.ContainsKey(range.Column))
				return _columns[range.Column];
			else
				return null;
		}

		/// <summary>
		/// 新建行操作
		/// </summary>
		/// <param name="Idx"></param>
		public void InsertRow(int Idx, Excel.Range range = null)
		{
			//记录操作
			_operations.Push(new RowOperation(Idx, _rowsConnection[Idx], OperationType.New));

			//更改行关联
			var point = _range.Row + _TableHeadHeight + _rowsConnection.Count; //指向最后一行（新的）
			_rowsConnection.Add(point, null);
			while (point > Idx)
			{
				_rowsConnection[point] = _rowsConnection[point - 1];
				point--;
			} //循环结束时:point==Idx
			//x_rowsConnection[Idx] = "new"; //不记录新插入行的关联信息，便于在保存时区分已有行和新建行
			_rowsConnection.Remove(point);

			//更新子表区域
			if (range != null)
				_range = range;
		}

		/// <summary>
		/// 撤销新增行操作
		/// </summary>
		/// <param name="Idx"></param>
		/// <param name="range"></param>
		private void UndoInsertRow(int Idx, Excel.Range range = null)
		{
			//更改行关联（所有）
			var point = Idx; //指向当前行（被撤销的『新增行』）
			while (_rowsConnection.ContainsKey(point + 1))
			{
				_rowsConnection[point] = _rowsConnection[point + 1];
				point++;
			} //循环结束时:point==存在的最大行号
			_rowsConnection.Remove(point);

			//更新子表区域
			if (range != null)
				_range = range;
		}

		/// <summary>
		/// 删除行操作
		/// </summary>
		/// <param name="Idx"></param>
		public void DeleteRow(int Idx, Excel.Range range = null)
		{
			//TODO: 删除多行的情况
			//记录操作
			_operations.Push(new RowOperation(Idx, _rowsConnection[Idx], OperationType.Delete));

			//更改newDataTalble中的Status: _newData.Select("OID={0}")[0][Idx or RowStatus]
			_newData.Select(string.Format("OID='{0}'", _rowsConnection[Idx]))[0]["Idx"] = RowStatus.Deleted.ToString();

			//更改行关联
			var point = Idx; //指向被删除的行
			while (_rowsConnection.ContainsKey(point + 1))
			{
				_rowsConnection[point] = _rowsConnection[point + 1];
				point++;
			} //循环结束时:point==存在的最大行号
			_rowsConnection.Remove(point);

			//更新子表区域
			if (range != null)
				_range = range;
		}

		/// <summary>
		/// 撤销新增行操作
		/// </summary>
		/// <param name="Idx"></param>
		/// <param name=_PkColumnName></param>
		/// <param name="range"></param>
		private void UndoDeleteRow(int Idx, string rowPk, Excel.Range range = null)
		{
			//更改newDataTalble中的Status: _newData.Select("OID={0}")[0][Idx or RowStatus]
			_newData.Select(string.Format("OID='{0}'", _rowsConnection[Idx]))[0]["Idx"] = RowStatus.Origin.ToString();

			//更改行关联
			var point = _range.Row + _TableHeadHeight + _rowsConnection.Count; //指向最后一行
			_rowsConnection.Add(point, null);
			while (point > Idx)
			{
				_rowsConnection[point] = _rowsConnection[point - 1];
				point--;
			} //循环结束时:point==Idx
			_rowsConnection[Idx] = rowPk;

			//更新子表区域
			if (range != null)
				_range = range;
		}

		/// <summary>
		/// 撤销操作
		/// </summary>
		/// <param name="range"></param>
		public void Undo(Excel.Range range = null)
		{
			if (_operations.Count == 0) return;
			RowOperation oper = (RowOperation)_operations.Pop();
			if (oper.OperationType == OperationType.New)
			{
				if (range != null)
					this.UndoInsertRow(oper.Row + 1, range);
				else
					this.UndoInsertRow(oper.Row + 1);
			}
			else if (oper.OperationType == OperationType.Delete)
			{
				if (range != null)
					this.UndoDeleteRow(oper.Row, oper.RowPk, range);
				else
					this.UndoDeleteRow(oper.Row, oper.RowPk);

			}
		}

		#endregion
	}

	/// <summary>
	/// 行操作类型
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// 新建行
		/// </summary>
		New,
		/// <summary>
		/// 已删除的行
		/// </summary>
		Delete
	}

	/// <summary>
	/// 行状态
	/// </summary>
	public enum RowStatus
	{
		/// <summary>
		/// 原始行
		/// </summary>
		Origin,
		/// <summary>
		/// 新建的行
		/// </summary>
		New,
		/// <summary>
		/// 已删除的行
		/// </summary>
		Deleted
	}

	/// <summary>
	/// 行操作
	/// </summary>
	struct RowOperation
	{
		//成员
		private int _row;
		private string _rowPk;
		private OperationType _operationType;

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="row">行(Idx)</param>
		/// <param name="rowPk">rowPk</param>
		/// <param name="operationType">操作类型</param>
		public RowOperation(int row, string rowPk, OperationType operationType)
		{
			this._row = row;
			this._rowPk = rowPk;
			this._operationType = operationType;
		}

		#region 属性
		/// <summary>
		/// 在Excel中的行
		/// </summary>
		public int Row
		{
			get { return this._row; }
		}
		/// <summary>
		/// 行对应的OID
		/// </summary>
		public string RowPk
		{
			get { return this._rowPk; }
		}
		/// <summary>
		/// 操作类型
		/// </summary>
		public OperationType OperationType
		{
			get { return this._operationType; }
		}
		#endregion
	}
}
