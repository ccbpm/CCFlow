﻿
$(function () {
    var barHtml="";
    var data;
    //MyCC
    if ($("#JS_CC").length == 1) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyCC");
        handler.AddUrlData();
        data = handler.DoMethodReturnString("InitToolBar");
        $('#ToolBar').html(barHtml);
    //MyView
    } else if ($("#JS_MyView").length == 1){
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyView");
        handler.AddUrlData();
        data = handler.DoMethodReturnString("InitToolBar");
    //MyFlow   
    } else {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddUrlData();
        data = handler.DoMethodReturnString("InitToolBar"); //执行保存方法.
    }

    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }
    data = JSON.parse(data);
    var toolBars = data.ToolBar;
    if (toolBars == undefined)
        toolBars = data;
    var _html = "";
    $.each(toolBars, function (i, toolBar) {
        var Oper = "";
        if (toolBar.Oper != "")
            Oper = "onclick=\"" + toolBar.Oper + "\"";

        if (toolBar.No == "NodeToolBar")
            //_html += "<input type=button  value='" + toolBar.Name + "' enable=true " + Oper + "/>";
            _html += "<input type=button  value='" + toolBar.Name + "' enable=true " + Oper + "/>";
        else
            _html += "<Button type=button name='" + toolBar.No + "' enable=true " + Oper + "><img src='Img/Btn/" + toolBar.No + ".png' width='15px' height='15px'>&nbsp;" + toolBar.Name + "</button>";
    });
    $('#ToolBar').html(_html);


    //按钮旁的下来框
    InitToNodeDDL(data);
   

    if ($('[name=Return]').length > 0) {
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
        $('[name=TransferCustom]').bind('click', function () {
            initModal("TransferCustom");
            $('#returnWorkModal').modal().show();
        });
    }


    if ($('[name=Shift]').length > 0) {
        $('[name=Shift]').bind('click', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Btn_WorkCheck]').length > 0) {
        $('[name=Btn_WorkCheck]').bind('click', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Askfor]').length > 0) {
        $('[name=Askfor]').bind('click', function () { initModal("askfor"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Track]').length > 0) {
        $('[name=Track]').bind('click', function () { initModal("Track"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=HuiQian]').length > 0) {
        $('[name=HuiQian]').bind('click', function () { initModal("HuiQian"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=AddLeader]').length > 0) {
        $('[name=AddLeader]').bind('click', function () { initModal("AddLeader"); $('#returnWorkModal').modal().show(); });
    }


    if ($('[name=CC]').length > 0) {
        $('[name=CC]').bind('click', function () { initModal("CC"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=PackUp_zip]').length > 0) {
        $('[name=PackUp_zip]').bind('click', function () { initModal("PackUp_zip"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=PackUp_html]').length > 0) {
        $('[name=PackUp_html]').bind('click', function () { initModal("PackUp_html"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=PackUp_pdf]').length > 0) {
        $('[name=PackUp_pdf]').bind('click', function () { initModal("PackUp_pdf"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=SelectAccepter]').length > 0) {
        $('[name=SelectAccepter]').bind('click', function () {
            initModal("accepter");
            $('#returnWorkModal').modal().show();
        });
    }

    if ($('[name=DBTemplate]').length > 0) {
        $('[name=DBTemplate]').bind('click', function () {
            initModal("DBTemplate");
            $('#returnWorkModal').modal().show();
        });
    }

    if ($('[name=Delete]').length > 0) {
        $('[name=Delete]').bind('click', function () {
            //增加删除前事件
            if (typeof beforeDelete != 'undefined' && beforeDelete instanceof Function)
                if (beforeDelete() == false)
                    return false;

            DeleteFlow();
        });
    }

    if ($('[name=CH]').length > 0) {
        $('[name=CH]').bind('click', function () { initModal("CH"); $('#returnWorkModal').modal().show(); });
    }

    if ($('[name=Note]').length > 0) {
        $('[name=Note').bind('click', function () { initModal("Note"); $('#returnWorkModal').modal().show(); });
    }

    //公文
    if ($('[name=DocWord]').length > 0) {
        $('[name=DocWord').bind('click', function () { initModal("DocWord"); $('#returnWorkModal').modal().show(); });
    }
   
});

function setModalMax() {
    //设置bootstrap最大化窗口
    var w = ddocument.body.clientWidth - 40;
    $("#returnWorkModal .modal-dialog").css("width", w + "px");
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode,url) {
    if ("undefined" != typeof flowData && flowData != null && flowData != undefined) {
        var node = flowData.WF_Node[0];
        if (node.FormType == 12 || (node.FormType == 11 && flowData.FrmNode[0] != null && flowData.FrmNode[0].FrmType == 8)) {
            if (modalType == "PackUp_pdf" || modalType == "PackUp_html" || modalType == "PackUp_zip") {
                PrintPDF(modalType.replace("PackUp_",""));
                return;
            }
        }
    }
    


    //初始化退回窗口的SRC.
    var html = '<div style=" height:auto;" class="modal fade" id="returnWorkModal" data-backdrop="static">' +
        '<div class="modal-dialog" style="border-radius:0px;width:920px;">'
        + '<div class="modal-content" id="viewModal" style="border-radius:0px;height:560px;">'
        + '<div class="modal-header" style="background-color:#f2f2f2;margin-bottom:8px">'
        + '<button id="ClosePageBtn" type="button" style="color:#000000;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
        + '<button id="MaxSizeBtn" type="button" style="color:#000000;float: right;background: transparent;border: none;" aria-hidden="true" >□</button>'
        + '<h4 class="modal-title" id="modalHeader" style="color:#000000;">提示信息</h4>'
        + '</div>'
        + '<div class="modal-body" style="margin:0px;padding:0px;height:560px">'
        + '<iframe style="width:100%;border:0px;height:100%;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
        + '</div>'
        + '</div><!-- /.modal-content -->'
        + '</div><!-- /.modal-dialog -->'
        + '</div>';

    if ($("#returnWorkModal").length == 0)
        $('body').append($(html));

    if ($("#returnWorkModal .modal-footer").length == 1)
        $("#returnWorkModal .modal-footer").remove();
   
    $("#returnWorkModal").on('hide.bs.modal', function () {
        setToobarEnable();
    });
    $("#MaxSizeBtn").click(function () {
     
        //按百分比自适应
        SetPageSize(100, 100);
    });

    var modalIframeSrc = '';
    if (modalType != undefined) {

        switch (modalType) {
            case "returnBack":
                $('#modalHeader').text("提示信息");
                //按百分比自适应
                SetPageSize(50, 60);
                var node = new Entity("BP.WF.Template.NodeExt", paramData.FK_Node);
                var info = "";
                if (node.ReturnField != "") {
                    if ($("#" + node.ReturnField).length == 1)
                        info = $("#" + node.ReturnField).val();
                    if (info == undefined && $("#TB_" + node.ReturnField).length == 1)
                        info = $("#TB_" + node.ReturnField).val();
                    if (info == undefined)
                        info = "";
                }
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/ReturnWork.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=" + info + "&s=" + Math.random()
                break;
            case "Send":
                SetChildPageSize(80, 80);
                break;
            case "TransferCustom":
                $('#modalHeader').text("流转自定义");
              
                //按百分比自适应
                SetPageSize(60, 60);

                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/TransferCustom.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random()
                break;
            case "accpter":
                $('#modalHeader').text("工作移交");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "shift":
                $('#modalHeader').text("工作移交");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Forward.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "DocWord":
                $('#modalHeader').text("公文");
                SetPageSize(40, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/DocWord.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "askfor":
                $('#modalHeader').text("加签");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Askfor.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "Btn_WorkCheck":
                $('#modalHeader').text("审核");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/WorkCheck.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;

            case "Track": //轨迹.
                $('#modalHeader').text("轨迹");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "HuiQian":

                if (toNode != null) {
                    $('#modalHeader').text("先会签，后发送。");
                    SetPageSize(80, 80);
                }
                else {
                    $('#modalHeader').text("会签");
                    SetPageSize(80, 80);
                }

                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random()

                break;
            case "AddLeader":
                $('#modalHeader').text("加主持人");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&HuiQianType=AddLeader&s=" + Math.random()

                break;
            case "CC":
                $('#modalHeader').text("抄送");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/CC.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random()
                break;
            case "PackUp_zip":
            case "PackUp_html":
            case "PackUp_pdf":
                $('#modalHeader').text("打包下载/打印");

                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "accepter":
                $('#modalHeader').text("选择下一个节点及下一个节点接受人");
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random()
                break;

            //发送选择接收节点和接收人                
            case "sendAccepter":
                $('#modalHeader').text("选择接受人");
                SetPageSize((80, 80);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random()
                break;
            case "SelectNodeUrl":
                modalIframeSrc = url;
                break;

            case "BySelfUrl"://接收人规则自定义URL
                SetPageSize(80, 80);
                modalIframeSrc = url;
                if ($("#returnWorkModal .modal-footer").length == 0) {
                    var footBtn = $('<div class="modal-footer"><div>');
                    $("#returnWorkModal .modal-body").after(footBtn);
                    var footerOK = $('<button type="button" class="Btn" data-dismiss="modal"> 发送</button >');
                    footerOK.click(function () {

                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                        handler.AddUrlData();
                        var data = handler.DoMethodReturnString("Send");
                        if (data.indexOf("err@") != -1) {
                            var reg = new RegExp('err@', "g")
                            var data = data.replace(reg, '');

                            $(".modal-body").html(data);
                            $('#MessageDiv').modal().show();
                            setToobarEnable();
                            return;
                        }

                        OptSuc(data);
                    });

                    var footerClose = $('<button type="button" class="Btn" data - dismiss="modal" > 取消</button >');
                    footerClose.click(function () {
                        var sels = new Entities("BP.WF.Template.SelectAccpers");
                        sels.Delete("WorkID", GetQueryString("WorkID"), "FK_Node", getQueryStringByNameFromUrl(url, "ToNode"));
                        $("#returnWorkModal").modal('hide');

                    });

                   
                    footBtn.append(footerOK).append(footerClose);
                }
                $(".modal-content").css("height", "auto");
                
                break;
            case "sendAccepterOfOrg":
                $('#modalHeader').text("选择接受人");
                SetPageSize(80, 80);
                modalIframeSrc = ccbpmPath + "/WF/WorkOpt/AccepterOfOrg.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random()
                break;
            case "DBTemplate":
                $('#modalHeader').text("历史发起记录&模版");
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/DBTemplate.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "CH":
                $('#modalHeader').text("节点时限");
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/CH.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random();
                break;
            case "Note":
                $('#modalHeader').text("备注");
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Note.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random();
            case "PR":
                $('#modalHeader').text("重要性设置");
                //按百分比自适应
                SetPageSize(50, 60);
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/PRI.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PRIEnable=" + node.PRIEnable + "&Info=&s=" + Math.random();

            default:
                break;
        }
    }
    $('#iframeReturnWorkForm').attr('src', modalIframeSrc);
   
    
}

//设置弹出页面比例
function SetPageSize(w, h) {
    $("#returnWorkModal .modal-dialog").css("width", w + "%");
    $("#returnWorkModal .modal-dialog").css("height", h + "%");

    $("#returnWorkModal .modal-content").css("width", "100%");
    $("#returnWorkModal .modal-content").css("height", "100%");
    $("#returnWorkModal .modal-content .modal-body").css("height", "100%");

}

//禁用按钮功能
function setToobarDisiable() {
    $('#ToolBar input').css('background', 'gray');
    $('#ToolBar input').attr('disabled', 'disabled');
}

//启用按钮功能
function setToobarEnable() {
    $('#ToolBar input').css('background', '');
    $('#ToolBar input').removeAttr('disabled');
}

//初始化发送节点下拉框
function InitToNodeDDL(JSonData) {

    if (JSonData.ToNodes == undefined)
        return;

    if (JSonData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=Send]');
    if (btn == null || btn == undefined) {
        window.location.href = window.location.href;
        return;
    }

    //如果是会签且不是主持人时，则发送给主持人，不需要选择下一个节点和接收人
    if (btn.length != 0) {
        var dataType = $(btn[0]).attr("data-type");
        if (dataType != null && dataType != undefined && dataType == "isAskFor")
            return;
    }
    var toNodeDDL = $('<select style="width:auto;" id="DDL_ToNode"></select>');
    $.each(JSonData.ToNodes, function (i, toNode) {
        var opt = "";
        if (toNode.IsSelected == "1") {
            var opt = $("<option value='" + toNode.No + "' selected='true' >" + toNode.Name + "</option>");
            opt.data(toNode);
        } else {
            var opt = $("<option value='" + toNode.No + "'>" + toNode.Name + "</option>");
            opt.data(toNode);
        }

        toNodeDDL.append(opt);

    });

    $('[name=Send]').after(toNodeDDL);
}

/**
 * 流程发送的方法,这个是通用的方法
 * @param {isHuiQian} isHuiQian 是否是会签模式
 * @param {formType} formType 表单方案模式
 */
function Send(isHuiQian,formType) {
    SetPageSize(80, 80);

    /**发送前处理的信息 Start**/
    //SDK表单
    if (formType == 3) {
        if (SDKSend() == false)
            return;
    }

    //嵌入式表单
    if (formType == 2) {
        if (SendSelfFrom() == false)
            return;
    }
    //表单方案：傻瓜表单、自由表单、开发者表单、累加表单、绑定表单库的表单（单表单)
    if (formType == 0 || formType == 1 || formType == 10 || formType == 11 || formType == 12) {
        if (NodeFormSend() == false)
            return;
    }

    //绑定多表单
    if (formType == 5)
        if (FromTreeSend() == false)
            return;

    /**发送前处理的信息 End**/

    window.hasClickSend = true; //标志用来刷新待办.

    var toNodeID = 0;

    //含有发送节点 且接收
    if ($('#DDL_ToNode').length > 0) {
        var selectToNode = $('#DDL_ToNode  option:selected').data();
        toNodeID = selectToNode.No;
        if (selectToNode.IsSelectEmps == "1") { //跳到选择接收人窗口
            Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
                $('#returnWorkModal').modal().show();
            } else {
                initModal("sendAccepter", toNodeID);
                $('#returnWorkModal').modal().show();
            }
            return false;
        }
        if (selectToNode.IsSelectEmps == "2") {
            Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
                $('#returnWorkModal').modal().show();
            } else {
                var url = selectToNode.DeliveryParas;
                if (url != null && url != undefined && url!= "") {
                    url = url + "?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNodeID + "&s=" + Math.random();
                    initModal("BySelfUrl", toNodeID, url);
                    $('#returnWorkModal').modal().show();
                    return false;
                }

                
            }
            
        }

        if (selectToNode.IsSelectEmps == "3") {
            Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
                $('#returnWorkModal').modal().show();
            } else {
                initModal("sendAccepterOfOrg", toNodeID);
                $('#returnWorkModal').modal().show();
            }

            return false;
        }
       
        if (isHuiQian == true) {
            Save(1); //执行保存.
            initModal("HuiQian", toNodeID);
            $('#returnWorkModal').modal().show();
            return false;
        }
        
    }

    //执行发送.
    execSend(toNodeID, formType);
}

function execSend(toNodeID, formType) {

    //先设置按钮等不可用.
    setToobarDisiable();

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    if (formType != 3 && formType!=2) {
        //组织数据.
        var dataStrs = getFormData(true, true);
        $.each(dataStrs.split("&"), function (i, o) {
            //计算出等号的INDEX
            var indexOfEqual = o.indexOf('=');
            var objectKey = o.substr(0, indexOfEqual);
            var objectValue = o.substr(indexOfEqual + 1);
            if (validate(objectValue)) {
                handler.AddPara(objectKey, objectValue);
            } else {
                handler.AddPara(objectKey, "");
            }
        });
    }
    handler.AddPara("ToNode", toNodeID);
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("Send"); //执行保存方法.

    if (data.indexOf('err@') == 0) { //发送时发生错误

        var reg = new RegExp('err@', "g")
        var data = data.replace(reg, '');

        $('#Message').html(data);
        $('#MessageDiv').modal().show();
        setToobarEnable();
        return;
    }

    if (data.indexOf('TurnUrl@') == 0) {  //发送成功时转到指定的URL 
        var url = data;
        url = url.replace('TurnUrl@', '');
        window.location.href = url;
        return;
    }

    

    if (data.indexOf('SelectNodeUrl@') == 0) {
        var url = data;
        url = url.replace('SelectNodeUrl@', '');
        initModal("SelectNodeUrl",null,url); $('#returnWorkModal').modal().show();
        return;
    }

    if (data.indexOf('BySelfUrl@') == 0) {  //发送成功时转到自定义的URL 
        var url = data;
        url = url.replace('BySelfUrl@', '');
        initModal("BySelfUrl", null, url); $('#returnWorkModal').modal().show();
        return;
    }


    if (data.indexOf('url@') == 0) {  //发送成功时转到指定的URL 

        if (data.indexOf("AccepterOfOrg")!=-1) {
            var params = data.split("&");

            for (var i = 0; i < params.length; i++) {
                if (params[i].indexOf("ToNode") == -1)
                    continue;

                toNodeID = params[i].split("=")[1];
                break;
            }
            initModal("sendAccepterOfOrg", toNodeID);
            $('#returnWorkModal').modal().show();
            return;
        }

        if (data.indexOf('Accepter') != 0 && data.indexOf('AccepterGener') == -1) {

            //求出来 url里面的FK_Node=xxxx 
            var params = data.split("&");

            for (var i = 0; i < params.length; i++) {
                if (params[i].indexOf("ToNode") == -1)
                    continue;

                toNodeID = params[i].split("=")[1];
                break;
            }
            initModal("sendAccepter", toNodeID);
            $('#returnWorkModal').modal().show();
            return;
        }
        

        var url = data;
        url = url.replace('url@', '');

        window.location.href = url;
        return;
    }
    OptSuc(data);
}

//发送 退回 移交等执行成功后转到  指定页面
var interval;
function OptSuc(msg) {
				if (window.parent != null && window.parent.ReloadGxjtList != null) {
					window.parent.ReloadGxjtList(msg);
				} else {
					ReloadGxjtList(msg);
				}

    if ($('#returnWorkModal:hidden').length == 0 && $('#returnWorkModal').length > 0) {
        $('#returnWorkModal').modal().hide()
    }

    //增加msg的模态窗口
    //初始化退回窗口的SRC.
    var html = '<div class="modal fade" id="msgModal" data-backdrop="static">'
        + '<div class="modal-dialog">'
        + '<div class="modal-content" style="border-radius: 0px;">'
        + '<div class="modal-header" style="background:#1d7dd4;">'
        + '<button type="button" class="close" id="btnMsgModalOK1" aria-hidden="true" style="color: #0000007a;display: none;">&times;</button>'
        + '<h4 class="modal-title" style="color:white;">提示信息</h4>'
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

    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')

    $("#msgModalContent").html(msg.replace(/@/g, '<br/>').replace(/null/g, ''));
    var trackA = $('#msgModalContent a:contains("工作轨迹")');
    var trackImg = $('#msgModalContent img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();

    $('#btnMsgModalOK').bind('click', function () {
        closeWindow();
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

var num = 30;
function clock() {
    num >= 0 ? num-- : clearInterval(interval);
    $("#btnMsgModalOK").html("确定(" + num + "秒)");
    if (num == 0)
        closeWindow();
}

/**
 * 关闭弹出消息页面同时关闭父页面
 */
function closeWindow() {
    try {
        if (window.opener) {

            if (window.opener.name && window.opener.name == "main") {
                window.opener.location.href = window.opener.location.href;
                if (window.opener.top && window.opener.top.leftFrame) {
                    window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
                }
            } else if (window.opener.name && window.opener.name == "运行流程") {
                //测试运行流程，不进行刷新
            } else {
                //window.opener.location.href = window.opener.location.href;
            }
        }
    } catch (err) {
        if (err.code == 18){
             //可能出现跨域的问题
        }
       

    }
    

    //提示消息有错误，页面不跳转
    var msg = $("#msgModalContent").html();
    if (msg.indexOf("err@") == -1)
        window.close();
    else {
        setToobarEnable();
        $("#msgModal").modal("hidden");
    }
    // 取得父页面URL，用于判断是否是来自测试流程
    var pareUrl = window.top.document.referrer;
    if (pareUrl.indexOf("test") != -1 || pareUrl.indexOf("Test") != -1) {
        // 测试流程时，发送成功刷新测试容器页面右侧
        window.parent.parent.refreshRight();
    }
    if (window.parent != null && window.parent != undefined
        && pareUrl.indexOf("test") == -1 && pareUrl.indexOf("Test") == -1) {
        window.parent.close();
    }
}

/**
 * SDK表单的发送前的验证
 */
function SDKSend() {
    if (Save() == false) {
        alert("信息保存失败");
        return false;
    }
    //审核信息的保存
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false)
            return false;
    }
    return true;
}
/**
 * 节点表单发送前的验证
 */
function NodeFormSend() {
    //保存前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false)
            return false;

    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false)
            return false;
    }

    //附件检查
    var msg = checkAths();
    if (msg != "") {
        alert(msg);
        return false;
    }



    //检查最小最大长度.
    var f = CheckMinMaxLength();
    if (f == false)
        return false;

    //必填项和正则表达式检查.
    if (checkBlanks() == false) {
        alert("检查必填项出现错误，边框变红颜色的是否填写完整？");
        return false;
    }

    if (checkReg() == false) {
        alert("发送错误:请检查字段边框变红颜色的是否填写完整？");
        return false;
    }

    //如果启用了流程流转自定义，必须设置选择的游离态节点
    if ($('[name=TransferCustom]').length > 0) {
        var ens = new Entities("BP.WF.TransferCustoms");
        ens.Retrieve("WorkID", pageData.WorkID, "IsEnable", 1);
        if (ens.length == 0) {
            alert("该节点启用了流程流转自定义，但是没有设置流程流转的方向，请点击流转自定义按钮进行设置");
            return false;
        }
    }

    return true;
}

/**
 * 绑定多表单发送前的验证
 */
function FromTreeSend() {
    //保存前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false)
            return false;
    OnTabChange("btnsave");
    var p = $(document.getElementById("tabs")).find("li");

    //查看附件上传的最新数量
    var isSend = true;
    var msg = "";
    $.each(p, function (i, val) {
        selectSpan = $(val).find("span")[0];
        var currTab = $("#tabs").tabs("getTab", i);
        tabText = $(selectSpan).text();
        var lastChar = tabText.substring(tabText.length - 1, tabText.length);
        if (lastChar == "*")
            tabText = tabText.substring(0, tabText.length - 1);
        var currScope = currTab.find('iframe')[0];

        var contentWidow = currScope.contentWindow;
        // 不支持火狐浏览器。
        var frms = contentWidow.document.getElementsByName("Attach");
        for (var i = 0; i < frms.length; i++) {
            msg = frms[i].contentWindow.CheckAthNum();
            if (msg != "") {
                msg += "[" + tabText + "]表单" + msg + ";";
                isSend = false;
            }
        }
    });
    if (isSend == false) {
        alert(msg);
        return;
    }
    return true;
}
/**
 * 嵌入式表单
 */
function SendSelfFrom() {
    var val = SaveSelfFrom();
    if (val == false) {
        return false;
    }

    if (val != true) {
        //就说明是传来的参数，这些参数需要存储到WF_GenerWorkFlow里面去，用于方向条件的判断。
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("WorkID", GetQueryString("WorkID"));
        handler.AddPara("Paras", val);
        handler.DoMethodReturnString("SaveParas");
    }
    return true;
}

/**
 * 暂时不起作用
 */
function SysCheckFrm() {
}

/**
 * 保存后的事件
 * @param {any} formType 表单方案类型
 */
function SaveEnd(formType) {
    //SDK表单，保存表单中的信息
    if (formType == 3) {
       
    }
}

//关注 按钮.
function FocusBtn(btn, workid) {

    if (btn.innerText.trim() == "关注") {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='15px' height='15px'>&nbsp;取消关注";
    }
    else {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='15px' height='15px'>&nbsp;关注";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    handler.DoMethodReturnString("Focus"); //执行保存方法.
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.innerText.trim() == '确认') {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='15px' height='15px'>&nbsp;取消确认";
    }
    else {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='15px' height='15px'>&nbsp;确认";
    }

    //btn.value = (btn.value == '确认' ? '取消确认' : '确认');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    handler.DoMethodReturnString("Confirm"); //执行保存方法.

}
//结束流程.
function DoStop(msg, flowNo, workid) {

    if (confirm('您确定要执行 [' + msg + '] ?') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Flow", flowNo);
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("MyFlow_StopFlow");
    alert(msg);

    if (msg.indexOf('err@') == 0)
        return;

    if (window.parent != null) {

    }
    window.close();

}
//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    window.location.href = window.history.url;
}

//结束子流程
function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("WorkID", workid);

    var data = handler.DoMethodReturnString("DelSubFlow"); //删除子流程..
    alert(data);
    window.location.href = window.location.href;

}

/**打印开发者表单 */
function PrintPDF(packUpType) {
    var W = document.body.clientWidth;
    var H = document.body.clientHeight - 40;
    $("#Btn_PrintPdf").val("PDF打印中...");
    $("#Btn_PrintPdf").attr("disabled", true);
    var _html = document.getElementById("divCurrentForm").innerHTML;
    _html = _html.replace("height: " + $("#topContentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#contentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#divCCForm").height() + "px", "");
    //把附件、从表替换
    var dtls = $("[name=Dtl]");
    $.each(dtls, function (i, dtl) {
        _html = _html.replace(dtl.innerHTML, "@Dtl_" + dtl.id);
    });
    var aths = $("[name=Ath]");
    $.each(aths, function (i, ath) {
        _html = _html.replace(ath.innerHTML, "@Ath_" + ath.id);
    });
    

    _html = _html.replace(/仿宋, SimSun/g, 'SimSun');
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("html", _html);
    handler.AddPara("FrmID", flowData.Sys_MapData[0].No);
    handler.AddPara("WorkID", GetQueryString("WorkID"));

    var data = handler.DoMethodReturnString("Packup_Init");
    if (data.indexOf("err@") != -1) {
        alert(data);
    } else {
        $("#Btn_PrintPdf").val("PDF打印成功");
        $("#Btn_PrintPdf").attr("disabled", false);
        $("#Btn_PrintPdf").val("打印pdf");
        var urls = JSON.parse(data);
        for (var i = 0; i < urls.length; i++) {
            if (urls[i].No == packUpType) {
                window.open(urls[i].Name.replace("../../DataUser/", "../DataUser/"));
                break;
            }

            if (urls[i].No == packUpType) {
                window.open(urls[i].Name.replace("../../DataUser/", "../DataUser/"));
                break;
            }

        }
    }



}

//删除流程
function DeleteFlow() {

    if (window.confirm('您确定要删除吗？') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    handler.AddPara("FK_Node", GetQueryString("FK_Node"));
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    var str = handler.DoMethodReturnString("DeleteFlow");

    alert(str);

    self.opener.location.reload();
    window.close();
}

/**
 * 取回审批
 * @param {any} fk_node
 * @param {any} toNode
 */
function Takeback(fk_node, toNode) {
    if (confirm('您确定要执行吗？') == false)
        return;
    var url = '../../GetTask.aspx?DoType=Tackback&FK_Flow=' + GetQueryString("FK_Flow") + '&FK_Node=' + fk_node + '&ToNode=' + toNode + '&WorkID=' + GetQueryString("WorkID");
    window.location.href = url;
}
/**
 * 回滚
 */
function Rollback() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    handler.AddPara("FK_Node", GetQueryString("FK_Node"));
    var data = handler.DoMethodReturnString("OP_ComeBack");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }
    window.close();
}

