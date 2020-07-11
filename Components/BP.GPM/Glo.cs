﻿using System;
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
        /// 是否允许用户注册.
        /// </summary>
        public static bool IsEnableRegUser
        {
            get
            {
                string str= SystemConfig.AppSettings["IsEnableRegUser"];
                if (DataType.IsNullOrEmpty(str)==true)
                    return false;

                if (str.Equals("1") == true)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public static OSModel OSModel
        {
            get
            {
                return OSModel.OneMore;
                //OSModel os = (OSModel)SystemConfig.GetValByKeyInt("OSModel", 1);
                //return os;  
            }
        }
        /// <summary>
        /// 安装包
        /// </summary>
        public static void DoInstallDataBase()
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

                if (en.ToString().Contains("BP.GPM.") == false)
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
                    if (en.EnMap.PhysicsTable.ToLower().Contains("demo_") == true)
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
            DBAccess.RunSQLScript(sqlscript);
            #endregion 修复

            #region 5, 初始化数据。
            if (DBAccess.IsExitsObject("GPM_AppSort") == true)
            {
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\InitPublicData.sql";
                DBAccess.RunSQLScript(sqlscript);
            }
            #endregion 初始化数据

            #region 6, 创建视图。
            sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MSSQL_GPM_VIEW.sql";

            //MySQL 语法有所区别
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MySQL_GPM_VIEW.sql";

            //Oracle 语法有所区别
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\Oracle_GPM_VIEW.sql";

            DBAccess.RunSQLScriptGo(sqlscript);
            #endregion 创建视图

          
            //处理全路径
            Depts depts = new Depts();
            depts.RetrieveAll();
            foreach (Dept dept in depts)
                dept.GenerNameOfPath();

        }
        
    }
}
