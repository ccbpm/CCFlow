<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/QingJia/Site1.Master" AutoEventWireup="true" CodeBehind="S1_TianxieShenqingDan.aspx.cs" Inherits="CCFlow.SDKFlowDemo.QingJia.S1_TianxieShenqingDan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<fieldset>
<legend>请假基本信息</legend>
<table border="1" width="90%">
<tr>
<th>项目</th>
<th>数据</th>
<th>说明</th>
</tr>

<tr>
<td>请假人帐号:</td>
<td>
    <asp:TextBox ID="TB_No" ReadOnly=true runat="server" ></asp:TextBox>
    </td>
<td>(只读)当前登录人登录帐号BP.Web.WebUser.No</td>
</tr>


<tr>
<td>请假人名称:</td>
<td>
    <asp:TextBox ID="TB_Name" ReadOnly=true runat="server" > </asp:TextBox>
    </td>
<td>(只读)当前登录人名称BP.Web.WebUser.Name</td>
</tr>


<tr>
<td>请假人部门编号:</td>
<td>
    <asp:TextBox ID="TB_DeptNo" ReadOnly=true runat="server"  ></asp:TextBox>
    </td>
<td>(只读)当前登录人部门编号BP.Web.WebUser.FK_Dept</td>
</tr>

<tr>
<td>请假人部门名称:</td>
<td>
    <asp:TextBox ID="TB_DeptName" ReadOnly=true runat="server"  ></asp:TextBox>
    </td>
<td>(只读)当前登录人部门名称BP.Web.WebUser.FK_DeptName</td>
</tr>


<tr>
<td>请假天数:</td>
<td>
    <asp:TextBox ID="TB_QingJiaTianShu" runat="server" Text="0"  ></asp:TextBox>
    </td>
<td>请输入一个数字</td>
</tr>


<tr>
<td>请假原因:</td>
<td>
    <asp:TextBox ID="TB_QingJiaYuanYin" runat="server" Text="请输入请假原因..."  ></asp:TextBox>
    </td>
<td></td>
</tr>

</table>
</fieldset>

<fieldset>
<legend>功能操作区域</legend>
    <asp:Button ID="Btn_Send" runat="server" Text="发送(发送给部门经理审批)" 
        onclick="Btn_Send_Click" />
    <asp:Button ID="Btn_Save" runat="server" onclick="Btn_Save_Click" Text="保存" />

    <% 
     //   int workid = int.Parse(this.Request.QueryString["WorkID"]);
        //   var url = "javascript:window.open('/WF/WFRpt.htm?WorkID=" + workid + "')";
         %>
    <asp:Button ID="Btn_Track" runat="server" Text="流程图" 
        onclick="Btn_Track_Click" />
</fieldset>



<fieldset>
<legend>URL传值</legend>
<font color=blue>
<%=this.Request.RawUrl %>
</font>

我们要注意这个URL,它是流程引擎经过处理过以后的Url ,加上了很多的系统参数，开发人员可以根据这些系统参数实现自己的业务。


</fieldset>

</asp:Content>
