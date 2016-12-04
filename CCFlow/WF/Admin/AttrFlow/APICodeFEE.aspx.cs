using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class APICodeFEE : System.Web.UI.Page
    {
        public string FK_Flow
        {
            get
            {
                return Request["FK_Flow"];
            }
        }

        public bool Download
        {
            get
            {
                return Request["Download"] == "1";
            }
        }

        public new string Title { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FK_Flow))
            {
                Pub1.AddEasyUiPanelInfo("错误", "FK_Flow参数不能为空！");
                return;
            }

            Flow flow = new Flow(FK_Flow);

            if (string.IsNullOrWhiteSpace(flow.No))
            {
                Pub1.AddEasyUiPanelInfo("错误", string.Format("FK_Flow参数不正确，未找到编号为{0}的流程！", FK_Flow));
                return;
            }

            string tmpPath = Server.MapPath(Glo.CCFlowAppPath) + @"\WF\Admin\AttrFlow\APICodeFEE.cs";

            if (!File.Exists(tmpPath))
            {
                Pub1.AddEasyUiPanelInfo("错误", string.Format(@"未找到事件编写模板文件“{0}”，请联系管理员！", tmpPath));
                return;
            }

            Title = flow.Name + "[" + flow.No + "]";
            string code = File.ReadAllText(tmpPath, System.Text.Encoding.UTF8).Replace("F001Templepte", string.Format("FEE{0}", flow.No)).Replace("@FlowName", flow.Name).Replace("@FlowNo", flow.No);

            if (Download)
            {
                Response.ClearHeaders();
                Response.Clear();
                Response.Expires = 0;
                Response.Buffer = true;
                Response.AddHeader("Content-Type", "text/html; charset=utf-8");
                Response.AddHeader("content-disposition", string.Format("attachment; filename=FEE{0}.cs", flow.No));
                Response.ContentType = "application/octet-stream";
                Response.Write(code);
                Response.End();
                return;
            }

            //此处将重要行标示出来，根据下面的数组中的项来检索重要行号
            string[] lineStrings = new[]
                                       {
                                           "namespace BP.FlowEvent",
                                           ": BP.WF.FlowEventBase",
                                           "public override string FlowMark",
                                           "public override string SendWhen()",
                                           "public override string SendSuccess()",
                                           "public override string SendError()",
                                           "public override string FlowOnCreateWorkID()",
                                           "public override string FlowOverBefore()",
                                           "public override string FlowOverAfter()",
                                           "public override string BeforeFlowDel()",
                                           "public override string AfterFlowDel()",
                                           "public override string SaveAfter()",
                                           "public override string SaveBefore()",
                                           "public override string UndoneBefore()",
                                           "public override string UndoneAfter()",
                                           "public override string ReturnBefore()",
                                           "public override string ReturnAfter()",
                                           "public override string AskerAfter()",
                                           "public override string AskerReAfter()"
                                       };


            //this.Pub1.AddFieldSet("使用帮助");
            //this.Pub1.AddUL();
            //this.Pub1.AddLi("ccbpm提供了可以让程序员编写代码与流程引擎，表单引擎进行交互，以处理复杂的业务逻辑。");
            //this.Pub1.AddLi("ccbpm预留一个基类 BP.WF.FlowEventBase ，只要从这个基类上集成下来的子类，按照约定的格式重写相关的方法属性，流程引擎就会把这些代码注册到流程引擎中，并在运动中执行。");
            //this.Pub1.AddLi("该功能提供了一个自动生成的代码模版，如果您有编程基础，就很容易明白如何通过该子类实现复杂的业务逻辑。");
            //this.Pub1.AddLi("下载下来该类后，您必须把他放入一个以BP.开头的类库里，ccflow才能被注册到引擎中去。");
            //this.Pub1.AddULEnd();
            //this.Pub1.AddFieldSetEnd();
            
            this.Pub1.AddLi(string.Format("<a href=\"APICodeFEE.aspx?FK_Flow={0}&Download=1\" target=\"_blank\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-save',plain:true\">下载代码</a><br />", FK_Flow));
            Pub1.Add(string.Format("<pre type=\"syntaxhighlighter\" class=\"brush: csharp; html-script: false; highlight: [{2}]\" title=\"{0}[编号：{1}] 流程自定义事件代码生成\">", flow.Name, flow.No, GetImportantLinesNumbers(lineStrings, code)));
            Pub1.Add(code.Replace("<", "&lt;"));    //SyntaxHighlighter中，使用<Pre>包含的代码要将左尖括号改成其转义形式
            Pub1.Add("</pre>");
            Pub1.Add("<script type=\"text/javascript\">SyntaxHighlighter.highlight();</script>");
            Pub1.Add(string.Format(
                "<a href=\"APICodeFEE.aspx?FK_Flow={0}&Download=1\" target=\"_blank\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-save',plain:true\">下载代码</a>  您需要把该代码整合到您的类库里，并且该类库必须以BP 开头命名。<br />", FK_Flow));
        }
        /// <summary>
        /// 获取重要行的标号连接字符串，如3,6,8
        /// </summary>
        /// <param name="lineInStrings">重要行中包含的字符串数组，只要行中包含其中的一项字符串，则这行就是重要行</param>
        /// <param name="str">要检索的字符串，使用Environment.NewLine分行</param>
        /// <returns></returns>
        private string GetImportantLinesNumbers(string[] lineInStrings, string str)
        {
            string[] lines = str.Replace(Environment.NewLine, "`").Split('`');
            string nums = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                foreach (string instr in lineInStrings)
                {
                    if (lines[i].IndexOf(instr) != -1)
                    {
                        nums += (i + 1) + ",";
                        break;
                    }
                }
            }

            return nums.TrimEnd(',');
        }
    }
}