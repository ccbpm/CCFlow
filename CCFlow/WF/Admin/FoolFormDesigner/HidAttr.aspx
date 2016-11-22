<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_HidAttr" Codebehind="HidAttr.aspx.cs" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
        function Edit(fk_mapdata, mypk, ftype) {
        var url = 'EditF.aspx?DoType=Edit&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&FType=' + ftype;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.close();
        //window.location.href = window.location.href;
    }
    function EditEnum(mypk, refno, enumKey) {
        var url = 'EditEnum.aspx?DoType=Edit&MyPK=' + mypk + '&KeyOfEn=' + refno + '&EnumKey=' + enumKey;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.close();
        //window.location.href = window.location.href;
    }
    function EditTable(mypk, keyOfEn, sfTable) {
        var url = 'EditTable.aspx?DoType=Edit&MyPK=' + mypk + '&KeyOfEn=' + keyOfEn + '&SFTable=' + sfTable;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.close();
        //window.location.href = window.location.href;
    }
</script>
<base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

