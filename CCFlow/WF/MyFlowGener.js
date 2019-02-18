

//从MyFlowFree2017.htm 中拿过过的.

var pageData = {};
var globalVarList = {};
var flowData = {};

//处理，表单没有加载完，就可以点击发送按钮.
var isLoadOk = false;

$(function () {

    initPageParam(); //初始化参数

    InitToolBar(); //工具栏.ajax

    GenerWorkNode(); //表单数据.ajax

    if ($("#Message").html() == "") {
        $(".Message").hide();
    }

    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });

    $('#btnMsgModalOK').bind('click', function () {
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
        window.close();
        if (window.parent != null && window.parent != undefined)
            window.parent.close();
    });


    $('#btnMsgModalOK1').bind('click', function () {
        window.close();
        if (window.parent != null && window.parent != undefined)
            window.parent.close();
        opener.window.focus();
    });
})


//从表在新建或者在打开行的时候，如果 EditModel 配置了使用卡片的模式显示一行数据的时候，就调用此方法.
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage) {
    // model=1 自由表单, model=2傻瓜表单.
    var pathName = document.location.pathname;
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    var projectName = pathName.length > 3 ? pathName.substring(1, 3) : "";
    var wWidth = $(window).width();
    var wHeight = $(window).height();
    if (wWidth > 1200) {
        wWidth = 1000;
    }
    if (wHeight < 600) {
        wHeight = 600;
    } else {
        wHeight = wHeight - 10;
    }

    if (projectName == "WF") {
        projectName = "";
    }
    if (plant == "JFlow")
        projectName = basePath;
    var url = projectName + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal;

    if (typeof ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal) === "function") {
        ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal)(url, "editSubGrid", '编辑', wWidth, wHeight, "icon-property", true, function () { }, null, function () {
            if (typeof InitPage === "function") {
                InitPage.call();
            } else {
                alert("请手动刷新表单");
            }
        }, "editSubGridDiv");
    } else {
        window.open(url);
    }
}

//单表单加载需要执行的函数
function CCFormLoaded() {
    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        //$('#topContentDiv').height(height);

        $(window).resize(function () {
            //$("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); 
            $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto");
        });
    }
    else {
        //新加
        //计算高度，展示滚动条
        //var height = $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff");
        var height = $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto");
        // $('#topContentDiv').height(height);

        $(window).resize(function () {
            //$("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff") ;
            $("#CCForm").height($(window).height() - 115 + "px").css("overflow-y", "auto");
        });
    }

    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }
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

$(function () {
    $('#MessageDiv').on('hide.bs.modal', function () {
        // alert('嘿，我听说您喜欢模态框...');
    })
});

function CloseOKBtn() {
    //   alert('嘿，我听说您喜欢模态框...');
}

//双击签名
function figure_Template_Siganture(SigantureID, val, type) {

    //先判断，是否存在签名图片
    var handler = new HttpHandler("BP.WF.HttpHandler.WF");
    handler.AddPara('no', val);
    data = handler.DoMethodReturnString("HasSealPic");

    //如果不存在，就显示当前人的姓名
    if (data.length > 0 && type == 0) {
        $("#TB_" + SigantureID).before(data);
        var obj = document.getElementById("Img" + SigantureID);
        var impParent = obj.parentNode; //获取img的父对象
        impParent.removeChild(obj);
    }
    else {
        var src = '../DataUser/Siganture/' + val + '.JPG';    //新图片地址
        document.getElementById("Img" + SigantureID).src = src;
    }
    isSigantureChecked = true;

    var sealData = new Entities("BP.Tools.WFSealDatas");
    sealData.Retrieve("OID", GetQueryString("WorkID"), "FK_Node", GetQueryString("FK_Node"), "SealData", GetQueryString("UserNo"));
    if (sealData.length > 0) {
        return;
    }
    else {
        sealData = new Entity("BP.Tools.WFSealData");
        sealData.MyPK = GetQueryString("WorkID") + "_" + GetQueryString("FK_Node") + "_" + val;
        sealData.OID = GetQueryString("WorkID");
        sealData.FK_Node = GetQueryString("FK_Node");
        sealData.SealData = val;
        sealData.Insert();
    }

}

//签字板
function figure_Template_HandWrite(HandWriteID, val) {
    var url = "CCForm/HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenEasyUiDialogExt(url, '签字板', 400, 300, false);
}

function setHandWriteSrc(HandWriteID, imagePath) {
    imagePath = "../" + imagePath.substring(imagePath.indexOf("DataUser"));
    document.getElementById("Img" + HandWriteID).src = "";
    $("#Img" + HandWriteID).attr("src", imagePath);
    // document.getElementById("Img" + HandWriteID).src = imagePath;
    $("#TB_" + HandWriteID).val(imagePath);
    $('#eudlg').dialog('close');
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
function OpenCC() {
    var url = $("#CC_Url").val();
    var v = window.showModalDialog(url, 'cc', 'scrollbars=yes;resizable=yes;center=yes;minimize:yes;maximize:yes;dialogHeight: 650px; dialogWidth: 850px; dialogTop: 100px; dialogLeft: 150px;');
    if (v == '1')
        return true;
    return false;
}

//原有的
function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
    var date = new Date();
    var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();

    var url = 'WebOffice/AttachOffice.htm?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

//关注 按钮.
function FocusBtn(btn, workid) {

    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("Focus"); //执行保存方法.
}

//确认 按钮.
function ConfirmBtn(btn, workid) {

    if (btn.value == '确认') {
        btn.value = '取消确认';
    }
    else {
        btn.value = '确认';
    }

    btn.value = (btn.value == '确认' ? '取消确认' : '确认');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("WorkID", workid);
    var data = handler.DoMethodReturnString("Confirm"); //执行保存方法.
     
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
    pageData.IsReadonly = GetQueryString("IsReadonly"); //如果是IsReadonly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
}

//将获取过来的URL参数转成URL中的参数形式  &
function pageParamToUrl() {
    var paramUrlStr = '';
    for (var param in pageData) {

        var val = pageData[param];
        if (val == null || val == undefined)
            continue;

        paramUrlStr += '&' + (param.indexOf('@') == 0 ? param.substring(1) : param) + '=' + pageData[param];
    }
    return paramUrlStr;
}

//设置附件为只读
function setAttachDisabled() {
    //附件设置
    var attachs = $('iframe[src*="Ath.htm"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadonly') == -1) {
            $(attach).attr('src', $(attach).attr('src') + "&IsReadonly=1");
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
    $('.Bar input').css('background', 'gray');
    $('.Bar input').attr('disabled', 'disabled');
}

function setToobarEnable() {
    //隐藏下方的功能按钮
    $('.Bar input').css('background', '');
    $('.Bar input').removeAttr('disabled');
}
//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}

function CheckMinMaxLength() {

    return true;

    var editor = document.activeEditor;
    if (editor) {
        var wordslen = editor.getContent().length,
            msg = "";

        if (wordslen > editor.MaxLen || wordslen < editor.MinLen) {
            msg += '@' + editor.BindFieldName + ' , 输入的值长度必须在:' + editor.MinLen + ', ' + editor.MaxLen + '之间. 现在输入是:' + wordslen;
        }

        if (msg != "") {
            alert(msg);
            return false;
        }
    }
    return true;
}

//保存
function Save() {

    //判断是否有保存按钮，如果有就需要安全性检查，否则就不执行，这种情况在会签下，发送的时候不做检查。
    var btn = document.getElementById('Btn_Save');
    if (btn != null) {
        //检查最小最大长度.
        var f = CheckMinMaxLength();
        if (f == false)
            return false;
    }

    if (checkAths() == false)
        return false;


    //必填项和正则表达式检查
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        //alert("请检查表单必填项和正则表达式");
        return false;
    }

    setToobarDisiable();

    //树形表单保存
    if (flowData) {
        var node = flowData.WF_Node[0];
        //   alert(node.FormType);
        if (node && node.FormType == 5) {
            if (OnTabChange("btnsave") == true) {
                //判断内容是否保存到待办
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddPara("FK_Flow", pageData.FK_Flow);
                handler.AddPara("FK_Node", pageData.FK_Node);
                handler.AddPara("WorkID", pageData.WorkID);
                handler.DoMethodReturnString("SaveFlow_ToDraftRole");
            }
            setToobarEnable();
            return;
        }
    }

    var json = getFormData(true, true);
    //alert(json);

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(json);
    var data = handler.DoMethodReturnString("Save"); //执行保存方法.
//    $.ajax({
//        type: 'post',
//        async: true,
//        data: json,
//        url: MyFlow + "?DoType=Save",
//        dataType: 'html',
//        success: function (data) {
            setToobarEnable();
            //刷新 从表的IFRAME
            var dtls = $('.Fdtl');
            $.each(dtls, function (i, dtl) {
                $(dtl).attr('src', $(dtl).attr('src'));
            });

            if (data.indexOf('保存成功') != 0 || data.indexOf('err@') == 0) {
                $('#Message').html(data.substring(4, data.length));
                $('#MessageDiv').modal().show();
            }
//        }
//    });
    
}

//调用后，就关闭刷新按钮.
function returnWorkWindowClose(data) {

    if (data == "" || data == "取消") {
        $('#returnWorkModal').modal('hide');
        setToobarEnable();
        return;
    }

    $('#returnWorkModal').modal('hide');
    //通过下发送按钮旁的下拉框选择下一个节点
    if (data.indexOf('SaveOK@') == 0) {
        //说明保存人员成功,开始调用发送按钮.
        var toNode = 0;
        //含有发送节点 且接收
        if ($('#DDL_ToNode').length > 0) {
            var selectToNode = $('#DDL_ToNode  option:selected').data();
            toNode = selectToNode.No;
        }

        execSend(toNode);
        //$('[name=Send]:visible').click();
        return;
    } else {//可以重新打开接收人窗口
        winSelectAccepter = null;
    }

    if (data.indexOf('err@') == 0 || data == "取消") {//发送时发生错误
        $('#Message').html(data);
        $('#MessageDiv').modal().show();
        return;
    }

    OptSuc(data);
}


//刷新子流程
function refSubSubFlowIframe() {
    var iframe = $('iframe[src*="SubFlow.aspx"]');
    //iframe[0].contentWindow.location.reload();
    iframe[0].contentWindow.location.href = iframe[0].src;
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
function InitDDLOperation(flowData, mapAttr, defVal) {

    var operations = '';

    //外键类型的.
    if (mapAttr.LGType == 2) {

        var data = flowData[mapAttr.KeyOfEn];

        if (data == undefined)
            data = flowData[mapAttr.UIBindKey];

        if (data == undefined) {
            var sfTable = new Entity("BP.Sys.SFTable", mapAttr.UIBindKey);
            if (sfTable != null && sfTable != "") {
                var selectStatement = sfTable.SelectStatement;
                var srcType = sfTable.SrcType;
                //Handler 获取外部数据源
                if (srcType == 5)
                    data = DBAccess.RunDBSrc(selectStatement, 1);
                //JavaScript获取外部数据源
                if (srcType == 6)
                    data = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
            }
        }

        if (data == undefined) {
            alert('没有获得约定的数据源..' + mapAttr.KeyOfEn + " " + mapAttr.UIBindKey);
            return;
        }

        $.each(data, function (i, obj) {
            operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
        });


        return operations;
    }

    //枚举类型的.
    if (mapAttr.LGType == 1) {

        var enums = flowData.Sys_Enum;
        enums = $.grep(enums, function (value) {
            return value.EnumKey == mapAttr.UIBindKey;
        });


        $.each(enums, function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
        });
        return operations;
    }



    //外部数据源类型 FrmGener.js.InitDDLOperation
    if (mapAttr.LGType == 0) {

        //如果是一个函数.
        var fn;
        try {
            if (mapAttr.UIBindKey) {
                fn = eval(mapAttr.UIBindKey);
            }
        } catch (e) {
            //alert(e);
        }

        if (typeof fn == "function") {
            $.each(fn.call(), function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        var data = flowData[mapAttr.KeyOfEn];
        if (data == undefined)
            data = flowData[mapAttr.UIBindKey];

        if (data == undefined) {
            var sfTable = new Entity("BP.Sys.SFTable", mapAttr.UIBindKey);
            if (sfTable != null && sfTable != "") {
                var selectStatement = sfTable.SelectStatement;
                var srcType = sfTable.SrcType;
                // SQL获取外部数据源
                if (srcType == 3)
                    data = DBAccess.RunDBSrc(selectStatement, 0);
                //WebService 获取外部数据源
                if (srcType == 5) {
                    data = SFTaleHandler(selectStatement);
                    if (data == "false") {
                        alert(mapAttr.KeyOfEn + "外部数据源获取错误");
                        return;
                    }

                    data = JSON.parse(data);
                }
                //JavaScript获取外部数据源
                if (srcType == 6)
                    data = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
            }
        }
        if (data == undefined) {
            alert('没有获得约定的数据源..' + mapAttr.KeyOfEn + " " + mapAttr.UIBindKey);
            return;
        }

        $.each(data, function (i, obj) {
            operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";

        });
        operations += "<option value=''>- 请选择 -</option>";
        return operations;

        if (mapAttr.UIIsEnable == 0) {

            alert('不可编辑');
            operations = "<option  value='" + defVal + "'>" + defVal + "</option>";
            return operations;
        }


    }
}


//填充默认数据
function ConvertDefVal(flowData, defVal, keyOfEn) {
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

    var mainTable = flowData.MainTable[0];


    //通过MAINTABLE返回的参数
    for (var ele in mainTable) {
        if (keyOfEn == ele && mainTable != '') {
            //console.info(ele + "==" + flowData.MainTable[0][ele]);
            result = mainTable[ele];
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    //console.info(defVal+"=="+keyOfEn+"=="+result);
    var result = unescape(result);

    if (result == "null")
        result = "";

    return result;
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

        if (ele.split('=')[0].indexOf('DDL_') == 0) {

            var ctrlID = ele.split('=')[0];

            var item = $("select[name='" + ctrlID + "'] option[selected]").text();

            var mystr = '';

            //mystr = ctrlID.substring(4) + 'T=' + item;
            mystr = ctrlID + 'T=' + item;

            //var mystr = ctrlID + 'T=' + item;

            formArrResult.push(mystr);
        }

        formArrResult.push(ele);
    });



    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {

        var name = $(disabledEle).attr('id');

        switch (disabledEle.tagName.toUpperCase()) {

            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult.push(name + '=' + ($(disabledEle).is(':checked') ? 1 : 0));
                        break;
                    case "TEXT": //文本框
                        formArrResult.push(name + '=' + $(disabledEle).val());
                        break;
                    case "RADIO": //单选钮
                        name = $(disabledEle).attr('name');
                        var eleResult = name + '=' + $('[name="' + name + '"]:checked').val();
                        if ($.inArray(eleResult, formArrResult) == -1) {
                            formArrResult.push(eleResult);
                        }
                        break;
                }
                break;
            //下拉框       
            case "SELECT":
                formArrResult.push(name + '=' + $(disabledEle).children('option:checked').val());
                formArrResult.push(name + 'T=' + $(disabledEle).children('option:checked').text());
                break;

            //文本区域               
            case "TEXTAREA":
                formArrResult.push(name + '=' + $(disabledEle).val());
                break;
        }
    });

    //获取树形结构的表单值
    var combotrees = $(".easyui-combotree");
    $.each(combotrees, function (i, combotree) {
        var name = $(combotree).attr('id');
        var tree = $('#' + name).combotree('tree');
        //获取当前选中的节点
        var data = tree.tree('getSelected');
        if (data != null) {
            formArrResult.push(name + '=' + data.id);
            formArrResult.push(name + 'T=' + data.text);
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

    //为了复选框  合并一下值  复选框的值以  ，号分割
    //用& 符号截取数据
    var formDataArr = formData.split('&');

    var formDataResultObj = {};
    $.each(formDataArr, function (i, formDataObj) {
        //计算出等号的INDEX
        var indexOfEqual = formDataObj.indexOf('=');
        var objectKey = formDataObj.substr(0, indexOfEqual);
        var objectValue = formDataObj.substr(indexOfEqual + 1);
        if (formDataResultObj[objectKey] == undefined) {
            formDataResultObj[objectKey] = objectValue;
        } else {
            formDataResultObj[objectKey] = formDataResultObj[objectKey] + ',' + objectValue;
        }
    });

    var formdataResultStr = '';
    for (var ele in formDataResultObj) {
        formdataResultStr = formdataResultStr + ele + '=' + formDataResultObj[ele] + '&';
    }

    // 处理没有选择的文本框.
    //获得checkBoxIDs 格式为: CB_IsXX,CB_IsYY,
    var ids = GenerCheckIDs();

    if (ids) {
        var scores = ids.split(",");
        var arrLength = scores.length;
        var sum = 0;
        var average = null;
        for (var i = 0; i < arrLength; i++) {
            var field = scores[i];
            var index = formdataResultStr.indexOf(field);
            if (index == -1) {
                formdataResultStr += '&' + field + '=0';
            }
        }
    }

    formdataResultStr = formdataResultStr.replace('&&', '&');

    return formdataResultStr;
}


//获得所有的checkbox 的id组成一个string用逗号分开, 以方便后台接受的值保存.
function GenerCheckIDs() {

    var checkBoxIDs = "";
    var arrObj = document.all;

    for (var i = 0; i < arrObj.length; i++) {

        if (arrObj[i].type != 'checkbox')
            continue;

        var cid = arrObj[i].id;
        if (cid == null || cid == "" || cid == '')
            continue;

        checkBoxIDs += arrObj[i].id + ',';
    }
    return checkBoxIDs;
}

//发送
function Send(isHuiQian) {

    if (CheckFWC() == false)
        return false;

    if (checkAths() == false)
        return false;


    //检查最小最大长度.
    var f = CheckMinMaxLength();
    if (f == false)
        return false;

    //必填项和正则表达式检查.
    if (checkBlanks() == false) {
        alert("检查必填项出现错误，边框变红颜色的是否填写完整？");
        return;
    }

    if (checkReg() == false) {
        alert("发送错误:请检查字段边框变红颜色的是否填写完整？");
        return;
    }

    window.hasClickSend = true; //标志用来刷新待办.

    var toNodeID = 0;

    //含有发送节点 且接收
    if ($('#DDL_ToNode').length > 0) {

        var selectToNode = $('#DDL_ToNode  option:selected').data();
        toNodeID = selectToNode.No;

        if (selectToNode.IsSelectEmps == "1") { //跳到选择接收人窗口

            Save(); //执行保存.

            if (isHuiQian == true) {
                initModal("HuiQian", toNodeID);
                $('#returnWorkModal').modal().show();

            } else {
                initModal("sendAccepter", toNodeID);
                $('#returnWorkModal').modal().show();
            }
            return false;

        } else {

            if (isHuiQian == true) {

                Save(); //执行保存.
                initModal("HuiQian", toNodeID);
                $('#returnWorkModal').modal().show();
                return false;
            }
        }
    }

    //执行发送.
    execSend(toNodeID);
}

function execSend(toNodeID) {

    //先设置按钮等不可用.
    setToobarDisiable();
    //树形表单保存
    if (flowData) {
        var node = flowData.WF_Node[0];
        if (node && node.FormType == 5) {
            OnTabChange("btnsave");
        }
    }

    //组织数据.
    var dataStrs = getFormData(true, true) + "&ToNode=" + toNodeID;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(dataStrs);
    var data = handler.DoMethodReturnString("Send"); //执行保存方法.

    if (data.indexOf('err@') == 0) { //发送时发生错误
        $('#Message').html(data.substring(4, data.length));
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
        window.location.href = url;
        return;
    }



    if (data.indexOf('url@') == 0) {  //发送成功时转到指定的URL 

        if (data.indexOf('Accepter') != 0 && data.indexOf('AccepterGener') == -1) {

            //求出来 url里面的FK_Node=xxxx 
            var params = data.split("&");

            for (var i = 0; i < params.length; i++) {
                if (params[i].indexOf("ToNode") == -1)
                    continue;

                toNodeID = params[i].split("=")[1];
                break;
            }

            //   var toNode = new Entity("BP.WF.Node",toNodeID)
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
function OptSuc(msg) {

    // window.location.href = "/WF/MyFlowInfo.aspx";
    // $('#MessageDiv').modal().hide();

    if ($('#returnWorkModal:hidden').length == 0 && $('#returnWorkModal').length > 0) {
        $('#returnWorkModal').modal().hide()
    }
    msg = msg.replace("@查看<img src='/WF/Img/Btn/PrintWorkRpt.gif' >", '')

    $("#msgModalContent").html(msg.replace(/@/g, '<br/>'));
    var trackA = $('#msgModalContent a:contains("工作轨迹")');
    var trackImg = $('#msgModalContent img[src*="PrintWorkRpt.gif"]');
    trackA.remove();
    trackImg.remove();

    $("#msgModal").modal().show();
}

//初始化发送节点下拉框
function InitToNodeDDL(flowData) {

    if (flowData.ToNodes == undefined)
        return;

    if (flowData.ToNodes.length == 0)
        return;

    //如果没有发送按钮，就让其刷新,说明加载不同步.
    var btn = $('[name=Send]');
    if (btn == null || btn == undefined) {
        window.location.href = window.location.href;
        return;
    }

    // $('[value=发送]').
    var toNodeDDL = $('<select style="width:auto;" id="DDL_ToNode"></select>');
    $.each(flowData.ToNodes, function (i, toNode) {
        //IsSelectEmps: "1"
        //Name: "节点2"
        //No: "702"

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

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInfo() {
    var rbs = flowData.Sys_FrmRB;
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
                            case "1": //可用
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                ele.removeAttr('disabled');


                                break;
                            case "2": //可见
                                if (labDiv.css('display').toUpperCase() == "NONE" && ele[0].id.indexOf('DDL_') == 0) {
                                    needShowDDLids.push(ele[0].id);
                                }

                                labDiv.css('display', 'block');
                                eleDiv.css('display', 'block');
                                break;
                            case "3": //不可见
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
function ShowTextBoxNoticeInfo() {
    var mapAttr = flowData.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {
            var mapAttr = flowData.Sys_MapAttr;

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

//检查附件数量.
function checkAths() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('Ath1');

    if (frm == null || frm == undefined) {
        return true;
        //alert('系统错误,没有找到SelfForm的ID.');
    }

    //执行保存.
    return frm.contentWindow.CheckAthNum();
}


//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {

    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput
    //var lbs = $('[class*=col-md-1] label:contains(*)');
    var lbs = $('.mustInput'); //获得所有的class=mustInput的元素.

    $.each(lbs, function (i, obj) {

        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
        } else {
            return;
        }

        var keyofen = $(obj).data().keyofen;
        var ele = $('[id$=_' + keyofen + ']');
        if (ele.length == 0)
            return;

        $.each(ele, function (i, obj) {
            var eleM = $(obj);
            switch (eleM[0].tagName.toUpperCase()) {
                case "INPUT":
                    if (eleM.attr('type') == "text") {
                        if (eleM.val() == "") {
                            checkBlankResult = false;
                            eleM.addClass('errorInput');
                        } else {
                            eleM.removeClass('errorInput');
                        }
                    }
                    break;
                case "SELECT":
                    if (eleM.val() == "" || eleM.children('option:checked').text() == "*请选择") {
                        checkBlankResult = false;
                        eleM.addClass('errorInput');
                    } else {
                        eleM.removeClass('errorInput');
                    }
                    break;
                case "TEXTAREA":
                    if (eleM.val() == "") {
                        checkBlankResult = false;
                        eleM.addClass('errorInput');
                    } else {
                        eleM.removeClass('errorInput');
                    }
                    break;
            }
        });

    });


    //2.对 UMEditor 中的必填项检查
    if (document.activeEditor != null && document.activeEditor.$body != null) {
        /* #warning 这个地方有问题.*/

        //        var ele = document.activeEditor.$body;
        //        if (ele != null && document.activeEditor.getPlainTxt().trim() === "") {
        //            checkBlankResult = false;
        //            ele.addClass('errorInput');
        //        } else {
        //            ele.removeClass('errorInput');
        //        }
    }

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

// 杨玉慧
function GenerWorkNode() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("GenerWorkNode"); //执行保存方法.


    if (data.indexOf('err@') == 0) {
        alert(data);
        // console.log(data);
        return;
    }

    try {

        flowData = JSON.parse(data);

    } catch (err) {
        //console.log(data);
        alert(" GenerWorkNode转换JSON失败,请查看控制台日志,或者联系管理员.");
        return;
    }

    var node = flowData.WF_Node[0];
    var gfs = flowData.Sys_MapAttr;
    //console.log(gfs);

    //设置标题.
    document.title = node.FlowName + ',' + node.Name; // "业务流程管理（BPM）平台";

    //循环之前的提示信息.
    var info = "";
    for (var i = 0; i < flowData.AlertMsg.length; i++) {
        var alertMsg = flowData.AlertMsg[i];
        var alertMsgEle = figure_Template_MsgAlert(alertMsg, i);
        $('#Message').append(alertMsgEle);
        $('#Message').append($('<hr/>'));
    }

    if (flowData.AlertMsg.length != 0) {
        $('#MessageDiv').modal().show();
    }

    ShowNoticeInfo();

    ShowTextBoxNoticeInfo();

    //发送旁边下拉框 edit by zhoupeng 放到这里是为了解决加载不同步的问题.
    InitToNodeDDL(flowData);

    if (node.FormType == 11) {
        //获得配置信息.
        var frmNode = flowData["FrmNode"];
        if (frmNode) {
            frmNode = frmNode[0];
            if (frmNode.FrmSln == 1)
                pageData.IsReadonly = 1
        }
    }
    //判断类型不同的类型不同的解析表单. 处理中间部分的表单展示.

    if (node.FormType == 5) {
        GenerTreeFrm(flowData); /*树形表单*/
        return;
    }

    if (node.FormType == 0 || node.FormType == 10) {
        GenerFoolFrm(flowData); //傻瓜表单.
    }

    if (node.FormType == 1) {
        GenerFreeFrm(flowData);  //自由表单.
    }

    //2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    if (node.FormType == 11) {
        GenerFreeFrm(flowData);
    }

    //公文表单
    if (node.FormType == 7) {
        var btnOffice = new Entity("BP.WF.Template.BtnLabExtWebOffice", pageData.FK_Node);
        if (btnOffice.WebOfficeFrmModel == 1)
            GenerFreeFrm(flowData);  //自由表单.
        else
            GenerFoolFrm(flowData); //傻瓜表单.
    }

    $.parser.parse("#CCForm");

    //以下代码是 傻瓜表单与自由表单, 公共方法.
    var local = window.location.href;

    var frm = document.forms["divCCForm"];
    if (plant == "CCFlow")
        frm.action = "MyFlow.ashx?method=login";
    else
        frm.action = MyFlow + "?method=login";

    //单表单加载后执行.
    CCFormLoaded();

    //装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(flowData);

    //初始化Sys_MapData
    var h = flowData.Sys_MapData[0].FrmH;
    var w = flowData.Sys_MapData[0].FrmW;

    // $('#topContentDiv').height(h);
    $('#topContentDiv').width(w);
    $('.Bar').width(w + 15);
    $('#lastOptMsg').width(w + 15);

    //2018.1.1 新增加的类型, 流程独立表单， 为了方便期间都按照自由表单计算了.
    if (node.FormType == 11) {
        //获得配置信息.
        var frmNode = flowData["FrmNode"];
        if (frmNode) {
            frmNode = frmNode[0];
            if (frmNode.FrmSln == 1) {
                /*只读的方案.*/
                //alert("把表单设置为只读.");
                SetFrmReadonly();
                //alert('ssssssssssss');
            }

            if (frmNode.FrmSln != 1)
            //处理下拉框级联等扩展信息
                AfterBindEn_DealMapExt(flowData);
        }
    } else {
        //处理下拉框级联等扩展信息
        AfterBindEn_DealMapExt(flowData);
    }

    Common.MaxLengthError();







    var marginLeft = $('#topContentDiv').css('margin-left');
    marginLeft = marginLeft.replace('px', '');

    marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
    $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
    //原有的

    //为 DISABLED 的 TEXTAREA 加TITLE 
    var disabledTextAreas = $('#divCCForm textarea:disabled');
    $.each(disabledTextAreas, function (i, obj) {
        $(obj).attr('title', $(obj).val());
    })

    ////加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = flowData.Sys_MapData[0].No;
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
        s.src = "../DataUser/JSLibData/" + enName + ".js";
        var tmp = document.getElementsByTagName('script')[0];
        tmp.parentNode.insertBefore(s, tmp);
    }
    catch (err) {

    }

    $(".pimg").on("dblclick", function () {
        var _this = $(this); //将当前的pimg元素作为_this传入函数  
        imgShow("#outerdiv", "#innerdiv", "#bigimg", _this);
    });


    //给富文本创建编辑器
    if (document.BindEditorMapAttr) {
        //给富文本 创建编辑器
        var editor = document.activeEditor = UM.getEditor('editor', {
            'autoHeightEnabled': false,
            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
            'initialFrameWidth': document.BindEditorMapAttr.UIWidth
        });
        var height = document.BindEditorMapAttr.UIHeight;
        $(".edui-container").css("height", height);

        if (editor) {

            editor.MaxLen = document.BindEditorMapAttr.MaxLen;
            editor.MinLen = document.BindEditorMapAttr.MinLen;
            editor.BindField = document.BindEditorMapAttr.KeyOfEn;
            editor.BindFieldName = document.BindEditorMapAttr.Name;

            //调整样式,让必选的红色 * 随后垂直居中
            $(editor.container).css({ "display": "inline-block", "margin-right": "4px", "vertical-align": "middle" });
        }
    }
    //给富文本创建编辑器
}

function SetFrmReadonly() {


    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);

    $('#Btn_Save').attr('disabled', true);
}

function sel(n, KeyOfEn, FK_MapData) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
    frmEleDB.FK_MapData = FK_MapData;
    frmEleDB.EleID = KeyOfEn;
    frmEleDB.RefPKVal = (pageData.WorkID || pageData.OID || "");
    frmEleDB.Tag1 = n;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}

function unsel(n, KeyOfEn) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
    frmEleDB.Delete();
}
// V
function getMapExt(Sys_MapExt, KeyOfEn) {
    var ext = {};
    for (var p in Sys_MapExt) {
        if (KeyOfEn == Sys_MapExt[p].AttrOfOper) {
            ext = Sys_MapExt[p];
            break;
        }
    }
    return ext;
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

var appPath = "../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量

function figure_Template_MsgAlert(msgAlert, i) {
    var eleHtml = $('<div></div>');
    var titleSpan = $('<span class="titleAlertSpan"> ' + (parseInt(i) + 1) + "&nbsp;&nbsp;&nbsp;" + msgAlert.Title + '</span>');
    var msgDiv = $('<div>' + msgAlert.Msg + '</div>');
    eleHtml.append(titleSpan).append(msgDiv)
    return eleHtml;
}

//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = fram.URL.replace(new RegExp(/(：)/g), ':');
    var params = '&FID=' + pageData.FID;
    params += '&WorkID=' + pageData.WorkID;
    if (src.indexOf("?") > 0) {
        var params = getQueryStringFromUrl(src);
        if (params != null && params.length > 0) {
            $.each(params, function (i, param) {
                if (param.indexOf('@') == 0) {//是需要替换的参数
                    paramArr = param.split('=');
                    if (paramArr.length == 2 && paramArr[1].indexOf('@') == 0) {
                        if (paramArr[1].indexOf('@WebUser.') == 0) {
                            params[i] = paramArr[0].substring(1) + "=" + flowData.MainTable[0][paramArr[1].substr('@WebUser.'.length)];
                        }
                        if (flowData.MainTable[0][paramArr[1].substr(1)] != undefined) {
                            params[i] = paramArr[0].substring(1) + "=" + flowData.MainTable[0][paramArr[1].substr(1)];
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
                        for (var ele in flowData.MainTable[0]) {
                            if (paramArr[0].substring(1) == ele) {
                                result = flowData.MainTable[0][ele];
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

/*
公共的工作处理器js. 
1. 该js的方法都是从各个类抽取出来的.
2. MyFlowFool.htm, MyFlowFree.htm, MyFlowSelfForm.htm 引用它.
3. 用于处理流程业务逻辑，表单业务逻辑.
*/


//初始化按钮
//var MyFlow = "MyFlow.ashx";
function InitToolBar() {

    var href = window.location.href;
    var urlParam = href.substring(href.indexOf('?') + 1, href.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData(urlParam);
    var data = handler.DoMethodReturnString("InitToolBar"); //执行保存方法.

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
           
            DeleteFlow();
        });
    }


}

/* ss */
function OpenOffice() {

    //string paras = this.RequestParas;
    //            if (paras.Contains("SID") == false)
    //                paras += "&SID=" + BP.Web.WebUser.SID;
    //		    
    //            if (paras.Contains("UserNo") == false)
    //                paras += "&UserNo=" + BP.Web.WebUser.No;
    //            if (string.IsNullOrWhiteSpace(BP.Sys.SystemConfig.AppSettings["IsAutoTesting"]))
    //                paras += "&IsAutoTesting=0";
    //            else
    //                paras += "&IsAutoTesting=" + Convert.ToInt32(BP.Sys.SystemConfig.AppSettings["IsAutoTesting"]); //用于自动化测试
    //                        
    //			paras = paras.Replace("&", ",");



    var paras = "WorkID=" + GetQueryString("WorkID") + ",";
    paras += "FK_Flow=" + GetQueryString("FK_Flow") + ",";
    paras += "FK_Node=" + GetQueryString("FK_Node") + ",";

    var webUser = new WebUser();
    paras += "UserNo=" + webUser.No + ",";
    paras += "SID=" + webUser.SID + ",";

    var local = window.location.protocol + "//" + window.location.hostname + ":" + window.location.port;

    var urlWS = local + "/WF/CCForm/CCFormAPI.asmx";
    var url = "httpCCWord://-fromccflow,App=FrmExcel," + paras + "WSUrl=" + urlWS;
    alert(url);
    window.open(url);
}


//初始化退回、移交、加签窗口
function initModal(modalType, toNode) {

    //初始化退回窗口的SRC.
    var returnWorkModalHtml = '<div class="modal fade" id="returnWorkModal" data-backdrop="static">' +
       '<div class="modal-dialog">'
           + '<div class="modal-content" style="border-radius:0px;width:900px;text-align:left;">'
              + '<div class="modal-header">'
                  + '<button type="button" style="color:white;float: right;background: transparent;border: none;" data-dismiss="modal" aria-hidden="true">&times;</button>'
                   + '<h4 class="modal-title" id="modalHeader">工作退回</h4>'
               + '</div>'
               + '<div class="modal-body" style="margin:0px;padding:0px">'
                   + '<iframe style="width:100%;border:0px;height:450px;" id="iframeReturnWorkForm" name="iframeReturnWorkForm"></iframe>'
               + '</div>'
           + '</div><!-- /.modal-content -->'
       + '</div><!-- /.modal-dialog -->'
   + '</div>';

    $('body').append($(returnWorkModalHtml));
    $("#returnWorkModal").on('hide.bs.modal', function () {
        setToobarEnable();
    });
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

                $('#modalHeader').text("发送到节点");
                modalIframeSrc = "./WorkOpt/Accepter.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.WorkID + "&FK_Flow=" + pageData.FK_Flow + "&ToNode=" + toNode + "&s=" + Math.random()
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

// 检查审核组件,是否加盖了电子签章？
function CheckFWC() {

    var frm = document.getElementById('FWC');
    if (frm == null || frm == undefined)
        return true;

    return frm.contentWindow.IsCanSendWork();
}


//结束流程.
function DoStop(msg, flowNo, workid) {

    if (confirm('您确定要执行 [' + msg + '] ?') == false)
        return;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("MyFlow_StopFlow");
    alert(msg);

    if (msg.indexOf('err@') == 0)
        return;

    if (window.parent != null) {
        //@袁丽娜 如何刷新父窗口.
        //window.parent.ref
    }
    window.close();

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

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddPara("FK_Flow", fk_flow);
    handler.AddPara("WorkID", workid);

    var data = handler.DoMethodReturnString("DelSubFlow"); //删除子流程..
    alert(data);
    window.location.href = window.location.href;
     
}

//. 保存嵌入式表单. add 2015-01-22 for GaoLing.
function SaveSelfFrom() {

    // 不支持火狐浏览器。
    var frm = document.getElementById('SelfForm');
    if (frm == null) {
        alert('系统错误,没有找到SelfForm的ID.');
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
/*************************************  以下的方法方便对独立表单模式下的工作处理器，嵌入方式的控件取值与赋值. ***********************************************/
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

// 获取TB值
function ReqTB(tbID) {
    var v = document.getElementById('TB_' + tbID).value;
    if (v == null) {
        alert('没有找到ID=' + tbID + '的文本框控件.');
    }
    return v;
}
// 获取CheckBox值
function ReqCB(cbID) {
    var v = document.getElementById('CB_' + cbID).value;
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
    var ctrl = document.getElementById('TB_' + ctrlID);
    if (ctrl) {
        ctrl.value = val;
    }

    ctrl = document.getElementById('DDL_' + ctrlID);
    if (ctrl) {
        ctrl.value = val;
    }

    ctrl = document.getElementById('CB_' + ctrlID);
    if (ctrl) {
        ctrl.value = val;
    }
}