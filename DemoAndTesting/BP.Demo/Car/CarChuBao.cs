using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
namespace BP.OA.Car
{
    public enum BaoXianGongSi2
    {   //中国平安
        ZhongGuoPingAn,
        //太平洋保险
        TaiPingYangBaoXian
    }
    
    /// <summary>
    /// 出险登记与查询属性
    /// </summary>

    public class CarChuBaoAttr : EntityOIDAttr
    {
        //保险公司
        public const string BaoXianGongSi2 = "BaoXianGongSi2";
        //理赔金额
        public const string LiPeiJE = "LiPeiJE";
        //理赔时间
        public const string LiPeiDate = "LiPeiDate";
       //出险时间
        public const string ChuXianDate = "ChuXianDate";
        //处理状态
        public const string ChuLiZhuangTai = "ChuLiZhuangTai";
        //经办人
        public const string JinBanRen = "JinBanRen";
        //责任人
        public const string ZeRenRen = "ZeRenRen";
        //责任方
        public const string ZeRenFang = "ZeRenFang";
        //事故地点
        public const string ShiGuDiDian = "ShiGuDiDian";
        //事故日期
        public const string ShiGuDate = "ShiGuDate";
        //备注
        public const string BeiZhu = "BeiZhu";
        //车外键
        public const string FK_car = "FK_car";
        //驾驶员外键
        public const string FK_JiaShiYuan = "FK_JiaShiYuan";
    }
    public class CarChuBao : EntityOID
    {
        public BaoXianGongSi2 BaoXianGongSi2
        {
            get
            {
                return (BaoXianGongSi2)this.GetValIntByKey(CarChuBaoAttr.BaoXianGongSi2);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.BaoXianGongSi2, value);
            }
        }

        public DateTime LiPeiDate
        {
            get
            {
                return this.GetValDateTime(CarChuBaoAttr.LiPeiDate);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.LiPeiDate, value);
            }
        }
        public DateTime ChuXianDate
        {
            get
            {
                return this.GetValDateTime(CarChuBaoAttr.ChuXianDate);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ChuXianDate, value);
            }
        }
        public DateTime ShiGuDate
        {
            get
            {
                return this.GetValDateTime(CarChuBaoAttr.ShiGuDate);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ShiGuDate, value);
            }
        }
        public decimal LiPeiJE
        {
            get
            {
                return this.GetValDecimalByKey(CarChuBaoAttr.LiPeiJE);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.LiPeiJE, value);
            }
        }
        public string JinBanRen
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.JinBanRen);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.JinBanRen, value);
            }
        }
        public string ShiGuDiDian
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.ShiGuDiDian);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ShiGuDiDian, value);
            }
        }
        public string ZeRenRen
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.ZeRenRen);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ZeRenRen, value);
            }
        }
        public string ZeRenFang
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.ZeRenFang);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ZeRenFang, value);
            }
        }

        public string ChuLiZhuangTai
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.ChuLiZhuangTai);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.ChuLiZhuangTai, value);
            }
        }

        public string BeiZhu
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.BeiZhu, value);
            }
        }
        public string FK_car
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.FK_car);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.FK_car, value);
            }
        }

        public string FK_JiaShiYuan
        {
            get
            {
                return this.GetValStrByKey(CarChuBaoAttr.FK_JiaShiYuan);
            }
            set
            {
                this.SetValByKey(CarChuBaoAttr.FK_JiaShiYuan, value);
            }
        }
        #region
        //构造方法
        public CarChuBao()
        {
        }
        public CarChuBao(int oid) : base(oid) { }
        #endregion
        //出车记录Map
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("OA_CarChuBao");
                map.EnDesc = "车辆出保与车辆事故";
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(CarChuBaoAttr.FK_car, null, "汽车", new CarInfos(), true);

                map.AddDDLSysEnum(CarChuBaoAttr.BaoXianGongSi2, 0, "保险公司", true, true, CarChuBaoAttr.BaoXianGongSi2, "@0=中国平安@1=太平洋保险");
               
                map.AddTBDecimal(CarChuBaoAttr.LiPeiJE, 0, "理赔金额", true, false);
                map.AddTBDate(CarChuBaoAttr.LiPeiDate, "理赔时间", true, false);
                // map.AddTBDateTime(CarChuBaoAttr.LiPeiDate, "理赔时间", true, false);
                map.AddTBString(CarChuBaoAttr.ShiGuDiDian, null, "事故地点", true, false, 2, 100, 60);
                map.AddTBDate(CarChuBaoAttr.ShiGuDate, "事故日期", true, false);
                map.AddDDLEntities(CarChuBaoAttr.FK_JiaShiYuan, null, "驾驶员", new Drivers(), true);
                map.AddTBString(CarChuBaoAttr.JinBanRen, null, "经办人", true, false, 2, 100, 60);
                map.AddTBString(CarChuBaoAttr.ZeRenRen, null, "责任人", true, false, 2, 100, 60);
                map.AddTBString(CarChuBaoAttr.ZeRenFang, null, "责任方", true, false, 2, 100, 60);
                map.AddTBString(CarChuBaoAttr.ChuLiZhuangTai, null, "处理状态", true, false, 1, 100, 60);

                
                map.AddTBStringDoc(CarChuBaoAttr.BeiZhu, null, "备注", true, false, true);
                //查询条件
                map.AddSearchAttr(CarChuBaoAttr.FK_car);

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_car))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty((int)this.BaoXianGongSi2+""))
            {
                throw new Exception("保险公司不能为空！");
            }
            if (string.IsNullOrEmpty(this.LiPeiJE+""))
            {
                throw new Exception("理赔金额不能为空！");
            }
            if (this.LiPeiDate == null)
            {
                throw new Exception("理赔日期不能为空！");
            }
            if (string.IsNullOrEmpty(this.JinBanRen))
            {
                throw new Exception("车牌号不能为空！");
            } 
            return base.beforeUpdateInsertAction();
        }
        //此处排除出现费用对费用综合分析的影响，有点异议
        protected override void afterInsertUpdateAction()
        {
            // 更新费用记录
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_car;
            feiyong.JingBanRen = this.JinBanRen;
            feiyong.FeiYongJE = this.LiPeiJE;
            feiyong.FeiYongRiQi = this.LiPeiDate.ToString("yyyy-MM-dd");
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.ChuXian;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 出车记录
    /// </summary>
    public class CarChuBaos : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public CarChuBaos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarChuBao();
            }
        }
    }
}
