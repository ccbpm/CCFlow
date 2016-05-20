<%@ Page Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Msg.WF_Msg_Read" Title="已读" Codebehind="Read.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<base target=_self /> 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

