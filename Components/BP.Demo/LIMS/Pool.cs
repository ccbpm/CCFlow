using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.LIMS
{
    /// <summary>
    /// 样本库 attr
    /// </summary>
    public class PoolAttr : EntityOIDAttr
    {
        /// <summary>
        /// workID
        /// </summary>
        public const string RefPK = "RefPK";
        public const string RDT = "RDT";
        public const string Rec = "Rec";
        public const string MingCheng = "MingCheng";
        public const string BianHao = "BianHao";
        public const string WorkDate = "WorkDate";
        public const string DTBegin = "DTBegin";
        public const string DTTo = "DTTo";
        public const string DaLei = "DaLei";
        public const string XiaoLei = "XiaoLei";
        public const string DaiHao = "DaiHao";
        public const string ShuLiang = "ShuLiang";
        public const string CaiLiao = "CaiLiao";
        public const string DanZhong = "DanZhong";
        public const string BeiZhu = "BeiZhu";
        public const string ChangJia = "ChangJia";
        public const string SuoShuZhuangPei = "SuoShuZhuangPei";
        public const string LiaoHao = "LiaoHao";
        public const string GongYiBeiZhu = "GongYiBeiZhu";
        public const string WorkIDOfFX = "WorkIDOfFX";
        /// <summary>
        /// 0=未处理. 1=等待检验. 2=检验中. 3=检验完成.
        /// </summary>
        public const string YBSta = "YBSta";
    }
    /// <summary>
    ///  样本库
    /// </summary>
    public class Pool : EntityOID
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
        public int YBSta
        {
            get
            {
                return this.GetValIntByKey(PoolAttr.YBSta);
            }
            set
            {
                this.SetValByKey(PoolAttr.YBSta, value);
            }
        }
        public Int64 RefPK
        {
            get
            {
                return this.GetValInt64ByKey(PoolAttr.RefPK);
            }
            set
            {
                this.SetValByKey(PoolAttr.RefPK, value);
            }
        }
        public Int64 WorkIDOfFX
        {
            get
            {
                return this.GetValInt64ByKey(PoolAttr.WorkIDOfFX);
            }
            set
            {
                this.SetValByKey(PoolAttr.WorkIDOfFX, value);
            }
        }


        #region 构造方法
        /// <summary>
        /// 样本库
        /// </summary>
        public Pool()
        {
        }
        /// <summary>
        /// 样本库
        /// </summary>
        /// <param name="_No"></param>
        public Pool(int _No) : base(_No) { }
        /// <summary>
        /// 样本库Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("YB_Pool", "样本库");

                map.AddTBIntPKOID();

                map.AddTBString(PoolAttr.RefPK, null, "关联主键", false, true, 0, 50, 50);

                //  0=未处理. 1=等待检验. 2=检验中. 3=检验完成.
                map.AddTBInt(PoolAttr.YBSta, 0, "状态", false, true);

                map.AddTBString(PoolAttr.MingCheng, null, "名称", false, true, 0, 50, 50);
                map.AddTBString(PoolAttr.BianHao, null, "编号", false, true, 0, 50, 50);

                map.AddTBInt(PoolAttr.WorkIDOfFX, 0, "WorkIDOfFX-分析的workid", false, true);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


    }
    /// <summary>
    /// 样本库s
    /// </summary>
    public class Pools : EntitiesOID
    {
        /// <summary>
        /// 样本库s
        /// </summary>
        public Pools() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Pool();
            }
        }

        /// <summary>
        /// 执行发起检验检测流程
        /// </summary>
        /// <param name="YangBenOIDs">样本ID, 多个用逗号分开</param>
        /// <returns></returns>
        public string DoStartFlow002(string YangBenOIDs)
        {
            try
            {
                //生成检测的workid.
                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("002");

                //遍历样本.
                string[] strs = YangBenOIDs.Split(',');
                foreach (string mystr in strs)
                {
                    if (DataType.IsNullOrEmpty(mystr) == true)
                        continue;

                    Pool pool = new Pool(int.Parse(mystr));
                    pool.WorkIDOfFX = workid;
                    pool.YBSta = 2; //检验中。
                    pool.Update(); //设置他的分析ID.

                    YBFenXi fx = new YBFenXi();
                    fx.Copy(pool); //复制一下.
                    fx.Delete(); //首先执行删除，怕有垃圾数据.

                    fx.OID = 0;
                    fx.RefPK = workid;
                    fx.WorkIDOfWT = pool.RefPK;
                    fx.InsertAsOID(pool.OID);
                }

                string str = BP.WF.Dev2Interface.Node_SendWork("002", workid, 202, BP.Web.WebUser.No).ToMsgOfText();

                return workid.ToString();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
    }
}
