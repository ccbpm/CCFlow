using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class Portal_SaaS : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Portal_SaaS()
        {
        }

      



        #region  界面 .
        public string AccepterRole_Init()
        {
            return "方法未完成";
        }
        public string Student_JiaoNaXueFei()
        {
            string no = this.GetRequestVal("No");
            string name = this.GetRequestVal("Name");
            string note = this.GetRequestVal("Note");
            var jine = this.GetRequestValFloat("JinE");
             

            return "学费缴纳成功["+no+"]["+name+"]["+note+"]["+jine+"]";

        }
        #endregion 界面方法.

    }
}
