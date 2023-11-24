using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Template;
using BP.WF.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template.SFlow;


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
            dt.Columns.Add("ChekOption"); //检查的项目.
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
            dt.Columns.Add("ChekOption"); //检查的项目.
            dt.Columns.Add("Msg");
            dt.Columns.Add("NodeID");
            dt.Columns.Add("NodeName");
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nd"></param>
        private void AddMsgInfo(string checkOption,string info, Node nd = null)
        {
            AddMsg("信息", checkOption, info, nd);
        }
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="info"></param>
        /// <param name="nd"></param>
        private void AddMsgWarning(string checkOption,string info, Node nd = null)
        {
            AddMsg("警告", checkOption, info, nd);
        }
        private void AddMsgError(string checkOption, string info, Node nd = null)
        {
            AddMsg("错误", checkOption, info, nd);
        }
        /// <summary>
        /// 增加审核信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="info">消息</param>
        /// <param name="nd">节点</param>
        /// <returns></returns>
        private void AddMsg(string type, string checkOption, string info, Node nd = null)
        {
            DataRow dr = this.dt.NewRow();
            dr[0] = type;
            dr[1] = info;
            dr[2] = checkOption;


            if (nd != null)
            {
                dr[3] = nd.NodeID;
                dr[4] = nd.Name;
            }
            this.dt.Rows.Add(dr);
        }
        #endregion 构造方法与属性.

        /// <summary>
        /// 校验流程
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            #region 基础操作设置.
            DBAccess.RunSQL("DELETE FROM Sys_MapExt WHERE DoWay='0' or DoWay='None'");
            Cache.ClearCache();
            #endregion 基础操作设置.

            this.flow.ClearAutoNumCache(true);
            try
            {
                //设置自动计算.
                CheckMode_Auto();

                ///检查独立表单的完整性.
                CheckMode_Frms();

                //通用检查.
                CheckMode_Gener();

                //检查子线程，数据必须是轨迹模式.
                CheckMode_SpecTable();

                //节点表单字段数据类型检查 
                CheckModel_FormFields();

                //检查越轨流程,子流程发起.
                CheckModel_SubFlowYanXus();

                //检查报表.
                string str = this.DoCheck_CheckRpt(this.nds);
                if (DataType.IsNullOrEmpty(str) == false)
                {
                    this.AddMsgError("检查Rpt表","@错误:表单枚举,外键字段UIBindKey信息丢失,请描述该字段的设计过程，反馈给开发人员,并删除错误字段重新在表单上创建。错误字段信息如下:",null);
                }

                //检查焦点字段设置是否还有效.
                CheckMode_FocusField();

                //检查质量考核点.
                CheckMode_EvalModel();

                //检查如果是合流节点必须不能是由上一个节点指定接受人员.
                CheckMode_HeliuAccpterRole();

                // 检查是否是计算未来处理人.
                CheckMode_FullSA();

                // 检查游离态节点, 设置是否正确.
                CheckMode_YouLiTai();
                //如果协作模式的节点，方向条件规则是下拉框的，修改为按线的.
                string sql = "UPDATE WF_Node SET CondModel = 2 WHERE CondModel = 1 AND TodolistModel = 1";
                DBAccess.RunSQL(sql);

                // 检查流程， 处理计算字段.
                Node.CheckFlow(nds, this.flow.No);
                foreach (Node nd in nds)
                {

                    nd.ClearAutoNumCache();
                    nd.Row = null;
                    BP.DA.Cache2019.DeleteRow("BP.WF.Node", nd.NodeID + "");
                }
                //创建track.
                Track.CreateOrRepairTrackTable(this.flow.No);

                //如果是引用的表单库的表单，就要检查该表单是否有FID字段，没有就自动增加.
                CheckMode_Ref();

                return BP.Tools.Json.ToJson(dt);
            }
            catch (Exception ex)
            {
                this.AddMsgError("系统异常",  ex.Message);
                return BP.Tools.Json.ToJson(dt);
            }
        }
        /// <summary>
        /// 检查游离态节点, 设置是否正确.
        /// </summary>
        public void CheckMode_YouLiTai()
        {
            //判断是否启用了 【面板】 功能.
            foreach (Node nd in nds)
            {
                if (nd.GetParaBoolen(NodeAttr.IsYouLiTai) == true)
                {
                    if (nd.CondModel != DirCondModel.ByLineCond)
                    {
                        nd.CondModel = DirCondModel.ByLineCond;
                        nd.Update();

                        this.AddMsgWarning("游离态节点设置","游离态节点转向规则必须是自动计算,系统帮您自动设置了.", nd);
                    }
                }
            }

            //查询出来节点.
            string sql = "SELECT NodeID FROM WF_Node WHERE TCEnable=1 AND FK_Flow='" + this.flow.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                int nodeID = int.Parse(dr[0].ToString());
                foreach (Node nd in nds)
                {
                    if (nd.NodeID == nodeID)
                    {
                        if (nd.CondModel != DirCondModel.ByLineCond)
                        {
                            nd.CondModel = DirCondModel.ByLineCond;
                            nd.Update();
                            this.AddMsgWarning("流转自定义设置","启动流转自定义节点的转向规则必须是自动计算,系统帮您自动设置了.", nd);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 通用的检查.
        /// </summary>
        public void CheckMode_Gener()
        {
            //条件集合.
            Conds conds = new Conds(this.flow.No);
            //删除垃圾数据.
            string sql = "DELETE FROM WF_Direction  WHERE Node NOT IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.flow.No + "') AND FK_Flow='" + this.flow.No + "' ";
            DBAccess.RunSQL(sql);
            sql = "DELETE FROM WF_Direction  WHERE ToNode NOT IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.flow.No + "') AND FK_Flow='" + this.flow.No + "' ";
            DBAccess.RunSQL(sql);

            foreach (Node nd in nds)
            {
                nd.CleanObject();

                //流程是极简模式，设置每一个节点的NodeFrmID为开始节点表单
                if (this.flow.FlowDevModel == FlowDevModel.JiJian)
                    nd.SetValByKey(NodeAttr.NodeFrmID, "ND" + Int32.Parse(this.flow.No) + "01");

                #region 设置路由节点或者用户节点到路由节点的转向规则为连接线
                //路由节点
                if (nd.HisNodeType == NodeType.RouteNode)
                    nd.CondModel = DirCondModel.ByLineCond;
                //到达的节点
                Nodes toNDs = nd.HisToNodes;
                foreach (Node toND in toNDs)
                {
                    if (toND.HisNodeType == NodeType.RouteNode)
                    {
                        nd.CondModel = DirCondModel.ByLineCond;
                        break;
                    }
                }
                #endregion 设置路由节点或者用户节点到路由节点的转向规则为连接线

                try
                {
                    //设置它的位置类型.
                    nd.SetValByKey(NodeAttr.NodePosType, (int)nd.GetHisNodePosType());
                }
                catch (Exception ex)
                {
                    this.AddMsgError("节点位置类型", "节点ID: (" + nd.NodeID + ")名称: (" + nd.Name + ") 到达节点错误：" + ex.Message, nd);
                }

                this.AddMsgInfo("修复表单字段","修复&检查节点信息", nd);
                nd.RepareMap(this.flow);

                // 从表检查。
                Sys.MapDtls dtls = new BP.Sys.MapDtls("ND" + nd.NodeID);
                foreach (BP.Sys.MapDtl dtl in dtls)
                {
                    this.AddMsgInfo("从表自动创建表","检查明细表" + dtl.Name, nd);
                    dtl.HisGEDtl.CheckPhysicsTable();
                }

                MapAttrs mattrs = new MapAttrs("ND" + nd.NodeID);

                #region 对节点的访问规则进行检查

                //this.AddMsgInfo("开始对节点的访问规则进行检查", nd);
                if(nd.HisNodeType == NodeType.UserNode)
                {
                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
                        case DeliveryWay.FindSpecDeptEmpsInStationlist:
                            if (nd.NodeStations.Count == 0)
                                this.AddMsgError("接收人规则", "错误:您设置了该节点的访问规则是按角色，但是您没有为节点绑定角色。", nd);
                            break;
                        case DeliveryWay.ByDept:
                            if (nd.NodeDepts.Count == 0)
                                this.AddMsgError("接收人规则", "设置了该节点的访问规则是按部门，但是您没有为节点绑定部门", nd);

                            break;
                        case DeliveryWay.ByBindEmp:
                            if (nd.NodeEmps.Count == 0)
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是按人员，但是您没有为节点绑定人员。", nd);

                            break;
                        case DeliveryWay.BySpecNodeEmp: /*按指定的角色计算.*/
                        case DeliveryWay.BySpecNodeEmpStation: /*按指定的角色计算.*/
                            if (nd.DeliveryParas.Trim().Length == 0)
                            {
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是按指定的角色计算，但是您没有设置节点编号。", nd);
                            }
                            else
                            {
                                if (DataType.IsNumStr(nd.DeliveryParas) == false)
                                {
                                    this.AddMsgError("接收人规则", "您没有设置指定角色的节点编号，目前设置的为{" + nd.DeliveryParas + "}", nd);
                                }
                            }
                            break;
                        case DeliveryWay.ByDeptAndStation: /*按部门与角色的交集计算.*/
                            string mysql = string.Empty;
                            //added by liuxc,2015.6.30.
                            //区别集成与BPM模式
                            mysql = "SELECT pdes.fk_emp AS No"
                               + " FROM   Port_DeptEmpStation pdes"
                               + "        INNER JOIN WF_NodeDept wnd"
                               + "             ON  wnd.fk_dept = pdes.fk_dept"
                               + "             AND wnd.fk_node = " + nd.NodeID
                               + "        INNER JOIN WF_NodeStation wns"
                               + "             ON  wns.FK_Station = pdes.fk_station"
                               + "             AND wnd.fk_node =" + nd.NodeID
                               + " ORDER BY"
                               + "        pdes.fk_emp";

                            DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                            if (mydt.Rows.Count == 0)
                                this.AddMsgError("接收人规则", "按照角色与部门的交集计算错误，没有人员集合{" + mysql + "}", nd);
                            break;
                        case DeliveryWay.BySQL:
                        case DeliveryWay.BySQLAsSubThreadEmpsAndData:
                            if (nd.DeliveryParas.Trim().Length <= 5)
                            {
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是按SQL查询，但是您没有在节点属性里设置查询sql，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.", nd);
                                continue;
                            }

                            sql = nd.DeliveryParas;
                            foreach (MapAttr item in mattrs)
                            {
                                if (item.ItIsNum)
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
                                this.AddMsgError("接收人规则", "您编写的sql变量填写不正确，实际执行中，没有被完全替换下来" + sql, nd);
                                continue;
                            }

                            DataTable testDB = null;
                            try
                            {
                                testDB = DBAccess.RunSQLReturnTable(sql);
                            }
                            catch (Exception ex)
                            {
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是按SQL查询,执行此语句错误." + sql + " err:" + ex.Message, nd);
                                break;
                            }

                            if (testDB.Columns.Contains("no") == false
                                || testDB.Columns.Contains("name") == false)
                            {
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是按SQL查询，设置的sql不符合规则，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册.", nd);
                            }

                            break;
                        case DeliveryWay.ByPreviousNodeFormEmpsField:
                        case DeliveryWay.ByPreviousNodeFormStationsAI:
                        case DeliveryWay.ByPreviousNodeFormStationsOnly:
                        case DeliveryWay.ByPreviousNodeFormDepts:
                            //去rpt表中，查询是否有这个字段
                            string str = nd.NodeID.ToString().Substring(0, nd.NodeID.ToString().Length - 2);
                            MapAttrs rptAttrs = new BP.Sys.MapAttrs();
                            rptAttrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + str + "Rpt", MapAttrAttr.KeyOfEn);

                            if (rptAttrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, nd.DeliveryParas) == false)
                            {
                                /*检查节点字段是否有FK_Emp字段*/
                                this.AddMsgError("接收人规则", "您设置了该节点的访问规则是[06.按上一节点表单指定的字段值作为本步骤的接受人]，但是您没有在节点属性的[访问规则设置内容]里设置指定的表单字段，详细参考开发手册.", nd);
                            }

                            break;
                        case DeliveryWay.BySelected: /* 由上一步发送人员选择 */
                            break;
                        case DeliveryWay.ByPreviousNodeEmp: /* 与上一个节点人员相同. */
                            if (nd.ItIsStartNode)
                            {
                                this.AddMsgError("接收人规则", "节点访问规则设置错误:开始节点，不允许设置与上一节点的工作人员相同.", nd);
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                
                #endregion

                #region 检查节点完成条件，方向条件的定义.
                if (conds.Count != 0)
                {
                    //this.AddMsgInfo("方向条件","开始检查(" + nd.Name + ")方向条件:", nd);

                    foreach (Cond cond in conds)
                    {

                        Node ndOfCond = new Node();
                        ndOfCond.NodeID = cond.NodeID;
                        if (ndOfCond.RetrieveFromDBSources() == 0)
                            continue;

                        if (cond.AttrKey.Length < 2)
                            continue;
                        string frmID = cond.GetValStringByKey(CondAttr.FrmID);
                        if (DataType.IsNullOrEmpty(frmID) == false)
                        {
                            GEEntity en = new GEEntity(frmID);
                            if (en.EnMap.Attrs.Contains(cond.AttrKey) == false)
                            {
                                this.AddMsgError("方向条件", "属性:" + cond.AttrKey + " , " + cond.AttrName + " 不存在。", nd);
                                continue;
                            }
                        }
                       /*
                        if (ndOfCond.HisWork.EnMap.Attrs.Contains(cond.AttrKey) == false)
                        {
                            this.AddMsgError("方向条件","属性:" + cond.AttrKey + " , " + cond.AttrName + " 不存在。", nd);
                            continue;
                        }*/
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
            //DBAccess.RunSQL("UPDATE WF_Node SET ReturnRole=0 WHERE NodePosType=0 AND ReturnRole !=0");

            //删除垃圾,非法数据.
            string sqls = "DELETE FROM Sys_FrmSln WHERE FK_MapData NOT IN (SELECT No from Sys_MapData)";
            sqls += "@ DELETE FROM WF_Direction WHERE Node=ToNode";
            DBAccess.RunSQLs(sqls);

            //更新计算数据.
            //this.flow.NumOfBill = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_FrmPrintTemplate WHERE NodeID IN (SELECT NodeID FROM WF_Flow WHERE No='" + this.flow.No + "')");
            //this.flow.NumOfDtl = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_MapDtl WHERE FK_MapData='ND" + int.Parse(this.flow.No) + "Rpt'");
            //this.flow.DirectUpdate();

            //一直没有找到设置3列，自动回到四列的情况.
            //DBAccess.RunSQL("UPDATE Sys_MapAttr SET ColSpan=3 WHERE  UIHeight<=23 AND ColSpan=4");
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
                    this.AddMsgError("绑定表单库的表单","节点绑定的表单ID=" + item.FK_Frm + "，但该表单已经不存在.", new Node(item.NodeID));
                    continue;
                }
                md.ClearCache();
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
                    mattr.setMyPK(nd.NodeFrmID + "_FID");
                    if (mattr.RetrieveFromDBSources() == 0)
                    {
                        mattr.SetValByKey(MapAttrAttr.KeyOfEn, "FID");
                        mattr.SetValByKey(MapAttrAttr.FK_MapData, nd.NodeFrmID);
                        mattr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppInt);
                        mattr.SetValByKey(MapAttrAttr.UIVisible, false);
                        mattr.SetValByKey(MapAttrAttr.Name, "FID(自动增加)");


                        mattr.Insert();

                        GEEntity en = new GEEntity(nd.NodeFrmID);
                        en.CheckPhysicsTable();
                    }
                }
            }
        }
        /// <summary>
        ///子线城，子线程的表单必须是轨迹模式
        /// </summary>
        public void CheckMode_SpecTable()
        {
            foreach (Node nd in nds)
            {
                if (nd.ItIsSubThread == false)
                    continue;
                MapData md = new MapData();
                md.No = "ND" + nd.NodeID;
                if (md.RetrieveFromDBSources() == 1)
                {
                    if (md.PTable.Equals( this.flow.PTable)==false)
                    {
                        md.PTable = this.flow.PTable;
                        md.Update();
                        md.ClearCache();
                    }
                }
                //检查数据表.
                GEEntity geEn = new GEEntity(md.No);
                geEn.CheckPhysicsTable();
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
            foreach (Attr attr in rpt.EnMap.Attrs)
            {
                rpt.SetValByKey(attr.Key, "0");
            }
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
                        msg = "节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的请设置焦点字段.";
                        this.AddMsgWarning("焦点字段",msg, nd);
                    }
                    else
                    {
                        msg = "节点ID:" + nd.NodeID + " 名称:" + nd.Name + "属性里没有设置焦点字段，会导致信息写入轨迹表空白，为了能够保证流程轨迹是可读的系统自动设置了焦点字段为" + attrKey + ".";
                        this.AddMsgInfo("焦点字段", msg, nd);

                        nd.FocusField = attrKey;
                        nd.DirectUpdate();
                    }
                    continue;
                }

                string strs = nd.FocusField.Clone() as string;
                strs = Glo.DealExp(strs, rpt, "err");
                if (strs.Contains("@") == true)
                {
                    msg = "焦点字段（" + nd.FocusField + "）在节点(step:" + nd.Step + " 名称:" + nd.Name + ")属性里的设置已无效，表单里不存在该字段.";
                    this.AddMsgWarning("焦点字段", msg, nd);
                }

                if (this.flow.ItIsMD5)
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
                if (nd.ItIsEval)
                {
                    /*如果是质量考核点，检查节点表单是否具别质量考核的特别字段？*/
                    string sql = "SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "' AND KeyOfEn IN ('EvalEmpNo','EvalEmpName','EvalEmpCent')";
                    if (DBAccess.RunSQLReturnValInt(sql) != 3)
                        this.AddMsgInfo("质量考核","@信息:您设置了节点(" + nd.NodeID + "," + nd.Name + ")为质量考核节点，但是您没有在该节点表单中设置必要的节点考核字段.",nd);
                }
            }
        }
        /// <summary>
        /// 是否是自动计算未来处理人?
        /// </summary>
        public void CheckMode_FullSA()
        {
            //是否是自动计算未来处理人.
            if (this.flow.ItIsFullSA == false)
                return;

            string msg = "";
            foreach (Node nd in nds)
            {
                //方向条件转向规则设置为，自动计算的.
                if (nd.CondModel != DirCondModel.ByLineCond)
                {
                    nd.CondModel = DirCondModel.ByLineCond;
                    nd.Update();
                    this.AddMsgInfo("计算未来处理人","计算未来接收人的流程，转向规则必须是按照条件计算，系统已经自动为您修复。", nd);
                }
                if (nd.ItIsStartNode == false)
                {
                    if (nd.HisDeliveryWay == DeliveryWay.BySelected
                        || nd.HisDeliveryWay == DeliveryWay.BySelected_2
                        || nd.HisDeliveryWay == DeliveryWay.BySelected_2)
                    {
                        this.AddMsgError("计算未来处理人", "计算未来处理人的流程,接收人规则不能是主管选择的，请在节点右键设置接收人规则.", nd);
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
                        msg = "节点ID:" + nd.NodeID + " 名称:" + nd.Name + "是合流或者分合流节点，但是该节点设置的接收人规则为由上一步指定，这是错误的，应该为自动计算而非每个子线程人为的选择.";
                        this.AddMsgError("合流节点接收人设置",msg, nd);
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

                //筛选出类型不同的字段.
                string checkSQL = "SELECT   AA.KEYOFEN, COUNT(*) AS MYNUM FROM ("
                                    + "  SELECT A.KEYOFEN,  MYDATATYPE,  COUNT(*) AS MYNUM "
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
                        this.DoCheck_CheckRpt(this.flow.HisNodes);
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
                    errorAppend.Append("@基础表" + baseMapAttr.FrmID + "，字段" + keyOfEn + "数据类型为：" + baseMapAttr.MyDataTypeStr);
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
                        ndMapAttr.setMinLen(baseMapAttr.MinLen);
                        ndMapAttr.setMaxLen(baseMapAttr.MaxLen);
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
                    rptMapAttr.setMinLen(baseMapAttr.MinLen);
                    rptMapAttr.setMaxLen(baseMapAttr.MaxLen);
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
        private string DoCheck_CheckRpt(Nodes nds)
        {
            string msg = "";
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
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
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
            sql = "SELECT MyPK, KeyOfEn,DefVal,Name,LGType,MyDataType,UIContralType,UIBindKey,FK_MapData FROM Sys_MapAttr WHERE FK_MapData IN (" + ndsstrs + ")";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //求已经存在的字段集合。
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='ND" + flowId + "Rpt'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            //查询出来已经有的映射.
            MapAttrs attrs = new MapAttrs(fk_mapData);

            //遍历 - 所有节点表单字段的合集
            foreach (DataRow dr in dt.Rows)
            {
                //如果是枚举，外键字段，判断是否判定了对应的枚举和外键
                Int32 lgType = Int32.Parse(dr["LGType"].ToString());
                Int32 contralType = Int32.Parse(dr["UIContralType"].ToString());

                if ((lgType == 2 && contralType == 1) || (lgType == 0 && contralType == 1 && Int32.Parse(dr["MyDataType"].ToString()) == 1))
                {
                    if (dr["UIBindKey"] == null || DataType.IsNullOrEmpty(dr["UIBindKey"].ToString()) == true)
                        msg += "表单" + dr["FK_MapData"].ToString() + "中,外键/外部数据源字段:" + dr["Name"].ToString() + "(" + dr["KeyOfEn"].ToString() + ");";
                }
                if (lgType == 1 && (dr["UIBindKey"] == null || DataType.IsNullOrEmpty(dr["UIBindKey"].ToString()) == true))
                    msg += "表单" + dr["FK_MapData"].ToString() + "中,枚举字段:" + dr["Name"].ToString() + "(" + dr["KeyOfEn"].ToString() + ");";

                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@") == true)
                    continue;

                string mypk = dr["MyPK"].ToString();

                pks += dr["KeyOfEn"].ToString() + "@";

                //找到这个属性.
                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);
                ma.setMyPK("ND" + flowId + "Rpt_" + ma.KeyOfEn);
                ma.FrmID = "ND" + flowId + "Rpt";
                ma.setUIIsEnable(false);

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                //如果包含他,就说已经存在.
                if (attrs.Contains("MyPK", ma.MyPK) == true)
                    continue;
                // 如果不存在.
                ma.Insert();
            }

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
                if (md.Name.Equals(this.flow.Name) == false || md.PTable.Equals(this.flow.PTable) == false)
                {
                    md.Name = this.flow.Name;
                    md.PTable = this.flow.PTable;
                    md.Update();
                }

            }
            #endregion 插入字段。

            #region 补充上流程字段到NDxxxRpt.
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case GERptAttr.FK_Dept:
                        attr.setUIContralType(UIContralType.TB);
                        attr.setLGType(FieldTypeS.Normal);
                        attr.setUIVisible(true);
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.setUIIsEnable(false);
                        attr.DefVal = "";
                        attr.setMaxLen(100);
                        attr.Update();
                        break;

                    case "FK_NY":
                        //  attr.UIBindKey = "BP.Pub.NYs";
                        attr.setUIContralType(UIContralType.TB);
                        attr.setLGType(FieldTypeS.Normal);
                        attr.setUIVisible(true);
                        attr.setUIIsEnable(false);
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
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.Title); // "FlowEmps";
                attr.setName("标题"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(400);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.setKeyOfEn("OID");
                attr.setName("WorkID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }


            if (attrs.Contains(md.No + "_" + GERptAttr.FID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.setKeyOfEn("FID");
                attr.setName("FID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFState) == false)
            {
                /* 流程状态 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.WFState);
                attr.setName("流程状态"); //  
                attr.setMyDataType(DataType.AppInt);
                attr.UIBindKey = GERptAttr.WFState;
                attr.setUIContralType(UIContralType.DDL);
                attr.setLGType(FieldTypeS.Enum);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(1000);
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFSta) == false)
            {
                /* 流程状态Ext */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.WFSta);
                attr.setName("状态"); //  
                attr.setMyDataType(DataType.AppInt);
                attr.UIBindKey = GERptAttr.WFSta;
                attr.setUIContralType(UIContralType.DDL);
                attr.setLGType(FieldTypeS.Enum);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(1000);
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEmps) == false)
            {
                /* 参与人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowEmps); // "FlowEmps";
                attr.setName("参与人"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(1000);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowStarter);
                attr.setName("发起人"); //  
                attr.setMyDataType(DataType.AppString);

                //attr.UIBindKey = "BP.Port.Emps";
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowStartRDT); // "FlowStartRDT";
                attr.setName("发起时间");
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowEnder);
                attr.setName("结束人"); //  
                attr.setMyDataType(DataType.AppString);
                // attr.UIBindKey = "BP.Port.Emps";
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowEnderRDT); // "FlowStartRDT";
                attr.setName("结束时间");
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEndNode) == false)
            {
                /* 结束节点 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowEndNode);
                attr.setName("结束节点");
                attr.setMyDataType(DataType.AppInt);
                attr.DefVal = "0";
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowDaySpan) == false)
            {
                /* FlowDaySpan */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.FlowDaySpan); // "FlowStartRDT";
                attr.setName("流程时长(天)");
                attr.setMyDataType(DataType.AppFloat);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PFlowNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PFlowNo);
                attr.setName("父流程编号"); //  父流程流程编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PNodeID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PNodeID);
                attr.setName("父流程启动的节点");
                attr.setMyDataType(DataType.AppInt);
                attr.DefVal = "0";
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PWorkID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PWorkID);
                attr.setName("父流程WorkID");
                attr.setMyDataType(DataType.AppInt);
                attr.DefVal = "0";
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PEmp) == false)
            {
                /* 调起子流程的人员 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PEmp);
                attr.setName("调起子流程的人员");
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.BillNo);
                attr.setName("单据编号"); //  单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }




            if (attrs.Contains(md.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.AtPara);
                attr.setName("参数"); // 单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(4000);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.GUID) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.GUID);
                attr.setName("GUID"); // 单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(32);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PrjNo) == false)
            {
                /* 项目编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PrjNo);
                attr.setName("项目编号"); //  项目编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }
            if (attrs.Contains(md.No + "_" + GERptAttr.PrjName) == false)
            {
                /* 项目名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FrmID = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.PrjName);
                attr.setName("项目名称"); //  项目名称
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FK_DeptName) == false)
            {
                MapAttr attr = new BP.Sys.MapAttr();
                attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
                attr.setEditType(BP.En.EditType.UnDel);
                attr.SetValByKey(MapAttrAttr.KeyOfEn, "FK_DeptName");
                attr.SetValByKey(MapAttrAttr.Name, "操作员部门名称");
                attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.SetValByKey(MapAttrAttr.UIVisible, false);
                attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
                attr.setLGType(FieldTypeS.Normal);
                attr.SetValByKey(MapAttrAttr.MinLen, 0);
                attr.SetValByKey(MapAttrAttr.MaxLen, 50);
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
                sql = "UPDATE Sys_MapAttr SET GroupID='" + flowGF.OID + "' WHERE  FK_MapData='" + fk_mapData + "'  AND KeyOfEn IN('" + GERptAttr.PFlowNo + "','" + GERptAttr.PWorkID + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FK_NY + "','" + GERptAttr.FlowDaySpan + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEnder + "','" + GERptAttr.FlowEnderRDT + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.FlowStarter + "','" + GERptAttr.FlowStartRDT + "','" + GERptAttr.WFState + "')";
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex.Message);
            }
            #endregion 为流程字段设置分组

            #region 尾后处理.
            GERpt gerpt = this.HisGERpt;
            gerpt.CheckPhysicsTable();  //让报表重新生成.

            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.HGDB || DBAccess.AppCenterDBType == DBType.UX)
                DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE FrmID='" + fk_mapData + "' AND  ''||OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");
            else
                DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE FrmID='" + fk_mapData + "' AND  OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");


            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='活动时间' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='CDT'");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='参与者' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='Emps'");
            #endregion 尾后处理.
            return msg;
        }
    }
}
