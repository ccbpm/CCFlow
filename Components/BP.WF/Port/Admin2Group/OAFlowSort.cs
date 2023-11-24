using BP.En;
using BP.WF.Template;

namespace BP.WF.Port.Admin2Group
{
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OAFlowSortAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 关联的二级管理员
        /// </summary>
        public const string RefOrgAdminer = "RefOrgAdminer";
        /// <summary>
        /// 管理员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// FlowSortNo
        /// </summary>
        public const string FlowSortNo = "FlowSortNo";
    }
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OAFlowSort : EntityMyPK
    {
        #region 属性
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(OAFlowSortAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(OAFlowSortAttr.FK_Emp, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(OAFlowSortAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(OAFlowSortAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 组织管理员
        /// </summary>
        public OAFlowSort()
        {
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
                Map map = new Map("Port_OrgAdminerFlowSort", "组织管理员流程目录权限");
                map.AddMyPK();
                map.AddTBString(OAFlowSortAttr.OrgNo, null, "组织", true, false, 0, 100, 20);
                map.AddTBString(OAFlowSortAttr.FK_Emp, null, "管理员", true, false, 0, 100, 20);
                map.AddTBString(OAFlowSortAttr.RefOrgAdminer, null, "组织管理员", true, false, 0, 100, 20);

                //map.AddDDLEntities(OAFlowSortAttr.FK_Emp, null, "管理员", new Emps(), false);
                //map.AddDDLEntities(OAFlowSortAttr.RefOrgAdminer, null, "管理员", new Emps(), false);
                map.AddDDLEntities(OAFlowSortAttr.FlowSortNo, null, "流程目录", new BP.WF.Template.FlowSorts(), false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            string str = this.GetValStringByKey("RefOrgAdminer");

            this.MyPK = str + "_" + this.GetValStringByKey("FlowSortNo");
            OrgAdminer oa = new OrgAdminer(str);
            this.OrgNo = oa.OrgNo;
            this.EmpNo = oa.EmpNo;
            return base.beforeInsert();
        }
       
       protected override void afterInsert()
       {
            //插入入后更改OrgAdminer中
            string str = "";
            FlowSorts ens = new FlowSorts();
            ens.RetrieveInSQL("SELECT FlowSortNo FROM Port_OrgAdminerFlowSort WHERE  FK_Emp='" + this.EmpNo + "' AND OrgNo='" + this.OrgNo + "'");
		    foreach (FlowSort item in ens)
		    {
			    str += "(" + item.No + ")" + item.Name + ";";
		    }
            OrgAdminer adminer = new OrgAdminer(this.GetValStringByKey("RefOrgAdminer"));
            adminer.SetValByKey("FlowSorts", str);
		    adminer.Update();
            base.afterInsert();
	   }
     }
    /// <summary>
    /// 组织管理员s
    /// </summary>
    public class OAFlowSorts : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 组织s
        /// </summary>
        public OAFlowSorts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OAFlowSort();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OAFlowSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<OAFlowSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OAFlowSort> Tolist()
        {
            System.Collections.Generic.List<OAFlowSort> list = new System.Collections.Generic.List<OAFlowSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OAFlowSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
