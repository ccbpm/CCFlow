<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/FoolFormDesigner/WinOpen.master" AutoEventWireup="true" CodeBehind="TBFullCtrl_Dtl.aspx.cs" Inherits="CCFlow.WF.MapDef.TBFullCtrl_Dtl" %>

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
<caption><a href="javascript:void(0)" onclick="history.go(-1)">返回</a>设置自动填充从表 </caption>

<tr>
<td style="width:70%;"  valign="top"  >  
<fieldset>
<legend><a href="javascript:ShowHidden('onblur')" >设置自动填充从表（点击查看帮助）: </a></legend>
    编号:<asp:Label ID="LabNo" runat="server" Text="Label"></asp:Label>,名称:<asp:Label ID="LabName" runat="server" Text="Label"></asp:Label>
<asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine"  Rows="5" ToolTip="点击标签显示帮助."  Width="95%"></asp:TextBox>
    <div>可填充的字段:<asp:Label ID="LabZD" runat="server" Text="Label"></asp:Label></div>
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
