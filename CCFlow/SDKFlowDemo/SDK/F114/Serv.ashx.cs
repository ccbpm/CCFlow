using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.WF;
using BP.Web;
namespace CCFlow.SDKFlowDemo.SDK.F114
{
    /// <summary>
    /// Serv 的摘要说明
    /// </summary>
    public class Serv : IHttpHandler
    {
        #region 参数.
        /// <summary>
        /// 封装有关个别 HTTP 请求的所有 HTTP 特定的信息
        /// </summary>
        HttpContext MyContext = null;
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(MyContext.Request[param], System.Text.Encoding.UTF8);
        }
        public string DoFunc
        {
            get
            {
                return getUTF8ToString("DoFunc");
            }
        }
        public string DoType
        {
            get
            {
                return getUTF8ToString("DoType");
            }
        }
        public string CFlowNo
        {
            get
            {
                return getUTF8ToString("CFlowNo");
            }
        }
        public string WorkIDs
        {
            get
            {
                return getUTF8ToString("WorkIDs");
            }
        }
        public string FK_Flow
        {
            get
            {
                return getUTF8ToString("FK_Flow");
            }
        }
        public int FK_Node
        {
            get
            {
                string fk_node = getUTF8ToString("FK_Node");
                if (!DataType.IsNullOrEmpty(fk_node))
                    return Int32.Parse(getUTF8ToString("FK_Node"));
                return 0;
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(getUTF8ToString("WorkID"));
            }
        }
        #endregion 参数.

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            if (BP.Web.WebUser.No == null)
                return;

            string result = "";
            MyContext = context;
            string doType = MyContext.Request["DoType"].ToString();
            if (DataType.IsNullOrEmpty(doType) == true)
                doType = "Save";

            switch (doType)
            {
                case "Send"://发送
                    result = Send();
                    break;
                case "Save"://保存
                    result = Save();
                    break;
                default:
                    break;
            }

            //输出.
            this.OutHtml(result);
        }
        public void OutHtml(string msg)
        {
            //组装ajax字符串格式,返回调用客户端
            MyContext.Response.Charset = "UTF-8";
            MyContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            MyContext.Response.ContentType = "text/html";
            MyContext.Response.Expires = 0;
            MyContext.Response.Write(msg);
            MyContext.Response.End();
        }
        private string Save()
        {
            try
            {
                //BP.Demo.ND112Rpt rpt = new BP.Demo.ND112Rpt(this.WorkID);
                //rpt = PubClass.CopyFromRequest(rpt) as BP.Demo.ND112Rpt;
                //rpt.Update();
                return "保存成功...";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 发送案件 
        /// </summary>
        /// <returns></returns>
        private string Send()
        {
            //首先执行保存.
            this.Save();
            string resultMsg = "";
            try
            {
                if (Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No) == false)
                {
                    resultMsg = "error|您好：" + BP.Web.WebUser.No + ", " + WebUser.Name + "当前的工作已经被处理，或者您没有执行此工作的权限。";
                }
                else
                {
                    SendReturnObjs returnObjs = Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
                  //  resultMsg = returnObjs.ToMsgOfHtml();
                    resultMsg = returnObjs.ToMsgOfHtml();
                     
                    #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                    /*这里有两种情况
                     * 1，从中间的节点，通过批量处理，也就是说合并审批处理的情况，这种情况子流程需要执行到下一步。
                       2，从流程已经完成，或者正在运行中，也就是说合并审批处理的情况. */
                    try
                    {
                        //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                        BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs);
                    }
                    catch (Exception ex)
                    {
                        resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
                        return resultMsg;
                    }
                    #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                }
            }
            catch (Exception ex)
            {
                resultMsg = "sysError|" + ex.Message.Replace("@", "<br/>");
            }
            return resultMsg;
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