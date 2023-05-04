
//定义全局的变量
var pageData = {};//全局的参数变量
var frmData = {}; // 表单数据
var isReadonly = false;//表单方案是只读时的变化
webUser = typeof webUser == "undefined" || webUser == null ? new WebUser() : webUser;//用户信息

var UserICon = getConfigByKey("UserICon", '../../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀

var currentUrl = GetHrefUrl();
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
    var dbVer = GetQueryString("DBVer");
    dbVer = dbVer == null || dbVer == undefined || dbVer == "" ? null : dbVer;
    try {
        frmData = JSON.parse(data);

        //获取表单的历史数据
        if (dbVer != null) {
            handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
            handler.AddUrlData(urlParam);
            var data = handler.DoMethodReturnString("ChartFrm_GetBigTextByVer"); //执行保存方法.
            if (data.indexOf('err@') == 0) {
                alert(data);

            } else {
                if (data && data != "") {
                    var frmDB = JSON.parse(data);
                    //设置主表数据.
                    frmData.MainTable[0] = frmDB;
                }
            }
        }

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
    var frmNode = frmData["WF_FrmNode"]
    //只读时显示打印按钮
    if ((isReadonly == true || (frmNode && frmNode[0].FrmSln == 1)) && dbVer == null) {
        $("#PrintPDF").show();
        $("#PrintPDF").on("click", function () {
            PrintPDF();
        })
    }
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
        //$('#ContentDiv').width(w);
        $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");
    }

    // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = frmData.Sys_MapData[0].No;
    if (mapData.IsEnableJs == 1)
        loadScript("../../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());


    if ($("#WorkCheck").length == 1) {
        loadScript("../WorkOpt/WorkCheck.js", function () {
            NodeWorkCheck_Init();
        });
    }

    //loadScript("../../DataUser/JSLibData/" + enName + ".js?t=" + Math.random());

    //如果是富文本编辑器
    if ($(".rich").length > 0) {// && richTextType == "tinymce"
        var images_upload_url = "";
        var handlerUrl = "";
        if (plant == "CCFlow")
            handlerUrl = basePath + "/WF/Comm/Handler.ashx";
        else
            handlerUrl = basePath + "/WF/Comm/Sys/ProcessRequest.do";

        images_upload_url = handlerUrl + '?DoType=HttpHandler&DoMethod=RichUploadFile';
        images_upload_url += '&HttpHandlerName=BP.WF.HttpHandler.WF_Comm_Sys&FrmID=' + mapData.No + "&WorkID=" + pageData.WorkID;
        layui.extend({
            tinymce: '../Scripts/layui/ext/tinymce/tinymce'
        }).use('tinymce', function () {
            var tinymce = layui.tinymce;
            $(".rich").each(function (i, item) {
                var id = item.id;
                var h = item.style.height.replace("px", "");
                tinymce.render({
                    elem: "#" + id
                    , height: h
                    , images_upload_url: images_upload_url
                    , paste_data_images: true
                });
            })

        });

    }
    //if ($(".EditorClass").length > 0 && richTextType == "ueditor") {
    //    $('head').append('<link href="../Comm/umeditor1.2.3-utf8/themes/default/css/umeditor.css" type="text/css" rel="stylesheet">');
    //    Skip.addJs("../Comm/umeditor1.2.3-utf8/third-party/template.min.js?Version=" + Math.random());
    //    Skip.addJs("../Comm/umeditor1.2.3-utf8/umeditor.config.js?Version=" + Math.random());
    //    Skip.addJs("../Comm/umeditor1.2.3-utf8/umeditor.js?Version=" + Math.random());
    //    Skip.addJs("../Comm/umeditor1.2.3-utf8/lang/zh-cn/zh-cn.js?Version=" + Math.random());
    //    $.each($(".EditorClass"), function (i, EditorDiv) {
    //        var editorId = $(EditorDiv).attr("id");
    //        //给富文本 创建编辑器
    //        var editor = document.activeEditor = UM.getEditor(editorId, {
    //            'autoHeightEnabled': false, //是否自动长高
    //            'fontsize': [10, 12, 14, 16, 18, 20, 24, 36],
    //            'initialFrameWidth': '98%'
    //        });
    //        var mapAttr = $(EditorDiv).data();
    //        var height = mapAttr.UIHeight
    //        $("#Td_" + mapAttr.KeyOfEn).find('div[class = "edui-container"]').css("height", height);

    //        if (editor) {

    //            editor.MaxLen = mapAttr.MaxLen;
    //            editor.MinLen = mapAttr.MinLen;
    //            editor.BindField = mapAttr.KeyOfEn;
    //            editor.BindFieldName = mapAttr.Name;

    //            //调整样式,让必选的红色 * 随后垂直居中
    //            $(editor.container).css({ "display": "inline-block", "margin-right": "10px", "vertical-align": "middle" });
    //        }
    //    })
    //}
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
                trigger: 'click',
                ready: function (date) {
                    var now = new Date();
                    var mm = "";
                    if (now.getMinutes() < 10)
                        mm = "0" + now.getMinutes();
                    else
                        mm = now.getMinutes();

                    var ss = "";
                    if (now.getSeconds() < 10)
                        ss = "0" + now.getSeconds();
                    else
                        ss = now.getSeconds();

                    this.dateTime.hours = now.getHours();
                    this.dateTime.minutes = mm;
                    this.dateTime.seconds = ss;
                },
                change: function (value, date, endDate) {
                    $('.laydate-btns-confirm').click();
                },
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
                type: type,
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
function SaveIt(saveType) {
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
    if ($("#WorkCheck_Doc").length == 1 || $("#WorkCheck_Doc0").length == 1) {
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
        //debugger;
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
function Save(isSend) {
    if (isSend == null || isSend == undefined)
        isSend = 0;
    //保存从表数据
    var dtlCheck = true;
    $("[name=Dtl]").each(function (i, obj) {
        var contentWidow = obj.contentWindow;
        if (contentWidow != null && contentWidow.SaveAll != undefined && typeof (contentWidow.SaveAll) == "function") {
            var IsSaveTrue = contentWidow.SaveAll();
            if (IsSaveTrue == false && isSend==1)
                dtlCheck= false;
            if (isSend == 1) {
                //最小从表明细不为0
                if (contentWidow.CheckDtlNum != undefined && typeof (contentWidow.CheckDtlNum) == "function") {
                    if (contentWidow.CheckDtlNum() == false) {
                        layer.alert("[" + contentWidow.sys_MapDtl.Name + "]明细不能少于最小数量" + contentWidow.sys_MapDtl.NumOfDtl + "");
                        dtlCheck = false;
                    }
                }
            }
        } 
    });
    if (dtlCheck == false)
        return false;

    //审核组件
    if ($("#WorkCheck_Doc").length == 1 || $("textarea[id='WorkCheck_Doc0']").length == 1 ||$("#WorkCheck_Doc0").length == 1) {
        //保存审核信息
        SaveWorkCheck();
        if (isSend == 1 && isCanSend == false)
            return false;
    }

    //必填项和正则表达式检查
    var formCheckResult = true;
    if (checkBlanks() == false && isSend == false) {
        //layer.alert("检查必填项出现错误，边框变红颜色的是否填写完整？");
        return false;

    }
    if (CheckReg() == false && isSend == false) {
        layer.alert("保存错误:请检查字段边框变红颜色的是否填写完整？");
        return false;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("OID", pageData.OID);
    var params = getTreeFormData(true, true);
    handler.AddUrlData();
    handler.AddJson(params);
    var data = handler.DoMethodReturnString("FrmGener_Save");

    if (data.indexOf('err@') == 0) {
        layer.alert(data);
        return false;
    }
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
    if (formss == "")
        return {};

    var formArr = "\"" + formss.replace(/=/g, "\":\"");
    var stringObj = "{" + formArr.replace(/&/g, "\",\"") + "\"}";
    var formArrResult = JSON.parse(stringObj);

    var haseExistStr = ",";
    var mcheckboxs = "";
    //1.富文本编辑器
    if ($(".rich").length > 0 && richTextType == "tinymce") {
        $(".rich").each(function (i, item) {
            var edit = layui.tinymce.get('#' + item.id)
            var val = edit.getContent();
            formArrResult[item.id] = val;
            haseExistStr += item.id + ","
        })
    }
    for (var key in formArrResult) {
        if (key.indexOf('CB_') == 0) {
            //如果ID获取不到值，Name获取到值为复选框多选
            if ($('#' + key).length == 1) {
                if ($('#' + key + ':checked').length == 1) {
                    formArrResult[key] = 1;
                } else {
                    formArrResult[key] = 0;
                }
            } else {
                if (mcheckboxs.indexOf(key + ",") == -1) {
                    mcheckboxs += key + ",";
                    var str = "";
                    $("input[name='" + key + "']:checked").each(function (index, item) {
                        if ($("input[name='" + key + "']:checked").length - 1 == index) {
                            str += $(this).val();
                        } else {
                            str += $(this).val() + ",";
                        }
                    });
                    formArrResult[key] = str;
                }
            }
        }
        if (key.indexOf('DDL_') == 0) {
            var item = $("#" + key).children('option:checked').text();
            var mystrID = key.replace("DDL_", "TB_") + 'T';
            formArrResult[mystrID] = item;
            haseExistStr += mystrID + ",";
        }


    };


    //$.each(formArr, function (i, ele) {
    //    var ctrID = ele.split('=')[0];
    //    if (ctrID.indexOf('TB_') == 0) {
    //        if (haseExistStr.indexOf("," + ctrID + ",") == -1) {
    //            formArrResult.push(ele);
    //            haseExistStr += ctrID + ",";
    //        }


    //    }
    //});



    //获取表单中禁用的表单元素的值
    var disabledEles = $('#divCCForm :disabled');
    $.each(disabledEles, function (i, disabledEle) {

        var name = $(disabledEle).attr('id');

        switch (disabledEle.tagName.toUpperCase()) {

            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult[name] = encodeURIComponent(($(disabledEle).is(':checked') ? 1 : 0));
                        break;
                    case "TEXT": //文本框
                    case "HIDDEN":
                        if (haseExistStr.indexOf("," + name + ",") == -1) {
                            formArrResult[name] = encodeURIComponent($(disabledEle).val());
                            haseExistStr += name + ",";
                        }

                        break;
                    case "RADIO": //单选钮
                        name = $(disabledEle).attr('name');
                        if ($.inArray(eleResult, formArrResult) == -1) {
                            formArrResult[name] = $('[name="' + name + '"]:checked').val();
                        }
                        break;
                }
                break;
            //下拉框            
            case "SELECT":
                formArrResult[name]= encodeURIComponent($(disabledEle).children('option:checked').val());
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1) {
                    if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                        formArrResult[tbID] = $(disabledEle).children('option:checked').text();
                        haseExistStr += tbID + ",";
                    }
                }
                break;

            //文本区域                    
            case "TEXTAREA":
                formArrResult[name] = encodeURIComponent($(disabledEle).val());
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
            formArrResult[name] = data.id;
            formArrResult[name] = data.text;
        }
    });


    if (!isCotainTextArea) {
        formArrResult = $.grep(formArrResult, function (value) {
            return value.split('=').length == 2 ? value.split('=')[1].length <= 50 : true;
        });
    }


    return formArrResult;
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

function PrintPDF() {
    var element = $("#CCForm");
    var w = element.width();
    var h = element.height();
    var offsetTop = element.offset().top;
    var offsetLeft = element.offset().left;
    var canvas = document.createElement("canvas");
    var abs = 0;
    var win_i = $(window).width();
    var win_o = window.innerWidth;
    if (win_o > win_i) {
        abs = (win_o - win_i) / 2;
    }
    canvas.width = w * 2;
    canvas.height = h * 2;
    var context = canvas.getContext("2d");
    context.scale(2, 2);
    context.translate(-offsetLeft - abs, -offsetTop);
    var element = $("#CCForm")[0];
    html2canvas(element, { async: false, }).then(function (canvas) {
        var contentWidth = canvas.width;
        var contentHeight = canvas.height;
        var pageHeight = contentWidth / 592.28 * 841.89;
        var leftHeight = contentHeight;
        var position = 0;
        var imgWidth = 595.28;
        var imgHeight = 592.28 / contentWidth * contentHeight;
        console.log(canvas);
        var pageData = canvas.toDataURL('image/jpeg', 1.0);
        var pdf = new jsPDF('', 'pt', 'a4');
        if (leftHeight < pageHeight) {
            pdf.addImage(pageData, 'JPEG', 20, 0, imgWidth, imgHeight);
        } else {    // 分页
            while (leftHeight > 0) {
                pdf.addImage(pageData, 'JPEG', 0, position, imgWidth, imgHeight)
                leftHeight -= pageHeight;
                position -= 841.89;
                //避免添加空白页
                if (leftHeight > 0) {
                    pdf.addPage();
                }
            }
        }
        var Name = 'report_pdf_' + new Date().getTime();
        pdf.save(Name + '.pdf');
        //pdf.output('dataurlnewwindow', Name + ".pdf");
    });
}