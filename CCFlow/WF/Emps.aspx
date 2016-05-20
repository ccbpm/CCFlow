<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Emps" Title="Untitled Page" Codebehind="Emps.aspx.cs" %>

<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function DoUp(no, keys) {
        var url = "Do.aspx?DoType=EmpDoUp&RefNo=" + no + '&dt=' + keys;
        val = window.showModalDialog(url, 'f4', 'dialogHeight: 5px; dialogWidth: 6px; dialogTop: 100px; dialogLeft: 100px; center: yes; help: no');
        window.location.href = window.location.href;
        return;
    }
    function DoDown(no, keys) {
        var url = "Do.aspx?DoType=EmpDoDown&RefNo=" + no + '&sd=' + keys;
        val = window.showModalDialog(url, 'f4', 'dialogHeight: 5px; dialogWidth: 6px; dialogTop: 100px; dialogLeft: 100px; center: yes; help: no');
        window.location.href = window.location.href;
        return;
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

