using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: WeiHu.cs
   Author:周鹏、孙浚顺
   Date:2014-05-18
   Description:车辆维护类
   ***********************************************************/
namespace BP.OA.Car
{
    
    /// <summary>
    /// 维护台帐 属性
    /// </summary>
    public class WeiHuAttr : EntityOIDAttr
    {
        /// <summary>
        /// 维修人
        /// </summary>
        public const string WXR = "WXR";
        /// <summary>
        /// 维修时间
        /// </summary>
        public const string WXRQ = "WXRQ";
        /// <summary>
        /// 维修费用
        /// </summary>
        public const string WSFY = "WSFY";
        /// <summary>
        /// 填写日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 维修公司
        /// </summary>
        public const string WXGS = "WXGS";
        /// <summary>
        /// 关联的车号
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
    }
    /// <summary>
    ///  维护台帐
    /// </summary>
    public class WeiHu : EntityOID
    {
        #region 属性
        public string WXR
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.WXR); }

            set
            {
                this.SetValByKey(WeiHuAttr.WXR, value);
            }
        }
         public string WXRQ
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.WXRQ); }

            set
            {
                this.SetValByKey(WeiHuAttr.WXRQ, value);
            }
        }
         public string FK_NY
        {
            get
            { return this.GetValStrByKey(WeiHuAttr.FK_NY); }

            set
            {
                this.SetValByKey(WeiHuAttr.FK_NY, value);
            }
        }
         public string FK_Car 
         {
             get { return this.GetValStringByKey(WeiHuAttr.FK_Car); }
             set { this.SetValRefTextByKey(WeiHuAttr.FK_Car,value); }
         }
        public decimal WSFY
        {
            get
            { return this.GetValDecimalByKey(WeiHuAttr.WSFY); }

            set
            {
                this.SetValByKey(WeiHuAttr.WSFY, value);
            }
        }
        #endregion 属性

        #region 权限控制属性.
        #endregion 权限控制属性.

        #region 构造方法
        /// <summary>
        /// 维护台帐
        /// </summary>
        public WeiHu()
        {
        }
        /// <summary>
        /// 维护台帐
        /// </summary>
        /// <param name="oid"></param>
        public WeiHu(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// 维护台帐Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarWeiXiu");
                map.EnDesc = "维修管理";
                map.CodeStruct = "3";
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.None;

                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(WeiHuAttr.FK_Car, null, "车辆", new CarInfos(),true);
                map.AddTBString(WeiHuAttr.WXR, null, "维修人", true, false, 2, 100, 30, false);
                map.AddTBDate(WeiHuAttr.WXRQ, null, "维修日期", true, false);
                map.AddTBMoney(WeiHuAttr.WSFY, 0, "维修费用(人民币)", true, false);
                map.AddTBString(WeiHuAttr.WXGS, null, "维修公司", true, false, 2, 100, 30, true);
                map.AddTBStringDoc(CarInfoAttr.Note, null, "备注", true, false, true);

                //map.AddDDLEntities(WeiHuAttr.FK_NY, null, "隶属年月", new BP.Pub.NYs(), false);

                //查询条件.
                map.AddSearchAttr(WeiHuAttr.FK_Car);
                //map.AddSearchAttr(WeiHuAttr.FK_NY);


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
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.WXRQ))
            {
                throw new Exception("维修日期不能为空！");
            }
            if (string.IsNullOrEmpty(this.WSFY+""))
            {
                throw new Exception("维修费用不能为空！");
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
            feiyong.JingBanRen = this.WXR;
            feiyong.FeiYongJE = this.WSFY;
            feiyong.FeiYongRiQi = this.WXRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.WeiXiu;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 维护台帐
    /// </summary>
    public class WeiHus : BP.En.EntitiesOID
    {
        /// <summary>
        /// 维护台帐s
        /// </summary>
        public WeiHus() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WeiHu();
            }
        }
    }
}
