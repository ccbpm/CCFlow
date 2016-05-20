using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_DoType : BP.Web.WebPage
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.


        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "FlowCheck":
                    BP.WF.Flow fl = new BP.WF.Flow(this.RefNo);
                    this.Ucsys1.AddFieldSet("流程检查信息");

                    this.Title = fl.Name+"流程检查";

                    string info = fl.DoCheck().Replace("@", "<BR>@");
                    info = info.Replace("@错误", "<font color=red><b>@错误</b></font>");
                    info = info.Replace("@警告", "<font color=yellow><b>@警告</b></font>");
                    info = info.Replace("@信息", "<font color=black><b>@信息</b></font>");

                    this.Ucsys1.Add(info); //  流程检查信息
                    this.Ucsys1.AddFieldSetEnd();
                    break;
                default:
                    this.Ucsys1.AddMsgOfInfo("错误标记", this.DoType);
                    break;
            }
        }
    }
}