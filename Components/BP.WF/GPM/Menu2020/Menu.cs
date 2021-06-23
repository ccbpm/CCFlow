using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.Sys;

namespace BP.GPM.Menu2020
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuAttr : EntityTreeAttr
    {
        /// <summary>
        /// 控制方法
        /// </summary>
        public const string MenuCtrlWay = "MenuCtrlWay";
        /// <summary>
        /// 系统
        /// </summary>
        public const string ModuleNo = "ModuleNo";
        /// <summary>
        /// 系统编号
        /// </summary>
        public const string SystemNo = "SystemNo";
        /// <summary>
        /// 图片
        /// </summary>
        public const string WorkType = "WorkType";
        /// <summary>
        /// 连接
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 连接（pc）
        /// </summary>
        public const string UrlExt = "UrlExt";
        /// <summary>
        /// 连接（移动端）
        /// </summary>
        public const string MobileUrlExt = "MobileUrlExt";
        /// <summary>
        /// 控制内容
        /// </summary>
        public const string CtrlObjs = "CtrlObjs";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 打开方式
        /// </summary>
        public const string OpenWay = "OpenWay";
        /// <summary>
        /// 标记
        /// </summary>
        public const string Mark = "Mark";
        /// <summary>
        /// 扩展1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 扩展2
        /// </summary>
        public const string MenuModel = "MenuModel";
        /// <summary>
        /// 列表模式
        /// </summary>
        public const string ListModel = "ListModel";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";

        public const string FrmID = "FrmID";
        public const string FlowNo = "FlowNo";

    }
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : EntityNoName
    {
        #region 属性
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = false;
                    return uac;
                }
                else
                {
                    uac.Readonly();
                }
                return uac;
            }
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SystemNo
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.SystemNo);
            }
            set
            {
                this.SetValByKey(MenuAttr.SystemNo, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(MenuAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 标记
        /// </summary>
        public string Mark
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Mark);
            }
            set
            {
                this.SetValByKey(MenuAttr.Mark, value);
            }
        }
        /// <summary>
        /// Tag 1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Tag1);
            }
            set
            {
                this.SetValByKey(MenuAttr.Tag1, value);
            }
        }
        //public CtrlWay HisCtrlWay
        //{
        //    get
        //    {
        //        return (CtrlWay)this.GetValIntByKey(MenuAttr.CtrlWay);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MenuAttr.CtrlWay, (int)value);
        //    }
        //}
        /// <summary>
        /// 功能
        /// </summary>
        public MenuType HisMenuType
        {
            get
            {
                return (MenuType)this.GetValIntByKey(MenuAttr.MenuType);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuType, (int)value);
            }
        }
        public MenuCtrlWay MenuCtrlWay
        {
            get
            {
                return (MenuCtrlWay)this.GetValIntByKey(MenuAttr.MenuCtrlWay);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuCtrlWay, (int)value);
            }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MenuAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(MenuAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 是否是ccSytem
        /// </summary>
        //public MenuType MenuType
        //{
        //    get
        //    {
        //        return (MenuType)this.GetValIntByKey(MenuAttr.MenuType);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MenuAttr.MenuType, (int)value);
        //    }
        //}
        /// <summary>
        /// 类别编号
        /// </summary>
        public string ModuleNo
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.ModuleNo);
            }
            set
            {
                this.SetValByKey(MenuAttr.ModuleNo, value);
            }
        }
        /// <summary>
        /// 模式
        /// </summary>
        public string MenuModel
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.MenuModel);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuModel, value);
            }
        }
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Icon);
            }
            set
            {
                this.SetValByKey(MenuAttr.Icon, value);
            }
        }
        /// <summary>
        /// 菜单工作类型 0=自定义菜单， 1=系统菜单，不可以删除.
        /// </summary>
        public int WorkType
        {
            get
            {
                return this.GetValIntByKey(MenuAttr.WorkType);
            }
            set
            {
                this.SetValByKey(MenuAttr.WorkType, value);
            }
        }
        public string UrlExt
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.UrlExt);
            }
            set
            {
                this.SetValByKey(MenuAttr.UrlExt, value);
            }
        }
        public string MobileUrlExt
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.MobileUrlExt);
            }
            set
            {
                this.SetValByKey(MenuAttr.MobileUrlExt, value);
            }
        }
        public bool IsCheck = false;
        /// <summary>
        /// 标记
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(MenuAttr.Idx);
            }
            set
            {
                this.SetValByKey(MenuAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 菜单
        /// </summary>
        public Menu()
        {
        }
        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="mypk"></param>
        public Menu(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 业务处理.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
                this.No = DBAccess.GenerGUID();
            this.OrgNo = BP.Web.WebUser.OrgNo;

            this.InitIcon();
            return base.beforeInsert();
        }
        /// <summary>
        /// 初始化icon.
        /// </summary>
        private void InitIcon()
        {
            if (this.Mark.Equals("StartFlow") == true) this.Icon = "icon-paper-plane";
            if (this.Mark.Equals("Todolist") == true) this.Icon = "icon-bell";
            if (this.Mark.Equals("Runing") == true) this.Icon = "icon-clock";
            if (this.Mark.Equals("Group") == true) this.Icon = "icon-chart";
            if (this.Mark.Equals("Search") == true) this.Icon = "icon-grid";

        }
        protected override bool beforeDelete()
        {
            if (this.WorkType == 1)
                throw new Exception("@删除失败,此项为系统菜单，不能删除只能隐藏。");

            return base.beforeDelete();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("GPM_Menu");  // 类的基本属性.
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "系统菜单";
                map.EnType = EnType.Sys;
                map.CodeStruct = "4";

                map.AddTBStringPK(MenuAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(MenuAttr.Name, null, "名称", true, false, 0, 300, 200, true);

                map.AddTBString(MenuAttr.MenuModel, null, "菜单模式", true, true, 0, 50, 50);
                map.AddTBString(MenuAttr.Mark, null, "标记", true, false, 0, 300, 200, false);
                map.AddTBString(MenuAttr.Tag1, null, "Tag1", true, false, 0, 300, 200, false);


                map.AddTBString(MenuAttr.FrmID, null, "FrmID", false, false, 0, 300, 200, false);
                map.AddTBString(MenuAttr.FlowNo, null, "FlowNo", false, false, 0, 300, 200, false);


                // @0=系统根目录@1=系统类别@2=系统.
                map.AddDDLSysEnum(MenuAttr.OpenWay, 1, "打开方式", true, true, MenuAttr.OpenWay,
                    "@0=新窗口@1=本窗口@2=覆盖新窗口");

                map.AddTBString(MenuAttr.UrlExt, null, "PC端连接", true, false, 0, 500, 200, true);
                map.AddTBString(MenuAttr.MobileUrlExt, null, "移动端连接", true, false, 0, 500, 200, true);

                map.AddDDLSysEnum(MenuAttr.MenuCtrlWay, 0, "控制方式", true, true, MenuAttr.MenuCtrlWay,
                    "@0=按照设置的控制@1=任何人都可以使用@2=Admin用户可以使用");
                map.AddBoolean(MenuAttr.IsEnable, true, "是否启用?", true, true);

                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 50, 50, true);


                //  map.AddTBString(MenuAttr.ModuleNo, null, "ModuleNo", false, false, 0, 50, 50);
                map.AddTBString(MenuAttr.SystemNo, null, "SystemNo", false, false, 0, 50, 50);

                //隶属模块，可以让用户编辑。
                map.AddDDLSQL(MenuAttr.ModuleNo, null, "隶属模块编号",
                    "SELECT No,Name FROM GPM_Module WHERE SystemNo='@SystemNo'", true);

                map.AddDDLSysEnum(MenuAttr.ListModel, 0, "列表模式", true, true, MenuAttr.ListModel,
                    "@0=编辑模式@1=视图模式");
                string msg = "提示";
                msg += "\t\n 1. 编辑模式就是可以批量的编辑方式打开数据, 可以批量的表格方式编辑数据.";
                msg += "\t\n 2. 视图模式就是查询的模式打开数据..";
                map.SetHelperAlert(MenuAttr.ListModel, msg);

                map.AddTBInt(MenuAttr.Idx, 0, "顺序号", true, false);

                // @0=自定义菜单. @1=系统菜单.  系统菜单不可以删除.
                map.AddTBInt(MenuAttr.WorkType, 0, "工作类型", false, false);


                if (Sys.SystemConfig.CCBPMRunModel != Sys.CCBPMRunModel.Single)
                    map.AddTBString(MenuAttr.OrgNo, null, "组织编号", true, false, 0, 50, 20);

                //查询条件.
                //map.AddSearchAttr(MenuAttr.MenuType);
                //map.AddSearchAttr(MenuAttr.OpenWay);

                //map.AddDDLSysEnum(AppAttr.CtrlWay, 1, "控制方式", true, true, AppAttr.CtrlWay,
                //    "@0=游客@1=所有人员@2=按岗位@3=按部门@4=按人员@5=按SQL");
                // map.AddTBString(MenuAttr.CtrlObjs, null, "控制内容", false, false, 0, 4000, 20);
                //// 一对多的关系.
                //map.AttrsOfOneVSM.Add(new ByStations(), new Stations(), ByStationAttr.RefObj, ByStationAttr.FK_Station,
                //    StationAttr.Name, StationAttr.No, "可访问的岗位");
                //map.AttrsOfOneVSM.Add(new ByDepts(), new Depts(), ByStationAttr.RefObj, ByDeptAttr.FK_Dept,
                //    DeptAttr.Name, DeptAttr.No, "可访问的部门");
                //map.AttrsOfOneVSM.Add(new ByEmps(), new Emps(), ByStationAttr.RefObj, ByEmpAttr.FK_Emp,
                //    EmpAttr.Name, EmpAttr.No, "可访问的人员");

                #region 基本功能.
                //可以访问的权限组.
                map.AttrsOfOneVSM.Add(new GroupMenus(), new Groups(),
                    GroupMenuAttr.FK_Menu, GroupMenuAttr.FK_Group, EmpAttr.Name, EmpAttr.No, "绑定到权限组");

                //可以访问的权限组.
                map.AttrsOfOneVSM.Add(new StationMenus(), new BP.Port.Stations(),
                    StationMenuAttr.FK_Menu, StationMenuAttr.FK_Station, EmpAttr.Name, EmpAttr.No, "绑定到岗位-列表模式");

                //可以访问的权限组.
                map.AttrsOfOneVSM.AddGroupListModel(new StationMenus(), new BP.Port.Stations(),
                    StationMenuAttr.FK_Menu, StationMenuAttr.FK_Station, "绑定到岗位-分组模式", BP.Port.StationAttr.FK_StationType, "Name", EmpAttr.No);
                //可以访问的权限组.(岗位)
                map.AttrsOfOneVSM.Add(new DeptMenus(), new BP.Port.Depts(),
                    DeptMenuAttr.FK_Menu, DeptMenuAttr.FK_Dept, DeptAttr.Name, DeptAttr.No, "绑定到部门-列表模式");

                map.AttrsOfOneVSM.AddBranches(new DeptMenus(), new Depts(),
                   DeptMenuAttr.FK_Menu, DeptMenuAttr.FK_Dept, "部门(树)", EmpAttr.Name, EmpAttr.No);

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new EmpMenus(), new BP.Port.Emps(),
                   EmpMenuAttr.FK_Menu,
                   EmpMenuAttr.FK_Emp, "绑定人员-树结构", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //不带有参数的方法.
                RefMethod rm = new RefMethod();
                rm.Title = "增加(增删改查)功能权限";
                rm.Warning = "确定要增加吗？";
                rm.ClassMethodName = this.ToString() + ".DoAddRight3";
                rm.IsForEns = true;
                rm.IsCanBatch = true; //是否可以批处理？
                                      // map.AddRefMethod(rm);
                #endregion 基本功能.

                #region 创建菜单.
                rm = new RefMethod();
                rm.GroupName = "创建菜单";
                rm.Title = "创建单据";
                rm.Warning = "您确定要创建吗？";

                rm.HisAttrs.AddTBString("No", null, "单据编号", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("Name", null, "单据名称", true, false, 0, 100, 400);
                rm.HisAttrs.AddTBString("PTable", null, "存储表(为空则为编号相同)", true, false, 0, 100, 100);
                rm.HisAttrs.AddDDLSysEnum("FrmType", 0, "单据模式", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单");
                rm.HisAttrs.AddDDLSQL("Sys_FormTree", "", "选择表单树", "SELECT No,Name FROM Sys_FormTree WHERE ParentNo='1'");
                rm.ClassMethodName = this.ToString() + ".DoAddCCBill";
                // map.AddRefMethod(rm);
                #endregion 创建菜单.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.ModuleNo) == true)
                throw new Exception("err@模块编号不能为空.");

            //获取他的系统编号.
            Module md = new Module(this.ModuleNo);
            this.SystemNo = md.SystemNo;

            return base.beforeUpdateInsertAction();
        }

        #region 移动方法.
        /// <summary>
        /// 向上移动
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(MenuAttr.ModuleNo, this.ModuleNo, ModuleAttr.Idx);
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(MenuAttr.ModuleNo, this.ModuleNo, ModuleAttr.Idx);
        }
        #endregion 移动方法.

    }
    /// <summary>
    /// 菜单s
    /// </summary>
    public class Menus : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 菜单s
        /// </summary>
        public Menus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Menu();
            }
        }
        public override int RetrieveAll()
        {
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.SAAS)
                return base.RetrieveAll("Idx");

            //集团模式下的岗位体系: @0=每套组织都有自己的岗位体系@1=所有的组织共享一套岗则体系.
            if (BP.Sys.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, "Idx");
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Menu> ToJavaList()
        {
            return (System.Collections.Generic.IList<Menu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Menu> Tolist()
        {
            System.Collections.Generic.List<Menu> list = new System.Collections.Generic.List<Menu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Menu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
