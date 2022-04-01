using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;

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

    }
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OrgAdminer : EntityMyPK
    {
        #region 属性
        public string FK_Emp
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
                map.AddMyPK();
                map.AddTBString(OrgAdminerAttr.OrgNo, null, "组织", true, false, 0, 50, 20);
                map.AddDDLEntities(OrgAdminerAttr.FK_Emp, null, "管理员", new Emps(), false);

                map.AddTBStringDoc("FlowSorts", null, "管理的流程目录", true, true,true);
                map.AddTBStringDoc("FrmTrees", null, "管理的表单目录", true, true, true);

                map.AttrsOfOneVSM.AddGroupPanelModel(new OAFlowSorts(), new BP.WF.Template.FlowSorts(),
                  OAFlowSortAttr.RefOrgAdminer,
                  OAFlowSortAttr.FlowSortNo, "流程目录权限");

                map.AttrsOfOneVSM.AddGroupPanelModel(new OAFrmTrees(), new BP.Sys.FrmTrees(),
                OAFrmTreeAttr.RefOrgAdminer,
                OAFrmTreeAttr.FrmTreeNo, "表单目录权限");

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
            ens.RetrieveInSQL("SELECT FlowSortNo FROM Port_OrgAdminerFlowSort WHERE  FK_Emp='"+this.FK_Emp+"' AND OrgNo='"+this.OrgNo+"'");
            foreach (BP.WF.Template.FlowSort item in ens)
                str += "(" + item.No + ")" + item.Name+";";
            this.SetValByKey("FlowSorts", str);

            str = "";
            BP.Sys.FrmTrees enTrees = new BP.Sys.FrmTrees();
            enTrees.RetrieveInSQL("SELECT FrmTreeNo FROM Port_OrgAdminerFrmTree WHERE  FK_Emp='" + this.FK_Emp + "' AND OrgNo='" + this.OrgNo + "'");
            foreach (BP.Sys.FrmTree item in enTrees)
                str += "("+item.No+")"+item.Name + ";";
            this.SetValByKey("FrmTrees", str);

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
            return this.Retrieve(OrgAdminerAttr.OrgNo, BP.Web.WebUser.OrgNo);
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
