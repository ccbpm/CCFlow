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
    /// 我部门的流程
	/// </summary>
    public class MyDeptFlowAttr
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
        /// 时间段
        /// </summary>
        public const string TSpan = "TSpan";
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
    /// 我部门的流程
	/// </summary>
	public class MyDeptFlow : Entity
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
                return MyDeptFlowAttr.WorkID;
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.FlowNote);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.FlowNote, value);
            }
        }
		/// <summary>
		/// 工作流程编号
		/// </summary>
		public string  FK_Flow
		{
			get
			{
				return this.GetValStrByKey(MyDeptFlowAttr.FK_Flow);
			}
			set
			{
				SetValByKey(MyDeptFlowAttr.FK_Flow,value);
			}
		}
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.BillNo);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.FlowName);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(MyDeptFlowAttr.PRI);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(MyDeptFlowAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.TodoEmps);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.Emps);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(MyDeptFlowAttr.TaskSta);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.FK_FlowSort);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
		public string  FK_Dept
		{
			get
			{
				return this.GetValStrByKey(MyDeptFlowAttr.FK_Dept);
			}
			set
			{
				SetValByKey(MyDeptFlowAttr.FK_Dept,value);
			}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string  Title
		{
			get
			{
				return this.GetValStrByKey(MyDeptFlowAttr.Title);
			}
			set
			{
				SetValByKey(MyDeptFlowAttr.Title,value);
			}
		}
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.GuestNo);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.GuestName);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.GuestName, value);
            }
        }
		/// <summary>
		/// 产生时间
		/// </summary>
		public string  RDT
		{
			get
			{
				return this.GetValStrByKey(MyDeptFlowAttr.RDT);
			}
			set
			{
				SetValByKey(MyDeptFlowAttr.RDT,value);
			}
		}
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.SDTOfFlow, value);
            }
        }
		/// <summary>
		/// 流程ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
                return this.GetValInt64ByKey(MyDeptFlowAttr.WorkID);
			}
			set
			{
				SetValByKey(MyDeptFlowAttr.WorkID,value);
			}
		}
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(MyDeptFlowAttr.FID);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyDeptFlowAttr.PWorkID);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(MyDeptFlowAttr.PNodeID);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.PFlowNo);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.PEmp);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.Starter);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.StarterName);
            }
            set
            {
                this.SetValByKey(MyDeptFlowAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.DeptName);
            }
            set
            {
                this.SetValByKey(MyDeptFlowAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(MyDeptFlowAttr.NodeName);
            }
            set
            {
                this.SetValByKey(MyDeptFlowAttr.NodeName, value);
            }
        }
		/// <summary>
		/// 当前工作到的节点
		/// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MyDeptFlowAttr.FK_Node);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.FK_Node, value);
            }
        }
        /// <summary>
		/// 工作流程状态
		/// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(MyDeptFlowAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(MyDeptFlowAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(MyDeptFlowAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(MyDeptFlowAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(MyDeptFlowAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(MyDeptFlowAttr.WFSta);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.WFSta, (int)value);
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
                return this.GetValStrByKey(MyDeptFlowAttr.GUID);
            }
            set
            {
                SetValByKey(MyDeptFlowAttr.GUID, value);
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
		public MyDeptFlow()
		{
		}
        public MyDeptFlow(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MyDeptFlowAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 MyDeptFlow [" + workId + "]不存在。");
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

                Map map = new Map("WF_GenerWorkFlow", "我部门的流程");
                map.Java_SetEnType(EnType.View);

                map.AddTBString(MyDeptFlowAttr.Title, null, "标题", true, false, 0, 100, 150, true);
                map.AddDDLEntities(MyDeptFlowAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddTBString(MyDeptFlowAttr.BillNo, null, "单据编号", true, false, 0, 100, 50);

                map.AddTBString(MyDeptFlowAttr.StarterName, null, "发起人", true, false, 0, 30, 40);
                map.AddTBDateTime(MyDeptFlowAttr.RDT, "发起日期", true, true);

                map.AddTBString(MyDeptFlowAttr.NodeName, null, "当前节点", true, false, 0, 100, 80);
                map.AddTBString(MyDeptFlowAttr.TodoEmps, null, "当前处理人", true, false, 0, 100, 80);

                map.AddDDLSysEnum(MyDeptFlowAttr.WFSta, 0, "状态", true, false, MyDeptFlowAttr.WFSta);
                map.AddDDLSysEnum(MyFlowAttr.TSpan, 0, "时间段", true, false, MyFlowAttr.TSpan, "@0=本周@1=上周@2=两周以前@3=三周以前@4=更早");

                map.AddTBStringDoc(MyDeptFlowAttr.FlowNote, null, "备注", true, false,true);
                map.AddTBMyNum();

                //工作ID
                map.AddTBIntPK(MyDeptFlowAttr.WorkID, 0, "工作ID", true, true);

                //隐藏字段.
                map.AddTBInt(MyDeptFlowAttr.FID, 0, "FID", false, false);
                map.AddTBString(MyDeptFlowAttr.FK_Dept, null, "部门", false, false, 0, 30, 10);


                map.AddSearchAttr(MyDeptFlowAttr.FK_Flow);
                map.AddSearchAttr(MyDeptFlowAttr.WFSta);
                map.AddSearchAttr(MyDeptFlowAttr.TSpan);
                map.AddHidden(MyStartFlowAttr.FID, "=", "0");


                //增加隐藏的查询条件.
                AttrOfSearch search = new AttrOfSearch(MyDeptFlowAttr.FK_Dept, "部门",
                    MyDeptFlowAttr.FK_Dept, "=", BP.Web.WebUser.FK_Dept, 0, true);

                map.AttrsOfSearch.Add(search);

                RefMethod rm = new RefMethod();
                rm.Title = "流程轨迹";  
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                map.AddRefMethod(rm);
              
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

		#region 执行诊断
        public string DoTrack()
        {
            PubClass.WinOpen("../../WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow+"&FK_Node="+this.FK_Node, 900, 800);
            return null;
        }
		#endregion
	}
	/// <summary>
    /// 我部门的流程s
	/// </summary>
	public class MyDeptFlows : Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{			 
				return new MyDeptFlow();
			}
		}
		/// <summary>
		/// 我部门的流程集合
		/// </summary>
		public MyDeptFlows(){}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyDeptFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyDeptFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyDeptFlow> Tolist()
        {
            System.Collections.Generic.List<MyDeptFlow> list = new System.Collections.Generic.List<MyDeptFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyDeptFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
