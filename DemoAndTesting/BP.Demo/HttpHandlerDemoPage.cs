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
using BP.Demo.BPFramework;
using BP.WF.HttpHandler;

namespace BP.Demo
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class HttpHandlerDemoPage : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public HttpHandlerDemoPage(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpHandlerDemoPage()
        {
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

        /// <summary>
        /// 学生列表.
        /// </summary>
        /// <returns></returns>
        public string StudentList_Init()
        {
            Students ens = new Students();
            QueryObject qo = new QueryObject(ens);
            qo.DoQuery(StudentAttr.No, this.PageSize, this.PageIdx);
            return ens.ToJson();
        }
        public string StudentList_PageBar()
        {

            Hashtable ht = new Hashtable();

            Students ens = new Students();
            QueryObject qo = new QueryObject(ens);

            ht.Add("Count", qo.GetCount());
            ht.Add("PageIdx", this.PageIdx);

            return  BP.Tools.Json.ToJson(ht);
        }
    }
}
