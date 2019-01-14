
//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'CCFlow'; //运行平台.
var basePath = "";
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

 
// for Java.
// var controllerURLConfig = '/WF/Admin/CCFormDesigner/CCFromHandler';
// var plant = 'JFlow'; //运行平台.
// var Handler = "Handler";

 


