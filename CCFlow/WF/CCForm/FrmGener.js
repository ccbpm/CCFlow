
/*
说明:
1.  围绕该文件工作的有两个js文件，分别是。 FrmFool.js
2.  对于傻瓜表单自由表单的，展现方式不同以外其他的都相同.
3.  相同的部分写入到了该文件里，不同的部分分别在不同的两个js文件里.
4.  MapExt2016.js 文件是一个公用的文件，用于处理扩展业务逻辑的，它在多个地方别调用了.
*/

var colVisibleJsonStr = ''
var jsonStr = '';
var IsChange = false;
var webUser = null;
//初始化函数
$(function () {

    webUser = new WebUser();

    $("#CCForm").unbind().on('click', function () {
        Change(frmData);
    });

    initPageParam(); //初始化参数.

    //构造表单.
    GenerFrm(); //表单数据.

    //设置不可以用.
    var isReadonly = GetQueryString("IsReadonly");
    if (isReadonly == 1) {
        SetReadonly();
    }

    if (parent != null && parent.document.getElementById('MainFrames') != undefined) {
        //计算高度，展示滚动条
        var height = $(parent.document.getElementById('MainFrames')).height() - 110;
        $('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height($(window).height() - 150 + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); ;
        });
    }
    else {
        //新加
        //计算高度，展示滚动条
        var height = $(window).height() - 150;
        $("#CCForm").height(height + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff");
        $('#topContentDiv').height(height);

        $(window).resize(function () {
            $("#CCForm").height(height + "px").css("overflow-y", "auto").css("scrollbar-face-color", "#fff"); ;
        });
    }
    function movetb() {
        var move;
        $("#nav").css("top", top);
    }
    $('#btnCloseMsg').bind('click', function () {
        $('.Message').hide();
    });

    setAttachDisabled();
    setFormEleDisabled();

    SetHegiht();
    //打开表单检查正则表达式
    if (typeof FormOnLoadCheckIsNull != 'undefined' && FormOnLoadCheckIsNull instanceof Function) {
        FormOnLoadCheckIsNull();
    }
});

function numonly(e) {
    if (navigator.userAgent.indexOf("Firefox") > 0) {
        var code;
        if (window.event) {
            code = event.keyCode;
        } else {
            code = e.which;
        }
        if (code != 8 && code != 9 && code != 13 && code != 16 && code != 17 && code != 18) {
            if (code < 45 || code > 57) {
                if (window.event) {
                    event.returnValue = false;
                } else {
                    e.preventDefault();
                    return false;
                }
            }
        }
    }
}

//设置不可以用.
function SetReadonly() {
    //设置保存按钮不可以用.
    $("#Btn_Save").attr("disabled", true);
    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);
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

//从表在新建或者在打开行的时候，如果 EditModel 配置了使用卡片的模式显示一行数据的时候，就调用此方法.
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage) {
    // model=1 自由表单, model=2傻瓜表单.
    var pathName = document.location.pathname;
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    if (projectName.startsWith("/WF")) {
        projectName = "";
    }
    var url = projectName + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal;
    if (typeof ((parent && parent.OpenEasyUiDialog) || OpenEasyUiDialog) === "function") {
        ((parent && parent.OpenEasyUiDialog) || OpenEasyUiDialog)(url, "editSubGrid", '编辑', 1000, 550, "icon-property", false, null, null, null, function () {
            if (typeof InitPage === "function") {
                InitPage.call();
            } else {
                alert("请手动刷新表单");
            }
        });
    } else {
        window.open(url);
    }
}

var frmData = null;
//将v1版本表单元素转换为v2 杨玉慧  silverlight 自由表单转化为H5表单.
function GenerFrm() {

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
        url: Handler + "?DoType=FrmGener_Init&m=" + Math.random() + "&" + urlParam,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                alert('装载表单出错,请查看控制台console,或者反馈给管理员.');
                alert(data);
                console.log(data);
                return;
            }

            try {
                frmData = JSON.parse(data);
            }
            catch (err) {
                alert(" frmData数据转换JSON失败:" + data);
                console.log(data);
                return;
            }

            //获得sys_mapdata.
            var mapData = frmData["Sys_MapData"][0];

            //初始化Sys_MapData
            var h = mapData.FrmH;
            var w = mapData.FrmW;
            if (h <= 1200)
                h = 1200;

            //表单名称.
            document.title = mapData.Name;

            $('#divCCForm').height(h);

            $('#topContentDiv').height(h);
            $('#topContentDiv').width(w);
            $('.Bar').width(w + 15);

            var marginLeft = $('#topContentDiv').css('margin-left');
            marginLeft = parseFloat(marginLeft.substr(0, marginLeft.length - 2)) + 50;
            $('#topContentDiv i').css('left', marginLeft.toString() + 'px');
            $('#CCForm').html('');

            //根据表单类型不同生成表单.

            var isTest = GetQueryString("IsTest");

            if (isTest == "1") {

                var frmType = GetQueryString("FrmType");
                if (frmType == 'FreeFrm')
                    GenerFreeFrm(mapData, frmData); //自由表单.
                else
                    GenerFoolFrm(mapData, frmData); //生成傻瓜表单.

            } else {
                if (mapData.FrmType == 0)
                    GenerFoolFrm(mapData, frmData); //生成傻瓜表单.
                else
                    GenerFreeFrm(mapData, frmData); //自由表单.
            }

            $.parser.parse("#CCForm");
            var isReadonly = GetQueryString("IsReadonly");

            //原有的。
            //为 DISABLED 的 TEXTAREA 加TITLE 
            var disabledTextAreas = $('#divCCForm textarea:disabled');
            $.each(disabledTextAreas, function (i, obj) {
                $(obj).attr('title', $(obj).val());
            })


            //根据NAME 设置ID的值
            var inputs = $('[name]');
            $.each(inputs, function (i, obj) {
                if ($(obj).attr("id") == undefined || $(obj).attr("id") == '') {
                    $(obj).attr("id", $(obj).attr("name"));
                }
            })

            // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
            var enName = frmData.Sys_MapData[0].No;
            try {
                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "../../DataUser/JSLibData/" + enName + "_Self.js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }

            try {
                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = "../../DataUser/JSLibData/" + enName + ".js";
                var tmp = document.getElementsByTagName('script')[0];
                tmp.parentNode.insertBefore(s, tmp);
            }
            catch (err) {

            }


            if (isReadonly != "1") {

                Common.MaxLengthError();

                //处理下拉框级联等扩展信息
                AfterBindEn_DealMapExt(frmData);
            }

            //设置默认值
            for (var j = 0; j < frmData.Sys_MapAttr.length; j++) {

                var mapAttr = frmData.Sys_MapAttr[j];

                //添加 label
                //如果是整行的需要添加  style='clear:both'.
                var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);

                if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
                    $('#TB_' + mapAttr.KeyOfEn).val(defValue);
                }

                if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
                    // 判断下拉框是否有对应option, 若没有则追加
                    if ($("option[value='" + defValue + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                        var mainTable = frmData.MainTable[0];
                        var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                        $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + defValue + "'>" + selectText + "</option>");
                    }
                    //
                    $('#DDL_' + mapAttr.KeyOfEn).val(defValue);
                }

                if ($('#CB_' + mapAttr.KeyOfEn).length == 1) {
                    if (defValue == "1")
                        $('#CB_' + mapAttr.KeyOfEn).attr("checked", true);
                    else
                        $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
                }

                if (mapAttr.UIIsEnable == "0") {

                    $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
                    $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
                    $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
                }
            }

            ShowNoticeInfo();

            ShowTextBoxNoticeInfo();

            //            //初始化复选下拉框 
            //            var selectPicker = $('.selectpicker');
            //            $.each(selectPicker, function (i, selectObj) {
            //                var defVal = $(selectObj).attr('data-val');
            //                var defValArr = defVal.split(',');
            //                $(selectObj).selectpicker('val', defValArr);
            //            });

            //给富文本 创建编辑器
            var editor = document.activeEditor = UM.getEditor('editor', {
                'autoHeightEnabled': false,
                'fontsize': [10, 12, 14, 16, 18, 20, 24, 36]
            });
            if (document.BindEditorMapAttr) {
                editor.MaxLen = document.BindEditorMapAttr.MaxLen;
                editor.MinLen = document.BindEditorMapAttr.MinLen;
                editor.BindField = document.BindEditorMapAttr.KeyOfEn;
                editor.BindFieldName = document.BindEditorMapAttr.Name;
            }
            //调整样式,让必选的红色 * 随后垂直居中
            editor.$container.css({ "display": "inline-block", "margin-right": "10px", "vertical-align": "middle" });

            if (typeof setContentHeight == "function") {
                setContentHeight();
            }
        }
    })
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

//打开从表的从表
function DtlFoolFrm(dtl, refPK, refOID) {

    var url = 'DtlFoolFrm.htm?EnsDtl=' + dtl + '&RefPK=' + refPK + '&RefOID=' + refOID;
    alert('这里没有实现打开iurl ' + url);

    //引入了刘贤臣写的东西，一直缺少东西.可否改进一下，弄个稳定的？ @代国强.
    OpenEasyUiDialog(url, "eudlgframe", "编辑", 600, 450, "icon-edit", true, null, null, null, function () {
        //   window.location.href = window.location.href;
    });

    // window.open(url);
    //alert('打开从表卡片');
}

//保存
function Save() {

    //必填项和正则表达式检查
    var formCheckResult = true;
    if (!CheckBlanks()) {
        formCheckResult = false;
    }
    if (!CheckReg()) {
        formCheckResult = false;
    }
    if (!formCheckResult) {
        //alert("请检查表单必填项和正则表达式");
        return;
    }

    // setToobarDisiable();

    $.ajax({
        type: 'post',
        async: true,
        data: getFormData(true, true),
        url: Handler + "?DoType=FrmGener_Save&OID=" + pageData.OID,
        dataType: 'html',
        success: function (data) {

            if (data.indexOf('err@') == 0) {
                $('#Message').html(data.substring(4, data.length));
                $('.Message').show();
                return;
            }
            window.location.href = window.location.href;
            //alert(data);
        }
    });
}


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
    pageData.IsReadonly = GetQueryString("IsReadonly"); //如果是IsReadonly，就表示是查看页面，不是处理页面
    pageData.IsStartFlow = GetQueryString("IsStartFlow"); //是否是启动流程页面 即发起流程

    pageData.DoType1 = GetQueryString("DoType")//View
    pageData.FK_MapData = GetQueryString("FK_MapData")//View

    //$('#navIframe').attr('src', 'Admin/CCBPMDesigner/truck/centerTrakNav.html?FK_Flow=' + pageData.FK_Flow + "&FID=" + pageData.FID + "&WorkID=" + pageData.OID);
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
    var attachs = $('iframe[src*="AttachmentUpload.aspx"]');
    $.each(attachs, function (i, attach) {
        if (attach.src.indexOf('IsReadonly') == -1) {
            $(attach).attr('src', $(attach).attr('src') + "&IsReadonly=1");
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

//AtPara  @PopValSelectModel=0@PopValFormat=0@PopValWorkModel=0@PopValShowModel=0
function GepParaByName(name, atPara) {
    var params = atPara.split('@');
    var result = $.grep(params, function (value) {
        return value != '' && value.split('=').length == 2 && value.split('=')[0] == value;
    })
    return result;
}

//初始化下拉列表框的OPERATION
function InitDDLOperation(frmData, mapAttr, defVal) {
    var operations = '';
    //外键类型.
    if (mapAttr.LGType == 2) {

        var data = frmData[mapAttr.KeyOfEn];
        if (data == undefined)
            data = frmData[mapAttr.UIBindKey];

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


    //枚举类型.
    if (mapAttr.LGType == 1) {
        var enums = frmData.Sys_Enum;

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
            alert(e);
        }

        if (typeof fn == "function") {
            $.each(fn.call(), function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (typeof CommonHandler == "function") {
            CommonHandler.call("", mapAttr.UIBindKey, function (data) {
                GenerBindDDL("DDL_" + mapAttr.KeyOfEn, data, "No", "Name");
            })
            return "";
        }

        if (mapAttr.UIIsEnable == 0) {

            alert('不可编辑');
            operations = "<option  value='" + defVal + "'>" + defVal + "</option>";
            return operations;
        }

        if (flowData[mapAttr.KeyOfEn] != undefined) {
            $.each(flowData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (flowData[mapAttr.UIBindKey] != undefined) {

            $.each(flowData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }
        //   alert('没有获得约定的数据源.');
        alert('没有获得约定的数据源..' + mapAttr.KeyOfEn + " " + mapAttr.UIBindKey);
    }
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

    //通过URL参数传过来的参数 后台处理到MainTable 里面
    //for (var pageParam in pageParamObj) {
    //    if (pageParam == keyOfEn) {
    //        result = pageParamObj[pageParam];
    //        break;
    //    }
    //}

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }
    return result = unescape(result);
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

            var mystr = ctrlID.substring(4) + 'T=' + item;
            formArrResult.push(mystr);
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
                    case "CHECKBOX": //复选框
                        formArrResult.push(name + '=' + ($(disabledEle).is(':checked') ? 1 : 0));
                        break;
                    case "TEXT": //文本框
                        formArrResult.push(name + '=' + $(disabledEle).val());
                        break;
                    case "RADIO": //单选钮
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

            //对于复选下拉框获取值得方法  
            //                if ($('[data-id=' + name + ']').length > 0) { 
            //                    var val = $(disabledEle).val().join(','); 
            //                    formArrResult.push(name + '=' + val); 
            //                } else { 
            //                    formArrResult.push(name + '=' + $(disabledEle).children('option:checked').val()); 
            //                } 
            //                break; 
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
    return formdataResultStr;
}

//根据下拉框选定的值，弹出提示信息  绑定那个元素显示，哪个元素不显示  
function ShowNoticeInfo() {

    var rbs = frmData.Sys_FrmRB;
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

    var mapAttr = frmData.Sys_MapAttr;
    mapAttr = $.grep(mapAttr, function (attr) {
        var atParams = attr.AtPara;
        return atParams != undefined && AtParaToJson(atParams).Tip != undefined && AtParaToJson(atParams).Tip != '' && $('#TB_' + attr.KeyOfEn).length > 0 && $('#TB_' + attr.KeyOfEn).css('display') != 'none';
    })

    $.each(mapAttr, function (i, attr) {
        $('#TB_' + attr.KeyOfEn).bind('focus', function (obj) {

            var frmData = JSON.parse(jsonStr);
            var mapAttr = frmData.Sys_MapAttr;

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
function CheckBlanks() {
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
function CheckReg() {
    var CheckRegResult = true;
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
                    CheckRegResult = false;
                } else {
                    $(obj).removeClass('errorInput');
                }
            }
        }
    });
    return CheckRegResult;
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
if (plant == "JFlow")
    appPath = "./../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量


//处理URL，MainTable URL 参数 替换问题
function dealWithUrl(src) {
    var src = src.replace(new RegExp(/(：)/g), ':');

    //替换
    src = src.replace('@OID', pageData.OID);
    src = src.replace('@WorkID', pageData.OID);

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

function SaveDtlData() {
    if (IsChange == false)
        return;

    Save();
}

function Change(id) {
    IsChange = true;
    var tagElement = window.parent.document.getElementById("HL" + id);
    if (tagElement) {
        var tabText = tagElement.innerText;
        var lastChar = tabText.substring(tabText.length - 1, tabText.length);
        if (lastChar != "*") {
            tagElement.innerHTML = tagElement.innerText + '*';
        }
    }

    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitle();
        }
    }
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


function testExpression(exp) {
    if (exp == null || typeof exp == "undefined" || typeof exp != "string") {
        return false;
    }
    exp = exp.replace(/\s/g, "");
    if (exp == "" || exp.length == 0) {
        return false;
    }
    if (/[\+\-\*\/]{2,}/.test(exp)) {
        return false;
    }
    if (/\(\)/.test(exp)) {
        return false;
    }
    var stack = [];
    for (var i = 0; i < exp.length; i++) {
        var c = exp.charAt(i);
        if (c == "(") {
            stack.push("(");
        } else if (c == ")") {
            if (stack.length > 0) {
                stack.pop();
            } else {
                return false;
            }
        }
    }
    if (stack.length != 0) {
        return false;
    }
    if (/^[\+\-\*\/]|[\+\-\*\/]$/.test(exp)) {
        return false;
    }
    if (/\([\+\-\*\/]|[\+\-\*\/]\)/.test(exp)) {
        return false;
    }
    return true;
}

 