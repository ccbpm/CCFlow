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
    public class WF_AppClassic : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_AppClassic(HttpContext mycontext)
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

        #endregion xxx 界面方法.

        /// <summary>
        /// 初始化Home
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Home_Init();
        }
        public string Index_Init()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Index_Init();
        }

        #region 登录界面.
        public string Login_Init()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Login_Init();
        }

        public string Login_Out()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return "安全退出.";
        }

        public string Login_Submit()
        {
            WF_App_ACE page = new WF_App_ACE(context);
            return page.Login_Submit();
        }
        #endregion 登录界面.

    }
}
