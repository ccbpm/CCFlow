using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port ;
using BP.Sys;
using BP.En;
using BP.WF.Template;

namespace BP.WF.Data
{
	/// <summary>
    /// 流程统计
	/// </summary>
    public class GenerWorkFlowTodolistAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作流
        /// </summary>
        public const string FK_Flow = "FK_Flow";
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

        public const string TodoSta0 = "TodoSta0";
        public const string TodoSta1 = "TodoSta1";
        public const string TodoSta2 = "TodoSta2";
        public const string TodoSta3 = "TodoSta3";
        public const string TodoSta4 = "TodoSta4";
        #endregion
    }
	/// <summary>
    /// 流程统计
	/// </summary>
	public class GenerWorkFlowTodolist : EntityMyPK
	{	
		#region 基本属性
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
		/// 产生的工作流程
		/// </summary>
		public GenerWorkFlowTodolist()
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

                Map map = new Map("V_Todolist", "流程统计");
                map.EnType = EnType.View;

                map.AddMyPK();

                map.AddDDLEntities(GenerWorkFlowTodolistAttr.FK_FlowSort, null, "类别", new FlowSorts(), false);
                map.AddDDLEntities(GenerWorkFlowTodolistAttr.FK_Flow, null, "流程", new Flows(), false);


                map.AddTBInt(GenerWorkFlowTodolistAttr.TodoSta0, 0, "待办中", true, true);
                map.AddTBInt(GenerWorkFlowTodolistAttr.TodoSta1, 0, "预警中", true, true);
                map.AddTBInt(GenerWorkFlowTodolistAttr.TodoSta2, 0, "逾期中", true, true);
                map.AddTBInt(GenerWorkFlowTodolistAttr.TodoSta3, 0, "正常办结", true, true);
                map.AddTBInt(GenerWorkFlowTodolistAttr.TodoSta4, 0, "超期办结", true, true);
                 
                //map.AddSearchAttr(GenerWorkFlowTodolistAttr.FK_Flow);
                 
                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion 

        #region 执行功能.
        #endregion
		 
	}
	/// <summary>
    /// 流程统计s
	/// </summary>
	public class GenerWorkFlowTodolists : Entities
	{
		/// <summary>
		/// 根据工作流程,工作人员 ID 查询出来他当前的能做的工作.
		/// </summary>
		/// <param name="flowNo">流程编号</param>
		/// <param name="empId">工作人员ID</param>
		/// <returns></returns>
		public static DataTable QuByFlowAndEmp(string flowNo, int empId)
		{
			string sql="SELECT a.WorkID FROM WF_GenerWorkFlowTodolist a, WF_GenerWorkerlist b WHERE a.WorkID=b.WorkID   AND b.FK_Node=a.FK_Node  AND b.FK_Emp='"+empId.ToString()+"' AND a.FK_Flow='"+flowNo+"'";
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
				return new GenerWorkFlowTodolist();
			}
		}
		/// <summary>
		/// 流程统计集合
		/// </summary>
		public GenerWorkFlowTodolists(){}
		#endregion


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GenerWorkFlowTodolist> ToJavaList()
        {
            return (System.Collections.Generic.IList<GenerWorkFlowTodolist>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GenerWorkFlowTodolist> Tolist()
        {
            System.Collections.Generic.List<GenerWorkFlowTodolist> list = new System.Collections.Generic.List<GenerWorkFlowTodolist>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GenerWorkFlowTodolist)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
