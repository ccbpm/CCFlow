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
using BP.GPM.Menu2020;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_GPM_CreateMenu : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_GPM_CreateMenu()
        {

        }
        /// <summary>
        /// 创建独立流程
        /// </summary>
        /// <returns></returns>
        public string StandAloneFlow_Save()
        {
            //首先创建流程. 参数都通过 httrp传入了。
            BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel handler = new WF_Admin_CCBPMDesigner_FlowDevModel();
            string flowNo = handler.FlowDevModel_Save();
            return flowNo;
        }
        /// <summary>
        /// 模板复制
        /// </summary>
        /// <returns></returns>
        public string Menus_DictCopy()
        {
           // BP.CCBill.FrmDict en = new CCBill.FrmDict();
           // en.doC
            return "复制成功.";
        }
    }
}
