<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HandlerOfMessage.aspx.cs" Inherits="CCFlow.DataUser.HandlerOfMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>

    <h3>消息处理服务文件demo</h3>
    <ul>
        <li>ccbpm的产生的消息都会提交到一个这个服务文件里，这个服务文件的地址需要在全局文件里配置。</li>
        <li>jflow的配置文件在 jflow-web\src\main\resources\jflow.properties 的文件里. </li>
        <li>ccflow的配置文件在web.config 的文件里.  </li>
        <li>配置节点：HandlerOfMessage</li>
        <li>该服务文件接收到参数，需要供开发者二次开发使用比如：发送短信、微信、钉钉等其他设备连接.</li>
    </ul>

    <h3>参数说明</h3>
    <ul>
        <li> 1.  </li>
    </ul>

</body>
</html>
