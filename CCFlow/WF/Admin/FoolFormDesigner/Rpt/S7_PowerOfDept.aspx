<%@ Page Title="" Language="C#" MasterPageFile="../WinOpen.master" AutoEventWireup="true" CodeBehind="S7_PowerOfDept.aspx.cs" Inherits="CCFlow.WF.MapDef.Rpt.S7_PowerFlowDeptEmp" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

选择控制类型:
<asp:DropDownList ID="DDL_DeptPowerType" runat="server">
    <asp:ListItem Value="0">只能查看本部门</asp:ListItem>
    <asp:ListItem Value="1">查看本部门与下级部门</asp:ListItem>
    <asp:ListItem Value="2">按指定该流程的部门人员权限控制</asp:ListItem>
    <asp:ListItem Value="3">不控制，任何人都可以查看任何部门的数据.</asp:ListItem>
    </asp:DropDownList>
    
SELECT * FROM WF_DeptFlowSearch
<uc1:Pub ID="Pub2" runat="server" />
     
    <br />
    <asp:Button ID="Btn_Save" runat="server" Text="SaveAndClose" OnClick="Btn_Save_Click" />
    <asp:Button ID="Btn_SaveAndNext" runat="server" Text="SaveAndNext" OnClick="Btn_Save_Click" />
    <asp:Button ID="Btn_Close" runat="server" Text="Cancel" 
    onclick="Btn_Close_Click" />

</asp:Content>
