<%@ Page Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_Cond"
    Title="表单条件" CodeBehind="Cond.aspx.cs" %>

<%@ Register Src="UC/Cond.ascx" TagName="Cond" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:Cond ID="Cond1" runat="server" />
</asp:Content>
