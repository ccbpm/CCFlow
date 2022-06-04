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
var writeImg = "";//审核写字板
var FWCVer = 0;
$(function () {
    if (GetHrefUrl().indexOf(ccbpmPath) == -1)
        isEqualsDomain = true;
    //引入关联的js
    jQuery.getScript(DealText( ccbpmPath + "/WF/Scripts/config.js"), function () {
        jQuery.getScript(DealText( ccbpmPath + "/WF/Scripts/QueryString.js"), function () {
            jQuery.getScript(DealText( ccbpmPath + "/WF/Comm/Gener.js"), function () {
                if ($('#ccbpmJS').length != 0) {
                    var url = $('#ccbpmJS')[0].src;
                    var SID = getQueryStringByNameFromUrl(url, "Token");
                    //用户登陆
                    if (SID != null && SID != undefined) {
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF");
                        handler.AddPara("Token", SID);
                        handler.AddPara("DoWhat", "PortLogin");

                        var data = handler.DoMethodReturnString("Port_Init");
                    }
                }
                
            })
        });
    });


});

$(window).load(function () {
    //初始化网页URL参数
    paramData = {
        FK_Flow: GetQueryString("FK_Flow"),
        FK_Node: GetQueryString("FK_Node"),
        WorkID: GetQueryString("WorkID"),
        OID: GetQueryString("WorkID"),
        FID: GetQueryString("FID") == null ? 0 : GetQueryString("FID"),
        IsReadonly: GetQueryString("IsReadonly")
    }

    
    if ($("#ToolBar").length == 1 || $("#Toolbar").length == 1) {
        if ($('#ccbpmJS').length == 1) {
            var url = $('#ccbpmJS')[0].src;
            var type = getQueryStringByNameFromUrl(url, "type");
            if (type == "CC")
                loadScript(ccbpmPath + "/WF/Toolbar.js", function () { }, "JS_CC");
            else if (type == "MyView")
                loadScript(ccbpmPath + "/WF/Toolbar.js", function () { }, "JS_MyView");
            else if (type == "MyFrm")
                loadScript(ccbpmPath + "/WF/Toolbar.js", function () { }, "JS_MyFrm");
            else if (type = "MyGener")
                loadScript(ccbpmPath + "/WF/Toolbar.js");
        } else {
            loadScript(ccbpmPath + "/WF/Toolbar.js");
        }

    }

    //审核组件
    if ($("#WorkCheck").length == 1) {
        loadScript(ccbpmPath + "/WF/WorkOpt/WorkCheck.js", function () {
            NodeWorkCheck_Init();
        });  
    }
    //单个附件
    if ($("[name=AthSingle]").length != 0) {
        loadScript(ccbpmPath + "/WF/CCForm/AthSingle.js");
    }
       
    if ($("#FlowBBS").length == 1)
        loadScript(ccbpmPath + "/WF/WorkOpt/FlowBBS.js");
    if ($("#JobSchedule").length == 1)
        loadScript(ccbpmPath + "/WF/WorkOpt/OneWork/JobSchedule.js");

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


