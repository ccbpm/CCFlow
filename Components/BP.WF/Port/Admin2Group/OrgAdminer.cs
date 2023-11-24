using BP.En;
using BP.Port;
using BP.WF.Template;

namespace BP.WF.Port.Admin2Group
{
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OrgAdminerAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgNo = "OrgNo";
        public const string FlowSorts = "FlowSorts";
        public const string FrmTrees = "FrmTrees";
        public const string EmpName = "EmpName";
    }
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OrgAdminer : EntityMyPK
    {
        #region 属性
        public string FrmTrees
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.FrmTrees);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.FrmTrees, value);
            }
        }
        public string FlowSorts
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.FlowSorts);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.FlowSorts, value);
            }
        }
        public string EmpName
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.EmpName);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.EmpName, value);
            }
        }

        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.FK_Emp, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                //uac.IsDelete = false;
                // uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 组织管理员
        /// </summary>
        public OrgAdminer()
        {
        }
        /// <summary>
        /// 组织管理员
        /// </summary>
        /// <param name="mypk"></param>
        public OrgAdminer(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 组织管理员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_OrgAdminer", "组织管理员");

                //不能注释掉.
                map.AddMyPK(false);

                map.AddTBString(OrgAdminerAttr.OrgNo, null, "组织", false, false, 0, 100, 20);
                map.AddTBString(OrgAdminerAttr.FK_Emp, null, "管理员账号", true, true, 0, 100, 20);
                map.AddTBString(OrgAdminerAttr.EmpName, null, "管理员名称", true, true, 0, 50, 20);

                map.AddTBStringDoc(OrgAdminerAttr.FlowSorts, null, "管理的流程目录", true, true, true);
                map.AddTBStringDoc(OrgAdminerAttr.FrmTrees, null, "管理的表单目录", true, true, true);

                map.AttrsOfOneVSM.AddGroupPanelModel(new OAFlowSorts(), new BP.WF.Template.FlowSorts(),
                  OAFlowSortAttr.RefOrgAdminer,
                  OAFlowSortAttr.FlowSortNo, "流程目录权限");

                map.AttrsOfOneVSM.AddGroupPanelModel(new OAFrmTrees(), new SysFormTrees(),
                OAFrmTreeAttr.RefOrgAdminer,
                OAFrmTreeAttr.FrmTreeNo, "表单目录权限");

                if (BP.Web.WebUser.No != null)
                    map.AddHidden("OrgNo", " = ", "@WebUser.OrgNo");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            string str = "";
            BP.WF.Template.FlowSorts ens = new Template.FlowSorts();
            ens.RetrieveInSQL("SELECT FlowSortNo FROM Port_OrgAdminerFlowSort WHERE  FK_Emp='" + this.EmpNo + "' AND OrgNo='" + this.OrgNo + "'");
            foreach (BP.WF.Template.FlowSort item in ens)
                str += "(" + item.No + ")" + item.Name + ";";
            this.FlowSorts = str;

            str = "";
            SysFormTrees enTrees = new SysFormTrees();
            enTrees.RetrieveInSQL("SELECT FrmTreeNo FROM Port_OrgAdminerFrmTree WHERE  FK_Emp='" + this.EmpNo + "' AND OrgNo='" + this.OrgNo + "'");
            foreach (SysFormTree item in enTrees)
                str += "(" + item.No + ")" + item.Name + ";";
            this.FrmTrees = str;

            if (this.EmpName=="" || this.EmpName ==null)
            {
                Emp emp = new Emp(this.EmpNo);
                this.EmpName = emp.Name;
                this.MyPK = this.OrgNo + "_" + this.EmpNo;
            }
           

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 组织管理员s
    /// </summary>
    public class OrgAdminers : EntitiesMyPK
    {
        public override int RetrieveAll()
        {
            if (BP.Web.WebUser.No != null && BP.Web.WebUser.No.Equals("admin") == false)
                return this.Retrieve(OrgAdminerAttr.OrgNo, BP.Web.WebUser.OrgNo);

            return base.RetrieveAll();
        }

        #region 构造
        /// <summary>
        /// 组织s
        /// </summary>
        public OrgAdminers()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgAdminer();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgAdminer> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgAdminer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgAdminer> Tolist()
        {
            System.Collections.Generic.List<OrgAdminer> list = new System.Collections.Generic.List<OrgAdminer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgAdminer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
