<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmpWorks.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.EmpWorks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="jquery/lib/jquery/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/easyloader.js" type="text/javascript"></script>    
    <script src="js/AppData.js" type="text/javascript"></script>
    <script src="js/EmpWorks.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading">
    </div>
    <div data-options="region:'center'" border="false" style="margin: 0; padding: 0;
        overflow: hidden;">
        <table id="maingrid" fit="true" fitcolumns="true" class="easyui-datagrid">
        </table>
    </div>
</body>
</html>
