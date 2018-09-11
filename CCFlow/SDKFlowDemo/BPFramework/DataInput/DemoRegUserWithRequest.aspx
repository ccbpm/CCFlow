<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoRegUserWithRequest.aspx.cs" Inherits="CCFlow.SDKFlowDemo.DemoRegUserWithRequest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../../WF/Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../../WF/Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table border=1 align=center width="80%">
    <caption>用户注册(使用 BP.Sys.PubClass.CopyFromRequest 方法演示如何把数据写入到数据表里 )</caption>
     <tr>
    <td>帐号</td>
    <td>
        <asp:TextBox ID="TB_No" runat="server"></asp:TextBox>
        </td>
    <td>必须为字母或者数字下划线组合</td>
    </tr>
    <tr>
    <td>姓名</td>
    <td>
        <asp:TextBox ID="TB_Name" runat="server"></asp:TextBox>
        </td>
    <td></td>
    </tr>
    <tr>
    <td>密码</td>
    <td>
        <asp:TextBox ID="TB_PW" runat="server"></asp:TextBox>
        </td>
    <td>不能为空</td>
    </tr>
     <tr>
    <td>重输密码</td>
    <td>
        <asp:TextBox ID="TB_PW1" runat="server"></asp:TextBox>
         </td>
    <td>两次需要保持一致</td>
    </tr>
    <tr>
    <td>性别</td>
    <td>
        <asp:DropDownList ID="DDL_XB" runat="server">
            <asp:ListItem Value="1">男</asp:ListItem>
            <asp:ListItem Value="0">女</asp:ListItem>
        </asp:DropDownList>
        </td>
    <td></td>
    </tr>
    
    <tr>
    <td>年龄</td>
    <td>
        <asp:TextBox ID="TB_Age" runat="server" >0</asp:TextBox>
        </td>
    <td>输入int类型的数据</td>
    </tr>

    <tr>
    <td>地址</td>
    <td>
        <asp:TextBox ID="TB_Addr" runat="server"></asp:TextBox>
        </td>
    <td></td>
    </tr>

     <tr>
    <td>电话</td>
    <td>
        <asp:TextBox ID="TB_Tel" runat="server"></asp:TextBox>
         </td>
    <td></td>
    </tr>

    
     <tr>
    <td>邮件</td>
    <td>
        <asp:TextBox ID="TB_Email" runat="server"></asp:TextBox>
         </td>
    <td></td>
    </tr>

       <tr>
    <td colspan=3><asp:Button ID="Btn_Reg" runat="server" Text="注册新用户" 
            onclick="Btn_Reg_Click" /></td>
    </tr>


     <tr>
    <td colspan=3>
    说明：
    1， 此表单上的字段对应了类BP.Demo.BPFramework.BPUser 类，字段的命名格式按照BP框架的约定规则，文本类型的字段TB+"_" + 字段名.
    
    <br>
    2， 系统提交后，后台会根据Post过的的表单数据传入Entity 把数据赋值到实体类。

    </td>
    </tr>


    </table>

    </div>
    </form>
</body>
</html>
