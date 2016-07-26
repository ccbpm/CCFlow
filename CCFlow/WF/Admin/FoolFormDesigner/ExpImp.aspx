<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_ExpImp" Codebehind="ExpImp.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function LoadFrm(fk_flow, refno, fk_Frm) {
        if (confirm('您确定吗？') == false)
            return;
        window.location.target = '_self';
        window.location.href = 'ExpImp.aspx?DoType=Imp&FK_Flow=" + fk_flow + "&FK_MapData=" +refno + "&FromMap=' + fk_Frm;
    }
</script>
<base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

