using BP.En;

namespace BP.CCFast.Portal.WindowExt
{
    /// <summary>
    /// 折线图
    /// </summary>
    public class ChartLine : EntityNoName
    {
        #region 权限控制.
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.IsView = false;
                uac.IsInsert = false;
                uac.IsDelete = false;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        #endregion 属性

        #region 构造方法
        /// <summary>
        /// 折线图
        /// </summary>
        public ChartLine()
        {
        }
        /// <summary>
        /// 折线图
        /// </summary>
        /// <param name="no"></param>
        public ChartLine(string no)
        {
            this.No = no;
            this.Retrieve();
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

                this._enMap = Glo.StationDBSrcMap("ChartLine折线图");

                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 折线图s
    /// </summary>
    public class ChartLines : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 折线图s
        /// </summary>
        public ChartLines()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ChartLine();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ChartLine> ToJavaList()
        {
            return (System.Collections.Generic.IList<ChartLine>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ChartLine> Tolist()
        {
            System.Collections.Generic.List<ChartLine> list = new System.Collections.Generic.List<ChartLine>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ChartLine)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
