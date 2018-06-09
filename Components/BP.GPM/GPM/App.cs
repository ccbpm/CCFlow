using System;
using System.Collections;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 系统
    /// </summary>
    public class AppAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string AppModel = "AppModel";
        /// <summary>
        /// Url
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// SubUrl
        /// </summary>
        public const string SubUrl = "SubUrl";
        /// <summary>
        /// 是否启用.
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// FK_AppSort
        /// </summary>
        public const string FK_AppSort = "FK_AppSort";
        /// <summary>
        /// 关联菜单编号
        /// </summary>
        public const string RefMenuNo = "RefMenuNo";
        /// <summary>
        /// 用户控件ID
        /// </summary>
        public const string UidControl = "UidControl";
        /// <summary>
        /// 密码控件ID
        /// </summary>
        public const string PwdControl = "PwdControl";
        /// <summary>
        /// 提交方式
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// 登录方式
        /// </summary>
        public const string SSOType = "SSOType";
        /// <summary>
        /// 备注
        /// </summary>
        public const string AppRemark = "AppRemark";
        public const string OpenWay = "OpenWay";
    }
    /// <summary>
    /// 系统
    /// </summary>
    public class App : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 打开方式
        /// </summary>
        public string OpenWay
        {
            get
            {
                int openWay = 0;

                switch (openWay)
                {
                    case 0:
                        return "_blank";
                    case 1:
                        return this.No;
                    default:
                        return "";
                }
            }
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey("WebPath");
            }
        }
        /// <summary>
        /// ICON
        /// </summary>
        public string ICON
        {
            get
            {
                return this.WebPath;
            }
            set
            {
                this.SetValByKey("ICON", value);
            }
        }
        /// <summary>
        /// 连接
        /// </summary>
        public string Url
        {
            get
            {
                string url = this.GetValStrByKey(AppAttr.Url);
                if (DataType.IsNullOrEmpty(url)) return "";

                if (this.SSOType == "0")//SID验证
                {
                    string SID = DBAccess.RunSQLReturnStringIsNull("SELECT SID FROM Port_Emp WHERE No='" + Web.WebUser.No + "'", null);
                    if (url.Contains("?"))
                        url += "&UserNo=" + Web.WebUser.No + "&SID=" + SID;
                    else
                        url += "?UserNo=" + Web.WebUser.No + "&SID=" + SID;
                }
                return url;
            }
            set
            {
                this.SetValByKey(AppAttr.Url, value);
            }
        }
        /// <summary>
        /// 跳转连接
        /// </summary>
        public string SubUrl
        {
            get
            {
                return this.GetValStrByKey(AppAttr.SubUrl);
            }
            set
            {
                this.SetValByKey(AppAttr.Url, value);
            }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(AppAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(AppAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(AppAttr.Idx);
            }
            set
            {
                this.SetValByKey(AppAttr.Idx, value);
            }
        }
        /// <summary>
        /// 用户控件ID
        /// </summary>
        public string UidControl
        {
            get
            {
                return this.GetValStrByKey(AppAttr.UidControl);
            }
            set
            {
                this.SetValByKey(AppAttr.UidControl, value);
            }
        }
        /// <summary>
        /// 密码控件ID
        /// </summary>
        public string PwdControl
        {
            get
            {
                return this.GetValStrByKey(AppAttr.PwdControl);
            }
            set
            {
                this.SetValByKey(AppAttr.PwdControl, value);
            }
        }
        /// <summary>
        /// 提交方式
        /// </summary>
        public string ActionType
        {
            get
            {
                return this.GetValStrByKey(AppAttr.ActionType);
            }
            set
            {
                this.SetValByKey(AppAttr.ActionType, value);
            }
        }
        /// <summary>
        /// 登录方式@0=SID验证@1=连接@2=表单提交
        /// </summary>
        public string SSOType
        {
            get
            {
                return this.GetValStrByKey(AppAttr.SSOType);
            }
            set
            {
                this.SetValByKey(AppAttr.SSOType, value);
            }
        }
        public string FK_AppSort
        {
            get
            {
                return this.GetValStringByKey(AppAttr.FK_AppSort);
            }
            set
            {
                this.SetValByKey(AppAttr.FK_AppSort, value);
            }
        }
        public string RefMenuNo
        {
            get
            {
                return this.GetValStringByKey(AppAttr.RefMenuNo);
            }
            set
            {
                this.SetValByKey(AppAttr.RefMenuNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 系统
        /// </summary>
        public App()
        {
        }
        /// <summary>
        /// 系统
        /// </summary>
        /// <param name="mypk"></param>
        public App(string no)
        {
            this.No = no;
            this.Retrieve();
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
                Map map = new Map("GPM_App");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "系统";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(AppAttr.No, null, "编号", true, false, 2, 30, 20);
                map.AddTBString(AppAttr.Name, null, "名称", true, false, 0, 3900, 20);

                map.AddDDLSysEnum(AppAttr.AppModel, 0, "应用类型", true, true, AppAttr.AppModel, "@0=BS系统@1=CS系统");
                map.AddDDLEntities(AppAttr.FK_AppSort, null, "类别", new AppSorts(), true);
                map.AddTBString(AppAttr.Url, null, "默认连接", true, false, 0, 3900, 100, true);
                map.AddTBString(AppAttr.SubUrl, null, "第二连接", true, false, 0, 3900, 100, true);
                map.AddTBString(AppAttr.UidControl, null, "用户名控件", true, false, 0, 100, 100);
                map.AddTBString(AppAttr.PwdControl, null, "密码控件", true, false, 0, 100, 100);
                map.AddDDLSysEnum(AppAttr.ActionType, 0, "提交类型", true, true, AppAttr.ActionType, "@0=GET@1=POST");
                map.AddDDLSysEnum(AppAttr.SSOType, 0, "登录方式", true, true, AppAttr.SSOType, "@0=SID验证@1=连接@2=表单提交@3=不传值");
                map.AddDDLSysEnum(AppAttr.OpenWay, 0, "打开方式", true, true, AppAttr.OpenWay,
                    "@0=新窗口@1=本窗口@2=覆盖新窗口");

                map.AddTBInt(AppAttr.Idx, 0, "显示顺序", true, false);
                map.AddBoolean(AppAttr.IsEnable, true, "是否启用", true, true);

                map.AddTBString(AppAttr.RefMenuNo, null, "关联菜单编号", true, false, 0, 3900, 20);
                map.AddTBString(AppAttr.AppRemark, null, "备注", true, false, 0, 500, 500,true);
                map.AddMyFile("ICON");

                //map.AttrsOfOneVSM.Add(new ByStations(), new Stations(), ByStationAttr.RefObj,
                //    ByStationAttr.FK_Station, StationAttr.Name, StationAttr.No, "可访问的岗位");

                //map.AttrsOfOneVSM.Add(new ByDepts(), new Depts(), ByStationAttr.RefObj, 
                //    ByDeptAttr.FK_Dept, DeptAttr.Name, DeptAttr.No, "可访问的部门");

                //map.AttrsOfOneVSM.Add(new ByEmps(), new Emps(), ByStationAttr.RefObj, 
                //    ByEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "可访问的人员");

                RefMethod rm = new RefMethod();
                rm.Title = "编辑菜单";
                rm.ClassMethodName = this.ToString() + ".DoMenu";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查看可访问该系统的人员";
                rm.ClassMethodName = this.ToString() + ".DoWhoCanUseApp";

                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "刷新设置";
                rm.ClassMethodName = this.ToString() + ".DoRef";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "第二连接";
                //rm.Title = "第二连接：登录方式为不传值、连接不设置用户名密码转为第二连接。";
                rm.ClassMethodName = this.ToString() + ".About";
               // map.AddRefMethod(rm);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            Menu appMenu = new Menu(this.RefMenuNo);
            if (appMenu != null && appMenu.Flag.Contains("Flow"))
            {
                throw new Exception("@删除失败,此项为工作流菜单，不能删除。");
            }
            // 删除该系统.
            Menu menu = new Menu();
            menu.Delete(MenuAttr.FK_App, this.No);

            // 删除用户数据.
            EmpMenu em = new EmpMenu();
            em.Delete(MenuAttr.FK_App, this.No);

            EmpApp ea = new EmpApp();
            ea.Delete(MenuAttr.FK_App, this.No);

            return base.beforeDelete();
        }

        protected override bool beforeUpdate()
        {
            //系统类别
            AppSort appSort = new AppSort(this.FK_AppSort);
            Menu menu = new Menu(this.RefMenuNo);
            menu.Name = this.Name;
            menu.ParentNo = appSort.RefMenuNo;
            menu.Update();
            return base.beforeUpdate();
        }

        protected override bool beforeInsert()
        {
            AppSort sort = new AppSort(this.FK_AppSort);

            // 求系统类别的菜单 .
            Menu menu = new Menu(sort.RefMenuNo);

            // 创建子菜单.
            Menu appMenu = menu.DoCreateSubNode() as Menu;
            appMenu.FK_App = this.No;
            appMenu.Name = this.Name;
            appMenu.HisMenuType = MenuType.App;
            appMenu.Update();

            //设置相关的菜单编号.
            this.RefMenuNo = appMenu.No;

            #region 为该系统创建几个空白菜单
            //Menu en = appMenu.DoCreateSubNode() as Menu;
            //en.FK_App = this.No;
            //en.Name = this.Name;
            //en.MenuType = 2;
            //en.IsDir = true;
            //en.Update();

            Menu dir = appMenu.DoCreateSubNode() as Menu;
            dir.FK_App = this.No;
            dir.Name = "功能目录1";
            dir.MenuType = 3;
            dir.Update();

            Menu func = dir.DoCreateSubNode() as Menu;
            func.Name = "xxx管理1";
            func.FK_App = this.No;
            func.MenuType = 4;
            func.Url = "http://ccflow.org";
            func.Update();

            Menu funcDot = func.DoCreateSubNode() as Menu;
            funcDot.Name = "查看";
            funcDot.MenuType = 5;
            funcDot.FK_App = this.No;
            funcDot.Update();

            funcDot = func.DoCreateSubNode() as Menu;
            funcDot.Name = "增加";
            funcDot.MenuType = 5;
            funcDot.FK_App = this.No;
            funcDot.Update();

            funcDot = func.DoCreateSubNode() as Menu;
            funcDot.Name = "删除";
            funcDot.MenuType = 5;
            funcDot.FK_App = this.No;
            funcDot.Update();
            #endregion

            return base.beforeInsert();
        }

        /// <summary>
        /// 为BPM初始化菜单.
        /// </summary>
        public static void InitBPMMenu()
        {
            AppSort sort = new AppSort();
            sort.No = "01";
            if (sort.RetrieveFromDBSources() == 0)
            {
                sort.Name = "应用系统";
                sort.RefMenuNo = "2000";
                sort.Insert();
            }

            App app = new App();
            app.No = "CCFlowBPM";
            app.Name = "BPM系统";
            app.FK_AppSort = "01";
            app.Insert();
        }
        /// <summary>
        /// 信息介绍
        /// </summary>
        /// <returns></returns>
        public string About()
        {
            return null;
        }
        /// <summary>
        /// 刷新设置
        /// </summary>
        /// <returns></returns>
        public string DoRef()
        {
            return "../../../GPM/WhoCanUseApp.aspx?FK_App=" + this.No;

           // PubClass.WinOpen("/GPM/WhoCanUseApp.aspx?FK_App=" + this.No + "&IsRef=1", 500, 700);
            //return null;
        }
        /// <summary>
        /// 查看可以访问的人员
        /// </summary>
        /// <returns></returns>
        public string DoWhoCanUseApp()
        {
            return "../../../GPM/WhoCanUseApp.aspx?FK_App=" + this.No;


          //  PubClass.WinOpen("/GPM/WhoCanUseApp.aspx?FK_App=" + this.No, 500, 700);
            //return null;
        }
        /// <summary>
        /// 打开菜单
        /// </summary>
        /// <returns></returns>
        public string DoMenu()
        {
            return "../../../GPM/AppMenu.htm?FK_App=" + this.No;

            //PubClass.WinOpen("/GPM/AppMenu.aspx?FK_App=" + this.No, 800, 500);
            //  PubClass.WinOpen("/Comm/RefFunc/EntityTree.aspx?EnsName=BP.GPM.Menus&FK_Tem=" + this.No, 500, 700);
            // return null;
        }
        /// <summary>
        /// 刷新数据.
        /// </summary>
        public void RefData()
        {
            //删除数据.
            EmpMenus mymes = new EmpMenus();
            mymes.Delete(EmpMenuAttr.FK_App, this.No);

            //删除系统.
            EmpApps empApps = new EmpApps();
            empApps.Delete(EmpMenuAttr.FK_App, this.No);

            //查询出来菜单.
            Menus menus = new Menus();
            menus.Retrieve(EmpMenuAttr.FK_App, this.No);

            //查询出来人员.
            Emps emps = new Emps();
            emps.RetrieveAllFromDBSource();

            foreach (Emp emp in emps)
            {
                #region 初始化系统访问权限.

                EmpApp me = new EmpApp();
                me.Copy(this);
                me.FK_Emp = emp.No;
                me.FK_App = this.No;
                me.MyPK = this.No + "_" + me.FK_Emp;
                me.Insert();
                #endregion 初始化系统访问权限.

                //#region 初始化菜单权限.
                //foreach (Menu menu in menus)
                //{
                //    /* 把此人能看到的菜单 init 里面去。*/
                //    if (Glo.IsCanDoIt(menu.No, menu.HisCtrlWay, emp.No) == false)
                //        continue;

                //    EmpMenu em = new EmpMenu();
                //    em.Copy(menu);
                //    em.FK_Emp = emp.No;
                //    em.FK_Menu = menu.No;  //菜单编号.
                //    em.FK_App = menu.FK_App; //系统编号

                //    em.MyPK = menu.No + "_" + emp.No;
                //    em.Insert();
                //}
                //#endregion 初始化菜单权限.
            }
        }
    }
    /// <summary>
    /// 系统s
    /// </summary>
    public class Apps : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 系统s
        /// </summary>
        public Apps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new App();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<App> ToJavaList()
        {
            return (System.Collections.Generic.IList<App>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<App> Tolist()
        {
            System.Collections.Generic.List<App> list = new System.Collections.Generic.List<App>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((App)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
