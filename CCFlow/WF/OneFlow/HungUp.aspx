<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_HungUp" Codebehind="HungUp.aspx.cs" %>

<%@ Register src="UC/Working.ascx" tagname="Working" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc1:Working ID="Working1" runat="server" />
</asp:Content>

