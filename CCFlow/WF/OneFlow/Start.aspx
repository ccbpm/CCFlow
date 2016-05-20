<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_Start" Codebehind="Start.aspx.cs" %>
<%@ Register src="UC/StartList.ascx" tagname="StartList" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc1:StartList ID="StartList1" runat="server" />
</asp:Content>

