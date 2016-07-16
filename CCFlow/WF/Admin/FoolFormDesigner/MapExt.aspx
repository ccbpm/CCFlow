<%@ Page Title="表单扩展设置" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_MapExt" Codebehind="MapExt.aspx.cs" %>
<%@ Register src="UC/MExt.ascx" tagname="MExt" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
<script type="text/javascript">
    function WinOpen(url) {
        var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');
        newWindow.focus();
        return;
    }
    function LoadRETemplete(fk_mapdata, keyOfEn, myPK) {
        var url = 'RETemplete.aspx?FK_MapData=' + fk_mapdata + '&ForCtrl=' + keyOfEn + '&KeyOfEn=' + keyOfEn;
        var v = window.showModalDialog(url, 'dsd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
        window.location.href = window.location.href;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:MExt ID="MExt1" runat="server" />
</asp:Content>

