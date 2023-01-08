using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
using BP.Web;
using System.IO;
using BP.Difference;

namespace BP.Sys
{
    /// <summary>
    /// UI的设置.Search. Card, Group信息.
    /// </summary>
    public class UIConfig
    {
        public Entity HisEn;
        public AtPara HisAP;
        public UIConfig()
        {
        }
        /// <summary>
        /// UI的设置.Search. Card, Group信息.
        /// </summary>
        /// <param name="enName"></param>
        public UIConfig(Entity en)
        {
            this.HisEn = en;
            EnCfg cfg = new EnCfg(en.ToString());
            string paraStr = cfg.UI;
            if (DataType.IsNullOrEmpty(paraStr) == true)
                paraStr = "@UIRowStyleGlo=0@IsEnableDouclickGlo=1@IsEnableRefFunc=1@IsEnableFocusField=1@IsEnableOpenICON=1@FocusField=''@WinCardH=600@@WinCardW=800@ShowColumns=";
            HisAP = new AtPara(paraStr);
        }
        /// <summary>
        /// 获取显示列数组，中间用,隔开
        /// </summary>
        public string[] ShowColumns
        {
            get
            {
                var colstr = this.HisAP.GetValStrByKey("ShowColumns");

                if (string.IsNullOrWhiteSpace(colstr))
                    return new string[0];

                return colstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #region 移动.
        /// <summary>
        /// 移动到方式.
        /// </summary>
        public MoveToShowWay MoveToShowWay
        {
            get
            {
                return (MoveToShowWay)this.HisAP.GetValIntByKey("MoveToShowWay");
            }
        }


        /// <summary>
        /// 移动到字段
        /// </summary>
        public string MoveTo
        {
            get
            {
                string s = this.HisAP.GetValStrByKey("MoveTo");
                return s;
            }
        }
        #endregion 移动.

        /// <summary>
        /// 风格类型
        /// </summary>
        public int UIRowStyleGlo
        {
            get
            {
                return this.HisAP.GetValIntByKey("UIRowStyleGlo");
            }
        }
        /// <summary>
        /// 是否启用双击打开？
        /// </summary>
        public bool IsEnableDouclickGlo
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableDouclickGlo");
            }
        }
        /// <summary>
        /// 是否显示相关功能?
        /// </summary>
        public bool IsEnableRefFunc
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableRefFunc");
            }
        }
        /// <summary>
        /// 是否启用焦点字段
        /// </summary>
        public bool IsEnableFocusField
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableFocusField");
            }
        }
        /// <summary>
        /// 是否打开ICON
        /// </summary>
        public bool IsEnableOpenICON
        {
            get
            {
                return this.HisAP.GetValBoolenByKey("IsEnableOpenICON");
            }
        }
        /// <summary>
        /// 焦点字段
        /// </summary>
        public string FocusField
        {
            get
            {
                string s = this.HisAP.GetValStrByKey("FocusField");
                if (DataType.IsNullOrEmpty(s))
                {
                    if (this.HisEn.EnMap.Attrs.Contains("Name"))
                        return "Name";
                    if (this.HisEn.EnMap.Attrs.Contains("Title"))
                        return "Title";
                }
                return s;
            }
        }
        public int WinCardW
        {
            get
            {
                return this.HisAP.GetValIntByKey("WinCardW");
            }
        }
        public int WinCardH
        {
            get
            {
                return this.HisAP.GetValIntByKey("WinCardH");
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            EnCfg cfg = new EnCfg(this.HisEn.ToString());
            cfg.UI = this.HisAP.GenerAtParaStrs();
            return cfg.Save();
        }
    }
    /// <summary>
    ///  配置信息
    /// </summary>
    public class EnCfgAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 是否是数据加密
        /// </summary>
        public const string IsJM = "IsJM";
        /// <summary>
        /// 是否查询条件多选
        /// </summary>
        public const string IsSelectMore = "IsSelectMore";
        /// <summary>
        /// Url
        /// </summary>
        public const string UrlExt = "UrlExt";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJSavePath = "FJSavePath";
        /// <summary>
        /// 附件路径
        /// </summary>
        public const string FJWebPath = "FJWebPath";
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public const string Datan = "Datan";
        /// <summary>
        /// Search,Group,设置.
        /// </summary>
        public const string UI = "UI";

        /// <summary>
        /// 字段颜色设置
        /// </summary>
        public const string ColorSet = "ColorSet";
        /// <summary>
        /// 字段求和求平均设置
        /// </summary>
        public const string FieldSet = "FieldSet";
        public const string Drill = "Drill";
        public const string MobileFieldShowModel = "MobileFieldShowModel";
        public const string MobileShowContent = "MobileShowContent";
        /// <summary>
        /// 导入功能Url
        /// </summary>
        public const string ImpFuncUrl = "ImpFuncUrl";
        /// <summary>
        /// 是否显示分析按钮在查询工具栏里.
        /// </summary>
        public const string IsGroup = "IsGroup";
        /// <summary>
        /// 是否记录查询条件？
        /// </summary>
        public const string IsCond = "IsCond";
    }
    /// <summary>
    /// EnCfgs
    /// </summary>
    public class EnCfg : EntityNoName
    {
        #region UI设置.
        public string UI
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.UI);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.UI, value);
            }
        }
        #endregion UI设置.

        #region 基本属性
        /// <summary>
        /// 数据分析方式
        /// </summary>
        public string Datan
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.Datan);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.Datan, value);
            }
        }
        /// <summary>
        /// 附件路径
        /// </summary>
        public string FJSavePath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJSavePath);
                if (DataType.IsNullOrEmpty(str) == true)
                    return BP.Difference.SystemConfig.PathOfDataUser + this.No + "/";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJSavePath, value);
            }
        }
        /// <summary>
        /// 附件存储位置.
        /// </summary>
        public string FJWebPath
        {
            get
            {
                string str = this.GetValStringByKey(EnCfgAttr.FJWebPath);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = Path.Combine(HttpContextHelper.RequestApplicationPath,
                        "DataUser/", this.No);
                str = str.Replace("\\", "/");
                if (!str.EndsWith("/"))
                    str += "/";
                return str;
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FJWebPath, value);
            }
        }
        #endregion

        #region 参数属性.
        /// <summary>
        /// 批处理-设置页面大小
        /// </summary>
        public int PageSizeOfBatch
        {
            get
            {
                return this.GetParaInt("PageSizeOfBatch", 600);
            }
            set
            {
                this.SetPara("PageSizeOfBatch", value);
            }
        }
        /// <summary>
        /// 批处理-设置页面大小
        /// </summary>
        public int PageSizeOfSearch
        {
            get
            {
                return this.GetParaInt("PageSizeOfSearch", 15);
            }
            set
            {
                this.SetPara("PageSizeOfSearch", value);
            }
        }

        public string FieldSet
        {
            get
            {
                return this.GetValStringByKey(EnCfgAttr.FieldSet);
            }
            set
            {
                this.SetValByKey(EnCfgAttr.FieldSet, value);
            }
        }
        #endregion 参数属性.

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                if (WebUser.IsAdmin == true)
                {
                    uac.IsInsert = false;
                    uac.IsDelete = false;
                    uac.IsUpdate = true;
                }
                else
                {
                    uac.Readonly();
                    uac.IsView = false;
                }
                return uac;
            }
        }
        /// <summary>
        /// 系统实体
        /// </summary>
        public EnCfg()
        {
        }
        /// <summary>
        /// 系统实体
        /// </summary>
        /// <param name="no"></param>
        public EnCfg(string enName)
        {
            this.No = enName;
            this.RetrieveFromDBSources();
        }
        public override int Retrieve()
        {
            return base.RetrieveFromDBSources();
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_EnCfg", "实体配置");
                map.AddGroupAttr("基础信息");
                map.AddTBStringPK(EnCfgAttr.No, null, "实体名称", true, false, 1, 100, 60);

                #region 基本信息设置.
                //string cfg1 = "@0=En.htm 实体与实体相关功能编辑器";
                map.AddDDLSysEnum("UIRowStyleGlo", 0, "表格数据行风格(应用全局)", true, true);
                map.AddBoolean("IsEnableDouclickGlo", true, "是否启动双击打开(应用全局)?", true, true);

                map.AddBoolean("IsEnableFocusField", false, "是否启用焦点字段?", true, true);
                map.AddTBString("FocusField", null, "焦点字段", true, false, 0, 30, 60, true);
                map.SetHelperAlert("FocusField", "用于显示点击打开的列，比如:Name, Title列.");

                map.AddBoolean("IsEnableRefFunc", false, "是否启用相关功能列?", true, true);
                map.AddBoolean("IsEnableOpenICON", false, "是否启用打开图标?", true, true);

                //数据加密存储.
                map.AddBoolean(EnCfgAttr.IsJM, false, "是否是加密存储?", true, true);
                map.AddBoolean(EnCfgAttr.IsSelectMore, false, "是否下拉查询条件多选?", true, true);

                map.AddDDLSysEnum("MoveToShowWay", 0, "移动到显示方式", true, true);

                map.AddDDLSysEnum("TableCol", 0, "实体表单显示列数", true, true, "实体表单显示列数",
                 "@0=4列@1=6列");
                map.AddBoolean("IsShowIcon", false, "是否显示项目图标", true, true);
                map.AddTBString("KeyLabel", null, "关键字Label", true, false, 0, 30, 60, true);
                map.SetHelperAlert("KeyLabel", "(默认为:关键字:)");
                map.AddTBString("KeyPlaceholder", null, "关键字提示", true, false, 0, 300, 60, true);

                map.AddTBInt("PageSize", 10, "页面显示的条数", true, true);
                map.SetHelperAlert("PageSize", "(默认:10)");

                map.AddTBInt("FontSize", 14, "页面字体大小", true, true);
                map.SetHelperAlert("FontSize", "(默认:14px)");
                map.AddDDLSysEnum("EditerType", 0, "大块文本编辑器", true, true);

                map.AddBoolean("IsCond", false, "退出后是否清空查询条件?", true, true);
                map.SetHelperAlert("IsCond", "在查询组件中，是不是每次进入前都要清空以前的查询条件？默认不清空。");
                #endregion 基本信息设置.

                #region 查询排序.
                map.AddGroupAttr("查询排序");
                map.AddTBString("OrderBy", null, "查询排序字段", true, false, 0, 100, 60);
                map.AddBoolean("IsDeSc", true, "是否降序排序?", true, true);
                #endregion 查询排序.

                #region 附件保存路径.
                map.AddTBString(EnCfgAttr.FJSavePath, null, "附加保存路径", true, false, 0, 100, 60);
                map.AddTBString(EnCfgAttr.FJWebPath, null, "附件Web路径", true, false, 0, 100, 60);
                #endregion 附件保存路径.

                #region 其他高级设置.
                map.AddTBString(EnCfgAttr.Datan, null, "字段数据分析方式", true, false, 0, 200, 60);
                map.AddTBString(EnCfgAttr.UI, null, "UI设置", true, false, 0, 2000, 60);

                //字段颜色设置.
                map.AddTBString(EnCfgAttr.ColorSet, null, "颜色设置", true, false, 0, 500, 60, true);
                string msg = "对字段的颜色处理";
                msg += "\t\n@Age:From=0,To=18,Color=green;From=19,To=30,Color=red";
                map.SetHelperAlert(EnCfgAttr.ColorSet, msg);
                //对字段求总和平均.
                map.AddTBString(EnCfgAttr.FieldSet, null, "字段求和/平均设置", true, false, 0, 500, 60, true);

                //字段格式化函数.
                map.AddTBString("ForamtFunc", null, "字段格式化函数", true, false, 0, 200, 60, true);
                msg = "对字段的显示使用函数进行处理";
                msg += "\t\n 1. 对于字段内容需要处理后在输出出来.";
                msg += "\t\n 2. 比如：原字段内容 @zhangsa,张三@lisi,李四 显示的内容为 张三,李四";
                msg += "\t\n 3. 配置格式: 字段名@函数名; 比如:  FlowEmps@DealFlowEmps; ";
                msg += "\t\n 4. 函数写入到 \\DataUser\\JSLibData\\SearchSelf.js";
                map.SetHelperAlert("ForamtFunc", msg);


                map.AddTBString(EnCfgAttr.Drill, null, "数据钻取", true, false, 0, 200, 60, true);
                msg = "显示钻取链接的字段";
                msg += "\t\n 格式: @Age@JinE@ShouYi";
                msg += "\t\n 显示在Search, En组件里面.";
                map.SetHelperAlert(EnCfgAttr.Drill, msg);

                map.AddDDLSysEnum(EnCfgAttr.MobileFieldShowModel, 0, "移动端列表字段显示方式", true, true, EnCfgAttr.MobileFieldShowModel, "@0=默认设置@1=设置显示字段@2=设置模板");
              

                map.AddTBStringDoc(EnCfgAttr.MobileShowContent, EnCfgAttr.MobileShowContent, null, "移动端列表字段设置", true,
                    false, 0, 500, 10, true);

                string help = "格式1: Key1,Key2, 格式2: @Key1@Key2@Key3@";
                map.SetHelperAlert(EnCfgAttr.MobileShowContent, help);
                #endregion 其他高级设置.

                #region  Search.工具栏按钮.
                map.AddGroupAttr("工具栏按钮");
                map.AddBoolean("BtnsShowLeft", false, "按钮显示到左边?", true, true, false);
                msg = "配置的按钮显示位置.";
                msg += "\t\n1.默认配置的按钮显示在右边位置. ";
                msg += "\t\n1.这些按钮包括自定义按钮，新建，导入，导出，分组。";
                map.SetHelperAlert("BtnsShowLeft", msg);

                //导入功能.
                map.AddBoolean("IsImp", true, "是否显示导入?", true, true, false);
                map.AddTBString(EnCfgAttr.ImpFuncUrl, null, "导入功能Url", true, false, 0, 500, 60, true);
                map.SetHelperAlert(EnCfgAttr.ImpFuncUrl, "如果为空，则使用通用的导入功能.");

                map.AddBoolean("IsExp", false, "是否显示导出", true, true, true);
                map.AddBoolean(EnCfgAttr.IsGroup, true, "是否显示分析按钮（在查询工具栏里）?", true, true, true);
                map.AddBoolean("IsEnableLazyload", true, "是否启用懒加载？（对树结构实体有效）?", true, true, true);

                map.AddTBString("BtnLab1", null, "集合:自定义按钮标签1", true, false, 0, 70, 60, false);
                map.SetHelperAlert("BtnLab1", "自定义按钮与标签,函数可以写入到/DataUser/JSLabData/SearchSelf.js里面.");
                map.AddTBString("BtnJS1", null, "集合:Url/Javasccript", true, false, 0, 300, 60, false);

                map.AddTBString("BtnLab2", null, "集合:自定义按钮标签2", true, false, 0, 70, 60, false);
                map.AddTBString("BtnJS2", null, "集合:Url/Javasccript", true, false, 0, 300, 60, false);

                map.AddTBString("BtnLab3", null, "集合:自定义按钮标签3", true, false, 0, 70, 60, false);
                map.AddTBString("BtnJS3", null, "集合:Url/Javasccript", true, false, 0, 300, 60, false);
                #endregion 按钮配置信息 - 自定义按钮.

                #region  EnOnly.按钮配置信息.

                map.AddTBString("EnBtnLab1", null, "实体:自定义按钮标签1", true, false, 0, 70, 60, false);
                map.SetHelperAlert("EnBtnLab1", "实体:自定义按钮与标签,函数可以写入到/DataUser/JSLabData/SearchSelf.js里面.");
                map.AddTBString("EnBtnJS1", null, "实体:Url/Javasccript", true, false, 0, 300, 60, false);

                map.AddTBString("EnBtnLab2", null, "实体:自定义按钮标签2", true, false, 0, 70, 60, false);
                map.AddTBString("EnBtnJS2", null, "实体:Url/Javasccript", true, false, 0, 300, 60, false);
                #endregion 按钮配置信息 - 自定义按钮.

                #region 双击/单击行的配置.
                map.AddGroupAttr("双击/单击行的配置");
                string cfg = "@0=En.htm 实体与实体相关功能编辑器";
                cfg += "@1=EnOnly.htm 实体编辑器";
                cfg += "@2=/CCForm/FrmGener.htm 傻瓜表单解析器";
                cfg += "@3=/CCForm/FrmGener.htm 自由表单解析器";
                cfg += "@9=自定义URL";
                map.AddDDLSysEnum("SearchUrlOpenType", 0, "双击/单击行打开内容", true, true, "SearchUrlOpenType", cfg);
                map.AddBoolean("IsRefreshParentPage", true, "关闭后是否刷新本页面", true, true);

                map.AddTBString(EnCfgAttr.UrlExt, null, "要打开的Url", true, false, 0, 500, 60, true);
                map.AddDDLSysEnum("DoubleOrClickModel", 0, "双击/单击行弹窗模式", true, true, "DoubleOrClickModel",
                   "@0=双击行弹窗@1=单击行弹窗");
                map.AddDDLSysEnum("OpenModel", 0, "打开方式", true, true, "OpenModel",
                    "@0=弹窗-强制关闭@1=新窗口打开-winopen模式@2=弹窗-非强制关闭@3=执行指定的方法.@4=流程设计器打开模式");
                map.AddTBString("OpenModelFunc", null, "弹窗方法", true, false, 0, 300, 60, false);

                msg = "首先在写一个函数，放入到:/DataUser/JSLab/SearchSelf.js里面 ";
                msg += "\t\n 该函数里 OpenIt(传入一个已经计算好的url);";
                msg += "\t\n 比如您写一个方法: OpenItMyUrl(url);";
                map.SetHelperAlert("OpenModelFunc", msg);

                // map.AddTBInt("WinCardW", 1000, "窗体宽度", false, false);
                //  map.AddTBInt("WinCardH", 600, "高度", false, false);
                map.AddDDLSysEnum("WinCardW", 0, "宽度", true, true, "WinCardW", "@0=75%@1=50%@2=100%@3=25%");
                map.AddDDLSysEnum("WinCardH", 2, "高度", true, true, "WinCardH", "@0=75%@1=50%@2=100%@3=85%@4=25%");

                #endregion

                map.AddTBAtParas(3000);  //参数属性.


                #region 执行的方法 - 基本功能
                map.AddGroupMethod("基本功能");
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设置显示的列";
                rm.ClassMethodName = this.ToString() + ".SearchSettingCols()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "多表头设置";
                rm.ClassMethodName = this.ToString() + ".MultiTitle()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "启用傻瓜表单设计器";
                rm.ClassMethodName = this.ToString() + ".DesignerFool()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "导入数据";
                rm.ClassMethodName = this.ToString() + ".ImpData()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "字段颜色范围设置";
                rm.ClassMethodName = this.ToString() + ".FieldColors()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段求和/平均设置";
                rm.ClassMethodName = this.ToString() + ".FieldAvgSum()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "清除设计内容";
                rm.ClassMethodName = this.ToString() + ".ClearData()";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "移动端Search";
                rm.ClassMethodName = this.ToString() + ".ToMSearch()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                #endregion 执行的方法.

                #region 执行的方法 - 数据库信息
                rm = new RefMethod();
                rm.Title = "表结构";
                rm.ClassMethodName = this.ToString() + ".FieldDesc()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段UI";
                rm.ClassMethodName = this.ToString() + ".FieldDescUI()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string FieldDescUI()
        {
            return "../../Comm/Sys/SystemClassFieldUI.htm?EnsName=" + this.No;
        }

        public string FieldDesc()
        {
            return "../../Comm/Sys/SystemClassField.htm?EnsName=" + this.No;
        }

        public string ToMSearch()
        {
            return "../../../CCMobile/Comm/Search.htm?EnsName=" + this.No;
        }

        /// <summary>
        /// 字段颜色设置
        /// </summary>
        /// <returns></returns>
        public string FieldColors()
        {
            return "../../Comm/Sys/FieldColors.htm?EnsName=" + this.No;
        }
        /// <summary>
        /// 字段求和/平均设置
        /// </summary>
        /// <returns></returns>
        public string FieldAvgSum()
        {
            return "../../Comm/Sys/FieldAvgSum.htm?EnsName=" + this.No;
        }
        /// <summary>
        /// 清除数据.
        /// </summary>
        /// <returns></returns>
        public string ClearData()
        {
            MapData md = new MapData(this.No);
            md.No = this.No;
            if (md.RetrieveFromDBSources() == 0)
                return "err@没有设计的数据要清除.";
            md.Delete();


            return "清除成功.";
        }
        public string ImpData()
        {
            return "../../Comm/Sys/ImpData.htm?EnsName=" + this.No;
        }
        public string DesignerFool()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No;
        }

        public string SearchSettingCols()
        {
            return "../../Comm/Sys/SearchSettingCols.htm?EnsName=" + this.No;
        }

        public string SearchSetting()
        {
            return "../../Comm/Sys/SearchSetting.htm?EnsName=" + this.No;
        }
        /// <summary>
        /// 多表头
        /// </summary>
        /// <returns></returns>
        public string MultiTitle()
        {
            return "../../Comm/Sys/MultiTitle.htm?EnsName=" + this.No + "&DoType=Search";
        }
        /// <summary>
        /// 生成他的Attrs
        /// </summary>
        /// <returns></returns>
        public string GenerAttrs()
        {
            Entities ens = ClassFactory.GetEns(this.No);
            if (ens == null)
                return "err@" + this.No + ",类名错误.";
            MapAttrs attrs = new MapAttrs();
            MapData md = new MapData();
            md.No = this.No;
            int count = md.RetrieveFromDBSources();
            if (count == 0)
                attrs = ens.GetNewEntity.EnMap.Attrs.ToMapAttrs;
            else
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.No, MapAttrAttr.Idx);

            return attrs.ToJson();
        }
    }
    /// <summary>
    /// 实体集合
    /// </summary>
    public class EnCfgs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 配置信息
        /// </summary>
        public EnCfgs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnCfg();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EnCfg> ToJavaList()
        {
            return (System.Collections.Generic.IList<EnCfg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EnCfg> Tolist()
        {
            System.Collections.Generic.List<EnCfg> list = new System.Collections.Generic.List<EnCfg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EnCfg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
