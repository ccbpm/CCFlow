<%@ Page Title="" Language="C#" MasterPageFile="../WorkOpt/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WorkOpt.WF_WorkOpt_Home" Codebehind="Home.aspx.cs" %>

<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<uc1:Pub ID="Pub2" runat="server" />
</asp:Content>

