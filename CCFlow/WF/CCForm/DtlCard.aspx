<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_DtlFrm" Codebehind="DtlCard.aspx.cs" %>
<%@ Register src="../UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

<link href="../Style/themes/default/easyui.css" rel="stylesheet" type="text/css" />
<link href="../Style/themes/icon.css" rel="stylesheet" type="text/css" />
<script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
<script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
<script language="javascript">
    function SaveDtlData(iframeId) {
        var iframe = document.getElementById("IF" + iframeId);
        if (iframe) {
            iframe.contentWindow.SaveDtlData()
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:UCEn ID="UCEn1" runat="server" />
</asp:Content>
