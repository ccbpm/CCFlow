using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port ;
using BP.Sys;
using BP.En;

namespace BP.WF.Data
{
	/// <summary>
    /// 我参与的流程
	/// </summary>
    public class MyFlowAttr
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
        /// 时间段
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
    /// 我参与的流程
	/// </summary>
	public class MyJoinFlow : Entity
	{	
		#region 基本属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
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
                return MyFlowAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.FlowNote);
            }
            set
            {
                SetValByKey(MyFlowAttr.FlowNote, value);
            }
        }
		/// <summary>
		/// 工作流程编号
		/// </summary>
		public string  FK_Flow
		{
			get
			{
				return this.GetValStrByKey(MyFlowAttr.FK_Flow);
			}
			set
			{
				SetValByKey(MyFlowAttr.FK_Flow,value);
			}
		}
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.BillNo);
            }
            set
            {
                SetValByKey(MyFlowAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.FlowName);
            }
            set
            {
                SetValByKey(MyFlowAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(MyFlowAttr.PRI);
            }
            set
            {
                SetValByKey(MyFlowAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(MyFlowAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(MyFlowAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.TodoEmps);
            }
            set
            {
                SetValByKey(MyFlowAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.Emps);
            }
            set
            {
                SetValByKey(MyFlowAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(MyFlowAttr.TaskSta);
            }
            set
            {
                SetValByKey(MyFlowAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(MyFlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
		public string  FK_Dept
		{
			get
			{
				return this.GetValStrByKey(MyFlowAttr.FK_Dept);
			}
			set
			{
				SetValByKey(MyFlowAttr.FK_Dept,value);
			}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string  Title
		{
			get
			{
				return this.GetValStrByKey(MyFlowAttr.Title);
			}
			set
			{
				SetValByKey(MyFlowAttr.Title,value);
			}
		}
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.GuestNo);
            }
            set
            {
                SetValByKey(MyFlowAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.GuestName);
            }
            set
            {
                SetValByKey(MyFlowAttr.GuestName, value);
            }
        }
		/// <summary>
		/// 产生时间
		/// </summary>
		public string  RDT
		{
			get
			{
				return this.GetValStrByKey(MyFlowAttr.RDT);
			}
			set
			{
				SetValByKey(MyFlowAttr.RDT,value);
			}
		}
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(MyFlowAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(MyFlowAttr.SDTOfFlow, value);
            }
        }
		/// <summary>
		/// 流程ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
                return this.GetValInt64ByKey(MyFlowAttr.WorkID);
			}
			set
			{
				SetValByKey(MyFlowAttr.WorkID,value);
			}
		}
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(MyFlowAttr.FID);
            }
            set
            {
                SetValByKey(MyFlowAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyFlowAttr.PWorkID);
            }
            set
            {
                SetValByKey(MyFlowAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(MyFlowAttr.PNodeID);
            }
            set
            {
                SetValByKey(MyFlowAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.PFlowNo);
            }
            set
            {
                SetValByKey(MyFlowAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.PEmp);
            }
            set
            {
                SetValByKey(MyFlowAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.Starter);
            }
            set
            {
                SetValByKey(MyFlowAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.StarterName);
            }
            set
            {
                this.SetValByKey(MyFlowAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.DeptName);
            }
            set
            {
                this.SetValByKey(MyFlowAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(MyFlowAttr.NodeName);
            }
            set
            {
                this.SetValByKey(MyFlowAttr.NodeName, value);
            }
        }
		/// <summary>
		/// 当前工作到的节点
		/// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MyFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(MyFlowAttr.FK_Node, value);
            }
        }
        /// <summary>
		/// 工作流程状态
		/// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(MyFlowAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(MyFlowAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(MyFlowAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(MyFlowAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(MyFlowAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(MyFlowAttr.WFSta);
            }
            set
            {
                SetValByKey(MyFlowAttr.WFSta, (int)value);
            }
        }
        public string WFStateText
        {
            get
            {
                BP.WF.WFState ws = (WFState)this.WFState;
                switch(ws)
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
                return this.GetValStrByKey(MyFlowAttr.GUID);
            }
            set
            {
                SetValByKey(MyFlowAttr.GUID, value);
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
		public MyJoinFlow()
		{
		}
        public MyJoinFlow(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MyFlowAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 MyFlow [" + workId + "]不存在。");
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

                Map map = new Map("WF_GenerWorkFlow", "我审批的流程");

                map.Java_SetEnType(EnType.View);

                map.AddTBIntPK(MyFlowAttr.WorkID, 0, "WorkID", false, false);
                map.AddTBInt(MyFlowAttr.FID, 0, "FID", false, false);
                map.AddTBInt(MyFlowAttr.PWorkID, 0, "PWorkID", false, false);
                map.AddTBString(MyFlowAttr.Title, null, "流程标题", true, false, 0, 100, 150, true);
                map.AddDDLEntities(MyFlowAttr.FK_Flow, null, "流程名称", new Flows(), false);
                map.AddTBString(MyFlowAttr.BillNo, null, "单据编号", true, false, 0, 100, 50);
                map.AddTBString(MyFlowAttr.StarterName, null, "发起人", true, false, 0, 30, 40);

                //map.AddDDLEntities(MyFlowAttr.FK_Dept, null, "发起人部门", new BP.Port.Depts(), false);
                //map.AddTBString(MyFlowAttr.Starter, null, "发起人编号", true, false, 0, 30, 10);
                //map.AddTBString(MyFlowAttr.StarterName, null, "发起人名称", true, false, 0, 30, 10);
                //map.AddTBString(MyFlowAttr.BillNo, null, "单据编号", true, false, 0, 100, 10);

                map.AddTBDateTime(MyFlowAttr.RDT, "发起日期", true, true);
                map.AddDDLSysEnum(MyFlowAttr.WFSta, 0, "状态", true, false, MyFlowAttr.WFSta, "@0=运行中@1=已完成@2=其他");
                map.AddDDLSysEnum(MyFlowAttr.TSpan, 0, "时间段", true, false, MyFlowAttr.TSpan, "@0=本周@1=上周@2=两周以前@3=三周以前@4=更早");
                map.AddTBString(MyFlowAttr.NodeName, null, "当前节点", true, false, 0, 100, 100, true);
                map.AddTBString(MyStartFlowAttr.TodoEmps, null, "当前处理人", true, false, 0, 100, 100, true);

                map.AddTBString(MyFlowAttr.Emps, null, "参与人", false, false, 0, 4000, 10, true);
                map.AddTBStringDoc(MyFlowAttr.FlowNote, null, "备注", true, false, true);

                //隐藏字段.
                map.AddTBInt(MyFlowAttr.FK_Node, 0, "FK_Node", false, false);

                map.AddTBMyNum();

             //   map.AddSearchAttr(MyFlowAttr.FK_Flow);
                map.AddSearchAttr(MyFlowAttr.WFSta);
                map.AddSearchAttr(MyFlowAttr.TSpan);

                //增加隐藏的查询条件. 我参与的流程.
                AttrOfSearch search = new AttrOfSearch(MyFlowAttr.Emps, "人员",
                    MyFlowAttr.Emps, " LIKE ", "%" + BP.Web.WebUser.No + "%", 0, true);
                map.AttrsOfSearch.Add(search);

                RefMethod rm = new RefMethod();
                rm.Title = "流程轨迹";
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "打开表单";
                //rm.ClassMethodName = this.ToString() + ".DoOpenLastForm";
                //rm.Icon = "../../WF/Img/FileType/doc.gif";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

		#region 执行诊断
        public string DoTrack()
        {
            //PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow, 900, 800);
            return "../../WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow+"&FK_Node="+this.FK_Node;
        }
		#endregion
	}
	/// <summary>
    /// 我参与的流程s
	/// </summary>
	public class MyJoinFlows : Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{			 
				return new MyJoinFlow();
			}
		}
		/// <summary>
		/// 我参与的流程集合
		/// </summary>
		public MyJoinFlows(){}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyJoinFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyJoinFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyJoinFlow> Tolist()
        {
            System.Collections.Generic.List<MyJoinFlow> list = new System.Collections.Generic.List<MyJoinFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyJoinFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
