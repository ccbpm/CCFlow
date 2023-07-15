using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.WF.HttpHandler;


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Portal : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Portal()
        {
        }
        #endregion 构造方法.

        #region 欢迎页面初始化.
        /// <summary>
        /// 欢迎页面初始化
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("FlowNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow")); //流程数
            ht.Add("NodeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node")); //节点数据
            ht.Add("FromNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Sys_MapData")); //表单数

            ht.Add("FlowInstaceNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState >1 ")); //实例数.
            ht.Add("TodolistNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=2 ")); //待办数
            ht.Add("ReturnNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=5 ")); //退回数.

            ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=5 ")); //逾期数.

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 获得数据
        /// </summary>
        /// <returns></returns>
        public string Default_DataSet()
        {
            DataSet ds = new DataSet();

            //月份分组.
            string sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState >1 GROUP BY FK_NY ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "DTNY";
            ds.Tables.Add(dt);

            //部门分组.
            sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 GROUP BY DeptName";
            DataTable deptNums = DBAccess.RunSQLReturnTable(sql);
            deptNums.TableName = "DeptNums";
            ds.Tables.Add(deptNums);

            //流程分组.
            sql = "SELECT FlowName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 GROUP BY FlowName";
            DataTable flowNums = DBAccess.RunSQLReturnTable(sql);
            flowNums.TableName = "FlowNums";
            ds.Tables.Add(flowNums);
            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 欢迎页面初始化.

        #region 移动页面.
        /// <summary>
        /// 获取首页中草稿、待办、已完成的数据
        /// </summary>
        /// <returns></returns>
        public string Default_TodoNums()
        {
            Hashtable ht = new Hashtable();

            //我发起的草稿
            ht.Add("Todolist_Draft", BP.WF.Dev2Interface.Todolist_Draft);

            ////我发起在处理的流程
            ht.Add("MyStart_Runing", BP.WF.Dev2Interface.MyStart_Runing);

            ////我发起已完成的流程
            ht.Add("MyStart_Complete", BP.WF.Dev2Interface.MyStart_Complete);

            //我处理的待办数量
            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 获得最近使用的流程.
        /// </summary>
        /// <returns></returns>
        public string Default_FlowsNearly()
        {
            string sql = "";
            int top = GetRequestValInt("Top");
            if (top == 0) top = 8;

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP " + top + " FK_Flow,FlowName,F.Icon FROM WF_GenerWorkFlow G ,WF_Flow F WHERE  F.IsCanStart=1 AND F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON ORDER By Max(SendDT) DESC";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = " SELECT DISTINCT A.FK_Flow,A.FlowName,B.Icon,A.SendDT  FROM WF_GenerWorkFlow A ,WF_Flow B WHERE B.IsCanStart=1 AND B.No=A.FK_Flow AND A.Starter='"+WebUser.No+"'  Order By A.SendDT  limit " + top;
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT * FROM (SELECT DISTINCT FK_Flow as \"FK_Flow\",FlowName as \"FlowName\",F.Icon ,max(SendDT) SendDT FROM WF_GenerWorkFlow G ,WF_Flow F WHERE F.IsCanStart=1 AND F.No=G.FK_Flow AND Starter='" + WebUser.No + "' GROUP BY FK_Flow,FlowName,ICON Order By SendDT) WHERE  rownum <=" + top;
                    break;
                default:
                    throw new Exception("err@系统暂时还未开发使用" + BP.Difference.SystemConfig.AppCenterDBType + "数据库");
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 常用菜单
        /// </summary>
        /// <returns></returns>
        public string Default_MenusOfFlag()
        {
          
            string sql = "";
            int top = GetRequestValInt("Top");
            if (top == 0) top = 8;

            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP " + top + "  No,Name,Icon FROM GPM_Menu WHERE  LEN(MenuModel )  >1 ";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = " SELECT   No,Name,Icon FROM GPM_Menu WHERE  LEN(MenuModel )  >1 limit " + top;
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT   No,Name,Icon FROM GPM_Menu WHERE  LEN(MenuModel )  >1 rownum " + top;
                    break;
                default:
                    throw new Exception("err@系统暂时还未开发使用" + BP.Difference.SystemConfig.AppCenterDBType + "数据库");
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }

        public string Fast_Mobile_Default_Init11()
        {
            //DataSet ds = new DataSet();
            //ds.Table

            //最新使用的流程.
            string sql = "SELECT ";


            //最常用的菜单.


            //系统



            return "";
        }
        #endregion 

    }
}
