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
    /// 人员属性
    /// </summary>
    public class PeopleAttr : EntityTreeAttr
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

        public const string Bei = "Bei";

        public const string JiaPuNo = "JiaPuNo";


        public const string OpenID = "OpenID";
        public const string WXName = "WXName";
        public const string WXTel = "WXTel";

        public const string PeiOName = "PeiOName";
        public const string PeiOAddr = "PeiOAddr";
        public const string PeiOTel = "PeiOTel";
        public const string PeiONote = "PeiONote";
        public const string PeiOOpenID = "PeiOOpenID";
        public const string PeiOWXName = "PeiOWXName";
        public const string PeiOWXTel = "PeiOWXTel";
    }
    /// <summary>
    ///  人员
    /// </summary>
    public class People : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(PeopleAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Rec);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Rec, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.RDT);
            }
            set
            {
                this.SetValByKey(PeopleAttr.RDT, value);
            }
        }
        public string Bei
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.Bei);
            }
            set
            {
                this.SetValByKey(PeopleAttr.Bei, value);
            }
        }

        /// <summary>
        /// 隶属的家谱
        /// </summary>
        public string JiaPuNo
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.JiaPuNo);
            }
            set
            {
                this.SetValByKey(PeopleAttr.JiaPuNo, value);
            }
        }
        /// <summary>
        /// 微信ID
        /// </summary>
        public string OpenID
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.OpenID);
            }
            set
            {
                this.SetValByKey(PeopleAttr.OpenID, value);
            }
        }
        /// <summary>
        /// 微信名称
        /// </summary>
        public string WXName
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.WXName);
            }
            set
            {
                this.SetValByKey(PeopleAttr.WXName, value);
            }
        }
        /// <summary>
        /// 微信电话
        /// </summary>
        public string WXTel
        {
            get
            {
                return this.GetValStrByKey(PeopleAttr.WXTel);
            }
            set
            {
                this.SetValByKey(PeopleAttr.WXTel, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 人员
        /// </summary>
        public People()
        {
        }
        /// <summary>
        /// 人员
        /// </summary>
        /// <param name="_No"></param>
        public People(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 人员Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZP_Pople", "人员");

                map.AddTBStringPK(PeopleAttr.No, null, "编号", true, true, 1, 100, 20);
                map.AddTBString(PeopleAttr.Name, null, "姓名", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBString(PeopleAttr.JiaPuNo, null, "家谱编号", false, false, 0, 100, 30);


                map.AddTBString(PeopleAttr.Dai, null, "代", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.Shi, null, "世", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.Bei, null, "辈", true, false, 0, 100, 30);

                map.AddTBString(PeopleAttr.PaiHang, null, "排行", true, false, 0, 100, 30);
                map.AddDDLSysEnum(PeopleAttr.XB, 1, "性别", true, true, "XB", "@0=女@1=男");
                map.AddTBInt(PeopleAttr.Idx, 0, "序号", true, false);
                map.AddTBString(PeopleAttr.Remark, null, "备注", true, false, 0, 100, 30, true);

                #region 微信信息.
                map.AddTBString(PeopleAttr.OpenID, null, "微信OpenID", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.WXName, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.WXTel, null, "微信电话", true, false, 0, 100, 30);
                #endregion 微信信息.


                #region 配偶信息.
                map.AddTBString(PeopleAttr.PeiOName, null, "配偶姓名", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.PeiOAddr, null, "配偶地址", true, false, 0, 500, 30);
                map.AddTBString(PeopleAttr.PeiOTel, null, "配偶电话", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.PeiONote, null, "配偶备注", true, false, 0, 4000, 30);

                map.AddTBString(PeopleAttr.PeiOOpenID, null, "微信OpenID", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.PeiOWXName, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.PeiOWXTel, null, "微信电话", true, false, 0, 100, 30);
                #endregion 配偶信息.

                //系统信息.
                map.AddTBDateTime(PeopleAttr.RDT, null, "记录日期", true, false);
                map.AddTBString(PeopleAttr.Rec, null, "记录人", true, false, 0, 100, 30);
                map.AddTBString(PeopleAttr.OrgNo, null, "OrgNo", true, false, 0, 100, 30);

                //map.AddTBStringDoc();

                RefMethod rm = new RefMethod();
                rm.Title = "查看父辈家谱";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".DoShowView2";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查看爷辈家谱";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoShowView3";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查看老爷爷辈家谱";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoShowView4";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查看老老爷爷辈家谱";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoShowView5";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查看本人家谱";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.ClassMethodName = this.ToString() + ".DoShowView1";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            this.Rec = BP.Web.WebUser.No;
            this.RDT = DataType.CurrentDataTime;



            return base.beforeInsert();
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
            People en = new People(this.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView4()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            People en = new People(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new People(en.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView5()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            People en = new People(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new People(en.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            en = new People(en.ParentNo);
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
    /// 人员
    /// </summary>
    public class Peoples : EntitiesTree
    {
        /// <summary>
        /// 人员s
        /// </summary>
        public Peoples() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new People();
            }
        }
       
    }
}
