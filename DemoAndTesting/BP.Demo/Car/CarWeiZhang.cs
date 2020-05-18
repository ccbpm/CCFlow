using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
namespace BP.OA.Car
{

    /// <summary>
    /// 出车记录属性
    /// </summary>

    public class CarWeiZhangAttr : EntityOIDAttr
    {
        //记录单号
        public const string JiLuDanHao = "JiLuDanHao";
        //驾驶员
        public const string FK_JiaShiYuan = "FK_JiaShiYuan";
        //责任人
        public const string ZeRenRen = "ZeRenRen";

        //违章类型
        public const string WeiZhangLeiXing = "WeiZhangLeiXing";
        //违章位置
        public const string WeiZhangWeiZhi = "WeiZhangWeiZhi";
        //扣分
        public const string KouFen = "KouFen";
        //罚款
        public const string FaKuan = "FaKuan";
        //违章时间
        public const string WenZhangShiJian = "WenZhangShiJian";
        //备注
        public const string BeiZhu = "BeiZhu";
        //外键
        public const string FK_car = "FK_car";
    }

    public class CarWeiZhang : EntityOID
    {
        #region 属性
        public string JiLuDanHao
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.JiLuDanHao);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.JiLuDanHao, value);
            }
        }
        public string WenZhangShiJian
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.WenZhangShiJian);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.WenZhangShiJian, value);
            }
        }
        public string FK_JiaShiYuan
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.FK_JiaShiYuan);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.FK_JiaShiYuan, value);
            }
        }
        public string ZeRenRen
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.ZeRenRen);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.ZeRenRen, value);
            }
        }

        public string WeiZhangLeiXing
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.WeiZhangLeiXing);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.WeiZhangLeiXing, value);
            }
        }
        public string WeiZhangWeiZhi
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.WeiZhangWeiZhi);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.WeiZhangWeiZhi, value);
            }
        }
        public decimal KouFen
        {
            get
            {
                return this.GetValDecimalByKey(CarWeiZhangAttr.KouFen);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.KouFen, value);
            }
        }
        public decimal FaKuan
        {
            get
            {
                return this.GetValDecimalByKey(CarWeiZhangAttr.FaKuan);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.FaKuan, value);
            }
        }
        public string BeiZhu
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.BeiZhu, value);
            }
        }
        public string FK_car
        {
            get
            {
                return this.GetValStrByKey(CarWeiZhangAttr.FK_car);
            }
            set
            {
                this.SetValByKey(CarWeiZhangAttr.FK_car, value);
            }
        }
        #endregion
        #region
        //构造方法
        public CarWeiZhang()
        {
        }
        public CarWeiZhang(int oid) : base(oid) { }
        #endregion
        //出车记录Map
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("OA_CarWeiZhang");
                map.EnDesc = "车辆违章查询";
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(CarWeiZhangAttr.FK_car, null, "车辆", new CarInfos(), true);
                map.AddTBString(CarWeiZhangAttr.JiLuDanHao, null, "记录单号", true, false, 2, 100, 60);
                map.AddDDLEntities(CarRecordAttr.FK_JiaShiYuan, null, "驾驶员", new Drivers(), true);
                map.AddTBString(CarWeiZhangAttr.ZeRenRen, null, "责任人", true, false, 2, 100, 60);
                map.AddTBString(CarWeiZhangAttr.WeiZhangLeiXing, null, "违章类型", true, false, 2, 100, 70);
                map.AddTBString(CarWeiZhangAttr.WeiZhangWeiZhi, null, "违章位置", true, false, 2, 100, 70);

                map.AddTBDecimal(CarWeiZhangAttr.KouFen, 0, "扣分", true, false);
                map.AddTBDecimal(CarWeiZhangAttr.FaKuan, 0, "罚款", true, false);

                map.AddTBDateTime(CarWeiZhangAttr.WenZhangShiJian, "违章时间", true, false);
                map.AddTBStringDoc(CarWeiZhangAttr.BeiZhu, null, "备注", true, false, true);
                //查询条件
                map.AddSearchAttr(CarWeiZhangAttr.FK_car);
                map.AddSearchAttr(CarWeiZhangAttr.FK_JiaShiYuan);
                this._enMap = map;
                return this._enMap;
            }
        }
        protected override void afterDelete()
        {
            // 减少违章次数
            // 减少违章费用
            Driver driver = new Driver();
            driver.No = this.FK_JiaShiYuan;
            driver.Retrieve();
            string sql = "select SUM(FaKuan) from OA_CarWeiZhang where FK_JiaShiYuan = " + this.FK_JiaShiYuan;
            driver.FKZJE = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
            sql = "select count(*) from OA_CarWeiZhang where FK_JiaShiYuan = " + this.FK_JiaShiYuan;
            driver.WZCS = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
            driver.Update();
            base.afterDelete();
        }
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_car))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.FK_JiaShiYuan))
            {
                throw new Exception("驾驶员不能为空！");
            }
            if (string.IsNullOrEmpty(this.WeiZhangLeiXing))
            {
                throw new Exception("违章类型不能为空！");
            }
            if (string.IsNullOrEmpty(this.WeiZhangWeiZhi))
            {
                throw new Exception("违章位置不能为空！");
            }
            if (string.IsNullOrEmpty(this.KouFen + ""))
            {
                throw new Exception("扣分不能为空！");
            }
            if (string.IsNullOrEmpty(this.FaKuan + ""))
            {
                throw new Exception("罚款不能为空！");
            }
            if (string.IsNullOrEmpty(this.WenZhangShiJian))
            {
                throw new Exception("罚款不能为空！");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            // 修改违章费用合计
            Driver driver = new Driver();
            driver.No = this.FK_JiaShiYuan;
            driver.Retrieve();
            string sql = "select SUM(FaKuan) from OA_CarWeiZhang where FK_JiaShiYuan = " + this.FK_JiaShiYuan;
            driver.FKZJE = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
            sql = "select count(*) from OA_CarWeiZhang where FK_JiaShiYuan = " + this.FK_JiaShiYuan;
            driver.WZCS = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
            driver.Update();
            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 出车记录
    /// </summary>
    public class CarWeiZhangs : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public CarWeiZhangs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarWeiZhang();
            }
        }
    }
}
