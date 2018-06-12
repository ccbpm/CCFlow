using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.App.F001
{
    public partial class Apply : System.Web.UI.UserControl
    {
        #region 公用方法
        public void ToMsg(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect(
                "/SDKFlowDemo/SDK/Info.aspx?FK_Flow=2&FK_Type=2&FK_Node=2&WorkID=22" + DateTime.Now.ToString(), false);
        }
        public void ToErrorPage(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect("/SDKFlowDemo/SDK/ErrorPage.aspx", false);
        }
        #endregion 公用方法s

        #region 接受4大参数.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
         public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
         public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
         public Int64 FID
         {
             get
             {
                 return Int64.Parse(this.Request.QueryString["FID"]);
             }
         }
        #endregion 4大参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            this.Page.Title = "您好:"+BP.Web.WebUser.No+" - "+BP.Web.WebUser.Name+" . 当前节点:"+nd.Name;
            if (this.FK_Node == 11001)
            {
                /*如果是开始节点，就不能允许退回。*/
                this.Btn_Return.Enabled = false;
            }
        }
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                this.Response.Write(objs.ToMsgOfHtml());
                this.ToMsg(objs.ToMsgOfHtml());
            }
            catch(Exception ex)
            {
                this.ToMsg(ex.Message);
            }
        }
        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Return_Click(object sender, EventArgs e)
        {
           // BP.WF.Dev2Interface.UI_Window_Return(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
        }
        /// <summary>
        /// 轨迹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Track_Click(object sender, EventArgs e)
        {
           // BP.WF.Dev2Interface.UI_Window_OneWork(this.FK_Flow,   this.WorkID, this.FID);
        }
        /// <summary>
        /// 抄送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_CC_Click(object sender, EventArgs e)
        {
          //  BP.WF.Dev2Interface.UI_Window_CC(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
        }
    }
}