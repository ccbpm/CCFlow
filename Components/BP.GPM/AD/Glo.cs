using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing;
using System.Diagnostics.PerformanceData;
using System.Diagnostics.SymbolStore;

using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using BP;

using System.Runtime.InteropServices;   //必要引用
using System.Security.Principal;    //必要引用


namespace BP.GPM.AD
{
    public class Glo
    {
        #region 公共变量.
        public static string ADBasePath
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADBasePath"];
            }
        }
        public static string ADUser
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADUser"];
            }
        }
        public static string ADPassword
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADPassword"];
            }
        }
        public static string ADAppRoot
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADAppRoot"];
            }
        }
        /// <summary>
        /// 跟目录(主域)
        /// </summary>
        public static DirectoryEntry DirectoryEntryBasePath
        {
            get
            {
                DirectoryEntry domain = new DirectoryEntry();

                domain.Path = Glo.ADBasePath;
                domain.Username = Glo.ADUser;
                domain.Password = Glo.ADPassword;

                //domain.AuthenticationType = AuthenticationTypes.ReadonlyServer;

                domain.RefreshCache();
                return domain;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static DirectoryEntry DirectoryEntryAppRoot
        {
            get
            {
                DirectorySearcher search = new DirectorySearcher(Glo.DirectoryEntryBasePath); //查询组织单位.
                search.Filter = "(OU=" + Glo.ADAppRoot + ")";
                search.SearchScope = SearchScope.Subtree;

                SearchResult result = search.FindOne();
                DirectoryEntry de = result.GetDirectoryEntry();
                search.Dispose();

                return de;
            }
        }
        #endregion 公共变量.

        #region 相关方法.
         
        public static string GetPropertyValue(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 登录校验相关.
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser11(string lpszUsername, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);


        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        private static extern int LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider,
                                          ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private extern static int DuplicateToken(IntPtr hToken,
                                          int impersonationLevel,
                                          ref IntPtr hNewToken);
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private static WindowsImpersonationContext impersonationContext = null;
        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="userNo"></param>
        /// <param name="pass"></param>
        public static bool CheckLogin(string domain, string userNo, string pass)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (LogonUser(userNo, domain, pass, LOGON32_LOGON_INTERACTIVE,
            LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                    if (impersonationContext != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion 登录校验相关.

    }
}
