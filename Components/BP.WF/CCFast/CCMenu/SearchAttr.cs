using BP.DA;
using BP.En;

namespace BP.CCFast.CCMenu
{
    public class SearchAttrAttr:EntityNoNameAttr
    {
        public const string RefMenuNo = "RefMenuNo";
        public const string Icon = "Icon";
        public const string UrlExt = "UrlExt";
        public const string Tag1 = "Tag1";
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    public class SearchAttr : EntityNoName
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
                    uac.IsInsert = true;
                    return uac;
                }
                else
                {
                    uac.Readonly();
                }
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 查询条件
        /// </summary>
        public SearchAttr()
        {
        }
        protected override bool beforeDelete()
        {
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

                Map map = new Map("GPM_MenuDtl", "查询条件");  // 类的基本属性.
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(SearchAttrAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(SearchAttrAttr.RefMenuNo, null, "菜单编号", false, false, 0, 100, 100);

                map.AddTBString(SearchAttrAttr.Name, null, "条件标签", true, false, 0, 300, 100, true);
                map.AddTBString(SearchAttrAttr.Tag1, null, "标识", true, false, 0, 500, 100, true);
                map.AddTBString(SearchAttrAttr.UrlExt, null, "SQL/标记/枚举值", true, false, 0, 500, 500, true);

                map.AddTBInt(SearchAttrAttr.Idx, 0, "Idx", true, false);
                map.AddDtl(new SearchAttrs(), SearchAttrAttr.RefMenuNo);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 查询条件s
    /// </summary>
    public class SearchAttrs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 查询条件s
        /// </summary>
        public SearchAttrs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SearchAttr();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SearchAttr> ToJavaList()
        {
            return (System.Collections.Generic.IList<SearchAttr>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SearchAttr> Tolist()
        {
            System.Collections.Generic.List<SearchAttr> list = new System.Collections.Generic.List<SearchAttr>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SearchAttr)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
