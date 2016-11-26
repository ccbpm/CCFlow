<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartTree.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.StartTree" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="js/AppData.js" type="text/javascript"></script>
    <script src="js/startPageTree.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading">        
    </div>
    <div data-options="region:'center'" style="margin: 0; padding: 0;">
        <table id="maingrid" fit="true" fitcolumns="true" class="easyui-datagrid"></table>
    </div>
    <div id="showHistory" style="margin: 0;padding:0;">
        <div id="historyGrid"  fit="true" fitcolumns="true"></div>
    </div>
    <div id="divTitle" style="margin: 0; padding: 0;">
        <div id="panel" style=" margin-top:30px; padding: 0;">
            标题名称：<input type="text" id="TB_Title" name="TB_Title" style=" height:25px; width:320px;" />
        </div>
    </div>
    <div id="flowPicDiv">
        <img alt="流程图" id="FlowPic" src="" onerror="this.src='/DataUser/ICON/CCFlow/LogBig.png'" />
    </div>
</body>
</html>
