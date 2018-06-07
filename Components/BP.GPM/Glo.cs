using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using BP.Sys;
using BP.DA;
using BP.En;
using BP;

namespace BP.GPM
{
    public class Glo
    {
        /// <summary>
        /// 运行模式
        /// </summary>
        public static OSModel OSModel
        {
            get
            { 
                OSModel os = (OSModel)BP.Sys.SystemConfig.GetValByKeyInt("OSModel", 0);
                return os;  
            }
        }
        /// <summary>
        /// 钉钉是否启用
        /// </summary>
        public static bool IsEnable_DingDing
        {
            get
            {
                //如果两个参数都不为空说明启用
                string corpid = BP.Sys.SystemConfig.Ding_CorpID;
                string corpsecret = BP.Sys.SystemConfig.Ding_CorpSecret;
                if (DataType.IsNullOrEmpty(corpid) || DataType.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 微信是否启用
        /// </summary>
        public static bool IsEnable_WeiXin
        {
            get
            {
                //如果两个参数都不为空说明启用
                string corpid = BP.Sys.SystemConfig.WX_CorpID;
                string corpsecret = BP.Sys.SystemConfig.WX_AppSecret;
                if (DataType.IsNullOrEmpty(corpid) || DataType.IsNullOrEmpty(corpsecret))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 安装包
        /// </summary>
        public static void DoInstallDataBase(string lang, string yunXingHuanjing)
        {
            ArrayList al = null;
            string info = "BP.En.Entity";
            al = BP.En.ClassFactory.GetObjects(info);

            #region 1, 修复表
            foreach (Object obj in al)
            {
                Entity en = null;
                en = obj as Entity;
                if (en == null)
                    continue;

                if (en.ToString() == null)
                    continue;

                if (en.ToString().Contains("BP.Port."))
                    continue;
                //if (en.ToString().Contains("BP.GPM."))
                //    continue;
                //if (en.ToString().Contains("BP.Demo."))
                //    continue;
                 
                string table = null;
                try
                {
                    table = en.EnMap.PhysicsTable;
                    if (table == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                switch (table)
                {
                    case "WF_EmpWorks":
                    case "WF_GenerEmpWorkDtls":
                    case "WF_GenerEmpWorks":
                        continue;
                    case "Sys_Enum":
                        en.CheckPhysicsTable();
                        break;
                    default:
                        en.CheckPhysicsTable();
                        break;
                }

                en.PKVal = "123";
                try
                {
                    en.RetrieveFromDBSources();
                }
                catch (Exception ex)
                {
                    Log.DebugWriteWarning(ex.Message);
                    en.CheckPhysicsTable();
                }
            }
            #endregion 修复

            #region 2, 注册枚举类型 sql
            // 2, 注册枚举类型。
            BP.Sys.XML.EnumInfoXmls xmls = new BP.Sys.XML.EnumInfoXmls();
            xmls.RetrieveAll();
            foreach (BP.Sys.XML.EnumInfoXml xml in xmls)
            {
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums();
                ses.RegIt(xml.Key, xml.Vals);
            }
            #endregion 注册枚举类型

            #region 3, 执行基本的 sql
            string sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\Port_Inc_CH_BPM.sql";
            /*孙战平将RunSQLScript改为RunSQLScriptGo*/
            BP.DA.DBAccess.RunSQLScript(sqlscript);
            #endregion 修复

            #region 5, 初始化数据。

            sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\InitPublicData.sql";
            BP.DA.DBAccess.RunSQLScript(sqlscript);

            //sqlscript = SystemConfig.PathOfWebApp + "\\Data\\Install\\SQLScript\\GO.sql";
            //BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
            #endregion 初始化数据

            #region 6, 创建视图。
            
            if (DBAccess.IsExitsObject("V_GPM_EmpGroup"))
                BP.DA.DBAccess.RunSQL("DROP VIEW V_GPM_EmpGroup");

            if (DBAccess.IsExitsObject("V_GPM_EmpGroupMenu"))
                BP.DA.DBAccess.RunSQL("DROP VIEW V_GPM_EmpGroupMenu");

            if (DBAccess.IsExitsObject("V_GPM_EmpStationMenu"))
                BP.DA.DBAccess.RunSQL("DROP VIEW V_GPM_EmpStationMenu");

            if (DBAccess.IsExitsObject("V_GPM_EmpMenu_GPM"))
                BP.DA.DBAccess.RunSQL("DROP VIEW V_GPM_EmpMenu_GPM");

             if (DBAccess.IsExitsObject("V_GPM_EmpMenu"))
                BP.DA.DBAccess.RunSQL("DROP VIEW V_GPM_EmpMenu");

            

            sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MSSQL_GPM_VIEW.sql";

            //MySQL 语法有所区别
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MySQL_GPM_VIEW.sql";
            }
            //Oracle 语法有所区别
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\Oracle_GPM_VIEW.sql";
            }
            BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
            #endregion 创建视图

            #region 7, 初始化系统访问权限
            //查询出来系统
            Apps apps = new Apps();
            apps.RetrieveAll();

            //查询出来人员.
            Emps emps = new Emps();
            emps.RetrieveAllFromDBSource();
            //查询出来菜单
            Menus menus = new Menus();
            menus.RetrieveAllFromDBSource();

            //删除数据.
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_EmpApp");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_EmpMenu");


            foreach (Emp emp in emps)
            {
                #region 初始化系统访问权限.
                foreach (App app in apps)
                {
                    EmpApp me = new EmpApp();
                    me.Copy(app);
                    me.FK_Emp = emp.No;
                    me.FK_App = app.No;
                    me.Name = app.Name;
                    me.MyPK = app.No + "_" + me.FK_Emp;
                    me.Insert();
                }
                #endregion 初始化系统访问权限.

                #region 初始化人员菜单权限
                foreach (Menu menu in menus)
                {
                    EmpMenu em = new EmpMenu();
                    em.Copy(menu);
                    em.FK_Emp = emp.No;
                    em.FK_App = menu.FK_App;
                    em.FK_Menu = menu.No;
                    em.MyPK = menu.No + "_" + emp.No;
                    em.Insert();
                }
                #endregion
            }
            //处理全路径
            Depts depts = new Depts();
            depts.RetrieveAll();
            foreach (Dept dept in depts)
            {
                dept.GenerNameOfPath();
            }
            #endregion
        }
        /// <summary>
        /// 安装CCIM
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="yunXingHuanjing"></param>
        /// <param name="isDemo"></param>
        public static void DoInstallCCIM(string lang, string dbTypes)
        {
          //  string sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\CCIM_"+BP.Sys.SystemConfig.AppCenterDBType+".sql";
           // BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
        }

        /// <summary>
        /// 是否可以执行判断
        /// </summary>
        /// <param name="obj">判断对象</param>
        /// <param name="cw">方式</param>
        /// <returns>是否可以执行</returns>
        public static bool IsCanDoIt(string ctrlObj, BP.GPM.CtrlWay cw, string empID)
        {
            int n = 0;
            string sql = "";
            switch (cw)
            {
                case CtrlWay.AnyOne:
                    return true;
                case CtrlWay.ByStation:
                    sql = "SELECT count(*) FROM GPM_ByStation WHERE RefObj='" + ctrlObj + "'  AND FK_Station IN (select FK_Station from  Port_DeptEmpStation WHERE FK_Emp='" + empID + "')";
                    n = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    break;
                case CtrlWay.ByDept:
                    sql = "SELECT count(*) FROM GPM_ByDept WHERE RefObj='" + ctrlObj + "'  AND FK_Dept IN (SELECT FK_Dept FROM Port_Emp WHERE No='" + empID + "')";
                    n = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    break;
                case CtrlWay.ByEmp:
                    sql = "SELECT count(*) FROM GPM_ByEmp WHERE RefObj='" + ctrlObj + "'  AND  FK_Emp='" + empID + "'";
                    n = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    break;
                default:
                    break;
            }

            if (n == 0)
                return false;
            else
                return true;
        }
    }
}
