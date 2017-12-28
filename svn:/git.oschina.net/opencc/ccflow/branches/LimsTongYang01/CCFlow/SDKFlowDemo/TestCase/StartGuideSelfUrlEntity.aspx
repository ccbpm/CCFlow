<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/WinOpen.master" AutoEventWireup="true" CodeBehind="StartGuideSelfUrlEntity.aspx.cs" Inherits="CCFlow.app.Guide" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style=" width:100%;">
<caption>前置导航 </caption>
<tr>
<td>请输入身份证号</td>
<td> 
    <asp:TextBox ID="TB_SFZ" runat="server" Width="363px"></asp:TextBox>
    </td>
<td> 
    <asp:Button ID="Btn_Search" runat="server" Text="查询..." 
        onclick="Btn_Search_Click" />
    </td>
    
    <td>
      <asp:Button ID="Btn_Start" runat="server" Text="确定并发起流程" 
            onclick="Btn_Start_Click"  />
    </td>
</tr>

<tr>
<td>姓名</td>
<td>
    <asp:TextBox ID="TB_Name" runat="server"></asp:TextBox></td>
<td>性别</td>
<td>
    <asp:TextBox ID="TB_XB" runat="server"></asp:TextBox></td>
</tr>

<tr>
<td>出生年月</td>
<td>
    <asp:TextBox ID="TB_BDT" runat="server"></asp:TextBox></td>
<td>年龄</td>
<td>
    <asp:TextBox ID="TB_Age" runat="server"></asp:TextBox></td>
</tr>

<tr>
<td>地址</td>
<td colspan=3>
    <asp:TextBox ID="TB_Addr" runat="server" Width="806px"></asp:TextBox></td>
</tr>

<tr>
<td>电话</td>
<td>
    <asp:TextBox ID="TB_Tel" runat="server"></asp:TextBox></td>
<td>邮件</td>
<td>
    <asp:TextBox ID="TB_Email" runat="server"></asp:TextBox></td>
</tr>



<uc1:Pub ID="Pub1" runat="server" />

    
    
<tr>
<td colspan="4">
    <asp:Button ID="Btn_SaveToEntity" runat="server" Text="保存到数据库" onclick="Btn_SaveToEntity_Click" 
        /></td>
</tr>

</table>

 

 
</asp:Content>
