<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppList.aspx.cs" Inherits="GMP2.GPM.AppList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>系统管理</title>
    <link id="appstyle" href="themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="themes/icon.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jquery/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="jquery/jquery.easyui.min.js"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="javascript/AppList.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="mainPanle" region="center" border="false" style="margin:0px;">
        <table id="appGrid"  fit="true" class="easyui-datagrid">
        </table>
    </div>
</body>
</html>
