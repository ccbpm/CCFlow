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
using BP.WF.Rpt;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_RptDfine : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_RptDfine(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 流程列表
        /// </summary>
        /// <returns></returns>
        public string Flowlist_Init()
        {
            DataSet ds = new DataSet();
            string sql = "SELECT No,Name,ParentNo FROM WF_FlowSort ORDER BY ParentNo, Idx";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
            ds.Tables.Add(dt);

            sql = "SELECT No,Name,FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort, Idx";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

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

            RptDfine rd = new RptDfine(this.FK_Flow);

            #region 增加本部门发起流程的查询.
            if (rd.MyDeptRole == 0)
            {
                /*如果仅仅部门领导可以查看: 检查当前人是否是部门领导人.*/
                if (DBAccess.IsExitsTableCol("Port_Dept", "Leader") == true)
                {
                    string sql = "SELECT Leader FROM Port_Dept WHERE No='" + BP.Web.WebUser.FK_Dept + "'";
                    string strs = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (strs != null && strs.Contains(BP.Web.WebUser.No) == true)
                    {
                        ht.Add("MyDeptFlow", "我本部门发起的流程");
                    }
                }
            }

            if (rd.MyDeptRole == 1)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDeptFlow", "我本部门发起的流程");
            }

            if (rd.MyDeptRole == 2)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDeptFlow", "我本部门发起的流程");
            }

            #endregion 增加本部门发起流程的查询.



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

        /// <summary>
        /// 我的流程查询.
        /// </summary>
        /// <returns></returns>
        public string MyStartFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMy";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);

            //查询.
            // qo.DoQuery(BP.WF.Data.GERptAttr.OID, 15, this.PageIdx);

            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                DataTable dt = qo.DoQueryToTable();
                dt.TableName = "dt";
                ds.Tables.Add(dt);
            }
            else
            {
                qo.DoQuery();
                ds.Tables.Add(ges.ToDataTableField("dt"));
            }
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string MyDeptFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMyDept";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);

            //查询.
            // qo.DoQuery(BP.WF.Data.GERptAttr.OID, 15, this.PageIdx);

            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                DataTable dt = qo.DoQueryToTable();
                dt.TableName = "dt";
                ds.Tables.Add(dt);
            }
            else
            {
                qo.DoQuery();
                ds.Tables.Add(ges.ToDataTableField("dt"));
            }
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string MyJoinFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMyJoin";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");

            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                DataTable dt = qo.DoQueryToTable();
                dt.TableName = "dt";
                ds.Tables.Add(dt);
            }
            else
            {
                qo.DoQuery();
                ds.Tables.Add(ges.ToDataTableField("dt"));
            }
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
    }
}
