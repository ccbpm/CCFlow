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

表单编号：<%=md.No%>
表单名称：<%=md.Name%>

</fieldset>

<fieldset>
<legend>表单设计</legend>
设计自由表单  WF/MapDef/CCForm/Frm.aspx?FK_MapData=Demo_01
  设计傻瓜表单  WF/MapDef/MapDef.aspx?FK_MapData=xxxxxx
</fieldset>
</asp:Content>
