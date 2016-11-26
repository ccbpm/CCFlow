<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CCFlow.WF.Search" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<base target="mainS" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>流程查询方式</legend>
<ul>
<li><a href="Comm/Search.aspx?EnsName=BP.WF.Data.MyFlows" target="mainS" >我参与的流程</a></li>
<li><a href="Comm/Search.aspx?EnsName=BP.WF.Data.MyStartFlows" target="mainS">我发起的流程</a></li>
<li><a href="Comm/Search.aspx?EnsName=BP.WF.Data.MyDeptFlows" target="mainS">我部门发起的流程</a></li>
<li><a href="Comm/Search.aspx?EnsName=BP.WF.Data.MyDeptTodolists" target="mainS">我部门的待办</a></li>
<li><a href="KeySearch.aspx" target="mainS">关键字查询</a></li>
<li><a href="FlowSearch.aspx" target="mainS">按流程高级查询</a></li>
</ul>
</fieldset>


<fieldset>
<legend>流程统计方式</legend>
<ul>
<li><a href="Comm/Group.aspx?EnsName=BP.WF.Data.MyFlows" target="mainS" >我参与的流程</a></li>
<li><a href="Comm/Group.aspx?EnsName=BP.WF.Data.MyStartFlows" target="mainS">我发起的流程</a></li>
<li><a href="Comm/Group.aspx?EnsName=BP.WF.Data.MyDeptFlows" target="mainS">我部门发起的流程</a></li>
<li><a href="Comm/Group.aspx?EnsName=BP.WF.Data.MyDeptTodolists" target="mainS">我部门待办</a></li>

</ul>
</fieldset>

</asp:Content>
