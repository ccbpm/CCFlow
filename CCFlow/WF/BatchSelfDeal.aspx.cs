using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web;
using BP.DA;

namespace CCFlow.WF
{
    public partial class BatchSelfDeal : System.Web.UI.Page
    {
        #region  WorkIDs
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        public string FK_Flow
        {
            get { try { BP.WF.Node node = new BP.WF.Node(this.FK_Node); return node.FK_Flow; } catch { return null; } }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            string msg = "";
            try
            {
                BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
                string[] strs = this.WorkIDs.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    Int64 wkid = Int64.Parse(str);
                    BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(wkid);

                    BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, wkid, null);

                    msg += "<fieldset>";
                    msg += "<legend>对工作(" + gwf.Title + ")处理情况如下。</legend>";
                    msg += objs.ToMsgOfHtml();
                    msg += "</fieldset>";
                }
                msg = msg.Replace("@", "<br>@");
                this.Response.Write(msg + "<script type='text/javascript'> window.onload = function(){var ifreamTable = document.getElementById('ifreamTable');  ifreamTable.style.display='none';}    </script><br/>[<a href='/WF/Batch.aspx' style='color:blue;'>返回批处理</a>]");
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("节点没有岗位") || ex.Message.Contains("@没有找到可接受的工作人员") || ex.Message.Contains("没有找到人员") || ex.Message.Contains("流程设计错误") || ex.Message.Contains("@您设置的当前节点"))
                //{
                ErrorMessage.InnerHtml = ex.Message + "<br/>";
                //    return;
                //}

                //msg = ex.ToString().Replace("@", "<BR>@");
                //System.Web.HttpContext.Current.Session["info"] = msg;
                //System.Web.HttpContext.Current.Application["info" + WebUser.No] = msg;
                //System.Web.HttpContext.Current.Application["url"] = Request.RawUrl;
                //string url ="/WF/ErrorPage.aspx";
                //this.Response.Redirect(url, true);
            }
        }
    }
}