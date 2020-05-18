using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: Drivers.cs
   Author:周鹏、孙浚顺
   Date:2014-05-18
   Description:司机信息管理类
   ***********************************************************/
namespace BP.OA.Car
{
    
    /// <summary>
    /// 司机属性
    /// </summary>
    public class DriverAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 状态
        /// </summary>
        public const string CarSta = "CarSta";
        /// <summary>
        /// 树形的编号
        /// </summary>
        public const string TreeNo = "TreeNo";
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
        /// 司机名称
        /// </summary>
       /// public const string SiJiMingCheng = "SiJiMingCheng";
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

        //============================================================
        /// <summary>
        /// 备注
        /// </summary>
        public const string BZ = "BZ";
        /// <summary>
        /// 驾证到期日
        /// </summary>
        public const string JZDQR = "JZDQR";
        /// <summary>
        /// 领证日期
        /// </summary>
        public const string LZRQ = "LZRQ";
        /// <summary>
        /// 准驾车型
        /// </summary>
        public const string ZJCX = "ZJCX";
        /// <summary>
        /// 出车次数
        /// </summary>
        public const string CCCS = "CCCS";
        /// <summary>
        /// 违章次数
        /// </summary>
        public const string WZCS = "WZCS";
        /// <summary>
        /// 罚款金额累计
        /// </summary>
        public const string FKZJE = "FKZJE";
    }
    /// <summary>
    ///  司机
    /// </summary>
    public class Driver : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 出车次数
        /// </summary>
        public Int64 CCCS {
            get {
                return this.GetValInt64ByKey(DriverAttr.CCCS);
            }
            set {
                this.SetValByKey(DriverAttr.CCCS,value);
            }
        }
        /// <summary>
        /// 违章次数
        /// </summary>
        public Int64 WZCS {
            get {
                return this.GetValInt64ByKey(DriverAttr.WZCS);
            }
            set {
                this.SetValByKey(DriverAttr.WZCS,value);
            }
        }
        /// <summary>
        /// 罚款总金额
        /// </summary>
        public decimal FKZJE {
            get {
                return this.GetValDecimalByKey(DriverAttr.FKZJE);
            }
            set {
                this.SetValByKey(DriverAttr.FKZJE,value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        /// 
        public string FK_Dept
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(DriverAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 发布人
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DriverAttr.FK_Emp, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.RDT);
            }
            set
            {
                this.SetValByKey(DriverAttr.RDT, value);
            }
        }
        public string EDT
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.EDT);
            }
            set
            {
                this.SetValByKey(DriverAttr.EDT, value);
            }
        }
        public string ZJCX
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.ZJCX);
            }
            set
            {
                this.SetValByKey(DriverAttr.ZJCX, value);
            }
        }
        public string LZRQ
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.LZRQ);
            }
            set
            {
                this.SetValByKey(DriverAttr.LZRQ, value);
            }
        }
        public string JZDQR
        {
            get
            {
                return this.GetValStrByKey(DriverAttr.JZDQR);
            }
            set
            {
                this.SetValByKey(DriverAttr.JZDQR, value);
            }
        }

        #endregion 属性

        #region 权限控制属性.
        #endregion 权限控制属性.

        #region 构造方法
        /// <summary>
        /// 司机
        /// </summary>
        public Driver()
        {
        }
        /// <summary>
        /// 司机
        /// </summary>
        /// <param name="_No"></param>
        public Driver(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 司机Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarDriver");
                map.EnDesc = "司机";
                map.CodeStruct = "3";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(DriverAttr.No, null, "编号", true, true, 1, 100, 100);
                map.AddTBString(DriverAttr.Name, null, "驾驶员姓名", true, false, 0, 100, 120, false);
                map.AddTBString(DriverAttr.ZJCX, null, "准驾车型", true, false, 0, 100, 100, false);
                map.AddTBDate(DriverAttr.LZRQ, null, "领证日期", true, false);
                map.AddTBDate(DriverAttr.JZDQR, null, "驾证到期日", true, false);
                map.AddTBInt(DriverAttr.CCCS,0,"出车次数",true,true);
                map.AddTBInt(DriverAttr.WZCS,0,"违章次数",true,true);
                map.AddTBDecimal(DriverAttr.FKZJE, 0, "罚款金额累计", true, true);
                map.AddTBStringDoc(DriverAttr.BZ, null, "备注", true, false,true);

                //map.AddDDLSysEnum(DriverAttr.CarSta, 0, "状态", true, true, DriverAttr.CarSta,
                //    "@0=空闲中@1=出差中@2=维护中");
                //map.AddTBString(DriverAttr.FK_Emp, null, "发布人", true, false, 0, 100, 30);
                //map.AddTBString(DriverAttr.FK_Dept, null, "部门", true, false, 0, 100, 30);
                //map.AddTBInt(DriverAttr.Idx, 0, "Idx", false, false);
                //map.AddTBInt(DriverAttr.ReadTimes, 0, "阅读次数", false, false);
                //map.AddTBInt(DriverAttr.IsDownload, 0, "是否可以下载?", false, false);

                // 设置查询条件

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "出车登记";
                rm.ClassMethodName = this.ToString() + ".DoChuCheHistory";                
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "违章登记";
                rm.ClassMethodName = this.ToString() + ".DoWeiZhangHistory";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoChuCheHistory() 
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarRecords&FK_JiaShiYuan=" + this.No;
        }
        public string DoWeiZhangHistory()
        {
            return "/WF/Comm/SearchBS.htm?EnsName=BP.OA.Car.CarWeiZhangs&FK_JiaShiYuan=" + this.No;
        }

        #region 重写方法
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.Name))
                 throw new Exception("驾驶员姓名不能为空！");
            
            if (string.IsNullOrEmpty(this.ZJCX))
                           throw new Exception("准假车型不能为空！");
            
            if (string.IsNullOrEmpty(this.LZRQ))
                            throw new Exception("领证日期姓名不能为空！");
            
            if (string.IsNullOrEmpty(this.JZDQR))
                            throw new Exception("驾证到期日不能为空！");
            
            return base.beforeInsert();
        }

       
        #endregion 重写方法
    }
    /// <summary>
    /// 司机
    /// </summary>
    public class Drivers : BP.En.EntitiesNoName
    {
        /// <summary>
        /// 司机s
        /// </summary>
        public Drivers() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Driver();
            }
        }
    }
}
