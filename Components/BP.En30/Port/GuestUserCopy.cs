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
    public class GuestUserCopy
    {
        #region 属性.
        public string No = "";
        public string Name = "";
        #endregion 属性.

        /// <summary>
        /// GuestUser 的副本
        /// </summary>
        public GuestUserCopy()
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
            this.No = GuestUser.No;
            this.Name = GuestUser.Name;
        }
    }
}
