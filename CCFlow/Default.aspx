<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.Default" CodeBehind="Default.aspx.cs" %>

<html>
<head>
    <meta charset="utf-8" />
    <title>ccbpm导航页</title>
    <link href="DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body {
        }

        li {
            font-size: 14px;
            margin: 5px;
            border-style: none;
            color: Gray;
        }
    </style>
    <base target="_blank" />
</head>

<body>
     <h3>单组织版-集团版-SAAS登陆</h3>
    <ul>
        <li><a href="/Portal/Standard/Login.htm">用户登录 /Portal/Standard/Login.htm</a></li>
    </ul>

    <h3>单组织版</h3>
    <ul>
        <li><a href="http://localhost:3000">Vue3版本</a></li>
    </ul>

    <h3>Toolkit版本</h3>
    <ul>
        <li><a href="http://localhost:3000">Vue3版本</a></li>
        <li><a href="http://localhost:8080">Vue2版本toolkit</a></li>
        <li><a href="http://localhost:1207">H5 版本 toolkit</a></li>
    </ul>

    <h3>SAAS模式 - 宁波港</h3>
    <ul>
        <li><a href="/Portal/SaaS/Login.htm">Admin登陆</a>  </li>
    </ul>

    <h3>SAAS模式 - 运营</h3>
    <ul>
        <li><a href="/Portal/SaaSOperation/Login.htm">用户登陆</a>  </li>
        <li><a href="/Portal/SaaS/Admin/Login.htm">admin登陆</a>  </li>
    </ul>

    <h3>JFlow</h3>
    <ul>
        <li><a href="http://localhost:8085">http://localhost:8085 - JFlow </a></li>
    </ul>

</body>
</html>
