using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.GuestApp
{
    public partial class IncFrm : System.Web.UI.Page
    {
        #region 接受4大参数(这四大参数是有ccflow传递到此页面上的).
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
        /// 当前节点ID
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
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 接受4大参数(这四大参数是有ccflow传递到此页面上的).

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //求出该企业的监管人.
           // string jgr = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT  jgr FROM WHERE vUserName='" + BP.Web.GuestUser.No + "'", null);
           
            //执行发送.
            string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID).ToMsgOfHtml();
            this.Response.Write(msg);

            if (this.FK_Node == 102)
            {
                //根据选择，向业务表，初始化数据。
            }
             
        }
    }
}