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
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.GPM.AD
{
    /// <summary>
    /// 同步AD.
    /// </summary>
    public class Sync : Method
    {
        /// <summary>
        /// 同步
        /// </summary>
        public Sync()
        {
            this.Title = "同步AD数据到组织结构.";
            this.Help = "手工同步数据到组织结构。";
        }
        public override void Init()
        {
        }
        /// <summary>
        /// 是否可以处理？
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No.Equals("admin"))
                    return true;
                return false;
            }
        }

        #region## 同步
        private List<AdModel> list = new List<AdModel>();
        /// <summary>
        /// 功能:
        /// 创建人:Wilson
        /// 创建时间:2012-12-15
        /// </summary>
        /// <param name="entryOU"></param>
        public override object Do()
        {
             DirectorySearcher mySearcher = new DirectorySearcher(Glo.RootDirectoryEntry,"(objectclass=organizationalUnit)"); //查询组织单位.
            // DirectorySearcher mySearcher = new DirectorySearcher(Glo.RootDirectoryEntry, "(objectclass=organizationalUnit)"); //查询组织单位.

            ///DirectorySearcher mySearcher = new DirectorySearcher(Glo.RootDirectoryEntry); //查询组织单位.

            DirectoryEntry root = mySearcher.SearchRoot;   //查找根OU.

            //DirectoryEntry root = Glo.RootDirectoryEntry.Parent;   //查找根OU

            //同步数据.
            SyncRootOU(root);

            string msg = "";
            if (list.Count == 0)
                return "err@没有获得ad的任何数据，无法同步。";

            msg += "@开始删除AD_Dept, AD_Emp数据。";
            //删除现有的数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Dept");
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Emp");

            BP.GPM.AD.Dept dept = new Dept();
            BP.GPM.AD.Emp emp = new Emp();

            return "同步成功";
        }
        #endregion

        #region## 同步根组织单位
        string rootDeptNo = "";
        /// <summary>
        /// 功能: 同步根组织单位
        /// 创建人:Wilson
        /// 创建时间:2012-12-15
        /// </summary>
        /// <param name="entry"></param>
        private void SyncRootOU(DirectoryEntry entry)
        {
               //更目录.
            if (entry.Name.IndexOf("DC=dev") == 0)
            {
                foreach (DirectoryEntry item in entry.Children)
                {
                    SyncRootOU(item);
                }
                return;
            }
            //组织解构,更新跟目录的.
            if (entry.Name.IndexOf("OU=") == 0)
            {
                BP.GPM.AD.Dept dept = new Dept();
                dept.Name = entry.Name.Replace("OU=", "");

                dept.No = entry.Guid.ToString();
                dept.ParentNo = entry.Parent.Guid.ToString();
                dept.Insert();

                foreach (DirectoryEntry item in entry.Children)
                {
                    SyncRootOU(item);
                }
                return;
            }


            //用户.
            if (entry.Name.IndexOf("CN=") == 0)
            {
                BP.GPM.AD.Emp emp = new Emp();

                emp.Name = entry.Name.Replace("CN=", "");

                emp.No = entry.Guid.ToString();
                emp.FK_Dept = entry.Parent.Guid.ToString();
                emp.Insert();

                foreach (DirectoryEntry item in entry.Children)
                {
                     SyncRootOU(item);
                }
                return;
            }

            //if (entry.Properties.Contains("ou") && entry.Properties.Contains("objectGUID"))
            //{
            //    string rootOuName = entry.Properties["ou"][0].ToString();

            //    byte[] bGUID = entry.Properties["objectGUID"][0] as byte[];

            //    string id = BitConverter.ToString(bGUID);

            //    list.Add(new AdModel(id, rootOuName, (int)TypeEnum.OU, "0"));

            //    SyncSubOU(entry, id);
            //}
        }

        public void SyncRootOU(DirectoryEntry en, string parentEn)
        {

        }
        #endregion

        #region## 同步下属组织单位及下属用户
        /// <summary>
        /// 功能: 同步下属组织单位及下属用户
        /// 创建人:Wilson
        /// 创建时间:2012-12-15
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="parentId"></param>
        private void SyncSubOU(DirectoryEntry entry, string parentId)
        {
            foreach (DirectoryEntry subEntry in entry.Children)
            {
                string entrySchemaClsName = subEntry.SchemaClassName;

                string[] arr = subEntry.Name.Split('=');
                string categoryStr = arr[0];
                string nameStr = arr[1];
                string id = string.Empty;

                if (subEntry.Properties.Contains("objectGUID"))   //SID
                {
                    byte[] bGUID = subEntry.Properties["objectGUID"][0] as byte[];

                    id = BitConverter.ToString(bGUID);
                }

                bool isExist = list.Exists(d => d.Id == id);

                switch (entrySchemaClsName)
                {
                    case "organizationalUnit":

                        if (!isExist)
                        {
                            list.Add(new AdModel(id, nameStr, (int)TypeEnum.OU, parentId));
                        }

                        SyncSubOU(subEntry, id);
                        break;
                    case "user":
                        string accountName = string.Empty;

                        if (subEntry.Properties.Contains("samaccountName"))
                        {
                            accountName = subEntry.Properties["samaccountName"][0].ToString();
                        }

                        if (!isExist)
                        {
                            list.Add(new AdModel(id, accountName, (int)TypeEnum.USER, parentId));
                        }
                        break;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 类型
    /// </summary>
    public enum TypeEnum
    {
        /// <summary>
        /// 组织单位
        /// </summary>
        OU = 1,
        /// <summary>
        /// 用户
        /// </summary>
        USER = 2
    }
    /// <summary>
    /// Ad域信息实体
    /// </summary>
    public class AdModel
    {
        public AdModel(string id, string name, int typeId, string parentId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
            ParentId = parentId;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public string ParentId { get; set; }
    }
}
