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

        /// <summary>
        /// 风格:比如Tab,的风格.
        /// </summary>
        public const string Style = "Style";

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

            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            //如果是数据源列表.
            if (this.MenuModel.Equals("DBList") == true)
            {
                MapData md = new MapData(this.UrlExt);
                md.Delete();

                MapAttrs attrs = new MapAttrs();
                attrs.Delete(MapAttrAttr.FK_MapData, this.Mark + "Bak");
            }

            //删除窗体信息.
            if (this.MenuModel.Equals("Windows") == true)
            {
                BP.GPM.Home.WindowTemplates ens = new Home.WindowTemplates();
                ens.Delete(BP.GPM.Home.WindowTemplateAttr.PageID, this.No);

                // BP.GPM.Home.WindowExt.HtmlVarDtls dtls = new Home.WindowExt.HtmlVarDtls();
                // dtls.Delete(BP.GPM.Home.WindowTemplateAttr.PageID, this.No);
            }


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
                map.EnDesc = "菜单";
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

                map.AddTBString(MenuAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 20);

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
