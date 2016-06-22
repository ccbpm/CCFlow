using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_TestFlow : BP.Web.WebPage
    {
        #region 属性。
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        public string GloSID
        {
            get
            {
                return BP.Web.WebUser.SID;
            }
        }
        #endregion 属性。

        public void BindFlowList()
        {
            this.Title = "感谢您选择驰骋工作流程引擎-流程设计&测试界面";
        }

        public void DoReturnToUser()
        {
            string userNo=this.Request.QueryString["UserNo"];
            string sid= BP.WF.Dev2Interface.Port_Login(userNo, false);

            string url = "../../WF/Port.aspx?UserNo=" + userNo + "&SID=" + sid + "&DoWhat=" + this.Request.QueryString["DoWhat"] + "&FK_Flow=" + this.FK_Flow + "&&IsMobile=" + this.Request.QueryString["IsMobile"];
            this.Response.Redirect(url, true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (this.GloSID != BP.WF.Glo.GloSID)
            //{
            //    this.Response.Write("全局的安全验证码错误,或者您没有设置,请在Web.config中的appsetting节点里设置GloSID 的值.");
            //    return;
            //}

            //if (BP.Web.WebUser.No != "admin")
            //{
            //    this.ToErrorPage("@登录信息丢失，请使用admin登录。");
            //    return;
            //}

            if (this.DoType == "ReturnToUser")
            {
                DoReturnToUser();
                return;
            }


            BP.Sys.SystemConfig.DoClearCash();
            // 让admin 登录.
            BP.WF.Dev2Interface.Port_Login("admin");
            //BP.WF.Dev2Interface.Port_Login("admin", this.SID);

            if (this.FK_Flow == null)
            {
                this.Ucsys1.AddFieldSet("关于流程测试");
                this.Ucsys1.AddUL();
                this.Ucsys1.AddLi("现在是流程测试状态，此功能仅仅提供给流程设计人员使用。");
                this.Ucsys1.AddLi("提供此功能的目的是，快速的让各个角色人员登陆，以便减少登陆的繁琐麻烦。");
                this.Ucsys1.AddLi("点左边的流程列表后，系统自动显示能够发起此流程的工作人员，点一个工作人员就直接登陆了。");
                this.Ucsys1.AddULEnd();
                this.Ucsys1.AddFieldSetEnd();
                return;
            }

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGenerLang(emp, this.Lang);
                this.Session["FK_Flow"] = this.FK_Flow;
                if (this.Request.QueryString["Type"] != null)
                {
                    string url = "../WAP/MyFlow.aspx?FK_Flow=" + this.FK_Flow;
                    if (this.Request.QueryString["IsWap"] == "1")
                        this.Response.Redirect("../WAP/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01", true);
                    else
                        this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01", true);
                }
                else
                {
                    this.Response.Redirect("../Port/Home.htm?FK_Flow=" + this.FK_Flow, true);
                }
                return;
            }

            BP.Web.WebUser.SysLang = this.Lang;
            Flow fl = new Flow(this.FK_Flow);
            fl.DoCheck();

            int nodeid = int.Parse(this.FK_Flow + "01");
            DataTable dt=null;
            string sql = "";
            BP.WF.Node nd = new BP.WF.Node(nodeid);

            if (nd.IsGuestNode)
            {
                /*如果是guest节点，就让其跳转到 guest登录界面，让其发起流程。*/

                //这个地址需要配置.
                this.Response.Redirect("/SDKFlowDemo/GuestApp/Login.aspx?FK_Flow="+this.FK_Flow);
                return;
            }

            try
            {
                if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
                {
                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
                            // edit by stone , 如果是BPM 就不能工作.
                            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                                sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_EmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_Empstation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                            else
                                sql = "SELECT Port_Emp.No  FROM Port_Emp LEFT JOIN Port_Dept   Port_Dept_FK_Dept ON  Port_Emp.FK_Dept=Port_Dept_FK_Dept.No  join Port_DeptEmpStation on (fk_emp=Port_Emp.No)   join WF_NodeStation on (WF_NodeStation.fk_station=Port_DeptEmpStation.fk_station) WHERE (1=1) AND  FK_Node=" + nd.NodeID;
                            // emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");
                            break;
                        case DeliveryWay.ByDept:
                            sql = "select No,Name from Port_Emp where FK_Dept in (select FK_Dept from WF_NodeDept where FK_Node='" + nodeid + "') ";
                            //emps.RetrieveInSQL("");
                            break;
                        case DeliveryWay.ByBindEmp:
                            sql = "select No,Name from Port_Emp where No in (select FK_Emp from WF_NodeEmp where FK_Node='" + nodeid + "') ";
                            //emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");
                            break;
                        case DeliveryWay.ByDeptAndStation:
                            //added by liuxc,2015.6.30.
                            //区别集成与BPM模式
                            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                            {
                                sql = "SELECT No FROM Port_Emp WHERE No IN ";
                                sql += "(SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ";
                                sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nodeid + ")";
                                sql += ")";
                                sql += "AND No IN ";
                                sql += "(";
                                sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                                sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeid + ")";
                                sql += ") ORDER BY No ";
                            }
                            else
                            {
                                sql = "SELECT pdes.FK_Emp AS No"
                                      + " FROM   Port_DeptEmpStation pdes"
                                      + "        INNER JOIN WF_NodeDept wnd"
                                      + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                                      + "             AND wnd.FK_Node = " + nodeid
                                      + "        INNER JOIN WF_NodeStation wns"
                                      + "             ON  wns.FK_Station = pdes.FK_Station"
                                      + "             AND wnd.FK_Node =" + nodeid
                                      + " ORDER BY"
                                      + "        pdes.FK_Emp";
                            }
                            break;
                        case DeliveryWay.BySQL:
                            if (string.IsNullOrEmpty(nd.DeliveryParas))
                                throw new Exception("@您设置的按SQL访问开始节点，但是您没有设置sql.");
                            // emps.RetrieveInSQL(nd.DeliveryParas);
                            break;
                        default:
                            break;
                    }

                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        throw new Exception("@您按照:" + nd.HisDeliveryWay + "的方式设置的开始节点的访问规则，但是开始节点没有人员。");
                }
                else
                {
                    /*如果要从webservices 获取数据.*/
                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
                            string stas = BP.DA.DBAccess.GenerWhereInPKsString("SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nd.NodeID);
                            if (stas.Length == 0)
                                throw new Exception("@开始节点没有设置岗位.");
                            var v = BP.DA.DataType.GetPortalInterfaceSoapClientInstance();
                            dt = v.GenerEmpsByStations(stas);
                            break;
                        case DeliveryWay.ByDept:
                            string depts = BP.DA.DBAccess.GenerWhereInPKsString("SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID);
                            var deptSoap = BP.DA.DataType.GetPortalInterfaceSoapClientInstance();
                            dt = deptSoap.GenerEmpsByDepts(depts);
                            break;
                        case DeliveryWay.ByBindEmp:
                            sql = "SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node='" + nodeid + "'";
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            break;
                        case DeliveryWay.ByDeptAndStation:
                            throw new Exception("@开始节点不支持的模式ByDeptAndStation");
                        case DeliveryWay.BySQL:
                            if (string.IsNullOrEmpty(nd.DeliveryParas))
                                throw new Exception("@您设置的按SQL访问开始节点，但是您没有设置sql.");
                            break;
                        default:
                            break;
                    }
                    if (dt.Rows.Count == 0)
                        throw new Exception("@您按照:" + nd.HisDeliveryWay + "的方式设置的开始节点的访问规则，但是开始节点没有人员。");
                }
            }
            catch (Exception ex)
            {
                this.Ucsys1.AddMsgOfWarning("错误原因",
                        "<h2>您没有正确的设置开始节点的访问规则，这样导致没有可启动的人员，请查看流程设计操作手册。</h2> 系统错误提示:" + ex.StackTrace + " - " + ex.Message + "<br><h3>也有可能你你切换了OSModel导致的，什么是OSModel,请查看在线帮助文档 <a href='http://ccbpm.mydoc.io' target=_blank>http://ccbpm.mydoc.io</a>  .</h3>");
                return;
            }

          //  this.Ucsys1.AddFieldSet("可发起(<font color=red>" + fl.Name + "</font>)流程的人员,选择一个人员进入并测试。");
            this.Ucsys1.AddTable("align=center width='100%' ");

            //this.Ucsys1.AddCaptionLeft("流程编号:" + fl.No + " 名称:" + fl.Name);
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("IDX");

            CheckBox cball = new CheckBox();
            cball.Attributes["onclick"] = "SelectAll(this);";
            cball.Text = "选择全部";
            this.Ucsys1.AddTDTitle(cball);
            //this.Ucsys1.AddTDTitle("EasyUI模式");
            this.Ucsys1.AddTDTitle("经典模式");
            this.Ucsys1.AddTDTitle("素颜模式");
            this.Ucsys1.AddTDTitle("手机模式");
            this.Ucsys1.AddTDTitle("所在部门");
            this.Ucsys1.AddTREnd();
            bool is1 = false;
            int idx = 0;

            string emps = "";
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr[0] as string ;
                if (string.IsNullOrEmpty(no))
                    throw new Exception("人员基础数据不完整，人员编号为空，请执行如下SQL检查："+sql);

                string myemp = dr[0].ToString();
                if (emps.Contains("," + myemp + ",") == true)
                    continue;

                emps += "," + myemp + ",";

                BP.Port.Emp emp = new Emp(myemp);
                idx++;
                is1 = this.Ucsys1.AddTR(is1);
                this.Ucsys1.AddTDIdx(idx);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + emp.No;
                cb.Text = emp.No + "," + emp.Name;
                this.Ucsys1.AddTD(cb);

                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=StartLigerUI&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&SID=" + this.GloSID + "'  ><img src='./../Img/IE.gif' border=0 />LigerUI模式</a>");

                this.Ucsys1.AddTD("<a href='?DoType=ReturnToUser&DoWhat=StartClassic&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />经典</a>");
                this.Ucsys1.AddTD("<a href='?DoType=ReturnToUser&DoWhat=StartSimple&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />素颜</a>");
                this.Ucsys1.AddTD("<a href='?DoType=ReturnToUser&DoWhat=StartClassic&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&IsMobile=1'  ><img src='./CCFormDesigner/Img/telephone.png' border=0 />手机模式</a>");

                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=StartLigerUI&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&SID="+this.GloSID+"'  ><img src='./../Img/IE.gif' border=0 />LigerUI模式</a>");
                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=Start5&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&SID=" + this.GloSID + "'  ><img src='./../Img/IE.gif' border=0 />经典模式</a>");
                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=Start5&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&SID=" + this.GloSID + "&IsMobile=1'  ><img src='./../Img/IE.gif' border=0 />手机模式</a>");
                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=Start&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&SID=" + this.GloSID + "'  ><img src='./../Img/IE.gif' border=0 />素颜模式</a>");                
                ////this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=StartSmallSingle&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />Internet Explorer</a>");  
              //  this.Ucsys1.AddTD("<a href='TestSDK.aspx?RefNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&IsWap=1'  >SDK</a> ");
                //this.Ucsys1.AddTD("<a href=\"javascript:WinOpenAndBrowser('./../Port.aspx?DoWhat=Amaze&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "' ,'470px','600px','" + emp.No + "');\"  ><img src='./../Img/IE.gif' border=0 width=25px height=18px />AmazeUI模式</a> ");
                this.Ucsys1.AddTD(emp.FK_DeptText);
                this.Ucsys1.AddTREnd();
            }

            Button btn = new Button();
            btn.Text = "把选择的人员执行自动模拟运行";
            btn.Click += new EventHandler(btn_Click);

            this.Ucsys1.AddTR();
            this.Ucsys1.AddTD("colspan=7", btn);
            this.Ucsys1.AddTREnd();
            this.Ucsys1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            string ids = "";
            foreach (Control ctl in this.Ucsys1.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID.Contains("CB_") == false)
                    continue;
                CheckBox cb = ctl as CheckBox;
                if (cb.Checked == false)
                    continue;

                ids += ctl.ID.Replace("CB_", "") + ",";
            }
            if (string.IsNullOrEmpty(ids) == true)
            {
                this.Response.Write("请选择要模拟发起的工作人员");
                return;
            }

            string url = "SimulationRun.aspx?FK_Flow=" + this.FK_Flow + "&IDs=" + ids;
            this.Response.Redirect(url, true);
        }
    }
}