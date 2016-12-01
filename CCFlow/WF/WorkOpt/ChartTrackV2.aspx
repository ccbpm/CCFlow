<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChartTrackV2.aspx.cs" Inherits="CCFlow.WF.WorkOpt.ChartTrackV2" %>

<%@ Register Src="~/WF/WorkOpt/OneWork/TrackUC.ascx" TagName="TrackUC" TagPrefix="uc1" %>
<%@ Register Src="~/WF/UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="OneWork/css/style.css" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $("table.Table tr:gt(0)").hover(
                function () { $(this).addClass("tr_hover"); },
                function () { $(this).removeClass("tr_hover"); });
        });

        function WinOpen(url, winName) {
            var newWindow = window.open(url, winName, 'height=800,width=1030,top=' + (window.screen.availHeight - 800) / 2 + ',left=' + (window.screen.availWidth - 1030) / 2 + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
            newWindow.focus();
            return;
        }
    </script>
    <script type="text/javascript" language="javascript">

        function WinOp(url) {
            var winWidth = 850;
            var winHeight = 680;
            window.open(url, "_blank", "height=" + winHeight + ",width=" + winWidth + ",top=0,left=0,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no");
        }


        function getArgsFromHref(sArgName) {
            var sHref = window.location.href;
            var args = sHref.split("?");
            var retval = "";
            if (args[0] == sHref) /*参数为空*/
            {
                return retval; /*无需做任何处理*/
            }
            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1) continue;
                if (arg[0] == sArgName) retval = arg[1];
            }
            return retval;
        }

        $(function () {
            var fid = getArgsFromHref("FID");
            var fk_flow = getArgsFromHref("FK_Flow");
            var workId = getArgsFromHref("WorkID");
            var iframe = document.getElementById("content");
            //iframe.src = "ChartTrack.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId;
            iframe.src = "../Admin/CCBPMDesigner/truck/Truck.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId;
        });
        //切换样式  图表/div隐藏
        function toggleStyle(obj) {
            var iconId = obj.id;
            $('#' + iconId).toggleClass("changeIcon");

            iconId = iconId.replace("icon_", "");
            var contentId = "content_" + iconId;

            $('#' + contentId).toggleClass("displaynone");
        }
    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .divpanel
        {
            width: 100%;
            min-width: 1000px;
            height: 27px;
            margin-top: 5px;
            float: left;
            border-left: 2px solid #95B8E7;
            border-right: 2px solid #95B8E7;
            border-top: 2px solid #95B8E7;
        }
        .panelContent
        {
            width: 100%;
            min-width: 1000px;
            float: left;
            height: auto;
            border-left: 2px solid #95B8E7;
            border-right: 2px solid #95B8E7;
            border-bottom: 2px solid #95B8E7;
            float: left;
        }
        .paneltop
        {
            width: 100%;
            float: left;
            height: 27px;
            background-color: #E6EFFF;
        }
        .panelicon
        {
            height: 16px;
            width: 16px;
            float: right;
            margin-top: 4px;
            margin-right: 4px;
            background: rgba(0, 0, 0, 0) url("../../WF/Scripts/easyUI/themes/default/images/panel_tools.png") no-repeat scroll -32px 0;
        }
        .panelicon:hover
        {
            cursor: pointer;
        }
        .panelTitle
        {
            color: #0e2d5f;
            font-size: 12px;
            font-weight: bold;
            line-height: 23px;
            height: 23px;
            float: left;
            padding-left: 8px;
        }
        #content_1
        {
            height: 550px;
            padding-top: 5px;
            overflow: auto;
        }
        #content_2
        {
            height: 550px;
            padding-top: 5px;
            overflow: auto;
        }
        #content_3
        {
            height: 220px;
            padding-top: 5px;
        }
        
        .displaynone
        {
            display: none;
        }
        .changeIcon
        {
            background: rgba(0, 0, 0, 0) url("../../WF/Scripts/easyUI/themes/default/images/panel_tools.png") no-repeat scroll -32px -16px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'center'" style="padding-left: 10px; padding-right: 10px;">
        <form id="form1" runat="server">
        <hr />
        <div>
            操作：

            
            <asp:LinkButton runat="server" ID="LinkButton1" class="easyui-linkbutton" data-options="iconCls:'icon-reset '"
                OnClick="LinkButton1_Click">撤销</asp:LinkButton>
            <a id="A1" href="javascript:WinOp('../WorkOpt/Press.htm?FID=<%=FID%>&WorkID=<%=WorkID%>&FK_Flow=<%=FK_Flow%>')"
                class="easyui-linkbutton" data-options="iconCls:'icon-tip'">催办</a> 
                <% if (BP.Web.WebUser.No == "admin")
                   { %>
                <a id="A3" href="javascript:WinOp('../App/EasyUI/FlowShift.aspx?FK_Flow=<%=FK_Flow %>&WorkID=<%=WorkID %>&FK_Node=<%=FK_Node %>&FID=<%=FID %> ')"
                    class="easyui-linkbutton" data-options="iconCls:'icon-redo'">调度</a>
                <a id="A2" href="javascript:WinOp('../App/EasyUI/FlowSkip.aspx?FK_Flow=<%=FK_Flow %>&WorkID=<%=WorkID %> ')"
                    class="easyui-linkbutton" data-options="iconCls:'icon-undo'">跳转</a>
                     <a id="A4" href="javascript:WinOp('../Comm/RefMethod.aspx?Index=12&EnsName=BP.WF.Template.FlowSheets&No=<%=FK_Flow %>')"class="easyui-linkbutton" data-options="iconCls:'icon-reload'">回滚</a>
                     <asp:LinkButton runat="server" ID="LogOut" class="easyui-linkbutton" data-options="iconCls:'icon-delete'"
                OnClick="LogOut_Click">删除</asp:LinkButton>
                <%} %>
               
        </div>
        <hr />
        <div id="panel_1" class="divpanel">
            <div class="paneltop">
                <div class="panelTitle">
                    轨迹图
                </div>
                <div id="icon_1" class="panelicon" onclick="toggleStyle(this);">
                </div>
            </div>
        </div>
        <div id="content_1" class="panelContent">
            <iframe id="content" frameborder="0" scrolling="no" style="width: 100%; height: 100%">
            </iframe>
        </div>
        <div id="panel_2" class="divpanel">
            <div class="paneltop">
                <div class="panelTitle">
                    流程日志
                </div>
                <div id="icon_2" class="panelicon" onclick="toggleStyle(this);">
                </div>
            </div>
        </div>
        <div id="content_2" class="panelContent">
            <div data-options="region:'center',fit:true" id="flowNote" style="padding-left: 20%;
                vertical-align: top;">
                <uc1:TrackUC ID="TruakUC1" runat="server" />
            </div>
        </div>
        <div id="panel_3" class="divpanel">
            <div class="paneltop">
                <div class="panelTitle">
                    流程附件
                </div>
                <div id="icon_3" class="panelicon" onclick="toggleStyle(this);">
                </div>
            </div>
        </div>
        <div id="content_3" class="panelContent">
            <div data-options="region:'center'" style="padding: 5px">
                <uc1:Pub ID="Pub1" runat="server" />
            </div>
        </div>
        </form>
    </div>
</body>
</html>
