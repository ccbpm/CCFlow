<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/WinOpen.master" AutoEventWireup="true" CodeBehind="AuthorDemo.aspx.cs" Inherits="CCFlow.SDKFlowDemo.AuthorDemo" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
