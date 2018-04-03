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
    /// 我部门的待办
	/// </summary>
    public class MyDeptTodolistAttr
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
        public const string FK_Emp = "FK_Emp";
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
        /// 我部门
        /// </summary>
        public const string WorkerDept = "WorkerDept";
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
    /// 我部门的待办
	/// </summary>
	public class MyDeptTodolist : Entity
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
                return MyDeptTodolistAttr.WorkID;
            }
        }
		/// <summary>
		/// 工作流程编号
		/// </summary>
		public string  FK_Flow
		{
			get
			{
				return this.GetValStrByKey(MyDeptTodolistAttr.FK_Flow);
			}
			set
			{
				SetValByKey(MyDeptTodolistAttr.FK_Flow,value);
			}
		}
        /// <summary>
        /// BillNo
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.BillNo);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.FlowName);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(MyDeptTodolistAttr.PRI);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.PRI, value);
            }
        }
        /// <summary>
        /// 待办人员数量
        /// </summary>
        public int TodoEmpsNum
        {
            get
            {
                return this.GetValIntByKey(MyDeptTodolistAttr.TodoEmpsNum);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.TodoEmpsNum, value);
            }
        }
        /// <summary>
        /// 待办人员列表
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.TodoEmps);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 参与人
        /// </summary>
        public string Emps
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.Emps);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.Emps, value);
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public TaskSta TaskSta
        {
            get
            {
                return (TaskSta)this.GetValIntByKey(MyDeptTodolistAttr.TaskSta);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.TaskSta, (int)value);
            }
        }
        /// <summary>
        /// 类别编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.FK_Emp);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
		public string  FK_Dept
		{
			get
			{
				return this.GetValStrByKey(MyDeptTodolistAttr.FK_Dept);
			}
			set
			{
				SetValByKey(MyDeptTodolistAttr.FK_Dept,value);
			}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string  Title
		{
			get
			{
				return this.GetValStrByKey(MyDeptTodolistAttr.Title);
			}
			set
			{
				SetValByKey(MyDeptTodolistAttr.Title,value);
			}
		}
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.GuestNo);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.GuestName);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.GuestName, value);
            }
        }
		/// <summary>
		/// 产生时间
		/// </summary>
		public string  RDT
		{
			get
			{
				return this.GetValStrByKey(MyDeptTodolistAttr.RDT);
			}
			set
			{
				SetValByKey(MyDeptTodolistAttr.RDT,value);
			}
		}
        /// <summary>
        /// 节点应完成时间
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.SDTOfNode);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 流程应完成时间
        /// </summary>
        public string SDTOfFlow
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.SDTOfFlow);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.SDTOfFlow, value);
            }
        }
		/// <summary>
		/// 流程ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
                return this.GetValInt64ByKey(MyDeptTodolistAttr.WorkID);
			}
			set
			{
				SetValByKey(MyDeptTodolistAttr.WorkID,value);
			}
		}
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(MyDeptTodolistAttr.FID);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.FID, value);
            }
        }
        /// <summary>
        /// 父节点流程编号.
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(MyDeptTodolistAttr.PWorkID);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程调用的节点
        /// </summary>
        public int PNodeID
        {
            get
            {
                return this.GetValIntByKey(MyDeptTodolistAttr.PNodeID);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.PNodeID, value);
            }
        }
        /// <summary>
        /// PFlowNo
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.PFlowNo);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// 吊起子流程的人员
        /// </summary>
        public string PEmp
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.PEmp);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.PEmp, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.Starter);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.StarterName);
            }
            set
            {
                this.SetValByKey(MyDeptTodolistAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 发起人部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.DeptName);
            }
            set
            {
                this.SetValByKey(MyDeptTodolistAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStrByKey(MyDeptTodolistAttr.NodeName);
            }
            set
            {
                this.SetValByKey(MyDeptTodolistAttr.NodeName, value);
            }
        }
		/// <summary>
		/// 当前工作到的节点
		/// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(MyDeptTodolistAttr.FK_Node);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.FK_Node, value);
            }
        }
        /// <summary>
		/// 工作流程状态
		/// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(MyDeptTodolistAttr.WFState);
            }
            set
            {
                if (value == WF.WFState.Complete)
                    SetValByKey(MyDeptTodolistAttr.WFSta, (int)WFSta.Complete);
                else if (value == WF.WFState.Delete)
                    SetValByKey(MyDeptTodolistAttr.WFSta, (int)WFSta.Etc);
                else
                    SetValByKey(MyDeptTodolistAttr.WFSta, (int)WFSta.Runing);

                SetValByKey(MyDeptTodolistAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态(简单)
        /// </summary>
        public WFSta WFSta
        {
            get
            {
                return (WFSta)this.GetValIntByKey(MyDeptTodolistAttr.WFSta);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.WFSta, (int)value);
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
                return this.GetValStrByKey(MyDeptTodolistAttr.GUID);
            }
            set
            {
                SetValByKey(MyDeptTodolistAttr.GUID, value);
            }
        }
		#endregion

        #region 参数属性.
        #endregion 参数属性.

        #region 构造函数
        /// <summary>
		/// 产生的工作流程
		/// </summary>
		public MyDeptTodolist()
		{
		}
        public MyDeptTodolist(Int64 workId)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MyDeptTodolistAttr.WorkID, workId);
            if (qo.DoQuery() == 0)
                throw new Exception("工作 MyDeptTodolist [" + workId + "]不存在。");
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

                Map map = new Map("WF_EmpWorks", "我部门的待办");
                map.Java_SetEnType(EnType.View);

                map.AddTBInt(MyDeptTodolistAttr.FID, 0, "FID", false, false);
                map.AddTBString(MyDeptTodolistAttr.Title, null, "流程标题", true, false, 0, 300, 10, true);
                map.AddDDLEntities(MyDeptTodolistAttr.FK_Flow, null, "流程", new Flows(), false);
                map.AddTBString(MyDeptTodolistAttr.RDT, null, "发起时间", true, false, 0, 100, 10);

               
                map.AddTBString(MyDeptTodolistAttr.StarterName, null, "发起人名称", true, false, 0, 30, 10);

                map.AddTBString(MyDeptTodolistAttr.NodeName, null, "停留节点", true, false, 0, 100, 10);
                //map.AddTBString(MyDeptTodolistAttr.TodoEmps, null, "当前处理人", true, false, 0, 100, 10);

                map.AddTBStringDoc(MyDeptTodolistAttr.FlowNote, null, "备注", true, false,true);

                //作为隐藏字段.
                map.AddTBString(MyDeptTodolistAttr.WorkerDept, null, "工作人员部门编号", 
                    false, false, 0, 30, 10);
                map.AddTBMyNum();
                map.AddDDLEntities(MyDeptTodolistAttr.FK_Emp, null, "当前处理人", new BP.WF.Data.MyDeptEmps(), false);
                map.AddTBIntPK(MyDeptTodolistAttr.WorkID, 0, "工作ID", true, true);

                //查询条件.
                map.AddSearchAttr(MyDeptTodolistAttr.FK_Flow);
                map.AddSearchAttr(MyDeptTodolistAttr.FK_Emp);

                //增加隐藏的查询条件.
                AttrOfSearch search = new AttrOfSearch(MyDeptTodolistAttr.WorkerDept, "部门",
                    MyDeptTodolistAttr.WorkerDept, "=", BP.Web.WebUser.FK_Dept, 0, true);
                map.AttrsOfSearch.Add(search);

                RefMethod rm = new RefMethod();
                rm.Title = "轨迹";  
                rm.ClassMethodName = this.ToString() + ".DoTrack";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/CC.gif";
                rm.Title = "移交";
                rm.ClassMethodName = this.ToString() + ".DoShift";
                rm.HisAttrs.AddDDLEntities("ToEmp", null, "移交给:", new BP.WF.Flows(), true);
                rm.HisAttrs.AddTBString("Note", null, "移交原因", true, false, 0, 300, 100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "../../WF/Img/Btn/Back.png";
                rm.Title = "回滚";
                rm.IsForEns = false;
                rm.ClassMethodName = this.ToString() + ".DoComeBack";
                rm.HisAttrs.AddTBInt("NodeID", 0, "回滚到节点", true, false);
                rm.HisAttrs.AddTBString("Note", null, "回滚原因", true, false, 0, 300, 100);
                map.AddRefMethod(rm);

              
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        #region 执行功能.
        public string DoTrack()
        {
            PubClass.WinOpen("../../WFRpt.htm?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow+"&FK_Node=", 900, 800);
            return null;
        }
        /// <summary>
        /// 执行移交
        /// </summary>
        /// <param name="ToEmp"></param>
        /// <param name="Note"></param>
        /// <returns></returns>
        public string DoShift(string ToEmp, string Note)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID, this.FID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, ToEmp, Note);
                return "移交成功";
            }
            catch (Exception ex)
            {
                return "移交失败@" + ex.Message;
            }
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string DoDelete()
        {
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID, this.FID) == false)
                return "您没有操作该流程数据的权限.";

            try
            {
                BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
                return "删除成功";
            }
            catch (Exception ex)
            {
                return "删除失败@" + ex.Message;
            }
        }
        public string DoSkip()
        {
            PubClass.WinOpen("../../Admin/FlowDB/FlowSkip.aspx?WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node, 900, 800);
            return null;
        }
        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="note">回滚原因</param>
        /// <returns>回滚的结果</returns>
        public string DoComeBack(int nodeid, string note)
        {
            BP.WF.Template.FlowSheet fl = new Template.FlowSheet(this.FK_Flow);
            return fl.DoRebackFlowData(this.WorkID, nodeid, note);
        }
        #endregion
	}
	/// <summary>
    /// 我部门的待办s
	/// </summary>
	public class MyDeptTodolists : Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{			 
				return new MyDeptTodolist();
			}
		}
		/// <summary>
		/// 我部门的待办集合
		/// </summary>
		public MyDeptTodolists(){}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyDeptTodolist> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyDeptTodolist>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyDeptTodolist> Tolist()
        {
            System.Collections.Generic.List<MyDeptTodolist> list = new System.Collections.Generic.List<MyDeptTodolist>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyDeptTodolist)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
