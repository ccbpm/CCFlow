using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.DataUser.OverrideFiles
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string doType = context.Request.QueryString["DoType"];
            if (doType != null && doType.Equals("Timer") == true)
            {
                string msg = this.Timer();
                context.Response.Write(msg);
                return;
            }
        }
        /// <summary>
        /// 定期执行的任务比如:
        /// 1, 时效考核的逾期.
        /// 2, WF_GenerWorkFlow 的TSpan 字段的更新.
        /// </summary>
        /// <returns></returns>
        public string Timer()
        {
            #region 更新待办状态.正常,逾期,预警.
            string info="";
            try
            {
                BP.WF.DTS.DTS_GenerWorkFlowTodoSta ts = new BP.WF.DTS.DTS_GenerWorkFlowTodoSta();
              info+=  ts.Do();
            }
            catch (Exception ex)
            {
                info+= "err@" + ex.Message;
            }
            #endregion 更新待办状态.正常,逾期,预警.


            #region 更新时间戳
            try
            {
                BP.WF.DTS.DTS_GenerWorkFlowTimeSpan timeSpan = new BP.WF.DTS.DTS_GenerWorkFlowTimeSpan();
                info += timeSpan.Do();
            }
            catch (Exception ex)
            {
                info += "err@" + ex.Message;
            }
            #endregion 更新时间戳.


            #region 删除临时文件
            try
            {
                BP.WF.DTS.DeletTempFiles delTempFiles = new BP.WF.DTS.DeletTempFiles();
                info += delTempFiles.Do();
            }
            catch (Exception ex)
            {
                info += "err@" + ex.Message;
            }
            #endregion 删除临时文件.

            #region 处理逾期流程.
            try
            {
                BP.WF.DTS.AutoRunOverTimeFlow overtime = new BP.WF.DTS.AutoRunOverTimeFlow();
                info += overtime.Do();
            }
            catch (Exception ex)
            {
                info += "err@" + ex.Message;
            }
            #endregion 处理逾期流程.

            return info;

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