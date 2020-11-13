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
        /// 构造函数
        /// </summary>
        public WF_Admin_Cond()
        {
        }

        #region 方向优先级.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondPRI_Init()
        {
            Directions dirs = new Directions();
            dirs.Retrieve(DirectionAttr.Node, this.FK_Node, DirectionAttr.Idx);
            return dirs.ToJson();

            //Node mynd = new Node(this.FK_Node);
            //return mynd.HisToNodes.ToJson();

            //按照条件的先后计算.
            Conds cds = new Conds();
            cds.Retrieve(CondAttr.FK_Node, this.FK_Node,
                CondAttr.CondType, 2, CondAttr.Idx);

            foreach (Cond item in cds)
            {
                Node nd = new Node(item.ToNodeID);
                item.Note = nd.Name;
            }

            if (cds.Count <= 1)
                return "info@当前只有[" + cds.Count + "]个条件，无法进行排序.";

            return cds.ToJson();
        }
        public string CondPRI_Move()
        {
            string mtype = this.GetRequestVal("MoveType");
            if (mtype.Equals("Up"))
            {
                Direction dir = new Direction(this.MyPK);
                dir.DoUp();
            }

            if (mtype.Equals("Down"))
            {
                Direction dir = new Direction(this.MyPK);
                dir.DoDown();
            }


            return "移动成功.";
        }
        #endregion 方向优先级.

        Paras ps = new Paras();
        /// <summary>
        /// 初始化Init.
        /// </summary>
        /// <returns></returns>
        public string Condition_Init()
        {
            string toNodeID = this.GetRequestVal("ToNodeID");
            var cond = new Cond();
            cond.Retrieve(CondAttr.FK_Node, this.FK_Node, CondAttr.ToNodeID, toNodeID);
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
            ps = new Paras();
            ps.SQL = "SELECT A.NodeID, A.Name FROM WF_Node A,  WF_Direction B WHERE A.NodeID=B.ToNode AND B.Node=" + SystemConfig.AppCenterDBVarStr + "Node";
            ps.Add("Node", this.FK_Node);
            //string sql = "SELECT A.NodeID, A.Name FROM WF_Node A,  WF_Direction B WHERE A.NodeID=B.ToNode AND B.Node=" + this.FK_Node;

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.Columns[0].ColumnName = "NodeID";
            dt.Columns[1].ColumnName = "Name";
            return BP.Tools.Json.ToJson(dt);
        }
        #region 方向条件-审核组件
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByWorkCheck_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WorkCheck.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByWorkCheck_Save()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WorkCheck.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Url);

            Cond cond = new Cond();
            //cond.Delete(CondAttr.NodeID, fk_mainNode,
            //  CondAttr.ToNodeID, toNodeID,
            //   CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.WorkCheck;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.
            //if (CondOrAnd != null)
            //    cond.CondOrAnd = CondOrAnd;
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            cond.Save();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByWorkCheck_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WorkCheck.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

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

            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Url);

            Cond cond = new Cond();
            //cond.Delete(CondAttr.NodeID, fk_mainNode,
            //  CondAttr.ToNodeID, toNodeID,
            //   CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.Url;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.
            //if (CondOrAnd != null)
            //    cond.CondOrAnd = CondOrAnd;
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
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
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region WebApi
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByWebApi_Init()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            if (DataType.IsNullOrEmpty(fk_mainNode) == true)
                fk_mainNode = this.GetRequestVal("FK_Node");

            string toNodeID = this.GetRequestVal("ToNodeID");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WebApi.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();

            return cond.ToJson();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByWebApi_Save()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WebApi.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            Cond cond = new Cond();

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.WebApi;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            cond.Insert();

            return "保存成功..";
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string CondByWebApi_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.WebApi.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion WebApi

        #region 方向条件 Frm 模版
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string CondByFrm_Init()
        {
            DataSet ds = new DataSet();

            string toNodeID = this.GetRequestVal("ToNodeID");

            Node nd = new Node(this.FK_Node);

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            //string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            //增加条件集合.
            Conds conds = new Conds();
            conds.Retrieve(CondAttr.FK_Node, this.FK_Node,
                CondAttr.ToNodeID, int.Parse(toNodeID), CondAttr.DataFrom, (int)ConnDataFrom.NodeForm);

            ds.Tables.Add(conds.ToDataTableField("WF_Conds"));

            string noteIn = "'FID','PRI','PNodeID','PrjNo', 'PrjName', 'FK_NY','FlowDaySpan', 'Rec','CDT','RDT','AtPara','WFSta','FlowNote','FlowStartRDT','FlowEnderRDT','FlowEnder','FlowSpanDays','WFState','OID','PWorkID','PFlowNo','PEmp','FlowEndNode','GUID'";

            //增加字段集合.
            string sql = "";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn||' - '||Name as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN (" + noteIn + ") ";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT KeyOfEn as No, CONCAT(KeyOfEn,' - ', Name ) as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN (" + noteIn + ") ";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn+' - '+Name as Name FROM Sys_MapAttr WHERE FK_MapData='ND" + int.Parse(nd.FK_Flow) + "Rpt'";
                sql += " AND KeyOfEn Not IN (" + noteIn + ") ";
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
        public string CondByFrm_InitField()
        {
            //字段属性.
            MapAttr attr = new MapAttr();
            attr.MyPK = "ND" + int.Parse(this.FK_Flow) + "Rpt_" + this.KeyOfEn;
            attr.Retrieve();
            return AttrCond(attr);
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
            string operValT = this.GetRequestVal("OperValText");

            string saveType = this.GetRequestVal("SaveType"); //保存类型.
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            //   CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");

            //把其他的条件都删除掉.
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.NodeForm);

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.NodeForm;
            cond.ToNodeID = toNodeID;

            cond.FK_Node = this.FK_Node;
            cond.FK_Operator = oper;
            cond.OperatorValue = operVal; //操作值.
            cond.OperatorValueT = operValT;

            cond.FK_Attr = field; //字段属性.

            //  cond.OperatorValueT = ""; // this.GetOperValText;
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            //if (CondOrAnd != null)
            //    cond.CondOrAnd = CondOrAnd;
            //if (saveType == "AND")
            //    cond.CondOrAnd = CondOrAnd.ByAnd;
            //else
            //    cond.CondOrAnd = CondOrAnd.ByOr;

            #region 方向条件，全部更新.
            Conds conds = new Conds();
            QueryObject qo = new QueryObject(conds);
            qo.AddWhere(CondAttr.FK_Node, this.FK_Node);
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
            #endregion

            //string sql = "UPDATE WF_Cond SET DataFrom=" + (int)ConnDataFrom.NodeForm + " WHERE NodeID=" + cond.NodeID + "  AND FK_Node=" + cond.FK_Node + " AND ToNodeID=" + toNodeID;
            switch (condTypeEnum)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.Insert();
                    //DBAccess.RunSQL(sql);
                    break;
                case CondType.Dir:
                    // cond.MyPK = cond.NodeID +"_"+ this.Request.QueryString["ToNodeID"]+"_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    //DBAccess.RunSQL(sql);
                    break;
                case CondType.SubFlow: //启动子流程.
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    //DBAccess.RunSQL(sql);
                    break;
                default:
                    throw new Exception("未设计的情况。" + condTypeEnum.ToString());
            }

            return "保存成功!!";
        }
        #endregion 方向条件 Frm 模版

        #region 独立表单的方向条件.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string StandAloneFrm_Init()
        {
            ps = new Paras();
            ps.SQL = "SELECT m.No, m.Name, n.FK_Node, n.FK_Flow FROM WF_FrmNode n INNER JOIN Sys_MapData m ON n.FK_Frm=m.No WHERE n.FrmEnableRole!=5 AND n.FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node";
            ps.Add("FK_Node", this.FK_Node);
            //string sql = "SELECT m.No, m.Name, n.FK_Node, n.FK_Flow FROM WF_FrmNode n INNER JOIN Sys_MapData m ON n.FK_Frm=m.No WHERE n.FrmEnableRole!=5 AND n.FK_Node=" + this.FK_Node;
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Frms";
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            DataRow dr = dt.NewRow();
            dr[0] = "all";
            dr[1] = "请选择表单";
            dt.Rows.Add(dr);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            //增加条件集合.
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            Conds conds = new Conds();
            conds.Retrieve(CondAttr.FK_Node, fk_mainNode, CondAttr.ToNodeID, toNodeID, CondAttr.DataFrom, (int)ConnDataFrom.StandAloneFrm);
            ds.Tables.Add(conds.ToDataTableField("WF_Conds"));

            return BP.Tools.Json.DataSetToJson(ds, false); // cond.ToJson();
        }
        /// <summary>
        /// 获得一个表单的字段.
        /// </summary>
        /// <returns></returns>
        public string StandAloneFrm_InitFrmAttr()
        {
            string frmID = this.GetRequestVal("FrmID");
            MapAttrs attrs = new MapAttrs(frmID);
            return attrs.ToJson();
        }
        public string StandAloneFrm_Save()
        {
            string frmID = this.GetRequestVal("FrmID");

            //定义变量.
            string field = this.GetRequestVal("DDL_Fields");
            field = frmID + "_" + field;

            int toNodeID = this.GetRequestValInt("ToNodeID");
            int fk_Node = this.GetRequestValInt("FK_Node");
            string oper = this.GetRequestVal("DDL_Operator");

            string operVal = this.GetRequestVal("OperVal");

            //节点,子线城,还是其他
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            //    CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");
            //把其他的条件都删除掉.
            ///DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.StandAloneFrm);

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.StandAloneFrm;
            cond.ToNodeID = toNodeID;

            cond.FK_Node = this.FK_Node;
            cond.FK_Operator = oper;
            cond.OperatorValue = operVal; //操作值.

            cond.FK_Attr = field; //字段属性.
            //if (CondOrAnd != null)
            //    cond.CondOrAnd = CondOrAnd;
            //  cond.OperatorValueT = ""; // this.GetOperValText;
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;

            //; //保存类型.
            //if (this.GetRequestVal("SaveType").Equals("AND") == true)
            //    cond.CondOrAnd = CondOrAnd.ByAnd;
            //else
            //    cond.CondOrAnd = CondOrAnd.ByOr;

            #region 方向条件，全部更新.
            Conds conds = new Conds();
            QueryObject qo = new QueryObject(conds);
            qo.AddWhere(CondAttr.FK_Node, this.FK_Node);
            qo.addAnd();
            qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.StandAloneFrm);
            qo.addAnd();
            qo.AddWhere(CondAttr.CondType, (int)condTypeEnum);
            if (toNodeID != 0)
            {
                qo.addAnd();
                qo.AddWhere(CondAttr.ToNodeID, toNodeID);
            }
            int num = qo.DoQuery();
            #endregion

            /* 执行同步*/

            //string sql = "UPDATE WF_Cond SET DataFrom=" + (int)ConnDataFrom.StandAloneFrm + " WHERE NodeID=" + cond.NodeID + "  AND FK_Node=" + cond.FK_Node + " AND ToNodeID=" + toNodeID;
            switch (condTypeEnum)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.Insert();
                    break;
                case CondType.Dir:
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    break;
                case CondType.SubFlow: //启动子流程.
                    cond.MyPK = DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = toNodeID;
                    cond.Insert();
                    break;
                default:
                    throw new Exception("未设计的情况。" + condTypeEnum.ToString());
            }
            return "保存成功!!";
        }

        public string StandAloneFrm_InitField()
        {
            //字段属性.
            MapAttr attr = new MapAttr();
            attr.MyPK = this.FrmID + "_" + this.KeyOfEn;
            attr.Retrieve();
            return AttrCond(attr);
        }
        private string AttrCond(MapAttr attr)
        {
            //定义数据容器.
            DataSet ds = new DataSet();

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
                dr["No"] = "dayu";
                dr["Name"] = ">大于";
                dtOper.Rows.Add(dr);

                dr = dtOper.NewRow();
                dr["No"] = "xiaoyudengyu";
                dr["Name"] = " <= 小于等于";
                dtOper.Rows.Add(dr);

                dr = dtOper.NewRow();
                dr["No"] = "dayudengyu";
                dr["Name"] = " >= 大于等于";
                dtOper.Rows.Add(dr);

                dr = dtOper.NewRow();
                dr["No"] = "xiaoyu";
                dr["Name"] = "<小于";
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
        #endregion

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
            // CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQLTemplate.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.SQLTemplate);

            Cond cond = new Cond();
            //cond.Delete(CondAttr.NodeID, fk_mainNode,
            //  CondAttr.ToNodeID, toNodeID,
            //  CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.SQLTemplate;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            cond.Save();

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
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
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
            //  CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            string sql = this.GetRequestVal("TB_Docs");

            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType="+(int)condTypeEnum+" AND NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.SQL);

            Cond cond = new Cond();
            //cond.Delete(CondAttr.NodeID, fk_mainNode,
            //  CondAttr.ToNodeID, toNodeID,
            // CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.SQL;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            cond.Save();

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
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
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
            StationTypes tps = new StationTypes();
            tps.RetrieveAll();
            ds.Tables.Add(tps.ToDataTableField("StationTypes"));

            //岗位.
            Stations sts = new Stations();
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
            //cond.Delete(CondAttr.NodeID, FK_MainNode,
            //  CondAttr.ToNodeID, ToNodeID,
            // CondAttr.CondType, (int)HisCondType);

            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();

            //把其他的条件都删除掉.
            // DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)HisCondType + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + ToNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Stas);

            // 删除岗位条件.
            cond.MyPK = mypk;
            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Stas;
                cond.FK_Node = FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = ToNodeID;

                cond.Insert();
            }

            string val = this.GetRequestVal("emps").Replace(",", "@");
            string valT = this.GetRequestVal("orgEmps").Replace(",", "&nbsp;&nbsp;");
            cond.OperatorValue = val;
            //cond.OperatorValueT = valT;
            cond.SetPara("OrgEmps", valT);
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
            cond.CondType = CondType.Dir;
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
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
               CondAttr.ToNodeID, toNodeID,
               CondAttr.CondType, (int)condTypeEnum);

            if (i == 1)
                return "删除成功..";

            return "无可删除的数据.";
        }
        #endregion

        #region 按照部门条件计算CondByDept_Delete
        public string CondByDept_Save()
        {
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            CondType condType = (CondType)this.GetRequestValInt("CondType");
            //  CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");

            Cond cond = new Cond();

            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condType + " AND  NodeID=" + this.FK_Node + " AND ToNodeID=" + this.GetRequestValInt("ToNodeID") + ") AND DataFrom!=" + (int)ConnDataFrom.Depts);

            string mypk = this.GetRequestValInt("FK_MainNode") + "_" + this.GetRequestValInt("ToNodeID") + "_" + condType.ToString() + "_" + ConnDataFrom.Depts.ToString();
            cond.MyPK = mypk;

            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Depts;
                cond.FK_Node = this.GetRequestValInt("FK_MainNode");
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = this.GetRequestValInt("ToNodeID");
                //if (CondOrAnd != null)
                //    cond.CondOrAnd = CondOrAnd;
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
            cond.HisDataFrom = ConnDataFrom.Depts;
            cond.FK_Flow = this.FK_Flow;
            cond.CondType = CondType.Dir;
            cond.FK_Node = FK_MainNode;

            cond.ToNodeID = ToNodeID;
            cond.Update();

            return "保存成功!!";
        }
        public string CondByDept_Delete()
        {
            string fk_mainNode = this.GetRequestVal("FK_MainNode");
            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.SQL.ToString();

            Cond deleteCond = new Cond();
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
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
            //  CondOrAnd CondOrAnd = (CondOrAnd)this.GetRequestValInt("CondOrAnd");
            string mypk = fk_mainNode + "_" + toNodeID + "_" + condTypeEnum + "_" + ConnDataFrom.Paras.ToString();

            string sql = this.GetRequestVal("TB_Docs");


            //把其他的条件都删除掉.
            //DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (CondType=" + (int)condTypeEnum + " AND   NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Paras);

            Cond cond = new Cond();
            // cond.Delete(CondAttr.NodeID, fk_mainNode,
            //  CondAttr.ToNodeID, toNodeID,
            //  CondAttr.CondType, (int)condTypeEnum);

            cond.MyPK = mypk;
            cond.HisDataFrom = ConnDataFrom.Paras;

            cond.FK_Node = this.GetRequestValInt("FK_MainNode");
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            cond.FK_Flow = this.FK_Flow;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FK_Flow = this.FK_Flow;
            cond.CondType = condTypeEnum;
            cond.Save();

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
            int i = deleteCond.Delete(CondAttr.FK_Node, fk_mainNode,
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
            BP.Port.StationTypes tps = new StationTypes();
            tps.RetrieveAll();
            ds.Tables.Add(tps.ToDataTableField("StationTypes"));

            //岗位.
            BP.Port.Stations sts = new BP.Port.Stations();
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

        #endregion 按照岗位的方向条件.

    }
}
