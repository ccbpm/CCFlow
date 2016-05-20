<%@ Page Title="" Language="C#" MasterPageFile="~/WF/App/Setting/Site.Master" AutoEventWireup="true" CodeBehind="AutoDtl.aspx.cs" Inherits="CCFlow.WF.App.Setting.AutoDtl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
<table > 
         <caption class="CaptionMsgLong">
                授权详细信息</caption>
 <tr> 
<td  style=" width:200px"> 项目</td>
<td style =" width:200px">内容</td>
</tr>
<tr> 
<td> 授权给:</td>
  
<td>  <asp:Label ID="sqg" runat="server" Text="Label"></asp:Label></td>
</tr>
<tr>
<td>收回授权日期:</td>
<td><asp:Label ID="sqrq" runat="server" Text="Label"></asp:Label></td>
</tr>
<tr>
<td>授权方式:</td>
<td>
<select   runat="server" id="sel"> 
<option value="0">不授权</option> 
<option value="1">全部流程授权</option> 
<option value="2">指定流程授权</option> 
</select> 
</td>   
</tr>
<tr>
<td><a href="javascript:showModalDialog('../../ToolsSet.aspx?RefNo=AthFlows')" >设置授权的流程范围</a></td>
<td> <asp:Button ID="BtnSave" runat="server" Text="保存" onclick="BtnSave_Click" /></td>
</tr>
<tr><td  colspan="2">说明:在您确定了收回授权日期后，被授权人不能再以您的身份登陆，
如果未到<br />指定的日期您可以取回授权。</td>
</tr>
</table>
</asp:Content>
