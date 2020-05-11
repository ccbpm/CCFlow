/*  ****************************  说明: ***************************
1. 该功能js是ccbpm常用的组件, 比如：操作按钮,审核组件,评论组件,进度图,附件，从表，.
2. 使用该js必须对应的有相应的div配置
   操作按钮 ToolBar; 审核组件 WorkCheck;评论组件 FlowBBS 进度图  JobSchedule
3.在调用的页面中都需要增加Save方法
4.如果是节点属性按钮权限中启用了其他按钮功能，需要在页面中引入bootstrap相关的js和css样式
*/
var isEqualsDomain = false;  //调用ccbpm.js的页面和ccbpm.js域是否相同
var ccbpmPath = GetPath();
var paramData = {};
$(function () {
    if (window.location.href.startsWith(ccbpmPath) == false)
        isEqualsDomain = true;

    //引入关联的js
    loadScript(ccbpmPath + "/WF/Scripts/config.js", function () {
        loadScript(ccbpmPath + "/WF/Comm/Gener.js", function () {
                loadScript(ccbpmPath + "/WF/Scripts/QueryString.js");
        });
    });

    //初始化网页URL参数
    paramData = {
        FK_Flow: GetQueryString("FK_Flow"),
        FK_Node: GetQueryString("FK_Node"),
        WorkID: GetQueryString("WorkID"),
        OID: GetQueryString("WorkID"),
        FID: GetQueryString("FID") == null ? 0 : GetQueryString("FID"),
        IsReadonly: GetQueryString("IsReadonly")
    }

    

});

$(window).load(function () {
    //表单树形结构
    if ($("#tabs").length == 1) {
        return;
    }
    
    if ($("#ToolBar").length == 1) {
        if ($('#ccbpmJS').length == 1) {
            var url = $('#ccbpmJS')[0].src;
            var type = getQueryStringByNameFromUrl(url, "type");
            if(type == "CC")
                loadScript(ccbpmPath + "/WF/ToolBar.js", function () { }, "JS_CC");
            if (type == "MyView")
                loadScript(ccbpmPath + "/WF/ToolBar.js", function () { }, "JS_MyView");
        }
           
        else
            loadScript(ccbpmPath + "/WF/ToolBar.js");

    }
        
    if ($("#WorkCheck").length == 1)
       loadScript(ccbpmPath + "/WF/WorkOpt/WorkCheck.js");
    if ($("#FlowBBS").length == 1)
        loadScript(ccbpmPath + "/WF/WorkOpt/FlowBBS.js");

    if ($("#JobSchedule").length == 1)
        loadScript(ccbpmPath + "/WF/WorkOpt/JobSchedule.js");

});

//获取WF之前路径
function GetPath() {
    var js = document.scripts || document.getElementsByTagName("script");
    var jsPath;
    for (var i = js.length; i > 0; i--) {
        if (js[i - 1].src.indexOf("ccbpm.js") > -1) {
            jsPath = js[i - 1].src.substring(0, js[i - 1].src.lastIndexOf("/"));
            jsPath = jsPath.substring(0, jsPath.length - 3);
            return jsPath;
        }
    }
    return null;
}

/**
 * 动态异步加载JS的方法
 * @param {any} url 加载js的路径
 * @param {any} callback 加载完成后的回调函数
 */
function loadScript(url, callback,scriptID) {
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
    if (scriptID != null && scriptID!=undefined)
         script.id = scriptID;
   // document.head.appendChild(script);
    var tmp = document.getElementsByTagName('script')[0];
    tmp.parentNode.insertBefore(script,tmp);
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
    Skip.addJs = function (url) {
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
        var rootObject = document.getElementsByTagName('script')[0];
        Skip.includeJsText(rootObject, oXmlHttp.responseText);
    }

//Skip.addJs(rootObject, "test.js")//test.js文件中含有funciotn test(){alert("test");}


