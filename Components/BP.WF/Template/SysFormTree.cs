using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 属性
    /// </summary>
    public class SysFormTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 是否是目录
        /// </summary>
        public const string IsDir = "IsDir";
    }
    /// <summary>
    ///  独立表单树
    /// </summary>
    public class SysFormTree : EntityTree
    {
        #region 属性.
        /// <summary>
        /// 是否是目录
        /// </summary>
        public bool IsDir
        {
            get
            {
                return this.GetValBooleanByKey(SysFormTreeAttr.IsDir);
            }
            set
            {
                this.SetValByKey(SysFormTreeAttr.IsDir, value);
            }
        }
        /// <summary>
        /// 序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(SysFormTreeAttr.Idx);
            }
            set
            {
                this.SetValByKey(SysFormTreeAttr.Idx, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey(SysFormTreeAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(SysFormTreeAttr.ParentNo, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 独立表单树
        /// </summary>
        public SysFormTree()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        /// <param name="_No"></param>
        public SysFormTree(string _No) : base(_No) { }
        #endregion

        #region 系统方法.
        /// <summary>
        /// 独立表单树Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FormTree", "表单树");
                map.Java_SetCodeStruct("2");

                map.Java_SetDepositaryOfEntity(Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddTBStringPK(SysFormTreeAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(SysFormTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(SysFormTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBInt(SysFormTreeAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion 系统方法.

        protected override bool beforeDelete()
        {
            if (!DataType.IsNullOrEmpty(this.No))
                DeleteChild(this.No);
            return base.beforeDelete();
        }
        /// <summary>
        /// 删除子项
        /// </summary>
        /// <param name="parentNo"></param>
        private void DeleteChild(string parentNo)
        {
            SysFormTrees formTrees = new SysFormTrees();
            formTrees.RetrieveByAttr(SysFormTreeAttr.ParentNo, parentNo);
            foreach (SysFormTree item in formTrees)
            {
                MapData md = new MapData();
                md.FK_FormTree = item.No;
                md.Delete();
                DeleteChild(item.No);
            }
        }
        public string DoCreateSameLevelNodeIt(string name)
        {
            SysFormTree en = new SysFormTree();
            en.Copy(this);
            en.No = BP.DA.DBAccess.GenerOID().ToString();
            en.Name = name;
            en.Insert();
            return en.No;
        }
        public string DoCreateSubNodeIt(string name)
        {
            SysFormTree en = new SysFormTree();
            en.Copy(this);
            en.No = BP.DA.DBAccess.GenerOID().ToString();
            en.ParentNo = this.No;
            en.Name = name;
            en.Insert();
            return en.No;
        }
        public void DoUp()
        {
            this.DoOrderUp(SysFormTreeAttr.ParentNo, this.ParentNo, SysFormTreeAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(SysFormTreeAttr.ParentNo, this.ParentNo, SysFormTreeAttr.Idx);
        }
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class SysFormTrees : EntitiesTree
    {
        /// <summary>
        /// 独立表单树s
        /// </summary>
        public SysFormTrees() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SysFormTree();
            }

        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                SysFormTree fs = new SysFormTree();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new SysFormTree();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }
            return i;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SysFormTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<SysFormTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SysFormTree> Tolist()
        {
            System.Collections.Generic.List<SysFormTree> list = new System.Collections.Generic.List<SysFormTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SysFormTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
