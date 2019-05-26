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

        #region 关联单据.
        /// <summary>
        /// 设置父子关系.
        /// </summary>
        /// <returns></returns>
        public string RefBill_Done()
        {
            string frmID = this.GetRequestVal("FrmID");
            Int64 workID = this.GetRequestValInt64("WorkID");
            GERpt rpt = new GERpt(frmID, workID);

            string pFrmID = this.GetRequestVal("PFrmID");
            Int64 pWorkID = this.GetRequestValInt64("PWorkID");

            //把数据copy到当前的子表单里.
            GERpt rptP = new GERpt(pFrmID, pWorkID);
            rpt.Copy(rptP);
            rpt.PWorkID = pWorkID;
            rpt.SetValByKey("PFrmID", pFrmID);
            rpt.Update();

            //更新控制表,设置父子关系.
            GenerBill gbill = new GenerBill(workID);
            gbill.PFrmID = pFrmID;
            gbill.PWorkID = pWorkID;
            gbill.Update();
            return "执行成功";
        }
        public string RefBill_Init()
        {
            return "";
        }
        #endregion 关联单据.


    }
}
