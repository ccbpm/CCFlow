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
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Sys;
using System.Text;

namespace CCFlow.WF.WorkOpt.OneWork
{
    public partial class TrackUC : BP.Web.UC.UCBase3
    {
        #region 属性
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.FK_Flow + "01");
            }
        }
        public string FK_Flow
        {
            get
            {
                string flow = this.Request.QueryString["FK_Flow"];
                if (flow == null)
                {
                    throw new Exception("@没有获取它的流程编号。");
                }
                else
                {
                    return flow;
                }
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
                try
                {
                    return int.Parse(this.Request.QueryString["NodeID"]);
                }
                catch
                {
                    return 0;
                }
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
        #endregion

        public void ViewWork()
        {
            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node, ReturnWorkAttr.WorkID, this.WorkID);

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            this.AddB(wk.EnDesc);
            this.ADDWork(wk, rws, this.FK_Node);
        }
        public void BindTrack_ViewSpecialWork()
        {
            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node, ReturnWorkAttr.WorkID, this.WorkID);

            //ShiftWorks fws = new ShiftWorks();
            //fws.Retrieve(ShiftWorkAttr.FK_Node, this.FK_Node, ShiftWorkAttr.WorkID, this.WorkID);

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            this.AddB(wk.EnDesc);
            this.ADDWork(wk, rws, this.FK_Node);
        }
        /// <summary>
        /// view work.
        /// </summary>
        public void BindTrack_ViewWork()
        {
            string appPath = BP.WF.Glo.CCFlowAppPath;//this.Request.ApplicationPath;
            Track tk = new Track(this.FK_Flow, this.MyPK);
            Node nd = new Node(tk.NDFrom);
            Work wk = nd.HisWork;
            wk.OID = tk.WorkID;
            if (wk.RetrieveFromDBSources() == 0)
            {
                GERpt rtp = nd.HisFlow.HisGERpt;
                rtp.OID = this.WorkID;
                if (rtp.RetrieveFromDBSources() == 0)
                {
                    AddEasyUiPanelInfo("打开(" + nd.Name + ")错误",
                                       "<h4>当前的节点数据已经被删除！</h4>"
                                       + "<p style='font-weight:bold'>造成此问题出现的原因如下：<br /><br />"
                                       + "1、当前节点数据被非法删除；<br />"
                                       + "2、节点数据是退回人与被退回人中间的节点，这部分节点数据查看不支持。</p>",
                                       "icon-no");
                    return;
                }
                wk.Row = rtp.Row;
            }

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            gwf.RetrieveFromDBSources();
            if (nd.HisFlow.IsMD5 && wk.IsPassCheckMD5() == false)
            {
                AddEasyUiPanelInfo("打开(" + nd.Name + ")错误",
                                   "<h2>当前的节点数据已经被篡改，请报告管理员。</h2>",
                                   "icon-no");
                return;
            }

            this.UCEn1.IsReadonly = true;
            Frms frms = nd.HisFrms;
            if (frms.Count == 0)
            {
                if (nd.HisFormType == NodeFormType.FreeForm)
                {
                    /* 自由表单 */
                    this.UCEn1.Add("<div id=divCCForm >");
                    this.UCEn1.BindCCForm(wk, "ND" + nd.NodeID, true, 0, false); //, false, false, null);
                    if (wk.WorkEndInfo.Length > 2)
                        this.UCEn1.Add(wk.WorkEndInfo);
                    this.UCEn1.Add("</div>");
                }

                if (nd.HisFormType == NodeFormType.FixForm)
                {
                    /*傻瓜表单*/
                    this.UCEn1.IsReadonly = true;
                    this.UCEn1.BindColumn4(wk, "ND" + nd.NodeID); //, false, false, null);
                    if (wk.WorkEndInfo.Length > 2)
                        this.UCEn1.Add(wk.WorkEndInfo);
                }

                BillTemplates bills = new BillTemplates();
                bills.Retrieve(BillTemplateAttr.NodeID, nd.NodeID);
                if (bills.Count >= 1)
                {
                    string title = "";
                    foreach (BillTemplate item in bills)
                        title += "<img src='" + appPath + "WF/Img/Btn/Word.gif' border=0/>" + item.Name + "</a>";

                    string urlr = appPath + "WF/WorkOpt/PrintDoc.aspx?FK_Node=" + nd.NodeID + "&FID=" + tk.FID + "&WorkID=" + tk.WorkID + "&FK_Flow=" + tk.FK_Flow;
                    this.UCEn1.Add("<p><a  href=\"javascript:WinOpen('" + urlr + "','dsdd');\"  />" + title + "</a></p>");
                    //this.UCEn1.Add("<a href='' target=_blank><img src='/WF/Img/Btn/Word.gif' border=0/>" + bt.Name + "</a>");
                }
            }
            else
            {
                /* 涉及到多个表单的情况...*/
                if (nd.HisFormType != NodeFormType.DisableIt)
                {
                    Frm myfrm = new Frm();
                    myfrm.No = "ND" + nd.NodeID;
                    myfrm.Name = wk.EnDesc;
                    myfrm.HisFormRunType = (FormRunType)(int)nd.HisFormType;

                    FrmNode fnNode = new FrmNode();
                    fnNode.FK_Frm = myfrm.No;
                   // fnNode.IsEdit = true;
                    fnNode.IsPrint = false;
                    switch (nd.HisFormType)
                    {
                        case NodeFormType.FixForm:
                            fnNode.HisFrmType = FrmType.Column4Frm;
                            break;
                        case NodeFormType.FreeForm:
                            fnNode.HisFrmType = FrmType.FreeFrm;
                            break;
                        case NodeFormType.SelfForm:
                            fnNode.HisFrmType = FrmType.Url;
                            break;
                        default:
                            throw new Exception("出现了未判断的异常。");
                    }
                    myfrm.HisFrmNode = fnNode;
                    frms.AddEntity(myfrm, 0);
                }

                Int64 fid = this.FID;
                if (this.FID == 0)
                    fid = tk.WorkID;

                if (frms.Count == 1)
                {
                    /* 如果禁用了节点表单，并且只有一个表单的情况。*/
                    Frm frm = (Frm)frms[0];
                    FrmNode fn = frm.HisFrmNode;
                    string src = "";
                    src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=0&IsPrint=0&FK_Node=" + nd.NodeID + "&WorkID=" + tk.WorkID;
                    this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                    this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'  /></iframe>");
                    this.UCEn1.Add("\t\n </DIV>");
                }
                else
                {
                    #region 载入相关文件.
                    this.Page.RegisterClientScriptBlock("sg",
       "<link href='./Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");

                    this.Page.RegisterClientScriptBlock("s2g4",
             "<script language='JavaScript' src='./Style/Frm/jquery.min.js' ></script>");

                    this.Page.RegisterClientScriptBlock("sdf24j",
            "<script language='JavaScript' src='./Style/Frm/jquery.idTabs.min.js' ></script>");

                    this.Page.RegisterClientScriptBlock("sdsdf24j",
            "<script language='JavaScript' src='./Style/Frm/TabClick.js' ></script>");
                    #endregion 载入相关文件.

                    this.UCEn1.Clear();
                    this.UCEn1.Add("<div  style='clear:both' ></div>");
                    this.UCEn1.Add("\t\n<div  id='usual2' class='usual' >");  //begain.

                    #region 输出标签.
                    this.UCEn1.Add("\t\n <ul  class='abc' style='background:red;border-color: #800000;border-width: 10px;' >");
                    foreach (Frm frm in frms)
                    {
                        FrmNode fn = frm.HisFrmNode;
                        string src = "";
                        src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=0&IsPrint=0&FK_Node=" + nd.NodeID + "&WorkID=" + tk.WorkID;
                        this.UCEn1.Add("\t\n<li><a href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" + frm.Name + "</a></li>");
                    }
                    this.UCEn1.Add("\t\n </ul>");
                    #endregion 输出标签.

                    #region 输出表单 iframe 内容.
                    foreach (Frm frm in frms)
                    {
                        FrmNode fn = frm.HisFrmNode;
                        this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                        string src = "loading.htm";
                        this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'   /></iframe>");
                        this.UCEn1.Add("\t\n </DIV>");
                    }
                    #endregion 输出表单 iframe 内容.

                    this.UCEn1.Add("\t\n</div>"); // end  usual2

                    // 设置选择的默认值.
                    this.UCEn1.Add("\t\n<script type='text/javascript'>");
                    this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frms[0].No + "\");");
                    this.UCEn1.Add("\t\n</script>");
                }
            }
        }

        public string CCID
        {
            get
            {
                return this.Request.QueryString["CCID"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //以下判断没发现起到什么作用
            if (BP.WF.Dev2Interface.Flow_IsCanViewTruck(this.FK_Flow, this.WorkID, this.FID) == false)
            {
                //string url = this.Request.RawUrl;
                ////url=url.Replace
                ////this.Response.Redirect(url,true);

                //string errorStr = "您没有权限查看该工作";
                //errorStr += "<br>1，您非该流程的处理人.";
                //errorStr += "<br>2, 您不与该流程发起人是同一个部门。";
                //return ;
            }

            if (this.DoType == "View")
            {
                this.BindTrack_ViewWork();
                return;
            }

            if (this.DoType == "ViewSpecialWork")
            {
                this.BindTrack_ViewSpecialWork();
                return;
            }


            //this.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width: 100%'");
            //this.AddTR();
            //this.AddTDGroupTitle("style='text-align:center'", "序");
            //this.AddTDGroupTitle("操作时间");
            //this.AddTDGroupTitle("表单"); //moved by liuxc,2014-12-18,应zhangqingpeng要求将此列提前
            //this.AddTDGroupTitle("从节点");
            //this.AddTDGroupTitle("人员");
            //this.AddTDGroupTitle("到节点");
            //this.AddTDGroupTitle("人员");
            //this.AddTDGroupTitle("到达时间");
            //this.AddTDGroupTitle("用时");
            //this.AddTDGroupTitle("活动");
            //this.AddTDGroupTitle("信息");
            //this.AddTDGroupTitle("执行人");
            //this.AddTREnd();

            //获取track.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerTrack(this.FK_Flow, this.WorkID, this.FID).Tables["Track"];
            DataView dv = dt.DefaultView;
            dv.Sort = "RDT";

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            gwf.RetrieveFromDBSources();

            string currNodeID = "0";
            if (gwf.WFState != WFState.Complete)
                currNodeID = gwf.FK_Node.ToString(); //获得当前运行到的节点如果流程完成则为O.

            int idx = 1;
            string checkStr = "";

            #region    qin   改成时间轴的显示方式 15/7/15   如果不需要这种显示方式
            string str = "";//拼接字符串
            str += "<div style='width: 100%;height:400px;'><div class='content'>";
            str += "<div class='wrapper'>";
            str += "<div class='main'>";

            str += "<h1 class='title'>";
            str += "流程日志</h1>";
            str += "<div class='year'>";
            str += "<h2>";
            str += "<a href='#'>&nbsp;&nbsp;时间轴<i></i></a></h2>";
            str += "<div class='list'>";
            str += "<ul>";
            #endregion qin   改成时间轴的显示方式 15/7/15

            #region 开始循环添加数据
            foreach (DataRowView dr in dv)
            {
                Int64 fid = int.Parse(dr["FID"].ToString());
                if (fid != 0)
                    continue;

                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());

                //如果是协作发送，就不输出他. edit 2016.02.20 .
                if (at == ActionType.TeampUp)
                    continue;

                // 记录审核节点。
                if (at == ActionType.WorkCheck)
                    checkStr = dr[TrackAttr.NDFrom].ToString(); //记录当前的审核节点id.

                //审核信息过滤, 
                if (at == ActionType.WorkCheck)
                {
                    if (currNodeID == checkStr)
                        continue;
                    //如果当前节点与审核信息节点一致，就说明当前人员的审核意见已经保存，但是工作还没有发送,就不让他显示。
                }

                if (at == ActionType.Forward)
                {
                    if (checkStr == dr[TrackAttr.NDFrom].ToString())
                        continue;
                }
                str += "<li  class='cls highlight' style=\"background: url('../../Img/Action/" + at.ToString() + ".png') no-repeat 235px 31px\">";
                //this.AddTR();
                //this.AddTDIdx(idx);

                DateTime dtt = DataType.ParseSysDateTime2DateTime(dr[TrackAttr.RDT].ToString());

                // this.AddTD(dtt.ToString("yy年MM月dd日HH:mm"));</br>
                str += "<p class='date'>" + dtt.ToString("yy年MM月dd日") + " " + dtt.ToString("hh时mm分");
                str += "</br>" + BP.DA.DataType.GetWeek((int)dtt.DayOfWeek);
                str += "</br>" + BP.WF.Glo.GenerUserImgHtml(dr[TrackAttr.EmpFrom].ToString(), dr[TrackAttr.EmpFromT].ToString()) + "</p>";
                str += "<p class='intro'>" + dr[TrackAttr.NDFromT].ToString() + "</p>";

                str += "<div class='more'>";

                if (at == ActionType.Forward || at == ActionType.ForwardAskfor || at == ActionType.WorkCheck || at == ActionType.Order
                    || at == ActionType.FlowOver    //added by liuxc,2014-12-3,正常结束结点也显示表单
                    || at == ActionType.Skip)   //added by liuxc,2015-7-13,自动跳转的也显示表单
                {
                    //this.AddTD("<a class='easyui-linkbutton' data-options=\"iconCls:'icon-sheet'\" href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.aspx?WorkID=" + dr[TrackAttr.WorkID].ToString() + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + dr[TrackAttr.NDFrom].ToString() + "&DoType=View&MyPK=" + dr[TrackAttr.MyPK].ToString() + "','" + dr[TrackAttr.MyPK].ToString() + "');\">表单</a>");
                    str += "<p><a class='easyui-linkbutton' data-options=\"iconCls:'icon-sheet'\" href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.aspx?WorkID=" + dr[TrackAttr.WorkID].ToString() + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + dr[TrackAttr.NDFrom].ToString() + "&DoType=View&MyPK=" + dr[TrackAttr.MyPK].ToString() + "','" + dr[TrackAttr.MyPK].ToString() + "');\">打开<img src='../../Img/Form.png'>表单</a>" + "</p>";
                }
               
                if (at == ActionType.FlowOver
                    || at == ActionType.CC
                    || at == ActionType.UnSend)
                {
                    str += "<p></p>";
                    str += "<p></p>";
                }
                else
                {
                    str += "<p>发送到节点：" + dr[TrackAttr.NDToT].ToString() + "</p>";
                }

                //增加两列，到达时间、用时 added by liuxc,2014-12-4
                if (idx > 1)
                {
                    var toTime = Convert.ToDateTime(dv[idx - 1 - 1][TrackAttr.RDT].ToString());
                    str += "<p>到达时间：" + toTime.ToString("yy年MM月dd日HH:mm") + " 用时：" + DataType.GetSpanTime(toTime, dtt) + "</p>";
                }

                // 删除信息.
                string tag = dr[TrackAttr.Tag].ToString();
                if (tag != null)
                    tag = tag.Replace("~", "'");

                string msg = DataType.ParseText2Html(dr[TrackAttr.Msg].ToString());
                switch (at)
                {
                    case ActionType.CallChildenFlow: //被调用父流程吊起。
                        if (string.IsNullOrEmpty(tag) == false)
                        {
                            AtPara ap = new AtPara(tag);
                            GenerWorkFlow mygwf = new GenerWorkFlow();
                            mygwf.WorkID = ap.GetValInt64ByKey("PWorkID");
                            if (mygwf.RetrieveFromDBSources() == 1)
                                msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程{" + mygwf.FlowName + "},<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a></p>";
                            else
                                msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上，被父流程调用{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                            msg = "<a target=b" + ap.GetValStrByKey("PWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("PWorkID") + "&FK_Flow=" + ap.GetValStrByKey("PFlowNo") + "' >" + msg + "</a>";
                        }
                        break;
                    case ActionType.StartChildenFlow: //吊起子流程。
                        if (string.IsNullOrEmpty(tag) == false)
                        {
                            if (tag.Contains("Sub"))
                                tag = tag.Replace("Sub", "C");

                            AtPara ap = new AtPara(tag);
                            GenerWorkFlow mygwf = new GenerWorkFlow();
                            mygwf.WorkID = ap.GetValInt64ByKey("CWorkID");
                            if (mygwf.RetrieveFromDBSources() == 1)
                            {
                                msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}, <a target=b" + ap.GetValStrByKey("CWorkID") + " href='Track.aspx?WorkID=" + ap.GetValStrByKey("CWorkID") + "&FK_Flow=" + ap.GetValStrByKey("CFlowNo") + "' >" + msg + "</a></p>";
                                msg += "<p>当前子流程状态：{" + mygwf.WFStateText + "}，运转到:{" + mygwf.NodeName + "}，最后处理人{" + mygwf.TodoEmps + "}，最后处理时间{" + mygwf.RDT + "}。</p>";
                            }
                            else
                                msg = "<p>操作员:{" + dr[TrackAttr.EmpFromT].ToString() + "}在当前节点上调用了子流程{" + mygwf.FlowName + "}，但是该流程被删除了.</p>" + tag;

                        }
                        break;
                    default:
                        break;
                }

                msg = msg.Replace("\"", "");

                str += "<p>" + msg + "</p>";
                str += "</div>";
                str += "</li>";
                idx++;
            }

            //判断当前工作是否完成，如果没有完成，就输出当前人员的待办信息 ，读取信息。

            if (gwf.WFState == WFState.Runing)
            {
                GenerWorkerLists gwls = new GenerWorkerLists(gwf.WorkID, gwf.FK_Node);
                Node nd = new Node(gwf.FK_Node);

                str += "<li  class='cls highlight' style=\"background: url('../../Img/Action/Todolist.png') no-repeat 235px 31px\" >";

                str += "<BR><BR><p class='date'>流程运行到: " + gwf.NodeName + "。";

                if (gwf.TodoEmpsNum == 1)
                {
                    string myemp = gwf.TodoEmps;
                    myemp = myemp.Replace("(", "");
                    myemp = myemp.Replace(")", "");
                    string[] strs = myemp.Split(',');
                    str += "<br>" + BP.WF.Glo.GenerUserImgHtml(strs[0], strs[1]) + "。</p>";
                }
                else
                {
                 //   str += "<br>处理人员:（" + gwf.TodoEmps + "）计（" + gwf.TodoEmpsNum + "）个，。</p>";
                }

                str += "<div></div><br><br>";
                if (nd.HisNodeWorkType == NodeWorkType.WorkFL 
                    || nd.HisNodeWorkType == NodeWorkType.StartWorkFL
                    || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
                {
                    str += "<div><ul>";
                    foreach (GenerWorkerList item in gwls)
                    {
                        if (gwls.Count == 0
                            || nd.TodolistModel == TodolistModel.QiangBan
                            || nd.TodolistModel == TodolistModel.Sharing)
                        {
                            if (item.IsRead == false)
                                str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp; <font style='color:#FFFFFF; background-color:#00CC66' >未读</font></li>";
                            else
                                str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已读</strong></font></li>";
                            break;
                        }

                        switch (nd.TodolistModel)
                        {
                            case TodolistModel.QiangBan:
                                str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已读</strong></font></li>";
                                break;
                            case TodolistModel.Order:
                                if (item.IsPassInt == 1)
                                    str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已处理</strong></font></li>";
                                else
                                {
                                    if (item.IsRead == false)
                                        str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp; <font style='color:#FFFFFF; background-color:#00CC66' >未读</font></li>";
                                    else
                                        str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已读</strong></font></li>";
                                }
                                break;
                            case TodolistModel.Sharing:
                                str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已读</strong></font></li>";
                                break;
                            case TodolistModel.Teamup:
                                if (item.IsPassInt == 1)
                                    str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#999966' ><strong>已处理</strong></font></li>";
                                else
                                {
                                    if (item.IsRead == false)
                                        str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp; <font style='color:#FFFFFF; background-color:#00CC66' >未读</font></li>";
                                    else
                                        str += "<li><img src='../../Img/Dot.png' width='8px' >处理人:" + BP.WF.Glo.DealUserInfoShowModel(item.FK_Emp, item.FK_EmpText) + "&nbsp;&nbsp;<font style='color:#FFFFFF; background-color:#FF9966' ><strong>已读</strong></font></li>";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                str += "</ul>";
                str += "</div>";

                str += "<br>";
                str += "<br>";
                str += "<br>";
                str += "<br>";
                str += "<br>";
                str += "<br>";
                str += "<br>";

                str += "</li>";


                ///* 运行中的状态. */
                //str += "<li>";
                //str += "<table>";
                //str += "<caption>当前节点信息</caption>";
                //str += "<tr>";
                //str += "<th>停留节点</th>";
                //str += "<th>处理人</th>";
                //str += "<th>是否读取？</th>";
                //str += "</tr>";
                //foreach (GenerWorkerList item in gwls)
                //{
                //    str += "<tr>";
                //    str += "<td>" + item.FK_NodeText + "</td>";
                //    str += "<td>" + item.FK_EmpText + "</td>";
                //    if (item.IsRead)
                //        str += "<td>已读</td>";
                //    else
                //        str += "<td>未读</td>";
                //    str += "</tr>";
                //}
                //str += "</table>";
                //str += "</li>";
            }

            //结束循环.
            //this.AddTableEnd();
            #endregion

            if (this.CCID != null)
            {
                CCList cl = new CCList();
                cl.MyPK = this.CCID;
                cl.RetrieveFromDBSources();
                // this.AddFieldSet(cl.Title);
                // this.Add("抄送人:" + cl.Rec + ", 抄送日期:" + cl.RDT);
                // this.AddHR();
                // this.Add(cl.DocHtml);
                // this.AddFieldSetEnd();
                str += "<li  class='cls highlight' style=\"background: url('./Img/Action/circle.png') no-repeat 235px 31px\">";
                str += "<p class='date'>&nbsp;</p>";
                str += "<p class='intro'>" + cl.Title + "</p>";
                str += "<p class='version'>&nbsp;</p>";
                str += "<div class='more'>";
                str += "<p>抄送人:" + cl.Rec + "</p>";
                str += "<p>抄送日期:" + cl.RDT + "</p>";
                str += "</div>";
                str += "</li>";
                if (cl.HisSta == CCSta.UnRead)
                {
                    cl.HisSta = CCSta.Read;
                    cl.Update();
                }
            }

            str += "</ul>";
            str += "</div>";
            str += "</div>";
            str += "</div>";
            str += "</div>";
            str += "</div>";
            str += "</div>";
            //this.Clear();
            HiddenField1.Value = str.ToString();
        }

        #region 分流
        /// <summary>
        /// 分流支流
        /// </summary>
        /// <param name="fl"></param>
        public void BindBrach(Flow fl)
        {
            //  WorkFlow wf = new WorkFlow(fl, this.WorkID, this.FID);
            WorkNodes wns = new WorkNodes();
            wns.GenerByFID(fl, this.FID);

            this.AddH4("关于（" + fl.Name + "）工作报告");
            this.AddHR();

            Node nd = fl.HisStartNode;

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            this.Add("流程发起人：" + gwf.StarterName + "，发起日期：" + gwf.RDT + " ；流程状态：" + gwf.WFStateText);

            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.WorkID, this.WorkID);



            //  this.BindWorkNodes(wns, rws, fws);

            this.AddHR("流程报告结束");
        }
        #endregion 分流

        #region 合流
        /// <summary>
        /// 合流干流
        /// </summary>
        /// <param name="fl"></param>
        public void BindHeLiuRavie(Flow fl)
        {
        }
        /// <summary>
        /// 合流支流
        /// </summary>
        /// <param name="fl"></param>
        public void BindHeLiuBrach(Flow fl)
        {
        }
        #endregion 合流

        public void BindFHLWork(GenerFH hf)
        {
            this.AddH4(hf.Title);

            this.AddHR();
            this.AddFieldSet("当前节点基本信息");
            this.AddBR("接受时间：" + hf.RDT);
            this.AddBR("接受人：" + hf.ToEmpsMsg);
            this.AddFieldSetEndBR();

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FID, this.FID);

            this.AddFieldSet("分流人员信息");

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("标题");
            this.AddTDTitle("发起人");
            this.AddTDTitle("发起日期");
            this.AddTDTitle("");
            this.AddTREnd();

            foreach (GenerWorkFlow gwf in gwfs)
            {
                if (gwf.WorkID == this.FID)
                    continue;

                this.AddTR();
                this.AddTD(gwf.Title);
                this.AddTD(gwf.Starter);
                this.AddTD(gwf.RDT);
                this.AddTD("<a href='" + this.Request.ApplicationPath + "WF/WFRpt.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=" + gwf.FID + "' target=_b" + gwf.WorkID + ">工作报告</a>");
                this.AddTREnd();
            }
            this.AddTableEndWithBR();
            this.AddFieldSetEnd();
        }

        protected void AddContral(string desc, string text)
        {
            this.Add("<td  class='FDesc' nowrap> " + desc + "</td>");
            this.Add("<td width='40%' class=TD>");
            if (text == "")
                text = "&nbsp;";
            this.Add(text);
            this.AddTDEnd();
        }
        /// <summary>
        /// 增加一个工作
        /// </summary>
        /// <param name="en"></param>
        /// <param name="rws"></param>
        /// <param name="fws"></param>
        /// <param name="nodeId"></param>
        public void ADDWork(Work en, ReturnWorks rws, int nodeId)
        {
            this.BindViewEn(en, "class='Table' cellpadding='0' cellspacing='0' border='0' style='width: 100%'");
            foreach (ReturnWork rw in rws)
            {
                if (rw.ReturnToNode != nodeId)
                    continue;

                this.AddBR();

                AddEasyUiPanelInfo("退回信息", rw.NoteHtml);
            }

            //foreach (ShiftWork fw in fws)
            //{
            //    if (fw.FK_Node != nodeId)
            //        continue;
            //    this.AddBR();
            //    this.AddMsgOfInfo("转发信息：", fw.NoteHtml);
            //}


            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStrByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "WFRptDtl.aspx?RefPK=" + en.PKVal.ToString() + "&EnName=" + enDtl.Ens.GetNewEntity.ToString();
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a  href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    string sql = "SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                    int i = DBAccess.RunSQLReturnValInt(sql);

                    if (i == 0)
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入他门的相关功能
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					//int i=DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM "+vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable+" WHERE "+vsM.AttrOfMInMM+"='"+en.PKVal+"'");
            //					refstrs+="[<a href='"+url+"' target='_blank' >"+en1.Name+"</a>]";
            //					//refstrs+="编辑: <a href=\"javascript:window.open(RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+ens.ToString()+"'> )\" > "+en1.Name+"</a>";
            //					//var newWindow= window.open( this.Request.ApplicationPath+'/Comm/'+'RefFuncLink.aspx?RefFuncOID='+OID+'&MainEnsName='+ CurrEnsName +CurrKeys,'chosecol', 'width=100,top=400,left=400,height=50,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
            //					//refstrs+="编辑: <a href=\"javascript:EnsRefFunc('"+en1.OID.ToString()+"')\" > "+en1.Name+"</a>";
            //					//refstrs+="编辑:"+en1.Name+"javascript: EnsRefFunc('"+en1.OID.ToString()+"',)";
            //					//this.AddItem(en1.Name,"EnsRefFunc('"+en1.OID.ToString()+"')",en1.Icon);
            //				}
            //			}
            #endregion

            // 不知道为什么去掉。
            this.Add(refstrs);
        }

    }
}