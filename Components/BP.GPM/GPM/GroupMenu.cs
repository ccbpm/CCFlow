using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 权限组菜单
    /// </summary>
    public class GroupMenuAttr
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 权限组
        /// </summary>
        public const string FK_Group = "FK_Group";
        /// <summary>
        /// 是否选中.
        /// </summary>
        public const string IsChecked = "IsChecked";
    }
    /// <summary>
    /// 权限组菜单
    /// </summary>
    public class GroupMenu : EntityMM
    {
        #region 属性
        /// <summary>
        /// 菜单
        /// </summary>
        public string FK_Menu
        {
            get
            {
                return this.GetValStringByKey(GroupMenuAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(GroupMenuAttr.FK_Menu, value);
            }
        }
        /// <summary>
        /// 权限组
        /// </summary>
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey(GroupMenuAttr.FK_Group);
            }
            set
            {
                this.SetValByKey(GroupMenuAttr.FK_Group, value);
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public string IsChecked
        {
            get
            {
                return this.GetValStringByKey(GroupMenuAttr.IsChecked);
            }
            set
            {
                this.SetValByKey(GroupMenuAttr.IsChecked, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限组菜单
        /// </summary>
        public GroupMenu()
        {
        }
        /// <summary>
        /// 权限组菜单
        /// </summary>
        /// <param name="mypk"></param>
        public GroupMenu(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 权限组菜单
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_GroupMenu");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "权限组菜单";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(GroupMenuAttr.FK_Group, null, "权限组", false, false, 0, 50, 20);
                map.AddTBStringPK(GroupMenuAttr.FK_Menu, null, "菜单", false, false, 0, 50, 20);
                map.AddBoolean(GroupMenuAttr.IsChecked, true, "是否选中", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 权限组菜单s
    /// </summary>
    public class GroupMenus : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 权限组s
        /// </summary>
        public GroupMenus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupMenu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupMenu> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupMenu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupMenu> Tolist()
        {
            System.Collections.Generic.List<GroupMenu> list = new System.Collections.Generic.List<GroupMenu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupMenu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
