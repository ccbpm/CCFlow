using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.TA
{
    /// <summary>
    /// 工作人员属性
    /// </summary>
    public class WorkerListAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string TaskID = "TaskID";
        /// <summary>
        /// 父任务ID
        /// </summary>
        public const string PTaskID = "PTaskID";
        /// <summary>
        /// 工作人员内容
        /// </summary>
        public const string PrjNo = "PrjNo";
        /// <summary>
        /// 按表单字段工作人员
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 表单字段
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 是否工作人员到部门
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 是否工作人员到人员
        /// </summary>
        public const string RDT = "RDT";
        public const string ADT = "ADT";
        #endregion

        public const string PrjSta = "PrjSta";
        public const string PRI = "PRI";
        public const string WCL = "WCL";

        public const string IsPass = "IsPass";
        public const string IsRead = "IsRead";
        public const string TaskDoc = "TaskDoc";

        public const string Title = "Title";
        public const string TaskDesc = "TaskDesc";

        public const string SenderNo = "SenderNo";
        public const string SenderName = "SenderName";

        public const string PrjName = "PrjName";
        public const string StarterNo = "StarterNo";
        public const string StarterName = "StarterName";
    }
    /// <summary>
    /// 工作人员
    /// </summary>
    public class WorkerList : EntityMyPK
    {
        #region 属性
        public int TaskID
        {
            get
            {
                return this.GetValIntByKey(WorkerListAttr.TaskID);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.TaskID, value);
            }
        }
        public int PTaskID
        {
            get
            {
                return this.GetValIntByKey(WorkerListAttr.PTaskID);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.PTaskID, value);
            }
        }
        public string PrjNo
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.PrjNo);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.PrjNo, value);
            }
        }
        public string PrjName
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.PrjName);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.PrjName, value);
            }
        }
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.EmpNo);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.EmpNo, value);
            }
        }
        public string EmpName
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.EmpName);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.EmpName, value);
            }
        }
        public string SenderName
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.SenderName);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.SenderName, value);
            }
        }
        public string SenderNo
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.SenderNo);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.SenderNo, value);
            }
        }
        public string Docs
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.Docs);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.Docs, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.RDT);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.RDT, value);
            }
        }
        public string ADT
        {
            get
            {
                return this.GetValStringByKey(WorkerListAttr.ADT);
            }
            set
            {
                this.SetValByKey(WorkerListAttr.ADT, value);
            }
        }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 工作人员
        /// </summary>
        public WorkerList()
        {
        }
        /// <summary>
        /// 工作人员
        /// </summary>
        /// <param name="mypk"></param>
        public WorkerList(string mypk)
        {
            this.setMyPK(mypk);
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

                Map map = new Map("TA_WorkerList", "工作人员");

                map.AddGroupAttr("工作信息");
                map.AddMyPK(); //TaskID+"_"+EmpNo
                map.AddTBInt(WorkerListAttr.TaskID, 0, "任务ID", true, true);
                map.AddTBString(WorkerListAttr.EmpNo, null, "处理人", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.EmpName, null, "处理人", true, false, 0, 50, 300);

                map.AddTBInt(WorkerListAttr.IsPass, 0, "是否完成?", true, true);
                map.AddTBInt(WorkerListAttr.IsRead, 0, "是否读取?", true, true);
                map.AddTBInt(WorkerListAttr.WCL, 0, "完成率", true, true);
                map.AddTBString(WorkerListAttr.TaskDoc, null, "工作内容", true, false, 0, 50, 300);

                map.AddGroupAttr("任务信息");
                map.AddTBString(WorkerListAttr.Title, null, "标题", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.TaskDesc, null, "任务描述", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.SenderNo, null, "发起人编号", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.SenderName, null, "发起人名称", true, false, 0, 50, 300);
                map.AddTBDateTime(WorkerListAttr.RDT, null, "记录日期", true, true);
                map.AddTBDateTime(WorkerListAttr.ADT, null, "活动日期", true, true);

                map.AddTBInt(WorkerListAttr.PTaskID, 0, "P任务ID", true, true);

                map.AddGroupAttr("项目信息");
                map.AddTBString(WorkerListAttr.PrjNo, null, "项目编号", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.PrjName, null, "名称", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.StarterNo, null, "发起人编号", true, false, 0, 50, 300);
                map.AddTBString(WorkerListAttr.StarterName, null, "发起人名称", true, false, 0, 50, 300);
                map.AddTBInt(WorkerListAttr.PRI, 0, "优先级", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 工作人员s
    /// </summary>
    public class WorkerLists : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkerList();
            }
        }
        /// <summary>
        /// 工作人员
        /// </summary>
        public WorkerLists() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkerList> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkerList>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkerList> Tolist()
        {
            System.Collections.Generic.List<WorkerList> list = new System.Collections.Generic.List<WorkerList>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkerList)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
