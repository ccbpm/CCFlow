using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.Demo.SDK
{
    /// <summary>
    /// 内部办文
    /// </summary>
    public class ND112RptAttr : BP.WF.Data.NDXRptBaseAttr
    {
        #region 扩展字段
        /// <summary>
        /// 秘密登记
        /// </summary>
        public const string MiMiDengJi = "MiMiDengJi";
        /// <summary>
        /// 文件标题
        /// </summary>
        public const string WenJianBiaoTi = "WenJianBiaoTi";
        /// <summary>
        /// 紧急程度
        /// </summary>
        public const string JinJiChengDu = "JinJiChengDu";
        /// <summary>
        /// 需办公厅领导审核
        /// </summary>
        public const string BanGongTing = "BanGongTing";
        /// <summary>
        /// 办公厅领导
        /// </summary>
        public const string BgtLingDao = "BgtLingDao";
        /// <summary>
        /// 常委会领导
        /// </summary>
        public const string CwhLingDao = "CwhLingDao";
        /// <summary>
        /// 来文单位类别
        /// </summary>
        public const string Port_DeptType = "Port_DeptType";
        /// <summary>
        /// 来文单位
        /// </summary>
        public const string Port_OutDept = "Port_OutDept";
        /// <summary>
        /// 来文字号
        /// </summary>
        public const string FaWenZiHao = "LaiWenZiHao";
        /// <summary>
        /// 阅办单位
        /// </summary>
        public const string FK_LEADER = "FK_LEADER";
        /// <summary>
        /// 收文日期
        /// </summary>
        public const string ShouWenRiQi = "ShouWenRiQi";
        /// <summary>
        /// 办文编号
        /// </summary>
        public const string BanWenBianHao = "BanWenBianHao";
        /// <summary>
        /// 内容摘要
        /// </summary>
        public const string nrzynbyj = "nrzynbyj";
        #endregion 扩展字段
    }
    /// <summary>
    /// 发文
    /// </summary>
    public class ND112Rpt : BP.WF.Data.NDXRptBase
    {
        #region 基本属性
        /// <summary>
        /// 秘密等级
        /// </summary>
        public string MiMiDengJi
        {
            get { return this.GetValStringByKey(ND112RptAttr.MiMiDengJi); }
            set
            {
                this.SetValByKey(ND112RptAttr.MiMiDengJi, value);
            }
        }
        /// <summary>
        /// 文件标题
        /// </summary>
        public string WenJianBiaoTi
        {
            get { return this.GetValStringByKey(ND112RptAttr.WenJianBiaoTi); }
            set
            {
                this.SetValByKey(ND112RptAttr.WenJianBiaoTi, value);
            }
        }


        /// <summary>
        /// 需办公厅领导
        /// </summary>
        public bool BanGongTing
        {
            get { return this.GetValBooleanByKey(ND112RptAttr.BanGongTing); }
            set
            {
                this.SetValByKey(ND112RptAttr.BanGongTing, value);
            }
        }
        public string BgtLingDao
        {
            get { return this.GetValStringByKey(ND112RptAttr.BgtLingDao); }
            set
            {
                this.SetValByKey(ND112RptAttr.BgtLingDao, value);
            }
        }
        public string CwhLingDao
        {
            get { return this.GetValStringByKey(ND112RptAttr.CwhLingDao); }
            set
            {
                this.SetValByKey(ND112RptAttr.CwhLingDao, value);
            }
        }

        /// <summary>
        /// 收文日期
        /// </summary>
        public bool ShouWenRiQi
        {
            get { return this.GetValBooleanByKey(ND112RptAttr.ShouWenRiQi); }
            set
            {
                this.SetValByKey(ND112RptAttr.ShouWenRiQi, value);
            }
        }
        
        public string BanWenBianHao
        {
            get { return this.GetValStringByKey(ND112RptAttr.BanWenBianHao); }
            set
            {
                this.SetValByKey(ND112RptAttr.BanWenBianHao, value);
            }
        }

        /// <summary>
        /// 需常委会领导
        /// </summary>
        public string Port_DeptType
        {
            get { return this.GetValStringByKey(ND112RptAttr.Port_DeptType); }
            set { this.SetValByKey(ND112RptAttr.Port_DeptType, value); }
        }
        public string Port_OutDept
        {
            get { return this.GetValStringByKey(ND112RptAttr.Port_OutDept); }
            set { this.SetValByKey(ND112RptAttr.Port_OutDept, value); }
        }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string JinJiChengDu
        {
            get { return this.GetValStringByKey(ND112RptAttr.JinJiChengDu); }
            set { this.SetValByKey(ND112RptAttr.JinJiChengDu, value); }
        }

        /// <summary>
        /// 发文单位
        /// </summary>
        public string FaWenZiHao
        {
            get { return this.GetValStringByKey(ND112RptAttr.FaWenZiHao); }
            set { this.SetValByKey(ND112RptAttr.FaWenZiHao, value); }
        }

        /// <summary>
        /// 阅办单位
        /// </summary>
        public string FK_LEADER
        {
            get { return this.GetValStringByKey(ND112RptAttr.FK_LEADER); }
            set
            { this.SetValByKey(ND112RptAttr.FK_LEADER, value); }
        }

        public string nrzynbyj
        {
            get { return this.GetValStringByKey(ND112RptAttr.nrzynbyj); }
            set
            { this.SetValByKey(ND112RptAttr.nrzynbyj, value); }
        }

        #endregion 基本属性

        #region 构造函数

        /// <summary>
        /// 发文
        /// </summary>
        public ND112Rpt()
        {
        }

        /// <summary>
        /// 发文
        /// </summary>
        /// <param name="workid"></param>
        public ND112Rpt(Int64 workid)
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
                Map map = new Map("ND112Rpt");
                map.EnDesc = "编号发文";
                map.EnType = EnType.App;

                #region 流程的基本字段

                map.AddTBIntPKOID();
                map.AddTBString(ND112RptAttr.Title, null, "标题", false, true, 0, 500, 10);
                map.AddTBString(ND112RptAttr.FK_Dept, null, "隶属部门", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.FK_NY, null, "年月", false, true, 0, 50, 10);
                map.AddDDLSysEnum(ND112RptAttr.WFState, 0, "状态", false, true, "WFState");

                map.AddTBInt(ND112RptAttr.FID, 0, "状态", false, true);
                map.AddTBInt(ND112RptAttr.FlowDaySpan, 0, "状态", false, true);
                map.AddTBInt(ND112RptAttr.FlowEndNode, 0, "结束点", false, true);

                map.AddTBString(ND112RptAttr.FlowEmps, null, "参与人", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.FlowEnder, null, "最后节点处理人", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.FlowEnderRDT, null, "最后处理时间", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.FlowStarter, null, "流程发起人", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.FlowStartRDT, null, "流程发起时间", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.GuestNo, null, "客户编号", false, true, 0, 50, 10);
                map.AddTBString(ND112RptAttr.GuestName, null, "客户名称", false, true, 0, 50, 10);

                map.AddTBString(ND112RptAttr.PFlowNo, null, "父流程编号", false, true, 0, 50, 10);
                map.AddTBInt(ND112RptAttr.PWorkID, 0, "父流程ID", false, true);
                map.AddTBString(ND112RptAttr.BillNo, null, "单据编号", false, true, 0, 100, 10);

                #endregion 流程的基本字段

                #region 扩展字段

                map.AddTBString(ND112RptAttr.WenJianBiaoTi, null, "文件标题", false, true, 0, 20, 10);
                map.AddDDLSysEnum(ND112RptAttr.MiMiDengJi, 0, "秘密登记", false, true, ND112RptAttr.MiMiDengJi, "@0=无@1=普通@2=秘密@3=机密@4=绝密");
                map.AddDDLSysEnum(ND112RptAttr.JinJiChengDu, 0, "紧急程度", false, true, ND112RptAttr.JinJiChengDu, "@0=平件@1=紧急");
                map.AddTBString(ND112RptAttr.FaWenZiHao, null, "来文字号", false, true, 0, 20, 10);
                map.AddDDLEntities(ND112RptAttr.Port_DeptType, null, "来文单位类别", new BP.Port.Emps(), false);
                map.AddDDLEntities(ND112RptAttr.Port_OutDept, null, "来文单位", new BP.Port.Emps(), false);
                map.AddDDLEntities(ND112RptAttr.FK_LEADER, null, "阅办单位", new BP.Port.Depts(), false);

                map.AddTBString(ND112RptAttr.ShouWenRiQi, null, "收文日期", false, true, 0, 20, 10);
                map.AddTBString(ND112RptAttr.BanWenBianHao, null, "办文编号", false, true, 0, 20, 10);
                map.AddTBString(ND112RptAttr.nrzynbyj, null, "内容摘要", false, true, 0, 20, 10);

                map.AddBoolean(ND112RptAttr.BanGongTing, false, "办公厅领导", false, true);
                map.AddDDLEntities(ND112RptAttr.BgtLingDao, null, "办公厅领导", new BP.Port.Emps(), false);
                map.AddDDLEntities(ND112RptAttr.CwhLingDao, null, "常委会领导", new BP.Port.Emps(), false);

                #endregion 扩展字段

                this._enMap = map;
                return this._enMap;
            }
        }

        #endregion 构造函数
    }
    /// <summary>
    /// 发文s BP.Port.FK.ND112Rpts
    /// </summary>
    public class ND112Rpts : BP.WF.Data.NDXRptBases
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND112Rpt();
            }
        }
        /// <summary>
        ///
        /// </summary>
        public ND112Rpts()
        {
        }

        #endregion 方法
    }
}
