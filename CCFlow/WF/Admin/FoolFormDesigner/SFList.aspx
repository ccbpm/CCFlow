<%@ Page Title="数据表字典" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="SFList.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.SFList" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" language="javascript" >
    function AddSFTable(fk_mapdata, idx, key, groupField) {

        var url = 'EditTable.aspx?FK_MapData=' + fk_mapdata + '&IDX=' + idx + '&FK_SFTable=' + key + '&GroupField=' + groupField;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 500px;center: yes; help: no');
    }
    function AddSFSQL(mypk, idx, key) {

        var url = 'Do.aspx?DoType=AddSFSQLAttr&MyPK=' + mypk + '&IDX=' + idx + '&RefNo=' + key;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 500px;center: yes; help: no');
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    <uc1:Pub ID="Pub2" runat="server" />
</asp:Content>
