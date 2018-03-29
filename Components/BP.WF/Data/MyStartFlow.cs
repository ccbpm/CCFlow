using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.En;

namespace BP.WF.Data
{
    /// <summary>
    /// 我发起的流程
    /// </summary>
    public class MyStartFlowAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 工作流
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 流程状态(简单)
        /// </summary>
        public const string WFSta = "WFSta";
        /// <summary>
        /// TSpan
        /// </summary>
        public const string TSpan = "TSpan";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string Starter = "Starter";
        /// <summary>
        /// 产生时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 完成时间
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 得分
        /// </summary>
        public const string Cent = "Cent";
        /// <summary>
        /// 当前工作到的节点.
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 当前工作岗位
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 发起人名称
        /// </summary>
        public const string StarterName = "StarterName";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public const string SDTOfFlow = "SDTOfFlow";
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public const string SDTOfNode = "SDTOfNode";
        /// <summary>
        /// 父流程ID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 父流程节点
        /// </summary>
        public const string PNodeID = "PNodeID";
        /// <summary>
        /// 子流程的调用人.
        /// </summary>
        public const string PEmp = "PEmp";
        /// <summary>
        /// 客户编号(对于客户发起的流程有效)
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 客户名称
        /// </summary>
        public const string GuestName = "GuestName";
        /// <summary>
        /// 单据编号
        /// </summary>
        public const string BillNo = "BillNo";
        /// <summary>
        /// 备注
        /// </summary>
        public const string FlowNote = "FlowNote";
        /// <summary>
        /// 待办人员
        /// </summary>
        public const string TodoEmps = "TodoEmps";
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public const string TodoEmpsNum = "TodoEmpsNum";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// 临时存放的参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 参与人
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        #endregion
    }
    /// <summary>
    /// 我发起的流程
    /// </summary>
    public class MyStartFlow : Entity
    {
        #region 基本属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                uac.IsExp =  UserRegedit.HaveRoleForExp(this.ToString());
                return uac;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return MyStartFlowAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.FlowNote);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.FK_Flow);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.BillNo);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.FlowName);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(MyStartFlowAttr.PRI);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(MyStartFlowAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.TodoEmps);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.Emps);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(MyStartFlowAttr.TaskSta);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.FK_Dept);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.Title);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.Title, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.GuestNo);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.GuestName);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 产生时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.RDT);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.RDT, value);
            }
        }
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.SDTOfFlow, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyStartFlowAttr.WorkID);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(MyStartFlowAttr.FID);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyStartFlowAttr.PWorkID);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(MyStartFlowAttr.PNodeID);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.PFlowNo);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.PEmp);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.Starter);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.StarterName);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.DeptName);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.NodeName);
            }
            set
            {
                this.SetValByKey(MyStartFlowAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MyStartFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(MyStartFlowAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(MyStartFlowAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(MyStartFlowAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(MyStartFlowAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(MyStartFlowAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(MyStartFlowAttr.WFSta);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.WFSta, (int)value);
            }
        }
        public string WFStateText
        {
            get
            {
                BP.WF.WFState ws = (WFState)this.WFState;
                switch (ws)
                {
                    case WF.WFState.Complete:
                        return "已完成";
                    case WF.WFState.Runing:
                        return "在运行";
                    case WF.WFState.HungUp:
                        return "挂起";
                    case WF.WFState.Askfor:
                        return "加签";
                    default:
                        return "未判断";
                }
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(MyStartFlowAttr.GUID);
            }
            set
            {
                SetValByKey(MyStartFlowAttr.GUID, value);
            }
        }
        #endregion

        #region 参数属性.
        public string Paras_ToNodes
        {

            get
            {
                return this.GetParaString("ToNodes");
            }

            set
            {
                this.SetPara("ToNodes", value);
            }
        }
        /// <summary>
        /// 加签信息
        /// </summary>
        public string Paras_AskForReply
        {

            get
            {
                return this.GetParaString("AskForReply");
            }

            set
            {
                this.SetPara("AskForReply", value);
            }
        }
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
        /// 产生的工作流程
        /// </summary>
        public MyStartFlow()
        {
        }
        public MyStartFlow(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MyStartFlowAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 MyStartFlow [" + workId + "]不存在。");
        }
        /// <summary>
        /// 执行修复
        /// </summary>
        public void DoRepair()
        {
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

                Map map = new Map("WF_GenerWorkFlow", "我发起的流程");

                map.Java_SetEnType(EnType.View);

                map.AddTBIntPK(MyStartFlowAttr.WorkID, 0, "WorkID", false, false);
                map.AddTBString(MyStartFlowAttr.Title, null, "标题", true, false, 0, 100, 200, true);

                map.AddDDLEntities(MyStartFlowAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddTBString(MyStartFlowAttr.BillNo, null, "单据编号", true, true, 0, 100, 50);
                map.AddTBInt(MyStartFlowAttr.FK_Node, 0, "节点编号", false, false);

                map.AddDDLSysEnum(MyStartFlowAttr.WFSta, 0, "状态", true, true, MyStartFlowAttr.WFSta, "@0=运行中@1=已完成@2=其他");
                map.AddTBString(MyStartFlowAttr.Starter, null, "发起人", false, false, 0, 100, 100);
                map.AddTBDateTime(MyStartFlowAttr.RDT, "发起日期", true, true);

                map.AddTBString(MyStartFlowAttr.NodeName, null, "停留节点", true, true, 0, 100, 100, false);
                map.AddTBString(MyStartFlowAttr.TodoEmps, null, "当前处理人", true, false, 0, 100, 100, false);
                map.AddTBStringDoc(MyFlowAttr.FlowNote, null, "备注", true, false, true);


                map.AddTBString(MyFlowAttr.Emps, null, "参与人", false, false, 0, 4000, 100, true);
                map.AddDDLSysEnum(MyFlowAttr.TSpan, 0, "时间段", true, false, MyFlowAttr.TSpan, "@0=本周@1=上周@2=两周以前@3=三周以前@4=更早");

                map.AddTBMyNum();

                //隐藏字段.
                map.AddTBInt(MyStartFlowAttr.WFState, 0, "状态", false, false);
                map.AddTBInt(MyStartFlowAttr.FID, 0, "FID", false, false);
                map.AddTBInt(MyFlowAttr.PWorkID, 0, "PWorkID", false, false);

              //  map.AddSearchAttr(MyStartFlowAttr.FK_Flow);
                map.AddSearchAttr(MyStartFlowAttr.WFSta);
                map.AddSearchAttr(MyStartFlowAttr.TSpan);

                //我发起的流程.
                AttrOfSearch search = new AttrOfSearch(MyStartFlowAttr.Starter, "发起人",
                    MyStartFlowAttr.Starter, "=", BP.Web.WebUser.No, 0, true);

                map.AttrsOfSearch.Add(search);

                search = new AttrOfSearch(MyStartFlowAttr.WFState, "流程状态",
                    MyStartFlowAttr.WFState, "not in", "('0')", 0, true);
                map.AttrsOfSearch.Add(search);

                RefMethod rm = new RefMethod();
                rm.Title = "轨迹";
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "../../WF/Img/Track.png";
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenLastForm";
                rm.Icon = "../../WF/Img/Form.png";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行诊断
        public string DoTrack()
        {
            //PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow, 900, 800);
            return "/WF/WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow+"&FK_Node="+this.FK_Node;
        }
        /// <summary>
        /// 打开最后一个节点表单
        /// </summary>
        /// <returns></returns>
        public string DoOpenLastForm()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT MYPK FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID ORDER BY RDT DESC";
            pss.Add("ActionType", (int)BP.WF.ActionType.Forward);
            pss.Add("WorkID", this.WorkID);
            DataTable dt = DBAccess.RunSQLReturnTable(pss);
            if (dt != null && dt.Rows.Count > 0)
            {
                string myPk = dt.Rows[0][0].ToString();
                return "/WF/WFRpt.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&DoType=View&MyPK=" + myPk + "&PWorkID=" + this.PWorkID;
            }

            Node nd = new Node(this.FK_Node);
            return "/WF/CCForm/FrmGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + nd.NodeFrmID + "&ReadOnly=1&IsEdit=0";
        }
        #endregion
    }
    /// <summary>
    /// 我发起的流程s
    /// </summary>
    public class MyStartFlows : Entities
    {
        /// <summary>
        /// 根据工作流程,工作人员 ID 查询出来他当前的能做的工作.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="empId">工作人员ID</param>
        /// <returns></returns>
        public static DataTable QuByFlowAndEmp(string flowNo, int empId)
        {
            string sql = "SELECT a.WorkID FROM WF_MyStartFlow a, WF_GenerWorkerlist b WHERE a.WorkID=b.WorkID   AND b.FK_Node=a.FK_Node  AND b.FK_Emp='" + empId.ToString() + "' AND a.FK_Flow='" + flowNo + "'";
            return DBAccess.RunSQLReturnTable(sql);
        }

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MyStartFlow();
            }
        }
        /// <summary>
        /// 我发起的流程集合
        /// </summary>
        public MyStartFlows() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyStartFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyStartFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyStartFlow> Tolist()
        {
            System.Collections.Generic.List<MyStartFlow> list = new System.Collections.Generic.List<MyStartFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyStartFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
