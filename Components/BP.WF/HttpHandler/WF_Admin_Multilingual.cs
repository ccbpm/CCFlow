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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Multilingual : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_Multilingual(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Multilingual()
        {
        }
        /// <summary>
        /// 获得使用的语言.
        /// </summary>
        /// <returns>使用的语言</returns>
        public string GetLangue()
        {
            Hashtable ht = new Hashtable();

            if (SystemConfig.IsMultilingual == true)
                ht.Add("IsMultilingual", "1");
            else
                ht.Add("IsMultilingual", "0");

            ht.Add("Langue", SystemConfig.Langue);
            return BP.Tools.Json.ToJson(ht);
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

        #region ccform
        /// <summary>
        /// 表单的配置
        /// </summary>
        /// <returns></returns>
        public string CCForm_Init()
        {
            return "";
        }
        #endregion xxx 界面方法.

    }
}
