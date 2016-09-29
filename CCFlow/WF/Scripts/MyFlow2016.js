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
            $("#divCCForm").height(screenHeight - allHeight);

            $("#TDWorkPlace").height(screenHeight - allHeight - 10);

        }
        else {
            $("#divCCForm").height(parseFloat(frmHeight) + allHeight);
            $("#TDWorkPlace").height(parseFloat(frmHeight) + allHeight - 10);
        }
    }
    catch (e) {
    }
}

$(window).resize(function () {
    SetHegiht();
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
        var value = $("[id$='" + paramId + "']").val();
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
        paramUrlStr += '&' + param + '=' + pageData[param];
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

            $('[value=发送]').attr('onclick', $('[value=发送]').attr('onclick') + ";Send();");
            $('[value=保存]').attr('onclick', $('[value=保存]').attr('onclick') + ";Save();");

            $('[value=退回]').attr('onclick', "$('#returnWorkModal').modal().show();");
        }
    });


    //初始化退回窗口的SRC
    //$('#iframeReturnWorkForm').attr('src', "../WF/WorkOpt/ReturnWork.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + (new Date).toUTCString());
    //$('#btnReturnWorkOK').unbind('click');
    //$('#btnReturnWorkOK').bind('click', function () {
    //    var retrunVal = frames["iframeReturnWorkForm"].window.returnValue;
    //    if (retrunVal == undefined)
    //        retrunVal = "";
    //    $('#Message').html(retrunVal);
    //});
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

function returnWorkWindowClose(data) {
    $('#returnWorkModal').modal().hide();
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
                if (src.indexOf("?") > 0)
                    src += "&r=q" + params;
                else
                    src += "?r=q" + params;
                groupHtml += '<div class="col-md-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            }
            break;
        case "Dtl":
            break;
        case "Ath": //增加附件.
            for (var athIndex in workNodeData.Sys_FrmAttachment) {
                var ath = workNodeData.Sys_FrmAttachment[athIndex];
                if (ath.MyPK != groupFiled.CtrlID)
                    break;
                var src = "";
                if (pageData.IsReadonly)
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                else
                    src = "/WF/CCForm/AttachmentUpload.aspx?PKVal=" + pageData.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK;

                groupHtml += '<div class="col-md-12 " style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
            groupHtml += '<div class="col-md-12 " style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
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
            groupHtml += '<div class="col-md-12 " style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Track": //轨迹图.
            var src = "/WF/WorkOpt/Track.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-md-12 " style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Thread": //子线程.
            var src = "/WF/WorkOpt/Thread.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-md-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "FTC": //流转自定义.  有问题
            var src = "/WF/WorkOpt/FTC.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.WorkID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-md-12 " style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        default:
            break;
    }
    return groupHtml;
}

function InitForm() {
    var CCFormHtml = '';
    var workNodeData = JSON.parse(jsonStr);
    //解析节点名称
    $('#header span').text(workNodeData.Sys_MapData[0].Name);
    //解析分组
    var groupFileds = workNodeData.Sys_GroupField;

    for (var i = 0; i < groupFileds.length; i++) {
        var groupFiled = groupFileds[i];
        var groupHtml = '';
        //初始化分组
        var groupResultHtml = initGroup(workNodeData, groupFiled);
        if (groupResultHtml != '') {//返回的值如果为 ''，就表明是字段分组
            groupHtml = '<div class="col-md-12  " style=""><input class="group" type="button" value="+ ' + groupFiled.Lab + '"/></div>';
            groupHtml += groupResultHtml;
            CCFormHtml += groupHtml;
            continue;
        } else {//返回的值如果为 ''，就表明是字段分组
            groupHtml = '<div class="col-md-12  " style=""><input type="button" value="+ ' + groupFiled.Lab + '"/></div>';
            groupHtml += groupResultHtml;
            CCFormHtml += groupHtml;
        }

        //解析字段
        //过滤属于本分组的字段 
        groupHtml = '<div class="col-md-12 " style="clear:both;"> ' + '<input type="button" value="' + groupFiled.Lab + '"/></div>';
        var mapAttrData = $.grep(workNodeData.Sys_MapAttr, function (value) {
            return value.GroupID == groupFiled.OID;
        });

        //开始解析表单字段
        var mapAttrsHtml = InitMapAttr(mapAttrData, workNodeData);
        CCFormHtml += "<div id='" + "group" + groupFiled.Idx + "'>" + mapAttrsHtml + "</div>";
    }

    $('#CCForm').html(CCFormHtml);

    //根据NAME 设置ID的值
    var inputs = $('[name]');
    $.each(inputs, function (i, obj) {
        if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
            $(obj).attr("id", $(obj).attr("name"));
        }
    })

    //加载JS文件
    var enName = workNodeData.Sys_MapData[0].No;
    var jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + ".js' ></script>";
    $('body').append($('<div>' + jsSrc + '</div>'));

    //处理下拉框级联等扩展信息
    AfterBindEn_DealMapExt();

    //绑定分组的按钮事件  如果不是字段分组就变成可以折叠的
    $('.group').bind('click', function (obj) {
        var display = '';
        var text = $(obj.target).val().substring(2, $(obj.target).val().length);
        if ($(obj.target).val().indexOf('- ') == 0) {
            display = 'none';
            $(obj.target).val('+ ' + text);
        } else {
            display = 'block';
            $(obj.target).val('- ' + text);
        }

        var div = $(obj.target.parentNode.nextSibling).css('display', display);
    });

    //如果是IsReadOnly，就表示是查看页面，不是处理页面
    if (pageData.IsReadOnly != undefined && pageData.IsReadOnly == "1") {
        setAttachDisabled();
        setToobarUnVisible();
        setFormEleDisabled();
    }
}

//解析表单字段 MapAttr
function InitMapAttr(mapAttrData, workNodeData) {
    var resultHtml = '';
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



            if (mapAttr.UIIsInput == 1) {//必填
                eleHtml += '<span style="color:red" class="mustInput">*</span>';
            }

            eleHtml += '</label></div>';

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
                        eleHtml += '<div class="col-lg-' + mdCol + ' col-md-' + mdCol + ' col-sm-' + smCol + '">' +
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</operation>";
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
                                "<textarea style='height:" + mapAttr.UIHeight + "px;' name=TB_'" + mapAttr.KeyOfEn + "' type='text' value='" + defValue + "'" + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
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
                    eleHtml += '<div class="col-lg-' + colMd + ' col-md-' + mdCol + ' col-sm-' + colsm + '">' +
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                    + '</div>';
                    eleHtml += "</div>";
                }

                if (mapAttr.UIContralType == 3) {//RadioBtn
                    var operations = '';
                    eleHtml += '<div class="col-md-2 col-sm-4 col-lg-2">';
                    if (mapAttr.ColSpan == 1) {
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
                            operations += "<input id='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' type='radio' " + (obj.IntKey == defValue ? " checked='checked' " : "") + "  name='RB_" + mapAttr.KeyOfEn + "' value='" + obj.IntKey + "'>" + obj.Lab + "</input>" + (rbShowModel == "1" ? "</br>" : '');
                        });
                    }
                    eleHtml += operations;
                    eleHtml += "</div>";
                }

                if (mapAttr.ColSpan == 3) {
                    var operations = '';
                    isInOneRow = true;
                    eleHtml += '<div class="col-md-11 col-sm-10 col-lg-11">';
                    if (mapAttr.ColSpan == 1) {
                        //外键类型
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
                            operations += "<input id='RB_" + mapAttr.KeyOfEn + obj.IntKey + "' type='radio'" + (obj.IntKey == defValue ? " checked='checked' " : "") + "  name='RB_" + mapAttr.KeyOfEn + "' id='" + "RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'>" + obj.Lab + "</input>" + (rbShowModel == "1" ? "</br>" : '');
                        });
                    }
                    eleHtml += operations;
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
                eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  data-tag=' + str + ' class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + eleHtml;
            }
            resultHtml += eleHtml;
        }
    }

    return resultHtml;
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
                tb.attr("onclick", "ShowHelpDiv('TB_" + mapExt.AttrOfOper + "','','" + mapExt.MyPK + "','" + mapExt.FK_MapData + "','returnvalccformpopval');");
                tb.attr("ondblclick", "ReturnValCCFormPopValGoogle(this,'" + mapExt.MyPK + "','" + mapExt.FK_MapData + "', " + mapExt.W + "," + mapExt.H + ",'" + GepParaByName("Title", mapExt.AtPara) + "');");
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
                var ddlOper = t$("#DDL_" + mapExt.AttrOfOper);
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
    if (mapAttr.LGType == 2 && workNodeData[mapAttr.UIBindKey] != undefined) {
        $.each(workNodeData[mapAttr.UIBindKey], function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
        });
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
    return result;
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
            showEleDependOnDRDLValue();
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
                $('#Message').html(data.substring(4, data.length));
            }
            else {//发送时发生错误信息
                $('#Message').html(data);
            }
        }
    });
}

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
                    if ($('[id$="' + unVisibleEleArr[i] + '"]').length > 0) {
                        $('[id$="' + unVisibleEleArr[i] + '"]').parent().css('display', 'none');
                        $('[id$="' + unVisibleEleArr[i] + '"]').parent().prev().css('display', 'none');
                    }
                }

                var visibleEleArr = visibleEles.split(';');
                for (var i = 0; i < visibleEleArr.length; i++) {
                    if ($('[id$="' + visibleEleArr[i] + '"]').length > 0) {
                        if (visibleEleArr[i].indexOf('Attach') == 0) {
                            $('[id$="' + visibleEleArr[i] + '"]').parent().css('display', 'display');
                            $('[id$="' + visibleEleArr[i] + '"]').parent().prev().css('display', 'display');
                        } else {
                            $('[id$="' + visibleEleArr[i] + '"]').parent().css('display', 'table-cell');
                            $('[id$="' + visibleEleArr[i] + '"]').parent().prev().css('display', 'table-cell');
                        }
                    }
                }
            }
        }
    });


    var selects = $('input[type=radio],select');
    for (var k = 0; k < selects.length; k++) {
        var methodVal = selects[k].value;
        for (var j = 0; j < data.length; j++) {
            var value = data[j].Value;
            var visibleEles = data[j].VisibleCols;
            var unVisibleEles = data[j].UnVisibleCols;
            var drdlColName = data[j].DRDLColName;
            if (methodVal == value && selects[k].name.indexOf(drdlColName) == (selects[k].name.length - drdlColName.length)) {//处理人处理
                var unVisibleEleArr = unVisibleEles.split(';');
                for (var i = 0; i < unVisibleEleArr.length; i++) {
                    if ($('[id$="' + unVisibleEleArr[i] + '"]').length > 0) {
                        $('[id$="' + unVisibleEleArr[i] + '"]').parent().css('display', 'none');
                        $('[id$="' + unVisibleEleArr[i] + '"]').parent().prev().css('display', 'none');
                    }
                }

                var visibleEleArr = visibleEles.split(';');
                for (var i = 0; i < visibleEleArr.length; i++) {
                    if ($('[id$="' + visibleEleArr[i] + '"]').length > 0) {
                        if (visibleEleArr[i].indexOf('Attach') == 0) {
                            $('[id$="' + visibleEleArr[i] + '"]').parent().css('display', 'display');
                            $('[id$="' + visibleEleArr[i] + '"]').parent().prev().css('display', 'display');
                        } else {
                            $('[id$="' + visibleEleArr[i] + '"]').parent().css('display', 'table-cell');
                            $('[id$="' + visibleEleArr[i] + '"]').parent().prev().css('display', 'table-cell');
                        }
                    }
                }
            }
        }
    }
}

//根据下拉框选定的值，弹出提示信息
function showNoticeInfo() {
    return;
    $.ajax({
        type: 'post',
        async: true,
        url: "http://localhost:8080/Ashx/ITILFlow/NoticeInfoHandler.ashx?Method=GetList&m=" + Math.random() + "&FlowNo=" + Common.GetQueryString("FK_Flow"),
        dataType: "jsonp",
        jsonp: "callbackparam",
        jsonpCallback: "success_jsonpCallback",
        success: function (data) {
            data = JSON.parse(data);
            data = data.List;
            var id = data[0].DRDLColName;
            // $("select[id$='" + id + "']").bind('change', function (obj) {
            $("select").bind('change', function (obj) {
                var methodVal = obj.target.value;
                for (var j = 0; j < data.length; j++) {
                    var value = data[j].Value;
                    var noticeInfo = data[j].NoticeInfo;
                    var drdlColName = data[j].DRDLColName;
                    if (methodVal == value && obj.target.id.indexOf(drdlColName) == (obj.target.id.length - drdlColName.length)) {
                        var selectText = $(obj.target).find("option:selected").text();
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

                        $('#div_NoticeInfo').css('top', top - $('#div_NoticeInfo').height() - 30);
                        $('#div_NoticeInfo').css('left', left);
                        console.log(top)
                        console.log(left)
                        $('#div_NoticeInfo').css('display', 'block');
                        //$("#btnNoticeInfo").popover('show');
                        //$('#btnNoticeInfo').trigger('click');
                    }
                }
            });
        }
    });

    $('#span_CloseNoticeInfo').bind('click', function () {
        $('#div_NoticeInfo').css('display', 'none');
    })
}

//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL
    var lbs = $('[class*=col-md-1] label:contains(*)');
    $.each(lbs, function (i, obj) {
        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
            var children = $(obj).parent().next().children();
            if (children.length == 1) {
                children = $(children[0]);
                switch (children[0].tagName.toUpperCase()) {
                    case "INPUT":
                        if (children.attr('type') == "text") {
                            if (children.val() == "") {
                                checkBlankResult = false;
                                children.css('border-color', '#f48236');
                            } else {
                                children.css('border-color', '#2884fa');
                            }
                        }
                        break;
                    case "SELECT":
                        if (children.val() == "" || children.children('option:checked').text() == "*请选择") {
                            checkBlankResult = false;
                            children.css('border-color', '#f48236');
                        } else {
                            children.css('border-color', '#2884fa');
                        }
                        break;
                    case "TEXTAREA":
                        if (children.val() == "") {
                            checkBlankResult = false;
                            children.css('border-color', '#f48236');
                        } else {
                            children.css('border-color', '#2884fa');
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

var colVisibleJsonStr = '{"IsSuccess":true,"Msg":null,"MsgList":null,"ErrMsg":null,"ErrMsgList":null,"List":[{"No":"56","DRDLColName":"DRDL_QuoteKnowled","DisplayDRDLColName":"是否引用知识","Value":"1","VisibleCols":"","UnVisibleCols":"Links_KnowledgeNo","FlowNo":"5","NodeNo":"502","DisplayValue":"否","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"96","DRDLColName":"DRDL_QuoteKnowled","DisplayDRDLColName":"是否引用知识","Value":"2","VisibleCols":"Links_KnowledgeNo","UnVisibleCols":"","FlowNo":"5","NodeNo":"502","DisplayValue":"是","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"101","DRDLColName":"DRDL_QuoteKnowled","DisplayDRDLColName":"是否引用知识","Value":"0","VisibleCols":"","UnVisibleCols":"Links_KnowledgeNo","FlowNo":"5","NodeNo":"502","DisplayValue":"*请选择","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"98","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"5","VisibleCols":"TXB_AffectOther","UnVisibleCols":"DRDL_InfluenceDegree","FlowNo":"5","NodeNo":"502","DisplayValue":"其它","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"99","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"0","VisibleCols":"","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"*请选择","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"50","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"4","VisibleCols":"DRDL_InfluenceDegree","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"大于20家分行","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"52","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"1","VisibleCols":"DRDL_InfluenceDegree","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"1家分行","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"53","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"2","VisibleCols":"DRDL_InfluenceDegree","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"2-5家分行","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"54","DRDLColName":"DRDL_InfluenceRange","DisplayDRDLColName":"影响范围","Value":"3","VisibleCols":"DRDL_InfluenceDegree","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"5-20家分行","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"100","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"0","VisibleCols":"","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"*请选择","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"97","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"6","VisibleCols":"TXB_AffectOther","UnVisibleCols":"DRDL_InfluenceRange","FlowNo":"5","NodeNo":"502","DisplayValue":"其它","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"45","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"1","VisibleCols":"DRDL_InfluenceRange","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"单笔交易","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"46","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"2","VisibleCols":"DRDL_InfluenceRange","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"2-19笔交易","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"47","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"3","VisibleCols":"DRDL_InfluenceRange","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"20-50笔交易","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"48","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"4","VisibleCols":"DRDL_InfluenceRange","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"50-200笔交易","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"49","DRDLColName":"DRDL_InfluenceDegree","DisplayDRDLColName":"事件影响程度","Value":"5","VisibleCols":"DRDL_InfluenceRange","UnVisibleCols":"TXB_AffectOther","FlowNo":"5","NodeNo":"502","DisplayValue":"大于200笔","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"40","DRDLColName":"DRDL_AcceptMethod2","DisplayDRDLColName":"受理方式","Value":"0","VisibleCols":"Attach_AcceptOpinions;Attach_Public;CDT;DRDL_AcceptMethod2;DRDL_IncidentPRI;DRDL_IncidentSource1;DRDL_IncidentSource2;DRDL_IncidentType;DRDL_IncidentType1;DRDL_IncidentType2;DRDL_IncidentType3;DRDL_IncidentTypeT;DRDL_InfluenceDegree;DRDL_InfluenceRange;DRDL_QuoteKnowled;DRDL_RepeateIncident;Emps;FID;FK_Dept;Links_RelativeRecordNo;OID;RDT;Rec;TXB_AcceptOpinions2;TXB_Email2;TXB_Emp2;TXB_Mobile2;TXB_SystemName","UnVisibleCols":"TXB_DisposeEmp2;TXB_Servicer2","FlowNo":"5","NodeNo":"502","DisplayValue":"服务台分析解释","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"41","DRDLColName":"DRDL_AcceptMethod2","DisplayDRDLColName":"受理方式","Value":"1","VisibleCols":"Attach_AcceptOpinions;Attach_Public;CDT;DRDL_AcceptMethod2;DRDL_IncidentPRI;DRDL_IncidentSource1;DRDL_IncidentSource2;DRDL_IncidentType;DRDL_IncidentType1;DRDL_IncidentType2;DRDL_IncidentType3;DRDL_IncidentTypeT;DRDL_InfluenceDegree;DRDL_InfluenceRange;DRDL_RepeateIncident;Emps;FID;FK_Dept;Links_RelativeRecordNo;OID;RDT;Rec;TXB_AcceptOpinions2;TXB_DisposeEmp2;TXB_Email2;TXB_Emp2;TXB_Mobile2;TXB_SystemName","UnVisibleCols":"DRDL_QuoteKnowled;Links_KnowledgeNo;TXB_Servicer2","FlowNo":"5","NodeNo":"502","DisplayValue":"指定处理员","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"42","DRDLColName":"DRDL_AcceptMethod2","DisplayDRDLColName":"受理方式","Value":"2","VisibleCols":"Attach_AcceptOpinions;Attach_Public;CDT;DRDL_AcceptMethod2;DRDL_IncidentPRI;DRDL_IncidentSource1;DRDL_IncidentSource2;DRDL_IncidentType;DRDL_IncidentType1;DRDL_IncidentType2;DRDL_IncidentType3;DRDL_IncidentTypeT;DRDL_InfluenceDegree;DRDL_InfluenceRange;DRDL_RepeateIncident;Emps;FID;FK_Dept;Links_RelativeRecordNo;OID;RDT;Rec;TXB_AcceptOpinions2;TXB_Email2;TXB_Emp2;TXB_Mobile2;TXB_Servicer2;TXB_SystemName","UnVisibleCols":"DRDL_QuoteKnowled;Links_KnowledgeNo;TXB_DisposeEmp2","FlowNo":"5","NodeNo":"502","DisplayValue":"其他服务台","FlowName":"事件管理","NodeName":"服务台受理"},{"No":"43","DRDLColName":"DRDL_AcceptMethod2","DisplayDRDLColName":"受理方式","Value":"3","VisibleCols":"Attach_AcceptOpinions;Attach_Public;DRDL_AcceptMethod2;TXB_AcceptOpinions2","UnVisibleCols":"CDT;DRDL_IncidentPRI;DRDL_IncidentSource1;DRDL_IncidentSource2;DRDL_IncidentType;DRDL_IncidentType1;DRDL_IncidentType2;DRDL_IncidentType3;DRDL_IncidentTypeT;DRDL_InfluenceDegree;DRDL_InfluenceRange;DRDL_QuoteKnowled;DRDL_RepeateIncident;Emps;FID;FK_Dept;ITIL_CascadEnumT;Links_RelativeRecordNo;OID;RDT;Rec;TXB_DisposeEmp2;TXB_Email2;TXB_Emp2;TXB_Mobile2;TXB_Servicer2;TXB_SystemName","FlowNo":"5","NodeNo":"502","DisplayValue":"驳回","FlowName":"事件管理","NodeName":"服务台受理"}],"Data":null}';
var jsonStr = '{"Sys_GroupField":[{"OID":1643,"Lab":"开始节点","EnName":"ND17901","Idx":1,"GUID":"","CtrlType":"","CtrlID":"","AtPara":""},{"OID":1653,"Lab":"傻瓜表单测试","EnName":"ND17901","Idx":2,"GUID":"","CtrlType":"","CtrlID":"","AtPara":""}],"Sys_Enum":[{"MyPK":"FindLeader_CH_0","Lab":"直接领导","EnumKey":"FindLeader","IntKey":0,"Lang":"CH"},{"MyPK":"FindLeader_CH_1","Lab":"指定职务级别的领导","EnumKey":"FindLeader","IntKey":1,"Lang":"CH"},{"MyPK":"FindLeader_CH_2","Lab":"指定职务的领导","EnumKey":"FindLeader","IntKey":2,"Lang":"CH"},{"MyPK":"FindLeader_CH_3","Lab":"指定岗位的领导","EnumKey":"FindLeader","IntKey":3,"Lang":"CH"},{"MyPK":"PRI_CH_0","Lab":"低","EnumKey":"PRI","IntKey":0,"Lang":"CH"},{"MyPK":"PRI_CH_1","Lab":"中","EnumKey":"PRI","IntKey":1,"Lang":"CH"},{"MyPK":"PRI_CH_2","Lab":"高","EnumKey":"PRI","IntKey":2,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_0","Lab":"事假","EnumKey":"QingJiaLeiXing","IntKey":0,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_1","Lab":"病假","EnumKey":"QingJiaLeiXing","IntKey":1,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_2","Lab":"婚假","EnumKey":"QingJiaLeiXing","IntKey":2,"Lang":"CH"},{"MyPK":"WJLB_CH_0","Lab":"上行文","EnumKey":"WJLB","IntKey":0,"Lang":"CH"},{"MyPK":"WJLB_CH_1","Lab":"平行文","EnumKey":"WJLB","IntKey":1,"Lang":"CH"},{"MyPK":"WJLB_CH_2","Lab":"下行文","EnumKey":"WJLB","IntKey":2,"Lang":"CH"},{"MyPK":"WJLB_CH_3","Lab":"简讯","EnumKey":"WJLB","IntKey":3,"Lang":"CH"}],"WF_Node":[],"Sys_MapData":[{"No":"ND17901","Name":"填写请假申请单","FrmW":900,"FrmH":1200}],"Sys_MapAttr":[{"MyPK":"ND17901_Title","FK_MapData":"ND17901","KeyOfEn":"Title","Name":"标题","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":251,"UIHeight":23,"MinLen":0,"MaxLen":200,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":1,"UIIsInput":1,"IsSigan":0,"X":174.83,"Y":54.4,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":-1,"AtPara":""},{"MyPK":"ND17901_FK_DQ","FK_MapData":"ND17901","KeyOfEn":"FK_DQ","Name":"地区","DefVal":"","UIContralType":1,"MyDataType":1,"LGType":2,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"CN_PQ","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":1,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":1,"AtPara":""},{"MyPK":"ND17901_FK_DQT","FK_MapData":"ND17901","KeyOfEn":"FK_DQT","Name":"地区","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_PQ","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1653,"Idx":1,"AtPara":""},{"MyPK":"ND17901_QingJiaRen","FK_MapData":"ND17901","KeyOfEn":"QingJiaRen","Name":"请假人","DefVal":"@WebUser.Name","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":1,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaRenBuMen","FK_MapData":"ND17901","KeyOfEn":"QingJiaRenBuMen","Name":"请假人部门","DefVal":"@WebUser.FK_DeptName","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":2,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_FK_SFT","FK_MapData":"ND17901","KeyOfEn":"FK_SFT","Name":"省份","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_SF","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1653,"Idx":2,"AtPara":""},{"MyPK":"ND17901_PRI","FK_MapData":"ND17901","KeyOfEn":"PRI","Name":"优先级","DefVal":"2","UIContralType":1,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":200,"UIBindKey":"PRI","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":174.76,"Y":56.19,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":3,"AtPara":""},{"MyPK":"ND17901_QingJiaLeiXing","FK_MapData":"ND17901","KeyOfEn":"QingJiaLeiXing","Name":"请假类型","DefVal":"0","UIContralType":1,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"QingJiaLeiXing","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":4,"AtPara":"@RBShowModel=0"},{"MyPK":"ND17901_FK_SF","FK_MapData":"ND17901","KeyOfEn":"FK_SF","Name":"省份","DefVal":"","UIContralType":1,"MyDataType":1,"LGType":2,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"CN_SF","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":4,"AtPara":""},{"MyPK":"ND17901_QingJiaRiQiCong","FK_MapData":"ND17901","KeyOfEn":"QingJiaRiQiCong","Name":"请假日期从","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":5,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_SanLieWenBenKuang","FK_MapData":"ND17901","KeyOfEn":"SanLieWenBenKuang","Name":"三列文本框","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":5,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_RiQiDao","FK_MapData":"ND17901","KeyOfEn":"RiQiDao","Name":"日期到","DefVal":"","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":6,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_YouYanJing","FK_MapData":"ND17901","KeyOfEn":"YouYanJing","Name":"右眼睛","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":7,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaTianShu","FK_MapData":"ND17901","KeyOfEn":"QingJiaTianShu","Name":"请假天数","DefVal":"0","UIContralType":0,"MyDataType":3,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":7,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaYuanYin","FK_MapData":"ND17901","KeyOfEn":"QingJiaYuanYin","Name":"请假原因","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":4,"GroupID":1643,"Idx":8,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_ZuoYanJing","FK_MapData":"ND17901","KeyOfEn":"ZuoYanJing","Name":"左眼睛","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":10,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopDCCT","FK_MapData":"ND17901","KeyOfEn":"PopDCCT","Name":"Pop弹出窗体(表格模式)","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":12,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopFZMS","FK_MapData":"ND17901","KeyOfEn":"PopFZMS","Name":"Pop分组模式","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":13,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopSMS","FK_MapData":"ND17901","KeyOfEn":"PopSMS","Name":"Pop树模式","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":14,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopBGFY","FK_MapData":"ND17901","KeyOfEn":"PopBGFY","Name":"Pop表格分页","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":15,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_WJLB","FK_MapData":"ND17901","KeyOfEn":"WJLB","Name":"竖向展示枚举","DefVal":"0","UIContralType":3,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"WJLB","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":16,"AtPara":"@RBShowModel=1"},{"MyPK":"ND17901_FindLeader","FK_MapData":"ND17901","KeyOfEn":"FindLeader","Name":"竖向展示枚举左边","DefVal":"0","UIContralType":3,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"FindLeader","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":17,"AtPara":"@RBShowModel=1"},{"MyPK":"ND17901_DuYanLong","FK_MapData":"ND17901","KeyOfEn":"DuYanLong","Name":"独眼龙","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":18,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_Emps","FK_MapData":"ND17901","KeyOfEn":"Emps","Name":"Emps","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":400,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FID","FK_MapData":"ND17901","KeyOfEn":"FID","Name":"FID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_CityT","FK_MapData":"ND17901","KeyOfEn":"FK_CityT","Name":"城市","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_City","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_Dept","FK_MapData":"ND17901","KeyOfEn":"FK_Dept","Name":"操作员部门","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_CDT","FK_MapData":"ND17901","KeyOfEn":"CDT","Name":"发起时间","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"1","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_NY","FK_MapData":"ND17901","KeyOfEn":"FK_NY","Name":"年月","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":7,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_MyNum","FK_MapData":"ND17901","KeyOfEn":"MyNum","Name":"个数","DefVal":"1","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_OID","FK_MapData":"ND17901","KeyOfEn":"OID","Name":"WorkID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":2,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_RDT","FK_MapData":"ND17901","KeyOfEn":"RDT","Name":"接受时间","DefVal":"","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"1","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_Rec","FK_MapData":"ND17901","KeyOfEn":"Rec","Name":"发起人","DefVal":"@WebUser.No","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""}],"Sys_MapExt":[],"Sys_FrmLine":[{"MyPK":"29d9936a-fb29-4fd9-b6d3-611f523490d2","FK_MapData":"ND17901","X1":719.09,"X2":719.09,"Y1":40,"Y2":482.73,"BorderColor":"Black","BorderWidth":2},{"MyPK":"45638417-30af-4f05-a82b-b09cd501ad3a","FK_MapData":"ND17901","X1":81.55,"X2":718.82,"Y1":80,"Y2":80,"BorderColor":"Black","BorderWidth":2},{"MyPK":"4bfd8e91-99bb-4d96-aa8f-ed567d6c5684","FK_MapData":"ND17901","X1":83.36,"X2":717.91,"Y1":120.91,"Y2":120.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"6d8005f2-3018-44a4-9b16-5a1fc5aa4446","FK_MapData":"ND17901","X1":83.36,"X2":717.91,"Y1":40.91,"Y2":40.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"7c558b34-d002-4fcf-abb0-80e3b7b3b7b8","FK_MapData":"ND17901","X1":81.82,"X2":81.82,"Y1":40,"Y2":480.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"d6053f98-1b9e-42dc-bf93-0dff21ca9dff","FK_MapData":"ND17901","X1":81.82,"X2":720,"Y1":481.82,"Y2":481.82,"BorderColor":"Black","BorderWidth":2},{"MyPK":"d9fd4ff6-3142-4774-b3a9-a9c47f9faa52","FK_MapData":"ND17901","X1":281.82,"X2":281.82,"Y1":81.82,"Y2":121.82,"BorderColor":"Black","BorderWidth":2},{"MyPK":"ebe7c5ce-8947-4595-90be-25f406a639cb","FK_MapData":"ND17901","X1":360,"X2":360,"Y1":80.91,"Y2":120.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"f978aa2f-57e9-4f31-8d92-83289130ae22","FK_MapData":"ND17901","X1":158.82,"X2":158.82,"Y1":41.82,"Y2":482.73,"BorderColor":"Black","BorderWidth":2}],"Sys_FrmLink":[],"Sys_FrmBtn":[],"Sys_FrmImg":[{"MyPK":"I20160922161940_1","FK_MapData":"ND17901","ImgAppType":0,"X":577.26,"Y":3.45,"H":40,"W":137,"ImgURL":"/ccform；component/Img/LogoBig.png","ImgPath":"","LinkURL":"http：//ccflow.org","LinkTarget":"_blank","GUID":"","Tag0":"","SrcType":0,"IsEdit":0,"Name":"","EnPK":""}],"Sys_FrmLab":[{"MyPK":"Lab20160922161940_1","FK_MapData":"ND17901","Text":"优先级","X":109.05,"Y":58.1,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_10","FK_MapData":"ND17901","Text":"新建节点(请修改标题)","X":294.67,"Y":8.27,"FontColor":"Blue","FontName":"Portable User Interface","FontSize":23,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_13","FK_MapData":"ND17901","Text":"说明：以上内容是ccflow自动产生的，您可以修改/删除它。@为了更方便您的设计您可以到http：//ccflow.org官网下载表单模板.@因为当前技术问题与silverlight开发工具使用特别说明如下：@@1，改变控件位置： @  所有的控件都支持 wasd， 做为方向键用来移动控件的位置， 部分控件支持方向键. @2， 增加textbox， 从表， dropdownlistbox， 的宽度 shift+ -> 方向键增加宽度 shift + <- 减小宽度.@3， 保存 windows键 + s.  删除 delete.  复制 ctrl+c   粘帖： ctrl+v.@4， 支持全选，批量移动， 批量放大缩小字体.， 批量改变线的宽度.@5， 改变线的长度： 选择线，点绿色的圆点，拖拉它。.@6， 放大或者缩小　label 的字体 ， 选择一个多个label ， 按 A+ 或者　A－　按钮.@7， 改变线或者标签的颜色， 选择操作对象，点工具栏上的调色板.","X":168.24,"Y":163.7,"FontColor":"Red","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_4","FK_MapData":"ND17901","Text":"发起人","X":106.48,"Y":96.08,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_7","FK_MapData":"ND17901","Text":"发起时间","X":307.64,"Y":95.17,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0}],"Sys_FrmRB":[],"Sys_FrmEle":[],"Sys_FrmAttachment":[],"Sys_FrmImgAth":[],"Sys_MapDtl":[],"WF_NodeBar":[{"NodeID":17901,"Step":1,"FK_Flow":"179","Name":"填写请假申请单","Tip":"","WhoExeIt":0,"TurnToDeal":0,"TurnToDealDoc":"","ReadReceipts":0,"CondModel":0,"CancelRole":0,"IsTask":1,"IsRM":1,"DTFrom":"2016-09-22 16：19","DTTo":"2016-09-22 16：19","IsBUnit":0,"FocusField":"请假原因：@QingJiaYuanYin","SaveModel":0,"IsGuestNode":0,"SelfParas":"","RunModel":0,"SubThreadType":0,"PassRate":100,"SubFlowStartWay":0,"SubFlowStartParas":"","TodolistModel":0,"IsAllowRepeatEmps":0,"AutoRunEnable":0,"AutoRunParas":"","AutoJumpRole0":0,"AutoJumpRole1":0,"AutoJumpRole2":0,"WhenNoWorker":0,"SendLab":"发送","SendJS":"","SaveLab":"保存","SaveEnable":1,"ThreadLab":"子线程","ThreadEnable":0,"ThreadKillRole":0,"SubFlowLab":"子流程","SubFlowCtrlRole":0,"JumpWayLab":"跳转","JumpWay":0,"JumpToNodes":"","ReturnLab":"退回","ReturnRole":0,"ReturnAlert":"","IsBackTracking":0,"ReturnField":"","ReturnReasonsItems":"","CCLab":"抄送","CCRole":0,"CCWriteTo":0,"ShiftLab":"移交","ShiftEnable":0,"DelLab":"删除","DelEnable":0,"EndFlowLab":"结束流程","EndFlowEnable":0,"PrintDocLab":"打印单据","PrintDocEnable":0,"TrackLab":"轨迹","TrackEnable":1,"HungLab":"挂起","HungEnable":0,"SelectAccepterLab":"接受人","SelectAccepterEnable":0,"SearchLab":"查询","SearchEnable":0,"WorkCheckLab":"审核","WorkCheckEnable":0,"BatchLab":"批处理","BatchEnable":0,"AskforLab":"加签","AskforEnable":0,"TCLab":"流转自定义","TCEnable":0,"WebOffice":"公文","WebOfficeEnable":0,"PRILab":"重要性","PRIEnable":0,"CHLab":"节点时限","CHEnable":0,"FocusLab":"关注","FocusEnable":1,"FWCSta":0,"FWCShowModel":1,"FWCType":0,"FWCNodeName":"","FWCAth":0,"FWCTrackEnable":1,"FWCListEnable":1,"FWCIsShowAllStep":0,"SigantureEnabel":0,"FWCIsFullInfo":1,"FWCOpLabel":"审核","FWCDefInfo":"同意","FWC_H":300,"FWC_W":400,"FWCFields":"","MPhone_WorkModel":0,"MPhone_SrcModel":0,"MPad_WorkModel":0,"MPad_SrcModel":0,"FTCLab":"流转自定义","FTCSta":0,"FTCWorkModel":0,"FTC_X":5,"FTC_Y":5,"FTC_H":300,"FTC_W":400,"OfficeOpenLab":"打开本地","OfficeOpenEnable":0,"OfficeOpenTemplateLab":"打开模板","OfficeOpenTemplateEnable":0,"OfficeSaveLab":"保存","OfficeSaveEnable":1,"OfficeAcceptLab":"接受修订","OfficeAcceptEnable":0,"OfficeRefuseLab":"拒绝修订","OfficeRefuseEnable":0,"OfficeOverLab":"套红","OfficeOverEnable":0,"OfficeMarksEnable":1,"OfficePrintLab":"打印","OfficePrintEnable":0,"OfficeSealLab":"签章","OfficeSealEnable":0,"OfficeInsertFlowLab":"插入流程","OfficeInsertFlowEnable":0,"OfficeNodeInfo":0,"OfficeReSavePDF":0,"OfficeDownLab":"下载","OfficeDownEnable":0,"OfficeIsMarks":1,"OfficeTemplate":"","OfficeIsParent":1,"OfficeTHEnable":0,"OfficeTHTemplate":"","SFLab":"子流程","SFSta":0,"SFShowModel":1,"SFCaption":"","SFDefInfo":"","SFActiveFlows":"","SF_X":5,"SF_Y":5,"SF_H":300,"SF_W":400,"SFFields":"","SFShowCtrl":0,"SelectorDBShowWay":0,"SelectorModel":0,"SelectorP1":"","SelectorP2":"","OfficeOpen":"打开本地","OfficeOpenTemplate":"打开模板","OfficeSave":"保存","OfficeAccept":"接受修订","OfficeRefuse":"拒绝修订","OfficeOver":"套红按钮","OfficeMarks":1,"OfficeReadOnly":0,"OfficePrint":"打印按钮","OfficeSeal":"签章按钮","OfficeSealEnabel":0,"OfficeInsertFlow":"插入流程","OfficeInsertFlowEnabel":0,"OfficeIsDown":0,"OfficeIsTrueTH":0,"WebOfficeFrmModel":0,"FrmThreadLab":"子线程","FrmThreadSta":0,"FrmThread_X":5,"FrmThread_Y":5,"FrmThread_H":300,"FrmThread_W":400,"CheckNodes":"","DeliveryWay":0,"FWCLab":"审核信息","FWC_X":5,"FWC_Y":5,"CCIsStations":0,"CCIsDepts":0,"CCIsEmps":0,"CCIsSQLs":0,"CCCtrlWay":0,"CCSQL":"","CCTitle":"","CCDoc":"","IsExpSender":1,"DeliveryParas":"","BatchRole":0,"BatchListCount":12,"BatchParas":"","FormType":0,"NodeFrmID":"","FormUrl":"http：//","BlockModel":0,"BlockExp":"","BlockAlert":"","TSpanDay":0,"TSpanHour":8,"WarningDay":0,"WarningHour":4,"TCent":2,"CHWay":0,"IsEval":0,"OutTimeDeal":0,"DoOutTime":"","DoOutTimeCond":"","FrmTrackLab":"轨迹","FrmTrackSta":0,"FrmTrack_X":5,"FrmTrack_Y":5,"FrmTrack_H":300,"FrmTrack_W":400,"ICON":"前台","NodeWorkType":1,"FlowName":"我的流程(傻瓜表单)","FK_FlowSort":"01","FK_FlowSortT":"","FrmAttr":"","TAlertRole":0,"TAlertWay":0,"WAlertRole":0,"WAlertWay":0,"Doc":"","IsCanRpt":1,"IsCanOver":0,"IsSecret":0,"IsCanDelFlow":0,"IsHandOver":0,"NodePosType":0,"IsCCFlow":0,"HisStas":"@07@08@09@10@11","HisDeptStrs":"@07@08@09@10@11","HisToNDs":"@17902","HisBillIDs":"","HisSubFlows":"","PTable":"","ShowSheets":"","GroupStaNDs":"@17901","X":170,"Y":81,"AtPara":"","DocLeftWord":"","DocRightWord":""}],"WF_Flow":[{"No":"179","Name":"我的流程(傻瓜表单)","FK_FlowSort":"01","FK_FlowSortText":"线性流程","SysType":"","FlowRunWay":0,"RunObj":"","Note":"","RunSQL":"","NumOfBill":0,"NumOfDtl":0,"FlowAppType":0,"ChartType":1,"IsCanStart":"1","AvgDay":0,"IsFullSA":0,"IsMD5":0,"Idx":0,"TimelineRole":0,"Paras":"@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350","PTable":"","Draft":0,"DataStoreModel":1,"TitleRole":"","FlowMark":"","FlowEventEntity":"","HistoryFields":"","IsGuestFlow":0,"BillNoFormat":"","FlowNoteExp":"","DRCtrlType":0,"StartLimitRole":0,"StartLimitPara":"","StartLimitAlert":"","StartLimitWhen":0,"StartGuideWay":0,"StartGuidePara1":"","StartGuidePara2":"","StartGuidePara3":"","IsResetData":0,"IsLoadPriData":0,"CFlowWay":0,"CFlowPara":"","IsBatchStart":0,"BatchStartFields":"","IsAutoSendSubFlowOver":0,"Ver":"2016-09-20 15：11：11","DType":1,"AtPara":"","DTSWay":0,"DTSDBSrc":"","DTSBTable":"","DTSBTablePK":"","DTSTime":0,"DTSSpecNodes":"","DTSField":0,"DTSFields":""}],"MainTable":[{"QingJiaYuanYin":"我是请假原因","Title":"财务部-guobaogeng，郭宝庚在2016-09-22 17：14发起.","QingJiaRen":"郭宝庚","QingJiaRenBuMen":"财务部","PRI":2,"PRIText":"高","QingJiaLeiXing":1,"QingJiaLeiXingText":"事假","QingJiaRiQiCong":"2016-09-22 17：14","RiQiDao":"","QingJiaTianShu":0,"QingJiaYuanYin":"我是请假原因2","RDT":"2016-09-22 17：49","Rec":"guobaogeng","FK_NY":"2016-09","MyNum":1,"OID":102,"CDT":"2016-09-22 17：49","Emps":"guobaogeng","FID":0,"FK_CityT":"","FK_Dept":"5","FK_DQ":"","FK_DQText":"","FK_DQT":"","FK_SFT":"","FK_SF":"","FK_SFText":"","SanLieWenBenKuang":"","YouYanJing":"","ZuoYanJing":"","PopDCCT":"","PopFZMS":"","PopSMS":"","PopBGFY":"","WJLB":0,"WJLBText":"上行文","FindLeader":1,"FindLeaderText":"指定职务级别的领导","DuYanLong":""}],"CN_PQ":[{"No":"AA","Name":"城市"},{"No":"DB","Name":"东北"},{"No":"HB","Name":"华北"},{"No":"HD","Name":"华东"},{"No":"XB","Name":"西北"},{"No":"XN","Name":"西南"},{"No":"ZN","Name":"中南"},{"No":"ZZ","Name":"香澳台"}],"CN_SF":[{"No":"11","Name":"北京","Names":"北京市","JC":"京","FK_PQ":"AA"},{"No":"12","Name":"天津","Names":"天津市","JC":"津","FK_PQ":"AA"},{"No":"13","Name":"河北","Names":"河北省","JC":"冀","FK_PQ":"HB"},{"No":"14","Name":"山西","Names":"山西省","JC":"晋","FK_PQ":"HB"},{"No":"15","Name":"内蒙","Names":"内蒙古自治区","JC":"蒙","FK_PQ":"HB"},{"No":"21","Name":"辽宁","Names":"辽宁省","JC":"辽","FK_PQ":"DB"},{"No":"22","Name":"吉林","Names":"吉林省","JC":"吉","FK_PQ":"DB"},{"No":"23","Name":"黑龙江","Names":"黑龙江省","JC":"黑","FK_PQ":"DB"},{"No":"31","Name":"上海","Names":"上海市","JC":"沪","FK_PQ":"AA"},{"No":"32","Name":"江苏","Names":"江苏省","JC":"苏","FK_PQ":"HD"},{"No":"33","Name":"浙江","Names":"浙江省","JC":"浙","FK_PQ":"HD"},{"No":"34","Name":"安徽","Names":"安徽省","JC":"皖","FK_PQ":"HD"},{"No":"35","Name":"福建","Names":"福建省","JC":"闽","FK_PQ":"HD"},{"No":"36","Name":"江西","Names":"江西省","JC":"赣","FK_PQ":"HD"},{"No":"37","Name":"山东","Names":"山东省","JC":"鲁","FK_PQ":"HD"},{"No":"41","Name":"河南","Names":"河南省","JC":"豫","FK_PQ":"ZN"},{"No":"42","Name":"湖北","Names":"湖北省","JC":"鄂","FK_PQ":"ZN"},{"No":"43","Name":"湖南","Names":"湖南省","JC":"湘","FK_PQ":"ZN"},{"No":"44","Name":"广东","Names":"广东省","JC":"粤","FK_PQ":"ZN"},{"No":"45","Name":"广西","Names":"广西壮族自治区","JC":"桂","FK_PQ":"ZN"},{"No":"46","Name":"海南","Names":"海南省","JC":"琼","FK_PQ":"ZN"},{"No":"50","Name":"重庆","Names":"重庆市","JC":"渝","FK_PQ":"AA"},{"No":"51","Name":"四川","Names":"四川省","JC":"川","FK_PQ":"XN"},{"No":"52","Name":"贵州","Names":"贵州省","JC":"贵","FK_PQ":"XN"},{"No":"53","Name":"云南","Names":"云南省","JC":"云","FK_PQ":"XN"},{"No":"54","Name":"西藏","Names":"西藏自治区","JC":"藏","FK_PQ":"XN"},{"No":"61","Name":"陕西","Names":"陕西省","JC":"陕","FK_PQ":"XB"},{"No":"62","Name":"甘肃","Names":"甘肃省","JC":"甘","FK_PQ":"XB"},{"No":"63","Name":"青海","Names":"青海省","JC":"青","FK_PQ":"XB"},{"No":"64","Name":"宁夏","Names":"宁夏回族自治区","JC":"宁","FK_PQ":"XB"},{"No":"65","Name":"新疆","Names":"新疆维吾尔自治区","JC":"新","FK_PQ":"XB"},{"No":"71","Name":"台湾","Names":"台湾省","JC":"台","FK_PQ":"ZZ"},{"No":"81","Name":"香港","Names":"香港特别行政区","JC":"港","FK_PQ":"ZZ"},{"No":"82","Name":"澳门","Names":"澳门特别行政区","JC":"澳","FK_PQ":"ZZ"}]}'