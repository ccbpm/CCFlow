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
    /// 标签容器
    /// </summary>
    public class Tab : EntityNoName
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
        /// 标签容器
        /// </summary>
        public Tab()
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

                Map map = new Map("GPM_Menu", "标签容器");  // 类的基本属性.
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(MenuAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(MenuAttr.Name, null, "名称", true, false, 0, 300, 200, true);
                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 50, 50, true);

                //从表明细.
                map.AddDtl(new TabDtls(), TabDtlAttr.RefMenuNo);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 标签容器s
    /// </summary>
    public class Tabs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 标签容器s
        /// </summary>
        public Tabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Tab();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Tab> ToJavaList()
        {
            return (System.Collections.Generic.IList<Tab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Tab> Tolist()
        {
            System.Collections.Generic.List<Tab> list = new System.Collections.Generic.List<Tab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Tab)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
