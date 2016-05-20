<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/WinOpen.master" AutoEventWireup="true" CodeBehind="Testing.aspx.cs" Inherits="CCFlow.SDKFlowDemo.UnitTesting.Testing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;">
<caption> 产品质量是产品的生命线 </caption>

<tr>
<td>  
<ul>
<%
    //开始执行信息.
    System.Collections.ArrayList al = BP.En.ClassFactory.GetObjects("BP.UnitTesting.TestBase");
    int idx = 0;
    foreach (BP.UnitTesting.TestBase en in al)
    {
        %> <li> <a href="Testing.aspx?EnsName="> <%=en.Title %></a> </li> <%
    }
%>

</ul>

</td>
</tr>

</table>

</asp:Content>
