function InitToolBar() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Node", pageData.FK_Node);
    handler.AddPara("FK_Flow", pageData.FK_Flow);
    handler.AddPara("WorkID", pageData.WorkID);
    handler.AddPara("FID", pageData.FID);

    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("InitToolBar"); //执行保存方法.

    var barHtml = data;

    $('.Bar').html(barHtml);

    if ($('[name=Return]').length > 0) {
        $('[name=Return]').attr('onclick', '');
        $('[name=Return]').unbind('click');
        $('[name=Return]').bind('click', function () {
            //增加退回前的事件
            if (typeof beforeReturn != 'undefined' && beforeReturn instanceof Function)
                if (beforeReturn() == false)
                    return false;

            if (Save() == false) return;
            initModal("returnBack");
            $('#returnWorkModal').modal().show();
        });
    }

    //流转自定义
    if ($('[name=TransferCustom]').length > 0) {
        $('[name=TransferCustom]').attr('onclick', '');
        $('[name=TransferCustom]').unbind('click');
        $('[name=TransferCustom]').bind('click', function () {
            initModal("TransferCustom");
            $('#returnWorkModal').modal().show();
        });
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

    if ($('[name=Track]').length > 0) {
        $('[name=Track]').attr('onclick', '');
        $('[name=Track]').unbind('click');
        $('[name=Track]').bind('click', function () { initModal("Track"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=HuiQian]').length > 0) {
        $('[name=HuiQian]').attr('onclick', '');
        $('[name=HuiQian]').unbind('click');
        $('[name=HuiQian]').bind('click', function () { initModal("HuiQian"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=AddLeader]').length > 0) {
        $('[name=AddLeader]').attr('onclick', '');
        $('[name=AddLeader]').unbind('click');
        $('[name=AddLeader]').bind('click', function () { initModal("AddLeader"); $('#returnWorkModal').modal().show(); });
    }


    if ($('[name=CC]').length > 0) {
        $('[name=CC]').attr('onclick', '');
        $('[name=CC]').unbind('click');
        $('[name=CC]').bind('click', function () { initModal("CC"); $('#returnWorkModal').modal().show(); });
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
        $('[name=Delete]').attr('onclick', '');
        $('[name=Delete]').unbind('click');
        $('[name=Delete]').bind('click', function () {
            //增加删除前事件
            if (typeof beforeDelete != 'undefined' && beforeDelete instanceof Function)
                if (beforeDelete() == false)
                    return false;

            DeleteFlow();
        });
    }

    if ($('[name=CH]').length > 0) {

        $('[name=CH]').attr('onclick', '');
        $('[name=CH]').unbind('click');
        $('[name=CH]').bind('click', function () { initModal("CH"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Note]').length > 0) {

        $('[name=Note]').attr('onclick', '');
        $('[name=Note]').unbind('click');
        $('[name=Note').bind('click', function () { initModal("Note"); $('#returnWorkModal').modal().show(); });
    }

}

function setModalMax() {
    //设置bootstrap最大化窗口
    //获取width
    var w = ddocument.body.clientWidth - 40;
    $("#returnWorkModal .modal-dialog").css("width", w + "px");
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode) {

    //初始化退回窗口的SRC.
    var html = '<div class="modal fade" id="returnWorkModal" data-backdrop="static">' +
        '<div class="modal-dialog">'
        + '<div class="modal-content" style="border-radius:0px;width:900px;height:450px;text-align:left;">'
        + '<div class="modal-header">'
        + '<button type="button" style="color:#0000007a;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<button id="MaxSizeBtn" type="button" style="color:#0000007a;float: right;background: transparent;border: none;" aria-hidden="true" >□</button>'
        + '<h4 class="modal-title" id="modalHeader">提示信息</h4>'
        + '</div>'
        + '<div class="modal-body" style="margin:0px;padding:0px;height:450px">'
        + '<iframe style="width:100%;border:0px;height:100%;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
        + '</div>'
        + '</div><!-- /.modal-content -->'
        + '</div><!-- /.modal-dialog -->'
        + '</div>';

    $('body').append($(html));

    $("#returnWorkModal").on('hide.bs.modal', function () {
        setToobarEnable();
    });
    $("#MaxSizeBtn").click(function () {
        var w = document.body.clientWidth - 80;
        var h = document.body.clientHeight - 80;

        $("#returnWorkModal .modal-dialog").css("width", w + "px");
        $("#returnWorkModal .modal-dialog").css("height", h + "px");

        $("#returnWorkModal .modal-content").css("width", w + "px");
        $("#returnWorkModal .modal-content").css("height", h + "px");
        $("#returnWorkModal .modal-content .modal-body").css("height", h + "px");

    });
    var modalIframeSrc = '';
    if (modalType != undefined) {
        switch (modalType) {
            case "returnBack":
                $('#modalHeader').text("提示信息");
                modalIframeSrc = "./WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
            case "TransferCustom":
                $('#modalHeader').text("流转自定义");
                //设置弹出页面的宽度高度
                var w = document.body.clientWidth - 300;
                var h = document.body.clientHeight - 40;
                $("#returnWorkModal .modal-dialog").css("width", w + "px");
                $("#returnWorkModal .modal-dialog").css("height", h + "px");

                $("#returnWorkModal .modal-content").css("width", w + "px");
                $("#returnWorkModal .modal-content").css("height", h + "px");
                $("#returnWorkModal .modal-content .modal-body").css("height", h + "px");
                modalIframeSrc = "./WorkOpt/TransferCustom.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
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

            case "Track": //轨迹.
                $('#modalHeader').text("轨迹");
                modalIframeSrc = "./WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "HuiQian":

                if (toNode != null)
                    $('#modalHeader').text("先会签，后发送。");
                else
                    $('#modalHeader').text("会签");

                modalIframeSrc = "./WorkOpt/HuiQian.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random()

                break;
            case "AddLeader":
                $('#modalHeader').text("加组长");
                modalIframeSrc = "./WorkOpt/HuiQian.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&HuiQianType=AddLeader&s=" + Math.random()

                break;
            case "CC":
                $('#modalHeader').text("抄送");
                modalIframeSrc = "./WorkOpt/CC.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random()
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
                $('#modalHeader').text("选择接受人");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&s=" + Math.random()
                break;
            case "DBTemplate":
                $('#modalHeader').text("历史发起记录&模版");
                modalIframeSrc = "./WorkOpt/DBTemplate.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "CH":
                $('#modalHeader').text("节点时限");
                modalIframeSrc = "./WorkOpt/CH.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random();
                break;
            case "Note":
                $('#modalHeader').text("备注");
                modalIframeSrc = "./WorkOpt/Note.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random();

            default:
                break;
        }
    }
    $('#iframeReturnWorkForm').attr('src', modalIframeSrc);
}