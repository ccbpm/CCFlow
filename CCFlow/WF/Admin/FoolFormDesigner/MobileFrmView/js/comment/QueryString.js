//获取QueryString的数组 
function getQueryString() {
    var result = location.search.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));
    for (var i = 0; i < result.length; i++) {
        result[i] = result[i].substring(1);
    }
    return result;
}
//根据QueryString参数名称获取值 
function getQueryStringByName(name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
}
//根据QueryString参数索引获取值 
function getQueryStringByIndex(index) {
    if (index == null) {
        return "";
    }
    var queryStringList = getQueryString();
    if (index >= queryStringList.length) {
        return "";
    }
    var result = queryStringList[index];
    var startIndex = result.indexOf("=") + 1;
    result = result.substring(startIndex);
    return result;
}
//Other
function GetQueryString(name) {

    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");

    var r = window.location.search.substr(1).match(reg);

    if (r != null) return unescape(r[2]); return null;

}

//获取参数
var RequestArgs = function () {
    this.WorkID = getQueryStringByName("WorkID");
    this.FK_Flow = getQueryStringByName("FK_Flow");
    this.FK_Node = getQueryStringByName("FK_Node");
    if (this.FK_Node) {
        while (this.FK_Node.substring(0, 1) == '0') this.FK_Node = this.FK_Node.substring(1);
        this.FK_Node = this.FK_Node.replace('#', '');
    }
    this.NodeID = getQueryStringByName("NodeID");
    this.FK_MapData = getQueryStringByName("FK_MapData");
    this.UserNo = getQueryStringByName("UserNo");
    this.FID = getQueryStringByName("FID");
    this.SID = getQueryStringByName("SID");
    this.CWorkID = getQueryStringByName("CWorkID");
    this.PWorkID = getQueryStringByName("PWorkID");
    this.PFlowNo = getQueryStringByName("PFlowNo");

    this.DoFunc = getQueryStringByName("DoFunc");
    this.CFlowNo = getQueryStringByName("CFlowNo");
    this.WorkIDs = getQueryStringByName("WorkIDs");
    this.IsReadonly = getQueryStringByName("IsReadonly");
    this.IsEdit = getQueryStringByName("IsEdit");
    this.IsLoadData = getQueryStringByName("IsLoadData");
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
        extUrl += "&SID=" + args.SID;

    if (args.CWorkID != "")
        extUrl += "&CWorkID=" + args.CWorkID;
    if (args.PWorkID != "")
        extUrl += "&PWorkID=" + args.PWorkID;
    if (args.PFlowNo != "")
        extUrl += "&PFlowNo=" + args.PFlowNo;
    if (args.IsLoadData != "")
        extUrl += "&IsLoadData=" + args.IsLoadData;

    //获取其他参数
    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] != sHref) /*参数不为空*/
    {
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