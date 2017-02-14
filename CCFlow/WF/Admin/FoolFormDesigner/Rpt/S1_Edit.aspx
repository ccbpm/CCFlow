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
     <cc1:LinkBtn ID="Btn_New" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-new'"
        Text="创建新报表" OnClick="Btn_New_Click" />

    <cc1:LinkBtn ID="Btn_Save1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存" OnClick="Btn_Save_Click" />
    <cc1:LinkBtn ID="Btn_SaveAndNext1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-save'"
        Text="保存并设置显示列" OnClick="Btn_SaveAndNext1_Click" />
    <cc1:LinkBtn ID="Btn_Cancel1" runat="server" IsPlainStyle="false" data-options="iconCls:'icon-undo'"
        Text="取消" OnClick="Btn_Cancel_Click" />
    <br />
    <%
        string sql = "SELECT No,Name FROM Sys_MapData WHERE AtPara LIKE '%"+this.FK_Flow+"%'";
        System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        if (dt.Rows.Count == 0)
        {
            
         %>
        <table>
        <caption> 报表列表 </caption>

        <tr>
        <th> 编号</th>
        <th> 名称</th>
        <th> 操作</th>
        </tr>

        <% foreach (System.Data.DataRow dr in dt.Rows)
           {
               string no = dr["No"].ToString();
               %>
               <tr>
               <td> <%=dr["No"] %> </td>
               <td> <%=dr["Name"] %> </td>
               <td> <a href="" > 修改 </a> | <a href="" > 删除 </a> </td>
               </tr>
               <%
           } %>
        <tr>
        <td> </td>
        </tr>
        </table>

        <%} %>
</asp:Content>
