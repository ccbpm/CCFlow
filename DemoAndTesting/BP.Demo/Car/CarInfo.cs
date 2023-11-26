using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: 
   Author:    
   Date:
   Description:  
   ***********************************************************/
namespace BP.OA.Car
{
    public enum CarInfoType
    {
        /// <summary>
        /// 闲置中
        /// </summary>
        XianZhiing,
        /// <summary>
        /// 出车中
        /// </summary>
        ChuCheing
    }
    /// <summary>
    /// 汽车信息属性
    /// </summary>
    public class CarInfoAttr : EntityNoNameAttr
    {

        /// <summary>
        /// 车牌号
        /// </summary>
        public const string FK_CPH = "FK_CPH";
        /// <summary>
        /// 状态
        /// </summary>
        public const string CarSta = "CarSta";
        /// <summary>
        /// 司机
        /// </summary>
        public const string Driver = "Driver";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 阅读次数
        /// </summary>
        public const string ReadTimes = "ReadTimes";
        /// <summary>
        /// 发布人
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 发布部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 创建日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 修改日期
        /// </summary>
        public const string EDT = "EDT";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public const string IsDownload = "IsDownload";

        /// <summary>
        /// 公里数
        /// </summary>
        public const string GLS = "GLS";
        /// <summary>
        /// 维修费
        /// </summary>
        public const string FY_WSF = "FY_WSF";
        /// <summary>
        /// 年检费
        /// </summary>
        public const string FY_NianJian = "FY_NianJian";
        /// <summary>
        /// 下次年检日期
        /// </summary>
        public const string NJRQ = "NJRQ";
        /// <summary>
        /// 下次保养日期
        /// </summary>
        public const string BYRQ = "BYRQ";
        /// <summary>
        /// 备注 
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 车架号
        /// </summary>
        public const string CJH = "CJH";
        /// <summary>
        /// 保险到期日
        /// </summary>
        public const string BXDQR = "BXDQR";
        /// <summary>
        /// 购车日期
        /// </summary>
        public const string GCRQ = "GCRQ";

        public const string FY_Oil = "FY_Oil";
        public const string FY_Stop = "FY_Stop";
        public const string FY_Etc = "FY_Etc";

        //============================================================
        /// <summary>
        /// 车辆状态
        /// </summary>
        public const string CLZT = "CLZT";
        /// <summary>
        /// 车型
        /// </summary>
        public const string CX = "CX";
        /// <summary>
        /// 发动机号
        /// </summary>
        public const string FDJH = "FDJH";
        /// <summary>
        /// 责任人
        /// </summary>
        public const string ZRR = "ZRR";
        /// <summary>
        /// 专用部门
        /// </summary>
        public const string ZYBM = "ZYBM";
        /// <summary>
        /// 专用人
        /// </summary>
        public const string ZYR = "ZYR";
    }
    /// <summary>
    ///  汽车信息
    /// </summary>
    public class CarInfo : EntityNoName
    {
        #region 属性
        public string BXDQR
        {
            get { return this.GetValStringByKey(CarInfoAttr.BXDQR); }
            set { this.SetValByKey(CarInfoAttr.BXDQR, value); }
        }
        public string ZRR
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZRR);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZRR, value);
            }
        }
        public string ZYR
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZYR);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZYR, value);
            }
        }
        public string ZYBM
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.ZYBM);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ZYBM, value);
            }
        }
        public string GCRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.GCRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.GCRQ, value);
            }
        }
        public string BYRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.BYRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.BYRQ, value);
            }
        }
        public string NJRQ
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.NJRQ);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.NJRQ, value);
            }
        }
        public string FK_CPH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.FK_CPH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_CPH, value);
            }
        }
        public string FK_CPHText
        {
            get
            {
                return this.GetValRefTextByKey(CarInfoAttr.FK_CPH);
            }
        }
        public decimal FY_Oil
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Oil);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Oil, value);
            }
        }
        public decimal FY_Etc
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Etc);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Etc, value);
            }
        }
        public decimal FY_Stop
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_Stop);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_Stop, value);
            }
        }
        public string CX
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.CX);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.CX, value);
            }
        }
        public string CJH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.CJH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.CJH, value);
            }
        }
        public string FDJH
        {
            get
            {
                return this.GetValStringByKey(CarInfoAttr.FDJH);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FDJH, value);
            }
        }
        #endregion 属性

        #region 属性
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.Title);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.Title, value);
            }
        }
        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadTimes
        {
            get
            {
                return this.GetValIntByKey(CarInfoAttr.ReadTimes);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.ReadTimes, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 发布人
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FK_Emp, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.RDT);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.RDT, value);
            }
        }
        public string EDT
        {
            get
            {
                return this.GetValStrByKey(CarInfoAttr.EDT);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.EDT, value);
            }
        }
        public decimal FY_WSF
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_WSF);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_WSF, value);
            }
        }
        public decimal FY_NianJian
        {
            get
            {
                return this.GetValDecimalByKey(CarInfoAttr.FY_NianJian);
            }
            set
            {
                this.SetValByKey(CarInfoAttr.FY_NianJian, value);
            }
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
        /// 汽车信息
        /// </summary>
        public CarInfo()
        {
        }
        /// <summary>
        /// 汽车信息
        /// </summary>
        /// <param name="_No"></param>
        public CarInfo(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 汽车信息Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarInfo");
                map.EnDesc = "车辆信息";
                map.CodeStruct = "3";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(CarInfoAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(CarInfoAttr.Name, null, "", false, true, 2, 100, 30);

                // 车牌号
                map.AddDDLEntities(CarInfoAttr.FK_CPH, null, "车牌号", new ZhiBiaos(), true);
                map.AddTBString(CarInfoAttr.CX, null, "车型", true, false, 2, 100, 30, false);
                map.AddTBString(CarInfoAttr.CJH, null, "车架号", true, false, 2, 100, 30, false);
                map.AddTBString(CarInfoAttr.FDJH, null, "发动机号", true, false, 2, 100, 30, false);

                map.AddTBFloat(CarInfoAttr.GLS, 0, "当前里程", true, false);
                map.AddTBDate(CarInfoAttr.NJRQ, null, "下次年检日期", true, true);//==============
                map.AddTBDate(CarInfoAttr.BYRQ, null, "下次保养日期", true, true);//===============
                map.AddTBDate(CarInfoAttr.BXDQR, null, "保险到期日", true, true);
                map.AddTBDate(CarInfoAttr.GCRQ, null, "购车日期", true, false);

                //map.AddDDLEntities();
                map.AddTBString(CarInfoAttr.ZYBM, null, "专用部门", true, false, 2, 100, 30, false);//=====================
                map.AddTBString(CarInfoAttr.ZYR, null, "专用人", true, false, 2, 100, 30, false);
                //========================
                map.AddTBString(CarInfoAttr.ZRR, null, "责任人", true, false, 2, 100, 30, false);//========================
                map.AddDDLSysEnum(CarInfoAttr.CLZT, 0, "车辆状态", true, false, CarInfoAttr.CLZT, "@0=闲置中@1=出车中@2=维修中");
                map.AddTBStringDoc(CarInfoAttr.Note, null, "备注", true, false, true);

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "年检登记";
                rm.ClassMethodName = this.ToString() + ".DoNianJian";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "历史年检记录";
                rm.ClassMethodName = this.ToString() + ".DoNJHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "保养登记";
                rm.ClassMethodName = this.ToString() + ".DoBaoYang";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "历史保养记录";
                rm.ClassMethodName = this.ToString() + ".DoBYHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "维修登记";
                rm.ClassMethodName = this.ToString() + ".DoWeiHu";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "历史维修记录";
                rm.ClassMethodName = this.ToString() + ".DoWXHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "入保登记";
                rm.ClassMethodName = this.ToString() + ".DoRuBao";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "历史入保记录";
                rm.ClassMethodName = this.ToString() + ".DoRBHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "登记出保";
                rm.ClassMethodName = this.ToString() + ".DoChuBao";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "历史出保记录";
                rm.ClassMethodName = this.ToString() + ".DoCBHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "充值管理";
                rm.ClassMethodName = this.ToString() + ".DoChongZhi";
                map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "历史充值记录";
                rm.ClassMethodName = this.ToString() + ".DoCZHistory";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "费用综合查询";
                rm.ClassMethodName = this.ToString() + ".DoFeiYongZongHe";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoFeiYongZongHe()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarFeiYongGuanLis&FK_Car=" + this.No;
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public string DoChongZhi()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.FullMoneys&FK_Car=" + this.No;
        }
        public string DoCZHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.FullMoneys&FK_Car=" + this.No;
        }
        /// <summary>
        /// 入保
        /// </summary>
        /// <returns></returns>
        public string DoRuBao()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.CarRuBaos&FK_Car=" + this.No;
        }
        public string DoRBHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarRuBaos&FK_Car=" + this.No;
        }
        /// <summary>
        /// 出险
        /// </summary>
        /// <returns></returns>
        public string DoChuBao()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.CarChuBaos&FK_Car=" + this.No;
        }
        public string DoCBHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarChuBaos&FK_Car=" + this.No;
        }
        /// <summary>
        /// BaoYang
        /// </summary>
        /// <returns></returns>
        public string DoBaoYang()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.BaoYangs&FK_Car=" + this.No;
        }
        public string DoBYHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.BaoYangs&FK_Car=" + this.No;
        }
        /// <summary>
        /// 维修
        /// </summary>
        /// <returns></returns>
        public string DoWeiHu()
        {
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.WeiHus&FK_Car=" + this.No;
        }
        public string DoWXHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.WeiHus&FK_Car=" + this.No;
        }
        /// <summary>
        /// 年检
        /// </summary>
        /// <returns></returns>
        public string DoNianJian()
        {
            //if (BP.OA.Car.API.Car_NianJian_FlowIsEnable ==true) /*启用流程.*/
            //    return "/WF/MyFlow.htm?FK_Flow="+API.Car_NianJian_FlowMark+"&FK_Car=" + this.No,"sss", 500, 600);
            //else
            return "/WF/Comm/En.htm?EnsName=BP.OA.Car.NianJians&FK_Car=" + this.No;
        }
        public string DoNJHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.NianJians&FK_Car=" + this.No;
        }
        #region 重写方法
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.FK_CPH))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.CX))
            {
                throw new Exception("车型不能为空！");
            }
            if (string.IsNullOrEmpty(this.FDJH))
            {
                throw new Exception("发动机号不能为空！");
            }
            if (string.IsNullOrEmpty(this.CJH))
            {
                throw new Exception("车架号不能为空！");
            }
            // 设置Name与车牌号相同
            ZhiBiao zb = new ZhiBiao();
            zb.No = this.FK_CPH;
            zb.Retrieve();
            this.Name = zb.Name;
            return base.beforeUpdateInsertAction();
        }
        protected override void afterInsertUpdateAction()
        {
            try
            {
                // 此处需要修改指标使用状态
                ZhiBiao temp = new ZhiBiao();
                temp.No = this.FK_CPH;
                temp.Retrieve();
                temp.ZBZT = (int)ZBZT.YiYong;
                temp.Update();
            }
            catch (Exception ex)
            {

            }
            base.afterInsertUpdateAction();
        }

        protected override bool beforeUpdate()
        {

            return base.beforeUpdate();
        }
        /// <summary>
        /// 同步维护费
        /// </summary>
        public static void DTS_WHF()
        {
            CarInfos infos = new CarInfos();
            infos.RetrieveAll();
            foreach (CarInfo en in infos)
            {
                string sql = "SELECT SUM(WSFY) FROM oa_carweihu  WHERE FK_Car='" + en.No + "'";
                decimal d = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                en.FY_WSF = d;
                en.Update();
            }
        }
        public static void DTS_NianJian()
        {
            CarInfos infos = new CarInfos();
            infos.RetrieveAll();
            foreach (CarInfo en in infos)
            {
                string sql = "SELECT SUM(JE) FROM OA_CarNianJian  WHERE FK_Car='" + en.No + "'";
                decimal d = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                en.FY_NianJian = d;
                en.Update();
            }
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 汽车信息
    /// </summary>
    public class CarInfos : EntitiesNoName
    {
        /// <summary>
        /// 汽车信息s
        /// </summary>
        public CarInfos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CarInfo();
            }
        }
    }
}
