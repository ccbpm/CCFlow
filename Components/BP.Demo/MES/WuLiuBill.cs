using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.MES
{
    /// <summary>
    /// 物流单 attr
    /// </summary>
    public class WuLiuBillAttr : EntityOIDAttr
    {
        /// <summary>
        /// workID
        /// </summary>
        public const string RefPK = "RefPK";
        public const string RDT = "RDT";
        public const string Rec = "Rec";
        public const string Worker = "Worker";
        public const string XiangTiDtls = "XiangTiDtls";

        public const string RelEmpNo = "RelEmpNo";
        public const string DTRel = "DTRel";

        public const string BeiZhu = "BeiZhu";
  
        public const string FaHuoBill = "FaHuoBill";

        public const string KuaiDiName = "KuaiDiName";

        /// <summary>
        /// 0=未处理. 1=已完成. 2=重做. 3=已验收. 4=完成待检查.
        /// </summary>
        public const string WorkSta = "WorkSta";
    }
	/// <summary>
    ///  物流单
	/// </summary>
    public class WuLiuBill : EntityOID
    {
        #region 属性.
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        #endregion 属性.

        public Int64 RefPK
        {
            get
            {
                return this.GetValInt64ByKey(WuLiuBillAttr.RefPK);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.RefPK,value);
            }
        }
        public string FaHuoBill
        {
            get
            {
                return this.GetValStringByKey(WuLiuBillAttr.FaHuoBill);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.FaHuoBill, value);
            }
        }

        public string KuaiDiName
        {
            get
            {
                return this.GetValStringByKey(WuLiuBillAttr.KuaiDiName);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.KuaiDiName, value);
            }
        }

        public string XiangTiDtls
        {
            get
            {
                return this.GetValStringByKey(WuLiuBillAttr.XiangTiDtls);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.XiangTiDtls, value);
            }
        }
        public string RelEmpNo
        {
            get
            {
                return this.GetValStringByKey(WuLiuBillAttr.RelEmpNo);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.RelEmpNo, value);
            }
        }
        public string DTRel
        {
            get
            {
                return this.GetValStringByKey(WuLiuBillAttr.DTRel);
            }
            set
            {
                this.SetValByKey(WuLiuBillAttr.DTRel, value);
            }
        }
        #region 构造方法
        /// <summary>
        /// 物流单
        /// </summary>
        public WuLiuBill()
        {
        }
        /// <summary>
        /// 物流单
        /// </summary>
        /// <param name="_No"></param>
        public WuLiuBill(int _No) : base(_No) { }
        /// <summary>
        /// 物流单Map
        /// </summary>
        public override Map  EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
              
                Map map = new Map("WuLiuBill", "物流单");

                map.AddTBIntPKOID();

                map.AddTBString(WuLiuBillAttr.RefPK, null, "关联主键", false, true, 0, 50, 50);
                map.AddTBString(WuLiuBillAttr.KuaiDiName, null, "快递公司", false, true, 0, 50, 50);
                map.AddTBString(WuLiuBillAttr.FaHuoBill, null, "发货单号", false, true, 0, 50, 50);
                map.AddTBString(WuLiuBillAttr.XiangTiDtls, null, "箱体IDs", false, true, 0,80, 80);
                map.AddTBString(WuLiuBillAttr.RelEmpNo, null, "发货人", false, true, 0, 50, 50);

                map.AddTBString(WuLiuBillAttr.DTRel, null, "发货日期", false, true, 0, 50, 50);
                 
              map.AddTBString(WuLiuBillAttr.BeiZhu, null, "备注", true, true, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

       
      
      
    }
	/// <summary>
    /// 物流单s
	/// </summary>
    public class WuLiuBills : EntitiesOID
    {
        /// <summary>
        /// 物流单s
        /// </summary>
        public WuLiuBills() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WuLiuBill();
            }
        }
    }
}
