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
    public class WF_Admin_Cond2020 : DirectoryPageBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Cond2020()
        {
        }
        /// <summary>
        /// 初始化列表
        /// </summary>
        /// <returns></returns>
        public string List_Init()
        {
            // Conds condes = new Conds();
            // condes.RetrieveAll();
            return "";
        }

    }
}
