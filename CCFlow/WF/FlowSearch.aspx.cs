using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Data;

namespace CCFlow.WF
{
    public partial class Face_FlowSearch : BP.Web.WebPage
    {
        #region 属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                return s;
            }
        }
        /// <summary>
        /// 当前人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                string s = this.Request.QueryString["FK_Emp"];
                if (s == null)
                    return WebUser.No;
                return s;
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string DT_F
        {
            get
            {
                string f = this.Session["DF"] as string;
                if (f == null)
                {
                    this.Session["DF"] = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    return this.Session["DF"].ToString();
                }
                return f;
            }
        }
        public string DT_T
        {
            get
            {
                string f = this.Session["DT"] as string;
                if (f == null)
                {
                    this.Session["DT"] = DataType.CurrentData;
                    return this.Session["DT"].ToString();
                }
                return f;
            }
        }
        #endregion 属性

        public int PageSize = 600;
        public void BindSearch()
        {
            Node nd = new Node(this.FK_Node);
            Works wks = nd.HisWorks;
            QueryObject qo = new QueryObject(wks);
            qo.AddWhere(WorkAttr.Rec, WebUser.No);
            qo.addAnd();
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Access)
                qo.AddWhere("Mid(RDT,1,10) >='" + this.DT_F + "' AND Mid(RDT,1,10) <='" + this.DT_T + "' ");
            else
                qo.AddWhere("" + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) >='" + this.DT_F + "' AND " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) <='" + this.DT_T + "' ");

            this.Pub2.BindPageIdx(qo.GetCount(), this.PageSize, this.PageIdx, "FlowSearch.aspx?FK_Node=" + this.FK_Node);
            qo.DoQuery("OID", this.PageSize, this.PageIdx);

            // 生成页面数据。
            Attrs attrs = nd.HisWork.EnMap.Attrs;
            int colspan = 2;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                colspan++;
            }


            this.Pub1.AddTable("width='100%' align=center ");

            this.Pub1.AddCaption("<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/EmpWorks.gif' > <b><a href=FlowSearch.aspx >" + "流程查询" + "</a>-<a href='FlowSearch.aspx?FK_Flow=" + nd.FK_Flow + "'>" + nd.FlowName + "</a>-" + nd.Name + "</b>");

            this.Pub1.AddTR();
            this.Pub1.Add("<TD colspan=" + colspan + " class=TD>发生日期从:");
            TextBox tb = new TextBox();
            tb.ID = "TB_F";
            tb.Columns = 10;
            tb.Text = this.DT_F;
            this.Pub1.Add(tb);
            this.Pub1.Add("到:");
            tb = new TextBox();
            tb.ID = "TB_T";
            tb.Text = this.DT_T;
            tb.Columns = 10;
            this.Pub1.Add(tb);

            Button btn = new Button();
            btn.Text = " 查询 ";
            btn.CssClass = "Btn";
            btn.ID = "Btn_Search";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.CssClass = "Btn";
            btn.Text = "导出Excel";
            btn.ID = "Btn_Excel";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            this.Pub1.Add("</TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                this.Pub1.AddTDTitle(attr.Desc);
            }
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            bool is1 = false;
            foreach (Entity en in wks)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTD(idx);
                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;
                    switch (attr.MyDataType)
                    {
                        case DataType.AppBoolean:
                            this.Pub1.AddTD(en.GetValBoolStrByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                        case DataType.AppDouble:
                            this.Pub1.AddTD(en.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                this.Pub1.AddTD(en.GetValRefTextByKey(attr.Key));
                            }
                            else
                            {
                                this.Pub1.AddTD(en.GetValIntByKey(attr.Key));
                            }
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDMoney(en.GetValDecimalByKey(attr.Key));
                            break;
                        default:
                            this.Pub1.AddTD(en.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.Pub1.AddTD("<a href=\"./../WF/WFRpt.aspx?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >报告</a>-<a href=\"./../WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >轨迹</a>");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppDouble:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString());
                        break;
                    case DataType.AppMoney:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString("0.00"));
                        break;
                    default:
                        this.Pub1.AddTD();
                        break;
                }
            }
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == "Btn_Excel")
            {
                Node nd = new Node(this.FK_Node);
                Works wks = nd.HisWorks;
                QueryObject qo = new QueryObject(wks);
                qo.AddWhere(WorkAttr.Rec, WebUser.No);
                qo.addAnd();
                qo.AddWhere(BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) >='" + this.DT_F + "' AND " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) <='" + this.DT_T + "' ");

                this.Pub2.BindPageIdx(qo.GetCount(), this.PageSize, this.PageIdx, "FlowSearch.aspx?FK_Node=" + this.FK_Node);
                qo.DoQuery();

                try
                {
                    //this.ExportDGToExcel(ens.ToDataTableDescField(), this.HisEn.EnDesc);
                    this.ExportDGToExcel(wks.ToDataTableDesc(), nd.Name);
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.ExportDGToExcel(wks.ToDataTableDescField(), nd.Name);
                    }
                    catch (Exception ex1)
                    {
                        this.ToErrorPage("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message + ex1.Message);
                    }
                }
                return;
            }

            this.Session["DF"] = this.Pub1.GetTextBoxByID("TB_F").Text;
            this.Session["DT"] = this.Pub1.GetTextBoxByID("TB_T").Text;
            this.Response.Redirect("FlowSearch.aspx?FK_Node=" + this.FK_Node, true);
        }
        public void BindFlowWap()
        {
            Flow fl = new Flow(this.FK_Flow);
            int colspan = 4;

            this.Pub1.AddTable("width='600px' ");
            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleMsg colspan=" + colspan + " align=left><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Start.gif' > <b><a href='FlowSearch.aspx' >返回</a> - " + fl.Name + "</b></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("步骤");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("可执行否?");
            this.Pub1.AddTREnd();

            Nodes nds = new Nodes(this.FK_Flow);
            Stations sts = WebUser.HisStations;
            foreach (Node nd in nds)
            {
                bool isCan = false;
                foreach (Station st in sts)
                {
                    if (nd.HisStas.Contains("@" + st.No))
                    {
                        isCan = true;
                        break;
                    }
                }

                if (isCan == false)
                {
                    NodeEmp ne = new NodeEmp();
                    ne.FK_Emp = WebUser.No;
                    ne.FK_Node = nd.NodeID;
                    if (ne.IsExits)
                        isCan = true;

                    //if (nd.HisEmps.Contains("@" + WebUser.No))
                    //    isCan = true;
                }

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(nd.Step);
                if (isCan)
                    this.Pub1.AddTD("<a href='FlowSearch.aspx?FK_Node=" + nd.NodeID + "'>" + nd.Name + "</a>");
                else
                    this.Pub1.AddTD(nd.Name);

                if (isCan)
                {
                    this.Pub1.AddTD("可执行");
                }
                else
                {
                    this.Pub1.AddTD("不可执行");
                }
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("colspan=" + colspan, "&nbsp;");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public void BindFlow()
        {
            Flow fl = new Flow(this.FK_Flow);
            int colspan = 4;


            this.Pub1.AddTable("width=960px");
            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleMsg colspan=" + colspan + " align=left><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Start.gif' > <b><a href='FlowSearch.aspx' >返回</a> - " + fl.Name + "</b></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("节点步骤");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("可执行否?");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            Nodes nds = new Nodes(this.FK_Flow);
            Stations sts = WebUser.HisStations;
            foreach (Node nd in nds)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD(nd.Step);
                this.Pub1.AddTD(nd.Name);

                bool isCan = false;
                if (nd.HisFormType == NodeFormType.SDKForm || nd.HisFormType == NodeFormType.SelfForm)
                {
                    isCan = false;
                }
                else
                {
                    foreach (Station st in sts)
                    {
                        if (nd.HisStas.Contains("@" + st.No))
                        {
                            isCan = true;
                            break;
                        }
                    }
                }

                if (isCan)
                {
                    this.Pub1.AddTD("可执行");
                    this.Pub1.AddTD("<a href='FlowSearch.aspx?FK_Node=" + nd.NodeID + "'>查询</a>");
                }
                else
                {
                    this.Pub1.AddTD("不可执行");
                    this.Pub1.AddTD();
                }
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("colspan=" + colspan, "&nbsp;");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public void BindMyWork()
        {
            Node nd = new Node(this.FK_Node);
            Works wks = nd.HisWorks;
            QueryObject qo = new QueryObject(wks);
            qo.AddWhere(WorkAttr.Rec, WebUser.No);
            qo.addAnd();
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Access)
                qo.AddWhere("Mid(RDT,1,10) >='" + this.DT_F + "' AND Mid(RDT,1,10) <='" + this.DT_T + "' ");
            else
                qo.AddWhere("" + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) >='" + this.DT_F + "' AND " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) <='" + this.DT_T + "' ");

            this.Pub2.BindPageIdx(qo.GetCount(), this.PageSize, this.PageIdx, "FlowSearch.aspx?FK_Node=" + this.FK_Node);
            qo.DoQuery("OID", this.PageSize, this.PageIdx);

            // 生成页面数据。
            Attrs attrs = nd.HisWork.EnMap.Attrs;
            int colspan = 2;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                colspan++;
            }

            this.Pub1.AddTable("width='100%' align=center ");
            //this.Pub1.AddTR();
            //this.Pub1.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            //this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleMsg  align=left colspan=" + colspan + "><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/EmpWorks.gif' > <b><a href=FlowSearch.aspx >流程查询</a>-<a href='FlowSearch.aspx?FK_Flow=" + nd.FK_Flow + "'>" + nd.FlowName + "</a>-" + nd.Name + "</b></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD colspan=" + colspan + " class=TD>发生日期从:");

            TextBox tb = new TextBox();
            tb.ID = "TB_F";
            tb.Columns = 10;
            tb.Text = this.DT_F;
            tb.Attributes["onfocus"] = "WdatePicker();";
            this.Pub1.Add(tb);

            this.Pub1.Add("到:");
            tb = new TextBox();
            tb.ID = "TB_T";
            tb.Text = this.DT_T;
            tb.Columns = 7;
            tb.Attributes["onfocus"] = "WdatePicker();";
            this.Pub1.Add(tb);

            Button btn = new Button();
            btn.Text = " 查询 ";
            btn.CssClass = "Btn";
            btn.ID = "Btn_Search";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.Text = "导出Excel";
            btn.CssClass = "Btn";
            btn.ID = "Btn_Excel";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            this.Pub1.Add("</TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.Pub1.AddTDTitle(attr.Desc);
            }
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            bool is1 = false;
            foreach (Entity en in wks)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTD(idx);
                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;
                    switch (attr.MyDataType)
                    {
                        case DataType.AppBoolean:
                            this.Pub1.AddTD(en.GetValBoolStrByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                        case DataType.AppDouble:
                            this.Pub1.AddTD(en.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                this.Pub1.AddTD(en.GetValRefTextByKey(attr.Key));
                            }
                            else
                            {
                                this.Pub1.AddTD(en.GetValIntByKey(attr.Key));
                            }
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDMoney(en.GetValDecimalByKey(attr.Key));
                            break;
                        default:
                            this.Pub1.AddTD(en.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.Pub1.AddTD("<a href=\"./../WF/WFRpt.aspx?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >报告</a>-<a href=\"./../WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >轨迹</a>");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppDouble:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString());
                        break;
                    case DataType.AppMoney:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString("0.00"));
                        break;
                    default:
                        this.Pub1.AddTD();
                        break;
                }
            }
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public void BindBill()
        {
            Flow fl1 = new Flow(this.FK_Flow);
            this.Pub1.AddTable("width='100%'");
            this.Pub1.AddCaption("您的位置:单据查询 <a href='FlowSearch.aspx' >返回</a> => " + fl1.Name);
            this.Pub1.AddTR();
            this.Pub1.Add("<TD  class=TD  height=800px  width=100% >");
            string src = "" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Search.htm?EnsName=BP.WF.Bills&FK_Flow=" + this.FK_Flow;
            this.Pub1.Add("<iframe ID='f23' frameborder=0  style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' height='100%' width='100%' scrolling=no  /></iframe>");
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            return;



            //Bills bills = new Bills();
            //QueryObject qo = new QueryObject(bills);
            //qo.AddWhere(BillAttr.FK_Flow, this.FK_Flow);
            //qo.addAnd();
            //qo.addLeftBracket();
            //qo.AddWhere(BillAttr.FK_Emp, WebUser.No);
            //qo.addOr();
            //qo.AddWhere(BillAttr.FK_Starter, WebUser.No);
            //qo.addRightBracket();
            //qo.DoQuery();

            //if (this.FK_Flow != null)
            //{
            //    Flow fl = new Flow(this.FK_Flow);
            //    this.Pub1.AddTable("width=100%");
            //    this.Pub1.AddCaption("您的位置：单据查询 <a href='FlowSearch.aspx' >返回</a> => " + fl.Name);
            //}
            //else
            //{
            //    this.Pub1.AddTable();
            //}

            //this.Pub1.AddTR();
            //this.Pub1.AddTDTitle("ID");
            //this.Pub1.AddTDTitle("标题");
            //this.Pub1.AddTDTitle("发起");
            //this.Pub1.AddTDTitle("发起日期");
            //this.Pub1.AddTDTitle("发起人部门");
            //this.Pub1.AddTDTitle("单据名称");
            //this.Pub1.AddTDTitle("打印人");
            //this.Pub1.AddTDTitle("打印日期");
            //this.Pub1.AddTDTitle("月份");
            //this.Pub1.AddTREnd();

            //int i = 0;
            //bool is1 = false;
            //foreach (BP.WF.Data.Bill bill in bills)
            //{
            //    this.Pub1.AddTR(is1);
            //    i++;
            //    this.Pub1.AddTDIdx(i);
            //    this.Pub1.AddTD(bill.Title);
            //    this.Pub1.AddTD(bill.FK_StarterT);
            //    this.Pub1.AddTD(bill.RDT);
            //    this.Pub1.AddTD(bill.FK_DeptT);
            //    this.Pub1.AddTDA("javascript:WinOpen('" + bill.Url + "')", "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/Word.gif' border=0 />" + bill.FK_BillText);
            //    this.Pub1.AddTD(bill.FK_EmpT);
            //    this.Pub1.AddTD(bill.RDT);
            //    this.Pub1.AddTD(bill.FK_NY);
            //    this.Pub1.AddTREnd();
            //}
            //this.Pub1.AddTableEnd();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "查询与分析";
            switch (this.DoType)
            {
                case "Bill":
                    this.BindBill();
                    return;
                case "MyWork":
                    this.BindMyWork();
                    return;
                default:
                    break;
            }

            if (this.FK_Flow != null)
            {
                if (WebUser.IsWap)
                    this.BindFlowWap();
                else
                    this.BindFlow();
                return;
            }

            if (this.FK_Node != 0)
                return;

            if (WebUser.IsWap)
            {
                BindWap();
                return;
            }

            int colspan = 5;
            this.Pub1.AddTable("width='90%' align=left");
            if (WebUser.IsWap)
                this.Pub1.AddCaptionLeftTX2("<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Home.gif' ><a href='Home.aspx' >Home</a> - <img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Search.gif' >-流程查询");
            else
                this.Pub1.AddCaptionLeftTX2("<div style='float:left'><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Search.gif' >流程查询/分析</div><div style='float:right'><a href=\"javascript:WinOpen('KeySearch.aspx',900,900); \">关键字查询</a>|<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Search.htm?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900); \">综合查询</a>|<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Group.htm?EnsName=BP.WF.Data.GenerWorkFlowViews',900,900); \">综合分析</a>|<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Search.htm?EnsName=BP.WF.WorkFlowDeleteLogs',900,900); \">删除日志</a></div>");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("width='70%'", "流程名称");
            this.Pub1.AddTDTitle("单据");
            this.Pub1.AddTDTitle("流程查询-分析");
            this.Pub1.AddTREnd();

            Flows fls = new Flows();
            fls.RetrieveAll();
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();
            string search = "查询";
            string FX = "分析";
            int idx = 1;
            int gIdx = 0;
            foreach (FlowSort fs in fss)
            {
                if (fs.ParentNo == "0"
                    || fs.ParentNo == ""
                    || fs.No == "0")
                    continue;

                gIdx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDB("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='./Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + fs.Name + "</b>");
                this.Pub1.AddTREnd();
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;

                    this.Pub1.AddTR("ID='" + gIdx + "_" + idx + "'");
                    this.Pub1.AddTDIdx(idx++);

                    if (WebUser.IsWap == false)
                        this.Pub1.AddTD("width='60%'", "<a href=\"javascript:WinOpen('/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Flow=" + fl.No + "&DoType=Chart','sd');\"  >" + fl.Name + "</a>");
                    else
                        this.Pub1.AddTD(fl.Name);

                    if (fl.NumOfBill == 0)
                    {
                        this.Pub1.AddTD("无");
                    }
                    else
                    {
                        //string src = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Bill.aspx?EnsName=BP.WF.Bills&FK_Flow=" + fl.No;
                       // this.Pub1.AddTD("<a href=\"javascript:WinOpen('" + src + "');\"  ><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/Word.gif' border=0/>" + bill + "</a>");

                    }
                    this.Pub1.AddTDBegin();
                    string src2 = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No;
                    this.Pub1.Add("<a href=\"javascript:WinOpen('" + src2 + "');\" >" + search + "</a>");
                    src2 = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Group.aspx?FK_Flow=" + fl.No + "&DoType=Dept";
                    this.Pub1.Add(" - <a href=\"javascript:WinOpen('" + src2 + "');\" >" + FX + "</a>");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }
        public void BindWap()
        {
            this.Pub1.AddFieldSet("<a href='Home.aspx' ><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Home.gif' >Home</a>");
            Flows fls = new Flows();
            fls.RetrieveAll();
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            this.Pub1.AddUL();
            foreach (FlowSort fs in fss)
            {
                this.Pub1.AddBR(fs.Name);
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;
                    string src2 = BP.WF.Glo.CCFlowAppPath + "WF/Rpt/Search.aspx?EnsName=ND" + int.Parse(fl.No) + "Rpt&FK_Flow=" + fl.No + "&IsWap=1";
                    this.Pub1.AddLi("<a href='" + src2 + "' >" + fl.Name + "</a>");
                }
            }
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();
        }
        public void BindWap_bak()
        {
            this.Pub1.AddFieldSet("<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Home.gif' ><a href='Home.aspx' >Home</a>");
            Flows fls = new Flows();
            fls.RetrieveAll();
            bool is1 = false;

            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();

            string search = "查询";
            string dtl = "明细";
            string bill = "单据";
            string nodeSearch = "节点";
            string FX = "分析";

            this.Pub1.Add("<table width='100%' border=0 >");
            foreach (FlowSort fs in fss)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDTitle(fs.Name);
                this.Pub1.AddTREnd();
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;

                    is1 = this.Pub1.AddTR(is1);
                    this.Pub1.AddTDBegin();

                    this.Pub1.Add(fl.Name);
                    this.Pub1.AddBR();

                    if (fl.NumOfBill == 0)
                        this.Pub1.Add("--");
                    else
                    {
                        string src = "" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Search.htm?EnsName=BP.WF.Bills&FK_Flow=" + fl.No;
                        this.Pub1.Add("<a href=\"javascript:WinOpen('" + src + "');\"  >" + bill + "</a>");
                    }


                    string src1 = "" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Search.htm?EnsName=ND" + int.Parse(fl.No) + "Rpt";
                    this.Pub1.Add("-<a href=\"javascript:WinOpen('" + src1 + "');\" >" + search + "</a>");
                    this.Pub1.Add("-<a href=\"javascript:Dtl('" + fl.No + "');\" >" + dtl + "</a>");

                    src1 = BP.WF.Glo.CCFlowAppPath + "WF/Comm/Group.htm?EnsName=ND" + int.Parse(fl.No) + "Rpt";
                    this.Pub1.Add("-<a href=\"javascript:WinOpen('" + src1 + "');\" >" + FX + "</a>");

                    this.Pub1.Add("-<a href='FlowSearch.aspx?FK_Flow=" + fl.No + "'>" + nodeSearch + "</a>");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
            this.Pub1.AddFieldSetEnd();
        }
    }

}