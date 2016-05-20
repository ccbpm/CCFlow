<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_MyFlowInfo" Codebehind="MyFlowInfo.aspx.cs" %>

<%@ Register src="../UC/MyFlowInfo.ascx" tagname="MyFlowInfo" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc1:MyFlowInfo ID="MyFlowInfo1" runat="server" />
</asp:Content>

