using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.WF;
using BP.En;
using System.Text;


namespace CCFlow.AppDemoLigerUI.Base
{
    public partial class FlowManagerEUI : BasePage
    {

        HttpContext _Context = null;

        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();

            switch (method)
            {
                case "worklist"://启动项
                    s_responsetext = GetWorkList();
                    break;
                case "delete":
                    s_responsetext = Delete();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        //删除
        private string Delete()
        {

            string workID = Request["Key"];
            string fk_flow = Request["FK_Flow"];

            string str = null;

            if (string.IsNullOrEmpty(workID) || string.IsNullOrEmpty(fk_flow))
            {
                return "传入的参数有误";
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
                        return str;
                    }
                    else
                    {
                        str = info;
                        return str;
                    }

                }
                catch (Exception ex)
                {
                    return str = "删除出现错误:" + ex.Message;
                }
            }



        }




        private string GetWorkList()
        {
            string StrQuery = Request["key"];
            //当前页
            string pageNumber = getUTF8ToString("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = getUTF8ToString("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            StrQuery = HttpUtility.UrlDecode(StrQuery, System.Text.Encoding.UTF8);

            string sql = "";
            string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta;
            if (string.IsNullOrEmpty(StrQuery.Trim()))
            { sql = "SELECT  * FROM WF_GenerWorkFlow where 1=1 and  " + wfSql + " order by RDT desc"; }
            else
            {
                // StrQuery = Server.UrlDecode(StrQuery);
                sql = "select * from (SELECT * from WF_GenerWorkFlow where  " + wfSql + ") as WF_GenerWorkFlow where  1=1 and " + StrQuery + " order by RDT desc  ";
            }



            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);


            foreach (DataRow row in dt.Rows)
            {
                if (row["StarterName"].ToString() == "Guest")
                {
                    int oid = int.Parse(row["WorkID"].ToString());
                    if (oid != 0)
                    {
                        string ndXSql = "select * from ND" + int.Parse(row["FK_Flow"].ToString()) + "Rpt where OID = '" + oid + "'";
                                      
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
            return CommonDbOperator.GetJsonFromTable(dt);
        }


        /// <summary>
        /// 生成取得记录总数的语句
        /// </summary>
        /// <param name="sql">原查询语句</param>
        /// <returns>取得记录总数的语句</returns>
        private string MakeRecordCountsSql(string sql)
        {
            string sqlGetRecordCounts = "select Count(*) from (" + sql + ") as TempTable";
            return sqlGetRecordCounts;
        }

        /// <summary>
        /// 生成分页查询语句
        /// </summary>
        /// <param name="sql">原查询语句</param>
        /// <param name="orderField">用于分页排序的字段</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">页面记录数量</param>
        /// <param name="recordCounts">记录总数</param>
        /// <param name="pageCounts">页面总数</param>
        /// <returns>分页查询语句</returns>
        private string MakePagingSql(string sql, string orderField, int pageNumber, int pageSize, int recordCounts, out int pageCounts)
        {
            // 计算页面数量
            if (Convert.ToInt32(pageNumber) < 1) pageNumber = 1;
            if (Convert.ToInt32(pageSize) < 1) pageSize = 1;
            pageCounts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(recordCounts) / Convert.ToDouble(pageSize)));

            string sqlQuery = "";
            if (pageNumber == 1)
            {
                sqlQuery = "select top " + pageSize + " * from (" + sql + ") as TempTable order by " + orderField + " desc";
            }
            else
            {
                sqlQuery = "select top " + pageSize + " * from (" + sql + ") as TempTable where " + orderField + " < (select min(" + orderField + ") as MinID from ( select top " + (pageNumber - 1) * pageSize + "     * from (" + sql + ") as MaxTempTable order by " + orderField + " desc) as MinTempTable) order by " + orderField + " desc";
            }
            return sqlQuery;
        }
 
    }
}