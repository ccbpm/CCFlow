<%@ Page Title="我的流程" Language="C#" MasterPageFile="EasySearch.Master" AutoEventWireup="true" CodeBehind="EasySearchMyFlow.aspx.cs" Inherits="CCFlow.WF.Rpt.EasySearchMyFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="  border:0px; width:100%; ">
<tr>


<td>
<h3>我发起的流程</h3>
<%
    string sql = "";
    sql = "select FK_Flow, FlowName,Count(WorkID) as Num from WF_GenerWorkFlow  WHERE Starter='" + BP.Web.WebUser.No + "' GROUP BY FK_Flow, FlowName ";
    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
    string html = "<ul>";
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        html += "<li><a href='../Comm/Search.htm?EnsName=BP.WF.Data.MyStartFlows&FK_Flow=" + dr["FK_Flow"] + "&WFSta=All&TSpan=All' >" + dr["FlowName"] + "(" + dr["Num"] + ")</a></li>";
    }
    html += "</ul>";
%>
<%=html %>
</td>



<td>
<h3>我的待办(待处理)</h3>
<%
    sql = "select FK_Flow, FlowName,Count(WorkID) as Num from wf_empworks  WHERE FK_Emp='"+BP.Web.WebUser.No+"' GROUP BY FK_Flow, FlowName ";
    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
    html = "<ul>";
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        html += "<li><a href='../EmpWorks.aspx?FK_Flow="+dr["FK_Flow"]+"'>" + dr["FlowName"] + "(" + dr["Num"] + ")</a></li>";
    }
    html += "</ul>";
%>
<%=html %>
</td>



<td>
<h3>我的在途(未完成)</h3>
<%
    dt = BP.WF.Dev2Interface.DB_TongJi_Runing(); 
    html = "<ul>";
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        html += "<li><a href='../Runing.aspx?FK_Flow=" + dr["FK_Flow"] + "'>" + dr["FlowName"] + "(" + dr["Num"] + ")</a></li>";
    }
    html += "</ul>";
%>
<%=html %>
</td>


<td>
<h3>已归档(已完成)</h3>
<%
    dt = BP.WF.Dev2Interface.DB_TongJi_FlowComplete(); 
    html = "<ul>";
    foreach (System.Data.DataRow dr in dt.Rows)
    {
        html += "<li><a href='../Comm/Search.htm?EnsName=BP.WF.Data.MyFlows&FK_Flow=" + dr["FK_Flow"] + "&WFSta=1&TSpan=All' >" + dr["FlowName"] + "(" + dr["Num"] + ")</a></li>";
    }
    html += "</ul>";
%>
<%=html %>
</td>


</tr>
</table>
 


</asp:Content>
