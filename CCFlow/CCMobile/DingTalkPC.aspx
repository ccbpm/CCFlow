<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DingTalkPC.aspx.cs" Inherits="CCFlow.CCMobile.DingTalkPC" %>

<!DOCTYPE html>
<html manifest="app.appcache">
<head runat="server">
    <title></title>
    <link href="css/themes/default/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <link href="css/themes/classic/theme-classic.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="http://g.alicdn.com/dingding/dingtalk-pc-api/2.5.0/index.js" type="text/javascript"></script>
    <script src="js/ExtFold/zepto.min.js" type="text/javascript"></script>
    <script src="js/ExtFold/DingJsPCApiConfig.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        DingTalkPC.ready(function (res) {
            var dingcode = "";
            var ActionType = GetQueryString("ActionType");
            var FK_Flow = GetQueryString("FK_Flow");
            var FK_Node = GetQueryString("FK_Node");
            var WorkID = GetQueryString("WorkID");
            var FID = GetQueryString("FID");

            $.ajax({
                type: "GET", //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "DingTalk.aspx?DoType=getuserdingcode", //要访问的后台地址
                async: false,
                cache: false,
                success: function (scope) {//data为返回的数据，在这里做数据绑定
                    var pushData = eval('(' + scope + ')');
                    if (pushData.code == "error") {
                        alert(pushpushData.Msg);
                        return;
                    }
                    dingcode = pushData.Msg;
                }
            });
            DingTalkPC.runtime.permission.requestAuthCode({
                corpId: dingcode, //企业ID
                onSuccess: function (result) {
                    $.ajax({
                        type: "GET", //使用GET或POST方法访问后台
                        dataType: "text", //返回json格式的数据
                        contentType: "application/json; charset=utf-8",
                        url: "DingTalk.aspx?DoType=loginmobfromdingtalk&Code=" + result.code, //要访问的后台地址
                        async: false,
                        cache: false,
                        success: function (scope) {//data为返回的数据，在这里做数据绑定
                            var pushData = eval('(' + scope + ')');
                            if (pushData.code == "error") {
                                alert(pushData.Msg);
                                return;
                            }

                            if (ActionType == "ToDo") {
                                location.href = "MyView.htm?FK_Flow=" + FK_Flow + "&FK_Node=" + FK_Node + "&WorkID=" + WorkID + "&FID=" + FID + "&IsRead=1&Paras=&T=0510084851";
                                return;
                            }
                            location.href = "Home.htm";
                        }
                    });
                },
                onFail: function (err) {
                    alert(err);
                }
            });
        });
        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }
    </script>
</head>
<body>
    <div id="talkPage" data-role="page" data-theme="d">
        <div data-role="header" data-theme="b">
        </div>
        <div class="ui-content" data-role="main" style="padding: 0px;">
            <div data-role="fieldcontain" style="font-size: 1.5em; color: Green; margin-top: 100px;margin-left: 60px;">正在验证,请稍后...</div>
        </div>
    </div>
</body>
</html>
