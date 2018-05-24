using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Web;
using BP.DA;

namespace CCFlow.SDKFlowDemo
{
    public partial class DoUrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检查变量是否完整.
            string fk_flow = this.Request.QueryString["FK_Flow"];
            string fk_nodeStr = this.Request.QueryString["FK_Node"];
            string workidStr = this.Request.QueryString["WorkID"];
            if (DataType.IsNullOrEmpty(fk_flow) || DataType.IsNullOrEmpty(fk_nodeStr)
                || DataType.IsNullOrEmpty(workidStr))
            {
                this.Response.Write("ERR: 参数不完整,原始的url是:"+this.Request.RawUrl);
                return;
            }
            #endregion 检查变量是否完整.

            
            this.Response.Redirect("@子流程已经成功启动了.");
        }
    }
}