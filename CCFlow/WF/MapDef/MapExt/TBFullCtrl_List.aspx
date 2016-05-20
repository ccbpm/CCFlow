<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="TBFullCtrl_List.aspx.cs" Inherits="CCFlow.WF.MapDef.TBFullCtrl_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
       <style type="text/css">
        .style1
        {
            width: 451px;
            height: 127px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style=" width:100%;">
<caption><a href="javascript:void(0)" onclick="history.go(-1)">返回</a>设置级连菜单 </caption>

<tr>
<td style="width:70%;"  valign="top"  >  
<fieldset>
    <div>发文密级<asp:Label ID="LabJLZD" runat="server" Text="Label"></asp:Label>字段</div>
<asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="5" ToolTip="点击标签显示帮助."  Width="95%"></asp:TextBox>
</fieldset> 
 </td>
</tr>
<tr>
<td colspan="2">
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
     <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭" 
        onclick="Btn_SaveAndClose_Click" />
    <input value="关闭" type="button"  onclick="javascript: window.close();" />
 </td>
</tr>



</table>
</asp:Content>
