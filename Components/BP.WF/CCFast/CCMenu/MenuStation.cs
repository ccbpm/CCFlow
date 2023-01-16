using BP.En;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 角色菜单
    /// </summary>
    public class StationMenuAttr
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 角色
        /// </summary>
        public const string FK_Station = "FK_Station";
        /// <summary>
        /// 是否选中.
        /// </summary>
        public const string IsChecked = "IsChecked";
    }
    /// <summary>
    /// 角色菜单
    /// </summary>
    public class StationMenu : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 菜单
        /// </summary>
        public string FK_Menu
        {
            get
            {
                return this.GetValStringByKey(StationMenuAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(StationMenuAttr.FK_Menu, value);
            }
        }
        /// <summary>
        /// 角色
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(StationMenuAttr.FK_Station);
            }
            set
            {
                this.SetValByKey(StationMenuAttr.FK_Station, value);
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public string IsChecked
        {
            get
            {
                return this.GetValStringByKey(StationMenuAttr.IsChecked);
            }
            set
            {
                this.SetValByKey(StationMenuAttr.IsChecked, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 角色菜单
        /// </summary>
        public StationMenu()
        {
        }
       
        /// <summary>
        /// 角色菜单
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_StationMenu", "角色菜单");
                map.setEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(StationMenuAttr.FK_Menu, null, "菜单", false, false, 0, 50, 20);
                map.AddTBString(StationMenuAttr.FK_Station, null, "角色", false, false, 0, 50, 20);

                //map.AddDDLEntities(StationMenuAttr.FK_Station, null, "角色", new BP.Port.Stations(), true);
                map.AddBoolean(StationMenuAttr.IsChecked, true, "是否选中", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Menu + "_" + this.FK_Station;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 角色菜单s
    /// </summary>
    public class StationMenus : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 角色s
        /// </summary>
        public StationMenus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new StationMenu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StationMenu> ToJavaList()
        {
            return (System.Collections.Generic.IList<StationMenu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StationMenu> Tolist()
        {
            System.Collections.Generic.List<StationMenu> list = new System.Collections.Generic.List<StationMenu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StationMenu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
