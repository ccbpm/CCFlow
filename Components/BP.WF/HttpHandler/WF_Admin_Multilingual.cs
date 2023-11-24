using System;
using System.Collections;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Multilingual : DirectoryPageBase
    {
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

            if (BP.Difference.SystemConfig.isMultilingual == true)
                ht.Add("IsMultilingual", "1");
            else
                ht.Add("IsMultilingual", "0");

            ht.Add("Langue", BP.Difference.SystemConfig.Langue);
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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
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
