$(function () {
    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }

});

var isClearCach = true;

//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误.');
    }
    //执行保存.
    return frm.contentWindow.Save();
}

function SendSelfFrom() {
    if (SaveSelfFrom() == false) {
        alert('表单保存失败，不能发送。');
        return false;
    }
    return true;
}

var winSelectAccepter = null;
// 打开选择人接收器.
function OpenSelectAccepter(flowNo, nodeid, workid, fid) {
    var url = "./WorkOpt/Accepter.htm?WorkID=" + workid + "&FK_Node=" + nodeid + "&FK_Flow=" + flowNo + "&FID=" + fid + "&type=2";
    if (winSelectAccepter == null)
        winSelectAccepter = window.open(url, winSelectAccepter, 'height=600, width=600,scrollbars=yes');
    else
        winSelectAccepter.focus(); // (0, 0);
    return false;
}

function OpenAccepter() {

    var url = '/WF/CCForm/FrmPopVal.aspx?FK_MapExt=' + popNameInXML + '&CtrlVal=' + ctrl.value;
    var v = window.showModalDialog(url, 'opp', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    if (v == null || v == '' || v == 'NaN') {
        return;
    }
    ctrl.value = v;
    return;
}

function SetHegiht() {
    var screenHeight = document.documentElement.clientHeight;

    var messageHeight = $('#Message').height();
    var topBarHeight = 40;
    var childHeight = $('#childThread').height();
    var infoHeight = $('#flowInfo').height();

    var allHeight = messageHeight + topBarHeight + childHeight + childHeight + infoHeight;
    try {

        var BtnWord = $("#BtnWord").val();
        if (BtnWord == 2)
            allHeight = allHeight + 30;

        var frmHeight = $("#FrmHeight").val();
        if (frmHeight == NaN || frmHeight == "" || frmHeight == null)
            frmHeight = 0;

        if (screenHeight > parseFloat(frmHeight) + allHeight) {
            // $("#divCCForm").height(screenHeight - allHeight);

            $("#TDWorkPlace").height(screenHeight - allHeight - 10);

        }
        else {
            //$("#divCCForm").height(parseFloat(frmHeight) + allHeight);
            $("#TDWorkPlace").height(parseFloat(frmHeight) + allHeight - 10);
        }
    }
    catch (e) {
    }
}

$(window).resize(function () {
    //SetHegiht();
});
function SysCheckFrm() {
}
function Change() {
    var btn = document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
}
var longCtlID = 'ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_';
function KindEditerSync() {
    try {
        if (editor1 != null) {
            editor1.sync();
        }
    }
    catch (err) {
    }
}

function Shift() {
    //  var url = '/WF/WorkOpt/Forward.htm';
    //window.open(url);
}

function ReturnWork() {
    //var url = '/WF/WorkOpt/ReturnWork.htm';
    //window.open(url);
}



// ccform 为开发者提供的内置函数. 
// 获取DDL值 
function ReqDDL(ddlID) {
    var v = document.getElementById(longCtlID + 'DDL_' + ddlID).value;
    if (v == null) {
        alert('没有找到ID=' + ddlID + '的下拉框控件.');
    }
    return v;
}

//20160106 by 柳辉
//获取页面参数
//sArgName表示要获取哪个参数的值
function GetPageParas(sArgName) {

    var sHref = window.location.href;
    var args = sHref.split("?");
    var retval = "";
    if (args[0] == sHref) /*参数为空*/ {
        return retval; /*无需做任何处理*/
    }
    var str = args[1];
    args = str.split("&");
    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }
    return retval;
}

//获取Dtl中TB的值 20160106 from 柳辉
function ReqDtlBObj(dtlTable, DtlColumn, onValue) {

    var getworkid = $('#HidWorkID').val();//hiddenValue

    $.ajax({

        url: "../../DataUser/Do.aspx",
        data: { getworkid: getworkid, dtlTable: dtlTable, DtlColumn: DtlColumn, onValue: onValue },
        success: function (arr) {
            alert(arr);
            if (arr == "true") {
                return true;
            }
            else {
                return false;
            }
        }

    });

}
// 获取TB值
function ReqTB(tbID) {
    var v = document.getElementById(longCtlID + 'TB_' + tbID).value;
    if (v == null) {
        alert('没有找到ID=' + tbID + '的文本框控件.');
    }
    return v;
}
// 获取CheckBox值
function ReqCB(cbID) {
    var v = document.getElementById(longCtlID + 'CB_' + cbID).value;
    if (v == null) {
        alert('没有找到ID=' + cbID + '的 CheckBox （单选）控件.');
    }
    return v;
}
// 获取附件文件名称,如果附件没有上传就返回null.
function ReqAthFileName(athID) {
    var v = document.getElementById(athID);
    if (v == null) {
        return null;
    }
    var fileName = v.alt;
    return fileName;
}

/// 获取DDL Obj
function ReqDDLObj(ddlID) {
    var v = document.getElementById(longCtlID + 'DDL_' + ddlID);
    if (v == null) {
        alert('没有找到ID=' + ddlID + '的下拉框控件.');
    }
    return v;
}
// 获取TB Obj
function ReqTBObj(tbID) {
    var v = document.getElementById(longCtlID + 'TB_' + tbID);
    if (v == null) {
        alert('没有找到ID=' + tbID + '的文本框控件.');
    }
    return v;
}
// 获取CheckBox Obj值
function ReqCBObj(cbID) {
    var v = document.getElementById(longCtlID + 'CB_' + cbID);
    if (v == null) {
        alert('没有找到ID=' + cbID + '的单选控件(获取CheckBox)对象.');
    }
    return v;
}
// 设置值.
function SetCtrlVal(ctrlID, val) {
    document.getElementById(longCtlID + 'TB_' + ctrlID).value = val;
    document.getElementById(longCtlID + 'DDL_' + ctrlID).value = val;
    document.getElementById(longCtlID + 'CB_' + ctrlID).value = val;
}
//执行分支流程退回到分合流节点。
function DoSubFlowReturn(fid, workid, fk_node) {
    var url = 'ReturnWorkSubFlowToFHL.aspx?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
    var v = WinShowModalDialog(url, 'df');
    window.location.href = window.history.url;
}
function To(url) {
    //window.location.href = url;
    window.name = "dialogPage"; window.open(url, "dialogPage")
}

//退回，获取配置的退回信息的字段.
function ReturnWork(url, field) {
    var urlTemp;
    if (field == '' || field == null) {
        urlTemp = url;
    }
    else {
        // alert(field);
        //  alert(ReqTB(field));
        urlTemp = url + '&Info=' + ReqTB(field);
    }
    window.name = "dialogPage"; window.open(urlTemp, "dialogPage")
}

function WinOpen(url, winName) {
    var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
    newWindow.focus();
    return;
}


function DoDelSubFlow(fk_flow, workid) {
    if (window.confirm('您确定要终止进程吗？') == false)
        return;
    var url = 'Do.aspx?DoType1=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
    WinShowModalDialog(url, '');
    window.location.href = window.location.href; //aspxPage + '.aspx?WorkID=';
}
function Do(warning, url) {
    if (window.confirm(warning) == false)
        return;
    window.location.href = url;
}
//设置底部工具栏
function SetBottomTooBar() {
    var form;
    //窗口的可视高度 
    var windowHeight = document.all ? document.getElementsByTagName("html")[0].offsetHeight : window.innerHeight;
    var pageHeight = Math.max(windowHeight, document.getElementsByTagName("body")[0].scrollHeight);
    form = document.getElementById('divCCForm');

    //        if (form) {
    //            if (pageHeight > 20) pageHeight = pageHeight - 20;
    //            form.style.height = pageHeight + "px";
    //        }
    //设置toolbar
    var toolBar = document.getElementById("bottomToolBar");
    if (toolBar) {
        document.getElementById("bottomToolBar").style.display = "";
    }
}

window.onload = function () {
    //  ResizeWindow();
    SetBottomTooBar();
};

//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
}
function OpenCC() {
    var url = $("#CC_Url").val();
    var v = window.showModalDialog(url, 'cc', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == '1')
        return true;
    return false;
}

var LODOP; //声明为全局变量 

function printFrom() {
    var url = $("#PrintFrom_Url").val();
    LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
    LODOP.PRINT_INIT("打印表单");

    // LODOP.ADD_PRINT_URL(30, 20, 746, "100%", location.href);

    //LODOP.ADD_PRINT_HTM(20, 0, "100%", "100%", document.getElementById("divCCForm").innerHTML);
    LODOP.ADD_PRINT_URL(0, 0, "100%", "100%", url);

    LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
    LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
    //LODOP.SET_PRINT_PAGESIZE(0, 0, 0, "");
    LODOP.SET_PRINT_PAGESIZE(2, 2400, 2970, "A4");
    //		LODOP.SET_SHOW_MODE("MESSAGE_GETING_URL",""); //该语句隐藏进度条或修改提示信息
    //		LODOP.SET_SHOW_MODE("MESSAGE_PARSING_URL","");//该语句隐藏进度条或修改提示信息
    //  LODOP.PREVIEW();

    LODOP.PREVIEW();
}


//原有的
function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
    var date = new Date();
    var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();

    var url = 'WebOffice/AttachOffice.aspx?DoType1=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    //var url = 'WebOffice.aspx?DoType1=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal;
    // var str = window.showModalDialog(url, '', 'dialogHeight: 1250px; dialogWidth:900px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
    //var str = window.open(url, '', 'dialogHeight: 1200px; dialogWidth:1110px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

//按钮.
function FocusBtn(btn, workid) {
    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }
    $.ajax({ url: "Do.aspx?ActionType=Focus&WorkID=" + workid, async: false });
}

function ReturnVal(ctrl, url, winName) {
    if (url == "")
        return;
    //update by dgq 2013-4-12 判断有没有？
    if (ctrl && ctrl.value != "") {
        if (url.indexOf('?') > 0)
            url = url + '&CtrlVal=' + ctrl.value;
        else
            url = url + '?CtrlVal=' + ctrl.value;
    }
    //修改标题控制不进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitleRemove();
        }
    }

    //杨玉慧 用#控件类型#id  来作为变量绑定  如 Method=#DRDL_DealWithMethod

    while (url.indexOf('#') > 0) {
        var startIndex = url.indexOf('#');
        //获取#号后面的& 符号的位置
        var endIndex = url.indexOf('&', startIndex);
        if (endIndex < 0) {
            SetBottomTooBar
            endIndex = url.length;
        }
        var paramId = url.substring(startIndex + 1, endIndex);
        var value = $("[id$=_'" + paramId + "']").val();
        url = url.replace('#' + paramId, value);
    }


    //杨玉慧 模态框 先用这个
    $('#returnPopValModal .modal-header h4').text("请选择：" + $(ctrl).parent().prev().text());

    $('#iframePopModalForm').attr("src", url);
    $('#btnPopValOK').unbind('click');
    $('#btnPopValOK').bind('click', function () {
        var retrunVal = frames["iframePopModalForm"].window.returnValue;
        if (retrunVal == undefined)
            retrunVal = "";
        var txtId = ctrl.id;
        ctrl.value = retrunVal;
        if ($('#' + txtId + "_ReValue").length > 0) {
            $('#' + txtId + "_ReValue").val(retrunVal);
        }
    });
    $('#returnPopValModal').modal().show();
    //修改标题，失去焦点时进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitle();
        }
    }
    return;
}


//然浏览器最大化.
function ResizeWindow() {
    //if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen
    //    var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽
    //    var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高
    //    window.moveTo(0, 0);           //把window放在左上角
    //    window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh
    //}
}
window.onload = ResizeWindow;

//以下是软通写的
//初始化网页URL参数


function initPageParam() {
    //新建独有
    pageData.UserNo = GetQueryString("UserNo");
    pageData.DoWhat = GetQueryString("DoWhat");
    pageData.IsMobile = GetQueryString("IsMobile");

    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    //FK_Flow=004&FK_Node=402&FID=0&WorkID=232&IsRead=0&T=20160920223812&Paras=
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadOnly = GetQueryString("IsReadOnly");//如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow");//是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    //$('#navIframe').attr('src', 'Admin/CCBPMDesigner/truck/centerTrakNav.html?FK_Flow=' + pageData.FK_Flow + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID);
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
}
//初始化按钮
function initBar() {
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        url: "MyFlow.ashx?DoType=InitToolBar&m=" + Math.random(),
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
            if ($('[name=Askfor]').length > 0) {
                $('[name=Askfor]').attr('onclick', '');
                $('[name=Askfor]').unbind('click');
                $('[name=Askfor]').bind('click', function () { initModal("askfor"); $('#returnWorkModal').modal().show(); });
            }
            if ($('[name=SelectAccepter]').length > 0) {
                $('[name=SelectAccepter]').attr('onclick', '');
                $('[name=SelectAccepter]').unbind('click');
                $('[name=SelectAccepter]').bind('click', function () { initModal("accepter"); $('#returnWorkModal').modal().show(); });
            }
            if ($('[name=Delete]').length > 0) {
                $('[name=Delete]').attr('onclick', '');
                $('[name=Delete]').unbind('click');
                $('[name=Delete]').bind('click', function () { initModal("Delete"); $('#returnWorkModal').modal().show(); });

                //var onclickFun = $('[name=Delete]').attr('onclick');
                //if (onclickFun != undefined) {
                //    $('[name=Delete]').attr('onclick', onclickFun.replace('MyFlowInfo.htm', 'MyFlowInfo.aspx'));
                //}
            }
        }
    });
}

//初始化退回、移交、加签窗口
function initModal(modalType, toNode) {
    //初始化退回窗口的SRC
    var returnWorkModalHtml = '<div class="modal fade" id="returnWorkModal" data-backdrop="static">' +
       '<div class="modal-dialog" style="width:700px;margin:30px auto;">'
           + '<div class="modal-content" style="border-radius:0px;width:700px;text-align:left;">'
              + '<div class="modal-header">'
                  + '<button type="button" style="color:white;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
                   + '<h4 class="modal-title" id="modalHeader">工作退回</h4>'
               + '</div>'
               + '<div class="modal-body">'
                   + '<iframe style="width:100%;border:0px;height:250px;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
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
                var returnReasonsItems = JSON.parse(jsonStr).WF_Node[0].ReturnReasonsItems;
                var returnInfo = "";
                //只支持文本
                if (returnReasonsItems != undefined && returnReasonsItems != "") {
                    returnInfo = returnReasonsItems;
                    var txbs = $('[id^=TB_]');
                    $.each(txbs, function (i, obj) {
                        var tbId = $(obj).attr('id');
                        var tbKeyOfEnParam = '@' + tbId.substring(3) + ';';
                        var tbVal = $(obj).val();
                        while (returnInfo.indexOf(tbKeyOfEnParam) >= 0) {
                            //returnInfo = returnInfo.replace('/' + tbKeyOfEnParam + '/g', tbVal);
                            returnInfo = returnInfo.replace(tbKeyOfEnParam, tbVal);
                        }
                    });

                    returnInfo = escape(returnInfo);
                }
                modalIframeSrc = "../WF/WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random() + "&Info=" + returnInfo;
                break;
            case "shift":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "../WF/WorkOpt/forward.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "askfor":
                $('#modalHeader').text("工作加签");
                modalIframeSrc = "../WF/WorkOpt/Askfor.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "accepter":
                $('#modalHeader').text("选择下一个节点及下一个节点接受人");
                modalIframeSrc = "../WF/WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
                //发送选择接收节点和接收人
            case "sendAccepter":
                $('#modalHeader').text("发送到节点：" + toNode.Name);
                modalIframeSrc = "../WF/WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode.No + "&s=" + Math.random()
                break;
            case "Delete"://删除
                $('#modalHeader').text("工作" + $('[name = Delete]').val());
                modalIframeSrc = "../WF/WorkOpt/DelFlow.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow +"&m="+ Math.random()+"&Info";
                break;
            default:
                break;
        }
    }
    $('#iframeReturnWorkForm').attr('src', modalIframeSrc);
}

//退回操作  显示退回窗口
function showReturnWorkModel() {
    $('#returnWorkModal').modal().show();
}
//设置附件为只读
function setAttachDisabled() {
    //附件设置
    var attachs = $('iframe[src*="AttachmentUpload.aspx"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadOnly') == -1) {
            $(attach).attr('src', $(attach).attr('src') + "&IsReadOnly=1");
        }
    })
}
//设置附件为不只读
function setAttachAbled() {
    //附件设置
    var attachs = $('iframe[src*="AttachmentUpload.aspx"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadOnly') >=0) {
            $(attach).attr('src', $(attach).attr('src').replace('IsReadOnly=1',''));
        }
    })
}
//隐藏下方的功能按钮
function setToobarUnVisible() {
    //隐藏下方的功能按钮
    $('#bottomToolBar').css('display', 'none');
}

//隐藏下方的功能按钮
function setToobarDisiable() {
    //隐藏下方的功能按钮
    $('#bottomToolBar input').css('background', 'gray');
    $('#bottomToolBar input').attr('disabled', 'disabled');
}

function setToobarEnable() {
    //隐藏下方的功能按钮
    $('#bottomToolBar input').css('background', '#2884fa');
    $('#bottomToolBar input').removeAttr('disabled');
}
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');

    ////设置POP 窗不能弹出
    //$('#divCCForm .input-group-addon').attr('onclick', '');
}


//保存
function Save() {
    //必填项和正则表达式检查
    var formCheckResult = true;
    if (!checkBlanks()) {
        formCheckResult = false;
    }
    if (!checkReg()) {
        formCheckResult = false;
    }
    if (!formCheckResult) {
        //alert("请检查表单必填项和正则表达式");
        return;
    }
    setToobarDisiable();

    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(true, true),
        url: "MyFlow.ashx?Method=Save",
        dataType: 'html',
        success: function (data) {

            setToobarEnable();
            if (data.indexOf('err@') == 0) {
                $('#Message').html(data.substring(4, data.length));
                $('.Message').show();
            }
            else {
                //OptSuc(data);
                $('#Message').html(data);
                $('.Message').show();
                //表示退回OK
                //if (data.indexOf('工作已经被您退回到') == 0) {
                //  OptSuc(data);

                //setAttachDisabled();
                //setToobarUnVisible();
                //setFormEleDisabled();
                //}
            }
        }
    });
}

//退回工作
function returnWorkWindowClose(data) {
    $('#returnWorkModal').modal('hide');

    if (data.indexOf('err@') == 0 || data == "取消") {//发送时发生错误
        $('#Message').html(data);
        $('.Message').show();
    }
    else {
        OptSuc(data);
        ////发送成功时
        //setAttachDisabled();
        //setToobarUnVisible();
        //setFormEleDisabled();
    }
}
//移交
//子线程
//子流程
//{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}
function getData(data, url, dataParam) {
    var jsonStr = '{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}';
    var data = JSON.parse(jsonStr);
    if (data.IsSuccess != true) {
        alert('返回参数失败，ErrMsg:' + data.ErrMsg + ";Msg:" + data.Msg + ";url:" + url);

    }
    return data;
}

//FK_Flow=005&UserNo=zhwj&DoWhat=StartClassic&=&IsMobile=&FK_Node=501
var pageData = {};
var globalVarList = {};
//解析分组类型 如果返回的为 '' 就表明是字段分组
function initGroup(workNodeData, groupFiled) {
    var groupHtml = '';
    /*根据控件类型解析分组*/
    switch (groupFiled.CtrlType) {
        case "Frame": // 框架 类型.
            for (var frameIndex in workNodeData.Sys_MapFrame) {
                var fram = workNodeData.Sys_MapFrame[frameIndex];
                if (fram.MyPK != groupFiled.CtrlID)
                    continue;
                //将 中文的  冒号转成英文的冒号
                var src = fram.URL.replace(new RegExp(/(：)/g), ':');
                var params = '&FID=' + pageData.FID;
                params += '&WorkID=' + groupFiled.OID;


                if (src.indexOf("?") > 0) {
                    var params = getQueryStringFromUrl(src);
                    if (params != null && params.length > 0) {
                        $.each(params, function (i, param) {
                            if (param.indexOf('@') == 0) {//是需要替换的参数
                                paramArr = param.split('=');
                                if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                                    if (paramArr[1].indexOf('@WebUser.') == 0) {
                                        params[i] = paramArr[0].substring(1) + "=" + workNodeData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                                    }
                                    if (workNodeData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                                        params[i] = paramArr[0].substring(1) + "=" + workNodeData.MainTable[0][paramArr[1].substr(1)];
                                    }


                                    //使用URL中的参数
                                    var pageParams = getQueryString();
                                    var pageParamObj = {};
                                    $.each(pageParams, function (i, pageParam) {
                                        if (pageParam.indexOf('@') == 0) {
                                            var pageParamArr = pageParam.split('=');
                                            pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
                                        }
                                    });
                                    var result = "";
                                    //通过MAINTABLE返回的参数
                                    for (var ele in workNodeData.MainTable[0]) {
                                        if (paramArr[0].substring(1) == ele) {
                                            result = workNodeData.MainTable[0][ele];
                                            break;
                                        }
                                    }
                                    //通过URL参数传过来的参数
                                    for (var pageParam in pageParamObj) {
                                        if (pageParam == paramArr[0].substring(1)) {
                                            result = pageParamObj[pageParam];
                                            break;
                                        }
                                    }

                                    if (result != '') {
                                        params[i] = paramArr[0].substring(1) + "=" + unescape(result);
                                    }
                                }
                            }
                        });
                        src = src.substr(0, src.indexOf('?')) + "?" + params.join('&');
                    }
                    src += "&IsReadOnly=0";
                }
                else {
                    src += "?IsReadOnly=0";
                }
                src+="&____jsVersion="+load.Version;
                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:block;"  id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>" + '</div>';
            }
            break;
        case "Dtl":
            //WF/CCForm/Dtl.aspx?EnsName=ND501Dtl1&RefPKVal=0&PageIdx=1

            var href = window.location.href;
            var urlParam = href.substring(href.indexOf('?') + 1, href.length);
            urlParam = '&' + urlParam;
            urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
            var src = '';
            if (frmDtl.DtlShowModel == "0") {
                if (pageData.IsReadOnly) {

                    src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=1" + urlParam + "&Version=" + load.Version;
                } else {
                    src = "./CCForm/Dtl.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.WorkID + "&IsReadonly=0" + urlParam + "&Version=" + load.Version;
                }
            }
            //var src = "/WF/CCForm/Dtl.aspx?s=2&EnsName=" + groupFiled.CtrlID + "&RefPKVal=" + pageData.WorkID + "&PageIdx=1";
            //src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Ath": //增加附件.
            break;
            for (var athIndex in workNodeData.Sys_FrmAttachment) {
                var ath = workNodeData.Sys_FrmAttachment[athIndex];
                if (ath.MyPK != groupFiled.CtrlID)
                    continue;
                var src = "";
                if (pageData.IsReadonly)
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                else
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK;

                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            }
            break;
        case "FWC": //审核组件.
            var src = "/WF/WorkOpt/WorkCheck.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }


            if (workNodeData.WF_Node.length > 0 && workNodeData.WF_Node[0].FWCSTA == 1) {
                paras += "&DoType1=View";
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "SubFlow": //子流程..
            var src = "/WF/WorkOpt/SubFlow.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            if (workNodeData.WF_Node.length > 0 && workNodeData.WF_Node[0].FWCSTA == 1) {
                paras += "&DoType1=View";
            }
            src += "&r=q" + paras;
            src += "&IsShowTitle=0";
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Track": //轨迹图.
            var src = "/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
            //var paras = pageParamToUrl();
            //if (paras.indexOf('OID') < 0) {
            //    paras += "&OID=" + pageData.WorkID;
            //}
            src += '&FK_Flow=' + pageData.FK_Flow;
            src += '&FK_Node=' + pageData.FK_Node;
            src += '&WorkID=' + pageData.WorkID;
            src += '&FID=' + pageData.FID;
            //先临时写成这样的
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:500px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

            break;
        case "Thread": //子线程.
            var src = "/WF/WorkOpt/Thread.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "FTC": //流转自定义.  有问题
            var src = "/WF/WorkOpt/FTC.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        default:
            break;
    }
    return groupHtml;
}

//解析分组类型 如果返回的为 '' 就表明是字段分组
function initTrackList(workNodeData) {
    var trackNavHtml = '';
    var trackHtml = '';
    var trackList = workNodeData.Track;

    /*
    ActionType ActionTypeText
    1	前进
    2	退回
    5	撤消发起
    8	流程结束
    19	逻辑删除
    20	未知
    24	加签
    100	放入
    100	获取*/
    var filterTrackList = $.grep(trackList, function (value) {
        return value.ActionType == 19 || value.ActionType == 28 || value.ActionType == 27 || value.ActionType == 26 || value.ActionType == 11 || value.ActionType == 10 || value.ActionType == 9 || value.ActionType == 7 || value.ActionType == 6 || value.ActionType == 2 || value.ActionType == 1 || value.ActionType == 8 || value.ActionType == 5;
    });
    workNodeData.Track = filterTrackList;
    $.each(workNodeData.Track, function (i, track) {
        //流程执行人
        var exerNoName = track.Exer.substr(1, track.Exer.length - 2);
        //var exerNoName = track.Exer.substr(1, track.Exer.length - 2).replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");

        var exerNo = exerNoName.split(',')[0];
        var exerName = exerNoName.split(',')[1];
        var exerEmpP = (exerNo == track.EmpFrom ? "" : "（实际发送人：" + exerName + "）");
        track.RDT = track.RDT;
        //track.RDT = track.RDT.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");


        var actionType = track.ActionType;
        trackNavHtml += '<li class="scrollNav" title="发送人：' + track.EmpFromT + "；发送时间：" + track.RDT + "；信息：" + $('<p>' + track.Msg + '</p>').text() + '"><a href="#track' + i + '"><div>' + (i + 1) + '</div>' + (actionType == 5 ? track.NDToT : track.NDFromT) + '<p>发送人:' + track.EmpFromT +exerEmpP+ '</p><p>时间:' + track.RDT + '</p>' + '</a></li>';
        if (actionType != 1 && actionType != 6 && actionType != 7 && actionType != 11 && actionType != 8 && actionType !=28) {
            switch (actionType) {
                case 5:
                    trackHtml += '<div class="trackDiv"><i style="display:none;"></i>' + '<div class="returnTackHeader" id="track' + i + '" ><b>' + (i + 1) + '</b><span>' + "撤销发送信息" + '</span><p class="rdt">处理时间：' + track.RDT + '</p><p class="emps">处理人：' + track.EmpFromT + exerEmpP + '</p></div>' + "<div class='returnTackDiv' >" + track.NDToT + "撤消节点发送;时间" + track.RDT + '</div></div>';
                    break;
                case 2:
                    trackHtml += '<div class="trackDiv"><i style="display:none;"></i>' + '<div class="returnTackHeader" id="track' + i + '" ><b>' + (i + 1) + '</b><span>' + track.ActionTypeText + '信息</span><p class="rdt">处理时间：' + track.RDT + '</p><p class="emps">处理人：' + track.EmpFromT + exerEmpP + '</p></div>' + "<div class='returnTackDiv' >" + track.EmpFromT + "把工单从节点：（" + track.NDFromT + "）" + track.ActionTypeText + "至：(" + track.EmpToT + "," + track.NDToT + "):" + track.RDT + "</br>" + track.ActionTypeText + "信息：" + track.Msg + '</div></div>';
                    break;
                case 19:
                    trackHtml += '<div class="trackDiv"><i style="display:none;"></i>' + '<div class="returnTackHeader" id="track' + i + '" ><b>' + (i + 1) + '</b><span>' + track.ActionTypeText + '信息</span><p class="rdt">处理时间：' + track.RDT + '</p><p class="emps">处理人：' + track.EmpFromT + exerEmpP + '</p></div>' + "<div class='returnTackDiv' >" + track.EmpFromT + "执行" + track.ActionTypeText + ":" + track.RDT + "</br>" + track.ActionTypeText + "删除原因：" + track.Msg + '</div></div>';
                    break;
                default:
                    break;
            }
        } else {
            var trackSrc = "/WF/WorkOpt/ViewWorkNodeFrm.htm?WorkID=" + track.WorkID + "&FID=" + track.FID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + track.NDFrom + "&DoType1=View&MyPK=" + track.MyPK + '&IframeId=track' + i+"&Version="+load.Version;
            trackHtml += '<div class="trackDiv"><iframe id="track' + i + '" name="track11' + i + ' " src="' + trackSrc + '"></iframe></div>';
        }
    });
    //不是查看模式   显示当前处理节点
    function HgetNowFormatDate(time) {
        var date = time ? new Date(time) : new Date();
        var seperator1 = "-";
        var seperator2 = ":";
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        var strHours = date.getHours();
        var strMinutes = date.getMinutes()
        var strSeconds = date.getSeconds()
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        if (strHours >= 0 && strHours <= 9) {
            strHours = "0" + strHours;
        }
        if (strMinutes >= 0 && strMinutes <= 9) {
            strMinutes = "0" + strMinutes;
        }
        if (strSeconds >= 0 && strSeconds <= 9) {
            strSeconds = "0" + strSeconds;
        }
        var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + strHours + seperator2 + strMinutes + seperator2 + strSeconds;
        return {
            currentdate: currentdate,
            getDay: date.getFullYear() + seperator1 + month + seperator1 + strDate,
            getTime: strHours + seperator2 + strMinutes + seperator2 + strSeconds
        };
    }
    var sendName = $.cookie("CCS").split("=")[2].split("&")[0];
    var sendNo = $.cookie("CCS").split("=")[1].split("&")[0];
    var sendt = HgetNowFormatDate().currentdate;
    if (pageData.DoType1 != 'View') {
        trackNavHtml += '<li  class="scrollNav"><a href="#divCurrentForm"><div>' + (workNodeData.Track.length + 1) + '</div>' + workNodeData.Sys_MapData[0].Name + '<p>发送人:' + sendName + '</p></a></li>';
        $('#header b').text((workNodeData.Track.length + 1));
        //$('#header p.rdt').text("处理时间：" + sendt);
        $('#header p.emps').text("处理人：" + sendName);
        //trackNavHtml += '<li class="scrollNav" title="发送人："><a href="#divCurrentForm"><div>' + (workNodeData.Track.length + 1) + '</div>' + "dsfsf" + '</a></li>';
    }
    $('#nav').html(trackNavHtml);
    if (workNodeData.Track.length > 0) {
        $('.navbars').css('display', 'block');
    } else {//新建单子时，不显示轨迹导航，表单宽度为100%
        $('.navbars').css('display', 'none');
        $('#divCurrentForm').css('width', '100%');
        $('#header').css('background', '#5598f3');
    }

    //设置表单宽度为81%  当时新建工单的时候不显示左侧的导航栏
    var width = 81;
    //先去掉
    //if (workNodeData.Sys_MapData != undefined && workNodeData.Sys_MapData.length > 0 && workNodeData.Sys_MapData[0].TableWidth > 900) {//处于中屏时设置宽度最小值
    //    width = workNodeData.Sys_MapData[0].TableWidth;
    //}
    width = width + '%';
    $('#divCurrentForm').css('width', width);
    $('#divTrack').css('width', width);
    //显示左侧导航栏 暂时不显示
    $('#nav').css('display', 'block');

    if (workNodeData.Track.length > 0) {
        $('#nav').css('display', 'block');
    } else {//新建单子时，不显示轨迹导航，表单宽度为100%
        $('#nav').css('display', 'none');
        $('#divCurrentForm').css('width', '100%');
        $('#header').css('background', '#5598f3');
    }

    $($('#nav li')[0]).addClass('current');
    $('#nav').onePageNav();

    $('#divTrack').html(trackHtml);

    $('#divTrack').bind('click', function (obj) {
        var returnContentDiv = $(obj.target).next(".returnTackDiv");
        var i = returnContentDiv.parent().children().first();
        if (returnContentDiv.length == 0) {
            returnContentDiv = $(obj.target).parent().next(".returnTackDiv");
        }
        if (returnContentDiv.css('display') != 'none') {
            returnContentDiv.css('display', 'none');
            i.hide();
        } else {
            returnContentDiv.css('display', 'block');
            i.show();
        }
    });

    //如果工作已经处理  提示用户工作已处理  并关闭处理页面
    if (workNodeData.Track.length > 0 && (workNodeData.Track[workNodeData.Track.length - 1].NDFrom == pageData.FK_Node && workNodeData.Track[workNodeData.Track.length - 1].EmpFrom == sendNo) && (workNodeData.Track[workNodeData.Track.length - 1].ActionType != 5) && (workNodeData.Track[workNodeData.Track.length - 1].NDFromT==pageData.FK_Node) && pageData.DoType1 != 'View') {//ACTIONTYPE=5 是撤销移交
        alert("当前工作已处理");
        //刷新父窗口
        if (window.opener != null) {
            window.opener.location.reload();
        }
        window.close();
    }
}

function InitForm() {
    var workNodeData = JSON.parse(jsonStr);
    var CCFormHtml = '';

    var navGroupHtml = '';
    //解析节点名称
    $('#header span').text(workNodeData.Sys_MapData[0].Name);
    //解析分组
    var groupFileds = workNodeData.Sys_GroupField.sort(function (a, b) {
        return a.Idx - b.Idx;
    });
    groupFileds = $.grep(groupFileds, function (value) {
        return value.EnName == 'ND' + pageData.FK_Node;
    })
    for (var i = 0; i < groupFileds.length; i++) {
        var groupFiled = groupFileds[i];
        var groupHtml = '';
        //初始化分组
        var groupResultHtml = initGroup(workNodeData, groupFiled);
        if (groupResultHtml != '') {//返回的值如果为 ''，就表明是字段分组
            //if (groupFiled.CtrlType != "SubFlow") {
            var reloadBtn = '';
            if (groupFiled.CtrlType == "SubFlow") {
                //reloadBtn = '<label class="reloadIframe">刷新</label>'
                //groupFiled.Lab = "关联的流程";
                groupFiled.Lab = workNodeData.WF_Node[0].SFLab;
            } else if (groupFiled.CtrlType == "Track") {
                //reloadBtn = '<label class="reloadIframe">返回轨迹图</label>'
            }

            //默认展开框架  李建明要求
            var isExtentTxt = "+";
            if (groupFiled.CtrlType == "Frame") {
                isExtentTxt = "-";
            }
            groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style=""><div id="groupH' + groupFiled.Idx + '"  class="group section" data-target="group' + groupFiled.Idx + '"><label class="state">' + isExtentTxt + '</label>' +
                groupFiled.Lab + reloadBtn + '</div></div>';

            navGroupHtml += '<li class="scrollNav"><a href="#groupH' + groupFiled.Idx + '">' + $('<p>' + groupFiled.Lab + '</p>').text() + '</a></li>';
            //} else { }
            groupHtml += groupResultHtml;

            CCFormHtml += groupHtml;
            continue;
        } else if (groupResultHtml == '' && groupFiled.CtrlType == "Ath") {//无附件的分组不展示  子流程
            continue;
        }
        else {//返回的值如果为 ''，就表明是字段分组  分组名称不显示
            groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12"><div class="section group" id="groupH' + groupFiled.Idx + '"  data-target="group' + groupFiled.Idx + '"><label class="state">-</label>' +
                groupFiled.Lab + '</div></div>';
            navGroupHtml += '<li class="scrollNav"><a href="#groupH' + groupFiled.Idx + '">' + $('<p>' + groupFiled.Lab + '</p>').text() + '</a></li>';
            groupHtml += groupResultHtml;
            CCFormHtml += groupHtml;
        }

        //解析字段
        //过滤属于本分组的字段 
        groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12  col-xs-12" style="clear:both;"> ' + '<input type="button" value="' + groupFiled.Lab + '"/></div>';
        var mapAttrData = $.grep(workNodeData.Sys_MapAttr, function (value) {
            return value.GroupID == groupFiled.OID;
        });

        //开始解析表单字段
        var mapAttrsHtml = InitMapAttr(mapAttrData, workNodeData);
        CCFormHtml += "<div class='col-lg-12 col-md-12 col-sm-12 col-xs-12 ' id='" + "group" + groupFiled.Idx + "'>" + mapAttrsHtml + "</div>";

        CCFormHtml += "</div>";
    }

    $('#CCForm').html(CCFormHtml);

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    //分组导航被轨迹导航征用
    //$('#nav').html(navGroupHtml);

    //$($('#nav li')[0]).addClass('current');
    //$('#nav').onePageNav();

    //初始化提示信息
    var alertMsgs = workNodeData.AlertMsg;
    if (alertMsgs != undefined && alertMsgs.length > 0) {
        var alertMsgHtml = '';
        $.each(alertMsgs, function (i, alertMsg) {
            alertMsgHtml += "退回标题：" + alertMsg.Title + "退回信息：" + alertMsg.Msg + "</br>";
        });
        $('#Message').html(alertMsgHtml);
    }

    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    })

    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt();

    //设置默认值
    for (var j = 0; j < workNodeData.Sys_MapAttr.length; j++) {
        var mapAttr = workNodeData.Sys_MapAttr[j];
        //添加 label
        //如果是整行的需要添加  style='clear:both'


        var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
            $('#TB_' + mapAttr.KeyOfEn).val(defValue);
        }
    }
    //绑定扩展附件
    $('.divAth').bind('click', function (obj) {
        var keyOfEn = $(obj.target).data().target;
        var tbObj = $('#TB_' + keyOfEn);
        var divObj = $(obj.target);
        var atParamObj = AtParaToJson(tbObj.data().target);
        var athRefObj = atParamObj.AthRefObj;
        var divId = 'DIV_' + keyOfEn;
        var tbId = 'TB_' + keyOfEn;
        var ath = $.grep(workNodeData.Sys_FrmAttachment, function (value) {
            return value.MyPK == athRefObj;
        })
        var html = '<iframe style="width:100%;border:0px;height:400px;" id="iframeAthForm" name="iframeAthForm"></iframe>';
        $("#athModal .modal-body").html("");
        $("#athModal .modal-body").html(html);
        if (ath.length > 0) {
            ath = ath[0];
            var src = "";
            if (pageData.IsReadonly)
                src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1&___jsVersion="+load.Version;
            else
                src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK+"&___jsVersion="+load.Version;;
            $('#iframeAthForm').attr('src', src);
            atParamObj["tbId"] = tbId;
            atParamObj["divId"] = divId;
            $('#iframeAthForm').data(atParamObj);
            $('#athModal .modal-title').text("上传附件：" + $(obj.target).parent().prev().children('label').text());
            $('#athModal').modal().show();
        }
    });
    //绑定分组的按钮事件  如果不是字段分组就变成可以折叠的
    $('.group').bind('click', function (obj) {
        //阻止事件冒泡
        /*if (event.target != this) {
            return;
        }*/
        var display = '';
        var targetDiv = this;
        var state = $(targetDiv).find('.state');

        var stateText = state.text();
        if (stateText == "-") {
            display = 'none';
            state.text('+');
        } else {
            display = 'block';
            state.text('-');
        }

        var div = $('#' + $(targetDiv).data().target).css('display', display);
    });

    //刷新子流程的IFRAME
    $('.reloadIframe').bind('click', function (obj) {
        var targetDiv = $(obj.target).parent();
        var iframe = $('#' + $(targetDiv).data().target).children('iframe');
        //iframe[0].contentWindow.location.reload();
        iframe[0].contentWindow.location.href = iframe[0].src;
    })

    //如果是IsReadOnly，就表示是查看页面，不是处理页面
    if (pageData.IsReadOnly != undefined && pageData.IsReadOnly == "1") {
        setAttachDisabled();
        setToobarUnVisible();
        setFormEleDisabled();
    }

    //设置CCFORM的表格宽度  
    //if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
    //$('#CCForm').css('min-width', workNodeData.Sys_MapData[0].TableWidth);
    // }


    showNoticeInfo();

    showTbNoticeInfo();

    //FORM 窗体加载完成之后，再加载  _self.js  解决王海亮的删除按钮通过_self.js 控制展示展示与否的问题；
    ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题
    var enName = workNodeData.Sys_MapData[0].No;
    try {
        ////加载JS文件
        //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
        //$('body').append($('<div>' + jsSrc + '</div>'));

        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + "_Self.js" + (isClearCach ? "?Version=" + load.Version : "");
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    var jsSrc = '';
    try {
        //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + ".js' ></script>";
        //$('body').append($('<div>' + jsSrc + '</div>'));

        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + ".js" + (isClearCach ? "?Version=" + load.Version : "");
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }
}

//刷新子流程
function refSubSubFlowIframe() {
    var iframe = $('iframe[src*="SubFlow.aspx"]');
    //iframe[0].contentWindow.location.reload();
    iframe[0].contentWindow.location.href = iframe[0].src;
}
//回填扩展字段的值
function SetAth(data) {
    var atParamObj = $('#iframeAthForm').data();
    var tbId = atParamObj.tbId;
    var divId = atParamObj.divId;
    var athTb = $('#' + tbId);
    var athDiv = $('#' + divId);

    $('#athModal').modal('hide');
    //不存在或来自于viewWorkNodeFrm
    if (atParamObj != undefined && atParamObj.IsViewWorkNode != 1 && divId != undefined && tbId != undefined) {
        if (atParamObj.AthShowModel == "1") {
            athTb.val(data.join('*'));
            athDiv.html(data.join(';&nbsp;'));
        } else {
            athTb.val('@AthCount=' + data.length);
            athDiv.html("附件<span class='badge' >" + data.length + "</span>个");
        }
    } else {
        $('#athModal').removeClass('in');
    }
    $('#athModal').hide();
    var ifs = $("iframe[id^=track]").contents();
    if (ifs.length > 0) {
        for (var i = 0; i < ifs.length; i++) {
            $(ifs[i]).find(".modal-backdrop").hide();
        }
    }
}
function SetAths() {
   $('#athModal').hide();
    var ifs = $("iframe[id^=track]").contents();
    if (ifs.length > 0) {
        for (var i = 0; i < ifs.length; i++) {
            $(ifs[i]).find(".modal-backdrop").hide();
        }
    }
}

//查看页面的附件展示  查看页面调用
function ShowViewNodeAth(athLab, atParamObj, src) {
    var athForm = $('iframeAthForm');
    var athModal = $('athModal');
    var athFormTitle = $('#athModal .modal-title');
    athFormTitle.text("上传附件：" + athLab);
    athModal.modal().show();
}

window.onresize = function () {
    if (pageData.Col == 8) {
        if (jsonStr != undefined && jsonStr != '') {
            var workNodeData = JSON.parse(jsonStr);
            //设置CCFORM的表格宽度  
            if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
                $('#CCForm').css('min-width', workNodeData.Sys_MapData[0].TableWidth);
            }
            else {
                $('#CCForm').css('min-width', 0);
            }
        }
    }
}
//4列切为8列
function Col4To8() {
    var workNodeData = JSON.parse(jsonStr);
    pageData.Col = 8;
    $('.col-sm-2').attr('class', 'col-lg-1 col-md-1 col-sm-2');
    $('.col-sm-4').attr('class', 'col-lg-2 col-md-2 col-sm-4');
    $('.col-sm-10').attr('class', 'col-lg-11 col-md-11 col-sm-10')
    $('#topContentDiv').css('width', 'auto');
    if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
        $('#CCForm').css('min-width', workNodeData.Sys_MapData[0].TableWidth);
    }

    var sm2 = $('.col-sm-2');
    var sm4 = $('.col-sm-4');
    var sm8 = $('.col-sm-8');
    var sm10 = $('.col-sm-10');
    var sm12 = $('.col-sm-12');
    $.each(sm2, function (sm, i) {
        if (!$(sm).hasClass('col-xs-2')) {
            $(sm).addClass('col-xs-2');
        }
    });
    $.each(sm4, function (sm, i) {
        if (!$(sm).hasClass('col-xs-4')) {
            $(sm).addClass('col-xs-4');
        }
    });
    $.each(sm8, function (sm, i) {
        if (!$(sm).hasClass('col-xs-8')) {
            $(sm).addClass('col-xs-8');
        }
    });
    $.each(sm10, function (sm, i) {
        if (!$(sm).hasClass('col-xs-10')) {
            $(sm).addClass('col-xs-10');
        }
    });
    $.each(sm12, function (sm, i) {
        if (!$(sm).hasClass('col-xs-12')) {
            $(sm).addClass('col-xs-12');
        }
    });

    $('#topContentDiv').css('margin-left', '15px');
    $('#topContentDiv').css('margin-right', '15px');
    $('#divCurrentForm').css('width', 'auto');
    $('#divTrack').css('width', 'auto');
    //隐藏左侧导航栏
    $('#nav').css('display', 'none');
}
//8列切为4列
function Col8To4() {
    pageData.Col = 4;
    /*  $('.col-sm-2').attr('class', 'col-lg-2 col-md-2 col-sm-2');
      $('.col-sm-4').attr('class', 'col-lg-4 col-md-4 col-sm-4')
      $('.col-sm-10').attr('class', 'col-lg-10 col-md-10 col-sm-10');*/

    $('.col-sm-2').attr('class', 'col-lg-2 col-md-2 col-sm-2');
    $('.col-sm-4').attr('class', 'col-lg-4 col-md-4 col-sm-4')
    $('.col-sm-10').attr('class', 'col-lg-10 col-md-10 col-sm-10');

    $('#CCForm').css('min-width', 0);
    var sm2 = $('.col-sm-2');
    var sm4 = $('.col-sm-4');
    var sm8 = $('.col-sm-8');
    var sm10 = $('.col-sm-10');
    var sm12 = $('.col-sm-12');
    $.each(sm2, function (i,sm) {
        if (!$(sm).hasClass('col-xs-2')) {
            $(sm).addClass('col-xs-2');
        }
    });
    $.each(sm4, function (i, sm) {
        if (!$(sm).hasClass('col-xs-4')) {
            $(sm).addClass('col-xs-4');
        }
    });
    $.each(sm8, function (i, sm) {
        if (!$(sm).hasClass('col-xs-8')) {
            $(sm).addClass('col-xs-8');
        }
    });
    $.each(sm10, function (i, sm) {
        if (!$(sm).hasClass('col-xs-10')) {
            $(sm).addClass('col-xs-10');
        }
    });
    $.each(sm12, function (i, sm) {
        if (!$(sm).hasClass('col-xs-12')) {
            $(sm).addClass('col-xs-12');
        }
    });

    //$('#topContentDiv').css('width', '900px');
    //$('#topContentDiv').css('margin-left', 'auto');
    //$('#topContentDiv').css('margin-right', 'auto');
    //$('#header').css('width', '900px');
    //$('#Message').css('width', '900px');

}

//解析表单字段 MapAttr
function InitMapAttr(mapAttrData, workNodeData) {
    var resultHtml = '';

    var hiddenHtml = '';
    for (var j = 0; j < mapAttrData.length; j++) {
        var mapAttr = mapAttrData[j];
        if (mapAttr.UIVisible=="1") {//是否显示
            //添加 label
            //如果是整行的需要添加  style='clear:both'

            var str = '';
            var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
            for (var o in mapAttr) {
                str += o + ":" + mapAttr[o];
            }

            var eleHtml = '';
            var isInOneRow = false;//是否占一整行
            var islabelIsInEle = false;//

            eleHtml += '';

            if (mapAttr.UIContralType != 6) {
                //添加文本框 ，日期控件等
                //AppString   
                if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                    if (mapAttr.ColSpan == 0 || mapAttr.ColSpan == 2 || mapAttr.ColSpan == 1 || mapAttr.ColSpan == 3) {//占有1-2  3列的文本框
                        var mdCol = 2;
                        var smCol = 4;
                        switch (mapAttr.ColSpan) {
                            case 1:
                                mdCol = 2;
                                smCol = 4;
                                break;
                            case 2:
                                mdCol = 4;
                                smCol = 8;
                                break;
                            case 3:
                                mdCol = 11;
                                smCol = 10;
                                isInOneRow = true;
                                break;
                        }
                        if (mapAttr.UIContralType == "1") {//DDL 下拉列表框
                            eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">1' +
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" +
                                (workNodeData, mapAttr, defValue) + "</select>";
                            eleHtml += '</div>';
                        } else {//文本区域
                            if (mapAttr.UIHeight <= 23) {
                                eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">' +
                                    "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                    + '</div>';
                            }
                            else {//大于23就是多行
                                if (mapAttr.ColSpan == 1 || mapAttr.ColSpan == 2) {
                                    mdCol = 4;
                                    smCol = 12;
                                }
                                //把TEXTAREA都写成10个col-md-10
                                mdCol = 10;
                                var uiHeight = mapAttr.UIHeight / 23 * 30;
                                islabelIsInEle = true;
                                eleHtml += '<div style="text-align:right;padding:0px;margin:0px; clear:both;" class="col-lg-' + 2 + ' col-md-' + 2 + ' col-sm-' + 2 + '">'
                                    + "<label>" + mapAttr.Name + "</label>"
                                    +
                                    (mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")+"</div>";
                                eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">'
                                    +
                                    "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + uiHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                    + '</div>';

                            }
                        }
                    } else if (mapAttr.ColSpan == "4" || (mapAttr.ColSpan == "3" && mapAttr.UIHeight > 23)) {//大文本区域  且占一整行
                        var uiHeight = mapAttr.UIHeight / 23 * 30;
                        isInOneRow = true;
                        eleHtml += '<div class="col-lg-11 col-md-11 col-sm-10">' +
                            "<textarea  style='height:" + uiHeight + "px' maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "'" + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" + "</textarea>"
                            + '</div>';
                    }
                } //AppDate
                else if (mapAttr.MyDataType == 6) {//AppDate
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input maxlength=" + mapAttr.MaxLen + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + "</div>";
                }
                else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                        //enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input maxlength=" + mapAttr.MaxLen / 2 + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />" + "</div>";
                }
                else if (mapAttr.MyDataType == 4) {// AppBoolean = 7
                    var colMd = 2;
                    var colsm = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        colMd = 4;
                        colsm = 8;
                    }

                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    //CHECKBOX 默认值
                    var checkedStr = '';
                    if (checkedStr != "true" && checkedStr != '1') {
                        checkedStr = ' checked="checked" '
                    }
                    checkedStr = ConvertDefVal(workNodeData, '', mapAttr.KeyOfEn);
                    eleHtml += '<div class="col-lg-' + colMd + ' col-md-' + colMd + ' col-sm-' + colsm + '">' + "<input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>" + "</div>";
                }

                if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                    var colMd = 2;
                    var colsm = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        colMd = 11;
                        colsm = 10;
                    }
                    if (mapAttr.UIContralType == 1) {//DDL
                        eleHtml += '<div class="col-lg-' + colMd + ' col-md-' + colMd + ' col-sm-' + colsm + '">' +
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                        eleHtml += '</div>';
                        //eleHtml += "</div>";
                    }

                    if (mapAttr.UIContralType == 3) {
                        //RadioBtn
                        var operations = '';

                        if (mapAttr.ColSpan == 1 || mapAttr.ColSpan >= 3) {
                            if (mapAttr.ColSpan == 1) {
                                eleHtml += '<div class="col-md-2 col-sm-4 col-lg-2">';
                            } else if (mapAttr.ColSpan >= 3) {
                                eleHtml += '<div class="col-md-11 col-sm-10 col-lg-11" style="padding:3px 20px;">';
                            }
                            var enums = workNodeData.Sys_Enum;
                            enums = $.grep(enums, function (value) {
                                return value.EnumKey == mapAttr.UIBindKey;
                            });

                            var rbShowModel = 0;//RBShowModel=0时横着显示RBShowModel=1时竖着显示
                            var showModelindex = mapAttr.AtPara.indexOf('@RBShowModel=');
                            if (showModelindex >= 0) {//@RBShowModel=0
                                rbShowModel = mapAttr.AtPara.substring('@RBShowModel='.length, '@RBShowModel='.length + 1);
                            }
                            $.each(enums, function (i, obj) {
                                operations += "<input id='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' type='radio' " + (obj.IntKey == defValue ? " checked='checked' " : "") + "  name='RB_" + mapAttr.KeyOfEn + "' value='" + obj.IntKey + "'/><label for='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' class='labRb'>" + obj.Lab + "</label>" + (rbShowModel == "1" ? "</br>" : '');
                            });
                        }

                        eleHtml += operations;
                        //eleHtml += "</div>";
                        eleHtml += "</div>";
                    }
                }

                // AppDouble  AppFloat AppInt .
                if (mapAttr.MyDataType == 5 ||
                mapAttr.MyDataType == 3 ||
                (mapAttr.MyDataType == 2 && mapAttr.LGType != 1)
            ) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + "</div>";
                }
                //AppMoney  AppRate
                if (mapAttr.MyDataType == 8) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + "</div>";
                }

                if (mapAttr.LGType == 2) {
                    var mdCol = 2;
                    var smCol = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        mdCol = 4;
                        smCol = 8;
                    }

                    eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">' +
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";

                    eleHtml += '</div>';
                }
            } else {
                //展示附件信息
                var atParamObj = AtParaToJson(mapAttr.AtPara);
                if (atParamObj.AthRefObj != undefined) {//扩展设置为附件展示
                    var colMd = 2;
                    var colsm = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        colMd = 11;
                        colsm = 10;
                    }

                    eleHtml += '<div class="col-lg-' + colMd + ' col-md-' + colMd + ' col-sm-' + colsm + '">' +
                            "<input type='hidden' class='tbAth' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
                    defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
                    if (defValue.indexOf('@AthCount=') == 0) {
                        defValue = "附件" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>个";
                    } else {
                        defValue = defValue;
                    }
                    eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";

                    //var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html() +
                    //'<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
                    //  tb.parent().html(eleHtml);

                    eleHtml += '</div>';
                }
            }

            if (!islabelIsInEle) {
                eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
                (mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
                + "</div>" + eleHtml;

            }
            resultHtml += eleHtml;
        } else {
            var value = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (value == undefined) {
                value = '';
            } else {
                //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
            }

            //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
            hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
        }
    }

    return resultHtml + hiddenHtml;
}

//处理MapExt
function AfterBindEn_DealMapExt() {
    var workNode = JSON.parse(jsonStr);
    var mapExtArr = workNode.Sys_MapExt;
    for (var i = 0; i < mapExtArr.length; i++) {
        var mapExt = mapExtArr[i];
        switch (mapExt.ExtType) {
            case "PopVal"://PopVal窗返回值
                var tb = $('[name=TB_' + mapExt.AttrOfOper + ']');
                //tb.attr("placeholder", "请双击选择。。。");
                tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
                tb.attr("ondblclick", "ReturnValCCFormPopValGoogle(this,'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');");

                tb.attr('readonly', 'true');
                tb.attr('disabled', 'true');
                var icon = '';
                var popWorkModelStr = '';
                var popWorkModelIndex = mapExt.AtPara != undefined ? mapExt.AtPara.indexOf('@PopValWorkModel=') : -1;
                if (popWorkModelIndex >= 0) {
                    popWorkModelIndex = popWorkModelIndex + '@PopValWorkModel='.length;
                    popWorkModelStr = mapExt.AtPara.substring(popWorkModelIndex, popWorkModelIndex + 1);
                }
                switch (popWorkModelStr) {
                    /// <summary>
                    /// 自定义URL
                    /// </summary>
                    //SelfUrl =1,
                    case "1":
                        icon = "glyphicon glyphicon-th";
                        break;
                        /// <summary>
                        /// 表格模式
                        /// </summary>
                        // TableOnly,
                    case "2":
                        icon = "glyphicon glyphicon-list";
                        break;
                        /// <summary>
                        /// 表格分页模式
                        /// </summary>
                        //TablePage,
                    case "3":
                        icon = "glyphicon glyphicon-list-alt";
                        break;
                        /// <summary>
                        /// 分组模式
                        /// </summary>
                        // Group,
                    case "4":
                        icon = "glyphicon glyphicon-list-alt";
                        break;
                        /// <summary>
                        /// 树展现模式
                        /// </summary>
                        // Tree,
                    case "5":
                        icon = "glyphicon glyphicon-tree-deciduous";
                        break;
                        /// <summary>
                        /// 双实体树
                        /// </summary>
                        // TreeDouble
                    case "6":
                        icon = "glyphicon glyphicon-tree-deciduous";
                        break;
                    default:
                        break;
                }
                var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html() +
                '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
                tb.parent().html(eleHtml);
                break;
            case "RegularExpression"://正则表达式  统一在保存和提交时检查
                var tb = $('[name=TB_' + mapExt.AttrOfOper + ']');
                //tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('name') + "'," + mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}') + ",'" + mapExt.Tag1 + "')");

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)
                    //tb.data().name = tb.attr('name');
                    //tb.data().Doc = mapExt.Doc;
                    //tb.data().Tag1 = mapExt.Tag1;
                    //tb.attr("data-name", tb.attr('name'));
                    //tb.attr("data-Doc", tb.attr('name'));
                    //tb.attr("data-checkreginput", "CheckRegInput('" + tb.attr('name') + "'," + mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}') + ",'" + mapExt.Tag1 + "')");
                }
                break;
            case "InputCheck"://输入检查
                //var tbJS = $("#TB_" + mapExt.AttrOfOper);
                //if (tbJS != undefined) {
                //    tbJS.attr(mapExt.Tag2, mapExt.Tag1 + "(this)");
                //}
                //else {
                //    tbJS = $("#DDL_" + mapExt.AttrOfOper);
                //    if (ddl != null)
                //        ddl.attr(mapExt.Tag2, mapExt.Tag1 + "(this);");
                //}
                break;
            case "TBFullCtrl"://自动填充  先不做
                break;
                var tbAuto = $("#TB_" + mapExt.AttrOfOper);
                if (tbAuto == null)
                    continue;

                tbAuto.attr("ondblclick", "ReturnValTBFullCtrl(this,'" + mapExt.MyPK + "');");
                tbAuto.attr("onkeyup", "DoAnscToFillDiv(this,this.value,'" + "#TB_" + mapExt.AttrOfOper + "', '" + mapExt.MyPK + "');");
                tbAuto.attr("AUTOCOMPLETE", "OFF");
                if (me.Tag != "") {
                    /* 处理下拉框的选择范围的问题 */
                    var strs = mapExt.Tag.split('$');
                    for (var str in strs) {
                        var str = strs[k];
                        if (str = "") {
                            continue;
                        }

                        var myCtl = str.Split(':');
                        var ctlID = myCtl[0];
                        var ddlC1 = $("#DDL_" + ctlID);
                        if (ddlC1 == null) {
                            continue;
                        }

                        //如果文本库数值为空，就让其返回.
                        var txt = tbAuto.val();
                        if (txt == '')
                            continue;

                        //获取要填充 ddll 的SQL.
                        var sql = myCtl[1].Replace("~", "'");
                        sql = sql.Replace("@Key", txt);
                        //sql = BP.WF.Glo.DealExp(sql, en, null);  怎么办

                        //try
                        //{
                        //    dt = DBAccess.RunSQLReturnTable(sql);
                        //}
                        //catch (Exception ex)
                        //{
                        //    this.Clear();
                        //    this.AddFieldSet("配置错误");
                        //    this.Add(me.ToStringAtParas() + "<hr>错误信息:<br>" + ex.Message);
                        //    this.AddFieldSetEnd();
                        //    return;
                        //}

                        //if (dt.Rows.Count != 0)
                        //{
                        //    string valC1 = ddlC1.SelectedItemStringVal;
                        //    foreach (DataRow dr in dt.Rows)
                        //{
                        //        ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                        //    if (li == null)
                        //    {
                        //        ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        //    }
                        //    else
                        //    {
                        //        li.Attributes["enable"] = "false";
                        //        li.Attributes["display"] = "false";

                        //    }
                        //}
                        //ddlC1.SetSelectItem(valC1);
                    }
                }

                break;
            case "ActiveDDL":/*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlPerant == null || ddlChild == null)
                    continue;
                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\')");
                // 处理默认选择。
                //string val = ddlPerant.SelectedItemStringVal;
                var valClient = ConvertDefVal(workNode, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //ddlChild.select(valClient);  未写
                break;
            case "AutoFull"://自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                //直接替换

                if (mapExt.Doc != undefined && mapExt.Doc != '') {
                    //以 + -* 、% 来分割
                    //先来计算  + -* 、%  的位置

                    if (mapExt.Doc.indexOf('+') > 0 || mapExt.Doc.indexOf('-') > 0 || mapExt.Doc.indexOf('*') > 0 || mapExt.Doc.indexOf('/') > 0) {
                        var mapExtDocArr1 = [];
                        var lastOperatorIndex = -1;
                        var operatorArr = [];
                        for (var j = 0; j < mapExt.Doc.length; j++) {
                            if (mapExt.Doc[j] == "+" || mapExt.Doc[j] == "-" || mapExt.Doc[j] == "*" || mapExt.Doc[j] == "/"
                                ) {
                                operatorArr.push(mapExt.Doc[j]);

                                mapExtDocArr1.push(mapExt.Doc.substring(lastOperatorIndex + 1, j));
                                lastOperatorIndex = j;
                            }
                        }
                        mapExtDocArr1.push(mapExt.Doc.substring(lastOperatorIndex + 1, mapExt.Doc.length))

                        for (var m = 0; m < mapExtDocArr1.length; m++) {
                            var extDocObj1 = mapExtDocArr1[m].replace('@', '');
                            //将extDocObj1转换成KeyOfEn
                            var extObjAr = $.grep(workNodeData.Sys_MapAttr, function (val) { return val.Name == extDocObj1 || val.KeyOfEn == extDocObj1; });

                            if (extObjAr.length == 0) {
                                // alert("mapExt:" + mapExt.AttrOfOper + "配置有误");

                            } else {
                                extDocObj1 = extObjAr[0].KeyOfEn;
                                $(tr).find('[name=TB_' + mapExt.AttrOfOper + ']').attr('disabled', true);


                                if ($(tr).find('[name=TB_' + extDocObj1 + ']').length > 0) {
                                    $(tr).find('[name=TB_' + extDocObj1 + ']').data().mapExt = mapExt;
                                    $(tr).find('[name=TB_' + extDocObj1 + ']').bind('blur', function (obj) {


                                        //替换 
                                        var mapExt = $(obj.target).data().mapExt;
                                        var mapExtDoc = mapExt.Doc;
                                        var evelStr = mapExt.Doc;
                                        var tmpResult = 1;
                                        var tr = $(obj.target).parent().parent();
                                        var attrOfOperEle = $(obj.target).parent().parent().find('[name=TB_' + mapExt.AttrOfOper + "]");
                                        for (var m = 0; m < workNodeData.Sys_MapAttr.length; m++) {
                                            var mapAttr = workNodeData.Sys_MapAttr[m];
                                            var hasKeyOfEn = true;
                                            while (hasKeyOfEn) {
                                                var mapExdDocKeyOfEnIndex = mapExtDoc.indexOf('@' + mapAttr.KeyOfEn);
                                                var tranValue = mapAttr.KeyOfEn;
                                                if (mapExdDocKeyOfEnIndex == -1) {
                                                    mapExdDocKeyOfEnIndex = mapExtDoc.indexOf('@' + mapAttr.Name);
                                                    tranValue = mapAttr.Name;
                                                }
                                                //判断参数后面是否是一个运算操作符
                                                var optionVal = mapExtDoc.substring(mapExdDocKeyOfEnIndex + tranValue.length + 1, mapExdDocKeyOfEnIndex + tranValue.length + 2);

                                                if (mapExdDocKeyOfEnIndex >= 0 && (optionVal == '+' || optionVal == '-' || optionVal == '*' || optionVal == '/' || optionVal == '')) {
                                                    mapExtDoc = mapExtDoc.replace('@' + tranValue, "parseFloat($(tr).find('[name=TB_" + mapAttr.KeyOfEn + "]').val())");
                                                } else {
                                                    hasKeyOfEn = false;
                                                }
                                            }
                                        }

                                        tmpResult = eval(mapExtDoc);
                                        attrOfOperEle.val(tmpResult);

                                        $(tr).data().data[$(obj.target).data().mapExt.AttrOfOper] =
                                tmpResult;
                                    })
                                }
                            }
                        }
                    }
                }
                break;
            case "DDLFullCtrl":// 自动填充其他的控件..  先不做
                break;
                var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
                if (ddlOper == null)
                    continue;

                ddlOper.attr("onchange", "Change('" + workNode.Sys_MapData[0].No + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");
                if (me.Tag != "") {
                    /* 下拉框填充范围. */
                    var strs = me.Tag.split('$');
                    for (var k = 0; k < strs.length; k++) {
                        var str = strs[k];
                        if (str == "")
                            continue;

                        var myCtl = str.split(':');
                        var ctlID = myCtl[0];
                        var ddlC1 = $("#DDL_" + ctlID);
                        if (ddlC1 == null) {
                            //me.Tag = "";
                            //me.Update();
                            continue;
                        }

                        //如果触发的dll 数据为空，则不处理.
                        if (ddlOper.val() == "")
                            continue;

                        var sql = myCtl[1].Replace("~", "'");
                        sql = sql.Replace("@Key", ddlOper.val());

                        //需要执行SQL语句
                        //sql = BP.WF.Glo.DealExp(sql, en, null);

                        //dt = DBAccess.RunSQLReturnTable(sql);
                        //string valC1 = ddlC1.SelectedItemStringVal;
                        //if (dt.Rows.Count != 0)
                        //{
                        //    foreach (DataRow dr in dt.Rows)
                        //{
                        //        ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                        //    if (li == null)
                        //    {
                        //        ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        //    }
                        //    else
                        //    {
                        //        li.Attributes["visable"] = "false";
                        //    }
                        //}

                        var items = [{ No: 1, Name: '测试1' }, { No: 2, Name: '测试2' }, { No: 3, Name: '测试3' }, { No: 4, Name: '测试4' }, { No: 5, Name: '测试5' }];
                        var operations = '';
                        $.each(items, function (i, item) {
                            operations += "<option  value='" + item.No + "'>" + item.Name + "</option>";
                        });
                        ddlC1.children().remove();
                        ddlC1.html(operations);
                        //ddlC1.SetSelectItem(valC1);
                    }
                }
                break;
        }
    }
}
//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
}

//初始化下拉列表框的OPERATION
function InitDDLOperation(workNodeData, mapAttr, defVal) {
    var operations = '';
    //外键类型
    if (mapAttr.LGType == 2) {
        if (workNodeData[mapAttr.KeyOfEn] != undefined) {
            $.each(workNodeData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
        else if (workNodeData[mapAttr.UIBindKey] != undefined) {
            $.each(workNodeData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
    } else {
        var enums = workNodeData.Sys_Enum;
        enums = $.grep(enums, function (value) {
            return value.EnumKey == mapAttr.UIBindKey;
        });


        $.each(enums, function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
        });

    }
    return operations;
}

//填充默认数据
function ConvertDefVal(workNodeData, defVal, keyOfEn) {
    //计算URL传过来的表单参数@TXB_Title=事件测试

    var pageParams = getQueryString();
    var pageParamObj = {};
    $.each(pageParams, function (i, pageParam) {
        if (pageParam.indexOf('@') == 0) {
            var pageParamArr = pageParam.split('=');
            pageParamObj[pageParamArr[0].substring(1, pageParamArr[0].length)] = pageParamArr[1];
        }
    });

    var result = defVal;

    //通过MAINTABLE返回的参数
    for (var ele in workNodeData.MainTable[0]) {
        if (keyOfEn == ele && workNodeData.MainTable[0] != '') {
            result = workNodeData.MainTable[0][ele];
            break;
        }
    }

    //通过URL参数传过来的参数
    for (var pageParam in pageParamObj) {
        if (pageParam == keyOfEn) {
            result = pageParamObj[pageParam];
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    return result = unescape(result);
}
//加载表单数据.
function GenerWorkNode() {
    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = '&' + urlParam;
    urlParam = urlParam.replace('DoType=', 'DoTypeDel=xx');
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        url: "MyFlow.ashx?DoType=GenerWorkNode" +  "&m=" + Math.random()+urlParam,
        dataType: 'html',
        success: function (data) {
            jsonStr = data;
            var gengerWorkNode = {};
            try {
                var gengerWorkNode = JSON.parse(data);
            }
            catch (err) {
                alert("GenerWorkNode转换JSON失败:" + data);
                return;
            }
            //显示父流程 链接
            if (gengerWorkNode.WF_GenerWorkFlow != null && gengerWorkNode.WF_GenerWorkFlow.length > 0 && (gengerWorkNode.WF_GenerWorkFlow[0].PWorkID != 0 || gengerWorkNode.WF_GenerWorkFlow[0].PWorkID2 != 0)) {
                $('#btnShowPFlow').bind('click', function () {
                    var pworkid = 1;
                    var pfk_node = 1;
                    var pfk_flow = 1;
                    if (gengerWorkNode.WF_GenerWorkFlow[0].PWorkID != 0) {
                        pworkid = gengerWorkNode.WF_GenerWorkFlow[0].PWorkID;
                        pfk_flow = gengerWorkNode.WF_GenerWorkFlow[0].PFlowNo;
                        pfk_node = gengerWorkNode.WF_GenerWorkFlow[0].PNodeID;
                    } else {
                        pworkid = gengerWorkNode.WF_GenerWorkFlow[0].PWorkID2;
                        pfk_flow = gengerWorkNode.WF_GenerWorkFlow[0].PFlowNo2;
                        pfk_node = gengerWorkNode.WF_GenerWorkFlow[0].PNodeID2;
                    }

                    window.open("WorkOpt/FoolFrmTrack.htm?FK_Flow=" + pfk_flow + "&WorkID=" + pworkid + "&FK_Node=" + pfk_node);

                });

                $('#ShowPFlow').css('display', 'none');
            } else {
                $('#ShowPFlow').css('display', 'none');
            }

            //如果为查看页面，只显示历史轨迹
            initTrackList(gengerWorkNode);
            if (pageData.DoType1 == 'View') {
                $('#divCurrentForm').css('display', 'none');
                return;
            }
            //是分流或者分合流  且是 退回状态 转到页面 WF\WorkOpt\DealSubThreadReturnToHL.html
            if ((gengerWorkNode.WF_Node[0].RunModel == 2 || gengerWorkNode.WF_Node[0].RunModel == 3) && gengerWorkNode.WF_GenerWorkFlow[0].WFState == 5) {
                $('#')
                var iframeHtml = "<iframe style='width:100%;' src='./WorkOpt/DealSubThreadReturnToHL.html?FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.WorkID + "' name='Iframe_DealSubThreadReturnToHL' id='Iframe_DealSubThreadReturnToHL'></iframe>";
                $('#topContentDiv').html(iframeHtml);

                $('#topContentDiv').append($('<div style="margin-left: 80px;margin-right: 20px;' +
                'color: red;font-weight: bold;">备注：撤销前，请核实会签审批情况，已完成审批任务的工作将被保留，未完成审批任务的工作都会被撤消，若还存在其他审批人员未完成审批工作的，撤销后，需要再次为未完成审批工作的人员发起会签</div>'));
                $('#header span').html("处理退回信息");

                return;
            }
            //加签回复
            if (gengerWorkNode.WF_GenerWorkFlow[0].WFState == 11) {

            }

            //解析表单
            InitForm();
            ///根据下拉框选定的值，绑定表单中哪个元素显示，哪些元素不显示
            //showEleDependOnDRDLValue();
            //
            InitToNodeDDL();
            //
            Col8To4();


            Common.MaxLengthError();
            // window.location.href = "#divCurrentForm";
        }
    });
}

//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam) {
    var formss = $('#divCCForm').serialize();
    var formArr = formss.split('&');
    var formArrResult = [];
    //获取CHECKBOX的值
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            if ($('#' + ele.split('=')[0] + ':checked').length == 1) {
                ele = ele.split('=')[0] + '=1';
            } else {
                ele = ele.split('=')[0] + '=0';
            }
        }
        formArrResult.push(ele);
    });

    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {
        var name = $(disabledEle).attr('name');
        switch (disabledEle.tagName.toUpperCase()) {
            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX"://复选框
                        formArrResult.push(name + '=' + $(disabledEle).is(':checked') ? 1 : 0);
                        break;
                    case "TEXT"://文本框
                        formArrResult.push(name + '=' + $(disabledEle).val());
                        break;
                    case "RADIO"://单选钮
                        var eleResult = name + '=' + $('[name="' + name + ':checked"]').val();
                        if (!$.inArray(formArrResult, eleResult)) {
                            formArrResult.push();
                        }
                        break;
                }
                break;
                //下拉框
            case "SELECT":
                formArrResult.push(name + '=' + $(disabledEle).children('option:checked').val());
                break;
                //文本区域
            case "TEXTAREA":
                formArrResult.push(name + '=' + $(disabledEle).val());
                break;
        }
    });

    //获取表单中隐藏的表单元素的值
    var hiddens = $('input[type=hidden]');
    $.each(hiddens, function (i, hidden) {
        if ($(hidden).attr("name").indexOf('TB_') == 0) {
            //formArrResult.push($(hidden).attr("name") + '=' + $(hidden).val());
        }
    });

    if (!isCotainTextArea) {
        formArrResult = $.grep(formArrResult, function (value) {
            return value.split('=').length == 2 ? value.split('=')[1].length <= 50 : true;
        });
    }

    formss = formArrResult.join('&');
    var dataArr = [];
    //加上URL中的参数
    if (pageData != undefined && isCotainUrlParam) {
        var pageDataArr = [];
        for (var data in pageData) {
            pageDataArr.push(data + '=' + pageData[data]);
        }
        dataArr.push(pageDataArr.join('&'));
    }
    if (formss != '')
        dataArr.push(formss);
    var formData = dataArr.join('&');
    return formData;
}
//发送
function Send() {
    //比填写检查
    //必填项和正则表达式检查
    var formCheckResult = true;
    if (!checkBlanks()) {
        formCheckResult = false;
    }
    if (!checkReg()) {
        formCheckResult = false;
    }
    if (!formCheckResult) {
        //alert("表单填写不正常，请检查！！！");
        return;
    }
    var toNode = 0;
    //含有发送节点 且接收
    if ($('#DDL_ToNode').length > 0) {
        var selectToNode = $('#DDL_ToNode  option:selected').data();
        if (selectToNode.IsSelectEmps == "1") {//跳到选择接收人窗口
            initModal("sendAccepter", selectToNode);
            $('#returnWorkModal').modal().show();
            return false;
        } else {
            toNode = selectToNode.No;
        }
    }

    //先设置按钮等不可用
    setToobarDisiable();

    //设置文本框等不可用
    //setFormEleDisabled();

    //设置附件为只读
    setAttachDisabled();
    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(true, true) + "&ToNode=" + toNode,
        url: "MyFlow.ashx?Method=Send",
        dataType: 'html',
        success: function (data) {
            //设置附件可上传
            setAttachAbled();
            if (data.indexOf('err@') == 0) {//发送时发生错误
                $('#Message').html(data.substring(4, data.length));
                $('.Message').show();
                setToobarEnable();
            }
            else if (data.indexOf('url@') == 0) {//发送成功时转到指定的URL 
                var url = data;
                url = url.replace('url@', '');
                window.location.href = url;
                // WinOpen(url, 'ss');
                // $('#Message').html("<a href=" + data.substring(4, data.length) + ">待处理</a>");
                // $('.Message').show();
            }
            else {
                OptSuc(data);
                //if (window.opener != null && window.opener != undefined && window.opener)
                //    $('#Message').html(data);
                //$('.Message').show();
                ////发送成功时
                //setAttachDisabled();
                //setToobarUnVisible();
                //setFormEleDisabled();
            }
        }
    });
}

$(function () {
    $('#btnMsgModalOK').bind('click', function () {
        $('.in').remove();
        window.close();
        if (opener != null && opener != undefined) {
            opener.window.focus();
        }
    });

    setAttachDisabled();
    setToobarDisiable();
    setFormEleDisabled();

    $('#btnMsgModalOK1').bind('click', function () {
        window.close();
        opener.window.focus();
    });

})

//发送 退回 移交等执行成功后转到  指定页面
function OptSuc(msg) {
    // window.location.href = "/WF/MyFlowInfo.aspx";
    $('.Message').hide();
    if ($('#returnWorkModal:hidden').length == 0 && $('#returnWorkModal').length > 0) {
        $('#returnWorkModal').modal().hide();
        $('.in').remove();
    }
    $("#msgModalContent").html(msg);
    $("#msgModal").modal().show();
}
//移交
//初始化发送节点下拉框
function InitToNodeDDL() {
    var workNode = JSON.parse(jsonStr);
    if (workNode.ToNodes != undefined && workNode.ToNodes.length > 0) {
        // $('[value=发送]').
        var toNodeDDL = $('<select style="width:auto;" id="DDL_ToNode"></select>');
        $.each(workNode.ToNodes, function (i, toNode) {
            //IsSelectEmps: "1"
            //Name: "节点2"
            //No: "702"
            var opt = $("<option value='" + toNode.No + "'>" + toNode.Name + "</option>");
            opt.data(toNode);
            toNodeDDL.append(opt);
        });


        $('[name=Send]').after(toNodeDDL);
    }
}

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function showNoticeInfo() {
    var workNode = JSON.parse(jsonStr);
    var rbs = workNode.Sys_FrmRB;
    data = rbs;
    $("input[type=radio],select").bind('change', function (obj) {
        var needShowDDLids = [];
        var methodVal = obj.target.value;
        
        for (var j = 0; j < data.length; j++) {
            var value = data[j].IntKey;
            var noticeInfo = data[j].Tip;
            var drdlColName = data[j].KeyOfEn;

            if (obj.target.tagName == "SELECT") {
                drdlColName = 'DDL_' + drdlColName;
            } else {
                drdlColName = 'RB_' + drdlColName;
            }
            //if (methodVal == value &&  obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
            if (methodVal == value && (obj.target.name == drdlColName)) {
                //高级JS设置;  设置表单字段的  可用 可见 不可用 
                var fieldConfig = data[j].FieldsCfg;
                var fieldConfigArr = fieldConfig.split('@');
                for (var k = 0; k < fieldConfigArr.length; k++) {
                    var fieldCon = fieldConfigArr[k];
                    if (fieldCon != '' && fieldCon.split('=').length == 2) {
                        var fieldConArr = fieldCon.split('=');
                        var ele = $('[name="DDL_' + fieldConArr[0] + '"],[name="TB_' + fieldConArr[0] + '"],[name="RB_' + fieldConArr[0] + '"],[name="CK_' + fieldConArr[0] + '"]');
                        if (ele.length == 0) {
                            continue;
                        }
                        var labDiv = undefined;
                        var eleDiv = undefined;
                        if (ele.css('display').toUpperCase() == "NONE" && !(ele.attr('class') != undefined && ele.attr('class').indexOf('tbAth') >= 0)) {//附件例外
                            continue;
                        }

                        if (ele.parent().attr('class').indexOf('input-group') >= 0) {//POP窗口
                            labDiv = ele.parent().parent().prev();
                            eleDiv = ele.parent().parent();
                        } else {
                            labDiv = ele.parent().prev();
                            eleDiv = ele.parent();
                        }

                        
                        switch (fieldConArr[1]) {
                            case "1"://可用
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                ele.removeAttr('disabled');

                                break;
                            case "2"://可见
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                break;
                            case "3"://不可见
                                labDiv.css('display', 'none');
                                eleDiv.css('display', 'none');
                                break;
                        }
                    }
                }
                //根据下拉列表的值选择弹出提示信息
                if (noticeInfo == undefined || noticeInfo.trim() == '') {
                    break;
                }
                noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')
                var selectText = '';
                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    selectText = obj.target.nextSibling.textContent;
                } else {//select
                    selectText = $(obj.target).find("option:selected").text();
                }
                $($('#div_NoticeInfo .popover-title span')[0]).text(selectText);
                $('#div_NoticeInfo .popover-content').html(noticeInfo);

                var top = obj.target.offsetHeight;
                var left = obj.target.offsetLeft;
                var current = obj.target.offsetParent;
                while (current !== null) {
                    left += current.offsetLeft;
                    top += current.offsetTop;
                    current = current.offsetParent;
                }


                if (obj.target.tagName.toUpperCase() == 'INPUT' && obj.target.type.toUpperCase() == 'RADIO') {//radio button
                    left = left - 40;
                    top = top + 10;
                }
                if (top - $('#div_NoticeInfo').height() - 30 < 0) {
                    //让提示框在下方展示
                    $('#div_NoticeInfo').removeClass('top');
                    $('#div_NoticeInfo').addClass('bottom');
                    top = top;
                } else {
                    $('#div_NoticeInfo').removeClass('bottom');
                    $('#div_NoticeInfo').addClass('top');
                    top = top - $('#div_NoticeInfo').height() - 30;
                }
                $('#div_NoticeInfo').css('top', top);
                $('#div_NoticeInfo').css('left', left);
                $('#div_NoticeInfo').css('display', 'block');
                //$("#btnNoticeInfo").popover('show');
                //$('#btnNoticeInfo').trigger('click');
                break;
            }
        }

        $.each(needShowDDLids, function (i, ddlId) {
            $('#' + ddlId).change();
        });

        //当分组下没有信心显示时，分组名称也不显示
        var groups = $('.group');
        $.each(groups, function (i, group) {
            if ($(group).find(".state").text() == "+") {
                var groupEles=$(group).parent().next().children();
                var resultDisplay = false;
                for (var m = 0; m < groupEles.length; m++) {
                    if ($(groupEles[m]).css('display')!="none") {
                        resultDisplay = true;
                        break;
                    }
                }
                if (resultDisplay) {
                    $(group).parent().show();
                } else {
                    $(group).parent().hide();
                }
            } else {
                if ($(group).parent().next().children(":visible").length == 0) {
                    $(group).parent().hide();
                } else {
                    $(group).parent().show();
                }
            }
        })
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInfo').css('display', 'none');
    })

    $("input[type=radio]:checked:visible,select:visible").change();

    //先处理不受别人控制的DDL,RB
    //var rbSelArr = $("input[type=radio]:checked,select");
    //for (var i = 0; i < rbSelArr.length; i++) {
    //    var rbSel=rbSelArr[i];
    //    var name = $(rbSel).attr('name');
    //    var keyofEn = '';
    //    var type='';
    //    if (rbSel.tagName == "SELECT") {
    //        type = 'DDL_';
    //        keyofEn = name.substr(4, name.length);
    //    } else {
    //        type = 'RB_';
    //        keyofEn = name.substr(3, name.length);
    //    }
    //    if ($.grep(rbs, function (value) {
    //         var fieldConfig = value.FieldsCfg;

    //      return fieldConfig.indexOf("@" + name + "=") >= 0;
    //    }).length == 0) {
    //        $(rbSel).change();
    //    }
    //}
    $('#span_CloseNoticeInfo').click();
}

//给出文本框输入提示信息
function showTbNoticeInfo() {
    var workNode = JSON.parse(jsonStr);
    var mapAttr = workNode.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {
            var workNode = JSON.parse(jsonStr);
            var mapAttr = workNode.Sys_MapAttr;

            mapAttr = $.grep(mapAttr, function (attr) {
                return 'TB_' + attr.KeyOfEn == obj.target.id;
            })
            var atParams = AtParaToJson(mapAttr[0].AtPara);
            var noticeInfo = atParams.Tip;

            if (noticeInfo == undefined || noticeInfo == '')
                return;

            //noticeInfo = noticeInfo.replace(/\\n/g, '<br/>')

            $($('#div_NoticeInfo .popover-title span')[0]).text(mapAttr[0].Name);
            $('#div_NoticeInfo .popover-content').html(noticeInfo);

            var top = obj.target.offsetHeight;
            var left = obj.target.offsetLeft;
            var current = obj.target.offsetParent;
            while (current !== null) {
                left += current.offsetLeft;
                top += current.offsetTop;
                current = current.offsetParent;
            }

            if (top - $('#div_NoticeInfo').height() - 30 < 0) {
                //让提示框在下方展示
                $('#div_NoticeInfo').removeClass('top');
                $('#div_NoticeInfo').addClass('bottom');
                top = top;
            } else {
                $('#div_NoticeInfo').removeClass('bottom');
                $('#div_NoticeInfo').addClass('top');
                top = top - $('#div_NoticeInfo').height() - 30;
            }
            $('#div_NoticeInfo').css('top', top);
            $('#div_NoticeInfo').css('left', left);
            $('#div_NoticeInfo').css('display', 'block');
        });
    })
}
//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput
    //var lbs = $('[class*=col-md-1] label:contains(*)');
    var lbs = $('.mustInput');
    $.each(lbs, function (i, obj) {
        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
            var keyofen = $(obj).data().keyofen
            var ele = $('[id$=_' + keyofen + ']');
            if (ele.length == 1) {
                switch (ele[0].tagName.toUpperCase()) {
                    case "INPUT":
                        if (ele.attr('type') == "text") {
                            if (ele.val() == "") {
                                checkBlankResult = false;
                                ele.addClass('errorInput');
                            } else {
                                ele.removeClass('errorInput');
                            }
                        }
                        break;
                    case "SELECT":
                        if (ele.val() == "" || ele.children('option:checked').text() == "*请选择" || ele.val()==null) {
                            checkBlankResult = false;
                            ele.addClass('errorInput');
                        } else {
                            ele.removeClass('errorInput');
                        }
                        break;
                    case "TEXTAREA":
                        if (ele.val() == "") {
                            checkBlankResult = false;
                            ele.addClass('errorInput');
                        } else {
                            ele.removeClass('errorInput');
                        }
                        break;
                }
            }
        }
    });


    return checkBlankResult;
}

//正则表达式检查
function checkReg() {
    var checkRegResult = true;
    var regInputs = $('.CheckRegInput');
    $.each(regInputs, function (i, obj) {
        var name = obj.name;
        var mapExtData = $(obj).data();
        if (mapExtData.Doc != undefined) {
            var regDoc = mapExtData.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}').replace(/，/g, ',');
            var tag1 = mapExtData.Tag1;
            if ($(obj).val() != undefined && $(obj).val() != '') {

                var result = CheckRegInput(name, regDoc, tag1);
                if (!result) {
                    $(obj).addClass('errorInput');
                    checkRegResult = false;
                } else {
                    $(obj).removeClass('errorInput');
                }
            }
        }
    });


    return checkRegResult;
}

function SaveDtlAll() {
    return true;
}

//历史的导航  横向的圆球状
function SetHisIframe(obj) {
    $('#hisIframe').attr('src', obj.Href);
}

//设置历史表单的高度
function InitLoadFrame(obj) {
    var height = $(frames[obj.target.name].window.document.getElementsByTagName('BODY')).height();


    $(obj.target).height(height + 10);
}
var colVisibleJsonStr = ''
var jsonStr = '';

