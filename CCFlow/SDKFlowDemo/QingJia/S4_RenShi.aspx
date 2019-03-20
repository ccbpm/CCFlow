<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/QingJia/Site1.Master"
    AutoEventWireup="true" CodeBehind="S4_RenShi.aspx.cs" Inherits="CCFlow.SDKFlowDemo.QingJia.S4_RenShi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend><font color="blue"><b>请假基本信息</b></font> </legend>
        <table border="1" width="700px">
            <tr>
                <th>
                    项目
                </th>
                <th>
                    数据
                </th>
                <th>
                    说明
                </th>
            </tr>
            <tr>
                <td>
                    请假人帐号:
                </td>
                <td>
                    <asp:TextBox ID="TB_No" ReadOnly="true" runat="server"></asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
            <tr>
                <td>
                    请假人名称:
                </td>
                <td>
                    <asp:TextBox ID="TB_Name" ReadOnly="true" runat="server"> </asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
            <tr>
                <td>
                    请假人部门编号:
                </td>
                <td>
                    <asp:TextBox ID="TB_DeptNo" ReadOnly="true" runat="server"></asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
            <tr>
                <td>
                    请假人部门名称:
                </td>
                <td>
                    <asp:TextBox ID="TB_DeptName" ReadOnly="true" runat="server"></asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
            <tr>
                <td>
                    请假天数:
                </td>
                <td>
                    <asp:TextBox ID="TB_QingJiaTianShu" runat="server" Text="0"></asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
            <tr>
                <td>
                    请假原因:
                </td>
                <td>
                    <asp:TextBox ID="TB_QingJiaYuanYin" runat="server" Text="请输入请假原因..."></asp:TextBox>
                </td>
                <td>
                    (只读)
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <legend><font color="blue"><b>部门经理审核信息</b></font></legend>
        <asp:TextBox ID="TB_BMNote" ReadOnly="true" TextMode="MultiLine" runat="server" Height="93px"
            Width="682px"></asp:TextBox>
    </fieldset>
    <fieldset>
        <legend><font color="blue"><b>人力资源经理审核信息</b></font></legend>
        <asp:TextBox ID="TB_NoteRL" TextMode="MultiLine" runat="server" Height="93px" Width="682px"></asp:TextBox>
    </fieldset>
    <fieldset>
        <legend><font color="blue"><b>功能操作区域</b></font></legend>
        <asp:Button ID="Btn_Send" runat="server" Text="发送" OnClick="Btn_Send_Click" />
        <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
        <asp:Button ID="Btn_Return" runat="server" OnClick="Btn_Return_Click" Text="退回" />
        <asp:Button ID="Btn_Track" runat="server" Text="流程图" OnClick="Btn_Track_Click" />
    </fieldset>
    <fieldset>
        <legend>URL传值</legend><font color="blue">
            <%=this.Request.RawUrl %>
        </font>
    </fieldset>
</asp:Content>
