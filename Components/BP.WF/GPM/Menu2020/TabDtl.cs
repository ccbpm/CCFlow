using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.Sys;

namespace BP.GPM.Menu2020
{
    public class TabDtlAttr:EntityNoNameAttr
    {
        public const string RefMenuNo = "RefMenuNo";
        public const string Icon = "Icon";
        public const string UrlExt = "UrlExt";
        public const string Model = "Model";
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 标签
    /// </summary>
    public class TabDtl : EntityNoName
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
        #endregion

        #region 构造方法
        /// <summary>
        /// 标签
        /// </summary>
        public TabDtl()
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

                Map map = new Map("GPM_MenuDtl", "标签");  // 类的基本属性.
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(TabDtlAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(TabDtlAttr.RefMenuNo, null, "菜单编号", false, false, 0, 100, 100);

                map.AddTBString(TabDtlAttr.Icon, null, "Icon", true, false, 0, 50, 50, true);
                map.AddTBString(TabDtlAttr.Name, null, "Tab名称", true, false, 0, 300, 200, true);
                map.AddTBString(TabDtlAttr.UrlExt, null, "链接", true, false, 0, 50, 50, true);

                map.AddTBString(TabDtlAttr.Model, null, "模式", true, false, 0, 50, 50, true);

                map.AddTBInt(TabDtlAttr.Idx, 0, "Idx", true, false);

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
    /// 标签s
    /// </summary>
    public class TabDtls : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 标签s
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
