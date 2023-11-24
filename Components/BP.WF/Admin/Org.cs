using System;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Port.Admin2Group;

namespace BP.WF.Admin
{
    /// <summary>
    /// 独立组织属性
    /// </summary>
    public class OrgAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 管理员帐号
        /// </summary>
        public const string Adminer = "Adminer";
        /// <summary>
        /// 管理员名称
        /// </summary>
        public const string AdminerName = "AdminerName";
        /// <summary>
        /// 父级组织编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public const string ParentName = "ParentName";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 独立组织
    /// </summary>
    public class Org : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父级组织编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public string ParentName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentName);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentName, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 管理员名称
        /// </summary>
        public string AdminerName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AdminerName);
            }
            set
            {
                this.SetValByKey(OrgAttr.AdminerName, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 独立组织
        /// </summary>
        public Org() { }
        /// <summary>
        /// 独立组织
        /// </summary>
        /// <param name="no">编号</param>
        public Org(string no) : base(no) { }
        #endregion

        #region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Org", "系统信息");

                map.AddTBStringPK(OrgAttr.No, null, "编号", true, true, 1, 50, 40);
                map.AddTBString(OrgAttr.Name, null, "单位", true, false, 0, 60, 200);

                map.AddTBInt("FlowNums", 0, "流程模板数", true, true);
                map.AddTBInt("FrmNums", 0, "表单模板数", true, true);
                map.AddTBInt("Users", 0, "用户数", true, true);
                map.AddTBInt("Depts", 0, "部门数", true, true);
                map.AddTBInt("GWFS", 0, "运行中流程", true, true);
                map.AddTBInt("GWFSOver", 0, "完成流程", true, true);

                map.AddTBString(OrgAttr.Adminer, null, "管理员(创始人)", true, true, 0, 60, 200);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 60, 200);

                #region 低代码.
                RefMethod rm = new RefMethod();
                rm.GroupName = "低代码";
                rm.Title = "菜单体系";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".LowCodeList";
                rm.Icon = "icon-grid";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "低代码";
                rm.Title = "新建系统";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".LowCodeNew";
                rm.Icon = "icon-folder";
                map.AddRefMethod(rm);
                #endregion 流程管理.

                #region 流程管理.
                rm = new RefMethod();
                rm.GroupName = "流程";
                rm.Title = "流程模板";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FlowTemplate";
                rm.Icon = "icon-grid";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "流程";
                rm.Title = "模板目录";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FlowSorts";
                rm.Icon = "icon-folder";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "流程";
                rm.Title = "流程实例";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FlowGenerWorkFlowView";
                rm.Icon = "icon-layers";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "流程";
                rm.Title = "流程分析";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FlowRptWhite";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);
                #endregion 流程管理.

                #region 表单管理.
                rm = new RefMethod();
                rm.GroupName = "表单";
                rm.Title = "表单模板";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FrmTemplate";
                rm.Icon = "icon-grid";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "表单";
                rm.Title = "目录";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FrmSort";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "表单";
                rm.Title = "外键";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FrmFK";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "表单";
                rm.Title = "数据源";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".FrmDBSrc";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);
                #endregion 表单管理.

                #region 组织管理.
                rm = new RefMethod();
                rm.GroupName = "组织";
                rm.Title = "组织结构";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".OrgUrl";
                rm.Icon = "icon-grid";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "组织";
                rm.Title = "角色";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".OrgStation";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "组织";
                rm.Title = "角色类型";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".OrgStationType";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "组织";
                rm.Title = "部门";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".OrgDept";
                rm.Icon = "icon-chart";
                map.AddRefMethod(rm);
                #endregion 组织管理.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 低代码.
        public string LowCodeList()
        {
            return "/WF/GPM/SystemList.htm";
        }
        public string LowCodeNew()
        {
            return "/WF/GPM/NewSystem.htm";
        }
        #endregion 低代码.

        #region 组织.
        public string OrgUrl()
        {
            return "/GPM/Organization.htm";
        }
        public string OrgStation()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Port.Stations";
        }
        public string OrgStationType()
        {
            return "/WF/Comm/Ens.htm?EnsName=BP.Port.StationTypes";
        }
        public string OrgDept()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Port.Depts";
        }
        #endregion 组织.

        #region 表单.
        public string FrmFK()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Sys.SFTables";
        }
        public string FrmEnum()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Sys.EnumMains";
        }
        public string FrmDBSrc()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.Sys.SFDBSrcs";
        }

        public string FrmTemplate()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.WF.Template.Frms";
        }
        public string FrmSort()
        {
            return "/WF/Comm/Ens.htm?EnsName=BP.WF.Template.FrmSorts";
        }
        #endregion 表单.

        #region 流程模板.
        public string FlowTemplate()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.WF.Admin.Flows";
        }
        public string FlowSorts()
        {
            return "/WF/Comm/Ens.htm?EnsName=BP.WF.Template.FlowSorts";
        }
        public string FlowGenerWorkFlowView()
        {
            return "/WF/Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews";
        }

        public string FlowRptWhite()
        {
            return "/WF/Comm/Group.htm?EnsName=BP.WF.Data.GenerWorkFlowViews";
        }
        #endregion 流程模板

        protected override bool beforeUpdateInsertAction()
        {

            this.SetValByKey("FlowNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_Flow WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("FrmNums", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Sys_MapData WHERE OrgNo='" + WebUser.OrgNo + "'"));

            this.SetValByKey("Users", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Emp WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("Depts", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM Port_Dept WHERE OrgNo='" + WebUser.OrgNo + "'"));
            this.SetValByKey("GWFS", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + WebUser.OrgNo + "' AND WFState!=3"));
            this.SetValByKey("GWFSOver", DBAccess.RunSQLReturnValInt("SELECT COUNT(*) AS a FROM WF_GenerWorkFlow WHERE OrgNo='" + WebUser.OrgNo + "' AND WFState=3"));

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    ///独立组织集合
    /// </summary>
    public class Orgs : EntitiesNoName
    {

        #region 构造.
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Org();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Orgs()
        {
        }
        #endregion 构造.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Org> ToJavaList()
        {
            return (System.Collections.Generic.IList<Org>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Org> Tolist()
        {
            System.Collections.Generic.List<Org> list = new System.Collections.Generic.List<Org>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Org)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
