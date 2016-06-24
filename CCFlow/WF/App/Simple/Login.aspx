<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CCFlow.WF.App.Simple.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ccflow登录demo</title>
    <link href="../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align:center">
<h2>用户登录</h2>
   <fieldset>
    <legend> ccflow登录的API在Button事件后面 </legend>
    <br>
  用户名:  <asp:TextBox ID="TB_No" runat="server"></asp:TextBox>
  <br>
  密码： <asp:TextBox ID="TB_Pass" runat="server"></asp:TextBox>  <asp:Button ID="Button1" runat="server" Text=" 登录 " 
    onclick="Button1_Click" />

    <font  color=gray> 默认密码为:123 或者 pub</font>
    <br>
    <br>
    </fieldset>

    <fieldset>
    <legend>说明:</legend>
    <ul>
     <li>1, 登录界面是有您来编写的。 </li>
     <li>2, 密码的验证，登录前后要处理的业务逻辑均有您来开发。 </li>
     <li>3, 验证成功后，就调用ccflow的登录API。 </li>
    </ul>
    </fieldset>

    </div>
    </form>
</body>
</html>
