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
    public class WF_Admin : DirectoryPageBase
    {
        #region 属性.
        public string RefNo
        {
            get
            {
                return this.GetRequestVal("RefNo");
            }
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
            BP.Port.Stations sts = new Stations();
            sts.RetrieveAll();
            ds.Tables.Add(tps.ToDataTableField("Stations"));


            //取有可能存盘的数据.
            int FK_MainNode = this.GetRequestValInt("FK_MainNode");
            int ToNodeID = this.GetRequestValInt("ToNodeID");
            Cond cond = new Cond();
            string mypk = FK_MainNode + "_" + ToNodeID + "_Dir_" + ConnDataFrom.Stas.ToString();
            cond.MyPK = mypk;
            cond.RetrieveFromDBSources();
            ds.Tables.Add(cond.ToDataTableField("Cond"));

            return BP.Tools.Json.DataSetToJson(ds);


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



        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 初始化Init.
        /// </summary>
        /// <returns></returns>
        public string Condition_Init()
        {
            string toNodeID = this.GetRequestVal("ToNodeId");
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
            string sql = "SELECT A.NodeID, A.Name FROM WF_Node A,  WF_Direction B WHERE A.NodeID=B.ToNode AND B.Node="+this.FK_Node;

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
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ( NodeID=" + this.FK_Node + " AND ToNodeID=" + toNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Form);

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.Form;
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
            qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.Form);
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

            string sql = "UPDATE WF_Cond SET DataFrom=" + (int)ConnDataFrom.Form + " WHERE NodeID=" + cond.NodeID + "  AND FK_Node=" + cond.FK_Node + " AND ToNodeID=" + toNodeID;
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

        #region 按照部门条件计算》
        /// <summary>
        /// 按照部门条件计算.
        /// </summary>
        /// <returns></returns>
        public string CondByDept_Init()
        {
            CondType condType = (CondType)this.GetRequestValInt("CondType");
            string mypk = this.GetRequestValInt("FK_MainNode") + "_" + this.GetRequestValInt("ToNodeID") + "_" + condType.ToString() + "_" + ConnDataFrom.Depts.ToString();

            Cond cond = new Cond();
            cond.MyPK = mypk;
            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.Row.Add("Nums", 0);
                cond.SpecOperPara = "";
            }
            else
            {
                string[] strs = cond.OperatorValue.ToString().Split('@');
                cond.Row.Add("Nums", strs.Length);

                if (cond.SpecOperPara == "")
                    cond.SpecOperPara = "";
            }
            return cond.ToJson();
        }
        public string CondByDept_Save()
        {
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

            string val = "";

            //Depts sts = new Depts();
            //sts.RetrieveAllFromDBSource();
            //foreach (Dept st in sts)
            //{
            //    if (this.Pub1.IsExit("CB_" + st.No) == false)
            //        continue;
            //    if (this.Pub1.GetCBByID("CB_" + st.No).Checked)
            //        val += "@" + st.No;
            //}

            if (val == "")
                cond.Delete();

            val += "@";
            cond.OperatorValue = val;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = condType;
            cond.FK_Node = this.FK_Node;
            cond.ToNodeID = this.GetRequestValInt("ToNodeID");

            #region //获取“指定的操作员”设置，added by liuxc,2015-10-7

            int specOperWay = this.GetRequestValInt("DDL_SpecOperWay");
            cond.SpecOperWay = (SpecOperWay)specOperWay;

            if (cond.SpecOperWay != SpecOperWay.CurrOper)
            {
                cond.SpecOperPara = this.GetRequestVal("TB_SpecOperPara"); // Pub1.GetTBByID("TB_" + CondAttr.SpecOperPara).Text;
            }
            else
            {
                cond.SpecOperPara = string.Empty;
            }
            #endregion

            switch (condType)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.Update();
                    break;
                case CondType.Dir:
                    cond.ToNodeID = this.GetRequestValInt("ToNodeID");
                    cond.Update();
                    break;
                case CondType.SubFlow:
                    cond.ToNodeID = this.GetRequestValInt("ToNodeID");
                    cond.Update();
                    break;
                default:
                    throw new Exception("未设计的情况。");
            }

            return "保存成功!!";
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

        #region 测试页面.
        /// <summary>
        /// 初始化界面.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_Init()
        {
            //清除缓存.
            BP.Sys.SystemConfig.DoClearCash();

            // 让admin 登录.
            BP.WF.Dev2Interface.Port_Login("admin");

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGener(emp);
                context.Session["FK_Flow"] = this.FK_Flow;
                return "url@../MyFlow.htm?FK_Flow=" + this.FK_Flow;
            }

            Flow fl = new Flow(this.FK_Flow);
            //    fl.DoCheck();

            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt = null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);

            if (nd.IsGuestNode)
            {
                /*如果是guest节点，就让其跳转到 guest登录界面，让其发起流程。*/

                //这个地址需要配置.
                return "url@/SDKFlowDemo/GuestApp/Login.aspx?FK_Flow=" + this.FK_Flow;
            }

            try
            {

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.ByStationOnly:
                        // edit by stone , 如果是BPM 就不能工作.
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                            sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_EmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_Empstation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        else
                            sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                        // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                        break;
                    case DeliveryWay.ByDept:
                        sql = "select No,Name from Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("");
                        break;
                    case DeliveryWay.ByBindEmp:
                        sql = "select No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                        //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                        break;
                    case DeliveryWay.ByDeptAndStation:
                        //added by liuxc,2015.6.30.
                        //区别集成与BPM模式
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                        {
                            sql = "SELECT No FROM Port_Emp WHERE No IN ";
                            sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept IN ";
                            sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nodeid + ")";
                            sql += ")";
                            sql += "AND No IN ";
                            sql += "(";
                            sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                            sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeid + ")";
                            sql += ") ORDER BY No ";
                        }
                        else
                        {
                            sql = "SELECT pdes.FK_Emp AS No"
                                  + " FROM   Port_DeptEmpStation pdes"
                                  + "        INNER JOIN WF_NodeDept wnd"
                                  + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                  + "             AND wnd.FK_Node = " + nodeid
                                  + "        INNER JOIN WF_NodeStation wns"
                                  + "             ON  wns.FK_Station = pdes.FK_Station"
                                  + "             AND wnd.FK_Node =" + nodeid
                                  + " ORDER BY"
                                  + "        pdes.FK_Emp";
                        }
                        break;
                    case DeliveryWay.BySelected: //所有的人员多可以启动, 2016年11月开始约定此规则.
                        sql = "SELECT No as FK_Emp FROM Port_Emp ";
                        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count > 300)
                        {
                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
                                sql = "SELECT top 300 No as FK_Emp FROM Port_Emp ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE ROWNUM <300 ";

                            if (SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                                sql = "SELECT  No as FK_Emp FROM Port_Emp WHERE limit 0,300 ";
                        }
                        break;
                    case DeliveryWay.BySQL:
                        if (string.IsNullOrEmpty(nd.DeliveryParas))
                            return "err@您设置的按SQL访问开始节点，但是您没有设置sql.";
                        break;
                    default:
                        break;
                }

                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return "err@您按照:" + nd.HisDeliveryWay + "的方式设置的开始节点的访问规则，但是开始节点没有人员。";

                // 构造人员表.
                DataTable dtEmps = new DataTable();
                dtEmps.Columns.Add("No");
                dtEmps.Columns.Add("Name");
                dtEmps.Columns.Add("FK_DeptText");

                //处理发起人数据.
                string emps = "";
                foreach (DataRow dr in dt.Rows)
                {
                    string myemp = dr[0].ToString();
                    if (emps.Contains("," + myemp + ",") == true)
                        continue;

                    emps += "," + myemp + ",";
                    BP.Port.Emp emp = new Emp(myemp);

                    DataRow drNew = dtEmps.NewRow();

                    drNew["No"] = emp.No;
                    drNew["Name"] = emp.Name;
                    drNew["FK_DeptText"] = emp.FK_DeptText;

                    dtEmps.Rows.Add(drNew);
                }
               
                //返回数据源.
                return BP.Tools.Json.DataTableToJson(dtEmps, false);
            }
            catch (Exception ex)
            {
                return "err@<h2>您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员，<a href='http://bbs.ccflow.org/showtopic-4103.aspx' target=_blank ><font color=red>点击这查看解决办法</font>.</a>。</h2> 系统错误提示:" + ex.Message + "<br><h3>也有可能你你切换了OSModel导致的，什么是OSModel,请查看在线帮助文档 <a href='http://ccbpm.mydoc.io' target=_blank>http://ccbpm.mydoc.io</a>  .</h3>";
            }
        }
        /// <summary>
        /// 转到指定的url.
        /// </summary>
        /// <returns></returns>
        public string TestFlow_ReturnToUser()
        {
            string userNo = this.GetRequestVal("UserNo");
            string sid = BP.WF.Dev2Interface.Port_Login(userNo);
            string url = "../../WF/Port.htm?UserNo=" + userNo + "&SID=" + sid + "&DoWhat=" + this.GetRequestVal("DoWhat") + "&FK_Flow=" + this.FK_Flow + "&IsMobile=" + this.GetRequestVal("IsMobile");
            return "url@" + url;
        }
        #endregion 测试页面.

        #region 安装.
        public string DBInstall_Init()
        {
            if (DBAccess.TestIsConnection() == false)
                return "err@数据库连接配置错误 AppCenterDSN, AppCenterDBType 参数配置. ccflow请检查 web.config文件, jflow请检查 jflow.properties.";

            if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == true)
                return "err@info数据库已经安装上了，您不必在执行安装. 点击:<a href='./CCBPMDesigner/Login.htm' >这里直接登录流程设计器</a>";

            Hashtable ht = new Hashtable();
            ht.Add("OSModel", (int)BP.WF.Glo.OSModel); //组织结构类型.
            ht.Add("DBType", SystemConfig.AppCenterDBType.ToString()); //数据库类型.
            ht.Add("Ver", BP.WF.Glo.Ver); //版本号.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        public string DBInstall_Submit()
        {
            string lang = "CH";

            //是否要安装demo.
            int demoTye = this.GetRequestValInt("DemoType");

            //运行ccflow的安装.
            BP.WF.Glo.DoInstallDataBase(lang, demoTye);

            //执行ccflow的升级。
            BP.WF.Glo.UpdataCCFlowVer();

            //加注释.
            BP.Sys.PubClass.AddComment();

            return "info@系统成功安装 点击:<a href='./CCBPMDesigner/Login.htm' >这里直接登录流程设计器</a>";
            // this.Response.Redirect("DBInstall.aspx?DoType=OK", true);
        }
        #endregion

        #region 表单方案.
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string BindFrms_Init()
        {
            //注册这个枚举，防止第一次运行出错.
            BP.Sys.SysEnums ses = new SysEnums("FrmEnableRole");

            string text = "";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            //FrmNodeExt fns = new FrmNodeExt(this.FK_Flow, this.FK_Node);

            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);

            #region 如果没有ndFrm 就增加上.
            bool isHaveNDFrm = false;
            foreach (FrmNode fn in fns)
            {
                if (fn.FK_Frm == "ND" + this.FK_Node)
                {
                    isHaveNDFrm = true;
                    break;
                }
            }

            if (isHaveNDFrm == false)
            {
                FrmNode fn = new FrmNode();
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Frm = "ND" + this.FK_Node;
                fn.FK_Node = this.FK_Node;

                fn.FrmEnableRole = FrmEnableRole.Disable; //就是默认不启用.
                fn.FrmSln = 0;
                //  fn.IsEdit = true;
                fn.IsEnableLoadData = true;
                fn.Insert();
                fns.AddEntity(fn);
            }
            #endregion 如果没有ndFrm 就增加上.

            //组合这个实体才有外键信息.
            FrmNodeExts fnes = new FrmNodeExts();
            foreach (FrmNode fn in fns)
            {
                MapData md = new MapData();
                md.No = fn.FK_Frm;
                if (md.IsExits == false)
                {
                    fn.Delete();  //说明该表单不存在了，就需要把这个删除掉.
                    continue;
                }

                FrmNodeExt myen = new FrmNodeExt(fn.MyPK);
                fnes.AddEntity(myen);
            }

            //把json数据返回过去.
            return fnes.ToJson();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string BindFrms_Delete()
        {
            FrmNodeExt myen = new FrmNodeExt(this.MyPK);
            myen.Delete();
            return "删除成功.";
        }

        public string BindFrms_DoOrder()
        {
            FrmNode myen = new FrmNode(this.MyPK);

            if (this.GetRequestVal("OrderType") == "Up")
                myen.DoUp();
            else
                myen.DoDown();

            return "执行成功...";
        }

        #endregion 表单方案.


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


        public string ReLoginSubmit()
        {
            string userNo = this.GetValFromFrmByKey("TB_UserNo");
            string password = this.GetValFromFrmByKey("TB_Pass");

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";

            if (emp.CheckPass(password) == false)
                return "err@用户名或密码错误.";

            BP.Web.WebUser.SignInOfGener(emp);

            return "登录成功.";
        }
        /// <summary>
        /// 加载模版.
        /// </summary>
        /// <returns></returns>
        public string SettingTemplate_Init()
        {
            //类型.
            string templateType = this.GetRequestVal("TemplateType");
            string condType = this.GetRequestVal("CondType");

            BP.WF.Template.SQLTemplates sqls = new SQLTemplates();
            //sqls.Retrieve(BP.WF.Template.SQLTemplateAttr.SQLType, sqlType);

            DataTable dt = null;
            string sql = "";

            #region 节点方向条件模版.
            if (templateType == "CondBySQL")
            {
                /*方向条件, 节点方向条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.SQL;
            }

            if (templateType == "CondByUrl")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Url;
            }

            if (templateType == "CondByPara")
            {
                /*方向条件, 节点方向url条件.*/
                sql = "SELECT MyPK,Note,OperatorValue FROM WF_Cond WHERE CondType=" + condType + " AND DataFrom=" + (int)ConnDataFrom.Paras;
            }
            #endregion 节点方向条件模版.

            #region 表单扩展设置.
            if (templateType == "DDLFullCtrl")
                sql = "SELECT MyPK, '下拉框:'+ a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='DDLFullCtrl'";

            if (templateType == "ActiveDDL")
                sql = "SELECT MyPK, '下拉框:'+ a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='ActiveDDL'";

            //显示过滤.
            if (templateType == "AutoFullDLL")
                sql = "SELECT MyPK, '下拉框:'+ a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFullDLL'";

            //文本框自动填充..
            if (templateType == "TBFullCtrl")
                sql = "SELECT MyPK, '文本框:'+ a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='TBFullCtrl'";

            //自动计算.
            if (templateType == "AutoFull")
                sql = "SELECT MyPK, 'ID:'+ a.AttrOfOper as Name,Doc FROM Sys_MapExt a  WHERE ExtType='AutoFull'";
            #endregion 表单扩展设置.

            #region 节点属性的模版.
            //自动计算.
            if (templateType == "NodeAccepterRole")
                sql = "SELECT NodeID, FlowName +' - '+Name, a.DeliveryParas as Docs FROM WF_Node a WHERE  a.DeliveryWay=" + (int)DeliveryWay.BySQL;
            #endregion 节点属性的模版.

            if (sql == "")
                return "err@没有涉及到的标记[" + templateType + "].";

            dt = DBAccess.RunSQLReturnTable(sql);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                BP.WF.Template.SQLTemplate en = new SQLTemplate();
                en.No = dr[0].ToString();
                en.Name = dr[1].ToString();
                en.Docs = dr[2].ToString();

                if (strs.Contains(en.Docs.Trim() + ";") == true)
                    continue;
                strs += en.Docs.Trim() + ";";
                sqls.AddEntity(en);
            }


            return sqls.ToJson();
        }
        #region 绑定流程表单
        /// <summary>
        /// 获取流程所有节点
        /// </summary>
        /// <returns></returns>
        public string BindForm_GenderFlowNode()
        {
            //规范做法.
            Nodes nds = new Nodes(this.FK_Flow);
            return nds.ToJson();


            // 屏蔽一下代码.
            StringBuilder append = new StringBuilder();
            Flow flow = new Flow(this.FK_Flow);
            append.Append("[");
            foreach (Node node in flow.HisNodes)
            {
                append.Append("{No:'" + node.NodeID + "',Name:'" + node.Name + "'},");
            }
            if (flow.HisNodes.Count > 0)
                append.Remove(append.Length - 1, 1);
            append.Append("]");
            return append.ToString();
        }

        /// <summary>
        /// 获取表单库所有表单
        /// </summary>
        /// <returns></returns>
        public string BindForm_GenerForms()
        {
            //形成树
            FlowFormTrees appendFormTrees = new FlowFormTrees();
            //节点绑定表单
            FrmNodes frmNodes = new FrmNodes(this.FK_Flow, this.FK_Node);
            //所有表单类别
            SysFormTrees formTrees = new SysFormTrees();
            formTrees.RetrieveAll(SysFormTreeAttr.Idx);

            //根节点
            BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
            root.No = "00";
            root.ParentNo = "0";
            root.Name = "表单库";
            root.NodeType = "root";
            appendFormTrees.AddEntity(root);

            foreach (SysFormTree formTree in formTrees)
            {
                //已经添加排除
                if (appendFormTrees.Contains("No", formTree.No) == true)
                    continue;
                //根节点排除
                if (formTree.No.Equals("0"))
                    continue;
                //文件夹
                BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                nodeFolder.No = formTree.No;
                nodeFolder.ParentNo = formTree.ParentNo;
                nodeFolder.Name = formTree.Name;
                nodeFolder.NodeType = "folder";
                if (formTree.ParentNo.Equals("0"))
                    nodeFolder.ParentNo = root.No;
                appendFormTrees.AddEntity(nodeFolder);

                //表单
                MapDatas mapS = new MapDatas();
                mapS.RetrieveByAttr(MapDataAttr.FK_FormTree, formTree.No);
                if (mapS != null && mapS.Count > 0)
                {
                    foreach (MapData map in mapS)
                    {
                        BP.WF.Template.FlowFormTree formFolder = new BP.WF.Template.FlowFormTree();
                        formFolder.No = map.No;
                        formFolder.ParentNo = map.FK_FormTree;
                        formFolder.Name = map.Name + "[" + map.No + "]";
                        formFolder.NodeType = "form";
                        appendFormTrees.AddEntity(formFolder);
                    }
                }
            }

            string strCheckedNos = "";
            //设置选中
            foreach (FrmNode frmNode in frmNodes)
            {
                strCheckedNos += "," + frmNode.FK_Frm + ",";
            }
            //重置
            appendMenus.Clear();
            //生成数据
            TansEntitiesToGenerTree(appendFormTrees, root.No, strCheckedNos);
            return appendMenus.ToString();
        }

        /// <summary>
        /// 保存流程表单
        /// </summary>
        /// <returns></returns>
        public string BindForm_SaveBindForms()
        {
            try
            {
                string formNos = this.context.Request["formNos"];

                FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
                //删除已经删除的。
                foreach (FrmNode fn in fns)
                {
                    if (formNos.Contains("," + fn.FK_Frm + ",") == false)
                    {
                        fn.Delete();
                        continue;
                    }
                }

                // 增加集合中没有的。
                string[] strs = formNos.Split(',');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    if (fns.Contains(FrmNodeAttr.FK_Frm, s))
                        continue;

                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = s;
                    fn.FK_Flow = this.FK_Flow;
                    fn.FK_Node = this.FK_Node;
                    fn.Save();
                }
                return "true";
            }
            catch (Exception ex)
            {
                return "err:保存失败。";
            }
        }

        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityMultiTree root = ens.GetEntityByKey(rootNo) as EntityMultiTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");
            appendMenus.Append(",\"state\":\"open\"");

            //attributes
            BP.WF.Template.FlowFormTree formTree = root as BP.WF.Template.FlowFormTree;
            if (formTree != null)
            {
                string url = formTree.Url == null ? "" : formTree.Url;
                url = url.Replace("/", "|");
                appendMenus.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"Url\":\"" + url + "\"}");
            }
            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens, checkIds);
            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        public void AddChildren(EntityMultiTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityMultiTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");


                //attributes
                BP.WF.Template.FlowFormTree formTree = item as BP.WF.Template.FlowFormTree;
                if (formTree != null)
                {
                    string url = formTree.Url == null ? "" : formTree.Url;
                    string ico = "icon-tree_folder";
                    string treeState = "closed";
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form")
                    {
                        ico = "icon-sheet";
                    }
                    appendMenuSb.Append(",\"state\":\"" + treeState + "\"");
                    appendMenuSb.Append(",iconCls:\"");
                    appendMenuSb.Append(ico);
                    appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
        #endregion

    }
}
