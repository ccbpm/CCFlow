using BP.En;
using BP.CCFast.CCMenu;

namespace BP.CCFast.Rpt
{
    /// <summary>
    /// 三维报表
    /// </summary>
    public class Rpt3DLeft : EntityNoName
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
        /// 三维报表
        /// </summary>
        public Rpt3DLeft()
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

                Map map = new Map("GPM_Menu", "三维报表");  // 类的基本属性.
                map.setEnType(EnType.Sys);

                map.AddTBStringPK(MenuAttr.No, null, "编号", false, false, 1, 90, 50);
                map.AddTBString(MenuAttr.Name, null, "标题", true, false, 0, 300, 200, true);
                map.AddTBString(MenuAttr.Icon, null, "Icon", true, false, 0, 50, 50, true);

                map.AddTBStringDoc(MenuAttr.Tag1, null, "数据源SQL", true, false,true);
                map.AddTBStringDoc(MenuAttr.Tag2, null, "维度1SQL", true, false, true);
                map.AddTBStringDoc(MenuAttr.Tag3, null, "维度2SQL", true, false, true);

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
    /// 三维报表s
    /// </summary>
    public class Rpt3DLefts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 三维报表s
        /// </summary>
        public Rpt3DLefts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Rpt3DLeft();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Rpt3DLeft> ToJavaList()
        {
            return (System.Collections.Generic.IList<Rpt3DLeft>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Rpt3DLeft> Tolist()
        {
            System.Collections.Generic.List<Rpt3DLeft> list = new System.Collections.Generic.List<Rpt3DLeft>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Rpt3DLeft)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
