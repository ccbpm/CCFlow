using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.CallCenter
{
    /// <summary>
    /// 工作明细 attr
    /// </summary>
    public class ND101Dtl1Attr : EntityOIDAttr
    {
        /// <summary>
        /// workID
        /// </summary>
        public const string RefPK = "RefPK";
        public const string RDT = "RDT";
        public const string Rec = "Rec";
        public const string Worker = "Worker";
        public const string WorkerName = "WorkerName";

        public const string WorkDate = "WorkDate";
        public const string DTBegin = "DTBegin";
        public const string DTTo = "DTTo";
        

        public const string DaLei = "DaLei";
        public const string XiaoLei = "XiaoLei";
        public const string DaiHao = "DaiHao";
        public const string MingCheng = "MingCheng";
        public const string ShuLiang = "ShuLiang";
        public const string CaiLiao = "CaiLiao";
        public const string DanZhong = "DanZhong";
        public const string BeiZhu = "BeiZhu";
        public const string ChangJia = "ChangJia";
        public const string SuoShuZhuangPei = "SuoShuZhuangPei";
        public const string LiaoHao = "LiaoHao";
        public const string GongYiBeiZhu = "GongYiBeiZhu";
        public const string FeiWuLiaoBiaoShi = "FeiWuLiaoBiaoShi";


        /// <summary>
        /// 0=未处理. 1=已完成. 2=重做. 3=已验收. 4=完成待检查.
        /// </summary>
        public const string WorkSta = "WorkSta";
    }
	/// <summary>
    ///  工作明细
	/// </summary>
    public class ND101Dtl1 : EntityOID
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

        public Int64 RefPK
        {
            get
            {
                return this.GetValInt64ByKey(ND101Dtl1Attr.RefPK);
            }
            set
            {
                this.SetValByKey(ND101Dtl1Attr.RefPK,value);
            }
        }

        #region 构造方法
        /// <summary>
        /// 工作明细
        /// </summary>
        public ND101Dtl1()
        {
        }
        /// <summary>
        /// 工作明细
        /// </summary>
        /// <param name="_No"></param>
        public ND101Dtl1(int _No) : base(_No) { }
        /// <summary>
        /// 工作明细Map
        /// </summary>
        public override Map  EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
              
                Map map = new Map("ND101Dtl1", "工作明细");

                map.AddTBIntPKOID();

                map.AddTBString(ND101Dtl1Attr.RefPK, null, "关联主键", false, true, 0, 50, 50);

                // 0=未处理. 1=已完成. 2=重做. 3=检查合格. 4=完成待检查.
                map.AddTBInt(ND101Dtl1Attr.WorkSta, 0, "状态", false, true);
               

                map.AddTBString(ND101Dtl1Attr.Worker, null, "装配人", false, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.WorkerName, null, "装配人姓名", false, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.WorkDate, null, "工作日期", false, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.DTBegin, null, "工作开始从", false, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.DTTo, null, "工作开始到", false, true, 0, 50, 50);

                map.AddTBString(ND101Dtl1Attr.DaLei, null, "大类", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.XiaoLei, null, "小类", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.DaiHao, null, "代号", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.MingCheng, null, "名称", true, true, 0, 50, 50);
                map.AddTBInt(ND101Dtl1Attr.ShuLiang, 0, "数量", false, false);
                map.AddTBString(ND101Dtl1Attr.CaiLiao, null, "材料", true, true, 0, 50, 50);
               // map.AddTBString(ND101Dtl1Attr.DanZhong, null, "单重", true, true, 0, 50, 50);
                map.AddTBInt(ND101Dtl1Attr.DanZhong, 0, "单重", false, false);
              
                map.AddTBString(ND101Dtl1Attr.ChangJia, null, "厂家", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.SuoShuZhuangPei, null, "所属装配", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.LiaoHao, null, "料号", true, true, 0, 50, 50);
                map.AddTBString(ND101Dtl1Attr.GongYiBeiZhu, null, "工艺备注", true, true, 0, 50, 50);
              //  map.AddTBString(ND101Dtl1Attr.FeiWuLiaoBiaoShi, null, "非物料标识", true, true, 0, 50, 50);
                map.AddBoolean(ND101Dtl1Attr.FeiWuLiaoBiaoShi,true, "非物料标识", true,true);
              map.AddTBString(ND101Dtl1Attr.BeiZhu, null, "备注", true, true, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

       
      
      
    }
	/// <summary>
    /// 工作明细s
	/// </summary>
    public class ND101Dtl1s : EntitiesOID
    {
        /// <summary>
        /// 工作明细s
        /// </summary>
        public ND101Dtl1s() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ND101Dtl1();
            }
        }
    }
}
