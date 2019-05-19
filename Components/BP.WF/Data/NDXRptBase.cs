using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    ///  报表基类属性
    /// </summary>
    public class NDXRptBaseAttr
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 参与人员
        /// </summary>
        public const string FlowEmps = "FlowEmps";
        /// <summary>
        /// 紧急程度
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// Workid
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 发起年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 发起人ID
        /// </summary>
        public const string FlowStarter = "FlowStarter";
        /// <summary>
        /// 发起时间
        /// </summary>
        public const string FlowStartRDT = "FlowStartRDT";
        /// <summary>
        /// 发起人部门编号
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 流程
        /// </summary>
        public const string WFSta = "WFSta";

        /// <summary>
        /// 数量
        /// </summary>
        public const string MyNum = "MyNum";
        /// <summary>
        /// 结束人
        /// </summary>
        public const string FlowEnder = "FlowEnder";
        /// <summary>
        /// 最后处理时间
        /// </summary>
        public const string FlowEnderRDT = "FlowEnderRDT";
        /// <summary>
        /// 跨度
        /// </summary>
        public const string FlowDaySpan = "FlowDaySpan";
        /// <summary>
        /// 停留节点
        /// </summary>
        public const string FlowEndNode = "FlowEndNode";
        /// <summary>
        /// 客户编号
        /// </summary>
        public const string GuestNo = "GuestNo";
        /// <summary>
        /// 客户名称
        /// </summary>
        public const string GuestName = "GuestName";
        /// <summary>
        /// BillNo
        /// </summary>
        public const string BillNo = "BillNo";

        #region 项目相关.
        /// <summary>
        /// 项目编号
        /// </summary>
        public const string PrjNo = "PrjNo";
        /// <summary>
        /// 项目名称
        /// </summary>
        public const string PrjName = "PrjName";
        #endregion 项目相关.

        #region 父子流程属性.
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        /// <summary>
        /// 调用子流程的节点
        /// </summary>
        public const string PNodeID = "PNodeID";
        /// <summary>
        /// 吊起子流程的人
        /// </summary>
        public const string PEmp = "PEmp";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        #endregion 父子流程属性.
    }
    /// <summary>
    /// 报表基类
    /// </summary>
    abstract public class NDXRptBase : BP.En.EntityOID
    {
        #region 属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public new Int64 OID
        {
            get
            {
                return this.GetValInt64ByKey(NDXRptBaseAttr.OID);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.OID, value);
            }
        }
        /// <summary>
        /// 流程时间跨度
        /// </summary>
        public float FlowDaySpan
        {
            get
            {
                return this.GetValFloatByKey(NDXRptBaseAttr.FlowDaySpan);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowDaySpan, value);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int MyNum
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// 主流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(NDXRptBaseAttr.FID);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FID, value);
            }
        }
        /// <summary>
        /// 流程参与人员
        /// </summary>
        public string FlowEmps
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FlowEmps);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowEmps, value);
            }
        }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string GuestNo
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.GuestNo);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.GuestNo, value);
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string GuestName
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.GuestName);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.GuestName, value);
            }
        }
        /// <summary>
        /// 单据编号
        /// </summary>
        public string BillNo
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.BillNo);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.BillNo, value);
            }
        }
        /// <summary>
        /// 流程发起人
        /// </summary>
        public string FlowStarter
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FlowStarter);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowStarter, value);
            }
        }
        /// <summary>
        /// 流程发起时间
        /// </summary>
        public string FlowStartRDT
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FlowStartRDT);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowStartRDT, value);
            }
        }
        /// <summary>
        /// 流程结束者
        /// </summary>
        public string FlowEnder
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FlowEnder);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowEnder, value);
            }
        }
        /// <summary>
        /// 流程处理时间
        /// </summary>
        public string FlowEnderRDT
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FlowEnderRDT);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowEnderRDT, value);
            }
        }
        /// <summary>
        /// 停留节点
        /// </summary>
        public string FlowEndNodeText
        {
            get
            {
                Node nd = new Node(this.FlowEndNode);
                return nd.Name;
            }
        }
        /// <summary>
        /// 节点节点ID
        /// </summary>
        public int FlowEndNode
        {
            get
            {
                return this.GetValIntByKey(NDXRptBaseAttr.FlowEndNode);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FlowEndNode, value);
            }
        }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.Title);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.Title, value);
            }
        }
        /// <summary>
        /// 隶属年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 发起人部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(NDXRptBaseAttr.WFState);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 状态名称
        /// </summary>
        public string WFStateText
        {
            get
            {
                switch (this.WFState)
                {
                    case WF.WFState.Complete:
                        return "已完成";
                    case WF.WFState.Delete:
                        return "已删除";
                    default:
                        return "运行中";
                }
            }
        }
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(NDXRptBaseAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程流程编号
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// PNodeID
        /// </summary>
        public string PNodeID
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.PNodeID);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.PNodeID, value);
            }
        }
        public string PEmp
        {
            get
            {
                return this.GetValStringByKey(NDXRptBaseAttr.PEmp);
            }
            set
            {
                this.SetValByKey(NDXRptBaseAttr.PEmp, value);
            }
        }
        #endregion attrs

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        protected NDXRptBase()
        {
        }
        /// <summary>
		/// 根据OID构造实体
		/// </summary>
        /// <param name="工作ID">workid</param>
        protected NDXRptBase(int workid):base(workid)  
		{
        }
        #endregion 构造
    }
    /// <summary>
    /// 报表基类s
    /// </summary>
    abstract public class NDXRptBases : BP.En.EntitiesOID
    {
        /// <summary>
        /// 报表基类s
        /// </summary>
        public NDXRptBases()
        {
        }
    }
}
