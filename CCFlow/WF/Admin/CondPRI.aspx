<%@ Page Title="优先级设置" Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Admin.WF_Admin_CondPRI" CodeBehind="CondPRI.aspx.cs" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
