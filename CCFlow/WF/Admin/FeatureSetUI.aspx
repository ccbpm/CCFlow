<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_FeatureSetUI" Codebehind="FeatureSetUI.aspx.cs" %>

<%@ Register src="UC/FeatureSet.ascx" tagname="FeatureSet" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:FeatureSet ID="FeatureSet1" runat="server" />
</asp:Content>

