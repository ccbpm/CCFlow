/*
1. 该JS文件被嵌入到了MyFlowGener.htm 的工作处理器中. 
2. 开发者可以重写该文件处理通用的应用,比如通用的函数.
*/

function GenerNextStepEmp() {
    var qingjiaren = $("TB_QingJiaRen").val();

    var url = "xxx.aspx?QingJiaRen=" + qingjiaren;

    $("TB_DiYiJiShenPiRen").val("zhangsna");

    return "";
}

//页面启动函数.
$(document).ready(function () {
    //表单加载筛选发送下拉框（此方法在表单加载前执行的，取不到对应的表单元素，无法完成筛选）
    LoaclOperation();

});

/*
 * 此函数为发送前做JS效验检查demo所用.
 * 1. 函数里面可以使用jquery语法.
 * 2. 配置到发送JS按钮里 if (CheckBlank()==false) return true; 即可被调用.
 * 3. 此方法也可以写入到 xxxx_Self.js里面.
 * 4. return true执行发送动作，return false 阻止执行.
 **/
function CheckBlank() {
    if ($("#TB_Email").val() == null) {
        alert('邮件不能为空.');
        return false;
    }
    if ($("#TB_Tel").val() == null) {
        alert('电话不能为空.');
        return false;
    }
    return true;
}

function DZ() {

    alert('sss');
    var url = 'pop.htm';
    window.open(url);
}
function LoaclOperation() {

    if (GetQueryString("NodeID") != "202")
        return;
    var val = $("#DDL_SFBJBL option:selected").val();

    if (val == 0) {

        //启动子流程的选项设置为不可见
        $("#DDL_ToNode option").eq(0).hide();
        $("#DDL_ToNode option").eq(1).hide();
        $("#DDL_ToNode option").eq(2).show();

        //将Bar上的发送节点设置为结办节点
        $("#DDL_ToNode").val("203");

    }
    else {

        //显示启用启动子流程的选项
        $("#DDL_ToNode option").eq(0).show();
        $("#DDL_ToNode option").eq(1).show();
        $("#DDL_ToNode option").eq(2).hide();

        //将Bar上的发送节点设置为结办节点
        $("#DDL_ToNode").val("302");

    }
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


/**
 * 发送走之后，弹出来的消息.
 * @param {html格式的信息} msg
 */
function WindowCloseReloadPage(msg) {
    return;

    if ($('#returnWorkModal:hidden').length == 0 && $('#returnWorkModal').length > 0) {
        $('#returnWorkModal').modal('hide');
    }

    //增加msg的模态窗口
    //初始化退回窗口的SRC.
    var html = '<div class="modal fade" id="msgModal" data-backdrop="static">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content" style="border-radius: 0px;">'
        + '<div class="modal-header" style="background:#f2f2f2;">'
        + '<button type="button" class="close" id="btnMsgModalOK1" aria-hidden="true" style="color: #0000007a;display: none;">&times;</button>'
        + '<h4 class="modal-title" style="color:#000;">提示信息</h4>'
        + '</div>'
        + '<div class="modal-body" style="text-align: left; word-wrap: break-word;">'
        + '<div style="width:100%; border: 0px; height: 200px;overflow-y:auto" id="msgModalContent" name="iframePopModalForm"></div>'
        + '<div style="text-align: right;">'
        + ' <button type="button" id="btnMsgModalOK" class="btn" data-dismiss="modal">确定(30秒)</button >'
        + '</div>'
        + '</div>'
        + '</div><!-- /.modal-content -->'
        + '</div><!-- /.modal-dialog -->'
        + '</div>';

    $('body').append($(html));
    if (msg == null || msg == undefined)
        msg = "";
    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')

    $("#msgModalContent").html(msg.replace(/@/g, '<br/>').replace(/null/g, ''));
    var trackA = $('#msgModalContent a:contains("工作轨迹")');
    var trackImg = $('#msgModalContent img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();

    $('#btnMsgModalOK').bind('click', function () {
        var id = window.parent.nthTabs.getActiveId();
        var idlist = id.split("TLJ");
        //console.log("==="+idlist);
        if (idlist.length > 0) {
            $('#' + idlist[1], parent.document).attr('src', $('#' + idlist[1], parent.document).attr('src'));
        }
        window.parent.nthTabs.delTab(id);
    });
    $('#btnMsgModalOK1').bind('click', function () {
        //提示消息有错误，页面不跳转
        var msg = $("#msgModalContent").html();
        if (msg.indexOf("err@") == -1) {
            window.close();
        }
        else {
            setToobarEnable();
            $("#msgModal").modal("hidden");
        }

        if (window.parent != null && window.parent != undefined)
            window.parent.close();
        opener.window.focus();
    });

    $("#msgModal").modal().show();
    interval = setInterval("clock()", 1000);
}


