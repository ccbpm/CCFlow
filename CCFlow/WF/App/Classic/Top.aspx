<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top.aspx.cs" Inherits="CCFlow.WF.App.Classic.Top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <script language="JavaScript" src="/WF/Comm/JScript.js" type="text/javascript" ></script>
    <script type="text/javascript" src="/WF/Scripts/bootstrap/js/jquery.min.js"></script>
    <script type="text/javascript" src="/WF/Scripts/bootstrap/js/bootstrap.min.js"></script>
    <script src="/WF/Scripts/QueryString.js" type="text/javascript"></script>
    <script src="/WF/Scripts/config.js" type="text/javascript"></script>
     <base target="_self" />
     <script type="text/javascript">
         function AuthExit(userNo) {
             $.ajax({
                 type: 'post',
                 async: false,
                 url: "/WF/Handler.ashx?DoType=AuthExit&No=" + userNo + "&m=" + Math.random(),
                 dataType: 'html',
                 success: function (msg) {
                     if (msg.indexOf('err') == 0) {
                         alert('授权退出失败!');
                     }
                     else {
                         parent.location.reload();
                         top.location.href = '/WF/App/Classic/Default.aspx';
                     }
                 }
             });
         }
     </script>
<style>
    body
    {
        margin: 0 auto;
    }
    a
    {
        color:#0066CC;
        text-decoration:none;
    }
    a:hover
    {
        color:#0084C5;
        text-decoration:underline;
    }
    .top
    {
        width: 100%;
        background-color: #037fcc;
        height: 80px;
        border-bottom-color: Yellow;
    }
    .logo
    {
        height: 80px;
        float: left;
        background: url(Img/TopLeft.jpg); /* [disabled]height: 70px; */
        width: 450px;
        background-repeat: no-repeat;
        color: #fff;
        font-size: 26px;
        padding: 28px 0 0 50px;
        font-weight: bold;
        font-family: "黑体";
    }
    .logo_cz
    {
        float: right;
        height: 80px;
        width: 500px;
    }
    .logo_cz1
    {
        margin-top: -13px;
        margin-right: 20px;
        height: 45px;
    }
    .logo_cz1 ul
    {
        color: #FFF;
    }
    .logo_cz1 li
    {
        list-style-type: none;
        float: left;
        margin-left: 15px;
        margin-right: 5px;
    }
    .logo_cz2
    {
        background: url(Img/TopRight.png);
        float: right;
        height: 34px;
        width: 270px;
    }
    .logo_cz2 ul
    {
        font-size: 14px;
        margin-top: 10px;
        margin-left: 30px;
    }
    .logo_cz2 li
    {
        font-weight: bold;
        color: #1977cc;
        width: 66px;
        list-style-type: none;
        float: left;
    }
    .logo_cz3
    {
        background: url(Img/TopRight1.png);
        float: right;
        height: 34px;
        width: 270px;
    }
    .logo_cz3 ul
    {
        font-size: 14px;
        margin-top: 10px;
        margin-left: 30px;
    }
    .logo_cz3 li
    {
        font-weight: bold;
        color: #1977cc;
        width: 106px;
        list-style-type: none;
        float: left;
    }
</style>
</head>

<body>
    <div class="top">
        <div class="logo">
            <%=BP.Sys.SystemConfig.SysName %></div>
        <div class="logo_cz">
            <div class="logo_cz1">
                <ul>
                    <li>账号：<%=BP.Web.WebUser.No %></li>
                    <li>姓名：<%=BP.Web.WebUser.Name %></li><br />
                    <li>部门：<%=BP.Web.WebUser.FK_DeptName %></li>
                </ul>
            </div>
            <%
                if (BP.Web.WebUser.IsAuthorize==false)
                {
                 %>
            <div class="logo_cz2">
                <ul>
                <li><a href="../../Tools.aspx" target="main">设置</a></li>
                    <li><a href="../../Messages.aspx" target="main">消息</a></li>
                    <li><a href="Login.aspx?DoType=Logout" target="_parent">退出</a></li>
                </ul>
            </div>
            <%
                }
                else
                {
                     %>

            <div class="logo_cz3">
                <ul>
                <li><a href="javascript:AuthExit('<%=BP.Web.WebUser.No %>');">退出授权登录</a></li>
                </ul>
            </div>
            <%
                }
                     %>
        </div>
    </div>
</body>
</html>
