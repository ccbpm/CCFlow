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
    /// 百分比扇形图
    /// </summary>
    public class ChartRate : EntityNoName
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
        /// 百分比扇形图
        /// </summary>
        public ChartRate()
        {
        }
        /// <summary>
        /// 百分比扇形图
        /// </summary>
        /// <param name="no"></param>
        public ChartRate(string no)
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

                Map map = new Map("GPM_WindowTemplate", "百分比扇形图");

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

                #region 数据源.
                map.AddDDLSysEnum(WindowTemplateAttr.DBType, 0, "数据源类型", true, true, "WindowsDBType",
          "@0=数据库查询SQL@1=执行Url返回Json@2=执行\\DataUser\\JSLab\\Windows.js的函数.");
                map.AddDDLEntities(WindowTemplateAttr.DBSrc, null, "数据源", new BP.Sys.SFDBSrcs(), true);


                map.AddTBString(WindowTemplateAttr.LabOfFZ, null, "分子标签", true, false, 0, 100, 20);
                map.AddTBStringDoc(WindowTemplateAttr.SQLOfFZ, null, "分子表达式", true, false, true);

                map.AddTBString(WindowTemplateAttr.LabOfFM, null, "分母标签", true, false, 0, 100, 20);
                map.AddTBStringDoc(WindowTemplateAttr.SQLOfFM, null, "分子表达式", true, false, true);

                map.AddTBString(WindowTemplateAttr.LabOfRate, null, "率标签", true, false, 0, 100, 20);

                #endregion 数据源.

                this._enMap = map;  
                 
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 百分比扇形图s
    /// </summary>
    public class ChartRates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 百分比扇形图s
        /// </summary>
        public ChartRates()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ChartRate();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ChartRate> ToJavaList()
        {
            return (System.Collections.Generic.IList<ChartRate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ChartRate> Tolist()
        {
            System.Collections.Generic.List<ChartRate> list = new System.Collections.Generic.List<ChartRate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ChartRate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
