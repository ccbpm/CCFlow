<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="SysEnumList.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.SysEnumList" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript" language=javascript>

    function EditEnum(key) {
        var url = 'SysEnum.aspx?DoType=New&EnumKey=' + key;
        //  window.location.href=url;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
        window.location.reload();
    }
    function NewEnum() {
        var url = 'SysEnum.aspx?DoType=New&EnumKey=';
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function AddEnum(fk_mapdata, groupField, key) {
        var url = '';
        url = 'EditEnum.aspx?DoType=New&FK_MapData=' + fk_mapdata + '&EnumKey=' + key + '&GroupField=' + groupField;
        var c = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no');
        return;
    }

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:Pub ID="Pub1" runat="server" />
    <uc1:Pub ID="Pub2" runat="server" />
</asp:Content>
