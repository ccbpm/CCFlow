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
    public class CCMobile_WorkOpt_OneWork : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_WorkOpt_OneWork(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region xxx 界面 .
        public string TimeBase_Init()
        {
            WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork(this.context);
            return en.TimeBase_Init();
        }
        /// <summary>
        /// 执行撤销操作.
        /// </summary>
        /// <returns></returns>
        public string TimeBase_UnSend()
        {
            WF_WorkOpt_OneWork en = new WF_WorkOpt_OneWork(this.context);
            return en.OP_UnSend();
        }
        #endregion xxx 界面方法.

    }
}
