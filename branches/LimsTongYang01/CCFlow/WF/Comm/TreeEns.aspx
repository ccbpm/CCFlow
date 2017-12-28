<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TreeEns.aspx.cs" Inherits="CCOA.Comm.TreeEns" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Scripts/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/EasyUI/themes/default/tree.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/EasyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUI/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="JS/TreeEns.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <div id="pageloading">
        正在加载，请稍后......
    </div>
    <div region="west" border="true" split="true" title="<%=TreeEnsDesc %>" style="width: 310px;
        padding: 0;">
        <ul id="enTree" class="easyui-tree-line" data-options="animate:false,dnd:false" style="width: 300px;">
        </ul>
        <input type="hidden" id="treeNnName" value="<%= TreeEnName%>" />
    </div>
    <div data-options="region:'center'" style="padding: 0px;" border="false" title="<%=EnsDesc %>" style="overflow: hidden;">
        <table id="ensGrid" fit="true" class="easyui-datagrid">
        </table>
        <input type="hidden" id="enName" value="<%= EnName%>" />
        <input type="hidden" id="enPK" value="<%= EnPK%>" />
    </div>
</body>
</html>
