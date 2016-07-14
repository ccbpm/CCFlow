<%@ Page Title="4. 设置报表查询条件" Language="C#" MasterPageFile="RptGuide.Master" AutoEventWireup="true"
    CodeBehind="S5_SearchCond.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.SearchCond" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub2" runat="server" />
    <br />
    <cc1:LinkBtn ID="Btn_Save1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存" OnClick="Btn_Save_Click" />
    <cc1:LinkBtn ID="Btn_SaveAndNext1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存并继续" OnClick="Btn_SaveAndNext1_Click" />
    <cc1:LinkBtn ID="Btn_Cancel1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-undo'"
        Text="取消" OnClick="Btn_Cancel_Click" />
</asp:Content>
