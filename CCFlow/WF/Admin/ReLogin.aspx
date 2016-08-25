<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReLogin.aspx.cs" Inherits="CCFlow.WF.Admin.ReLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <center>
    <div style="text-align:center">
    <table>
    <caption>登录信息丢失，需要重新登录。</caption>
    <tr>
    <td>用户名 </td>
    <td> 
        <asp:TextBox ID="TB_UserNo" runat="server"></asp:TextBox>
        </td>
    </tr>

    <tr>
    <td>密 码： </td>
    <td> 
        <asp:TextBox ID="TB_Pass" runat="server"></asp:TextBox>
        </td>
    </tr>

    <tr>
    <td colspan=2> 
        <asp:Button ID="Btn_Login" runat="server" Text="重新登录" 
            onclick="Btn_Login_Click" />
        </td>
    </tr>

    </table>

    
    </div>

    </center>

    </form>
</body>
</html>
