<%@ Page Title="排名分析" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.CH.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;">
<caption>排名分析 </caption>


<tr>
<th colspan=3>综合排名 - 前10名  </th>  
<tr>

<tr>
<td>  
<fieldset>
<legend>逾期率最高的节点 </legend>
</fieldset>
</td>


<td>  
<fieldset>
<legend>逾期率最高的流程 </legend>
</fieldset>
</td>

<td>  
<fieldset>
<legend>预期最多的部门 </legend>
</fieldset>
</td>

</tr>




<tr>
<th colspan=3>本月排名 - 前10名 </th>  
<tr>


<tr>
<td>  
<fieldset>
<legend>逾期率最高的节点 </legend>
</fieldset>
</td>


<td>  
<fieldset>
<legend>逾期率最高的流程 </legend>
</fieldset>
</td>

<td>  
<fieldset>
<legend>逾期最多的部门 </legend>
</fieldset>
</td>

</tr>



</table>

</asp:Content>
