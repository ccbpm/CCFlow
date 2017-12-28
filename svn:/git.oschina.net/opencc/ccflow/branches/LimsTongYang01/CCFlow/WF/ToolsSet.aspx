<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ToolsSet" Codebehind="ToolsSet.aspx.cs" %>
<%@ Register src="UC/ToolsWap.ascx" tagname="ToolsWap" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ToolsWap ID="ToolsWap1" runat="server" />
</asp:Content>

