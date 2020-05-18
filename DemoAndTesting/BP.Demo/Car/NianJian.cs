using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: NianJian.cs
   Author:周鹏、孙浚顺
   Date:2014-05-18
   Description:车辆年检类
   ***********************************************************/
namespace BP.OA.Car
{
    /// <summary>
    /// 年检台帐 属性
    /// </summary>
    public class NianJianAttr : EntityOIDAttr
    {
        /// <summary>
        /// 年检人
        /// </summary>
        public const string NJR = "NJR";
        /// <summary>
        /// 年检时间
        /// </summary>
        public const string NJRQ = "NJRQ";
        /// <summary>
        /// 年检费用
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// 下次日期
        /// </summary>
        public const string NextDT = "NextDT";
        /// <summary>
        /// che 
        /// </summary>
        public const string FK_Car = "FK_Car";
        /// <summary>
        /// 
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 责任人
        /// </summary>
        public const string ZRR = "ZRR";
        /// <summary>
        /// 备注
        /// </summary>
        public const string BZ = "BZ";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";

    }
    /// <summary>
    ///  年检台帐
    /// </summary>
    public class NianJian : EntityOID
    {
        #region 属性
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(NianJianAttr.WorkID);
            }
            set
            {
                this.SetValByKey(NianJianAttr.WorkID, value);
            }
        }
        public string NextDT
        {
            get {
                return this.GetValStringByKey(NianJianAttr.NextDT);
            }
            set {
                this.SetValByKey(NianJianAttr.NextDT, value);
            }
        }
        public string FK_Car
        {
            get
            { return this.GetValStrByKey(NianJianAttr.FK_Car); }

            set
            {
                this.SetValByKey(NianJianAttr.FK_Car, value);
            }
        }
        public string FK_CarText 
        {
            get 
            {
                return this.GetValRefTextByKey(NianJianAttr.FK_Car);
            }
        }
        public string NJR
        {
            get
            { return this.GetValStrByKey(NianJianAttr.NJR); }

            set
            {
                this.SetValByKey(NianJianAttr.NJR, value);
            }
        }
        public string ZRR
        {
            get
            { return this.GetValStrByKey(NianJianAttr.ZRR); }

            set
            {
                this.SetValByKey(NianJianAttr.ZRR, value);
            }
        }
        public string BZ
        {
            get
            { return this.GetValStrByKey(NianJianAttr.BZ); }

            set
            {
                this.SetValByKey(NianJianAttr.BZ, value);
            }
        }
        /// <summary>
        /// 年检日期
        /// </summary>
        public string NJRQ
        {
            get
            {
                return this.GetValStrByKey(NianJianAttr.NJRQ); 
            }
            set
            {
                this.SetValByKey(NianJianAttr.NJRQ, value);
            }
        }
        public decimal JE
        {
            get
            { return this.GetValDecimalByKey(NianJianAttr.JE); }

            set
            {
                this.SetValByKey(NianJianAttr.JE, value);
            }
        }
        #endregion 属性

        #region 权限控制属性.
        #endregion 权限控制属性.

        #region 构造方法
        /// <summary>
        /// 年检台帐
        /// </summary>
        public NianJian()
        {
        }
        /// <summary>
        /// 年检台帐
        /// </summary>
        /// <param name="oid"></param>
        public NianJian(int oid) : base(oid) { }
        #endregion

        /// <summary>
        /// 年检台帐Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarNianJian");
                map.EnDesc = "年检管理";
                map.CodeStruct = "9";
                map.IsAutoGenerNo = true;

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.None;
                //增加OID主键字段。
                map.AddTBIntPKOID();

                map.AddDDLEntities(NianJianAttr.FK_Car, null, "车辆", new CarInfos(),true);
                map.AddTBMoney(NianJianAttr.JE, 0, "年检费用(人民币)", true, false);
                map.AddTBDate(NianJianAttr.NJRQ, null, "年检日期", true, false);
                map.AddTBDate(NianJianAttr.NextDT, null, "下次年检日期", true, false);
                map.AddTBString(NianJianAttr.NJR, null, "经办人", true, false, 2, 100, 30, false);
                map.AddTBString(NianJianAttr.ZRR, null, "责任人", true, false, 2, 100, 30, false);

                map.AddTBStringDoc(NianJianAttr.BZ,null,"备注",true,false,true);
                map.AddTBInt(NianJianAttr.WorkID, 0, "下次年WorkID检日期", false, false);

                //查询条件.
                map.AddSearchAttr(NianJianAttr.FK_Car);

                //RefMethod rm = new RefMethod();
                //rm = new RefMethod();
                //rm.Title = "年检流程记录";
                //rm.ClassMethodName = this.ToString() + ".DoNianJian";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        //public string DoNianJian()
        //{
        //    //return "/WF/WFRpt.htm?FK_Flow=" + API.Car_NianJian_FlowMark + "&WorkID=" + this.WorkID, "sd", "s", 500, 600);
        //    return null;
        //}

        #region 重写方法
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        /// <summary>
        /// 让其自动更新下次年检日期.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_Car))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.NJR))
            {
                throw new Exception("经办人不能为空！");
            } 
            if (string.IsNullOrEmpty(this.JE+""))
            {
                throw new Exception("年检金额车牌号不能为空！");
            } 
            // 同步更新车辆的下次年检日期
            CarInfo carInfo = new CarInfo();
            
            carInfo.No = this.FK_Car;
            carInfo.Retrieve();// 从数据库取出数据
            carInfo.NJRQ = this.NextDT;
            carInfo.Update();
            CarInfos carInfos = new CarInfos();
            return base.beforeUpdateInsertAction();//年检本身自己插入或者更新
        }
        protected override void afterInsertUpdateAction()
        {
            // 更新费用记录
            CarFeiYongGuanLi feiyong = new CarFeiYongGuanLi();
            feiyong.MyFK = "BY_" + this.OID;
            feiyong.RetrieveByAttr(CarFeiYongGuanLiAttr.MyFK, feiyong.MyFK);
            feiyong.FK_car = this.FK_Car;
            feiyong.JingBanRen = this.NJR;
            feiyong.FeiYongJE = this.JE;
            feiyong.FeiYongRiQi = this.NJRQ;
            feiyong.FeiYongMingCheng = (int)FeiYongMingCheng.NianJian;
            feiyong.SuoShuNY = feiyong.FeiYongRiQi.Substring(0, 7);
            feiyong.Save();
            base.afterInsertUpdateAction();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 年检台帐
    /// </summary>
    public class NianJians : BP.En.EntitiesOID
    {
        /// <summary>
        /// 年检台帐s
        /// </summary>
        public NianJians() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NianJian();
            }
        }
    }
}
