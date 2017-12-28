<%@ Page Title="翻页测试" Language="C#" MasterPageFile="../../WinOpen.master" AutoEventWireup="true" CodeBehind="DemoTurnPage.aspx.cs" Inherits="CCFlow.SDKFlowDemo.DemoTurnPage" %>
<%@ Register src="../../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../WF/Comm/Style/Table.css" rel="stylesheet" 
        type="text/css" />
    <link href="../../../WF/Comm/Style/Table0.css" rel="stylesheet" 
        type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%">
<caption>翻页输出</caption>
    <tr>
    <td>
    <!-- 表体 -->
    <uc1:Pub ID="Pub1" runat="server" /></td>
    </tr>

     <tr>
    <td>
    <!-- 尾部显示翻页的输出 -->
    <uc1:Pub ID="Pub2" runat="server" />
    </td>
    </tr>
</table>

</asp:Content>
