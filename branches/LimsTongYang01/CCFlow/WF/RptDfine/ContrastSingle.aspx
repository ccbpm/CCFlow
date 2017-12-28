<%@ Page Title="" Language="C#" MasterPageFile="Single.Master" AutoEventWireup="true" CodeBehind="ContrastSingle.aspx.cs" Inherits="CCFlow.WF.Rpt.ContrastSingle" %>
<%@ Register src="UC/Contrast.ascx" tagname="Contrast" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Contrast ID="Contrast1" runat="server" />
</asp:Content>
