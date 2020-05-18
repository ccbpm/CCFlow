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
    public class WF_RptSearch : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_RptSearch()
        {
        }

        #region 流程分布.
        public string DistributedOfMy_Init()
        {
            DataSet ds = new DataSet();

            //我发起的流程.
            Paras ps = new Paras();
            ps.SQL = "select FK_Flow, FlowName,Count(WorkID) as Num FROM WF_GenerWorkFlow  WHERE Starter=" + SystemConfig.AppCenterDBVarStr + "Starter GROUP BY FK_Flow, FlowName ";
            ps.Add("Starter", BP.Web.WebUser.No);
            
            //string sql = "";
            //sql = "select FK_Flow, FlowName,Count(WorkID) as Num FROM WF_GenerWorkFlow  WHERE Starter='" + BP.Web.WebUser.No + "' GROUP BY FK_Flow, FlowName ";
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Start";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NUM"].ColumnName = "Num";
            }
            ds.Tables.Add(dt);

            //待办.
            ps = new Paras();
            ps.SQL = "select FK_Flow, FlowName,Count(WorkID) as Num FROM wf_empworks  WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp GROUP BY FK_Flow, FlowName ";
            ps.Add("FK_Emp", BP.Web.WebUser.No);
            //sql = "select FK_Flow, FlowName,Count(WorkID) as Num FROM wf_empworks  WHERE FK_Emp='" + BP.Web.WebUser.No + "' GROUP BY FK_Flow, FlowName ";
            System.Data.DataTable dtTodolist = BP.DA.DBAccess.RunSQLReturnTable(ps);
            dtTodolist.TableName = "Todolist";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
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
            ht.Add("MyJoinFlow", "我审批的流程");



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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
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
            string keywords = this.GetRequestVal("TB_KWds");
            //对输入的关键字进行验证
            keywords = Glo.CheckKeyWord(keywords);
            if (Glo.CheckKeyWordInSql(keywords))
                return "@err:请输入正确字符！";

            Paras ps = new Paras();
            ps.SQL = "SELECT A.FlowName,A.NodeName,A.FK_Flow,A.FK_Node,A.WorkID,A.FID,A.Title,A.StarterName,A.RDT,A.WFSta,A.Emps, A.TodoEmps, A.WFState "
                    + " FROM WF_GenerWorkFlow A "
                    + " WHERE (A.Title LIKE '%" + keywords + "%' "
                    + " or A.Starter LIKE '%" + keywords + "%' "
                    + " or A.StarterName LIKE '%" + keywords + "%') "
                    + " AND (A.Emps LIKE '@%" + WebUser.No + "%' "
                    + " or A.TodoEmps LIKE '%" + WebUser.No + "%') "
                    + " AND A.WFState!=0 ";

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "WF_GenerWorkFlow";

            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps"; //处理人.
                dt.Columns["WFSTATE"].ColumnName = "WFState"; //处理人.
            }
            if (dt != null)
            {
                dt.Columns.Add("TDTime");
                foreach (DataRow dr in dt.Rows)
                {

                    dr["TDTime"] = BP.WF.HttpHandler.CCMobile.GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 判断是否可以执行当前工作？
        /// </summary>
        /// <returns></returns>
        public string KeySearch_GenerOpenUrl()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, WebUser.No) == true)
                return "1";
            else
                return "0";
        }
        //打开表单.
        public string KeySearch_OpenFrm()
        {
           BP.WF.HttpHandler.WF wf=new WF();
            return wf.Runing_OpenFrm();
        }
        #endregion

    }
}
