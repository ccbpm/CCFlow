using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Template;
using BP.WF.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 流程检查类
    /// 1. 创建修复数据表.
    /// 2. 检查流程设计的合法性.
    /// </summary>
    public class FlowCheckError
    {
        #region 构造方法与属性.
        public DataTable dt = null;
        /// <summary>
        /// 流程
        /// </summary>
        public Flow flow = null;
        /// <summary>
        /// 节点s
        /// </summary>
        public Nodes nds = null;
        /// <summary>
        /// 通用的
        /// </summary>
        public GERpt HisGERpt
        {
            get
            {
                return this.flow.HisGERpt;
            }
        }
        /// <summary>
        /// 流程检查
        /// </summary>
        /// <param name="fl">流程实体</param>
        public FlowCheckError(Flow fl)
        {
            this.flow = fl;
            this.nds = new Nodes(fl.No);
            //构造消息存储.
            dt = new DataTable();
            dt.Columns.Add("InfoType");
            dt.Columns.Add("Msg");
            dt.Columns.Add("NodeID");
            dt.Columns.Add("NodeName");
        }
        /// <summary>
        /// 流程检查
        /// </summary>
        /// <param name="flNo">流程编号</param>
        public FlowCheckError(string flNo)
        {
            this.flow = new Flow(flNo);
            this.nds = new Nodes(this.flow.No);

            //构造消息存储.
            dt = new DataTable();
            dt.Columns.Add("InfoType");
            dt.Columns.Add("Msg");
            dt.Columns.Add("NodeID");
            dt.Columns.Add("NodeName");

        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nd"></param>
        private void AddMsgInfo(string info, Node nd = null)
        {
            AddMsg("信息", info, nd);
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nd"></param>
        private void AddMsgWarning(string info, Node nd = null)
        {
            AddMsg("警告", info, nd);
        }
        private void AddMsgError(string info, Node nd = null)
        {
            AddMsg("错误", info, nd);
        }
        /// <summary>
        /// 增加审核信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="info">消息</param>
        /// <param name="nd">节点</param>
        /// <returns></returns>
        private void AddMsg(string type, string info, Node nd = null)
        {
            DataRow dr = this.dt.NewRow();
            dr[0] = type;
            dr[1] = info;

            if (nd != null)
            {
                dr[2] = nd.NodeID;
                dr[3] = nd.Name;
            }
            this.dt.Rows.Add(dr);
        }
        #endregion 构造方法与属性.

        /// <summary>
        /// 校验流程
        /// </summary>
        /// <returns></returns>
        public void DoCheck()
        {
            BP.DA.Cash.ClearCash();
            try
            {
                //设置自动计算.
                CheckMode_Auto();

                ///检查独立表单的完整性.
                CheckMode_Frms();

                //通用检查.
                CheckMode_Gener();

                //检查数据合并模式.
                CheckMode_SpecTable();

                //节点表单字段数据类型检查 
                CheckModel_FormFields();

                //检查越轨流程,子流程发起.
                CheckModel_SubFlowYanXus();



                //检查报表.
                this.DoCheck_CheckRpt(this.nds);

                //检查焦点字段设置是否还有效.
                CheckMode_FocusField();

                //检查质量考核点.
                CheckMode_EvalModel();

                //检查如果是合流节点必须不能是由上一个节点指定接受人员.
                CheckMode_HeliuAccpterRole();

                Node.CheckFlow(this.flow);

                //创建track.
                Track.CreateOrRepairTrackTable(this.flow.No);

                //如果是引用的表单库的表单，就要检查该表单是否有FID字段，没有就自动增加.
                CheckMode_Ref();
            }
            catch (Exception ex)
            {
                this.AddMsgError("err@" + ex.Message + " " + ex.StackTrace);
            }
        }
        /// <summary>
        /// 通用的检查.
        /// </summary>
        public void CheckMode_Gener()
        {
            //条件集合.
            Conds conds = new Conds(this.flow.No);

            foreach (Node nd in nds)
            {
                //设置它的位置类型.
                nd.SetValByKey(NodeAttr.NodePosType, (int)nd.GetHisNodePosType());

                this.AddMsgInfo("修复&检查节点信息", nd);
                nd.RepareMap(this.flow);

                // 从表检查。
                Sys.MapDtls dtls = new BP.Sys.MapDtls("ND" + nd.NodeID);
                foreach (Sys.MapDtl dtl in dtls)
                {
                    this.AddMsgInfo("检查明细表" + dtl.Name, nd);
                    dtl.HisGEDtl.CheckPhysicsTable();
                }

                MapAttrs mattrs = new MapAttrs("ND" + nd.NodeID);

                #region 对节点的访问规则进行检查

                this.AddMsgInfo("开始对节点的访问规则进行检查", nd);

                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                    case DeliveryWay.FindSpecDeptEmpsInStationlist:
                        if (nd.NodeStations.Count == 0)
                            this.AddMsgInfo("错误:您设置了该节点的访问规则是按岗位，但是您没有为节点绑定岗位。", nd);
                        break;
                    case DeliveryWay.ByDept:
                        if (nd.NodeDepts.Count == 0)
                            this.AddMsgInfo("设置了该节点的访问规则是按部门，但是您没有为节点绑定部门", nd);

                        break;
                    case DeliveryWay.ByBindEmp:
                        if (nd.NodeEmps.Count == 0)
                            this.AddMsgInfo("您设置了该节点的访问规则是按人员，但是您没有为节点绑定人员。", nd);

                        break;
                    case DeliveryWay.BySpecNodeEmp: /*按指定的岗位计算.*/
                    case DeliveryWay.BySpecNodeEmpStation: /*按指定的岗位计算.*/
                        if (nd.DeliveryParas.Trim().Length == 0)
                        {
                            this.AddMsgInfo("您设置了该节点的访问规则是按指定的岗位计算，但是您没有设置节点编号。", nd);
                        }
                        else
                        {
                            if (DataType.IsNumStr(nd.DeliveryParas) == false)
                            {
                                this.AddMsgInfo("您没有设置指定岗位的节点编号，目前设置的为{" + nd.DeliveryParas + "}", nd);
                            }
                        }
                        break;
                    case DeliveryWay.ByDeptAndStation: /*按部门与岗位的交集计算.*/
                        string mysql = string.Empty;
                        //added by liuxc,2015.6.30.
                        //区别集成与BPM模式
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                        {
                            mysql =
                                "SELECT No FROM Port_Emp WHERE No IN (SELECT No FK_Emp FROM Port_Emp WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" +
                                nd.NodeID + "))AND No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation +
                                " WHERE FK_Station IN ( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" +
                                nd.NodeID + " )) ORDER BY No ";
                        }
                        else
                        {
                            mysql = "SELECT pdes.FK_Emp AS No"
                                    + " FROM   Port_DeptEmpStation pdes"
                                    + "        INNER JOIN WF_NodeDept wnd"
                                    + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                    + "             AND wnd.FK_Node = " + nd.NodeID
                                    + "        INNER JOIN WF_NodeStation wns"
                                    + "             ON  wns.FK_Station = pdes.FK_Station"
                                    + "             AND wnd.FK_Node =" + nd.NodeID
                                    + " ORDER BY"
                                    + "        pdes.FK_Emp";
                        }

                        DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                        if (mydt.Rows.Count == 0)
                            this.AddMsgInfo("按照岗位与部门的交集计算错误，没有人员集合{" + mysql + "}", nd);
                        break;
                    case DeliveryWay.BySQL:
                    case DeliveryWay.BySQLAsSubThreadEmpsAndData:
                        if (nd.DeliveryParas.Trim().Length <= 5)
                        {
                            this.AddMsgInfo("您设置了该节点的访问规则是按SQL查询，但是您没有在节点属性里设置查询sql，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.", nd);
                            continue;
                        }

                        string sql = nd.DeliveryParas;
                        foreach (MapAttr item in mattrs)
                        {
                            if (item.IsNum)
                                sql = sql.Replace("@" + item.KeyOfEn, "0");
                            else
                                sql = sql.Replace("@" + item.KeyOfEn, "'0'");
                        }

                        sql = sql.Replace("@WebUser.No", "'ss'");
                        sql = sql.Replace("@WebUser.Name", "'ss'");
                        sql = sql.Replace("@WebUser.FK_DeptName", "'ss'");
                        sql = sql.Replace("@WebUser.FK_Dept", "'ss'");
                       

                        sql = sql.Replace("''''", "''"); //出现双引号的问题.

                        if (sql.Contains("@"))
                        {
                            this.AddMsgError("您编写的sql变量填写不正确，实际执行中，没有被完全替换下来" + sql, nd);
                            continue;
                        }

                        DataTable testDB = null;
                        try
                        {
                            testDB = DBAccess.RunSQLReturnTable(sql);
                        }
                        catch (Exception ex)
                        {
                            this.AddMsgError("您设置了该节点的访问规则是按SQL查询,执行此语句错误." + sql + " err:" + ex.Message, nd);
                            break;
                        }

                        if (testDB.Columns.Contains("no") == false
                            || testDB.Columns.Contains("name") == false)
                        {
                            this.AddMsgError("您设置了该节点的访问规则是按SQL查询，设置的sql不符合规则，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.", nd);
                        }

                        break;
                    case DeliveryWay.ByPreviousNodeFormEmpsField:

                        //去rpt表中，查询是否有这个字段
                        string str = nd.NodeID.ToString().Substring(0, nd.NodeID.ToString().Length - 2);
                        MapAttrs rptAttrs = new BP.Sys.MapAttrs();
                        rptAttrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + str + "Rpt", MapAttrAttr.KeyOfEn);

                        if (rptAttrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, nd.DeliveryParas) == false)
                        {
                            /*检查节点字段是否有FK_Emp字段*/
                            this.AddMsgError("您设置了该节点的访问规则是[06.按上一节点表单指定的字段值作为本步骤的接受人]，但是您没有在节点属性的[访问规则设置内容]里设置指定的表单字段，详细参考开发手册.", nd);
                        }

                        break;
                    case DeliveryWay.BySelected: /* 由上一步发送人员选择 */
                        break;
                    case DeliveryWay.ByPreviousNodeEmp: /* 与上一个节点人员相同. */
                        break;
                    default:
                        break;
                }
                #endregion

                #region 检查节点完成条件，方向条件的定义.
                //设置它没有流程完成条件.
                nd.IsCCFlow = false;

                if (conds.Count != 0)
                {
                    this.AddMsgInfo("开始检查(" + nd.Name + ")方向条件:", nd);

                    foreach (Cond cond in conds)
                    {
                        if (cond.FK_Node == nd.NodeID && cond.HisCondType == CondType.Flow)
                        {
                            nd.IsCCFlow = true;
                            nd.Update();
                        }

                        Node ndOfCond = new Node();
                        ndOfCond.NodeID = ndOfCond.NodeID;
                        if (ndOfCond.RetrieveFromDBSources() == 0)
                            continue;

                        if (cond.AttrKey.Length < 2)
                            continue;
                        if (ndOfCond.HisWork.EnMap.Attrs.Contains(cond.AttrKey) == false)
                        {
                            this.AddMsgError("@错误:属性:" + cond.AttrKey + " , " + cond.AttrName + " 不存在。", nd);
                            continue;
                        }
                    }
                }
                #endregion 检查节点完成条件的定义.
            }
        }

        /// <summary>
        /// 流程属性的预先计算与基础的更新
        /// </summary>
        public void CheckMode_Auto()
        {
            // 设置流程名称.
            DBAccess.RunSQL("UPDATE WF_Node SET FlowName = (SELECT Name FROM WF_Flow WHERE NO=WF_Node.FK_Flow)");

            //设置单据编号只读格式.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET UIIsEnable=0 WHERE KeyOfEn='BillNo' AND UIIsEnable=1");

            //开始节点不能有会签.
            DBAccess.RunSQL("UPDATE WF_Node SET HuiQianRole=0 WHERE NodePosType=0 AND HuiQianRole !=0");

            //开始节点不能有退回.
            DBAccess.RunSQL("UPDATE WF_Node SET ReturnRole=0 WHERE NodePosType=0 AND ReturnRole !=0");

            //删除垃圾,非法数据.
            string sqls = "DELETE FROM Sys_FrmSln WHERE FK_MapData NOT IN (SELECT No from Sys_MapData)";
            sqls += "@ DELETE FROM WF_Direction WHERE Node=ToNode";
            DBAccess.RunSQLs(sqls);

            //更新计算数据.
            this.flow.NumOfBill = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM WF_BillTemplate WHERE NodeID IN (SELECT NodeID FROM WF_Flow WHERE No='" + this.flow.No + "')");
            this.flow.NumOfDtl = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_MapDtl WHERE FK_MapData='ND" + int.Parse(this.flow.No) + "Rpt'");
            this.flow.DirectUpdate();

            //一直没有找到设置3列，自动回到四列的情况.
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET ColSpan=3 WHERE  UIHeight<=23 AND ColSpan=4");
        }
        /// <summary>
        /// 检查独立表单的完整性.
        /// </summary>
        public void CheckMode_Frms()
        {
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.flow.No);
            string frms = "";
            string err = "";
            foreach (FrmNode item in fns)
            {
                if (frms.Contains(item.FK_Frm + ","))
                    continue;
                frms += item.FK_Frm + ",";

                MapData md = new MapData();
                md.No = item.FK_Frm;
                if (md.RetrieveFromDBSources() == 0)
                {
                    this.AddMsgError("节点绑定的表单ID=" + item.FK_Frm + "，但该表单已经不存在.", new Node(item.FK_Node));
                    continue;
                }
            }
        }
        /// <summary>
        /// 如果是引用的表单库的表单，就要检查该表单是否有FID字段，没有就自动增加.
        /// </summary>
        public void CheckMode_Ref()
        {
            foreach (Node nd in nds)
            {
                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    MapAttr mattr = new MapAttr();
                    mattr.MyPK = nd.NodeFrmID + "_FID";
                    if (mattr.RetrieveFromDBSources() == 0)
                    {
                        mattr.KeyOfEn = "FID";
                        mattr.FK_MapData = nd.NodeFrmID;
                        mattr.MyDataType = DataType.AppInt;
                        mattr.UIVisible = false;
                        mattr.Name = "FID(自动增加)";
                        mattr.Insert();

                        GEEntity en = new GEEntity(nd.NodeFrmID);
                        en.CheckPhysicsTable();
                    }
                }
            }
        }
        /// <summary>
        /// 检查是否是数据合并模式
        /// </summary>
        public void CheckMode_SpecTable()
        {
            if (this.flow.HisDataStoreModel != Template.DataStoreModel.SpecTable)
                return;

            foreach (Node nd in nds)
            {
                MapData md = new MapData();
                md.No = "ND" + nd.NodeID;
                if (md.RetrieveFromDBSources() == 1)
                {
                    if (md.PTable != this.flow.PTable)
                    {
                        md.PTable = this.flow.PTable;
                        md.Update();
                    }
                }
            }
        }
        /// <summary>
        /// 检查越轨流程,子流程发起.
        /// </summary>
        public void CheckModel_SubFlowYanXus()
        {
            string msg = "";
            SubFlowYanXus yanxuFlows = new SubFlowYanXus();
            yanxuFlows.Retrieve(SubFlowYanXuAttr.SubFlowNo, this.flow.No);

            foreach (SubFlowYanXu flow in yanxuFlows)
            {
                Flow fl = new Flow(flow.SubFlowNo);

                /* 如果当前为子流程的时候，允许节点自动运行下一步骤，就要确定下一步骤的节点，必须有确定的可以计算的接收人. */
                if (fl.SubFlowOver == SubFlowOver.SendParentFlowToNextStep)
                {
                    Node nd = new Node(flow.FK_Node);
                    if (nd.HisToNodes.Count > 1)
                        this.AddMsgError("@当前节点[" + nd.Name + "]的可以启动子流程或者延续流程.被启动的子流程设置了当子流程结束时让父流程自动运行到下一个节点，但是当前节点有分支，导致流程无法运行到下一个节点.", nd);

                    if (nd.HisToNodes.Count == 1)
                    {
                        Node toNode = nd.HisToNodes[0] as Node;
                        if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                        {
                            msg = "@当前节点[" + nd.Name + "]的可以启动子流程或者延续流程.被启动的子流程设置了当子流程结束时让父流程自动运行到下一个节点，但是当前节点有分支，导致流程无法运行到下一个节点.";
                            this.AddMsgError(msg, nd);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 检查焦点字段设置是否还有效
        /// </summary>
        public void CheckMode_FocusField()
        {
            string msg = "";
            //获得gerpt字段.
            GERpt rpt = this.flow.HisGERpt;
            foreach (Node nd in nds)
            {
                if (nd.FocusField.Trim() == "")
                {
                    Work wk = nd.HisWork;
                    string attrKey = "";
                    foreach (Attr attr in wk.EnMap.Attrs)
                    {
                        if (attr.UIVisible == true && attr.UIIsDoc && attr.UIIsReadonly == false)
                            attrKey = attr.Desc + ":@" + attr.Key;
                    }

                    if (attrKey == "")
                    {
                        msg = "@警告:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的请设置焦点字段.";
                        this.AddMsgWarning(msg, nd);
                    }
                    else
                    {
                        msg = "@信息:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的系统自动设置了焦点字段为" + attrKey + ".";
                        this.AddMsgInfo(msg, nd);

                        nd.FocusField = attrKey;
                        nd.DirectUpdate();
                    }
                    continue;
                }

                string strs = nd.FocusField.Clone() as string;
                strs = Glo.DealExp(strs, rpt, "err");
                if (strs.Contains("@") == true)
                {
                    //@shilianyu. 
                    msg = "@警告:焦点字段（" + nd.FocusField + "）在节点(step:" + nd.Step + " 名称:" + nd.Name + ")属性里的设置已无效，表单里不存在该字段.";
                    this.AddMsgError(msg, nd);
                }

                if (this.flow.IsMD5)
                {
                    if (nd.HisWork.EnMap.Attrs.Contains(WorkAttr.MD5) == false)
                        nd.RepareMap(this.flow);
                }
            }
        }
        /// <summary>
        /// 检查质量考核点
        /// </summary>
        public void CheckMode_EvalModel()
        {
            string msg = "";
            foreach (Node nd in nds)
            {
                if (nd.IsEval)
                {
                    /*如果是质量考核点，检查节点表单是否具别质量考核的特别字段？*/
                    string sql = "SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "' AND KeyOfEn IN ('EvalEmpNo','EvalEmpName','EvalEmpCent')";
                    if (DBAccess.RunSQLReturnValInt(sql) != 3)
                    {
                        msg = "@信息:您设置了节点(" + nd.NodeID + "," + nd.Name + ")为质量考核节点，但是您没有在该节点表单中设置必要的节点考核字段.";
                        this.AddMsgError(msg, nd);
                    }
                }
            }
        }
        /// <summary>
        /// 检查如果是合流节点必须不能是由上一个节点指定接受人员.
        /// </summary>
        /// <returns></returns>
        public void CheckMode_HeliuAccpterRole()
        {
            string msg = "";
            foreach (Node nd in nds)
            {
                //如果是合流节点.
                if (nd.HisNodeWorkType == NodeWorkType.WorkHL || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
                {
                    if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        msg = "@错误:节点ID:" + nd.NodeID + " 名称:" + nd.Name + "是合流或者分合流节点，但是该节点设置的接收人规则为由上一步指定，这是错误的，应该为自动计算而非每个子线程人为的选择.";
                        this.AddMsgError(msg, nd);
                    }
                }

                //子线程节点
                if (nd.HisNodeWorkType == NodeWorkType.SubThreadWork)
                {
                    if (nd.CondModel == CondModel.ByUserSelected)
                    {
                        Nodes toNodes = nd.HisToNodes;
                        if (toNodes.Count == 1)
                        {
                            //msg += "@错误:节点ID:" + nd.NodeID + " 名称:" + nd.Name + " 错误当前节点为子线程，但是该节点的到达.";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 节点表单字段数据类型检查，名字相同的字段出现类型不同的处理方法：依照不同于NDxxRpt表中同名字段类型为基准
        /// </summary>
        /// <returns>检查结果</returns>
        private string CheckModel_FormFields()
        {
            StringBuilder errorAppend = new StringBuilder();
            errorAppend.Append("@信息: -------- 流程节点表单的字段类型检查: ------ ");
            try
            {
                Nodes nds = new Nodes(this.flow.No);
                string fk_mapdatas = "'ND" + int.Parse(this.flow.No) + "Rpt'";
                foreach (Node nd in nds)
                {
                    fk_mapdatas += ",'ND" + nd.NodeID + "'";
                }

                //筛选出类型不同的字段
                string checkSQL = "SELECT   AA.KEYOFEN, COUNT(*) AS MYNUM FROM ("
                                    + "  SELECT A.KEYOFEN,  MYDATATYPE,  COUNT(*) AS MYNUM"
                                    + "  FROM SYS_MAPATTR A WHERE FK_MAPDATA IN (" + fk_mapdatas + ") GROUP BY KEYOFEN, MYDATATYPE"
                                    + ")  AA GROUP BY  AA.KEYOFEN HAVING COUNT(*) > 1";
                DataTable dt_Fields = DBAccess.RunSQLReturnTable(checkSQL);
                foreach (DataRow row in dt_Fields.Rows)
                {
                    string keyOfEn = row["KEYOFEN"].ToString();
                    string myNum = row["MYNUM"].ToString();
                    int iMyNum = 0;
                    int.TryParse(myNum, out iMyNum);

                    //存在2种以上数据类型，有手动进行调整
                    if (iMyNum > 2)
                    {
                        errorAppend.Append("@错误：字段名" + keyOfEn + "在此流程表(" + fk_mapdatas + ")中存在2种以上数据类型(如：int，float,varchar,datetime)，请手动修改。");
                        return errorAppend.ToString();
                    }

                    //存在2种数据类型，以不同于NDxxRpt字段类型为主
                    MapAttr baseMapAttr = new MapAttr();
                    MapAttr rptMapAttr = new MapAttr("ND" + int.Parse(this.flow.No) + "Rpt", keyOfEn);

                    //Rpt表中不存在此字段
                    if (rptMapAttr == null || rptMapAttr.MyPK == "")
                    {
                        this.DoCheck_CheckRpt(this.nds);
                        rptMapAttr = new MapAttr("ND" + int.Parse(this.flow.No) + "Rpt", keyOfEn);
                        this.HisGERpt.CheckPhysicsTable();
                    }

                    //Rpt表中不存在此字段,直接结束
                    if (rptMapAttr == null || rptMapAttr.MyPK == "")
                        continue;

                    foreach (Node nd in nds)
                    {
                        MapAttr ndMapAttr = new MapAttr("ND" + nd.NodeID, keyOfEn);
                        if (ndMapAttr == null || ndMapAttr.MyPK == "")
                            continue;

                        //找出与NDxxRpt表中字段数据类型不同的表单
                        if (rptMapAttr.MyDataType != ndMapAttr.MyDataType)
                        {
                            baseMapAttr = ndMapAttr;
                            break;
                        }
                    }
                    errorAppend.Append("@基础表" + baseMapAttr.FK_MapData + "，字段" + keyOfEn + "数据类型为：" + baseMapAttr.MyDataTypeStr);
                    //根据基础属性类修改数据类型不同的表单
                    foreach (Node nd in nds)
                    {
                        MapAttr ndMapAttr = new MapAttr("ND" + nd.NodeID, keyOfEn);
                        //不包含此字段的进行返回,类型相同的进行返回
                        if (ndMapAttr == null || ndMapAttr.MyPK == "" || baseMapAttr.MyPK == ndMapAttr.MyPK || baseMapAttr.MyDataType == ndMapAttr.MyDataType)
                            continue;

                        ndMapAttr.Name = baseMapAttr.Name;
                        ndMapAttr.MyDataType = baseMapAttr.MyDataType;
                        ndMapAttr.UIWidth = baseMapAttr.UIWidth;
                        ndMapAttr.UIHeight = baseMapAttr.UIHeight;
                        ndMapAttr.MinLen = baseMapAttr.MinLen;
                        ndMapAttr.MaxLen = baseMapAttr.MaxLen;
                        if (ndMapAttr.Update() > 0)
                            errorAppend.Append("@修改了" + "ND" + nd.NodeID + " 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr);
                        else
                            errorAppend.Append("@错误:修改" + "ND" + nd.NodeID + " 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr + "失败。");
                    }
                    //修改NDxxRpt
                    rptMapAttr.Name = baseMapAttr.Name;
                    rptMapAttr.MyDataType = baseMapAttr.MyDataType;
                    rptMapAttr.UIWidth = baseMapAttr.UIWidth;
                    rptMapAttr.UIHeight = baseMapAttr.UIHeight;
                    rptMapAttr.MinLen = baseMapAttr.MinLen;
                    rptMapAttr.MaxLen = baseMapAttr.MaxLen;
                    if (rptMapAttr.Update() > 0)
                        errorAppend.Append("@修改了" + "ND" + int.Parse(this.flow.No) + "Rpt 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr);
                    else
                        errorAppend.Append("@错误:修改" + "ND" + int.Parse(this.flow.No) + "Rpt 表，字段" + keyOfEn + "修改为：" + baseMapAttr.MyDataTypeStr + "失败。");
                }
            }
            catch (Exception ex)
            {
                errorAppend.Append("@错误:" + ex.Message);
            }
            return errorAppend.ToString();
        }
        /// <summary>
        /// 检查数据报表.
        /// </summary>
        /// <param name="nds"></param>
        private void DoCheck_CheckRpt(Nodes nds)
        {
            string fk_mapData = "ND" + int.Parse(this.flow.No) + "Rpt";
            string flowId = int.Parse(this.flow.No).ToString();

            //生成该节点的 nds 比如  "'ND101','ND102','ND103'"
            string ndsstrs = "";
            foreach (BP.WF.Node nd in nds)
            {
                ndsstrs += "'ND" + nd.NodeID + "',";
            }
            ndsstrs = ndsstrs.Substring(0, ndsstrs.Length - 1);

            #region 插入字段。
            string sql = "SELECT distinct KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN (" + ndsstrs + ")";
            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                sql = "SELECT A.* FROM (" + sql + ") AS A ";
                string sql3 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='" + fk_mapData + "' ";
                DBAccess.RunSQL(sql3); // 删除不存在的字段.
            }
            else
            {
                string sql2 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='" + fk_mapData + "' ";
                DBAccess.RunSQL(sql2); // 删除不存在的字段.
            }

            //所有节点表单字段的合集.
            sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN (" + ndsstrs + ")";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求已经存在的字段集合。
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='ND" + flowId + "Rpt'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            //遍历 - 所有节点表单字段的合集
            foreach (DataRow dr in dt.Rows)
            {
                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@") == true)
                    continue;

                string mypk = dr["MyPK"].ToString();

                pks += dr["KeyOfEn"].ToString() + "@";

                //找到这个属性.
                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);

                ma.MyPK = "ND" + flowId + "Rpt_" + ma.KeyOfEn;
                ma.FK_MapData = "ND" + flowId + "Rpt";
                ma.UIIsEnable = false;

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                // 如果不存在.
                if (ma.IsExits == false)
                    ma.Insert();
            }

            MapAttrs attrs = new MapAttrs(fk_mapData);

            // 创建mapData.
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + flowId + "Rpt";
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = this.flow.Name;
                md.PTable = this.flow.PTable;
                md.Insert();
            }
            else
            {
                md.Name = this.flow.Name;
                md.PTable = this.flow.PTable;
                md.Update();
            }
            #endregion 插入字段。

            #region 补充上流程字段到NDxxxRpt.
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case StartWorkAttr.FK_Dept:
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.UIIsEnable = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case "FK_NY":
                        //  attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        attr.GroupID = groupID;
                        attr.Update();
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                attr.Name = "标题"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 400;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "OID";
                attr.Name = "WorkID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }


            if (attrs.Contains(md.No + "_" + GERptAttr.FID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "FID";
                attr.Name = "FID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFState) == false)
            {
                /* 流程状态 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.WFState;
                attr.Name = "流程状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIBindKey = GERptAttr.WFState;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFSta) == false)
            {
                /* 流程状态Ext */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.WFSta;
                attr.Name = "状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIBindKey = GERptAttr.WFSta;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEmps) == false)
            {
                /* 参与人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEmps; // "FlowEmps";
                attr.Name = "参与人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStarter;
                attr.Name = "发起人"; //  
                attr.MyDataType = DataType.AppString;

                //attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStartRDT; // "FlowStartRDT";
                attr.Name = "发起时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnder;
                attr.Name = "结束人"; //  
                attr.MyDataType = DataType.AppString;
                // attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnderRDT; // "FlowStartRDT";
                attr.Name = "结束时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEndNode) == false)
            {
                /* 结束节点 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEndNode;
                attr.Name = "结束节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowDaySpan) == false)
            {
                /* FlowDaySpan */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowDaySpan; // "FlowStartRDT";
                attr.Name = "跨度(天)";
                attr.MyDataType = DataType.AppFloat;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PFlowNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PFlowNo;
                attr.Name = "父流程编号"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 3;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PNodeID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PNodeID;
                attr.Name = "父流程启动的节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PWorkID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PWorkID;
                attr.Name = "父流程WorkID";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PEmp) == false)
            {
                /* 调起子流程的人员 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PEmp;
                attr.Name = "调起子流程的人员";
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.BillNo;
                attr.Name = "单据编号"; //  单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }


            if (attrs.Contains(md.No + "_MyNum") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "条";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "1";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.AtPara;
                attr.Name = "参数"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 4000;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.GUID) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.GUID;
                attr.Name = "GUID"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PrjNo) == false)
            {
                /* 项目编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjNo;
                attr.Name = "项目编号"; //  项目编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }
            if (attrs.Contains(md.No + "_" + GERptAttr.PrjName) == false)
            {
                /* 项目名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjName;
                attr.Name = "项目名称"; //  项目名称
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowNote) == false)
            {
                /* 流程信息 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowNote;
                attr.Name = "流程信息"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 500;
                attr.Idx = -100;
                attr.Insert();
            }
            #endregion 补充上流程字段。

            #region 为流程字段设置分组。
            try
            {
                string flowInfo = "流程信息";
                GroupField flowGF = new GroupField();
                int num = flowGF.Retrieve(GroupFieldAttr.FrmID, fk_mapData, GroupFieldAttr.Lab, "流程信息");
                if (num == 0)
                {
                    flowGF = new GroupField();
                    flowGF.Lab = flowInfo;
                    flowGF.FrmID = fk_mapData;
                    flowGF.Idx = -1;
                    flowGF.Insert();
                }
                sql = "UPDATE Sys_MapAttr SET GroupID='" + flowGF.OID + "' WHERE  FK_MapData='" + fk_mapData + "'  AND KeyOfEn IN('" + GERptAttr.PFlowNo + "','" + GERptAttr.PWorkID + "','" + GERptAttr.MyNum + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FK_NY + "','" + GERptAttr.FlowDaySpan + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEnder + "','" + GERptAttr.FlowEnderRDT + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.FlowStarter + "','" + GERptAttr.FlowStartRDT + "','" + GERptAttr.WFState + "')";
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
            }
            #endregion 为流程字段设置分组

            #region 尾后处理.
            GERpt gerpt = this.HisGERpt;
            gerpt.CheckPhysicsTable();  //让报表重新生成.

            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE FrmID='" + fk_mapData + "' AND OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");

            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='活动时间' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='CDT'");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='参与者' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='Emps'");
            #endregion 尾后处理.
        }
    }
}
