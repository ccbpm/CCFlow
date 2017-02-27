using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace CCFormExcel2010
{
	/// <summary>
	/// 
	/// 注意：
	/// Connection中仅记录当前Excel区域中存在的『原始数据行』的关联信息，不记录new/delete的情况，new/delete在Data["RowInExcel"]中记录
	/// </summary>
	public class SubTable
	{
		#region 成员

		private string _name;
		private Excel.Range _range; //子表区域
		private DataTable _originData; //原始数据
		private DataTable _newData; //新数据
		private Hashtable _columns; //表头信息：col:KeyOfEn
		private Hashtable _rowsConnection; //行绑定关系：rowidInExcel:rowOID
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
		/// Excel中数据行数（实时）//TODO: 不准确，因为有可能出现子表区域有10行数据，但实际有效数据只有5行的情况，此时不进行“插入行”操作，直接在空行中填入数据，也是新建行，但是不会触发this.InsertRow()，也就不会修改RowsConnection
		/// </summary>
		public int RowsCount
		{
			get { return this._rowsConnection.Count; }
		}

		/// <summary>
		/// 字段信息（Range.Column:KeyOfEn）
		/// </summary>
		public Hashtable Columns
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
		}

		#endregion

		#region 方法

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="range"></param>
		/// <param name="data"></param>
		public SubTable(Excel.Range range, DataTable data, Hashtable columns)
		{
			this._range = range;
			this._originData = data.Copy();
			this._newData = data;
			this._name = data.TableName;
			this._columns = columns;
			this._rowsConnection = new Hashtable();
			this._operations = new Stack();
			if (!_newData.Columns.Contains("RowInExcel"))
			{
				_newData.Columns.Add("RowInExcel");
			}
		}

		/// <summary>
		/// 设置行关联（同时保存到this.Data和this.Connection中）（暂定：只允许在填充数据时执行）
		/// 注意：插入行时不要执行该方法！
		/// </summary>
		/// <param name="rowInExcel"></param>
		/// <param name="rowInDatatable"></param>
		public void SetConnection(int rowInExcel, int rowInDatatable)
		{
			if (_rowsConnection.Contains(rowInExcel))
			{
				_newData.Rows[rowInDatatable]["RowInExcel"] = rowInExcel;
				_rowsConnection[rowInExcel] = _newData.Rows[rowInDatatable]["OID"];
			}
			else
			{
				_newData.Rows[rowInDatatable]["RowInExcel"] = rowInExcel;
				_rowsConnection.Add(rowInExcel, _newData.Rows[rowInDatatable]["OID"]);
			}
		}

		public string GetOidByRowid(int rowInExcel)
		{
			if (_rowsConnection.Contains(rowInExcel))
				return _rowsConnection[rowInExcel].ToString();
			else
				return null;
		}

		/// <summary>
		/// 新建行操作
		/// </summary>
		/// <param name="rowInExcel"></param>
		public void InsertRow(int rowInExcel, Excel.Range range)
		{
			//记录操作
			_operations.Push(new RowOperation(rowInExcel, (string)_rowsConnection[rowInExcel], OperationType.New));

			//更改行关联
			var point = _range.Row + (int)_columns["TableHeadHeight"] + _rowsConnection.Count; //指向最后一行（新的）
			_rowsConnection.Add(point, null);
			while (point > rowInExcel)
			{
				_rowsConnection[point] = _rowsConnection[point - 1];
				point--;
			} //循环结束时:point==rowInExcel
			//x_rowsConnection[rowInExcel] = "new"; //不记录新插入行的关联信息，便于在保存时区分已有行和新建行
			_rowsConnection.Remove(point);

			//更新子表区域
			_range = range;
		}

		/// <summary>
		/// 撤销新增行操作
		/// </summary>
		/// <param name="rowInExcel"></param>
		/// <param name="range"></param>
		private void UndoInsertRow(int rowInExcel, Excel.Range range)
		{
			//更改行关联（所有）
			var point = rowInExcel; //指向当前行（被撤销的『新增行』）
			while (_rowsConnection.Contains(point + 1))
			{
				_rowsConnection[point] = _rowsConnection[point + 1];
				point++;
			} //循环结束时:point==存在的最大行号
			_rowsConnection.Remove(point);

			//更新子表区域
			_range = range;
		}

		/// <summary>
		/// 删除行操作
		/// </summary>
		/// <param name="rowInExcel"></param>
		public void DeleteRow(int rowInExcel, Excel.Range range)
		{
			//记录操作
			_operations.Push(new RowOperation(rowInExcel, (string)_rowsConnection[rowInExcel], OperationType.Delete));

			//更改newDataTalble中的Status: _newData.Select("OID={0}")[0][RowInExcel or RowStatus]
			_newData.Select(string.Format("OID='{0}'", (string)_rowsConnection[rowInExcel]))[0]["RowInExcel"] = RowStatus.Deleted.ToString();

			//更改行关联
			var point = rowInExcel; //指向被删除的行
			while (_rowsConnection.Contains(point + 1))
			{
				_rowsConnection[point] = _rowsConnection[point + 1];
				point++;
			} //循环结束时:point==存在的最大行号
			_rowsConnection.Remove(point);

			//更新子表区域
			_range = range;
		}

		/// <summary>
		/// 撤销新增行操作
		/// </summary>
		/// <param name="rowInExcel"></param>
		/// <param name="oid"></param>
		/// <param name="range"></param>
		private void UndoDeleteRow(int rowInExcel, string oid, Excel.Range range)
		{
			//更改newDataTalble中的Status: _newData.Select("OID={0}")[0][RowInExcel or RowStatus]
			_newData.Select(string.Format("OID='{0}'", (string)_rowsConnection[rowInExcel]))[0]["RowInExcel"] = RowStatus.Origin.ToString();

			//更改行关联
			var point = _range.Row + (int)_columns["TableHeadHeight"] + _rowsConnection.Count; //指向最后一行
			_rowsConnection.Add(point, null);
			while (point > rowInExcel)
			{
				_rowsConnection[point] = _rowsConnection[point - 1];
				point--;
			} //循环结束时:point==rowInExcel
			_rowsConnection[rowInExcel] = oid;

			//更新子表区域
			_range = range;
		}

		/// <summary>
		/// 撤销操作
		/// </summary>
		/// <param name="range"></param>
		public void Undo(Excel.Range range)
		{
			if (_operations.Count == 0) return;
			RowOperation oper = (RowOperation)_operations.Pop();
			if (oper.OperationType == OperationType.New)
			{
				this.UndoInsertRow(oper.Row + 1, range);
			}
			else if (oper.OperationType == OperationType.Delete)
			{
				this.UndoDeleteRow(oper.Row, oper.OID, range);
			}
		}

		/// <summary>
		/// 获取新数据（在AfterSave时使用）
		/// </summary>
		/// <returns></returns>
		public DataTable GetNewData()
		{
			//删除已删除的行
			foreach (DataRow dr in _newData.Rows)
			{
				if (dr["RowInExcel"] == RowStatus.Deleted.ToString())
					_newData.Rows.Remove(dr);
				//else if (dr["RowInExcel"] == RowStatus.New.ToString()) //此时还没有dr["RowInExcel"]="new"的行
				//	dr["OID"] = "0";
			}

			/* 下面这部分操作在外部执行
			//新增行
			foreach (DictionaryEntry conn in _rowsConnection)
			{
				if (conn.Value == "new")
				{
					//!注意：要把实际的rowInExcel存到_newData.Rows[x]["RowInExcel"]中
				}
			}*/

			return _newData;
		}

		#endregion

		//TODO: 删除多行的情况
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
		private string _oid;
		private OperationType _operationType;

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="row">行(RowInExcel)</param>
		/// <param name="oid">OID</param>
		/// <param name="operationType">操作类型</param>
		public RowOperation(int row, string oid, OperationType operationType)
		{
			this._row = row;
			this._oid = oid;
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
		public string OID
		{
			get { return this._oid; }
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
