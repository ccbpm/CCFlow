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

            // 注意不要访问session 变量，不要访问cookies.
            int nodeID = int.Parse(fk_nodeStr);
            Int64 workid = Int64.Parse(workidStr);
            string sql = "SELECT * FROM Sys_M2M WHERE EnOID='"+workid+"' AND FK_MapData='ND"+fk_nodeStr+"'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            string doc = dt.Rows[0]["Doc"].ToString();
            string[] strs = doc.Split(',');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;
                // 发起子流程.
                Hashtable ht = new Hashtable();
                BP.WF.Dev2Interface.Node_StartWork("101", ht,
                    null, 0, null, workid, "102");
            }
            this.Response.Redirect("@子流程已经成功启动了.");
        }
    }
}