<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOfTimeline.aspx.cs" Inherits="CCFlow.WF.WorkOpt.OneWork.TruckUI" %>

<%@ Register Src="TrackUC.ascx" TagName="TrackUC" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程日志</title>
    <script src="../../Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
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
    <div id="flowNote" style="padding-left: 20%;vertical-align: top;">
        <uc1:TrackUC ID="TruakUC1" runat="server" />
    </div>
    </form>
</body>
</html>
