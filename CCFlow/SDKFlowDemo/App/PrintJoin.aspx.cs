using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.App
{
    public partial class PrintJoin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //空白文本
            string tempDoc = BP.Sys.SystemConfig.PathOfDataUser + "\\CyclostyleFile\\demo.rtf";
            //输出文本
            string outDoc = BP.Sys.SystemConfig.PathOfTemp + "\\StuTestMain.doc";

            BP.WF.PrintJoin.InsertMerge(tempDoc, BP.Sys.SystemConfig.PathOfTemp, outDoc);
            this.Response.Redirect("/DataUser/Temp/StuTestMain.doc", true);
        }

    }
}