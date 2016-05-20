<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchEUI.aspx.cs" Inherits="CCFlow.WF.Comm.SearchEUI" %>
<%@ Register Src="UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="JS/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="JS/EasyUI/themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link href="JS/EasyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="JS/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="JS/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="JS/EasyUI/locale/easyui-lang-zh_CN.js" type="text/javascript" charset="UTF-8"></script>
    <script src="JS/SearchEasyUI.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <form id="Form1" method="post" runat="server">
    <div id="pageloading">
    </div>
    <div data-options="region:'center'" style="padding: 0px;" border="false"
        style="overflow: hidden;">
        <div id="tb" style="padding: 3px;">
            <div id="Div1" runat="server" style="height:30px;">
                <div style="float:left;">
                <uc2:ToolBar ID="ToolBar1" runat="server" Text="df" />
                <a id="querybtn" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'" runat="server" onserverclick="ToolBar1_ButtonClick">查询</a>
                </div>
                <div class="datagrid-btn-separator"></div>
                <a id="newWin" href="#" style="float:left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-new'"
                    onclick="CreateEntityForm()">新建</a> 
                <a id="editWin" href="#" style="float:left;" class="easyui-linkbutton"
                        data-options="plain:true,iconCls:'icon-config'" onclick="EditEntityForm()">修改</a>
                <a id="delSelected" href="#" style="float:left;" class="easyui-linkbutton"
                        data-options="plain:true,iconCls:'icon-delete'" onclick="DelSelected()">删除</a>
            </div>
        </div>
        <table id="ensGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid">
        </table>
        <input type="hidden" id="enName" value="<%= EnName%>" />
        <input type="hidden" id="enPK" value="<%= EnPK%>" />
    </div>
    </form>
</body>
</html>
