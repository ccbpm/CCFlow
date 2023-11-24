using System.Security.Cryptography;
using System;
using BP.En;
using BP.DA;
using System.Configuration;
using BP.Web;

namespace BP.Port
{
    /// <summary>
    /// webUser 的副本
    /// </summary>
    public class WebUserCopy
    {
        #region 属性.
        public string No = "";
        public string Name = "";
        public string DeptNo = "";
        public string DeptName = "";
        public string OrgNo = "";
        public string OrgName = "";
        public string Token = "";
        public string Auth = "";
        public string AuthName = "";
        public bool IsAuthorize = false;
        #endregion 属性.

        /// <summary>
        /// webUser 的副本
        /// </summary>
        public WebUserCopy()
        { 
        }
        /// <summary>
        /// 按人员初始化数据
        /// </summary>
        /// <param name="emp"></param>
        public void LoadEmpEntity(Emp emp)
        {
            this.No = emp.No;
            this.Name = emp.Name;
            this.DeptNo = emp.DeptNo;
            this.DeptName = emp.DeptText;
            this.OrgNo = emp.OrgNo;
            this.OrgName = emp.OrgNo;
          //  this.Token = emp.Token;
        }
        public void LoadEmpNo(string empNo)
        {
           Emp emp = new Emp(empNo);
            LoadEmpEntity(emp);
        }
        /// <summary>
        /// 按webUser人员初始化数据
        /// </summary>
        /// <param name="emp"></param>
        public void LoadWebUser()
        {
            this.No = WebUser.No;
            this.Name = WebUser.Name;
            this.DeptNo = WebUser.DeptNo;
            this.DeptName = WebUser.DeptName;
            this.OrgNo = WebUser.OrgNo;
            this.OrgName = WebUser.OrgName;
            this.Token = WebUser.Token;

            this.IsAuthorize = WebUser.IsAuthorize;
            this.Auth = WebUser.Auth;
            this.AuthName=WebUser.AuthName;

        }
    }
}
