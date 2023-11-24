<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DingHome.aspx.cs" Inherits="CCFlow.CCMobile.DingHome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="http://g.alicdn.com/dingding/open-develop/0.8.4/dingtalk.js" type="text/javascript"></script>
    <script src="js/ExtFold/zepto.min.js" type="text/javascript"></script>
    <script src="js/ExtFold/DingJsApiConfig.js" type="text/javascript"></script>
    <script src="js/ExtFold/DingDingDev.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" onclick="StartRecorder()" value="开始录音" style="width:120px; height:36px;" /><br /><br /><br />
        <input type="button" onclick="StopRecorder()" value="停止录音" style="width:120px; height:36px;" /><br /><br /><br />
        <input type="button" onclick="PlayAudio()" value="播放" style="width:120px; height:36px;" /><br /><br /><br />
        <input type="button" onclick="OpenMapTest()" value="定位" style="width:120px; height:36px;" /><br /><br /><br />
        <input type="file" accept="audio/*" capture="microphone">
    </div>
    </form>
</body>
</html>
