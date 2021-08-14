using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.GPM.Home.WindowExt;
using BP.GPM.Home;

namespace BP.GPM.Home.WindowExt
{
    public class Glo
    {
        public static Map StationDBSrcMap(string desc)
        {
            Map map = new Map("GPM_WindowTemplate", desc);

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
            map.AddTBStringDoc(WindowTemplateAttr.Docs, null, "SQL内容表达式", true, false, true);
            #endregion 数据源.

            return map;


        }
    }
}
