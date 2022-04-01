using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text.RegularExpressions;


namespace BP.GPM.Home.WindowExt
{
    /// <summary>
    /// 柱状图
    /// </summary>
    public class ChartChina : EntityNoName
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
        /// 柱状图
        /// </summary>
        public ChartChina()
        {
        }
        /// <summary>
        /// 柱状图
        /// </summary>
        /// <param name="no"></param>
        public ChartChina(string no)
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

                Map map = new Map("GPM_WindowTemplate", "中国地图");

                #region 基本信息.
                map.AddTBStringPK(WindowTemplateAttr.No, null, "编号", true, true, 1, 40, 200);
                map.AddTBInt(WindowTemplateAttr.ColSpan, 1, "占的列数", true, false);
                map.SetHelperAlert(WindowTemplateAttr.ColSpan, "画布按照4列划分布局，输入的输在在1=4之间.");
                map.AddTBString(WindowTemplateAttr.Name, null, "标题", true, false, 0, 300, 20, true);
                map.AddTBString(WindowTemplateAttr.Icon, null, "Icon", true, false, 0, 100, 20, true);
                #endregion 基本信息.

                map.AddTBString(WindowTemplateAttr.MoreLab, null, "更多标签", true, false, 0, 300, 20, true);
                map.AddTBString(WindowTemplateAttr.MoreUrl, null, "更多链接", true, false, 0, 300, 20, true);
                map.AddDDLSysEnum(WindowTemplateAttr.MoreLinkModel, 0, "打开方式", true, true, WindowTemplateAttr.MoreLinkModel,
              "@0=新窗口@1=本窗口@2=覆盖新窗口");

                map.AddBoolean("IsPie", false, "饼图?", true, true);
                map.AddBoolean("IsLine", false, "折线图?", true, true);
                map.AddBoolean("IsZZT", false, "柱状图?", true, true);
                map.AddBoolean("IsRing", false, "显示环形图?", true, true);
             //   map.AddBoolean("IsRate", false, "百分比扇形图?", true, true);

                map.AddDDLSysEnum(WindowTemplateAttr.DefaultChart, 0, "默认显示图形", true, true, WindowTemplateAttr.DefaultChart,
            "@0=饼图@1=折线图@2=柱状图@3=环形图");

                #region 数据源.
                map.AddDDLSysEnum(WindowTemplateAttr.DBType, 0, "数据源类型", true, true, "WindowsDBType",
          "@0=数据库查询SQL@1=执行Url返回Json@2=执行\\DataUser\\JSLab\\Windows.js的函数.");
                map.AddTBStringDoc(WindowTemplateAttr.Docs, null, "SQL内容表达式", true, false, true);

                //map.AddTBStringDoc(WindowTemplateAttr.C1Ens, null, "列1外键数据(可选)", true, false, true);
                //map.AddTBStringDoc(WindowTemplateAttr.C2Ens, null, "列2外键数据(可选)", true, false, true);
                //map.AddTBStringDoc(WindowTemplateAttr.C3Ens, null, "列3外键数据(可选)", true, false, true);

                map.AddDDLEntities(WindowTemplateAttr.DBSrc, null, "数据源", new BP.Sys.SFDBSrcs(), true);
                #endregion 数据源.


                this._enMap = map;

                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 柱状图s
    /// </summary>
    public class ChartChinas : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 柱状图s
        /// </summary>
        public ChartChinas()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ChartChina();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ChartChina> ToJavaList()
        {
            return (System.Collections.Generic.IList<ChartChina>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ChartChina> Tolist()
        {
            System.Collections.Generic.List<ChartChina> list = new System.Collections.Generic.List<ChartChina>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ChartChina)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
