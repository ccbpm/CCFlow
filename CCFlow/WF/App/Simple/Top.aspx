<%@ Page Title="" Language="C#" MasterPageFile="~/WF/App/Simple/SiteMenu.Master" AutoEventWireup="true" CodeBehind="Top.aspx.cs" Inherits="CCFlow.WF.App.Simple.Top" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<base target="_parent" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<br>

<div style="float:left;vertical-align:middle">
<%=BP.Sys.SystemConfig.SysName %>
 </div>

<h4>
<div style="float:right;vertical-align:middle">
 <% if (BP.Web.WebUser.No == null)
      { %>
    [<a href="Login.aspx" >登陆</a>] -  [<a href='/WF/Admin/Xap/Designer.aspx' target="_blank"  >进入流程设计器</a>]
   <% }
      else
      { %>
    [您好:<%=BP.Web.WebUser.No%>,<%=BP.Web.WebUser.Name%>] - [<a href="Login.aspx">重登陆</a>] -[<a href="Login.aspx?DoType=Out">退出</a>]- [<a href='/WF/Admin/Xap/Designer.aspx' target="_blank"  >进入流程设计器</a>]
    <%} %>
 </div>
</h4>


</asp:Content>
