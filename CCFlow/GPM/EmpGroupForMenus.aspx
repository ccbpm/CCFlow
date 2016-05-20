<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmpGroupForMenus.aspx.cs" Inherits="GMP2.GPM.EmpGroupForMenus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>用户组菜单权限</title>
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css" />
    <link href="themes/default/tree.css" rel="stylesheet" type="text/css" />
    <link href="themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="themes/icon.css" />
    <script type="text/javascript" src="jquery/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="jquery/jquery.easyui.min.js"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script src="javascript/EmpGroupForMenus.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading" style="opacity:0.6; background-color:rgb(204, 204, 204);">
    </div>
    <div data-options="region:'north',border:false" style="height:40px;background:#E0ECFF;padding:10px">
        <div class="toolbar">
		    <div style="float:left;">查询内容：<input type="text" id="searchConten" name="searchConten" /></div>
		    <a href="#" style="float:left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'" onclick="QueryEmpGroups()">查询</a>
            <div class="datagrid-btn-separator"></div>
            <a href="#" id="saveRight" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save',disabled:true">保存</a>
            <a href="#" id="rightC" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'" onclick="CopyRight()">复制权限到其他组</a>
	    </div>
    </div>
    <div data-options="region:'west',split:true" title="权限组" style="width: 200px; padding1: 1px;
        overflow: hidden;">
        <div style="width: 100%; height: 100%; overflow: auto;">
             <select size="4" name="lbEmpGroup" id="lbEmpGroup" onchange="LoadMenusOnEmpChange()" style="height:100%;width:100%;">
            </select>
            <div id="mm1" class="easyui-menu" style="width: 150px;">
                <div id="rightCcontent" onclick="CopyRight()">复制权限到其他组</div>
            </div>
        </div>
        <div id="deptEmpDialog">
                <div style=" height:420px; overflow:auto">
                    <ul id="empGroupTree" class="easyui-tree" data-options="animate:true,dnd:false">
                    </ul>
                </div>
                <div style="font-weight:bold;background:#E0ECFF; height:40px;">
                    <span style="color:Red; font-weight:bold; margin-top:5px;">说明：
                    【清空式复制】首先将所选权限组权限清空，然后将所选权限组权限授权。<br />
                    【覆盖式复制】保留所选权限组之前的权限，将复制的权限递增给所选权限组。
                    </span>
                </div>
         </div>
         <span id="copyMsg" style=" color:Red; font-weight:bold; display:none"></span>
    </div>
    <div data-options="region:'center'" title="菜单" style="overflow: hidden;">
        <div style="overflow: auto; height: 100%;">
            <ul id="menuTree" class="easyui-tree" data-options="animate:false,dnd:false">
            </ul>
        </div>
    </div>
</body>
</html>
