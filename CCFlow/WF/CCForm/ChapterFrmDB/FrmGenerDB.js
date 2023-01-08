
//定义全局的变量
var pageData = {};//全局的参数变量
var frmData = {}; // 表单数据
var isReadonly = true;//表单方案是只读时的变化
webUser = typeof webUser == "undefined" || webUser == null ? new WebUser() : webUser;//用户信息
var UserICon = getConfigByKey("UserICon", '../../../DataUser/Siganture/'); //获取签名图片的地址
var UserIConExt = getConfigByKey("UserIConExt", '.jpg');  //签名图片的默认后缀
var currentUrl = GetHrefUrl();
var richTextType = getConfigByKey("RichTextType", 'tinymce');
var currentURL = GetHrefUrl();
//初始化函数
$(function () {
    UserICon = UserICon.replace("@basePath", basePath);
    //增加css样式
    $('head').append('<link href="../../../DataUser/Style/GloVarsCSS.css" rel="stylesheet" type="text/css" />');
    //初始化参数.
    initPageParam();
    //构造表单.
    GenerFrm(); //表单数据.
});


/**
 * 初始化获取网页数据
 */
function initPageParam() {
    pageData.FK_MapData = GetQueryString("FrmID");
    pageData.FrmID = pageData.FK_MapData;
    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    pageData.FK_Node = pageData.FK_Node == null || pageData.FK_Node == undefined ? 0 : pageData.FK_Node;
    pageData.FID = GetQueryString("FID") == null ? 0 : GetQueryString("FID");
    var oid = GetQueryString("OID");
    pageData.OID = oid;
    pageData.WorkID = oid;
    pageData.IsReadonly = 1;
    pageData.DBVer = GetQueryString("DBVer");
}

/**
 * 
 * 获取表单数据
 */
var frmData = null;
function GenerFrm() {
    var urlParam = currentUrl.substring(currentUrl.indexOf('?') + 1, currentUrl.length);
    urlParam = urlParam.replace('&DoType=', '&DoTypeDel=xx');
    frmData = window.parent.frmData;
    if (frmData != null) {
        var datas = frmData.MainData;
        $.each(datas, function (idx, item) {
            var data = item.Data;
            data = data.replace(/\\\\\\\\"/g, "'");
            if (item.Ver == pageData.DBVer)
                frmData.MainTable = JSON.parse(data);
           
            else
                frmData.CompareTable = JSON.parse(data);
           
                
        })
    }

    //处理附件的问题 
    if (frmData.Sys_FrmAttachment.length != 0) {

        Skip.addJs("../Ath.js");
        Skip.addJs("../JS/FileUpload/fileUpload.js");
        Skip.addJs("../../Scripts/jquery-form.js");
        Skip.addJs("../../../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='../JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");


    }

    //获得sys_mapdata.
    var mapData = frmData["Sys_MapData"][0];
    var frmNode = frmData["WF_FrmNode"]

    //根据表单类型不同生成表单.

    var isTest = GetQueryString("IsTest");
    var isFloolFrm = false;
    var isDevelopForm = false;
    if (isTest == "1") {

        var frmType = GetQueryString("FrmType");
        if (frmType == 'Develop') {
            $('head').append('<link href="../../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
            $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            Skip.addJs("../FrmDevelop2021.js?ver=1");
            isDevelopForm = true;
            GenerDevelopFrm(frmData, mapData.No); //开发者表单.

        }
        else {
            $('head').append('<link href="../../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
            Skip.addJs("../FrmFool.js?ver=" + Math.random());
            GenerFoolFrm(frmData);
            isFloolFrm = true;
        }

    } else {
        if (mapData.FrmType == 0 || mapData.FrmType == 10 || mapData.FrmType == 9) {
            $('head').append('<link href="../../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
            Skip.addJs("../FrmFool.js?ver=" + Math.random());
            isFloolFrm = true;
            GenerFoolFrm(frmData);
        }

        if (mapData.FrmType == 8) {
            $('head').append('<link href="../../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
            $('head').append('<link href="../../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            Skip.addJs("../FrmDevelop2021.js?ver=1");
            GenerDevelopFrm(frmData, mapData.No); //开发者表单.
            isDevelopForm = true;

        }

    }

    //表单名称.
    var w = mapData.FrmW;
    if (isFloolFrm == true) {
        //$('#ContentDiv').width(w);
        $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");
    }
    // 加载JS文件 改变JS文件的加载方式 解决JS在资源中不显示的问题.
    var enName = frmData.Sys_MapData[0].No;
    if (mapData.IsEnableJs == 1)
        loadScript("../../../DataUser/JSLibData/" + enName + "_Self.js?t=" + Math.random());
    layui.form.render();
    //3.装载表单数据与修改表单元素风格.
    LoadFrmDataAndChangeEleStyle(frmData);
    var verType = GetQueryString("VerType");
    if (verType == "main") {
        window.addEventListener("scroll", function () {
            var top = $(this).scrollTop();
            var iframe = $("#compareFrame", window.parent.document)[0];
            iframe.contentWindow.document.documentElement.scrollTop = top;
        }, true);
    }
    if (verType == "compare") {
        window.addEventListener("scroll", function () {
            var top = $(this).scrollTop();
            var iframe = $("#mainFrame", window.parent.document)[0];
            iframe.contentWindow.document.documentElement.scrollTop = top;
        }, true);
    }

}

function LoadFrmDataAndChangeEleStyle(frmData) {
    mapData = frmData.Sys_MapData[0];
    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    frmMapAttrs = mapAttrs;
    var checkData = null;
    $.each(mapAttrs, function (i, mapAttr) {
        if (mapAttr.UIContralType == 18)
            return true;
        if (mapAttr.UIVisible == 0 && $("#TB_" + mapAttr.KeyOfEn).length == 0) {
            return true;
        }
        var val = ConvertDefVal(frmData.MainTable, mapAttr.KeyOfEn);

        //设置ICON,如果有icon,并且是文本框类型.
        SetICONForCtrl(mapAttr);

        //下拉框赋值
        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if (val != "" && $("option[value='" + val + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.MainTable;
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                if (selectText == null || selectText == undefined || selectText == "")
                    selectText = mainTable[mapAttr.KeyOfEn + "T"];

                if (selectText != null && selectText != undefined && selectText != "")
                    $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + val + "'>" + selectText + "</option>");

            }
            if (val != "") {
                $('#DDL_' + mapAttr.KeyOfEn).val(val);
            }
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);


            return true;
        }
        //checkbox.
        if (mapAttr.UIContralType == 2) {
            if (val == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", "true");
            else
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
            return true;
        }

        //枚举
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            $("#RB_" + mapAttr.KeyOfEn + "_" + val).attr("checked", 'checked');
            $('input[name=RB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            return true;
        }

        //枚举复选框
        if (mapAttr.MyDataType == 1 && mapAttr.LGType == 1) {
            var checkBoxArray = val.split(",");
            for (var k = 0; k < checkBoxArray.length; k++) {
                $("input[name='CB_" + mapAttr.KeyOfEn + "']").each(function () {
                    if ($(this).val() == checkBoxArray[k]) {
                        $(this).attr("checked", true);
                    }
                });
            }
            $('input[name=CB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            return true;
        }
        $('#TB_' + mapAttr.KeyOfEn).val(val);
        var compareVal = ConvertDefVal(frmData.CompareTable, mapAttr.KeyOfEn);
        //文本框.
        if (mapAttr.UIContralType == 0) {
            if (mapAttr.TextModel == 3) {
                $("#TD_" + mapAttr.KeyOfEn +" div").html(val);
            }else
                $('#TB_' + mapAttr.KeyOfEn).val(val);
            if (mapAttr.Tip != "") {
                $('#TB_' + mapAttr.KeyOfEn).attr("placeholder", mapAttr.Tip);

            }
            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            if (val != compareVal)
                $('#TB_' + mapAttr.KeyOfEn).css("background-color", "yellow");
            return true;
        }





        if (mapAttr.UIContralType == 14) { //签批组件
            $("#TB_" + mapAttr.KeyOfEn).hide();
            if (GetHrefUrl().indexOf("AdminFrm.htm") != -1)
                return true;
            //获取审核组件信息
            var node = frmData.WF_Node == undefined ? null : frmData.WF_Node[0];
            if (node != null && (node.FWCVer == 0 || node.FWCVer == "" || node.FWCVer == undefined))
                FWCVer = 0;
            else
                FWCVer = 1;
            if (checkData == null && node != null) {
                Skip.addJs("../../WorkOpt/WorkCheck.js");
                isFistQuestWorkCheck = false;
                checkData = WorkCheck_Init(FWCVer);

            }
            if (checkData != null && checkData != undefined) {
                var checkField = ""
                if (frmData.WF_FrmNode != null && frmData.WF_FrmNode != undefined && frmData.WF_FrmNode[0].FK_Node != 0) {
                    checkField = frmData.WF_FrmNode[0].CheckField;
                } else {
                    checkField = checkData.WF_FrmWorkCheck[0].CheckField;
                }
                var height = $("#TB_" + mapAttr.KeyOfEn).css("height");
                var _Html = "<div style='min-height:" + height + ";'>" + GetWorkCheck_Node(checkData, mapAttr.KeyOfEn, checkField, FWCVer) + "</div>";
                $("#TB_" + mapAttr.KeyOfEn).after(_Html);
            }

            return true;
        }


    })
    layui.form.render();
    //联动其他控件
    $.each(mapAttrs, function (i, mapAttr) {
        if (mapAttr.LGType == 1 && (mapAttr.UIContralType == 1 || mapAttr.UIContralType == 3)
            || (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")
            || mapAttr.MyDataType == "4") {
            var selectVal = "";
            var model = "select;"
            if ($("#DDL_" + mapAttr.KeyOfEn).length = 1)
                selectVal = $("#DDL_" + mapAttr.KeyOfEn).val();
            else if ($("#CB_" + mapAttr.KeyOfEn).length = 1) {
                model = "checkbox"
                selectVal = $("#CB_" + mapAttr.KeyOfEn).val();
            }
            else {
                model = "radio";
                selectVal = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
            }

            var compareVal = ConvertDefVal(frmData.CompareTable, mapAttr.KeyOfEn);
            if (selectVal != compareVal)
                $("#DIV_" + mapAttr.KeyOfEn + " .layui-select-title input").css("background-color", "yellow");

            var AtPara = mapAttr.AtPara;
            var isEnableJS = AtPara == "" || AtPara == null || AtPara == undefined || AtPara.indexOf('@IsEnableJS=1') == -1 ? false : true;
            if (isEnableJS == true) {

                if ((selectVal != null && selectVal != undefined && selectVal != "")) {
                    if (model == "radio" && selectVal == -1) {
                    } else {
                        cleanAll(mapAttr.KeyOfEn, mapData.FrmType);
                        setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, selectVal, mapData.FrmType);
                    }

                }
            }
        }
    });
    //设置是否隐藏分组、获取字段分组所有的tr
    var trs = $("#CCForm .FoolFrmGroupBar");
    var isHidden = false;
    $.each(trs, function (i, obj) {
        //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
        var sibles = $(obj).nextAll();
        var isAllHidenField = false;
        for (var k = 0; k < sibles.length; k++) {
            var sible = $(sibles[k]);
            if (k == 0 && sible.hasClass("FoolFrmFieldRow") == true)
                break;
            if (k == 0 && sible.hasClass("layui-row") == true)
                break;

            if (k == 0 && sible.hasClass("FoolFrmFieldRow") == true) {
                isHidden = true;
                break;
            }
            if (sibles[k].type == "hidden" && sibles[k].localName == "input") {
                isAllHidenField = true;
                continue;
            }
            isAllHidenField = false;
        }
        if (isHidden == true || isAllHidenField == true)
            $(obj).hide();

    });

}

/**
 * 获取字段值
 * @param {any} mainData 表单属性数据
 * @param {any} keyOfEn 字段英文名
 */
function ConvertDefVal(mainData, keyOfEn) {
    var result = mainData[keyOfEn];
    if (result == undefined)
        return "";
    return result = unescape(result);
}

/**
 * 给字段增加图标
 * @param {any} mapAttr
 */
function SetICONForCtrl(mapAttr) {
    var icon = mapAttr.ICON;
    var id = "TDIV_" + mapAttr.KeyOfEn;
    if (mapAttr.ICON.length < 2 || mapAttr.UIContralType != 0) {
        $('#' + id).removeClass("ccbpm-input-group");
        return;
    }
    if (mapAttr.MyDataType >= 7)
        return;

    var ctrl = $('#' + id);
    if (ctrl.length <= 0)
        return;
    if (icon) {
        var htmlBefore = "";
        htmlBefore += ' <i class="' + icon + '"></i>';
        ctrl.prepend(htmlBefore);
    }
}

/**
 * 初始化获取下拉框字段的选项
 * @param {any} frmData
 * @param {any} mapAttr
 * @param {any} defVal
 */
function InitDDLOperation(frmData, mapAttr, defVal) {
    var operations = '';
    var data = frmData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = frmData[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = frmData.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });

            if (mapAttr.DefVal == -1)
                operations += "<option " + (mapAttr.DefVal == defVal ? " selected = 'selected' " : "") + " value='" + mapAttr.DefVal + "'>-无(不选择)-</option>";

            $.each(enums, function (i, obj) {
                operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
        }
        return operations;

    }
    //operations += "<option  value=''>请选择</option>";
    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });
    return operations;
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



