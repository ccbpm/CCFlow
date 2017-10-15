﻿using System;
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
    public class WF_RptSearch : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_RptSearch(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 流程分布.
        public string DistributedOfMy_Init()
        {
            DataSet ds = new DataSet();

            //我发起的流程.
            string sql = "";
            sql = "select FK_Flow, FlowName,Count(WorkID) as Num FROM WF_GenerWorkFlow  WHERE Starter='" + BP.Web.WebUser.No + "' GROUP BY FK_Flow, FlowName ";
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Start";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NUM"].ColumnName = "Num";
            }
            ds.Tables.Add(dt);

            //待办.
            sql = "select FK_Flow, FlowName,Count(WorkID) as Num FROM wf_empworks  WHERE FK_Emp='" + BP.Web.WebUser.No + "' GROUP BY FK_Flow, FlowName ";
            System.Data.DataTable dtTodolist = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtTodolist.TableName = "Todolist";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtTodolist.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dtTodolist.Columns["FLOWNAME"].ColumnName = "FlowName";
                dtTodolist.Columns["NUM"].ColumnName = "Num";
            }

            ds.Tables.Add(dtTodolist);

            //正在运行的流程.
            System.Data.DataTable dtRuning = BP.WF.Dev2Interface.DB_TongJi_Runing();
            dtRuning.TableName = "Runing";
            ds.Tables.Add(dtRuning);


            //归档的流程.
            System.Data.DataTable dtOK = BP.WF.Dev2Interface.DB_TongJi_FlowComplete();
            dtOK.TableName = "OK";
            ds.Tables.Add(dtOK);

            //返回结果.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        #endregion


        #region 功能列表
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("MyStartFlow", "我发起的流程");
            ht.Add("MyJoinFlow", "我参与的流程");

            //   ht.Add("MyDeptFlow", "我本部门发起的流程");
            //  ht.Add("MySubDeptFlow", "我本部门与子部门发起的流程");
            // ht.Add("AdvFlowsSearch", "高级查询");

            return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
        }
        #endregion

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.


        #region xxx 界面 .
        #endregion xxx 界面方法.

        #region KeySearch.htm
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public string KeySearch_Query()
        {
            string type = this.GetRequestVal("type");
            string keywords = this.GetRequestVal("TB_KeyWords");
            int myselft = this.GetRequestValInt("CHK_Myself");
            string sql = "";

            switch (type)
            {
                case "ByWorkID":
                    if (myselft == 1)
                        sql = "SELECT FlowName,FK_Flow,FK_Node,WorkID,Title,Starter,RDT,WFSta,Emps FROM WF_GenerWorkFlow WHERE  WorkID=" + keywords + " AND Emps LIKE '@%" + WebUser.No + "%'";
                    else
                        sql = "SELECT FlowName,FK_Flow,FK_Node,WorkID,Title,Starter,RDT,WFSta,Emps FROM WF_GenerWorkFlow WHERE  WorkID=" + keywords;
                    break;

                case "ByTitle":
                    if (myselft == 1)
                        sql = "SELECT FlowName,FK_Flow,FK_Node,WorkID,Title,Starter,RDT,WFSta,Emps FROM WF_GenerWorkFlow WHERE  Title LIKE '%" + keywords + "%' AND Emps LIKE '@%" + WebUser.No + "%'";
                    else
                        sql = "SELECT FlowName,FK_Flow,FK_Node,WorkID,Title,Starter,RDT,WFSta,Emps FROM WF_GenerWorkFlow WHERE  Title LIKE '%" + keywords + "%'";
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["EMPS"].ColumnName = "Emps";
            }

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

    }
}
