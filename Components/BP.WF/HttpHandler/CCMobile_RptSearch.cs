﻿using System;
using System.Collections.Generic;
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
    public class CCMobile_RptSearch : DirectoryPageBase
    {
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

          /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_RptSearch(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");

            #region 处理时间段数据源.
            if ( string.IsNullOrEmpty(this.FK_Flow) )
                sql = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' GROUP BY TSpan";
            else
                sql = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.FK_Flow + "' AND Emps LIKE '%" + WebUser.No + "%' GROUP BY TSpan";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "Num";
            }

            SysEnums ses = new SysEnums("TSpan");

            DataTable dtTSpan=ses.ToDataTableField();
            foreach (DataRow drSpan in dtTSpan.Rows)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() == drSpan[SysEnumAttr.IntKey].ToString())
                    {
                        drSpan[SysEnumAttr.Lab] = drSpan[SysEnumAttr.Lab] + "(" + dr["Num"] + ")";
                    }
                }
            }

            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);
            #endregion 处理时间段数据源.

            #region 处理流程列表.
            if (string.IsNullOrEmpty(tSpan) == true)
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE  Emps LIKE '%" + WebUser.No + "%' GROUP BY FK_Flow, FlowName";
            else
                sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE TSpan='" + tSpan + "' AND  Emps LIKE '%" + WebUser.No + "%' GROUP BY FK_Flow, FlowName";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion 处理时间段数据源.

            #region 处理流程列表.
            if (string.IsNullOrEmpty(this.FK_Flow) == true)
            {
                if (tSpan == null)
                    sql = "SELECT  * FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' ORDER BY FK_Flow, FlowName";
                else
                    sql = "SELECT  * FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' ORDER BY FK_Flow, FlowName";
            }
            else
            {
                if (tSpan == null)
                    sql = "SELECT  * FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' ORDER BY FK_Flow, FlowName";
                else
                    sql = "SELECT  * FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' ORDER BY FK_Flow, FlowName";
            }
            DataTable dtEns = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtEns.Columns[0].ColumnName = "No";
                dtEns.Columns[1].ColumnName = "Name";
                dtEns.Columns[2].ColumnName = "Num";
            }
            dtEns.TableName = "Ens";
            ds.Tables.Add(dtEns);
            #endregion 处理时间段数据源.

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Default_Search()
        {
            string sql = "SELECT  * FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' ";

            sql += " order by FK_Flow, FlowName";
            DataTable dtEns = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtEns.Columns[0].ColumnName = "No";
                dtEns.Columns[1].ColumnName = "Name";
                dtEns.Columns[2].ColumnName = "Num";
            }
            dtEns.TableName = "Ens";

            return BP.Tools.Json.ToJson(dtEns);
        }
    }
}
