using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;


namespace CCFlow.SDKFlowDemo
{
    public partial class TestCondOfFlowOver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 获取必要的变量.
            string flowNo = this.Request.QueryString["FK_Flow"];
            string nodeID = this.Request.QueryString["FK_Node"];
            string workid = this.Request.QueryString["WorkID"];
            string fid = this.Request.QueryString["FID"];
            string userNo = this.Request.QueryString["UserNo"];
            string sid = this.Request.QueryString["SID"];
            #endregion 获取必要的变量.

            //如果没有toNodeID ,就结束流程。
            string toNodeID = this.Request.QueryString["ToNodeID"].Trim();
            if (DataType.IsNullOrEmpty(toNodeID))
                this.Response.Write("1");
            else
                this.Response.Write("0");
        }
    }
}