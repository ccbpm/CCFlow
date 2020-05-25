using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.DataUser
{
    /// <summary>
    /// HandlerAPI 的摘要说明
    /// 1. 遵守gener.js 的开发规范.
    /// 2. 返回err@xxx就是错误信息，返回info@xxx输出执行信息. 其他的返回就是json格式.
    /// </summary>
    public class HandlerAPI : IHttpHandler
    {
        public HttpContext context = null;
        public void ProcessRequest(HttpContext _context)
        {
            context = _context;
            string doType = context.Request.QueryString["DoType"];

            try
            {
                switch (doType)
                {
                    case "CC_BatchCheckOver": //批量审核
                        this.CC_BatchCheckOver();
                        break;
                    default:
                        break;
                }
            }catch(Exception ex)
            {
                this.Output("err@错误：执行标记DoType=" + doType + "," + ex.Message);
            }
        }
        /// <summary>
        /// 查看工作
        /// </summary>
        /// <returns></returns>
        public void CC_BatchCheckOver()
        {
            string workids = this.GetValString("WorkIDs");
            string str = BP.WF.Dev2Interface.Node_CC_SetCheckOver(workids);
            Output(str);
        }

        public void Output(string info)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(info);
        }

        #region 参数.
        public string FlowNo
        {
            get
            {
                string flow = this.context.Request.QueryString["FK_Flow"];
                if (BP.DA.DataType.IsNullOrEmpty(flow)==true)
                    flow = this.context.Request.QueryString["FlowNo"];
                return flow;
            }
        }
        public Int64 WorkID
        {
            get
            {
                string var = this.context.Request.QueryString["WorkID"];
                if (BP.DA.DataType.IsNullOrEmpty(var) == true)
                    var = this.context.Request.QueryString["WorkID"];
                return Int64.Parse(var);
            }
        }
        public int FK_Node
        {
            get
            {
                string var = this.context.Request.QueryString["FK_Node"];
                if (BP.DA.DataType.IsNullOrEmpty(var) == true)
                    var = this.context.Request.QueryString["NodeID"];
                return int.Parse(var);
            }
        }
        public string GetValString(string key)
        {
            return this.context.Request.QueryString[key];
        }
        public int GetValInt(string key)
        {
            return int.Parse( this.context.Request.QueryString[key]);
        }
        public Int64 GetValInt64(string key)
        {
            return Int64.Parse(this.context.Request.QueryString[key]);
        }
        public float GetValFloat(string key)
        {
            return float.Parse(this.context.Request.QueryString[key]);
        }
        #endregion 参数.


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}