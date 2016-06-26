using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AppDemo_AdvFunc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BP.WF.XML.ClassicMenuAdvFuncs ens = new BP.WF.XML.ClassicMenuAdvFuncs();
        ens.RetrieveAll();

        int cols = 4; //定义显示列数 从0开始。
        decimal widthCell = 100 / cols;
        this.Pub1.AddTable("width=90% border=0");
        this.Pub1.AddCaptionLeft("<div class='CaptionMsg' >高级功能</div>");
        int idx = -1;
        bool is1 = false;
        foreach (BP.WF.XML.ClassicMenuAdvFunc en in ens)
        {
            if (en.Enable == false)
                continue;

            idx++;
            if (idx == 0)
                is1 = this.Pub1.AddTR(is1);
            this.Pub1.AddTDBegin("width='" + widthCell + "%' border=0 valign=top");

            #region 输出内容.
            
            string strs = "";
            strs += "<a  href='" + en.Url + "' target='main'><img src='" + en.Img + "' />" + en.Name + "</a>";
            this.Pub1.Add(strs);
            #endregion 输出内容.

            this.Pub1.AddTDEnd();
            if (idx == cols - 1)
            {
                idx = -1;
                this.Pub1.AddTREnd();
            }
        }

        while (idx != -1)
        {
            idx++;
            if (idx == cols - 1)
            {
                idx = -1;
                this.Pub1.AddTD();
                this.Pub1.AddTREnd();
                break;
            }
            else
            {
                this.Pub1.AddTD();
            }
        }
        this.Pub1.AddTableEnd();
    }
}
