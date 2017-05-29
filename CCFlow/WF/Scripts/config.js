
//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'CCFlow'; //运行平台.
var Handler = "Handler.ashx"; //处理器,一般来说，都放在与当前处理程序的相同的目录下。
var MyFlow = "MyFlow.ashx"; //工作处理器.


//公共方法
function Handler_AjaxQueryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: Handler, //要访问的后台地址.
        data: param, //要发送的数据.
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


//公共方法
function Handler_AjaxPostData(param, callback, scope) {
    $.post(Handler, param, callback);
}

// for Java.
// var controllerURLConfig = '/WF/Admin/CCFormDesigner/CCFromHandler';
// var plant = 'JFlow'; //运行平台.
// var Handler = "Handler";

 
var load = {};

/*JS 动态加载js 文件*/
load.LoadJs = function (jsPath, chartset, callbackfun) {
    try {
        var head = document.getElementsByTagName('head')[0];
        var script = document.createElement('script');
        script.src = jsPath + "?Version=" + load.Version;
        script.type = 'text/javascript';
        if (chartset != undefined) {
            script.chartset = chartset;
        }

        if (script.readyState) {
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    if (callbackfun != undefined && typeof (callbackfun) == 'function') {
                        callbackfun();
                    }
                }
            }
        } else {
            script.onload = function () {
                if (callbackfun != undefined && typeof (callbackfun) == 'function') {
                    callbackfun();
                }
            }
        }
        head.appendChild(script);
    }
    catch (err) {
        alert(err)
    }

    //$.getScript(jsPath, callbackfun);
}

var jsPathArrVersion = {};
/*js 和CSS 的版本号*/
load.Version = '0412145201';

