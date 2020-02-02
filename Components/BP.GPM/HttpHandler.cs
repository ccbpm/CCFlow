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

namespace BP.GPM
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class HttpHandler : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HttpHandler()
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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region 注册用户- 界面 .
        /// <summary>
        /// 注册用户.
        /// </summary>
        /// <returns></returns>
        public string RegUser_Init()
        {
            if (Glo.IsEnableRegUser == false)
                return "err@该系统尚未启动注册功能，请通知管理员把全局配置项 IsEnableRegUser 设置为1，没有该项就添加该配置项.";

            //返回部门信息，用与绑定部门.
            Depts ens = new Depts();
            ens.RetrieveAll();
            return ens.ToJson();
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        public string RegUser_Submit()
        {
            Emp emp = new Emp();
            emp.No = this.GetRequestVal("No");
            if (emp.RetrieveFromDBSources() == 1)
                return "err@用户名已经存在.";

            emp.Name = this.GetRequestVal("Name");
            emp.FK_Dept = this.GetRequestVal("DDL_FK_Dept");
            emp.Email = this.GetRequestVal("TB_Email");
            emp.Pass = this.GetRequestVal("TB_PW");
            emp.Insert();
            return "注册成功";
        }
        #endregion xxx 界面方法.

    }
}
