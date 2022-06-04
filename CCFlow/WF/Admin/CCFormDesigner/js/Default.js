//sArgName表示要获取哪个参数的值
function getArgsFromHref(sArgName) {
    var sHref = GetHrefUrl();
    var args = sHref.split("?");
    var retval = "";
    if (args[0] == sHref) /*参数为空*/
    {
        return retval; /* 无需做任何处理 */
    }
    var str = args[1];
    args = str.split("&");
    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1)
           continue;
        if (arg[0] == sArgName) 
            retval = arg[1];
    }
    return retval;
}

//公共方法
function ajaxServiceDefault(param, callback, scope, levPath) {

    var url = "/WF/Admin/CCBPMDesigner/CCBPMDesignerBase.ashx";

    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "text/plain; charset=utf-8",
        url: url, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) { //msg为返回的数据，在这里做数据绑定.
            var data = msg;
            callback(data, scope);
        }
    });
}

function Checklogin(fCallback, oScope) {
    /// <summary>检测登录信息</summary>
    /// <param name="fCallback" type="Function">检测完之后，要运行的方法</param>
    /// <param name="oScope" type="Object">检测完之后，要运行的方法的参数</param>

    ajaxServiceDefault({ method: "LetLogin" }, function (re, scps) {

        alert(re);

        if (re == null || re.length == 0) {
            if (scps.length == 2 && scps[0]) {
                scps[0](scps[1]);
            }

            return;
        }

        //else {
        //  $.messager.alert("错误", "验证登录信息失败，请重试。失败信息：" + re, "error");
        //}

    }, [fCallback, oScope]);
}