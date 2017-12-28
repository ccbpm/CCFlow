using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.SDK.SDKComponents.F159
{
    public partial class S15901 : System.Web.UI.Page
    {
        #region 接受4大参数.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 当前节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        /// <summary>
        /// 流程ID，不是分合流的时候始终等于0.
        /// </summary>
        public Int64 FID
        {
            get
            {
                if (this.Request.QueryString["FID"] == null)
                    return 0;
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 4大参数.

        #region 公用方法
        /// <summary>
        /// 转到提示信息界面.
        /// </summary>
        /// <param name="msg">提示信息</param>
        public void ToMsg(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect(
                "/SDKFlowDemo/SDK/Info.aspx?xx=" + DateTime.Now.ToString(), false);
        }
        /// <summary>
        /// 转到提示错误信息界面
        /// </summary>
        /// <param name="msg">错误信息</param>
        public void ToErrorPage(string msg)
        {
            System.Web.HttpContext.Current.Session["info"] = msg;
            System.Web.HttpContext.Current.Application["info" + BP.Web.WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            System.Web.HttpContext.Current.Response.Redirect("/SDKFlowDemo/SDK/ErrorPage.aspx?xxx=" + DateTime.Now.ToString(), false);
        }
        #endregion 公用方法

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                this.Save();

                //执行发送.
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                //转到提示.
                this.ToMsg(objs.ToMsgOfHtml());
            }
            catch(Exception ex)
            {
                this.Response.Write("发送失败:"+ex.Message);
            }
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            this.Save();
        }
        /// <summary>
        /// 保存方法
        /// </summary>
        public void Save()
        {
            this.Response.Write("<font color=green>@已经出发到保存的业务逻辑,保存成功.</font>");
        }
    }
}