using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Web.Script.Serialization;

namespace CCFlow.AppDemoLigerUI.Base
{
    /// <summary>
    /// DataServices 的摘要说明
    /// </summary>
    public class DataServices : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string type = context.Request["action"];
            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("getQiYeList"))//获取企业列表
                {
                    GetQiYeList(context);
                }
                else if (type.Equals("getEmp"))//获取用户
                {
                    GetEmps(context);
                }
                else if (type.Equals("GetState"))//获取企业的状态
                {
                    GetState(context);
                }
                else if (type.Equals("removeParts"))//获取是否有撤件流程
                {
                    GetRemoveParts(context);
                }
                else if (type.Equals("cleare"))//获取是否存在清场流程
                {
                    GetCleare(context);
                }
                else if (type.Equals("returnParts"))//获取是否已经存在请退流程
                {
                    GetReturn(context);
                }
                else if (type.Equals("unFix"))//获取是否存在解冻流程
                {
                    GetUnFix(context);
                }
                else if (type.Equals("GetFlow"))//获取所有流程的类型
                {
                    GetFlow(context);
                }
                else if (type.Equals("GetSurveyStatus"))//获取表中的状态字段内容
                {
                    GetSurveyStatus(context);
                }
                else if (type.Equals("GetRemarkStatus"))//获取表中的备注字段内容
                {
                    GetRemarkStatus(context);
                }
                else if (type.Equals("GetSchedule"))//获取进度状态
                {
                    GetSchedule(context);
                }
            }
        }

        /// <summary>
        /// 获取所有流程的类型
        /// </summary>
        /// <param name="context"></param>
        private void GetFlow(HttpContext context)
        {
            string sql = "select No,Name from WF_Flow";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataRow newRow = dt.NewRow();
            newRow["No"] = "";
            newRow["Name"] = "全部";
            dt.Rows.Add(newRow);

            DataView view = dt.AsDataView();
            view.Sort = "No Asc";
            dt = view.ToTable();

            System.Collections.ArrayList dic = new System.Collections.ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    drow.Add(dc.ColumnName, row[dc.ColumnName]);
                }
                dic.Add(drow);
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string result = jss.Serialize(dic);
            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();
        }
        /// <summary>
        /// 获取是否存在解冻流程
        /// </summary>
        /// <param name="context"></param>
        private void GetUnFix(HttpContext context)
        {
            string key = context.Request["key"];
            string result = null;
            if (!string.IsNullOrEmpty(key))//判断传入的workid 是否为空
            {
                /*查询是否存在此WorkID的流程*/
                string sql = "select COUNT(WorkID) from WF_GenerWorkFlow where (WFState!=7 or WFState!=3) and  FK_Flow='005'  and PWorkID='" + key + "'";
                result = BP.DA.DBAccess.RunSQLReturnValInt(sql.ToString()).ToString();
            }
            else
            {
                result = "error";
            }

            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();
        }
        /// <summary>
        /// 获取是否存在请退流程
        /// </summary>
        /// <param name="context"></param>
        private void GetReturn(HttpContext context)
        {
            string key = context.Request["key"];
            string result = null;
            if (!string.IsNullOrEmpty(key))//判断传入的workid 是否为空
            {
                /*查询是否存在此WorkID的流程*/
                string sql = "select COUNT(*) from WF_GenerWorkFlow where (WFState!=7 or WFState!=3) and  FK_Flow='008' and PWorkID='" + key + "'";
                result = BP.DA.DBAccess.RunSQLReturnValInt(sql.ToString()).ToString();
            }
            else
            {
                result = "error";
            }

            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();
        }
        /// <summary>
        /// 获取是否存在清场流程
        /// </summary>
        /// <param name="context"></param>
        private void GetCleare(HttpContext context)
        {
            string key = context.Request["key"];
            string result = null;
            if (!string.IsNullOrEmpty(key))//判断传入的workid 是否为空
            {
                /*查询是否存在此WorkID的流程*/
                StringBuilder sql = new StringBuilder();
                sql.Append("select count(*) from( ");
                sql.Append(" select * from WF_GenerWorkFlow where PWorkID in (select WorkID from WF_GenerWorkFlow where PWorkID='" + key + "' and FK_Flow='004') ");
                sql.Append(" union ");
                sql.Append(" select * from WF_GenerWorkFlow where PWorkID='" + key + "' and FK_Flow='006' )as WF_GenerWorkFlow where WFState!=7 and WFState!=3");
                result = BP.DA.DBAccess.RunSQLReturnValInt(sql.ToString()).ToString();
            }
            else
            {
                result = "error";
            }

            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();

        }

        /// <summary>
        /// 获取是否含有正在运行中的撤销流程
        /// </summary>
        /// <param name="context"></param>
        private void GetRemoveParts(HttpContext context)
        {
            string key = context.Request["key"];
            string result = null;
            if (!string.IsNullOrEmpty(key))//判断传入的workid 是否为空
            {
                /*查询是否存在此WorkID的流程*/
                string sql =
                    "select count(*) from (select * from WF_GenerWorkFlow where WFState!=7 or WFState!=3 )as WF_GenerWorkFlow where  PWorkID= '" +
                    key + "'";

                result = BP.DA.DBAccess.RunSQLReturnValInt(sql).ToString();
            }
            else
            {
                result = "error";
            }

            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();
        }

        
        /// <summary>
        /// 获取人员信息
        /// </summary>
        private void GetEmps(HttpContext context)
        {
            //string result = "";
            //string[] str = context.Request.RawUrl.Split('&');
            //string key3 = str[str.Length - 1].Split('=')[1];
            //string realKey = HttpUtility.UrlDecode(key3, System.Text.Encoding.UTF8);

            string result = "";
            string[] str = context.Request.RawUrl.Split('&');
            string key3 = null;
            string top = null;
            foreach (string single in str)
            {
                if (single.StartsWith("q="))
                {
                    key3 = single.Split('=')[1];
                }
                else if (single.StartsWith("limit="))
                {
                    top = single.Split('=')[1];
                }
            }
            if (string.IsNullOrEmpty(top))
            {
                top = "10";
            }

            string realKey = HttpUtility.UrlDecode(key3, System.Text.Encoding.UTF8);

            string sql = "select top " + top + " * from port_emp where Name like '%" + realKey + "%' or No like'%" + realKey + "%'";

            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            System.Collections.ArrayList dic = new System.Collections.ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                dic.Add(drow);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            result = jss.Serialize(dic);

            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();

        }
        /// <summary>
        /// 获取企业信息 
        /// </summary>
        /// <param name="context"></param>
        private void GetQiYeList(HttpContext context)
        {
            try
            {
                string result = "";
                string[] str = context.Request.RawUrl.Split('&');
                string key3 = null;
                string top = null;
                foreach (string single in str)
                {
                    if (single.StartsWith("q="))
                    {
                        key3 = single.Split('=')[1];
                    }
                    else if (single.StartsWith("limit="))
                    {
                        top = single.Split('=')[1];
                    }
                }
                if (string.IsNullOrEmpty(top))
                {
                    top = "10";
                }

                string realKey = HttpUtility.UrlDecode(key3, System.Text.Encoding.UTF8);

                string sql = "SELECT  top " + top + " * FROM V_Inc where Name like '%" + realKey + "%'";
                System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                System.Collections.ArrayList dic = new System.Collections.ArrayList();
                foreach (DataRow dr in dt.Rows)
                {
                    System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                    }
                    dic.Add(drow);
                }
                JavaScriptSerializer jss = new JavaScriptSerializer();
                result = jss.Serialize(dic);
                // result = CommonDbOperator.GetJsonFromTableEasy(dt,count);

                context.Response.Clear();
                context.Response.Write(result);
                context.Response.End();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 获取企业当前状态
        /// </summary>
        private void GetState(HttpContext context)
        {
            string key = context.Request["key"];
            string result = "true";
            if (!string.IsNullOrEmpty(key))
            {
                string sql = "select * from JXW_Inc where no = '" + key + "' and IncSta != 1";

                int count = BP.DA.DBAccess.RunSQLReturnCOUNT(sql);

                if (count > 0)
                {
                    result = "true";
                }
                else
                {
                    result = "false";
                }

            }
            else
            {
                result = "error";
            }
            context.Response.Clear();
            context.Response.Write(result);
            context.Response.End();

        }

        /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <returns></returns>
        private void GetSurveyStatus(HttpContext context)
        {
            string TableName = "";
            string Status = "";
            try
            {
                string FK_Flow = context.Request["FK_Flow"];
                string WorkID = context.Request["WorkID"];
                int _FK_Flow = int.Parse(FK_Flow);
                TableName = "ND" + _FK_Flow + "Rpt";
                string sqlSort = "SELECT Status FROM " + TableName + " WHERE OID=" + WorkID;
                Status = BP.DA.DBAccess.RunSQLReturnString(sqlSort);
            }
            catch { }
            context.Response.Clear();
            context.Response.Write(Status);
            context.Response.End();

        }

        /// <summary>
        /// 获取备注信息
        /// </summary>
        /// <returns></returns>
        private void GetRemarkStatus(HttpContext context)
        {
            string FK_Flow = context.Request["FK_Flow"];
            string WorkID = context.Request["WorkID"];
            if (FK_Flow == "001")
            {
                GetSchedule(context);
            }
            else
            {
                string TableName = "";
                string Status = "";
                try
                {
                    int _FK_Flow = int.Parse(FK_Flow);
                    TableName = "ND" + _FK_Flow + "Rpt";
                    string sqlSort = "SELECT Remark FROM " + TableName + " WHERE OID=" + WorkID;
                    Status = BP.DA.DBAccess.RunSQLReturnString(sqlSort);
                }
                catch { }
                context.Response.Clear();
                context.Response.Write(Status);
                context.Response.End();
            }
        }

        //获取进度
        private void GetSchedule(HttpContext context)
        {
            string Remark = "error";
            string WorkID = context.Request["WorkID"];
            try
            {
                int wfstate = 0;
                StringBuilder sql = new StringBuilder();

                sql.Append("select '002' as FlowNo,'政府分理' as FlowName,OID as WorkID,Title,WFState,BillNo,FLowStartRDT,FlowEndNode,FlowEnder,GovUserID as GuestNo, GovUserName as GuestName,GovNo as GuestDepNo,GovName as GuestDepName from ND2Rpt ");
                sql.Append(" where PFlowNo='001' and PWorkId='" + WorkID + "' and wfstate!=7 ");
                sql.Append(" UNION ");
                sql.Append(" select '012' as FlowNo,'转交子平台' as FlowName,OID as WorkID,Title,WFState,BillNo,FlowStartRDT,FlowEndNode,FlowENder,'' as GuestNo,''  as GuestName,FlowStarter as GuestDepNo, '' as GuestDepName from ND12Rpt ");
                sql.Append(" where PFlowNo='001' and PWorkId='" + WorkID + "' and wfstate!=7 ");

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql.ToString());
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["wfstate"].ToString() == "3")
                        wfstate++;
                }
                Remark = "进度：" + wfstate + "/" + dt.Rows.Count;
                string str = "update WF_GenerWorkFlow set Remark='" + Remark + "' where WorkID=" + WorkID;
                BP.DA.DBAccess.RunSQL(str);
            }
            catch { }
            context.Response.Clear();
            context.Response.Write(Remark);
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}