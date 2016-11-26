<%@ Page Title="" Language="C#" MasterPageFile="SDKComponents/Site.Master"
 AutoEventWireup="true" Inherits="CCFlow.WF.WF_JumpWay" Codebehind="JumpWay.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
   <link href="./Comm/Style/Table.css" rel="stylesheet" type="text/css" />
   <link href="./Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

 <table style=" text-align:left; width:100%">
<caption>您好:<%=BP.WF.Glo.GenerUserImgSmallerHtml(BP.Web.WebUser.No,BP.Web.WebUser.Name) %></caption>
<tr>
<td valign="top" style="text-align:center;">
    <br>
    <br>
 
 <center>
 <table style=" text-align:center; width:500px">
 <tr>
 <th>请选择要跳转的节点</th>
 </tr>
<tr>
<td valign="top" style="text-align:center">
        <uc2:Pub ID="Pub1" runat="server" />
         </td>
         </tr>
         </table>
    </div>
    
    <br>
    <br>
    <br>
    <br>
    <br>
    <br>
    <br>
    <br>
    <br>
    
    </td>
</tr>
</table>
</center>

    
</asp:Content>

