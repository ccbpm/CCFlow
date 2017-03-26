using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_CCFormDesigner_DialogCtr : WebContralBase
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner_DialogCtr(HttpContext mycontext)
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

        #region 功能界面 .
        /// <summary>
        /// 转化拼音
        /// </summary>
        /// <returns>返回转换后的拼音</returns>
        public string FrmTextBox_ParseStringToPinyin()
        {
            string name = getUTF8ToString("name");
            string flag = getUTF8ToString("flag");

            if (flag == "true")
                return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true);

            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, false);
        }
        #endregion 功能界面方法.
    }
}
