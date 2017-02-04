using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using BP.Excel;

namespace CCFlowExcel2007
{
    public partial class ThisAddIn
    {
        // 定义一个任务窗体 
        internal Microsoft.Office.Tools.CustomTaskPane helpTaskPane;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            #region 获得外部参数, 这是通过外部传递过来的参数.
            BP.Excel.Glo.UserNo = "wangtao";
            BP.Excel.Glo.SID = "2323232323sdsds";
            BP.Excel.Glo.FK_Flow = "001";
            BP.Excel.Glo.FK_Node = 301;
            BP.Excel.Glo.FrmID = "CY6602";
            BP.Excel.Glo.FK_Node = 305;
            BP.Excel.Glo.WorkID = 10000;
            BP.Excel.Glo.WSUrl = "http://localhost:26507/WF/CCForm/CCFormAPI.asmx";
            #endregion 获得外部参数, 这是通过外部传递过来的参数.

            #region 校验用户安全与下载文件.
            CCFlowExcel2007.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
            byte[] byt = client.GenerExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID);

            // 把这个bye 保存到 c:\temp.xls 里面.

            //打开这个文档.

            #endregion 校验用户安全与下载文件.


            //加载保存代码.
            this.Application.WorkbookBeforeSave += new Excel.AppEvents_WorkbookBeforeSaveEventHandler(Application_WorkbookBeforeSave);



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
        /// 执行保存.
        /// </summary>
        /// <param name="Wb"></param>
        /// <param name="SaveAsUI"></param>
        /// <param name="Cancel"></param>
        void Application_WorkbookBeforeSave(Excel.Workbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            //执行保存.
            CCFlowExcel2007.CCForm.CCFormAPISoapClient client = BP.Excel.Glo.GetCCFormAPISoapClient();
            client.SaveExcelFile(Glo.UserNo, Glo.SID, Glo.FrmID, Glo.WorkID,null,null);
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
