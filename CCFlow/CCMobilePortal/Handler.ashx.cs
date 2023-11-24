using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP.Cloud;

namespace CCFlow.CCMobilePortal
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 注册企业
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgShortName"></param>
        /// <param name="openid">小程序ID</param>
        /// <param name="userName"></param>
        /// <param name="tel"></param>
        /// <returns></returns>
        public string RegByXiaoChengXu(string orgName, string orgShortName, 
            string openid,string userName, string tel)
        {

            //注册企业.
            BP.Cloud.Org org = new BP.Cloud.Org();
            org.No = BP.DA.DBAccess.GenerGUID(4, "Port_Org", "No");
            org.Name = orgShortName;
            org.NameFull = orgName;
            org.Adminer = openid;
            org.AdminerName = userName;
            org.Insert();

            //增加管理员.
            BP.WF.Port.Admin2Group.OrgAdminer admin = new BP.WF.Port.Admin2Group.OrgAdminer();
            admin.FK_Emp = openid;
            admin.OrgNo = org.No;
            admin.Insert();

            //增加这个人员.
            Emp emp = new Emp();
            emp.No = org.No + "_" + openid;
            emp.Name = userName;

            // 设置ID.
            emp.UserID =   openid;
            emp.OpenID = openid;

            emp.FK_Dept = org.No;
            emp.Tel = tel;
            emp.Insert();

            //初始化部门.
            BP.Cloud.Dept dept = new Dept();
            dept.ParentNo = "100";
            dept.No = org.No;
            dept.Name = org.Name;
            dept.OrgNo = org.No;
            dept.Insert();

            dept.ParentNo = org.No;
            dept.No = BP.DA.DBAccess.GenerGUID();
            dept.Name = "办公室";
            dept.OrgNo = org.No;
            dept.Insert();

            dept.ParentNo = org.No;
            dept.No = BP.DA.DBAccess.GenerGUID();
            dept.Name = "财务部";
            dept.OrgNo = org.No;
            dept.Insert();


            //初始化岗位.
            BP.Cloud.Station sta = new Station();
            sta.No = BP.DA.DBAccess.GenerGUID();
            sta.Name = "办公室主任";
            sta.OrgNo = org.No;
            sta.Insert();

            sta = new Station();
            sta.No = BP.DA.DBAccess.GenerGUID();
            sta.Name = "财务部主任";
            sta.OrgNo = org.No;
            sta.Insert();


            return "注册成功.";
        }
    }
}