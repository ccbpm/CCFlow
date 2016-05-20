<%@ Page Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CondDept"
    Title="部门条件" CodeBehind="CondDept.aspx.cs" %>

<%@ Register Src="UC/CondDepts.ascx" TagName="CondDept" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:CondDept ID="CondDept1" runat="server" />
</asp:Content>
