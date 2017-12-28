<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/SDK/Site.Master" AutoEventWireup="true" CodeBehind="S11101.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F111.S11101" %>
<%@ Register src="DocRelease.ascx" tagname="DocRelease" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:DocRelease ID="DocRelease1" runat="server" />
</asp:Content>
