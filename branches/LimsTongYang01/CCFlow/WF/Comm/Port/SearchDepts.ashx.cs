using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Comm.Port
{
    /// <summary>
    /// SearchDepts 的摘要说明
    /// </summary>
    public class SearchDepts : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var method = context.Request["method"];
            switch (method)
            {
                case "search":
                    context.Response.Write(search(context));
                    break;
                case "group":
                    context.Response.Write(group(context));
                    break;
                case "all":
                    context.Response.Write(all(context));
                    break;
                case "getdepts":
                    context.Response.Write(getdepts(context));
                    break;
            }
        }

        private string getdepts(HttpContext context)
        {
            var selectedDepts = context.Request["kw"];

            if (string.IsNullOrWhiteSpace(selectedDepts))
                return "[]";

            var depts = selectedDepts.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var sql = "SELECT No,[Name] FROM Port_Dept WHERE";

            foreach (var dept in depts)
                sql += string.Format(" No = '{0}' OR", dept);

            var dt = BP.DA.DBAccess.RunSQLReturnTable(sql.TrimEnd(" OR".ToCharArray()));

            var reJson = "[";

            foreach(DataRow dr in dt.Rows)
            {
                reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
            }

            reJson = reJson.TrimEnd(',') + "]";

            return reJson;
        }

        private string search(HttpContext context)
        {
            var sql = "SELECT No,[Name],ParentNo FROM Port_Dept ORDER BY No ASC";
            var dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            var zjm = string.Empty;
            var kw = context.Request["kw"].ToLower();
            var haveSub = context.Request["havesub"] == "true";
            var haveSame = context.Request["havesame"] == "true";
            var reJson = "[";
            var drResults = new List<DataRow>();

            foreach (DataRow dr in dt.Rows)
            {
                zjm = BP.DA.DataType.ParseStringToPinyinJianXie(dr["Name"].ToString()).ToLower();

                if (zjm.Contains(kw) || dr["Name"].ToString().Contains(kw))
                {
                    if (drResults.FirstOrDefault(o => o["No"] == dr["No"]) == null)
                    {
                        reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
                        drResults.Add(dr);
                    }

                    if (haveSub)
                    {
                        reJson += getSubDatarow(drResults, dr["No"].ToString(), dt);
                    }

                    if (haveSame)
                    {
                        reJson += getSameDatarow(drResults, dr["ParentNo"].ToString(), dt);
                    }
                }
            }

            reJson = reJson.TrimEnd(',') + "]";

            return reJson;
        }

        /// <summary>
        /// 获取指定父结点下的同级结点
        /// </summary>
        /// <param name="drResults">用于识别结点已经加入JSON的集合</param>
        /// <param name="parentNo">父结点</param>
        /// <param name="dt">所有结点Table</param>
        /// <returns></returns>
        private string getSameDatarow(List<DataRow> drResults, string parentNo, DataTable dt)
        {
            var reJson = string.Empty;
            var drs = dt.Select(string.Format("ParentNo='{0}'", parentNo), "No Asc");

            foreach (var dr in drs)
            {
                if (drResults.FirstOrDefault(o => o["No"] == dr["No"]) != null)
                    continue;

                reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
                drResults.Add(dr);
            }

            return reJson;
        }

        /// <summary>
        /// 获取指定结点下的所有子结点
        /// </summary>
        /// <param name="drResults">用于识别结点已经加入JSON的集合</param>
        /// <param name="no">结点</param>
        /// <param name="dt">所有结点Table</param>
        /// <returns></returns>
        private string getSubDatarow(List<DataRow> drResults, string no, DataTable dt)
        {
            var reJson = string.Empty;
            var drs = dt.Select(string.Format("ParentNo='{0}'", no), "No Asc");

            foreach (var dr in drs)
            {
                if (drResults.FirstOrDefault(o => o["No"] == dr["No"]) != null)
                    continue;

                reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
                drResults.Add(dr);

                reJson += getSubDatarow(drResults, dr["No"].ToString(), dt);
            }

            return reJson;
        }

        private string group(HttpContext context)
        {
            var sql = "SELECT No,[Name],ParentNo FROM Port_Dept ORDER BY No ASC";
            var dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            var zjm = string.Empty;
            var kw = context.Request["kw"];
            var haveSub = context.Request["havesub"] == "true";
            var haveSame = context.Request["havesame"] == "true";
            var reJson = "[";
            var drResults = new List<DataRow>();

            foreach (DataRow dr in dt.Rows)
            {
                zjm = BP.DA.DataType.ParseStringToPinyinJianXie(dr["Name"].ToString());

                if (!string.IsNullOrWhiteSpace(zjm) && zjm[0].ToString() == kw)
                {
                    if (drResults.FirstOrDefault(o => o["No"] == dr["No"]) == null)
                    {
                        reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
                        drResults.Add(dr);
                    }

                    if (haveSub)
                    {
                        reJson += getSubDatarow(drResults, dr["No"].ToString(), dt);
                    }

                    if (haveSame)
                    {
                        reJson += getSameDatarow(drResults, dr["ParentNo"].ToString(), dt);
                    }
                }
            }

            reJson = reJson.TrimEnd(',') + "]";

            return reJson;
        }

        private string all(HttpContext context)
        {
            var sql = "SELECT No,[Name] FROM Port_Dept ORDER BY No ASC";
            var dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            var reJson = "[";

            foreach (DataRow dr in dt.Rows)
            {
                reJson += "{\"No\":\"" + dr["No"] + "\",\"Name\":\"" + dr["Name"] + "\"},";
            }

            reJson = reJson.TrimEnd(',') + "]";

            return reJson;
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