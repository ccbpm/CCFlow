using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
namespace BP.OA.Car
{
    public enum BaoXianGongSi
    {   //中国平安
       ZhongGuoPingAn,
        //太平洋保险
        TaiPingYangBaoXian
    }
    public enum BaoXianXianZhong
    {
        //交强险
        JiaoQiangXian,
        //车辆损失险
        CheLiangSunShiXian,
        //车上人员责任险
        CheShangRenYuanZeRenXian
    }
    /// <summary>
    /// 入险登记与查询属性
    /// </summary>

    public class CarRuBaoAttr : EntityOIDAttr
    {
        //保险公司
        public const string BaoXianGongSi = "BaoXianGongSi";
        //保险险种
        public const string BaoXianXianZhong = "BaoXianXianZhong";
        //保险金额
        public const string BaoXiaoJE = "BaoXiaoJE";
        //保险生效日
        public const string BaoXianShengXiaoDate = "BaoXianShengXiaoDate";

        //保险到期日
        public const string BaoXianDaoQiDate = "BaoXianDaoQiDate";
        //办理日期
        public const string BanLiDate = "BanLiDate";
        //经办人
        public const string JinBanRen = "JinBanRen";
        //责任人
        public const string ZeRenRen = "ZeRenRen";
        //备注
        public const string BeiZhu = "BeiZhu";
        //外键
        public const string FK_car = "FK_car";
    }
    public class CarRuBao : EntityOID
    {

        public BaoXianGongSi BaoXianGongSi
        {
            get
            {
                return (BaoXianGongSi)this.GetValIntByKey(CarRuBaoAttr.BaoXianGongSi);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BaoXianGongSi, value);
            }
        }
        public BaoXianXianZhong BaoXianXianZhong
        {
            get
            {
                return (BaoXianXianZhong)this.GetValIntByKey(CarRuBaoAttr.BaoXianXianZhong);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BaoXianXianZhong, value);
            }
        }
       


        public DateTime BaoXianShengXiaoDate
        {
            get
            {
                return this.GetValDateTime(CarRuBaoAttr.BaoXianShengXiaoDate);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BaoXianShengXiaoDate, value);
            }
        }
        public DateTime BaoXianDaoQiDate
        {
            get
            {
                return this.GetValDateTime(CarRuBaoAttr.BaoXianDaoQiDate);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BaoXianDaoQiDate, value);
            }
        }
         public DateTime BanLiDate
        {
            get
            {
                return this.GetValDateTime(CarRuBaoAttr.BanLiDate);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BanLiDate, value);
            }
        }
        public decimal BaoXiaoJE
        {
            get
            {
                return this.GetValDecimalByKey(CarRuBaoAttr.BaoXiaoJE);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BaoXiaoJE, value);
            }
        }
        public string JinBanRen
        {
            get
            {
                return this.GetValStrByKey(CarRuBaoAttr.JinBanRen);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.JinBanRen, value);
            }
        }
        public string ZeRenRen
        {
            get
            {
                return this.GetValStrByKey(CarRuBaoAttr.ZeRenRen);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.ZeRenRen, value);
            }
        }
        public string BeiZhu
        {
            get
            {
                return this.GetValStrByKey(CarRuBaoAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.BeiZhu, value);
            }
        }
        public string FK_car
        {
            get
            {
                return this.GetValStrByKey(CarRuBaoAttr.FK_car);
            }
            set
            {
                this.SetValByKey(CarRuBaoAttr.FK_car, value);
            }
        }
        #region
        //构造方法
        public CarRuBao()
        {
        }
        public CarRuBao(int oid) : base(oid) { }
        #endregion
        //出车记录Map
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("OA_CarRuBao");
                map.EnDesc = "车辆入保";
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(CarRuBaoAttr.FK_car, null, "汽车", new CarInfos(), true);

                map.AddDDLSysEnum(CarRuBaoAttr.BaoXianGongSi, 0, "保险公司", true, true, CarRuBaoAttr.BaoXianGongSi, "@0=中国平安@1=太平洋保险");
                map.AddDDLSysEnum(CarRuBaoAttr.BaoXianXianZhong, 0, "保险险种", true, true, CarRuBaoAttr.BaoXianXianZhong, "@0=交强险@1=车辆损失险@2=车上人员责任险");
                map.AddTBDecimal(CarRuBaoAttr.BaoXiaoJE, 0, "保险金额", true, false);
                //map.AddTBDate(CarRuBaoAttr.BaoXianShengXiaoDate, "保险生效日", true, false);
                map.AddTBDate(CarRuBaoAttr.BaoXianShengXiaoDate, "保险生效日", true, false);
                map.AddTBDate(CarRuBaoAttr.BaoXianDaoQiDate, "保险到期日", true, false);
                map.AddTBDate(CarRuBaoAttr.BanLiDate, "办理日期", true, false);

                map.AddTBString(CarRuBaoAttr.JinBanRen, null, "经办人", true, false, 2, 100, 60);
                map.AddTBString(CarRuBaoAttr.ZeRenRen, null, "责任人", true, false, 2, 100, 60);
                map.AddTBStringDoc(CarRuBaoAttr.BeiZhu, null, "备注", true, false, true);
                //查询条件
                map.AddSearchAttr(CarRuBaoAttr.FK_car);

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
            if (string.IsNullOrEmpty((int)this.BaoXianGongSi+""))
            {
                throw new Exception("保险公司不能为空！");
            }
            if (string.IsNullOrEmpty((int)this.BaoXianXianZhong+""))
            {
                throw new Exception("保险险种不能为空！");
            } 
            if (string.IsNullOrEmpty(this.BaoXiaoJE+""))
            {
                throw new Exception("保险金额不能为空！");
            } 
            if (this.BaoXianShengXiaoDate == null)
            {
                throw new Exception("保险生效日期不能为空！");
            } 
            if (this.BanLiDate == null)
            {
                throw new Exception("办理日期不能为空！");
            } 
            if (string.IsNullOrEmpty(this.JinBanRen))
            {
                throw new Exception("经办人不能为空！");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            // 更新保险到期日
            CarInfo car = new CarInfo();
            car.No = this.FK_car;
            car.Retrieve();
            car.BXDQR = this.BaoXianDaoQiDate.ToString("yyyy-MM-dd");
            car.Update();
            // 更新费用记录
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_car;
            feiyong.JingBanRen = this.JinBanRen;
            feiyong.FeiYongJE = this.BaoXiaoJE;
            feiyong.FeiYongRiQi = this.BanLiDate.ToString("yyyy-MM-dd");
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.RuBao;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 出车记录
    /// </summary>
    public class CarRuBaos : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public CarRuBaos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarRuBao();
            }
        }
    }
}
