$(function () {
    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }

});

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
    var url = 'Do.aspx?DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
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

    var url = 'WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    //var url = 'WebOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal;
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
                var onclickFun = $('[name=Delete]').attr('onclick');
                if (onclickFun != undefined) {
                    $('[name=Delete]').attr('onclick', onclickFun.replace('MyFlowInfo.htm', 'MyFlowInfo.aspx'));
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
                modalIframeSrc = "../WF/WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random()
                break;
            case "shift":
                $('#modalHeader').text("工作移交");
                modalIframeSrc = "../WF/WorkOpt/forward.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&Info=&s=" + Math.random()
                break;
            case "askfor":
                $('#modalHeader').text("工作移交");
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
        url: "MyFlow.ashx?DoType=Save",
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
                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>" + '</div>';
            }
            break;
        case "Dtl":
            //WF/CCForm/Dtl.aspx?EnsName=ND501Dtl1&RefPKVal=0&PageIdx=1
            var src = "/WF/CCForm/Dtl.aspx?s=2&EnsName=" + groupFiled.CtrlID + "&RefPKVal=" + pageData.WorkID + "&PageIdx=1";
            src += "&r=q" + paras;
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
                paras += "&DoType=View";
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
                paras += "&DoType=View";
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
    var filterTrackList = $.grep(trackList, function (value) {
        return value.ActionType == 28 || value.ActionType == 27 || value.ActionType == 26 || value.ActionType == 11 || value.ActionType == 10 || value.ActionType == 9 || value.ActionType == 7 || value.ActionType == 6 || value.ActionType == 2 || value.ActionType == 1 || value.ActionType == 8 || value.ActionType == 5;
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
        if (actionType != 1 && actionType != 6 && actionType != 7 && actionType != 11 && actionType != 8) {
            switch (actionType) {
                case 5:
                    trackHtml += '<div class="trackDiv"><i style="display:none;"></i>' + '<div class="returnTackHeader" id="track' + i + '" ><b>' + (i + 1) + '</b><span>' + "撤销发送信息" + '</span></div>' + "<div class='returnTackDiv' >" + track.NDToT + "撤消节点发送;时间" + track.RDT + '</div></div>';
                    break;
                case 2:
                    trackHtml += '<div class="trackDiv"><i style="display:none;"></i>' + '<div class="returnTackHeader" id="track' + i + '" ><b>' + (i + 1) + '</b><span>' + track.ActionTypeText + '信息</span></div>' + "<div class='returnTackDiv' >" + track.EmpFromT + "把工单从节点：（" + track.NDFromT + "）" + track.ActionTypeText + "至：(" + track.EmpToT + "," + track.NDToT + "):" + track.RDT + "</br>" + track.ActionTypeText + "信息：" + track.Msg + '</div></div>';
                    break;
                default:
                    break;
            }
        } else {
            var trackSrc = "/WF/WorkOpt/ViewWorkNodeFrm.htm?WorkID=" + track.WorkID + "&FID=" + track.FID + "&FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + track.NDFrom + "&DoType=View&MyPK=" + track.MyPK + '&IframeId=track' + i;
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
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                + " " + date.getHours() + seperator2 + date.getMinutes()
                + seperator2 + date.getSeconds();
        return {
            currentdate: currentdate,
            getDay: date.getFullYear() + seperator1 + month + seperator1 + strDate,
            getTime: date.getHours() + seperator2 + date.getMinutes()
                + seperator2 + date.getSeconds()
        };
    }
    var sendName = $.cookie("CCS").split("=")[2].split("&")[0];
    var sendNo = $.cookie("CCS").split("=")[1].split("&")[0];
    var sendt = HgetNowFormatDate().currentdate;
    if (pageData.DoType != 'View') {
        trackNavHtml += '<li  class="scrollNav"><a href="#divCurrentForm"><div>' + (workNodeData.Track.length + 1) + '</div>' + workNodeData.Sys_MapData[0].Name + '<p>发送人:' + sendName + '</p><p>时间:' + sendt + '</p></a></li>';
        $('#header b').text((workNodeData.Track.length + 1));
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
    if (workNodeData.Track.length > 0 && (workNodeData.Track[workNodeData.Track.length - 1].NDFrom == pageData.FK_Node && workNodeData.Track[workNodeData.Track.length - 1].EmpFrom == sendNo) && (workNodeData.Track[workNodeData.Track.length - 1].ActionType != 5) && pageData.DoType != 'View') {//ACTIONTYPE=5 是撤销移交
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
    
    //开始解析表单字段
    var mapAttrsHtml = InitMapAttr(workNodeData.Sys_MapAttr, workNodeData);
    $('#divCCForm').html(mapAttrsHtml);

    //设置位置和大小
    $.each(workNodeData.Sys_MapAttr, function (i, obj) {
        var ele = $('[name$=' + obj.KeyOfEn + ']');
        if (ele.length == 1) {
            $(ele).css('left', obj.X);
            $(ele).css('top', obj.Y);
            $(ele).css('position', 'absolute');
            $(ele).css('width', obj.UIWidth);
            $(ele).css('width', obj.UIHeight);
        }
    })

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

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

    ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题
    var enName = workNodeData.Sys_MapData[0].No;
    try {
        ////加载JS文件
        //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
        //$('body').append($('<div>' + jsSrc + '</div>'));

        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "/DataUser/JSLibData/" + enName + "_Self.js";
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
        s.src = "/DataUser/JSLibData/" + enName + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }



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
        if (ath.length > 0) {
            ath = ath[0];
            var src = "";
            if (pageData.IsReadonly)
                src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
            else
                src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;
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


    showNoticeInfo();

    showTbNoticeInfo();
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


//解析表单字段 MapAttr
function InitMapAttr(mapAttrData, workNodeData) {
    var resultHtml = '';

    var hiddenHtml = '';
    for (var j = 0; j < mapAttrData.length; j++) {
        var mapAttr = mapAttrData[j];
        if (mapAttr.UIVisible) {//是否显示
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
                            eleHtml += 
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" +
                                (workNodeData, mapAttr, defValue) + "</select>";
                        } else {//文本区域
                            if (mapAttr.UIHeight <= 23) {
                                eleHtml +=
                                    "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                    ;
                            }
                            else {
                                eleHtml +=
                                    "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + uiHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                ;
                            }
                        }
                    } else if (mapAttr.ColSpan == "4" || (mapAttr.ColSpan == "3" && mapAttr.UIHeight > 23)) {//大文本区域  且占一整行
                        var uiHeight = mapAttr.UIHeight / 23 * 30;
                        isInOneRow = true;
                        eleHtml +=
                            "<textarea  style='height:" + uiHeight + "px' maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "'" + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" + "</textarea>"
                            ;
                    }
                } //AppDate
                else if (mapAttr.MyDataType == 6) {//AppDate
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" ;
                }
                else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                        //enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml +=  "<input maxlength=" + mapAttr.MaxLen / 2 + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
                }
                else if (mapAttr.MyDataType == 4) {// AppBoolean = 7
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
                    eleHtml +=  "<div><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_"+mapAttr.KeyOfEn+"' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>" ;
                    eleHtml += '<label class="labRb" for="CB_'+mapAttr.KeyOfEn+'">'+mapAttr.Name+'</label></div>';
                    return eleHtml;
                }

                if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                    var colMd = 2;
                    var colsm = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        colMd = 11;
                        colsm = 10;
                    }
                    if (mapAttr.UIContralType == 1) {//DDL
                        eleHtml += 
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                        //eleHtml += "</div>";
                    }

                    if (mapAttr.UIContralType == 3) {
                        //RadioBtn
                        var operations = '';

                        if (mapAttr.ColSpan == 1 || mapAttr.ColSpan >= 3) {
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
                    eleHtml += "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" ;
                }
                //AppMoney  AppRate
                if (mapAttr.MyDataType == 8) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml +=  "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" ;
                }

                if (mapAttr.LGType == 2) {
                    var mdCol = 2;
                    var smCol = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        mdCol = 4;
                        smCol = 8;
                    }

                    eleHtml += 
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
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

                    eleHtml += "<input type='hidden' class='tbAth' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
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
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');
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
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');
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
                tbAuto.attr("onkeyup", "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + mapExt.MyPK + "\');");
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
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
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
function GenerWorkNode1() {
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        //url: "MyFlow.ashx?DoType=GenerWorkNode&DoType=" + pageData.DoType + "&m=" + Math.random(),
        url: "MyFlow.ashx?DoType=GenerWorkNode" + "&m=" + Math.random(),
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
            ////显示父流程 链接
            //if (gengerWorkNode.WF_GenerWorkFlow != null && gengerWorkNode.WF_GenerWorkFlow.length > 0 && (gengerWorkNode.WF_GenerWorkFlow[0].PWorkID != 0 || gengerWorkNode.WF_GenerWorkFlow[0].PWorkID2 != 0)) {
            //    $('#btnShowPFlow').bind('click', function () {
            //        var pworkid = 1;
            //        var pfk_node = 1;
            //        var pfk_flow = 1;
            //        if (gengerWorkNode.WF_GenerWorkFlow[0].PWorkID != 0) {
            //            pworkid = gengerWorkNode.WF_GenerWorkFlow[0].PWorkID;
            //            pfk_flow = gengerWorkNode.WF_GenerWorkFlow[0].PFlowNo;
            //            pfk_node = gengerWorkNode.WF_GenerWorkFlow[0].PNodeID;
            //        } else {
            //            pworkid = gengerWorkNode.WF_GenerWorkFlow[0].PWorkID2;
            //            pfk_flow = gengerWorkNode.WF_GenerWorkFlow[0].PFlowNo2;
            //            pfk_node = gengerWorkNode.WF_GenerWorkFlow[0].PNodeID2;
            //        }

            //        window.open("WorkOpt/FoolFrmTrack.htm?FK_Flow=" + pfk_flow + "&WorkID=" + pworkid + "&FK_Node=" + pfk_node);

            //    });

            //    $('#ShowPFlow').css('display', 'none');
            //} else {
            //    $('#ShowPFlow').css('display', 'none');
            //}

            ////如果为查看页面，只显示历史轨迹
            //initTrackList(gengerWorkNode);
            //if (pageData.DoType == 'View') {
            //    $('#divCurrentForm').css('display', 'none');
            //    return;
            //}
            ////是分流或者分合流  且是 退回状态 转到页面 WF\WorkOpt\DealSubThreadReturnToHL.html
            //if ((gengerWorkNode.WF_Node[0].RunModel == 2 || gengerWorkNode.WF_Node[0].RunModel == 3) && gengerWorkNode.WF_GenerWorkFlow[0].WFState == 5) {
            //    $('#')
            //    var iframeHtml = "<iframe style='width:100%;' src='./WorkOpt/DealSubThreadReturnToHL.html?FK_Flow=" + pageData.FK_Flow + "&FK_Node=" + pageData.FK_Node + "&WorkID=" + pageData.WorkID + "&FID=" + pageData.FID + "'></iframe>";
            //    $('#topContentDiv').html(iframeHtml);
            //    $('#header span').html("处理退回信息");
            //    return;
            //}
            ////加签回复
            //if (gengerWorkNode.WF_GenerWorkFlow[0].WFState == 11) {

            //}

            //解析表单
            InitForm();
            ///根据下拉框选定的值，绑定表单中哪个元素显示，哪些元素不显示
            //showEleDependOnDRDLValue();
            //
            InitToNodeDDL();
            
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

    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(true, true) + "&ToNode=" + toNode,
        url: "MyFlow.ashx?DoType=Send",
        dataType: 'html',
        success: function (data) {
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
                if (opener != null && opener.window != null && opener.window.parent != null && opener.window.parent.refSubSubFlowIframe != null && typeof (opener.window.parent.refSubSubFlowIframe) == "function") {
                    opener.window.parent.refSubSubFlowIframe();
                }
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
        $('#returnWorkModal').modal().hide()
    }
    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')

    $("#msgModalContent").html(msg.replace(/@/g, '<br/>'));
    var trackA = $('#msgModalContent a:contains("工作轨迹")');
    var trackImg = $('#msgModalContent img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();

    //如果是申请页面
    if ($('.navbars').css('display') == "none") {
        $("#msgModalContent").append("<a href='/ITILFlow/MainPage.html'>返回流程工作台</a>");
        $('#CCForm').html($('#msgModalContent').html());
        setToobarUnVisible();
    } else {
        $("#msgModal").modal().show();
        if (window.opener != null) {
            //刷新父窗口
            window.opener.location.reload();
        }
    }
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
                        var ele = $('[name$=' + fieldConArr[0] + ']');
                        if (ele.length == 0) {
                            continue;
                        }
                        var labDiv = undefined;
                        var eleDiv = undefined;
                        if (ele.css('display').toUpperCase() == "NONE") {
                            continue;
                        }

                        if (ele.parent().attr('class').indexOf('input-group') >= 0) {
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
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInfo').css('display', 'none');
    })

    $("input[type=radio]:checked,select").change();
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
                        if (ele.val() == "" || ele.children('option:checked').text() == "*请选择") {
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

//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单
function GenerWorkNode() {
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        //url: "MyFlow.ashx?DoType=GenerWorkNode&DoType=" + pageData.DoType + "&m=" + Math.random(),
        url: "MyFlow.ashx?DoType=GenerWorkNode" + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            jsonStr = data;
            var gengerWorkNode = {};
            var flow_Data;
            try {
                flow_Data = JSON.parse(data);
                workNodeData = flow_Data;
            }
            catch (err) {
                alert("GenerWorkNode转换JSON失败:" + jsonStr);
                return;
            }

            $('#CCForm').html('');
                //循环MapAttr
            for (var mapAtrrIndex in flow_Data.Sys_MapAttr) {
                var mapAttr = flow_Data.Sys_MapAttr[mapAtrrIndex];
                var eleHtml = figure_MapAttr_Template(mapAttr);
                $('#CCForm').append(eleHtml);
            }
                //循环FrmLab
                for (var i in flow_Data.Sys_FrmLab) {
                    var frmLab = flow_Data.Sys_FrmLab[i];
                    var label = figure_Template_Label(frmLab);
                    $('#CCForm').append(label);
                }
                //循环FrmRB
                for (var i in flow_Data.Sys_FrmRB) {
                    var frmLab = flow_Data.Sys_FrmRB[i];
                    var label = figure_Template_Rb(frmLab);
                    $('#CCForm').append(label);
                }

                //循环FrmBtn
                for (var i in flow_Data.Sys_FrmBtn) {
                    var frmBtn = flow_Data.Sys_FrmBtn[i];
                    var btn = figure_Template_Btn(frmBtn);
                    $('#CCForm').append(btn);
                }

                //循环Image
                for (var i in flow_Data.Sys_FrmImg) {
                    var frmImg = flow_Data.Sys_FrmImg[i];
                    var createdFigure = figure_Template_Image(frmImg);
                    $('#CCForm').append(createdFigure);
                }

                //循环 Link
                for (var i in flow_Data.Sys_FrmLink) {
                    var frmLink = flow_Data.Sys_FrmLink[i];
                    var createdFigure = figure_Template_HyperLink(frmLink);
                    $('#CCForm').append(createdFigure);
                }

                //循环 图片附件
                for (var i in flow_Data.Sys_FrmImgAth) {
                    var frmImgAth = flow_Data.Sys_FrmImgAth[i];
                    var createdFigure = figure_Template_ImageAth(frmImgAth);
                    $('#CCForm').append(createdFigure);
                }
                //循环 附件
                for (var i in flow_Data.Sys_FrmAttachment) {
                    var frmAttachment = flow_Data.Sys_FrmAttachment[i];
                    var createdFigure = figure_Template_Attachment(frmAttachment);
                    $('#CCForm').append(createdFigure);
                }

                //循环 从表
                for (var i in flow_Data.Sys_MapDtl) {
                    var frmMapDtl = flow_Data.Sys_MapDtl[i];
                    var createdFigure = figure_Template_Dtl(frmMapDtl);
                    $('#CCForm').append(createdFigure);
                }

                //循环线
                for (var i in flow_Data.Sys_FrmLine) {
                    var frmLine = flow_Data.Sys_FrmLine[i];
                    var createdConnector = connector_Template_Line(frmLine);
                    $('#CCForm').append(createdConnector);
                }

                //循环组件 轨迹图 审核组件 子流程 子线程
                for (var i in flow_Data.FigureCom) {
                    var figureCom = flow_Data.FigureCom[i];
                    var createdFigure = figure_Template_FigureCom(figureCom);
                    if (createdFigure != undefined) {
                        STACK.figureAdd(createdFigure);
                    }
                }




            //原有的
            
            //为 DISABLED 的 TEXTAREA 加TITLE 
                var disabledTextAreas = $('#divCCForm textarea:disabled');
                $.each(disabledTextAreas, function (i, obj) {
                    $(obj).attr('title', $(obj).val());
                })

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

            ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题
                var enName = workNodeData.Sys_MapData[0].No;
            try {
                ////加载JS文件
                //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
                //$('body').append($('<div>' + jsSrc + '</div>'));

                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "/DataUser/JSLibData/" + enName + "_Self.js";
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
                s.src = "/DataUser/JSLibData/" + enName + "_Self.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }



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
                    if (ath.length > 0) {
                        ath = ath[0];
                        var src = "";
                        if (pageData.IsReadonly)
                            src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                        else
                            src = "/WF/CCForm/AttachmentUpload.aspx?IsExtend=1&PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;
                        $('#iframeAthForm').attr('src', src);
                        atParamObj["tbId"] = tbId;
                        atParamObj["divId"] = divId;
                        $('#iframeAthForm').data(atParamObj);
                        $('#athModal .modal-title').text("上传附件：" + $(obj.target).parent().prev().children('label').text());
                        $('#athModal').modal().show();
                    }
                });
            


                showNoticeInfo();

                showTbNoticeInfo();
            
        }
    })
}

var workNodeData = {};
//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_Template( mapAttr) {
    var eleHtml = '';
    if (mapAttr.UIVisible) {//是否显示

        var str = '';
        var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);

        var isInOneRow = false;//是否占一整行
        var islabelIsInEle = false;//

        eleHtml += '';

        if (mapAttr.UIContralType != 6) {
            //添加文本框 ，日期控件等
            //AppString   
            if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                if (mapAttr.UIContralType == "1") {//DDL 下拉列表框
                    eleHtml +=
                        "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" +
                        (workNodeData, mapAttr, defValue) + "</select>";
                } else {//文本区域
                    if (mapAttr.UIHeight <= 23) {
                        eleHtml +=
                            "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                        ;
                    }
                    else {
                        eleHtml +=
                            "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                        ;
                    }
                }
            } //AppDate
            else if (mapAttr.MyDataType == 6) {//AppDate
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {
                    enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
            }
            else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {
                    enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                    //enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += "<input maxlength=" + mapAttr.MaxLen / 2 + "  type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' />";
            }
            else if (mapAttr.MyDataType == 4) {// AppBoolean = 7
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
                eleHtml += "<div><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>";
                eleHtml += '<label class="labRb" for="CB_' + mapAttr.KeyOfEn + '">' + mapAttr.Name + '</label></div>';
                //return eleHtml;
            }

            if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                if (mapAttr.UIContralType == 1) {//DDL
                    eleHtml +=
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
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
                eleHtml += "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
            }
            //AppMoney  AppRate
            if (mapAttr.MyDataType == 8) {
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {

                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += "<input maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
            }

            if (mapAttr.LGType == 2) {
                eleHtml +="<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
            }
        } else {
            //展示附件信息  FREE 不需要
            return;
            var atParamObj = AtParaToJson(mapAttr.AtPara);
            if (atParamObj.AthRefObj != undefined) {//扩展设置为附件展示
                eleHtml += "<input type='hidden' class='tbAth' data-target='" + mapAttr.AtPara + "' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' >" + "</input>";
                defValue = defValue != undefined && defValue != '' ? defValue : '&nbsp;';
                if (defValue.indexOf('@AthCount=') == 0) {
                    defValue = "附件" + "<span class='badge'>" + defValue.substring('@AthCount='.length, defValue.length) + "</span>个";
                } else {
                    defValue = defValue;
                }
                eleHtml += "<div class='divAth' data-target='" + mapAttr.KeyOfEn + "'  id='DIV_" + mapAttr.KeyOfEn + "'>" + defValue + "</div>";
            }
        }

        if (!islabelIsInEle) {
            //eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
            //(mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
            //+ "</div>" + eleHtml;
            //先把 必填项的 * 写到元素后面 可能写到标签后面更合适
            eleHtml +=
           mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "";
        }
    } else {
        var value = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (value == undefined) {
            value = '';
        } else {
            //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
        }

        //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
        eleHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
    }
    eleHtml=$(eleHtml);
    eleHtml.css('width', mapAttr.UIWidth).css('height', mapAttr.UIHeight).css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);
    return eleHtml;
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var eleHtml = '';
    eleHtml = '<label></label>'
    eleHtml = $(eleHtml);
    eleHtml.html(frmLab.Text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y).css('left', frmLab.X).css('font-size', frmLab.FontSize)
        .css('padding-top','5px');
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = '';
    eleHtml = '<input type="button" value="">';
    eleHtml = $(eleHtml);
    eleHtml.attr("Value",frmBtn.Text);
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y).css('left', frmBtn.X);
    return eleHtml;
}

//初始化单选按钮
function figure_Template_Rb(frmRb) {
    var eleHtml = '<div></div>';
    eleHtml = $(eleHtml);
    var childRbEle = $('<input id="RB_ChuLiFangShi2" type="radio"/>');
    var childLabEle = $('<label class="labRb"></label>');
    childLabEle.html(frmRb.Lab).attr('for', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    
    childRbEle.val(frmRb.IntKey).attr('id', 'RB_' + frmRb.KeyOfEn + frmRb.IntKey).attr('name', 'RB_' + frmRb.KeyOfEn);
    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y).css('left', frmRb.X);

    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    var eleHtml = '<a></a>';
    eleHtml = $(eleHtml);
    eleHtml.html(frmLin.Text).attr('href', frmLin.URL).attr("_target",frmLin.target);
    eleHtml.css('position', 'absolute').css('top', frmLin.Y).css('left', frmLin.X).css('color', frmLin.FontColr).css('fontsize', frmLin.FontSize);
    return eleHtml;
}

//初始化 IMAGE
function figure_Template_Image(frmImage) {
    var eleHtml = '';
    eleHtml = '<image/>';
    eleHtml = $(eleHtml);
    eleHtml.attr("src", frmImage.ImgPath).attr('href', frmImage.LinkURL).attr('_target', frmImage.LinkTarget);
    eleHtml.css('position', 'absolute').css('top', frmImage.Y).css('left', frmImage.X).css('width', frmImage.W).css('height', frmImage.H);
    return eleHtml;
}

//初始化 IMAGE
function figure_Template_ImageAth(frmImageAth) {
    return "";
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    return "";
}

function connector_Template_Line(frmLine) {
    var eleHtml = '';
    eleHtml = '<table><tr><td></td></tr></table>';
    eleHtml = $(eleHtml).css('position', 'absolute').css('top', frmLine.Y1).css('left', frmLine.X1);
    eleHtml.find('td').css('padding', '0px')
        //css('top',parseFloat(frmLine.Y1)>parseFloat( frmLine.Y2)?frmLine.Y2:frmLine.Y1).
        //css('left', parseFloat(frmLine.X1) > parseFloat(frmLine.X2 )? frmLine.X2 : frmLine.X1).
        .css('width', Math.abs(frmLine.X1 - frmLine.X2) == 0 ? frmLine.BorderWidth : Math.abs(frmLine.X1 - frmLine.X2))
    .css('height', Math.abs(frmLine.Y1 - frmLine.Y2) == 0 ? frmLine.BorderWidth : Math.abs(frmLine.Y1 - frmLine.Y2))
        .css("background", frmLine.BorderColor);

    return eleHtml;
}

//初始化从表
function figure_Template_Dtl(frmDtl) {
    return "";
}

//初始化轨迹图 审核组件 子流程 子线程
function figure_Template_FigureCom(figureCom) {
    return "";
}

var colVisibleJsonStr = ''
var jsonStr = '"{"Sys_GroupField":[{"OID":4696,"Lab":"表单信息","EnName":"ND7501","Idx":1,"GUID":"","CtrlType":"","CtrlID":"","AtPara":""}],"Sys_Enum":[{"MyPK":"IsAppend_CH_0","Lab":"否","EnumKey":"IsAppend","IntKey":0,"Lang":"CH"},{"MyPK":"IsAppend_CH_1","Lab":"是","EnumKey":"IsAppend","IntKey":1,"Lang":"CH"}],"Sys_MapData":[{"No":"ND7501","Name":"版本单提交","FrmW":900,"FrmH":1200,"TableWidth":800,"TableHeight":900,"TableCol":4}],"Sys_MapAttr":[{"MyPK":"ND7501_Title","FK_MapData":"ND7501","KeyOfEn":"Title","Name":"标题","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":251.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":1,"UIIsLine":1,"UIIsInput":0,"Idx":-1,"IsSigan":0,"X":171.2,"Y":68.4,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_BillNo","FK_MapData":"ND7501","KeyOfEn":"BillNo","Name":"申请单号","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100.0,"UIHeight":44.6,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":1,"IsSigan":0,"X":557.87,"Y":52.21,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsRichText=0@Tip=@IsSupperText=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_SystemName","FK_MapData":"ND7501","KeyOfEn":"SystemName","Name":"系统名称","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":157.61,"UIHeight":51.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":3,"IsSigan":0,"X":715.49,"Y":133.82,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsRichText=0@Tip=@IsSupperText=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_VesionState","FK_MapData":"ND7501","KeyOfEn":"VesionState","Name":"发布单状态","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":273.62,"UIHeight":75.81,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":6,"IsSigan":0,"X":117.01,"Y":461.86,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsRichText=0@Tip=@IsSupperText=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_IsAotoMate","FK_MapData":"ND7501","KeyOfEn":"IsAotoMate","Name":"是否自动化","DefVal":"0","UIContralType":1,"MyDataType":2,"LGType":1,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"IsAppend","UIRefKey":"NO","UIRefKeyText":"NAME","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":8,"IsSigan":0,"X":521.87,"Y":127.42,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsEnableJS=0@RBShowModel=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_DispatchDate","FK_MapData":"ND7501","KeyOfEn":"DispatchDate","Name":"调度会时间","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":11,"IsSigan":0,"X":520.27,"Y":174.62,"GUID":"","Tag":"","EditType":0,"AtPara":"@Tip=@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_Summary","FK_MapData":"ND7501","KeyOfEn":"Summary","Name":"上板摘要","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":250.0,"UIHeight":46.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":14,"IsSigan":0,"X":432.26,"Y":235.43,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsRichText=0@Tip=@IsSupperText=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_Reason","FK_MapData":"ND7501","KeyOfEn":"Reason","Name":"上版原因","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":500.0,"UIHeight":46.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"Idx":16,"IsSigan":0,"X":108.21,"Y":357.05,"GUID":"","Tag":"","EditType":0,"AtPara":"@IsRichText=0@Tip=@IsSupperText=0@FontSize=12","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_Rec","FK_MapData":"ND7501","KeyOfEn":"Rec","Name":"发起人","DefVal":"@WebUser.No","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_CDT","FK_MapData":"ND7501","KeyOfEn":"CDT","Name":"发起时间","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"1","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_Emps","FK_MapData":"ND7501","KeyOfEn":"Emps","Name":"Emps","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_FID","FK_MapData":"ND7501","KeyOfEn":"FID","Name":"FID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_FK_Dept","FK_MapData":"ND7501","KeyOfEn":"FK_Dept","Name":"操作员部门","DefVal":"","UIContralType":1,"MyDataType":1,"LGType":2,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"Port_Dept","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_FK_NY","FK_MapData":"ND7501","KeyOfEn":"FK_NY","Name":"年月","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_MyNum","FK_MapData":"ND7501","KeyOfEn":"MyNum","Name":"个数","DefVal":"1","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_OID","FK_MapData":"ND7501","KeyOfEn":"OID","Name":"OID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"","EditType":2,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null},{"MyPK":"ND7501_RDT","FK_MapData":"ND7501","KeyOfEn":"RDT","Name":"更新时间","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100.0,"UIHeight":23.0,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"Idx":999,"IsSigan":0,"X":5.0,"Y":5.0,"GUID":"","Tag":"1","EditType":1,"AtPara":"","ExtDefVal":null,"ExtDefValText":null,"MinLen":null,"MaxLen":null,"ExtRows":null,"IsRichText":null,"IsSupperText":null,"Tip":null,"ColSpan":null,"ColSpanText":null,"GroupID":"4696","GroupIDText":null}],"Sys_MapExt":[],"Sys_FrmLine":[{"MyPK":"LE_1_ND7501","FK_MapData":"ND7501","X1":81.55,"X2":718.82,"Y1":80.0,"Y2":80.0,"BorderColor":"Black","BorderWidth":2.0},{"MyPK":"LE_2_ND7501","FK_MapData":"ND7501","X1":81.82,"X2":81.82,"Y1":40.0,"Y2":480.91,"BorderColor":"Black","BorderWidth":2.0},{"MyPK":"LE_3_ND7501","FK_MapData":"ND7501","X1":81.82,"X2":720.0,"Y1":481.82,"Y2":481.82,"BorderColor":"Black","BorderWidth":2.0},{"MyPK":"LE_4_ND7501","FK_MapData":"ND7501","X1":83.36,"X2":717.91,"Y1":40.91,"Y2":40.91,"BorderColor":"Black","BorderWidth":2.0},{"MyPK":"LE_5_ND7501","FK_MapData":"ND7501","X1":83.36,"X2":717.91,"Y1":120.91,"Y2":120.91,"BorderColor":"Black","BorderWidth":2.0},{"MyPK":"LE_6_ND7501","FK_MapData":"ND7501","X1":719.09,"X2":719.09,"Y1":40.0,"Y2":482.73,"BorderColor":"Black","BorderWidth":2.0}],"Sys_FrmLink":[],"Sys_FrmBtn":[],"Sys_FrmImg":[{"MyPK":"Img_1_ND7501","FK_MapData":"ND7501","ImgAppType":0,"X":579.26,"Y":-2.55,"H":40.0,"W":137.0,"ImgURL":"/ccform;component/Img/LogoBig.png","ImgPath":"/CCFormDesigner;component/Img/Logo/CCFlow/LogoBig.png","LinkURL":"http://ccflow.org","LinkTarget":"_blank","GUID":"","Tag0":"","SrcType":0,"IsEdit":1,"Name":"","EnPK":"","ImgSrcType":null}],"Sys_FrmLab":[{"MyPK":"LB_1_ND7501","FK_MapData":"ND7501","Text":"填写xxxx申请单","X":688.55,"Y":258.7,"FontColor":"#FF000000","FontName":"Portable User Interface","FontSize":23,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"LB_2_ND7501","FK_MapData":"ND7501","Text":"Made&nbsp;by&nbsp;ccform","X":605.0,"Y":490.0,"FontColor":"#FF000000","FontName":"Portable User Interface","FontSize":14,"FontStyle":"Normal","FontWeight":"","IsBold":0,"IsItalic":0}],"Sys_FrmRB":[],"Sys_FrmEle":[],"Sys_MapFrame":[],"Sys_FrmAttachment":[],"Sys_FrmImgAth":[],"Sys_MapDtl":[],"MainTable":[{"Title":"系统管理室-zhhujie,胡捷在2017-04-16 09:20发起.","BillNo":"","SystemName":"","VesionState":"","IsAotoMate":0,"IsAotoMateText":"否","DispatchDate":"2017-04-11 23:29","Summary":"","Reason":"","Rec":"zhhujie","CDT":"2017-04-16 09:20","Emps":"zhhujie","FID":0,"FK_Dept":"0001.02.000000001091","FK_DeptText":"系统管理室","FK_NY":"2017-04","MyNum":1,"OID":37533,"RDT":"2017-04-16 09:20"}],"AlertMsg":[],"WF_GenerWorkFlow":[],"WF_Node":[{"NodeID":7501,"Name":"版本单提交","Step":1,"FK_Flow":"075","CheckNodes":null,"DeliveryWay":14,"FWCSta":1,"FWCShowModel":1,"FWCType":0,"FWCNodeName":"","FWCAth":0,"FWCTrackEnable":1,"FWCListEnable":1,"FWCIsShowAllStep":0,"FWCOpLabel":"审核","FWCDefInfo":"同意","SigantureEnabel":0,"FWCIsFullInfo":1,"FWC_X":22.6,"FWC_Y":17.0,"FWC_H":300.0,"FWC_W":400.0,"FWCFields":"","Tip":"","IsExpSender":0,"DeliveryParas":"","WhoExeIt":0,"TurnToDeal":0,"TurnToDealDoc":"","ReadReceipts":0,"CondModel":0,"CancelRole":0,"BatchRole":0,"BatchListCount":12,"BatchParas":"","IsTask":1,"IsRM":1,"DTFrom":"2016-08-03 13:06","DTTo":"2016-08-03 13:06","IsBUnit":0,"FormType":1,"NodeFrmID":"","FormUrl":"http://","FocusField":"上版原因:@Reason","SaveModel":0,"RunModel":0,"SubThreadType":0,"PassRate":100.0,"SubFlowStartWay":0,"SubFlowStartParas":"","TodolistModel":0,"BlockModel":0,"BlockExp":"","BlockAlert":"","IsAllowRepeatEmps":0,"IsGuestNode":0,"AutoJumpRole0":0,"AutoJumpRole1":0,"AutoJumpRole2":0,"WhenNoWorker":0,"SendLab":"发送","SendJS":"","SaveLab":"保存","SaveEnable":1,"ThreadLab":"子线程","ThreadEnable":0,"ThreadKillRole":0,"SubFlowLab":"子流程","SubFlowCtrlRole":0,"JumpWayLab":"跳转","JumpWay":0,"JumpToNodes":"","ReturnLab":"退回","ReturnRole":0,"IsBackTracking":0,"ReturnField":"","CCLab":"抄送","CCRole":0,"CCWriteTo":0,"ShiftLab":"移交","ShiftEnable":0,"DelLab":"删除","DelEnable":0,"EndFlowLab":"结束流程","EndFlowEnable":0,"PrintDocLab":"打印单据","PrintDocEnable":0,"TrackLab":"轨迹","TrackEnable":0,"HungLab":"挂起","HungEnable":0,"SelectAccepterLab":"接受人","SelectAccepterEnable":0,"SearchLab":"查询","SearchEnable":0,"WorkCheckLab":"审核","WorkCheckEnable":0,"BatchLab":"批处理","BatchEnable":0,"AskforLab":"加签","AskforEnable":0,"TCLab":"流转自定义","TCEnable":0,"WebOffice":"公文","WebOfficeEnable":0,"PRILab":"重要性","PRIEnable":0,"CHLab":"节点时限","CHEnable":0,"FocusLab":"关注","FocusEnable":0,"TSpanDay":0.0,"TSpanHour":8.0,"WarningDay":0.0,"WarningHour":4.0,"TCent":2.0,"CHWay":0,"IsEval":0,"OutTimeDeal":0,"DoOutTime":"","DoOutTimeCond":"","SFSta":0,"SFShowModel":1,"SFCaption":"","SFDefInfo":"","SFActiveFlows":"","SFFields":"","SF_X":0.0,"SF_Y":0.0,"SF_H":0.0,"SF_W":0.0,"OfficeOpen":"打开本地","OfficeOpenEnable":0,"OfficeOpenTemplate":"打开模板","OfficeOpenTemplateEnable":0,"OfficeSave":"保存","OfficeSaveEnable":1,"OfficeAccept":"接受修订","OfficeAcceptEnable":0,"OfficeRefuse":"拒绝修订","OfficeRefuseEnable":0,"OfficeOver":"套红按钮","OfficeOverEnable":0,"OfficeMarks":1,"OfficeReadOnly":0,"OfficePrint":"打印按钮","OfficePrintEnable":0,"OfficeSeal":"签章按钮","OfficeSealEnabel":0,"OfficeInsertFlow":"插入流程","OfficeInsertFlowEnabel":0,"OfficeNodeInfo":0,"OfficeReSavePDF":0,"OfficeDownLab":"下载","OfficeIsDown":0,"OfficeIsMarks":1,"OfficeTemplate":"","OfficeIsParent":1,"OfficeIsTrueTH":0,"OfficeTHTemplate":"","MPhone_WorkModel":0,"MPhone_SrcModel":0,"MPad_WorkModel":0,"MPad_SrcModel":0,"SelectorDBShowWay":0,"SelectorModel":5,"SelectorP1":"","SelectorP2":"","ICON":"前台","NodeWorkType":1,"FlowName":"版本单管理","FK_FlowSort":"","FK_FlowSortT":"","FrmAttr":"","TAlertRole":0,"TAlertWay":0,"WAlertRole":0,"WAlertWay":0,"Doc":"","ReturnReasonsItems":"","ReturnAlert":"","IsCanRpt":1,"IsCanOver":0,"IsSecret":0,"IsCanDelFlow":0,"IsHandOver":0,"NodePosType":0,"IsCCFlow":0,"HisStas":"@51","HisDeptStrs":"@51","HisToNDs":"@7502","HisBillIDs":"","HisSubFlows":"","PTable":"","ShowSheets":"","GroupStaNDs":"@7501","X":200,"Y":150,"AtPara":"","DocLeftWord":"","DocRightWord":"","AutoRunEnable":0,"AutoRunParas":"","CCIsStations":0,"CCIsDepts":0,"CCIsEmps":0,"CCIsSQLs":0,"CCCtrlWay":0,"CCSQL":"","CCTitle":"","CCDoc":"","SelfParas":"","OfficeOpenLab":"打开本地","OfficeOpenTemplateLab":"打开模板","OfficeSaveLab":"保存","OfficeAcceptLab":"接受修订","OfficeRefuseLab":"拒绝修订","OfficeOverLab":"套红","OfficeMarksEnable":1,"OfficePrintLab":"打印","OfficeSealLab":"签章","OfficeSealEnable":0,"OfficeInsertFlowLab":"插入流程","OfficeInsertFlowEnable":0,"OfficeDownEnable":0,"OfficeTHEnable":0,"FWCLab":"审核信息","SFLab":"子流程","FrmThreadLab":"子线程","FrmThreadSta":1,"FrmThread_X":5.0,"FrmThread_Y":5.0,"FrmThread_H":300.0,"FrmThread_W":400.0,"FrmTrackLab":"轨迹","FrmTrackSta":1,"FrmTrack_X":5.0,"FrmTrack_Y":5.0,"FrmTrack_H":300.0,"FrmTrack_W":400.0,"FTCLab":"流转自定义","FTCSta":1,"FTCWorkModel":0,"FTC_X":5.0,"FTC_Y":5.0,"FTC_H":300.0,"FTC_W":400.0,"SFShowCtrl":0,"SubFlowEnable":0,"SFDeleteRole":0,"SFOpenType":0,"ThreadIsCanDel":0,"ThreadIsCanShift":0,"FWCIsShowTruck":0,"TimeLimit":2.0,"TWay":0,"AccepterDBSort":0}],"Track":[{"MyPK":1154884557,"ActionType":22,"ActionTypeText":"审核","FID":0,"WorkID":37533,"NDFrom":7501,"NDFromT":"版本单提交","NDTo":7501,"NDToT":"版本单提交","EmpFrom":"zhhujie","EmpFromT":"胡捷","EmpTo":"zhhujie","EmpToT":"胡捷","RDT":"2017-04-13 20:43:36","WorkTimeSpan":0.0,"Msg":"同意","NodeData":"","Exer":"(zhhujie,胡捷)","Tag":"37533_7501_37533_0_zhhujie"}]}"';