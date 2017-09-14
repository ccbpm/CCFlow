using System;
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
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";
            
            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan=ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            sql = "SELECT  TSpan as No, '' as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE Emps LIKE '%" + WebUser.No + "%' GROUP BY TSpan";
            DataTable dtTSpanNum = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtTSpanNum.Columns[0].ColumnName = "No";
                dtTSpanNum.Columns[1].ColumnName = "Name";
                dtTSpanNum.Columns[2].ColumnName = "Num";
            }
            dtTSpanNum.TableName = "TSpanNum";
            ds.Tables.Add(dtTSpanNum);
            #endregion 

            #region 2、处理流程类别列表.
            sql = "SELECT  FK_Flow as No, FlowName as Name, COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE  Emps LIKE '%" + WebUser.No + "%' GROUP BY FK_Flow, FlowName";
            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion 

            #region 3、处理流程实例列表.

            GenerWorkFlows gwfs = new GenerWorkFlows();
            BP.En.QueryObject qo = new QueryObject(gwfs);
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", "%" + BP.Web.WebUser.No + "%");
            qo.Top = 50;
            DataTable dtGwls = qo.DoQueryToTable();
            dtGwls.TableName = "Ens";
            ds.Tables.Add(dtGwls);

            #endregion 

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public string Default_Search()
        {
            string TSpan = this.GetRequestVal("TSpan");
            string FK_Flow = this.GetRequestVal("FK_Flow");

            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.AddWhere(GenerWorkFlowAttr.Emps," LIKE ","%" + BP.Web.WebUser.No + "%");
            if (!string.IsNullOrEmpty(TSpan))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.TSpan, this.GetRequestVal("TSpan")); 
            }
            if (!string.IsNullOrEmpty(FK_Flow))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, this.GetRequestVal("FK_Flow"));
            }
            qo.Top = 50;
            DataTable dt = qo.DoQueryToTable();
            return BP.Tools.Json.ToJson(dt);
        }

        /// <summary>
        /// 获取退回
        /// </summary>
        /// <returns></returns>
        public string DB_GenerReturnWorks() {
            CCMobile cc = new CCMobile(this.context);
            return cc.DB_GenerReturnWorks();
        }
    }
}
