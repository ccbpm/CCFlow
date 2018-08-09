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
        /// 是否选中.
        /// </summary>
        public const string IsChecked = "IsChecked";
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
    }
    /// <summary>
    /// 人员菜单功能
    /// </summary>
    public class EmpMenu : EntityMM
    {
        #region 属性
       
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
        /// <summary>
        /// 是否选中
        /// </summary>
        public string IsChecked
        {
            get
            {
                return this.GetValStringByKey(EmpMenuAttr.IsChecked);
            }
            set
            {
                this.SetValByKey(EmpMenuAttr.IsChecked, value);
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


                map.AddTBStringPK(EmpMenuAttr.FK_Emp, null, "操作员", true, false, 0, 3900, 20);
               // map.AddDDLEntitiesPK(EmpMenuAttr.FK_Menu, null, "菜单功能",new BP.GPM.Menus(),true);
                map.AddTBStringPK(EmpMenuAttr.FK_Menu, null, "菜单", false, false, 0, 50, 20);
                map.AddBoolean(EmpMenuAttr.IsChecked, true, "是否选中", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 人员菜单功能s
    /// </summary>
    public class EmpMenus : EntitiesMM
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
