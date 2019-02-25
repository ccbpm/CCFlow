using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 任务 属性
	/// </summary>
    public class TaskAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 发起人
        /// </summary>
        public const string Starter = "Starter";
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Paras = "Paras";
        /// <summary>
        /// 任务状态
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// Msg
        /// </summary>
        public const string Msg = "Msg";
        /// <summary>
        /// 发起时间
        /// </summary>
        public const string StartDT = "StartDT";
        /// <summary>
        /// 插入日期
        /// </summary>
        public const string RDT = "RDT";

        /// <summary>
        /// 到达节点（可以为0）
        /// </summary>
        public const string ToNode = "ToNode";
        /// <summary>
        /// 到达人员（可以为空）
        /// </summary>
        public const string ToEmps = "ToEmps";
        #endregion
    }
	/// <summary>
	/// 任务
	/// </summary>
    public class Task : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 参数
        /// </summary>
        public string Paras
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.Paras);
            }
            set
            {
                this.SetValByKey(TaskAttr.Paras, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.Starter);
            }
            set
            {
                this.SetValByKey(TaskAttr.Starter, value);
            }
        }
        /// <summary>
        /// 到达的人员
        /// </summary>
        public string ToEmps
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.ToEmps);
            }
            set
            {
                this.SetValByKey(TaskAttr.ToEmps, value);
            }
        }
        /// <summary>
        /// 到达节点（可以为0）
        /// </summary>
        public int ToNode
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.ToNode);
            }
            set
            {
                this.SetValByKey(TaskAttr.ToNode, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(TaskAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 发起时间（可以为空）
        /// </summary>
        public string StartDT
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.StartDT);
            }
            set
            {
                this.SetValByKey(TaskAttr.StartDT, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// Task
        /// </summary>
        public Task()
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
                Map map = new Map("WF_Task", "任务");
                map.Java_SetEnType(EnType.Admin);

                map.AddMyPK(); //唯一的主键.
                map.AddTBString(TaskAttr.FK_Flow, null, "流程编号", true, false, 0, 200, 10);
                map.AddTBString(TaskAttr.Starter, null, "发起人", true, false, 0, 200, 10);

                //为上海同事科技增加两个字段. 可以为空.
                map.AddTBInt(TaskAttr.ToNode, 0, "到达的节点", true, false);
                map.AddTBString(TaskAttr.ToEmps, null, "到达人员", true, false, 0, 200, 10);

                map.AddTBString(TaskAttr.Paras, null, "参数", true, false, 0, 4000, 10);

                // TaskSta 0=未发起，1=成功发起，2=发起失败.
                map.AddTBInt(TaskAttr.TaskSta, 0, "任务状态", true, false);

                map.AddTBString(TaskAttr.Msg, null, "消息", true, false, 0, 4000, 10);
                map.AddTBString(TaskAttr.StartDT, null, "发起时间", true, false, 0, 20, 10);
                map.AddTBString(TaskAttr.RDT, null, "插入数据时间", true, false, 0, 20, 10);
                
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 任务
	/// </summary>
	public class Tasks: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Task();
			}
		}
		/// <summary>
        /// 任务
		/// </summary>
		public Tasks(){} 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Task> ToJavaList()
        {
            return (System.Collections.Generic.IList<Task>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Task> Tolist()
        {
            System.Collections.Generic.List<Task> list = new System.Collections.Generic.List<Task>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Task)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
