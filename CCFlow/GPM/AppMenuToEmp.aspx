<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppMenuToEmp.aspx.cs" Inherits="GMP2.GPM.AppMenuToEmp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>菜单管理</title>
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link href="themes/default/tree.css" rel="stylesheet" type="text/css" />
    <link href="themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script type="text/javascript" src="jquery/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="jquery/jquery.easyui.min.js"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="jquery/easytemplate.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script src="javascript/AppMenuToEmp.js" type="text/javascript"></script>
    <style type="text/css">
        #templatePanel
        {
            height: 280px;
            margin: 20px 20px 20px 20px;
            margin-bottom: 40px;
        }
        #ckbAll
        {
            margin-left:20px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div id="pageloading" style="filter: alpha(opacity=80); opacity:0.80;">
    </div>
    <div data-options="region:'west',split:true" style="width: 300px; padding: 1px; overflow: hidden;">
        <div style="width: 100%; height: 100%; overflow: auto;">
            <ul id="menuTree" class="easyui-tree-line" data-options="animate:false,dnd:false">
            </ul>
        </div>
    </div>
    <div data-options="region:'center'" style="overflow: hidden;">
        <div style="padding: 0px; background: #fafafa; width: 100%;">
            <a href="#" class="easyui-linkbutton" data-options="plain:true"><b><span id="curModelText" style="color: Red;">当前选择按人员分配权限</span></b></a>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-reset'" onclick="CheckedAll()"><span id="ckbAllText">全选</span></a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'" onclick="SaveMenuForEmp();">保存</a>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-rights'" onclick="DistributeRight('emp')">按人员分配权限</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-rights'" onclick="DistributeRight('station')">按岗位分配权限</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-rights'" onclick="DistributeRight('group')">按权限组分配权限</a>             
            
            
        </div>
        <div style="overflow: auto; height: 95%;">
            <%-- 下列为easytemplate的控件--%>
            <div id="templatePanel">
            </div>
            <br />
        </div>
    </div>
</body>
</html>
