
/*
 公共的工作处理器js. 
 1. 该js的方法都是从各个类抽取出来的.
 2. MyFlowFool.htm, MyFlowFree.htm, MyFlowSelfForm.htm 引用它.
 3. 用于处理流程业务逻辑，表单业务逻辑.
*/


//初始化按钮
//var MyFlow = "MyFlow.ashx";
function initBar() {

    // 为啥要注释 else MyFlow = "MyFlow.do";
    if (plant == "CCFlow")
        MyFlow = "MyFlow.ashx";

    //else
    //MyFlow = "MyFlow.do";

    var url = MyFlow + "?DoType=InitToolBar&m=" + Math.random();

    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        url: url,
        dataType: 'html',
        success: function (data) {

            var barHtml = data;

            $('.Bar').html(barHtml);

            if ($('[name=Return]').length > 0) {
                $('[name=Return]').attr('onclick', '');
                $('[name=Return]').unbind('click');
                $('[name=Return]').bind('click', function () { initModal("returnBack"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=Shift]').length > 0) {

                $('[name=Shift]').attr('onclick', '');
                $('[name=Shift]').unbind('click');
                $('[name=Shift]').bind('click', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=Btn_WorkCheck]').length > 0) {

                $('[name=Btn_WorkCheck]').attr('onclick', '');
                $('[name=Btn_WorkCheck]').unbind('click');
                $('[name=Btn_WorkCheck]').bind('click', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=Askfor]').length > 0) {
                $('[name=Askfor]').attr('onclick', '');
                $('[name=Askfor]').unbind('click');
                $('[name=Askfor]').bind('click', function () { initModal("askfor"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=HuiQian]').length > 0) {
                $('[name=HuiQian]').attr('onclick', '');
                $('[name=HuiQian]').unbind('click');
                $('[name=HuiQian]').bind('click', function () { initModal("HuiQian"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=PackUp_zip]').length > 0) {
                $('[name=PackUp_zip]').attr('onclick', '');
                $('[name=PackUp_zip]').unbind('click');
                $('[name=PackUp_zip]').bind('click', function () { initModal("PackUp_zip"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=PackUp_html]').length > 0) {
                $('[name=PackUp_html]').attr('onclick', '');
                $('[name=PackUp_html]').unbind('click');
                $('[name=PackUp_html]').bind('click', function () { initModal("PackUp_html"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=PackUp_pdf]').length > 0) {
                $('[name=PackUp_pdf]').attr('onclick', '');
                $('[name=PackUp_pdf]').unbind('click');
                $('[name=PackUp_pdf]').bind('click', function () { initModal("PackUp_pdf"); $('#returnWorkModal').modal().show(); });
            }

            if ($('[name=SelectAccepter]').length > 0) {
                $('[name=SelectAccepter]').attr('onclick', '');
                $('[name=SelectAccepter]').unbind('click');
                $('[name=SelectAccepter]').bind('click', function () {
                    initModal("accepter");
                    $('#returnWorkModal').modal().show();
                });
            }

            if ($('[name=DBTemplate]').length > 0) {
                $('[name=DBTemplate]').attr('onclick', '');
                $('[name=DBTemplate]').unbind('click');
                $('[name=DBTemplate]').bind('click', function () {
                    initModal("DBTemplate");
                    $('#returnWorkModal').modal().show();
                });
            }

            
            if ($('[name=Delete]').length > 0) {
                var onclickFun = $('[name=Delete]').attr('onclick');
                if (onclickFun != undefined) {
                    if (plant == 'CCFlow') {
                        $('[name=Delete]').attr('onclick', onclickFun.replace('MyFlowInfo.htm', 'MyFlowInfo.aspx'));
                    } else {
                        $('[name=Delete]').attr('onclick', onclickFun.replace('MyFlowInfo.htm', 'MyFlowInfo.jsp'));
                    }
                }
            }
        }
    });
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode) {

    //初始化退回窗口的SRC
    var returnWorkModalHtml = '<div class="modal fade" id="returnWorkModal" data-backdrop="static">' +
       '<div class="modal-dialog">'
           + '<div class="modal-content" style="border-radius:0px;width:700px;text-align:left;">'
              + '<div class="modal-header">'
                  + '<button type="button" style="color:white;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
                   + '<h4 class="modal-title" id="modalHeader">工作退回</h4>'
               + '</div>'
               + '<div class="modal-body">'
                   + '<iframe style="width:100%;border:0px;height:400px;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
               + '</div>'
           + '</div><!-- /.modal-content -->'
       + '</div><!-- /.modal-dialog -->'
   + '</div>';

    $('body').append($(returnWorkModalHtml));

    var modalIframeSrc = '';
    if (modalType != undefined) {
        switch (modalType) {
            case "returnBack":
                $('#modalHeader').text("工作退回");
                modalIframeSrc = "./WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
            case "accpter":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "shift":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "./WorkOpt/Forward.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "askfor":
                $('#modalHeader').text("加签");
                modalIframeSrc = "./WorkOpt/Askfor.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "Btn_WorkCheck":
                $('#modalHeader').text("审核");
                modalIframeSrc = "./WorkOpt/WorkCheck.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "HuiQian":
                $('#modalHeader').text("会签");
                modalIframeSrc = "./WorkOpt/HuiQian.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "PackUp_zip":
            case "PackUp_html":
            case "PackUp_pdf":
                $('#modalHeader').text("打包下载/打印");
                var url = "./WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random();
                // alert(url);
                modalIframeSrc = "./WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "accepter":
                $('#modalHeader').text("选择下一个节点及下一个节点接受人");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;

            //发送选择接收节点和接收人     
            case "sendAccepter":
                $('#modalHeader').text("发送到节点：" + toNode.Name);
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode.No + "&s=" + Math.random()
                break;
            case "DBTemplate":
                $('#modalHeader').text("历史发起记录&模版");
                modalIframeSrc = "./WorkOpt/DBTemplate.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            default:
                break;
        }
    }
    $('#iframeReturnWorkForm').attr('src', modalIframeSrc);
}


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



//关注 按钮.
function FocusBtn(btn, workid) {

    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }

    var para = "DoType=Focus&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        // alert(msg);
    });
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.value == '确认') {
        btn.value = '取消确认';
    }
    else {
        btn.value = '确认';
    }

    var para = "DoType=Confirm&WorkID=" + workid;
    AjaxService(para, function (msg, scope) {
        //  alert(msg);
    });
}
