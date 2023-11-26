using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: BaoYang.cs
   Author:孙浚顺
   Date:2014-05-18
   Description:车辆保养类
   ***********************************************************/
namespace BP.OA.Car
{
    /// <summary>
    /// 保养管理 属性
    /// </summary>
    public class BaoYangAttr : EntityOIDAttr
    {
        /// <summary>
        /// 保养单号
        /// </summary>
        public const string BYDH = "BYDH";
        /// <summary>
        /// 保养日期
        /// </summary>
        public const string BYRQ = "BYRQ";
        /// <summary>
        /// 下次保养日期
        /// </summary>
        public const string XCBYRQ = "XCBYRQ";
        /// <summary>
        /// 保养金额
        /// </summary>
        public const string BYJE = "BYJE";
        /// <summary>
        /// 保养公司
        /// </summary>
        public const string BYGS = "BYGS";
        /// <summary>
        /// 保养公里数
        /// </summary>
        public const string BYGLS = "BYGLS";
        /// <summary>
        /// 当前保养里程
        /// </summary>
        public const string DQBYLC = "DQBYLC";
        /// <summary>
        /// 下次保养里程
        /// </summary>
        public const string XCBYLC = "XCBYLC";
        /// <summary>
        /// 结算状态
        /// </summary>
        public const string JSZT = "JSZT";
        /// <summary>
        /// 经办人
        /// </summary>
        public const string JBR = "JBR";
        /// <summary>
        /// 责任人
        /// </summary>
        public const string ZRR = "ZRR";
        /// <summary>
        /// 备注
        /// </summary>
        public const string BZ = "BZ";
        /// <summary>
        /// 汽车
        /// </summary>
        public const string FK_Car = "FK_Car";
    }
    /// <summary>
    ///  保养管理
    /// </summary>
    public class BaoYang : EntityOID
    {
        #region 属性
        public string FK_Car
        {
            get
            {
                return this.GetValStringByKey(BaoYangAttr.FK_Car);
            }
            set
            {
                this.SetValByKey(BaoYangAttr.FK_Car, value);
            }
        }
        public string XCBYRQ
        {
            get
            {
                return this.GetValStringByKey(BaoYangAttr.XCBYRQ);
            }
            set
            {
                this.SetValByKey(BaoYangAttr.XCBYRQ, value);
            }
        }
        public decimal BYJE
        {
            get { return this.GetValDecimalByKey(BaoYangAttr.BYJE); }
            set { this.SetValByKey(BaoYangAttr.BYJE, value); }
        }
        public string BYRQ
        {
            get { return this.GetValStringByKey(BaoYangAttr.BYRQ); }
            set { this.SetValByKey(BaoYangAttr.BYRQ, value); }
        }
        public string JBR
        {
            get { return this.GetValStringByKey(BaoYangAttr.JBR); }
            set { this.SetValByKey(BaoYangAttr.JBR, value); }
        }
        public string XCBYLC
        {
            get { return this.GetValStringByKey(BaoYangAttr.XCBYLC); }
            set { this.SetValByKey(BaoYangAttr.XCBYLC, value); }
        }
        #endregion 属性

        #region 权限控制属性.
        //public override UAC HisUAC
        //{
        //    get
        //    {
        //        UAC uac = new UAC();
        //        if (Web.WebUser.No == "admin")
        //        {
        //            uac.IsDelete = false;
        //        }
        //        return uac;
        //    }
        //}
        #endregion 权限控制属性.

        #region 构造方法
        /// <summary>
        /// 保养管理
        /// </summary>
        public BaoYang()
        {
        }
        /// <summary>
        /// 保养管理
        /// </summary>
        /// <param name="oid"></param>
        public BaoYang(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// 保养管理Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarBaoYang");
                map.EnDesc = "保养管理";
                map.CodeStruct = "9";

                map.DepositaryOfEntity = Depositary.None;

                //增加OID主键字段。
                map.AddTBIntPKOID();

                //map.AddDDLEntities(BaoYangAttr.FK_Card, null, "卡", new Cards(),false);
                //map.AddDDLEntities(BaoYangAttr.FK_NY, null, "隶属年月", new BP.Pub.NYs(), false);

                map.AddDDLEntities(BaoYangAttr.FK_Car, null, "车辆", new CarInfos(), true);

                map.AddTBString(BaoYangAttr.BYDH, null, "保养单号", true, false, 2, 100, 30);
                map.AddTBDate(BaoYangAttr.BYRQ, null, "保养日期", true, false);
                map.AddTBDate(BaoYangAttr.XCBYRQ, null, "下次保养日期", true, false);
                map.AddTBMoney(BaoYangAttr.BYJE, 0, "保养余额（元）", true, false);
                map.AddTBString(BaoYangAttr.BYGS, null, "保养公司", true, false, 2, 100, 30);
                map.AddTBFloat(BaoYangAttr.BYGLS, 0, "保养公里数", true, false);

                map.AddTBFloat(BaoYangAttr.DQBYLC, 0, "当前保养里程", true, false);
                map.AddTBFloat(BaoYangAttr.XCBYLC, 0, "下次保养里程", true, false);

                map.AddTBString(BaoYangAttr.JSZT, null, "结算状态", true, false, 1, 100, 30);
                map.AddTBString(BaoYangAttr.JBR, null, "经办人", true, false, 2, 100, 30);
                map.AddTBString(BaoYangAttr.ZRR, null, "责任人", true, false, 2, 100, 30);
                map.AddTBStringDoc(BaoYangAttr.BZ, null, "备注", true, false, true);
                //查询条件.
                //map.AddSearchAttr(BaoYangAttr.FK_NY);
                //map.AddSearchAttr(BaoYangAttr.FK_Card);
                map.AddSearchAttr(BaoYangAttr.FK_Car);

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 重写方法
        /// <summary>
        /// 让其自动更新维护费.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_Car))
            {
                throw new Exception("车辆不能为空！");
            }
            if (string.IsNullOrEmpty(this.BYJE + ""))
            {
                throw new Exception("保养金额不能为空！");
            }
            if (string.IsNullOrEmpty(this.BYRQ))
            {
                throw new Exception("保养日期不能为空！");
            }
            if (string.IsNullOrEmpty(this.XCBYRQ))
            {
                throw new Exception("下次保养日期不能为空！");
            }
            if (string.IsNullOrEmpty(this.XCBYLC))
            {
                throw new Exception("下次保养里程不能为空！");
            }
            // 同步更新车辆的下次BaoYang日期
            CarInfo carInfo = new CarInfo();
            carInfo.No = this.FK_Car;
            carInfo.Retrieve();
            carInfo.BYRQ = this.XCBYRQ;
            carInfo.Update();
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
            feiyong.FeiYongJE = this.BYJE;
            feiyong.FeiYongRiQi = this.BYRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.BaoYang;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 保养管理
    /// </summary>
    public class BaoYangs : BP.En.EntitiesOID
    {
        /// <summary>
        /// 保养管理s
        /// </summary>
        public BaoYangs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BaoYang();
            }
        }
    }
}
