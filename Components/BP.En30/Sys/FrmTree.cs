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
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    ///  独立表单树
    /// </summary>
    public class FrmTree : EntityTree
    {
        #region 属性.
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
                map.CodeStruct = "2";

                map.IndexField = FrmTreeAttr.ParentNo; 


                map.AddTBStringPK(FrmTreeAttr.No, null, "编号", true, true, 1, 10, 40);
                map.AddTBString(FrmTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(FrmTreeAttr.ParentNo, null, "父节点No", false, false, 0, 100, 40);
                map.AddTBString(FrmTreeAttr.OrgNo, null, "组织编号", false, false, 0, 50, 40);
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
            formTrees.Retrieve(FrmTreeAttr.ParentNo, parentNo);
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
            en.No = DBAccess.GenerOID().ToString();
            en.Name = "新建节点";
            en.Insert();
            return en;
        }
        public FrmTree DoCreateSameLevelNodeMy(string dirName)
        {
            FrmTree en = new FrmTree();
            en.Copy(this);
            en.No = DBAccess.GenerOID().ToString();
            en.Name = dirName;
            en.Insert();
            return en;
        }
        public FrmTree DoCreateSubNode()
        {
            FrmTree en = new FrmTree();
            en.Copy(this);
            en.No = DBAccess.GenerOID().ToString();
            en.ParentNo = this.No;
            en.Name = "新建节点";
            en.Insert();
            return en;
        }
        /// <summary>
        /// 创建子目录 
        /// </summary>
        /// <param name="dirName">要创建的子目录名字</param>
        /// <returns>返回子目录编号</returns>
        public string CreateSubNode(string dirName)
        {
            FrmTree en = new FrmTree();
            en.Copy(this);
            en.No = DBAccess.GenerOID().ToString();
            en.ParentNo = this.No;
            en.Name = dirName;
            en.Insert();
            return en.No;
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(FrmTreeAttr.ParentNo, this.ParentNo, FrmTreeAttr.Idx);
            return "移动成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(FrmTreeAttr.ParentNo, this.ParentNo, FrmTreeAttr.Idx);
            return "移动成功";
        }
    }
    /// <summary>
    /// 独立表单树
    /// </summary>
    public class FrmTrees : EntitiesTree
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
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo);

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
