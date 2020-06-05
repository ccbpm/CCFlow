using System;
using System.Collections.Generic;
using BP.WF;
using BP.WF.Template;
using BP.DA;
using BP.Web;
using System.Web;
using System.Web.SessionState;

namespace CCFlow.DataUser
{
    /// <summary>
    /// HandlerAPI 的摘要说明
    /// 1. 遵守gener.js 的开发规范.
    /// 2. 返回err@xxx就是错误信息，返回info@xxx输出执行信息. 其他的返回就是json格式.
    /// </summary>
    public class HandlerAPI : IHttpHandler, IRequiresSessionState, IReadOnlySessionState
    {
        public HttpContext context = null;
        public void ProcessRequest(HttpContext _context)
        {
            context = _context;

            //让其支持跨域访问.
            string origin = _context.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin))
            {
                var allAccess_Control_Allow_Origin = System.Web.Configuration.WebConfigurationManager.AppSettings["Access-Control-Allow-Origin"];
                _context.Response.Headers["Access-Control-Allow-Origin"] = origin;
                _context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                _context.Response.Headers["Access-Control-Allow-Headers"] = "x-requested-with,content-type";
            }

            string doType = context.Request.QueryString["DoType"];
            try
            {
                switch (doType)
                {
                    case "CC_BatchCheckOver": //批量抄送审核.
                        this.CC_BatchCheckOver();
                        break;
                    case "Flow_BatchDeleteByFlag": //批量删除.
                        this.Flow_BatchDeleteByFlag();
                        break;
                    case "Flow_BatchDeleteByReal": //批量删除.
                        this.Flow_BatchDeleteByReal();
                        break;
                    case "Flow_BatchDeleteByFlagAndUnDone": //恢复批量删除.
                        this.Flow_BatchDeleteByFlagAndUnDone();
                        break;
                    case "Flow_DoUnSend": //撤销发送..
                        this.Flow_DoUnSend();
                        break;
                    case "Flow_DeleteDraft": //删除草稿箱..
                        this.Flow_DeleteDraft();
                        break;
                    case "Flow_DoFlowOver": //批量结束.
                        this.Flow_DoFlowOver();
                        break;
                    case "Flow_NeiFa": //个性化:转内发.
                        this.Flow_NeiFa();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Output("err@错误：执行标记DoType=" + doType + "," + ex.Message);
            }
        }
        /// <summary>
        /// 转内发.
        /// </summary>
        public void Flow_NeiFa()
        {
            BP.DOC.App_Page page = new BP.DOC.App_Page();
            int WorkID = Convert.ToInt32(this.WorkID);
            string str=page.NeiFa_Init_To(WorkID);
            Output(str);
        }
        public void Flow_DoFlowOver()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                BP.WF.Dev2Interface.Flow_DoFlowOver( Int64.Parse(workidStr),"批量结束",1);
            }
            Output("删除成功.");
        }
        /// <summary>
        /// 删除草稿
        /// </summary>
        public void Flow_DeleteDraft()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                 BP.WF.Dev2Interface.Node_DeleteDraft(Int64.Parse(workidStr));
            }
            Output("删除成功.");
        }
        
        /// <summary>
        /// 撤销发送
        /// </summary>
        public void Flow_DoUnSend()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                info += BP.WF.Dev2Interface.Flow_DoUnSend(null, Int64.Parse(workidStr));
            }
            Output(info);
        }
        public void Flow_BatchDeleteByReal()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(Int64.Parse(workidStr),true);
            }

            Output("删除成功.");
        }
        
        /// <summary>
        /// 删除功能
        /// </summary>
        public void Flow_BatchDeleteByFlag()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(null, Int64.Parse(workidStr), "删除", true);
            }

            Output("删除成功.");
        }
        /// <summary>
        /// 恢复删除功能
        /// </summary>
        public void Flow_BatchDeleteByFlagAndUnDone()
        {
            string workids = this.GetValString("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoUnDeleteFlowByFlag(null,int.Parse(workidStr), "删除");
            }

            Output("恢复成功.");
        }
        /// <summary>
        /// 查看工作
        /// </summary>
        /// <returns></returns>
        public void CC_BatchCheckOver()
        {
            string workids = this.GetValString("WorkIDs");
            string str = BP.WF.Dev2Interface.Node_CC_SetCheckOverBatch(workids);
            Output(str);
        }

        public void Output(string info)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Headers.Add( = "Access - Control - Allow - Origin: *";
            context.Response.Write(info);
        }

        #region 参数.
        public string MyPK
        {
            get
            {
                string flow = this.context.Request.QueryString["MyPK"];
                if (BP.DA.DataType.IsNullOrEmpty(flow) == true)
                    flow = this.context.Request.QueryString["PK"];
                return flow;
            }
        }
        public string FlowNo
        {
            get
            {
                string flow = this.context.Request.QueryString["FK_Flow"];
                if (BP.DA.DataType.IsNullOrEmpty(flow) == true)
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
            return int.Parse(this.context.Request.QueryString[key]);
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