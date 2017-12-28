<%@ Page Title="流程日志" Language="C#" AutoEventWireup="true" CodeBehind="TruckOnly.aspx.cs"
    Inherits="CCFlow.WF.WorkOpt.OneWork.TruckOnly" %>

<%@ Register Src="TrackUC.ascx" TagName="TruakUC" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
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
    <base target="_self" />
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',noheader:true">
        <uc1:TruakUC ID="TruakUC1" runat="server" />
    </div>
    </form>
</body>
</html>
