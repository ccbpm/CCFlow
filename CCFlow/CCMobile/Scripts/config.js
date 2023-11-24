
//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'CCFlow'; //运行平台.
var Handler = basePath + "/WF/Comm/ProcessRequest"; //处理器,一般来说，都放在与当前处理程序的相同的目录下。
var basePath = basePath();

function basePath() {

    //jflow下常用目录
    var dirs = ['/WF', '/DataUser', '/GPM', '/App', '/Portal', '/CCMobile', '/CCFast', '/CCMobilePortal', '/FastMobilePortal', '/AdminSys', '/Admin'];
    //获取当前网址，如： http://localhost:80/jflow-web/index.jsp

    var curPath = window.document.location.href;
    //获取主机地址之后的目录，如： jflow-web/index.jsp  
    var pathName = window.document.location.pathname;
    if (pathName == "/") //说明不存在项目名
        return curPath;
    var pos = curPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:80  
    var localhostPath = curPath.substring(0, pos);
    //获取带"/"的项目名，如：/jflow-web
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    for (var i = 0; i < dirs.length; i++) {
        if (projectName == dirs[i]) {
            projectName = "";
            break;
        }
    }

    var path = localhostPath + projectName;
    if ("undefined" != typeof ccbpmPath && ccbpmPath != null && ccbpmPath != "") {
        if (ccbpmPath != path)
            return ccbpmPath;
    }
    return path

}

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

/**
* 动态异步加载JS的方法
* @param {any} url 加载js的路径
* @param {any} callback 加载完成后的回调函数
*/
function loadScript(url, callback, scriptID) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    if (typeof (callback) != "undefined") {
        if (script.readyState) {
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            };
        } else {
            script.onload = function () {
                callback();
            };
        }
    }
    script.src = url;
    if (scriptID != null && scriptID != undefined)
        script.id = scriptID;
    // document.head.appendChild(script);
    var tmp = document.getElementsByTagName('script')[0];
    tmp.parentNode.insertBefore(script, tmp);
}

var Skip = {};
//获取XMLHttpRequest对象(提供客户端同http服务器通讯的协议)
Skip.getXmlHttpRequest = function () {
    if (window.XMLHttpRequest) // 除了IE外的其它浏览器
        return new XMLHttpRequest();
    else if (window.ActiveXObject) // IE 
        return new ActiveXObject("MsXml2.XmlHttp");
},
//导入内容
Skip.includeJsText = function (rootObject, jsText) {
    if (rootObject != null) {
        var oScript = document.createElement("script");
        oScript.type = "text/javascript";
        oScript.text = jsText;
        rootObject.appendChild(oScript);
    }
},
//导入文件
Skip.includeJsSrc = function (rootObject, fileUrl) {
    if (rootObject != null) {
        var oScript = document.createElement("script");
        oScript.type = "text/javascript";
        oScript.src = fileUrl;
        rootObject.appendChild(oScript);
    }
},
//同步加载
Skip.addJs = function (url, rootObject) {
    $.ajax({
        url: url,
        method: 'GET',
        async: false
    }).success(function (result) {
        if (rootObject == null || rootObject == undefined)
            rootObject = document.getElementsByTagName('script')[0];
        Skip.includeJsText(rootObject, result.responseText);
    }).error(function (result) {
        if (rootObject == null || rootObject == undefined)
            rootObject = document.getElementsByTagName('script')[0];
        Skip.includeJsText(rootObject, result.responseText);
    });

}





 


