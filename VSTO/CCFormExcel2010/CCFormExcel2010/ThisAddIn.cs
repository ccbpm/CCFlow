using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using BP.Excel;
using System.Management;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;


namespace CCFormExcel2010
{
	public partial class ThisAddIn
	{
		private readonly string regexRangeSingle = "^\\=\\S+\\!\\$\\D+\\$\\d+$"; //=Sheet1!$B$2 //合并的单元格：仅通过Selection获取区域时不能使用此正则验证
		private readonly string regexRangeArea = "^\\=\\S+\\!\\$\\D+\\$\\d+\\:\\$\\D+\\$\\d+$"; //=Sheet1!$B$2:$C$3
		private DataSet _originData; //原始数据
		private Hashtable _htDtlsColumns = new Hashtable(); //excel中的子表字段信息

		// 定义一个任务窗体 
		//internal Microsoft.Office.Tools.CustomTaskPane helpTaskPane;
		/// <summary>
		/// 测试的参数变量.
		/// </summary>
		public void InitTester()
		{
			Glo.UserNo = "anjian";
			Glo.SID = "d-d-d-d-sdsds";
			Glo.WorkID = 2201;
			Glo.FK_Flow = "002";
			Glo.FK_Node = 201;
			Glo.FrmID = "CY_6501"; //采样表单ID.
			Glo.WSUrl = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
		}

		public void InitTesterDemo()
		{
			Glo.UserNo = "zhoupeng";
			Glo.SID = "d-d-d-d-sdsds";
			Glo.WorkID = 10001;
			Glo.FK_Flow = "001";
			Glo.FK_Node = 101;
			Glo.FrmID = "ND101";
			Glo.WSUrl = "http://localhost/WF/CCForm/CCFormAPI.asmx";
		}

		private void ThisAddIn_Startup(object sender, System.EventArgs e)
		{

			#region 获得外部参数, 这是通过外部传递过来的参数.
			try
			{
				Dictionary<string, string> args = Glo.GetArguments();
				Glo.LoadSuccessful = args["fromccflow"] == "true";
				//	Globals.Ribbons.RibbonCCFlow.btnSaveFrm.Enabled = true;
				Glo.UserNo = args["UserNo"];
				Glo.SID = args["SID"];
				Glo.FK_Flow = args["FK_Flow"];
				Glo.FK_Node = int.Parse(args["FK_Node"]);
				Glo.FrmID = args["FrmID"];
				Glo.WorkID = int.Parse(args["WorkID"]);
				Glo.WSUrl = args["WSUrl"];
			}
			catch (Exception ex)
			{
				//MessageBox.Show("加载期间出现错误", ex.Message);
			}
			#endregion 获得外部参数, 这是通过外部传递过来的参数.

			// 测试当前数据.
			this.InitTester();

			#region 校验用户安全与下载文件.
			try
			{
				CCFormExcel2010.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
				byte[] bytes = null;
				var isExists = client.GenerExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID, ref bytes);

				// 把这个byt 保存到 c:\temp.xlsx 里面.
				string tempFile = "C:\\CCFlow\\temp.xlsx";
				if (System.IO.File.Exists(tempFile) == true)
					System.IO.File.Delete(tempFile);
				//写入文件.
				BP.Excel.Glo.WriteFile(tempFile, bytes);

				//打开文件
				Globals.ThisAddIn.Application.Workbooks.Open("C:\\CCFlow\\temp.xlsx");

				//如果打开的是模板，则还需填充数据
				if (isExists == false)
				{
					//获得该表单的，物理数据.
					_originData = client.GenerDBForVSTOExcelFrmModel(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID);

					#region 加载外键枚举数据

					//DataTable dtMapAttr = ds.Tables["Sys_MapAttr"];
					//foreach (DataRow dr in dtMapAttr.Rows)
					//{
					//    int lgType=int.Parse(dr["LGType"].ToString());
					//    if (lgType == 0)
					//        continue; //普通类型的字段。
					//    string uiBindKey = dr["UIBindKey"].ToString();
					//    if (string.IsNullOrEmpty(uiBindKey) == true)
					//        continue; // 没有外键枚举.
					//    DataTable dt = ds.Tables[uiBindKey];
					//}

					SetMetaData(_originData);

					#endregion 加载外键枚举数据

					#region 给主从表赋值.
					//给主表赋值.
					DataTable dtMain = _originData.Tables["MainTable"];
					SetMainData(dtMain);

					//给从表赋值.
					foreach (DataTable dt in _originData.Tables)
					{
						if (dt.TableName == "MainTable")
							continue;
						SetDtlData(dt);
					}
					#endregion 给主从表赋值.
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message + "\r@\r" + exp.StackTrace);
				//TODO: ThisAddIn需要执行什么操作？
			}
			#endregion 校验用户安全与下载文件.

			//Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.ActiveSheet);
			//Excel.Range firstRow = activeWorksheet.get_Range("A1");
			//firstRow.EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
			//Excel.Range newFirstRow = activeWorksheet.get_Range("A1");
			//newFirstRow.Value2 = "This text was added by using code";
			//newFirstRow.Interior.Color = 100;
			//this.Application.WorkbookBeforeSave += new Excel.AppEvents_WorkbookBeforeSaveEventHandler(Application_WorkbookBeforeSave);

			//保存到.
			//activeWorksheet.SaveAs("c:\\" + BP.Excel.Glo.FK_Flow + ".xls");

			// 把自定义窗体添加到CustomTaskPanes集合中 
			//// ExcelHelp 是一个自定义控件类 
			//helpTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(new TaskPanel(), "采样任务列表");
			//// 使任务窗体可见 
			//helpTaskPane.Visible = true;

			// 通过DockPosition属性来控制任务窗体的停靠位置， 
			// 设置为 MsoCTPDockPosition.msoCTPDockPositionRight这个代表停靠到右边，这个值也是默认值 
			//helpTaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionRight; 
			// Application.ThisWorkbook.OpenLinks(
			//  Application.ThisWorkbook.Open(
			//Workbooks.Open Filename
			//  Utility
			// activeWorksheet.r
		}

		/// <summary>
		/// 单元格内容变动监听事件
		/// </summary>
		/// <param name="sh"></param>
		/// <param name="range"></param>
		void Application_SheetChange(object sh, Excel.Range range)
		{
			//if (!Glo.LoadSuccessful) return;

			//Excel.Worksheet sheet = sh as Excel.Worksheet;
			//MessageBox.Show(sheet.Name + "," + range.Value);
		}

		/// <summary>
		/// 执行保存
		/// </summary>
		/// <param name="Wb"></param>
		/// <param name="Success"></param>
		void Application_WorkbookAfterSave(Excel.Workbook Wb, bool Success)
		{
			DataSet ds = new DataSet();
			var a = GetData(ref ds);

			//若插件没有加载成功：直接
			if (!Glo.LoadSuccessful) return;
			//执行保存.
			//CCFlowExcel2007.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
			//string json = "";
			//string mainTableAtParas = ""; //主表字段
			//DataSet ds = new DataSet();	//子表
			//byte[] bytes = null;
			//client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID, mainTableAtParas, ds, bytes);
			//MessageBox.Show("After save");
		}

		private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
		{

		}

		/// <summary>
		/// 填充枚举、外键元数据
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		public bool SetMetaData(DataSet ds)
		{
			//x Worksheet ws = (Worksheet)Application.Worksheets.get_Item("MetaData"); 
			//x var ws = Application.Sheets.Item["MetaData"];

			var ws = Application.Worksheets["MetaData"]; //只遍历元数据sheet内的名称//未发现Contains方法
			if (ws == null)
			{
				MessageBox.Show("不存在Sheet页：MetaData");
				return false;
			}

			//遍历命名区域.
			foreach (Excel.Name name in Application.Names)
			{
				var strName = name.NameLocal;
				var location = name.RefersToLocal;
				if (location.IndexOf("MetaData") > -1 && ds.Tables.Contains(strName)) //需确保name使用的是UIBindKey
				{
					var range = name.RefersToRange;
					var col = ConvertInt2Letter(range.Column);
					//填充数据
					for (var r = 1; r <= ds.Tables[strName].Rows.Count; r++)
					{
						range.Worksheet.get_Range(col + r, missing).Value2 = ds.Tables[strName].Rows[r - 1]["Name"].ToString();
					}
					//设置命名
					Application.Names.Add(strName, "=MetaData!$" + col + "$1:$" + col + "$" + ds.Tables[strName].Rows.Count);
				}
			}
			return true;
		}

		/// <summary>
		/// 填充表单数据
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public bool SetMainData(DataTable dt) //尚未测试
		{
			//var r = false;
			foreach (DataColumn dc in dt.Columns)
			{
				if (dt.Rows[0][dc.ColumnName] != null && Application.Name.Contains(dc.ColumnName))//字段值不为空 且 Excel文档中存在此字段的命名
				{
					var range = Application.Names.Item(dc.ColumnName).RefersToRange;
					var location = Application.Names.Item(dc.ColumnName).RefersToLocal; //=Sheet1!$B$2
					if (Regex.IsMatch(location, regexRangeSingle)) //是单个单元格
					{
						var strUiBindKey = string.Empty;
						if (IsEnumOrFk("Sys_MapAttr", dc.ColumnName, ref strUiBindKey))
							range.Value2 = Glo.GetNameByNo(_originData, strUiBindKey, dt.Rows[0][dc.ColumnName].ToString());
						else
							range.Value2 = dt.Rows[0][dc.ColumnName].ToString(); //TODO: 枚举/外键key转换为value
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 填充子表数据
		/// </summary>
		/// <param name="dt">确保TableName为子表表名</param>
		/// <returns>是否填充成功</returns>
		public bool SetDtlData(DataTable dt) //尚未测试
		{
			if (!Application.Name.Contains(dt.TableName)) //excel中不存在该子表时
				return false;
			var range = Application.Names.Item(dt.TableName).RefersToRange;
			var location = Application.Names.Item(dt.TableName).RefersToLocal;
			if (!Regex.IsMatch(location, regexRangeArea)) //excel中子表所在区域不是『区域』（是单个单元格）
				return false;

			//获取字段信息
			var htColumns = GetAreaColumns(range); //只在加载时运行一次故不需要判断是否已经获取了该子表区域的字段信息
			_htDtlsColumns[dt.TableName] = htColumns;

			//若子表区域行数不够，则插入行
			var intTableHeadHeight = Convert.ToInt32(htColumns["TableHeadHeight"]);
			if (dt.Rows.Count > range.Rows.Count - intTableHeadHeight) //表单实际数据中『子表行数』多于excel文档子表『区域行数』时
			{
				//插入行（在区域最后一行上方）
				int addRowsCount = dt.Rows.Count - (range.Rows.Count - intTableHeadHeight);
				var rangeLastRow = range.Worksheet.get_Range(ConvertInt2Letter(range.Column) + (range.Row + range.Rows.Count - 1), missing);
				while (addRowsCount > 0)
				{
					rangeLastRow.Insert();
					addRowsCount--;
				}
			}

			//填充数据
			foreach (DictionaryEntry col in htColumns)
			{
				if (dt.Columns.Contains(col.Value.ToString())) //DataTable中含有该字段
				{
					var colL = ConvertInt2Letter(Convert.ToInt32(col.Key));
					for (var r = 0; r <= dt.Rows.Count; r++)
					{
						var rangeCell = range.Worksheet.get_Range(colL + (range.Row + intTableHeadHeight - 1 + r), missing);
						rangeCell.Value2 = dt.Rows[r][col.Value.ToString()].ToString();
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 获取子表区域中的表头（字段）信息
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public Hashtable GetAreaColumns(Excel.Range range)
		{
			//获取字段信息，及数据开始的行号
			Hashtable htColumns = new Hashtable();
			int intTableHeadHeight = 1; //表头占子表区域的高度（行数）
			for (var c = range.Column; c <= range.Column + range.Columns.Count - 1; )
			{
				string name = string.Empty;
				var currentThMergeColumnsCount = 1; //当前表头（单元格）所占列数
				for (var r = range.Row; r < range.Row + range.Rows.Count - 1; )
				{
					var rangeTableHead = range.Worksheet.get_Range(ConvertInt2Letter(c) + range.Row, missing);

					//若该单元格有命名
					if (rangeTableHead.Name.Name != null)
					{
						name = rangeTableHead.Name.Name;
						var currentThMergeRowsCount = 1;
						if (rangeTableHead.MergeCells)
						{
							currentThMergeColumnsCount = rangeTableHead.MergeArea.Columns.Count; //用于计算下一个要循环的列
							currentThMergeRowsCount = rangeTableHead.MergeArea.Rows.Count; //用于计算当前表头所占行数
						}
						var currentThHeight = rangeTableHead.Row + currentThMergeRowsCount - range.Row; //当前表头占子表区域的高度（行数）
						if (intTableHeadHeight < currentThHeight)
							intTableHeadHeight = currentThHeight;
						break; //发现表头即停止循环，不再往下寻找表头
					}

					//!下一个要循环的行
					//r += (rangeTableHead.MergeCells ? rangeTableHead.MergeArea.Rows.Count - 1 : 1);
					if (rangeTableHead.MergeCells)
						r += rangeTableHead.MergeArea.Rows.Count;
					else
						r++;
				}

				//若该列有绑定列
				if (!string.IsNullOrEmpty(name))
					htColumns.Add(c, name);

				//!下一个要循环的列
				c += currentThMergeColumnsCount;
			}
			htColumns["TableHeadHeight"] = intTableHeadHeight; //约定：使用Key=TableHeadHeight存放该子表区域中表头所占行数
			return htColumns;
		}

		///// <summary>
		///// 获取excel中所有的表单字段数据（json）
		///// </summary>
		///// <returns></returns>
		//public string GetJsonData()
		//{
		//    return null;
		//}

		/// <summary>
		/// 获取主、从表数据
		/// </summary>
		/// <param name="ds">ref子表数据</param>
		/// <returns>(AtParas)主表数据</returns>
		public string GetData(ref DataSet ds) //x尚未测试
		{
			Hashtable htParas = new Hashtable();
			for (int i = 1; i <= Application.Names.Count; i++)
			{
				var name = Application.Names.Item(i).NameLocal; //name1
				var location = Application.Names.Item(i).RefersToLocal; //=Sheet1!$B$2
				var range = Application.Names.Item(i).RefersToRange;
				//var sheet = range.Worksheet.Name; //Sheet1
				//var col = Application.Names.Item(i).RefersToRange.Column; //2
				//var row = range.Row; //2
				//var val = range.Value2; //null/abc
				//range.Address //$B$2
				//range.AddressLocal //$B$2

				string strRgxSingleCell = "^\\=\\S+\\!\\$\\D+\\$\\d+$";
				if (range.Value2 != null && Regex.IsMatch(location, strRgxSingleCell)) //是单个单元格
				{
					var strBelongDtl = GetBelongDtlName(range.Worksheet.Name, range.Column, range.Row);
					if (strBelongDtl == null) //不属于某个子表
					{
						if (range.Value2 != null)
						{
							var strUiBindKey = string.Empty;
							if (IsValidList(range) && IsEnumOrFk("Sys_MapAttr", name, ref strUiBindKey))
								htParas.Add(name, Glo.GetNoByName(_originData, strUiBindKey, range.Value2)); //TODO: 是否型的字段
							else
								htParas.Add(name, range.Value2);
						}
					}
					else //属于子表
					{
						if (!ds.Tables.Contains(strBelongDtl))
						{
							var dt = GetDtlData(strBelongDtl);
							if (dt != null)
								ds.Tables.Add();
						}
					}
				}
			}
			//把hashtable转换为@a=1形式的字符串
			string r = string.Empty;
			foreach (DictionaryEntry para in htParas)
			{
				r += "@" + para.Key + "=" + para.Value;
			}
			return r;
		}

		/// <summary>
		/// 获取从表数据
		/// </summary>
		/// <param name="strTableName"></param>
		/// <returns></returns>
		public DataTable GetDtlData(string strTableName) //x尚未测试
		{
			var range = Application.Names.Item(strTableName).RefersToRange; //需确保Names[strTableName]存在
			if (!_originData.Tables.Contains(strTableName)) //理论上不存在这种情况，_originData中包含所有的子表数据
				return null;
			DataTable dt = _originData.Tables[strTableName].Clone(); //是否会复制表名？
			dt.TableName = strTableName;


			//获取字段信息
			var htColumns = new Hashtable();
			if (_htDtlsColumns.Contains(strTableName))
			{
				htColumns = (Hashtable)_htDtlsColumns[strTableName];
			}
			else
			{
				htColumns = GetAreaColumns(range);
				_htDtlsColumns[strTableName] = htColumns;
			}

			//填充数据
			DataRow dr = dt.NewRow();
			var intTableHeadHeight = Convert.ToInt32(htColumns["TableHeadHeight"]);
			for (var r = range.Row + intTableHeadHeight; r < range.Row + range.Rows.Count; r++)
			{
				foreach (DictionaryEntry col in htColumns)
				{
					var rangeCell = range.Worksheet.get_Range(ConvertInt2Letter((int)col.Key) + r, missing);
					if (rangeCell.Value2 != null)
					{
						var strUiBindKey = string.Empty;
						if (IsValidList(rangeCell) && IsEnumOrFk(strTableName, (string)col.Value, ref strUiBindKey))
							dr[(string)col.Value] = Glo.GetNoByName(_originData, strUiBindKey, rangeCell.Value2); //TODO: 是否型的字段
						else
							dr[(string)col.Value] = rangeCell.Value2;
					}
				}
				dt.Rows.Add(dr);
			}
			return dt;
		}

		#region 表单数据相关方法

		/// <summary>
		/// 判断某个字段是否是枚举/外键类型
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="strKeyOfEn"></param>
		/// <param name="UiBindKey"></param>
		/// <returns></returns>
		public bool IsEnumOrFk(string tableName, string strKeyOfEn, ref string UiBindKey)//TODO: 外键类型、有范围限制的字段，不能直接使用UiBindKey
		{
			if (_originData.Tables.Contains(tableName))
			{
				foreach (DataRow dr in _originData.Tables[tableName].Rows)
				{
					if (dr["KeyOfEn"].ToString() == strKeyOfEn && dr["LGType"].ToString() != "0")
					{
						UiBindKey = dr["UIBindKey"].ToString();
						return !string.IsNullOrEmpty(UiBindKey);
					}
				}
			}
			return false;
		}

		#endregion

		#region VSTO相关方法

		/// <summary>
		/// 获取所属子表名称
		/// </summary>
		/// <param name="strSheet">Range.WorkSheet.Name</param>
		/// <param name="col">Range.Column</param>
		/// <param name="row">Range.Row</param>
		/// <returns>子表名/null</returns>
		public string GetBelongDtlName(string strSheet, int col, int row) //x尚未测试
		{
			for (int i = 1; i <= Application.Names.Count; i++)
			{
				var location = Application.Names.Item(i).RefersToLocal; //=Sheet1!$B$2:$C$3
				var range = Application.Names.Item(i).RefersToRange;
				if (range.Count > 1 && Regex.IsMatch(location, regexRangeArea)) //是区域
				{
					var name = Application.Names.Item(i).NameLocal;
					if (strSheet == range.Worksheet.Name
						&& col >= range.Column && col <= range.Column + range.Columns.Count - 1
						&& row >= range.Row && col <= range.Row + range.Rows.Count - 1)
					{
						return name;
					}
				}
			}
			return null;
		}

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
			MessageBox.Show(range.Validation.Type.ToString());
			return (range.Validation.Type.ToString() == "3");
		}

		#endregion

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

		/// <summary>
		/// 获取excel文档的二进制数据
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes()
		{
			return null;
		}

		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(ThisAddIn_Startup);
			this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);

			//单元格内容变动监听事件
			this.Application.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(Application_SheetChange);
			//保存事件
			this.Application.WorkbookAfterSave += new Excel.AppEvents_WorkbookAfterSaveEventHandler(Application_WorkbookAfterSave);
		}

		#endregion
	}
}
