<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_Runing" Codebehind="Runing.aspx.cs" %>

<%@ Register src="./UC/Runing.ascx" tagname="Runing" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc1:Runing ID="Runing1" runat="server" />
</asp:Content>

