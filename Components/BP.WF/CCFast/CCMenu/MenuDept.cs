using BP.En;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 部门菜单
    /// </summary>
    public class DeptMenuAttr
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public const string FK_Menu = "FK_Menu";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 是否选中.
        /// </summary>
        public const string IsChecked = "IsChecked";
    }
    /// <summary>
    /// 部门菜单
    /// </summary>
    public class DeptMenu : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 菜单
        /// </summary>
        public string MenuNo
        {
            get
            {
                return this.GetValStringByKey(DeptMenuAttr.FK_Menu);
            }
            set
            {
                this.SetValByKey(DeptMenuAttr.FK_Menu, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptNo
        {
            get
            {
                return this.GetValStringByKey(DeptMenuAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(DeptMenuAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public string IsChecked
        {
            get
            {
                return this.GetValStringByKey(DeptMenuAttr.IsChecked);
            }
            set
            {
                this.SetValByKey(DeptMenuAttr.IsChecked, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 部门菜单
        /// </summary>
        public DeptMenu()
        {
        }
        /// <summary>
        /// 部门菜单
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_DeptMenu", "部门菜单");
                map.setEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(DeptMenuAttr.FK_Menu, null, "菜单", false, false, 0, 50, 20);
                map.AddTBString(DeptMenuAttr.FK_Dept, null, "部门", false, false, 0, 50, 20);

                //map.AddTBStringPK(DeptMenuAttr.FK_Station, null, "部门", false, false, 0, 50, 20);
                // map.AddDDLEntitiesPK(DeptMenuAttr.FK_Dept, null, " 部门", new BP.Port.Depts(), true);
                map.AddBoolean(DeptMenuAttr.IsChecked, true, "是否选中", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            //@wwh.
            this.MyPK = this.MenuNo + "_" + this.DeptNo;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 部门菜单s
    /// </summary>
    public class DeptMenus : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 部门s
        /// </summary>
        public DeptMenus()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptMenu();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptMenu> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptMenu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptMenu> Tolist()
        {
            System.Collections.Generic.List<DeptMenu> list = new System.Collections.Generic.List<DeptMenu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptMenu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
