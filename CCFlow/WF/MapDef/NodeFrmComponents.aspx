<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="NodeFrmComponents.aspx.cs" Inherits="CCFlow.WF.MapDef.NodeFrmComponents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<% if (this.Request.QueryString["DoType"] == "FWC")
   { %>
<h5>欢迎使用审核组件</h5>
<ul>
<li>在傻瓜表单里，审核组件的启用、不启用在表单属性=》节点表单属性里设置。</li>
<li>审核组件的高度也在表单属性设置。</li>
<li>审核组件可以方便快速的完成表单设计。</li>
<li>在多人处理节点中尤其用到审核组件。</li>
</ul>
<%} %>


<% if (this.Request.QueryString["DoType"] == "SubFlow")
   { %>
<h5>子流程组件</h5>
<ul>
<li>在傻瓜表单里，启用、不启用在表单属性=》节点表单属性里设置。</li>
<li>该组件的高度也在表单属性设置。</li>
</ul>
<%} %>


<% if (this.Request.QueryString["DoType"] == "FrmThread")
   { %>
<h5>子线程组件</h5>
<ul>
<li>在傻瓜表单里，审核组件的启用、不启用在表单属性=》节点表单属性里设置。</li>
<li>该组件的高度也在表单属性设置。</li>
</ul>
<%} %>


<% if (this.Request.QueryString["DoType"] == "FrmTrack")
   { %>
<h5>轨迹组件</h5>
<ul>
<li>在傻瓜表单里，审核组件的启用、不启用在表单属性=》节点表单属性里设置。</li>
<li>该组件的高度也在表单属性设置。</li>
</ul>
<%} %>

</asp:Content>
