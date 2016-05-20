<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tree.aspx.cs" Inherits="CCOA.Comm.Tree" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/EasyUI/themes/default/tree.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/EasyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="JS/Tree.js" type="text/javascript"></script>
</head>

<body class="easyui-layout">
    <div id="pageloading">
    正在加载，请稍后......
    </div>
    <div data-options="region:'center'" title="<%=EnsDesc %>" style="overflow: hidden;">
    <div style="width: 100%; height: 100%; overflow: auto;">
        <div style="padding: 0px; background: #fafafa; width: 100%;">
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-new'" onclick="CreateSampleNode();">新建同级</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-new'" onclick="CreateSubNode();">新建下级</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-edit'" onclick="EditNode();">修改</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'" onclick="DeleteNode();">删除</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-up'" onclick="DoUp();">上移</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-down'" onclick="DoDown();">下移</a>
        </div>
        <ul id="enTree" class="easyui-tree-line" data-options="animate:false,dnd:false"  style="width:400px;">
        </ul>
        <div id="treeMM" class="easyui-menu" style="width: 120px;">
            <div data-options="iconCls:'icon-new'" onclick="CreateSampleNode();">
                新建同级</div>
            <div data-options="iconCls:'icon-new'" onclick="CreateSubNode();">
                新建下级</div>
            <div data-options="iconCls:'icon-edit'" onclick="EditNode();">
                修改</div>
            <div data-options="iconCls:'icon-cancel'" onclick="DeleteNode();">
                删除</div>
            <div class="menu-sep">
            </div>
            <div data-options="iconCls:'icon-up'" onclick="DoUp();">
                上移</div>
            <div data-options="iconCls:'icon-down',disabled:false" onclick="DoDown();">
                下移</div>
        </div>
        <input type="hidden" id="enName" value="<%= EnName%>" />
    </div>
    </div>
    <div id="treeDialog" data-options="iconCls:'icon-save'">
        <div style="height: 30px; overflow: auto; margin: 10px;">
            节点名称:<input type="text" name="nodeName" id="nodeName" />
        </div>
    </div>
</body>
</html>
