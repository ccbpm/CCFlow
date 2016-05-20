<%@ Page Title="批量修改节点属性" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true"
 Inherits="CCFlow.WF.Admin.FeatureSetUI" Codebehind="FeatureSetUI.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
<table border="0" width='100%' >
<caption> 批量修改节点属性 </caption>
<tr>
<td valign="top" align="left" width='30%'>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
<td valign="top" align="left" width='70%'>
    <uc1:Pub ID="Pub2" runat="server" />
    </td>
</tr>
</table>


</asp:Content>

