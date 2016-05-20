<%@ Page Title="" Language="C#" MasterPageFile="~/WF/SDKComponents/Site.Master" AutoEventWireup="true" CodeBehind="ShowMsg.aspx.cs" Inherits="ShenZhenGovOA.WF.SDKComponents.ShowMsg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

 <link href="../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
 <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" language="javascript" >
    if (window.opener && !window.opener.closed) {
        if (window.opener.name == "main") {
            window.opener.location.href = window.opener.location.href;
            window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
        }
    }
</script>
 
<table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td>
 
 <%=this.Info %>

 </td>
</tr>
</table>

</asp:Content>
