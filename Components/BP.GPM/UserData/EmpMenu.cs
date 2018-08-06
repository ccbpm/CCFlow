using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 人员菜单功能
    /// </summary>
    public class EmpMenuAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 菜单功能
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
    }
    /// <summary>
    /// 人员菜单功能
    /// </summary>
    public class EmpMenu : EntityMyPK
    {
        #region 属性
        public string CtrlObjs
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.CtrlObjs);
            }
            set
            {
                this.SetValByKey(MenuAttr.CtrlObjs, value);
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(EmpMenuAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.FK_Emp, value);
            }
        }
        public string FK_Menu
        {
            get
            {
                return this.GetValStringByKey(EmpMenuAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.FK_Menu, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Name);
            }
            set
            {
                this.SetValByKey(MenuAttr.Name, value);
            }
        }
        public CtrlWay HisCtrlWay
        {
            get
            {
                return (CtrlWay)this.GetValIntByKey(MenuAttr.CtrlWay);
            }
            set
            {
                this.SetValByKey(MenuAttr.CtrlWay, (int)value);
            }
        }
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
        /// 是否是ccSytem
        /// </summary>
        public int MenuType
        {
            get
            {
                return this.GetValIntByKey(MenuAttr.MenuType);
            }
            set
            {
                this.SetValByKey(MenuAttr.MenuType, value);
            }
        }
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

        public string FK_App
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.FK_App);
            }
            set
            {
                this.SetValByKey(MenuAttr.FK_App, value);
            }
        }
        public string Img
        {
            get
            {
                string s = this.GetValStringByKey("WebPath");
                if (DataType.IsNullOrEmpty(s))
                {
                    if (this.HisMenuType == GPM.MenuType.Dir)
                        return "../../Images/Btn/View.gif";
                    else
                        return "../../Images/Btn/Go.gif";
                }
                else
                {
                    return s;
                }
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey(MenuAttr.Url);
            }
            set
            {
                this.SetValByKey(MenuAttr.Url, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 人员菜单功能
        /// </summary>
        public EmpMenu()
        {
        }
        /// <summary>
        /// 人员菜单功能
        /// </summary>
        /// <param name="mypk"></param>
        public EmpMenu(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 人员菜单功能
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_EmpMenu");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "人员菜单对应";
                map.EnType = EnType.App;

               // map.AddMyPK();

                map.AddTBStringPK(EmpMenuAttr.FK_Emp, null, "操作员", true, false, 0, 3900, 20);
                map.AddDDLEntitiesPK(EmpMenuAttr.FK_Menu, null, "菜单功能",new BP.GPM.Menus(),true);

                //map.AddDDLEntitiesPK(EmpMenuAttr.FK_Emp,null,"操作员",new 
                //map.AddTBString(EmpMenuAttr.FK_Emp, null, "操作员", true, false, 0, 30, 20);
                //map.AddTBString(EmpMenuAttr.FK_Menu, null, "菜单功能", true, false, 0, 30, 20);

                //map.AddTBString(MenuAttr.Name, null, "菜单功能-名称", true, false, 0, 3900, 20);
                //map.AddTBString(MenuAttr.ParentNo, null, "ParentNo", true, false, 1, 30, 20);
                //map.AddTBString(AppAttr.Url, null, "连接", true, false, 0, 3900, 20, true);
                //map.AddDDLSysEnum(MenuAttr.MenuType, 0, "菜单类型", true,true, MenuAttr.MenuType,
                //    "@3=目录@4=功能@5=功能控制点");
                //map.AddTBString(MenuAttr.FK_App, null, "系统", true, false, 0, 30, 20);
                //map.AddMyFile("图标");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 人员菜单功能s
    /// </summary>
    public class EmpMenus : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 菜单s
        /// </summary>
        public EmpMenus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpMenu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpMenu> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpMenu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpMenu> Tolist()
        {
            System.Collections.Generic.List<EmpMenu> list = new System.Collections.Generic.List<EmpMenu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpMenu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
