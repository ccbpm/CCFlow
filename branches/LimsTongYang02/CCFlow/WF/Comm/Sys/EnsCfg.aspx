<%@ Page Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true" Inherits="CCFlow_Comm_Sys_EnConfig" Title="功能配置" Codebehind="EnsCfg.aspx.cs" %>

<%@ Register src="../UC/UCSys.ascx" tagname="UCSys" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Table.css" rel="stylesheet" type="text/css" />
    <base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:UCSys ID="UCSys1" runat="server" />
</asp:Content>

