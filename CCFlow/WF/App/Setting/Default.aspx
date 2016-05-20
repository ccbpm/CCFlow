<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CCFlow.WF.App.Setting.Per" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

<table style=" white-space:100%; border:1px;" >
<caption class="CaptionMsgLong" >个人信息设置</caption>
<tr>
<td> 
<img src="/DataUser/UserIcon/<%=BP.Web.WebUser.No %>Small.png" alt='图标' onerror="this.src='/DataUser/UserIcon/DefaultBiger.png'" /> 

</td>

<td>

<%=BP.Web.WebUser.No %>
<br />
<%=BP.Web.WebUser.Name %>

</td>
</tr>


<tr>
<td>邮件</td>
<td></td>
</tr>


<tr>
<td>电话</td>
<td></td>
</tr>


<tr>
<td>部门</td>
<td></td>
</tr>



<tr>
<td>岗位</td>
<td></td>
</tr>



</table>


</asp:Content>
