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
                string str = this.Request.QueryString["UserNo"];
                return HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
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
                BP.WF.Dev2Interface.Port_Login(this.UserNo);
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
                    this.Response.Redirect(this.AppPath + "WF/WFRpt.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                case DoWhatList.StartSimple: // ����ģʽ�ķ�ʽ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "AppSimple/Default.aspx", true);
                    else
                        this.Response.Redirect(this.AppPath + "AppSimple/Default.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start5: // ������
                case "StartClassic": // ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect(this.AppPath + "WF/AppClassic/Home.htm", true);
                    else
                        this.Response.Redirect(this.AppPath + "WF/AppClassic/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case "ACE": // ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect("../AppACE/Login.htm", true);
                    else
                        this.Response.Redirect("../AppACE/Home.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Start: // ������
                    if (this.FK_Flow == null)
                        this.Response.Redirect("Start.htm", true);
                    else
                        this.Response.Redirect("MyFlow.htm?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + nodeID, true);
                    break;
                case DoWhatList.Runing: // ��;�й���
                    this.Response.Redirect("Runing.htm?FK_Flow=" + this.FK_Flow, true);
                    break;
                case DoWhatList.Tools: // ������Ŀ��
                    this.Response.Redirect("Tools.htm", true);
                    break;
                case DoWhatList.EmpWorks: // �ҵĹ���С����.
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
                case DoWhatList.Emps: // ͨѶ¼��
                    this.Response.Redirect("Emps.aspx", true);
                    break;
                case DoWhatList.FlowSearch: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("./RptSearch/Default.htm", true);
                    else
                        this.Response.Redirect("./RptDfine/FlowSearch.htm?2=1&FK_Flow=001&EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmall: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("./RptSearch/Default.htm", true);
                    else
                        this.Response.Redirect("./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowSearchSmallSingle: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        this.Response.Redirect("FlowSearchSmallSingle.aspx", true);
                    else
                        this.Response.Redirect("./Comm/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.FlowFX: // ���̲�ѯ��
                    if (this.FK_Flow == null)
                        throw new Exception("@û�в������̱�š�");

                    this.Response.Redirect("./Comm/Group.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt" + paras, true);
                    break;
                case DoWhatList.DealWork:
                    if (this.FK_Flow == null || this.WorkID == null)
                    {
                        this.ToErrorPage("@���� FK_Flow ���� WorkID ΪNull ��");
                        return;
                    }
                    this.Response.Redirect("MyFlow.htm?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    break;
                case DoWhatList.DealMsg: //������Ϣ�����ӣ�������졢�˻ء��ƽ�����Ϣ����.
                    string guid = this.Request.QueryString["GUID"];

                    BP.WF.SMS sms = new SMS();
                    sms.MyPK = guid;
                    sms.Retrieve();

                    //�жϵ�ǰ�ĵ�¼��Ա.
                    if (BP.Web.WebUser.No != sms.SendToEmpNo)
                        BP.WF.Dev2Interface.Port_Login(sms.SendToEmpNo);

                    BP.DA.AtPara ap = new AtPara(sms.AtPara);
                    switch (sms.MsgType)
                    {
                        case SMSMsgType.SendSuccess: // ���ͳɹ�����ʾ.

                            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(ap.GetValStrByKey("FK_Flow"),
                                ap.GetValIntByKey("FK_Node"), ap.GetValInt64ByKey("WorkID"), WebUser.No) == true)
                                this.Response.Redirect("MyFlow.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            else
                                this.Response.Redirect("WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            return;
                        default: //������������ǲ鿴��������.
                            this.Response.Redirect("WFRpt.htm?FK_Flow=" + ap.GetValStrByKey("FK_Flow") + "&WorkID=" + ap.GetValStrByKey("WorkID") + "&o2=1" + paras, true);
                            return;
                    }
                    //this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&o2=1" + paras, true);
                    return ;
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
