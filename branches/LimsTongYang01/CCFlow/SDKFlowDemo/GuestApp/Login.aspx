<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CCFlow.SDKFlowDemo.GuestApp.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/login.css" rel="stylesheet" type="text/css" />
    <link href="css/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../WF/Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../WF/Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            document.onkeydown = function (event) {
                var e = event || window.event || arguments.callee.caller.arguments[0];
                if (e && e.keyCode == 13) { // enter 键
                    document.getElementById('<%= userlogin.ClientID %>').click();
                }
            };
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="login">
        <div id="top">
            <div id="top_left">
                <img src="images/login/login_03.gif" /></div>
            <div id="top_center">
            </div>
        </div>
        <div id="center">
            <div id="center_left">
            </div>
            <div id="center_middle">
                <div id="user">
                    <input type="text" runat="server" style="width: 180px; text-indent: 20px; border-radius: 5px;"
                        name="UserName" id="UserName" />
                    <img src="images/man.png" style="position: relative; left: -185px; top: 2px; margin-left: 2px;" />
                </div>
                <div id="password">
                    <input type="password" style="width: 180px; text-indent: 20px; border-radius: 5px;"
                        name="PassWord" id="PassWord" runat="server" />
                    <img src="images/lock.png" style="position: relative; left: -185px; top: 2px; margin-left: 2px;" />
                </div>
                <div id="btn">
                    <a href="javascript:;" id="userlogin" runat="server" onserverclick="Btn_Login_Click1"
                        class="easyui-linkbutton" data-options="iconCls:'icon-ok'" style="padding: 5px 0px;">
                        <span style="font-size: 14px;">Guest用户登录</span> </a>
                </div>
            </div>
            <div id="center_right">
            </div>
        </div>
        <div id="down">
            <div id="down_left">
                <div id="inf">
                    <span class="inf_text">版本信息</span> <span class="copyright">肯拉铎 2016 v1.0</span>
                </div>
            </div>
            <div id="down_center">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
