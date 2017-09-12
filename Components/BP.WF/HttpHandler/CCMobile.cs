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
    public class CCMobile : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile(HttpContext mycontext)
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
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        public string Login_Init()
        {
            BP.WF.HttpHandler.WF_App_ACE ace = new WF_App_ACE(this.context);
            return ace.Login_Init();
        }

        public string Home_Init()
        {

            BP.WF.HttpHandler.WF_App_ACE ace = new WF_App_ACE(this.context);
            return ace.Home_Init();
        }

        public string Runing_Init()
        {
            BP.WF.HttpHandler.WF wfPage = new WF(this.context);
          return  wfPage.Runing_Init();
        }

        public string Todolist_Init()
        {
            BP.WF.HttpHandler.WF wfPage = new WF(this.context);
            return wfPage.Todolist_Init();
        }

        public string Start_Init()
        {
            BP.WF.HttpHandler.WF wfPage = new WF(this.context);
            return wfPage.Start_Init();
        }
    }
}
