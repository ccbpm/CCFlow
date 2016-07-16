<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_Admin_MapDef_Action" Codebehind="Action.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function DoDel(fk_mapdata, xmlEvent) {
            if (window.confirm('您确认要删除吗?') == false)
                return;
            window.location.href = 'Action.aspx?FK_MapData=' + fk_mapdata + '&DoType=Del&RefXml=' + xmlEvent;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width='80%' align=center>
<uc1:Pub ID="Pub3" runat="server" />
<tr>
<td valign=top><uc1:Pub ID="Pub1" runat="server" /></td>
<td valign=top><uc1:Pub ID="Pub2" runat="server" /></td>
  </tr>
    </table>
</asp:Content>