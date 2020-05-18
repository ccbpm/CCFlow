using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
namespace BP.OA.Car
{

    public enum FeiYongMingCheng { 
        // 保养
        BaoYang,
        // 出险
        ChuXian,
        // 入保
        RuBao,
        // 充值
        FullMoney,
        // 年检
        NianJian,
        // 维修
        WeiXiu

    }
    /// <summary>
    /// 费用登记与查询属性
    /// </summary>

    public class CarFeiYongGuanLiAttr : EntityOIDAttr
    {
        //经办人
        public const string JingBanRen = "JingBanRen";
        //费用名称
        public const string FeiYongMingCheng = "FeiYongMingCheng";
        //报销日期
        public const string FeiYongRiQi = "FeiYongRiQi";
        //费用金额
        public const string FeiYongJE = "FeiYongJE";
        //所属年月
        public const string SuoShuNY = "SuoShuNY";
        //备注
        public const string BeiZhu = "BeiZhu";
        //外键
        public const string FK_car = "FK_car";
        //自定义外键符合
        //规则：BY/CB/RB/CZ/NJ/WH+"_"+ID，即：保养、出险、入保、充值、年检、维修的简写+下划线+各自表内的ID
        public const string MyFK = "MyFK";
    }
    public class CarFeiYongGuanLi : EntityOID
    {
        #region 属性
        public string MyFK
        {
            get { return  this.GetValStringByKey(CarFeiYongGuanLiAttr.MyFK); }
            set { this.SetValByKey(CarFeiYongGuanLiAttr.MyFK,value); }
        }
        public string JingBanRen
        {
            get
            {
                return this.GetValStrByKey(CarFeiYongGuanLiAttr.JingBanRen);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.JingBanRen, value);
            }
        }
        public string YongCheShenPiRen
        {
            get
            {
                return this.GetValStrByKey(CarFeiYongGuanLiAttr.FeiYongMingCheng);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.FeiYongMingCheng, value);
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
                this.SetValByKey(CarRecordAttr.YongCheKaiShiShiJian, value);
            }
        }
        public string FeiYongRiQi
        {
            get
            {
                return this.GetValStringByKey(CarFeiYongGuanLiAttr.FeiYongRiQi);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.FeiYongRiQi, value);
            }
        }
        public decimal FeiYongJE
        {
            get
            {
                return this.GetValDecimalByKey(CarFeiYongGuanLiAttr.FeiYongJE);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.FeiYongJE, value);
            }
        }
       
        public string BeiZhu
        {
            get
            {
                return this.GetValStrByKey(CarFeiYongGuanLiAttr.BeiZhu);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.BeiZhu, value);
            }
        }
        public string FK_car
        {
            get
            {
                return this.GetValStrByKey(CarFeiYongGuanLiAttr.FK_car);
            }
            set
            {
                this.SetValByKey(CarFeiYongGuanLiAttr.FK_car, value);
            }
        }
        public string SuoShuNY
        {
            get { return this.GetValStringByKey(CarFeiYongGuanLiAttr.SuoShuNY); }
            set { this.SetValByKey(CarFeiYongGuanLiAttr.SuoShuNY,value); }
        }
        public string SuoShuNYText {
            get { return this.GetValRefTextByKey(CarFeiYongGuanLiAttr.SuoShuNY); }
        }
        public int FeiYongMingCheng
        {
            get { return this.GetValIntByKey(CarFeiYongGuanLiAttr.FeiYongMingCheng); }
            set { this.SetValByKey(CarFeiYongGuanLiAttr.FeiYongMingCheng,value); }
        }
        public string FeiYongMingChengText
        {
            get { return this.GetValRefTextByKey(CarFeiYongGuanLiAttr.FeiYongMingCheng); }
        }
        #endregion 属性
        #region 权限控制属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No == "admin")
                {
                    uac.IsDelete = false;
                    uac.IsInsert = false;
                    uac.IsUpdate = false;
                }
                return uac;
            }
        }
        #endregion
        #region 构造方法
        public CarFeiYongGuanLi()
        {
        }
        public CarFeiYongGuanLi(int oid) : base(oid) { }
        #endregion
        //出车记录Map
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("OA_CarFeiYongGuanLi");
                map.EnDesc = "费用管理";
                map.CodeStruct = "3";
               // map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(CarFeiYongGuanLiAttr.FK_car, null, "车辆", new CarInfos(), true);

                map.AddTBString(CarFeiYongGuanLiAttr.JingBanRen, null, "经办人", true, false, 1, 100, 60);
                //map.AddDDLSysEnum(CarInfoAttr.CLZT, 0, "车辆状态", true, false, CarInfoAttr.CLZT, "@0=闲置中@1=出车中");
                map.AddDDLSysEnum(CarFeiYongGuanLiAttr.FeiYongMingCheng, 0, "费用名称", true, false,CarFeiYongGuanLiAttr.FeiYongMingCheng,"@0=保养@1=出险@2=入保@3=充值@4=年检@5=维修");
                map.AddTBDecimal(CarFeiYongGuanLiAttr.FeiYongJE, 0, "金额", true, false);
                map.AddTBDate(CarFeiYongGuanLiAttr.FeiYongRiQi, "日期", true, false);
                map.AddDDLEntities(CarFeiYongGuanLiAttr.SuoShuNY,null,"所属年月",new BP.Pub.NYs(),false);
                
                map.AddTBStringDoc(CarFeiYongGuanLiAttr.BeiZhu, null, "备注", true, false, true);

                map.AddTBString(CarFeiYongGuanLiAttr.MyFK,null,"自定义外键复合",false,true,0,100,30);
                //查询条件
                map.AddSearchAttr(CarFeiYongGuanLiAttr.FK_car);
                map.AddSearchAttr(CarFeiYongGuanLiAttr.SuoShuNY);
                map.AddSearchAttr(CarFeiYongGuanLiAttr.FeiYongMingCheng);
                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }

    }
    /// <summary>
    /// 出车记录
    /// </summary>
    public class CarFeiYongGuanLis : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public CarFeiYongGuanLis() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarFeiYongGuanLi();
            }
        }
    }
}
