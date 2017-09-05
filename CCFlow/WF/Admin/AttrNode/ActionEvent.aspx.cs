using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.WF.XML;
using System.Reflection;

namespace CCFlow.WF.Admin
{
    public partial class ActionEvent : BP.Web.WebPage
    {
        #region 属性
        public string EventDoType
        {
            get
            {
                return this.Request.QueryString["EventDoType"];
            }
        }
        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }

        public string NodeID
        {
            get
            {
                return this.Request.QueryString["NodeID"];
            }
        }

        public string FK_MapData
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["FK_MapData"]))
                    return this.Request.QueryString["FK_MapData"];

                return "ND" + this.Request.QueryString["NodeID"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
       
        #endregion

        #region 通用方法.
        /// <summary>
        /// 初始化类
        /// </summary>
        /// <param name="ddl"></param>
        /// <param name="aa"></param>
        /// <param name="selectVal"></param>
        public Type InitClassName(DDL ddl, Type[] types, string selectVal)
        {
            Type mytype = null;
            foreach (Type tp in types)
            {
                if (tp.IsClass == false)
                    continue;
                if (tp.IsCOMObject == true)
                    continue;
                if (tp.IsEnum == true)
                    continue;
                if (mytype == null)
                    mytype = tp;
                if (tp.Name.Contains("<") || tp.Name.Contains("{"))
                    continue;
                ddl.Items.Add(new ListItem(tp.FullName, tp.FullName));
                if (tp.FullName == selectVal)
                    mytype = tp;
            }
            //返回.
            return mytype;
        }
        /// <summary>
        /// 生成参数提示
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string GenerParas(ParameterInfo[] paras)
        {
            string strs = "";
            if (paras != null)
            {
                foreach (ParameterInfo item in paras)
                {
                    strs += "" + item.Name + " =val(类型:" + item.ParameterType.ToString() + ");";
                }
            }
            return "格式为: <font color=blue>" + strs + "</font>参数可以使用ccbpm的表达式。<span style='font-weight:bold'>注意：ccbpm的表达式格式如：`Title`，属性两端使用`号，键盘上“Esc键”下方的键，英文状态。</span>";
        }

        public ParameterInfo[] InitMethodName(DDL ddl, MethodInfo[] mis, string selectVal)
        {
            ParameterInfo[] myPI = null;
            foreach (MethodInfo mi in mis)
            {
                if (mi.IsPublic == false)
                    continue;

                if (mi.IsVirtual == true)
                    continue;

                if (mi.MemberType != MemberTypes.Method)
                    continue;

                if (mi.Name.Contains("set_") || mi.Name.Contains("get_"))
                    continue;

                string methodString = GenerateMethodString(mi);

                if ((string.IsNullOrWhiteSpace(selectVal) && myPI == null) || methodString == selectVal)
                    myPI = mi.GetParameters();

                ddl.Items.Add(new ListItem(methodString, methodString));
            }
            return myPI;
        }

        /// <summary>
        /// 获取方法的描述字符串，格式如：Set(String name,Int32 age)
        /// </summary>
        /// <param name="method">方法对象</param>
        /// <returns></returns>
        private string GenerateMethodString(MethodInfo method)
        {
            string desc = "";
            ParameterInfo[] ps = method.GetParameters();

            foreach (ParameterInfo item in ps)
            {
                desc += item.ParameterType.ToString()
                            .Replace("System.IO.", "")
                            .Replace("System.", "")
                            .Replace("System.Collections.Generic.", "")
                            .Replace("System.Collections.", "") + " " + item.Name + ",";
            }

            return string.Format("{0}({1})", method.Name, desc.Length > 0 ? desc.Substring(0, desc.Length - 1) : "");
        }
        #endregion 通用方法.


        protected void Page_Load(object sender, EventArgs e)
        {
            FrmEvent mynde = new FrmEvent();
            mynde.MyPK = (this.ShowType == "Flow" ? this.FK_Flow : this.FK_MapData) + "_" + this.Event;

            if (mynde.RetrieveFromDBSources() == 0)
            {
                mynde.MyPK = string.Empty;
                mynde.FK_Event = this.Event;
            }

            if (!string.IsNullOrWhiteSpace(this.EventDoType))
                mynde.HisDoType = (EventDoType)Enum.Parse(typeof(EventDoType), this.EventDoType);

            this.Pub1.AddTable("class='Table' cellspacing='1' cellpadding='1' border='1' style='width:100%'");

            //删除旧类型.
            BP.DA.DBAccess.RunSQL("delete from sys_enum where enumkey='EventDoType'");

            this.Pub1.AddTR();
            this.Pub1.AddTD("width='200'", "内容类型:");
            DDL ddl = new DDL();
            ddl.BindSysEnum("EventDoType");
            ddl.ID = "DDL_EventDoType";
            ddl.SetSelectItem((int)mynde.HisDoType);
            ddl.Attributes["onchange"] = string.Format("location.href='ActionEvent.aspx?NodeID={0}&MyPK={1}&Event={2}&FK_MapData={3}&EventDoType=' + this.options[this.selectedIndex].value + '&ShowType={4}&FK_Flow={5}&tk=' + Math.random()", this.NodeID, this.MyPK, this.Event, this.FK_MapData, this.ShowType, this.FK_Flow);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTREnd();

            TextBox tb = null;

            #region //执行DLL类方法设置， edited by liuxc,2016-01-17
            if (mynde.HisDoType == BP.Sys.EventDoType.SpecClass)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD("选择一个DLL文件:");
                //绑定 DLL 文件. 
                ddl = new DDL();
                ddl.ID = "DDL_MonthedDLL";
                string[] fs = System.IO.Directory.GetFiles(BP.Sys.SystemConfig.PathOfWebApp + "\\Bin\\", "*.dll");
                foreach (string str in fs)
                {
                    string mystr = str.Replace(BP.Sys.SystemConfig.PathOfWebApp + "\\Bin\\", "");
                    switch (mystr)
                    {
                        case "BitmapCutter.Core.dll":
                        case "BP.Demo.dll":
                        //   case "BP.En30.dll":
                        case "BP.GPM.dll":
                        case "BP.GPMClient.dll":
                        case "BP.Web.Controls.dll":
                        //  case "BP.WF.dll":
                        case "CCFlow.dll":
                        case "ChineseConverter.dll":
                        case "FtpSupport.dll":
                        case "FusionCharts.dll":
                        case "IBM.Data.Informix.dll":
                        case "ICSharpCode.SharpZipLib.dll":
                        case "Interop.Excel.dll":
                        case "Interop.VBIDE.dll":
                        case "Microsoft.Expression.Interactions.dll":
                        case "Microsoft.Web.UI.WebControls.dll":
                        case "Newtonsoft.Json.dll":
                        case "NPOI.dll":
                        case "NPOI.OOXML.dll":
                        case "NPOI.OpenXml4Net.dll":
                        case "NPOI.OpenXmlFormats.dll":
                        case "office.dll":
                        case "Silverlight.DataSetConnector.dll":
                        case "System.Windows.Interactivity.dll":
                            continue;
                        default:
                            break;
                    }

                    ddl.Items.Add(new ListItem(mystr, str));
                    //ddl.Items.Add(new ListItem(str, str));
                }
                ddl.SetSelectItem(mynde.MonthedDLL);
                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTREnd();


                //获得类名.
                this.Pub1.AddTR();
                this.Pub1.AddTD("选择一个类名:");

                Assembly abl = Assembly.LoadFrom(ddl.SelectedItemStringVal); // 载入程序集 
                Type[] types = abl.GetTypes();
                ddl = new DDL();
                ddl.ID = "DDL_MonthedClass";

                //绑定类名称.
                Type mytype = this.InitClassName(ddl, types, mynde.MonthedClass);
                ddl.SetSelectItem(mynde.MonthedClass);
                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(ddl_Class_SelectedIndexChanged);
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTREnd();


                this.Pub1.AddTR();
                this.Pub1.AddTD("选择一个方法:");

                ddl = new DDL();
                ddl.ID = "DDL_MonthedName";
                //绑定方法.
                ParameterInfo[] myPI = this.InitMethodName(ddl, mytype.GetMethods(), mynde.MonthedName);
                ddl.SetSelectItem(mynde.MonthedName);
                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(ddl_Monthed_SelectedIndexChanged);
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTREnd();

                #region 参数与参数格式.

                this.Pub1.AddTR();
                this.Pub1.AddTD("执行方法的参数");
                //参数.
                tb = new TextBox();
                tb.ID = "TB_MonthedParas";
                tb.Text = mynde.MonthedParas.Replace("~", "@");
                tb.Columns = 80;
                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();


                this.Pub1.AddTR();
                this.Pub1.AddTD("参数格式:");

                Label lab = new Label();
                lab.ID = "Lab_Note";
                lab.Text = this.GenerParas(myPI);
                this.Pub1.AddTD(lab);
                this.Pub1.AddTREnd();

                #endregion 参数与参数格式.
            }
            #endregion

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin("colspan=2");
            this.Pub1.Add("&nbsp;要执行的内容<br>");
            tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Columns = 50;
            tb.Style.Add("width", "99%");
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Text = mynde.DoDoc;
            this.Pub1.Add(tb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin("colspan=2");
            tb = new TextBox();
            tb.ID = "TB_MsgOK";
            tb.Style.Add("width", "99%");
            tb.Text = mynde.MsgOKString;
            this.Pub1.Add("执行成功信息提示(可为空)<br>");
            this.Pub1.Add(tb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin("colspan=2");
            this.Pub1.Add("执行失败信息提示(可为空)<br>");
            tb = new TextBox();
            tb.ID = "TB_MsgErr";
            tb.Style.Add("width", "99%");
            tb.Text = mynde.MsgErrorString;
            this.Pub1.Add(tb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            Pub1.AddBR();
            Pub1.AddSpace(1);

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            Pub1.Add(btn);

            if (!string.IsNullOrWhiteSpace(mynde.MyPK))
            {
                Pub1.AddSpace(1);
                Pub1.Add(
                    string.Format(
                        "<a href='javascript:void(0)' onclick=\"DoDel('{2}','{0}','{1}','{3}')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-delete'\">删除</a>",
                        NodeID, Event, FK_Flow, ShowType));
            }
        }
        /// <summary>
        /// 方法名变化后，参数格式发省变化.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddl_Monthed_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获得选择的名称.
            string ddlName = this.Pub1.GetDDLByID("DDL_MonthedDLL").SelectedItemStringVal;

            //类名.
            string clsName = this.Pub1.GetDDLByID("DDL_MonthedClass").SelectedItemStringVal;

            //实例化该Type.
            Assembly abl = Assembly.LoadFrom(ddlName); // 载入程序集
            Type[] tps = abl.GetTypes();
            Type mytp = null;
            foreach (Type item in tps)
            {
                if (clsName.Contains(item.Name) == true)
                {
                    mytp = item;
                    break;
                }
            }

            //获得选得方法名.
            string methodString = this.Pub1.GetDDLByID("DDL_MonthedName").SelectedItemStringVal;

            //获得该方法下的参数集合.
            MethodInfo[] mis = mytp.GetMethods();
            ParameterInfo[] myPI = null;
            foreach (MethodInfo mi in mis)
            {
                if (mi.IsPublic == false)
                    continue;
                if (mi.MemberType != MemberTypes.Method)
                    continue;

                if (mi.Name.Contains("set_") || mi.Name.Contains("get_"))
                    continue;

                if (GenerateMethodString(mi) == methodString)
                {
                    myPI = mi.GetParameters();
                    break;
                }
            }

            //生成提示标签.
            this.Pub1.GetLabelByID("Lab_Note").Text = this.GenerParas(myPI);
        }
        /// <summary>
        /// 类名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddl_Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获得选择的名称.
            string ddlName = this.Pub1.GetDDLByID("DDL_MonthedDLL").SelectedItemStringVal;

            //类名.
            string clsName = this.Pub1.GetDDLByID("DDL_MonthedClass").SelectedItemStringVal;

            //实例化该Type.
            Assembly abl = Assembly.LoadFrom(ddlName); // 载入程序集
            Type[] tps = abl.GetTypes();
            Type mytp = null;
            foreach (Type item in tps)
            {
                if (clsName.Contains(item.Name) == true)
                {
                    mytp = item;
                    break;
                }
            }

            // 获得该类的方法名, 重新绑定方法.
            DDL ddl = this.Pub1.GetDDLByID("DDL_MonthedName");
            ddl.Items.Clear();
            //绑定方法.
            ParameterInfo[] myPI = this.InitMethodName(ddl, mytp.GetMethods(), null);

            //生成提示标签.
            this.Pub1.GetLabelByID("Lab_Note").Text = this.GenerParas(myPI);
        }
        /// <summary>
        /// dll文件.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获得选择的名称.
            string ddlName = this.Pub1.GetDDLByID("DDL_MonthedDLL").SelectedItemStringVal;

            //获得
            DDL ddl = this.Pub1.GetDDLByID("DDL_MonthedClass");
            ddl.Items.Clear();

            //获得方法.
            Assembly abl = Assembly.LoadFrom(ddlName); // 载入程序集 
            Type[] types = null;
            try
            {
                types = abl.GetTypes();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('载入类出现错误：" + ex.Message + "');history.back();</script>");
                return;
            }
            Type mytype = this.InitClassName(ddl, types, "");


            // 获得该类的方法名, 重新绑定方法.
            ddl = this.Pub1.GetDDLByID("DDL_MonthedName");
            ddl.Items.Clear();
            ParameterInfo[] myPI = InitMethodName(ddl, mytype.GetMethods(), ""); ;

            //生成提示标签.
            this.Pub1.GetLabelByID("Lab_Note").Text = this.GenerParas(myPI);
        }

        void btn_Click(object sender, EventArgs e)
        {
            string mypk = (this.ShowType == "Flow" ? this.FK_Flow : this.FK_MapData) + "_" + this.Event;
            FrmEvent fe = new FrmEvent();
            fe.MyPK = mypk;
            fe.RetrieveFromDBSources();

            string doc = this.Pub1.GetTextBoxByID("TB_Doc").Text.Trim();

            fe = (FrmEvent)this.Pub1.Copy(fe);
            fe.MyPK = mypk;
            fe.DoDoc = doc;
            fe.FK_Event = this.Event;
            fe.FK_MapData = this.ShowType == "Flow" ? this.FK_Flow : this.FK_MapData;
            fe.HisDoType = (EventDoType)this.Pub1.GetDDLByID("DDL_EventDoType").SelectedItemIntVal;
            fe.MsgOKString = this.Pub1.GetTextBoxByID("TB_MsgOK").Text;
            fe.MsgErrorString = this.Pub1.GetTextBoxByID("TB_MsgErr").Text;

            //DLL参数.
            if (fe.HisDoType == BP.Sys.EventDoType.SpecClass)
            {
                fe.MonthedDLL = this.Pub1.GetDDLByID("DDL_MonthedDLL").SelectedItemStringVal;
                fe.MonthedClass = this.Pub1.GetDDLByID("DDL_MonthedClass").SelectedItemStringVal;
                fe.MonthedName = this.Pub1.GetDDLByID("DDL_MonthedName").SelectedItemStringVal;
                fe.MonthedParas = this.Pub1.GetTextBoxByID("TB_MonthedParas").Text;
            }

            fe.Save();

            this.Response.Redirect("ActionEvent.aspx?FK_Flow=" + this.FK_Flow + "&NodeID=" + this.NodeID + "&MyPK=" + fe.MyPK + "&Event=" + this.Event + "&FK_MapData=" + this.FK_MapData + "&ShowType=" + this.ShowType + "&tk=" + new Random().NextDouble(), true);
        }
    }
}