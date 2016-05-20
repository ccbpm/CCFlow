<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="AutoFullDLL.aspx.cs" Inherits="CCFlow.WF.MapDef.AutoFullDLLUIUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<table style=" width:100%;">
<caption>为下拉框“<%=this.Request.QueryString["RefNo"] %>”设置数据过滤. </caption>
<tr>
<td class="Idx"> 1 </td>
<td valign="top" colspan=2  style=" width:100%;" >  
<a href="javascript:ShowHidden('sqlexp')" >自动填充SQL: </a>
 <div id="sqlexp" style="color:Gray; display:none">
 <ul>

 <li>该功能的作用是，在表单装载的时候，该下拉框绑定的数据范围需要动态调整。</li>
 <li>比如当前是一个选择部门的下拉框，业务需求在该下拉框中仅仅显示我部门与隶属与我下一级部门的下拉框。</li>
 <li>设置参数为：SELECT No,Name  FROM Port_Dept WHERE ParentNo='@WebUser.FK_Dept' OR No='@WebUser.FK_Dept'</li>
 <li>提示：设置一个查询的SQL语句，必须返回No,Name两个列，ccform将会按照这个查询获得的数据源填充该下拉框。</li>
 <li>该参数支持ccbpm的表达式，什么是ccbpm表达式，请baidu ccbpm 表达式。</li>
 </ul>
 </div>
  <asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="5"
   ToolTip="点击标签显示帮助."  Width="95%"></asp:TextBox> 
 </td>
</tr>


<tr>
<td class="Idx">2</td>
<td valign="top" >  <a href="javascript:ShowHidden('dbsrc')" >执行SQL的数据源: </a>
<div id="dbsrc" style="color:Gray; display:none">
 <ul>
   <li>执行上述表达式的数据源.</li>
   <li>ccform支持从其它数据源里获取数据.</li>
   <li>您可以在系统维护里维护数据源.</li>
 </ul>
 </div>
</td>
<td  style="width:50%;">
 <asp:DropDownList ID="DDL_DBSrc" runat="server">
    </asp:DropDownList>
 </td>
</tr>

 
<tr>
<td colspan="3">
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" 
        onclick="Btn_SaveAndClose_Click" />
    <input value="关闭" type="button"  onclick="javascript:window.close();" />
    <asp:Button ID="Btn_Delete"  runat="server" Text="删除" 
        onclick="Btn_Delete_Click" /></td>
</tr>
</table>

</asp:Content>
