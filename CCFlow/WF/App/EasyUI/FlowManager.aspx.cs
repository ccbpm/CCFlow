using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CCFlow.AppDemoLigerUI.Base;
using BP.WF;
using System.Web.Script.Serialization;
using System.Data;

namespace CCFlow.AppDemoLigerUI
{
    public partial class FlowManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request["action"];
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("loadData"))
                {
                    GetData();
                }
                else if (type.Equals("Delete"))
                {
                    Delete();
                }
            }
        }


        private void Delete()
        {

            string workID = Request["Key"];
            string fk_flow = Request["FK_Flow"];

            string str = null;

            if (string.IsNullOrEmpty(workID) || string.IsNullOrEmpty(fk_flow))
            {
                str = "传入的参数有误";
            }
            else
            {
                try
                {
                    //WorkFlow wf = new WorkFlow(fk_flow, Convert.ToInt64(workID));
                    //wf.DoDeleteWorkFlowByReal(true);

                    string info = BP.WF.Dev2Interface.Flow_DoDeleteDraft(fk_flow, long.Parse(workID), true);
                    if (info.Equals("删除成功"))
                    {
                        str = "True";
                    }
                    else
                    {
                        str = info;
                    }
                       
                }
                catch(Exception ex)
                {
                    str = "删除出现错误:"+ex.Message;
                }
            }

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //str = serializer.Serialize(str);

            Response.Clear();
            Response.Write(str);
            Response.End();
            

        }
        private void GetData()
        {
            string StrQuery = Request["key"];

            StrQuery = HttpUtility.UrlDecode(StrQuery, System.Text.Encoding.UTF8);

            string sql = "";
            string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta;
            if (string.IsNullOrEmpty(StrQuery.Trim()))
            { sql = "SELECT  * FROM WF_GenerWorkFlow where 1=1 and  "+wfSql+" order by RDT desc"; }
            else
            {
                // StrQuery = Server.UrlDecode(StrQuery);
                sql = "select * from (SELECT * from WF_GenerWorkFlow where  "+wfSql+") as WF_GenerWorkFlow where  1=1 and "+StrQuery+" order by RDT desc  ";
            }

            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                if (row["StarterName"].ToString() == "Guest")
                {
                    int oid = int.Parse(row["WorkID"].ToString());
                    if (oid != 0)
                    {
                        string ndXSql = "select * from ND" + int.Parse(row["FK_Flow"].ToString()) + "Rpt where OID = '" +
                                        oid + "'";
                        DataTable ndTable = BP.DA.DBAccess.RunSQLReturnTable(ndXSql);
                        if (ndTable.Rows.Count > 0)
                        {
                            row["StarterName"] = ndTable.Rows[0]["GuestName"];
                        }
                    }
                }
            }
            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.ToUpper();
            }
            string result = CommonDbOperator.GetJsonFromTable(dt);

            Response.Clear();
            Response.Write(result);
            Response.End();
        }


    }
}