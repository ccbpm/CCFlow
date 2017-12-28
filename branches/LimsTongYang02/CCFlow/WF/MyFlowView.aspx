<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_MyFlowView" Codebehind="MyFlowView.aspx.cs" %>
<%@ Register src="UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
.Bar
{
    width:500px;
    text-align:center;
}
#tabForm, D
{
    width:960px;
    text-align:left;
    margin:0 auto;
    margin-bottom:5px;
}
#divCCForm {
 position:relative;
 left:25PX;
 width:960px;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id=tabForm >
</div>
<div id="D" >
    <uc1:UCEn ID="UCEn1" runat="server" />
</div>
</asp:Content>

