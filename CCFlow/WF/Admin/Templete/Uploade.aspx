<%@ Page Title="我为人人，人人为我。" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="Uploade.aspx.cs" Inherits="CCFlow.WF.Admin.Templete.Uploade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%">
<caption>模版共享</caption>
<tr>
<td>选择要共享的流程</td>
<td>共享信息</td>
</tr>

<tr>
<td>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    </td>
<td>行业： http://www.actionsoft.com.cn/case/case_index.jsp<br />
    BBS帐号:<br />
    电话：<br />
    邮件：</td>
</tr>

</table>


    <asp:Button ID="Btn_Upload" runat="server" Text="执行共享" />


</asp:Content>
