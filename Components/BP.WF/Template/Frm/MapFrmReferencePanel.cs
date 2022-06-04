using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
    /// <summary>
    /// Word表单属性 attr
    /// </summary>
    public class MapFrmReferencePanelAttr : MapDataAttr
    {
        /// <summary>
        /// 标题
        /// </summary>
		public const string RefTitle = "RefTitle";
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string RefWorkModel = "RefWorkModel";
        /// <summary>
        /// 连接
        /// </summary>
        public const string RefUrl = "RefUrl";
        /// <summary>
        /// 触发字段
        /// </summary>
        public const string RefBlurField = "RefBlurField";
        /// <summary>
        /// 静态的html脚本内容
        /// </summary>
        public const string RefHtml = "RefHtml";

    }
    /// <summary>
    /// Word表单属性
    /// </summary>
    public class MapFrmReferencePanel : EntityNoName
    {
        #region 文件模版属性.
        /// <summary>
        /// 模版版本号
        /// </summary>
        public string RefTitle
        {
            get
            {
                return this.GetValStringByKey(MapFrmReferencePanelAttr.RefTitle);
            }
            set
            {
                this.SetValByKey(MapFrmReferencePanelAttr.RefTitle, value);
            }
        }
        #endregion 文件模版属性.


        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true)
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
                return uac;
            }
        }
        #endregion 权限控制.

        #region 构造方法
        /// <summary>
        /// Word表单属性
        /// </summary>
        public MapFrmReferencePanel()
        {
        }
        /// <summary>
        /// Word表单属性
        /// </summary>
        /// <param name="no">表单ID</param>
        public MapFrmReferencePanel(string no)
            : base(no)
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
                Map map = new Map("Sys_MapData", "参考面板");


                #region 基本属性.
                map.AddTBStringPK(MapFrmReferencePanelAttr.No, null, "表单编号", true, true, 1, 190, 20);

                map.AddDDLSysEnum(MapFrmReferencePanelAttr.RefWorkModel, 0, "工作模式", true, true,
                    MapFrmReferencePanelAttr.RefWorkModel, "@0=禁用@1=静态Html脚本@2=静态框架Url@3=动态Url@4=动态Html脚本");

                map.AddTBString(MapFrmReferencePanelAttr.RefBlurField, null, "失去焦点字段", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapFrmReferencePanelAttr.RefBlurField, "配置表单字段名字，对【动态url】有效.");

                map.AddTBString(MapFrmReferencePanelAttr.RefUrl, null, "连接", true, false, 0, 500, 20, true);
                map.AddTBStringDoc(MapFrmReferencePanelAttr.RefHtml, null, "静态Html脚本", true, false, true);

                #endregion 基本属性.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


    }
    /// <summary>
    /// Word表单属性s
    /// </summary>
    public class MapFrmReferencePanels : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// Word表单属性s
        /// </summary>
        public MapFrmReferencePanels()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrmReferencePanel();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrmReferencePanel> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrmReferencePanel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrmReferencePanel> Tolist()
        {
            System.Collections.Generic.List<MapFrmReferencePanel> list = new System.Collections.Generic.List<MapFrmReferencePanel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrmReferencePanel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
