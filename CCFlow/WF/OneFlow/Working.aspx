<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_Working" Codebehind="Working.aspx.cs" %>
<%@ Register src="../UC/EmpWorks.ascx" tagname="EmpWorks" tagprefix="uc1" %>
<%@ Register src="UC/Working.ascx" tagname="Working" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc2:Working ID="Working1" runat="server" />
</asp:Content>


