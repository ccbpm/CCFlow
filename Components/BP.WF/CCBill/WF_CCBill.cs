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

namespace BP.WF.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_CCBill(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill()
        {
        }
        /// <summary>
        /// 发起列表.
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.WF.CCBill.Dev2Interface.DB_StartFlows(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 产生一个WorkID.
        /// </summary>
        /// <returns></returns>
        public string Start_GenerWorkID()
        {
            return BP.WF.CCBill.Dev2Interface.CreateBlankWork(this.FrmID, WebUser.No, null).ToString();
        }
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            //草稿列表.
            DataTable dt = BP.WF.CCBill.Dev2Interface.DB_Draft(this.FrmID, BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 单据初始化
        /// </summary>
        /// <returns></returns>
        public string MyBill_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.WF.CCBill.Dev2Interface.DB_StartFlows(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        #region 单据处理.
        /// <summary>
        /// 创建空白的WorkID.
        /// </summary>
        /// <returns></returns>
        public string MyBill_CreateBlankWorkID()
        {
            return BP.WF.CCBill.Dev2Interface.CreateBlankWork(this.FrmID, BP.Web.WebUser.No, null).ToString();
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyBill_SaveIt()
        {
            string str = BP.WF.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
            return str;
        }
        public string MyBill_SaveAsDraft()
        {
            string str = BP.WF.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
            return str;
        }
        public string MyBill_Delete()
        {
            return BP.WF.CCBill.Dev2Interface.MyBill_Delete(this.FrmID, this.WorkID);
        }
        #endregion 单据处理.

        #region 按照日期查询.
        public string Search_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            #region 3、处理流程实例列表.
            string sqlWhere = "";
            sqlWhere = "(1 = 1)AND Starter = '" + WebUser.No + "' AND BillState >= 1";
            sqlWhere += "AND (FrmID = '" + this.FrmID + "')  ";
            sqlWhere += "ORDER BY RDT DESC";

            string fields = " WorkID,BillNo,FrmID,FrmName,Title,BillState,Starter,StarterName,Sender,RDT ";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT " + fields + " FROM (SELECT * FROM WF_CCBill WHERE " + sqlWhere + ") WHERE rownum <= 50";
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 50 " + fields + " FROM WF_CCBill WHERE " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT  " + fields + " FROM WF_CCBill WHERE " + sqlWhere + " LIMIT 50";

            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "BillNo";
                mydt.Columns[2].ColumnName = "FrmID";
                mydt.Columns[3].ColumnName = "FrmName";
                mydt.Columns[4].ColumnName = "Title";
                mydt.Columns[5].ColumnName = "BillState";
                mydt.Columns[6].ColumnName = "Starter";
                mydt.Columns[7].ColumnName = "StarterName";
                mydt.Columns[8].ColumnName = "Sender";
                mydt.Columns[9].ColumnName = "RDT";
            }
            mydt.TableName = "WF_CCBill";
            #endregion

            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询初始化
        /// </summary>
        /// <returns></returns>
        public string SearchData_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");
            if (tSpan == "")
                tSpan = null;

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            GenerBill gb = new GenerBill();
            gb.CheckPhysicsTable();


            sql = "SELECT TSpan as No, COUNT(WorkID) as Num FROM WF_CCBill WHERE FrmID='" + this.FrmID + "'  AND Starter='" + WebUser.No + "' AND BillState >= 1 GROUP BY TSpan";

            DataTable dtTSpanNum = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            sql = " SELECT  a.BillState as No, B.Lab as Name, COUNT(WorkID) as Num FROM WF_CCBill A, Sys_Enum B ";
            sql += " WHERE A.BillState=B.IntKey AND B.EnumKey='BillState' AND  a.Starter='" + WebUser.No + "' AND BillState >=1";
            if (tSpan.Equals("-1") == false)
                sql += "  AND a.TSpan=" + tSpan;

            sql += "  GROUP BY a.BillState, B.Lab  ";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            string sqlWhere = "";
            sqlWhere = "(1 = 1)AND Starter = '" + WebUser.No + "' AND BillState >= 1";
            if (tSpan.Equals("-1") == false)
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }

            if (this.FK_Flow != null)
            {
                sqlWhere += "AND (FrmID = '" + this.FrmID + "')  ";
            }
            else
            {
                // sqlWhere += ")";
            }
            sqlWhere += "ORDER BY RDT DESC";

            string fields = " WorkID,FrmID,FrmName,Title,BillState, Starter, StarterName,Sender,RDT ";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT " + fields + " FROM (SELECT * FROM WF_CCBill WHERE " + sqlWhere + ") WHERE rownum <= 50";
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 50 " + fields + " FROM WF_CCBill WHERE " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT  " + fields + " FROM WF_CCBill WHERE " + sqlWhere + " LIMIT 50";

            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "FrmID";
                mydt.Columns[2].ColumnName = "FrmName";
                mydt.Columns[3].ColumnName = "Title";
                mydt.Columns[4].ColumnName = "BillState";
                mydt.Columns[5].ColumnName = "Starter";
                mydt.Columns[6].ColumnName = "StarterName";
                mydt.Columns[7].ColumnName = "Sender";
                mydt.Columns[8].ColumnName = "RDT";
            }

            mydt.TableName = "WF_CCBill";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    //   dr["TDTime"] =  GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion

            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 查询.

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

    }
}
