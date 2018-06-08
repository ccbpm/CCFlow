<%@ Page Title="" Language="C#" MasterPageFile="~/GPM/WinOpen.master" AutoEventWireup="true" CodeBehind="WhoCanViewInfoPush.aspx.cs" Inherits="GMP2.GPM.WhoCanViewInfoPush" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
