using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Web;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    /// 逾期流程属性
    /// </summary>
    public class DelayAttr
    {
        #region 属性
        public const string MyPK = "MyPK";
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 发起人
        /// </summary>
        public const string Starter = "Starter";
        /// <summary>
        /// 发起人名称
        /// </summary>
        public const string StarterName = "StarterName";
        /// <summary>
        /// 状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 停留节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 节点名称
        /// </summary>
        public const string NodeName = "NodeName";
        /// <summary>
        /// 发起人工作部门
        /// </summary>
        public const string WorkerDept = "WorkerDept";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 应完成日期
        /// </summary>
        public const string SDT = "SDT";
        /// <summary>
        /// 当事人
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// 流程类别
        /// </summary>
        public const string FK_FlowSort = "FK_FlowSort";
        /// <summary>
        /// 系统类型
        /// </summary>
        public const string SysType = "SysType";
        /// <summary>
        /// 应完成日期
        /// </summary>
        public const string SDTOfNode = "SDTOfNode";
        /// <summary>
        ///催办次数
        /// </summary>
        public const string PressTimes = "PressTimes";
        /// <summary>
        /// 单据编号
        /// </summary>
        public const string BillNo = "BillNo";
        /// <summary>
        /// 流程备注
        /// </summary>
        public const string FlowNote = "FlowNote";
        /// <summary>
        /// 当前办理人
        /// </summary>
        public const string TodoEmps = "TodoEmps";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        #endregion
    }
    /// <summary>
    /// 逾期流程
    /// </summary> 
    public class Delay : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// workid
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(DelayAttr.WorkID);
            }
            set
            {
                this.SetValByKey(DelayAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Starter
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.Starter);
            }
            set
            {
                this.SetValByKey(DelayAttr.Starter, value);
            }
        }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string StarterName
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.StarterName);
            }
            set
            {
                this.SetValByKey(DelayAttr.StarterName, value);
            }
        }
        /// <summary>
        /// 流程状态
        /// </summary>
        public int WFStateInt
        {
            get
            {
                return this.GetValIntByKey(DelayAttr.WFState);
            }
            set
            {
                this.SetValByKey(DelayAttr.WFState, value);
            }
        }
        /// <summary>
        /// 流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(DelayAttr.WFState);
            }
            set
            {
                this.SetValByKey(DelayAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(DelayAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.DeptName);
            }
            set
            {
                this.SetValByKey(DelayAttr.DeptName, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(DelayAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FlowName);
            }
            set
            {
                this.SetValByKey(DelayAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 当前节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(DelayAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(DelayAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.NodeName);
            }
            set
            {
                this.SetValByKey(DelayAttr.NodeName, value);
            }
        }
        /// <summary>
        /// 工作人部门
        /// </summary>
        public string WorkerDept
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.WorkerDept);
            }
            set
            {
                this.SetValByKey(DelayAttr.WorkerDept, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.Title);
            }
            set
            {
                this.SetValByKey(DelayAttr.Title, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.RDT);
            }
            set
            {
                this.SetValByKey(DelayAttr.RDT, value);
            }
        }
        /// <summary>
        /// 应完成日期
        /// </summary>
        public string SDT
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.SDT);
            }
            set
            {
                this.SetValByKey(DelayAttr.SDT, value);
            }
        }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DelayAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// fid
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(DelayAttr.FID);
            }
            set
            {
                this.SetValByKey(DelayAttr.FID, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(DelayAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 系统类型
        /// </summary>
        public string SysType
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.SysType);
            }
            set
            {
                this.SetValByKey(DelayAttr.SysType, value);
            }
        }
        /// <summary>
        /// 应完成日期
        /// </summary>
        public string SDTOfNode
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.SDTOfNode);
            }
            set
            {
                this.SetValByKey(DelayAttr.SDTOfNode, value);
            }
        }
        /// <summary>
        /// 催办次数
        /// </summary>
        public int PressTimes
        {
            get
            {
                return this.GetValIntByKey(DelayAttr.PressTimes);
            }
            set
            {
                this.SetValByKey(DelayAttr.PressTimes, value);
            }
        }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.BillNo);
            }
            set
            {
                this.SetValByKey(DelayAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string FlowNote
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.FlowNote);
            }
            set
            {
                this.SetValByKey(DelayAttr.FlowNote, value);
            }
        }
        /// <summary>
        /// 待办处理人
        /// </summary>
        public string TodoEmps
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.TodoEmps);
            }
            set
            {
                this.SetValByKey(DelayAttr.TodoEmps, value);
            }
        }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStringByKey(DelayAttr.Sender);
            }
            set
            {
                this.SetValByKey(DelayAttr.Sender, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
                uac.IsView = true;
                return uac;
            }
        }
        /// <summary>
        /// 逾期流程
        /// </summary>
        public Delay() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        public Delay(string pk)
            : base(pk)
        {
        }
        #endregion

        #region Map
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("V_WF_Delay", "逾期流程");
                map.EnType = En.EnType.View;

                map.AddMyPK();

                map.AddTBInt(DelayAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBString(DelayAttr.FK_Emp, null, "待办人", true, true, 0, 50, 40);
                map.AddTBInt(DelayAttr.FK_Node, 0, "节点", false, true);

                map.AddTBString(DelayAttr.Title, null, "标题", true, true, 0, 50, 100);
                map.AddTBString(DelayAttr.Starter, null, "Starter", false, false, 0, 50, 5);
                map.AddTBString(DelayAttr.StarterName, null, "发起人", true, true, 0, 50, 30);

                map.AddDDLSysEnum(DelayAttr.WFState, 0, "状态", true, true, DelayAttr.WFState);

                map.AddTBInt(DelayAttr.FK_Dept, 0, "隶属部门", false, true);
                map.AddTBString(DelayAttr.DeptName, null, "隶属部门", true, true, 0, 50, 40);
                map.AddDDLEntities(DelayAttr.FK_Flow, null, "流程", new Flows(), false);
              //  map.AddTBString(DelayAttr.FlowName, null, "流程名称", true, true, 0, 50, 40);
                map.AddTBString(DelayAttr.NodeName, null, "节点名称", true, true, 0, 50, 40);

                map.AddTBInt(DelayAttr.WorkerDept, 0, "工作人员部门", false, true);
                map.AddTBString(DelayAttr.RDT, null, "接受日期", true, true, 0, 50, 30);
                map.AddTBString(DelayAttr.SDT, null, "应完成日期", true, true, 0, 50, 50);
                map.AddTBInt(DelayAttr.FID, 0, "FID", false, false);
                map.AddTBInt(DelayAttr.FK_FlowSort, 0, "流程类别", false, true);
                map.AddTBString(DelayAttr.SysType, null, "SysType", false, true, 0, 50, 5);
               // map.AddTBString(DelayAttr.SDTOfNode, null, "节点应完成日期", true, true, 0, 50, 70);
                map.AddTBString(DelayAttr.PressTimes, null, "催办次数", false, true, 0, 50, 5);
                map.AddTBString(DelayAttr.BillNo, null, "单据号", true, true, 0, 50, 5);
                map.AddTBString(DelayAttr.FlowNote, null, "FlowNote", false, true, 0, 50, 5);
                map.AddTBString(DelayAttr.TodoEmps, null, "待办人员", true, true, 0, 50, 5);
                map.AddTBString(DelayAttr.Sender, null, "发送者", true, true, 0, 50, 50);

                //查询条件.
                map.AddSearchAttr(DelayAttr.WFState);
                map.AddSearchAttr(DelayAttr.FK_Flow);


                RefMethod rm = new En.RefMethod();
                rm.Title = "打开轨迹";
                rm.ClassMethodName = this.ToString() + ".DoOpenTrack";
                rm.RefMethodType = En.RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoOpenTrack()
        {
            return "../WFRpt.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
        }

    }
    /// <summary>
    /// 逾期流程s
    /// </summary>
    public class Delays : EntitiesMyPK
    {
        #region 构造方法属性
        /// <summary>
        /// 逾期流程s
        /// </summary>
        public Delays() { }
        #endregion

        #region 属性
        /// <summary>
        /// 逾期流程
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Delay();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Delay> ToJavaList()
        {
            return (System.Collections.Generic.IList<Delay>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Delay> Tolist()
        {
            System.Collections.Generic.List<Delay> list = new System.Collections.Generic.List<Delay>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Delay)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
