using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class DataUser_AppCoder : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 自动执行定时任务.
        /// </summary>
        /// <returns></returns>
        public string AutoTask_Init()
        {
            BP.WF.DTS.ccbpmServices dts = new DTS.ccbpmServices();
            dts.Do();

            return "执行成功:"+DataType.CurrentDataCNOfLong;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataUser_AppCoder()
        {
        }

        #region 欢迎页面初始化.
        /// <summary>
        /// 欢迎页面初始化-获得数量.
        /// </summary>
        /// <returns></returns>
        public string FlowDesignerWelcome_Init()
        {
            string whereStr = "";
            string whereStrPuls = "";


            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
            {
                whereStr += " WHERE OrgNo = '" + WebUser.OrgNo + "'";
                whereStrPuls += " AND OrgNo = '" + WebUser.OrgNo + "'";

            }

            Hashtable ht = new Hashtable();
            ht.Add("FlowNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow " + whereStr)); //流程数


            //获得节点的数量 @hongyan.
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                ht.Add("NodeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node " + whereStr)); //节点数据
            else
            {
                string mysql = "SELECT COUNT(B.NodeID) as Num FROM WF_Flow A, WF_Node B WHERE A.No=B.FK_Flow AND A.OrgNo='"+WebUser.OrgNo+"' ";
                ht.Add("NodeNum", DBAccess.RunSQLReturnValInt(mysql)); //节点数据.
            }


            //表单数.
            ht.Add("FromNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Sys_MapData  WHERE FK_FormTree !=''" + whereStrPuls + " AND FK_FormTree IS NOT NULL ")); //表单数

            //所有的实例数量.
            ht.Add("FlowInstaceNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState >1 " + whereStrPuls)); //实例数.

            //所有的待办数量.
            ht.Add("TodolistNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=2 " + whereStrPuls));

            //退回数.
            ht.Add("ReturnNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=5 " + whereStrPuls));

            //说有逾期的数量. 应该根据 WF_GenerWorkerList的 SDT 字段来求.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkerList WHERE IsPass=0 AND STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now()"));

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                
                string sql = "SELECT COUNT(*) from (SELECT *  FROM WF_GenerWorkerList WHERE IsPass=0 AND   REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 ";
                sql += "UNION SELECT * FROM WF_GenerWorkerList WHERE  REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 )";

                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt(sql));
            }
            else
            {
                ht.Add("OverTimeNum", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkerList WHERE IsPass=0 AND convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120)"));
            }
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 获得数量  流程饼图，部门柱状图，月份折线图.
        /// </summary>
        /// <returns></returns>
        public string FlowDesignerWelcome_DataSet()
        {
            string whereStr = "";
            string whereStrPuls = "";

            if (Glo.CCBPMRunModel != CCBPMRunModel.Single )
            {
                whereStr += " WHERE OrgNo = '" + WebUser.OrgNo + "'";
                whereStrPuls += " AND OrgNo = '" + WebUser.OrgNo + "'";
            }

            DataSet ds = new DataSet();

            #region  实例分析
            //月份分组.
            string sql = "SELECT FK_NY, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState >1 " + whereStrPuls + " GROUP BY FK_NY ";
            DataTable FlowsByNY = DBAccess.RunSQLReturnTable(sql);
            FlowsByNY.TableName = "FlowsByNY";
            ds.Tables.Add(FlowsByNY);

            //部门分组.
            sql = "SELECT DeptName, count(WorkID) as Num FROM WF_GenerWorkFlow WHERE WFState >1 " + whereStrPuls + "  GROUP BY DeptName ";
            DataTable FlowsByDept = DBAccess.RunSQLReturnTable(sql);
            FlowsByDept.TableName = "FlowsByDept";
            ds.Tables.Add(FlowsByDept);
            #endregion 实例分析。

            #region 待办 分析
            //待办 - 部门分组.
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT B.Name, count(a.WorkID) as Num FROM WF_GenerWorkerList A,Port_Dept B WHERE A.FK_Dept=B.No GROUP BY B.Name";
            else
                sql = "SELECT B.Name, count(a.WorkID) as Num FROM WF_GenerWorkerList A,Port_Dept B WHERE A.FK_Dept=B.No AND B.OrgNo='" + WebUser.OrgNo + "' GROUP BY B.Name";
            DataTable TodolistByDept = DBAccess.RunSQLReturnTable(sql);
            TodolistByDept.TableName = "TodolistByDept";
            ds.Tables.Add(TodolistByDept);

            //待办的 - 流程分组.
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT c.FlowName as name, count(a.WorkID) as value FROM WF_GenerWorkerList A,Port_Dept B, WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND A.IsPass=0 GROUP BY C.FlowName";
            else
                sql = "SELECT c.FlowName as name, count(a.WorkID) as value  FROM WF_GenerWorkerList A,Port_Dept B, WF_GenerWorkFlow C WHERE A.FK_Dept=B.No AND A.WorkID=C.WorkID AND A.IsPass=0 AND C.OrgNo='" + WebUser.OrgNo+"' GROUP BY C.FlowName";

            //sql = "SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 GROUP BY FlowName";
            DataTable TodolistByFlow = DBAccess.RunSQLReturnTable(sql);
            TodolistByFlow.TableName = "TodolistByFlow";
            ds.Tables.Add(TodolistByFlow);
            #endregion 待办。

            #region 逾期 分析.
            //逾期的 - 流程分组.
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 "+ whereStrPuls+" and STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() GROUP BY FlowName";

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY FlowName ";
                sql += "UNION SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY FlowName";
            }
            else
            {
                sql = "SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) GROUP BY FlowName";
            }
            sql = "SELECT FlowName as name, count(WorkID) as value FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  GROUP BY FlowName";
            DataTable OverTimeByFlow = DBAccess.RunSQLReturnTable(sql);
            OverTimeByFlow.TableName = "OverTimeByFlow";
            ds.Tables.Add(OverTimeByFlow);

            //逾期的 - 部门分组.

            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now() GROUP BY DeptName";

            }
            else if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND(sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 GROUP BY DeptName ";
                sql += "UNION SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 GROUP BY DeptName";
            }
            else
            {
                sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 " + whereStrPuls + "  and convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120) GROUP BY DeptName";
            }
            //sql = "SELECT DeptName, count(WorkID) as Num FROM WF_EmpWorks WHERE WFState >1 GROUP BY DeptName";
            DataTable OverTimeByDept = DBAccess.RunSQLReturnTable(sql);
            OverTimeByDept.TableName = "OverTimeByDept";
            ds.Tables.Add(OverTimeByDept);
            #endregion 逾期。


            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 欢迎页面初始化.

    }
}
