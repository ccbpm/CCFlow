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
        public static string ADPath
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADPath"];
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
        public static string ADRoot
        {
            get
            {
                return Sys.SystemConfig.AppSettings["ADRoot"];
            }
        }
        /// <summary>
        /// 跟目录(主域)
        /// </summary>
        public static DirectoryEntry RootDirectoryEntry
        {
            get
            {
                DirectoryEntry domain = new DirectoryEntry();

                domain.Path = Glo.ADRoot;
                domain.Username = Glo.ADUser;
                domain.Password = Glo.ADPassword;

                //domain.AuthenticationType = AuthenticationTypes.ReadonlyServer;

                domain.RefreshCache();
                return domain;

            }
        }
        #endregion 公共变量.

        #region 相关方法.
        /// <summary>
        /// 根据查询字符串取得 DirectoryEntry 对象实例
        /// </summary>
        /// <param name="filterString"></param>
        /// <returns>如果找到该对象，则返回用户的 DirectoryEntry 对象；否则返回 null</returns>
        public static DirectoryEntry FindObject(string filterString)
        {
            DirectoryEntry dirEntry = RootDirectoryEntry;
            DirectorySearcher deSearch = new DirectorySearcher(dirEntry);
            deSearch.SearchScope = SearchScope.Subtree;
            deSearch.Filter = filterString;
            try
            {
                SearchResult result = deSearch.FindOne();
                dirEntry = result.GetDirectoryEntry();
                return dirEntry;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 根据查询字符串取得所有的 DirectoryEntry 对象实例
        /// </summary>
        /// <param name="rootDirEntry"></param>
        /// <param name="filterString">查询字符串</param>
        /// <returns>所有的 DirectoryEntry 对象</returns>
        public static DirectoryEntry[] FindObjects(string filterString)
        {
            DirectorySearcher deSearch = new DirectorySearcher(Glo.RootDirectoryEntry);
            deSearch.Filter = filterString;
            deSearch.SearchScope = SearchScope.Subtree;
            try
            {
                SearchResultCollection results = deSearch.FindAll();
                int i = 0, count = results.Count;
                DirectoryEntry[] resultDirs = new DirectoryEntry[count];
                foreach (SearchResult e in results)
                {
                    resultDirs[i++] = e.GetDirectoryEntry();
                }
                return resultDirs;
            }
            catch
            {
                return null;
            }
        }
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
