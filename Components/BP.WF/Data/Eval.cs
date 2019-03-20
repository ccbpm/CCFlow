using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port; 
using BP.En;

namespace BP.WF.Data
{
	/// <summary>
	/// 工作质量评价
	/// </summary>
	public class EvalAttr 
	{
		#region 基本属性
		/// <summary>
		/// 流程编号
		/// </summary>
		public const  string FK_Flow="FK_Flow";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 考核的节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 隶属部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
		/// <summary>
		/// 年月
		/// </summary>
		public const  string FK_NY="FK_NY";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 评价时间
        /// </summary>
        public const string RDT = "RDT";
		/// <summary>
		/// 被考核的人员名称
		/// </summary>
		public const  string EvalEmpName="EvalEmpName";
		/// <summary>
        /// 被考核的人员编号
		/// </summary>
		public const  string EvalEmpNo="EvalEmpNo";
        /// <summary>
        /// 评价分值
        /// </summary>
        public const string EvalCent = "EvalCent";
        /// <summary>
        /// 评价内容
        /// </summary>
        public const string EvalNote = "EvalNote";
		/// <summary>
		/// 评价人员
		/// </summary>
		public const  string Rec="Rec";
        /// <summary>
        /// 评价人员名称
        /// </summary>
        public const string RecName = "RecName";
		#endregion
	}
	/// <summary>
	/// 工作质量评价
	/// </summary>
	public class Eval : EntityMyPK
	{
		#region 基本属性
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.Title);
            }
            set
            {
                this.SetValByKey(EvalAttr.Title, value);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
		{
			get
			{
				return this.GetValInt64ByKey(EvalAttr.WorkID);
			}
			set
			{
				this.SetValByKey(EvalAttr.WorkID,value);
			}
		}
        /// <summary>
        /// 节点编号
        /// </summary>
		public int FK_Node
		{
			get
			{
				return this.GetValIntByKey(EvalAttr.FK_Node);
			}
			set
			{
				this.SetValByKey(EvalAttr.FK_Node,value);
			}
		}
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.NodeName);
            }
            set
            {
                this.SetValByKey(EvalAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 被评估人员名称
        /// </summary>
		public string  EvalEmpName
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.EvalEmpName);
			}
			set
			{
				this.SetValByKey(EvalAttr.EvalEmpName,value);
			}
		}
        /// <summary>
        /// 记录日期
        /// </summary>
		public string  RDT
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.RDT);
			}
			set
			{
				this.SetValByKey(EvalAttr.RDT,value);
			}
		}
		/// <summary>
		/// 流程隶属部门
		/// </summary>
		public string FK_Dept
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.FK_Dept);
			}
			set
			{
				this.SetValByKey(EvalAttr.FK_Dept,value);
			}
		}
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.DeptName);
            }
            set
            {
                this.SetValByKey(EvalAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 隶属年月
        /// </summary>
		public string  FK_NY
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.FK_NY);
			}
			set
			{
				this.SetValByKey(EvalAttr.FK_NY,value);
			}
		}
        /// <summary>
        /// 流程编号
        /// </summary>
		public string  FK_Flow
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.FK_Flow);
			}
            set
            {
                this.SetValByKey(EvalAttr.FK_Flow, value);
            }
		}
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
		{
			get
			{
                return this.GetValStringByKey(EvalAttr.FlowName);
			}
			set
			{
                this.SetValByKey(EvalAttr.FlowName, value);
			}
		}
        /// <summary>
        /// 评价人
        /// </summary>
		public string  Rec
		{
			get
			{
				return this.GetValStringByKey(EvalAttr.Rec);
			}
			set
			{
				this.SetValByKey(EvalAttr.Rec,value);
			}
		}
        /// <summary>
        /// 评价人名称
        /// </summary>
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.RecName);
            }
            set
            {
                this.SetValByKey(EvalAttr.RecName, value);
            }
        }
        /// <summary>
        /// 评价内容
        /// </summary>
        public string EvalNote
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.EvalNote);
            }
            set
            {
                this.SetValByKey(EvalAttr.EvalNote, value);
            }
        }
        /// <summary>
        /// 被考核的人员编号
        /// </summary>
        public string EvalEmpNo
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.EvalEmpNo);
            }
            set
            {
                this.SetValByKey(EvalAttr.EvalEmpNo, value);
            }
        }
        /// <summary>
        /// 评价分值
        /// </summary>
        public string EvalCent
        {
            get
            {
                return this.GetValStringByKey(EvalAttr.EvalCent);
            }
            set
            {
                this.SetValByKey(EvalAttr.EvalCent, value);
            }
        }
		#endregion 

		#region 构造函数
		/// <summary>
		/// 工作质量评价
		/// </summary>
		public Eval()
        {
        }
        /// <summary>
        /// 工作质量评价
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="FK_Node"></param>
		public Eval(int workid, int FK_Node)
		{
			this.WorkID=workid;
			this.FK_Node=FK_Node;
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
                Map map = new Map("WF_CHEval", "工作质量评价");


                map.AddMyPK();
                map.AddTBString(EvalAttr.Title, null, "标题", false, true, 0, 500, 10);
                map.AddTBString(EvalAttr.FK_Flow, null, "流程编号", false, true, 0, 7, 10);
                map.AddTBString(EvalAttr.FlowName, null, "流程名称", false, true, 0, 100, 10);

                map.AddTBInt(EvalAttr.WorkID, 0, "工作ID", false, true);
                map.AddTBInt(EvalAttr.FK_Node, 0, "评价节点", false, true);
                map.AddTBString(EvalAttr.NodeName, null, "停留节点", false, true, 0, 100, 10);

                map.AddTBString(EvalAttr.Rec, null, "评价人", false, true, 0, 50, 10);
                map.AddTBString(EvalAttr.RecName, null, "评价人名称", false, true, 0, 50, 10);

                map.AddTBDateTime(EvalAttr.RDT, "评价日期", true, true);

                map.AddTBString(EvalAttr.EvalEmpNo, null, "被考核的人员编号", false, true, 0, 50, 10);
                map.AddTBString(EvalAttr.EvalEmpName, null, "被考核的人员名称", false, true, 0, 50, 10);
                map.AddTBString(EvalAttr.EvalCent, null, "评价分值", false, true, 0, 20, 10);
                map.AddTBString(EvalAttr.EvalNote, null, "评价内容", false, true, 0, 20, 10);

                map.AddTBString(EvalAttr.FK_Dept, null, "部门", false, true, 0, 50, 10);
                map.AddTBString(EvalAttr.DeptName, null, "部门名称", false, true, 0, 100, 10);
                map.AddTBString(EvalAttr.FK_NY, null, "年月", false, true, 0, 7, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion
	}
	/// <summary>
	/// 工作质量评价s BP.Port.FK.Evals
	/// </summary>
	public class Evals : EntitiesMyPK
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Eval();
			}
		}
		/// <summary>
        /// 工作质量评价s
		/// </summary>
		public Evals(){}
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Eval> ToJavaList()
        {
            return (System.Collections.Generic.IList<Eval>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Eval> Tolist()
        {
            System.Collections.Generic.List<Eval> list = new System.Collections.Generic.List<Eval>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Eval)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
	
}
