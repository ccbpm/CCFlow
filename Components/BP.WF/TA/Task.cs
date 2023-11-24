using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.TA
{
    /// <summary>
    /// 任务属性
    /// </summary>
    public class TaskAttr : EntityOIDAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 任务内容
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 按表单字段任务
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 表单字段
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 是否启用任务到角色
        /// </summary>
        public const string TaskSta = "TaskSta";
        /// <summary>
        /// 按照角色计算方式
        /// </summary>
        public const string IsRead = "IsRead";
        /// <summary>
        /// 即时消息
        /// </summary>
        public const string NowMsg = "NowMsg";
        /// <summary>
        /// 最近活动日期.
        /// </summary>
        public const string ADT = "ADT";
        #endregion

        #region 项目信息.
        /// <summary>
        /// 是否任务到部门
        /// </summary>
        public const string PrjNo = "PrjNo";
        /// <summary>
        /// 是否任务到人员
        /// </summary>
        public const string PrjName = "PrjName";
        /// <summary>
        /// 是否启用按照SQL任务.
        /// </summary>
        public const string StarterNo = "StarterNo";
        /// <summary>
        /// 要任务的SQL
        /// </summary>
        public const string StarterName = "StarterName";
        public const string PrjSta = "PrjSta";
        public const string PRI = "PRI";
        public const string WCL = "WCL";
        public const string RDT = "RDT";
        #endregion 项目信息.
        public const string PTaskID = "PTaskID";

        public const string SenderNo = "SenderNo";
        public const string SenderName = "SenderName";

        public const string NodeNo = "NodeNo";
        public const string NodeName = "NodeName";


    }
    /// <summary>
    /// 任务
    /// </summary>
    public class Task : EntityOID
    {
        #region 属性
        public Int64 PTaskID
        {
            get
            {
                return this.GetValInt64ByKey(TaskAttr.PTaskID);
            }
            set
            {
                this.SetValByKey(TaskAttr.PTaskID, value);
            }
        }
        public Int64 TaskID
        {
            get
            {
                return this.GetValInt64ByKey(TaskAttr.OID);
            }
            set
            {
                this.SetValByKey(TaskAttr.OID, value);
            }
        }
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.NodeName);
            }
            set
            {
                this.SetValByKey(TaskAttr.NodeName, value);
            }
        }
        public string NodeNo
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.NodeNo);
            }
            set
            {
                this.SetValByKey(TaskAttr.NodeNo, value);
            }
        }
        public string SenderNo
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.SenderNo);
            }
            set
            {
                this.SetValByKey(TaskAttr.SenderNo, value);
            }
        }
        public string SenderName
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.SenderName);
            }
            set
            {
                this.SetValByKey(TaskAttr.SenderName, value);
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.Title);
            }
            set
            {
                this.SetValByKey(TaskAttr.Title, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.RDT);
            }
            set
            {
                this.SetValByKey(TaskAttr.RDT, value);
            }
        }
        /// <summary>
        /// 最近活动日期
        /// </summary>
        public string ADT
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.ADT);
            }
            set
            {
                this.SetValByKey(TaskAttr.ADT, value);
            }
        }
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.EmpNo);
            }
            set
            {
                this.SetValByKey(TaskAttr.EmpNo, value);
            }
        }
        public string EmpName
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.EmpName);
            }
            set
            {
                this.SetValByKey(TaskAttr.EmpName, value);
            }
        }
        public int TaskSta
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.TaskSta);
            }
            set
            {
                this.SetValByKey(TaskAttr.TaskSta, value);
            }
        }
        public int ItIsRead
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.IsRead);
            }
            set
            {
                this.SetValByKey(TaskAttr.IsRead, value);
            }
        }
        public string NowMsg
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.NowMsg);
            }
            set
            {
                this.SetValByKey(TaskAttr.NowMsg, value);
            }
        }
        
        public string PrjNo
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.PrjNo);
            }
            set
            {
                this.SetValByKey(TaskAttr.PrjNo, value);
            }
        }
        public string PrjName
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.PrjName);
            }
            set
            {
                this.SetValByKey(TaskAttr.PrjName, value);
            }
        }
        public string StarterNo
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.StarterNo);
            }
            set
            {
                this.SetValByKey(TaskAttr.StarterNo, value);
            }
        }
        public string StarterName
        {
            get
            {
                return this.GetValStringByKey(TaskAttr.StarterName);
            }
            set
            {
                this.SetValByKey(TaskAttr.StarterName, value);
            }
        }
        public int PrjSta
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.PrjSta);
            }
            set
            {
                this.SetValByKey(TaskAttr.PrjSta, value);
            }
        }
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.PRI);
            }
            set
            {
                this.SetValByKey(TaskAttr.PRI, value);
            }
        }
        public int WCL
        {
            get
            {
                return this.GetValIntByKey(TaskAttr.WCL);
            }
            set
            {
                this.SetValByKey(TaskAttr.WCL, value);
            }
        }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 任务
        /// </summary>
        public Task()
        {
        }
        /// <summary>
        /// 任务
        /// </summary>
        /// <param name="oid"></param>
        public Task(Int64 taskID)
        {
            this.SetValByKey("OID", taskID);
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

                Map map = new Map("TA_Task", "任务");

                #region 任务信息
                map.AddTBIntPK(TaskAttr.OID, 0, "工作ID", true, true, true);
                map.AddTBString(TaskAttr.Title, null, "标题", true, false, 0, 50, 300);
                map.AddTBString(TaskAttr.EmpNo, null, "负责人", true, false, 0, 50, 300);
                map.AddTBString(TaskAttr.EmpName, null, "名称", true, false, 0, 50, 300);
                map.AddTBInt(TaskAttr.TaskSta, 0, "Task状态", true, true);
                map.AddTBInt(TaskAttr.IsRead, 0, "是否读取?", true, true);
                map.AddTBDateTime(TaskAttr.RDT, null, "下达日期", true, false);
                map.AddTBDateTime(TaskAttr.ADT, null, "活动日期", true, false);
                map.AddTBString(TaskAttr.NowMsg, null, "即时消息", true, false, 0, 600, 300);

                map.AddTBInt(TaskAttr.PTaskID, 0, "父节点ID", true, true);

                map.AddTBString(TaskAttr.SenderNo, null, "分配人", false, false, 0, 50, 300);
                map.AddTBString(TaskAttr.SenderName, null, "分配人", true, true, 0, 50, 300);
                #endregion 任务信息

                #region 项目信息
                map.AddTBString(TaskAttr.PrjNo, null, "项目编号", true, false, 0, 50, 300);
                map.AddTBString(TaskAttr.PrjName, null, "名称", true, false, 0, 50, 300);
                map.AddTBString(TaskAttr.StarterNo, null, "发起人编号", false, false, 0, 50, 300);
                map.AddTBString(TaskAttr.StarterName, null, "发起人名称", true, true, 0, 50, 300);
                map.AddTBInt(TaskAttr.PrjSta, 0, "状态", true, true);
                map.AddTBInt(TaskAttr.PRI, 0, "优先级", true, true);
                map.AddTBInt(TaskAttr.WCL, 0, "完成率", true, true);
                #endregion 任务信息

                #region 节点信息
                map.AddTBString(TaskAttr.NodeNo, null, "节点编号", true, false, 0, 50, 300);
                map.AddTBString(TaskAttr.NodeName, null, "节点名称", true, false, 0, 50, 300);
                #endregion 节点信息

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 任务s
    /// </summary>
    public class Tasks : EntitiesOID
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
        public Tasks() { }
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
