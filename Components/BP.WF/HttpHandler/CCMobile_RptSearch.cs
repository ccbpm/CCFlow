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
    public class CCMobile_RptSearch : DirectoryPageBase
    {
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

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_RptSearch(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_RptSearch()
        {
        }

        #region 关键字查询.
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string KeySearch_OpenFrm()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch(this.context);
            return search.KeySearch_OpenFrm();
        }
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <returns></returns>
        public string KeySearch_Query()
        {
            BP.WF.HttpHandler.WF_RptSearch search = new WF_RptSearch(this.context);
            return search.KeySearch_Query();
        }
        #endregion 关键字查询.

    }
}
