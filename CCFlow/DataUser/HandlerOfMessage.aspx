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
        <li>配置节点：HandlerOfMessage , 如果没有这个标记，请增加一个. </li>
        <li>配置可以是 http://yourserver:portNo:/yourself.xxx 的url. </li>
        <li>该服务文件接收到参数，需要供开发者二次开发使用比如：发送短信、微信、钉钉等其他设备连接.</li>
    </ul>

    <h3>参数说明</h3>
    <ul>
        <li>详见代码..

            <%--  string doType = this.Request.QueryString["DoType"];
            if (doType == "SendToCCMSG")
            {
                byte[] data = new byte[this.Request.InputStream.Length];
                this.Request.InputStream.Read(data, 0, data.Length); //获得传入来的数据.
                string txt = System.Text.Encoding.UTF8.GetString(data);  //编码.


                //转成json.
                Dictionary<string, object> dictionary = null;
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);

                //获得里面的参数.
                string send = dictionary["sender"].ToString(); //发送人.
                string sendTo = dictionary["sendTo"].ToString(); //发送给 与人员表Port_Emp的No一致.
                string tel = "";
                if (dictionary["tel"] != null) //配置的电话。
                    tel = dictionary["tel"].ToString();

                string title = dictionary["title"].ToString(); //标题
                string content = dictionary["content"].ToString(); //信息内容.
                string openUrl = dictionary["openUrl"].ToString(); //要打开的url.

                //执行相关处理,接受以上参数后，可以发送丁丁，微信，手机短信，或者其他的即时通讯里面去.
            }--%>
        </li>
    </ul>

</body>
</html>
