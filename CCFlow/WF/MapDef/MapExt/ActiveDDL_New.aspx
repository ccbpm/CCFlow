<%@ Page Title="联动下拉框" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="ActiveDDL.aspx.cs" Inherits="CCFlow.WF.MapDef.ActiveDDLUIUIs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%;">
<caption>为下拉框【<%=this.Request.QueryString["RefNo"] %>】设置联动. </caption>
<tr>
<td class=Idx> 1 </td>
<td > 

<a href="javascript:ShowHidden('ldx')" >要联动的下拉框: </a>
 <div id="ldx" style="color:Gray; display:none">
 <ul>
   <li>以当前选择的下来框为主动项，选择的项目为从动项.</li>
   <li>比如:省份与城市，主动项是省份，从动项是城市。大类别与小类别。</li>
 </ul>
 </div>

</td>

<td>
   <asp:DropDownList ID="DDL_List" runat="server">
    </asp:DropDownList>
 </td>
 </tr>

<tr>
<td class=Idx> 2 </td>
<td>  

 <a href="javascript:ShowHidden('dbsrc')" >选择数据源: </a>
 <div id="dbsrc" style="color:Gray; display:none">
 <ul>
   <li>ccform支持从其它数据源里获取数据.</li>
   <li>您可以在系统维护里维护数据源.</li>
 </ul>
 </div>
</td> 
 
 <td>  
  <asp:DropDownList ID="DDL_DBSrc" runat="server">
    </asp:DropDownList>
</td> 

</tr>
<tr>
<td class=Idx> 3 </td>

<td  colspan="2"> 
 <a href="javascript:ShowHidden('mysql')" >请设置一个SQL: </a>
 <div id="mysql" style="color:Gray; display:none">
 <ul>
   <li>该SQL支持ccbpm的表达式.</li>
   <li>SQL中必须有@Key参数，就是上一个发生变化时的item值。</li>
   <li>比如：SELECT No, Name FROM CN_City WHERE FK_SF = '@Key' </li>
   <li>@Key 参数就是下拉框选择的值。 </li>
 </ul>
 </div>
  <asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="3" ToolTip="点击标签显示帮助."  Width="95%"></asp:TextBox> 
 </td>
</tr>


<tr>
<td class=Idx> 4 </td>
<td colspan="2">
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" 
        onclick="Btn_SaveAndClose_Click" />
    <input value="关闭" type="button"  onclick="javascript:window.close();" />

    <asp:Button ID="Btn_Delete" runat="server" Text="删除" 
        OnClientClick="return confirm('您确定要删除吗？');" 
        onclick="Btn_Delete_Click"   />
 </td>
</tr>

</table>

</asp:Content>
