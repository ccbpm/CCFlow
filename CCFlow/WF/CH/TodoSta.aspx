<%@ Page Title="流程待办统计汇总" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="TodoSta.aspx.cs" Inherits="CCFlow.WF.CH.TodoSta" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%">
<caption> 流程运行情况一览表 </caption>

<tr>
<th>序号</th>
<th>类别</th>
<th>流程</th>
<th><img src="/WF/Img/TolistSta/0.png" alt="绿牌" />待办中</th>
<th><img src="/WF/Img/TolistSta/1.png" alt="黄牌" />预警中</th>
<th><img src="/WF/Img/TolistSta/2.png" alt="红牌"/>预期中</th>
<th><img src="/WF/Img/TolistSta/1.png" alt="绿牌" />正常办结</th>
<th><img src="/WF/Img/TolistSta/2.png" alt="绿牌" />超期办结</th>
<th>详细</th>
</tr>

<%
    string sql = "SELECT ";
    sql += "FK_FlowSort, FK_Flow, FlowName,";
sql+=" (SELECT COUNT(TodoSta) FROM WF_GenerWorkFlow WHERE TodoSta=0) AS  TodoSta0, ";
sql+=" (SELECT COUNT(TodoSta) FROM WF_GenerWorkFlow WHERE TodoSta=1) AS  TodoSta1, ";
sql+=" (SELECT COUNT(TodoSta) FROM WF_GenerWorkFlow WHERE TodoSta=2) AS  TodoSta2, ";
sql+=" (SELECT COUNT(TodoSta) FROM WF_GenerWorkFlow WHERE TodoSta=3) AS  TodoSta3, ";
sql+=" (SELECT COUNT(TodoSta) FROM WF_GenerWorkFlow WHERE TodoSta=4) AS  TodoSta4  ";
sql += " FROM WF_GenerWorkFlow A GROUP BY FK_FlowSort,FK_Flow, FlowName ";
 System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

 int idx = 0;
 foreach (System.Data.DataRow dr in dt.Rows)
 {
     idx++;
     %>
     <tr>
<td class=Idx ><%=idx %></td>
<td><%=dr["FK_FlowSort"]%></td>
<td><%=dr["FlowName"]%></td>

<td><%=dr["TodoSta0"]%></td>
<td><%=dr["TodoSta1"]%></td>
<td><%=dr["TodoSta2"]%></td>
<td><%=dr["TodoSta3"]%></td>
<td><%=dr["TodoSta4"]%></td>
 
<td><a href="javascript:WinOpen('FlowOne.aspx?FK_Flow=<%=dr["FK_Flow"] %>' )" >详细</a></td>
</tr>

     <%
 }
 %>

</table>



</asp:Content>
