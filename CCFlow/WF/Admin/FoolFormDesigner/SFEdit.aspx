<%@ Page Title="创建一个外键字段" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="SFEdit.aspx.cs" Inherits="CCFlow.WF.Admin.FoolFormDesigner.SFEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
<!--  创建外键表/视图  -->
<fieldset>
<legend>   <asp:RadioButton ID="RB_0" GroupName="xxxx" runat="server" Text="创建外键表/视图" />  </legend>
<table style=" width:100%">
<tr>
<td>中文名</td>
<td> 
    <asp:TextBox ID="TB_Table_Name" runat="server"  
        ontextchanged="TB_Table_Name_TextChanged" AutoPostBack="True"></asp:TextBox>
    </td>
    <td>表的中文名。</td>
</tr>

<tr>
<td>数据表/视图: </td>
<td> 
    <asp:TextBox ID="TB_Table_No" runat="server"  Width="100%" ></asp:TextBox>
    </td>
    <td>在当前数据源下已经存在的，数据表或视图名称。</td>
</tr>

<tr>
<td>数据结构: </td>
<td> 
     
    <asp:DropDownList ID="DDL_Table_CodeStruct" runat="server">
    </asp:DropDownList>
    </td>
    <td>数据的编码格式</td>
</tr>

<tr>
<td>数据源: </td>
<td> 
     
    <asp:DropDownList ID="DDL_Table_DBSrc" runat="server">
    </asp:DropDownList>
     
    </td>
    <td>该表或视图存在那个数据源上？ 新建数据源。</td>
</tr>
</table>

</fieldset>






<!--  创建查询数据源  -->
<fieldset>
<legend>   <asp:RadioButton ID="RB_1" GroupName="xxxx" runat="server" Text="创建查询数据源" />  </legend>
<table style=" width:100%">

<tr>
<td>中文名</td>
<td> 
    <asp:TextBox ID="TB_SQL_Name" runat="server"></asp:TextBox>
    </td>
    <td>描述 </td>
</tr>


<tr>
<td>数据源标记: </td>
<td> 
    <asp:TextBox ID="TB_SQL_No" runat="server"></asp:TextBox>
    </td>
    <td>必须以英文字母或者下划线开头.</td>
</tr>

<tr>
<td colspan="4" >查询SQL(该SQL可以有变量:WebUser.No,WebUser.Name,WebUser.FK_Dept)</td>
</tr>

<tr>
<td colspan="4" >
    <asp:TextBox ID="TB_SQL" runat="server" Height="71px" TextMode="MultiLine" 
        Width="93%"></asp:TextBox>
    </td>
</tr>



<tr>
<td>数据结构: </td>
<td> 
     
    <asp:DropDownList ID="DDL_SQL_CodeStruct" runat="server">
    </asp:DropDownList>
    </td>
    <td>数据的编码格式</td>
</tr>



<tr>
<td>数据源: </td>
<td> 
    <asp:DropDownList ID="DDL_SQL_DBSrc" runat="server">
    </asp:DropDownList>
    </td>
    <td> </td>
</tr>

</table>
</fieldset>




<!--  创建webservices数据源  -->
<fieldset>
<legend>   <asp:RadioButton ID="RB_2" GroupName="xxxx" runat="server" Text="创建webservices数据源" />  </legend>


<table style=" width:100%">

<tr>
<td>中文名</td>
<td> 
    <asp:TextBox ID="TB_WS_Name" runat="server"></asp:TextBox>
    </td>
    <td>描述 </td>
</tr>


<tr>
<td>数据源标记: </td>
<td> 
    <asp:TextBox ID="TB_WS_No" runat="server"></asp:TextBox>
    </td>
    <td>必须以英文字母或者下划线开头.</td>
</tr>


<tr>
<td>约定参数: </td>
<td> 
    <asp:TextBox ID="TB_WS_Paras" runat="server"></asp:TextBox>
    </td>
    <td>必须以英文字母或者下划线开头.</td>
</tr>
 

 <tr>
<td>数据结构: </td>
<td> 
     
    <asp:DropDownList ID="DDL_WS_CodeStruct" runat="server">
    </asp:DropDownList>
    </td>
    <td>数据的编码格式</td>
</tr>


 

<tr>
<td>数据源: </td>
<td> 
     
    <asp:DropDownList ID="DDL_WS_DBSrc" runat="server">
    </asp:DropDownList>
     
    </td>
    <td>该表或视图存在那个数据源上？ 新建数据源。</td>
</tr>
</table>
</fieldset>



    <asp:Button ID="Btn_Create" runat="server" Text="创建" 
        onclick="Btn_Create_Click" />
    <asp:Button ID="Btn_Close" runat="server" Text="关闭" onclick="Btn_Close_Click" />




</asp:Content>
