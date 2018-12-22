using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using System.Collections;
using BP.Port;
using BP.WF.Data;
using BP.WF.Template;
using BP.WF.Port;
using System.Collections.Generic;

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
        public CondModel CondModel
        {
            get
            {
                return (CondModel)this.GetValIntByKey(NodeAttr.CondModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.CondModel, (int)value);
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
        /// 子线程类型
        /// </summary>
        public SubThreadType HisSubThreadType
        {
            get
            {
                return (SubThreadType)this.GetValIntByKey(NodeAttr.SubThreadType);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubThreadType, (int)value);
            }
        }
        #endregion

        #region 外键属性.
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
                    obj.AddEntities(this.HisToNDs);
                    this.SetRefObject("HisToNodes", obj);
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
                if (this.IsStartNode)
                {
                    wk = new BP.WF.GEStartWork(this.NodeID, this.NodeFrmID);
                    wk.HisNode = this;
                    wk.NodeID = this.NodeID;
                    return wk;
                }

                if (this.FormType != NodeFormType.FoolTruck || this.WorkID == 0 || this.IsStartNode == true)
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
                sql += "  AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.Start + ")  ";
                sql += "  AND B.FormType=" + (int)NodeFormType.FoolTruck + " "; // 仅仅找累加表单.
                sql += "  AND NDFrom!=" + this.NodeID + " "; //排除当前的表单.

                //if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+CONVERT(varchar(10),B.NodeID) ) ";

                //if (SystemConfig.AppCenterDBType == DBType.MySQL)
                //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+cast(B.NodeID as varchar(10)) ) ";

                //if (SystemConfig.AppCenterDBType == DBType.Oracle)
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
                        item.UIIsEnable = false; //设置为只读的.
                        item.DefValReal = "";    //设置默认值为空.

                        ma.Attrs.Add(item.HisAttr);
                    }

                    //设置为空.
                    wk.SQLCash = null;
                }

                wk.HisNode = this;
                wk.NodeID = this.NodeID;
                wk.SQLCash = null;

                BP.DA.Cash.SQL_Cash.Remove( "ND"+this.NodeID);
                return wk;
                //this.SetRefObject("HisWork", obj);

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
                return obj;
                ////Works obj = this.GetRefObject("HisWorks") as Works;
                ////if (obj == null)
                ////{
                //    this.SetRefObject("HisWorks",obj);
                //}
                //return obj;
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
                     obj = new PushMsgs(this.NodeID);

                    //检查是否有默认的发送？如果没有就增加上他。
                     bool isHaveSend = false;
                     bool isHaveReturn = false;
                     foreach (PushMsg item in obj)
                     {
                         if (item.FK_Event == EventListOfNode.SendSuccess)
                             isHaveSend = true;

                         if (item.FK_Event == EventListOfNode.ReturnAfter)
                             isHaveReturn = true;
                     }

                     if (isHaveSend == false)
                     {
                         PushMsg pm = new PushMsg();
                         pm.FK_Event = EventListOfNode.SendSuccess;
                         pm.MailPushWay = 1; /*默认: 让其使用消息提醒.*/
                         pm.SMSPushWay = 0;  /*默认:不让其使用短信提醒.*/
                         obj.AddEntity(pm);
                     }

                     if (isHaveReturn == false)
                     {
                         PushMsg pm = new PushMsg();
                         pm.FK_Event = EventListOfNode.ReturnAfter;
                         pm.MailPushWay = 1; /*默认: 让其使用消息提醒.*/
                         pm.SMSPushWay = 0;  /*默认:不让其使用短信提醒.*/
                         obj.AddEntity(pm);
                     }

                    this.SetRefObject("PushMsg", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// HisFrms
        /// </summary>
        public Frms HisFrms
        {
            get
            {
                Frms frms = new Frms();
                FrmNodes fns = new FrmNodes(this.FK_Flow, this.NodeID);
                foreach (FrmNode fn in fns)
                {
                    if (fn.FrmEnableRole == FrmEnableRole.Disable)
                        continue;
                    frms.AddEntity(fn.HisFrm);
                }
                return frms;

                //this.SetRefObject("HisFrms", obj);
                //Frms obj = this.GetRefObject("HisFrms") as Frms;
                //if (obj == null)
                //{
                //    obj = new Frms();
                //    FrmNodes fns = new FrmNodes(this.NodeID);
                //    foreach (FrmNode fn in fns)
                //        obj.AddEntity(fn.HisFrm);
                //    this.SetRefObject("HisFrms", obj);
                //}
                //return obj;
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
        public BillTemplates BillTemplates
        {
            get
            {
                BillTemplates obj = this.GetRefObject("BillTemplates") as BillTemplates;
                if (obj == null)
                {
                    obj = new BillTemplates(this.NodeID);
                    this.SetRefObject("BillTemplates", obj);
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
        /// 节点事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                FrmEvents obj = this.GetRefObject("FrmEvents") as FrmEvents;
                if (obj == null)
                {
                    obj = new FrmEvents(this.NodeID);
                    this.SetRefObject("FrmEvents", obj);
                }
                return obj;
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
                if (BP.Web.WebUser.No == "admin")
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
            string nodeid = this.NodeID.ToString();
            if (nodeid.Substring(nodeid.Length - 2) == "01")
                return NodePosType.Start;

            if (this.FromNodes.Count == 0)
                return NodePosType.Mid;

            if (this.HisToNodes.Count == 0)
                return NodePosType.End;
            return NodePosType.Mid;
        }
        /// <summary>
        /// 检查流程，修复必要的计算字段信息.
        /// </summary>
        /// <param name="fl">流程</param>
        /// <returns>返回检查信息</returns>
        public static string CheckFlow(Flow fl)
        {
            string sqls = "UPDATE WF_Node SET IsCCFlow=0";
            sqls += "@UPDATE WF_Node  SET IsCCFlow=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond a WHERE a.NodeID= NodeID AND CondType=1 )";
            BP.DA.DBAccess.RunSQLs(sqls);

            if (SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                // 删除必要的数据.
                DBAccess.RunSQL("DELETE FROM WF_NodeEmp WHERE FK_Emp  NOT IN (SELECT No from Port_Emp)");
                DBAccess.RunSQL("DELETE FROM WF_Emp WHERE NO NOT IN (SELECT No FROM Port_Emp )");
                DBAccess.RunSQL("UPDATE WF_Emp SET Name=(SELECT Name From Port_Emp WHERE Port_Emp.No=WF_Emp.No),FK_Dept=(select FK_Dept from Port_Emp where Port_Emp.No=WF_Emp.No)");
            }

            Nodes nds = new Nodes();
            nds.Retrieve(NodeAttr.FK_Flow, fl.No);

            //FlowSort fs = new FlowSort(fl.FK_FlowSort);

            if (nds.Count == 0)
                return "流程[" + fl.No + fl.Name + "]中没有节点数据，您需要注册一下这个流程。";

            // 更新是否是有完成条件的节点。
            BP.DA.DBAccess.RunSQL("UPDATE WF_Node SET IsCCFlow=0  WHERE FK_Flow='" + fl.No + "'");
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE Node=0 OR ToNode=0");
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE Node  NOT IN (SELECT NODEID FROM WF_Node )");
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE ToNode  NOT IN (SELECT NODEID FROM WF_Node) ");

            string sql = "";
            DataTable dt = null;

            // 单据信息，岗位，节点信息。
            foreach (Node nd in nds)
            {
                BP.Sys.MapData md = new BP.Sys.MapData();
                md.No = "ND" + nd.NodeID;
                if (md.IsExits == false)
                    nd.CreateMap();

                // 工作岗位。
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

                // 单据
                strs = "";
                BillTemplates temps = new BillTemplates(nd);
                foreach (BillTemplate temp in temps)
                    strs += "@" + temp.No;
                nd.HisBillIDs = strs;

                // 检查节点的位置属性。
                nd.HisNodePosType = nd.GetHisNodePosType();
                try
                {
                    nd.DirectUpdate();
                }
                catch(Exception ex)
                {
                    throw new Exception("err@" + ex.Message + " node=" + nd.Name);
                }
            }

            // 处理岗位分组.
            sql = "SELECT HisStas, COUNT(*) as NUM FROM WF_Node WHERE FK_Flow='" + fl.No + "' GROUP BY HisStas";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
            /* 判断流程的类型 */
            sql = "SELECT Name FROM WF_Node WHERE (NodeWorkType=" + (int)NodeWorkType.StartWorkFL + " OR NodeWorkType=" + (int)NodeWorkType.WorkFHL + " OR NodeWorkType=" + (int)NodeWorkType.WorkFL + " OR NodeWorkType=" + (int)NodeWorkType.WorkHL + ") AND (FK_Flow='" + fl.No + "')";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            fl.DirectUpdate();
            return null;
        }

        protected override bool beforeUpdate()
        {
            if (this.IsStartNode)
            {
                this.SetValByKey(BtnAttr.ReturnRole, (int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.ShiftEnable, 0);
                this.SetValByKey(BtnAttr.EndFlowEnable, 0);
            }

            //给icon设置默认值.
            if (this.GetValStrByKey(NodeAttr.ICON) == "")
                this.ICON = "审核.png";

            #region 如果是数据合并模式，就要检查节点中是否有子线程，如果有子线程就需要单独的表.
            if (this.HisRunModel == RunModel.SubThread)
            {
                MapData md = new MapData("ND" + this.NodeID);
                if (md.PTable != "ND" + this.NodeID)
                {
                    md.PTable = "ND" + this.NodeID;
                    md.Update();
                }
            }
            #endregion 如果是数据合并模式，就要检查节点中是否有子线程，如果有子线程就需要单独的表.

            //更新版本号.
            Flow.UpdateVer(this.FK_Flow);
             

            #region 更新流程判断条件的标记。
            DBAccess.RunSQL("UPDATE WF_Node SET IsCCFlow=0  WHERE FK_Flow='" + this.FK_Flow + "'");
            DBAccess.RunSQL("UPDATE WF_Node SET IsCCFlow=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond WHERE CondType=1) AND FK_Flow='" + this.FK_Flow + "'");
            #endregion


            Flow fl = new Flow(this.FK_Flow);

            Node.CheckFlow(fl);
            this.FlowName = fl.Name;

            DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + this.Name + "' WHERE No='ND" + this.NodeID + "'");
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
                case RunModel.SubThread:
                    this.HisNodeWorkType = NodeWorkType.SubThreadWork;
                    break;
                default:
                    throw new Exception("eeeee");
                    break;
            }
            //创建审核组件附件
            FrmAttachment workCheckAth = new FrmAttachment();
            bool isHave = workCheckAth.RetrieveByAttr(FrmAttachmentAttr.MyPK, this.NodeID + "_FrmWorkCheck");
            //不包含审核组件
            if (isHave == false)
            {
                workCheckAth = new FrmAttachment();
                /*如果没有查询到它,就有可能是没有创建.*/
                workCheckAth.MyPK = this.NodeID + "_FrmWorkCheck";
                workCheckAth.FK_MapData = this.NodeID.ToString();
                workCheckAth.NoOfObj = this.NodeID + "_FrmWorkCheck";
                workCheckAth.Exts = "*.*";

                //存储路径.
                workCheckAth.SaveTo = "/DataUser/UploadFile/";
                workCheckAth.IsNote = false; //不显示note字段.
                workCheckAth.IsVisable = false; // 让其在form 上不可见.

                //位置.
                workCheckAth.X = (float)94.09;
                workCheckAth.Y = (float)333.18;
                workCheckAth.W = (float)626.36;
                workCheckAth.H = (float)150;

                //多附件.
                workCheckAth.UploadType = AttachmentUploadType.Multi;
                workCheckAth.Name = "审核组件";
                workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                workCheckAth.Insert();
            }
            return base.beforeUpdate();
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 是否启动自动运行？
        /// </summary>
        public bool AutoRunEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.AutoRunEnable);
            }
            set
            {
                this.SetValByKey(NodeAttr.AutoRunEnable, value);
            }
        }
        /// <summary>
        /// 自动运行参数
        /// </summary>
        public string AutoRunParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.AutoRunParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.AutoRunParas, value);
            }
        }
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
                    Log.DefaultLogWriteLineInfo(ex.Message + " - " + this.NodeID);
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
            set
            {
                this.SetValByKey(NodeAttr.AutoJumpRole1, value);
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
            set
            {
                this.SetValByKey(NodeAttr.AutoJumpRole2, value);
            }
        }
        /// <summary>
        /// 启动参数
        /// </summary>
        public string SubFlowStartParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.SubFlowStartParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubFlowStartParas, value);
            }
        }
        /// <summary>
        /// 子线程启动方式
        /// </summary>
        public SubFlowStartWay SubFlowStartWay
        {
            get
            {
                return (SubFlowStartWay)this.GetValIntByKey(NodeAttr.SubFlowStartWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubFlowStartWay, (int)value);
            }
        }
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


                if (this.HisFormType == NodeFormType.FreeForm)
                    return "自由表单";

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
        public string ICON
        {
            get
            {
                string s = this.GetValStrByKey(NodeAttr.ICON);
                if (DataType.IsNullOrEmpty(s))
                    if (this.IsStartNode)
                        return "审核.png";
                    else
                        return "前台.png";
                return s;
            }
            set
            {
                this.SetValByKey(NodeAttr.ICON, value);
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
                str = str.Replace("@SDKFromServHost",
                    BP.Sys.SystemConfig.AppSettings["SDKFromServHost"]);
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
                return this.GetValIntByKey(NodeAttr.TimeLimit);
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
        public int WarningDay
        {
            get
            {
                int i = this.GetValIntByKey(NodeAttr.WarningDay);
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
            set
            {
                this.SetValByKey(NodeAttr.WhoExeIt, value);
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
                string str = this.GetValStrByKey(NodeAttr.NodeFrmID);
                if (DataType.IsNullOrEmpty(str)==true)
                    return "ND" + this.NodeID;

                if (this.HisFormType == NodeFormType.FoolTruck 
                    || this.HisFormType == NodeFormType.SheetTree)
                    return "ND" + this.NodeID;

                if (str.Equals("Pri") == true &&
                    (this.HisFormType == NodeFormType.FoolForm || this.HisFormType == NodeFormType.FreeForm))
                {
                    if (this.WorkID == 0)
                        return "ND" + this.NodeID;
                    // throw new Exception("err@获得当前节点的上一个节点表单出现错误,没有给参数WorkID赋值.");

                    /* 要引擎上一个节点表单 */
                    string sql = "SELECT NDFrom FROM ND" + int.Parse(this.FK_Flow) + "Track A, WF_Node B ";
                    sql += " WHERE A.NDFrom=B.NodeID AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.Start + ")  ";
                    sql += "  AND (FormType=0 OR FormType=1) ";

                    //if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'+CONVERT(varchar(10),B.NodeID) ) ";

                    //if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID= CONCAT('ND',cast(B.NodeID as varchar(10)) ) ) ";

                    //if (SystemConfig.AppCenterDBType == DBType.Oracle)
                    //    sql += "  AND (B.NodeFrmID='' OR B.NodeFrmID IS NULL OR B.NodeFrmID='ND'||to_char(B.NodeID) ) ";


                    sql += "  AND (A.WorkID=" + this.WorkID + ") ";

                    sql += " ORDER BY A.RDT ";

                    int nodeID = DBAccess.RunSQLReturnValInt(sql, 0);
                    if (nodeID == 0)
                        throw new Exception("err@没有找到指定的节点.");

                    return "ND" + nodeID;
                }
                return str;
            }
            set
            {
                SetValByKey(NodeAttr.NodeFrmID, value);
            }
        }
        /// <summary>
        /// 要启动的子流程
        /// </summary>
        public string SFActiveFlows
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.SFActiveFlows);
            }
            set
            {
                SetValByKey(NodeAttr.SFActiveFlows, value);
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
        public PrintDocEnable HisPrintDocEnable
        {
            get
            {
                return (PrintDocEnable)this.GetValIntByKey(NodeAttr.PrintDocEnable);
            }
            set
            {
                this.SetValByKey(NodeAttr.PrintDocEnable, (int)value);
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
                string str = this.GetValStringByKey(NodeAttr.BatchParas);

                //替换约定的URL.
                str = str.Replace("@SDKFromServHost", BP.Sys.SystemConfig.AppSettings["SDKFromServHost"]);
                //if (str.Length <=3)
                //    str="Title,RDT"
                return str;
            }
            set
            {
                this.SetValByKey(NodeAttr.BatchParas, value);
            }
        }
        /// <summary>
        /// 是否是自定义的url,处理批处理.
        /// </summary>
        public bool BatchParas_IsSelfUrl
        {
            get
            {
                if (this.BatchParas.Contains(".aspx")
                    || this.BatchParas.Contains(".jsp")
                    || this.BatchParas.Contains(".htm")
                    || this.BatchParas.Contains("http:"))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 批量审核数量
        /// </summary>
        public int BatchListCount
        {
            get { return this.GetValIntByKey(NodeAttr.BatchListCount); }
            set { this.SetValByKey(NodeAttr.BatchListCount, value); }
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
        /// 要显示在后面的表单
        /// </summary>
        public string ShowSheets
        {
            get
            {
                string s = this.GetValStrByKey(NodeAttr.ShowSheets);
                if (s == "")
                    return "@";
                return s;
            }
            set
            {
                SetValByKey(NodeAttr.ShowSheets, value);
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

        #region 扩展属性
        /// <summary>
        /// 是不是多岗位工作节点.
        /// </summary>
        public bool IsMultiStations
        {
            get
            {
                if (this.NodeStations.Count > 1)
                    return true;
                return false;
            }
        }
        public string HisStationsStr
        {
            get
            {
                string s = "";
                foreach (NodeStation ns in this.NodeStations)
                {
                    s += ns.FK_StationT + ",";
                }
                return s;
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
        public CCRole HisCCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(NodeAttr.CCRole);
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
                this.SetValByKey(NodeAttr.WhenNoWorker,value);
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
                    case RunModel.SubThread:
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
                if (SystemConfig.IsDebug)
                {
                    this.SetValByKey(NodeAttr.NodePosType, (int)this.GetHisNodePosType());
                    return this.GetHisNodePosType();
                }
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
                if (this.HisNodePosType == NodePosType.End)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否允许子线程接受人员重复(对子线程点有效)?
        /// </summary>
        public bool IsAllowRepeatEmps
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsAllowRepeatEmps);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsAllowRepeatEmps, value);
            }
        }
        /// <summary>
        /// 是否启用发送短信？
        /// </summary>
        public bool IsEnableSMSMessage
        {
            get
            {
                int i = BP.DA.DBAccess.RunSQLReturnValInt("SELECT SMSEnable FROM Sys_FrmEvent WHERE FK_MapData='ND" + this.NodeID + "' AND FK_Event='SendSuccess'", 0);
                if (i == 0)
                    return false;
                return true;
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
                return this.GetValDecimalByKey(NodeAttr.PassRate);
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
        /// <summary>
        /// 是否是业务单元
        /// </summary>
        public bool IsBUnit
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsBUnit);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsBUnit, value);
            }
        }

        
      
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
        /// 是否有流程完成条件
        /// </summary>
        public bool IsCCFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCCFlow);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCCFlow, value);
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
            if (SystemConfig.IsDebug)
            {
                if (this.RetrieveFromDBSources() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            }
            else
            {
                // 去掉缓存.
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                {
                    string err = "err@NodeID=" + this.NodeID + "不存在";
                    err += "可能出现错误的原因如下:";
                    err += "1.你在FEE中或者SDK模式中使用了节点跳转,跳转到的节点已经不存在.";
                    throw new Exception(err);
                }

                //if (this.Retrieve() <= 0)
                //    throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            }
        }
        public Node(string ndName)
        {
            ndName = ndName.Replace("ND", "");
            this.NodeID = int.Parse(ndName);

            if (SystemConfig.IsDebug)
            {
                if (this.RetrieveFromDBSources() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + ndName);
            }
            else
            {
                if (this.Retrieve() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + ndName);
            }
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
                string str = this.GetValStringByKey(FrmWorkCheckAttr.FWCNodeName);
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
               return this.GetValIntByKey(FrmWorkCheckAttr.FWCOrderModel);
            }
        }
        public float FWC_H
        {
            get
            {
                return this.GetValFloatByKey(FrmWorkCheckAttr.FWC_H);
            }
        }
        public float FWC_W
        {
            get
            {
                return this.GetValFloatByKey(FrmWorkCheckAttr.FWC_W);
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

                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);

                #region 基本属性.
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 150, 10);
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, true, 0, 100, 10, false);

                map.AddTBInt(NodeAttr.Step, (int)NodeWorkType.Work, "流程步骤", true, false);

                map.AddTBString(NodeAttr.ICON, null, "节点ICON图片路径", true, false, 0, 70, 10);

                map.AddTBInt(NodeAttr.NodeWorkType, 0, "节点类型", false, false);
                map.AddTBInt(NodeAttr.SubThreadType, 0, "子线程ID", false, false);

                map.AddTBString(NodeAttr.FK_Flow, null, "FK_Flow", false, false, 0, 3, 10);
                map.AddTBInt(NodeAttr.IsGuestNode, 0, "是否是客户执行节点", false, false);

                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 200, 10);

                //map.AddTBString(NodeAttr.FK_FlowSort, null, "FK_FlowSort", false, true, 0, 4, 10);
                //map.AddTBString(NodeAttr.FK_FlowSortT, null, "FK_FlowSortT", false, true, 0, 100, 10);

                map.AddTBString(NodeAttr.FrmAttr, null, "FrmAttr", false, true, 0, 300, 10);
                map.AddTBInt(NodeAttr.IsBUnit, 0, "是否是节点模版(业务单元)", true, false);
                #endregion 基本属性.

                #region 审核组件.
                map.AddTBInt(NodeAttr.FWCSta, 0, "审核组件", false, false);
                map.AddTBFloat(NodeAttr.FWC_H, 0, "审核组件高度", false, true);
                map.AddTBInt(FrmWorkCheckAttr.FWCOrderModel, 0, "协作模式下操作员显示顺序", false, false);
                #endregion 审核组件.

                #region 考核属性.
                map.AddTBFloat(NodeAttr.TimeLimit, 2, "限期(天)", true, false); //"限期(天)".
              //  map.AddTBFloat(NodeAttr.TSpanHour, 0, "小时", true, false); //"限期(分钟)".
                map.AddTBInt(NodeAttr.TWay, 0, "时间计算方式", true, false); //0=不计算节假日,1=计算节假日.

                map.AddTBInt(NodeAttr.TAlertRole, 0, "逾期提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "逾期提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.WarningDay, 1, "工作预警(天)", true, false);    // "警告期限(0不警告)"
                //  map.AddTBFloat(NodeAttr.WarningHour, 0, "工作预警(小时)", true, false); // "警告期限(0不警告)"
                //  map.SetHelperUrl(NodeAttr.WarningHour, "http://ccbpm.mydoc.io/?v=5404&t=17999");

                map.AddTBInt(NodeAttr.WAlertRole, 0, "预警提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "预警提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "扣分(每延期1小时)", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "考核方式", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", true, true);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "超时处理方式", false, false);
                map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);

                #endregion 考核属性.

                map.AddTBString(FrmWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);
                map.AddTBString(NodeAttr.Doc, null, "描述", true, false, 0, 100, 10);
                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true);

                //退回相关.
                map.AddTBInt(NodeAttr.ReturnRole, 2, "退回规则", true, true);
                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 50, 10, true);
                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 50, 10, true);

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
                map.AddTBInt(NodeAttr.TeamLeaderConfirmRole, 0, "组长确认规则", true, true);
                map.AddTBString(NodeAttr.TeamLeaderConfirmDoc, null, "组长确认设置内容", true, false, 0, 100, 10);

                map.AddTBInt(NodeAttr.IsAllowRepeatEmps, 0, "是否允许子线程接受人员重复(对子线程点有效)?", true, true);
                map.AddTBInt(NodeAttr.IsBackTracking, 1, "是否可以在退回后原路返回(只有启用退回功能才有效)", true, true);
                map.AddTBInt(NodeAttr.IsRM, 1, "是否启用投递路径自动记忆功能?", true, true);
                map.AddBoolean(NodeAttr.IsHandOver, false, "是否可以移交", true, true);
                map.AddTBDecimal(NodeAttr.PassRate, 100, "通过率", true, true);
                map.AddTBInt(NodeAttr.RunModel, 0, "运行模式(对普通节点有效)", true, true);
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

                //父子流程.
                map.AddTBString(NodeAttr.SFActiveFlows, null, "启动的子流程", true, false, 0, 100, 10);

                // 批处理.
                map.AddTBInt(NodeAttr.BatchRole, 0, "批处理", true, true);
                map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, true);
                map.AddTBString(NodeAttr.BatchParas, null, "参数", true, false, 0, 500, 10);
                map.AddTBInt(NodeAttr.PrintDocEnable, 0, "打印方式", true, true);

                //与未来处理人有关系.
                //map.AddTBInt(NodeAttr.IsFullSA, 1, "是否计算未来处理人?", false, false);
                //map.AddTBInt(NodeAttr.IsFullSATime, 0, "是否计算未来接受与处理时间?", false, false);
                //map.AddTBInt(NodeAttr.IsFullSAAlert, 0, "是否接受未来工作到达消息提醒?", false, false);

                //表单相关.
                map.AddTBInt(NodeAttr.FormType, 1, "表单类型", false, false);
                map.AddTBString(NodeAttr.FormUrl, "http://", "表单URL", true, false, 0, 300, 10);
                map.AddTBString(NodeAttr.DeliveryParas, null, "接受人SQL", true, false, 0, 300, 10, true);
                map.AddTBInt(NodeAttr.TurnToDeal, 0, "转向处理", false, false);
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "发送后提示信息", true, false, 0, 200, 10, true);
                map.AddTBInt(NodeAttr.NodePosType, 0, "位置", false, false);
                map.AddTBInt(NodeAttr.IsCCFlow, 0, "是否有流程完成条件", false, false);
                map.AddTBString(NodeAttr.HisStas, null, "岗位", false, false, 0, 3000, 10);
                map.AddTBString(NodeAttr.HisDeptStrs, null, "部门", false, false, 0, 3000, 10);
                map.AddTBString(NodeAttr.HisToNDs, null, "转到的节点", false, false, 0, 50, 10);
                map.AddTBString(NodeAttr.HisBillIDs, null, "单据IDs", false, false, 0, 50, 10);
                //  map.AddTBString(NodeAttr.HisEmps, null, "HisEmps", false, false, 0, 3000, 10);
                map.AddTBString(NodeAttr.HisSubFlows, null, "HisSubFlows", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.PTable, null, "物理表", false, false, 0, 100, 10);

                map.AddTBString(NodeAttr.ShowSheets, null, "显示的表单", false, false, 0, 100, 10);
                map.AddTBString(NodeAttr.GroupStaNDs, null, "岗位分组节点", false, false, 0, 500, 10);
                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 100, 10, true);

                map.AddTBString(NodeAttr.RefOneFrmTreeType, "", "独立表单类型", false, false, 0, 100, 10);//RefOneFrmTree

                //按钮控制部分.
                // map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10, true);
                map.AddTBAtParas(500);

                // 启动子线程参数 2013-01-04
                map.AddTBInt(NodeAttr.SubFlowStartWay, 0, "子线程启动方式", true, false);
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10);

                //   map.AddTBString(NodeAttr.DocLeftWord, null, "公文左边词语(多个用@符合隔开)", true, false, 0, 100, 10);
                //  map.AddTBString(NodeAttr.DocRightWord, null, "公文右边词语(多个用@符合隔开)", true, false, 0, 200, 10);

                // 启动自动运行. 2013-01-04
                map.AddTBInt(NodeAttr.AutoRunEnable, 0, "是否启动自动运行？", true, false);
                map.AddTBString(NodeAttr.AutoRunParas, null, "自动运行参数", true, false, 0, 100, 10);

                #region 与参数有关系的属性。
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
        /// <summary>
        /// 我能处理当前的节点吗？
        /// </summary>
        /// <returns></returns>
        public bool CanIdoIt()
        {
            return false;
        }
        #endregion


        /// <summary>
        /// 删除前的逻辑处理.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            //判断是否可以被删除. 
            int num = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Node=" + this.NodeID);
            if (num != 0)
                throw new Exception("@该节点[" + this.NodeID + "," + this.Name + "]有待办工作存在，您不能删除它.");

            // 删除它的节点。
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            md.Delete();

            // 删除分组.
            BP.Sys.GroupFields gfs = new BP.Sys.GroupFields();
            gfs.Delete(BP.Sys.GroupFieldAttr.FrmID, md.No);

            //删除它的明细。
            BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(md.No);
            dtls.Delete();

            //删除框架
            BP.Sys.MapFrames frams = new BP.Sys.MapFrames(md.No);
            frams.Delete();
       
            // 删除扩展
            BP.Sys.MapExts exts = new BP.Sys.MapExts(md.No);
            exts.Delete();

            //删除节点与岗位的对应.
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_NodeStation WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_NodeEmp  WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_NodeDept WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_FrmNode  WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_CCEmp  WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_CH  WHERE FK_Node=" + this.NodeID);


            //删除附件.
            BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmAttachment  WHERE FK_MapData='" + this.NodeID + "'");
            return base.beforeDelete();
        }
        /// <summary>
        /// 文书流程
        /// </summary>
        /// <param name="md"></param>
        private void AddDocAttr(BP.Sys.MapData md)
        {
            /*如果是单据流程？ */
            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();

            //attr = new BP.Sys.MapAttr();
            //attr.FK_MapData = md.No;
            //attr.HisEditType = BP.En.EditType.UnDel;
            //attr.KeyOfEn = "Title";
            //attr.Name = "标题";
            //attr.MyDataType = BP.DA.DataType.AppString;
            //attr.UIContralType = UIContralType.TB;
            //attr.LGType = FieldTypeS.Normal;
            //attr.UIVisible = true;
            //attr.UIIsEnable = true;
            //attr.MinLen = 0;
            //attr.MaxLen = 300;
            //attr.Idx = 1;
            //attr.UIIsLine = true;
            //attr.Idx = -100;
            //attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "KeyWord";
            attr.Name = "主题词";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.UIIsLine = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = -99;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "FZ";
            attr.Name = "附注";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.UIIsLine = true;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.Idx = -98;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = "DW_SW";
            attr.Name = "收文单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.UIIsLine = true;
            attr.Idx = 1;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_FW";
            attr.Name = "发文单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_BS";
            attr.Name = "主报(送)单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_CS";
            attr.Name = "抄报(送)单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "NumPrint";
            attr.Name = "印制份数";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 10;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "JMCD";
            attr.Name = "机密程度";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.DDL;
            attr.LGType = FieldTypeS.Enum;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.UIBindKey = "JMCD";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = "PRI";
            attr.Name = "紧急程度";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.DDL;
            attr.LGType = FieldTypeS.Enum;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.UIIsLine = false;
            attr.UIBindKey = "PRI";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "GWWH";
            attr.Name = "公文文号";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.Idx = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.Insert();
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

            if (attrs.Contains(MapAttrAttr.KeyOfEn, "FID", MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "FID";
                attr.Name = "FID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.RDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.RDT;
                attr.Name = "接受时间";  //"接受时间";
                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.CDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.CDT;
                if (this.IsStartNode)
                    attr.Name = "发起时间"; //"发起时间";
                else
                    attr.Name = "完成时间"; //"完成时间";

                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.Rec, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.Rec;
                if (this.IsStartNode == false)
                    attr.Name = "记录人"; // "记录人";
                else
                    attr.Name = "发起人"; //"发起人";

                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MaxLen = 32;
                attr.MinLen = 0;
                attr.DefVal = "@WebUser.No";
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.Emps, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.Emps;
                attr.Name = WorkAttr.Emps;
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MaxLen = 8000;
                attr.MinLen = 0;
                attr.Insert();
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, StartWorkAttr.FK_Dept, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = StartWorkAttr.FK_Dept;
                attr.Name = "操作员部门"; //"操作员部门";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;
                attr.UIBindKey = "BP.Port.Depts";
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.Insert();
            }


            if (fl.IsMD5  && attrs.Contains(MapAttrAttr.KeyOfEn, WorkAttr.MD5, MapAttrAttr.FK_MapData, md.No) == false)
            {
                /* 如果是MD5加密流程. */
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = StartWorkAttr.MD5;
                attr.UIBindKey = attr.KeyOfEn;
                attr.Name = "MD5";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.UIVisible = false;
                attr.MinLen = 0;
                attr.MaxLen = 40;
                attr.Idx = -100;
                attr.Insert();
            }

            if (this.NodePosType == NodePosType.Start)
            {

                if (attrs.Contains(MapAttrAttr.KeyOfEn, StartWorkAttr.Title, MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = StartWorkAttr.Title;
                    attr.Name = "标题"; // "流程标题";
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = true;
                    attr.UIWidth = 251;

                    attr.MinLen = 0;
                    attr.MaxLen = 200;
                    attr.Idx = -100;
                    attr.X = (float)171.2;
                    attr.Y = (float)68.4;
                    attr.Insert();
                }

                if (attrs.Contains(MapAttrAttr.KeyOfEn, "FK_NY", MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = "FK_NY";
                    attr.Name = "年月"; //"年月";
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.LGType = FieldTypeS.Normal;
                    //attr.UIBindKey = "BP.Pub.NYs";
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.MinLen = 0;
                    attr.MaxLen = 7;
                    attr.Insert();
                }

                if (attrs.Contains(MapAttrAttr.KeyOfEn, "MyNum", MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = "MyNum";
                    attr.Name = "个数"; // "个数";
                    attr.DefVal = "1";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
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
            if (this.HisFlow.HisDataStoreModel == DataStoreModel.SpecTable)
                md.PTable = this.HisFlow.PTable;
            md.Insert();

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
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

            if (this.HisFlow.FlowAppType == FlowAppType.DocFlow)
            {
                this.AddDocAttr(md);
            }

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "FID";
            attr.Name = "FID";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.DefVal = "0";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.RDT;
            attr.Name = "接受时间";  //"接受时间";
            attr.MyDataType = BP.DA.DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.Tag = "1";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.CDT;
            if (this.IsStartNode)
                attr.Name = "发起时间"; //"发起时间";
            else
                attr.Name = "完成时间"; //"完成时间";

            attr.MyDataType = BP.DA.DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.DefVal = "@RDT";
            attr.Tag = "1";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.Rec;
            if (this.IsStartNode == false)
                attr.Name = "记录人"; // "记录人";
            else
                attr.Name = "发起人"; //"发起人";

            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MaxLen = 32;
            attr.MinLen = 0;
            attr.DefVal = "@WebUser.No";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.Emps;
            attr.Name = "Emps";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MaxLen = 8000;
            attr.MinLen = 0;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = StartWorkAttr.FK_Dept;
            attr.Name = "操作员部门"; //"操作员部门";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MinLen = 0;
            attr.MaxLen = 50;
            attr.Insert();

            if (this.NodePosType == NodePosType.Start)
            {
                //开始节点信息.
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.Edit;
                //   attr.edit
                attr.KeyOfEn = "Title";
                attr.Name = "标题"; // "流程标题";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.UIWidth = 251;
                attr.MinLen = 0;
                attr.MaxLen = 200;
                attr.Idx = -100;
                attr.X = (float)174.83;
                attr.Y = (float)54.4;
                attr.Insert();


                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "FK_NY";
                attr.Name = "年月"; //"年月";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 7;
                attr.Insert();

                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "个数"; // "个数";
                attr.DefVal = "1";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
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

        #region 查询方法
        /// <summary>
        /// RetrieveAll
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            Nodes nds = Cash.GetObj(this.ToString(), Depositary.Application) as Nodes;
            if (nds == null)
            {
                nds = new Nodes();
                QueryObject qo = new QueryObject(nds);
                qo.AddWhereInSQL(NodeAttr.NodeID, " SELECT Node FROM WF_Direction ");
                qo.addOr();
                qo.AddWhereInSQL(NodeAttr.NodeID, " SELECT ToNode FROM WF_Direction ");
                qo.DoQuery();

                Cash.AddObj(this.ToString(), Depositary.Application, nds);
                Cash.AddObj(this.GetNewEntity.ToString(), Depositary.Application, nds);
            }

            this.Clear();
            this.AddEntities(nds);
            return this.Count;
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
