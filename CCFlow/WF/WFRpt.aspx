<%@ Page Language="C#" MasterPageFile="WinOpenEUI.Master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_WFRpt" Title="未命名頁面" Codebehind="WFRpt.aspx.cs" %>
<%@ Register src="UC/WFRpt.ascx" tagname="WFRpt" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
</script>
  <link href="Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:WFRpt ID="WFRpt1" runat="server" />
</asp:Content>

