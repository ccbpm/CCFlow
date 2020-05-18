using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.En;
using BP.DA;
/************************************************************
   Copyright (C), 2005-2014, manstrosoft Co., Ltd.
   FileName: ZhiBiao.cs
   Author:孙浚顺
   Date:2014-05-18
   Description:指标管理类
   ***********************************************************/
namespace BP.OA.Car
{
    public enum ZBZT
    {
        /// <summary>
        /// 未用
        /// </summary>
        WeiYong,
        /// <summary>
        /// 已用
        /// </summary>
        YiYong
    }
    /// <summary>
    /// 指标获得方式
    /// </summary>
    public enum ZBHDFS
    {
        /// <summary>
        /// 摇号
        /// </summary>
        YaoHao,
        /// <summary>
        /// 变更
        /// </summary>
        BianGeng
    }
    /// <summary>
    /// 指标属性
    /// </summary>
    public class ZhiBiaoAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 原车牌号
        /// </summary>
        public const string YCPH = "YCPH";
        /// <summary>
        /// 登记人
        /// </summary>
        public const string DJR = "DJR";
        /// <summary>
        /// 登记日期
        /// </summary>
        public const string DJRQ = "DJRQ";
        /// <summary>
        /// 指标有效日期
        /// </summary>
        public const string ZBYXRQ = "ZBYXRQ";
        /// <summary>
        /// 指标到期日期
        /// </summary>
        public const string ZBDQRQ = "ZBDQRQ";
        /// <summary>
        /// 获得方式
        /// </summary>
        public const string HDFS = "HDFS";
        /// <summary>
        /// 指标编号
        /// </summary>
        public const string ZBBH = "ZBBH";
        /// <summary>
        /// 所属单位
        /// </summary>
        public const string SSDW = "SSDW";
        /// <summary>
        /// 指标状态
        /// </summary>
        public const string ZBZT = "ZBZT";
        /// <summary>
        /// 经办人
        /// </summary>
        public const string JBR = "JBR";
        /// <summary>
        /// 备注
        /// </summary>
        public const string BZ = "BZ";
    }
    /// <summary>
    ///  指标
    /// </summary>
    public class ZhiBiao : EntityNoName
    {
        #region 属性
        public int ZBZT
        {
            get
            {
                return this.GetValIntByKey(ZhiBiaoAttr.ZBZT);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.ZBZT, value);
            }
        }
        public int HDFS
        {
            get
            {
                return this.GetValIntByKey(ZhiBiaoAttr.HDFS);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.HDFS, value);
            }
        }
        public string ZBYXRQ
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.ZBYXRQ);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.ZBYXRQ, value);
            }
        }
        public string YCPH
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.YCPH);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.YCPH, value);
            }
        }
        public string DJR
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.DJR);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.DJR, value);
            }
        }
        public string DJRQ
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.DJRQ);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.DJRQ, value);
            }
        }
        public string ZBDQRQ
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.ZBDQRQ);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.ZBDQRQ, value);
            }
        }
        public string ZBBH
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.ZBBH);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.ZBBH, value);
            }
        }
        public string SSDW
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.SSDW);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.SSDW, value);
            }
        }
        public string JBR
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.JBR);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.JBR, value);
            }
        }

        public string BZ
        {
            get
            {
                return this.GetValStringByKey(ZhiBiaoAttr.BZ);
            }
            set
            {
                this.SetValByKey(ZhiBiaoAttr.BZ, value);
            }
        }







        #endregion 属性
        #region 权限控制属性.
        #endregion 权限控制属性.
        #region 构造方法
        /// <summary>
        /// 指标
        /// </summary>
        public ZhiBiao()
        {
        }
        /// <summary>
        /// 指标
        /// </summary>
        /// <param name="_No"></param>
        public ZhiBiao(string _No) : base(_No) { }
        #endregion
        /// <summary>
        /// 指标Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_CarZhiBiao");
                map.EnDesc = "指标管理";
                map.CodeStruct = "3";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.None;

                map.AddTBStringPK(ZhiBiaoAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(ZhiBiaoAttr.Name, null, "车牌号", true, false, 2, 100, 30, false);
                map.AddTBString(ZhiBiaoAttr.YCPH, null, "原车牌号", true, false, 2, 100, 30, false);
                map.AddTBString(ZhiBiaoAttr.DJR, null, "登记人", true, false, 2, 100, 30, false);
                map.AddTBDate(ZhiBiaoAttr.DJRQ, null, "登记日期", true, false);
                map.AddTBDate(ZhiBiaoAttr.ZBYXRQ, null, "指标有效日期", true, false);
                map.AddTBDate(ZhiBiaoAttr.ZBDQRQ, null, "指标到期日期", true, false);
                map.AddDDLSysEnum(ZhiBiaoAttr.HDFS, 0, "获得方式", true, true, ZhiBiaoAttr.HDFS, "@0=摇号@1=变更");
                map.AddDDLSysEnum(ZhiBiaoAttr.ZBZT, 0, "指标状态", true, false, ZhiBiaoAttr.ZBZT, "@0=未用@1=已用");
                map.AddTBString(ZhiBiaoAttr.ZBBH, null, "指标编号", true, false, 2, 100, 30, false);
                map.AddTBString(ZhiBiaoAttr.SSDW, null, "所属单位", true, false, 2, 100, 30, false);
                map.AddTBString(ZhiBiaoAttr.JBR, null, "经办人", true, false, 2, 100, 30, false);
                map.AddTBStringDoc(ZhiBiaoAttr.BZ, null, "备注", true, false, true);

                // 设置查询条件
                map.AddSearchAttr(ZhiBiaoAttr.HDFS);
                map.AddSearchAttr(ZhiBiaoAttr.ZBZT);

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 重写方法
        protected override bool beforeUpdateInsertAction()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                throw new Exception("车牌号不能为空！");
            }
            if (string.IsNullOrEmpty(this.ZBYXRQ))
            {
                throw new Exception("指标有效期不能为空！");
            }

            return base.beforeUpdateInsertAction();
        }
        #endregion 重写方法
    }
    /// <summary>
    /// 指标
    /// </summary>
    public class ZhiBiaos : EntitiesNoName
    {
        /// <summary>
        /// 指标s
        /// </summary>
        public ZhiBiaos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ZhiBiao();
            }
        }
    }
}
