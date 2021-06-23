using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text.RegularExpressions;
namespace BP.GPM.Home
{
    /// <summary>
    /// 信息块
    /// </summary>
    public class WindowTemplateAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 信息块类型
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Docs = "Docs";
        public const string WinDocType = "WinDocType";
        /// <summary>
        /// tag1
        /// </summary>
        public const string ColSpan = "ColSpan";
        /// <summary>
        /// Tag2
        /// </summary>
        public const string MoreLinkModel = "MoreLinkModel";
        /// <summary>
        /// PopW
        /// </summary>
        public const string PopW = "PopW";
        public const string PopH = "PopH";

        /// <summary>
        /// 是否可删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 控制方式
        /// </summary>
        public const string CtrlWay = "CtrlWay";
        /// <summary>
        /// 打开方式
        /// </summary>
        public const string OpenWay = "OpenWay";
        /// <summary>
        /// 更多标签
        /// </summary>
        public const string MoreLab = "MoreLab";
        /// <summary>
        /// MoreUrl
        /// </summary>
        public const string MoreUrl = "MoreUrl";
        /// <summary>
        /// 产生时间
        /// </summary>
        public const string DocGenerRDT = "DocGenerRDT";
        /// <summary>
        /// 是否独占一行
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 权限控制方式.
        /// </summary>
        public const string WindCtrlWay = "WindCtrlWay";
    }
    /// <summary>
    /// 信息块
    /// </summary>
    public class WindowTemplate : EntityNoName
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
                return uac;
            }
        }
        #endregion 权限控制.


        #region 属性
        /// <summary>
        /// 文件内容
        /// </summary>
        public WinDocType WinDocType
        {
            get
            {
                return (WinDocType)this.GetValIntByKey(WindowTemplateAttr.WinDocType);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.WinDocType, (int)value);
            }
        }
        /// <summary>
        /// 更多的URL
        /// </summary>
        public string MoreUrl
        {
            get
            {
                return this.GetValStrByKey(WindowTemplateAttr.MoreUrl);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.MoreUrl, value);
            }
        }
        /// <summary>
        /// 更多标签
        /// </summary>
        public string MoreLab
        {
            get
            {
                return this.GetValStrByKey(WindowTemplateAttr.MoreLab);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.MoreLab, value);
            }
        }
        public int PopW
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.PopW);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.PopW, value);
            }
        }
        public int PopH
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.PopH);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.PopH, value);
            }
        }
        public int ColSpan
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.ColSpan);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.ColSpan, value);
            }
        }
        public int MoreLinkModel
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.MoreLinkModel);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.MoreLinkModel, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>

        /// <summary>
        /// 用户是否可以删除
        /// </summary>
        public bool IsDel
        {
            get
            {
                return this.GetValBooleanByKey(WindowTemplateAttr.IsDel);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.IsDel, value);
            }
        }
        /// <summary>
        /// 是否禁用?
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(WindowTemplateAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.IsEnable, value);
            }
        }

        /// <summary>
        /// 打开方式
        /// </summary>
        public int OpenWay
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.OpenWay);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.OpenWay, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(WindowTemplateAttr.Idx);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.Idx, value);
            }
        }
        public string DocGenerRDT
        {
            get
            {
                return this.GetValStrByKey(WindowTemplateAttr.DocGenerRDT);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.DocGenerRDT, value);
            }
        }

        public string Docs
        {
            get
            {
                return this.GetValStrByKey(WindowTemplateAttr.Docs);
            }
            set
            {
                this.SetValByKey(WindowTemplateAttr.Docs, value);
            }
        }
        //public string Doc
        //{
        //    get
        //    {
        //        string html = "";
        //        switch (this.WindowTemplateType)
        //        {
        //            case 0:
        //                html += "\t\n<ul style='padding-left:13px;width:200px;overflow:hidden;'>";

        //                string sql = this.Tag1;
        //                sql = sql.Replace("~", "'");
        //                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
        //                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
        //                DataTable dt = DBAccess.RunSQLReturnTable(sql);
        //                foreach (DataRow dr in dt.Rows)
        //                {
        //                    string no = dr["No"].ToString();
        //                    string name = dr["Name"].ToString();
        //                    string url = this.Tag2.Clone().ToString();
        //                    //url = url.Replace("@No", no);
        //                    url = url.Replace("~", "'");
        //                    //if (url.Contains("@"))
        //                    //{
        //                    //   foreach (DataColumn dc in dt.Columns)
        //                    //        url = url.Replace("@" + dc.ColumnName, dr[dc.ColumnName].ToString());
        //                    //}
        //                    url = this.GetParameteredString(url, dr);

        //                    if (url.ToLower().StartsWith("javascript:"))
        //                    {
        //                        html += "\t\n<li><a href=\"" + url + "\">" + name + "</a></li>";
        //                    }
        //                    else
        //                    {
        //                        switch (this.OpenWay)
        //                        {
        //                            case 0: //新窗口
        //                                html += "\t\n<li><a href=\"" + url + "\"  target='_blank' >" + name + "</a></li>";
        //                                break;
        //                            case 1: // 本窗口
        //                                html += "\t\n<li><a href=\"" + url + "\" target='_self' >" + name + "</a></li>";
        //                                break;
        //                            case 2: //覆盖新窗口 
        //                                html += "\t\n<li><a href=\"" + url + "\" target='" + this.No + "' >" + name + "</a></li>";
        //                                break;
        //                            default:
        //                                break;
        //                        }
        //                    }
        //                }
        //                html += "\t\n</ul>";
        //                return html;
        //            case 1:
        //                return this.Tag1;
        //            default:
        //                break;
        //        }
        //        return this.GetValStrByKey(WindowTemplateAttr.Doc);
        //    }
        //}
        /// <summary>
        /// 获取参数化的字符串
        /// </summary>
        /// <param name="stringInput"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string GetParameteredString(string stringInput, DataRow dr)
        {
            String regE = "@[a-zA-Z]([\\w-]*[a-zA-Z0-9])?"; //字母开始，字母+数字结尾，字母+数字+下划线+中划线中间
            //String regE = "@[\\w-]+";                               //字母+数字+下划线+中划线
            MatchCollection mc = Regex.Matches(stringInput, regE, RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                string v = m.Value;
                string f = m.Value.Substring(1);
                stringInput = stringInput.Replace(v, String.Format("{0}", dr[f]));
            }
            return stringInput;
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 信息块
        /// </summary>
        public WindowTemplate()
        {
        }
        /// <summary>
        /// 信息块
        /// </summary>
        /// <param name="mypk"></param>
        public WindowTemplate(string no)
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
                Map map = new Map("GPM_WindowTemplate");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "信息块";
                map.EnType = EnType.Sys;

                #region 基本信息.
                map.AddTBStringPK(WindowTemplateAttr.No, null, "编号", true, true, 1, 40, 200);
                map.AddTBInt(WindowTemplateAttr.ColSpan, 1, "占的列数", true, false);
                map.SetHelperAlert(WindowTemplateAttr.ColSpan, "画布按照4列划分布局，输入的输在在1=4之间.");
                map.AddTBString(WindowTemplateAttr.Name, null, "标题", true, false, 0, 300, 20, true);

                map.AddTBString(WindowTemplateAttr.Icon, null, "Icon", true, false, 0, 100, 20, true);
                #endregion 基本信息.

                // map.AddDDLSysEnum(WindowTemplateAttr.ColSpan, 1, "占的列数", true, true, WindowTemplateAttr.ColSpan,
                //  "@1=1列@2=2列@3=覆盖新窗口");
                #region 更多的信息定义.
                map.AddTBString(WindowTemplateAttr.MoreLab, null, "更多标签", true, false, 0, 300, 20, true);
                map.AddTBString(WindowTemplateAttr.MoreUrl, null, "更多链接", true, false, 0, 300, 20, true);
                map.AddDDLSysEnum(WindowTemplateAttr.MoreLinkModel, 0, "打开方式", true, true, WindowTemplateAttr.MoreLinkModel,
              "@0=新窗口@1=本窗口@2=覆盖新窗口");
                map.AddTBInt(WindowTemplateAttr.PopW, 500, "Pop宽度", false, true);
                map.AddTBInt(WindowTemplateAttr.PopH, 400, "Pop高度", false, true);
                #endregion 更多的信息定义.


                #region 内容定义.
                // 0=Html,   1=SQL列表, 2=折线图, 3=柱状图, 4=饼图.
                // map.AddTBInt(WindowTemplateAttr.WinDocType, 0, "内容类型", true, true);

                map.AddDDLSysEnum(WindowTemplateAttr.WinDocType, 0, "内容类型", true, true, WindowTemplateAttr.WinDocType,
             "@0=Html@1=系统内置@2=SQL列表@3=折线图@4=柱状图@5=饼图");

                map.AddTBStringDoc(WindowTemplateAttr.Docs, null, "内容表达式", true, false);
                #endregion 内容定义.

                #region 权限定义.
                // 0=Html , 1=SQL列表
                //  map.AddTBInt(WindowTemplateAttr.WinDocType, 0, "内容类型", false, true);
                // map.AddTBString(WindowTemplateAttr.Docs, null, "内容", true, false, 0, 4000, 20);
                #endregion 权限定义.


                #region 其他
                map.AddTBInt(WindowTemplateAttr.Idx, 0, "默认的排序", true, false);
                map.AddBoolean(WindowTemplateAttr.IsDel, true, "用户是否可删除", true, true);
                map.AddBoolean(WindowTemplateAttr.IsEnable, false, "是否禁用?", true, true);
                map.AddTBString(WindowTemplateAttr.OrgNo, null, "OrgNo", false, false, 0, 50, 20);
                map.AddDDLSysEnum(WindowTemplateAttr.WindCtrlWay, 0, "控制方式", true, true, WindowTemplateAttr.WindCtrlWay,
                   "@0=任何人都可以使用@1=按照设置的控制@2=Admin用户可以使用");
                #endregion 其他




                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            return base.beforeInsert();
        }

        protected override void afterDelete()
        {
            string sql = "DELETE FROM GPM_Window WHERE FK_WindowTemplate='" + this.No + "'";
            DBAccess.RunSQL(sql);
            base.afterDelete();
        }
    }
    /// <summary>
    /// 信息块s
    /// </summary>
    public class WindowTemplates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 信息块s
        /// </summary>
        public WindowTemplates()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WindowTemplate();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<WindowTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<WindowTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<WindowTemplate> Tolist()
        {
            System.Collections.Generic.List<WindowTemplate> list = new System.Collections.Generic.List<WindowTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((WindowTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        public void InitDocs()
        {
            //处理内容.
            foreach (WindowTemplate item in this)
            {
                //文本的, 不用转化.
                if (item.WinDocType == 0)
                    continue;

                //内置的.
                if (item.WinDocType == WinDocType.System)
                {
                    string exp = item.Docs.Clone() as string;
                    exp = BP.WF.Glo.DealExp(exp, null);
                    item.Docs = exp;
                }

                //SQL列表. 
                if (item.WinDocType == WinDocType.SQLList
                    || item.WinDocType == WinDocType.ChatZhuZhuang //sql列表.
                    || item.WinDocType == WinDocType.ChatZheXian
                    || item.WinDocType == WinDocType.ChatPie)
                {
                    try
                    {
                        string sql = item.Docs.Clone() as string;
                       // sql = sql.Replace("~", "'");
                        sql = BP.WF.Glo.DealExp(sql, null);
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        item.Docs = BP.Tools.Json.ToJson(dt);
                    }
                    catch (Exception ex)
                    {
                        item.WinDocType = WinDocType.Html;
                        item.Docs = "err@" + ex.Message + " SQL=" + item.Docs;
                    }

                }
            }
        }

        public override int RetrieveAll()
        {
            int i = this.RetrieveAllFromDBSource("Idx");
            if (i >= 1)
            {
                InitDocs();
                return i;
            }

            //初始化模板数据.
            InitData();

            //查询模数据.
            i = this.RetrieveAllFromDBSource("Idx");
            InitDocs();
            return i;
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            WindowTemplate en = new WindowTemplate();
            

            #region 关于我们.
            en.No = "001";
            en.WinDocType = WinDocType.Html;
            en.Name = "关于我们";
            string html = "";
            html += "<ul>";
            html += "<li>ccbpm是一个100%的开源软件,包含工作流程引擎、表单引擎、组织结构管理、菜单管理等敏捷开发的基础模块。</li>";
            html += "<li>该开源软件由驰骋公司从2003年开始研发到至今，经过多个版本迭代，并历经数千个项目于用户需求场景完成。</li>";
            html += "<li>设计严谨、考究抽象程度高、覆盖大部分客户应用需求，属于一款不可多得的应用国产的敏捷开发工具。</li>";
            html += "<li>源代码都发布在giee上，采用GPL开源协议进行开源，遵守GPL开源协议使用ccbpm合法有效。</li>";
            html += "<li>驰骋公司对外提供现场培训、技术支持、协助集成、协助项目落地服务，对小微企业，小企业，中等企业，大企业收费8,12,18,23三个等级的付费。</li>";
            html += "</ul>";
            en.Docs = html;
            en.MoreLinkModel = 1;
            en.ColSpan = 2;
            en.IsDel = true;
            en.Insert();
            #endregion 关于我们.

            #region 登录信息.
            en = new WindowTemplate();
            en.No = "002";
            en.Name = "登录信息";
            en.WinDocType = WinDocType.System;  //系统内置的.

            html = "<table>";
            html += "<tr>";
            html += " <td>帐号</td>";
            html += " <td>@WebUser.No</td>";
            html += "</tr>";

            html += "<tr>";
            html += " <td>姓名</td>";
            html += " <td>@WebUser.Name</td>";
            html += "</tr>";

            html += "<tr>";
            html += " <td>部门</td>";
            html += " <td>@WebUser.FK_DeptName</td>";
            html += "</tr>";
            en.Docs = html;
            en.ColSpan = 1;
            en.Insert();
            #endregion 登录信息.

            #region 我的待办.
            en = new WindowTemplate();
            en.No = "003";
            en.Name = "我的待办";
            en.WinDocType = WinDocType.ChatZhuZhuang; //柱状图.

            html = "SELECT FK_NodeText AS FlowName, COUNT(WorkID) as Num ";
            html += " FROM WF_GenerWorkerlist WHERE FK_Emp = '@WebUser.No' AND IsPass=0 GROUP BY FK_NodeText ";
            en.Docs = html;
            en.MoreLinkModel = 1;
            en.ColSpan = 4;
            en.Insert();
            #endregion 我的待办分布.

            #region 全部流程.
            en = new WindowTemplate();
            en.No = "004";
            en.Name = "全部流程";
            en.WinDocType = WinDocType.ChatZhuZhuang; //柱状图.

            if (Sys.SystemConfig.CCBPMRunModel == Sys.CCBPMRunModel.Single)
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 GROUP BY FlowName";
            else
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 AND OrgNo='@WebUser.OrgNo' GROUP BY FlowName";

            en.MoreLinkModel = 1;
            en.ColSpan = 2;
            en.Insert();
            #endregion 我的待办分布.

            #region 我的未完成.
            en = new WindowTemplate();
            en.WinDocType = WinDocType.ChatZhuZhuang;  //.
            en.No = "005";
            en.Name = "未完成";
            html = "SELECT FlowName, COUNT(WorkID) AS Num FROM WF_GenerWorkFlow  WHERE WFState = 2 ";
            html += "and Emps like '%@WebUser.No%' GROUP BY FlowName";
            en.Docs = html; 
            en.MoreLinkModel = 1;
            en.ColSpan = 4;
            en.Insert();
            #endregion 我的未完成.


            #region 我的发起.
            en = new WindowTemplate();
            en.No = "006";
            en.Name = "我的发起";
            en.WinDocType = WinDocType.ChatPie; //柱状图.

            if (Sys.SystemConfig.CCBPMRunModel == Sys.CCBPMRunModel.Single)
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 AND Starter='@WebUser.No'  GROUP BY FlowName";
            else
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 AND Starter='@WebUser.No' AND OrgNo='@WebUser.OrgNo' GROUP BY FlowName";

            en.MoreLinkModel = 1;
            en.ColSpan = 1;
            en.Insert();
            #endregion 我的发起.


            #region 我参与的.
            en = new WindowTemplate();
            en.No = "007";
            en.Name = "我参与的";
            en.WinDocType = WinDocType.ChatPie; //柱状图.

            if (Sys.SystemConfig.CCBPMRunModel == Sys.CCBPMRunModel.Single)
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 AND Emps LIKE  '%@WebUser.No,%'  GROUP BY FlowName";
            else
                en.Docs = "SELECT FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 AND Emps LIKE '%@WebUser.No,%' AND OrgNo='@WebUser.OrgNo' GROUP BY FlowName";

            en.MoreLinkModel = 1;
            en.ColSpan = 2;
            en.Insert();
            #endregion 我的发起.

            #region 流程实例月份柱状图.
            en = new WindowTemplate();
            en.No = "008";
            en.Name = "月统计发起";
            en.WinDocType = WinDocType.ChatZhuZhuang;

            html = "SELECT FK_NY AS FlowName, COUNT(WorkID) AS Num  FROM WF_GenerWorkFlow WHERE WFState !=0 GROUP BY FK_NY";
            en.Docs = html;
            en.MoreLinkModel = 1;
            en.ColSpan = 4;
            en.Insert();
            #endregion 流程实例月份柱状图.

         


        }
    }
}
