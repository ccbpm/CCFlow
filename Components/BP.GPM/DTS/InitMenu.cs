using System;
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
            this.Icon = "<img src='/WF/Img/Btn/Delete.gif'  border=0 />";
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
            Group g = new Group();
            g.CheckPhysicsTable();

            GroupEmp ge = new GroupEmp();
            ge.CheckPhysicsTable();

            GroupStation gs = new GroupStation();
            gs.CheckPhysicsTable();

            UserMenu um = new UserMenu();
            um.CheckPhysicsTable();

            ////删除数据.
            //EmpMenus mymes = new EmpMenus();
            //mymes.GetNewEntity.CheckPhysicsTable();
            //mymes.ClearTable();

            //EmpApps empApps = new EmpApps();
            //empApps.GetNewEntity.CheckPhysicsTable();
            //empApps.ClearTable();

            ////查询出来菜单.
            //Menus menus = new Menus();
            //menus.RetrieveAllFromDBSource();

            ////查询出来所有的应用系统.
            //Apps apps = new Apps();
            //apps.RetrieveAllFromDBSource();

            ////查询出来人员.
            //Emps emps = new Emps();
            //emps.RetrieveAllFromDBSource();
           
            return "所有的成员都被初始化成功.";
        }
    }
}
