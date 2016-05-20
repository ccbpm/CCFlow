<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowSearch.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.FlowSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/WF/Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="/WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="/WF/Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="/WF/Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="js/AppData.js" type="text/javascript"></script>
    <script src="js/FlowSearch.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading">
    </div>
    <div data-options="region:'center'" border="false" style="margin: 0; padding: 0;">
        <div id="maingrid" fit="true" style="margin: 0; padding: 0;">
        </div>
        <div id="opengrid">
        </div>
    </div>
</body>
</html>
