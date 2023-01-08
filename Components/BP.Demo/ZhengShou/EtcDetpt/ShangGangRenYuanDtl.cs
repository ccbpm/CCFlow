using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF.Data;
using BP.En;

namespace BP.ZS
{
    /// <summary>
    /// 不良记录s Attr
    /// </summary>
    public class ShangGangRenYuanDtlAttr : EntityOIDAttr
    {
        #region 基本属性
        public const string RefPK = "RefPK";

        /// <summary>
        /// 箱体名称
        /// </summary>
        public const string Docs = "Docs";


        /// <summary>
        /// 期限限定
        /// </summary>
        public const string QiXianXianDing = "QiXianXianDing";
        /// <summary>
        /// 紧急程度
        /// </summary>
        public const string RDT = "RDT";
        public const string RDTText = "RDTText";

        /// <summary>
        /// 图纸编号
        /// </summary>
        public const string TuZhiBianHao = "TuZhiBianHao";
        /// <summary>
        /// 图纸制图人
        /// </summary>
        public const string TuZhiZhiTuRen = "TuZhiZhiTuRen";
        /// <summary>


        /// <summary>
        /// 0=未处理. 1=已完成. 2=重做. 3=已验收. 4=发货.
        /// </summary>
        public const string XTSta = "XTSta";
        #endregion
    }
    /// <summary>
    /// 不良记录s
    /// </summary>
    public class ShangGangRenYuanDtl : EntityOID
    {
        #region 属性
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(ShangGangRenYuanDtlAttr.RefPK);
            }
            set
            {
                this.SetValByKey(ShangGangRenYuanDtlAttr.RefPK, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 不良记录
        /// </summary>
        public ShangGangRenYuanDtl()
        {

        }
        /// <summary>
        /// 不良记录
        /// </summary>
        /// <param name="workid">工作ID</param>
        public ShangGangRenYuanDtl(int workid)
        {
            this.OID = workid;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ShangGangRenYuanDtl", "不良记录");

                #region 流程的基本字段
                map.AddTBIntPKOID();

                map.AddTBString(ShangGangRenYuanDtlAttr.RefPK, null, "主键", false, true, 0, 30, 10);
                map.AddTBDate(ShangGangRenYuanDtlAttr.RDT, null, "日期", true, false);
                map.AddTBString(ShangGangRenYuanDtlAttr.Docs, null, "内容", true, false, 0, 500, 500);

                this._enMap = map;
                return this._enMap;

                #endregion 流程的基本字段

            }
        }
        #endregion
    }
    /// <summary>
    /// 不良记录s
    /// </summary>
    public class ShangGangRenYuanDtls : EntitiesOID
    {

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ShangGangRenYuanDtl();
            }
        }
        /// <summary>
        /// 不良记录s
        /// </summary>
        public ShangGangRenYuanDtls() { }
        #endregion
    }
}
