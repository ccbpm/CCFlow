using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;
using BP.DA;

namespace BP.MES
{
    /// <summary>
    /// 电气柜 Attr
    /// </summary>
    public class ND2RptAttr : NDXRptBaseAttr
    {
        #region 基本属性
        /// <summary>
        /// 电气柜编码
        /// </summary>
        public const string BoxCode = "BoxCode";
        /// <summary>
        /// 所属项目
        /// </summary>
        public const string FK_Project = "FK_Project";
        /// <summary>
        /// 出厂编号
        /// </summary>
        public const string SerialNumber = "SerialNumber";
        /// <summary>
        /// 设备尺寸
        /// </summary>
        public const string BoxSize = "BoxSize";
        /// <summary>
        /// 生产日期
        /// </summary>
        public const string ProdDate = "ProdDate";
        /// <summary>
        /// 关联图纸编号
        /// </summary>
        public const string DwgNo = "DwgNo";
        /// <summary>
        /// 所属装配
        /// </summary>
        public const string BomCode = "BomCode";
        /// <summary>
        /// 期限限定
        /// </summary>
        public const string FinishDate = "FinishDate";
        /// <summary>
        /// 紧急程度
        /// </summary>
        public const string EmgLevel = "EmgLevel";
        /// <summary>
        /// 时限状态
        /// </summary>
        public const string TimeStatus = "TimeStatus";
        /// <summary>
        /// 生产状态
        /// </summary>
        public const string ProdStatus = "ProdStatus";
        #endregion
    }
    /// <summary>
    /// 电气柜
    /// </summary>
    public class ND2Rpt : NDXRptBase
    {
        #region 属性
        /// <summary>
        /// 电气柜编码
        /// </summary>
        public string BoxCode
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.BoxCode);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.BoxCode, value);
            }
        }
        /// <summary>
        /// 所属项目
        /// </summary>
        public string FK_Project
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.FK_Project);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.FK_Project, value);
            }
        }
        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string FK_ProjectText
        {
            get
            {
                return this.GetValRefTextByKey(ND2RptAttr.FK_Project);
            }
        }
        /// <summary>
        /// 出厂编号
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.SerialNumber);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.SerialNumber, value);
            }
        }
        /// <summary>
        /// 设备尺寸
        /// </summary>
        public string BoxSize
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.BoxSize);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.BoxSize, value);
            }
        }
        /// <summary>
        /// 关联图纸编号
        /// </summary>
        public string DwgNo
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.DwgNo);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.DwgNo, value);
            }
        }
        /// <summary>
        /// 所属装配
        /// </summary>
        public string BomCode
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.BomCode);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.BomCode, value);
            }
        }
        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProdDate
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.ProdDate);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.ProdDate, value);
            }
        }
        /// <summary>
        /// 期限限定
        /// </summary>
        public string FinishDate
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.FinishDate);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.FinishDate, value);
            }
        }
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string EmgLevel
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.EmgLevel);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.EmgLevel, value);
            }
        }
        /// <summary>
        /// 时限状态
        /// </summary>
        public string TimeStatus
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.TimeStatus);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.TimeStatus, value);
            }
        }
        /// <summary>
        /// 生产状态
        /// </summary>
        public string ProdStatus
        {
            get
            {
                return this.GetValStringByKey(ND2RptAttr.ProdStatus);
            }
            set
            {
                this.SetValByKey(ND2RptAttr.ProdStatus, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 电气柜
        /// </summary>
        public ND2Rpt()
        {

        }
        /// <summary>
        /// 电气柜
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ND2Rpt(Int64 workid)
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
                //数据表映射配置
                Map map = new Map("ND2Rpt", "电气柜生产装配信息");

                #region 流程的基本字段
                map.AddTBIntPKOID();
                map.AddTBString(ND2RptAttr.Title, String.Empty, "标题", true, true, 0, 500, 10);
                map.AddTBString(ND2RptAttr.BillNo, String.Empty, "编号", true, true, 0, 50, 10);
                map.AddTBString(ND2RptAttr.BoxCode, String.Empty, "电气柜编号", true, false, 1, 50, 50);
                map.AddTBString(ND2RptAttr.SerialNumber, String.Empty, "出厂编号", true, false, 0, 30, 100);
                map.AddTBString(ND2RptAttr.BoxSize, String.Empty, "设备尺寸", true, false, 0, 100, 100);
                map.AddTBString(ND2RptAttr.ProdDate, String.Empty, "生产日期", true, false, 0, 20, 50);
                map.AddTBString(ND2RptAttr.BomCode, String.Empty, "所属装配", true, false, 0, 10, 50);
                map.AddTBString(ND2RptAttr.DwgNo, String.Empty, "关联图纸", true, false, 0, 100, 50);
                //日期字段
                map.AddTBDate(ND2RptAttr.FinishDate, String.Empty, "期限限定", true, false);
                //外键字段
                map.AddDDLEntities(ND2RptAttr.FK_Project, String.Empty, "所属项目", new Projects(), true);
                //枚举字段
                map.AddDDLSysEnum(ND2RptAttr.EmgLevel, 0, "紧急程度", true, true, ND2RptAttr.EmgLevel, "@0=一般@1=紧急");
                map.AddDDLSysEnum(ND2RptAttr.TimeStatus, 0, "时限状态", true, false, ND2RptAttr.TimeStatus, "@0=未开始@1=正常@2=逾期");
                map.AddDDLSysEnum(ND2RptAttr.ProdStatus, 0, "生产状态", true, false, ND2RptAttr.ProdStatus, "@0=未开始@1=生产中@2=已挂起@3=已完成@4=已终止");
                map.AddTBInt(ND2RptAttr.FlowEndNode, 0, "订单状态(运行的节点)", false, true);
                #endregion 流程的基本字段

                //箱体信息.
                //map.AddDtl(new ND201Dtl1s(), ND201Dtl1Attr.RefPK);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 电气柜s
    /// </summary>
    public class ND2Rpts : NDXRptBases
    {

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND2Rpt();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public ND2Rpts() { }
        #endregion
    }
}
