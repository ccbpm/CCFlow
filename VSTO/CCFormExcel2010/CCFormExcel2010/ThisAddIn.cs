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
        // 定义一个任务窗体 
		//internal Microsoft.Office.Tools.CustomTaskPane helpTaskPane;
        /// <summary>
        /// 测试的参数变量.
        /// </summary>
        public void InitTester()
        {
            Glo.UserNo = "anjian";
            Glo.SID = "d-d-d-d-sdsds";
            Glo.WorkID = 10001;
            Glo.FK_Flow = "002";
            Glo.FK_Node = 201;
            Glo.FrmID = "CY_6501"; //采样表单ID.
            Glo.WSUrl = "http://localhost/WF/CCForm/CCFormAPI.asmx";
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

                string tempFile="C:\\CCFlow\\temp.xlsx";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //写入文件.
                BP.Excel.Glo.WriteFile(tempFile, bytes);

                string tempFile2 = "C:\\CCFlow\\temp2.xlsx";
                System.IO.File.Copy(tempFile, tempFile2, true);

                //启动这个文件.
                Process.Start("excel.exe", tempFile2);

                //如果打开的是模板，则还需填充数据
                if (isExists == false)
                {
                    //获得该表单的，物理数据.
                    DataSet ds = client.GenerDBForVSTOExcelFrmModel(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID);

                    #region 给主从表赋值.
                    //给主表赋值.
                    DataTable dtMain = ds.Tables["MainTable"];

                    //给从表赋值.
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt.TableName == "MainTable")
                            continue;
                    }
                    #endregion 给主从表赋值.
                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                //TODO: 
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

			Excel.Worksheet sheet = sh as Excel.Worksheet;
			MessageBox.Show(sheet.Name + "," + range.Value);
		}

		/// <summary>
		/// 执行保存
		/// </summary>
		/// <param name="Wb"></param>
		/// <param name="Success"></param>
		void Application_WorkbookAfterSave(Excel.Workbook Wb, bool Success)
		{
			var a = GetMainTableAtParas();


			//若插件没有加载成功：直接
			if (!Glo.LoadSuccessful)
                return;

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
		/// 获取excel中所有的表单字段数据（json）
		/// </summary>
		/// <returns></returns>
		public string GetJsonData()
		{
			return null;
		}

		public string GetMainTableAtParas()//ref DataSet ds)
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
						htParas.Add(name, range.Value2);
					}
					else //属于子表
					{

					}
				}
				else
				{
					continue;
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
		/// 判断某单元格是否属于某个子表
		/// </summary>
		/// <param name="strAddr">单元格Address</param>
		/// <returns>子表名/null</returns>
		public string GetBelongDtlName(string strSheet, int col, int row)
		{
			for (int i = 1; i <= Application.Names.Count; i++)
			{
				var location = Application.Names.Item(i).RefersToLocal; //=Sheet1!$B$2:$C$3
				string rgxArea = "^\\=\\S+\\!\\$\\D+\\$\\d+\\:\\$\\D+\\$\\d+$";
				if (Regex.IsMatch(location, rgxArea)) //是区域
				{
					var name = Application.Names.Item(i).NameLocal;
					var range = Application.Names.Item(i).RefersToRange;
					if (strSheet == range.Worksheet.Name
						&& col >= range.Column && col <= range.Column + range.Columns.Count - 1
						&& row >= range.Row && col <= range.Row + range.Rows.Count - 1)
					{
						return name;
					}
				}
				else
				{
					continue;
				}
			}
			return null;
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
