// UI风格配置. UIPlant, 为了适应不同风格的版本需要. 我们增加这个配置, UIPlant=BS,Ele.

var uiPlant = 'BS'; //风格文件.

//  For .net 后台的调用的url ,  java的与.net的不同.
var plant = 'JFlow'; //运行平台. JFlow
var basePath = basePath();
var Handler = basePath + "/WF/Comm/ProcessRequest"; //处理器,一般来说，都放在与当前处理程序的相同的目录下。
var webUser = null; //定义通用变量用户信息
var IsIELower10 = false;

var ver = IEVersion();
if (ver == 6 || ver == 7 || ver == 8 || ver == 9)
    IsIELower10 = true;


function basePath() {

    //jflow下常用目录
    var dirs = ['/WF', '/DataUser', '/GPM', '/App', '/Portal', '/CCMobile', '/CCFast', '/CCMobilePortal', '/FastMobilePortal',  '/Admin'];
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

/**
 * 获取项目路径
 * @returns
 */
function getContextPath() {
    return basePath.substring(basePath.lastIndexOf("/"));
}

/**
* 动态异步加载JS的方法
* @param {any} url 加载js的路径
* @param {any} callback 加载完成后的回调函数
*/
function loadScript(url, callback, scriptID) {
    try {
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
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(script, tmp);
    } catch (e) {
        alert(url + "文件不存在");
    }

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
            rootObject.append(oScript);
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

function Exists(url) {
    var isExists;
    $.ajax({
        url: url,
        type: 'HEAD',
        async: false,
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



