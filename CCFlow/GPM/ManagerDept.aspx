<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagerDept.aspx.cs" Inherits="GMP2.GPM.ManagerDept" %>

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
    <script src="javascript/ManagerDept.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading" style="filter: alpha(opacity=80); opacity: 0.80;">
    </div>
    <div data-options="region:'west',split:true" style="width: 300px; padding: 1px; overflow: hidden;">
        <div style="width: 100%; height: 100%; overflow: auto;">
            <ul id="appTree" class="easyui-tree-line" data-options="animate:false,dnd:false">
            </ul>
        </div>
    </div>
    <div data-options="region:'center',split:true" style="overflow: hidden;">
        <div id="tb">
            <a href="#" id="addEmpApp" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-addG'"
                onclick="AddPersonForApp();">添加管理员</a> <a href="#" id="delEmpApp" class="easyui-linkbutton"
                    data-options="plain:true,iconCls:'icon-deleteG'" onclick="DeleteEmpApp()">删除管理员</a>
            <input type="checkbox" id="orderByDept" onchange="switchRadio('dept')" checked style="vertical-align: middle">按部门排序
            <input type="checkbox" id="orderByEmp" onchange="switchRadio('emp')" style="vertical-align: middle">按姓名排序
        </div>
        <table id="empAppGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    <div id="deptEmpDialog">
        <div style="height: 400px; overflow: auto">
            <ul id="deptEmpTree" class="easyui-tree" data-options="animate:true,dnd:false">
            </ul>
        </div>
    </div>
     <div id="mm" class="easyui-menu" style="width: 120px;">
        <div data-options="iconCls:'icon-new'" onclick="checkDeptInfo()">
            编辑部门</div>
        <div data-options="iconCls:'icon-add'" onclick="append('peer')">
            新建同级部门</div>
        <div data-options="iconCls:'icon-add'" onclick="append('son')">
            新建子级部门</div>
        <div data-options="iconCls:'icon-up'" onclick="upOdownNode('up')">
            上移</div>
        <div data-options="iconCls:'icon-down'" onclick="upOdownNode('down')">
            下移</div>
        <div data-options="iconCls:'icon-sum'" onclick="glry()">
            关联人员</div>
        <div data-options="iconCls:'icon-delete'" onclick="deleteNode()">
            删除</div>
        <div class="menu-sep">
        </div>
        <div data-options="iconCls:'icon-reload'" onclick="refresh()">
            刷新</div>
    </div>
</body>
</html>
