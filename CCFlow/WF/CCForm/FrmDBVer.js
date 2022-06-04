
var currentURL = GetHrefUrl();
var pageData = {};
var ver = null;
var vers = null;
var isReadonly = true;
var mapData = null;
var frmID = "";
$(function () {
    pageData.FK_Flow = GetQueryString("FlowNo");
    if (pageData.FK_Flow == null || pageData.FK_Flow == undefined || pageData.FK_Flow == "")
        pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.FK_Node = GetQueryString("NodeID");
    pageData.FrmID = GetQueryString("FrmID");
    pageData.trackID = GetQueryString("TrackID");
    frmID = pageData.FrmID;

    InitPage();

});
/**
 * 初始化表单数据
 */
function InitPage() {
    $(".formTree").hide();
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FrmDBVer_Init");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        return;
    }
    data = JSON.parse(data);

    //绑定多表单
    var frmNodes = data.WF_FrmNode;
    if (frmNodes != undefined) {
        $(".formTree").show();
        GenerBindDDLAppend("DDL_FormTree", frmNodes, "FK_Frm", "FK_FrmText", frmID);

        layui.form.on('select(FormTree)', function (element) {
            ChangeFrmID();
        });
    }

    mapData = data.Sys_MapData[0];
    frmID = mapData.No;

    //数据版本
    vers = data.Sys_FrmDBVer;
    debugger
    //绑定主版本
    GenerBindDDLAppend("DDL_MainVer", vers, "MyPK", "RDT");

    //绑定比对版本
    GenerBindDDL("DDL_CompareVer", vers, "MyPK", "RDT");

    if (vers.length != 0) {
        if (pageData.trackID != null && pageData.trackID != undefined && pageData.trackID != "") {
            $.each(vers, function (i, item) {
                if (trackID != null && item.TrackID == pageData.trackID) {
                    $("#DDL_MainVer").val(item.MyPK);
                    return false;
                }
            });
        } else {
            $("#DDL_MainVer").val(vers[vers.length - 1].MyPK);
        }
    }

    layui.form.render("select");

    if (vers.length == 1) {
        layer.alert("表单数据存只存在一个版本，数据不用比对");
    } else {
        layui.form.on('select(MainVer)', function (element) {
            DoCompare(vers, element.value, $("#DDL_CompareVer").val());
        });
        layui.form.on('select(CompareVer)', function (element) {
            DoCompare(vers, $("#DDL_MainVer").val(), element.value);

        });
    }


    //处理附件的问题 
    if (data.Sys_FrmAttachment.length != 0) {
        Skip.addJs("./Ath.js");
        Skip.addJs("./JS/FileUpload/fileUpload.js");
        Skip.addJs("../Scripts/jquery-form.js");
        Skip.addJs("../../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='./JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }

    //初始化显示表单
    if (mapData.FrmType == 0 || mapData.FrmType == 10 || mapData.FrmType == 9) {
        $('head').append('<link href="../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');
        Skip.addJs("./FrmFool.js?ver=" + Math.random());
        GenerFoolFrm(data);
    }

    if (mapData.FrmType == 8) {
        $('head').append('<link href="../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
        $('head').append('<link href="../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
        Skip.addJs("./FrmDevelop2021.js?ver=" + Math.random());
        GenerDevelopFrm(data, mapData.No); //开发者表单.
    }


    //设置默认值
    LoadData(frmData);

    //调整页面宽度
    var w = mapData.FrmW;
    if (mapData.FrmType == 8)
        w = w + 100;
    //傻瓜表单的名称居中的问题
    if ($(".form-unit-title img").length > 0) {
        var width = $(".form-unit-title img")[0].width;
        $(".form-unit-title center h4 b").css("margin-left", "-" + width + "px");
    }

    $('#ContentDiv').width(w);
    $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");

}

function ChangeFrmID() {
    frmID = $("#DDL_FormTree").val();

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("WorkID", pageData.WorkID);
    handler.AddPara("FrmID", frmID);
    handler.AddPara("FK_Node", pageData.FK_Node);
    var data = handler.DoMethodReturnString("FrmDBVer_ChangeFrm");
    if (data.indexOf("err@") != -1) {
        layer.alert(data);
        $("#CCForm").html("");
        return;
    }
    data = JSON.parse(data);
    //重新加载版本
    vers = data.Sys_FrmDBVer;

    $("#DDL_MainVer").empty();
    $("#DDL_CompareVer").empty();
    GenerBindDDLAppend("DDL_MainVer", vers, "MyPK", "RDT");
    GenerBindDDL("DDL_CompareVer", vers, "MyPK", "RDT");

    if (vers.length != 0)
        $("#DDL_MainVer").val(vers[vers.length - 1].MyPK);

    $("#DDL_CompareVer").val("");
    layui.form.render("select");

    mapData = data.Sys_MapData[0];
    //处理附件的问题 
    if (data.Sys_FrmAttachment.length != 0) {
        Skip.addJs("./Ath.js");
        Skip.addJs("./JS/FileUpload/fileUpload.js");
        Skip.addJs("../Scripts/jquery-form.js");
        Skip.addJs("../../DataUser/OverrideFiles/Ath.js");
        $('head').append("<link href='./JS/FileUpload/css/fileUpload.css' rel='stylesheet' type='text/css' />");
    }

    //初始化显示表单
    if (mapData.FrmType == 0 || mapData.FrmType == 10 || mapData.FrmType == 9) {
        $('head').append('<link href="../../DataUser/Style/FoolFrmStyle/Default.css" rel="stylesheet" type="text/css" />');

        Skip.addJs("./FrmFool.js?ver=" + Math.random());
        GenerFoolFrm(data);
    }

    if (mapData.FrmType == 8) {
        $('head').append('<link href="../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
        $('head').append('<link href="../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
        Skip.addJs("./FrmDevelop2021.js?ver=" + Math.random());
        GenerDevelopFrm(data, mapData.No); //开发者表单.
    }


    //设置默认值
    LoadData(frmData);

    var w = mapData.FrmW;
    if (mapData.FrmType == 8)
        w = w + 100;

    $('#ContentDiv').width(w);

    layui.form.render();
}
/**
 * 版本比对
 * @param {any} vers
 * @param {any} mainVer
 * @param {any} compareVer
 */
function DoCompare(vers, mainVer, compareVer) {
    if (mainVer == "" || compareVer == "")
        return;

    if (mainVer == compareVer) {
        layer.alert("主版本和比对版本选择一致，不需要比对");
        return;
    }
    if (mainVer != "" && compareVer != "") {
        //请求表单数据
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddUrlData();
        handler.AddPara("MainVer", mainVer);
        handler.AddPara("CompareVer", compareVer);
        handler.AddPara("FrmID", frmID);
        var data = handler.DoMethodReturnString("FrmDB_DoCompareVer");
        if (data.indexOf("err@") != -1) {
            layer.alert(data);
            //解析表单
            $("#CCForm").html("");
            return;
        }
        var frmData = JSON.parse(data);
        //解析表单
        $("#CCForm").html("");
        mapData = frmData.Sys_MapData[0];
        if (mapData.FrmType == 0 || mapData.FrmType == 10 || mapData.FrmType == 9) {
            Skip.addJs("./FrmFool.js?ver=" + Math.random());
            GenerFoolFrm(frmData, true);
        }

        if (mapData.FrmType == 8) {
            $('head').append('<link href="../../DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />');
            $('head').append('<link href="../../DataUser/Style/MyFlowGenerDevelop.css" rel="Stylesheet" />');
            //Skip.addJs("./FrmDevelop2021.js?ver=" + Math.random());
            Skip.addJs("./FrmDevelop2021.js?ver=" + Math.random());
            GenerDevelopFrm(frmData, mapData.No, true); //开发者表单.
        }

        //从表比对
        $.each(frmData.Sys_MapDtl, function (idx, mapDtl) {
            Ele_CompareDtl(mapDtl, mainVer, compareVer);
        })

        //设置默认值
        LoadData(frmData);
    }

    ver = $.grep(vers, function (en) {
        return en.MyPK == compareVer;
    })[0];

    layui.form.render();
    var w = mapData.FrmW;
    if (mapData.FrmType == 8)
        w = w + 100;
    //傻瓜表单的名称居中的问题
    if ($(".form-unit-title img").length > 0) {
        var width = $(".form-unit-title img")[0].width;
        $(".form-unit-title center h4 b").css("margin-left", "-" + width + "px");
    }

    $('#ContentDiv').width(w);
    $('#ContentDiv').css("margin-left", "auto").css("margin-right", "auto");

}

function OpenDialog(obj) {
    var val = $(obj).attr("data-info");
    layer.open({
        type: 1
        , offset: 'auto'
        , content: '<div style="padding: 20px 100px;">修订时间:' + ver.RDT + '<br/>修订人:' + ver.RecName + '<br/>变更值:' + val + '</div>'
        , btn: '关闭'
        , btnAlign: 'c'
        , shade: 0
        , yes: function () {
            layer.closeAll();
        }
    });
}
function Ele_CompareDtl(frmDtl, mainVer, compareVer) {
    if (frmDtl.ListShowModel == "2") {
        if (frmDtl.UrlDtl == null || frmDtl.UrlDtl == undefined || frmDtl.UrlDtl == "")
            $("#Dtl_" + frmDtl.No).append("从表" + frmDtl.Name + "没有设置URL,请在" + frmDtl.FK_MapData + "_Self.js中解析");
        src = basePath + "/" + frmDtl.UrlDtl;
        if (src.indexOf("?") == -1)
            src += "?1=1";
        src += "&EnsName=" + frmDtl.No + "&RefPKVal=" + pageData.WorkID + "&FK_MapData=" + frmDtl.FK_MapData + "&IsReadonly=1";
        var mainUrl = src + "&VerMyPK=" + mainVer;
        var compareUrl = src + "&VerMyPK=" + compareVer;
        _html = "<div>";
        _html += "<div style='float:left;width:50%'>";
        _html += "<h5>主版本数据</h5>";
        _html += "<iframe style='width:100%;height:100%' name='Dtl' ID='Frame_" + mainVer + "'    src='" + mainUrl + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";
        _html += "</div>";
        _html += "<div style='float:left;width:50%'>";
        _html += "<h5>比对版本数据</h5>";
        _html += "<iframe style='width:50%;height:100%' name='Dtl' ID='Frame_" + compareVer + "'    src='" + compareUrl + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";
        _html += "</div>";
        _html += "</div>";
        $("#Dtl_" + frmDtl.No).append(_html);
        return;
    }

    var url = "./DtlCompare.htm?FK_MapDtl=" + frmDtl.No + "&MainVer=" + mainVer + "&CompareVer=" + compareVer + "&t=" + Math.random();
    var _html = "<iframe style='width:100%;height:100%' name='Dtl' ID='Frame_" + frmDtl.No + "'    src='" + url + "' frameborder=0  leftMargin='0'  topMargin='0' scrolling=auto></iframe>";
    $("#Dtl_" + frmDtl.No).append(_html);
}


function LoadData(frmData) {

    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    var checkData = null;
    $.each(mapAttrs, function (i, mapAttr) {
        if (mapAttr.UIContralType == 18)
            return true;
        var val = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (mapAttr.UIVisible == 0 && $("#TB_" + mapAttr.KeyOfEn).length == 0) {
            $('#CCForm').append($("<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + val + "' />"));
            return true;
        }

        $('#TB_' + mapAttr.KeyOfEn).attr("name", "TB_" + mapAttr.KeyOfEn);
        $('#DDL_' + mapAttr.KeyOfEn).attr("name", "DDL_" + mapAttr.KeyOfEn);
        $('#CB_' + mapAttr.KeyOfEn).attr("name", "CB_" + mapAttr.KeyOfEn);

        //设置只读模式       
        $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
        $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
        $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
        $('input[name=CB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
        $('input[name=RB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
        if (mapAttr.MyDataType == "8")
            $('#TB_' + mapAttr.KeyOfEn).css("text-align", "");
        $('#TB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#CB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#RB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#DDL_' + mapAttr.KeyOfEn).removeClass("form-control");

        //富文本绑数据
        if ($("#editor_" + mapAttr.KeyOfEn).length > 0) {
            if (mapAttr.UIHeight < 180)
                mapAttr.UIHeight = 180;
            $("#editor_" + mapAttr.KeyOfEn).data(mapAttr);
        }
        if (mapAttr.DefValType == 0 && mapAttr.LGType != 1 && (val == "0" || val == "0.0000"))
            val = "";

        //设置ICON,如果有icon,并且是文本框类型.
        SetICONForCtrl(mapAttr);

        //下拉框赋值
        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if (val != "" && $("option[value='" + val + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.mainData[0];
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                if (selectText == null || selectText == undefined || selectText == "")
                    selectText = mainTable[mapAttr.KeyOfEn + "T"];

                if (selectText != null && selectText != undefined && selectText != "")
                    $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + val + "'>" + selectText + "</option>");
            }
            if (val != "") {
                $('#DDL_' + mapAttr.KeyOfEn).val(val);
            }

            return true;
        }

        $('#TB_' + mapAttr.KeyOfEn).val(val);

        //文本框.
        if (mapAttr.UIContralType == 0) {

            if (mapAttr.MyDataType == 8 && val != "") {
                //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
                var attrdefVal = mapAttr.DefVal;
                var bit;
                if (attrdefVal != null && attrdefVal !== "" && attrdefVal.indexOf(".") >= 0)
                    bit = attrdefVal.substring(attrdefVal.indexOf(".") + 1).length;
                else
                    bit = 2;
                if (bit == 2)
                    val = formatNumber(val, 2, ",");
            }

            $('#TB_' + mapAttr.KeyOfEn).val(val);
            return true;
        }

        //checkbox.
        if (mapAttr.UIContralType == 2) {
            if (val == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", "true");
            else
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
        }

        //枚举
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            $("#RB_" + mapAttr.KeyOfEn + "_" + val).attr("checked", 'checked');
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
            return true;
        }

    })



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
 * @param {any} frmData 表单属性数据
 * @param {any} defVal 字段默认值
 * @param {any} keyOfEn 字段英文名
 */
function ConvertDefVal(frmData, defVal, keyOfEn) {
    //防止传值为null的情况
    if (defVal == null) {
        defVal = "";
    }
    var style = "layui-input-fool";
    if (mapData.FrmType == 8)
        style = "layui-input-develop";
    var result = defVal;
    var val = "";
    var mainData = frmData.mainData;
    if (mainData != undefined)
        mainData = mainData[0];
    else
        mainData = {};
    var compareData = frmData.compareData;
    if (compareData != undefined)
        compareData = compareData[0];
    else
        compareData = null;
    //通过MAINTABLE返回的参数
    for (var ele in mainData) {
        if (keyOfEn == ele) {
            result = mainData[ele];
            if (compareData == null)
                break;
            val = compareData[ele];
            if (result != val) {
                var w = $("#TD_" + keyOfEn).width() - 10;

                if ($("#TB_" + keyOfEn).length != 0) {
                    if (mapData.FrmType == 8)
                        w = $("#TB_" + keyOfEn)[0].offsetWidth - 10;
                    $("#TB_" + keyOfEn).after("<div class='" + style + "'data-info='" + val + "' style='margin-left:" + w + "px' onclick='OpenDialog(this)'></div>");
                }

                if ($("#DDL_" + keyOfEn).length != 0) {
                    if (mapData.FrmType == 8)
                        w = $("#DDL_" + keyOfEn)[0].offsetWidth - 10;
                    $("#DDL_" + keyOfEn).after("<div class='" + style + "'data-info='" + val + "' style='margin-left:" + w + "px'onclick='OpenDialog(this)'></div>");

                }
                if ($("#CB_" + keyOfEn).length != 0) {
                    if (mapData.FrmType == 8)
                        w = $("#CB_" + keyOfEn)[0].offsetWidth - 10;
                    $("#CB_" + keyOfEn).after("<div class='" + style + "'data-info='" + val + "' style='margin-left:" + w + "px'onclick='OpenDialog(this)'></div>");

                }
                if ($("input[name=RB_" + keyOfEn + "]").length != 0) {
                    if ($("#TD_" + keyOfEn).length == 0)
                        w = $("#SR_" + keyOfEn).width() - 10;

                    $("#SR_" + keyOfEn).append("<div class='" + style + "'data-info='" + val + "' style='margin-left:" + w + "px'onclick='OpenDialog(this)'></div>");

                }
            }
            if (result == null) {
                result = "";
            }
            break;
        }
    }

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
