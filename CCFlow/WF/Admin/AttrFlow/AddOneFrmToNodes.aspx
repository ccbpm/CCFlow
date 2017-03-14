<%@ Page Title="增加一个表单到流程多个节点里" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="AddOneFrmToNodes.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.AddOneFrmToNodes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- edit by xushuhao -->

<%
    string flowNo = this.Request.QueryString["FK_Flow"];
    if (flowNo == null)
        flowNo = "001";
    
    BP.WF.Flow fl = new BP.WF.Flow(flowNo);
    
     %>
<table style="width:100%">
<caption > 增加一个表单到流程[<%=fl.Name %>]多个节点里 </caption>
<tr>
<td valign=top style="30%;">
<fieldset>
<legend>帮助</legend>
<ul>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
<li>sdsdsdsdsd</li>
</ul>
</fieldset>
</td>

<td valign="top" style="70%;" >

<fieldset>
<legend>请从表单库里选择一个要绑定的表单</legend>
<%
    //BP.Sys.SysFormTrees trees = new BP.Sys.SysFormTrees();
    //trees.RetrieveAll();
%>
</fieldset>


<fieldset>
<legend>选择要绑定的节点</legend>
<%
    //BP.WF.Nodes nds = new BP.WF.Nodes(flowNo);
    //nds.RetrieveAll();
%>
</fieldset>

<input type="Button" value="保存" />

</td>
</tr>
</table>


</asp:Content>
