//获取QueryString的数组 
function getQueryString() {
    var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
    for (var i = 0; i < result.length; i++) {
        result[i] = result[i].substring(1);
    }
    return result;
}

//Other
function GetQueryString(name) {

    if(typeof name === 'string' && name.toLocaleLowerCase() === 'token') {
        return filterXSS(localStorage.getItem("Token"))
    }
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");

    var r = window.location.search.substr(1).match(reg);

    if (r != null)
        return filterXSS(decodeURI(r[2]));
    return null;

}

function GetQueryStringByUrl(url, name) {
    //if (typeof name === 'string' && name.toLocaleLowerCase() === 'token') {
   //     return filterXSS(localStorage.getItem("Token"))
    //}
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");

    var r = url.match(reg);

    if (r != null)
        return filterXSS(decodeURI(r[2]));
    return null;
}

//通过URL获取QueryString的数组
function getQueryStringFromUrl(url) {
    if (url.indexOf('?') >= 0) {
        url = url.substring(url.indexOf('?'));
        var result = url.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
        if (result != undefined) {
            for (var i = 0; i < result.length; i++) {
                result[i] = result[i].substring(1);
            }
            return result;
        }
        else {
            return [];
        }
    }
    else {
        return [];
    }
}

/* 文本框根据输入内容自适应高度
* @param                {HTMLElement}        输入框元素
* @param                {Number}                设置光标与输入框保持的距离(默认0)
* @param                {Number}                设置最大高度(可选)
*/
var autoTextarea = function (elem, extra, maxHeight) {
    extra = extra || 0;
    var isFirefox = !!document.getBoxObjectFor || 'mozInnerScreenX' in window,
        isOpera = !!window.opera && !!window.opera.toString().indexOf('Opera'),
        addEvent = function (type, callback) {
            elem.addEventListener ?
                elem.addEventListener(type, callback, false) :
                elem.attachEvent('on' + type, callback);
        },
        getStyle = elem.currentStyle ? function (name) {
            var val = elem.currentStyle[name];

            if (name === 'height' && val.search(/px/i) !== 1) {
                var rect = elem.getBoundingClientRect();
                return rect.bottom - rect.top -
                    parseFloat(getStyle('paddingTop')) -
                    parseFloat(getStyle('paddingBottom')) + 'px';
            };

            return val;
        } : function (name) {
            return getComputedStyle(elem, null)[name];
        },
        minHeight = parseFloat(getStyle('height'));

    elem.style.resize = 'none';

    var change = function () {
        var scrollTop, height,
            padding = 0,
            style = elem.style;

        if (elem._length === elem.value.length) return;
        elem._length = elem.value.length;

        if (!isFirefox && !isOpera) {
            padding = parseInt(getStyle('paddingTop')) + parseInt(getStyle('paddingBottom'));
        };
        scrollTop = document.body.scrollTop || document.documentElement.scrollTop;

        elem.style.height = minHeight + 'px';
        if (elem.scrollHeight > minHeight) {
            if (maxHeight && elem.scrollHeight > maxHeight) {
                height = maxHeight - padding + 10;
                style.overflowY = 'auto';
            } else {
                height = elem.scrollHeight - padding + 10;
                style.overflowY = 'hidden';
            };
            style.height = height + extra + 'px';
            scrollTop += parseInt(style.height) - elem.currHeight;
            document.body.scrollTop = scrollTop;
            document.documentElement.scrollTop = scrollTop;
            elem.currHeight = parseInt(style.height);
        };
    };

    addEvent('propertychange', change);
    addEvent('input', change);
    addEvent('focus', change);
    change();
};


//修改URL参数值
function replaceParamVal(url, paramName, replaceWith) {
    var re = eval('/(&' + paramName + '=)([^&]*)/gi');
    var nUrl = url.replace(re, "&"+paramName + '=' + replaceWith);
    return nUrl;
}

//根据QueryString参数名称获取值 
function getQueryStringByNameFromUrl(url, name) {
    if (url.indexOf('?') >= 0) {
        url = url.substring(url.indexOf('?'));
        var result = url.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    }
    else {
        return "";
    }
}

//获取参数
var RequestArgs = function () {
    this.WorkID = GetQueryString("WorkID");
    this.FK_Flow = GetQueryString("FK_Flow");
    this.FK_Node = GetQueryString("FK_Node");
    if (this.FK_Node) {
        while (this.FK_Node.substring(0, 1) == '0') this.FK_Node = this.FK_Node.substring(1);
        this.FK_Node = this.FK_Node.replace('#', '');
    }
    this.NodeID = GetQueryString("NodeID");
    this.FK_MapData = GetQueryString("FK_MapData");
    this.UserNo = GetQueryString("UserNo");
    this.FID = GetQueryString("FID");
    this.SID = GetQueryString("Token");
    this.CWorkID = GetQueryString("CWorkID");
    this.PWorkID = GetQueryString("PWorkID");
    this.PFlowNo = GetQueryString("PFlowNo");

    this.DoFunc = GetQueryString("DoFunc");
    this.CFlowNo = GetQueryString("CFlowNo");
    this.WorkIDs = GetQueryString("WorkIDs");
    this.IsReadonly = GetQueryString("IsReadonly");
    this.IsEdit = GetQueryString("IsEdit");
    this.IsLoadData = GetQueryString("IsLoadData");
}
//传参
var urlExtFrm = function () {
    var extUrl = "";
    var args = new RequestArgs();
    if (args.WorkID != "")
        extUrl += "&WorkID=" + args.WorkID;
    if (args.FK_Flow != "")
        extUrl += "&FK_Flow=" + args.FK_Flow;
    if (args.FK_Node != "")
        extUrl += "&FK_Node=" + args.FK_Node;
    if (args.NodeID != "")
        extUrl += "&NodeID=" + args.NodeID;
    if (args.UserNo != "")
        extUrl += "&UserNo=" + args.UserNo;
    if (args.FID != "")
        extUrl += "&FID=" + args.FID;
    if (args.SID != "")
        extUrl += "&Token=" + args.SID;

    if (args.CWorkID != "")
        extUrl += "&CWorkID=" + args.CWorkID;
    if (args.PWorkID != "")
        extUrl += "&PWorkID=" + args.PWorkID;
    if (args.PFlowNo != "")
        extUrl += "&PFlowNo=" + args.PFlowNo;
    if (args.IsLoadData != "")
        extUrl += "&IsLoadData=" + args.IsLoadData;

    //获取其他参数
    var sHref = GetHrefUrl();
    var args = sHref.split("?");
    var retval = "";
    if (args[0] != sHref) /*参数不为空*/ {
        var str = args[1];
        args = str.split("&");
        for (var i = 0; i < args.length; i++) {
            str = args[i];
            var arg = str.split("=");
            if (arg.length <= 1) continue;
            //不包含就添加
            if (extUrl.indexOf(arg[0]) == -1) {
                extUrl += "&" + arg[0] + "=" + arg[1];
            }
        }
    }
    return extUrl;
}