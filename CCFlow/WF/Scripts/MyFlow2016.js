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
    var url = "./WorkOpt/Accepter.aspx?WorkID=" + workid + "&FK_Node=" + nodeid + "&FK_Flow=" + flowNo + "&FID=" + fid + "&type=2";
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
function ReqDtlBObj(dtlTable,DtlColumn, onValue) {

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
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高
        window.moveTo(0, 0);           //把window放在左上角
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh
    }
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
    pageData.FID = GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadOnly = GetQueryString("IsReadOnly");//如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow");//是否是启动流程页面 即发起流程
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
        url: "MyFlow.ashx?Method=InitToolBar&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            var barHtml = data;
            $('.Bar').html(barHtml);
            if ($('[value=退回]').length > 0) {
                $('[value=退回]').attr('onclick', '');
                $('[value=退回]').unbind('click');
                $('[value=退回]').bind('click', function () { initModal("returnBack"); $('#returnWorkModal').modal().show(); });
            }
            if ($('[value=移交]').length > 0) {

                $('[value=移交]').attr('onclick', '');
                $('[value=移交]').unbind('click');
                $('[value=移交]').bind('click', function () { initModal("shift"); $('#returnWorkModal').modal().show(); });
            }
            if ($('[value=加签]').length > 0) {
                $('[value=加签]').attr('onclick', '');
                $('[value=加签]').unbind('click');
                $('[value=加签]').bind('click', function () { initModal("askfor"); $('#returnWorkModal').modal().show(); });
            }
        }
    });
}

//初始化退回、移交、加签窗口
function initModal(modalType) {
    //初始化退回窗口的SRC
    var returnWorkModalHtml = '<div class="modal fade" id="returnWorkModal">' +
       '<div class="modal-dialog">'
           + '<div class="modal-content" style="border-radius:0px;width:700px;text-align:left;">'
              + '<div class="modal-header">'
                  + '<button type="button" class="close" style="color:white;opacity:1;" data-dismiss="modal" aria-hidden="true">&times;</button>'
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
            default:
                break;
        }
    }
    $('#iframeReturnWorkForm').attr('src',modalIframeSrc );
    $('#btnReturnWorkOK').unbind('click');
    $('#btnReturnWorkOK').bind('click', function () {
        var retrunVal = frames["iframeReturnWorkForm"].window.returnValue;
        if (retrunVal == undefined)
            retrunVal = "";
        $('#Message').html(retrunVal);
    });
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
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}


//保存
function Save() {
    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(),
        url: "MyFlow.ashx?Method=Save",
        dataType: 'html',
        success: function (data) {
            if (data.indexOf('err@') == 0) {
                $('#Message').html(data.substring(4, data.length));
            }
            else {
                $('#Message').html(data);
                //表示退回OK
                if (data.indexOf('工作已经被您退回到') == 0) {
                    setAttachDisabled();
                    setToobarUnVisible();
                    setFormEleDisabled();
                }
            }
        }
    });
}

//退回工作
function returnWorkWindowClose(data) {
    $('#returnWorkModal').modal('hide');
    $('#Message').html(data);
    if (data.indexOf('工作已经被您退回到') == 0) {
        setAttachDisabled();
        setToobarUnVisible();
        setFormEleDisabled();
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
        console.log('返回参数失败，ErrMsg:' + data.ErrMsg + ";Msg:" + data.Msg + ";url:" + url);
        console.log(dataParam);
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
                    break;
                //将 中文的  冒号转成英文的冒号
                var src = fram.URL.replace(new RegExp(/(：)/g), ':');
                var params = '&FID=' + pageData.FID;
                params += '&WorkID=' + groupFiled.OID;

                //var urlParamArr = getQueryString();
                //var urlPramTranArr = [];
                //$.each(urlParamArr, function (i, urlParam) {
                //    if (urlParam.split('=').length == 2) {
                //        urlPramTranArr.push((urlParam.indexOf('@') == 0 ? urlParam.split('=')[0].substring(1) : urlParam) + '=' + urlParam.split('=')[1]);
                //    }
                //})

               
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

                    //src += "&r=q&" + result.join('&');
                    //src += "&r=q" + params;
                    //src = src.substring(0, src.indexOf("?")) + '?' + result.join('&');
                }
                else {
                    //src += "?r=q&" + result.join('&');
                    //src += "?r=q" + params;
                    //src = src.substring(0, src.indexOf("?")) + '?' + result.join('&');
                }
                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            }
            break;
        case "Dtl":
            //WF/CCForm/Dtl.aspx?EnsName=ND501Dtl1&RefPKVal=0&PageIdx=1
            var src = "/WF/CCForm/Dtl.aspx?s=2&EnsName=" + groupFiled.CtrlID + "&RefPKVal=" + pageData.WorkID + "&PageIdx=1";
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Ath": //增加附件.
            for (var athIndex in workNodeData.Sys_FrmAttachment) {
                var ath = workNodeData.Sys_FrmAttachment[athIndex];
                if (ath.MyPK != groupFiled.CtrlID)
                    continue;
                var src = "";
                if (pageData.IsReadonly)
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                else
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK;

                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Track": //轨迹图.
            var src = "/WF/WorkOpt/OneWork/Track.aspx?s=2";
            //var paras = pageParamToUrl();
            //if (paras.indexOf('OID') < 0) {
            //    paras += "&OID=" + pageData.WorkID;
            //}
            src += '&FK_Flow=' + pageData.FK_Flow;
            src += '&FK_Node=' + pageData.FK_Node;
            src += '&WorkID=' + pageData.WorkID;
            src += '&FID=' + pageData.FID;
            //先临时写成这样的
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:500px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

            break;
        case "Thread": //子线程.
            var src = "/WF/WorkOpt/Thread.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "FTC": //流转自定义.  有问题
            var src = "/WF/WorkOpt/FTC.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        default:
            break;
    }
    return groupHtml;
}

function InitForm() {
    var CCFormHtml = '';
    var workNodeData = JSON.parse(jsonStr);
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
           
            var reloadBtn = '';
            if (groupFiled.CtrlType == "SubFlow") {
                reloadBtn = '<label class="reloadIframe">刷新</label>'
            } else if (groupFiled.CtrlType == "Track") {
                reloadBtn = '<label class="reloadIframe">返回轨迹图</label>'
            }

            groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12" style=""><div id="groupH' + groupFiled.Idx + '"  class="group section" data-target="group' + groupFiled.Idx + '"><label class="state">+</label>' +
                groupFiled.Lab + reloadBtn + '</div></div>';
            
            navGroupHtml += '<li class="scrollNav"><a href="#groupH' + groupFiled.Idx + '">' + $('<p>' + groupFiled.Lab + '</p>').text() + '</a></li>';
            
            groupHtml += groupResultHtml;

            CCFormHtml += groupHtml;
            continue;
        } else if (groupResultHtml == '' && groupFiled.CtrlType == "Ath") {//无附件的分组不展示
            continue;
        }
        else {//返回的值如果为 ''，就表明是字段分组
            groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12"><div class="section group" id="groupH' + groupFiled.Idx + '"  data-target="group' + groupFiled.Idx + '"><label class="state">-</label>' +
                groupFiled.Lab + '</div></div>';
            navGroupHtml += '<li class="scrollNav"><a href="#groupH' + groupFiled.Idx + '">' + $('<p>' + groupFiled.Lab + '</p>').text() + '</a></li>';
            groupHtml += groupResultHtml;
            CCFormHtml += groupHtml;
        }

        //解析字段
        //过滤属于本分组的字段 
        groupHtml = '<div class="col-lg-12 col-md-12 col-sm-12" style="clear:both;"> ' + '<input type="button" value="' + groupFiled.Lab + '"/></div>';
        var mapAttrData = $.grep(workNodeData.Sys_MapAttr, function (value) {
            return value.GroupID == groupFiled.OID;
        });

        //开始解析表单字段
        var mapAttrsHtml = InitMapAttr(mapAttrData, workNodeData);
        CCFormHtml += "<div class='col-lg-12 col-md-12 col-sm-12 ' id='" + "group" + groupFiled.Idx + "'>" + mapAttrsHtml + "</div>";

        CCFormHtml += "</div>";
    }

    $('#CCForm').html(CCFormHtml);
    $('#nav').html(navGroupHtml);
    
    $($('#nav li')[0]).addClass('current');
    $('#nav').onePageNav();

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

    try {
        ////加载JS文件
        var enName = workNodeData.Sys_MapData[0].No;
        var jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + ".js' ></script>";
        jsSrc += "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
        $('body').append($('<div>' + jsSrc + '</div>'));
    }
    catch (err) {
        console.log(err);
    }

    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt();

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
    if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
        $('#CCForm').css('min-width', workNodeData.Sys_MapData[0].TableWidth);
    }
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
    $('#topContentDiv').css('margin-left', '15px');
    $('#topContentDiv').css('margin-right', '15px');
    $('#header').css('width', 'auto');
    $('#Message').css('width', 'auto');
    //隐藏左侧导航栏
    $('#nav').css('display', 'none');
}
//8列切为4列
function Col8To4() {
    pageData.Col = 4;
    $('.col-sm-2').attr('class', 'col-lg-2 col-md-2 col-sm-2');
    $('.col-sm-4').attr('class', 'col-lg-4 col-md-4 col-sm-4')
    $('.col-sm-10').attr('class', 'col-lg-10 col-md-10 col-sm-10');

    $('#CCForm').css('min-width', 0);
    $('#topContentDiv').css('width', '900px');
    $('#topContentDiv').css('margin-left', 'auto');
    $('#topContentDiv').css('margin-right', 'auto');
    $('#header').css('width', '900px');
    $('#Message').css('width', '900px');

    //显示左侧导航栏
    $('#nav').css('display', 'block');
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

            //添加文本框 ，日期控件等
            //AppString   
            if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                if (mapAttr.ColSpan == 2 || mapAttr.ColSpan == 1 || mapAttr.ColSpan == 3) {//占有1-2  3列的文本框
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
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                        eleHtml += '</div>';
                    } else {//文本区域
                        if (mapAttr.UIHeight <= 23) {
                            eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">' +
                                "<input name='TB_" + mapAttr.KeyOfEn + "' type='text' value='" + defValue + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                + '</div>';
                        }
                        else {//大于23就是多行
                            if (mapAttr.ColSpan == 1 || mapAttr.ColSpan == 2) {
                                mdCol = 4;
                                smCol = 12;
                            }
                            islabelIsInEle = true;
                            eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">'
                                + "<label>" + mapAttr.Name + "</label>"
                                +
                                (mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="'+mapAttr.KeyOfEn+'">*</span>' : "")
                                +
                                "<textarea style='height:" + mapAttr.UIHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' value='" + defValue + "'" + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                                + '</div>';

                        }
                    }
                } else if (mapAttr.ColSpan == "4" || (mapAttr.ColSpan == "3" && mapAttr.UIHeight > 23)) {//大文本区域  且占一整行
                    isInOneRow = true;
                    eleHtml += '<div class="col-lg-11 col-md-11 col-sm-10">' +
                        "<textarea name='TB_" + mapAttr.KeyOfEn + "'" + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" + defValue + "</textarea>"
                        + '</div>';
                }
            } //AppDate
            else if (mapAttr.MyDataType == 6) {//AppDate
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {
                    enableAttr = 'onfocus="WdatePicker("' + ")" + '";';
                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + defValue + "</div>";
            }
            else if (mapAttr.MyDataType == 7) {// AppDateTime = 7
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {
                    enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input type='text' class='TBcalendar'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' value='" + defValue + "'/>" + "</div>";
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

                    if (mapAttr.ColSpan == 1 || mapAttr.ColSpan == 3) {
                        if (mapAttr.ColSpan == 1) {
                            eleHtml += '<div class="col-md-2 col-sm-4 col-lg-2">';
                        } else if (mapAttr.ColSpan == 3) {
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
                            operations += "<input id='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' type='radio' " + (obj.IntKey == defValue ? " checked='checked' " : "") + "  name='RB_" + mapAttr.KeyOfEn + "' value='" + obj.IntKey + "'/><label for='RB_"+ mapAttr.KeyOfEn + obj.IntKey +"' class='labRb'>" + obj.Lab + "</label>" + (rbShowModel == "1" ? "</br>" : '');
                        });
                    }

                    eleHtml += operations;
                    //eleHtml += "</div>";
                    eleHtml += "</div>";
                }

                //if (mapAttr.ColSpan == 3) {
                //    var operations = '';
                //    isInOneRow = true;
                //    eleHtml += '<div class="col-md-11 col-sm-10 col-lg-11">';
                //    if (mapAttr.ColSpan == 1) {
                //        //外键类型
                //        var enums = workNodeData.Sys_Enum;
                //        enums = $.grep(enums, function (value) {
                //            return value.EnumKey == mapAttr.UIBindKey;
                //        });

                //        var rbShowModel = 0;//RBShowModel=0时横着显示RBShowModel=1时竖着显示
                //        var showModelindex = mapAttr.AtPara.indexOf('@RBShowModel=');
                //        if (showModelindex >= 0) {//@RBShowModel=0
                //            rbShowModel = mapAttr.AtPara.substring('@RBShowModel='.length, '@RBShowModel='.length + 1);
                //        }
                //        $.each(enums, function (i, obj) {
                //            operations += "<input id='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' type='radio'" + (obj.IntKey == defValue ? " checked='checked' " : "") + "  name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'>" + obj.Lab + "</input>" + (rbShowModel == "1" ? "</br>" : '');
                //        });
                //    }
                //    eleHtml += operations;
                //    //eleHtml += "</div>";
                //    eleHtml += "</div>";
                //}
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
                eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input value='" + defValue + "' type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + "</div>";
            }
            //AppMoney  AppRate
            if (mapAttr.MyDataType == 8) {
                var enableAttr = '';
                if (mapAttr.UIIsEnable == 1) {

                } else {
                    enableAttr = "disabled='disabled'";
                }
                eleHtml += '<div class="col-lg-2 col-md-2 col-sm-4 col-xs-8">' + "<input value='" + defValue + "' type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>" + "</div>";
            }

            if (mapAttr.LGType == 2) {
                var mdCol = 2;
                var smCol = 4;
                if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                    mdCol = 4;
                    smCol = 8;
                }

                eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">' +
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + defValue + "'" + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr) + "</select>";

                eleHtml += '</div>';
            }

            if (!islabelIsInEle) {
                eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  data-tag=' + str + ' class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
                (mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
                + "</div>" + eleHtml;

            }
            resultHtml += eleHtml;
        } else {
            var value = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (value == undefined) {
                value = '';
            } else {
                value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
            }
            
            //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
            hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' value='" + value + "' name='TB_" + mapAttr.KeyOfEn + "'></input>";
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
                console.log("icon:" + icon + popWorkModelStr);
                var eleHtml = ' <div class="input-group form_tree">' + tb.parent().html() +
                '<span class="input-group-addon" onclick="' + "ReturnValCCFormPopValGoogle('TB_" + mapExt.AttrOfOper + "','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');" + '"><span class="' + icon + '"></span></span></div>';
                tb.parent().html(eleHtml);
                break;
            case "RegularExpression"://正则表达式
                var tb = $('[name$=' + mapExt.AttrOfOper + ']');
                tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('name') + "'," + mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}') + ",'" + mapExt.Tag1 + "')");
                break;
            case "InputCheck"://输入检查
                var tbJS = $("#TB_" + mapExt.AttrOfOper);
                if (tbJS != undefined) {
                    tbJS.attr(mapExt.Tag2, mapExt.Tag1 + "(this)");
                }
                else {
                    tbJS = $("#DDL_" + mapExt.AttrOfOper);
                    if (ddl != null)
                        ddl.attr(mapExt.Tag2, mapExt.Tag1 + "(this);");
                }
                break;
            case "TBFullCtrl"://自动填充  先不做
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
                operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
        else if (workNodeData[mapAttr.UIBindKey] != undefined) {
            $.each(workNodeData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
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
        if (keyOfEn == ele) {
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
    return result = unescape(result);
}
//加载表单数据.
function GenerWorkNode() {
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        url: "MyFlow.ashx?Method=GenerWorkNode&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            jsonStr = data;
            //解析表单
            InitForm();
            ///根据下拉框选定的值，绑定表单中哪个元素显示，哪些元素不显示
            //showEleDependOnDRDLValue();
            //
            showNoticeInfo();
        }
    });
}

//获取表单数据
function getFormData() {
    var formss = $('#divCCForm').serialize();
    var formArr = formss.split('&');
    var formArrResult = [];
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
            formArrResult.push($(hidden).attr("name") + '=' + $(hidden).val());
        }
    });
    formss = formArrResult.join('&');
    //加上URL中的参数
    var dataArr = [];
    if (pageData != undefined) {
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
    if (!checkBlanks()) {
        return;
    }

    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(),
        url: "MyFlow.ashx?Method=Send",
        dataType: 'html',
        success: function (data) {
            if (data.indexOf('err@') == 0) {//发送时发生错误
                $('#Message').html(data.substring(4, data.length));
            }
            else if (data.indexOf('url@') == 0) {//发送成功时转到指定的URL 
                $('#Message').html("<a href=" + data.substring(4, data.length) + ">待处理</a>");
            }
            else if (data.indexOf('@当前工作') == 0) {
                $('#Message').html(data);
                //发送成功时
                setAttachDisabled();
                setToobarUnVisible();
                setFormEleDisabled();
            }
            else {//发送时发生错误信息
                $('#Message').html(data);
            }
        }
    });
}
//移交
//

//根据下拉框选定的值，绑定表单中哪个元素显示，哪些元素不显示
function showEleDependOnDRDLValue() {
    var data = colVisibleJsonStr;

    data = JSON.parse(data);
    data = data.List;
    var id = data[0].DRDLColName;
    // $("select[id$='" + id + "']").bind('change', function (obj) {
    $("input[type=radio],select").bind('change', function (obj) {
        var methodVal = obj.target.value;
        for (var j = 0; j < data.length; j++) {
            var value = data[j].Value;
            var visibleEles = data[j].VisibleCols;
            var unVisibleEles = data[j].UnVisibleCols;
            var drdlColName = data[j].DRDLColName;
            if (methodVal == value && obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
                var unVisibleEleArr = unVisibleEles.split(';');
                for (var i = 0; i < unVisibleEleArr.length; i++) {
                    if ($('[id$=_"' + unVisibleEleArr[i] + '"]').length > 0) {
                        $('[id$=_"' + unVisibleEleArr[i] + '"]').parent().css('display', 'none');
                        $('[id$=_"' + unVisibleEleArr[i] + '"]').parent().prev().css('display', 'none');
                    }
                }

                var visibleEleArr = visibleEles.split(';');
                for (var i = 0; i < visibleEleArr.length; i++) {
                    if ($('[id$=_"' + visibleEleArr[i] + '"]').length > 0) {
                        if (visibleEleArr[i].indexOf('Attach') == 0) {
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().css('display', 'display');
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().prev().css('display', 'display');
                        } else {
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().css('display', 'table-cell');
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().prev().css('display', 'table-cell');
                        }
                    }
                }
            }
        }
    });


    var selects = $('input[type=radio],select');
    for (var k = 0; k < selects.length; k++) {
        var methodVal = selects[i].value;
        for (var j = 0; j < data.length; j++) {
            var value = data[j].Value;
            var visibleEles = data[j].VisibleCols;
            var unVisibleEles = data[j].UnVisibleCols;
            var drdlColName = data[j].DRDLColName;
            if (methodVal == value && selects[i].name.indexOf(drdlColName) == (selects[i].name.length - drdlColName.length)) {
                var unVisibleEleArr = unVisibleEles.split(';');
                for (var i = 0; i < unVisibleEleArr.length; i++) {
                    if ($('[id$=_"' + unVisibleEleArr[i] + '"]').length > 0) {
                        $('[id$=_"' + unVisibleEleArr[i] + '"]').parent().css('display', 'none');
                        $('[id$=_"' + unVisibleEleArr[i] + '"]').parent().prev().css('display', 'none');
                    }
                }

                var visibleEleArr = visibleEles.split(';');
                for (var i = 0; i < visibleEleArr.length; i++) {
                    if ($('[id$=_"' + visibleEleArr[i] + '"]').length > 0) {
                        if (visibleEleArr[i].indexOf('Attach') == 0) {
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().css('display', 'display');
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().prev().css('display', 'display');
                        } else {
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().css('display', 'table-cell');
                            $('[id$=_"' + visibleEleArr[i] + '"]').parent().prev().css('display', 'table-cell');
                        }
                    }
                }
            }
        }
    }
}

//根据下拉框选定的值，弹出提示信息
function showNoticeInfo() {
    var workNode = JSON.parse(jsonStr);
    var rbs = workNode.Sys_FrmRB;
    data = rbs;
    $("input[type=radio],select").bind('change', function (obj) {
        var methodVal = obj.target.value;
        for (var j = 0; j < data.length; j++) {
            var value = data[j].IntKey;
            var noticeInfo = data[j].Tip;
            var drdlColName = data[j].KeyOfEn;
            if (methodVal == value && obj.target.name.indexOf(drdlColName) == (obj.target.name.length - drdlColName.length)) {
                //高级JS设置;  设置表单字段的  可用 可见 不可用 
                var fieldConfig = data[j].FieldsCfg;
                var fieldConfigArr = fieldConfig.split('@');
                $.each(fieldConfigArr, function (i, fieldCon) {
                    if (fieldCon != '' && fieldCon.split('=').length == 2) {
                        var fieldConArr = fieldCon.split('=');
                        var ele = $('[name$=' + fieldConArr[0] + ']');
                        var labDiv = undefined;
                        var eleDiv = undefined; 
                        if (ele.parent().attr('class').indexOf('input-group') >= 0) {
                            labDiv = ele.parent().parent().prev();
                            eleDiv = ele.parent().parent();
                        } else {
                            labDiv = ele.parent().prev();
                            eleDiv = ele.parent();
                        }
                        switch (fieldConArr[1]) {
                            case "1"://可用
                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                ele.removeAttr('disabled');
                                break;
                            case "2"://可见
                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                break;
                            case "3"://不可见
                                labDiv.css('display', 'none');
                                eleDiv.css('display', 'none');
                                break;
                        }
                    }
                });

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
                    top=top - $('#div_NoticeInfo').height() - 30;
                }
                $('#div_NoticeInfo').css('top',top );
                $('#div_NoticeInfo').css('left', left);
                $('#div_NoticeInfo').css('display', 'block');
                //$("#btnNoticeInfo").popover('show');
                //$('#btnNoticeInfo').trigger('click');
                break;
            }
        }
    });


    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInfo').css('display', 'none');
    })

    $("input[type=radio]:checked,select").change();
    $('#span_CloseNoticeInfo').click();
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
            console.log(keyofen)
            console.log($(obj).data())
            var ele = $('[id$=_' + keyofen + ']');
            console.log(ele)
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

    if (!checkBlankResult) {
        alert('有必填项未填写或者填写格式不正确，请检查表单补充内容');
    }
    return checkBlankResult;
}

//正则表达式检查
function checkReg() {

}

function SaveDtlAll() {
    return true;
}
var colVisibleJsonStr = ''
var jsonStr = '';