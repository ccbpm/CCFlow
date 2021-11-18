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
    /// 族谱树属性
    /// </summary>
    public class ZuPuTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string PeiO = "PeiO";
        public const string Remark = "Remark";
        public const string XB = "XB";
        /// <summary>
        /// 代
        /// </summary>
        public const string Dai = "Dai";
        /// <summary>
        /// 世
        /// </summary>
        public const string Shi = "Shi";
        public const string PaiHang = "PaiHang";

    }
    /// <summary>
    ///  族谱树
    /// </summary>
    public class ZuPuTree : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(ZuPuTreeAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(ZuPuTreeAttr.OrgNo, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 族谱树
        /// </summary>
        public ZuPuTree()
        {
        }
        /// <summary>
        /// 族谱树
        /// </summary>
        /// <param name="_No"></param>
        public ZuPuTree(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 族谱树Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Zhou_Tree", "族谱树");

                map.AddTBStringPK(ZuPuTreeAttr.No, null, "编号", true, true, 1, 100, 20);
                map.AddTBString(ZuPuTreeAttr.Name, null, "姓名", true, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.PeiO, null, "配偶", true, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.Dai, null, "代", true, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.Shi, null, "世", true, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.PaiHang, null, "排行", true, false, 0, 100, 30);

                map.AddDDLSysEnum(ZuPuTreeAttr.XB, 1, "性别", true, true, "XB", "@0=女@1=男");
                map.AddTBInt(ZuPuTreeAttr.Idx, 0, "序号", true, false);
                //map.AddTBString(ZuPuTreeAttr.Chapter, null, "Chapter", true, false, 0, 100, 30);
                //map.AddTBString(ZuPuTreeAttr.Remark, null, "Remark", true, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(ZuPuTreeAttr.Remark, null, "备注", true, false, 0, 100, 30, true);

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

        public string DoShowView1()
        {
            return "/App/Tree/zhou.html?RootNo="+this.No;
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
            ZuPuTree en = new ZuPuTree(this.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView4()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            ZuPuTree en = new ZuPuTree(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new ZuPuTree(en.ParentNo);
            return "/App/Tree/zhou.html?RootNo=" + en.ParentNo;
        }

        public string DoShowView5()
        {
            if (this.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            ZuPuTree en = new ZuPuTree(this.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;


            en = new ZuPuTree(en.ParentNo);
            if (en.ParentNo == "0")
                return "/App/Tree/zhou.html?RootNo=" + this.ParentNo;

            en = new ZuPuTree(en.ParentNo);
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
    /// 族谱树
    /// </summary>
    public class ZuPuTrees : EntitiesTree
    {
        /// <summary>
        /// 族谱树s
        /// </summary>
        public ZuPuTrees() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ZuPuTree();
            }
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll( ZuPuTreeAttr.Idx );
            if (i == 0)
            {
                ZuPuTree fs = new ZuPuTree();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new ZuPuTree();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }
            return i;
        }
         
    }
}
