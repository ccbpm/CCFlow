<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="TruckViewPower.aspx.cs" 
Inherits="CCFlow.WF.Admin.AttrFlow.UITruckViewPower" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%; padding-left:19px;" >
<caption>流程轨迹查看权限</caption>

 
<tr>
<td colspan="1"  valign="top" rowspan="9" style="width:25%;">

<fieldset> 
<legend>帮助</legend>
<ol>
<li>该设置为了控制该流程的流程实例是可以被那些范围的人可查询，可见。</li>
<li>该控制在工作流程查看器里校验，就是说在操作员要打开工作查看器的时候检查当前的人员是否有权限查看该流程。</li>
<li>与流程相关的人员是必选项，也就是说，流程相关的人员是可以查看轨迹图的，</li>
</ol>

</fieldset>

</td>

<th colspan="4">必选项</th>

</tr>

<tr>
<td> 
    <asp:CheckBox ID="CB_FQR" runat="server" Enabled="false" Text="发起人可见" /> </td>
<td>
    <asp:CheckBox ID="CB_CYR" runat="server"  Enabled="false"  Text="参与人可见" /> </td>
<td>
    <asp:CheckBox ID="CB_CSR" runat="server" Enabled="false"  Text="被抄送人可见"/> </td>
<td>
     </td>
</tr>

<tr>
<th colspan="4"> 按照部门划分 </th>
</tr>

<tr>
<td> 
    <asp:CheckBox ID="CB_BBM" runat="server" Text="本部门人可看" /> 
    </td>
<td>
     <asp:CheckBox ID="CB_ZSSJ" runat="server"  Text="直属上级部门可看(比如:我是)" />
    </td>
<td>
     <asp:CheckBox ID="CB_SJ" runat="server"  Text="上级部门可看"/> 
    </td>
<td>
   <asp:CheckBox ID="CB_PJ" runat="server"  Text="平级部门可看"/> 
    </td> 
</tr>

<tr>
<td> 
    <asp:CheckBox ID="QY_ZDBM" runat="server"  />指定的部门可见 </td>
<td colspan="3">
    部门编号&nbsp&nbsp<asp:TextBox ID="TB_ZDBM" runat="server"></asp:TextBox>
      </td>
</tr>

<tr>
<th colspan="4">按照其他方式指定 </th>
</tr>


<tr>
<td> <asp:CheckBox ID="QY_ZDGW" runat="server" /> 指定的岗位可见 </td>
<td colspan="3">
    岗位编号&nbsp&nbsp<asp:TextBox ID="TB_ZDGW" runat="server"></asp:TextBox>
      </td>
</tr>

<tr>
<td>  <asp:CheckBox ID="QY_ZDQXZ" runat="server" /> 指定的权限组可看 </td>
<td colspan="3">
    权限组&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp<asp:TextBox ID="TB_ZDQXZ" runat="server"></asp:TextBox>
      </td>
</tr>

<tr>
<td> <asp:CheckBox ID="QY_ZDRY" runat="server"  /> 指定的人员可看 </td>
<td colspan="3">
    指定人员编号&nbsp&nbsp&nbsp<asp:TextBox ID="TB_ZDRY" runat="server"></asp:TextBox>
      </td>
</tr>
</table>

<div style=" text-align:center">
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    </div>
<%--
    <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" 
        onclick="Btn_SaveAndClose_Click" />
    <asp:Button ID="Btn_Close" runat="server" Text="关闭" onclick="Btn_Close_Click" />--%>
</asp:Content>
    