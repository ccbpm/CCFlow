using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: FullMoney.cs
   Author:周鹏、孙浚顺
   Date:2014-05-18
   Description:充值类
   ***********************************************************/
namespace BP.OA.Car
{
    /// <summary>
    /// 充值卡类型
    /// </summary>
    public enum FullMoneyCarCardType
    {
        /// <summary>
        /// 停车卡
        /// </summary>
        STOP,
        /// <summary>
        /// 油卡
        /// </summary>
        OIL,
        /// <summary>
        /// ETC充值卡
        /// </summary>
        ETC,
    }
    /// <summary>
    /// 充值台帐 属性
    /// </summary>
    public class FullMoneyAttr : EntityOIDAttr
    {
        /// <summary>
        /// 充值人
        /// </summary>
        public const string JBR = "JBR";
        /// <summary>
        /// 充值时间
        /// </summary>
        public const string CZRQ = "CZRQ";
        /// <summary>
        /// 卡号
        /// </summary>
        public const string CardNo = "CardNo";
        /// <summary>
        /// 合作单位
        /// </summary>
        public const string HZDW = "HZDW";
        /// <summary>
        /// 办理地点
        /// </summary>
        public const string BLDD = "BLDD";
        /// <summary>
        /// 填写日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 充值公司
        /// </summary>
        public const string WXGS = "WXGS";
        /// <summary>
        /// 关联的卡
        /// </summary>
        public const string FK_Card = "FK_Card";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 类型
        /// </summary>
        public const string CarCardType = "CarCardType";
        /// <summary>
        /// 汽车
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// 充值金额
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// 充值前金额
        /// </summary>
        public const string CZQJE = "CZQJE";
        /// <summary>
        /// 充值公里数
        /// </summary>
        public const string CZGLS = "CZGLS";
    }
    /// <summary>
    ///  充值台帐
    /// </summary>
    public class FullMoney : EntityOID
    {
        #region 属性
        public string FK_NY
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_NY); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_NY, value);
            }
        }
        public string FK_Car
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_Car); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_Car, value);
            }
        }
        public string FK_Card
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.FK_Card); }

            set
            {
                this.SetValByKey(FullMoneyAttr.FK_Card, value);
            }
        }
        public string CZRQ
        {
            get
            { return this.GetValStrByKey(FullMoneyAttr.CZRQ); }

            set
            {
                this.SetValByKey(FullMoneyAttr.CZRQ, value);
            }
        }
        public float JE
        {
            get
            { return this.GetValFloatByKey(FullMoneyAttr.JE); }

            set
            {
                this.SetValByKey(FullMoneyAttr.JE, value);
            }
        }
        public string JBR 
        {
            get { return this.GetValStringByKey(FullMoneyAttr.JBR); }
            set { this.SetValByKey(FullMoneyAttr.JBR,value); }
        }
        #endregion 属性

        #region 权限控制属性.
        #endregion 权限控制属性.

        #region 构造方法
        /// <summary>
        /// 充值台帐
        /// </summary>
        public FullMoney()
        {
        }
        /// <summary>
        /// 充值台帐
        /// </summary>
        /// <param name="oid"></param>
        public FullMoney(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// 充值台帐Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarFullMoney");
                map.EnDesc = "充值台帐";
                map.CodeStruct = "9";

                map.DepositaryOfEntity = Depositary.None;

                //增加OID主键字段。
                map.AddTBIntPKOID();

                //map.AddDDLEntities(FullMoneyAttr.FK_Card, null, "卡", new Cards(),false);
                //map.AddDDLEntities(FullMoneyAttr.FK_NY, null, "隶属年月", new BP.Pub.NYs(), false);

                map.AddDDLEntities(FullMoneyAttr.FK_Car, null, "车辆", new CarInfos(), true);
                map.AddDDLSysEnum(FullMoneyAttr.CarCardType, 0, "类型", true, true, FullMoneyAttr.CarCardType,
                 "@0=停车卡@1=油卡@2=Etc卡");
                map.AddTBString(FullMoneyAttr.CardNo, null, "卡号", true, false, 2, 100, 30);
                map.AddTBString(FullMoneyAttr.HZDW, null, "合作单位", true, false, 2, 100, 30);
                map.AddTBString(FullMoneyAttr.BLDD, null, "办理地点", true, false, 2, 100, 30);
                map.AddTBMoney(FullMoneyAttr.CZQJE, 0, "充值前余额", true, false);
                map.AddTBMoney(FullMoneyAttr.JE, 0, "费用金额（元）", true, false);

                map.AddTBDate(FullMoneyAttr.CZRQ, null, "充值日期", true, false);
                map.AddTBInt(FullMoneyAttr.CZGLS, 0, "充值公里数", true, false);
                map.AddTBString(FullMoneyAttr.JBR, null, "经办人", true, false, 2, 100, 30, false);
                map.AddTBStringDoc(CarInfoAttr.Note, null, "备注", true, false, true);

                //查询条件.
                //map.AddSearchAttr(FullMoneyAttr.FK_NY);
                //map.AddSearchAttr(FullMoneyAttr.FK_Card);
                map.AddSearchAttr(FullMoneyAttr.CarCardType);
                map.AddSearchAttr(FullMoneyAttr.FK_Car);
                this._enMap = map;
                return this._enMap;
            }
        }

        #region 重写方法
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// 让其自动更新维护费.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_Car))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.CZRQ))
            {
                throw new Exception("充值日期不能为空！");
            }
            if (string.IsNullOrEmpty(this.JE+""))
            {
                throw new Exception("费用不能为空！");
            }
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            // 更新费用记录
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_Car;
            feiyong.JingBanRen = this.JBR;
            feiyong.FeiYongJE = Decimal.Parse(this.JE+"");
            feiyong.FeiYongRiQi = this.CZRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.FullMoney;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 充值台帐
    /// </summary>
    public class FullMoneys : BP.En.EntitiesOID
    {
        /// <summary>
        /// 充值台帐s
        /// </summary>
        public FullMoneys() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FullMoney();
            }
        }
    }
}
