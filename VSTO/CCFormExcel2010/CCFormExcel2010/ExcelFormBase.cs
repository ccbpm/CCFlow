using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace CCFormExcel2010
{
	class ExcelFormBase
	{
		#region 成员

		public readonly string regexRangeSingle = "^\\=\\S+\\!\\$\\D+\\$\\d+$"; //=Sheet1!$B$2 //合并的单元格：仅通过Selection获取区域时不能使用此正则验证
		public readonly string regexRangeArea = "^\\=\\S+\\!\\$\\D+\\$\\d+\\:\\$\\D+\\$\\d+$"; //=Sheet1!$B$2:$C$3
		public readonly string regexAddressRows = "^\\$\\d+\\:\\$\\d+$"; //$2:$2
		public readonly string regexAddressColumns = "^\\$\\D+\\:\\$\\D+$"; //$C:$C
		public readonly string regexAddressCell = "^\\$\\D+\\$\\d+$"; //$C$4
		private Excel.Application _app;

		#endregion

		public ExcelFormBase(Excel.Application app)
		{
			this._app = app;
		}

		/// <summary>
		/// 获取Worksheet
		/// </summary>
		/// <param name="name">Sheet页名称</param>
		/// <returns>Worksheet/null</returns>
		public Excel.Worksheet GetWorksheet(string name)
		{
			try
			{
				//x Worksheet ws = (Worksheet)Application.Worksheets.get_Item(name); 
				//x var ws = Application.Sheets.Item[name];

				return _app.Worksheets[name]; //未发现Contains方法
			}
			catch
			{
				return null;
			}
		}

		public Excel.Worksheet GetFirstWorkSheet()
		{
			return _app.Worksheets[1];
		}

		#region Cell相关
		/// <summary>
		/// 将整数转换为英文字母（e.g. 1->A,2->B，27->AA）
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public string ConvertInt2Letter(int i)
		{
			if (i <= 26)
			{
				return Convert.ToChar(64 + i).ToString();
			}
			else
			{
				return Convert.ToChar(64 + (i / 26)).ToString() + Convert.ToChar((64 + (i % 26)));
			}
		}

		/// <summary>
		/// 判断单元格是否是单个单元格（包括合并后的单元格）
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool IsSingle(Excel.Range range)
		{
			if (range.Count == 1)
				return true;
			else
				return IsMerge(range);
		}

		/// <summary>
		/// 是否为合并单元格：仅当range只包含一个合并单元格时返回true（并取得合并的行、列数）
		/// </summary>
		/// <param name="range"></param>
		/// <param name="c"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		public bool IsMerge(Excel.Range range, ref int c, ref int r)
		{
			if (range.MergeCells)
			{
				if (range.MergeArea != null)
				{
					c = range.MergeArea.Columns.Count;
					r = range.MergeArea.Rows.Count;
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// 是否为合并单元格：仅当range只包含一个合并单元格时返回true
		/// 1.可以判断【合并后单元格】中的任意一个单元格是否为合并单元格（e.g. 合并单元格为A2:E5，传入range=C4，此时返回true）。
		/// 2.若Range中『第一个单元格为合并』且『还有其他单元格』，此时仍然会返回false（e.g. range=[merge(A1,A2)+A3]）。
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool IsMerge(Excel.Range range)
		{
			try
			{
				if (range.MergeCells)
				{
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 判断两个区域是否有交集
		/// </summary>
		/// <param name="range1"></param>
		/// <param name="range2"></param>
		/// <returns></returns>
		public bool IsIntersect(Excel.Range range1, Excel.Range range2)
		{
			var isRowIntersect = false;
			for (var r1 = range1.Row; r1 < range1.Row + range1.Rows.Count; r1++)
			{
				for (var r2 = range2.Row; r2 < range2.Row + range2.Rows.Count; r2++)
				{
					if (r1 == r2)
					{
						isRowIntersect = true;
						break;
					}
				}
				if (isRowIntersect) break;
			}
			if (isRowIntersect) //若行有交叉
			{
				var isColumnIntersect = false;
				for (var r1 = range1.Column; r1 < range1.Column + range1.Columns.Count; r1++)
				{
					for (var r2 = range2.Column; r2 < range2.Column + range2.Columns.Count; r2++)
					{
						if (r1 == r2)
						{
							isColumnIntersect = true;
							break;
						}
					}
					if (isColumnIntersect) break;
				}
				return (isRowIntersect && isColumnIntersect); //行、列均有交叉时才有交集
				//TODO: 整行、整列的情况？
			}
			else //若行无交叉
			{
				return false;
			}
		}

		/// <summary>
		/// 判断单元格是否有“数据有效性-序列”的设置
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool IsValidList(Excel.Range range)
		{
			try
			{
				//MessageBox.Show(range.Validation.Type.ToString()); //3
				return range.Validation.Type == Excel.XlDVType.xlValidateList.GetHashCode();
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 判断区域是否为空（所有单元格均没有值）
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public bool IsEmpty(Excel.Range range)
		{
			if (range.Count == 1)
			{
				return string.IsNullOrEmpty(range.Value2);
			}
			else
			{
				object[,] ary = range.Value2;
				for (var i = 1; i <= ary.GetLength(0); i++)
				{
					for (var j = 1; j <= ary.GetLength(1); j++)
					{
						if (ary.GetValue(i, j) != null && !string.IsNullOrEmpty(ary.GetValue(i, j).ToString().Trim()))
							return false;
					}
				}
				return true;
			}
		}

		#endregion

		#region Name相关

		/// <summary>
		/// 获取命名对象（通过名称）
		/// </summary>
		/// <param name="strName"></param>
		/// <returns></returns>
		public Excel.Name GetNameByName(string strName)
		{
			try
			{
				return _app.Names.Item(strName);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 获取区域命名（用于形如“=Sheet1!$A$1:$A$1”这种由单元格“冒充”区域设置的命名的名称）
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public string GetNameFakeArea(Excel.Range range)
		{
			try
			{
				foreach (Excel.Name name in _app.Names)
				{
					if (name.RefersToLocal == "=" + range.Worksheet.Name + "!" + range.Address + ":" + range.Address)
						return name.NameLocal;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 判断是否存在命名对象（通过名称）
		/// </summary>
		/// <param name="strName"></param>
		/// <returns></returns>
		public bool IsExistsName(string strName)
		{
			try
			{
				var name = _app.Names.Item(strName);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 判断是否存在命名对象，并返回该Name对象（通过名称）
		/// </summary>
		/// <param name="strName"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		public bool IsExistsName(string strName, out Excel.Name Name)
		{
			try
			{
				Name = _app.Names.Item(strName);
				return true;
			}
			catch
			{
				Name = null;
				return false;
			}
		}

		/// <summary>
		/// 获取所属子表名称
		/// </summary>
		/// <param name="strSheet">Range.WorkSheet.Name</param>
		/// <param name="col">Range.Column</param>
		/// <param name="row">Range.Row</param>
		/// <returns>子表名/null</returns>
		public string GetBelongDtlName(Excel.Range range)
		{
			var strSheet = range.Worksheet.Name;
			var col = range.Column;
			var row = range.Row;
			for (int i = 1; i <= _app.Names.Count; i++)
			{
				var location = _app.Names.Item(i).RefersToLocal; //=Sheet1!$B$2:$C$3
				if (location == "=#NAME?") //若单元格配置了公式（函数），则有可能被识别为NAME
					continue;
				var rangeTemp = _app.Names.Item(i).RefersToRange;
				//Excel.Range rangeTemp;
				//try
				//{
				//	rangeTemp = _app.Names.Item(i).RefersToRange;
				//}
				//catch
				//{
				//	return null;
				//}

				if (rangeTemp.Count > 1 && Regex.IsMatch(location, regexRangeArea)) //是区域//其实若匹配regexRangeArea则注定Count > 1
				{
					//排除合并单元格的情况（可能是主/子表字段 或 子表表头）
					if (IsMerge(rangeTemp))
						continue;
					var name = _app.Names.Item(i).NameLocal;
					if (strSheet == rangeTemp.Worksheet.Name
						&& col >= rangeTemp.Column && col <= rangeTemp.Column + rangeTemp.Columns.Count - 1
						&& row >= rangeTemp.Row && col <= rangeTemp.Row + rangeTemp.Rows.Count - 1)
					{
						return name;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 获取Sheet页中Name的个数
		/// </summary>
		/// <param name="strSheetName"></param>
		/// <returns></returns>
		public int GetNamesCountInSheet(string strSheetName)
		{
			var count = 0;
			var location = string.Empty;
			foreach (Excel.Name name in _app.Names)
			{
				location = name.RefersToLocal.ToString();
				if (location == "=#NAME?") //若单元格配置了公式（函数），则有可能被识别为NAME
					continue;
				if (location.IndexOf(strSheetName) > -1)
				{
					count += 1;
				}
			}
			return count;
		}

		#endregion
	}
}
