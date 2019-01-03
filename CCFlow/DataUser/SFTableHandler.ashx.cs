using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCBPM.DataUser
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string doType = context.Request.QueryString["DoType"];

            string json="";
            if (doType == "Demo_HandlerEmps")
                json = Demo_HandlerEmps();

            if (doType == "Demo_HandlerDepts")
                json = Demo_HandlerDepts();

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