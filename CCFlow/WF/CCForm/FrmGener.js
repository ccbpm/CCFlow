﻿
//定义全局的变量
var pageData = {};//全局的参数变量
var frmData = {}; // 表单数据
var isReadonly = false;//表单方案是只读时的变化
webUser = typeof webUser == "undefined" || webUser == null ? new WebUser() : webUser;//用户信息

var UserICon = getConfigByKey("UserICon", '../../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀

var currentUrl = window.location.href;
var richTextType = getConfigByKey("RichTextType", 'tinymce');
//初始化函数
$(function () {
    UserICon = UserICon.replace("@basePath", basePath);
    //增加css样式
    $('head').append('<link href="../../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');

    //初始化参数.
    initPageParam();
    //构造表单.
    GenerFrm(); //表单数据.

    //时间轴的表单增加打印单据按钮
    var wfNode = frmData["WF_Node"];
    var fromWorkOpt = GetQueryString("FromWorkOpt");
    if (fromWorkOpt == 2 && wfNode[0].PrintDocEnable == 1) {
        var PrintDocHtml = "<input type=button name='PrintDoc' value='打印单据' enable=true onclick='printDoc()' />";
        $("#topToolBar").append(PrintDocHtml);
    }
});


/**
 * 初始化获取网页数据
 */
function initPageParam() {
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FK_Node = pageData.FK_Node == null || pageData.FK_Node == undefined ? 0 : pageData.FK_Node;
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    var oid = GetQueryString("WorkID");
    if (oid == null || oid == undefined)
        oid = GetQueryString("OID");
    oid = oid == null || oid == undefined ? 0 : oid;
    pageData.OID = oid;
    pageData.WorkID = oid;
    pageData.Paras = GetQueryString("Paras");
    pageData.IsReadonly = GetQueryString("IsReadonly");
    pageData.IsStartFlow = GetQueryString("IsStartFlow");
    pageData.FK_MapData = GetQueryString("FK_MapData");
    isReadonly = pageData.IsReadonly == null || pageData.IsReadonly == undefined || pageData.IsReadonly == "" || pageData.IsReadonly == "0" ? false : true;
}

/**
 * 
 * 获取表单数据
 */
var frmData = null;
function GenerFrm() {
    var urlParam = currentUrl.substring(currentUrl.indexOf('?') + 1, currentUrl.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');

    //隐藏保存按钮.
    if (currentUrl.indexOf('&IsReadonly=1') > 1 || currentUrl.indexOf('&IsEdit=0') > 1) {
        $("#Btn").hide();
    }
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData(urlParam);
    handler.AddJson(pageData);
    var data = handler.DoMethodReturnString("FrmGener_Init");

    if (data.indexOf('err@') == 0) {
        layer.alert('装载表单出错,请查看控制台console,或者反馈给管理员.<br/>' + data);
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

    //处理附件的问题 
    if (frmData.Sys_FrmAttachment.length != 0) {
        if (currentUrl.indexOf("/CCBill/") != -1) {
            Skip.addJs("../CCForm/Ath.js");
            Skip.addJs("../CCForm/JS/FileUpload/fileUpload.js");
            Skip.addJs("../Scripts/jquery-form.js");
            Skip.addJs("../../DataUser/OverrideFiles/Ath.js");
            $('head').append("<link href='../CCForm/JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
        } else {
            Skip.addJs("./Ath.js");
            Skip.addJs("./JS/FileUpload/fileUpload.js");
            Skip.addJs("../Scripts/jquery-form.js");
            Skip.addJs("../../DataUser/OverrideFiles/Ath.js");
            $('head').append("<link href='./JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
        }

    }



    //获取没有解析的外部数据源
    var uiBindKeys = frmData["UIBindKey"];
    if (uiBindKeys.length != 0) {
        //获取外部数据源 handler/JavaScript
        var operdata;
        for (var i = 0; i < uiBindKeys.length; i++) {
            var sfTable = new Entity("BP.Sys.SFTable", uiBindKeys[i].No);
            var srcType = sfTable.SrcType;
            if (srcType != null && srcType != "") {
                //Handler 获取外部数据源
                if (srcType == 5) {
                    var selectStatement = sfTable.SelectStatement;
                    if (plant == "CCFLow")
                        selectStatement = basePath + "/DataUser/SFTableHandler.ashx" + selectStatement;
                    else
                        selectStatement = basePath + "/DataUser/SFTableHandler" + selectStatement;
                    operdata = DBAccess.RunDBSrc(selectStatement, 1);
                }
                //JavaScript获取外部数据源
                if (srcType == 6) {
                    operdata = DBAccess.RunDBSrc(sfTable.FK_Val, 2);
                }
                frmData[uiBindKeys[i].No] = operdata;
            }
        }

    }

    //获得sys_mapdata.
    var mapData = frmData["Sys_MapData"][0];
    //根据表单类型不同生成表单.

    var isTest = GetQueryString("IsTest");
    var isFloolFrm = false;
    var isDevelopForm = false;
    if (isTest == "1") {

        var frmType = GetQueryString("FrmType");
        if (frmType == 'Develop') {
            $('head').append('<link href="../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
            $('head').append('<link href="../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            if (currentUrl.indexOf("/CCBill/") != -1)
                Skip.addJs("../CCForm/FrmDevelop2021.js?ver=1");
            else
                Skip.addJs("./FrmDevelop2021.js?ver=1");
            isDevelopForm = true;
            GenerDevelopFrm(frmData, mapData.No); //开发者表单.

        }
        else {
            $('head').append('<link href="../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
            if (currentUrl.indexOf("/CCBill/") != -1)
                Skip.addJs("../CCForm/FrmFool.js?ver=" + Math.random());
            else
                Skip.addJs("./FrmFool.js?ver=" + Math.random());
            GenerFoolFrm(frmData);
            isFloolFrm = true;
        }

    } else {
        if (mapData.FrmType == 0 || mapData.FrmType == 10 || mapData.FrmType == 9) {
            $('head').append('<link href="../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
            if (currentUrl.indexOf("/CCBill/") != -1)
                Skip.addJs("../CCForm/FrmFool.js?ver=" + Math.random());
            else
                Skip.addJs("./FrmFool.js?ver=" + Math.random());
            isFloolFrm = true;
            GenerFoolFrm(frmData);
        }

       // alert(mapData.FrmType);

        if (mapData.FrmType == 8) {
            $('head').append('<link href="../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
            $('head').append('<link href="../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            if (currentUrl.indexOf("/CCBill/") != -1)
                Skip.addJs("../CCForm/FrmDevelop2021.js?ver=1");
            else
                Skip.addJs("./FrmDevelop2021.js?ver=1");
            GenerDevelopFrm(frmData, mapData.No); //开发者表单.
            isDevelopForm = true;

        }

    }

    //表单名称.
    document.title = mapData.Name;
    var w = mapData.FrmW;
    if (isFloolFrm == true) {
        $('#ContentDiv').width(w);
        $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");
    }

    // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = frmData.Sys_MapData[0].No;
    if (mapData.IsEnableJs == 1)
        loadScript("../../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());
   
   
    //loadScript("../../DataUser/JSLibData/" + enName + ".js?t=" + Math.random());

    //如果是富文本编辑器
    if ($(".rich").length > 0 && richTextType == "tinymce") {
        var images_upload_url = "";
        var directory = "ND" + pageData.FK_Flow;
        var handlerUrl = "";
        if (plant == "CCFlow")
            handlerUrl = basePath + "/WF/Comm/Handler.ashx";
        else
            handlerUrl = basePath + "/WF/Comm/Sys/ProcessRequest.do";

        images_upload_url = handlerUrl + '?DoType=HttpHandler&DoMethod=RichUploadFile';
        images_upload_url += '&HttpHandlerName=BP.WF.HttpHandler.WF_Comm_Sys&Directory=' + directory;
        layui.extend({
            tinymce: '../Scripts/layui/ext/tinymce/tinymce'
        }).use('tinymce', function () {
            var tinymce = layui.tinymce;
            $(".rich").each(function (i, item) {
                var id = item.id;
                tinymce.render({
                    elem: "#" + id
                    , height: 200
                    , images_upload_url: images_upload_url
                });
            })

        });

    }
    if ($(".EditorClass").length > 0 && richTextType == "ueditor") {
        $('head').append('<link href="../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css" type="text/css" rel="stylesheet">');
        Skip.addJs("../Comm/umeditor1.2.3-utf8/third-party/template.min.js?Version=" + Math.random());
        Skip.addJs("../Comm/umeditor1.2.3-utf8/umeditor.config.js?Version=" + Math.random());
        Skip.addJs("../Comm/umeditor1.2.3-utf8/umeditor.js?Version=" + Math.random());
        Skip.addJs("../Comm/umeditor1.2.3-utf8/lang/zh-cn/zh-cn.js?Version=" + Math.random());
        $.each($(".EditorClass"), function (i, EditorDiv) {
            var editorId = $(EditorDiv).attr("id");
            //给富文本 创建编辑器
            var editor = document.activeEditor = UM.getEditor(editorId, {
                'autoHeightEnabled': false, //是否自动长高
                'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
                'initialFrameWidth': '98%'
            });
            var mapAttr = $(EditorDiv).data();
            var height = mapAttr.UIHeight
            $("#Td_" + mapAttr.KeyOfEn).find('div[class = "edui-container"]').css("height", height);

            if (editor) {

                editor.MaxLen = mapAttr.MaxLen;
                editor.MinLen = mapAttr.MinLen;
                editor.BindField = mapAttr.KeyOfEn;
                editor.BindFieldName = mapAttr.Name;

                //调整样式,让必选的红色 * 随后垂直居中
                $(editor.container).css({ "display": "inline-block", "margin-right": "10px", "vertical-align": "middle" });
            }
        })
    }
    //3.装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(frmData);


    layui.form.render();
    //4.解析表单的扩展功能
    AfterBindEn_DealMapExt(frmData);

    $.each($(".ccdate"), function (i, item) {
        var format = $(item).attr("data-info");
        var type = $(item).attr("data-type");
        if (format.indexOf("HH") != -1) {
            layui.laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                type: type,
                done: function (value, date, endDate) {
                    var data = $(this.elem).data();
                    $(this.elem).val(value);
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            });
        } else {
            layui.laydate.render({
                elem: '#' + item.id,
                format: $(item).attr("data-info"), //可任意组合
                done: function (value, date, endDate) {
                    var data = $(this.elem).data();
                    $(this.elem).val(value);
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            });
        }

    })



    //星级评分事件
    var scoreDiv = $(".score-star");
    if (isReadonly == false)
        $.each(scoreDiv, function (idex, item) {
            var divId = $(item).attr("id");
            var KeyOfEn = divId.substring(3);//获取字段值
            $("#Star_" + KeyOfEn + " img").click(function () {
                var index = $(this).index() + 1;
                $("#Star_" + KeyOfEn + " img:lt(" + index + ")").attr("src", "../Style/Img/star_2.png");
                $("#SP_" + KeyOfEn + " strong").html(index + "  分");
                $("#TB_" + KeyOfEn).val(index);//给评分的隐藏input赋值
                index = index - 1;
                $("#Star_" + KeyOfEn + " img:gt(" + index + ")").attr("src", "../Style/Img/star_1.png");
            });
        });

}

/**
 *保存表单数据 
 */
function Save(saveType) {

    //正在保存弹出层
    var index = layer.msg('正在保存，请稍后..', {
        icon: 16
        , shade: 0.01
    });

    //保存从表数据
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            contentWidow.SaveAll();
        }
    });
    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isCanSend == false && saveType != 0)
            return false;
    }

    if (checkBlanks() == false) {
        layer.alert("必填项不能为空");
        return false;
    }
    //保存前事件
    if (typeof beforeSave != 'undefined' && beforeSave() instanceof Function)
        if (beforeSave() == false) {
            layer.close(index);
            return false;
        }


    //监听提交
    layui.form.on('submit(Save)', function (data) {

        //保存信息
        var formData = getFormData(data.field);
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddUrlData();
        for (var key in formData) {
            handler.AddPara(key, encodeURIComponent(formData[key]));
        }
        var data = handler.DoMethodReturnString("FrmGener_Save");
        layer.close(index);
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
        }
        if (saveType == 0)
            layer.alert("数据保存成功");


    });

    //附件检查
    var msg = checkAths();
    if (msg != "") {
        alert(msg);
        return false;
    }
    return true;
}

//保存
function SaveTreeFrm(scope) {

    //保存从表数据
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            IsSaveTrue = contentWidow.SaveAll();

        }
    });

    //审核组件
    if ($("#WorkCheck_Doc").length == 1) {
        //保存审核信息
        SaveWorkCheck();
    }

    //必填项和正则表达式检查
    var formCheckResult = true;
    if (checkBlanks() == false) {
        //alert("检查必填项出现错误，边框变红颜色的是否填写完整？");
        //return false;

    }
    if (CheckReg() == false) {
        alert("发送错误:请检查字段边框变红颜色的是否填写完整？");
        return false;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("OID", pageData.OID);
    var params = getTreeFormData(true, true);
    $.each(params.split("&"), function (i, o) {
        var param = o.split("=");
        if (param.length == 2 && validate(param[1])) {
            handler.AddPara(param[0], decodeURIComponent(param[1], true));
        } else {
            handler.AddPara(param[0], "");
        }
    });
    handler.AddPara("FK_MapData", pageData.FK_MapData);
    var data = handler.DoMethodReturnString("FrmGener_Save");

    if (data.indexOf('err@') == 0) {
        $('#Message').html(data.substring(4, data.length));
        $('.Message').show();
        return false;
    }

    if (scope != "btnsave")
        window.location.href = window.location.href;
    return true;


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

//获取表单数据
function getTreeFormData(isCotainTextArea, isCotainUrlParam) {
    var formss = $('#divCCForm').serialize();
    var formArr = formss.split('&');
    var formArrResult = [];
    var haseExistStr = ",";
    var mcheckboxs = "";
    $.each(formArr, function (i, ele) {
        if (ele.split('=')[0].indexOf('CB_') == 0) {
            //如果ID获取不到值，Name获取到值为复选框多选
            var targetId = ele.split('=')[0];
            if ($('#' + targetId).length == 1) {
                if ($('#' + targetId + ':checked').length == 1) {
                    ele = targetId + '=1';
                } else {
                    ele = targetId + '=0';
                }
                formArrResult.push(ele);
            } else {
                if (mcheckboxs.indexOf(targetId + ",") == -1) {
                    mcheckboxs += targetId + ",";
                    var str = "";
                    $("input[name='" + targetId + "']:checked").each(function (index, item) {
                        if ($("input[name='" + targetId + "']:checked").length - 1 == index) {
                            str += $(this).val();
                        } else {
                            str += $(this).val() + ",";
                        }
                    });

                    formArrResult.push(targetId + '=' + str);
                }
            }
        }
        if (ele.split('=')[0].indexOf('DDL_') == 0) {

            var ctrlID = ele.split('=')[0];

            var item = $("#" + ctrlID).children('option:checked').text();

            var mystr = '';
            mystr = ctrlID.replace("DDL_", "TB_") + 'T=' + item;
            formArrResult.push(mystr);
            formArrResult.push(ele);
            haseExistStr += ctrlID.replace("DDL_", "TB_") + "T" + ",";
        }
        if (ele.split('=')[0].indexOf('RB_') == 0) {
            formArrResult.push(ele);
        }

    });


    $.each(formArr, function (i, ele) {
        var ctrID = ele.split('=')[0];
        if (ctrID.indexOf('TB_') == 0) {
            if (haseExistStr.indexOf("," + ctrID + ",") == -1) {
                formArrResult.push(ele);
                haseExistStr += ctrID + ",";
            }


        }
    });



    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {

        var name = $(disabledEle).attr('id');

        switch (disabledEle.tagName.toUpperCase()) {

            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult.push(name + '=' + encodeURIComponent(($(disabledEle).is(':checked') ? 1 : 0)));
                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        if (haseExistStr.indexOf("," + name + ",") == -1) {
                            formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).val()));
                            haseExistStr += name + ",";
                        }

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
                formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).children('option:checked').val()));
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1) {
                    if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                        formArrResult.push(tbID + '=' + $(disabledEle).children('option:checked').text());
                        haseExistStr += tbID + ",";
                    }
                }
                break;

            //文本区域                    
            case "TEXTAREA":
                formArrResult.push(name + '=' + encodeURIComponent($(disabledEle).val()));
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
    return formdataResultStr;
}
//打印单据
function printDoc() {
    WinOpen("../WorkOpt/PrintDoc.htm?FK_Node=" + pageData.FK_Node + "&FID=" + pageData.FID + "&WorkID=" + pageData.OID + "&FK_Flow=" + pageData.FK_Flow + "&s=" + Math.random() + "', 'dsdd'");
}


//设置不可以用.
function SetReadonly() {
    //设置保存按钮不可以用.
    $("#Btn_Save").attr("disabled", true);
    $('#CCForm').find('input,textarea,select').attr('disabled', false);
    $('#CCForm').find('input,textarea,select').attr('readonly', true);
    $('#CCForm').find('input,textarea,select').attr('disabled', true);
}



//从表在新建或者在打开行的时候，如果 EditModel 配置了使用卡片的模式显示一行数据的时候，就调用此方法. // IsSave 弹出页面关闭时是否要删除从表
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage, FK_MapData, FK_Node, FID, IsSave, H) {
    // model=1 自由表单, model=2傻瓜表单.
    var pathName = document.location.pathname;
    var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);
    if (projectName.startsWith("/WF")) {
        projectName = "";
    }
    if (H == undefined || H < 600)
        H = 600;
    if (H > 1000)
        H = 1000;

    var url = projectName + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal + "&FK_MapData=" + FK_MapData + "&FK_Node=" + FK_Node + "&FID=" + FID + "&IsSave=" + IsSave;
    if (typeof ((parent && parent.OpenBootStrapModal) || OpenBootStrapModal) === "function") {
        OpenBootStrapModal(url, "editSubGrid", '编辑', 1000, H, "icon-property", false, function () { }, null, function () {
            if (typeof InitPage === "function") {
                InitPage.call();
            } else {
                alert("请手动刷新表单");
            }
        }, "editSubGridDiv", null, false);
    } else {
        window.open(url);
    }
}












//设置表单元素不可用
function setFormEleDisabled() {
    //文本框等设置为不可用
    $('#divCCForm textarea').attr('disabled', 'disabled');
    $('#divCCForm select').attr('disabled', 'disabled');
    $('#divCCForm input[type!=button]').attr('disabled', 'disabled');
}








var appPath = "/";
if (plant == "JFlow")
    appPath = "./../../";
var DtlsCount = " + dtlsCount + "; //应该加载的明细表数量




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

function SaveDtlData(scope) {
    if (IsChange == false)
        return true;

    return Save(scope);
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