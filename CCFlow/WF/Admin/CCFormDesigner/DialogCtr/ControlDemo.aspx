<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlDemo.aspx.cs" Inherits="CCFlow.WF.Admin.CCFormDesigner.DialogCtr.ControlDemo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:Button ID="Button1" runat="server" Text="Button" Height="31px" 
        BorderColor="#6666FF" ForeColor="#FF6699" />
    <br />
    <br />
    <asp:CheckBox ID="CheckBox1" runat="server" />
    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
    </asp:CheckBoxList>
    <asp:Label ID="Label1" runat="server" BackColor="#CCCCCC" Height="21px" 
        Text="Label" Width="1068px"></asp:Label>
    <br />
    <br />
    <asp:Label ID="Label2" runat="server" Text="Label水电费水电费是的范德萨范德萨"></asp:Label>
    <p>
    <asp:TextBox ID="TextBox1" runat="server" Height="31px" Width="923px"></asp:TextBox>
    </p>
    <p>
        <asp:RadioButton ID="RadioButton1" runat="server" GroupName="rd" Text="是" />
        <asp:RadioButton ID="RadioButton2" runat="server" GroupName="rd" Text="否" />
    </p>
    <p>
        <asp:RadioButtonList ID="RadioButtonList1" runat="server">
            <asp:ListItem>高</asp:ListItem>
            <asp:ListItem>中</asp:ListItem>
            <asp:ListItem>低</asp:ListItem>
        </asp:RadioButtonList>
    </p>
    <p>
        &nbsp;</p>
    </form>
</body>
</html>
