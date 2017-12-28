<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="FrmPopValDir.aspx.cs" Inherits="CCFlow.WF.CCForm.FrmPopValDir" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%">
<tr>
<td width="30%"><uc1:Pub ID="Left" runat="server" /></td>
<td valign=top><uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>
</table>
<hr />
    <asp:Button ID="Btn_OK" runat="server" Text="确定" onclick="Btn_OK_Click" />
    
</asp:Content>
