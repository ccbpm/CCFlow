using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BP.DA;

namespace CCFlow.SDKFlowDemo
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        public HttpContext myHttpContext = null;
        public void ProcessRequest(HttpContext context)
        {
            myHttpContext = context;

            string sql = "";
            string doType = context.Request.QueryString["DoType"];


            #region 开窗返回值的demo.
            //获得部门列表, 开窗返回值json. 这里一定要确定获取的顶级部门的 ParentNo=0.
            if (doType == "ReqDepts")
            {
                string rootNo = context.Request.QueryString["Key"];

                if (BP.WF.Glo.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                        sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE OrgNo='" + BP.Web.WebUser.OrgNo + "'  ";

                if (BP.WF.Glo.CCBPMRunModel == BP.Sys.CCBPMRunModel.GroupInc)
                {
                    sql = "SELECT No,Name,ParentNo FROM Port_Dept ";
                    if (BP.DA.DataType.IsNullOrEmpty(rootNo) == false)
                        sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE ParentNo='"+ rootNo + "' ";
                }

                if (BP.WF.Glo.CCBPMRunModel == BP.Sys.CCBPMRunModel.Single)
                {
                    sql = "SELECT No,Name,ParentNo FROM Port_Dept ";
                    if (BP.DA.DataType.IsNullOrEmpty(rootNo) == false)
                        sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE ParentNo='" + rootNo + "' ";
                }

                DataTable dt =DBAccess.RunSQLReturnTable(sql);

                //对saas模式单独的处理.
                if (BP.WF.Glo.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["No"].ToString().Equals(BP.Web.WebUser.OrgNo))
                        {
                            dr["ParentNo"] = "0";
                            break;
                        }
                    }
                }


                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }

            //获得查询来的人员信息, 开窗返回值json.
            if (doType == "SearchEmps")
            {
                string key = context.Request.QueryString["Keyword"];

                if (BP.WF.Glo.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                    sql = "SELECT No,Name  FROM Port_Emp WHERE (No like '%" + key + "%' OR Name like '%" + key + "%') AND OrgNo='" + BP.Web.WebUser.OrgNo + "'  ";
                else
                    sql = "SELECT No,Name  FROM Port_Emp WHERE No like '%" + key + "%' OR Name like '%" + key + "%' ";

                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }

            //获得部门列表, 开窗返回值json.
            if (doType == "ReqEmpsByDeptNo")
            {
                string deptNo = context.Request.QueryString["DeptNo"];

                sql = " SELECT A.No,A.Name FROM Port_Emp A, Port_DeptEmp B ";
                sql+=" WHERE ( A.FK_Dept=B.FK_Dept AND B.FK_Dept='"+ deptNo + "') OR A.FK_Dept = '"+ deptNo + "'";

                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }
            #endregion 开窗返回值的demo.

            #region 从表导入.
            if (doType == "DtlImpSearchKey")
            {
                string key = context.Request.QueryString["Keyword"];
                key = HttpUtility.UrlDecode(key, System.Text.Encoding.UTF8);
                sql = "SELECT No,Name  FROM Port_Emp WHERE No like '%" + key + "%' OR Name like '%" + key + "%' ";
                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }
            if (doType == "DtlImpReqAll")
            {
                string key = context.Request.QueryString["Keyword"];
                sql = "select No,Name FROM Port_StationType";
                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }
            if (doType == "DtlImpReq1" || doType == "DtlImpReq2" || doType == "DtlImpReq3")
            {
                sql = "SELECT No,Name FROM Port_Emp WHERE  1=1 ";
                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }

            if (doType == "DtlImpFullData")
            {
                string key = context.Request.QueryString["Keyword"];
                sql = "SELECT No,Name  FROM Port_Emp WHERE  FK_Duty='0" + key + "' ";
                if (key.Equals("all"))
                {
                    sql = "SELECT No,Name  FROM Port_Emp WHERE  1=1";
                }
                DataTable dt =DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }
            #endregion 从表导入.
        }

        public void OutInfo(string info)
        {
            myHttpContext.Response.ContentType = "text/plain";
            myHttpContext.Response.Write(info);
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