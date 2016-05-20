<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.Master" AutoEventWireup="true" CodeBehind="DictList.aspx.cs" Inherits="CCFlow.App.ListBR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<base target="mainS" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>组织结构维护</legend>
<ul>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.GPM.DeptTypes" target="mainS" > 部门类型 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.Depts" target="mainS" > 部门维护 </a> </li>
<li><a href="/WF/Comm/Tree.aspx?EnsName=BP.SQ.Depts" target="mainS" > 部门树维护 </a> </li>
<li><a href="/WF/Comm/TreeEns.aspx?TreeEnsName=BP.SQ.Depts&EnsName=BP.Port.Emps&RefPK=FK_Dept" target="mainS" > 部门人员维护 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.GPM.Stations" > 岗位维护 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.GPM.Emps" > 人员维护 </a> </li>
</ul>
</fieldset>


<fieldset>
<legend>BIR基础数据配置</legend>
<ul>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.SZDDs" > 试制地点 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.WTFLs" > 问题分类 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.SSXTs" > 所属系统 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.CXPTs" > 车辆平台 </a> </li>
</ul>
</fieldset>

<fieldset>
<legend>TIR基础数据配置</legend>
<ul>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.ZCJDs" > 造车阶段 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.GZFJs" > 故障分级 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.SYDDs" > 试验地点 </a> </li>
<li><a href="/WF/Comm/Search.aspx?EnsName=BP.SQ.PHASEs" > 问题发生阶段 </a> </li>
</ul>
</fieldset>

</asp:Content>
