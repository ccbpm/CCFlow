﻿using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using System.Collections;
using BP.Port;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.Template.CCEn;
using BP.WF.Port;
using System.Collections.Generic;
using BP.WF.Template.SFlow;


namespace BP.WF
{
    /// <summary>
    /// 这里存放每个节点的信息.	 
    /// </summary>
    public class Node : Entity
    {
        #region 参数属性
        /// <summary>
        /// 方向条件控制规则
        /// </summary>
        public DirCondModel CondModel
        {
            get
            {
                //if (this.TodolistModel == TodolistModel.Teamup
                //      && this.IsEndNode == false)
                //    return DirCondModel.ByUserSelected;

                var model = (DirCondModel)this.GetValIntByKey(NodeAttr.CondModel);
                return model;
            }
            set
            {
                //var model = (DirCondModel)value;
                //if (this.TodolistModel == TodolistModel.Teamup
                //  && model == DirCondModel.SendButtonSileSelect
                //  && this.IsEndNode == false)
                //    model = DirCondModel.ByUserSelected;

                this.SetValByKey(NodeAttr.CondModel, (int)value);
            }
        }
        /// <summary>
        /// 抢办发送后处理规则
        /// </summary>
        public QiangBanSendAfterRole QiangBanSendAfterRole
        {
            get
            {
                return (QiangBanSendAfterRole)this.GetParaInt(NodeAttr.QiangBanSendAfterRole);
            }
        }
        /// <summary>
        /// 超时处理方式
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        /// <summary>
        /// 待办删除规则
        /// </summary>
        public int GenerWorkerListDelRole
        {
            get
            {
                return this.GetParaInt("GenerWorkerListDelRole", 0);
            }
            set
            {
                this.SetPara("GenerWorkerListDelRole", value);
            }
        }
        /// <summary>
        /// 超时处理内容.
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// 手工启动的子流程个数
        /// </summary>
        public int SubFlowHandNum
        {
            get
            {
                return this.GetParaInt("SubFlowHandNum", 0);
            }
        }
        /// <summary>
        /// 是否是发送返回节点？
        /// </summary>
        public bool IsSendBackNode
        {
            get
            {
                return this.GetParaBoolen(NodeAttr.IsSendBackNode, false);
            }
            set
            {
                this.SetPara(NodeAttr.IsSendBackNode, value);
            }
        }
        /// <summary>
        /// 抄送数量
        /// </summary>
        public int CCRoleNum
        {
            get
            {
                return this.GetParaInt("CCRoleNum", 0);
            }
        }
        /// <summary>
        /// 自动启动的子流程个数
        /// </summary>
        public int SubFlowAutoNum
        {
            get
            {
                return this.GetParaInt("SubFlowAutoNum", 0);
            }
        }
        /// <summary>
        /// 延续子流程个数
        /// </summary>
        public int SubFlowYanXuNum
        {
            get
            {
                return this.GetParaInt("SubFlowYanXuNum", 0);
            }
        }
        #endregion

        public static void ClearNodeAutoNum(int nodeID)
        {
            Node nd = new Node(nodeID);
            nd.ClearAutoNumCash(true);
        }

        #region 外键属性.
        /// <summary>
        /// 它的抄送规则.
        /// </summary>
        public CC HisCC
        {
            get
            {

                CC obj = this.GetRefObject("HisCC") as CC;
                if (obj == null)
                {
                    obj = new CC();
                    obj.NodeID = this.NodeID;
                    obj.Retrieve();
                    this.SetRefObject("HisCC", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 流程完成条件
        /// </summary>
        public Conds CondsOfFlowComplete
        {
            get
            {
                var ens = this.GetEntitiesAttrFromAutoNumCash(new Conds(),
                    CondAttr.FK_Node, this.NodeID, CondAttr.CondType, (int)CondType.Flow, CondAttr.Idx);
                return ens as Conds;
            }
        }
        /// <summary>
        /// 他的将要转向的方向集合
        /// 如果他没有到转向方向,他就是结束节点.
        /// 没有生命周期的概念,全部的节点.
        /// </summary>
        public Nodes HisToNodes
        {
            get
            {
                Nodes obj = this.GetRefObject("HisToNodes") as Nodes;
                if (obj == null)
                {
                    obj = new Nodes();
                    if (this.HisToNDNum == 0)
                        obj = new Nodes();
                    if (this.HisToNDNum == 1)
                        obj.AddEntities(this.HisToNDs);
                    if (this.HisToNDNum > 1)
                    {
                        string toNodes = this.HisToNDs.Replace("@", ",");
                        if (toNodes.Length > 1)
                            toNodes = toNodes.Substring(1);
                        obj.RetrieveInOrderBy("NodeID", toNodes, "Step");
                    }
                    this.SetRefObject("HisToNodes", obj);
                }
                return obj;
            }
        }
        public BP.WF.Template.NodeSimples HisToNodeSimples
        {
            get
            {
                NodeSimples obj = this.GetRefObject("HisToNodesSipm") as NodeSimples;
                if (obj == null)
                {
                    obj = new NodeSimples();
                    if (this.HisToNDNum == 0)
                        obj = new NodeSimples();
                    if (this.HisToNDNum == 1)
                        obj.AddEntities(this.HisToNDs);

                    if (this.HisToNDNum > 1)
                    {
                        string inStrs = this.HisToNDs.Replace('@', ',');
                        inStrs = inStrs.Substring(1);
                        obj.RetrieveIn("NodeID", inStrs);

                        //@101@102@103.
                        string[] ndStrs = this.HisToNDs.Split('@');

                        NodeSimples myobjs = new NodeSimples();
                        foreach (string nd in ndStrs)
                        {
                            if (DataType.IsNullOrEmpty(nd) == true)
                                continue;
                            Entity mynd = obj.GetEntityByKey("NodeID", int.Parse(nd));
                            myobjs.AddEntity(mynd);
                        }
                        obj = myobjs;
                    }
                    this.SetRefObject("HisToNodesSipm", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 他的工作
        /// </summary>
        public Work HisWork
        {
            get
            {
                Work wk = null;
                if (this.FormType != NodeFormType.FoolTruck
                    || this.WorkID == 0
                    || this.IsStartNode == true)
                {
                    wk = new BP.WF.GEWork(this.NodeID, this.NodeFrmID);
                    wk.HisNode = this;
                    wk.NodeID = this.NodeID;
                    return wk;
                }

                //如果是累加表单.
                wk = new BP.WF.GEWork(this.NodeID, this.NodeFrmID);

                Map ma = wk.EnMap;

                /* 求出来走过的表单集合 */
                string sql = "SELECT NDFrom FROM ND" + int.Parse(this.FK_Flow) + "Track A, WF_Node B ";
                sql += " WHERE A.NDFrom=B.NodeID  ";
                sql += "  AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.Start + "  OR ActionType=" + (int)ActionType.Skip + ")  ";
                sql += "  AND B.FormType=" + (int)NodeFormType.FoolTruck + " "; // 仅仅找累加表单.
                sql += "  AND NDFrom!=" + this.NodeID + " "; //排除当前的表单.

                //if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+CONVERT(varchar(10),B.NodeID) ) ";

                //if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+cast(B.NodeID as varchar(10)) ) ";

                //if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle)
                //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+to_char(B.NodeID) ) ";

                sql += "  AND (A.WorkID=" + this.WorkID + ") ";
                sql += " ORDER BY A.RDT ";

                // 获得已经走过的节点IDs.
                DataTable dtNodeIDs = DBAccess.RunSQLReturnTable(sql);
                string frmIDs = "";
                if (dtNodeIDs.Rows.Count > 0)
                {
                    //把所有的节点字段.
                    foreach (DataRow dr in dtNodeIDs.Rows)
                    {
                        if (frmIDs.Contains("ND" + dr[0].ToString()) == true)
                            continue;
                        frmIDs += "'ND" + dr[0].ToString() + "',";
                    }

                    frmIDs = frmIDs.Substring(0, frmIDs.Length - 1);
                    wk.HisPassedFrmIDs = frmIDs;  //求出所有的fromID.

                    MapAttrs attrs = new MapAttrs();
                    QueryObject qo = new QueryObject(attrs);
                    qo.AddWhere(MapAttrAttr.FK_MapData, " IN ", "(" + frmIDs + ")");
                    qo.DoQuery();

                    //设置成不可以用.
                    foreach (MapAttr item in attrs)
                    {
                        item.setUIIsEnable(false); //设置为只读的.
                        item.setDefValReal("");    //设置默认值为空.

                        ma.Attrs.Add(item.HisAttr);
                    }

                    //设置为空.
                    wk.SQLCash = null;
                }

                wk.HisNode = this;
                wk.NodeID = this.NodeID;
                wk.SQLCash = null;

                Cash.SQL_Cash.Remove("ND" + this.NodeID);
                return wk;

            }
        }
        /// <summary>
        /// 他的工作s
        /// </summary>
        public Works HisWorks
        {
            get
            {
                Works obj = this.HisWork.GetNewEntities as Works;
                obj.Clear();
                return obj;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FrmNodes HisFrmNodes
        {
            get
            {
                FrmNodes obj = this.GetRefObject("FrmNodes") as FrmNodes;
                if (obj == null)
                {
                    obj = new FrmNodes(this.NodeID);
                    this.SetRefObject("FrmNodes", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                Flow obj = this.GetRefObject("Flow") as Flow;
                if (obj == null)
                {
                    obj = new Flow(this.FK_Flow);
                    this.SetRefObject("Flow", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 消息推送.
        /// </summary>
        public PushMsgs HisPushMsgs
        {
            get
            {
                PushMsgs obj = this.GetRefObject("PushMsg") as PushMsgs;
                if (obj == null)
                {
                    var ens = this.GetEntitiesAttrFromAutoNumCash(new PushMsgs(),
                  PushMsgAttr.FK_Node, this.NodeID);

                    obj = ens as PushMsgs;

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    {

                        //检查是否有默认的发送？如果没有就增加上他。
                        bool isHaveSend = false;
                        bool isHaveReturn = false;
                        foreach (PushMsg item in obj)
                        {
                            if (item.FK_Event == EventListNode.SendSuccess)
                                isHaveSend = true;

                            if (item.FK_Event == EventListNode.ReturnAfter)
                                isHaveReturn = true;
                        }

                        if (isHaveSend == false)
                        {
                            PushMsg pm = new PushMsg();
                            pm.FK_Event = EventListNode.SendSuccess;
                            pm.SMSPushWay = 1; //*默认:让其使用短消息提醒.
                            pm.SMSPushModel = "Email";
                            obj.AddEntity(pm);
                        }

                        if (isHaveReturn == false)
                        {
                            PushMsg pm = new PushMsg();
                            pm.FK_Event = EventListNode.ReturnAfter;
                            pm.SMSPushWay = 1;  //*默认:让其使用短消息提醒. 
                            pm.SMSPushModel = "Email";
                            obj.AddEntity(pm);
                        }
                    }

                    this.SetRefObject("PushMsg", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 他的将要来自的方向集合
        /// 如果他没有到来的方向,他就是开始节点.
        /// </summary>
        public Nodes FromNodes
        {
            get
            {
                Nodes obj = this.GetRefObject("HisFromNodes") as Nodes;
                if (obj == null)
                {
                    // 根据方向生成到达此节点的节点。
                    Directions ens = new Directions();
                    if (this.IsStartNode)
                        obj = new Nodes();
                    else
                        obj = ens.GetHisFromNodes(this.NodeID);
                    this.SetRefObject("HisFromNodes", obj);
                }
                return obj;
            }
        }

        public NodeStations NodeStations
        {
            get
            {
                NodeStations obj = this.GetRefObject("NodeStations") as NodeStations;
                if (obj == null)
                {
                    obj = new NodeStations(this.NodeID);
                    this.SetRefObject("NodeStations", obj);
                }
                return obj;
            }
        }
        public NodeDepts NodeDepts
        {
            get
            {
                NodeDepts obj = this.GetRefObject("NodeDepts") as NodeDepts;
                if (obj == null)
                {
                    obj = new NodeDepts(this.NodeID);
                    this.SetRefObject("NodeDepts", obj);
                }
                return obj;
            }
        }
        public NodeEmps NodeEmps
        {
            get
            {
                NodeEmps obj = this.GetRefObject("NodeEmps") as NodeEmps;
                if (obj == null)
                {
                    obj = new NodeEmps(this.NodeID);
                    this.SetRefObject("NodeEmps", obj);
                }
                return obj;
            }
        }

        public FrmNodes FrmNodes
        {
            get
            {
                FrmNodes obj = this.GetRefObject("FrmNodes") as FrmNodes;
                if (obj == null)
                {
                    obj = new FrmNodes(this.FK_Flow, this.NodeID);
                    this.SetRefObject("FrmNodes", obj);
                }
                return obj;
            }
        }
        public MapData MapData
        {
            get
            {
                MapData obj = this.GetRefObject("MapData") as MapData;
                if (obj == null)
                {
                    obj = new MapData(this.NodeFrmID);
                    this.SetRefObject("MapData", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 获得事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                var ens = this.GetEntitiesAttrFromAutoNumCash(new FrmEvents(),
                    FrmEventAttr.FK_Node, this.NodeID);
                return ens as FrmEvents;
            }
        }

        #endregion

        #region 初试化全局的 Node
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true)
                    uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 初试化全局的
        /// </summary>
        /// <returns></returns>
        public NodePosType GetHisNodePosType()
        {
            if (this.IsSendBackNode)
                return NodePosType.Mid;

            string nodeid = this.NodeID.ToString();
            if (nodeid.Substring(nodeid.Length - 2).Equals("01") == true)
                return NodePosType.Start;

            if (this.HisToNodes.Count == 0)
                return NodePosType.End;

            return NodePosType.Mid;
        }

        public FWCAth FWCAth
        {
            get
            {
                return (FWCAth)this.GetValIntByKey(NodeWorkCheckAttr.FWCAth);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCAth, (int)value);
            }
        }

        /// <summary>
        /// 检查流程，修复必要的计算字段信息.
        /// </summary>
        /// <param name="fl">流程</param>
        /// <returns>返回检查信息</returns>
        public static string CheckFlow(Nodes nds, string flowNo)
        {
            string sql = "";
            DataTable dt = null;

            // 单据信息，角色，节点信息。
            foreach (Node nd in nds)
            {
                // 工作角色。
                sql = "SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nd.NodeID;
                dt = DBAccess.RunSQLReturnTable(sql);
                string strs = "";
                foreach (DataRow dr in dt.Rows)
                    strs += "@" + dr[0].ToString();
                nd.HisStas = strs;

                // 工作部门。
                sql = "SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID;
                dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                    strs += "@" + dr[0].ToString();
                nd.HisDeptStrs = strs;

                // 节点方向.
                strs = "";
                Directions dirs = new Directions(nd.NodeID);
                foreach (Direction dir in dirs)
                    strs += "@" + dir.ToNode;
                nd.HisToNDs = strs;


                // 检查节点的位置属性。
                nd.HisNodePosType = nd.GetHisNodePosType();
                if (nd.IsSendBackNode == true)
                {
                    nd.NodePosType = NodePosType.Mid;
                }
                try
                {
                    nd.DirectUpdate();
                }
                catch (Exception ex)
                {
                    throw new Exception("err@" + ex.Message + " node=" + nd.Name);
                }
            }

            // 处理角色分组.
            sql = "SELECT HisStas, COUNT(*) as NUM FROM WF_Node WHERE FK_Flow='" + flowNo + "' GROUP BY HisStas";
            dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string stas = dr[0].ToString();
                string nodes = "";
                foreach (Node nd in nds)
                {
                    if (nd.HisStas == stas)
                        nodes += "@" + nd.NodeID;
                }

                foreach (Node nd in nds)
                {
                    if (nodes.Contains("@" + nd.NodeID.ToString()) == false)
                        continue;

                    nd.GroupStaNDs = nodes;
                    nd.DirectUpdate();
                }
            }
            return "检查成功.";
        }

        protected override bool beforeUpdate()
        {
            //检查设计流程权限,集团模式下，不是自己创建的流程，不能设计流程.
            BP.WF.Template.TemplateGlo.CheckPower(this.FK_Flow);

            //删除自动数量的缓存数据.
            this.ClearAutoNumCash(false);


            if (this.IsStartNode)
            {
                //this.SetValByKey(BtnAttr.ReturnRole, (int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.ShiftEnable, 0);
                this.SetValByKey(BtnAttr.EndFlowEnable, 0);

                Node oldN = new Node(this.NodeID);
                if (this.Name.Equals(oldN.Name) == false)
                {
                    //清空WF_Emp中的StartFlows 的内容
                    try
                    {
                        DBAccess.RunSQL("UPDATE WF_Emp Set StartFlows ='' ");
                    }
                    catch (Exception e)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        if (DBAccess.IsExitsTableCol("WF_Emp", "StartFlows") == false)
                        {
                            string sql = "";
                            sql = "ALTER TABLE WF_Emp ADD StartFlows text ";

                            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                                sql = "ALTER TABLE WF_Emp ADD StartFlows blob";

                            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                                sql = "ALTER TABLE  WF_Emp ADD StartFlows bytea NULL ";

                            DBAccess.RunSQL(sql);
                        }

                    }
                }
                //清空WF_Emp中的StartFlow 
                DBAccess.RunSQL("UPDATE  WF_Emp Set StartFlows =''");

            }

            //给icon设置默认值.
            if (this.GetValStrByKey(NodeAttr.Icon) == "")
                this.Icon = "审核.png";

            #region 如果是数据合并模式，就要检查节点中是否有子线程，如果有子线程就需要单独的表.
            if (this.IsSubThread == true)
            {
                MapData md = new MapData();
                md.No = "ND" + this.NodeID;

                if (md.RetrieveFromDBSources() != 0 && md.PTable.Equals("ND" + this.NodeID) == false)
                {
                    md.PTable = "ND" + this.NodeID;
                    md.Update();
                }
            }
            #endregion 如果是数据合并模式，就要检查节点中是否有子线程，如果有子线程就需要单独的表.

            //更新版本号.
            Flow.UpdateVer(this.FK_Flow);

            Flow fl = new Flow(this.FK_Flow);

            this.FlowName = fl.Name;

            //更新表单名称.
            DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + this.Name + "' WHERE No='ND" + this.NodeID + "' AND name=''");
            switch (this.HisRunModel)
            {
                case RunModel.Ordinary:
                    if (this.IsStartNode)
                        this.HisNodeWorkType = NodeWorkType.StartWork;
                    else
                        this.HisNodeWorkType = NodeWorkType.Work;
                    break;
                case RunModel.FL:
                    if (this.IsStartNode)
                        this.HisNodeWorkType = NodeWorkType.StartWorkFL;
                    else
                        this.HisNodeWorkType = NodeWorkType.WorkFL;
                    break;
                case RunModel.HL:
                    //if (this.IsStartNode)
                    //    throw new Exception("@您不能设置开始节点为合流节点。");
                    //else
                    //    this.HisNodeWorkType = NodeWorkType.WorkHL;
                    break;
                case RunModel.FHL:
                    //if (this.IsStartNode)
                    //    throw new Exception("@您不能设置开始节点为分合流节点。");
                    //else
                    //    this.HisNodeWorkType = NodeWorkType.WorkFHL;
                    break;
                case RunModel.SubThreadUnSameWorkID:
                case RunModel.SubThreadSameWorkID:
                    this.HisNodeWorkType = NodeWorkType.SubThreadWork;
                    break;
                default:
                    throw new Exception("eeeee");
                    break;
            }
            //断头路节点
            if (this.IsSendBackNode == true)
            {
                this.NodePosType = NodePosType.Mid;
            }

            //创建审核组件附件
            FrmAttachment workCheckAth = new FrmAttachment();
            bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK,
                "ND" + this.NodeID + "_FrmWorkCheck");
            //不包含审核组件
            if (isHave == false && this.FWCAth == FWCAth.MinAth)
            {
                workCheckAth = new FrmAttachment();
                /*如果没有查询到它,就有可能是没有创建.*/
                workCheckAth.setMyPK("ND" + this.NodeID + "_FrmWorkCheck");
                workCheckAth.setFK_MapData("ND" + this.NodeID.ToString());
                workCheckAth.NoOfObj = "FrmWorkCheck";
                workCheckAth.Exts = "*.*";

                //存储路径.
                //  workCheckAth.SaveTo = "/DataUser/UploadFile/";
                workCheckAth.IsNote = false; //不显示note字段.
                workCheckAth.IsVisable = false; // 让其在form 上不可见.

                //位置.

                workCheckAth.H = (float)150;

                //多附件.
                workCheckAth.UploadType = AttachmentUploadType.Multi;
                workCheckAth.Name = "审核组件";
                workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=");
                workCheckAth.Insert();
            }

            return base.beforeUpdate();
        }
        #endregion

        protected override void afterInsertUpdateAction()
        {
            Flow fl = new Flow();
            fl.No = this.FK_Flow;
            fl.RetrieveFromDBSources();
            MapData mapData = new MapData();
            mapData.No = "ND" + this.NodeID;
            if (mapData.RetrieveFromDBSources() != 0)
            {
                if (this.IsSubThread == true)
                    mapData.PTable = mapData.No;
                else
                    mapData.PTable = fl.PTable;
                mapData.Update();
            }


            if (this.FormType == NodeFormType.RefOneFrmTree)
            {
                GEEntity en = new GEEntity(this.NodeFrmID);
                if (this.IsSubThread == true && en.EnMap.Attrs.Contains("FID") == false)
                {
                    MapAttr attr = new BP.Sys.MapAttr();
                    attr.setFK_MapData(this.NodeFrmID);
                    attr.setKeyOfEn("FID");
                    attr.setName("干流程ID");
                    attr.setMyDataType(DataType.AppInt);
                    attr.setUIContralType(UIContralType.TB);
                    attr.setLGType(FieldTypeS.Normal);
                    attr.setUIVisible(false);
                    attr.setUIIsEnable(false);
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
                if (en.EnMap.Attrs.Contains("Rec") == false)
                {
                    MapAttr attr = new BP.Sys.MapAttr();
                    attr.setFK_MapData(this.NodeFrmID);
                    attr.setKeyOfEn("Rec");
                    attr.setName("记录人");
                    attr.setMyDataType(DataType.AppString);
                    attr.setUIContralType(UIContralType.TB);
                    attr.setLGType(FieldTypeS.Normal);
                    attr.setUIVisible(false);
                    attr.setUIIsEnable(false);
                    attr.setMaxLen(100);
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }


            }

            base.afterInsertUpdateAction();
        }

        #region 基本属性
        /// <summary>
        /// 审核组件
        /// </summary>
        public BP.WF.Template.FrmWorkCheckSta FrmWorkCheckSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeAttr.FWCSta);
            }
            set
            {
                this.SetValByKey(NodeAttr.FWCSta, (int)value);
            }
        }

        public FrmSubFlowSta SFSta
        {
            get
            {
                return (FrmSubFlowSta)this.GetValIntByKey(FrmSubFlowAttr.SFSta);
            }
            set
            {
                this.SetValByKey(FrmSubFlowAttr.SFSta, (int)value);
            }
        }
        /// <summary>
        /// 审核组件版本
        /// </summary>
        public int FWCVer
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCVer, 0);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCVer, value);
            }
        }
        /// <summary>
        /// 内部编号
        /// </summary>
        public string No
        {
            get
            {
                try
                {
                    return this.NodeID.ToString().Substring(this.NodeID.ToString().Length - 2);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError(ex.Message + " - " + this.NodeID);
                    throw new Exception("@没有获取到它的NodeID = " + this.NodeID);
                }
            }
        }
        /// <summary>
        /// 自动跳转规则0-处理人就是提交人
        /// </summary>
        public bool AutoJumpRole0
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.AutoJumpRole0);
            }
            set
            {
                this.SetValByKey(NodeAttr.AutoJumpRole0, value);
            }
        }
        /// <summary>
        /// 自动跳转规则1-处理人已经出现过
        /// </summary>
        public bool AutoJumpRole1
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.AutoJumpRole1);
            }

        }
        /// <summary>
        /// 自动跳转规则2-处理人与上一步相同
        /// </summary>
        public bool AutoJumpRole2
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.AutoJumpRole2);
            }
        }
        /// <summary>
        /// 跳转的表达式.
        /// </summary>
        public string AutoJumpExp
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.AutoJumpExp);
            }
        }
        /// <summary>
        /// 执行跳转时间 0=上节点发送时，1=打开时.
        /// </summary>
        public int SkipTime
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.SkipTime);
            }
        }
        /// <summary>
        /// 是不是节点表单.
        /// </summary>
        public bool IsNodeFrm
        {
            get
            {
                if (this.HisFormType == NodeFormType.FoolForm || this.HisFormType == NodeFormType.Develop
                    || this.HisFormType == NodeFormType.RefNodeFrm || this.HisFormType == NodeFormType.FoolTruck)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 表单方案类型
        /// </summary>
        public NodeFormType HisFormType
        {
            get
            {
                return (NodeFormType)this.GetValIntByKey(NodeAttr.FormType);
            }
            set
            {
                this.SetValByKey(NodeAttr.FormType, (int)value);
            }
        }
        public string HisFormTypeText
        {
            get
            {
                if (this.HisFormType == NodeFormType.DisableIt)
                    return "树表单";

                if (this.HisFormType == NodeFormType.ExcelForm)
                    return "Excel表单";

                if (this.HisFormType == NodeFormType.FoolForm)
                    return "傻瓜表单";

                if (this.HisFormType == NodeFormType.FoolTruck)
                    return "傻瓜轨迹表单";

                if (this.HisFormType == NodeFormType.Develop)
                    return "开发者表单";

                if (this.HisFormType == NodeFormType.RefNodeFrm)
                    return "引用" + this.NodeFrmID;

                if (this.HisFormType == NodeFormType.SDKForm)
                    return "SDK表单";

                if (this.HisFormType == NodeFormType.SelfForm)
                    return "自定义表单";

                if (this.HisFormType == NodeFormType.SheetAutoTree)
                    return "动态表单树";

                return "未知";
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        public bool IsEnableTaskPool
        {
            get
            {
                if (this.TodolistModel == WF.TodolistModel.Sharing)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 节点头像
        /// </summary>
        public string Icon
        {
            get
            {
                string s = this.GetValStrByKey(NodeAttr.Icon);
                if (DataType.IsNullOrEmpty(s))
                    if (this.IsStartNode)
                        return "审核.png";
                    else
                        return "前台.png";
                return s;
            }
            set
            {
                this.SetValByKey(NodeAttr.Icon, value);
            }
        }
        /// <summary>
        /// FormUrl 
        /// </summary>
        public string FormUrl
        {
            get
            {
                string str = this.GetValStrByKey(NodeAttr.FormUrl);
                str = str.Replace("@SDKFromServHost", BP.Difference.SystemConfig.AppSettings["SDKFromServHost"]);
                return str;
            }
            set
            {
                this.SetValByKey(NodeAttr.FormUrl, value);
            }
        }
        /// <summary>
        /// 表单类型
        /// </summary>
        public NodeFormType FormType
        {
            get
            {
                return (NodeFormType)this.GetValIntByKey(NodeAttr.FormType);
            }
            set
            {
                this.SetValByKey(NodeAttr.FormType, (int)value);
            }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(EntityOIDNameAttr.Name);
            }
            set
            {
                this.SetValByKey(EntityOIDNameAttr.Name, value);
            }
        }
        /// <summary>
        /// 限期天
        /// </summary>
        public int TimeLimit
        {
            get
            {
                return (int)this.GetValFloatByKey(NodeAttr.TimeLimit);
            }
            set
            {
                this.SetValByKey(NodeAttr.TimeLimit, value);
            }
        }
        /// <summary>
        /// 限期小时
        /// </summary>
        public int TimeLimitHH
        {
            get
            {
                return this.GetParaInt("TimeLimitHH", 0);
            }
            set
            {
                this.SetPara("TimeLimitHH", value);
            }
        }
        /// <summary>
        /// 限期分钟
        /// </summary>
        public int TimeLimitMM
        {
            get
            {
                return this.GetParaInt("TimeLimitMM", 0);
            }
            set
            {
                this.SetPara("TimeLimitMM", value);
            }
        }

        /// <summary>
        /// 逾期提醒规则
        /// </summary>
        public CHAlertRole TAlertRole
        {
            get
            {
                return (CHAlertRole)this.GetValIntByKey(NodeAttr.TAlertRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.TAlertRole, (int)value);
            }
        }
        /// <summary>
        /// 时间计算方式
        /// </summary>
        public TWay TWay
        {
            get
            {
                return (TWay)this.GetValIntByKey(NodeAttr.TWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.TWay, (int)value);
            }
        }
        /// <summary>
        /// 逾期 - 提醒方式
        /// </summary>
        public CHAlertWay TAlertWay
        {
            get
            {
                return (CHAlertWay)this.GetValIntByKey(NodeAttr.TAlertWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.TAlertWay, (int)value);
            }
        }
        /// <summary>
        /// 预警天
        /// </summary>
        public float WarningDay
        {
            get
            {
                float i = this.GetValFloatByKey(NodeAttr.WarningDay);
                if (i == 0)
                    return 1;
                return i;
            }
            set
            {
                this.SetValByKey(NodeAttr.WarningDay, value);
            }
        }

        /// <summary>
        /// 预警 - 提醒规则
        /// </summary>
        public CHAlertRole WAlertRole
        {
            get
            {
                return (CHAlertRole)this.GetValIntByKey(NodeAttr.WAlertRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.WAlertRole, (int)value);
            }
        }
        /// <summary>
        /// 预警 - 提醒方式
        /// </summary>
        public CHAlertWay WAlertWay
        {
            get
            {
                return (CHAlertWay)this.GetValIntByKey(NodeAttr.WAlertWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.WAlertWay, (int)value);
            }
        }
        /// <summary>
        /// 保存方式 @0=仅节点表 @1=节点与NDxxxRtp表.
        /// </summary>
        public SaveModel SaveModel
        {
            get
            {
                return (SaveModel)this.GetValIntByKey(NodeAttr.SaveModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.SaveModel, (int)value);
            }
        }
        /// <summary>
        /// 流程步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }


        /// <summary>
        /// 扣分率（分/天）
        /// </summary>
        public float TCent
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.TCent);
            }
            set
            {
                this.SetValByKey(NodeAttr.TCent, value);
            }
        }
        /// <summary>
        /// 是否是客户执行节点？
        /// </summary>
        public bool IsGuestNode
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsGuestNode);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsGuestNode, value);
            }
        }


        /// <summary>
        /// 是否是开始节点
        /// </summary>
        public bool IsStartNode
        {
            get
            {
                if (this.No == "01")
                    return true;
                return false;

                //if (this.HisNodePosType == NodePosType.Start)
                //    return true;
                //else
                //    return false;
            }
        }
        /// <summary>
        /// x
        /// </summary>
        public int X
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.X);
            }
            set
            {
                if (value <= 0)
                    this.SetValByKey(NodeAttr.X, 5);
                else
                    this.SetValByKey(NodeAttr.X, value);
            }
        }
        /// <summary>
        /// y
        /// </summary>
        public int Y
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Y);
            }
            set
            {
                if (value <= 0)
                    this.SetValByKey(NodeAttr.Y, 5);
                else
                    this.SetValByKey(NodeAttr.Y, value);
            }
        }
        /// <summary>
        /// 水执行它？
        /// </summary>
        public int WhoExeIt
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.WhoExeIt);
            }
        }
        /// <summary>
        /// 位置
        /// </summary>
        public NodePosType NodePosType
        {
            get
            {
                return (NodePosType)this.GetValIntByKey(NodeAttr.NodePosType);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodePosType, (int)value);
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public RunModel HisRunModel
        {
            get
            {
                return (RunModel)this.GetValIntByKey(NodeAttr.RunModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.RunModel, (int)value);
            }
        }
        public NodeType HisNodeType
        {
            get
            {
                return (NodeType)this.GetValIntByKey(NodeAttr.NodeType);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeType, (int)value);
            }
        }
        /// <summary>
        /// 是不是子线程?
        /// </summary>
        public bool IsSubThread
        {
            get
            {
                if (this.HisRunModel == RunModel.SubThreadSameWorkID
                    || this.HisRunModel == RunModel.SubThreadUnSameWorkID)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 操纵提示
        /// </summary>
        public string Tip
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.Tip);
            }
            set
            {
                this.SetValByKey(NodeAttr.Tip, value);
            }
        }
        /// <summary>
        /// 焦点字段
        /// </summary>
        public string FocusField
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FocusField);
            }
            set
            {
                this.SetValByKey(NodeAttr.FocusField, value);
            }
        }
        /// <summary>
        /// 被退回节点退回信息地步.
        /// </summary>
        public string ReturnAlert
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.ReturnAlert);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnAlert, value);
            }
        }

        public bool ReturnCHEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.ReturnCHEnable);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnCHEnable, value);
            }
        }
        /// <summary>
        /// 退回原因
        /// </summary>
        public string ReturnReasonsItems
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.ReturnReasonsItems);
            }
        }
        /// <summary>
        /// 节点的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FK_Flow);
            }
            set
            {
                SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 获取它的上一步的分流点
        /// </summary>
        private Node _GetHisPriFLNode(Nodes nds)
        {
            foreach (Node mynd in nds)
            {
                if (mynd.IsFL)
                    return mynd;
                else
                    return _GetHisPriFLNode(mynd.FromNodes);
            }
            return null;
        }
        /// <summary>
        /// 它的上一步分流节点
        /// </summary>
        public Node HisPriFLNode
        {
            get
            {
                return _GetHisPriFLNode(this.FromNodes);
            }
        }
        public string TurnToDealDoc
        {
            get
            {
                string s = this.GetValStrByKey(NodeAttr.TurnToDealDoc);
                if (this.HisTurnToDeal == TurnToDeal.SpecUrl)
                {
                    if (s.Contains("1=1"))
                        return s;
                    if (s.Contains("?"))
                        s += "&1=1";
                    else
                        s += "?1=1";
                }
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                SetValByKey(NodeAttr.TurnToDealDoc, value);
            }
        }
        /// <summary>
        /// 可跳转的节点
        /// </summary>
        public string JumpToNodes
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.JumpToNodes);
            }
            set
            {
                SetValByKey(NodeAttr.JumpToNodes, value);
            }
        }

        public JumpWay JumpWay
        {
            get
            {
                return (JumpWay)this.GetValIntByKey(NodeAttr.JumpWay);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID = 0;
        /// <summary>
        /// 节点表单ID
        /// </summary>
        public string NodeFrmID
        {
            get
            {
               
                if (this.HisFlow.FlowDevModel != FlowDevModel.JiJian && this.HisFormType == NodeFormType.FoolForm || this.HisFormType == NodeFormType.ChapterFrm
                    || this.HisFormType == NodeFormType.FoolTruck
                    || this.HisFormType == NodeFormType.Develop)
                    return "ND" + this.NodeID; //@hongyan.


                string str = this.GetValStrByKey(NodeAttr.NodeFrmID);
                if (DataType.IsNullOrEmpty(str) == true)
                    return "ND" + this.NodeID;

                if (this.HisFormType == NodeFormType.FoolTruck
                    || this.HisFormType == NodeFormType.SheetTree)
                    return "ND" + this.NodeID;

                //与指定的节点相同 =  Pri 
                if (str.Equals("Pri") == true &&
                    (this.HisFormType == NodeFormType.FoolForm
                    || this.HisFormType == NodeFormType.Develop))
                {
                    if (this.WorkID == 0)
                        return "ND" + this.NodeID;
                    // throw new Exception("err@获得当前节点的上一个节点表单出现错误,没有给参数WorkID赋值.");

                    /* 要引擎上一个节点表单 */
                    string sql = "SELECT NDFrom FROM ND" + int.Parse(this.FK_Flow) + "Track A, WF_Node B ";
                    sql += " WHERE A.NDFrom=B.NodeID AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.Start + ")  ";
                    sql += "  AND (FormType=0 OR FormType=1) ";

                    sql += "  AND (A.WorkID=" + this.WorkID + ") ";

                    sql += " ORDER BY A.RDT ";

                    int nodeID = DBAccess.RunSQLReturnValInt(sql, 0);
                    if (nodeID == 0)
                        throw new Exception("err@没有找到指定的节点.");

                    return "ND" + nodeID;
                }

                //返回设置的表单ID.
                return str;
            }
            set
            {
                SetValByKey(NodeAttr.NodeFrmID, value);
            }
        }

        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FlowName);
            }
            set
            {
                SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 打印方式
        /// </summary>
        public bool HisPrintDocEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.PrintDocEnable);
            }
            set
            {
                this.SetValByKey(NodeAttr.PrintDocEnable, value);
            }
        }


        /// <summary>
        /// PDF打印规则
        /// </summary>
        public int HisPrintPDFModle
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.PrintPDFModle);
            }
            set
            {
                this.SetValByKey(BtnAttr.PrintPDFModle, (int)value);
            }
        }
        /// <summary>
        /// 打印水印设置规则
        /// </summary>
        public string ShuiYinModle
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShuiYinModle);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShuiYinModle, value);
            }
        }
        /// <summary>
        /// 批处理规则
        /// </summary>
        public BatchRole HisBatchRole
        {
            get
            {
                return (BatchRole)this.GetValIntByKey(NodeAttr.BatchRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.BatchRole, (int)value);
            }
        }
        /// <summary>
        /// 批量处理规则
        /// @显示的字段.
        /// </summary>
        public string BatchParas
        {
            get
            {
                return this.GetParaString("BatchFields");
            }
            set
            {
                this.SetPara("BatchFields", value);
            }
        }
        /// <summary>
        /// 批量审核数量
        /// </summary>
        public int BatchListCount
        {
            get
            {
                return this.GetParaInt("BatchCheckListCount");
            }
            set
            {
                this.SetPara("BatchCheckListCount", value);
            }
        }
        public string PTable
        {
            get
            {

                return "ND" + this.NodeID;
            }
            set
            {
                SetValByKey(NodeAttr.PTable, value);
            }
        }

        /// <summary>
        /// Doc
        /// </summary> 
        public string Doc
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.Doc);
            }
            set
            {
                SetValByKey(NodeAttr.Doc, value);
            }
        }
        public string GroupStaNDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.GroupStaNDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.GroupStaNDs, value);
            }
        }
        /// <summary>
        /// 到达的节点数量.
        /// </summary>
        public int HisToNDNum
        {
            get
            {
                string[] strs = this.HisToNDs.Split('@');
                return strs.Length - 1;
            }
        }
        /// <summary>
        /// 到达的节点
        /// </summary>
        public string HisToNDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisToNDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisToNDs, value);
            }
        }
        /// <summary>
        /// 部门Strs
        /// </summary>
        public string HisDeptStrs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisDeptStrs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisDeptStrs, value);
            }
        }
        public string HisStas
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisStas);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisStas, value);
            }
        }
        /// <summary>
        /// 单据IDs
        /// </summary>
        public string HisBillIDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisBillIDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisBillIDs, value);
            }
        }
        #endregion

        #region 退回信息.
        public string ReturnField
        {
            get
            {
                return this.GetValStrByKey(BtnAttr.ReturnField);
            }
            set
            {
                this.SetValByKey(BtnAttr.ReturnField, value);
            }
        }
        /// <summary>
        /// 单节点退回规则
        /// </summary>
        public int ReturnOneNodeRole
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.ReturnOneNodeRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnOneNodeRole, value);
            }
        }
        #endregion


        #region 公共方法
        /// <summary>
        /// 得到一个工作data实体
        /// </summary>
        /// <param name="workId">工作ID</param>
        /// <returns>如果没有就返回null</returns>
        public Work GetWork(Int64 workId)
        {
            Work wk = this.HisWork;
            wk.SetValByKey("OID", workId);
            if (wk.RetrieveFromDBSources() == 0)
                return null;
            else
                return wk;

        }
        #endregion

        #region 节点的工作类型
        /// <summary>
        /// 转向处理
        /// </summary>
        public TurnToDeal HisTurnToDeal
        {
            get
            {
                return (TurnToDeal)this.GetValIntByKey(NodeAttr.TurnToDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.TurnToDeal, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// 访问规则Text
        /// </summary>
        public string HisDeliveryWayText
        {
            get
            {
                SysEnum se = new SysEnum("DeliveryWay", (int)this.HisDeliveryWay);
                return se.Lab;
            }
        }
        /// <summary>
        /// 考核规则
        /// </summary>
        public CHWay HisCHWay
        {
            get
            {
                return (CHWay)this.GetValIntByKey(NodeAttr.CHWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.CHWay, (int)value);
            }
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        public CCRoleEnum HisCCRole
        {
            get
            {
                return (CCRoleEnum)this.GetValIntByKey(NodeAttr.CCRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.CCRole, (int)value);
            }
        }
        public string HisCCRoleText
        {
            get
            {
                SysEnum se = new SysEnum(NodeAttr.CCRole, (int)this.HisCCRole);
                return se.Lab;
            }
        }
        /// <summary>
        /// 删除流程规则
        /// </summary>
        public DelWorkFlowRole HisDelWorkFlowRole
        {
            get
            {
                return (DelWorkFlowRole)this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }
        /// <summary>
        /// 未找到处理人时是否跳转.
        /// </summary>
        public bool HisWhenNoWorker
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.WhenNoWorker);
            }
            set
            {
                this.SetValByKey(NodeAttr.WhenNoWorker, value);
            }
        }
        /// <summary>
        /// 撤销规则
        /// </summary>
        public CancelRole HisCancelRole
        {
            get
            {
                return (CancelRole)this.GetValIntByKey(NodeAttr.CancelRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.CancelRole, (int)value);
            }
        }
        /// <summary>
        /// 对方已读不能撤销
        /// </summary>
        public bool CancelDisWhenRead
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.CancelDisWhenRead);
            }
            set
            {
                this.SetValByKey(NodeAttr.CancelDisWhenRead, value);
            }
        }
        /// <summary>
        /// 数据写入规则
        /// </summary>
        public CCWriteTo CCWriteTo
        {
            get
            {
                return (CCWriteTo)this.GetValIntByKey(NodeAttr.CCWriteTo);
            }
            set
            {
                this.SetValByKey(NodeAttr.CCWriteTo, (int)value);
            }
        }
        /// <summary>
        /// Int type
        /// </summary>
        public NodeWorkType HisNodeWorkType
        {
            get
            {
                //#warning 2012-01-24修订,没有自动计算出来属性。
                switch (this.HisRunModel)
                {
                    case RunModel.Ordinary:
                        if (this.IsStartNode)
                            return NodeWorkType.StartWork;
                        else
                            return NodeWorkType.Work;
                    case RunModel.FL:
                        if (this.IsStartNode)
                            return NodeWorkType.StartWorkFL;
                        else
                            return NodeWorkType.WorkFL;
                    case RunModel.HL:
                        return NodeWorkType.WorkHL;
                    case RunModel.FHL:
                        return NodeWorkType.WorkFHL;
                    case RunModel.SubThreadSameWorkID:
                    case RunModel.SubThreadUnSameWorkID:
                        return NodeWorkType.SubThreadWork;
                    default:
                        throw new Exception("@没有判断类型NodeWorkType.");
                }
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeWorkType, (int)value);
            }
        }
        public string HisRunModelT
        {
            get
            {
                SysEnum se = new SysEnum(NodeAttr.RunModel, (int)this.HisRunModel);
                return se.Lab;
            }
        }
        #endregion

        #region 推算属性 (对于节点位置的判断)
        /// <summary>
        /// 类型
        /// </summary>
        public NodePosType HisNodePosType
        {
            get
            {
                this.SetValByKey(NodeAttr.NodePosType, (int)this.GetHisNodePosType());
                return (NodePosType)this.GetValIntByKey(NodeAttr.NodePosType);
            }
            set
            {
                if (value == NodePosType.Start)
                    if (this.No != "01")
                        value = NodePosType.Mid;

                this.SetValByKey(NodeAttr.NodePosType, (int)value);
            }
        }
        /// <summary>
        /// 是不是结束节点
        /// </summary>
        public bool IsEndNode
        {
            get
            {
                if (this.IsSendBackNode == true)
                    return false;

                if (this.HisNodePosType == NodePosType.End)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 异表单子线程WorkID生成规则
        /// </summary>
        public int USSWorkIDRole
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.USSWorkIDRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.USSWorkIDRole, value);
            }
        }
        /// <summary>
        /// 是否可以在退回后原路返回？
        /// </summary>
        public bool IsBackTracking
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsBackTracking);
            }
        }
        /// <summary>
        /// 原路返回后是否自动计算接收人
        /// </summary>
        public bool IsBackResetAccepter
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsBackResetAccepter);
            }
        }
        public bool IsSendDraftSubFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsSendDraftSubFlow);
            }
        }
        /// <summary>
        /// 是否杀掉全部的子线程
        /// </summary>
        public bool ThreadIsCanDel
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanDel);
            }
        }

        public bool ThreadIsCanAdd
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadIsCanAdd);
            }
        }
        /// <summary>
        /// 是否启用自动记忆功能
        /// </summary>
        public bool IsRememberMe
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsRM);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsRM, value);
            }
        }
        public string Mark
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.Mark);
            }
            set
            {
                this.SetValByKey(NodeAttr.Mark, value);
            }
        }
        /// <summary>
        /// 是否打开即审批
        /// </summary>
        public bool IsOpenOver
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsOpenOver);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsOpenOver, value);
            }
        }
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public bool IsCanDelFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCanDelFlow);
            }
        }
        /// <summary>
        /// 普通工作节点处理模式
        /// </summary>
        public TodolistModel TodolistModel
        {
            get
            {
                if (this.IsStartNode == true)
                    return WF.TodolistModel.QiangBan;

                return (TodolistModel)this.GetValIntByKey(NodeAttr.TodolistModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.TodolistModel, (int)value);
            }
        }
        /// <summary>
        /// 组长确认规则
        /// </summary>
        public TeamLeaderConfirmRole TeamLeaderConfirmRole
        {
            get
            {
                return (TeamLeaderConfirmRole)this.GetValIntByKey(NodeAttr.TeamLeaderConfirmRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.TeamLeaderConfirmRole, (int)value);
            }
        }
        /// <summary>
        /// 组长确认规则内容.
        /// </summary>
        public string TeamLeaderConfirmDoc
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.TeamLeaderConfirmDoc);
            }
            set
            {
                this.SetValByKey(NodeAttr.TeamLeaderConfirmDoc, value);
            }
        }
        public HuiQianLeaderRole HuiQianLeaderRole
        {
            get
            {
                return (HuiQianLeaderRole)this.GetValIntByKey(BtnAttr.HuiQianLeaderRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianLeaderRole, (int)value);
            }
        }
        /// <summary>
        /// 阻塞模式
        /// </summary>
        public BlockModel BlockModel
        {
            get
            {
                return (BlockModel)this.GetValIntByKey(NodeAttr.BlockModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.BlockModel, (int)value);
            }
        }
        /// <summary>
        /// 阻塞的表达式
        /// </summary>
        public string BlockExp
        {
            get
            {
                string str = this.GetValStringByKey(NodeAttr.BlockExp);

                if (DataType.IsNullOrEmpty(str))
                {
                    if (this.BlockModel == WF.BlockModel.CurrNodeAll)
                        return "还有子流程没有完成您不能提交,需要等到所有的子流程完成后您才能发送.";

                    if (this.BlockModel == WF.BlockModel.SpecSubFlow)
                        return "还有子流程没有完成您不能提交,需要等到所有的子流程完成后您才能发送.";
                }
                return str;
            }
            set
            {
                this.SetValByKey(NodeAttr.BlockExp, value);
            }
        }
        /// <summary>
        /// 被阻塞时提示信息
        /// </summary>
        public string BlockAlert
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.BlockAlert);
            }
            set
            {
                this.SetValByKey(NodeAttr.BlockAlert, value);
            }
        }
        /// <summary>
        /// 子线程删除规则
        /// </summary>
        public ThreadKillRole ThreadKillRole
        {
            get
            {
                return (ThreadKillRole)this.GetValIntByKey(NodeAttr.ThreadKillRole);
            }
        }
        /// <summary>
        /// 完成通过率
        /// </summary>
        public decimal PassRate
        {
            get
            {
                var val = this.GetValDecimalByKey(NodeAttr.PassRate);
                if (val == 0)
                    return 100;
                return val;
            }
        }
        /// <summary>
        /// 是否允许分配工作
        /// </summary>
        public bool IsTask
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsTask);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsTask, value);
            }
        }
        ///// <summary>
        ///// 是否是业务单元
        ///// </summary>
        //public bool IsBUnit
        //{
        //    get
        //    {
        //        return this.GetValBooleanByKey(NodeAttr.IsBUnit);
        //    }
        //    set
        //    {
        //        this.SetValByKey(NodeAttr.IsBUnit, value);
        //    }
        //}
        /// <summary>
        /// 是否可以移交
        /// </summary>
        public bool IsHandOver
        {
            get
            {
                if (this.IsStartNode)
                    return false;

                return this.GetValBooleanByKey(NodeAttr.IsHandOver);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsHandOver, value);
            }
        }
        /// <summary>
        /// 是否可以退回？
        /// </summary>
        public bool IsCanReturn
        {
            get
            {
                if (this.HisReturnRole == ReturnRole.CanNotReturn)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 已读回执
        /// </summary>
        public ReadReceipts ReadReceipts
        {
            get
            {
                return (ReadReceipts)this.GetValIntByKey(NodeAttr.ReadReceipts);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReadReceipts, (int)value);
            }
        }
        /// <summary>
        /// 退回规则
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// 是不是中间节点
        /// </summary>
        public bool IsMiddleNode
        {
            get
            {
                if (this.HisNodePosType == NodePosType.Mid)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否是工作质量考核点
        /// </summary>
        public bool IsEval
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsEval);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsEval, value);
            }
        }
        public string HisSubFlows11
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.HisSubFlows);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisSubFlows, value);
            }
        }
        public string FrmAttr
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FrmAttr);
            }
            set
            {
                this.SetValByKey(NodeAttr.FrmAttr, value);
            }
        }
        public bool IsHL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkHL:
                    case NodeWorkType.WorkFHL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 是否是分流
        /// </summary>
        public bool IsFL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkFL:
                    case NodeWorkType.WorkFHL:
                    case NodeWorkType.StartWorkFL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 是否分流合流
        /// </summary>
        public bool IsFLHL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkHL:
                    case NodeWorkType.WorkFL:
                    case NodeWorkType.WorkFHL:
                    case NodeWorkType.StartWorkFL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 按照角色智能获取人员模式 
        /// 0=集合模式,1=切片-严谨模式. 2=切片-宽泛模式
        /// </summary>
        public int DeliveryStationReqEmpsWay
        {
            get
            {
                int val = this.GetParaInt("StationReqEmpsWay", 0);
                return val;
            }
        }
        /// <summary>
        /// 接受人sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                string s = this.GetValStringByKey(NodeAttr.DeliveryParas);
                s = s.Replace("~", "'");

                if (this.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField && DataType.IsNullOrEmpty(s) == true)
                    return "ToEmps";
                return s;
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// 接受人员集合里,是否排除当前操作员?
        /// </summary>
        public bool IsExpSender
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsExpSender);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsExpSender, value);
            }
        }
        /// <summary>
        /// 是不是PC工作节点
        /// </summary>
        public bool IsPCNode
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 工作性质
        /// </summary>
        public string NodeWorkTypeText
        {
            get
            {
                return this.HisNodeWorkType.ToString();
            }
        }


        public int CHRole
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.CHRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.CHRole, value);
            }
        }
        #endregion

        #region 公共方法 (用户执行动作之后,所要做的工作)
        /// <summary>
        /// 用户执行动作之后,所要做的工作		 
        /// </summary>
        /// <returns>返回消息,运行的消息</returns>
        public string AfterDoTask()
        {
            return "";
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public Node() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="_oid">节点ID</param>	
        public Node(int _oid)
        {
            this.NodeID = _oid;
            this.Retrieve();

            //if (BP.Difference.SystemConfig.IsDebug)
            //{
            //    if (this.RetrieveFromDBSources() <= 0)
            //        throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            //}
            //else
            //{
            //    // 去掉缓存.
            //    int i = this.RetrieveFromDBSources();
            //    if (i == 0)
            //    {
            //        string err = "err@NodeID=" + this.NodeID + "不存在";
            //        err += "可能出现错误的原因如下:";
            //        err += "1.你在FEE中或者SDK模式中使用了节点跳转,跳转到的节点已经不存在.";
            //        throw new Exception(err);
            //    }

            //    //if (this.Retrieve() <= 0)
            //    //    throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ndName">表单ID,或者是Mark标记.</param>
        /// <exception cref="Exception"></exception>
        public Node(string ndName)
        {
            if (ndName.IndexOf("ND") == 0)
            {
                ndName = ndName.Replace("ND", "");
                this.NodeID = int.Parse(ndName);
                if (this.Retrieve() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + ndName);
                return;
            }

            if (this.Retrieve("Mark", ndName) == 0)
                throw new Exception("err@标记:" + ndName + "不存在.");


        }
        public string EnName
        {
            get
            {
                return "ND" + this.NodeID;
            }
        }
        public string EnsName
        {
            get
            {
                return "ND" + this.NodeID + "s";
            }
        }
        /// <summary>
        /// 节点意见名称，如果为空则取节点名称.
        /// </summary>
        public string FWCNodeName
        {
            get
            {
                string str = this.GetValStringByKey(NodeWorkCheckAttr.FWCNodeName);
                if (DataType.IsNullOrEmpty(str))
                    return this.Name;
                return str;
            }
        }
        /// <summary>
        /// 审核组件里面的工作人员先后顺序排列模式
        /// 0= 按照审批时间.
        /// 1= 按照接受人员列表(官职大小)
        /// </summary>
        public int FWCOrderModel
        {
            get
            {
                return this.GetValIntByKey(NodeWorkCheckAttr.FWCOrderModel);
            }
        }
        public float FWC_H
        {
            get
            {
                return this.GetValFloatByKey(NodeWorkCheckAttr.FWC_H);
            }
        }

        public HuiQianRole HuiQianRole
        {
            get
            {
                return (HuiQianRole)this.GetValIntByKey(BtnAttr.HuiQianRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianRole, (int)value);
            }
        }
        public bool IsResetAccepter
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsResetAccepter);
            }
        }

        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "节点");

                //出现 缓存问题.现在把缓存取消了.
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                #region 基本属性.
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 150, 10);
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, true, 0, 100, 10, false);
                map.AddTBString(NodeAttr.Mark, null, "标记", true, true, 0, 100, 10, false);


                map.AddTBInt(NodeAttr.Step, (int)NodeWorkType.Work, "流程步骤", true, false);

                map.AddTBString(NodeAttr.Icon, null, "节点ICON图片路径", true, false, 0, 70, 10);

                map.AddTBInt(NodeAttr.NodeWorkType, 0, "节点类型", false, false);
                // map.AddTBInt(NodeAttr.SubThreadType, 0, "子线程ID", false, false);

                map.AddTBString(NodeAttr.FK_Flow, null, "FK_Flow", false, false, 0, 3, 10);
                map.AddTBInt(NodeAttr.IsGuestNode, 0, "是否是客户执行节点", false, false);

                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 200, 10);

                //为铁路局,会签子流程. 增加
                map.AddTBInt(NodeAttr.IsSendDraftSubFlow, 0, "是否发送草稿子流程？", false, false);
                map.AddTBInt(NodeAttr.IsResetAccepter, 0, "可逆节点时是否重新计算接收人", false, false);
                map.AddTBString(NodeAttr.FrmAttr, null, "FrmAttr", false, true, 0, 300, 10);
                #endregion 基本属性.

                #region 审核组件.
                map.AddTBInt(NodeAttr.FWCSta, 0, "审核组件", false, false);
                map.AddTBFloat(NodeAttr.FWC_H, 0, "审核组件高度", false, true);
                map.AddTBInt(NodeWorkCheckAttr.FWCOrderModel, 0, "协作模式下操作员显示顺序", false, false);
                map.AddTBInt(NodeWorkCheckAttr.FWCVer, 0, "审核组件版本", false, false);
                map.AddTBInt("FWCAth", 0, "审核附件是否启用", false, false);
                map.AddTBString(NodeWorkCheckAttr.CheckField, null, "签批字段", true, false, 0, 50, 10, false);
                map.AddTBString(NodeWorkCheckAttr.FWCDefInfo, null, "默认意见", true, false, 0, 50, 10);
                #endregion 审核组件.

                #region 子流程信息
                map.AddTBInt(FrmSubFlowAttr.SFSta, 0, "父子流程组件", false, false);
                map.AddTBInt(NodeAttr.SubFlowX, 0, "子流程设计器位置X", false, false);
                map.AddTBInt(NodeAttr.SubFlowY, 0, "子流程设计器位置Y", false, false);
                #endregion 子流程信息

                #region 考核属性.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.CHRole, 0, "时限规则", true, false);

                map.AddTBString(BtnAttr.HelpLab, "帮助提示", "帮助", true, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.HelpRole, 0, "帮助提示规则", true, false);
                map.AddTBFloat(NodeAttr.TimeLimit, 2, "限期(天)", true, false); //"限期(天)".
                                                                             //  map.AddTBFloat(NodeAttr.TSpanHour, 0, "小时", true, false); //"限期(分钟)".
                map.AddTBInt(NodeAttr.TWay, 0, "时间计算方式", true, false); //0=不计算节假日,1=计算节假日.
                map.AddTBInt(NodeAttr.TAlertRole, 0, "逾期提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "逾期提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.WarningDay, 1, "工作预警(天)", true, false);    // "警告期限(0不警告)"

                map.AddTBInt(NodeAttr.WAlertRole, 0, "预警提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "预警提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "扣分(每延期1小时)", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "考核方式", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", true, true);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "超时处理方式", false, false);
                map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);
                //map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时的条件", false, false, 0, 200, 100);
                #endregion 考核属性.

                map.AddTBString(NodeWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);
                map.AddTBString(NodeAttr.Doc, null, "描述", true, false, 0, 100, 10);
                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true);

                //退回相关.
                map.AddTBInt(NodeAttr.ReturnRole, 2, "退回规则", true, true);
                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 50, 10, true);
                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 50, 10, true);
                map.AddBoolean(NodeAttr.ReturnCHEnable, false, "是否启用退回考核规则", true, true);

                map.AddTBInt(NodeAttr.ReturnOneNodeRole, 0, "单节点退回规则", true, true);
                map.AddTBString(BtnAttr.ReturnField, null, "退回信息填写字段", true, false, 0, 50, 10, true);

                map.AddTBInt(NodeAttr.DeliveryWay, 0, "访问规则", true, true);
                map.AddTBInt(NodeAttr.IsExpSender, 1, "本节点接收人不允许包含上一步发送人", true, true);

                map.AddTBInt(NodeAttr.CancelRole, 0, "撤销规则", true, true);
                map.AddTBInt(NodeAttr.CancelDisWhenRead, 0, "对方已读不能撤销", true, true);

                map.AddTBInt(NodeAttr.WhenNoWorker, 0, "未找到处理人时", true, true);
                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                map.AddTBString(NodeAttr.NodeFrmID, null, "节点表单ID", true, false, 0, 50, 10);

                map.AddTBInt(NodeAttr.CCRole, 0, "抄送规则", true, true);
                map.AddTBInt(NodeAttr.CCWriteTo, 0, "抄送数据写入规则", true, true);

                map.AddTBInt(BtnAttr.DelEnable, 0, "删除规则", true, true);
                map.AddTBInt(NodeAttr.SaveModel, 0, "保存模式", true, true);

                map.AddTBInt(NodeAttr.IsCanDelFlow, 0, "是否可以删除流程", true, true);

                map.AddTBInt(NodeAttr.ThreadKillRole, 0, "子线程删除方式", true, true);

                map.AddTBInt(NodeAttr.TodolistModel, 0, "多人处理规则", true, true);

                //add.
                map.AddTBInt(BtnAttr.HuiQianRole, 0, "会签模式", true, true);
                map.AddTBInt(NodeAttr.TeamLeaderConfirmRole, 0, "组长确认规则", true, true);
                map.AddTBString(NodeAttr.TeamLeaderConfirmDoc, null, "组长确认设置内容", true, false, 0, 100, 10);
                map.AddTBInt(BtnAttr.HuiQianLeaderRole, 0, "组长会签规则", true, true);

                map.AddTBInt(BtnAttr.ScripRole, 0, "小纸条规则", true, true);

                map.AddTBInt(NodeAttr.USSWorkIDRole, 0, "是否允许子线程接受人员重复(对子线程点有效)?", true, true);
                map.AddTBInt(NodeAttr.IsBackTracking, 1, "是否可以在退回后原路返回(只有启用退回功能才有效)", true, true);
                map.AddTBInt(NodeAttr.IsBackResetAccepter, 0, "原路返回后是否自动计算接收人", true, true);
                map.AddTBInt(BtnAttr.ThreadIsCanDel, 0, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true);
                map.AddTBInt(BtnAttr.ThreadIsCanAdd, 0, "是否可以增加子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效)？", true, true);

                map.AddTBInt(NodeAttr.IsKillEtcThread, 0, "是否允许删除所有的子线程(对于子线程向分流节点退回有效)", true, true);

                map.AddTBInt(NodeAttr.IsRM, 1, "是否启用投递路径自动记忆功能?", true, true);
                map.AddTBInt(NodeAttr.IsOpenOver, 0, "是否打开即审批?", true, true);
                map.AddBoolean(NodeAttr.IsHandOver, false, "是否可以移交", true, true);
                map.AddTBDecimal(NodeAttr.PassRate, 100, "通过率", true, true);
                map.AddTBInt(NodeAttr.RunModel, 0, "运行模式(对普通节点有效)", true, true);
                map.AddTBInt(NodeAttr.NodeType, 0, "节点类型", true, true); //2023.06.
                map.AddTBInt(NodeAttr.BlockModel, 0, "阻塞模式", true, true);
                map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 200, 10);
                map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞提示信息", true, false, 0, 100, 10);

                map.AddTBInt(NodeAttr.WhoExeIt, 0, "谁执行它", true, true);
                map.AddTBInt(NodeAttr.ReadReceipts, 0, "已读回执", true, true);
                map.AddTBInt(NodeAttr.CondModel, 0, "方向条件控制规则", true, true);

                // 自动跳转.
                map.AddTBInt(NodeAttr.AutoJumpRole0, 0, "处理人就是提交人0", false, false);
                map.AddTBInt(NodeAttr.AutoJumpRole1, 0, "处理人已经出现过1", false, false);
                map.AddTBInt(NodeAttr.AutoJumpRole2, 0, "处理人与上一步相同2", false, false);

                map.AddTBString(NodeAttr.AutoJumpExp, null, "表达式", true, false, 0, 200, 10, true);
                //@0=上一个节点发送时@1=当前节点工作打开时.
                map.AddTBInt(NodeAttr.SkipTime, 0, "执行跳转事件", false, false);

                // 批处理规则， 2021.1.20 为福建人寿重构.
                // @0=不启用，1=审核组件模式，2=审核分组字段模式,3=自定义url模式.
                map.AddTBInt(NodeAttr.BatchRole, 0, "批处理", true, true);

                //map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, true);
                //map.AddTBString(NodeAttr.BatchParas, null, "参数", true, false, 0, 500, 10);

                map.AddTBInt(NodeAttr.PrintDocEnable, 0, "打印方式", true, true);
                //打印PDF的处理
                map.AddTBInt(BtnAttr.PrintPDFModle, 0, "PDF打印规则", true, true);
                map.AddTBInt(BtnAttr.PRIEnable, 0, "重要性规则", true, true);
                map.AddTBString(BtnAttr.ShuiYinModle, null, "打印水印规则", true, false, 20, 100, 100, true);

                //与未来处理人有关系.
                //map.AddTBInt(NodeAttr.IsFullSA, 1, "是否计算未来处理人?", false, false);
                //map.AddTBInt(NodeAttr.IsFullSATime, 0, "是否计算未来接受与处理时间?", false, false);
                //map.AddTBInt(NodeAttr.IsFullSAAlert, 0, "是否接受未来工作到达消息提醒?", false, false);

                //表单相关.
                map.AddTBInt(NodeAttr.FormType, 1, "表单类型", false, false);
                map.AddTBString(NodeAttr.FormUrl, "http://", "表单URL", true, false, 0, 300, 10);
                map.AddTBInt(NodeAttr.TurnToDeal, 0, "转向处理", false, false);
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "发送后提示信息", true, false, 0, 200, 10, true);
                map.AddTBInt(NodeAttr.NodePosType, 0, "位置", false, false);
                map.AddTBString(NodeAttr.HisStas, null, "角色", false, false, 0, 300, 10);
                map.AddTBString(NodeAttr.HisDeptStrs, null, "部门", false, false, 0, 600, 10);
                map.AddTBString(NodeAttr.HisToNDs, null, "转到的节点", false, false, 0, 80, 10);
                map.AddTBString(NodeAttr.HisBillIDs, null, "单据IDs", false, false, 0, 50, 10);
                //  map.AddTBString(NodeAttr.HisEmps, null, "HisEmps", false, false, 0, 3000, 10);
                map.AddTBString(NodeAttr.HisSubFlows, null, "HisSubFlows", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.PTable, null, "物理表", false, false, 0, 100, 10);

                map.AddTBString(NodeAttr.GroupStaNDs, null, "角色分组节点", false, false, 0, 200, 10);
                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 100, 10, true);
                map.AddTBInt(NodeAttr.JumpWay, 0, "跳转规则", false, false);
                map.AddTBString(NodeAttr.RefOneFrmTreeType, "", "独立表单类型", false, false, 0, 100, 10);//RefOneFrmTree

                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时的条件", false, false, 0, 200, 100);

                //按钮控制部分.
                // map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10, true);
                map.AddTBAtParas(500);

                // 启动自动运行. 2013-01-04
                //map.AddTBInt(NodeAttr.AutoRunEnable, 0, "是否启动自动运行？", true, false);
                //map.AddTBString(NodeAttr.AutoRunParas, null, "自动运行参数", true, false, 0, 100, 10);
                map.AddTBString(NodeAttr.SelfParas, null, "自定义参数(如果太小可以手动扩大)", true, false, 0, 1000, 10);

                #region 与参数有关系的属性。

                #region 子流程相关的参数
                map.AddTBFloat(FrmSubFlowAttr.SF_H, 300, "高度", true, false);
                #endregion 子流程相关的参数

                //map.AddDDLSysEnum(FrmEventAttr.MsgCtrl, 0, "消息发送控制", true, true, FrmEventAttr.MsgCtrl,
                //  "@0=不发送@1=按设置的发送范围自动发送@2=由本节点表单系统字段(IsSendEmail,IsSendSMS)来决定@3=由SDK开发者参数(IsSendEmail,IsSendSMS)来决定", true);

                //map.AddBoolean(FrmEventAttr.MailEnable, true, "是否启用邮件发送？(如果启用就要设置邮件模版，支持ccflow表达式。)", true, true, true);
                //map.AddTBString(FrmEventAttr.MailTitle, null, "邮件标题模版", true, false, 0, 200, 20, true);
                //map.AddTBStringDoc(FrmEventAttr.MailDoc, null, "邮件内容模版", true, false, true);

                ////是否启用手机短信？
                //map.AddBoolean(FrmEventAttr.SMSEnable, false, "是否启用短信发送？(如果启用就要设置短信模版，支持ccflow表达式。)", true, true, true);
                //map.AddTBStringDoc(FrmEventAttr.SMSDoc, null, "短信内容模版", true, false, true);
                //map.AddBoolean(FrmEventAttr.MobilePushEnable, true, "是否推送到手机、pad端。", true, true, true);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 删除前的逻辑处理.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            // 检查设计流程权限,集团模式下，不是自己创建的流程，不能设计流程.
            BP.WF.Template.TemplateGlo.CheckPower(this.FK_Flow);

            int num = 0;
            //如果是结束节点，则自动结束流程.
            //if (this.NodePosType == NodePosType.End)
            //{
            //    GenerWorkFlows gwfs = new GenerWorkFlows();
            //    gwfs.Retrieve("FK_Flow", this.FK_Flow);
            //    foreach (GenerWorkFlow gwf in gwfs)
            //    {
            //        try
            //        {
            //            BP.WF.Dev2Interface.Flow_DoFlowOver(gwf.WorkID, "流程成功结束");
            //        }
            //        catch (Exception ex)
            //        {
            //            //删除错误，有可能是删除该流程.
            //            continue;
            //        }
            //    }
            //}
            // 判断是否可以被删除. 
            num = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Node=" + this.NodeID + " AND IsPass=0 ");
            if (num != 0)
                throw new Exception("@该节点[" + this.NodeID + "," + this.Name + "]有待办工作存在，您不能删除它.");

            // 删除它的节点
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            md.Delete();

            // 删除分组.
            BP.Sys.GroupFields gfs = new BP.Sys.GroupFields();
            gfs.Delete(BP.Sys.GroupFieldAttr.FrmID, md.No);

            // 删除它的明细
            BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(md.No);
            dtls.Delete();

            //删除框架.
            BP.Sys.MapFrames frams = new BP.Sys.MapFrames(md.No);
            frams.Delete();

            //删除扩展.
            BP.Sys.MapExts exts = new BP.Sys.MapExts(md.No);
            exts.Delete();

            //删除节点与角色的对应.
            DBAccess.RunSQL("DELETE FROM WF_NodeStation WHERE FK_Node=" + this.NodeID);
            DBAccess.RunSQL("DELETE FROM WF_NodeEmp  WHERE FK_Node=" + this.NodeID);
            DBAccess.RunSQL("DELETE FROM WF_NodeDept WHERE FK_Node=" + this.NodeID);
            DBAccess.RunSQL("DELETE FROM WF_FrmNode  WHERE FK_Node=" + this.NodeID);
            DBAccess.RunSQL("DELETE FROM WF_CCEmp  WHERE FK_Node=" + this.NodeID);
            DBAccess.RunSQL("DELETE FROM WF_CH WHERE FK_Node=" + this.NodeID);

            //删除附件.
            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachment WHERE FK_MapData='" + this.NodeID + "'");

            //删除节点后，把关联该节点表单的ID也要删除掉. 同步过去.
            DBAccess.RunSQL("UPDATE WF_Node SET NodeFrmID='' WHERE NodeFrmID='ND" + this.NodeID + "' AND FK_Flow='" + this.FK_Flow + "'");

            //写入日志.
            BP.Sys.Base.Glo.WriteUserLog("删除节点:" + this.Name + " - " + this.NodeID);

            return base.beforeDelete();
        }

        /// <summary>
        /// 修复map
        /// </summary>
        public string RepareMap(Flow fl)
        {
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            if (md.RetrieveFromDBSources() == 0)
            {
                this.CreateMap();
                return "";
            }

            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, md.No);

            #region 增加节点必要的字段.
            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (attrs.Contains(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr.setFK_MapData(md.No);
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

            if (this.IsSubThread == false)
                return "修复成功.";


            if (attrs.Contains(MapAttrAttr.KeyOfEn, "FID", MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setKeyOfEn("FID");
                attr.setName("FID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setEditType(EditType.UnDel);
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, GERptAttr.RDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(GERptAttr.RDT);
                attr.setName("接受时间");  //"接受时间";
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, GERptAttr.CDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(GERptAttr.CDT);
                if (this.IsStartNode)
                    attr.setName("发起时间"); //"发起时间";
                else
                    attr.setName("完成时间"); //"完成时间";

                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.Rec, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(WorkAttr.Rec);
                if (this.IsStartNode == false)
                    attr.setName("记录人"); // "记录人";
                else
                    attr.setName("发起人"); //"发起人";

                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMaxLen(100);
                attr.setMinLen(0);
                attr.DefVal = "@WebUser.No";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.Emps, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(WorkAttr.Emps);
                attr.Name = WorkAttr.Emps;
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMaxLen(8000);
                attr.setMinLen(0);
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, GERptAttr.FK_Dept, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(GERptAttr.FK_Dept);
                attr.setName("操作员部门"); //"操作员部门";
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.DDL);
                attr.setLGType(FieldTypeS.FK);
                attr.UIBindKey = "BP.Port.Depts";
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Insert();
            }


            if (fl.IsMD5 && attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.MD5, MapAttrAttr.FK_MapData, md.No) == false)
            {
                /* 如果是MD5加密流程. */
                attr = new BP.Sys.MapAttr();
                attr.setFK_MapData(md.No);
                attr.setEditType(EditType.UnDel);
                attr.setKeyOfEn(WorkAttr.MD5);
                attr.UIBindKey = attr.KeyOfEn;
                attr.setName("MD5");
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setUIVisible(false);
                attr.setMinLen(0);
                attr.setMaxLen(40);
                attr.Idx = -100;
                attr.Insert();
            }

            if (this.NodePosType == NodePosType.Start)
            {

                if (attrs.Contains(MapAttrAttr.KeyOfEn, GERptAttr.Title, MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.setFK_MapData(md.No);
                    attr.setEditType(EditType.UnDel);
                    attr.setKeyOfEn(GERptAttr.Title);
                    attr.setName("标题"); // "流程标题";
                    attr.setMyDataType(DataType.AppString);
                    attr.setUIContralType(UIContralType.TB);
                    attr.setLGType(FieldTypeS.Normal);
                    attr.setUIVisible(false);
                    attr.setUIIsEnable(true);
                    attr.UIIsLine = true;
                    attr.UIWidth = 251;

                    attr.setMinLen(0);
                    attr.setMaxLen(200);
                    attr.Idx = -100;
                    //attr.X = (float)171.2;
                    //attr.Y = (float)68.4;
                    attr.Insert();
                }
            }
            #endregion 增加节点必要的字段.

            //表单自检.
            md.RepairMap();

            string msg = "";
            if (this.FocusField != "")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, this.FocusField, MapAttrAttr.FK_MapData, md.No) == false)
                    msg += "@焦点字段 " + this.FocusField + " 被非法删除了.";
            }

            return msg;
        }
        /// <summary>
        /// 建立map
        /// </summary>
        public void CreateMap()
        {
            //创建节点表单.
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            md.Delete();

            md.Name = this.Name;

            if (this.HisFormType == NodeFormType.FoolForm || this.HisFormType == NodeFormType.FoolTruck)
                md.HisFrmType = FrmType.FoolForm;

            if (this.HisFormType == NodeFormType.Develop)
                md.HisFrmType = FrmType.Develop;

            md.PTable = this.HisFlow.PTable;
            md.Insert();

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            attr.setFK_MapData(md.No);
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


            attr = new BP.Sys.MapAttr();
            attr.setFK_MapData(md.No);
            attr.setKeyOfEn("FID");
            attr.setName("FID");
            attr.setMyDataType(DataType.AppInt);
            attr.setUIContralType(UIContralType.TB);
            attr.setLGType(FieldTypeS.Normal);
            attr.setUIVisible(false);
            attr.setUIIsEnable(false);
            attr.setEditType(EditType.UnDel);
            attr.DefVal = "0";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
            attr.setEditType(BP.En.EditType.UnDel);
            attr.SetValByKey(MapAttrAttr.KeyOfEn, "RDT");
            attr.SetValByKey(MapAttrAttr.Name, "接受时间");
            attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDateTime);
            attr.setUIContralType(UIContralType.TB);
            attr.SetValByKey(MapAttrAttr.UIVisible, false);
            attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
            attr.setLGType(FieldTypeS.Normal);
            attr.SetValByKey(MapAttrAttr.MinLen, 0);
            attr.SetValByKey(MapAttrAttr.MaxLen, 40);
            attr.SetValByKey(MapAttrAttr.DefVal, "@RDT");
            attr.SetValByKey(MapAttrAttr.Tag, "1");
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
            attr.setEditType(BP.En.EditType.UnDel);
            attr.SetValByKey(MapAttrAttr.KeyOfEn, "Rec");

            if (this.IsStartNode == false)
                attr.SetValByKey(MapAttrAttr.Name, "发起时间");
            else
                attr.SetValByKey(MapAttrAttr.Name, "完成时间");

            attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDateTime);
            attr.setUIContralType(UIContralType.TB);
            attr.SetValByKey(MapAttrAttr.UIVisible, false);
            attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
            attr.setLGType(FieldTypeS.Normal);
            attr.SetValByKey(MapAttrAttr.MinLen, 0);
            attr.SetValByKey(MapAttrAttr.MaxLen, 40);
            attr.SetValByKey(MapAttrAttr.DefVal, "@RDT");
            attr.SetValByKey(MapAttrAttr.Tag, "1");
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
            attr.setEditType(BP.En.EditType.UnDel);
            attr.SetValByKey(MapAttrAttr.KeyOfEn, "Rec");

            if (this.IsStartNode == false)
                attr.SetValByKey(MapAttrAttr.Name, "记录人");
            else
                attr.SetValByKey(MapAttrAttr.Name, "发起人");

            attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
            attr.setUIContralType(UIContralType.TB);
            attr.SetValByKey(MapAttrAttr.UIVisible, false);
            attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
            attr.setLGType(FieldTypeS.Normal);
            attr.SetValByKey(MapAttrAttr.MinLen, 0);
            attr.SetValByKey(MapAttrAttr.MaxLen, 8000);
            attr.SetValByKey(MapAttrAttr.DefVal, "@WebUser.No");
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
            attr.setEditType(BP.En.EditType.UnDel);
            attr.SetValByKey(MapAttrAttr.KeyOfEn, "Emps");
            attr.SetValByKey(MapAttrAttr.Name, "Emps");
            attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
            attr.setUIContralType(UIContralType.TB);
            attr.SetValByKey(MapAttrAttr.UIVisible, false);
            attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
            attr.setLGType(FieldTypeS.Normal);
            attr.SetValByKey(MapAttrAttr.MinLen, 0);
            attr.SetValByKey(MapAttrAttr.MaxLen, 8000);
            attr.Insert();



            attr = new BP.Sys.MapAttr();
            attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
            attr.setEditType(BP.En.EditType.UnDel);
            attr.SetValByKey(MapAttrAttr.KeyOfEn, "FK_Dept");
            attr.SetValByKey(MapAttrAttr.Name, "操作员部门");
            attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
            attr.setUIContralType(UIContralType.TB);
            attr.SetValByKey(MapAttrAttr.UIVisible, false);
            attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
            attr.setLGType(FieldTypeS.Normal);
            attr.SetValByKey(MapAttrAttr.MinLen, 0);
            attr.SetValByKey(MapAttrAttr.MaxLen, 50);
            attr.Insert();

            attr = new BP.Sys.MapAttr();
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

            if (this.NodePosType == NodePosType.Start)
            {
                //开始节点信息.
                attr = new BP.Sys.MapAttr();

                attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
                attr.setEditType(BP.En.EditType.Edit);

                attr.SetValByKey(MapAttrAttr.KeyOfEn, "Title");
                attr.SetValByKey(MapAttrAttr.Name, "标题");
                attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.SetValByKey(MapAttrAttr.UIVisible, false);
                attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
                attr.setLGType(FieldTypeS.Normal);
                attr.SetValByKey(MapAttrAttr.MinLen, 0);
                attr.SetValByKey(MapAttrAttr.MaxLen, 200);
                attr.Insert();

                attr = new BP.Sys.MapAttr();
                attr.SetValByKey(MapAttrAttr.FK_MapData, md.No);
                attr.setEditType(BP.En.EditType.UnDel);
                attr.SetValByKey(MapAttrAttr.KeyOfEn, "FK_NY");
                attr.SetValByKey(MapAttrAttr.Name, "年月");
                attr.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.SetValByKey(MapAttrAttr.UIVisible, false);
                attr.SetValByKey(MapAttrAttr.UIIsEnable, false);
                attr.setLGType(FieldTypeS.Normal);
                attr.SetValByKey(MapAttrAttr.MinLen, 0);
                attr.SetValByKey(MapAttrAttr.MaxLen, 7);
                attr.Insert();
            }
        }
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class Nodes : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Node();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public Nodes()
        {
        }
        /// <summary>
        /// 节点集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public Nodes(string fk_flow)
        {
            //   Nodes nds = new Nodes();
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
            //this.AddEntities(NodesCash.GetNodes(fk_flow));
            return;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Node> ToJavaList()
        {
            return (System.Collections.Generic.IList<Node>)this;
        }
        /// <summary>
        /// 转化成list 为了翻译成java的需要
        /// </summary>
        /// <returns>List</returns>
        public List<BP.WF.Node> Tolist()
        {
            List<BP.WF.Node> list = new List<BP.WF.Node>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BP.WF.Node)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
