<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.Comm_MapDef_CopyFieldFromNode" Title="复制字段" Codebehind="CopyFieldFromNode.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript">
function Go(FK_Node, cid,  seleNd)
{
    var   province=document.getElementById("_ctl0_ContentPlaceHolder1_Pub2_DDL1");
                var   pindex   =   province.selectedIndex;
                var   pValue   =   province.options[pindex].value;
                var   pText     =   province.options[pindex].text;
  window.location.href='CopyFieldFromNode.aspx?FK_Node='+FK_Node+'&NodeOfSelect='+pValue;
}
function GroupClick(groupID)
{
}
</script>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width='100%' style=" width:100%;">
<%--<tr>
    <TD valign=top   colspan=2 ><caption class=Caption>复制字段</caption></TD>
    </tr>--%>
<tr>
    <TD valign=top   >
        <uc1:Pub ID="Pub1" runat="server" />
    </TD>
    <TD valign=top   >
        <uc1:Pub ID="Pub2" runat="server" />
    </TD>
    </tr>
 </table>
</asp:Content>

