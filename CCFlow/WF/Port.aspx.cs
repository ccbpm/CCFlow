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
    /// Port ��ժҪ˵����
    /// </summary>
    public partial class Port : System.Web.UI.Page
    {
        #region ���봫�ݲ���
        /// <summary>
        /// ִ�е�����
        /// </summary>
        public string DoWhat
        {
            get
            {
                return this.Request.QueryString["DoWhat"];
            }
        }
        /// <summary>
        /// ��ǰ���û�
        /// </summary>
        public string UserNo
        {
            get
            {
                return this.Request.QueryString["UserNo"];
            }
        }
        /// <summary>
        /// �û��İ�ȫУ����(��ο������½�)
        /// </summary>
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        #endregion

        #region  ��ѡ��Ĳ���
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

            #region ��ȫ��У��.
            if (this.UserNo == null || this.SID == null || this.DoWhat == null)
            {
                this.ToErrorPage("@��Ҫ�Ĳ���û�д��룬��ο��ӿڹ���");
                return;
            }

            if (BP.WF.Dev2Interface.Port_CheckUserLogin(this.UserNo, this.SID) == false)
            {
                this.Response.Write("�Ƿ��ķ��ʣ��������Ա��ϵ��SID=" + this.SID);
                return;
            }

            if (BP.Web.WebUser.No != this.UserNo)
            {
                BP.WF.Dev2Interface.Port_SigOut();
                BP.WF.Dev2Interface.Port_Login(this.UserNo, true);
            }
            if (this.Request.QueryString["IsMobile"] == "1")
                BP.Web.WebUser.UserWorkDev = UserWorkDev.Mobile;
            else
                BP.Web.WebUser.UserWorkDev = UserWorkDev.PC;
            #endregion ��ȫ��У��.

            #region ���ɲ�����.
            string paras = "";
            foreach (string str in this.Request.QueryString)
            {
                string val = this.Request.QueryString[str];
                if (val.IndexOf('@') != -1)
                    throw new Exception("��û���ܲ���: [ " + str + " ," + val + " ] ��ֵ ��URL �����ܱ�ִ�С�");

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
            #endregion ���ɲ�����.



            string nodeID = int.Parse(this.FK_Flow + "01").ToString();
            switch (this.DoWhat)
            {
                case DoWhatList.OneWork: // ��������������.
                    if (this.FK_Flow == null || this.WorkID == null)
                        throw new Exception("@���� FK_Flow ���� WorkID Ϊ Null ��");
                    this.Response.Redirect(this.AppPath + "WF/WFRpt.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                case DoWhatList.StartSimple: // ����ģʽ�ķ�ʽ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "WF/App/Simple/Default.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/App/Simple/Default.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start5: // ������
                case "StartClassic": // ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "WF/App/Classic/Default.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/App/Classic/Default.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.StartLigerUI:
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "WF/App/EasyUI/Default.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/App/EasyUI/Default.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start: // ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect("Start.aspx", true);
                    else
                        this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Runing: // ��;�й���
                    this.Response.Redirect("Runing.aspx?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Tools: // ������Ŀ��
                    this.Response.Redirect("Tools.aspx", true);
                    break;
                case DoWhatList.EmpWorks: // �ҵĹ���С����.
                    if (this.FK_Flow == null || this.FK_Flow == "")
                        this.Response.Redirect("EmpWorks.aspx", true);
                    else
                        this.Response.Redirect("EmpWorks.aspx?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Login:
                    if (this.FK_Flow == null)
                        this.Response.Redirect("EmpWorks.aspx", true);
                    else
                        this.Response.Redirect("EmpWorks.aspx?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Emps: // ͨѶ¼��
                    this.Response.Redirect("Emps.aspx", true);
                    break;
                case DoWhatList.FlowSearch: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("FlowSearch.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/Rpt/Search.aspx?Endse=s&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmall: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("FlowSearch.aspx", true);
                    else
                        this.Response.Redirect("./Comm/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmallSingle: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("FlowSearchSmallSingle.aspx", true);
                    else
                        this.Response.Redirect("./Comm/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowFX: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        throw new Exception("@û�в������̱�š�");

                    this.Response.Redirect("./Comm/Group.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.DealWork:
                    if (this.FK_Flow == null || this.WorkID == null)
                    {
                        this.ToErrorPage("@���� FK_Flow ���� WorkID ΪNull ��");
                        return;
                    }
                    this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                default:
                    this.ToErrorPage("û��Լ���ı��:DoWhat=" + this.DoWhat);
                    break;
            }
        }
        public void ShowMsg(string msg)
        {
            this.Response.Write(msg);
        }
        /// <summary>
        /// ��֤��½�û��Ƿ�Ϸ�
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

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
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
        /// �л�����ϢҲ�档
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
