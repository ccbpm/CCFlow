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
        return;
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
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        if (btn.value.valueOf('*') == -1)
            btn.value = btn.value + '*';
    }
} 

// ccform 为开发者提供的内置函数. 
// 获取DDL值 
function ReqDDL(ddlID) {
    var v = document.getElementById('DDL_' + ddlID).value;
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
    if (args[0] == sHref) /*参数为空*/{
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
    var v = document.getElementById('DDL_' + ddlID);
    if (v == null) {
        alert('没有找到ID=' + ddlID + '的下拉框控件.');
    }
    return v;
}
// 获取TB Obj
function ReqTBObj(tbID) {
    var v = document.getElementById('TB_' + tbID);
    if (v == null) {
        alert('没有找到ID=' + tbID + '的文本框控件.');
    }
    return v;
}
// 获取CheckBox Obj值
function ReqCBObj(cbID) {
    var v = document.getElementById('CB_' + cbID);
    if (v == null) {
        alert('没有找到ID=' + cbID + '的单选控件(获取CheckBox)对象.');
    }
    return v;
}
// 设置值.
function SetCtrlVal(ctrlID, val) {
    document.getElementById('TB_' + ctrlID).value = val;
    document.getElementById('DDL_' + ctrlID).value = val;
    document.getElementById('CB_' + ctrlID).value = val;
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


//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
        window.moveTo(0, 0);           //把window放在左上角     
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
    }
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

    var oid = GetQueryString("WorkID");
    if (oid == null)
        oid = GetQueryString("OID");
    pageData.OID = oid;

    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadOnly = GetQueryString("IsReadOnly"); //如果是IsReadOnly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    pageData.FK_MapData = GetQueryString("FK_MapData")//View

    //$('#navIframe').attr('src', 'Admin/CCBPMDesigner/truck/centerTrakNav.html?FK_Flow=' + pageData.FK_Flow + "&FID=" + pageData.FID + "&WorkID=" + pageData.OID);
}
//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {
        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
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


//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}

//FK_Flow=005&UserNo=zhwj&DoWhat=StartClassic&=&IsMobile=&FK_Node=501
var pageData = {};
var globalVarList = {};
//解析分组类型 如果返回的为 '' 就表明是字段分组
function initGroup(frmData, groupFiled) {
    var groupHtml = '';
    /*根据控件类型解析分组*/
    switch (groupFiled.CtrlType) {
        case "Frame": // 框架 类型.
            for (var frameIndex in frmData.Sys_MapFrame) {
                var fram = frmData.Sys_MapFrame[frameIndex];
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
                                        params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                                    }
                                    if (frmData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                                        params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr(1)];
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
                                    for (var ele in frmData.MainTable[0]) {
                                        if (paramArr[0].substring(1) == ele) {
                                            result = frmData.MainTable[0][ele];
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
            var src = "/WF/CCForm/DtlReadonly.htm?s=2&EnsName=" + groupFiled.CtrlID + "&RefPKVal=" + pageData.OID + "&PageIdx=1";
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "Ath": //增加附件.
            break;
            for (var athIndex in frmData.Sys_FrmAttachment) {
                var ath = frmData.Sys_FrmAttachment[athIndex];
                if (ath.MyPK != groupFiled.CtrlID)
                    continue;
                var src = "";
                src = "AttachmentUpload.aspx?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";

                groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            }
            break;
        case "FWC": //审核组件.
            var src = "/WF/WorkOpt/WorkCheck.htm?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.OID;
            }


            if (frmData.WF_Node.length > 0 && frmData.WF_Node[0].FWCSTA == 1) {
                paras += "&DoType=View";
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:150px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "SubFlow": //子流程..
            var src = "/WF/WorkOpt/SubFlow.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.OID;
            }
            if (frmData.WF_Node.length > 0 && frmData.WF_Node[0].FWCSTA == 1) {
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
            //    paras += "&OID=" + pageData.OID;
            //}
            src += '&FK_Flow=' + pageData.FK_Flow;
            src += '&FK_Node=' + pageData.FK_Node;
            src += '&WorkID=' + pageData.OID;
            src += '&FID=' + pageData.FID;
            //先临时写成这样的
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;"  id="group' + groupFiled.Idx + '">' + "<iframe style='width:100%; height:500px;'   src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';

            break;
        case "Thread": //子线程.
            var src = "/WF/WorkOpt/Thread.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.OID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        case "FTC": //流转自定义.  有问题
            var src = "/WF/WorkOpt/FTC.aspx?s=2";
            var paras = pageParamToUrl();
            if (paras.indexOf('OID') < 0) {
                paras += "&OID=" + pageData.OID;
            }
            src += "&r=q" + paras;
            groupHtml += '<div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="display:none;" id="group' + groupFiled.Idx + '">' + "<iframe  style='width:100%;'  src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
            break;
        default:
            break;
    }
    return groupHtml;
}


function InitForm() {
    var frmData = JSON.parse(jsonStr);
    var CCFormHtml = '';

    //开始解析表单字段
    var mapAttrsHtml = InitMapAttr(frmData.Sys_MapAttr, frmData);
    $('#divCCForm').html(mapAttrsHtml);

    //设置位置和大小
    $.each(frmData.Sys_MapAttr, function (i, obj) {
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
    var alertMsgs = frmData.AlertMsg;
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
    var enName = frmData.Sys_MapData[0].No;
    try {
        ////加载JS文件
        //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
        //$('body').append($('<div>' + jsSrc + '</div>'));

        var s = document.createElement('script');
        s.type = 'text/javascript';
        s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
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
        s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }
     

    //设置默认值
    for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {
        var mapAttr = frmData.Sys_MapAttr[j];
        //添加 label
        //如果是整行的需要添加  style='clear:both'


        var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
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
        var ath = $.grep(frmData.Sys_FrmAttachment, function (value) {
            return value.MyPK == athRefObj;
        })
        if (ath.length > 0) {
            ath = ath[0];
            var src = "";
            src = "AttachmentUpload.htm?IsExtend=1&PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + groupFiled.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
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
            var frmData = JSON.parse(jsonStr);
            //设置CCFORM的表格宽度  
            if (document.body.clientWidth > 992) {//处于中屏时设置宽度最小值
                $('#CCForm').css('min-width', frmData.Sys_MapData[0].TableWidth);
            }
            else {
                $('#CCForm').css('min-width', 0);
            }
        }
    }
}


//解析表单字段 MapAttr
function InitMapAttr(mapAttrData, frmData) {
    var resultHtml = '';

    var hiddenHtml = '';
    for (var j = 0; j < mapAttrData.length; j++) {
        var mapAttr = mapAttrData[j];
        if (mapAttr.UIVisible) {//是否显示
            //添加 label
            //如果是整行的需要添加  style='clear:both'

            var str = '';
            var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            for (var o in mapAttr) {
                str += o + ":" + mapAttr[o];
            }

            var eleHtml = '';
            var isInOneRow = false; //是否占一整行
            var islabelIsInEle = false; //

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
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" +
                                (frmData, mapAttr, defValue) + "</select>";
                        } else {//文本区域
                            if (mapAttr.UIHeight <= 23) {
                                eleHtml +=
                                    "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' disabled='disabled'/>"
                                    ;
                            }
                            else {
                                eleHtml +=
                                    "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + uiHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' disabled='disabled'/>"
                                ;
                            }
                        }
                    } else if (mapAttr.ColSpan == "4" || (mapAttr.ColSpan == "3" && mapAttr.UIHeight > 23)) {//大文本区域  且占一整行
                        var uiHeight = mapAttr.UIHeight / 23 * 30;
                        isInOneRow = true;
                        eleHtml +=
                            "<textarea  style='height:" + uiHeight + "px' maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' disabled='disabled'>" + "</textarea>"
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
                    checkedStr = ConvertDefVal(frmData, '', mapAttr.KeyOfEn);
                    eleHtml += "<div><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>";
                    eleHtml += '<label class="labRb" for="CB_' + mapAttr.KeyOfEn + '">' + mapAttr.Name + '</label></div>';
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
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
                        //eleHtml += "</div>";
                    }

                    if (mapAttr.UIContralType == 3) {
                        //RadioBtn
                        var operations = '';

                        if (mapAttr.ColSpan == 1 || mapAttr.ColSpan >= 3) {
                            var enums = frmData.Sys_Enum;
                            enums = $.grep(enums, function (value) {
                                return value.EnumKey == mapAttr.UIBindKey;
                            });

                            var rbShowModel = 0; //RBShowModel=0时横着显示RBShowModel=1时竖着显示
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
                    var mdCol = 2;
                    var smCol = 4;
                    if (mapAttr.ColSpan == 4 || mapAttr.ColSpan == 3) {
                        mdCol = 4;
                        smCol = 8;
                    }

                    eleHtml +=
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
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
                }
            }

            if (!islabelIsInEle) {
                eleHtml = '<div style="text-align:right;padding:0px;margin:0px; ' + (isInOneRow ? "clear:left;" : "") + '"  class="col-lg-1 col-md-1 col-sm-2 col-xs-4"><label>' + mapAttr.Name + "</label>" +
                (mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '">*</span>' : "")
                + "</div>" + eleHtml;

            }
            resultHtml += eleHtml;
        } else {
            var value = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
            if (value == undefined) {
                value = '';
            } else {
                //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
            }

            //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
            hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
        }
    }

    return resultHtml + hiddenHtml;
}

//处理MapExt
function AfterBindEn_DealMapExt() {
    return;
}
//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
}
 
//填充默认数据
function ConvertDefVal(frmData, defVal, keyOfEn) {
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
    for (var ele in frmData.MainTable[0]) {
        if (keyOfEn == ele && frmData.MainTable[0] != '') {
            result = frmData.MainTable[0][ele];
            break;
        }
    }
     

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    return result = unescape(result);
}
 

//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单.
function GenerFreeFrmReadonly() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    //隐藏保存按钮.
    if (href.indexOf('&IsReadonly=1') > 1 || href.indexOf('&IsEdit=0') > 1) {
        $("#Btn").hide();
    }

    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        //url: "../MyFlow.ashx?DoType=GenerWorkNode&DoType=" + pageData.DoType + "&m=" + Math.random(),
        url: Handler + "?DoType=FrmFreeReadonly_Init&m=" + Math.random() + "&" + urlParam,
        // url:"Handler.ashx?DoType=FrmFree_Init&FK_MapData="+pageData.FK_MapData + "&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {

            jsonStr = data;

            var gengerWorkNode = {};
            var flow_Data;
            try {
                flow_Data = JSON.parse(data);
                frmData = flow_Data;
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

            //循环之前的提示信息
            for (var i in flow_Data.AlertMsg) {
                var alertMsg = flow_Data.AlertMsg[i];
                var alertMsgEle = figure_Template_MsgAlert(alertMsg);
                $('#lastOptMsg').append(alertMsgEle);
            }
            //循环Sys_MapFrame
            for (var i in flow_Data.Sys_MapFrame) {
                var frame = flow_Data.Sys_MapFrame[i];
                var alertMsgEle = figure_Template_IFrame(frame);
                $('#lastOptMsg').append(alertMsgEle);
            }


            if (flow_Data["WF_Node"] != undefined && flow_Data["WF_Node"].length == 1) {
                //循环组件 轨迹图 审核组件 子流程 子线程
                $('#CCForm').append(figure_Template_FigureFlowChart(flow_Data["WF_Node"][0]));
                $('#CCForm').append(figure_Template_FigureFrmCheck(flow_Data["WF_Node"][0]));
                $('#CCForm').append(figure_Template_FigureSubFlowDtl(flow_Data["WF_Node"][0]));
                $('#CCForm').append(figure_Template_FigureThreadDtl(flow_Data["WF_Node"][0]));
            }

            if (flow_Data["Sys_MapData"] != undefined && flow_Data["Sys_MapData"].length == 1) {
                //初始化Sys_MapData
                var h = flow_Data.Sys_MapData[0].FrmH;
                var w = flow_Data.Sys_MapData[0].FrmW;

                $('#topContentDiv').height(h);
                $('#topContentDiv').width(w);
                $('.Bar').width(w + 15);
            }

            var marginLeft = $('#topContentDiv').css('margin-left');
            marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
            $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
            //原有的

            //为 DISABLED 的 TEXTAREA 加TITLE 
            var disabledTextAreas = $('#divCCForm textarea:disabled');
            $.each(disabledTextAreas, function (i, obj) {
                $(obj).attr('title', $(obj).val());
            })

            //初始化提示信息
            var alertMsgs = frmData.AlertMsg;
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


            //// 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
            var enName = frmData.Sys_MapData[0].No;
            try {
                ////加载JS文件
                //jsSrc = "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + "_Self.js' ></script>";
                //$('body').append($('<div>' + jsSrc + '</div>'));

                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
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
                s.src = "../DataUser/JSLibData/" + enName + "_Self.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }

            InitToNodeDDL();
            Common.MaxLengthError();

            //处理下拉框级联等扩展信息
            //  AfterBindEn_DealMapExt();

            //设置默认值
            for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {

                var mapAttr = frmData.Sys_MapAttr[j];

                //添加 label
                //如果是整行的需要添加  style='clear:both'

                var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
                if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
                    $('#TB_' + mapAttr.KeyOfEn).val(defValue);
                }

            }

            showNoticeInfo();

            showTbNoticeInfo();
            
        }
    })
}

var frmData = {};
//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_Template(mapAttr) {
    var eleHtml = '';
    if (mapAttr.UIVisible == 1) {//是否显示

        var str = '';
        var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

        var isInOneRow = false; //是否占一整行
        var islabelIsInEle = false; //

        eleHtml += '';

        if (mapAttr.UIContralType != 6) {

            if (mapAttr.LGType == 2) {
                eleHtml += "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
            } else {
                //添加文本框 ，日期控件等
                //AppString   
                if (mapAttr.MyDataType == "1" && mapAttr.LGType != "2") {//不是外键
                    if (mapAttr.UIContralType == "1") {//DDL 下拉列表框
                        eleHtml +=
                            "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + ">" + (frmData, mapAttr, defValue) + "</select>";
                    } else {//文本区域

                        if (mapAttr.UIHeight <= 23) {
                            eleHtml +=
                                "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>"
                            ;
                        }
                        else {

                            if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

                                //只读状态直接 div 展示富文本内容

                                eleHtml += "<div class='richText' style='width:" + mapAttr.UIWidth + "px'>" + defValue + "</div>";
                            } else {
                                eleHtml +=
                                "<textarea maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' disabled='disabled'/>"
                            }
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
                    checkedStr = ConvertDefVal(frmData, '', mapAttr.KeyOfEn);
                    eleHtml += "<div><input " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + "/>";
                    eleHtml += '<label class="labRb" for="CB_' + mapAttr.KeyOfEn + '">' + mapAttr.Name + '</label></div>';
                    //return eleHtml;
                }

                if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
                    if (mapAttr.UIContralType == 1) {//DDL
                        eleHtml +=
                                "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(frmData, mapAttr, defValue) + "</select>";
                    }
                }


                // AppDouble  AppFloat 
                if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                if ((mapAttr.MyDataType == 2 && mapAttr.LGType != 1)) {//AppInt
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
                //AppMoney  AppRate
                if (mapAttr.MyDataType == 8) {
                    var enableAttr = '';
                    if (mapAttr.UIIsEnable == 1) {

                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input style='text-align:right;' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                }
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
        var value = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (value == undefined) {
            value = '';
        } else {
            //value = value.toString().replace(/：/g, ':').replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
        }

        //hiddenHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + " value='" + ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' name='TB_" + mapAttr.KeyOfEn + "></input>";
        eleHtml += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
    }
    eleHtml = $('<div>' + eleHtml + '</div>');
    eleHtml.children(0).css('width', mapAttr.UIWidth).css('height', mapAttr.UIHeight);
    eleHtml.css('position', 'absolute').css('top', mapAttr.Y).css('left', mapAttr.X);

    if (mapAttr.UIIsEnable == "0") {
        enableAttr = eleHtml.find('[name=TB_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
        enableAttr = eleHtml.find('[name=DDL_' + mapAttr.KeyOfEn + ']').attr('disabled', true);
    }
    return eleHtml;
}

//将#FF000000 转换成 #FF0000
function TranColorToHtmlColor(color) {
    if (color != undefined && color.indexOf('#') == 0 && color.length == 9) {
        color = color.substring(0, 7);
    }
    return color;
}

//FontStyle, FontWeight, IsBold, IsItalic
//fontStyle font-size:19;font-family:"Portable User Interface";font-weight:bolder;color:#FF0051; 为H5设计的，不用解析后面3个
function analysisFontStyle(ele, fontStyle, isBold, isItalic) {
    if (fontStyle != undefined && fontStyle.indexOf(':') > 0) {
        var fontStyleArr = fontStyle.split(';');
        $.each(fontStyleArr, function (i, fontStyleObj) {
            ele.css(fontStyleObj.split(':')[0], fontStyleObj.split(':')[1]);
        });
    }
    else {
        if (isBold == 1) {
            ele.css('font-weight', 'bold');
        }
        if (isItalic == 1) {
            ele.css('font-style', 'italic')
        }
    }
}

//升级表单元素 初始化Label
function figure_Template_Label(frmLab) {
    var eleHtml = '';
    eleHtml = '<label></label>'
    eleHtml = $(eleHtml);
    var text = frmLab.Text.replace(/@/g, "<br>");
    eleHtml.html(text);
    eleHtml.css('position', 'absolute').css('top', frmLab.Y).css('left', frmLab.X).css('font-size', frmLab.FontSize)
        .css('padding-top', '5px').css('color', TranColorToHtmlColor(frmLab.FontColr));
    analysisFontStyle(eleHtml, frmLab.FontStyle, frmLab.isBold, frmLab.IsItalic);
    return eleHtml;
}

//初始化按钮
function figure_Template_Btn(frmBtn) {
    var eleHtml = $('<div></div>');
    var btnHtml = $('<input type="button" value="">');
    btnHtml.val(frmBtn.Text).width(frmBtn.W).height(frmBtn.H).addClass('btn');
    var doc = frmBtn.EventContext;
    doc = doc.replace("~", "'");
    var eventType = frmBtn.EventType;
    if (eventType == 0) {//禁用
        btnHtml.attr('disabled', 'disabled').css('background', 'gray');
    } else if (eventType == 5 || eventType == 6) {//运行Exe文件. 运行JS
        btnHtml.attr('onclick', doc);

    }
    eleHtml.append(btnHtml);
    //别的一些属性先不加
    eleHtml.css('position', 'absolute').css('top', frmBtn.Y).css('left', frmBtn.X).width(frmBtn.W).height(frmBtn.H);
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
    if (frmRb.UIIsEnable == false)
        childRbEle.attr('disabled', 'disabled');
    var defVal = ConvertDefVal(frmData, '', frmRb.KeyOfEn);
    if (defVal == frmRb.IntKey) {
        childRbEle.attr("checked", "checked");
    }

    eleHtml.append(childRbEle).append(childLabEle);
    eleHtml.css('position', 'absolute').css('top', frmRb.Y).css('left', frmRb.X);
    return eleHtml;
}

//初始化超链接
function figure_Template_HyperLink(frmLin) {
    //URL @ 变量替换
    var url = frmLin.URL;
    $.each(frmData.Sys_MapAttr, function (i, obj) {
        if (url.indexOf('@' + obj.KeyOfEn) > 0) {
            //替换
            //url=  url.replace(new RegExp(/(：)/g), ':');
            //先这样吧
            url = url.replace('@' + obj.KeyOfEn, frmData.MainTable[0][obj.KeyOfEn]);
        }
    });

    var eleHtml = '<span></span>';
    eleHtml = $(eleHtml);

    var a = $("<a></a>");
    a.attr('href', url).attr('target', frmLin.Target).html(frmLin.Text);
    eleHtml.append(a);
    eleHtml.css('position', 'absolute')
        .css('top', frmLin.Y)
        .css('left', frmLin.X)
        .css('color', frmLin.FontColr)
        .css('fontsize', frmLin.FontSize)
        .css('font-family', frmLin.FontName);
    return eleHtml;
}


//初始化 IMAGE  只初始化了图片类型
function figure_Template_Image(frmImage) {
    var eleHtml = '';
    var imgSrc = "";
    if (frmImage.ImgAppType == 0) {//图片类型
        //数据来源为本地.
        if (frmImage.ImgSrcType == 0) {
            if (frmImage.ImgPath.indexOf(";") < 0)
                imgSrc = frmImage.ImgPath;
        }
        //数据来源为指定路径.
        if (frmImage.ImgSrcType == 1) {
            //图片路径不为默认值
            imgSrc = frmImage.ImgURL;
            if (imgSrc.indexOf("@") == 0) {
                /*如果有变量 此处可能已经处理过    和周总商量*/
                //imgSrc = BP.WF.Glo.DealExp(imgSrc, en, "");
                imgSrc = imgSrc;
            }

        }
        // 由于火狐 不支持onerror 所以 判断图片是否存在放到服务器端
        if (imgSrc == "")//|| !File.Exists(Server.MapPath("~/" + imgSrc)))  //
            imgSrc = "../DataUser/ICON/CCFlow/LogBig.png";
        eleHtml = $('<div></div>');
        var a = $("<a></a>");
        var img = $("<img/>")
        img.attr("src", imgSrc).css('width', frmImage.W).css('height', frmImage.H).attr('onerror', "this.src='/DataUser/ICON/CCFlow/LogBig.png'");
        if (frmImage.LinkURL != undefined && frmImage.LinkURL != '') {
            a.attr('href', frmImage.LinkTarget).attr('target', frmImage.LinkTarget).css('width', frmImage.W).css('height', frmImage.H);
            a.append(img);
            eleHtml.append(a);
        } else {
            eleHtml.append(img);
        }

        eleHtml.attr("id", frmImage.MyPK);
        eleHtml.css('position', 'absolute').css('top', frmImage.Y).css('left', frmImage.X).css('width', frmImage.W).css('height', frmImage.H); ;
    } else if (frmImage.ImgAppType == 3)//二维码  手机
    {


    } else if (frmImage.ImgAppType == 1) {//暂不解析
        //电子签章  写后台
    }
    return eleHtml;
}

//初始化 IMAGE附件   L4418  问下周总
function figure_Template_ImageAth(frmImageAth) {
    return "";
}

//初始化 附件
function figure_Template_Attachment(frmAttachment) {
    var eleHtml = '';
    var ath = frmAttachment;
    if (ath.UploadType == 0) {//单附件上传 L4204
        return $('');
    }
    var src = "";
    src = "AttachmentUpload.htm?PKVal=" + pageData.OID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + ath.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";

    eleHtml += '<div>' + "<iframe style='width:" + ath.W + "px;height:" + ath.H + "px;' ID='Attach_" + ath.MyPK + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', ath.Y).css('left', ath.X).css('width', ath.W).css('height', ath.H);

    return eleHtml;
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


function addLoadFunction(id, eventName, method) {
    var js = "";
    js = "<script type='text/javascript' >";
    js += "function F" + id + "load() { ";
    js += "if (document.all) {";
    js += "document.getElementById('F" + id + "').attachEvent('on" + eventName + "',function(event){" + method + "('" + id + "');});";
    js += "} ";

    js += "else { ";
    js += "document.getElementById('F" + id + "').contentWindow.addEventListener('" + eventName + "',function(event){" + method + "('" + id + "');}, false); ";
    js += "} }";

    js += "</script>";
    return $(js);
}
var appPath = "/";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

//初始化从表
function figure_Template_Dtl(frmDtl) {
    var eleHtml = $("<DIV id='Fd" + frmDtl.No + "' style='position:absolute; left:" + frmDtl.X + "px; top:" + frmDtl.Y + "px; width:" + frmDtl.W + "px; height:" + frmDtl.H + "px;text-align: left;' >");
    var paras = this.pageData;
    var strs = "";
    for (var str in paras) {
        if (str == "EnsName" || str == "RefPKVal" || str == "IsReadonly")
            continue
        else
            strs += "&" + str + "=" + paras[str];
    }
    var src = "";
    ////switch (frmDtl.RowShowModel) {
    ////    case "0"://Table
    ////        if (pageData.IsReadOnly) {
    ////            src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=1" + strs;
    ////        } else {
    ////            src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=0" + strs;
    ////        }
    ////        break;
    ////    case "1"://
    ////        if (pageData.IsReadOnly)
    ////            src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=1" + strs;
    ////        else
    ////            src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=0" + strs;
    ////        break;
    ////}

    src = appPath + "WF/CCForm/DtlReadonly.htm?EnsName=" + frmDtl.No + "&RefPKVal=" + this.pageData.OID + "&IsReadonly=1" + strs;

    var eleIframe = '<iframe></iframe>';
    eleIframe = $("<iframe ID='F" + frmDtl.No + "' src='" + src +
                 "' frameborder=0  style='position:absolute;width:" + frmDtl.W + "px; height:" + frmDtl.H +
                 "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
    if (pageData.IsReadOnly) {

    } else {
        if (frmDtl.DtlSaveModel == 0) {
            eleHtml.append(addLoadFunction(frmDtl.No, "blur", "SaveDtl"));
            eleIframe.attr('onload', frmDtl.No + "load()");
        }
    }
    eleHtml.append(eleIframe);


    //added by liuxc,2017-1-10,此处前台JS中增加变量DtlsLoadedCount记录明细表的数量，用于加载完全部明细表的判断
    var js = "";
    if (!pageData.IsReadonly) {
        js = "<script type='text/javascript' >";
        js += " function SaveDtl(dtl) { ";
        js += "   GenerPageKVs(); //调用产生kvs ";
        js += "\n   var iframe = document.getElementById('F' + dtl );";
        js += "   if(iframe && iframe.contentWindow){ ";
        js += "      iframe.contentWindow.SaveDtlData(); ";
        js += "   } ";
        js += " } ";
        js += " function SaveM2M(dtl) { ";
        js += "   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
        js += "} ";
        js += "</script>";
        eleHtml.append($(js));
    }
    return eleHtml;
}

//初始化轨迹图
function figure_Template_FigureFlowChart(wf_node) {
    //轨迹图
    var sta = wf_node.FrmTrackSta;
    var x = wf_node.FrmTrack_X;
    var y = wf_node.FrmTrack_Y;
    var h = wf_node.FrmTrack_H;
    var w = wf_node.FrmTrack_W;
    if (sta == 0) {
        return $('');
    }
    var src = "/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Track";
    src += '&FK_Flow=' + pageData.FK_Flow;
    src += '&FK_Node=' + pageData.FK_Node;
    src += '&WorkID=' + pageData.OID;
    src += '&FID=' + pageData.FID;
    var eleHtml = '<div id="divtrack' + wf_node.NodeID + '">' + "<iframe id='track" + wf_node.NodeID + "' style='width:" + w + "px;height=" + h + "px;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//审核组件
function figure_Template_FigureFrmCheck(wf_node) {
    //审核组键FWCSta Sta,FWC_X X,FWC_Y Y,FWC_H H, FWC_W W from WF_Node
    var sta = wf_node.FWCSta;
    var x = wf_node.FWC_X;
    var y = wf_node.FWC_Y;
    var h = wf_node.FWC_H;
    var w = wf_node.FWC_W;
    if (sta == 0)
        return $('');

    var src = appPath + "WF/WorkOpt/WorkCheck.htm?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id="FFWC' + wf_node.NodeID + '">' + "<iframe style='width:100%;' id='FFWC" + wf_node.NodeID + "'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//子线程
function figure_Template_FigureThreadDtl(wf_node) {
    //FrmThreadSta Sta,FrmThread_X X,FrmThread_Y Y,FrmThread_H H,FrmThread_W
    var sta = wf_node.FrmThreadSta;
    var x = wf_node.FrmThread_X;
    var y = wf_node.FrmThread_Y;
    var h = wf_node.FrmThread_H;
    var w = wf_node.FrmThread_W;
    if (sta == 0)
        return $('');

    var src = appPath + "WF/WorkOpt/Thread.aspx?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVFT' + wf_node.NodeID + '>' + "<iframe id=FFT" + wf_node.NodeID + " style='width:100%;'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//子流程
function figure_Template_FigureSubFlowDtl(wf_node) {
    //SFSta Sta,SF_X X,SF_Y Y,SF_H H, SF_W W
    var sta = wf_node.SFSta;
    var x = wf_node.SF_X;
    var y = wf_node.SF_Y;
    var h = wf_node.SF_H;
    var w = wf_node.SF_W;
    if (sta == 0)
        return $('');

    var src = appPath + "WF/WorkOpt/SubFlow.aspx?s=2";
    var fwcOnload = "";
    var paras = '';

    paras += "&FID=" + pageData["FID"];
    paras += "&OID=" + pageData["WorkID"];
    paras += '&FK_Flow=' + pageData.FK_Flow;
    paras += '&FK_Node=' + pageData.FK_Node;
    paras += '&WorkID=' + pageData.OID;
    if (sta == 2)//只读
    {
        src += "&DoType=View";
    }
    else {
        fwcOnload = "onload= 'WC" + wf_node.NodeID + "load();'";
        $('body').append(addLoadFunction("WC" + wf_node.NodeID, "blur", "SaveDtl"));
    }
    src += "&r=q" + paras;
    var eleHtml = '<div id=DIVWC' + wf_node.NodeID + '>' + "<iframe id=FSF" + wf_node.NodeID + " style='width:" + w + "px';height:" + h + "px'    src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>" + '</div>';
    eleHtml = $(eleHtml);
    eleHtml.css('position', 'absolute').css('top', y).css('left', x).css('width', w).css('height', h);

    return eleHtml;
}

//初始化框架
function figure_Template_IFrame(fram) {
    var eleHtml = '';
    var src = dealWithUrl(fram.src) + "IsReadOnly=0";
    eleHtml = $('<div id="iframe' + fram.MyPK + '">' + '</div>');
    var iframe = $(+"<iframe  style='width:" + fram.W + "px; height:" + fram.H + "'     src='" + src + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling='no'></iframe>");

    eleHtml.css('position', 'absolute').css('top', fram.Y).css('left', fram.X).css('width', fram.W).css('height', fram.H);
    return frameHtml;
}

function figure_Template_MsgAlert(msgAlert) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span>' + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv)
    return eleHtml;
}

//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = fram.URL.replace(new RegExp(/(：)/g), ':');
    var params = '&FID=' + pageData.FID;
    params += '&WorkID=' + pageData.OID;
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') == 0) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (frmData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + frmData.MainTable[0][paramArr[1].substr(1)];
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
                        for (var ele in frmData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = frmData.MainTable[0][ele];
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
    }
    else {
        src += "?q=1";
    }
    return src;
}

var colVisibleJsonStr = ''
var jsonStr = '';