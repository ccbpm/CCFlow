<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="CheckFlow.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.CheckFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%
    string flowNo = this.Request.QueryString["FK_Flow"];
    BP.WF.Flow fl = new BP.WF.Flow(flowNo);
 %>

 
 <table style="width:100%;">
 <caption> 流程检查 </caption>
 <tr>
 <td style="width:30%;" valign="top" >

 <fieldset>
 <legend>帮助</legend>
 <ol>
 <li>创建没有创建的表单数据表</li>
 <li>修复表单字段与表字段的不一致性，比如设计的字段的类型与长度发生变化与数据表不一致，系统会自动修复。</li>
 <li>检查流程设计的错误，对于检查错误的，就标识该流程设计的有问题，不符合规则。</li>
 <li>执行流程模版的升级</li>
 </ol>
 </fieldset>

 </td>
 
 <td>
 <fieldset>
 <legend>流程检查信息</legend>
 <%
     string str = fl.DoCheck();
     str = str.Replace("@", "<BR>@");
  %>
 <%= str%>
 </fieldset>
 </td>
 </tr>


 </table>

</asp:Content>
