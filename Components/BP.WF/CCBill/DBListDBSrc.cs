using BP.En;
using BP.Sys;


namespace BP.CCBill
{
    /// <summary>
    /// 数据源实体
    /// </summary>
    public class DBListDBSrc : EntityNoName
    {
        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        #endregion

        #region 构造方法
        /// <summary>
        /// 数据源实体
        /// </summary>
        public DBListDBSrc()
        {
        }
        /// <summary>
        /// 数据源实体
        /// </summary>
        /// <param name="no">映射编号</param>
        public DBListDBSrc(string no)
            : base(no)
        {
        }
        public int DBType
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.DBType);
            }
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

                Map map = new Map("Sys_MapData", "数据源实体");

                map.CodeStruct = "4";

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                #endregion 基本属性.

                #region 数据源.
                map.AddDDLSysEnum(MapDataAttr.DBType, 0, "数据源类型", true, true, "DBListDBType",
         "@0=数据库查询SQL@1=执行Url返回Json@2=执行存储过程");
                map.AddDDLEntities(MapDataAttr.DBSrc, null, "数据源", new BP.Sys.SFDBSrcs(), true);
                map.SetHelperAlert(MapDataAttr.DBSrc, "您可以在系统管理中新建SQL数据源.");
                #endregion 数据源.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeUpdate()
        {
            DBList db = new DBList(this.No);
            if (db.DBType != this.DBType)
            {
                db.ExpEn = "";
                db.ExpList = "";
                db.ExpCount = "";
                db.Update();
            }
            return base.beforeUpdate();
        }
    }
    /// <summary>
    /// 数据源实体s
    /// </summary>
    public class DBListDBSrcs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 数据源实体s
        /// </summary>
        public DBListDBSrcs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DBListDBSrc();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DBListDBSrc> ToJavaList()
        {
            return (System.Collections.Generic.IList<DBListDBSrc>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DBListDBSrc> Tolist()
        {
            System.Collections.Generic.List<DBListDBSrc> list = new System.Collections.Generic.List<DBListDBSrc>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DBListDBSrc)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
