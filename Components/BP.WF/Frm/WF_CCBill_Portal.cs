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
using BP.WF.Data;
using BP.WF.HttpHandler;
using BP.NetPlatformImpl;

namespace BP.Frm
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

    }
}
