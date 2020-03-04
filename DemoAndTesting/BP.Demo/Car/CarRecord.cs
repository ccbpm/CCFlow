using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
namespace BP.OA.Car
{
    public enum ChuCheFanWei
    {
        //市内
        ShiNei,
        //市外
        ShiWai

    }

    /// <summary>
    /// 出车记录属性
    /// </summary>

    public class CarRecordAttr :EntityOIDAttr
    {
        //出车范围
        public const string ChuCheFanWei = "ChuCheFanWei";
        //用车人
        public const string YongCheRen="YongCheRen";
        //用车审批人
        public const string YongCheShenPiRen = "YongCheShenPiRen";
        //批准人
        public const string PiZhuiRen = "PiZhuiRen";
        //驾驶员
        public const string FK_JiaShiYuan = "FK_JiaShiYuan";
        //出车事由
        public const string ChuCheShiYou = "ChuCheShiYou";
        //用车开始时间
         public const string YongCheKaiShiShiJian="YongCheKaiShiShiJian";
        //用车结束时间
        public const string YongCheJieShuShiJian="YongCheJieShuShiJian";
        //出车前里程
         public const string ChuCheQianLiCheng="ChuCheQianLiCheng";
        //出车后里程
         public const string ChuCheHouLiCheng = "ChuCheHouLiCheng";
        //油耗
         public const string YouHao="YouHao";
        //用车原因
        public const string YongCheYuanYin="YongCheYuanYin";
        //备注
        public const string BeiZhu="BeiZhu";
        //外键
        public const string FK_car ="FK_car";
    }
    public class CarRecord : EntityOID
    {
        public ChuCheFanWei ChuCheFanWei
        {
            get
            {
                return (ChuCheFanWei)this.GetValIntByKey(CarRecordAttr.ChuCheFanWei);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.ChuCheFanWei, (int)value);
            }
        }


        public string YongCheRen
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.YongCheRen);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YongCheRen,value);
            }
        }
        public string YongCheShenPiRen
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.YongCheShenPiRen);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YongCheShenPiRen, value);
            }
        }

        public string PiZhuiRen
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.PiZhuiRen);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.PiZhuiRen, value);
            }
        }
        public string FK_JiaShiYuan
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.FK_JiaShiYuan);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.FK_JiaShiYuan, value);
            }
        }
       public string ChuCheShiYou
       {
           get
           {
               return this.GetValStrByKey(CarRecordAttr.ChuCheShiYou);
           }
           set
           {
               this.SetValByKey(CarRecordAttr.ChuCheShiYou, value);
           }
       }
         public DateTime YongCheKaiShiShiJian
        {
            get
            {
                return this.GetValDateTime(CarRecordAttr.YongCheKaiShiShiJian);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YongCheKaiShiShiJian,value);
            }
        }
         public DateTime YongCheJieShuShiJian
        {
            get
            {
                return this.GetValDateTime(CarRecordAttr.YongCheJieShuShiJian);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YongCheJieShuShiJian,value);
            }
        }
        public decimal ChuCheQianLiCheng
        {
            get
            {
                return this.GetValDecimalByKey(CarRecordAttr.ChuCheQianLiCheng);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.ChuCheQianLiCheng,value);
            }
        }
       public decimal ChuCheHouLiCheng
        {
            get
            {
                return this.GetValDecimalByKey(CarRecordAttr.ChuCheHouLiCheng);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.ChuCheHouLiCheng,value);
            }
        }
        public decimal YouHao
        {
            get
            {
                return this.GetValDecimalByKey(CarRecordAttr.YouHao);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YouHao,value);
            }
        }
       public string YongCheYuanYin
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.YongCheYuanYin);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.YongCheYuanYin,value);
            }
        }
        public string BeiZhu
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.BeiZhu,value);
            }
        }
        public string FK_car
        {
            get
            {
                return this.GetValStrByKey(CarRecordAttr.FK_car);
            }
            set
            {
                this.SetValByKey(CarRecordAttr.FK_car, value);
            }
        }
        #region 构造方法
        public CarRecord(){
        }
        public CarRecord(int oid) : base(oid) { }
        #endregion
         //出车记录Map
        public override Map EnMap
        {
	        get
            { 
                if(this._enMap!=null)
                return this._enMap;
                Map map=new Map("OA_CarRecord");
                map.EnDesc="出车记录";
                map.CodeStruct="3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(CarRecordAttr.FK_car, null, "车辆", new CarInfos(), true);

                map.AddTBString(CarRecordAttr.YongCheRen, null, "用车人", true, false, 2, 100, 60);
                map.AddTBString(CarRecordAttr.YongCheShenPiRen, null, "用车审批人", true, false, 2, 100, 70);
                map.AddTBString(CarRecordAttr.PiZhuiRen, null, "批准人", true, false, 2, 100, 60);
                map.AddDDLEntities(CarRecordAttr.FK_JiaShiYuan, null, "驾驶员", new Drivers(), true);
                map.AddTBString(CarRecordAttr.YongCheRen, null, "用车人", true, false, 2, 100, 60);

                map.AddTBString(CarRecordAttr.ChuCheShiYou, null, "出车事由", true, false, 5, 100, 60);
    
                map.AddDDLSysEnum(CarRecordAttr.ChuCheFanWei, 0, "出车范围", true, true, CarRecordAttr.ChuCheFanWei,
                    "@0=市内@1=市外");
                map.AddTBDateTime(CarRecordAttr.YongCheKaiShiShiJian, "用车开始时间", true, false);
                map.AddTBDateTime(CarRecordAttr.YongCheJieShuShiJian, "用车结束时间", true, false);

               
               map.AddTBDecimal(CarRecordAttr.ChuCheQianLiCheng,0,"出车前里程",true,false);
               map.AddTBDecimal(CarRecordAttr.ChuCheHouLiCheng, 0, "出车后里程", true, false);
               map.AddTBDecimal(CarRecordAttr.YouHao, 0, "油耗", true, false);

               map.AddTBStringDoc(CarRecordAttr.BeiZhu, null, "备注", true, false, true);
                 //查询条件
               map.AddSearchAttr(CarRecordAttr.FK_car);
               map.AddSearchAttr(CarRecordAttr.FK_JiaShiYuan);
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
            if (string.IsNullOrEmpty(this.ChuCheQianLiCheng+""))
            {
                throw new Exception("出车前不能为空！");
            }
            if (string.IsNullOrEmpty(this.ChuCheHouLiCheng+""))
            {
                throw new Exception("出车后不能为空！");
            }
            if (string.IsNullOrEmpty(this.YongCheRen))
            {
                throw new Exception("用车人不能为空！");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            string sql = "select count(*) from OA_CarRecord where FK_Car = " + this.FK_car;
            int temp = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
            Driver driver = new Driver();
            driver.No = this.FK_JiaShiYuan;
            driver.Retrieve();
            driver.CCCS = temp;
            driver.Update();
            base.afterInsertUpdateAction();
        }

        protected override void afterDelete()
        {
            // 更新驾驶员出车次数
            Driver driver = new Driver();
            driver.No = this.FK_JiaShiYuan;
            driver.Retrieve();
            string sql = "select count(*) from OA_CarWeiZhang where FK_JiaShiYuan = " + this.FK_JiaShiYuan;
            driver.CCCS = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
            driver.Update();
            base.afterDelete();
        }
    }
    /// <summary>
    /// 出车记录
    /// </summary>
    public class CarRecords : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public CarRecords() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarRecord();
            }
        }
    }
}
