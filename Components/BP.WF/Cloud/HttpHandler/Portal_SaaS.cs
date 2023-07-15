using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Difference;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using System.Collections;

namespace BP.Cloud.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class Portal_SaaS : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Portal_SaaS()
        {
        }
        public string CheckEncryptEnable()
        {
            if (SystemConfig.IsEnablePasswordEncryption == true)
                return "1";
            return "0";
        }
        /// <summary>
        /// 获取组织
        /// </summary>
        /// <returns></returns>
        public string SelectOnOrg_Init() 
        {
            BP.Cloud.Orgs orgs = new BP.Cloud.Orgs();
            orgs.RetrieveAll();
            DataTable dt = orgs.ToDataTableField("Orgs");
            return BP.Tools.Json.ToJson(dt);
        }

        public string Login_Submit()
        {
            try
            {
                string orgNo = this.OrgNo;
                string userNo = this.GetRequestVal("TB_No");
                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");
                pass = pass.Trim();
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = this.OrgNo + "_" + userNo;
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@用户名["+userNo+"]不存在.";

                if (emp.CheckPass(pass) == false)
                    return "err@密码错误.";

                //BP.Cloud.Emp emp2 = new BP.Cloud.Emp(emp.No);
                BP.WF.Dev2Interface.Port_Login(userNo,this.OrgNo);
                string token = BP.WF.Dev2Interface.Port_GenerToken();
                return WebUser.ToJson();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public string Login_SubmitSaaSOption()
        {
            try
            {
                string userNo = this.GetRequestVal("TB_No");
                if (DataType.IsNullOrEmpty(userNo)==true)
                    return "err@账号不能为空.";

                if (userNo.ToLower().Equals("admin")==true)
                    return "err@请登录admin后台.";

                string pass = this.GetRequestVal("TB_PW");
                if (pass == null)
                    pass = this.GetRequestVal("TB_Pass");
                pass = pass.Trim();
                BP.Cloud.Emps emps = new BP.Cloud.Emps();
                if (emps.Retrieve(EmpAttr.UserID, userNo) == 0)
                    return "err@用户名[" + userNo + "]不存在.";

                BP.Cloud.Emp myemp = emps[0] as BP.Cloud.Emp;

                //检查密码
                BP.Port.Emp emp1 = new BP.Port.Emp();
                emp1.No = myemp.No;
                emp1.RetrieveFromDBSources();
                if (emp1.CheckPass(pass) == false)
                    return "err@密码错误.";

                //BP.Cloud.Emp emp2 = new BP.Cloud.Emp(emp.No);
                BP.WF.Dev2Interface.Port_Login(userNo, myemp.OrgNo);
                string token = BP.WF.Dev2Interface.Port_GenerToken();
                return WebUser.ToJson();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
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

        #region xxx 界面 .
        #endregion xxx 界面方法.

    }
}
