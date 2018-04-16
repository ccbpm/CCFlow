using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;


namespace CCFlow.SDKFlowDemo
{
    public partial class FrmEventND8601 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region 检查变量是否完整.
                string fk_flow = this.Request.QueryString["FK_Flow"];
                string fk_nodeStr = this.Request.QueryString["FK_Node"];
                string OID = this.Request.QueryString["OID"];
                if (DataType.IsNullOrEmpty(fk_flow) || DataType.IsNullOrEmpty(fk_nodeStr)
                    || DataType.IsNullOrEmpty(OID))
                {
                    this.Response.Write("ERR: 参数不完整,原始的url是:" + this.Request.RawUrl);
                    return;
                }
                #endregion 检查变量是否完整.

                string workidsStrs = this.Request.QueryString["WorkIDs"];
                if (DataType.IsNullOrEmpty(workidsStrs))
                {
                    /*有可能是查看节点表单信息，就不要向下执行填充信息了.*/
                    return;
                }

                string[] workids =workidsStrs.Split(',');

                // 清除已经有的信息.
                BP.DA.DBAccess.RunSQL("DELETE FROM  ND8601Dtl1 WHERE RefPK='" + OID + "'");

                foreach (string id in workids)
                {
                    if (DataType.IsNullOrEmpty(id))
                        continue;

                    string sql = "SELECT * FROM ND82Rpt WHERE OID=" + id;
                    DataTable dt =BP.DA.DBAccess.RunSQLReturnTable(sql);
                    string oid = dt.Rows[0]["OID"].ToString();
                    string Title = dt.Rows[0]["Title"].ToString();
                    string XMBH = dt.Rows[0]["XMBH"].ToString();
                    string XMMC = dt.Rows[0]["XMMC"].ToString();
                    string XMDZ = dt.Rows[0]["XMDZ"].ToString();
                    string XMJE = dt.Rows[0]["XMJE"].ToString();


                    //插入之前需要删除该oid,以防止pk重复.
                    BP.DA.DBAccess.RunSQL("DELETE FROM   ND8601Dtl1 WHERE OID='" + id + "'");


                    //将数据插入里面，并且把子线程的WorkID做主键，当前工作ID做RefPK.
                    string insertSQL = "";
                    insertSQL = "INSERT INTO ND8601Dtl1(OID,RefPK,XMBH,XMMC,XMDZ,XMJE) VALUES(" + id + ",'" + OID + "','" + XMBH + "'";
                    insertSQL += ",'" + XMMC + "'";
                    insertSQL += ",'" + XMDZ + "'";
                    insertSQL += "," + XMJE + ")";

                    BP.DA.DBAccess.RunSQL(insertSQL);
                }

                this.Response.Write("装载成功,");
            }
            catch(Exception ex)
            {
                this.Response.Write("err: 装载表单执行事件出现错误:" +ex.Message+" ,URL:"+ this.Request.RawUrl);
            }
        }
    }
}