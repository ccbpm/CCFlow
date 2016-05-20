<%@ Page Language="C#" MasterPageFile="SysMapEn.master" AutoEventWireup="true" Inherits="CCFlow.WF.Comm.RefFunc.SysMapEnUI" Title="Untitled Page" Codebehind="SysMapEn.aspx.cs" %>
<%@ Register src="SysMapEnUC.ascx" tagname="SysMapEnUC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="Style/JScript.js"></script>
    <script language="javascript">
    function GroupBarClick(rowIdx) {
        var alt = document.getElementById('Img' + rowIdx).alert;
        var sta = 'block';
        if (alt == 'Max') {
            sta = 'block';
            alt = 'Min';
        } else {
            sta = 'none';
            alt = 'Max';
        }
        document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
        document.getElementById('Img' + rowIdx).alert = alt;
        var i = 0
        for (i = 0; i <= 40; i++) {
            if (document.getElementById(rowIdx + '_' + i) == null)
                continue;
            if (sta == 'block') {
                document.getElementById(rowIdx + '_' + i).style.display = '';
            } else {
                document.getElementById(rowIdx + '_' + i).style.display = sta;
            }
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:SysMapEnUC ID="SysMapEnUC1" runat="server" />
</asp:Content>

