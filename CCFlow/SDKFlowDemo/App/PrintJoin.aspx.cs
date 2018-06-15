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
            string docs = "";
            string[] fls = System.IO.Directory.GetFiles(BP.Sys.SystemConfig.PathOfTemp);
            foreach (string fl in fls)
            {
                if (fl.Contains("StuTest.doc") == false)
                    continue;

                docs += BP.DA.DataType.ReadTextFile(fl);
                System.IO.File.Delete(fl);
            }

            BP.DA.DataType.WriteFile(BP.Sys.SystemConfig.PathOfTemp + "\\T.doc", docs);
            this.Response.Redirect("/DataUser/Temp/T.doc", true);
        }

    }
}