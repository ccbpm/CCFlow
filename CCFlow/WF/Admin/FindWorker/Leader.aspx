<%@ Page Title="直线领导" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Leader.aspx.cs" Inherits="CCFlow.WF.Admin.FindWorker.Leader" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Comm/Style/Tabs.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%" >
    <caption>找直接领导</caption>
<tr>
<td valign=top><uc1:Pub ID="UCS1" runat="server" /></td>
</tr>

<tr>
<td valign=top><uc1:Pub ID="UCS2" runat="server" /></td>
</tr>

<tr>
<td valign=top><uc1:Pub ID="UCS3" runat="server" /></td>
</tr>
</table>
     
</asp:Content>
