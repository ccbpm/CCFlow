<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CCFlow.WF.Admin.Clound.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<div id="LoadingBar" style="margin-left: auto; margin-right: auto; margin-top: 40%;
    width: 250px; height: 38px; line-height: 38px; padding-left: 50px; padding-right: 5px;
    background: url(../../Scripts/easyUI/themes/default/images/pagination_loading.gif) no-repeat scroll 5px 10px;
    border: 2px solid #95B8E7; color: #696969; font-family: 'Microsoft YaHei'">
    正在连接到云服务器,请稍候…
</div>
<head runat="server">
    <title></title>
    <link href="../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="js/loading.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
        }
        .panel
        {
            position: absolute;
            top: 40%;
            height: 400px;
            margin: 0 auto;
            margin-top: -120px;
            left: 45%;
            right: 50%;
            margin-left: -150px;
            height: 500px;
        }
        .textbox
        {
            height: 20px;
            margin: 0;
            padding: 0 2px;
            box-sizing: content-box;
        }
    </style>
</head>
<body>
    <div class="easyui-panel divcenter" title="用户登陆" style="width: 420px;">
        <div style="padding: 10px 60px 20px 60px">
            <form id="ff" method="post" runat="server">
            <table cellpadding="5">
                <tr>
                    <td>
                        账号:
                    </td>
                    <td>
                        <asp:TextBox ID="TB_No" runat="server" class="easyui-validatebox textbox" data-options="required:true"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        密码:
                    </td>
                    <td>
                        <asp:TextBox ID="pwd" runat="server" class="easyui-validatebox textbox" 
                            data-options="required:true" TextMode="Password"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="Label2" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <input id="Checkbox1" type="checkbox" checked="checked" />
                        保存登陆状态
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <div style="text-align: center;">
                <asp:Button ID="BtnLogin" class="easyui-linkbutton" runat="server" Text="登陆" Height="28px"
                    Width="43px" OnClick="BtnLogin_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="BtnRegUser" class="easyui-linkbutton" runat="server" Text="注册" Height="28px"
                    Width="43px" OnClick="BtnRegUser_Click" />
            </div>
            </form>
        </div>
    </div>
</body>
</html>
