<%@ Page Title="发起" Language="C#" MasterPageFile="SiteMenu.Master" AutoEventWireup="true" CodeBehind="Start.aspx.cs" Inherits="CCFlow.App.Start" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
   //获取可以发起的流程集合。
   System.Data.DataTable dt= 
       BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);
   // 输出集合.
   %>
   <table border="1"  style="  border-color:Black;  border-width:1px;"  >
    <caption>发起(列出当前人员可以发起的流程,<a href="Do.aspx?DoType=ViewStartSrc" target=_blank >查看源代码</a>)</caption>
   <tr>
   <th>序</th>
   <th>类别</th>
   <th>流程</th>
   <th>操作</th>
   </tr>
     <%
    
    int idx = 0;
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        idx++;
        int flowNum = int.Parse(dr["No"].ToString());
        %>
        <tr>
        <td class="Idx"><%= idx%></td>
        <td><%=dr["FK_FlowSortText"] %></td>
        <td> <a target="_blank" href='/WF/MyFlow.aspx?FK_Flow=<%=dr["No"] %>' ><%=dr["Name"].ToString() %> </a> </td>
        <td><a href="../../Rpt/Search.aspx?RptNo=ND<%=flowNum %>MyRpt&FK_Flow=<%=dr["No"] %>"  >查询</a></td>

        </tr>
   <% } %> 
   </table>
</asp:Content>
