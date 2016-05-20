<%@ Page Title="附件" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="WorkCheckAth.aspx.cs" Inherits="CCFlow.WF.WorkOpt.WorkCheckAth" %>
<%@ Register src="../SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<center>
    <uc1:DocMultiAth ID="DocMultiAth1" runat="server" />
    </center>
</asp:Content>
