using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using BP.WF;
using BP.Port;
using BP.DA;
using BP.Web;
using CCFlow.AppDemoLigerUI.Base;

namespace CCFlow.AppDemoLigerUI
{
    public partial class Default : BasePage
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

        public string usermsg = "";
        public string mainSrc = "";
        //节点绑定的表单类型
        public string nodeformtype = "";
        //public string treedata = "";
        //public string strcanstartflow = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.Web.WebUser.No == null)
            {
                this.Response.Redirect("Login.aspx", true);
                return;
            }
            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                GetBPMMenu();
            }
            else
            {
                GetMenuTree();
            }

            if (!IsPostBack)
            {
                usermsg = "帐号：" + BP.Web.WebUser.No + "  姓名： " + BP.Web.WebUser.Name + " 部门：" + BP.Web.WebUser.FK_DeptName;
                //是否有传值
                if (this.Request.QueryString.Count > 2)
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
                    //判断起始节点 的表单类型
                    BP.WF.Node CurrentNode = new BP.WF.Node(this.FK_Node);
                    if (CurrentNode.HisFormType == NodeFormType.SheetTree)
                    {
                        nodeformtype = "SheetTree";
                    }
                    else
                    {
                        nodeformtype = "Other";
                    }
                    mainSrc = "../../MyFlow.aspx?FK_Flow=" + this.FK_Flow + paras + "&FK_Node=" + FK_Node;
                }
            }
        }
        /// <summary>
        /// 加载Ligertree
        /// </summary>
        public void GetMenuTree()
        {
            //默认菜单
            StringBuilder sbXML = GetCCForm();
            string sqlSort = "";
            string stitle = "流程列表";

            //加载流程权限
            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                //加载流
                sqlSort = "SELECT * FROM V_GPM_EmpMenu WHERE FK_Emp='" + BP.Web.WebUser.No + "' and FK_App = '" + BP.Sys.SystemConfig.SysNo + "'";
                DataTable dtSort = BP.DA.DBAccess.RunSQLReturnTable(sqlSort);
                if (dtSort != null && dtSort.Rows.Count > 0)
                {
                    //加载树
                    sbXML.AppendFormat("<div title='{0}' class='l-scroll'>", stitle);
                    sbXML.Append("<ul id=\"tree\"></ul>");
                    sbXML.Append("</div>");
                }
            }
            //加载组织结构
            if (BP.Web.WebUser.No == "admin")
            {
                sbXML.Append(GetGpmMenu());
            }
            this.accordion1.InnerHtml = sbXML.ToString();
        }

        /// <summary>
        /// 功能菜单
        /// </summary>
        public void GetBPMMenu()
        {
            StringBuilder sbXML = new StringBuilder("");
            
                sbXML.Append(GetCCForm());
            this.accordion1.InnerHtml = sbXML.ToString();
        }
        /// <summary>
        /// 功能菜单
        /// </summary>
        public StringBuilder GetCCForm()
        {
            StringBuilder sbXML = new StringBuilder("");
            sbXML.Append("<div title='功能列表' class='l-scroll'>");
            if (Glo.IsAdmin)
            {
                sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"FlowManager\",\"流程调度\",\"FlowManager.aspx\")'>");
                sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start1.png' /><span id='Span1'>流程调度</span></a>");
            }

            sbXML.Append("<a id='allowStartCount' class='l-link' href='javascript:f_addTab(\"startpage\",\"发起\",\"Start.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />发起</a>");

            sbXML.Append("<a id='allowStartCount' class='l-link' href='javascript:f_addTab(\"startpageTree\",\"发起(树)\",\"StartTree.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />发起(树)</a>");

            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"empworks\",\"待办\",\"EmpWorks.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/EmpWorks.png' /><span id='empworkCount'>待办(0)</span></a>");

            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"Batch\",\"批处理\",\"../../Batch.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start1.png' /><span id='Span1'>批处理</span></a>");

            if (BP.WF.Glo.IsEnableTaskPool == true)
            {
                sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"TaskPoolSmall\",\"共享任务\",\"../../TaskPoolSharing.aspx\")'>");
                sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start1.png' /><span id='TaskPoolNum'>共享任务</span></a>");
            }

            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"CCSmall\",\"抄送\",\"CC.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/CC.png' /><span id='ccsmallCount'>抄送(0)</span></a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"HungUp\",\"挂起\",\"HungUp.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/hungup.png' /><span id='hungUpCount'>挂起(0)</span></a>");
            sbXML.Append(" <a class='l-link' href='javascript:f_addTab(\"Running\",\"在途\",\"Running.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Runing1.png' />在途</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"FlowSearch\",\"查询\",\"FlowSearch.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Searchss.png' />查询</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"keySearch\",\"关键字查询\",\"keySearch.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Searchkey.png' />关键字查询</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"GetTask\",\"取回审批\",\"GetTask.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/tackback.png' />取回审批</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"Emps\",\"通讯录\",\"Emps.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/AddressCard.png' />通讯录</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"SmsList\",\"系统消息\",\"SmsList.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/sms.png' />系统消息</a>");
            sbXML.Append("<a class='l-link' href='javascript:f_addTab(\"ToolsSmall\",\"设置\",\"../../Tools.aspx\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Set.png' />设置</a>");
            sbXML.Append("</div>");

            return sbXML;
        }

        /// <summary>
        /// 组织结构 菜单
        /// </summary>
        public string GetGpmMenu()
        {
            StringBuilder sbXML = new StringBuilder("");
            sbXML.Append("<div title='组织结构' class='l-scroll'>");
            //sbXML.Append("<a id='a1' class='l-link' href='javascript:f_addTab(\"a1\",\"部门类型\",\"../../Comm/Ens.aspx?EnsName=BP.GPM.DeptTypes\")'>");
            //sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />部门类型</a>");

            sbXML.Append("<a id='a2' class='l-link' href='javascript:f_addTab(\"a2\",\"岗位类型\",\"../../Comm/Ens.aspx?EnsName=BP.GPM.StationTypes\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />岗位类型</a>");
            sbXML.Append("<a id='a3' class='l-link' href='javascript:f_addTab(\"a3\",\"岗位\",\"../../Comm/Ens.aspx?EnsName=BP.GPM.Stations\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />岗位</a>");
            sbXML.Append("<a id='a4' class='l-link' href='javascript:f_addTab(\"a4\",\"职务维护\",\"../../Comm/Ens.aspx?EnsName=BP.GPM.Dutys\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />职务维护</a>");
            sbXML.Append("<a id='a5' class='l-link' href='javascript:f_addTab(\"a5\",\"组织结构\",\"../../Admin/OrganizationalStructure.aspx?EnsName=BP.GPM.Depts\")'>");
            sbXML.Append("<img class='img-menu' align='middle' alt='' src='Img/Menu/Start.png' />组织结构</a>");
            sbXML.Append("</div>");
            return sbXML.ToString();
        }

    }
}