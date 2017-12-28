
// 转到消息界面提示,不再本窗口提示.
function ToMsg(msg) {
    alert(msg);
    window.location.href = '/WF/MyFlowInfo.aspx?Msg=' + data;
}

var paras = "";
//生成url Para.
function GetParas() {
    paras = "";
    //获取其他参数
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] != sHref) /*参数不为空*/
    {
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1)
                continue;
            //不包含就添加
            if (paras.indexOf(arg[0]) == -1) {
                paras += "&" + arg[0] + "=" + arg[1];
            }
        }
    }
}