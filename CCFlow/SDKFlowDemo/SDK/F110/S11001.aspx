<%@ Page Title="" Language="C#" MasterPageFile="../Site.Master" AutoEventWireup="true" CodeBehind="S11001.aspx.cs" Inherits="CCFlow.App.F001.S101" %>
<%@ Register src="../../../WF/SDKComponents/DocMainAth.ascx" tagname="DocMainAth" tagprefix="uc2" %>
<%@ Register src="../../../WF/SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc3" %>
<%@ Register src="../../../WF/SDKComponents/TruakOnly.ascx" tagname="TruakOnly" tagprefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%@ Register src="../../../WF/SDKComponents/Toolbar.ascx" tagname="Toolbar" tagprefix="uc4" %>
<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc1" %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



<!-- 重写toolbar 预留的send 与save 两个方法。-->
<script type="text/javascript">
    function Save() {
        var btnSave = document.getElementById('ContentPlaceHolder1_BtnSave');
        btnSave.click();
    }
    function Send() {
        if (window.confirm('将要执行发送您确认吗？') == false)
            return;
        var btnSend = document.getElementById('ContentPlaceHolder1_BtnSend');
        btnSend.disable = true; //使其不能第2次按下此按钮.
        btnSend.click();
    }
</script>

<!-- 把这两个按钮隐藏起来，利用重写javascript 的send 与save 方法调用他们。 -->
<div style=" visibility:hidden">
<asp:Button ID="BtnSend" runat="server" Text="[Send]"  onclick="Btn_Send_Click" />
<asp:Button ID="BtnSave" runat="server" Text="[Save]"  onclick="Btn_Save_Click" />
</div>
<!-- 隐藏保存与发送按钮. -->


<!-- 嵌入式表单部分........................... -->
<table style="width:100%; text-align:left; background-color:White">
<tr>
<td>
<!-- 把工具栏从sdk组建库里引入进来, 该工具栏目上面的所有按钮都可以通过节点属性按钮区域配置. -->
    <uc4:Toolbar ID="Toolbar1" runat="server" />
</td>
</tr>
<tr>
<td  class="Title">
<fieldset>
<legend><font color=green ><b>请假基本信息</b></font></legend>
<table border="1" width="90%">
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
<!-- end 嵌入式表单部分........................... -->

 </td>
</tr>

<tr>
<td  class="Title"> 单附件</td>
</tr>

<tr>
<td> 
    <uc2:DocMainAth ID="DocMainAth1" runat="server" />
    </td>

</tr>

<tr>
<td class="Title"> 多附件</td>
</tr>

<tr>
<td> 
    <uc3:DocMultiAth ID="DocMultiAth1" runat="server" />
    </td>
</tr>


<!-- 审核组件 ....................  -->
<% 
    string str = this.Request.QueryString["FK_Node"];
    if (str != "11001")
    {
        /*如果不是开始节点，就不它显示审核按钮. */
     %>
    <tr>
   <td  class="Title" >审核组件</td>
    </tr>
<tr>
<td style=" width:100%; height:100%;"> 
     
    <uc1:FrmCheck ID="FrmCheck1" runat="server" />

</td>
</tr>
<%  } %>

<tr>
<td class="Title"> 流程轨迹图组件 </td>
</tr>

<tr>
<td>  
<uc5:TruakOnly ID="TruakOnly1" runat="server" />
    </td>
</tr>

</table>
 
</asp:Content>
