var flowNo = null;

$(function () {
    return;
    var Guide = $("#Guide");
    var str = '';
    str = "<style>.Guide{position:fixed;bottom:0px; z-index:999; width:100%;height:50px; background:rgba(0,0,0,0.6);}.Guide .guideb i{color:#fff;font-size:18px;padding-right:15px;padding-top:4px;}.Guide .guideb{float:left;width:20%;text-align:center;line-height:50px;position:relative;font-family:'Microsoft YaHei';font-size:18px;color:#fff}.guideb:after{content:'';border-width:12px;position:absolute;display:block;width:0;height:0;border-color:transparent;border-style:solid;top:14px;right:0;margin-left:1px;border-right-width:0;border-left-color:#fff}.guideb:last-child::after{content:'';border-width:12px;position:absolute;display:block;width:0;height:0;border-color:transparent;border-style:solid;top:10px;right:0;margin-left:1px;border-right-width:0;border-left-color:none} .guideicon {line-height: 50px; text-align: center; color:#fff}.guideicon i {line-height: 50px;}</style>"
    str +="<div class='Guide'>"
    flowNo = GetQueryString("FlowNo");
    if (flowNo == null)
        flowNo = GetQueryString("FK_Flow");

    var dbs = GetDBDtl();
    console.log(dbs.length)
    for (i = 0; i < dbs.length; i++) {
        var node = dbs[i];
        str += "<div class=guideb onclick=\"GetUrl('" + node.No + "')\" ><i class='iconfont "+ node.ICON +"' ></i> " + node.Name+"</div>";
    }
    str += '<div class=guideicon><i class="iconfont icon-you2"></i></div>'
    str += '</div>';
    Guide.html(str);
    return;
});


function GetDBDtl() {

    var json = [
        { "No": "Flow", "Name": "流程", "ICON": " icon-Track" },
        { "No": "Frm", "Name": "表单", "ICON": " icon-biaoge" },
        { "No": "Accepter", "Name": "接受人", "ICON": " icon-Shift" },
        { "No": "TestingContainer", "Name": "测试运行", "ICON": " icon-Send" }
    ];
    return json;
}

function GetUrl(funcID) {
    alert(funcID);

    if (funcID == "Accepter") return Accepter();
    if (funcID == "Flow") return Flow();
    if (funcID == "Frm") return Frm();
    if (funcID == "TestingContainer") return Frm();
}

function Frm() {
    var url = "./BatchSetting/Default.htm?FlowNo=" + flowNo + "&FK_Flow=" + flowNo;
}
function Flow() {
    var url = "./BatchSetting/Default.htm?FlowNo=" + flowNo + "&FK_Flow=" + flowNo;
}
function Accepter() {
    var url = "./BatchSetting/Default.htm?FlowNo=" + flowNo + "&FK_Flow=" + flowNo;
}

