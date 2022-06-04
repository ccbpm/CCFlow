using BP.En;


namespace BP.CCOA.CCBBS
{
    /// <summary>
    /// 类型 属性
    /// </summary>
    public class BBSTypeAttr : EntityNoNameAttr
    {
    }
    /// <summary>
    /// 类型
    /// </summary>
    public class BBSType : EntityNoName
    {
        #region 基本属性
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                uac.IsInsert = true;
                return uac;
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public BBSType()
        {
        }
        public BBSType(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("OA_BBSType", "类型类型");
                map.setCodeStruct("3");
                map.AddTBStringPK(BBSTypeAttr.No, null, "编号", false, true, 3, 3, 3);
                map.AddTBString(BBSTypeAttr.Name, null, "名称", true, false, 0, 100, 10, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 类型 s
    /// </summary>
    public class BBSTypes : EntitiesNoName
    {
        #region 重写.
        /// <summary>
        /// 类型
        /// </summary>
        public BBSTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BBSType();
            }
        }
        #endregion 重写.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<BBSType> ToJavaList()
        {
            return (System.Collections.Generic.IList<BBSType>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<BBSType> Tolist()
        {
            System.Collections.Generic.List<BBSType> list = new System.Collections.Generic.List<BBSType>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((BBSType)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
