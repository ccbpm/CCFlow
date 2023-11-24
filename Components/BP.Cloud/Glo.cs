using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using LitJson;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.WF;
using System.Web.SessionState;
using Newtonsoft.Json;
using System.Linq;

namespace BP.Cloud
{
    /// <summary>
    /// 云的公共类
    /// </summary>
    public class Glo
    {
        
       
        
        /// <summary>
        /// 生成BPM微信
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="orgNo"></param>
        /// <param name="orgName"></param>
        public static void Port_Org_InstallByWeiXin(string userID, string userName,
            string orgNo, string orgName, string empstrs, string deptstrs)
        {
            #region 处理管理员与adminer 的数据.
            BP.Cloud.Emp emp = new BP.Cloud.Emp();
            emp.No = userID;

            //如果没有，请插入.
            if (emp.RetrieveFromDBSources() == 0)
            {
                emp.No = userID;
                emp.Name = userName;
                emp.FK_Dept = orgNo;
                emp.Pass = DBAccess.GenerGUID();
                emp.Insert();
            } 

            BP.Cloud.Org org = new Org();
            org.No = orgNo;
            if (org.RetrieveFromDBSources() == 0)
            {
                org.Name = orgName;
                org.Adminer = userID;
                org.AdminerName = userName;
                org.WXUseSta = 1;
                org.DTReg = DataType.CurrentDataTime;
                org.Insert(); //把他设置为超级管理员.
            }else
            {
                org.WXUseSta = 1; //把他变成启用状态.
                org.Update();
            }
            #endregion 处理管理员与adminer 的数据.

            #region 开始同步人员信息.
            #endregion 开始同步人员信息.

            #region 开始同步部门信息.
            Dept deptRoot = new Dept();
            deptRoot.No = orgNo;
            if (deptRoot.RetrieveFromDBSources()==0)
            {
                deptRoot.Name = orgName;
                deptRoot.ParentNo = "100";
                deptRoot.OrgNo = orgNo;
                deptRoot.Insert();
            }else
            {
                deptRoot.Name = orgName;
                deptRoot.ParentNo = "100";
                deptRoot.OrgNo = orgNo;
                deptRoot.Update();
            }
            #endregion 开始同步部门信息.

            //让管理员登录.
            BP.Cloud.Dev2Interface.Port_Login(userID, orgNo,false);
        }
        public static void Port_Org_UnInstallByWeiXin(string userID, string userName,
          string orgNo, string orgName)
        {
            Org org = new Org(orgNo);
            org.WXUseSta = 0;
            org.DTEnd = DataType.CurrentDataTime;
            org.Update();
        }
        /// <summary>
        /// 当人员信息变化时
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="deptNo"></param>
        public static void Port_Change_Emp(string id, string name, string deptNo)
        {

        }
        public static void Port_Change_Dept(string id, string name, string deptNo)
        {

        }
        public static void Port_Change_Org(string id, string name, string deptNo)
        {

        }

        public static void SaveImgByUrl(string corpSquareLogoUrl, string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}

