using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using BP.Excel;
using System.Management;
using System.Windows.Forms;

namespace CCFlowExcel
{
    public partial class ThisAddIn
    {
        // 定义一个任务窗体 
        internal Microsoft.Office.Tools.CustomTaskPane helpTaskPane;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            #region 获得外部参数, 这是通过外部传递过来的参数.
            
            Dictionary<string, string> args = Glo.GetArguments();
            Glo.LoadSuccessful = args["fromccflow"] == "true";

            if(!Glo.LoadSuccessful)
            {
                //隐藏操作按钮区域
                Globals.Ribbons.RibbonCCFlow.btnSaveFrm.Enabled = false;
                return;
            }

            Globals.Ribbons.RibbonCCFlow.btnSaveFrm.Enabled = true;
            Glo.UserNo = args["UserNo"];
            Glo.SID = args["SID"];
            Glo.FK_Flow = args["FK_Flow"];
            Glo.FK_Node = int.Parse(args["FK_Node"]);
            Glo.FrmID = args["FrmID"];
            Glo.WorkID = int.Parse(args["WorkID"]);
            Glo.WSUrl = args["WSUrl"];
            #endregion 获得外部参数, 这是通过外部传递过来的参数.

            #region 校验用户安全与下载文件.
            //CCFlowExcel2007.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
            //byte[] byt = client.GenerExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID);

            // 把这个bye 保存到 c:\temp.xls 里面.

            #endregion 校验用户安全与下载文件.

            //单元格内容变动监听事件
            this.Application.SheetChange += new Excel.AppEvents_SheetChangeEventHandler(Application_SheetChange);
            //保存事件
            this.Application.WorkbookAfterSave += new Excel.AppEvents_WorkbookAfterSaveEventHandler(Application_WorkbookAfterSave);
            
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
            if (!Glo.LoadSuccessful) return;

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
            if (!Glo.LoadSuccessful) return;

            //执行保存.
            //CCFlowExcel2007.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
            //client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID,null,null);
            //System.Windows.Forms.MessageBox.Show("before save");
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {

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
        }
        
        #endregion
    }
}
