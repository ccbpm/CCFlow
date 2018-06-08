<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WhoCanUseApp.aspx.cs" Inherits="CCGPM.GPM.WhoCanUseApp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script type="text/javascript" src="jquery/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="jquery/jquery.easyui.min.js"></script>
    <script src="jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script src="javascript/WhoCanUseApp.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading" style="filter: alpha(opacity=80); opacity:0.80;">
    </div>
    <div data-options="region:'west',split:true" style="width: 300px; padding: 1px; overflow: hidden;">
        <div style="width: 100%; height: 100%; overflow: auto;">
            <ul id="appTree" class="easyui-tree-line" data-options="animate:false,dnd:false">
            </ul>
        </div>
    </div>
    <div data-options="region:'center',split:true" style="overflow: hidden;">
        <div id="tb">
            <a href="#" id="addEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'" onclick="AddPersonForApp();">添加管理员</a>
            <a href="#" id="delEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-delete'" onclick="DeleteEmpApp()">删除管理员</a>
            <a href="#" id="A1" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-down'" onclick="LoadGridOrderBy('')">按系统排序</a>
            <a href="#" id="A2" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-down'" onclick="LoadGridOrderBy('dept')">按部门排序</a>
            <a href="#" id="A3" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-down'" onclick="LoadGridOrderBy('emp')">按姓名排序</a>
        </div>
        <table id="empAppGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid"></table>
    </div>
    <div id="deptEmpDialog">
        <div style="height: 400px; overflow: auto">
            <ul id="deptEmpTree" class="easyui-tree" data-options="animate:true,dnd:false">
            </ul>
        </div>
    </div>
</body>
</html>
