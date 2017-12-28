<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Rpt.WF_Rpt_Attachment" Codebehind="Attachment.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

