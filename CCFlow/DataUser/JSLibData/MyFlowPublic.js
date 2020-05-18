/*
1. 该JS文件被嵌入到了MyFlowGener.htm 的工作处理器中. 
2. 开发者可以重写该文件处理通用的应用,比如通用的函数.

*/
//为广西计算中心增加初始化引用js文件
$(function () {
  
    

    
});
//外发公文.
function WaiFa() {

    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var nodeid = GetQueryString('NodeID');


    var url = "../App/WaiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&NodeID=" + nodeid;
    WinOpen(url);
    //window.open(url);
}

//转内发公文.
function NeiFa()
{
    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var mypk = GetQueryString('MyPK');
    var pkVal = GetQueryString('PKVal');

    var url = "../App/NeiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&MyPK=" + pkVal;
    WinOpen(url);
}

function DZ() {

    alert('sss');
    var url = 'pop.htm';
    window.open(url);
}

/*

1. beforeSave、beforeSend、 beforeReturn、 beforeDelete 
2 .MyFlowGener、MyFlowTree的固定方法，禁止删除
3.主要写保存前、发送前、退回前、删除前事件
4.返回值为 true、false

*/

//保存前事件
function beforeSave() {
    return true;
}

//发生前事件
function beforeSend() {
    return true;
}

//退回前事件
function beforeReturn() {
    return true;
}

//删除前事件
function beforeDelete() {
    return true;
}


//抄送阅读页面增加关闭前事件
function beforeCCClose() {
    return true;
}
//广西抄送阅读页面增加关闭前事件
function beforeCCClose() {
    $("#TB_Msg").val("已阅");
   
    if ($("#TB_Msg").val() == null || $("#TB_Msg").val() == "" || $("#TB_Msg").val().trim().length == 0) {
        alert("请填写评论内容!");
        return;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    handler.AddFormData();
    var data = handler.DoMethodReturnString("FlowBBS_Save");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    return true;
}
//广西抄送阅读页面增加关闭前事件结束
//广西计算中心打开公文的方式
function openhtml() {
    var strUrl = "office.html";
    var dataSendToChild = $("#param").val();
    var ntkoed = ntkoBrowser.ExtensionInstalled();
    if (ntkoed) {
        ntkoBrowser.openWindow(strUrl, "", "", "", "", "", dataSendToChild);
    } else {
        //没有安装跨浏览器控件的话提示下载安装
        var iTop = ntkoBrowser.NtkoiTop(); //获得窗口的垂直位置;
        var iLeft = ntkoBrowser.NtkoiLeft(); //获得窗口的水平位置;
        window.open("downloadExe.html", "", "height=200px,width=500px,top=" + iTop + "px,left=" + iLeft +
            "px,titlebar=no,toolbar=no,menubar=no,scrollbars=auto,resizeable=no,location=no,status=no");
    }
}

//在父页面定义的跨浏览器插件应用程序关闭事件响应方法，且方法名不能自定义，必须是ntkoCloseEvent
function ntkoCloseEvent() {
    console.log("跨浏览器插件应用程序窗口已关闭!");
}

//在父页面定义的用于接收子页面回传值的方法，方法名可以自定义，
//定义后的方法名需要在子页面中通过window.external.SetReturnValueToParentPage进行注册
function OnData(callData) {
    var data = decodeURI(callData);
    $("#callback").val(data);
}

//广西计算中心打开公文的方式
