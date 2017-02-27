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
			catch (Exception exp)
			{
				return null;
			}
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
			catch (Exception exp)
			{
				return false;
			}
		}

		/// <summary>
		/// 是否为合并单元格（并取得合并的行、列数）
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
			catch (Exception exp)
			{
				return false;
			}
		}

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
						if (ary.GetValue(i, j) != null && !string.IsNullOrEmpty(ary.GetValue(i, j).ToString()))
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
			catch (Exception exp)
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
			catch (Exception exp)
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
			catch (Exception exp)
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
		public string GetBelongDtlName(Excel.Range range) //x尚未测试
		{
			var strSheet = range.Worksheet.Name;
			var col = range.Column;
			var row = range.Row;
			for (int i = 1; i <= _app.Names.Count; i++)
			{
				var location = _app.Names.Item(i).RefersToLocal; //=Sheet1!$B$2:$C$3
				var rangeTemp = _app.Names.Item(i).RefersToRange;
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
