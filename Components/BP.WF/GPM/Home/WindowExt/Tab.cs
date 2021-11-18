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
    /// 标签页
    /// </summary>
    public class Tab : EntityNoName
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
        /// 标签页
        /// </summary>
        public Tab()
        {
        }
        /// <summary>
        /// 标签页
        /// </summary>
        /// <param name="no"></param>
        public Tab(string no)
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

                Map map = new Map("GPM_WindowTemplate", "Tab标签页");

                #region 基本信息.
                map.AddTBStringPK(WindowTemplateAttr.No, null, "编号", true, true, 1, 40, 200);
                map.AddTBInt(WindowTemplateAttr.ColSpan, 2, "占的列数", true, false);
                map.SetHelperAlert(WindowTemplateAttr.ColSpan, "画布按照4列划分布局，输入的输在在1=4之间.");
                map.AddTBString(WindowTemplateAttr.Name, null, "标题", true, false, 0, 300, 20, true);
                map.AddTBString(WindowTemplateAttr.Icon, null, "Icon", true, false, 0, 100, 20, true);
                #endregion 基本信息.

                #region 更多链接.
                map.AddTBString(WindowTemplateAttr.MoreUrl, null, "更多链接", true, false, 0, 300, 20, true);
                map.AddDDLSysEnum(WindowTemplateAttr.MoreLinkModel, 0, "打开方式", true, true, WindowTemplateAttr.MoreLinkModel,
              "@0=新窗口@1=本窗口@2=覆盖新窗口");
                map.AddTBString(WindowTemplateAttr.MoreLab, null, "更多标签", true, false, 0, 300, 20);
                #endregion 更多链接.

                map.AddDtl(new TabDtls(), DtlAttr.RefPK);

                RefMethod rm = new RefMethod();
                rm.Title = "样例";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".AddTemplate()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据源";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".AddDBSrc()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据源参考";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.ClassMethodName = this.ToString() + ".RefSQL()";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        public string AddTemplate()
        {
            return "../../GPM/Window/Tab.png";
        }
        public string RefSQL()
        {
            return "../../GPM/Window/RefSQL.htm";
        }
        public string AddDBSrc()
        {
            return "../../Comm/Search.htm?EnsName=BP.Sys.SFDBSrcs";
        }
        #endregion 方法.

    }
    /// <summary>
    /// 标签页s
    /// </summary>
    public class Tabs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 标签页s
        /// </summary>
        public Tabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Tab();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Tab> ToJavaList()
        {
            return (System.Collections.Generic.IList<Tab>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Tab> Tolist()
        {
            System.Collections.Generic.List<Tab> list = new System.Collections.Generic.List<Tab>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Tab)this[i]);

            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
