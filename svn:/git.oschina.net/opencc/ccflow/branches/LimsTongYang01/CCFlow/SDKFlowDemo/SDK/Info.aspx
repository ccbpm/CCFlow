<%@ Page Title="信息提示" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Info.aspx.cs" Inherits="CCFlow.App.SDK.Info" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
 <br>
 <br>
 <br>

 <fieldset>
 <legend>信息提示</legend>

 <div style=" float:left; text-align:left">
 <%
     string msg = BP.WF.Glo.SessionMsg;     
     string s = Application["info" + BP.Web.WebUser.No] as string;
     if (s == null)
         s = this.Session["info"] as string;
     if (s == null)
         s = "提示信息丢失了.";
     s = s.Replace("@", "<br>@");
      %>

     <%=s %>
     </div>
 </fieldset>

 </asp:Content>
