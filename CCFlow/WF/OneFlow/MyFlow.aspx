<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_MyFlow" Codebehind="MyFlow.aspx.cs" %>
<%@ Register src="../UC/MyFlow.ascx" tagname="MyFlow" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Right" Runat="Server">
    <uc1:MyFlow ID="MyFlow1" runat="server" />
</asp:Content>

