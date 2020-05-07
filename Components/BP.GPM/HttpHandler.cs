using BP.Sys;

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
            BP.GPM.Emp emp = new BP.GPM.Emp();
            emp.No = this.GetRequestVal("TB_No");
            if (emp.RetrieveFromDBSources() == 1)
                return "err@用户名已经存在.";

            //从Request对象中复制数据.
            PubClass.CopyFromRequest(emp);

            //emp.Name = this.GetRequestVal("TB_Name");
            //emp.FK_Dept = this.GetRequestVal("DDL_FK_Dept");
            //emp.Email = this.GetRequestVal("TB_Email");
            //emp.Pass = this.GetRequestVal("TB_PW");
            //emp.Tel = this.GetRequestVal("TB_Tel");
            emp.Insert();

            return "注册成功";
        }
        #endregion xxx 界面方法.

    }
}
