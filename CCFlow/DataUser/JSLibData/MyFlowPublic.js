
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

//function AfterWindowLoad() {
//}

////toolbar 加载后执行的事件.
//function AfterToolbarLoad() {
//}
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

    if (1 == 2) {
        //获得下拉框.
        var xb = $("#DDL_XB").val();
    }

 //   var xb = this.r $("#DDL_XB").val();

    return true;
}
 
function LoaclOperation() {

}
/*

1. beforeSave、beforeSend、 beforeReturn、 beforeDelete 
2 .MyFlowGener、MyFlowTree的固定方法，禁止删除
3.主要写保存前、发送前、退回前、删除前事件
4.返回值为 true、false,  如果返回false 就不执行该事件.

*/

//保存前事件
function beforeSave() {
    return true;
}

//发生前事件
function beforeSend() {
    var nodeID = GetQueryString("NodeID"); //获得当前节点.
    if (nodeID == 101) {
        var name = $("#TB_Name").val(); //可以使用jquery.
    }
    if (nodeID === 702)
    {
        return CheckBlank();
    }



    if (nodeID === 901) {

        //var km = $("#DDL_KM").val();
        //var hj = $("#TB_BaoXiaoJinE").val();
        //if (km == 0 && hj > 10000) {
        //    alert('报销金额不能超过1w.');
        //    return false;
        //}
        return true;
       // return CheckBlank();
    }

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

/**
 * 调用各项目各自电子签字代码
 */
function Siganture() {

}
/**
 * 调用各项目各自盖章代码
 */
function Stamp() {

}


