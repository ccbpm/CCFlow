using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.UC
{
    public partial class MyFlowInfoWap : BP.Web.UC.UCBase3
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                    s = "012";
                return s;
            }
        }
        /// <summary>
        /// 当前的工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                if (ViewState["WorkID"] == null)
                {
                    if (this.Request.QueryString["WorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                else
                    return Int64.Parse(ViewState["WorkID"].ToString());
            }
            set
            {
                ViewState["WorkID"] = value;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "DeleteFlow":
                    string fk_flow = this.Request.QueryString["FK_Flow"];
                    Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
                    try
                    {
                        WorkFlow wf = new WorkFlow(new Flow(fk_flow), workid);
                        wf.DoDeleteWorkFlowByReal(true);
                    }
                    catch
                    {

                    }
                    this.Session["info"] = "流程删除成功";
                    break;
                case "UnShift":
                    try
                    {
                        WorkFlow mwf = new WorkFlow(this.FK_Flow, this.WorkID);
                        string str = mwf.DoUnShift();
                        this.Session["info"] = str;
                    }
                    catch (Exception ex)
                    {
                        this.Session["info"] = "@执行撤消失败。@失败信息" + ex.Message;
                    }
                    break;
                case "UnSend":
                    try
                    {
                        string str = BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
                        this.Session["info"] = str;
                    }
                    catch (Exception ex)
                    {
                        this.Session["info"] = "@执行撤消失败。@失败信息" + ex.Message;
                    }
                    break;
                default:
                    break;
            }

            string s = this.Session["info"] as string;
            this.Session["info"] = null;
            if (s == null)
                s = this.Application["info" + WebUser.No] as string;

            if (s == null)
                s = BP.WF.Glo.SessionMsg;

            s = s.Replace("@@", "@");

            if (s.Length > 0 && s.Substring(s.Length - 1) == "@")
                s = s.Substring(0, s.Length - 1);

            s = s.Replace("@", "<BR><BR><img src='Img/dot.png' align='middle' width='8px' />&nbsp;");

            this.Add("<div style='width:500px;text-align:left;font-size:14px'>");
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='" + BP.WF.Glo.CCFlowAppPath + "/WF/Img/Home.gif' border=0/>Home</a> - 操作提示", s);
            else
                this.AddFieldSet("<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/info.png' align='middle' />操作提示", s);

            this.Add("<br><br></div>");
            return;
        }
    }

}