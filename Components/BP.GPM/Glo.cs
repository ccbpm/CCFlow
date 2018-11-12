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
                return OSModel.OneMore;
                //OSModel os = (OSModel)BP.Sys.SystemConfig.GetValByKeyInt("OSModel", 1);
                //return os;  
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
            BP.DA.DBAccess.RunSQLScript(sqlscript);
            #endregion 修复

            #region 5, 初始化数据。
            if (BP.DA.DBAccess.IsExitsObject("GPM_AppSort") == true)
            {
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\InitPublicData.sql";
                BP.DA.DBAccess.RunSQLScript(sqlscript);
            }
            #endregion 初始化数据

            #region 6, 创建视图。
            sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MSSQL_GPM_VIEW.sql";

            //MySQL 语法有所区别
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\MySQL_GPM_VIEW.sql";

            //Oracle 语法有所区别
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
                sqlscript = SystemConfig.PathOfWebApp + "\\GPM\\SQLScript\\Oracle_GPM_VIEW.sql";

            BP.DA.DBAccess.RunSQLScriptGo(sqlscript);
            #endregion 创建视图

          
            //处理全路径
            Depts depts = new Depts();
            depts.RetrieveAll();
            foreach (Dept dept in depts)
                dept.GenerNameOfPath();

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
