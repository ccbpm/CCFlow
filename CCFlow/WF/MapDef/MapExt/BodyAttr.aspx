<%@ Page Title="表单属性" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="BodyAttr.aspx.cs" Inherits="CCFlow.WF.MapDef.BodyAttr" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<table style="width:100%;">
<caption>表单body属性</caption>
<tr>
<td> 
    <asp:TextBox ID="TB_Attr" runat="server" TextMode="MultiLine" Height="88px" Width="522px"></asp:TextBox>
    </td>
</tr>

<tr>
<td> 
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    </td>
</tr>

</table>

</asp:Content>
