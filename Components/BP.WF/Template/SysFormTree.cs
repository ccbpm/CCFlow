using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Difference;
using BP.WF.Port.Admin2Group;

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
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
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
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(SysFormTreeAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(SysFormTreeAttr.OrgNo, value);
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
                //map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(SysFormTreeAttr.No, null, "编号", true, true, 1, 50, 40);
                map.AddTBString(SysFormTreeAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(SysFormTreeAttr.ParentNo, null, "父节点编号", false, false, 0, 100, 40);
                map.AddTBInt(SysFormTreeAttr.Idx, 0, "Idx", false, false);
                map.AddTBString(SysFormTreeAttr.OrgNo, null, "组织编号", false, false, 0, 50, 30);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoCreateSameLevelFormNodeMy(string name)
        {
            EntityTree en = this.DoCreateSameLevelNode(name);
            en.Name = name;
            en.Update();
            return en.No;
        }
        /// <summary>
        /// 创建下级目录.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string DoCreateSubFormNodeMy(string name)
        {
            EntityTree en = this.DoCreateSubNode(name);
            en.Name = name;
            en.Update();
            return en.No;
        }
        #endregion 系统方法.

        /// <summary>
        /// 组织编号
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.OrgNo)==true && Glo.CCBPMRunModel != CCBPMRunModel.Single)
                this.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
           
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerOID(this.ToString()).ToString();
            return base.beforeInsert();
        }

        protected override bool beforeDelete()
        {
            string sql = "SELECT COUNT(*) as Num FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num != 0)
                throw new Exception("err@您不能删除该目录，下面有表单。");


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
            en.No = DBAccess.GenerGUID(10); 
            en.Name = name;
            en.Insert();
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc && SystemConfig.GroupStationModel == 2)
            {
                //如果当前人员不是部门主要管理员
                BP.WF.Admin.Org org = new BP.WF.Admin.Org(BP.Web.WebUser.OrgNo);
                if (BP.Web.WebUser.No.Equals(org.Adminer) == false)
                {
                    OAFrmTree oatree = new OAFrmTree();
                    oatree.FK_Emp = BP.Web.WebUser.No;
                    oatree.OrgNo = BP.Web.WebUser.OrgNo;
                    oatree.SetValByKey("RefOrgAdminer", oatree.OrgNo + "_" + oatree.FK_Emp);
                    oatree.SetValByKey("FrmTreeNo", en.No);
                    oatree.Insert();
                }
            }
            return en.No;
        }
        public string DoCreateSubNodeIt(string name)
        {
            SysFormTree en = new SysFormTree();
            en.Copy(this);
            en.No = DBAccess.GenerGUID(10); 
            en.ParentNo = this.No;
            en.Name = name;
            en.Insert();
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc && SystemConfig.GroupStationModel == 2)
            {
                //如果当前人员不是部门主要管理员
                BP.WF.Admin.Org org = new BP.WF.Admin.Org(BP.Web.WebUser.OrgNo);
                if (BP.Web.WebUser.No.Equals(org.Adminer) == false)
                {
                    OAFrmTree oatree = new OAFrmTree();
                    oatree.FK_Emp = BP.Web.WebUser.No;
                    oatree.OrgNo = BP.Web.WebUser.OrgNo;
                    oatree.SetValByKey("RefOrgAdminer", oatree.OrgNo + "_" + oatree.FK_Emp);
                    oatree.SetValByKey("FrmTreeNo", en.No);
                    oatree.Insert();
                }
            }
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
        /// <summary>
        /// 初始化数据.
        /// </summary>
        private void InitData()
        {
            SysFormTree fs = new SysFormTree();
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                fs.setName("流程树");
                fs.setNo(BP.Web.WebUser.OrgNo);
                fs.setParentNo("100");
                fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
                fs.Save(); 

                fs = new SysFormTree();
                fs.setName("公文类");
                fs.setNo(DBAccess.GenerGUID());
                fs.setParentNo(BP.Web.WebUser.OrgNo);
                fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
                fs.Insert();

                fs = new SysFormTree();
                fs.setName("办公类");
                fs.setNo(DBAccess.GenerGUID());
                fs.setParentNo(BP.Web.WebUser.OrgNo);
                fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
                fs.Insert();
                return;
            }

            fs = new SysFormTree();
            fs.Name = "流程树";
            fs.No = "100";
            fs.ParentNo = "0";
            fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
            fs.Insert();

            fs = new SysFormTree();
            fs.setName("办公类");
            fs.setNo("01");
            fs.setParentNo("100");
            fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
            fs.Insert();

            fs = new SysFormTree();
            fs.setName("公文类");
            fs.setNo("01");
            fs.setParentNo("100");
            fs.SetValByKey("OrgNo", BP.Web.WebUser.OrgNo);
            fs.Insert();

        }
        /// <summary>
        /// 查询全部.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No.Equals("admin") == true)
                return this.RetrieveAll(FlowSortAttr.Idx);

            var num = 0;
            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                num = this.Retrieve(FlowSortAttr.OrgNo, BP.Web.WebUser.OrgNo, FlowSortAttr.Idx);

            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                num = this.Retrieve(FlowSortAttr.Idx);

            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                num = this.Retrieve(FlowSortAttr.OrgNo, BP.Web.WebUser.OrgNo, FlowSortAttr.Idx);

            if (num == 0)
            {
                InitData();
                return this.RetrieveAll();
            }

            return num;
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
