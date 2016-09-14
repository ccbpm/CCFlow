<%@ Page Title="外键表引用" Language="C#" MasterPageFile="~/WF/Admin/CCFormDesigner/Site.Master" AutoEventWireup="true" CodeBehind="TableRef.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.TableRef" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- 徐淑豪: 2016-07-05 -->

<table>

<tr>
<th>序</th>
<th>引用表单</th>
<th>字段英文</th>
<th>字段中文</th>
</tr>
<%
    string RefNo = this.Request.QueryString["RefNo"];
    BP.Sys.MapAttrs mapAttrs = new BP.Sys.MapAttrs();
    mapAttrs.RetrieveByAttr(BP.Sys.MapAttrAttr.UIBindKey, RefNo);
    int idx = 0;
    foreach (BP.Sys.MapAttr attr in mapAttrs)
    {
        idx++;
     %>
<tr>
<td><%=idx%></td>
<td><%=attr.FK_MapData%></td>
<td><%=attr.KeyOfEn%></td>
<td><%=attr.Name%></td>

</tr>
<%
    } %>

</table>
</asp:Content>
