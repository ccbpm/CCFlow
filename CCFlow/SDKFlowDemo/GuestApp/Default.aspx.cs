using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.DA;
using BP.Web;

public partial class AppDemo_GuestApp_Default1 : System.Web.UI.Page
{
    #region  运行变量
    /// <summary>
    /// 当前的流程编号
    /// </summary>
    public string FK_Flow
    {
        get
        {
            string s = this.Request.QueryString["FK_Flow"];
            if (string.IsNullOrEmpty(s))
                throw new Exception("@流程编号为空...");
            return s;
        }
    }
    public string FromNode
    {
        get
        {
            return this.Request.QueryString["FromNode"];
        }
    }
    /// <summary>
    /// 当前的工作ID
    /// </summary>
    public Int64 WorkID
    {
        get
        {
            if (ViewState["WorkID"] == null)
            {
                if (this.Request.QueryString["WorkID"] == null)
                    return 0;
                else
                    return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
            else
                return Int64.Parse(ViewState["WorkID"].ToString());
        }
        set
        {
            ViewState["WorkID"] = value;
        }
    }
    private int _FK_Node = 0;
    /// <summary>
    /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
    /// </summary>
    public int FK_Node
    {
        get
        {
            string fk_nodeReq = this.Request.QueryString["FK_Node"];
            if (string.IsNullOrEmpty(fk_nodeReq))
                fk_nodeReq = this.Request.QueryString["NodeID"];

            if (string.IsNullOrEmpty(fk_nodeReq) == false)
                return int.Parse(fk_nodeReq);

            if (_FK_Node == 0)
            {
                if (this.Request.QueryString["WorkID"] != null)
                {
                    string sql = "SELECT FK_Node from  WF_GenerWorkFlow where WorkID=" + this.WorkID;
                    _FK_Node = DBAccess.RunSQLReturnValInt(sql);
                }
                else
                {
                    _FK_Node = int.Parse(this.FK_Flow + "01");
                }
            }
            return _FK_Node;
        }
    }
    public int FID
    {
        get
        {
            try
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
            catch
            {
                return 0;
            }
        }
    }
    public int PWorkID
    {
        get
        {
            try
            {
                return int.Parse(this.Request.QueryString["PWorkID"]);
            }
            catch
            {
                return 0;
            }
        }
    }
    #endregion

    public string mainSrc = "Start.aspx";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (BP.Web.WebUser.No == null)
        {
            this.Response.Redirect("Login.aspx", true);
            return;
        }

        if (this.IsPostBack==false)
        {
            //是否有传值
            if (this.Request.QueryString.Count > 1)
            {
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
                //mainSrc = "/WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + FK_Node;
                BP.Demo.BPFramework.Student stu = new BP.Demo.BPFramework.Student();
                stu.No = "0001";
                stu.RetrieveFromDBSources();

                BP.WF.Dev2InterfaceGuest.Port_Login(stu.No, stu.Name);

                string s = "../../WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + FK_Node + "&GuestNo=" + GuestUser.No + "&GuestName=" + GuestUser.Name;
                this.Response.Write("<script type='text/javascript' language='javascript'> window.open('" + s + "');</script>");
            }
        }
    }
}