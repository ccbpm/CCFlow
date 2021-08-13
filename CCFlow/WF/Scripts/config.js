﻿// UI风格配置. UIPlant, 为了适应不同风格的版本需要. 我们增加这个配置, UIPlant=BS,Ele.
var uiPlant = 'BS'; //风格文件.

//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'CCFlow'; //运行平台.
var basePath = basePath();
var Handler = "Handler.ashx"; //处理器,一般来说，都放在与当前处理程序的相同的目录下。
var MyFlow = "MyFlow.ashx"; //工作处理器.
var webUser = null; //定义通用变量用户信息
var IsIELower10 = false;

var ver = IEVersion();
if (ver == 6 || ver == 7 || ver == 8 || ver == 9)
    IsIELower10 = true;


function basePath() {
    
    //获取当前网址，如： http://localhost:80/jflow-web/index.jsp  
    var curPath = window.document.location.href;
    //获取主机地址之后的目录，如： jflow-web/index.jsp  
    var pathName = window.document.location.pathname;
    if (pathName == "/") { //说明不存在项目名
        if ("undefined" != typeof ccbpmPath && ccbpmPath != null && ccbpmPath != "") {
            if (ccbpmPath != curPath)
                return ccbpmPath;
        }
        return curPath;
    }
    var pos = curPath.indexOf(pathName);
    //获取主机地址，如： http://localhost:80  
    var localhostPath = curPath.substring(0, pos);
    //获取带"/"的项目名，如：/jflow-web
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);

    if ("undefined" != typeof ccbpmPath && ccbpmPath != null && ccbpmPath != "") {
        if (ccbpmPath != localhostPath)
            return ccbpmPath;
    }

    return localhostPath;

}


/**
 * 获取项目路径
 * @returns
 */
function getContextPath(){
	return basePath.substring(basePath.lastIndexOf("/"));
}

//公共方法
function Handler_AjaxQueryData(param, callback, scope, method, showErrMsg) {
    if (!method) method = 'GET';
    $.ajax({
        type: method, //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "text/plain; charset=utf-8",
        url: Handler, //要访问的后台地址.
        data: param, //要发送的数据.
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
        success: function (msg) {//msg为返回的数据，在这里做数据绑定
            var data = msg;
            callback(data, scope);
        }
    });
}

/**
* 动态异步加载JS的方法
* @param {any} url 加载js的路径
* @param {any} callback 加载完成后的回调函数
*/
function loadScript(url, callback, scriptID) {
    if (Exists(url) == false)
        return;
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
    if (Exists(url) == false)
        return;
    var oXmlHttp = Skip.getXmlHttpRequest();
    oXmlHttp.onreadystatechange = function () {//其实当在第二次调用导入js时,因为在浏览器当中存在这个*.js文件了,它就不在访问服务器,也就不在执行这个方法了,这个方法也只有设置成异步时才用到
        if (oXmlHttp.readyState == 4) { //当执行完成以后(返回了响应)所要执行的
            if (oXmlHttp.status == 200 || oXmlHttp.status == 304) { //200有读取对应的url文件,404表示不存在这个文件
                Skip.includeJsSrc(rootObject, url);
            } else {
                alert('XML request error: ' + oXmlHttp.statusText + ' (' + oXmlHttp.status + ')');
            }
        }
    }
    //1.True 表示脚本会在 send() 方法之后继续执行，而不等待来自服务器的响应,并且在open()方法当中有调用到onreadystatechange()这个方法。通过把该参数设置为 "false"，可以省去额外的 onreadystatechange 代码,它表示服务器返回响应后才执行send()后面的方法.
    //2.同步执行oXmlHttp.send()方法后oXmlHttp.responseText有返回对应的内容,而异步还是为空,只有在oXmlHttp.readyState == 4时才有内容,反正同步的在oXmlHttp.send()后的操作就相当于oXmlHttp.readyState == 4下的操作,它相当于只有了这一种状态.
    oXmlHttp.open('GET', url, false); //url为js文件时,ie会自动生成 '<script src="*.js" type="text/javascript"> </scr ipt>',ff不会  
    oXmlHttp.send(null);
    if (rootObject == null || rootObject==undefined)
        rootObject = document.getElementsByTagName('script')[0];
    Skip.includeJsText(rootObject, oXmlHttp.responseText);
}

function Exists(url) {
    var isExists;
    $.ajax({
        url: url,
        type: 'HEAD',
        async:false,
        error: function () {
            isExists = 0;
        },
        success: function () {
            isExists = 1;
        }
    });
    if (isExists == 1) {
        return true;
    }
    else {
        return false;
    }
}

/**
 *判断是不是移动端 
 */
function IsMobile() {
    let info = navigator.userAgent;
    let agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPod", "iPad"];
    for (let i = 0; i < agents.length; i++) {
        if (info.indexOf(agents[i]) >= 0) return true;
    }
    return false;
}

function IEVersion() {
    var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串  
    var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1; //判断是否IE<11浏览器  
    var isEdge = userAgent.indexOf("Edge") > -1 && !isIE; //判断是否IE的Edge浏览器  
    var isIE11 = userAgent.indexOf('Trident') > -1 && userAgent.indexOf("rv:11.0") > -1;
    if (isIE) {
        if (document.documentMode) return document.documentMode;
    } else if (isEdge) {
        return 'edge';//edge
    } else if (isIE11) {
        return 11; //IE11  
    } else {
        return -1;//不是ie浏览器
    }
}



