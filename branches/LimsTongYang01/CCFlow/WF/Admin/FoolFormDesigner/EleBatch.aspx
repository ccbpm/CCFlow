<%@ Page Title="把当前表单元素Copy到其他的表单上去" Language="C#" MasterPageFile="WinOpen.master"
    AutoEventWireup="true" CodeBehind="EleBatch.aspx.cs" Inherits="CCFlow.WF.MapDef.EleCopy" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'west',title:'批量操作'" style="width: 200px;">
            <uc1:Pub ID="Left" runat="server" />
        </div>
        <div data-options="region:'center'">
            <uc1:Pub ID="Pub1" runat="server" />
        </div>
    </div>
</asp:Content>
