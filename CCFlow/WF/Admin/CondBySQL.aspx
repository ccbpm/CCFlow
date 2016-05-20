<%@ Page Title="SQL条件" Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Admin.WF_Admin_CondBySQL" CodeBehind="CondBySQL.aspx.cs" %>

<%@ Register Src="UC/CondBySQL.ascx" TagName="CondBySQL" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:CondBySQL ID="CondBySQL1" runat="server" />
</asp:Content>
