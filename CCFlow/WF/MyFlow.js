/*
 公共的工作处理器js. 
 1. 该js的方法都是从各个类抽取出来的.
 2. MyFlowFool.htm, MyFlowFree.htm, MyFlowSelfForm.htm 引用它.
 3. 用于处理流程业务逻辑，表单业务逻辑.
*/

//停止流程.
function DoStop(msg, flowNo, workid) {

    if (confirm('您确定要执行 [' + msg + '] ?') == false)
        return;

    var para = 'DoType=MyFlow_StopFlow&FK_Flow=' + flowNo + '&WorkID=' + workid;

    AjaxService(para, function (msg, scope) {

        alert(msg);
        if (msg.indexOf('err@') == 0) {
            return;
        } else {
            window.close();
        }
    });
}


//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    window.location.href = window.history.url;
}
function To(url) {
    //window.location.href = url;
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}

function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;

    var para = 'DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;

    AjaxService(para, function (msg, scope) {
        alert(msg);
        window.location.href = window.location.href;
    });
}

//公共方法
function AjaxService(param, callback, scope, levPath) {
    $.ajax({
        type: "GET", //使用GET或POST方法访问后台
        dataType: "text", //返回json格式的数据
        contentType: "application/json; charset=utf-8",
        url: MyFlow, //要访问的后台地址
        data: param, //要发送的数据
        async: true,
        cache: false,
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