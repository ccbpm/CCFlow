using BP.En;


namespace BP.CCFast.Portal.WindowExt
{
    /// <summary>
    /// 框架信息块
    /// </summary>
    public class iFrame : EntityNoName
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
        /// 框架信息块
        /// </summary>
        public iFrame()
        {
        }
        /// <summary>
        /// 框架信息块
        /// </summary>
        /// <param name="no"></param>
        public iFrame(string no)
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

                Map map = new Map("GPM_WindowTemplate", "iFrame框架信息块");

                #region 基本信息.
                map.AddTBStringPK(WindowTemplateAttr.No, null, "编号", true, true, 1, 40, 200);
                map.AddTBInt(WindowTemplateAttr.ColSpan, 1, "占的列数", true, false);
                map.SetHelperAlert(WindowTemplateAttr.ColSpan, "画布按照4列划分布局，输入的输在在1=4之间.");
                map.AddTBString(WindowTemplateAttr.Name, null, "标题", true, false, 0, 300, 20, true);
                map.AddTBString(WindowTemplateAttr.Icon, null, "Icon", true, false, 0, 100, 20, true);
                #endregion 基本信息.

                map.AddTBString(WindowTemplateAttr.Docs, null, "Url", true, false, 0, 100, 20, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string AddTemplate()
        {
            return "../../GPM/Window/iFrame.png";
        }
    }
    /// <summary>
    /// 框架信息块s
    /// </summary>
    public class iFrames : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 框架信息块s
        /// </summary>
        public iFrames()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new iFrame();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<iFrame> ToJavaList()
        {
            return (System.Collections.Generic.IList<iFrame>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<iFrame> Tolist()
        {
            System.Collections.Generic.List<iFrame> list = new System.Collections.Generic.List<iFrame>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((iFrame)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
