using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.ZHOU
{
    /// <summary>
    /// 家谱属性
    /// </summary>
    public class JiaPuAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string PeiO = "PeiO";
        public const string Remark = "Remark";
        public const string XB = "XB";

        public const string Dai = "Dai";
        public const string Shi = "Shi";
        public const string PaiHang = "PaiHang";

        public const string RDT = "RDT";
        public const string Rec = "Rec";
        public const string XingShi = "XingShi";
        public const string PeopleNo = "PeopleNo";
        public const string PeopleName = "PeopleName";

        public const string TangHao = "TangHao";

        public const string Addr = "Addr";
        public const string Tel = "Tel";
    }
    /// <summary>
    ///  家谱
    /// </summary>
    public class JiaPu : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 开基祖
        /// </summary>
        public string PeopleNo
        {
            get
            {
                return this.GetValStrByKey(JiaPuAttr.PeopleNo);
            }
            set
            {
                this.SetValByKey(JiaPuAttr.PeopleNo, value);
            }
        }
        public string PeopleName
        {
            get
            {
                return this.GetValStrByKey(JiaPuAttr.PeopleName);
            }
            set
            {
                this.SetValByKey(JiaPuAttr.PeopleName, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(JiaPuAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(JiaPuAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStrByKey(JiaPuAttr.Rec);
            }
            set
            {
                this.SetValByKey(JiaPuAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(JiaPuAttr.RDT);
            }
            set
            {
                this.SetValByKey(JiaPuAttr.RDT, value);
            }
        }

        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 家谱
        /// </summary>
        public JiaPu()
        {
        }
        /// <summary>
        /// 家谱
        /// </summary>
        /// <param name="_No"></param>
        public JiaPu(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 家谱Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZP_JiaPu", "家谱");

                map.AddTBStringPK(JiaPuAttr.No, null, "编号", true, true, 1, 100, 20);
                map.AddTBString(JiaPuAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(JiaPuAttr.XingShi, null, "姓氏", true, false, 0, 100, 30);
                map.AddTBString(JiaPuAttr.PeopleNo, null, "开基祖-编号", true, false, 0, 100, 30);
                map.AddTBString(JiaPuAttr.PeopleName, null, "开基祖-名称", true, false, 0, 100, 30);

                map.AddTBString(JiaPuAttr.TangHao, null, "堂号", true, false, 0, 100, 30);


                map.AddTBString(JiaPuAttr.Addr, null, "详细地址", true, false, 0, 100, 30);

                map.AddTBString(JiaPuAttr.Tel, null, "联系电话", true, false, 0, 100, 30);

                //map.AddTBString(JiaPuAttr.ShengFen, null, "省份", true, false, 0, 100, 30);
                //map.AddTBString(JiaPuAttr.DiShi, null, "地市", true, false, 0, 100, 30);
                //map.AddTBString(JiaPuAttr.QuXian, null, "区县", true, false, 0, 100, 30);
                //map.AddTBString(JiaPuAttr.QuXian, null, "街道/乡镇", true, false, 0, 100, 30);
                //map.AddTBString(JiaPuAttr.QuXian, null, "地址", true, false, 0, 100, 30);

                map.AddTBString(JiaPuAttr.Remark, null, "备注", true, false, 0, 100, 30, true);

             //   map.AddTBString(JiaPuAttr.PuShu, null, "普书", true, false, 0, 4000, 30);

                //系统信息.
                map.AddTBDateTime(JiaPuAttr.RDT, null, "记录日期", true, false);
                map.AddTBString(JiaPuAttr.Rec, null, "记录人", true, false, 0, 100, 30);
                map.AddTBString(JiaPuAttr.OrgNo, null, "OrgNo", true, false, 0, 100, 30);

                map.AddTBInt("Idx",0, "Idx", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            this.Rec = BP.Web.WebUser.No;
            this.RDT = DataType.CurrentDateTime;

            //初始化该家谱的根目录.
            People pe = new People();
            pe.Name = this.PeopleName;
            pe.ParentNo = "0";
            pe.JiaPuNo = this.No;
            pe.Insert();

            this.PeopleNo = pe.No;

            return base.beforeInsert();
        }

        protected override void afterInsert()
        {
         


            base.afterInsert();
        }

        public string DoShowView1()
        {
            return "/App/Tree/zhou.html?RootNo=" + this.No;
        }

        public string DoShowView2()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;
        }

        public string DoShowView3()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;
            JiaPu en = new JiaPu(this.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView4()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            JiaPu en = new JiaPu(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new JiaPu(en.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView5()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            JiaPu en = new JiaPu(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new JiaPu(en.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            en = new JiaPu(en.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }


        public string DoMyCreateSubNode()
        {
            EntityTree en = this.CreateInstance() as EntityTree;
            en.No = BP.DA.DBAccess.GenerOID(this.ToString()).ToString(); // en.GenerNewNoByKey(EntityTreeAttr.No);
            en.Name = "新建节点" + en.No;
            en.ParentNo = this.No;
            en.Insert();
            return en.No;
        }

        public string DoMyCreateSameLevelNode()
        {
            EntityTree en = this.CreateInstance() as EntityTree;
            en.No = BP.DA.DBAccess.GenerOID(this.ToString()).ToString(); // en.GenerNewNoByKey(EntityTreeAttr.No);
            en.Name = "新建节点" + en.No;
            en.ParentNo = this.ParentNo;
            en.Insert();
            return en.No;
        }
    }
    /// <summary>
    /// 家谱
    /// </summary>
    public class JiaPus : EntitiesTree
    {
        /// <summary>
        /// 家谱s
        /// </summary>
        public JiaPus() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new JiaPu();
            }
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll(JiaPuAttr.Idx);
            if (i == 0)
            {
                JiaPu fs = new JiaPu();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new JiaPu();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }
            return i;
        }

    }
}
