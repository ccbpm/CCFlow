using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.Web.Port
{
    /// <summary>
    /// Port 的摘要说明。
    /// </summary>
    public partial class Port : System.Web.UI.Page
    {
        #region 必须传递参数
        /// <summary>
        /// 执行的内容
        /// </summary>
        public string DoWhat
        {
            get
            {
                return this.Request.QueryString["DoWhat"];
            }
        }
        /// <summary>
        /// 当前的用户
        /// </summary>
        public string UserNo
        {
            get
            {
                string str = this.Request.QueryString["UserNo"];
                return HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
            }
        }
        /// <summary>
        /// 用户的安全校验码(请参考集成章节)
        /// </summary>
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        #endregion

        #region  可选择的参数
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        public string AppPath
        {
            get
            {
                return BP.WF.Glo.CCFlowAppPath;
            }
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");

            #region 安全性校验.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null)
            {
                this.ToErrorPage("@必要的参数没有传入，请参考接口规则。");
                return;
            }

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
            {
                this.Response.Write("非法的访问，请与管理员联系。SID=" + this.SID);
                return;
            }

            if (BP.Web.WebUser.No != this.UserNo)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                BP.WF.Dev2Interface.Port_Login(this.UserNo);
            }
            if (this.Request.QueryString["IsMobile"] == "1")
                BP.Web.WebUser.UserWorkDev = UserWorkDev.Mobile;
            else
                BP.Web.WebUser.UserWorkDev = UserWorkDev.PC;
            #endregion 安全性校验.

            #region 生成参数串.
            string paras = "";
            foreach (string str in this.Request.QueryString)
            {
                string val = this.Request.QueryString[str];
                if (val.IndexOf('@') != -1)
                    throw new Exception("您没有能参数: [ " + str + " ," + val + " ] 给值 ，URL 将不能被执行。");

                switch (str)
                {
                    case DoWhatList.DoNode:
                    case DoWhatList.Emps:
                    case DoWhatList.EmpWorks:
                    case DoWhatList.FlowSearch:
                    case DoWhatList.Login:
                    case DoWhatList.MyFlow:
                    case DoWhatList.MyWork:
                    case DoWhatList.Start:
                    case DoWhatList.Start5:
                    case DoWhatList.StartSimple:
                    case DoWhatList.FlowFX:
                    case DoWhatList.DealWork:
                    case "FK_Flow":
                    case "WorkID":
                    case "FK_Node":
                    case "SID":
                        break;
                    default:
                        paras += "&" + str + "=" + val;
                        break;
                }
            }
            #endregion 生成参数串.

            string nodeID = int.Parse(this.FK_Flow + "01").ToString();
            switch (this.DoWhat)
            {
                case DoWhatList.OneWork: // 工作处理器调用.
                    if (this.FK_Flow == null || this.WorkID == null)
                        throw new Exception("@参数 FK_Flow 或者 WorkID 为 Null 。");
                    this.Response.Redirect(this.AppPath + "WF/WFRpt.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                case DoWhatList.StartSimple: // 极速模式的方式发起工作
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "AppSimple/Default.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "AppSimple/Default.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start5: // 发起工作
                case "StartClassic": // 发起工作
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "WF/AppClassic/Home.htm", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/AppClassic/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case "ACE": // 发起工作
                    if (this.FK_Flow == null)
                        this.Response.Redirect("../AppACE/Login.htm", true);
                    else
                        this.Response.Redirect("../AppACE/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start: // 发起工作
                    if (this.FK_Flow == null)
                        this.Response.Redirect("Start.htm", true);
                    else
                        this.Response.Redirect("MyFlow.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Runing: // 在途中工作
                    this.Response.Redirect("Runing.htm?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Tools: // 工具栏目。
                    this.Response.Redirect("Tools.htm", true);
                    break;
                case DoWhatList.EmpWorks: // 我的工作小窗口.
                    if (this.FK_Flow == null || this.FK_Flow == "")
                        this.Response.Redirect("Todolist.htm", true);
                    else
                        this.Response.Redirect("Todolist.htm?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Login:
                    if (this.FK_Flow == null)
                        this.Response.Redirect("Todolist.htm", true);
                    else
                        this.Response.Redirect("Todolist.htm?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Emps: // 通讯录。
                    this.Response.Redirect("Emps.aspx", true);
                    break;
                case DoWhatList.FlowSearch: // 流程查询。
                    if (this.FK_Flow == null)
                        this.Response.Redirect("./RptSearch/Default.htm", true);
                    else
                        this.Response.Redirect("./RptDfine/FlowSearch.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmall: // 流程查询。
                    if (this.FK_Flow == null)
                        this.Response.Redirect("./RptSearch/Default.htm", true);
                    else
                        this.Response.Redirect("./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmallSingle: // 流程查询。
                    if (this.FK_Flow == null)
                        this.Response.Redirect("FlowSearchSmallSingle.aspx", true);
                    else
                        this.Response.Redirect("./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowFX: // 流程查询。
                    if (this.FK_Flow == null)
                        throw new Exception("@没有参数流程编号。");

                    this.Response.Redirect("./Comm/Group.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.DealWork:
                    if (this.FK_Flow == null || this.WorkID == null)
                    {
                        this.ToErrorPage("@参数 FK_Flow 或者 WorkID 为Null 。");
                        return;
                    }
                    this.Response.Redirect("MyFlow.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                case DoWhatList.DealMsg: //处理消息的连接，比如待办、退回、移交的消息处理.
                    string guid = this.Request.QueryString["GUID"];

                    BP.WF.SMS sms = new SMS();
                    sms.MyPK = guid;
                    sms.Retrieve();

                    //判断当前的登录人员.
                    if (BP.Web.WebUser.No != sms.SendToEmpNo)
                        BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);

                    BP.DA.AtPara ap = new AtPara(sms.AtPara);
                    switch (sms.MsgType)
                    {
                        case SMSMsgType.SendSuccess: // 发送成功的提示.

                            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValStrByKey("FK_Flow"),
                                ap.GetValIntByKey("FK_Node"), ap.GetValInt64ByKey("WorkID"), WebUser.No) == true)
                                this.Response.Redirect("MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            else
                                this.Response.Redirect("WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            return;
                        default: //其他的情况都是查看工作报告.
                            this.Response.Redirect("WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            return;
                    }
                    //this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    return ;
                default:
                    this.ToErrorPage("没有约定的标记:DoWhat=" + this.DoWhat);
                    break;
            }
        }
        public void ShowMsg(string msg)
        {
            this.Response.Write(msg);
        }
        /// <summary>
        /// 验证登陆用户是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCanLogin()
        {
            if (BP.Sys.SystemConfig.AppSettings["IsAuth"] == "1")
            {
                if (this.SID != this.GetSID())
                {
                    if (SystemConfig.IsDebug)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        }
        public string GetSID()
        {
            return BP.DA.DBAccess.RunSQLReturnString("SELECT SID From Port_Emp WHERE No='" + this.UserNo + "'");
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        public void ToMsgPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.ApplicationPath + "Port/InfoPage.aspx", true);
            return;
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        public void ToErrorPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(this.AppPath + "WF/Comm/Port/InfoPage.aspx");
            return;
        }
    }
}
