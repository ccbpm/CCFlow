using BP.DA;
using BP.En;


namespace BP.CCFast.Portal.WindowExt
{
    /// <summary>
    /// 变量信息
    /// </summary>
    public class TabDtl : EntityMyPK
    {
        /// <summary>
        /// 表达式
        /// </summary>
        public string Exp0
        {
            get
            {
                return this.GetValStrByKey(DtlAttr.Exp0);
            }
            set
            {
                this.SetValByKey(DtlAttr.Exp0,value);
            }
        }

        #region 权限控制.
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                uac.IsInsert = true;
                uac.IsDelete = true;
                uac.IsView = true;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        #endregion 属性

        #region 构造方法
        /// <summary>
        /// 变量信息
        /// </summary>
        public TabDtl()
        {
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

                Map map = new Map("GPM_WindowTemplateDtl", "Tab页数据项");

                map.AddMyPK(false);
                map.AddTBString(DtlAttr.RefPK, null, "RefPK", false, false, 0, 40, 20, false);

                map.AddDDLSysEnum(DtlAttr.DBType, 0, "数据源类型", true, true, "WindowsDBType",
          "@0=数据库查询SQL@1=执行Url返回Json@2=执行\\DataUser\\JSLab\\Windows.js的函数.");
                map.AddDDLEntities(DtlAttr.DBSrc, null, "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddDDLSysEnum(DtlAttr.WindowsShowType, 0, "显示类型", true, true, "WindowsShowType",
      "@0=饼图@1=柱图@2=折线图@4=简单Table");

                map.AddTBString(DtlAttr.Name, null, "标签", true, false, 0, 300, 70, true);
                map.AddTBString(DtlAttr.FontColor, null, "字体风格", true, false, 0, 300, 100, true);
                map.AddTBString(DtlAttr.Exp0, null, "表达式", true, false, 0, 300, 1000, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            return base.beforeInsert();
        }

    }
    /// <summary>
    /// 变量信息s
    /// </summary>
    public class TabDtls : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 变量信息s
        /// </summary>
        public TabDtls()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TabDtl();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TabDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<TabDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TabDtl> Tolist()
        {
            System.Collections.Generic.List<TabDtl> list = new System.Collections.Generic.List<TabDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TabDtl)this[i]);

            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
