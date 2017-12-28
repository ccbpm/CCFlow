<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/Comm/Site.Master" AutoEventWireup="true" CodeBehind="Track.aspx.cs" Inherits="CCFlow.SDKFlowDemo.Comm.Track" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
