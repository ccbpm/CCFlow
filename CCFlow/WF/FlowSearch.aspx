<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
 Inherits="CCFlow.WF.Face_FlowSearch" Title="查询与分析" Codebehind="FlowSearch.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language=javascript>
    function Dtl(fk_flow) {
        WinOpen('DtlSearch.aspx?FK_Flow=' + fk_flow, 'ss');
    }
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
        for (i = 0; i <= 5000; i++) {
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
    <script language="JavaScript" src="./Comm/JScript.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div style=" text-align:center">
    <uc2:Pub ID="Pub1" runat="server" />
    <uc2:Pub ID="Pub2" runat="server" />
    </div>
</asp:Content>


