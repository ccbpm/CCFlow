﻿/*  ****************************  说明: ***************************
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
    Skip.addJs(ccbpmPath + "/WF/Scripts/config.js");
    Skip.addJs(ccbpmPath + "/WF/Comm/Gener.js");
    //Skip.addJs(ccbpmPath + "/WF/Scripts/QueryString.js");
    loadScript(ccbpmPath + "/WF/Scripts/config.js", function () {
        loadScript(ccbpmPath + "/WF/Comm/Gener.js", function () {
            loadScript(ccbpmPath + "/WF/Scripts/QueryString.js", function () {
            });
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



//Skip.addJs(rootObject, "test.js")//test.js文件中含有funciotn test(){alert("test");}


