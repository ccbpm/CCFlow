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

        string msg = "";
        /// <summary>
        /// 功能:
        /// 创建人:Wilson
        /// 创建时间:2012-12-15
        /// </summary>
        /// <param name="entryOU"></param>
        public override object Do()
        {
            DirectorySearcher mySearcher = new DirectorySearcher(Glo.RootDirectoryEntry, "(objectclass=organizationalUnit)"); //查询组织单位.
            DirectoryEntry root = mySearcher.SearchRoot;   //查找根OU.
            DirectoryEntry rootAdmin = mySearcher.SearchRoot;   //查找根OU.

            msg += "@开始删除AD_Dept, AD_Emp数据。";

            //删除现有的数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Dept");
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Emp");
            BP.DA.DBAccess.RunSQL("DELETE FROM Port_Station");

            //同步数据.
            SyncRootOU(root);


            //增加admin用户.
            BP.GPM.AD.Dept dept = new Dept();
            dept.Retrieve(BP.GPM.AD.DeptAttr.ParentNo, "0");

            BP.GPM.AD.Emp emp = new Emp();
            emp.No = "admin";
            emp.Name = "admin";
            if (emp.RetrieveFromDBSources() == 0)
            {
                emp.FK_Dept = dept.No;
                emp.Insert();
            }
            else
            {
                emp.FK_Dept = dept.No;
                emp.Update();
            }

            // 同步岗位》

            SyncStatioins();

            return msg;
        }
        #endregion

        public void SyncStatioins()
        {

            DirectorySearcher ds = new DirectorySearcher();
            ds.SearchRoot = Glo.RootDirectoryEntry;
            ds.SearchScope=  SearchScope.Subtree; //搜索全部对象.
            ds.Filter = ("(objectClass=group)");

            //ds.Filter = "(&(objectClass=group)(cn=" + "YBS" + "))";  //YBS组名
            //ds.Filter = ("(objectCategory=YBS)(objectClass=user)") ;
            //. Find("ybs", "Group")) 

            DBAccess.RunSQL("DELETE FROM Port_Station ");
            DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation ");


            foreach (SearchResult result in ds.FindAll())
            {

                //DirectoryEntry deGroup=result.GetDirectoryEntry();
                // string name = result.GetDirectoryEntry().Name.ToString();

                DirectoryEntry deGroup = new DirectoryEntry(result.Path, Glo.ADUser, Glo.ADPassword, AuthenticationTypes.Secure);

                Station sta = new Station();
                sta.No = deGroup.Guid.ToString();
                sta.Name = deGroup.Name;
                sta.Insert();


                System.DirectoryServices.PropertyCollection pcoll = deGroup.Properties;
                int n = pcoll["member"].Count;
                for (int l = 0; l < n; l++)
                {

                    DirectoryEntry deUser = new DirectoryEntry(Glo.ADPath + "/" + pcoll["member"][l].ToString(),
                        Glo.ADUser, Glo.ADPassword, AuthenticationTypes.Secure);
                   
                    BP.GPM.DeptEmpStation des = new DeptEmpStation();
                    des.FK_Dept = deUser.Parent.Guid.ToString();
                    des.FK_Emp = deUser.Name;
                    des.FK_Station =deGroup.Guid.ToString(); //  result.GetDirectoryEntry()
                    des.Insert();

                   // string sss = deUser.Name.ToString() + GetProperty(deUser, "mail");
                    //  Page.Response.Write(sss);
                }
            }

        }

         public string GetProperty(DirectoryEntry oDE, string PropertyName)
        {
            try
            {
                if (oDE.Properties.Contains(PropertyName))
                {
                    return oDE.Properties[PropertyName][0].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
        } 


        public string GetValFromDirectoryEntryByKey(DirectoryEntry en, string key, string isNullAsVal = "")
        {
            if (en.Properties.Contains(key) == false)
                return isNullAsVal;


            PropertyValueCollection valueCollection = en.Properties[key];

            if (valueCollection.Value == null)
                return isNullAsVal;

            
            return valueCollection.Value.ToString();
        }

        #region## 同步根组织单位
        string rootDeptNo = "";
        int idxDept = 0;
        int idxStation = 0;
        int idxEmp = 0;
        /// <summary>
        /// 功能: 同步根组织单位
        /// 创建人:Wilson
        /// 创建时间:2012-12-15
        /// </summary>
        /// <param name="entry"></param>
        private void SyncRootOU(DirectoryEntry entry)
        {
            msg += "<hr>开始同步:" + entry.Name;

            string myInfo="";
            //foreach (string elmentName in entry.Properties.PropertyNames)
            //{
            //    PropertyValueCollection valueCollection = entry.Properties[elmentName];
            //    myInfo += "<br>KEY=" + elmentName + ",   " + valueCollection.Value; // +valueCollection[i].ToString();
            //}
            msg += " 属性：" + myInfo;

            //更目录.
            if (entry.Name.IndexOf("DC=") == 0)
            {
                BP.GPM.AD.Dept dept = new Dept();
                dept.No = entry.Guid.ToString();
                dept.Name = SystemConfig.CustomerShortName;
                if (dept.Name == "")
                    dept.Name = SystemConfig.CustomerName;

                if (dept.Name == "")
                    dept.Name = "总部";

                dept.ParentNo = "0";
                if (dept.IsExits == true)
                    return;

                dept.Idx = idxDept++;
                dept.Insert();

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
                dept.Idx = idxDept++;
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
                string name = entry.Name.Replace("CN=", "");
                string objectCategory = this.GetValFromDirectoryEntryByKey(entry, "objectCategory");

                if (objectCategory.Contains("CN=Group") == true)
                {

                    //判断是 group 还是 user.
                    BP.GPM.Station station  =new Station(); 
                    // emp.No = name;// this.GetValFromDirectoryEntryByKey(entry, "samaccountname");
                    station.No = entry.Guid.ToString();
                    station.Name = name;// this.GetValFromDirectoryEntryByKey(entry, "cn"); 
                   // station.Idx = idxStation++;
                    station.Insert();
                    return;
                }
                else
                {
                    //判断是 group 还是 user.
                    BP.GPM.AD.Emp emp = new Emp();
                    // emp.No = name;// this.GetValFromDirectoryEntryByKey(entry, "samaccountname");
                    emp.No = name;// this.GetValFromDirectoryEntryByKey(entry, "cn");
                    emp.Name = this.GetValFromDirectoryEntryByKey(entry, "displayName");

                    if (emp.IsExits == true)
                        return;

                    emp.FK_Dept = entry.Parent.Guid.ToString();

                    if (emp.No.Length > 20)
                        return;

                     emp.Idx = idxEmp++;
                    emp.Insert();
                    return;
                }
            }
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
