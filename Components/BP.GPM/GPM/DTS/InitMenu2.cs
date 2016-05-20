using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.GPM.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class InitMenu : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public InitMenu()
        {
            this.Title = "为每个操作员初始化所有系统菜单";
            this.Help = "此功能需要在菜单权限,功能，功能点，权限组,系统发生变化后执行.";
            this.Icon = "<img src='/Images/Btn/Delete.gif'  border=0 />";
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
            //删除数据.
            EmpMenus mymes = new EmpMenus();
            mymes.GetNewEntity.CheckPhysicsTable();
            mymes.ClearTable();

            EmpApps empApps = new EmpApps();
            empApps.GetNewEntity.CheckPhysicsTable();
            empApps.ClearTable();

            //查询出来菜单.
            Menus menus = new Menus();
            menus.RetrieveAllFromDBSource();

            //查询出来所有的应用系统.
            Apps apps = new Apps();
            apps.RetrieveAllFromDBSource();

            //查询出来人员.
            Emps emps = new Emps();
            emps.RetrieveAllFromDBSource();

            foreach (Emp emp in emps)
            {
                // 删除该人员的菜单权限.
                string sql = "";
                BP.DA.DBAccess.RunSQL("DELETE GPM_EmpMenu WHERE FK_Emp='" + emp.No + "'");


                string menuIDs = "";

                #region 首先解决对一个人员的个性化设置.
                //从人员菜单设置信息表中查询.
                UserMenus ums = new UserMenus();
                ums.Retrieve(UserMenuAttr.FK_Emp, emp.No);
                foreach (UserMenu um in ums)
                {
                    menuIDs += um.FK_Menu + ",";
                }
                #endregion 首先解决对一个人员的个性化设置.
                
                // 求出该用户有多少个权限组.
                string groupIDs = ",";
                 sql = "SELECT FK_Group FROM GPM_GroupEmp WHERE FK_Emp='" + emp.No + "'";
                sql += " UNION ";
                sql += "SELECT FK_Group FROM GPM_GroupStation WHERE FK_Station in (SELECT FK_Station from Port_DeptEmpStation where FK_Emp='" + emp.No + "') ";

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (groupIDs.Contains("," + dr[0].ToString() + ","))
                        continue;
                    groupIDs += dr[0].ToString() + ",";
                }

            }

            return "所有的成员都被初始化成功.";
        }
    }
}
