using System;
using System.Collections.Generic;
using System.Collections;
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
using BP.WF.Data;
using BP.WF.HttpHandler;

namespace BP.Frm
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Opt : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_CCBill_Opt(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Opt()
        {
        }
        #endregion 构造方法.

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string RefBill_SetBill()
        {
            string frmID = this.GetRequestVal("FrmID");
            Int64 workID = this.GetRequestValInt64("WorkID");

            string pFrmID = this.GetRequestVal("PFrmID");
            Int64 pWorkID = this.GetRequestValInt64("PWorkID");


            return "执行成功";
        }

        public string RefBill_Init()
        {
            return "";
        }

    }
}
