<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Toolbar.ascx.cs" Inherits="CCFlow.WF.App.Comm.Toolbar" %>
<%@ Import Namespace="BP.En" %>
<%@ Import Namespace="BP.Web" %>
<%@ Import Namespace="BP.WF" %>
<%@ Import Namespace="BP.WF.Template" %>
<%@ Import Namespace="BP.WF.Data" %>

<%@ Register Src="../UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Import Namespace="BP.WF" %>

<%
    int fk_node = int.Parse(this.Request.QueryString["FK_Node"]);
    Int64 workID = Int64.Parse(this.Request.QueryString["WorkID"]);
    Int64 fid = Int64.Parse(this.Request.QueryString["FID"]);
    string fk_flow = this.Request.QueryString["FK_Flow"];
    this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "SDKData", "" + Glo.CCFlowAppPath + "SDKComponents/Base/SDKData.js");
    string isToolbar = Request.QueryString["IsToobar"];
    bool toolbar = true;
    if (!string.IsNullOrEmpty(isToolbar) && isToolbar == "0")
        toolbar = false;
         
    string paras = this.Request.QueryString["AtPara"];

    bool isCC = false;
    if (paras != null && paras.Contains("IsCC=1"))
        isCC = true;

    paras = this.Request.QueryString["Paras"];
    if (paras != null && paras.Contains("IsCC=1"))
        isCC = true;

    BP.WF.Template.BtnLab btn = new BP.WF.Template.BtnLab(fk_node);
    BP.WF.Node node = new Node(fk_node);

    WFState workState = WFState.Runing;
    string appPath = Glo.CCFlowAppPath; //this.Request.ApplicationPath;
    string msg = null;
    bool isInfo = false;
    try
    {
        WorkFlow workFlow = new WorkFlow(node.FK_Flow, workID);
        workState = workFlow.HisGenerWorkFlow.WFState;
        if (workState != WFState.Complete)
        {
            switch (workFlow.HisGenerWorkFlow.WFState)
            {
                case WFState.AskForReplay: // 返回加签的信息.
                    string mysql = "SELECT * FROM ND" + int.Parse(node.FK_Flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;
                    System.Data.DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(mysql);
                    foreach (System.Data.DataRow dr in mydt.Rows)
                    {
                        string msgAskFor = dr[TrackAttr.Msg].ToString();
                        string worker = dr[TrackAttr.EmpFrom].ToString();
                        string workerName = dr[TrackAttr.EmpFromT].ToString();
                        string rdt = dr[TrackAttr.RDT].ToString();

                        //提示信息.
                        this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "回复信息:",
                            BP.DA.DataType.ParseText2Html(msgAskFor) + "<br>" + rdt);
                        isInfo = true;
                    }
                    break;
                case WFState.Askfor: //加签.
                    string sql = "SELECT * FROM ND" + int.Parse(node.FK_Flow) + "Track WHERE WorkID=" + workID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        string msgAskFor = dr[TrackAttr.Msg].ToString();
                        string worker = dr[TrackAttr.EmpFrom].ToString();
                        string workerName = dr[TrackAttr.EmpFromT].ToString();
                        string rdt = dr[TrackAttr.RDT].ToString();

                        //提示信息.
                        this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "请求加签:",
                             BP.DA.DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + " --<a href='./WorkOpt/AskForRe.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workID + "&FID=" + fid + "' >回复加签意见</a> --");
                        isInfo = true;

                    }

                    break;
                case WFState.ReturnSta:
                    /* 如果工作节点退回了*/
                    ReturnWorks rws = new ReturnWorks();
                    rws.Retrieve(ReturnWorkAttr.ReturnToNode, fk_node,
                        ReturnWorkAttr.WorkID, workID,
                        ReturnWorkAttr.RDT);
                    if (rws.Count != 0)
                    {
                        string msgInfo = "";
                        foreach (BP.WF.ReturnWork rw in rws)
                        {
                            msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='" + appPath + "DataUser/ReturnLog/" + fk_flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                            msgInfo += rw.NoteHtml;
                            msgInfo += "</fieldset>";
                        }

                        msgInfo += "<br>" + node.ReturnAlert;
                        
                        this.FlowMsg.AlertMsg_Info("流程退回提示", msgInfo);
                        isInfo = true;

                        //gwf.WFState = WFState.Runing;ruhe huoqu yige div
                        //gwf.DirectUpdate();
                    }
                    break;
                case WFState.Shift:
                    /* 判断移交过来的。 */
                    ShiftWorks fws = new ShiftWorks();
                    BP.En.QueryObject qo = new QueryObject(fws);
                    qo.AddWhere(ShiftWorkAttr.WorkID, workID);
                    qo.addAnd();
                    qo.AddWhere(ShiftWorkAttr.FK_Node, fk_node);
                    qo.addOrderBy(ShiftWorkAttr.RDT);
                    qo.DoQuery();
                    if (fws.Count >= 1)
                    {
                        this.FlowMsg.AddFieldSet("移交历史信息");
                        foreach (ShiftWork fw in fws)
                        {
                            msg = "@移交人[" + fw.FK_Emp + "," + fw.FK_EmpName + "]。@接受人：" + fw.ToEmp + "," + fw.ToEmpName + "。<br>移交原因@" + fw.NoteHtml;
                            if (fw.FK_Emp == WebUser.No)
                                msg = "<b>" + msg + "</b>";

                            msg = msg.Replace("@", "<br>@");
                            this.FlowMsg.Add(msg + "<hr>");
                        }
                        this.FlowMsg.AddFieldSetEnd();
                    }
                    workFlow.HisGenerWorkFlow.WFState = WFState.Runing;
                    workFlow.HisGenerWorkFlow.DirectUpdate();
                    isInfo = true;

                    break;
                default:
                    break;
            }

            bool isCanDo = workFlow.IsCanDoCurrentWork(BP.Web.WebUser.No);
            if (!isCanDo)
                workState = WFState.Complete;
        }
    }
    catch (Exception)
    {
        try
        {
            Flow fl = new Flow(fk_flow);
            GERpt rpt = fl.HisGERpt;
            rpt.OID = workID;
            rpt.Retrieve();

            if (rpt != null)
            {
                workState = rpt.WFState;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
%>
<script type="text/javascript" language="javascript" src="/DataUser/PrintTools/LodopFuncs.js"></script>
<script type="text/javascript">
    function ShowUrl(obj) {

            var strTimeKey = "";
                    var date = new Date();
                    strTimeKey += date.getFullYear(); //年
                    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
                    strTimeKey += date.getDate(); //日
                    strTimeKey += date.getHours(); //HH
                    strTimeKey += date.getMinutes(); //MM
                    strTimeKey += date.getSeconds(); //SS
        
        var btnID = obj.id;

         if (btnID == 'Btn_Return') {
             //OpenUrl('ReturnMsg', '退回', '/WF/WorkOpt/ReturnWork.aspx?1=2', 500, 350);
             OpenUrlLocation('/WF/WorkOpt/ReturnWork.aspx?1=2');
             return;
         }

         if (btnID == 'Btn_Track') {
             OpenUrl('TrackMsg', '流程步骤', '/WF/WorkOpt/OneWork/Track.aspx?1=2', 800, 500);
             return;
         }

         if (btnID == 'Btn_SelectAccepter') {
             OpenUrl('SelectAccepterMsg', '接受人', '/WF/WorkOpt/Accepter.aspx?1=2', 600, 450);
             
             return;
         }

         if (btnID == 'Btn_Askfor') {
            // OpenUrl('AskforMsg', '加签', '/WF/WorkOpt/AskFor.aspx?1=2', 600, 400);
             OpenUrlLocation( '/WF/WorkOpt/AskFor.aspx?1=2');
             return;
         }

         if (btnID == 'Btn_Shift') {
             //OpenUrl('ShiftMsg', '移交', '/WF/WorkOpt/Forward.aspx?1=2', 600, 450);
             OpenUrlLocation('/WF/WorkOpt/Forward.aspx?1=2');
             return;
         }

         if (btnID == 'Btn_CC') {
             OpenUrl('CCMsg', '抄送', '/WF/WorkOpt/CC.aspx?1=2', 700, 450);
             return;
         }

         if (btnID == 'Btn_Delete') {
         if (confirm("是否真的需要删除?"))
              OpenUrlLocation('/WF/DeleteWorkFlow.aspx?1=2');
             return;
         }

     if (btnID == 'Btn_CheckNote') {
            // OpenUrl('CCMsg', '审核', '/WF/WorkOpt/CCCheckNote.aspx?1=2', 700, 450);
         OpenUrlLocation('/WF/WorkOpt/CCCheckNote.aspx?1=2');
             return;
         }

         if (btnID == 'Btn_Office') {
             WinOpen( '/WF/WorkOpt/WebOffice.aspx?1=2', '公文正文', 700, 450);
             return;
         }

         if (btnID == 'Btn_Read') {
            Application.data.ReadCC("<%=node.NodeID %>", "<%=workID %>", ReadCCResult, this);
         }

         if (btnID == 'Btn_Print') {
             printFrom();
             return;
         }

//        if (btnID == 'Btn_Delete') {
//             if (confirm("是否真的需要删除?"))
//                        DelCase();
//            return;
//        }
    }
    function DeleteResult(json) {
        
    }
    function ReadCCResult(json) {
        if (json != "true") {
            alert('已阅失败!');
        } 
    }

    var LODOP;
     function printFrom() {
        LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
        LODOP.PRINT_INIT("打印表单");
     //    LODOP.ADD_PRINT_URL(30, 20, 746, "100%", location.href);
        LODOP.ADD_PRINT_HTM(0, 0, "100%", "100%", document.getElementById("divCCForm").innerHTML);
        // LODOP.ADD_PRINT_URL(0, 0, "100%", "100%", url);
        LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
        LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
        //		LODOP.SET_SHOW_MODE("MESSAGE_GETING_URL",""); //该语句隐藏进度条或修改提示信息
        //		LODOP.SET_SHOW_MODE("MESSAGE_PARSING_URL","");//该语句隐藏进度条或修改提示信息
        //  LODOP.PREVIEW();

        LODOP.PREVIEW();


    }

    function ShowFlowMessage() {
          $('#flowMessage').window(
        {
            closeable: false,
            title: "信息日志",
            modal: true,
            width: 800,
            height: 400,
            buttons: [{ text: '关闭', handler: function () {
                $('#flowMessage').dialog("close");
            }
            }]
        }
        );
    }
      function WinOpen(url, winName,width,height) {

       // 生成参数.
        _GetParas();

        // 把IsEUI处理一下，让对方的功能界面接收到此参数进行个性化处理.
        url = url + _paras + '&IsEUI=1';
        var newWindow = window.open(url, winName, 'width='+width+',height='+height+',top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }

      function DelCase() {
            var strTimeKey = "";
            var date = new Date();
            strTimeKey += date.getFullYear(); //年
            strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
            strTimeKey += date.getDate(); //日
            strTimeKey += date.getHours(); //HH
            strTimeKey += date.getMinutes(); //MM
            strTimeKey += date.getSeconds(); //SS

            Application.data.delcase(<%=fk_flow %>, <%=fk_node %>, <%=workID %>, "", function (js) {
                if (js) {
                    var str = js;
                    if (str == "删除成功") {
                        $.messager.alert('提示', '删除成功！');
                    }
                    else {
                        $.messager.alert('提示', str);
                    }
                }
            }, this);
        }
        function OpenUrlLocation(url) {
              _GetParas();
              url = url + _paras + '&IsEUI=1';
                window.name = "dialogPage";
            window.open(url, "dialogPage");
//            window.location.href = url;
        }
    function OpenUrl(divID, title, url, w, h) {

    // 生成参数.
        _GetParas();
              url = url + _paras + '&IsEUI=1';
        <% if (BP.Sys.SystemConfig.AppSettings["SDKWinOpenType"] == "1")
           { %>
        // 把IsEUI处理一下，让对方的功能界面接收到此参数进行个性化处理.
        try {
         window.parent.OpenJboxIfream(title,url,w,h);
        } catch (e) {
            OpenWindow(url,title,w,h);
        }
       
<% }
           else
           { %>
        OpenWindow(url,title,w,h);
<% } %>
    }

    $(function() {
        var html = $('#flowMessage').text();
        if (html != "" &&  html != null && html.length>6) {
            ShowFlowMessage();
        }
    });

    function OpenWindow(url,title,w,h) {
        $('#windowIfrem').window(
        {
              content: ' <iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>',
            closeable: false,
            title: title,
            modal: true,
            width: w,
            height: h,
            buttons: [{ text: '关闭', handler: function () {
                $('#windowIfrem').dialog("close");
            }
            }]
        }
        );
    }

    var _paras = "";
    function _GetParas() {
        _paras = "";
        //获取其他参数
        var sHref = window.location.href;
        var args = sHref.split("?");
        var retval = "";
        if (args[0] != sHref) /*参数不为空*/
        {
            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1)
                    continue;

                //不包含就添加
                if (_paras.indexOf(arg[0]) == -1) {
                    _paras += "&" + arg[0] + "=" + arg[1];
                }
            }
        }
    }

    function closeWin() {
        if (window.dialogArguments && window.dialogArguments.window) {
            window.dialogArguments.window.location = window.dialogArguments.window.location;
        }
        if (window.opener) {
            if (window.opener.name && window.opener.name == "main") {
                window.opener.location.href = window.opener.location.href;
                window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
            }
        }
        window.close();
    }
</script>
<div id="ReturnMsg">
</div>
<div id="SelectAccepterMsg">
</div>
<div id="AskforMsg">
</div>
<div id="ShiftMsg">
</div>
<div id="CCMsg">
</div>
<div id="TrackMsg">
</div>
<div id="msgPanel">
</div>
<div id="windowIfrem">
</div>
<div id="flowMessage">
    <uc1:Pub ID="FlowMsg" runat="server" />
</div>
<% if (toolbar)
   { %>
<% if (isCC == false && workState != WFState.Complete)
   { %>
<% if (workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<!--发送-->
<input type="button" onclick="Send()" id="Btn_Send" value='<%= btn.SendLab %>' />
<% } %>
<!-- 保存-->
<% if (btn.SaveEnable == true && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="Save()" id="Btn_Save" value='<%= btn.SaveLab %>' />
<% } %>
<!-- 退回-->
<% if (btn.ReturnEnable == true && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Return" name="Btn_Return" value='<%= btn.ReturnLab %>' />
<% } %>
<!-- 接受人-->
<% if (btn.SelectAccepterEnable != 0 && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_SelectAccepter" value='<%= btn.SelectAccepterLab %>' />
<% } %>
<!-- 移交-->
<% if (btn.ShiftEnable == true && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Shift" value='<%= btn.ShiftLab %>' />
<% } %>
<!-- 删除-->
<% if (btn.DeleteEnable != 0 && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Delete" value='<%= btn.DeleteLab %>' />
<% } %>
<!-- 加签-->
<% if (btn.AskforEnable == true && workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Askfor" value='<%= btn.AskforLab %>' />
<% } %>
<% }
   else
   {
       /* 如果是抄送. */ %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Read" value="已阅" />
<%
    FrmWorkCheck fwc = new FrmWorkCheck(fk_node);

    if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
    {
        /*如果没有启用,就显示出来可以审核的窗口. */
        string url = "";
%>
<input type="button" value='填写审核意见' id="Btn_CheckNote" onclick="ShowUrl(this)" />
<%
    }
   } %>
<!-- 抄送-->
<% if (btn.CCRole == BP.WF.CCRole.HandAndAuto || btn.CCRole == BP.WF.CCRole.HandCC)
   { %>
<% if (workState != WFState.Complete && workState != WFState.Fix && workState != WFState.HungUp)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_CC" value='<%= btn.CCLab %>' />
<% } %>
<% } %>
<!-- 轨迹-->
<% if (btn.TrackEnable)
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Track" value='<%= btn.TrackLab %>' />
<% } %>
<% } %>
<!-- 打印-->
<% if (btn.PrintDocEnable)
   { %>
<% if (node.HisPrintDocEnable == PrintDocEnable.PrintHtml)
   { %>
<object id="LODOP_OB" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0"
    height="0">
    <embed id="LODOP_EM" type="application/x-print-lodop" width="0" height="0" pluginspage="/DataUser/PrintTools/install_lodop32.exe"></embed>
</object>
<input type="button" onclick="ShowUrl(this)" id="Btn_Print" value='<%= btn.PrintDocLab %>' />
<% } %>
<% } %>

<!-- 公文-->
<% if (btn.WebOfficeWorkModel == WebOfficeWorkModel.Button )
   { %>
<input type="button" onclick="ShowUrl(this)" id="Btn_Office" value='<%= btn.WebOfficeLab %>' />
<% } %>
<% if (isInfo)
   { %>
<input type="button" onclick="ShowFlowMessage()" id="Btn_flowInfo" value='信息日志' />
<% } %>
