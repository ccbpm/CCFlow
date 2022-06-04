using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.WF.Data
{
    /// <summary>
    /// 我授权的流程
    /// </summary>
    public class MyAuthtoAttr
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
    /// 我授权的流程
    /// </summary>
    public class MyAuthto : Entity
    {
        #region 基本属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                uac.IsExp = UserRegedit.HaveRoleForExp(this.ToString());
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
                return MyAuthtoAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.FlowNote);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 工作流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.FK_Flow);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.BillNo);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.FlowName);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(MyAuthtoAttr.PRI);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(MyAuthtoAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.TodoEmps);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.Emps);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(MyAuthtoAttr.TaskSta);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.FK_Dept);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.Title);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.Title, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.GuestNo);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.GuestName);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 产生时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.RDT);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.RDT, value);
            }
        }
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.SDTOfFlow, value);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyAuthtoAttr.WorkID);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(MyAuthtoAttr.FID);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyAuthtoAttr.PWorkID);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(MyAuthtoAttr.PNodeID);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.PFlowNo);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.PEmp);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.Starter);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.StarterName);
            }
            set
            {
                this.SetValByKey(MyAuthtoAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.DeptName);
            }
            set
            {
                this.SetValByKey(MyAuthtoAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(MyAuthtoAttr.NodeName);
            }
            set
            {
                this.SetValByKey(MyAuthtoAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 当前工作到的节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MyAuthtoAttr.FK_Node);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(MyAuthtoAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(MyAuthtoAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(MyAuthtoAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(MyAuthtoAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(MyAuthtoAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(MyAuthtoAttr.WFSta);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.WFSta, (int)value);
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
                    case WF.WFState.Hungup:
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
                return this.GetValStrByKey(MyAuthtoAttr.GUID);
            }
            set
            {
                SetValByKey(MyAuthtoAttr.GUID, value);
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
        /// 我授权的流程
        /// </summary>
        public MyAuthto()
        {
        }
        /// <summary>
        /// 我授权的流程
        /// </summary>
        /// <param name="workId">工作ID</param>
        public MyAuthto(Int64 workId)
        {
            this.WorkID = workId;
            this.Retrieve();
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

                Map map = new Map("WF_GenerWorkFlow", "我授权的流程");
                map.setEnType(EnType.View);

                map.AddTBIntPK(MyAuthtoAttr.WorkID, 0, "WorkID", false, false);
                map.AddTBString(MyAuthtoAttr.Title, null, "标题", true, false, 0, 300, 200, true);

                map.AddDDLEntities(MyAuthtoAttr.FK_Flow, null, "流程", new Flows(), false);

                map.AddTBString(MyAuthtoAttr.BillNo, null, "单据编号", true, true, 0, 100, 50);
                map.AddTBInt(MyAuthtoAttr.FK_Node, 0, "节点编号", false, false);

                map.AddDDLSysEnum(MyAuthtoAttr.WFSta, 0, "状态", true, true, MyAuthtoAttr.WFSta, "@0=运行中@1=已完成@2=其他");
                map.AddTBString(MyAuthtoAttr.Starter, null, "发起人", false, false, 0, 100, 100);
                map.AddTBDate(MyAuthtoAttr.RDT, "发起日期", true, true);

                map.AddTBString(MyAuthtoAttr.NodeName, null, "停留节点", true, true, 0, 100, 100, false);
                map.AddTBString(MyAuthtoAttr.TodoEmps, null, "当前处理人", true, false, 0, 100, 100, false);
                map.AddTBString(MyFlowAttr.Emps, null, "参与人", false, false, 0, 4000, 100, true);
            //    map.AddDDLSysEnum(MyFlowAttr.TSpan, 0, "时间段", true, false, MyFlowAttr.TSpan, "@0=本周@1=上周@2=两周以前@3=三周以前@4=更早");

                //隐藏字段.
                map.AddTBInt(MyAuthtoAttr.WFState, 0, "状态", false, false);
                map.AddTBInt(MyAuthtoAttr.FID, 0, "FID", false, false);
                map.AddTBInt(MyFlowAttr.PWorkID, 0, "PWorkID", false, false);
                map.AddTBString(MyFlowAttr.AtPara, null, "AtPara", false, false, 0, 4000, 100, false);

                map.AddSearchAttr(MyAuthtoAttr.WFSta);

                map.DTSearchWay = DTSearchWay.ByDate;
                map.DTSearchLable = "发起日期";
                map.DTSearchKey = MyAuthtoAttr.RDT;

                #region 增加多个隐藏条件.
                ////我授权的流程.
                //AttrOfSearch search = new AttrOfSearch(MyAuthtoAttr.Starter, "发起人",
                //    MyAuthtoAttr.Starter, "=", BP.Web.WebUser.No, 0, true);
                //map.AttrsOfSearch.Add(search);

                //search = new AttrOfSearch(MyAuthtoAttr.WFState, "流程状态",
                //    MyAuthtoAttr.WFState, " not in", "('0')", 0, true);
                //map.AttrsOfSearch.Add(search);

                AttrOfSearch search = new AttrOfSearch(MyAuthtoAttr.AtPara, "授权人",
                MyAuthtoAttr.AtPara, " LIKE ", " '%@Auth="+BP.Web.WebUser.Name+"%' ", 0, true);
                map.AttrsOfSearch.Add(search);

                #endregion 增加多个隐藏条件.


                RefMethod rm = new RefMethod();
                rm.Title = "轨迹";
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "../../WF/Img/Track.png";
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单";
                rm.ClassMethodName = this.ToString() + ".DoForm";
                rm.Icon = "../../WF/Img/Form.png";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.IsForEns = true;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "打印表单";
                //rm.ClassMethodName = this.ToString() + ".DoPrintFrm";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //rm.IsForEns = false;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行诊断
        public string DoPrintFrm()
        {
            return "../../WorkOpt/Packup.htm?FileType=zip,pdf&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&NodeID=" + this.FK_Node + "&FK_Node=" + this.FK_Node;
        }
        public string DoTrack()
        {
            return "../../WFRpt.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node;
        }
        /// <summary>
        /// 打开表单
        /// </summary>
        /// <returns></returns>
        public string DoForm()
        {
            return "../MyViewGener.htm?HttpHandlerName=BP.WF.HttpHandler.WF_MyView&WorkID="+this.WorkID+"&NodeID="+this.FK_Node+"&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&UserNo="+BP.Web.WebUser.No+"&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 打开最后一个节点表单
        /// </summary>
        /// <returns></returns>
        public string DoOpenLastForm()
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT MYPK FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ActionType=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ActionType AND WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID ORDER BY RDT DESC";
            pss.Add("ActionType", (int)BP.WF.ActionType.Forward);
            pss.Add("WorkID", this.WorkID);
            DataTable dt = DBAccess.RunSQLReturnTable(pss);
            if (dt != null && dt.Rows.Count > 0)
            {
                string myPk = dt.Rows[0][0].ToString();
                return "/WF/WFRpt.htm?CurrTab=Frm&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&DoType=View&MyPK=" + myPk + "&PWorkID=" + this.PWorkID;
            }

            Node nd = new Node(this.FK_Node);
            nd.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

            return "/WF/CCForm/FrmGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + nd.NodeFrmID + "&ReadOnly=1&IsEdit=0";
        }
        #endregion
    }
    /// <summary>
    /// 我授权的流程s
    /// </summary>
    public class MyAuthtos : Entities
    {
        #region 方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MyAuthto();
            }
        }
        /// <summary>
        /// 我授权的流程集合
        /// </summary>
        public MyAuthtos() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyAuthto> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyAuthto>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyAuthto> Tolist()
        {
            System.Collections.Generic.List<MyAuthto> list = new System.Collections.Generic.List<MyAuthto>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyAuthto)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
