<%@ Page Title="开发者参数" Language="C#" MasterPageFile="Condition.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.Admin.WF_Admin_CondByPara" CodeBehind="CondByPara.aspx.cs" %>

<%@ Register Src="UC/CondByPara.ascx" TagName="CondByPara" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="Server">
    <uc1:CondByPara ID="CondByPara1" runat="server" />
</asp:Content>
