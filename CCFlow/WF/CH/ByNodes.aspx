<%@ Page Title="流程考核分析" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="ByNodes.aspx.cs" Inherits="CCFlow.WF.CH.OneFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<%
    string flowNo = this.Request.QueryString["FK_Flow"];
    
     %>

<table>
<caption>节点维度</caption>
<tr>
<th>序号</th>
<th>节点</th>
<th>工作总数</th>
<th>提前完成时间(分钟)</th>
<th>逾期完成时间(分钟)</th>
<th>及时完成</th>
<th>按期完成</th>
<th>逾期完成</th>
<th>超期完成</th>
<th>按期办结率</th>
<th>主要参与人</th>
</tr>
</table>


<table>
<caption>人员维度</caption>
<tr>
<th>序号</th>
<th>部门</th>
<th>工作人员</th>
<th>岗位</th>
<th>提前完成时间(分钟)</th>
<th>逾期完成时间(分钟)</th>
<th>及时完成</th>
<th>按期完成</th>
<th>逾期完成</th>
<th>超期完成</th>
<th>按期办结率</th>
</tr>
</table>


</asp:Content>
