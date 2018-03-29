<%@ Page Title="" Language="C#" MasterPageFile="SiteEUI.Master" AutoEventWireup="true"
    CodeBehind="Search.htm.cs" Inherits="CCFlow.WF.Rpt.SearchEUI" %>

<%@ Register Src="UC/ToolBar.ascx" TagName="ToolBar" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/FrmReportField.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="pageloading">
    </div>
    <div region="center" border="true" style="margin: 0; padding: 0; overflow: hidden;">
        <div id="tb" style="padding: 3px;">
            <div id="Div1" runat="server" style="height: 30px;">
                <div style="float: left;">
                    <uc2:ToolBar ID="ToolBar1" runat="server" Text="df" />
                    <a id="querybtn" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'"
                        runat="server" onserverclick="ToolBar1_ButtonClick">查询</a>
                </div>
                <div class="datagrid-btn-separator">
                </div>
                <a id="newWin" href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'"
                    onclick="CreateEntityForm()">新建</a> 
                <a id="editWin" href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-edit'" 
                    onclick="EditEntityForm()">修改</a> 
                <a id="deleteWin" href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-delete'" 
                    onclick="DeleteEntity()">删除</a>
                <div class="datagrid-btn-separator">
                </div>
                <a id="A1" href="#" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-config'" 
                    onclick="SetEntity()">设置</a>
            </div>
        </div>
        <table id="ensGrid" fit="true" fitcolumns="true" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    <input type="hidden" id="EnNo" value="<%=FK_MapData %>" />
</asp:Content>
