using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Data;
using BP.WF;
using BP.DA;
using BP.WF.Template;
using System.Web;
using System.Web.SessionState;//第一步：导入此命名空间

namespace CCFlow.SDKFlowDemo
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler, IRequiresSessionState
    {
        #region 属性.
        public HttpContext myHttpContext = null;
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(GetVal("WorkID"));
            }
        }
        public string DoType
        {
            get
            {
                return GetVal("DoType");
            }
        }
        public string UserNo
        {
            get
            {
                return GetVal("UserNo");
            }
        }
        public string GetVal(string key)
        {
            string str = myHttpContext.Request.QueryString[key];
            if (str == null)
                str = myHttpContext.Request.Form[key];

            str = BP.WF.Glo.DealExp(str, null, null);
            //str = myHttpContext.Request.InputStream [key];
            return str;
        }
        #endregion 属性.
        public void ProcessRequest(HttpContext context)
        {
            myHttpContext = context;

            #region 登录 & 获取配置信息,公共的区域设置，所有的系统操作都有此接口.
            if (this.DoType.Equals("SystemParas") == true)
            {
                Hashtable ht = new Hashtable();
                ht.Add("SystemNo", "CCFlow");
                ht.Add("SystemShortName", "驰骋BPM");
                ht.Add("SystemName", "驰骋低代码开发平台");
                ht.Add("IncName", "济南驰骋信息技术有限公司");
                ht.Add("IsShowQRLogin", "1"); //是否显示扫码登录？
                this.OutInfo(BP.Tools.Json.ToJson(ht));
                return;
            }
            if (this.DoType.Equals("LogOut"))
            {
                BP.Web.WebUser.Exit();
            }
            if (this.DoType.Equals("Login"))
            {
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = this.GetVal("UserNo");
                if (emp.RetrieveFromDBSources() == 0)
                {
                    this.OutInfo("err@用户名或密码错误.");
                    return;
                }
                string pass = this.GetVal("Pass");
                if (emp.CheckPass(pass) == false)
                {
                    this.OutInfo("err@用户名或密码错误.");
                    return;
                }
                BP.WF.Dev2Interface.Port_Login(emp.No);
                string devType = GetVal("DevType");
                if (BP.DA.DataType.IsNullOrEmpty(devType) == true)
                    devType = "PC";

                //输出token.
                string token = BP.WF.Dev2Interface.Port_GenerToken(emp.No);
                this.OutInfo(token);
                return;
            }
            #endregion 登录.& 获取配置信息.

            string sql = "";

            #region 开窗返回值的demo.
            //获得部门列表, 开窗返回值json.
            if (this.DoType.Equals("ReqDepts") == true)
            {
                sql = "SELECT No,Name, ParentNo FROM Port_Dept ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }

            //获得查询来的人员信息, 开窗返回值json.
            if (this.DoType.Equals("SearchEmps") == true)
            {
                string key = context.Request.QueryString["Keyword"];
                sql = "SELECT No,Name  FROM Port_Emp WHERE No like '%" + key + "%' OR Name like '%" + key + "%' ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }

            //获得部门列表, 开窗返回值json.
            if (this.DoType.Equals("ReqEmpsByDeptNo") == true)
            {
                string deptNo = context.Request.QueryString["DeptNo"];
                sql = "SELECT No,Name  FROM Port_Emp WHERE FK_Dept='" + deptNo + "'";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
                return;
            }
            #endregion 开窗返回值的demo.

            #region 从表导入.
            if (this.DoType.Equals("DtlImpSearchKey") == true)
            {
                string key = context.Request.QueryString["Keyword"];
                key = HttpUtility.UrlDecode(key, System.Text.Encoding.UTF8);
                sql = "SELECT No,Name  FROM Port_Emp WHERE No like '%" + key + "%' OR Name like '%" + key + "%' ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }
            if (this.DoType.Equals("DtlImpReqAll") == true)
            {
                string key = context.Request.QueryString["Keyword"];
                sql = "select No,Name from Port_StationType";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }
            if (this.DoType.Equals("DtlImpReq1") || this.DoType.Equals("DtlImpReq2") || this.DoType.Equals("DtlImpReq3"))
            {
                sql = "SELECT No,Name  FROM Port_Emp WHERE  1=1 ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                string json = BP.Tools.Json.ToJson(dt);
                this.OutInfo(json);
            }

            if (this.DoType.Equals("DtlImpFullData"))
            {
                string key = context.Request.QueryString["Keyword"];
                sql = "SELECT No,Name  FROM Port_Emp WHERE  FK_Duty='0" + key + "' ";
                if (key.Equals("all"))
                {
                    sql = "SELECT No,Name  FROM Port_Emp WHERE  1=1";
                }
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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