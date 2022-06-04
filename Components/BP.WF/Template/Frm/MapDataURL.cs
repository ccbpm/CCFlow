using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
    /// <summary>
    /// 表单属性
    /// </summary>
    public class MapDataURL : EntityNoName
    {
        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin")==true)
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
 
        #region 属性
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (DataType.IsNullOrEmpty(s) == true)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string UrlExt
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.UrlExt);
            }
            set
            {
                this.SetValByKey(MapDataAttr.UrlExt, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单属性
        /// </summary>
        public MapDataURL()
        {
        }
        /// <summary>
        /// 表单属性
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapDataURL(string no)
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

                Map map = new Map("Sys_MapData", "URL表单属性");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, false, 1, 200, 20);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.PTable, null, "存储表", false, false, 0, 500, 20);

                map.AddTBString(MapDataAttr.UrlExt, null, "URL连接", true, false, 0, 500, 20, true);

                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型",true, false, MapDataAttr.FrmType);

                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false, true);

                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.


                RefMethod rm = new RefMethod();
                rm.Title = "傻瓜表单设计";
                rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
                rm.Icon = "icon-note";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "开发者表单设计器";
                rm.ClassMethodName = this.ToString() + ".DoDesignerDev";
                rm.Icon = "icon-note";
                rm.Visable = true;
                rm.Target = "_blank";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoDesignerDev()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsFirst=1&IsEditMapData=True";
        }
        /// <summary>
        /// 傻瓜表单设计器
        /// </summary>
        /// <returns></returns>
        public string DoDesignerFool()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsFirst=1&IsEditMapData=True";
        }

        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoOpenUrl()
        {
            return this.UrlExt;
        }
    }
    /// <summary>
    /// 表单属性s
    /// </summary>
    public class MapDataURLs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 表单属性s
        /// </summary>
        public MapDataURLs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapDataURL();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapDataURL> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapDataURL>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapDataURL> Tolist()
        {
            System.Collections.Generic.List<MapDataURL> list = new System.Collections.Generic.List<MapDataURL>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapDataURL)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
