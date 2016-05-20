<%@ Page Title="JSLab" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow_Comm_Sys_FuncLib" Codebehind="FuncLib.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
		<script language="JavaScript" src="../JScript.js" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table style=" width:100%;" >
<caption> javascript函数库</caption>
<tr>
<td>
    <uc1:Pub ID="Pub1" runat="server" />
</td>
</tr>
    </table>
</asp:Content>