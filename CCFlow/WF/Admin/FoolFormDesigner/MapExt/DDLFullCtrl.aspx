<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="DDLFullCtrl.aspx.cs" Inherits="CCFlow.WF.MapDef.DDLFullCtrlUIUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<table style=" width:100%;">
<caption>为下拉框【<%=this.Request.QueryString["RefNo"] %>】设置自动填充 </caption>
<tr>
<td class="Idx">1</td>
<td valign="top" colspan="2"  style=" width:100%;" >  
<a href="javascript:ShowHidden('sqlexp')" >自动填充SQL: </a>
 <div id="sqlexp" style="color:Gray; display:none">
 <ul>
 <li>设置一个查询的SQL语句，必须返回No,Name两个列，用于显示下拉列表，其他的列与字段名称对应以便系统自动对应填充。</li>
 <li>比如:SELECT Name as CZYName, Tel as DianHua, Email as YouJian FROM WF_Emp WHERE No like '@Key%' </li>
 <li>ccform为您准备了一个demo,请参考表单库\\本机数据源\\表单元素\\基础功能</li>
 <li><img alt="" src="../Img/TBCtrlFull.png" /></li>
 </ul>
 </div>
  <asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="5" ToolTip="点击标签显示帮助."  Width="95%"></asp:TextBox> 
 </td>
</tr>


<tr>
<td class="Idx">2</td>
<td valign="top" >  <a href="javascript:ShowHidden('dbsrc')" >执行SQL的数据源: </a>
<div id="dbsrc" style="color:Gray; display:none">
 <ul>
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


    <asp:Button ID="Btn_FullDtl" runat="server" Text="填充从表" 
        ToolTip="当数据填充后，需要改变指定的从表数据。比如：主表选择人员，从表人员简历信息。" onclick="Btn_FullDtl_Click"  />
    <asp:Button ID="Btn_FullDDL" runat="server" Text="填充下拉框"  
        ToolTip="当数据填充后，需要改变指定的下拉框内容。比如：选择人员后，有一个人员岗位的下拉框，该下拉框的内容仅仅需要显示人员岗位。" 
        onclick="Btn_FullDDL_Click"  />
    <asp:Button ID="Btn_Delete"  
        OnClientClick="javascript:return confirm('您确定要删除吗？');"  runat="server" 
        Text="删除" onclick="Btn_Delete_Click" />
  </td>
</tr>
</table>


</asp:Content>
