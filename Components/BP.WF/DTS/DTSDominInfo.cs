using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
// using Security.Principal.WindowsIdentity;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class DTSDominInfo : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DTSDominInfo()
        {
            this.Title = "生成域数据";
            this.Help = "生成域数据(未完成)";
           // this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            return "功能未实现。";

            string domainHost = "127.0.0.1";

            string sqls = "";
            sqls += "@DELETE FROM Port_Emp";
            sqls += "@DELETE FROM Port_Dept";
            sqls += "@DELETE FROM Port_Station";
            sqls += "@DELETE FROM Port_DeptEmpStation";
            DBAccess.RunSQLs(sqls);

           
            // 把部门导入里面去。

            //DirectoryEntry de = new DirectoryEntry("LDAP://" + domain, name, pass);
            //DirectorySearcher srch = new DirectorySearcher();
            //srch.Filter = ("(objectclass=User)");

            //srch.SearchRoot = de;
            //srch.SearchScope = SearchScope.Subtree;
            //srch.PropertiesToLoad.Add("sn");
            //srch.PropertiesToLoad.Add("givenName");
            //srch.PropertiesToLoad.Add("uid");
            //srch.PropertiesToLoad.Add("telephoneNumber");
            //srch.PropertiesToLoad.Add("employeeNumber");
            //foreach (SearchResult res in srch.FindAll())
            //{
            //    string[] strArray;
            //    string str;
            //    str = "";
            //    strArray = res.Path.Split(',');
            //    for (int j = strArray.Length; j > 0; j--)
            //    {
            //        if (strArray[j - 1].Substring(0, 3) == "OU=")
            //        {
            //            str = "└" + strArray[j - 1].Replace("OU=", "");
            //        }
            //    }
            //}

            return "生成成功，请打开 。<br>如果您想共享出来请压缩后发送到template＠ccflow.org";
        }
    }
}
