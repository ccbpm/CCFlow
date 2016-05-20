<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegUser.aspx.cs" Inherits="CCFlow.WF.Admin.Clound.RegUser" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<div id="LoadingBar" style="margin-left: auto; margin-right: auto; margin-top: 40%;
    width: 250px; height: 38px; line-height: 38px; padding-left: 50px; padding-right: 5px;
    background: url(../../Scripts/easyUI/themes/default/images/pagination_loading.gif) no-repeat scroll 5px 

10px;
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
    <script type="text/javascript">
        function clearForm() {
            $('#ff').form('clear');
        }
    </script>
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
            left: 50%;
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
    <div class="easyui-panel divcenter" title="注册用户" style="width: 420px;">
        <div style="padding: 10px 60px 20px 60px">
            <form id="ff" method="post" runat="server">
            <table cellpadding="5">
                <tr>
                    <td>
                        账号:
                    </td>
                    <td>
                        <input id="userNo" class="easyui-validatebox textbox" missingmessage="最少6个字符" type="text"
                            name="userNo" data-options="required:true,validType:'zhanghao'"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        姓名:
                    </td>
                    <td>
                        <input class="easyui-validatebox textbox" missingmessage="姓名必须填写" type="text" name="userName"
                            data-options="required:true"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        密码:
                    </td>
                    <td>
                        <input id="pwd" class="easyui-validatebox textbox" missingmessage="登陆密码必须填写" type="password"
                            name="password" data-options="required:true"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        重复密码:
                    </td>
                    <td>
                        <input id="rpwd" class="easyui-validatebox textbox" missingmessage="重新输入登陆密码" type="password"
                            name="repeatPassword" data-options="required:true" validtype="equals['#pwd']"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        E-mail:
                    </td>
                    <td>
                        <input class="easyui-validatebox textbox" name="email" data-options="required:true,validType:'email'"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        Tel:
                    </td>
                    <td>
                        <input class="easyui-validatebox textbox" missingmessage="请输入手机号" name="tel" data-options="required:true,validType:'phoneRex'"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        QQ:
                    </td>
                    <td>
                        <input class="easyui-numberbox textbox" missingmessage="请输入QQ号" name="qq" data-options="required:true"></input>
                    </td>
                </tr>
                <tr>
                    <td>
                        您的身份:
                    </td>
                    <td>
                        <asp:DropDownList ID="DDL_UserType" runat="server" Style="width: 153px;" CssClass="combobox-item-hover">
                            <asp:ListItem Value="0">IT公司职员</asp:ListItem>
                            <asp:ListItem Value="1">企业信息部职员</asp:ListItem>
                            <asp:ListItem Value="2">事业政府单位</asp:ListItem>
                            <asp:ListItem Value="3">高校教师</asp:ListItem>
                            <asp:ListItem Value="4">学生</asp:ListItem>
                            <asp:ListItem Value="4">其他</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        学习目的:
                    </td>
                    <td>
                        <asp:DropDownList ID="DDL_UserTarget" runat="server" Style="width: 153px;" CssClass="combobox-item-hover">
                            <asp:ListItem Value="0">为单位技术选型作评估</asp:ListItem>
                            <asp:ListItem Value="1">想应用到本单位系统里</asp:ListItem>
                            <asp:ListItem Value="2">想用它开发项目</asp:ListItem>
                            <asp:ListItem Value="3">WorkFlow爱好者</asp:ListItem>
                            <asp:ListItem Value="4">想找到更好工作</asp:ListItem>
                            <asp:ListItem Value="4">其他</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div style="text-align: center; padding: 5px">
                <asp:Button ID="BtnRegUser" class="easyui-linkbutton" runat="server" Text="注册" Height="28px"
                    Width="43px" OnClick="BtnRegUser_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript:void(0)"
                        class="easyui-linkbutton" onclick="clearForm()">重置</a>
            </div>
            </form>
        </div>
    </div>
    <script type="text/javascript">


        //自定义验证
        $.extend($.fn.validatebox.defaults.rules, {
            phoneRex: {
                validator: function (value) {
                    var rex = /^1[3-8]+\d{9}$/;
                    //var rex=/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
                    //区号：前面一个0，后面跟2-3位数字 ： 0\d{2,3}
                    //电话号码：7-8位数字： \d{7,8
                    //分机号：一般都是3位数字： \d{3,}
                    //这样连接起来就是验证电话的正则表达式了：/^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/		 
                    var rex2 = /^((0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/;
                    if (rex.test(value) || rex2.test(value)) {
                        // alert('t'+value);
                        return true;
                    } else {
                        //alert('false '+value);
                        return false;
                    }

                },
                message: '请输入正确电话或手机格式'
            }
        });
        $.extend($.fn.validatebox.defaults.rules, {
            zhanghao: {
                validator: function (value) {
                    var rex = /^[a-zA-z]\w{6,16}$/;
                    if (rex.test(value)) {
                        //alert('t'+value);
                        return true;
                    } else {
                        //alert('false '+value);
                        return false;
                    }

                },
                message: '字母、数字、下划线组成,字母开头,6-16位'
            }
        });
    </script>
</body>
</html>
