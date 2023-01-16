using BP.En;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.WF
{
    public class WorkOptAttr
    {
        #region 基本属性
        /// <summary>
        /// 状态
        /// </summary>
        public const string NodeID = "NodeID";
        //
        public const string ToNodeID = "ToNodeID";
        /// <summary>
        /// H
        /// </summary>
        public const string WorkID = "WorkID";
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 发送到人员
        /// </summary>
        public const string SendEmps = "SendEmps";
        /// <summary>
        /// 发送到部门
        /// </summary>
        public const string SendDepts = "SendDepts";
        /// <summary>
        /// 发送到角色.
        /// </summary>
        public const string SendStas = "SendStas";
        /// <summary>
        /// 发送内容
        /// </summary>
        public const string SendNote = "SendNote";

        /// <summary>
        /// 抄送到人员
        /// </summary>
        public const string CCEmps = "CCEmps";
        /// <summary>
        /// 抄送到部门
        /// </summary>
        public const string CCDepts = "CCDepts";
        /// <summary>
        /// 抄送到角色.
        /// </summary>
        public const string CCStas = "CCStas";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string CCNote = "CCNote";
        public const string FlowNo = "FlowNo";
        #endregion

        public const string Title = "Title";
        public const string NodeName = "NodeName";
        public const string ToNodeName = "ToNodeName";
        public const string TodoEmps = "TodoEmps";
        public const string SenderName = "SenderName";
        public const string SendRDT = "SendRDT";
        public const string SendSDT = "SendSDT";
    }
    /// <summary>
    /// 退回轨迹
    /// </summary>
    public class WorkOpt : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(WorkOptAttr.WorkID);
            }
            set
            {
                SetValByKey(WorkOptAttr.WorkID, value);
            }
        }

        /// <summary>
        /// 退回到节点
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(WorkOptAttr.NodeID);
            }
            set
            {
                SetValByKey(WorkOptAttr.NodeID, value);
            }
        }
        public int ToNodeID
        {
            get
            {
                return this.GetValIntByKey(WorkOptAttr.ToNodeID);
            }
            set
            {
                SetValByKey(WorkOptAttr.ToNodeID, value);
            }
        }
        public string FlowNo
        {
            get
            {
                return this.GetValStrByKey(WorkOptAttr.FlowNo);
            }
            set
            {
                SetValByKey(WorkOptAttr.FlowNo, value);
            }
        }
        public string SendNote
        {
            get
            {
                return this.GetValStrByKey(WorkOptAttr.SendNote);
            }
            set
            {
                SetValByKey(WorkOptAttr.SendNote, value);
            }
        }
        public string EmpNo
        {
            get
            {
                return this.GetValStrByKey(WorkOptAttr.EmpNo);
            }
            set
            {
                SetValByKey(WorkOptAttr.EmpNo, value);
            }
        }
        /// <summary>
        /// 退回人
        /// </summary>
        public string SendEmps
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.SendEmps);
            }
            set
            {
                SetValByKey(WorkOptAttr.SendEmps, value);
            }
        }
        public string SendDepts
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.SendDepts);
            }
            set
            {
                SetValByKey(WorkOptAttr.SendDepts, value);
            }
        }
        public string SendStas
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.SendStas);
            }
            set
            {
                SetValByKey(WorkOptAttr.SendStas, value);
            }
        }
        public string CCEmps
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.CCEmps);
            }
            set
            {
                SetValByKey(WorkOptAttr.CCEmps, value);
            }
        }
        public string CCDepts
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.CCDepts);
            }
            set
            {
                SetValByKey(WorkOptAttr.CCDepts, value);
            }
        }
        public string CCStas
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.CCStas);
            }
            set
            {
                SetValByKey(WorkOptAttr.CCStas, value);
            }
        }
        public string CCNote
        {
            get
            {
                return this.GetValStringByKey(WorkOptAttr.CCNote);
            }
            set
            {
                SetValByKey(WorkOptAttr.CCNote, value);
            }
        }
        #endregion

        #region 构造函数
        public WorkOpt() { }
        /// <summary>
        /// 退回轨迹
        /// </summary>
        public WorkOpt(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("WF_WorkOpt", "工作处理器");

                map.AddMyPK();

                map.AddTBInt(WorkOptAttr.WorkID, 0, "工作ID", false, true);
                map.AddTBInt(WorkOptAttr.NodeID, 0, "节点ID", false, true);
                map.AddTBInt(WorkOptAttr.ToNodeID, 0, "到达的节点ID", false, true);

                map.AddTBString(WorkOptAttr.EmpNo, null, "操作员", true, true, 0, 100, 10);
                map.AddTBString(WorkOptAttr.FlowNo, null, "FlowNo", false, false, 0, 10, 10);


                map.AddGroupAttr("发送");
                map.AddTBString(WorkOptAttr.SendEmps, null, "发送到人员", true, true, 0, 500, 10, true);
                map.AddTBString(WorkOptAttr.SendEmps + "T", null, "发送到人员", false, false, 0, 500, 10, true);

                map.AddTBString(WorkOptAttr.SendDepts, null, "发送到部门", true, true, 0, 500, 10, true);
                map.AddTBString(WorkOptAttr.SendDepts + "T", null, "发送到部门", false, false, 0, 500, 10, true);

                map.AddTBString(WorkOptAttr.SendStas, null, "发送到角色", true, true, 0, 100, 10, true);
                map.AddTBString(WorkOptAttr.SendStas + "T", null, "发送到角色", false, false, 0, 500, 10, true);
                map.AddTBStringDoc(WorkOptAttr.SendNote, null, "小纸条", true, true, true);

                map.AddGroupAttr("抄送");
                map.AddTBString(WorkOptAttr.CCEmps, null, "抄送到人员", true, true, 0, 100, 10, true);
                map.AddTBString(WorkOptAttr.CCEmps + "T", null, "抄送到人员", false, false, 0, 500, 10, true);

                map.AddTBString(WorkOptAttr.CCDepts, null, "抄送到部门", true, true, 0, 100, 10, true);
                map.AddTBString(WorkOptAttr.CCDepts + "T", null, "抄送到部门", false, false, 0, 500, 10, true);

                map.AddTBString(WorkOptAttr.CCStas, null, "抄送到角色", true, true, 0, 100, 10, true);
                map.AddTBString(WorkOptAttr.CCStas + "T", null, "抄送到部门", false, false, 0, 500, 10, true);
                map.AddTBStringDoc(WorkOptAttr.CCNote, null, "抄送说明", true, true, true);

                map.AddGroupAttr("工作信息");
                map.AddTBString(WorkOptAttr.Title, null, "标题", true, true, 0, 200, 10, true);
                map.AddTBString(WorkOptAttr.NodeName, null, "当前节点", true, true, 0, 500, 10, false, null);
                map.AddTBString(WorkOptAttr.ToNodeName, null, "到达节点", true, true, 0, 500, 10, false, null);

                map.AddTBInt(WorkOptAttr.ToNodeID, 0, "到达节点ID", false, false);

                map.AddTBString(WorkOptAttr.TodoEmps, null, "当前处理人", true, true, 0, 200, 10,true);
                map.AddTBString(WorkOptAttr.SenderName, null, "发送人", true, true, 0, 100, 10);

                map.AddTBString(WorkOptAttr.SendRDT, null, "发送日期", true, true, 0, 100, 10);
                map.AddTBString(WorkOptAttr.SendSDT, null, "限期", true, true, 0, 100, 10);
                map.AddTBInt(WorkOptAttr.WorkID, 0,"工作ID", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 退回轨迹s 
    /// </summary>
    public class WorkOpts : Entities
    {
        #region 构造
        /// <summary>
        /// 退回轨迹s
        /// </summary>
        public WorkOpts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WorkOpt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WorkOpt> ToJavaList()
        {
            return (System.Collections.Generic.IList<WorkOpt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WorkOpt> Tolist()
        {
            System.Collections.Generic.List<WorkOpt> list = new System.Collections.Generic.List<WorkOpt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WorkOpt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}
