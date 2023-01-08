using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.LIMS
{
    /// <summary>
    /// 样本分析 attr
    /// </summary>
    public class YBFenXiAttr : EntityOIDAttr
    {
        /// <summary>
        /// workID
        /// </summary>
        public const string RefPK = "RefPK";
        public const string RDT = "RDT";
        public const string Rec = "Rec";
        public const string MingCheng = "MingCheng";
        public const string BianHao = "BianHao";
        /// <summary>
        /// 0=未处理. 1=等待检验. 2=检验中. 3=检验完成.
        /// </summary>
        public const string YBSta = "YBSta";

        public const string WorkIDOfWT = "WorkIDOfWT";
        public const string PoolOID = "PoolOID";
    }
    /// <summary>
    ///  样本分析
    /// </summary>
    public class YBFenXi : EntityOID
    {
        #region 属性.
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }
        #endregion 属性.
        public Int64 WorkIDOfWT
        {
            get
            {
                return this.GetValInt64ByKey(YBFenXiAttr.WorkIDOfWT);
            }
            set
            {
                this.SetValByKey(YBFenXiAttr.WorkIDOfWT, value);
            }
        }
        public Int64 RefPK
        {
            get
            {
                return this.GetValInt64ByKey(YBFenXiAttr.RefPK);
            }
            set
            {
                this.SetValByKey(YBFenXiAttr.RefPK, value);
            }
        }

        #region 构造方法
        /// <summary>
        /// 样本分析
        /// </summary>
        public YBFenXi()
        {
        }
        /// <summary>
        /// 样本分析
        /// </summary>
        /// <param name="_No"></param>
        public YBFenXi(int _No) : base(_No) { }
        /// <summary>
        /// 样本分析Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("YB_YBFenXi", "样本分析");

                map.AddTBIntPKOID();

                map.AddTBString(YBFenXiAttr.RefPK, null, "关联主键", false, true, 0, 50, 50);

                //  0=未处理. 1=等待检验. 2=检验中. 3=检验完成.
                map.AddTBInt(YBFenXiAttr.YBSta, 0, "状态", false, true);

                map.AddTBString(YBFenXiAttr.MingCheng, null, "名称", false, true, 0, 50, 50);
                map.AddTBString(YBFenXiAttr.BianHao, null, "编号", false, true, 0, 50, 50);


                map.AddTBInt(YBFenXiAttr.WorkIDOfWT, 0, "委托WorkID", false, true);
                map.AddTBInt(YBFenXiAttr.PoolOID, 0, "PoolOID", false, true);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


    }
    /// <summary>
    /// 样本分析s
    /// </summary>
    public class YBFenXis : EntitiesOID
    {
        /// <summary>
        /// 样本分析s
        /// </summary>
        public YBFenXis() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new YBFenXi();
            }
        }
    }
}
