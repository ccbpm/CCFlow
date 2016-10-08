using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP.Rpt;
using BP.DA;
using BP.En;
using BP.Port;

namespace CCFlow.WF.Comm.Rpt
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context = null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = context.Request.QueryString["MyPK"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                string str = context.Request.QueryString["EnumKey"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 实体 EnsName
        /// </summary>
        public string EnsName
        {
            get
            {
                string str = context.Request.QueryString["EnsName"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        public string RptName
        {
            get
            {
                string str = context.Request.QueryString["RptName"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion 执行.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "InitPie": //获得枚举列表的JSON.
                        BP.Rpt.Rpt2Base rpt = BP.En.ClassFactory.GetRpt2Base(this.RptName);
                        Rpt2Attr attr = rpt.AttrsOfGroup.GetD2(0);
                        //转化成json.
                        msg= attr.ToJson();

                        break;
                    default:
                        msg = "err@没有判断的标记:" + this.DoType;
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
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