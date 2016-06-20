<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="S12901.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F129.S12901" %>
<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>嵌入式表单</title>
</head>
<body>
    <form id="form1" runat="server"  >
   <script type="text/javascript">
       /*
       *  保存方法demo: 
       *  1, 该方法的方法名与ccflow &jFlow 约定好的，如果保存成功就返回true,用户就可以执行发送.
       *  2, 如果失败就返回false引擎就终止发送操作.
       */
       function Save() {
           try {

               var ts = document.getElementById('TB_QingJiaTianShu').value;
               if (ts == 0) {
                   alert('请假天数不能为零。');
                   return false;
               }

               document.getElementById('Btn_Save').click(); //调用btn_save事件.
               alert('save ok');
               return true; //保存成功，用户可以发送.
           } catch (e) {
               alert(e.name + " :  " + e.message);
               return false; // 保存失败不能发送.
           }
       }
    </script>

<!-- 保存按钮让其隐藏 ............................ -->
  <div style="display:none ">
    <asp:Button ID="Btn_Save" runat="server" Text="Save" onclick="Btn_Save_Click" />
    </div>
  

<!-- 嵌入式表单部分........................... -->
<table style="width:100%; text-align:left; background-color:White">
<caption style=" text-align:left;">嵌入式表单演示 - 请假基本信息</caption>
<tr>
<td>
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
    <asp:TextBox ID="TB_QingJiaTianShu" runat="server" Text="0" Enabled=true  ></asp:TextBox>
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
<!-- end 嵌入式表单部分........................... -->

 </td>
</tr>


<!-- 审核组件, 非开始节点不让其显示审核组件 ....................  -->
<% 
    string str = this.Request.QueryString["FK_Node"];
    if (str != "12901")
    {
       /*如果不是开始节点，就不它显示审核组件. */
     %>
<tr>
<td> 
<fieldset>
<legend><font color=green ><b>审批区域</b></font></legend>
    <uc1:FrmCheck ID="FrmCheck1" runat="server" />
</fieldset>
</td>
</tr>
<%  } %>

</table>


<fieldset>
<legend> 开发说明：</legend>
<ul>
<li>您所看到的该表单demo位于：<%=BP.Sys.SystemConfig.PhysicalApplicationPath %>SDKFlowDemo\\SDK\F129\\S12901.aspx</li>
<li>您可以打开该文件仔细的看看文档说明。</li>
<li>该文件中有一个Save 的javascript方法，这是ccbpm与您执行保存动作的约定，在用户点击发送或者保存的时候，就会触发该方法。</li>
<li>该方法如果保存成功就返回true, 否则返回fase.  在用户点击发送按钮的时候，你返回false 就标识保存错误ccbpm就不能发送。</li>
</ul>
</fieldset>
     
    </form>
</body>
</html>
