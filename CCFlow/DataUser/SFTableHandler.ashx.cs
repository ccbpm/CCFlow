using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web;
using System.Web.SessionState;//第一步：导入此命名空间

namespace CCBPM.DataUser
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string doType = context.Request.QueryString["DoType"];

            string json="";
            if (doType == "Demo_HandlerEmps")
                json = Demo_HandlerEmps();

            if (doType == "Demo_HandlerDepts")
                json = Demo_HandlerDepts();

            if (doType == "Handler_CPLX")
            {
                return "SDSSSSSS";
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string Demo_HandlerEmps()
        {
            BP.Port.Emps emps = new BP.Port.Emps();
            emps.RetrieveAll();
            return emps.ToJson();
        }
        public string Demo_HandlerDepts()
        {
            BP.Port.Depts depts = new BP.Port.Depts();
            depts.RetrieveAll();
            return depts.ToJson();
        }
    }
}