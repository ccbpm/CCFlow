
//访问的ccbpm服务器地址.
//var host = "http://101.43.55.81:8022"; //演示服务器. h5登陆地址: http://vue3.ccbpm.cn
var host = "http://localhost:2296";

// 流程域名 , 默认为空. 比如:CRM, ERP, OA 等等。 该域名配置流程表单树的属性上， 如果要获取全部的就保留为空.
// 表示该目录下所有的流程都属于这个域里.
var domain = "ERP"; //可以为空，则表示获取全部的.
var plant = "CCFlow"; //For.net 请设置ccflow, forJava请设置JFlow.

//私钥. 这里明文定义到这里了, 为了安全需要写入到后台.
var PrivateKey = "DiGuaDiGua,IamCCBPM";

var UserNo = null; //当前用户名变量.
var ccbpmHostDevelopAPI = host + "/WF/API/"; //驰骋BPM流程服务器地址
function GetHrefUrl() {
    return window.location.href;
    //return GetHrefUrl();
}
/**
 * 执行登录：返回SID.
 * 私约是本地服务器与BPM服务器双方约定的一个字符串.
 * BPM服务配置在 java的jflow.properties .net 的Web.config，
 * 
 * 本地应用系统在登录成功后，访问bpm服务器登录的时候传入 私约 + 用户编号，让其登录并且返回一个 sid (与token概念一样)。
 * 您获得这个SID后，记住它，并且在访问的时候，使用UserNo+SID来访问ccbpm的功能页面。
 * 为了安全期间：您可以把这个获得sid的方法放入后台实现。
 * 
 * @param {私钥} privateKey
 * @param {用户账号} userNo
 */
function LoginCCBPM(privateKey, userNo) {

    //url 地址。
    var url = ccbpmHostDevelopAPI + "Portal_Login?privateKey=" + privateKey + "&userNo=" + userNo;
    var str = RunUrlReturnString(url);

    var json = JSON.parse(str);
    localStorage.setItem('UserInfoJson', str);

    UserNo = userNo;
    return json.Token;
}
function LoginByToken(token) {
    //url 地址。
    var url = ccbpmHostDevelopAPI + "Portal_LoginByToken?privateKey=" + privateKey + "&userNo=" + userNo;
    var str = RunUrlReturnString(url);

    var json = JSON.parse(str);
    localStorage.setItem('UserInfoJson', str);

    UserNo = userNo;
    return json.Token;
}
//获得当前登陆信息.
function GetWebUser() {
    var str = localStorage.getItem('UserInfoJson');
    if (str == null || str == undefined) {
        alert('登陆信息丢失.');
        window.location.href = '/Portal/Login.htm';
        return;
    }
    return JSON.parse(str);
}

/**
 * 退出登录
 * */
function LoginOut() {

    //url 地址。
    var url = ccbpmHostDevelopAPI + "Portal_LoginOut?Token=" + GetWebUser().Token;
    var token = RunUrlReturnString(url);
    //赋值给公共变量
    UserNo = "";
    localStorage.setItem('UserInfoJson', "");
}

/**
 * 获得当前的用户.
 * */
function GetUserNo() {
    return GetWebUser().No;
}

/**
 * 获得token.
 * */
function GetToken() {
    return localStorage.Token;// GetWebUser().Token;
}

/**
 * 打开工作处理器的方法，可以被重写。
 * 1. 默认的工作处理器在一个新的页面打开。
 * 2. 可以在框架的 tab 页打开.
 * @param {打开工作处理器的url} url
 */
function OpenMyFlow(url) {

    window.open(url);
}

/**
 * 执行URL转化为json对象.转化失败为null.
 * @param {url} url
 */
function RunUrlReturnJSON(url) {

    var str = RunUrlReturnString(url);
    if (str == null) return null;

    try {
        return JSON.parse(str);
    } catch (e) {
        alert("json解析错误: " + str);
        return null;
    }
}

/**
 * 执行URL返回string.
 * @param {any} url
 */
function RunUrlReturnString(url) {

    if (url == null || typeof url === "undefined") {
        alert("err@url无效");
        return;
    }

    var string;

    $.ajax({
        type: 'post',
        async: false,
        url: url,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@url') != -1) {
                string = data.replace('err@', ''); //这个错误是合法的.
                return;
            }

            if (data.indexOf("err@") != -1) {
                alert(data);
                return;
            }
            string = data;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (confirm('系统异常:' + url + " 您想打开url查看吗？") == true) {
                window.open(url);
                return;
            }
        }
    });
    return string;
}

/**
* 动态异步加载JS的方法
* @param {any} url 加载js的路径
* @param {any} callback 加载完成后的回调函数
*/
function loadScript(url, callback, scriptID) {
    var script = document.createElement("script");
    script.type = "text/javascript";
    if (callback != null && typeof (callback) != "undefined") {
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


/**
 * 父页面监听子页面调用方法
 */
window.addEventListener('message', function (event) {
    var data = event.data;
    var info = event.data.info;
    if (true) {
        switch (data.action) {
            case 'returnWorkWindowClose':
                if (typeof returnWorkWindowClose != 'undefined' && returnWorkWindowClose instanceof Function)
                    returnWorkWindowClose(info);
                break;
            case 'WindowCloseReloadPage':
                if (typeof WindowCloseReloadPage != 'undefined' && WindowCloseReloadPage instanceof Function)
                    WindowCloseReloadPage(info);
                break;
            default:
                break;
        }
    }
}, false);


function GetPara(key, atparaStr) {
    var atPara = atparaStr;
    if (typeof atPara != "string" || typeof key == "undefined" || key == "") {
        return undefined;
    }
    var reg = new RegExp("(^|@)" + key + "=([^@]*)(@|$)");
    var results = atPara.match(reg);
    if (results != null) {
        return unescape(results[2]);
    }
    return undefined;
}

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


var IsIELower10 = false;

var ver = IEVersion();
if (ver == 6 || ver == 7 || ver == 8 || ver == 9)
    IsIELower10 = true;

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
