using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Template;
using BP.Difference;
using System.Web;

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
            dirs.Retrieve(DirectionAttr.Node, this.NodeID, DirectionAttr.Idx);
            return dirs.ToJson();
            //按照条件的先后计算.
            Conds cds = new Conds();
            cds.Retrieve(CondAttr.FK_Node, this.NodeID,
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
        /// <summary>
        /// 移动.
        /// </summary>
        /// <returns></returns>
        public string CondPRI_Move()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                string enNo = ens[i];
                string sql = "UPDATE WF_Direction SET Idx=" + i + " WHERE MyPK='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "顺序移动成功..";
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
            Cond cond = new Cond();
            cond.Retrieve(CondAttr.FK_Node, this.NodeID, CondAttr.ToNodeID, toNodeID);
            cond.Row.Add("HisDataFrom", cond.HisDataFrom.ToString());

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
            ps.SQL = "SELECT A.NodeID, A.Name FROM WF_Node A,  WF_Direction B WHERE A.NodeID=B.ToNode AND B.Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Node";
            ps.Add("Node", this.NodeID);

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.Columns[0].ColumnName = "NodeID";
            dt.Columns[1].ColumnName = "Name";
            return BP.Tools.Json.ToJson(dt);
        }
        #region 方向条件-审核组件
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByWorkCheck_Save()
        {

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string sql = this.GetRequestVal("TB_Docs");
            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.WorkCheck;
            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.ToNodeID;
            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.
            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }


            return "保存成功..";
        }

        #endregion

        #region 方向条件URL
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByUrl_Save()
        {

            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string sql = this.GetRequestVal("TB_Docs");
            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.Url;

            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.ToNodeID;

            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功..";
        }
        #endregion

        #region WebApi

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByWebApi_Save()
        {

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string sql = this.GetRequestVal("TB_Docs");
            string atParas = this.GetRequestVal("TB_AtParas");

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.WebApi;

            if (this.GetRequestValInt("FK_MainNode") == 0)
                cond.NodeID = this.NodeID;
            else
                cond.NodeID = this.GetRequestValInt("FK_MainNode");
            if (this.ToNodeID == 0)
                cond.ToNodeID = this.NodeID;
            else
                cond.ToNodeID = this.ToNodeID;

            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql; 
            cond.Note = this.GetRequestVal("TB_Note"); //备注.
            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            // cond.OperatorValue = atParas; //在存储一遍.
            cond.SetPara("OperatorValue", atParas);
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功..";
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
            Node nd = new Node(this.NodeID);

            string frmID = this.FrmID;
            if (DataType.IsNullOrEmpty(frmID) == true)
                frmID = "ND" + int.Parse(nd.FlowNo) + "Rpt";

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            //增加条件.
            if (DataType.IsNullOrEmpty(this.MyPK) == false)
            {
                Cond cond = new Cond(this.MyPK);
                ds.Tables.Add(cond.ToDataTableField("WF_Cond"));
            }

            string noteIn = "'FID','PRI','PNodeID','PrjNo', 'PrjName', 'FK_NY','FlowDaySpan', 'Rec','CDT','RDT','AtPara','WFSta','FlowNote','FlowStartRDT','FlowEnderRDT','FlowEnder','FlowSpanDays','WFState','OID','PWorkID','PFlowNo','PEmp','FlowEndNode','GUID'";

            //增加字段集合.
            string sql = "";
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.KingBaseR3 || SystemConfig.AppCenterDBType == DBType.KingBaseR6 || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn||' - '||Name as Name FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "'";
                sql += " AND KeyOfEn Not IN (" + noteIn + ") ";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT KeyOfEn as No, CONCAT(KeyOfEn,' - ', Name ) as Name FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "'";
                sql += " AND KeyOfEn Not IN (" + noteIn + ") ";
                sql += " AND MyDataType NOT IN (6,7) ";
            }
            else
            {
                sql = "SELECT KeyOfEn as No, KeyOfEn+' - '+Name as Name FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "'";
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
            attr.setMyPK("ND" + int.Parse(this.FlowNo) + "Rpt_" + this.KeyOfEn);
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
            field = "ND" + int.Parse(this.FlowNo) + "Rpt_" + field;

            MapAttr attr = new MapAttr(field);

            int toNodeID = this.ToNodeID;
            string oper = this.GetRequestVal("DDL_Operator");

            string operVal = this.GetRequestVal("OperVal");
            string operValT = this.GetRequestVal("OperValText");

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.NodeForm;
            cond.DataFromText = "表单字段";

            cond.ToNodeID = toNodeID;

            cond.NodeID = this.NodeID;
            cond.OperatorNo = oper;
            cond.OperatorValue = operVal; //操作值.
            cond.OperatorValueT = operValT;

            cond.AttrNo = field; //字段属性.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;

            cond.Note = "表单[" + attr.FrmID + "]字段:[" + attr.KeyOfEn + "," + attr.Name + "][" + oper + "][" + operValT + "]";

            //#region 方向条件，全部更新.
            //Conds conds = new Conds();
            //QueryObject qo = new QueryObject(conds);
            //qo.AddWhere(CondAttr.FK_Node, this.NodeID);
            //qo.addAnd();
            //qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.NodeForm);
            //qo.addAnd();
            //qo.AddWhere(CondAttr.CondType, (int)condTypeEnum);
            //if (toNodeID != 0)
            //{
            //    qo.addAnd();
            //    qo.AddWhere(CondAttr.ToNodeID, toNodeID);
            //} 
            //int num = qo.DoQuery();
            //#endregion

            switch (condTypeEnum)
            {
                case Template.CondType.Flow:
                case Template.CondType.Node:

                    break;
                case Template.CondType.Dir:
                case Template.CondType.SubFlow: //启动子流程.
                    cond.ToNodeID = toNodeID;
                    break;
                default:
                    throw new Exception("未设计的情况。" + condTypeEnum.ToString());
            }
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
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
            ps.SQL = "SELECT m.No, m.Name, n.FK_Node, n.FK_Flow FROM WF_FrmNode n INNER JOIN Sys_MapData m ON n.FK_Frm=m.No WHERE n.FrmEnableRole!=5 AND n.FK_Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Node";
            ps.Add("FK_Node", this.NodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Frms";
            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";

            //@gaoxin. 
            DataRow dr = dt.NewRow();
            dr[0] = "ND" + int.Parse(this.FlowNo) + "Rpt";
            dr[1] = "节点表单(内置表单)";
            dt.Rows.Add(dr);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            //增加条件集合.
            string toNodeID = this.GetRequestVal("ToNodeID");

            // 增加条件.
            if (DataType.IsNullOrEmpty(this.MyPK) == false)
            {
                Cond cond = new Cond(this.MyPK);
                ds.Tables.Add(cond.ToDataTableField("WF_Cond"));
            }

            return BP.Tools.Json.DataSetToJson(ds, false);
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

            int toNodeID = this.ToNodeID;
            string oper = this.GetRequestVal("DDL_Operator");
            string operVal = this.GetRequestVal("OperVal");

            //节点,子线城,还是其他
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.StandAloneFrm;
            cond.ToNodeID = toNodeID;

            cond.NodeID = this.NodeID;
            cond.OperatorNo = oper;
            cond.OperatorValue = operVal; //操作值.

            cond.AttrNo = field; //字段属性.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;

            //#region 方向条件，全部更新.
            //Conds conds = new Conds();
            //QueryObject qo = new QueryObject(conds);
            //qo.AddWhere(CondAttr.FK_Node, this.NodeID);
            //qo.addAnd();
            //qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.StandAloneFrm);
            //qo.addAnd();
            //qo.AddWhere(CondAttr.CondType, (int)condTypeEnum);
            //if (toNodeID != 0)
            //{
            //    qo.addAnd();
            //    qo.AddWhere(CondAttr.ToNodeID, toNodeID);
            //}
            //int num = qo.DoQuery();
            //#endregion

            /* 执行同步*/

            switch (condTypeEnum)
            {
                case Template.CondType.Flow:
                case Template.CondType.Node:
                    break;
                case Template.CondType.Dir:
                case Template.CondType.SubFlow:
                    cond.ToNodeID = toNodeID;
                    break;
                default:
                    throw new Exception("未设计的情况。" + condTypeEnum.ToString());
            }
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }
            return "保存成功!!";
        }

        public string StandAloneFrm_InitField()
        {
            //字段属性.
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_" + this.KeyOfEn);
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
            if (attr.ItIsNum)
            {
                DataTable dtOperNumber = new DataTable();
                dtOperNumber.TableName = "Opers";
                dtOperNumber.Columns.Add("No", typeof(string));
                dtOperNumber.Columns.Add("Name", typeof(string));

                if (attr.MyDataType == BP.DA.DataType.AppBoolean)
                {
                    DataRow dr = dtOperNumber.NewRow();
                    dr["No"] = "dengyu";
                    dr["Name"] = "= 等于";
                    dtOperNumber.Rows.Add(dr);
                }
                else
                {
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
                }

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
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondBySQLTemplate_Save()
        {

            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string sql = this.GetRequestVal("TB_Docs");
            string sqlT = this.GetRequestVal("SqlValT");
            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.SQLTemplate;

            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.ToNodeID;

            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql;
            cond.OperatorValueT = sqlT;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }
            return "保存成功..";
        }

        #endregion 方向条件SQL 模版

        #region 方向条件SQL

        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondBySQL_Save()
        {
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");

            string sql = this.GetRequestVal("TB_Docs");
            string FK_DBSrc = this.GetRequestVal("FK_DBSrc");

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.SQL;
            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.ToNodeID;

            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql;
            cond.DBSrcNo = FK_DBSrc;
            cond.Note = this.GetRequestVal("TB_Note"); //备注.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功..";
        }

        #endregion

        #region 方向条件角色

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByStation_Save()
        {

            int ToNodeID = this.ToNodeID;

            Cond cond = new Cond();

            string val = this.GetRequestVal("Stations").Replace(",", "@");
            string valT = this.GetRequestVal("StationNames");
            cond.OperatorValue = val;
            cond.OperatorValueT = valT;
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
            cond.FlowNo = this.FlowNo;
            cond.NodeID = FK_MainNode;

            cond.ToNodeID = ToNodeID;
            cond.CondType = (BP.WF.Template.CondType)this.GetRequestValInt("CondType"); //条件类型. Dir,Node,Flow

            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功..";
        }

        #endregion

        #region 按照部门条件计算CondByDept_Delete
        public string CondByDept_Save()
        {

            CondType condType = (CondType)this.CondType;
            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.Depts;
            cond.NodeID = this.FK_MainNode;
            cond.RefFlowNo = this.FlowNo;
            cond.FlowNo = this.FlowNo;
            cond.ToNodeID = this.ToNodeID;
            cond.CondTypeInt = this.CondType;

            string val = this.GetRequestVal("depts").Replace(",", "@");
            string valT = this.GetRequestVal("deptNames");
            cond.OperatorValue = val;
            cond.OperatorValueT = valT;
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
            cond.DataFromText = "部门条件";

            cond.FlowNo = this.FlowNo;
            cond.CondTypeInt = this.CondType;
            cond.NodeID = this.FK_MainNode;

            cond.ToNodeID = this.ToNodeID;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功!!";
        }

        #endregion

        public int FK_MainNode
        {
            get
            {
                int fk_mainNode = this.GetRequestValInt("FK_MainNode");
                if (fk_mainNode == 0)
                    fk_mainNode = this.GetRequestValInt("FK_Node");
                return fk_mainNode;
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int CondType
        {
            get
            {
                int val = this.GetRequestValInt("CondType");
                return val;
            }
        }
        public int ToNodeID
        {
            get
            {
                int val = this.GetRequestValInt("ToNodeID");
                return val;
            }
        }

        #region 方向条件Para

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string CondByPara_Save()
        {

            string toNodeID = this.GetRequestVal("ToNodeID");
            CondType condTypeEnum = (CondType)this.GetRequestValInt("CondType");
            string sql = HttpUtility.UrlDecode(this.GetRequestVal("TB_Docs"), System.Text.Encoding.UTF8);

            Cond cond = new Cond();

            cond.HisDataFrom = ConnDataFrom.Paras;

            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.ToNodeID;

            cond.FlowNo = this.FlowNo;
            cond.OperatorValue = sql;
            cond.Note = HttpUtility.UrlDecode(this.GetRequestVal("TB_Note"), System.Text.Encoding.UTF8); //备注.

            cond.FlowNo = this.FlowNo;
            cond.CondType = condTypeEnum;
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
            {
                cond.setMyPK(DBAccess.GenerGUID());
                cond.Insert();
            }
            else
            {
                cond.setMyPK(this.MyPK);
                cond.Update();
            }

            return "保存成功..";
        }

        #endregion


    }
}
