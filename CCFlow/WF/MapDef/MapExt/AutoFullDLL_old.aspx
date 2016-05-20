<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="AutoFullDLL_old.aspx.cs" Inherits="CCFlow.WF.MapDef.MapExtUI.AutoFullDLLUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style=" width:100%;">
<caption>数据源填充:配置SQL返回No,Name两个列填充下拉框,支持ccbpm表达式.</caption>

<%
  
     %>
<tr>
<td>

<textarea id="TB_Doc" runat="server"  style=" width:100%;"  >
</textarea>
</td>

<tr>
<td>
    <asp:Button ID="Button1" runat="server" Text="保存" onclick="Button1_Click" />
    <asp:Button ID="Button2" runat="server" Text="删除" onclick="But_Del_Click" OnClientClick="return window.confirm('您确认要删除吗?');" />

    </td>
</tr>
</tr>
</table>


</asp:Content>
