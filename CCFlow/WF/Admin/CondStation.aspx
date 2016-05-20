<%@ Page Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CondStation"
    Title="岗位条件" CodeBehind="CondStation.aspx.cs" %>

<%@ Register Src="UC/CondStation.ascx" TagName="CondStation" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../Comm/JScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:CondStation ID="CondStation1" runat="server" />
</asp:Content>
