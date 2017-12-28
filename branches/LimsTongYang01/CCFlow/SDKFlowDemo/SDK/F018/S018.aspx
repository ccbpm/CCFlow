<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/SDK/Site.Master" AutoEventWireup="true" CodeBehind="S018.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F018.S018" %>
<%@ Register src="Apply.ascx" tagname="Apply" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../WF/Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Apply ID="Apply1" runat="server" />
</asp:Content>
