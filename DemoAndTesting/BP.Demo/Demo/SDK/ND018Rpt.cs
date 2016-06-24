using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF.Data;
using BP.En;

namespace BP.Demo.SDK
{
    public class ND018RptAttr : NDXRptBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 请假人编号
        /// </summary>
        public const string QingJiaRenNo = "QingJiaRenNo";
        /// <summary>
        /// 请假人名称
        /// </summary>
        public const string QingJiaRenName = "QingJiaRenName";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string QingJiaRenDeptNo = "QingJiaRenDeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string QingJiaRenDeptName = "QingJiaRenDeptName";
        /// <summary>
        /// 请假天数
        /// </summary>
        public const string QingJiaTianShu = "QingJiaTianShu";
        /// <summary>
        /// 请假原因
        /// </summary>
        public const string QingJiaYuanYin = "QingJiaYuanYin";
        /// <summary>
        /// 部门审批意见
        /// </summary>
        public const string NoteBM = "NoteBM";
        /// <summary>
        /// 经理审批意见
        /// </summary>
        public const string NoteZJL = "NoteZJL";
        /// <summary>
        /// 人力资源意见
        /// </summary>
        public const string NoteRL = "NoteRL";
        #endregion
    }
    public class ND018Rpt : NDXRptBase
    {
        #region 属性
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string QingJiaRenDeptName
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.QingJiaRenDeptName);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaRenDeptName, value);
            }
        }
        /// <summary>
        /// 请假人编号
        /// </summary>
        public string QingJiaRenNo
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.QingJiaRenNo);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaRenNo, value);
            }
        }
        /// <summary>
        /// 请假人名称
        /// </summary>
        public string QingJiaRenName
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.QingJiaRenName);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaRenName, value);
            }
        }
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public string QingJiaRenDeptNo
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.QingJiaRenDeptNo);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaRenDeptNo, value);
            }
        }
        /// <summary>
        /// 请假原因
        /// </summary>
        public string QingJiaYuanYin
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.QingJiaYuanYin);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaYuanYin, value);
            }
        }
        /// <summary>
        /// 请假天数
        /// </summary>
        public float QingJiaTianShu
        {
            get
            {
                return this.GetValIntByKey(ND018RptAttr.QingJiaTianShu);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.QingJiaTianShu, value);
            }
        }
        /// <summary>
        /// 部门审批意见
        /// </summary>
        public string NoteBM
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.NoteBM);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.NoteBM, value);
            }
        }
        /// <summary>
        /// 总经理意见
        /// </summary>
        public string NoteZJL
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.NoteZJL);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.NoteZJL, value);
            }
        }
        /// <summary>
        /// 人力资源意见
        /// </summary>
        public string NoteRL
        {
            get
            {
                return this.GetValStringByKey(ND018RptAttr.NoteRL);
            }
            set
            {
                this.SetValByKey(ND018RptAttr.NoteRL, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 请假
        /// </summary>
        public ND018Rpt()
        { 
            
        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND018Rpt(Int64 workid)
        {
            this.OID = workid;
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

                Map map = new Map("ND18Rpt");
                map.EnDesc = "请假";
                map.EnType = EnType.App;

                #region 流程的基本字段
                map.AddTBIntPKOID();
                map.AddTBString(ND018RptAttr.Title, null, "标题", false, true, 0, 500, 10);
                map.AddTBString(ND018RptAttr.FK_Dept, null, "隶属部门", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.FK_NY, null, "年月", false, true, 0, 50, 10);
                map.AddDDLSysEnum(ND018RptAttr.WFState, 0, "状态", false, true, "WFState");

                map.AddTBInt(ND018RptAttr.FID, 0, "状态", false, true);
                map.AddTBInt(ND018RptAttr.FlowDaySpan, 0, "状态", false, true);
                map.AddTBInt(ND018RptAttr.FlowEndNode, 0, "结束点", false, true);

                map.AddTBString(ND018RptAttr.FlowEmps, null, "参与人", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.FlowEnder, null, "最后节点处理人", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.FlowEnderRDT, null, "最后处理时间", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.FlowStarter, null, "流程发起人", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.FlowStartRDT, null, "流程发起时间", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.GuestNo, null, "客户编号", false, true, 0, 50, 10);
                map.AddTBString(ND018RptAttr.GuestName, null, "客户名称", false, true, 0, 50, 10);

                map.AddTBString(ND018RptAttr.PFlowNo, null, "父流程编号", false, true, 0, 50, 10);
                map.AddTBInt(ND018RptAttr.PWorkID, 0, "父流程ID", false, true);
                map.AddTBString(ND018RptAttr.BillNo, null, "单据编号", false, true, 0, 100, 10);
                #endregion 流程的基本字段

                map.AddTBIntPKOID();
                map.AddTBString(ND018RptAttr.QingJiaRenNo, null, "请假人编号", false, false, 0, 200, 10);
                map.AddTBString(ND018RptAttr.QingJiaRenName, null, "请假人名称", true, false, 0, 200, 70);
                map.AddTBString(ND018RptAttr.QingJiaRenDeptNo, "", "请假人部门编号", true, false, 0, 200, 50);
                map.AddTBString(ND018RptAttr.QingJiaRenDeptName, null, "请假人部门名称", true, false, 0, 200, 50);
                map.AddTBString(ND018RptAttr.QingJiaYuanYin, null, "请假原因", true, false, 0, 200, 150);
                map.AddTBFloat(ND018RptAttr.QingJiaTianShu, 0, "请假天数", true, false);

                // 审核信息.
                map.AddTBString(ND018RptAttr.NoteBM, null, "部门经理意见", true, false, 0, 200, 150);
                map.AddTBString(ND018RptAttr.NoteZJL, null, "总经理意见", true, false, 0, 200, 150);
                map.AddTBString(ND018RptAttr.NoteRL, null, "人力资源意见", true, false, 0, 200, 150);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class ND018Rpts : NDXRptBases
    { 
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND018Rpt();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND018Rpts() { }
        #endregion
    }
}
