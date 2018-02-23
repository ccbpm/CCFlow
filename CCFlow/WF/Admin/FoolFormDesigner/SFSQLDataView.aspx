<%@ Page Title="字典表视图" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="SFSQLDataView.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.SFSQLDataView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    BP.Sys.SFTable sf = new BP.Sys.SFTable( this.Request.QueryString["FK_SFTable"]);
     %>
<table>

<tr>
<th>序</th>
<th> 编号 No</th>
<th>名称Name </th>

<% if (sf.CodeStruct == BP.Sys.CodeStruct.Tree) { %>
<th> 父节点编号 </th>

<% } %>

</tr>


<%
    System.Data.DataTable dt = sf.GenerHisDataTable;
    int idx = 0;
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        idx++;
        %>
        <tr>
        <td class=Idx><%=idx %> </td>
        <td><%=dr["No"] %> </td>
        <td><%=dr["Name"] %> </td>
     <% if (sf.CodeStruct == BP.Sys.CodeStruct.Tree) { %>
        <td><%=dr["ParentNo"] %> </td>
      <% } %>
        </tr>
<% } %>
</table>

</asp:Content>
