<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" CodeBehind="SepcFiledsSepcUsersDtl.aspx.cs" Inherits="CCFlow.WF.Admin.AttrNode.SepcFiledsSepcUsersDtl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style="width:100%;">
<caption>分配给指定的用户</caption>

<%
    
    string mypk=this.Request.QueryString["MyPK"];

    BP.Sys.MapExt me = new BP.Sys.MapExt();
    if (mypk != null)
    {
        me.MyPK = mypk;
        me.RetrieveFromDBSources();
        this.TB_Emps.Text = me.Tag1;
        this.TB_SQL.Text = me.Tag2;
    }
    else
    {
        me.Doc= this.Request.QueryString["Fields"];
    }
     %>

<tr>
<th> 激活的字段集合 </th>
</tr>

<tr>
<td> <%=me.Doc %></td>
</tr>

<tr>
<th> 以下用户有权限启用字段(输入人员编号，多个人员用逗号分开,比如:zhangsan,lisi,wangwu,) </th>
</tr>

<tr>
<td>   
    <asp:TextBox ID="TB_Emps" runat="server" TextMode="MultiLine"  Rows=4 Columns="60" ></asp:TextBox>
    </td>
</tr>


<tr>
<th> 以下SQL表达式有权限启用字段(设置一个SQL,支持ccbpm表达式,返回一行一列，该值大于零就成立。) </th>
</tr>

<tr>
<td>   
    <asp:TextBox ID="TB_SQL" runat="server" TextMode="MultiLine" Rows=4 Columns="60" ></asp:TextBox>
    </td>
</tr>


<tr>
<th> 指定部门人员的权限(设置一个字符，是部门编号，多个部门使用逗号分开，比如：003,004, 表示该部门下的所有人员都可以激活该字段集合。) </th>
</tr>

<tr>
<td>   
    <fieldset>
    <legend>帮助</legend>

    <ul>
    <li>请设置一个部门，部门下所有的人员都可以处理该表单。</li>
    </ul>

    </fieldset>
    </td>
</tr>


<tr>
<td>   
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
    <asp:Button ID="Btn_Del" runat="server" Text="删除" onclick="Btn_Del_Click" />
    </td>
</tr>

</table>
</asp:Content>
