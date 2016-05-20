<%@ Page Title="1. 基本信息" Language="C#" MasterPageFile="RptGuide.Master" AutoEventWireup="true"
    CodeBehind="S1_Edit.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.NewOrEdit" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class='Table' cellpadding='0' cellspacing='0' style='width: 100%;'>
        <tr>
            <td class="GroupTitle">
                编号:
            </td>
            <td>
                <asp:TextBox ID="TB_No" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="GroupTitle">
                名称:
            </td>
            <td>
                <asp:TextBox ID="TB_Name" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="GroupTitle">
                备注:
            </td>
            <td>
                <asp:TextBox ID="TB_Note" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <cc1:LinkBtn ID="Btn_Save1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存" OnClick="Btn_Save_Click" />
    <cc1:LinkBtn ID="Btn_SaveAndNext1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存并设置显示列" OnClick="Btn_SaveAndNext1_Click" />
    <cc1:LinkBtn ID="Btn_Cancel1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-undo'"
        Text="取消" OnClick="Btn_Cancel_Click" />
</asp:Content>
