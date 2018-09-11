<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestFlow.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   学号： <asp:TextBox ID="TB_No" runat="server"></asp:TextBox>
   密码： <asp:TextBox ID="TB_PW"  TextMode=Password runat="server"></asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="登陆学生请假系统" 
        onclick="Button1_Click" />
    <div>
    
    </div>
    </form>
</body>
</html>
