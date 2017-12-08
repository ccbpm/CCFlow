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
    public class WF_Admin_Cond : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_Cond(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 方向优先级.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondPRI_Init()
        {
            Conds cds = new Conds();
            cds.Retrieve(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, 2, CondAttr.PRI);

            foreach (Cond item in cds)
            {
                Node nd = new Node(item.ToNodeID);
                item.Note = nd.Name;
            }

            return cds.ToJson();
        }
        public string CondPRI_Move()
        {
            switch (this.GetRequestVal("MoveType"))
            {
                case "Up":
                    Cond up = new Cond(this.MyPK);
                    up.DoUp(this.FK_Node);
                    up.RetrieveFromDBSources();
                    DBAccess.RunSQL("UPDATE WF_Cond SET PRI=" + up.PRI + " WHERE ToNodeID=" + up.ToNodeID);
                    break;
                case "Down":
                    Cond down = new Cond(this.MyPK);
                    down.DoDown(this.FK_Node);
                    down.RetrieveFromDBSources();
                    DBAccess.RunSQL("UPDATE WF_Cond SET PRI=" + down.PRI + " WHERE ToNodeID=" + down.ToNodeID);
                    break;
                default:
                    break;
            }


            Conds cds = new Conds();
            cds.Retrieve(CondAttr.FK_Node, this.FK_Node, CondAttr.CondType, 2, CondAttr.PRI);
            return cds.ToJson();
        }
        #endregion 方向优先级.


        /// <summary>
        /// 初始化Init.
        /// </summary>
        /// <returns></returns>
        public string Condition_Init()
        {
            string toNodeID = this.GetRequestVal("ToNodeID");
            var cond = new Cond();
            cond.Retrieve(CondAttr.NodeID, this.FK_Node, CondAttr.ToNodeID, toNodeID);
            cond.Row.Add("HisDataFrom", cond.HisDataFrom.ToString());

            //   cond.HisDataFrom
            //CurrentCond = DataFrom[cond.HisDataFrom];
            return cond.ToJson();
        }

        /// <summary>
        /// 打开方向条件的初始化.
        /// 到达的节点.
        /// </summary>
        /// <returns></returns>
        public string ConditionLine_Init()
        {
            string sql = "SELECT A.NodeID, A.Name FROM WF_Node A,  WF_Direction B WHERE A.NodeID=B.ToNode AND B.Node=" + this.FK_Node;

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.Columns[0].ColumnName = "NodeID";
            dt.Columns[1].ColumnName = "Name";

            return BP.Tools.Json.DataTableToJson(dt, false);
        }


        #region 方向条件URL
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByUrl_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Url.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByUrl_Save()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Url.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, fk_mainNode,
              CondAttr.ToNodeID, toNodeID,
              CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.Url;

            cond.NodeID = this.GetRequestValInt("FK_MainNode");
            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condTypeEnum;
            cond.Insert();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByUrl_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Url.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region 方向条件 Frm 模版
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByFrm_Init()
        {
            DataSet ds = new DataSet();

            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            Node nd = new Node(int.Parse(fk_mainNode));

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            //string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            //增加条件集合.
            Conds conds = new Conds();
            conds.Retrieve(CondAttr.FK_Node, fk_mainNode, CondAttr.ToNodeID, toNodeID);
            ds.Tables.Add(conds.ToDataTableField("WF_Conds"));

            //增加字段集合.
            string sql = "";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn||' - '||Name as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN('FID','MyNum','Rec','CDT','RDT','AtPara','WFSta','FlowNote','FlowStartRDT','FlowEnderRDT','FlowEnder','FlowSpanDays','WFState','OID','PWorkID','PFlowNo','PEmp','FlowEndNode','GUID')";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT KeyOfEn as No, CONCAT(KeyOfEn,' - ', Name ) as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN('FID','MyNum','Rec','CDT','RDT','AtPara','WFSta','FlowNote','FlowStartRDT','FlowEnderRDT','FlowEnder','FlowSpanDays','WFState','OID','PWorkID','PFlowNo','PEmp','FlowEndNode','GUID')";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn+' - '+Name as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN('FID','MyNum','Rec','CDT','RDT','AtPara','WFSta','FlowNote','FlowStartRDT','FlowEnderRDT','FlowEnder','FlowSpanDays','WFState','OID','PWorkID','PFlowNo','PEmp','FlowEndNode','GUID')";
                sql += " AND MyDataType NOT IN (6,7) ";
            }


            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            DataRow dr = dt.NewRow();
            dr[0] = "all";
            dr[1] = "请选择表单字段";
            dt.Rows.Add(dr);
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false); // cond.ToJson();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByBindFrms_Init()
        {
            string sql = "SELECT m.No, m.Name, n.FK_Node, n.FK_Flow FROM WF_FrmNode n INNER JOIN Sys_MapData m ON n.FK_Frm=m.No WHERE n.FK_Node=" + this.FK_Node;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Frms";
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            DataRow dr = dt.NewRow();
            dr[0] = "all";
            dr[1] = "请选择表单";
            dt.Rows.Add(dr);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false); // cond.ToJson();
        }

        public string CondByFrm_InitField()
        {
            //定义数据容器.
            DataSet ds = new DataSet();

            //字段属性.
            MapAttr attr = new MapAttr();
            attr.MyPK = "ND" + int.Parse(this.FK_Flow) + "Rpt_" + this.KeyOfEn;
            attr.Retrieve();

            ds.Tables.Add(attr.ToDataTableField("Sys_MapAttr"));

            if (attr.LGType == FieldTypeS.Enum)
            {
                SysEnums ses = new SysEnums(attr.UIBindKey);
                ds.Tables.Add(ses.ToDataTableField("Enums"));
            }


            #region 增加操作符 number.
            if (attr.IsNum)
            {
                DataTable dtOperNumber = new DataTable();
                dtOperNumber.TableName = "Opers";
                dtOperNumber.Columns.Add("No", typeof(string));
                dtOperNumber.Columns.Add("Name", typeof(string));

                DataRow dr = dtOperNumber.NewRow();
                dr["No"] = "dengyu";
                dr["Name"] = "= 等于";
                dtOperNumber.Rows.Add(dr);

                dr = dtOperNumber.NewRow();
                dr["No"] = "dayu";
                dr["Name"] = " > 大于";
                dtOperNumber.Rows.Add(dr);

                dr = dtOperNumber.NewRow();
                dr["No"] = "dayudengyu";
                dr["Name"] = " >= 大于等于";
                dtOperNumber.Rows.Add(dr);

                dr = dtOperNumber.NewRow();
                dr["No"] = "xiaoyu";
                dr["Name"] = " < 小于";
                dtOperNumber.Rows.Add(dr);

                dr = dtOperNumber.NewRow();
                dr["No"] = "xiaoyudengyu";
                dr["Name"] = " <= 小于等于";
                dtOperNumber.Rows.Add(dr);

                dr = dtOperNumber.NewRow();
                dr["No"] = "budengyu";
                dr["Name"] = " != 不等于";
                dtOperNumber.Rows.Add(dr);

                ds.Tables.Add(dtOperNumber);
            }
            else
            {
                #region 增加操作符 string.
                DataTable dtOper = new DataTable();
                dtOper.TableName = "Opers";
                dtOper.Columns.Add("No", typeof(string));
                dtOper.Columns.Add("Name", typeof(string));

                DataRow dr = dtOper.NewRow();
                dr["No"] = "dengyu";
                dr["Name"] = "= 等于";
                dtOper.Rows.Add(dr);

                dr = dtOper.NewRow();
                dr["No"] = "like";
                dr["Name"] = " LIKE 包含";
                dtOper.Rows.Add(dr);

                dr = dtOper.NewRow();
                dr["No"] = "budengyu";
                dr["Name"] = " != 不等于";
                dtOper.Rows.Add(dr);
                ds.Tables.Add(dtOper);
                #endregion 增加操作符 string.
            }
            #endregion 增加操作符 number.

            return BP.Tools.Json.DataSetToJson(ds, false); // cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByFrm_Save()
        {
            //定义变量.
            string field = this.GetRequestVal("DDL_Fields");
            field = "ND" + int.Parse(this.FK_Flow) + "Rpt_" + field;

            int toNodeID = this.GetRequestValInt("ToNodeID");
            int fk_Node = this.GetRequestValInt("FK_Node");
            string oper = this.GetRequestVal("DDL_Operator");

            string operVal = this.GetRequestVal("OperVal");

            string saveType = this.GetRequestVal("SaveType"); //保存类型.
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            //把其他的条件都删除掉.
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ( NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.NodeForm);

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.NodeForm;
            cond.NodeID = fk_Node;
            cond.ToNodeID = toNodeID;

            cond.FK_Node = this.FK_Node;
            cond.FK_Operator = oper;
            cond.OperatorValue = operVal; //操作值.

            cond.FK_Attr = field; //字段属性.

            //  cond.OperatorValueT = ""; // this.GetOperValText;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condTypeEnum;

            if (saveType == "AND")
                cond.CondOrAnd = CondOrAnd.ByAnd;
            else
                cond.CondOrAnd = CondOrAnd.ByOr;

            #region 方向条件，全部更新.
            Conds conds = new Conds();
            QueryObject qo = new QueryObject(conds);
            qo.AddWhere(CondAttr.NodeID, this.FK_Node);
            qo.addAnd();
            qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.NodeForm);
            qo.addAnd();
            qo.AddWhere(CondAttr.CondType, (int)condTypeEnum);
            if (toNodeID != 0)
            {
                qo.addAnd();
                qo.AddWhere(CondAttr.ToNodeID, toNodeID);
            }
            int num = qo.DoQuery();
            foreach (Cond item in conds)
            {
                item.CondOrAnd = cond.CondOrAnd;
                item.Update();
            }
            #endregion

            /* 执行同步*/
            string sqls = "UPDATE WF_Node SET IsCCFlow=0";
            sqls += "@UPDATE WF_Node  SET IsCCFlow=1 WHERE NodeID IN (SELECT NODEID FROM WF_Cond a WHERE a.NodeID= NodeID AND CondType=1 )";
            BP.DA.DBAccess.RunSQLs(sqls);

            string sql = "UPDATE WF_Cond SET DataFrom=" + (int)ConnDataFrom.NodeForm + " WHERE NodeID=" + cond.NodeID + "  AND FK_Node=" + cond.FK_Node + " AND ToNodeID=" + toNodeID;
            switch (condTypeEnum)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    break;
                case CondType.Dir:
                    // cond.MyPK = cond.NodeID +"_"+ this.Request.QueryString["ToNodeID"]+"_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    break;
                case CondType.SubFlow: //启动子流程.
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    break;
                default:
                    throw new Exception("未设计的情况。" + condTypeEnum.ToString());
            }

            return "保存成功!!";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByFrm_Delete()
        {
            Cond deleteCond = new Cond();
            deleteCond.MyPK = this.MyPK;
            int i = deleteCond.Delete();
            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion 方向条件 Frm 模版

        #region 方向条件SQL 模版
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondBySQLTemplate_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondBySQLTemplate_Save()
        {

            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, fk_mainNode,
              CondAttr.ToNodeID, toNodeID,
              CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.SQLTemplate;

            cond.NodeID = this.GetRequestValInt("FK_MainNode");
            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condTypeEnum;
            cond.Insert();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondBySQLTemplate_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion 方向条件SQL 模版

        #region 方向条件SQL
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondBySQL_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondBySQL_Save()
        {

            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, fk_mainNode,
              CondAttr.ToNodeID, toNodeID,
              CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.SQL;

            cond.NodeID = this.GetRequestValInt("FK_MainNode");
            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condTypeEnum;
            cond.Insert();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondBySQL_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region 方向条件岗位
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByStation_Init()
        {
            DataSet ds = new DataSet();

            //岗位类型.
            BP.GPM.StationTypes tps = new BP.GPM.StationTypes();
            tps.RetrieveAll();
            ds.Tables.Add(tps.ToDataTableField("StationTypes"));

            //岗位.
            BP.GPM.Stations sts = new BP.GPM.Stations();
            sts.RetrieveAll();
            ds.Tables.Add(sts.ToDataTableField("Stations"));


            //取有可能存盘的数据.
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            Cond cond = new Cond();
            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();
            ds.Tables.Add(cond.ToDataTableField("Cond"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByStation_Save()
        {

            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            CondType HisCondType = CondType.Dir;

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, FK_MainNode,
              CondAttr.ToNodeID, ToNodeID,
              CondAttr.CondType, (int)HisCondType);

            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();

            // 删除岗位条件.
            cond.MyPK = mypk;
            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Stas;
                cond.NodeID = FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = ToNodeID;
                cond.Insert();
            }

            string val = this.GetRequestVal("emps").Replace(",", "@");
            cond.OperatorValue = val;
            cond.SpecOperWay = (SpecOperWay)this.GetRequestValInt("DDL_SpecOperWay");
            if (cond.SpecOperWay != SpecOperWay.CurrOper)
            {
                cond.SpecOperPara = this.GetRequestVal("TB_SpecOperPara");
            }
            else
            {
                cond.SpecOperPara = string.Empty;
            }
            cond.HisDataFrom = ConnDataFrom.Stas;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = CondType.Dir;
            cond.FK_Node = FK_MainNode;



            cond.ToNodeID = ToNodeID;
            cond.Update();


            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByStation_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region 按照部门条件计算CondByDept_Delete
        /// <summary>
        /// 按照部门条件计算.
        /// </summary>
        /// <returns></returns>
        public string CondByDept_Init()
        {
            DataSet ds = new DataSet();

            //部门
            Depts depts = new Depts();
            depts.RetrieveAllFromDBSource();
            ds.Tables.Add(depts.ToDataTableField("Depts"));

            //取有可能存盘的数据.
            //int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            //int ToNodeID = this.GetRequestValInt("ToNodeID");
            Cond cond = new Cond();
            //CondType condType = (CondType)this.GetRequestValInt("CondType");
            //string mypk = this.GetRequestValInt("FK_MainNode") + "_" + this.GetRequestValInt("ToNodeID") + "_" + condType.ToString() + "_" + ConnDataFrom.Depts.ToString();
            cond.MyPK = this.GetRequestVal("MyPK"); ;
            cond.RetrieveFromDBSources();
            ds.Tables.Add(cond.ToDataTableField("Cond"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        public string CondByDept_Save()
        {
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            CondType condType = (CondType)this.GetRequestValInt("CondType");

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, this.GetRequestValInt("FK_MainNode"),
               CondAttr.ToNodeID, this.GetRequestValInt("ToNodeID"),
               CondAttr.CondType, (int)condType);


            string mypk = this.GetRequestValInt("FK_MainNode") + "_" + this.GetRequestValInt("ToNodeID") + "_" + condType.ToString() + "_" + ConnDataFrom.Depts.ToString();
            cond.MyPK = mypk;

            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Depts;
                cond.NodeID = this.GetRequestValInt("FK_MainNode");
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = this.GetRequestValInt("ToNodeID");
                cond.Insert();
            }

            string val = this.GetRequestVal("depts").Replace(",", "@");
            cond.OperatorValue = val;
            cond.SpecOperWay = (SpecOperWay)this.GetRequestValInt("DDL_SpecOperWay");
            if (cond.SpecOperWay != SpecOperWay.CurrOper)
            {
                cond.SpecOperPara = this.GetRequestVal("TB_SpecOperPara");
            }
            else
            {
                cond.SpecOperPara = string.Empty;
            }
            cond.HisDataFrom = ConnDataFrom.Stas;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = CondType.Dir;
            cond.FK_Node = FK_MainNode;



            cond.ToNodeID = ToNodeID;
            cond.Update();

            //switch (condType)
            //{
            //    case CondType.Flow:
            //    case CondType.Node:
            //        cond.Update();
            //        break;
            //    case CondType.Dir:
            //        cond.ToNodeID = this.GetRequestValInt("ToNodeID");
            //        cond.Update();
            //        break;
            //    case CondType.SubFlow:
            //        cond.ToNodeID = this.GetRequestValInt("ToNodeID");
            //        cond.Update();
            //        break;
            //    default:
            //        throw new Exception("未设计的情况。");
            //}

            return "保存成功!!";
        }
        public string CondByDept_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }

        #endregion

        #region 方向条件Para
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByPara_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Paras.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByPara_Save()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Paras.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, fk_mainNode,
              CondAttr.ToNodeID, toNodeID,
              CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.Paras;

            cond.NodeID = this.GetRequestValInt("FK_MainNode");
            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condTypeEnum;
            cond.Insert();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByPara_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Paras.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.NodeID, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region 按照岗位的方向条件.
        public string CondStation_Init()
        {
            DataSet ds = new DataSet();

            //岗位类型.
            BP.GPM.StationTypes tps = new BP.GPM.StationTypes();
            tps.RetrieveAll();
            ds.Tables.Add(tps.ToDataTableField("StationTypes"));

            //岗位.
            BP.GPM.Stations sts = new BP.GPM.Stations();
            sts.RetrieveAll();
            ds.Tables.Add(sts.ToDataTableField("Stations"));


            //取有可能存盘的数据.
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            Cond cond = new Cond();
            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();
            ds.Tables.Add(cond.ToDataTableField("Cond"));

            return BP.Tools.Json.DataSetToJson(ds, false);


        }
        public string CondStation_Save()
        {
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            CondType HisCondType = CondType.Dir;

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, FK_MainNode,
              CondAttr.ToNodeID, ToNodeID,
              CondAttr.CondType, (int)HisCondType);

            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();

            // 删除岗位条件.
            cond.MyPK = mypk;
            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Stas;
                cond.NodeID = FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = ToNodeID;
                cond.Insert();
            }

            string val = "";
            Stations sts = new Stations();
            sts.RetrieveAllFromDBSource();
            foreach (Station st in sts)
            {
                if (this.GetRequestVal("CB_" + st.No) != "1")
                    continue;
                val += "@" + st.No;
            }

            val += "@";
            cond.OperatorValue = val;
            cond.HisDataFrom = ConnDataFrom.Stas;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = CondType.Dir;
            cond.FK_Node = FK_MainNode;

            #region //获取“指定的操作员”设置，added by liuxc,2015-10-7
            cond.SpecOperWay = (SpecOperWay)this.GetRequestValInt("DDL_" + CondAttr.SpecOperWay);

            if (cond.SpecOperWay != SpecOperWay.CurrOper)
                cond.SpecOperPara = this.GetRequestVal("TB_" + CondAttr.SpecOperPara);
            else
                cond.SpecOperPara = string.Empty;
            #endregion

            cond.ToNodeID = ToNodeID;
            cond.Update();

            return "保存成功..";
        }
        #endregion 按照岗位的方向条件.

    }
}
