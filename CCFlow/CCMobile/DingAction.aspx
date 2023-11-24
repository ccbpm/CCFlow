<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DingAction.aspx.cs" Inherits="CCFlow.CCMobile.DingAction" %>

<!DOCTYPE html>
<html manifest="app.appcache">
<head runat="server">
    <title></title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <script src="https://g.alicdn.com/ilw/ding/0.6.6/scripts/dingtalk.js" type="text/javascript"></script>
    <link href="css/themes/default/jquery.mobile-1.4.5.min.css" rel="stylesheet" type="text/css" />
    <link href="css/themes/classic/theme-classic.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery.mobile-1.4.5.min.js" type="text/javascript"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        dd.ready(function () {
            

            dd.runtime.permission.requestAuthCode({
                corpId: 'ding5018b50b0c7a3342ffe93478753d9884',
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
                                $("#msgBtn").html(pushData.Msg);
                                return;
                            }
                            
                            location.href = "../CCMobilePortal/Home.htm";
                        }
                    });
                },
                onFail: function (err) {
                    alert(JSON.stringify(err));
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
    <div id="homePage" data-role="page" data-theme="d">
        <div data-role="header" data-theme="b">
            <h2>认证</h2>
        </div>
        <div class="ui-content" data-role="main">
            <form action="#" method="post">
                <ul data-role='listview'>
                    <button id="msgBtn" class='ui-btn ui-state-disabled ui-icon-alert ui-btn-icon-top'></button>
                </ul>
            </form>
        </div>
    </div>
</body>
</html>
