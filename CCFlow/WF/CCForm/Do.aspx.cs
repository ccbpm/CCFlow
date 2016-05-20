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
using BP.Web;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// Do ��ժҪ˵����
    /// </summary>
    public partial class Do : PageBase
    {
        public string ActionType
        {
            get
            {
                string s = this.Request.QueryString["ActionType"];
                if (s == null)
                    s = this.Request.QueryString["DoType"];

                return s;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string RefNo
        {
            get
            {
                return this.Request.QueryString["RefNo"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.Request.QueryString["FK_Emp"];
            }
        }
        public string PageID
        {
            get
            {
                return this.Request.QueryString["PageID"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                string s = this.Request.QueryString["NodeID"];
                if (s == null || s == "")
                    s = this.Request.QueryString["FK_Node"];
                return int.Parse(s);
            }
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");
            Response.AddHeader("Cache-Control", "no-store");
            Response.AddHeader("Expires", "0");
            Response.AddHeader("Pragma", "no-cache");
            string url = this.Request.RawUrl;
            if (url.Contains("DTT=") == false)
            {
                //this.Response.Redirect(url + "&DTT=" + DateTime.Now.ToString("mmDDhhmmss"), true);
                //return;
            }

            try
            {
                switch (this.ActionType)
                {
                    case "DoOpenCC":
                        string fk_flow1 = this.Request.QueryString["FK_Flow"];
                        string fk_node1 = this.Request.QueryString["FK_Node"];
                        string workid1 = this.Request.QueryString["WorkID"];
                        string fid1 = this.Request.QueryString["FID"];
                        string Sta = this.Request.QueryString["Sta"];
                        if (Sta == "0")
                        {
                            BP.WF.Template.CCList cc1 = new BP.WF.Template.CCList();
                            cc1.MyPK = this.Request.QueryString["MyPK"];
                            cc1.Retrieve();
                            cc1.HisSta = CCSta.Read;
                            cc1.Update();
                        }
                        this.Response.Redirect(this.Request.ApplicationPath + "WF/WorkOpt/OneWork/Track.aspx?FK_Flow=" + fk_flow1 + "&FK_Node=" + fk_node1 + "&WorkID=" + workid1 + "&FID=" + fid1, false);
                        return;
                    case "DelCC": //ɾ������.
                        CCList cc = new CCList();
                        cc.MyPK = this.MyPK;
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        this.WinClose();
                        break;
                    case "DelSubFlow": //ɾ�����̡�
                        try
                        {
                            WorkFlow wf14 = new WorkFlow(this.FK_Flow, this.WorkID);
                            wf14.DoDeleteWorkFlowByReal(true);
                            this.WinClose();
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                        }
                        break;
                    case "DownBill":
                        Bill b = new Bill(this.MyPK);
                        b.DoOpen();
                        break;
                    case "DelDtl":
                        GEDtls dtls = new GEDtls(this.EnsName);
                        GEDtl dtl = (GEDtl)dtls.GetNewEntity;
                        dtl.OID = this.RefOID;
                        if (dtl.RetrieveFromDBSources() == 0)
                        {
                            this.WinClose();
                            break;
                        }

                        FrmEvents fes = new FrmEvents(this.EnsName); //����¼�.
                        // ����ɾ��ǰ�¼�.
                        try
                        {
                           string r= fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelBefore, dtl);
                           if (r == "false" || r == "0")
                           {
                               this.WinClose();
                               return;
                           }
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        dtl.Delete();

                        // ����ɾ�����¼�.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        this.WinClose();
                        break;
                    case "EmpDoUp":
                        BP.WF.Port.WFEmp ep = new BP.WF.Port.WFEmp(this.RefNo);
                        ep.DoUp();

                        BP.WF.Port.WFEmps emps111 = new BP.WF.Port.WFEmps();
                      //  emps111.RemoveCash();
                        emps111.RetrieveAll();
                        this.WinClose();
                        break;
                    case "EmpDoDown":
                        BP.WF.Port.WFEmp ep1 = new BP.WF.Port.WFEmp(this.RefNo);
                        ep1.DoDown();

                        BP.WF.Port.WFEmps emps11441 = new BP.WF.Port.WFEmps();
                      //  emps11441.RemoveCash();
                        emps11441.RetrieveAll();
                        this.WinClose();
                        break;
                    case "OF":
                        string sid = this.Request.QueryString["SID"];
                        string[] strs = sid.Split('_');
                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                            GenerWorkerListAttr.WorkID, strs[1],
                            GenerWorkerListAttr.FK_Node, strs[2]);
                        if (i == 0)
                        {
                            this.Response.Write("<h2>��ʾ</h2>�˹����Ѿ������˴�����ߴ�������ɾ����");
                            return;
                        }
                        BP.Port.Emp emp155 = new BP.Port.Emp(wl.FK_Emp);

                        BP.Web.WebUser.SignInOfGener(emp155, true);
                        string u = "MyFlow.aspx?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID;
                        if (this.Request.QueryString["IsWap"] != null)
                            u = "./.../WAP/" + u;
                        this.Response.Write("<script> window.location.href='" + u + "'</script> *^_^*  <br><br>���ڽ���ϵͳ���Ժ������ʱ��û�з�Ӧ����<a href='" + u + "'>��������롣</a>");
                        return;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGenerLang(emp, WebUser.SysLang);
                        this.WinClose();
                        return;
                    case "LogAs":
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.FK_Emp);
                        if (wfemp.AuthorIsOK == false)
                        {
                            this.WinCloseWithMsg("��Ȩʧ��");
                            return;
                        }
                        BP.Port.Emp emp1 = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGener(emp1, WebUser.SysLang, WebUser.No, true, false);
                        this.WinClose();
                        return;
                    case "TakeBack": // ȡ����Ȩ��
                        BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
                        BP.DA.Log.DefaultLogWriteLineInfo("ȡ����Ȩ:" + WebUser.No + "ȡ���˶�(" + myau.Author + ")����Ȩ��");
                        myau.Author = "";
                        myau.AuthorWay = 0;
                        myau.Update();
                        this.WinClose();
                        return;
                    case "AutoTo": // ִ����Ȩ��
                        BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp();
                        au.No = WebUser.No;
                        au.RetrieveFromDBSources();
                        au.AuthorDate = BP.DA.DataType.CurrentData;
                        au.Author = this.FK_Emp;
                        au.AuthorWay = 1;
                        au.Save();
                        BP.DA.Log.DefaultLogWriteLineInfo("ִ����Ȩ:" + WebUser.No + "ִ���˶�(" + au.Author + ")����Ȩ��");
                        this.WinClose();
                        return;
                    case "UnSend": // ִ�г������͡�
                        try
                        {
                           string str1= BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
                           this.Session["info"] = str1;
                            this.Response.Redirect("MyFlowInfo" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID, false);
                            return;
                        }
                        catch (Exception ex)
                        {
                            this.Session["info"] = "@ִ�г���ʧ�ܡ�@ʧ����Ϣ" + ex.Message;
                            this.Alert(ex.Message);
                            //this.WinCloseWithMsg(ex.Message);
                            this.Response.Redirect("MyFlowInfo" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Type=warning", false);
                            return;
                        }
                    // this.Response.Redirect("MyFlow.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow, true);
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        Bill bk1 = new Bill(this.Request.QueryString["OID"]);
                        Node nd = new Node(bk1.FK_Node);
                        this.Response.Redirect("WFRpt.aspx?WorkID=" + bk1.WorkID + "&FID=" + bk1.FID + "&FK_Flow=" + nd.FK_Flow + "&NodeId=" + bk1.FK_Node, false);
                        //this.WinOpen();
                        //this.WinClose();
                        break;
                    case "PrintBill":
                        //Bill bk2 = new Bill(this.Request.QueryString["OID"]);
                        //Node nd2 = new Node(bk2.FK_Node);
                        //this.Response.Redirect("NodeRefFunc.aspx?NodeId=" + bk2.FK_Node + "&FlowNo=" + nd2.FK_Flow + "&NodeRefFuncOID=" + bk2.FK_NodeRefFunc + "&WorkFlowID=" + bk2.WorkID);
                        ////this.WinClose();
                        break;
                    //ɾ�������е�һ���ڵ�����ݣ��������칤��
                    case "DeleteFlow":
                        string fk_flow = this.Request.QueryString["FK_Flow"];
                        Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
                        //����DoDeleteWorkFlowByReal����
                        WorkFlow wf = new WorkFlow(new Flow(fk_flow), workid);
                        wf.DoDeleteWorkFlowByReal(true);
                        BP.WF.Glo.ToMsg("����ɾ���ɹ�");
                      //  this.ToWFMsgPage("����ɾ���ɹ�");
                        break;
                    default:
                        throw new Exception("ActionType error" + this.ActionType);
                }
            }
            catch (Exception ex)
            {
                this.ToErrorPage("ִ����������쳣��<BR>" + ex.Message);
            }
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
    }
}
