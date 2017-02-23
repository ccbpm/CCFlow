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

		private CCFormExcel2010.CCForm.CCFormAPISoapClient client;
		private DataSet _originData; //原始数据
		private Hashtable _htDtlsColumns = new Hashtable(); //excel中的子表字段信息

		// 定义一个任务窗体 
		//internal Microsoft.Office.Tools.CustomTaskPane helpTaskPane;
		/// <summary>
		/// 测试的参数变量.
		/// </summary>
		public void InitTester()
		{
			Glo.LoadSuccessful = true;
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
				client = BP.Excel.Glo.GetCCFormAPISoapClient();
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

				//获得该表单的，物理数据.
				_originData = client.GenerDBForVSTOExcelFrmModel(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID);

				//如果打开的是模板，则还需填充数据//TODO: 如果打开的是DBFile二进制流，是否还执行填充操作？（表单数据是否有可能被修改？）
				if (isExists == false)
				{
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
			if (!Glo.LoadSuccessful)
				return;

			if (range.Name == null) //单元格没有绑定字段
				return;

			if (!IsValidList(range)) //单元格不是下拉列表类型 //TODO: 『手填』类型的字段是否有可能与其他字段联动？
				return;

			if (!_originData.Tables.Contains("Sys_MapExt")) //没有MapExt信息
				return;

			var strBelongDtlName = GetBelongDtlName(range);
			if (strBelongDtlName == null)
			{
				var drs = _originData.Tables["Sys_MapExt"].Select("AttrOfOper='" + "strKeyOfEn" + "'");
				if (drs.Length == 0) //MapExt信息中不含当前单元格绑定字段
					return;

				DataRow dr = drs[0];

				//填充数据（序列）
				//0.获取联动目标单元格range
				if (!Application.Name.Contains(dr["AttrsOfActive"].ToString()))
					return;
				Excel.Range rangeAim = Application.Names.Item(dr["AttrsOfActive"].ToString()).RefersToRange;
				if (!IsValidList(rangeAim)) //如果联动目标单元格不是下拉列表类型
					return;
				//1.获取序列区域命名
				var strListAreaName = rangeAim.Validation.Formula1.Replace("=", "");
				if (!Application.Name.Contains(strListAreaName)) //若Workbook中没有该区域
					return;
				//x2.清除原序列 //用新序列覆盖即可
				//3.填充新序列
				var val = Glo.GetNoByName(_originData, range.Validation.Formula1.Replace("=", ""), range.Value2); //获取当前单元格的值(key)
				//var dt = client.GetActiveList(val,dr["Doc"]); //获取联动目标单元格的值列表 //或者使用MapExt的相关方法？//TODO: 如何获取『目标单元格的值列表』?
				DataTable dt = new DataTable();
				Excel.Range rangeList = Application.Names.Item(strListAreaName).RefersToRange;
				var colL = ConvertInt2Letter(rangeList.Column);
				for (var r = 1; r <= dt.Rows.Count; r++)
				{
					Excel.Range rangeTemp = rangeList.get_Range(colL + r, missing);
					rangeTemp.Value2 = dt.Rows[r - 1]["Name"].ToString();
				}

				//清除字段值
				range.Value2 = null;
			}
			else //TODO: 字段属于某子表时。。待添加处理
			{

			}
		}

		/// <summary>
		/// 执行保存
		/// </summary>
		/// <param name="Wb"></param>
		/// <param name="Success"></param>
		void Application_WorkbookAfterSave(Excel.Workbook Wb, bool Success)
		{
			//测试用
			DataSet ds1 = new DataSet();
			var a = GetData(ref ds1);

			//若插件没有加载成功：直接
			if (!Glo.LoadSuccessful) return;

			//执行保存.
			CCFormExcel2010.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
			DataSet ds = new DataSet();
			string mainTableAtParas = GetData(ref ds); //主表字段
			byte[] bytes = Glo.ReadFile("C:\\CCFlow\\temp.xlsx");
			//Globals.ThisAddIn.Application.Workbooks.Item[0]
			client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID, mainTableAtParas, ds, bytes);
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
					//更新命名
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
						//TODO: 根据字段类型分别处理
						var strUiBindKey = string.Empty;
						if (IsEnumOrFk("Sys_MapAttr", dc.ColumnName, ref strUiBindKey)) //枚举/外键：key转换为value
							range.Value2 = Glo.GetNameByNo(_originData, strUiBindKey, dt.Rows[0][dc.ColumnName].ToString());
						else
							range.Value2 = dt.Rows[0][dc.ColumnName].ToString();
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
			if (location.IndexOf("=MetaData!") > -1) //若是元数据list区域
				return false;

			//获取字段信息
			var htColumns = GetAreaColumns(range); //只在加载时运行一次故不需要判断是否已经获取了该子表区域的字段信息
			_htDtlsColumns[dt.TableName] = htColumns;

			//若子表区域行数不够，则插入行
			var intTableHeadHeight = Convert.ToInt32(htColumns["TableHeadHeight"]);
			if (dt.Rows.Count > range.Rows.Count - intTableHeadHeight) //表单实际数据中『子表行数』多于excel文档子表『区域行数』时
			{
				//插入行（在区域最后一行上方） //TODO: 待验证 ：插入行后range的范围是否扩大了？（后面要用到range）
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
				if (col.Key == "TableHeadHeight") continue;

				//为子表的每一行的DropdownList类型的字段添加 Validation （分两种情况：作为级联的子级；有范围限制的外键/枚举） //另：验证插入行是否能自动添加上 Validation
				var strListName = string.Empty;
				FieldType ftype = GetFieldType(dt.TableName, col.Value.ToString(), ref strListName);
				if (ftype == FieldType.LimitedSubList) //有范围限制的外键字段 且 为级联字段的子级（值需要动态获取）
				{
					//为每行的该字段声明一个区域（但暂时不赋值）并设置 Validation
					var colL = ConvertInt2Letter(Convert.ToInt32(col.Key));
					for (var r = 0; r < dt.Rows.Count; r++)
					{
						var row = range.Row + intTableHeadHeight + r;
						var rangeCell = range.Worksheet.get_Range(colL + row, missing);
						var tempListName = strListName + "_R" + row;
						AddMetaList(tempListName);
						rangeCell.Validation.Add(Excel.XlDVType.xlValidateList, Formula1: "=" + tempListName);
					}
				}
				else if (ftype == FieldType.LimitedList || ftype == FieldType.List) //枚举字段/无范围限制的外键字段/有范围限制但无级联关系的外键字段/有范围限制且有级联关系且不为级联的子级的外键字段
				{
					if (_originData.Tables.Contains(strListName))
					{
						if (!Application.Name.Contains(strListName))
						{
							//若没有则新建一个
							AddMetaList(strListName);
						}
						var dtAreaData = _originData.Tables[strListName];
						Excel.Range rangeListArea = Application.Names.Item(strListName).RefersToRange;
						var colL = ConvertInt2Letter(Convert.ToInt32(rangeListArea.Column));
						for (var r = 0; r <= dtAreaData.Rows.Count; r++)
						{
							var rangeCell = rangeListArea.Worksheet.get_Range(colL + (r + 1), missing);
							rangeCell.Value2 = dtAreaData.Rows[r]["Name"].ToString();
						}
						//更名区域命名
						Application.Names.Add(strListName, "=MetaData!$" + colL + "$1:$" + colL + "$" + dtAreaData.Rows.Count);
						//设置数据有效性
						var rangeColumn = range.Worksheet.get_Range(colL + (range.Row + intTableHeadHeight) + ":" + colL + (range.Row + range.Rows.Count - 1), missing);//TODO: 能否取到range？
						rangeColumn.Validation.Add(Excel.XlDVType.xlValidateList, Formula1: "=" + strListName);
					}
				}

				//填充行数据
				if (dt.Columns.Contains(col.Value.ToString())) //DataTable中含有该字段
				{
					var colL = ConvertInt2Letter(Convert.ToInt32(col.Key));
					for (var r = 0; r < dt.Rows.Count; r++)
					{
						var rangeCell = range.Worksheet.get_Range(colL + (range.Row + intTableHeadHeight + r), missing);
						//rangeCell.Value2 = dt.Rows[r][col.Value.ToString()].ToString();//TODOne: 枚举/外键key转换为value
						var strUiBindKey = string.Empty;
						if (IsEnumOrFk("Sys_MapAttr_For_" + dt.TableName, col.Value.ToString(), ref strUiBindKey)) //枚举/外键：key转换为value
							rangeCell.Value2 = Glo.GetNameByNo(_originData, strUiBindKey, dt.Rows[r][col.Value.ToString()].ToString()); //TODO: 级联子级字段？
						else
							rangeCell.Value2 = dt.Rows[0][col.Value.ToString()].ToString();
					}
				}
			}

			return true;
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

				if (range.Value2 != null && Regex.IsMatch(location, this.regexRangeSingle)) //是单个单元格
				{
					var strBelongDtl = GetBelongDtlName(range);
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
								ds.Tables.Add(dt);
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
			DataRow dr;
			var intTableHeadHeight = Convert.ToInt32(htColumns["TableHeadHeight"]);
			for (var r = range.Row + intTableHeadHeight; r < range.Row + range.Rows.Count; r++)
			{
				dr = dt.NewRow();
				foreach (DictionaryEntry col in htColumns)
				{
					if (col.Key == "TableHeadHeight") continue;
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

		public enum FieldType
		{
			/// <summary>
			/// 不是表单字段
			/// </summary>
			Nothing,
			/// <summary>
			/// 普通字段
			/// </summary>
			Normal,
			/// <summary>
			/// 是否型字段
			/// </summary>
			TrueOrFalse,
			/// <summary>
			/// 序列（下拉）
			/// </summary>
			List,
			/// <summary>
			/// 有限制的序列（下拉）
			/// </summary>
			LimitedList,
			/// <summary>
			/// 有限制的、作为级联子级的序列（下拉）
			/// </summary>
			LimitedSubList
		}

		public FieldType GetFieldType(string tableName, string strKeyOfEn, ref string strListName)
		{
			var strMapExtTableName = tableName == "MainTable" ? "Sys_MapExt" : "Sys_MapExt_For_" + tableName;
			var strMapAttrTableName = tableName == "MainTable" ? "Sys_MapAttr" : "Sys_MapAttr_For_" + tableName;
			if (_originData.Tables.Contains(strMapExtTableName))
			{
				//作为级联的子级
				var drs = _originData.Tables[strMapExtTableName].Select(string.Format("ExtType='ActiveDDL' and AttrsOfActive='{0}'", strKeyOfEn));
				if (drs.Length > 0)
				{
					strListName = SelectDataTable(strMapAttrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "MyPk");
					return FieldType.LimitedSubList;
				}
				//作为级联的父级
				drs = _originData.Tables[strMapExtTableName].Select(string.Format("ExtType='ActiveDDL' and AttrOfOper='{0}'", strKeyOfEn));
				if (drs.Length > 0)
				{
					strListName = SelectDataTable(strMapAttrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "UIBindKey");
					return FieldType.LimitedList;
				}
				//外键（e.g. 本部门的人员）
				drs = _originData.Tables[strMapExtTableName].Select(string.Format("ExtType='AutoFullDLL' and AttrOfOper='{0}'", strKeyOfEn));
				if (drs.Length > 0)
				{
					strListName = SelectDataTable(strMapAttrTableName, string.Format("KeyOfEn='{0}'", strKeyOfEn), "UIBindKey");
					return FieldType.LimitedList;
				}
				//TODO: ExtType=TBFullCtrl?
			}
			else //该表单没有对应的MapExt信息
			{
				strListName = SelectDataTable(strMapAttrTableName, "KeyOfEn='" + strKeyOfEn + "'", "UIBindKey");
				if (string.IsNullOrEmpty(strListName))
				{
					return FieldType.Normal;
				}
				else
				{
					return FieldType.List;
				}
			}
			return FieldType.Nothing;
		}

		/// <summary>
		/// 获取DataTable中的某一行某个字段的值
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="strWhere"></param>
		/// <param name="strSelectColumn"></param>
		/// <returns></returns>
		public string SelectDataTable(string strTableName, string strWhere, string strSelectColumn)
		{
			if (!_originData.Tables.Contains(strTableName))
				return null;
			var drs = _originData.Tables[strTableName].Select(strWhere);
			if (drs.Length == 0)
				return null;
			//else if (drs.Length > 1)
			else
				return drs[0][strSelectColumn].ToString();
		}

		/// <summary>
		/// 判断某个字段是否是枚举/外键类型
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="strKeyOfEn"></param>
		/// <param name="UiBindKey"></param>
		/// <returns></returns>
		public bool IsEnumOrFk(string tableName, string strKeyOfEn, ref string UiBindKey)
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

		/// <summary>
		/// 【弃用】判断某个字段是否是『有范围限制的外键』字段
		/// </summary>
		/// <param name="tableName">表名（MainTable/子表名）</param>
		/// <param name="strKeyOfEn">字段名（KeyOfEn）</param>
		/// <param name="UiBindKey">ref关联外键区域的命名（return false时不改变值，return true时返回该字段的MyPk）</param>
		/// <returns></returns>
		public bool IsLimitedFk(string tableName, string strKeyOfEn, ref string UiBindKey)
		{
			var strMapExtTableName = tableName == "MainTable" ? "Sys_MapExt" : "Sys_MapExt_For_" + tableName;
			var strMapAttrTableName = tableName == "MainTable" ? "Sys_MapAttr" : "Sys_MapAttr_For_" + tableName;
			if (_originData.Tables.Contains(strMapExtTableName))
			{
				foreach (DataRow dr in _originData.Tables[strMapExtTableName].Rows)
				{
					if ((dr["ExtType"].ToString() == "ActiveDDL" && dr["AttrsOfActive"].ToString() == strKeyOfEn) && //作为级联的下级
						(dr["ExtType"].ToString() == "AutoFullDLL" && dr["AttrOfOper"].ToString() == strKeyOfEn) && //外键（e.g. 本部门的人员）
						!string.IsNullOrEmpty(dr["Doc"].ToString()))
					{
						if (_originData.Tables.Contains(strMapAttrTableName))
						{
							var drs = _originData.Tables[strMapAttrTableName].Select("KeyOfEn='" + strKeyOfEn + "'");
							if (drs.Length == 0)
								UiBindKey = null;
							else
								UiBindKey = drs[0]["MyPK"].ToString();
						}
						else
						{
							UiBindKey = null;
						}
						return true;
					}
				}
			}
			return false;
		}

		//动态获取『有范围限制的外键字段』的区域填充数据
		public DataTable GetAreaList()
		{
			//TODO: 获取数据成功后更新_originData
			return null;
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
		public string GetBelongDtlName(Excel.Range range) //x尚未测试
		{
			var strSheet = range.Worksheet.Name;
			var col = range.Column;
			var row = range.Row;
			for (int i = 1; i <= Application.Names.Count; i++)
			{
				var location = Application.Names.Item(i).RefersToLocal; //=Sheet1!$B$2:$C$3
				var rangeTemp = Application.Names.Item(i).RefersToRange;
				if (rangeTemp.Count > 1 && Regex.IsMatch(location, regexRangeArea)) //是区域
				{
					//排除合并单元格的情况（可能是主/子表字段 或 子表表头）
					if (IsMerge(rangeTemp))
						continue;
					var name = Application.Names.Item(i).NameLocal;
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
		/// 获取子表某中个单元格的所属列名
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public string GetDtlColName(Excel.Range range)
		{
			//获取所属子表
			var strBelongDtlName = GetBelongDtlName(range);
			if (string.IsNullOrEmpty(strBelongDtlName))
				return null;
			//获取子表列（表头）信息
			Hashtable htColumns;
			if (_htDtlsColumns.Contains(strBelongDtlName))
			{
				htColumns = (Hashtable)_htDtlsColumns[strBelongDtlName];
			}
			else
			{
				Excel.Range rangeBelongDtl = range.Worksheet.Names.Item(strBelongDtlName).RefersToRange;
				htColumns = GetAreaColumns(rangeBelongDtl);
				_htDtlsColumns.Add(strBelongDtlName, htColumns);
			}
			if (htColumns.Contains(range.Column))
				return htColumns[range.Column].ToString();
			else
				return null;
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
					var rangeTableHead = range.Worksheet.get_Range(ConvertInt2Letter(c) + r, missing);

					try
					{
						//若该单元格有命名
						if (rangeTableHead.Name != null && rangeTableHead.Name.Name != null) //rangeTableHead.Name抛出异常
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
					}
					catch (Exception exp)
					{
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

		/// <summary>
		/// 获取Sheet页中Name的个数
		/// </summary>
		/// <param name="strSheetName"></param>
		/// <returns></returns>
		public int GetNamesCountInSheet(string strSheetName)
		{
			var count = 0;
			var location = string.Empty;
			foreach (Excel.Name name in Application.Names)
			{
				location = name.RefersToLocal.ToString();
				if (location.IndexOf(strSheetName) > -1)
				{
					count += 1;
				}
			}
			return count;
		}

		/// <summary>
		/// 新增一个元数据List区域
		/// </summary>
		/// <param name="strListName"></param>
		/// <returns></returns>
		public string AddMetaList(string strListName)
		{
			var count = GetNamesCountInSheet("MetaData") + 1;
			var rangeCxR1 = Application.get_Range(string.Format("=MetaData!${0}$1", ConvertInt2Letter(count)));//TODO: 能否正确获取range?
			var strBeloneListName = GetBelongDtlName(rangeCxR1);
			while (strBeloneListName != null)
			{
				count += 1;
				rangeCxR1 = Application.get_Range(string.Format("=MetaData!${0}$1", ConvertInt2Letter(count)));
				strBeloneListName = GetBelongDtlName(rangeCxR1);
			}
			var colL = ConvertInt2Letter(count);
			Application.Names.Add(strListName, string.Format("=MetaData!${0}$1:${0}$2", colL));
			return colL;
		}
		public string AddMetaList(string strListName, DataTable dt)
		{
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
			try
			{
				//MessageBox.Show(range.Validation.Type.ToString());
				return (range.Validation.Type == Excel.XlDVType.xlValidateList.GetHashCode());
			}
			catch (Exception exp)
			{
				//MessageBox.Show(exp.Message);
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
			if (range.MergeCells)
			{
				return true;
			}
			return false;
		}

		#endregion

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
