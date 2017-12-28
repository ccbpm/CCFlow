//公共方法
function ajaxService(param, callback, scope) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: Handler, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
        complete: function () { }, //AJAX请求完成时隐藏loading提示
        error: function (XMLHttpRequest, errorThrown) {
            callback(XMLHttpRequest);
        },
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}

//检查登录.
function checklogin(fCallback, oScope) {
    /// <summary>检测登录信息</summary>
    /// <param name="fCallback" type="Function">检测完之后，要运行的方法</param>
    /// <param name="oScope" type="Object">检测完之后，要运行的方法的参数</param>
    ajaxService({ action: "LetLogin" }, function (re, scps) {

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