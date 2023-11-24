var _MsgDialogT = null;
//弹出消息
function MessageShow(msg, autoClose) {
    if (autoClose == true) {
        $.mobile.loading().loader("show", "c", msg, true);
        _MsgDialogT = setTimeout("MsgHidenLoader()", 2000);
    } else {
        $.mobile.loading().loader("show", "c", "", false);
        //最长15秒自动隐藏
        _MsgDialogT = setTimeout("MsgHidenLoader()", 15000);
    }
}
//关闭消息
function MsgHidenLoader() {
    $.mobile.loading().loader('hide');
    if (_MsgDialogT != null) {
        clearTimeout(_MsgDialogT);
    }
}

//确定消息
function UpdateStatus(msgConfirm, ConfirmYesFun) {
    var popupDialogId = 'popupDialog';
    var msgConfirmPop = "是否确定？";
    if (msgConfirm) {
        msgConfirmPop = msgConfirm;
    }

    var html = "<div data-role='popup' id='" + popupDialogId + "' data-confirmed='no' data-transition='pop' data-overlay-theme='a' data-theme='d' data-dismissible='false' style='min-width:216px;max-width:500px;'>";
    html+="     <div role='main' class='ui-content'>";
    html+="         <h3 class='ullabel' style='text-align:center;margin-bottom:15px'>" + msgConfirmPop + "</h3>";
    html+="         <a href='#' class='ui-btn ui-corner-all ui-btn-inline ui-btn-b ullabel optionConfirm' data-rel='back' style='width: 33%;border-radius: 5px;height: 30px;line-height: 30px;padding: 0;margin: 0 0 0 12%;'>确定</a>";
    html+= "        <a href='#' class='ui-btn ui-corner-all ui-btn-inline ui-btn-b ullabel optionCancel' data-rel='back' style='width: 33%;border-radius: 5px;height: 30px;line-height: 30px;padding: 0;margin: 0 0 0 5%;'>取消</a>";
    html+="     </div>";
    html+="</div>";
    $(html).appendTo($.mobile.pageContainer);
    
    var popupDialogObj = $('#' + popupDialogId);
    popupDialogObj.trigger('create');
    popupDialogObj.popup({
        afterclose: function (event, ui) {
            popupDialogObj.find(".optionConfirm").first().off('click');
            var isConfirmed = popupDialogObj.attr('data-confirmed') === 'yes' ? true : false;
            $(event.target).remove();
            if (isConfirmed) {
                //这里执行确认需要执行的代码
                if (ConfirmYesFun)
                    ConfirmYesFun(isConfirmed);
            }
        }
    });

    popupDialogObj.popup('open');
    popupDialogObj.find(".optionConfirm").first().on('click', function () {
        popupDialogObj.attr('data-confirmed', 'yes');
    });
}

//公共方法
//function AjaxMobileService(param, callback, scope, method, showErrMsg, path) {
//    if (!path) path = "common/action.ashx";
//    if (!method) method = 'GET';
//    $.ajax({
//        type: method, //使用GET或POST方法访问后台
//        dataType: "text", //返回json格式的数据
//        contentType: "application/json; charset=utf-8",
//        url: path, //要访问的后台地址
//        data: param, //要发送的数据
//        async: true,
//        cache: false,
//        complete: function () { }, //AJAX请求完成时隐藏loading提示
//        error: function (XMLHttpRequest, errorThrown) {
//            if (showErrMsg) {
//                callback(showErrMsg);
//            } else {
//                callback(XMLHttpRequest);
//            }
//        },
//        success: function (msg) {//msg为返回的数据，在这里做数据绑定
//            callback(msg, scope);
//        }
//    });
//}