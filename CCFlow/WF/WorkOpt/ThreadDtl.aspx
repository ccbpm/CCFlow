<%@ Page Title="子线程信息" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_WorkOpt_ThreadDtl" Codebehind="ThreadDtl.aspx.cs" %>
<%@ Register src="../SDKComponents/ThreadDtl.ascx" tagname="ThreadDtl" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="JavaScript" src="../Comm/JScript.js" type="text/javascript" ></script>
    <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table  style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %> --子线程信息</caption>
<tr>
<td style="text-align:center">
<div style="text-align:center">
<uc1:ThreadDtl ID="ThreadDtl1" runat="server" />
</div>
</td>
</tr>
</table>
</asp:Content>

