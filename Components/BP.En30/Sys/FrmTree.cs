using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class FrmTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 是否是目录
        /// </summary>
        public const string IsDir = "IsDir";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    ///  独立表单树
    /// </summary>
    public class FrmTree : EntitySimpleTree
    {
        #region 属性.
        /// <summary>
        /// 是否是目录
        /// </summary>
        public bool IsDir
        {
            get
            {
                return this.GetValBooleanByKey(FrmTreeAttr.IsDir);
            }
            set
            {
                this.SetValByKey(FrmTreeAttr.IsDir, value);
            }
        }
        /// <summary>
        /// 序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmTreeAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmTreeAttr.Idx, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(FrmTreeAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FrmTreeAttr.OrgNo, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 独立表单树
        /// </summary>
        public FrmTree()
        {
        }
        /// <summary>
        /// 独立表单树
        /// </summary>
        /// <param name="_No"></param>
        public FrmTree(string _No) : base(_No) { }
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

                map.AddTBStringPK(FrmTreeAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(FrmTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FrmTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);
                map.AddTBString(FrmTreeAttr.OrgNo, null, "组织编号", false, false, 0, 100, 30);

                map.AddTBInt(FrmTreeAttr.IsDir, 0, "是否是目录?", false, false);
                map.AddTBInt(FrmTreeAttr.Idx, 0, "Idx", false, false);

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
            FrmTrees formTrees = new FrmTrees();
            formTrees.RetrieveByAttr(FrmTreeAttr.ParentNo, parentNo);
            foreach (FrmTree item in formTrees)
            {
                MapData md = new MapData();
                md.FK_FormTree = item.No;
                md.Delete();
                DeleteChild(item.No);
            }
        }
        public FrmTree DoCreateSameLevelNode()
        {
            FrmTree en = new FrmTree();
            en.Copy(this);
            en.No = BP.DA.DBAccess.GenerOID().ToString();
            en.Name = "新建节点";
            en.Insert();
            return en;
        }
        public FrmTree DoCreateSubNode()
        {
            FrmTree en = new FrmTree();
            en.Copy(this);
            en.No = BP.DA.DBAccess.GenerOID().ToString();
            en.ParentNo = this.No;
            en.Name = "新建节点";
            en.Insert();
            return en;
        }
        public void DoUp()
        {
            this.DoOrderUp(FrmTreeAttr.ParentNo, this.ParentNo, FrmTreeAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(FrmTreeAttr.ParentNo, this.ParentNo, FrmTreeAttr.Idx);
        }
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class FrmTrees : EntitiesSimpleTree
    {
        /// <summary>
        /// 独立表单树s
        /// </summary>
        public FrmTrees() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmTree();
            }

        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                FrmTree fs = new FrmTree();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new FrmTree();
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
        public System.Collections.Generic.IList<FrmTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmTree> Tolist()
        {
            System.Collections.Generic.List<FrmTree> list = new System.Collections.Generic.List<FrmTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
