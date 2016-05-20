<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_FrmEvent" Codebehind="FrmEvent.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table style="width:100%;border:0px" class='ddd'  >
<tr>
<td valign=top>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
<td valign=top align=center>
    <uc1:Pub ID="Pub2" runat="server" />
    </td>
    </tr>
</table>
</asp:Content>