<%@ Page Title="主页" Language="C#" MasterPageFile="~/WF/Admin/CCFormDesigner/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%
    string fk_mapdata=this.Request.QueryString["FK_MapData"];
    BP.Sys.MapData md = new BP.Sys.MapData(fk_mapdata);
%>

<fieldset>
<legend>基础信息</legend>
<ul>
<li>表单编号：<%=md.No%></li>
<li>表单名称：<%=md.Name%></li>
<li>物理表：<%=md.PTable%></li>
<li>表单类型：<%=md.HisFrmType%></li>

<li>总数据：<%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM  "+md.PTable+" ")%>条</li>

<li><a href="/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapFrmExcels&PK=<%=fk_mapdata %>" >属性</a>：<%=md.HisFrmType%></li>

</ul>
</fieldset>

<% 
    if (md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
    {
     %>
<fieldset>
<legend>表单设计</legend>
<ul>
<li>设计自由表单  WF/Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=Demo_01 </li>
<li>设计自由表单  WF/Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=Demo_01 </li>
</ul>
</fieldset>

<%} %>
</asp:Content>
