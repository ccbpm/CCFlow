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
    public class WF_Admin_AttrNode_AccepterRole : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_AttrNode_AccepterRole()
        {
        }

        #region  界面 .
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public string AccepterRole_ClearStartFlowsCash()
        {
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows=''");
            else
                DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows='' WHERE OrgNo='" + BP.Web.WebUser.OrgNo + "'");
            return "执行成功 ";
        }
        /// <summary>
        /// 清楚所有组织的缓存,用于多组织.
        /// </summary>
        /// <returns></returns>
        public string AccepterRole_ClearAllOrgStartFlowsCash()
        {
            DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows=''");
            return "执行成功 ";
        }
        #endregion 界面方法.

    }
}
