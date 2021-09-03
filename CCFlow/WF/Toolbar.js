
var wf_node = null;
var webUser = new WebUser();
$(function () {

    var barHtml = "";
    var data;
    //广西计算中心特意加的一个url参数，用来控制toolbar隐藏
    if (GetQueryString("hideAllBtn") === "1") return;

    var handler = "";
    //MyCC
    if ($("#JS_CC").length == 1) {
        handler = "BP.WF.HttpHandler.WF_MyCC";
    } else if ($("#JS_MyView").length == 1 || $("#JS_MyFrm").length == 1) {
        handler = "BP.WF.HttpHandler.WF_MyView";
    } else {
        handler = "BP.WF.HttpHandler.WF_MyFlow";
    }

    var handler = new HttpHandler(handler);
    handler.AddUrlData();
    if ($("#JS_MyFrm").length == 1)
        data = handler.DoMethodReturnString("MyFrm_InitToolBar");
    else
        data = handler.DoMethodReturnString("InitToolBar");

    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }
    data = JSON.parse(data);

    //当前节点的信息
    if (data.WF_Node != undefined)
        wf_node = data.WF_Node[0];

    var toolBars = data.ToolBar;
    if (toolBars == undefined)
        toolBars = data;
    var _html = "";
    var sendBtnOper = "";
    $.each(toolBars, function (i, toolBar) {
        var Oper = "";
    
        if (toolBar.Oper != ""){
            if (wf_node&&(wf_node.FormType == 3 || wf_node.FormType == 2) && toolBar.No=="Send")
				Oper = "onclick=\"" + toolBar.Oper.replace("SaveDtlAll();","") + "\""; 
			else
                Oper = "onclick=\"" + toolBar.Oper + "\"";
            if (toolBar.No == "Send")
                sendBtnOper = Oper;
		}
   
        if (toolBar.No == "Save") {
            _html += "<button type='button' class='layui-btn layui-btn-primary' lay-submit lay-filter='Save' name='" + toolBar.No + "Btn' enable=true " + Oper + "><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='22px' height='22px'>&nbsp;" + toolBar.Name + "</button>";
            return true;
        }
       

        if (toolBar.No == "NodeToolBar") { //自定义工具栏按钮

            var Icon = toolBar.Icon;
            //自定义的默认按钮
            var img = "<img src='" + basePath + "/WF/Img/Btn/CH.png' width='22px' height='22px'>&nbsp;"
            //有上传的icon,否则用默认的
            if (Icon != "" && Icon != undefined) {
                var index = Icon.indexOf("\DataUser");
                if (index != -1)
                    Icon = Icon.replace(Icon.substr(0, index), "../");
                img = "<img src='" + Icon + "' width='22px' height='22px'>&nbsp;";
            }

            _html += "<button type='button' class='layui-btn layui-btn-primary' name='" + toolBar.No + "' enable=true " + Oper + ">" + img + toolBar.Name + "</button>";

        }
        else {
            if (toolBar.No == "Send")
                _html += "<button type='button' class='layui-btn layui-btn-primary' name='" + toolBar.No + "Btn' enable=true " + Oper + " lay-submit lay-filter='Send'><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='22px' height='22px'>&nbsp;" + toolBar.Name + "</button>";
            else
                _html += "<button type='button' class='layui-btn layui-btn-primary' name='" + toolBar.No + "' enable=true " + Oper + "><img src='" + basePath + "/WF/Img/Btn/" + toolBar.No + ".png' width='22px' height='22px'>&nbsp;" + toolBar.Name + "</button>";
        }
    });
    $('#ToolBar').html(_html);
	$('#Toolbar').html(_html);

    //按钮旁的下来框
    if (wf_node != null && wf_node.IsBackTrack == 0)
        InitToNodeDDL(data, wf_node);


    if ($('[name=Return]').length > 0) {
        $('[name=Return]').bind('click', function () {
            //增加退回前的事件
            if (typeof beforeReturn != 'undefined' && beforeReturn instanceof Function)
                if (beforeReturn() == false)
                    return false;

            if (typeof Save != 'undefined' && Save instanceof Function)
                Save(0);
            initModal("returnBack");
        });
    }

    //流转自定义
    if ($('[name=TransferCustom]').length > 0) {
        $('[name=TransferCustom]').bind('click', function () {
            initModal("TransferCustom");
        });
    }


    if ($('[name=Thread]').length > 0) {
        $('[name=Thread]').bind('click', function () {
            initModal("Thread");
        });
    }


    if ($('[name=Shift]').length > 0) {
        $('[name=Shift]').bind('click', function () { initModal("shift"); });
    }

    if ($('[name=Btn_WorkCheck]').length > 0) {
        $('[name=Btn_WorkCheck]').bind('click', function () { initModal("shift"); });
    }

    if ($('[name=Askfor]').length > 0) {
        $('[name=Askfor]').bind('click', function () { initModal("askfor"); });
    }

    if ($('[name=Track]').length > 0) {
        $('[name=Track]').bind('click', function () { initModal("Track"); });
    }

    if ($('[name=HuiQian]').length > 0) {
        $('[name=HuiQian]').bind('click', function () { initModal("HuiQian"); });
    }

    if ($('[name=AddLeader]').length > 0) {
        $('[name=AddLeader]').bind('click', function () { initModal("AddLeader");});
    }


    if ($('[name=CC]').length > 0) {
        $('[name=CC]').bind('click', function () { initModal("CC");});
    }

    if ($('[name=PackUp_zip]').length > 0) {
        $('[name=PackUp_zip]').bind('click', function () { initModal("PackUp_zip");});
    }

    if ($('[name=PackUp_html]').length > 0) {
        $('[name=PackUp_html]').bind('click', function () { initModal("PackUp_html");});
    }

    if ($('[name=PackUp_pdf]').length > 0) {
        $('[name=PackUp_pdf]').bind('click', function () { initModal("PackUp_pdf");});
    }

    if ($('[name=SelectAccepter]').length > 0) {
        $('[name=SelectAccepter]').bind('click', function () {
            initModal("accepter");
        });
    }

    if ($('[name=DBTemplate]').length > 0) {
        $('[name=DBTemplate]').bind('click', function () {
            initModal("DBTemplate");
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


    if ($('[name=ParentForm]').length > 0) {
        $('[name=ParentForm]').bind('click', function() {

            var myPWorkID = GetQueryString("PWorkID");
            var myPFlow = GetQueryString("PFlowNo");
            if(myPFlow == null){
            	//取得父流程FK_Flow
            	 var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
            	 myPFlow=gwf.PFlowNo;
            }
            var url = "MyView.htm?WorkID=" + myPWorkID+"&FK_Flow=" + myPFlow;
            window.open(url);
        });
    }

    if ($('[name=CH]').length > 0) {
        $('[name=CH]').bind('click', function () { initModal("CH");});
    }

    if ($('[name=Note]').length > 0) {
        $('[name=Note').bind('click', function () { initModal("Note");});
    }


    //公文正文组件
    if ($('[name=GovDocFile]').length > 0) {
        $('[name=GovDocFile').bind('click', function () { initModal("GovDocFile");});
    }

    //公文
    if ($('[name=DocWord]').length > 0) {
        $('[name=DocWord').bind('click', function () { initModal("DocWord"); });
    }

    if ($('[name=Press]').length > 0) {
       // $('[name=Press]').bind('click', function () { initModal("Press"); $('#returnWorkModal').modal().show(); });
    }

    //回滚 Rollback
    if ($('[name=Rollback]').length > 0) {
        $('[name=Rollback]').bind('click', function () { initModal("Rollback");});
    }

    //跳转 JumpWay
    if ($('[name=JumpWay]').length > 0) {
        $('[name=JumpWay]').bind('click', function () { initModal("JumpWay");});
    }
    //生成二维码
    if ($('[name=QRCode]').length > 0) {
        $('[name=QRCode]').bind('click', function () { initModal("QRCode"); });
    }
    

});
//添加保存动态
function SaveOnly() {

    $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;正在保存...");

    try {
        Save(0);
    } catch (e) {
        alert(e);
        return;
    }
    $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;....");
    setTimeout(function () { $("button[name=Save]").html("<img src='./Img/Btn/Save.png' width='22px' height='22px'>&nbsp;保存"); }, 300);
}
function setModalMax() {
    //设置bootstrap最大化窗口
    var w = ddocument.body.clientWidth - 40;
    $("#returnWorkModal .modal-dialog").css("width", w + "px");
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode, url) {

    if ("undefined" != typeof flowData && flowData != null && flowData != undefined) {
        var node = flowData.WF_Node[0];
        if (node.FormType == 12 || (node.FormType == 11 && flowData.WF_FrmNode[0] != null && flowData.WF_FrmNode[0].FrmType == 8)) {
            if (modalType == "PackUp_pdf" || modalType == "PackUp_html" || modalType == "PackUp_zip") {
                PrintPDF(modalType.replace("PackUp_", ""));
                return;
            }
        }
    }


    $("#returnWorkModal").on('hide.bs.modal', function () {
        setToobarEnable();
    });

    var isFrameCross = window.location.href.indexOf(basePath)==-1 ? 1 : 0;
    var modalIframeSrc = '';
    var width = window.innerWidth / 2;
    var height = 50;
    var title = "";
    if (modalType != undefined) {
       

        switch (modalType) {
            case "returnBack":
                title = "退回";
                width =window.innerWidth/2; 
                var node = new Entity("BP.WF.Template.NodeExt", paramData.FK_Node);

                var info = "";
           
                if (info == "" && node.ReturnField != "") {
                    if ($("#" + node.ReturnField).length == 1)
                        info = $("#" + node.ReturnField).val();
                    if (info == undefined && $("#TB_" + node.ReturnField).length == 1)
                        info = $("#TB_" + node.ReturnField).val();
                    if (info == undefined)
                        info = "";
                }
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/ReturnWork.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=" + info + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Send":
                SetChildPageSize(80, 80);
                break;
            case "TransferCustom":
                title = "流转自定义";
                width = window.innerWidth * 3/5; 
                height = 60;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/TransferCustom.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "accpter":
                title = "工作移交";
                width = window.innerWidth * 4/5;
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Thread":
            case "thread":
                title = "子线程";
                width = window.innerWidth * 4/5;
                height =80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/ThreadDtl.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "shift":
                title = "工作移交";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Shift.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "GovDocFile":
                title = "公文正文";
                width = window.innerWidth * 2/5; 
                height = 40;
                modalIframeSrc = ccbpmPath +"/WF/CCForm/GovDocFile.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "DocWord":
                title = "公文";
                width = window.innerWidth * 2/5; 
                height = 40;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/DocWord.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "askfor":
                title = "加签";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Askfor.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Btn_WorkCheck":
                title = "审核";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/WorkCheck.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;

            case "Track": //轨迹.
                title = "处理记录、轨迹";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "HuiQian":
                width = window.innerWidth * 4/5; 
                height = 80;
                if (toNode != null)
                    title = "先会签，后发送。";
                else
                    title = "会签";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;

                break;
            case "AddLeader":
                title = "加主持人";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/HuiQian.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&HuiQianType=AddLeader&s=" + Math.random() + "&isFrameCross=" + isFrameCross;

                break;
            case "CC":
                title = "抄送";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/CC.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNode + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "PackUp_zip":
            case "PackUp_html":
            case "PackUp_pdf":
                title = "打包下载/打印";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Packup.htm?FileType=" + modalType.replace('PackUp_', '') + "&FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Press":
                //$('#modalHeader').text("催办");
                //modalIframeSrc = ccbpmPath + "/WF/WorkOpt/Press.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "accepter":
                title = "选择下一个节点及下一个节点接受人";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;

            //发送选择接收节点和接收人                
            case "sendAccepter":
                //获取到达节点
                var nodeOne = new Entity("BP.WF.Template.NodeSimple", toNode);
                title = "选择接受人(到达节点:" + nodeOne.Name + ")";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Accepter.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "SelectNodeUrl":
                title ="请选择到达的节点";
                width = window.innerWidth * 1/2; 
                height = 50;
                modalIframeSrc = url;
                break;

            case "BySelfUrl"://接收人规则自定义URL
                width = window.innerWidth * 4/5; 
                height = 80;
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
                title ="选择接受人";
                width = window.innerWidth *4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/AccepterOfOrg.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "AccepterOfDept":
                title ="选择接受人";
                width = window.innerWidth * 4/5; 
                height = 80;
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/AccepterOfDept.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&ToNode=" + toNode + "&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "DBTemplate":
                title ="历史发起记录&模版";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/DBTemplate.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "CH":
                title ="节点时限";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/CH.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Note":
                title ="备注";
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Note.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
            case "PR":
                title ="重要性设置";
                width = window.innerWidth * 1/2; 
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/PRI.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PRIEnable=" + node.PRIEnable + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "Rollback":
                title ="流程回滚";
                width = window.innerWidth/2; 
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/Rollback.htm?WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random();
                break;
            case "JumpWay":
                title ="流程节点跳转";
                width =window.innerWidth/2; 
                modalIframeSrc = ccbpmPath +"/WF/WorkOpt/JumpWay.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&Info=&s=" + Math.random() + "&isFrameCross=" + isFrameCross;
                break;
            case "QRCode":
                title = "二维码扫描";
                width = window.innerWidth / 2;
                modalIframeSrc = ccbpmPath+"/WF/WorkOpt/QRCode/GenerCode.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&FID=" + paramData.FID + "&PWorkID=" + paramData.PWorkID + "&Info=&s=" + Math.random();
                break;
            default:
                break;
        }
    }
    OpenLayuiDialog(modalIframeSrc, title, width, height, "auto");
    return false;
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
function InitToNodeDDL(JSonData, wf_node) {

    if (JSonData.ToNodes == undefined)
        return;

    if (JSonData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=SendBtn]');
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
    var _html = '<button class="layui-btn layui-btn-primary" id="Btn_ToNode">';
    _html += '<span id="DDL_ToNode"></span>';
    _html += '<input type="hidden" id="TB_ToNode"/>';
    _html += '<i class="layui-icon layui-icon-down layui-font-12"></i>';
    _html += '</button>';
    $('[name=SendBtn]').after(_html);
    var data = [];
    var isSelected = false;
    $.each(JSonData.ToNodes, function (i, toNode) {
        var item = {
            id: toNode.No,
            title: toNode.Name,
            DeliveryParas: toNode.DeliveryParas,
            IsSelectEmps: toNode.IsSelectEmps,
            IsSelected: toNode.IsSelected
        }
        data.push(item)
        if (item.IsSelected == "1") {
            isSelected = true;
            $("#DDL_ToNode").html(item.title);
            $("#TB_ToNode").data(item);
        }
            
    })

    if (isSelected == false) {
        $("#DDL_ToNode").html(data[0].title);
        $("#TB_ToNode").data(data[0]);
    }
       
    layui.dropdown.render({
        elem: '#Btn_ToNode',
        data: data,
        click: function (obj) {
            $("#DDL_ToNode").html(obj.title);
            $("#TB_ToNode").data(obj);
        }
    });
    if (wf_node.CondModel == 3) {
        var _html = "";
        $.each(JSonData.ToNodes, function (i, toNode) {
           var obj= $("<Button  class='layui-btn layui-btn-primary' id='" + toNode.No + "' name='ToNodeBtn' enable=true onclick='ChangeToNodeState(" + toNode.No + ")'><img src='" + basePath + "/WF/Img/Btn/Send.png' width='22px' height='22px'>&nbsp;" + toNode.Name + "</button>");
            $('[name=SendBtn]').before(obj);
            obj.data(toNode);
        });
        $("#Btn_ToNode").hide();
        $('[name=SendBtn]').hide();
    }
    
}

function ChangeToNodeState(toNode) {
    $("#TB_ToNode").data($("#" + toNode).data());
    $('[name=SendBtn]').trigger("click");
}
/**
 * 流程发送的方法,这个是通用的方法
 * @param {isHuiQian} isHuiQian 是否是会签模式
 * @param {formType} formType 表单方案模式
 */
var IsRecordUserLog = getConfigByKey("IsRecordUserLog", false);
var isSaveOnly = false;
function Send(isHuiQian, formType) {


    /**发送前处理的信息 Start**/
    //SDK表单
    if (formType == 3) {
        if (SDKSend() == false)
            return false;
    }

    //嵌入式表单
    if (formType == 2) {
        if (SendSelfFrom() == false)
            return false;
    }
    //表单方案：傻瓜表单、自由表单、开发者表单、累加表单、绑定表单库的表单（单表单)
    if (formType == 0 || formType == 1 || formType == 10 || formType == 11 || formType == 12) {
        if (NodeFormSend() == false)
            return false;
    }

    //绑定多表单
    if (formType == 5)
        if (FromTreeSend() == false)
            return false;
   
    if (IsRecordUserLog == true) {
        if(pageData.FK_Node==parseInt(pageData.FK_Flow)+"01")
            UserLogInsert("StartFlow", "发起流程");
        else
            UserLogInsert("TodoList", "处理待办");
    }
        

    /**发送前处理的信息 End**/
    var isShowToNode = true;
    if (wf_node != null && wf_node.CondModel == 1 && wf_node.IsBackTrack == 0) {
        //协作模式
        if (wf_node.TodolistModel == 1) {
            var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));
            var huiqianSta = gwf.GetPara("HuiQianTaskSta");
            var todoEmps = gwf.TodoEmps.split(";");
            if (todoEmps.length > 1)
                isShowToNode = false;
        }
        if (isShowToNode == true) {
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            var url = ccbpmPath + "/WF/WorkOpt/ToNodes.htm?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&PWorkID=" + GetQueryString("PWorkID") + "&IsSend=0" + "&s=" + Math.random();

            initModal("SelectNodeUrl", null, url);
            return false;
        } 
        
    }

    window.hasClickSend = true; //标志用来刷新待办.

    var toNodeID = 0;
    var isReturnNode = 0;


    //含有发送节点 且接收
    if ($('#DDL_ToNode').length > 0 ) {
        var gwf = new Entity("BP.WF.GenerWorkFlow", GetQueryString("WorkID"));

        var isLastHuiQian = true;
        //待办人数
        var todoEmps = gwf.TodoEmps;
        if (todoEmps != null && todoEmps != undefined) {
            var huiqianSta = gwf.GetPara("HuiQianTaskSta") == 1 ? true : false;
            if (wf_node.TodolistModel == 1 && huiqianSta == true && todoEmps.split(";").length > 1)
                isLastHuiQian = false;
        }
  
        var selectToNode = $('#TB_ToNode').data();
        toNodeID = selectToNode.id;
        if (selectToNode.IsSelected == 2)
            isReturnNode = 1;
        if (selectToNode.IsSelectEmps == "1" && isLastHuiQian == true) { //跳到选择接收人窗口
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("sendAccepter", toNodeID);
            }
            return false;
        }
        if (selectToNode.IsSelectEmps == "2") {
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            //Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                var url = selectToNode.DeliveryParas;
                if (url != null && url != undefined && url != "") {
                    url = url + "?FK_Node=" + paramData.FK_Node + "&FID=" + paramData.FID + "&WorkID=" + paramData.WorkID + "&FK_Flow=" + paramData.FK_Flow + "&ToNode=" + toNodeID + "&s=" + Math.random();
                    initModal("BySelfUrl", toNodeID, url);
                    return false;
                }
            }
        }
        if (selectToNode.IsSelectEmps == "3") {
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            //Save(1); //执行保存.
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("sendAccepterOfOrg", toNodeID);
            }
            return false;
        }

        if (selectToNode.IsSelectEmps == "4") {
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
          
            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
            } else {
                initModal("AccepterOfDept", toNodeID);
            }
            return false;
        }
        if (isHuiQian == true) {
            isSaveOnly = true;
            $('[name=SaveBtn]').trigger("click");
            isSaveOnly = false;
            initModal("HuiQian", toNodeID);
            return false;
        }
    }

    //执行发送.
    execSend(toNodeID, formType, isReturnNode);
}

function execSend(toNodeID, formType, isReturnNode) {
    //正在发送弹出层
    var index = layer.msg('正在发送，请稍后..', {
        icon: 16
        , shade: 0.01
    });
    //先设置按钮等不可用.
    setToobarDisiable();

    layui.form.on('submit(Send)', function (data) {
        //提交信息的校验
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        if (formType != 3 && formType != 2) {
            var formData = getFormData(data.field);
            for (var key in formData) {
                handler.AddPara(key, encodeURIComponent(formData[key]));
            }
        }
     
        handler.AddPara("ToNode", toNodeID);
        handler.AddUrlData();
        handler.AddPara("IsReturnNode", isReturnNode);
        var data = handler.DoMethodReturnString("Send"); //执行保存方法.
        layer.close(index);//关闭正在发送
        if (data.indexOf('err@') == 0) { //发送时发生错误
            
            var reg = new RegExp('err@', "g")
            var data = data.replace(reg, '');
            layer.alert(data);
            setToobarEnable();
            return false;
        }

        if (data.indexOf('TurnUrl@') == 0) {  //发送成功时转到指定的URL 
            var url = data;
            url = url.replace('TurnUrl@', '');
            window.location.href = url;
            return false;
        }
        if (data.indexOf('SelectNodeUrl@') == 0) {
            var url = data;
            url = url.replace('SelectNodeUrl@', '');
            initModal("SelectNodeUrl", null, url);
            return false;
        }

        if (data.indexOf('BySelfUrl@') == 0) {  //发送成功时转到自定义的URL 
            var url = data;
            url = url.replace('BySelfUrl@', '');
            initModal("BySelfUrl", null, url);
            return false;
        }


        if (data.indexOf('url@') == 0) {  //发送成功时转到指定的URL 

            if (data.indexOf("AccepterOfOrg") != -1) {
                var params = data.split("&");

                for (var i = 0; i < params.length; i++) {
                    if (params[i].indexOf("ToNode") == -1)
                        continue;

                    toNodeID = params[i].split("=")[1];
                    break;
                }
                initModal("sendAccepterOfOrg", toNodeID);
                return false;
            }

            if (data.indexOf("AccepterOfDept") != -1) {
                var params = data.split("&");

                for (var i = 0; i < params.length; i++) {
                    if (params[i].indexOf("ToNode") == -1)
                        continue;

                    toNodeID = params[i].split("=")[1];
                    break;
                }
                initModal("AccepterOfDept", toNodeID);
                return false;
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
                return false;
            }


            var url = data;
            url = url.replace('url@', '');

            window.location.href = url;
            return false;
        }
        OptSuc(data);
        return false;
    });

   
}

//调用后，就关闭刷新按钮.
function returnWorkWindowClose(data) {

    if (data == "" || data == "取消") {
        layer.close(layer.index);
        setToobarEnable();
        return;
    }
    layer.close(layer.index); 
    if (data == undefined) {
        //应该关闭页面
        if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function")
            window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
        else {
            // 取得父页面URL，用于判断是否是来自测试流程
            var pareUrl = window.top.document.referrer;
            if (pareUrl.indexOf("test") != -1 || pareUrl.indexOf("Test") != -1) {
                // 测试流程时，发送成功刷新测试容器页面右侧
                window.parent.parent.refreshRight();
            }
            window.close();
        }
           
    }
    //通过下发送按钮旁的下拉框选择下一个节点
    if (data != null && data != undefined && data.indexOf('SaveOK@') == 0) {
        //说明保存人员成功,开始调用发送按钮.
        var toNode = 0;
        //含有发送节点 且接收
        if ($('#TB_ToNode').length > 0) {
            var selectToNode = $('#TB_ToNode').data();
            toNode = selectToNode.No;
        }

        execSend(toNode);
        return false;
    } else {//可以重新打开接收人窗口
        winSelectAccepter = null;
    }

    if (data != null && data != undefined && (data.indexOf('err@') == 0 || data == "取消")) {//发送时发生错误
        layer.alert(data);
        return false;
    }

    OptSuc(data);
}


//发送 退回 移交等执行成功后转到  指定页面
var interval;
function OptSuc(msg) {
    if (window.parent != null && window.parent.WindowCloseReloadPage != null && typeof window.parent.WindowCloseReloadPage === "function") {
        window.parent.WindowCloseReloadPage(msg);
    } else {
        if (typeof WindowCloseReloadPage != 'undefined' && WindowCloseReloadPage instanceof Function)
            WindowCloseReloadPage(msg);
    }
    layer.close(layer.index);
    if (msg == null || msg == undefined)
        msg = "";
    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')
    msg = msg.replace(/@/g, '<br/>').replace(/null/g, '');
    if (msg.indexOf("err@") != -1) {
        layer.alert(msg);
        setToobarEnable();
        return false;
    }
    var WF_WorkOpt_LeftSecond = getConfigByKey("WF_WorkOpt_LeftSecond", 30);
    layer.open({
        type: 1
        , id: 'layerDemo'
        , content: msg
        , btn: ["确定(" + WF_WorkOpt_LeftSecond-- + "秒)"]
        , btnAlign: 'r' //按钮居中
        , area: ['40%', '40%']
        , shade: 0

        , yes: function () {
            layer.closeAll();
        }
        , success: function (layero, index) {
            var timer = null;
            var fn = function () {
                layero.find(".layui-layer-btn0").text("确定(" + WF_WorkOpt_LeftSecond + "秒)");
                if (WF_WorkOpt_LeftSecond == 0) {
                    layer.close(index);
                    clearInterval(timer);
                    closeWindow();
                }
                WF_WorkOpt_LeftSecond--;
            };
            timer = setInterval(fn, 1000);
            fn();
        }
        , end: function () {
            clearInterval(interval);
            closeWindow();
        }
    });
    $("#layerDemo").css("padding", "0px 20px");
}
/**
 * 关闭弹出消息页面同时关闭父页面
 */
function closeWindow() {
    if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function")
        window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
    else {
        // 取得父页面URL，用于判断是否是来自测试流程
        var pareUrl = window.top.document.referrer;
        if (pareUrl.indexOf("test") != -1 || pareUrl.indexOf("Test") != -1) {
            // 测试流程时，发送成功刷新测试容器页面右侧
            window.parent.parent.refreshRight();
        }
        window.close();
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
    //保存从表信息
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            IsSaveTrue = contentWidow.SaveAll();
        }
    });

    //发送前事件
    if (typeof beforeSend != 'undefined' && beforeSend instanceof Function)
        if (beforeSend() == false)
            return false;

    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        if ($("#WorkCheck_Doc").val() == "" && $("#Img_WorkCheck").length!=0 && $("#Img_WorkCheck")[0].src =="../DataUser/Siganture/UnSiganture.jpg") {
            alert("请填写审核意见!!!!");
            return false;
        }
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false)
            return false;
    }

    if (checkBlanks() == false) {
        layer.alert("必填项不能为空");
        return false;
    }
    //附件检查
    var msg = checkAths();
    if (msg != "") {
        alert(msg);
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
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;取消关注";
    }
    else {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;关注";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    handler.DoMethodReturnString("Focus"); //执行保存方法.
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.innerText.trim() == '确认') {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;取消确认";
    }
    else {
        btn.innerHTML = "<img src='Img/Btn/Focus.png' width='22px' height='22px'>&nbsp;确认";
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    handler.DoMethodReturnString("Confirm");

}
//结束流程.
function DoStop(msg, flowNo, workid) {
    layui.confirm('您确定要执行 [' + msg + '] ?', function (index) {
        layer.close(index);
        //流程结束前
        if (typeof beforeStopFow != 'undefined' && beforeStopFow instanceof Function)
            if (beforeStopFow() == false)
                return false;
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
        handler.AddPara("FK_Flow", flowNo);
        handler.AddPara("WorkID", workid);
        var data = handler.DoMethodReturnString("MyFlow_StopFlow");
        layer.alert(data);
        if (data.indexOf('err@') == 0)
            return;
        //流程结束后
        if (typeof afterStopFow != 'undefined' && afterStopFow instanceof Function)
            if (afterStopFow() == false)
                return false;
        if (window.top.vm && typeof window.top.vm.closeCurrentTabs == "function")
            window.top.vm.closeCurrentTabs(window.top.vm.selectedTabsIndex);
        else
            window.close();
    })
 
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
        _html = _html.replace(ath.innerHTML, "@Ath_" + ath.id.replace("Div_", ""));
    });


    _html = _html.replace(/仿宋, SimSun/g, 'SimSun');
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("html", _html);
    handler.AddPara("FrmID", flowData.Sys_MapData[0].No);
    handler.AddPara("WorkID", GetQueryString("WorkID"));

    var data = handler.DoMethodReturnString("Packup_Init");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
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


/**------------------------子线程退回分流节点的工作处理器按钮操作-------------------------------**/

/**
 * 驳回子线程
 */
function ReSend() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("ReSend");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    OptSuc(data);
}
/**
 * 取消子线程
 */
function KillThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("KillThread");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    window.close();
}
/**
 * 撤销整体发送
 */
function UnSendAllThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("UnSendAllTread");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    if (data.indexOf("url@") != -1) {
        var url = data.replace("url@", "");
        window.location.href = url;
    }
}

/***
 * 
 * 撤销
 */
function UnSend() {

    if (window.confirm('您确定要撤销本次发送吗？') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyView");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("MyView_UnSend");
    if (data.indexOf('err@') == 0) {
        data = data.replace('err@', '');
        data = data.replace('err@', '');
        alert(data);
        return;
    }

    var url = 'MyFlow.htm?FK_Flow=' + GetQueryString("FK_Flow") + '&WorkID=' + GetQueryString("WorkID") + '&FID=' + GetQueryString("FID");
    window.location.href = url;
    return;
}
/**
 * 发起子流程
 * @param {any} subFlowNo 子流程编号
 */
function SendSubFlow(subFlowNo, subFlowMyPK) {
    var W = document.body.clientWidth - 340;
    var H = document.body.clientHeight - 340;
    var url = "./WorkOpt/SubFlowGuid.htm?SubFlowMyPK=" + subFlowMyPK + "&WorkID=" + GetQueryString("WorkID") + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    OpenBootStrapModal(url, "eudlgframe", "选择", W, H,
        "icon-edit", true, function () {
            var iframe = document.getElementById("eudlgframe");
            if (iframe) {
                var result = iframe.contentWindow.Btn_OK();
                if(result == true){
                    var subFlowGuid = new Entity("BP.WF.Template.SubFlowHandGuide", subFlowMyPK);
                    if (subFlowGuid.SubFlowHidTodolist == 1) {
                        if (window.parent != null && window.parent != undefined)
                            window.parent.close();
                        else
                            window.close();
                        
                    }
                    //显示子流程信息
                    var html = window.parent.SubFlow_Init(wf_node);
                    $("#SubFlow").html("").html(html);
                }
               
            }

        }, null, function () {

        });

}
function StartThread() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("FK_Flow", GetQueryString("FK_Flow"));
    handler.AddPara("FK_Node", GetQueryString("FK_Node"));
    var data = handler.DoMethodReturnString("MyFlow_StartThread");
    if (data.indexOf("err@") != -1) {
        alert(data); 
        console.log(data);
        return;
    }

    if (data == null || data == undefined)
        data = "";
    data = data.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')
    $("#HelpAlter").html(data.replace(/@/g, '<br/>').replace(/null/g, ''));
    var trackA = $('#HelpAlter a:contains("工作轨迹")');
    var trackImg = $('#HelpAlter img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();
    $("#HelpAlter").css("text-align", "left");
    $('#HelpAlterDiv').modal().show();

    $('#BtnOK1').bind('click', function () {
        if ($("#SubFlow").length == 1) {
            var node = new Entity("BP.WF.Node",GetQueryString("FK_Node"));
            $("#SubFlow").html(SubFlow_Init(node));
        }
    });
    $('#BtnOK').bind('click', function () {
        if ($("#SubFlow").length == 1) {
            var node = new Entity("BP.WF.Node",GetQueryString("FK_Node"));
            $("#SubFlow").html(SubFlow_Init(node));
        }
    });

    
}

/**
 * 帮助提醒
 */
function HelpAlter() {
    var node = flowData.WF_Node[0];
    //判断该节点是否启用了帮助提示 0 禁用 1 启用 2 强制提示 3 选择性提示
    var btnLab = new Entity("BP.WF.Template.BtnLab", node.NodeID);
    if (btnLab.HelpRole != 0) {
        var count = 0;
        if (btnLab.HelpRole == 3) {
            var mypk = webUser.No + "_ND" + node.NodeID + "_HelpAlert";
            var userRegedit = new Entity("BP.Sys.UserRegedit");
            userRegedit.SetPKVal(mypk);
            count = userRegedit.RetrieveFromDBSources();
        }

        if (btnLab.HelpRole == 2 || (count == 0 && btnLab.HelpRole == 3)) {
            var filename = basePath + "/DataUser/CCForm/HelpAlert/" + node.NodeID + ".htm";
            var htmlobj = $.ajax({ url: filename, async: false });
            if (htmlobj.status == 404)
                return;
            var str = htmlobj.responseText;
            if (str != null && str != "" && str != undefined) {
                $('#HelpAlter').html("").append(str);
                $('#HelpAlterDiv').modal().show();
            }
        }
    }
}
