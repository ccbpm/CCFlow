/// <reference path="frmdevelop2021.js" />

var athDesc = {}
$(function () {
    Skip.addJs(basePath + "/WF/Scripts/xss.js");
    var theme = filterXSS(localStorage.getItem("themeColorInfo"));
    if (theme == null || theme == undefined || theme == "")
        return;
    var data = JSON.parse(theme);
    var styleScope = document.getElementById("theme-data");
    if (styleScope != null && data != null) {

        var html = "";
        //按钮
        html += "\n .layui-btn{\n background-color:" + data.selectedMenu + ";\n}";
        if (filterXSS(localStorage.getItem("themeColor")) == "rynn") {
            html += "\n button>img{\n filter: hue-rotate(90deg) brightness(220%);\n}\n .layui-btn-primary{\n color: white;\n}";
        } else {
            html += "\n .layui-btn-primary{\n background:0 0 \n}";
        }
        //分页信息
        html += "\n .layui-laypage .layui-laypage-curr .layui-laypage-em{\n background-color:" + data.selectedMenu + ";\n}\n .layui-laypage input:focus,.layui-laypage select:focus{\n border-color:" + data.selectedMenu + " !important\n}";
        //时间
        html += "\n .layui-laydate .layui-this{\n background-color:" + data.selectedMenu + " !important;\n}";
        //复选框
        html += "\n .layui-form-checked[lay-skin=primary] i{\n background-color:" + data.selectedMenu + " !important;\n} \n .layui-form-select dl dd.layui-this{\n background-color:" + data.selectedMenu + " !important;\n}";
        html += "\n .layui-form-onswitch{\n background-color:" + data.selectedMenu + " !important;\n border-color:" + data.selectedMenu + " !important;\n}";
        //多选
        html += "\n .layui-form-checked, .layui-form-checked:hover{\n border-color:" + data.selectedMenu + " !important;\n} \n  .layui-form-checked:hover i{\n color:" + data.selectedMenu + " !important;\n}";
        //单选
        html += "\n .layui-form-radio:hover *, .layui-form-radioed, .layui-form-radioed>i{\n color:" + data.selectedMenu + " !important;\n} \n .layui-form-select dl dd.layui-this{\n background-color:" + data.selectedMenu + " !important;\n}";
        styleScope.innerHTML = filterXSS(html);
    }

})

var frmMapAttrs;
var mapData = "";
function LoadFrmDataAndChangeEleStyle(frmData) {
    mapData = frmData.Sys_MapData[0];
    //加入隐藏控件.
    var mapAttrs = frmData.Sys_MapAttr;
    frmMapAttrs = mapAttrs;
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
                var mainTable = frmData.MainTable[0];
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
                val = formatNumber(val, bit, ",", 1);
            }
            $('#TB_' + mapAttr.KeyOfEn).val(val);

            if (mapAttr.Tip != "" && mapAttr.UIIsEnable == "1" && pageData.IsReadonly != "1") {
                $('#TB_' + mapAttr.KeyOfEn).attr("placeholder", mapAttr.Tip);
                $('#TB_' + mapAttr.KeyOfEn).on("focus", function () {
                    var elementId = this.id;
                    layer.tips("<span style='color:black;'>" + $(this).attr("placeholder") + "</span>", '#' + elementId, { tips: [3, "#fff"] });
                    $(".layui-layer-tips").css("box-shadow", "1px 1px 50px rgb(0 0 0 / 30%)");
                })

            }
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



        if (mapAttr.UIContralType == 14) { //签批组件
            $("#TB_" + mapAttr.KeyOfEn).hide();
            $("#WorkCheck").remove();
            $("#Group_FWC").hide();
            if (GetHrefUrl().indexOf("AdminFrm.htm") != -1)
                return true;
            //获取审核组件信息
            var node = frmData.WF_Node == undefined ? null : frmData.WF_Node[0];
            if (node != null && (node.FWCVer == 0 || node.FWCVer == "" || node.FWCVer == undefined))
                FWCVer = 0;
            else
                FWCVer = 1;
            if (checkData == null && node != null) {
                Skip.addJs(laybase + "WorkOpt/WorkCheck.js");
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

        if (mapAttr.UIContralType == 15) { //评论组件
            $("#TB_" + mapAttr.KeyOfEn).hide();

            $("#TB_" + mapAttr.KeyOfEn).after("<div id='FlowBBS'></div>");
            return true;
        }

        /*if (mapAttr.UIContralType == 110) { //公文正文组件
            if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly != "1") {
                var localHref = GetLocalWFPreHref();
                var url = localHref + "/WF/CCForm/Components/GovDocFile.htm?FrmID=" + frmData.Sys_MapData[0].No + "&OID=" + pageData.WorkID + "&FK_Flow=" + GetQueryString("FK_Flow");
                $("#TB_GovDocFile").attr("readonly", "readonly");
                $("#TB_GovDocFile").on("click", function () {
                    window.OpenBootStrapModal(url, "GovDocFileIFrame", "公文正文组件", 600, 200, "icon-edit", false);
                })
            }
            return true;
        }*/

        if (mapAttr.UIContralType == 17) { //发文字号
            if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly != "1") {
                var localHref = GetLocalWFPreHref();
                var url = localHref + "/WF/CCForm/Components/DocWord.htm?FrmID=" + frmData.Sys_MapData[0].No + "&OID=" + pageData.WorkID + "&FK_Flow=" + GetQueryString("FK_Flow");
                $("#TB_DocWord").attr("readonly", "readonly");
                $("#TB_DocWord").on("click", function () {
                    window.OpenBootStrapModal(url, "DocWordIFrame", "发文字号", 600, 200, "icon-edit", false);
                })
            }
            return true;
        }

        if (mapAttr.UIContralType == 170) { //收文字号
            if (mapAttr.UIIsEnable == 1 && pageData.IsReadonly != "1") {
                var localHref = GetLocalWFPreHref();
                var url = localHref + "/WF/CCForm/Components/DocWordReceive.htm?FrmID=" + frmData.Sys_MapData[0].No + "&OID=" + pageData.WorkID + "&FK_Flow=" + GetQueryString("FK_Flow");
                $("#TB_DocWordReceive").attr("readonly", "readonly");
                $("#TB_DocWordReceive").on("click", function () {
                    window.OpenBootStrapModal(url, "DocWordReceiveIFrame", "收文字号", 600, 200, "icon-edit", false);
                })
            }
            return true;
        }


    })


    //增加审核组件附件上传的功能
    if ($("#uploaddiv").length > 0) {
        var explorer = window.navigator.userAgent;
        if (((explorer.indexOf('MSIE') >= 0) && (explorer.indexOf('Opera') < 0) || (explorer.indexOf('Trident') >= 0)))
            AddUploadify("uploaddiv", $("#uploaddiv").attr("data-info"));
        else
            AddUploafFileHtm("uploaddiv", $("#uploaddiv").attr("data-info"));
    }


    //获取正则表达式
    var expressions = $.grep(frmData.Sys_MapExt, function (mapExt) {
        return mapExt.ExtType == "RegularExpression";
    })
    var verifys = {
        required: [/[\S]+/, "必填项不能为空"],
    };
    //设置只读，必填，正则表单式字段
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        //去掉左右空格.
        mapAttr.KeyOfEn = mapAttr.KeyOfEn.replace(/(^\s*)|(\s*$)/g, "");

        //设置文本框只读.
        if (mapAttr.UIVisible != 0 && (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0 || isReadonly == true)) {
            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('input[name=CB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            $('input[name=RB_' + mapAttr.KeyOfEn + ']').attr("disabled", "disabled");
            if (mapAttr.MyDataType == "8")
                $('#TB_' + mapAttr.KeyOfEn).css("text-align", "");
        }
        $('#TB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#CB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#RB_' + mapAttr.KeyOfEn).removeClass("form-control");
        $('#DDL_' + mapAttr.KeyOfEn).removeClass("form-control");
        var layVerify = "";
        var expression = $.grep(expressions, function (mapExt) {
            return mapExt.AttrOfOper == mapAttr.KeyOfEn && mapExt.FK_MapData == mapAttr.FK_MapData;
        });
        if (mapAttr.UIIsInput == 1 && mapAttr.UIIsEnable == 1) {
            if ($("#TB_" + mapAttr.KeyOfEn).hasClass("rich"))
                layVerify = "";
            else
                layVerify = "required";

        }


        if (expression.length != 0) {
            layVerify = layVerify == "" ? mapAttr.KeyOfEn : layVerify + "|" + mapAttr.KeyOfEn;
            if (expression[0].Doc != null && expression[0].Doc != "") {
                var doc = expression[0].Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
                verifys[mapAttr.KeyOfEn] = [];
                verifys[mapAttr.KeyOfEn].push(cceval(doc));
                verifys[mapAttr.KeyOfEn].push(expression[0].Tag1);
                $('#TB_' + mapAttr.KeyOfEn).addClass("CheckRegInput");
            }


        }

        if (layVerify != "") {
            $('#TB_' + mapAttr.KeyOfEn).attr("lay-verify", layVerify);
            $('#CB_' + mapAttr.KeyOfEn).attr("lay-verify", layVerify);
            $('#RB_' + mapAttr.KeyOfEn).attr("lay-verify", layVerify);
            $('#DDL_' + mapAttr.KeyOfEn).attr("lay-verify", layVerify);
        }
    }

    layui.form.verify(verifys);

    //是否存在图标附件
    if ($("#editimg").length > 0) {
        layui.config({
            base: laybase + 'Scripts/layui/ext/'
        }).use(['form', 'croppers'], function () {
            var croppers = layui.croppers
                , layer = layui.layer;

            //创建一个头像上传组件
            croppers.render({
                elem: '#editimg'
                , saveW: 150     //保存宽度
                , saveH: 150   //保存高度
                , mark: 1 / 1    //选取比例
                , area: '900px'  //弹窗宽度
                , url: $("#editimg").data("info") //图片上传接口返回和（layui 的upload 模块）返回的JOSN一样
                , done: function (data) { //上传完毕回调
                    if (data.SourceImage != undefined) {
                        $('#Img' + $("#editimg").data("ref")).attr('src', basePath + "/" + data.SourceImage + "?M=" + Math.random());
                    } else {
                        return layer.msg('上传失败');
                    }

                }
            });

        });

    }

    //设置是否隐藏分组、获取字段分组所有的tr
    var trs = $("#CCForm .FoolFrmGroupBar");
    var isHidden = false;
    $.each(trs, function (i, obj) {
        //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
        var sibles = $(obj).nextAll();
        var isAllHidenField = false;
        if (sibles.length == 0) {
            $(obj).hide();
            return true;
        }
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

var currentURL = GetHrefUrl();
var laybase = "./";
if (currentURL.indexOf("CCForm") != -1 || currentURL.indexOf("CCBill") != -1)
    laybase = "../";
if (currentURL.indexOf("AdminFrm.htm") != -1 || currentURL.indexOf("/Admin/") != -1 || currentURL.indexOf("Comm/RefFunc") != -1)
    laybase = "../../";

//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
function AfterBindEn_DealMapExt(frmData) {
    var mapData = frmData.Sys_MapData[0];
    var frmType = mapData.FrmType;

    //字段扩展属性集合
    var mapExts = frmData.Sys_MapExt;
    //根据字段的主键分组
    mapExts = GetMapExtsGroup(mapExts);

    var mapAttrs = frmData.Sys_MapAttr;
    var isFirstTBFull = true;

    var baseUrl = "./CCForm/";
    if (GetHrefUrl().indexOf("CCForm") != -1)
        baseUrl = "./";
    if (GetHrefUrl().indexOf("CCBill") != -1)
        baseUrl = "../CCForm/";
    if (GetHrefUrl().indexOf("AdminFrm.htm") != -1 || GetHrefUrl().indexOf("RefFunc") != -1)
        baseUrl = "../../CCForm/";


    var isHaveLoadMapExt = false;
    var isHaveEnableJs = false;
    $.each(mapAttrs, function (idx, mapAttr) {
        //字段不可见
        if (mapAttr.UIVisible == 0)
            return true;

        //证件类扩展
        if (mapAttr.UIContralType == 13)
            return true;

        if (isHaveLoadMapExt == false) {
            Skip.addJs(baseUrl + "MapExt2021.js");
            isHaveLoadMapExt = true;
        }
        var mypk = mapAttr.MyPK;
        if (mypk == null || mypk == undefined || mypk == "")
            mapAttr.MyPK = mapAttr.FK_MapData + "_" + mapAttr.KeyOfEn;
        //如果是枚举、下拉框、复选框判断是否有选项联动其他控件
        if (mapAttr.LGType == 1 && (mapAttr.UIContralType == 1 || mapAttr.UIContralType == 3)
            || (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")
            || mapAttr.MyDataType == "4") {

            var AtPara = mapAttr.AtPara;
            var isEnableJS = AtPara == "" || AtPara == null || AtPara == undefined || AtPara.indexOf('@IsEnableJS=1') == -1 ? false : true;

            //不存在联动其他控件且不存在其他扩展属性

            if (isEnableJS == false && (mapExts[mapAttr.MyPK] == undefined || mapExts[mapAttr.MyPK].length == 0))
                return true;
            var model = mapAttr.LGType == 1 && (mapAttr.UIContralType == 2 || mapAttr.UIContralType == 3) ? "radio" : "select";

            model = mapAttr.MyDataType == "4" ? "checkbox" : model;

            var selectVal = "";
            if (model == "radio") {
                selectVal = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val()
            }
            if (model == "select") {
                selectVal = $("#DDL_" + mapAttr.KeyOfEn).val()
            }

            if (model == "checkbox") {
                selectVal = $("#CB_" + mapAttr.KeyOfEn).val()
            }

            if (isEnableJS == true && isHaveEnableJs == false) {
                Skip.addJs(baseUrl + "OptionLinkOthers2021.js");
                isHaveEnableJs = true;
            }


            SetRadioSelectMapExt(mapExts[mapAttr.MyPK], mapAttr, selectVal, isEnableJS, model, frmType, false);

            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                return;

            if (model == "radio") {
                layui.form.on('radio(' + mapAttr.KeyOfEn + ')', function (element) {
                    var data = $(this).data();
                    SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value, data.isEnableJS, "radio", frmType, true, element);
                });
            } else if (model == "select") {
                layui.form.on('select(' + mapAttr.KeyOfEn + ')', function (element) {
                    SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value, data.isEnableJS, "select", frmType, true, element);
                });
            } else if (model == "checkbox") {
                var obj = $("#CB_" + mapAttr.KeyOfEn);
                var sky = obj.attr("lay-skin");
                sky = sky == null || sky == undefined ? "" : sky;
                if (sky == "switch")
                    layui.form.on('switch(' + mapAttr.KeyOfEn + ')', function (element) {
                        if (element.elem.checked == true)
                            SetRadioSelectMapExt(data.mapExts, data.mapAttr, "1", data.isEnableJS, "select", frmType, true, element);
                        else
                            SetRadioSelectMapExt(data.mapExts, data.mapAttr, "0", data.isEnableJS, "select", frmType, true, element);
                    });
                else
                    layui.form.on('checkbox(' + mapAttr.KeyOfEn + ')', function (element) {
                        SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value, data.isEnableJS, "select", frmType, true, element);
                    });

            }
            var data = {
                mapAttr: mapAttr,
                mapExts: mapExts[mapAttr.MyPK],
                isEnableJS: isEnableJS
            };

            $("input[name=RB_" + mapAttr.KeyOfEn + "]").data(data);
            $("#CB_" + mapAttr.KeyOfEn).data(data);
            $("#DDL_" + mapAttr.KeyOfEn).data(data);
            return true;
        }

        //如果是整数，浮点型，金额类型的扩展属性
        if (mapAttr.MyDataType == 2 || mapAttr.MyDataType == 3 || mapAttr.MyDataType == 5 || mapAttr.MyDataType == 8) {
            if (isReadonly == true)
                return true;
            //数值字段绑定的chang事件
            var isEnableNumEnterLimit = GetPara(mapAttr.AtPara, "IsEnableNumEnterLimit");
            isEnableNumEnterLimit = isEnableNumEnterLimit == null || isEnableNumEnterLimit == "" || isEnableNumEnterLimit == undefined || isEnableNumEnterLimit == "0" ? false : true;
            var min = 0, max = 0;
            if (isEnableNumEnterLimit == true) {
                min = GetPara(mapAttr.AtPara, "MinNum");
                if (min == undefined || min == "")
                    min = 0;
                max = GetPara(mapAttr.AtPara, "MaxNum");
                if (max == undefined || max == "")
                    max = 100000;
                $("#TB_" + mapAttr.KeyOfEn).attr("data-min", min);
                $("#TB_" + mapAttr.KeyOfEn).attr("data-max", max);
            }
            //没有扩展属性
            if (mapExts[mapAttr.MyPK] == undefined || mapExts[mapAttr.MyPK].length == 0) {
                $('#TB_' + mapAttr.KeyOfEn).bind("change", function () {
                    var expVal = $(this).val();//获取要转换的值
                    var min = $(this).attr("data-min");
                    var max = $(this).attr("data-max");
                    if (min && parseInt(expVal) < parseInt(min)) {
                        layer.alert("值不能小于设置的最小值:" + min);
                        expVal = min;
                        $(this).val(expVal);
                    }
                    if (max && parseInt(expVal) > parseInt(max)) {
                        layer.alert("值不能大于设置的最大值:" + max);
                        expVal = max;
                        $(this).val(expVal);
                    }
                });
                return true;
            }
            SetNumberMapExt(mapExts[mapAttr.MyPK], mapAttr);
            return true;
        }

        //没有扩展属性
        if (mapExts[mapAttr.MyPK] == undefined || mapExts[mapAttr.MyPK].length == 0)
            return true;

        //如果是日期型或者时间型
        if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                return true;
            SetDateExt(mapExts[mapAttr.MyPK], mapAttr)
            return true;
        }

        //文本字段扩展属性
        $.each(mapExts[mapAttr.MyPK], function (k, mapExt1) {
            var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
            mapExt.MyPK = mapExt1.MyPK;
            //处理Pop弹出框的问题
            var PopModel = GetPara(mapAttr.AtPara, "PopModel");

            if (PopModel != undefined && PopModel != "" && mapExt.ExtType.indexOf("Pop") != -1) {
                if (mapExt.ExtType != PopModel)
                    return true;
                if (mapAttr.UIIsEnable == 0 || isReadonly == true || $("#TB_" + mapAttr.KeyOfEn).length == 0)
                    return true;
                PopMapExt(PopModel, mapAttr, mapExt, frmData, baseUrl, mapExts);
                return true;
            }
            //处理文本自动填充
            var TBModel = GetPara(mapAttr.AtPara, "TBFullCtrl");
            if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
                if (mapAttr.UIIsEnable == 0 || isReadonly == true || $("#TB_" + mapExt.AttrOfOper).length == 0)
                    return true;
                if (TBModel == "Simple") {
                    if (isFirstTBFull == true) {
                        layui.config({
                            base: laybase + 'Scripts/layui/ext/'
                        });
                        isFirstTBFull = false;
                    }
                    //判断时简洁模式还是表格模式
                    layui.use('autocomplete', function () {
                        var autocomplete = layui.autocomplete;
                        autocomplete.render({
                            elem: "#TB_" + mapAttr.KeyOfEn,
                            url: mapExt.MyPK,
                            response: { code: 'code', data: 'data' },
                            template_val: '{{d.No}}',
                            template_txt: '{{d.Name}} <span class=\'layui-badge layui-bg-gray\'>{{d.No}}</span>',
                            onselect: function (data) {
                                FullIt(data.No, this.url, this.elem[0].id);
                            }
                        })
                    });
                    return true;
                }
                if (TBModel == "Table") {
                    var obj = $("#TB_" + mapAttr.KeyOfEn);
                    obj.attr("onkeyup", "showDataGrid(\'TB_" + mapAttr.KeyOfEn + "\',this.value, \'" + mapExt.MyPK + "\');");
                    //showDataGrid("TB_" + mapAttr.KeyOfEn, $("#TB_" + mapAttr.KeyOfEn).val(), mapExt);
                }
            }

            //alert(mapExt.Doc);

            switch (mapExt.ExtType) {
                case "MultipleChoiceSmall"://小范围多选
                case "SingleChoiceSmall"://小范围单选
                    if (mapExt.DoWay == 0)//不设置
                        break;
                    var tag = mapExt.Tag;
                    if (tag == null || tag == undefined || tag == "")
                        tag = "0";
                    if (tag == "0" && (mapAttr.UIIsEnable == 0 || isReadonly == true)) {
                        //只显示
                        $("#TB_" + mapAttr.KeyOfEn).hide();
                        var val = frmData.MainTable[0][mapAttr.KeyOfEn + "T"];
                        $("#TB_" + mapAttr.KeyOfEn).after("<div style='border:1px solid #eee;line-height:36px;width:100%;height:36px'>" + val + "</div>");
                        break;
                    }

                    var data = GetDataTableOfTBChoice(mapExt, frmData, $("#TB_" + mapAttr.KeyOfEn).val());
                    data = data == null ? [] : data;
                    $("#TB_" + mapAttr.KeyOfEn).hide();

                    //下拉框
                    if (tag == "0") {
                        $("#TB_" + mapAttr.KeyOfEn).after("<div id='mapExt_" + mapAttr.KeyOfEn + "'style='width:99%'></div>")
                        layui.use('xmSelect', function () {
                            var xmSelect = layui.xmSelect;
                            xmSelect.render({
                                el: "#mapExt_" + mapAttr.KeyOfEn,
                                paging: data.length > 15 ? true : false,
                                data: data,
                                autoRow: true,
                                radio: mapExt.ExtType == "MultipleChoiceSmall" ? false : true,
                                clickClose: mapExt.ExtType == "MultipleChoiceSmall" ? false : true,
                                on: function (data) {
                                    var arr = data.arr;
                                    var vals = [];
                                    var valTexts = [];
                                    $.each(arr, function (i, obj) {
                                        vals[i] = obj.value;
                                        valTexts[i] = obj.name;
                                    })
                                    var elID = data.el.replace("mapExt", "TB");
                                    $(elID).val(vals.join(","));
                                    $(elID + "T").val(valTexts.join(","));
                                }
                            })
                        });
                        break;
                    }
                    var val = $("#TB_" + mapAttr.KeyOfEn).val();
                    if (data == null || data == undefined || data == "") {
                        $("#TB_" + mapAttr.KeyOfEn).after("<span>数据获取有误,没有选项</span>");
                        break;
                    }
                    var disabled = "";
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                        disabled = "disabled='disabled'";
                    var tbID = "TB_" + mapAttr.KeyOfEn;
                    var name = "CB_" + mapAttr.KeyOfEn + "_" + mapAttr.KeyOfEn;
                    var selectType = mapExt.ExtType == "MultipleChoiceSmall" ? 1 : 0;
                    var _html = ' <div class="layui-input-block">';
                    $.each(data, function (i, item) {
                        var id = "CB_" + mapAttr.KeyOfEn + "_" + item.value;
                        var checked = "";
                        if (selectType == 0 && val == item.value)
                            checked = " checked='checked'";
                        if (selectType == 1 && val.indexOf(item.value + ",") != -1)
                            checked = " checked='checked'";

                        _html += "<input " + disabled + checked + " type='checkbox' id='" + id + "'name='" + name + "' data-info='" + mapAttr.KeyOfEn + "' value='" + item.value + "' lay-skin='primary' title='" + item.name + "'lay-filter='" + name + "'  />";
                        if (selectType == 0)
                            _html += "<input " + disabled + checked + " type='radio' id='" + id + "' name='" + name + "' data-info='" + mapAttr.KeyOfEn + "' value='" + item.value + "' title='" + item.name + "' lay-filter='" + name + "'  />";

                        if (tag == "2")
                            _html += "</br>";
                    })
                    _html += "</div>";
                    $("#TB_" + mapAttr.KeyOfEn).after(_html);

                    if (selectType == 1)
                        layui.form.on('checkbox(' + name + ')', function (element) {
                            changeValue(element.value, $(this).data().info, 1);
                        });
                    if (selectType == 0)
                        layui.form.on('radio(' + name + ')', function (element) {
                            var data = $(this).data();
                            changeValue(element.value, $(this).data().info, 0);
                        });
                    layui.form.render();
                    break;

                case "MultipleChoiceSearch"://搜索多选
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                        break;

                    var isLoad = true;
                    $("#TB_" + mapAttr.KeyOfEn).hide();
                    $("#TB_" + mapAttr.KeyOfEn).after("<div id='mapExt_" + mapAttr.KeyOfEn + "' style='width:99%'></div>")
                    //单选还是多选
                    var selectType = mapExt.GetPara("SelectType");
                    selectType = selectType == null || selectType == undefined || selectType == "" ? 1 : selectType;
                    layui.use('xmSelect', function () {
                        var xmSelect = layui.xmSelect;
                        xmSelect.render({
                            el: "#mapExt_" + mapAttr.KeyOfEn,
                            autoRow: true,
                            prop: {
                                name: 'Name',
                                value: 'No',
                            },
                            radio: selectType == 1 ? false : true,
                            clickClose: selectType == 1 ? false : true,
                            toolbar: { show: selectType == 1 ? true : false },
                            filterable: true,
                            remoteSearch: true,
                            mapExt: mapExt.MyPK,
                            on: function (data) {
                                if (isLoad == true) {
                                    isLoad = false;
                                    return;
                                }
                                var arr = data.arr;
                                var vals = [];
                                var valTexts = [];
                                $.each(arr, function (i, obj) {
                                    vals[i] = obj.No;
                                    valTexts[i] = obj.Name;
                                })
                                var elID = data.el.replace("mapExt", "TB");
                                $(elID).val(valTexts.join(","));
                                SaveFrmEleDBs(arr, elID.replace("#TB_", ""), mapExt);
                            },
                            remoteMethod: function (val, cb, show) {
                                //这里如果val为空, 则不触发搜索
                                /*if (!val) {
                                    return cb([]);
                                }*/
                                var mapExt = new Entity("BP.Sys.MapExt", this.mapExt);
                                //选中的值
                                var selects = new Entities("BP.Sys.FrmEleDBs");
                                selects.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", pageData.WorkID);
                                var dt = GetDataTableByDB(mapExt.Doc, mapExt.DBType, mapExt.FK_DBSrc, val,mapExt,"Doc");
                                var data = [];
                                dt.forEach(function (item) {
                                    data.push({
                                        No: item.No,
                                        Name: item.Name,
                                        selected: IsHaveSelect(item.No, selects)
                                    })
                                })
                                cb(data);
                            },

                        })
                    })
                    break;
                case "FastInput": //是否启用快速录入
                    if (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0 || GetQueryString("IsReadonly") == "1")
                        break;
                    var tbFastInput = $("#TB_" + mapExt.AttrOfOper);

                    //获取大文本的长度
                    var left = 0;
                    if (tbFastInput.parent().length != 0)
                        left = tbFastInput.parent().css('left') == "auto" ? 0 : parseInt(tbFastInput.parent().css('left').replace("px", ""));
                    var width = tbFastInput.width() + left;
                    width = tbFastInput.parent().css('left') == "auto" ? width - 180 : width - 70;

                    var content = $("<span style='margin-left:" + width + "px;top: -15px;position: relative;' id='span_" + mapExt.AttrOfOper + "'></span><br/>");
                    content.append("<a href='javascript:void(0)' onclick='TBHelp(\"TB_" + mapExt.AttrOfOper + "\",\"" + mapExt.MyPK + "\")'>常用词汇</a> <a href='javascript:void(0)' onclick='clearContent(\"TB_" + mapExt.AttrOfOper + "\")'>清空<a>");
                    tbFastInput.after(content);
                    tbFastInput.on("mouseover", function () {
                        $("#" + this.id.replace("TB_", "span_")).show();

                    });
                    tbFastInput.on("blur", function () {
                        $("#" + this.id.replace("TB_", "span_")).hide();

                    });
                    $("#span_" + mapExt.AttrOfOper).hide();
                    break;
                case "MultipleInputSearch"://高级快速录入

                    break;
                case "BindFunction"://绑定函数(现在只处理文本，其他的单独处理了)
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                        break;
                    if ($('#TB_' + mapExt.AttrOfOper).length == 1) {
                        $('#TB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "TB_"));
                        break;
                    }
                    break;

                case "FieldNameLink": //标签名超链接
                    if (mapExt.DoWay == 0)
                        break;
                    var ctrl = $("#Lab_" + mapAttr.KeyOfEn);
                    $("#Lab_" + mapAttr.KeyOfEn).wrap($('<a href="javascript:void(0)" onclick="GetFieldNameLink(\'' + mapAttr.Name + '\',\'' + mapExt.MyPK + '\')"></a>'));
                    break;
                case "ReadOnlyLink"://字段值超链接
                    if (mapExt.DoWay == 0)
                        break;

                    var val = $("#TB_" + mapAttr.KeyOfEn).val();
                    if (val == "")
                        break;
                    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
                    frmEleDBs.Retrieve("EleID", mapExt.AttrOfOper, "RefPKVal", pageData.WorkID);
                    var container = $("<div style='width:99%;line-height: 38px;'></div>");
                    $.each(frmEleDBs, function (idx, item) {
                        container.append($('<a href="javascript:void(0)" style="margin-right:10px" onclick="GetReadOnlyLink(\'' + mapAttr.Name + '\',\'' + item.Tag1 + '\',\'' + mapExt.MyPK + '\')">' + item.Tag2 + '</a>'))
                    })

                    $("#TB_" + mapAttr.KeyOfEn).after(container);
                    $("#TB_" + mapAttr.KeyOfEn).hide();
                    break;

                case "FullData"://POP返回值的处理，放在了POP2021.js
                    break;
                case "RegularExpression":
                    $('#TB_' + mapExt.AttrOfOper).data(mapExt);
                    $('#TB_' + mapExt.AttrOfOper).on(mapExt.Tag.substring(2), function () {
                        var mapExt = $(this).data();
                        var filter = mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
                        var re = filter;
                        if (typeof (filter) == "string") {
                            if (filter.indexOf('/') == 0) {
                                filter = filter.substr(1, filter.length - 2);
                            }
                            re = new RegExp(filter);
                        } else {
                            re = filter;
                        }
                        if (re.test($(this).val()) == false) {
                            layer.msg(mapExt.Tag1, { icon: 5 });
                            $(this).css("border-color", "red");
                        } else {
                            $(this).css("border-color", "#eee");
                        }

                    })
                    break;
                default:
                    layer.alert(mapAttr.Name + "字段扩展属性" + mapExt.ExtType + "该类型还未解析，请反馈给开发人员");
                    break;
            }

        });
        return true;

    });
}

function GetFieldNameLink(name, extMyPK) {
    var en = new Entity("BP.Sys.MapExt");
    en.SetPKVal(extMyPK);
    if (en.RetrieveFromDBSources() == 0) {
        alert("字段名" + name + ":弹窗提示配置丢失");
        return
    }
    if (en.DoWay == 0)
        return;
    var doc = en.Doc;
    doc = DealExp(doc);
    switch (parseInt(en.DoWay)) {
        case 0:
            return;
        case 1:
            layer.alert(doc);
            return;
        case 2:
            OpenLayuiDialog(doc, "", window.innerWidth * 2 / 3, 100, "r");
            return;
        case 3:
            OpenLayuiDialog(doc, "", window.innerWidth * 2 / 3, 80, "auto");
            return;
        case 4:
            window.open(doc, '_blank').location
            return;
    }


}
function GetReadOnlyLink(name, val, extMyPK) {
    var en = new Entity("BP.Sys.MapExt");
    en.SetPKVal(extMyPK);
    if (en.RetrieveFromDBSources() == 0) {
        alert("字段值" + name + ":弹窗提示配置丢失");
        return
    }
    if (en.DoWay == 0)
        return;
    var doc = en.Doc;
    doc = doc.replace("@Key", val).replace("@key", val);
    doc = DealExp(doc);
    switch (parseInt(en.DoWay)) {
        case 0:
            return;
        case 1:
            layer.alert(doc);
            return;
        case 2:
            OpenLayuiDialog(doc, "", window.innerWidth * 2 / 3, 100, "r");
            return;
        case 3:
            OpenLayuiDialog(doc, "", window.innerWidth * 2 / 3, 80, "auto");
            return;
        case 4:
            window.open(doc, '_blank').location
            return;
    }

}
function changeValue(tbval, tbID, type) {
    var strgetSelectValue = "";
    var getSelectValueMenbers = $("input[name='CB_" + tbID + "_" + tbID + "']:checked").each(function (j) {
        if (type == 0)
            strgetSelectValue = $(this).val();
        else {
            if (j >= 0) {
                strgetSelectValue += $(this).val() + ",";
            }
        }

    });
    $("#TB_" + tbID).val(strgetSelectValue);
}



/**
 * 对mapExt分组
 * @param {any} mapExts
 */
var isFirstXmSelect = true;
function GetMapExtsGroup(mapExts) {
    var map = {};
    var mypk = "";
    //对mapExt进行分组，根据AttrOfOper
    $.each(mapExts, function (i, mapExt) {
        //不是操作字段不解析
        if (mapExt.AttrOfOper == "")
            return true;
        if (mapExt.ExtType == "DtlImp"
            || mapExt.MyPK.indexOf(mapExt.FK_MapData + '_Table') >= 0
            || mapExt.MyPK.indexOf('PageLoadFull') >= 0
            || mapExt.ExtType == 'StartFlow'
            || mapExt.ExtType == 'AutoFullDLL'
            || mapExt.ExtType == 'ActiveDDLSearchCond'
            || mapExt.ExtType == 'AutoFullDLLSearchCond'
            || mapExt.ExtType == 'HtmlText')
            return true;

        mypk = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;

        if (isFirstXmSelect == true) {
            layui.config({
                base: laybase + 'Scripts/layui/ext/'
            });
            isFirstXmSelect = false;
        }
        if (!map[mypk])
            map[mypk] = [mapExt];
        else
            map[mypk].push(mapExt);
    });
    var res = [];
    Object.keys(map).forEach(key => {
        res.push({
            attrKey: key,
            data: map[key],
        })
    });
    console.log(res);
    return map;
}

/**
 * 枚举，下拉框字段的扩展属性
 * @param {any} mapExts 扩展属性集合
 * @param {any} mapAttr 字段属性
 * @param {any} selectVal 选中的值
 * @param {any} isEnableJS 是否联动其他控件
 * @param {any} model 类型 单选按钮 下拉框 复选框
 * @param {any} frmType 表单类型 傻瓜表单 开发者表单
 * @param {any} tag 标记，无实际意义
 */
function SetRadioSelectMapExt(mapExts, mapAttr, selectVal, isEnableJS, model, frmType, tag, element) {
    //联动其他控件
    if (isEnableJS == true && (selectVal != null && selectVal != undefined && selectVal != "")) {
        if (model == "radio" && selectVal == -1) {
        } else {
            cleanAll(mapAttr.KeyOfEn, frmType);
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, selectVal, frmType);
        }

    }
    if (mapExts == null || mapExts == undefined || mapExts.length == 0)
        return;
    //其他扩展属性
    $.each(mapExts, function (idx, mapExt) {
        //填充其他控件
        switch (mapExt.ExtType) {
            case "FullData": //填充其他控件
                if (model == "checkbox")
                    break;
                var isFullData = GetPara(mapAttr.AtPara, "IsFullData");
                isFullData = isFullData == undefined || isFullData == "" || isFullData != "1" ? false : true;
                if (isFullData == true)
                    DDLFullCtrl(selectVal, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK);
                break;
            case "BindFunction"://绑定函数
                if (tag == true) {
                    var doc = DealExp(mapExt.Doc);
                    DBAccess.RunFunctionReturnStr(doc);
                }
                break;
            case "ActiveDDL"://级联其他控件
                if (model == "checkbox")
                    break;
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlPerant.length == 0 || ddlChild.length == 0)
                    break;
                DDLAnsc(selectVal, "DDL_" + mapExt.AttrsOfActive, mapExt);
                break;
            case "FieldNameLink":
                if (tag == true)
                    break;
                //大块文本帮助.
                if (mapExt.DoWay == 0)
                    break;
                $("#Lab_" + mapAttr.KeyOfEn).wrap($('<a href="javascript:void(0)" onclick="GetFieldNameLink(\'' + mapAttr.Name + '\',\'' + mapExt.MyPK + '\')"></a>'));
                break;
            default:
                layer.alert(mapAttr.Name + "字段扩展属性" + mapExt.ExtType + "该类型还未解析，请反馈给开发人员");
                break;
        }
    })
    layui.form.render();
}
/**
 * 时间字段扩展属性的解析
 * @param {any} mapExts
 * @param {any} mapAttr
 * @param {any} targetID
 */
function SetDateExt(mapExts, mapAttr) {
    var funcDoc = "";
    var roleExt = null;
    var FullExt = null;
    $.each(mapExts, function (k, mapExt1) {
        var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
        mapExt.MyPK = mapExt1.MyPK;
        if (mapExt.ExtType == "BindFunction")
            funcDoc = mapExt.Doc;
        if (mapExt.ExtType == "DataFieldInputRole" && mapExt.DoWay == 1) {
            roleExt = mapExt;
        }
        if (mapExt.ExtType == "FullData") {
            FullExt = mapExt;
        }
        if (mapExt.ExtType == "FieldNameLink") {
            //大块文本帮助.
            if (mapExt.DoWay == 0)
                return true;
            $("#Lab_" + mapAttr.KeyOfEn).wrap($('<a href="javascript:void(0)" onclick="GetFieldNameLink(\'' + mapAttr.Name + '\',\'' + mapExt.MyPK + '\')"></a>'));
        }

    });
    var isFullData = GetPara(mapAttr.AtPara, "IsFullData");
    isFullData = isFullData == undefined || isFullData == "" || isFullData != "1" ? false : true;


    var format = $("#TB_" + mapAttr.KeyOfEn).attr("data-info");
    var type = $("#TB_" + mapAttr.KeyOfEn).attr("data-type");
    var dateOper = "";
    if (roleExt != null) {
        if (roleExt.Tag1 == 1) {//不能选择历史时间
            dateOper = {
                elem: '#TB_' + mapAttr.KeyOfEn,
                format: format, //可任意组合
                type: type,
                min: 0,
                funDoc: funcDoc,
                keyOfEn: mapAttr.KeyOfEn,
                fullMyPK: isFullData == false ? "" : FullExt.MyPK,
                ready: function (date) {
                    if (this.format.indexOf("HH") != -1) {
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
                    }

                },
                change: function (value, date, endDate) {
                    if (this.type != "year")
                        $('.laydate-btns-confirm').click();
                },
                done: function (value, date, endDate) {
                    $(this.elem).val(value);
                    if (this.funDoc != "")
                        DBAccess.RunFunctionReturnStr(this.funDoc);
                    var data = $(this.elem).data();
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);


                    if (this.fullMyPK != "")
                        DDLFullCtrl(value, "TB_" + this.keyOfEn, this.fullMyPK);
                }
            }
        }
        if (roleExt.Tag2 == 1) {
            //根据选择的条件进行日期限制
            var isHaveOper = $("#TB_" + roleExt.Tag4).is(".ccdate");
            var tagM = $.grep(frmData.Sys_MapAttr, function (item) {
                return item.KeyOfEn == roleExt.Tag4 && item.FK_MapData == roleExt.FK_MapData;
            })[0];

            /** var startOper = "";
             startOper = {
                 elem: '#TB_' + roleExt.Tag4,
                 format: format, //可任意组合
                 type: type,
                 operKey: mapAttr.KeyOfEn,
                 operKeyName: mapAttr.Name,
                 oper: roleExt.Tag3,
                 ready: function (date) {
                     if (this.format.indexOf("HH") != -1) {
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
                     }
 
                 },
                 change: function (value, date, endDate) {
                     $('.laydate-btns-confirm').click();
                 },
                 done: function (value, date, endDate) {
                     //比对的时间字段值
                     var operVal = $('#TB_' + this.operKey).val();
                     var oper = this.oper;
                     var msg = "";
                     switch (oper) {
                         case "dayu":
                             if (value >= operVal && operVal != "")
                                 msg = "所选日期不能大于等于[" + this.operKeyName + "]对应的日期时间";
                             break;
                         case "dayudengyu":
                             if (value > operVal && operVal != "")
                                 msg = "所选日期不能大于[" + this.operKeyName + "]对应的日期时间";
                             break;
                         case "xiaoyu":
                             if (value <= operVal && operVal != "")
                                 msg = "所选日期不能小于等于[" + this.operKeyName + "]对应的日期时间";
                             break;
                         case "xiaoyudengyu":
                             if (value < operVal && operVal != "")
                                 msg = "所选日期不能小于[" + this.operKeyName + "]对应的日期时间";
                             break;
                         case "budengyu":
                             if (value == operVal && operVal != "")
                                 msg = "所选日期不能等于[" + this.operKeyName + "]对应的日期时间";
                             break;
                     }
                     if (msg != "") {
                         layer.alert(msg);
                         this.value = "";
                         this.elem.val("");
                     }
 
 
                 }
             } */
            dateOper = {
                elem: '#TB_' + mapAttr.KeyOfEn,
                format: format, //可任意组合
                type: type,
                operKey: roleExt.Tag4,
                operKeyName: tagM.Name,
                oper: roleExt.Tag3,
                mapAttr: mapAttr,
                mapExt: mapExts,
                funDoc: funcDoc,
                fullMyPK: isFullData == false ? "" : FullExt.MyPK,
                keyOfEn: mapAttr.KeyOfEn,
                ready: function (date) {
                    if (this.format.indexOf("HH") != -1) {
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
                    }

                },
                change: function (value, date, endDate) {
                    if (this.type != "year")
                        $('.laydate-btns-confirm').click();
                },
                done: function (value, date, endDate) {
                    //比对的时间字段值
                    var operVal = $('#TB_' + this.operKey).val();
                    var oper = this.oper;
                    var msg = "";
                    switch (oper) {
                        case "dayu":
                            if (value <= operVal && operVal != "") {
                                msg = "所选日期不能小于等于[" + this.operKeyName + "]对应的日期时间";
                                break;
                            }
                            break;
                        case "dayudengyu":
                            if (value < operVal && operVal != "") {
                                msg = "所选日期不能小于[" + this.operKeyName + "]对应的日期时间";
                                break;
                            }
                            break;
                        case "xiaoyu":
                            if (value >= operVal && operVal != "") {
                                msg = "所选日期不能大于等于[" + this.operKeyName + "]对应的日期时间";
                                break;
                            }
                            break;
                        case "xiaoyudengyu":
                            if (value > operVal && operVal != "") {
                                msg = "所选日期不能大于[" + this.operKeyName + "]对应的日期时间";
                                break;
                            }
                            break;
                        case "budengyu":
                            if (value == operVal && operVal != "") {
                                msg = "所选日期不能等于[" + this.operKeyName + "]对应的日期时间";
                                break;
                            }
                            break;
                    }

                    if (msg != "") {
                        layer.alert(msg);
                        value = "";
                    }

                    $(this.elem).val(value);
                    if (this.funDoc != "")
                        DBAccess.RunFunctionReturnStr(this.funDoc);
                    var data = $(this.elem).data();
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                    if (this.fullMyPK != "")
                        DDLFullCtrl(value, "TB_" + this.keyOfEn, this.fullMyPK);
                    if (value == "") {
                        var elemenID = "TB_" + this.keyOfEn;
                        $("#" + elemenID).parent().append(`<input class="ccdate layui-input" data-info="${this.format}" data-type="${this.type}" type="text" id="${elemenID}1" >`)
                        $("#" + elemenID).remove();
                        $(`#${elemenID}1`).attr("id", `${elemenID}`);
                        $(`#${elemenID}`).attr("name", `${elemenID}`);
                        SetDateExt(this.mapExt, this.mapAttr);
                    }
                }
            }
        }

    } else {
        dateOper = {
            elem: '#TB_' + mapAttr.KeyOfEn,
            format: format, //可任意组合
            type: type,
            min: 0,
            funDoc: funcDoc,
            keyOfEn: mapAttr.KeyOfEn,
            fullMyPK: isFullData == false ? "" : FullExt.MyPK,
            ready: function (date) {
                if (this.format.indexOf("HH") != -1) {
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
                }

            },
            change: function (value, date, endDate) {
                if (this.type != "year")
                    $('.laydate-btns-confirm').click();
            },
            done: function (value, date, endDate) {
                $(this.elem).val(value);
                if (this.funDoc != "" && this.funcDoc != undefined)
                    DBAccess.RunFunctionReturnStr(this.funDoc);
                var data = $(this.elem).data();
                if (data && data.ReqDay != null && data.ReqDay != undefined)
                    ReqDays(data.ReqDay);


                if (this.fullMyPK != "" && this.fullMyPK != undefined)
                    DDLFullCtrl(value, "TB_" + this.keyOfEn, this.fullMyPK);
            }
        }
    }

    layui.laydate.render(dateOper);
    $("#TB_" + mapAttr.KeyOfEn).removeClass("ccdate");
}
/**
 * 整数，浮点型，金额型扩展属性的解析
 * @param {any} mapExts
 * @param {any} mapAttr
 */
var statisticColumnsSet = new Set();
var statisticColumnsArr = [];
function SetNumberMapExt(mapExts, mapAttr) {
    // 主表扩展(统计从表)
    var detailExt = {};

    $.each(mapExts, function (idx, mapExt) {
        switch (mapExt.ExtType) {
            case "RegularExpression"://正则表达式
                $('#TB_' + mapExt.AttrOfOper).data(mapExt);
                $('#TB_' + mapExt.AttrOfOper).on(mapExt.Tag.substring(2), function () {
                    var mapExt = $(this).data();
                    var filter = mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
                    var re = filter;
                    if (typeof (filter) == "string") {
                        if (filter.indexOf('/') == 0) {
                            filter = filter.substr(1, filter.length - 2);
                        }

                        re = new RegExp(filter);
                    } else {
                        re = filter;
                    }
                    if (re.test($(this).val()) == false) {
                        layer.msg(mapExt.Tag1, { icon: 5 });
                        $(this).css("border-color", "red");
                    } else {
                        $(this).css("border-color", "#eee");
                    }
                })
                break;
            case "BindFunction"://绑定函数
                $('#TB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "TB_"));
                break;
            case "AutoFull"://自动计算
                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    return true;
                calculator(mapExt);
                break;
            case "AutoFullDtlField": //主表扩展(统计从表)
                var docs = mapExt.Doc.split("\.");

                //判断是否显示大写
                var tag3 = mapExt.Tag3;
                var DaXieAttrOfOper = "";
                if (tag3 == 1)
                    DaXieAttrOfOper = mapExt.Tag4;

                if (docs.length == 3) {
                    var ext = {
                        "DtlNo": docs[0],
                        "FK_MapData": mapExt.FK_MapData,
                        "AttrOfOper": mapExt.AttrOfOper,
                        "DaXieAttrOfOper": DaXieAttrOfOper,
                        "Doc": mapExt.Doc,
                        "DtlColumn": docs[1],
                        "exp": docs[2],
                        "Tag": mapExt.Tag,
                        "Tag1": mapExt.Tag1
                    };
                    if (!$.isArray(detailExt[ext.DtlNo])) {
                        detailExt[ext.DtlNo] = [];
                    }
                    detailExt[ext.DtlNo].push(ext);
                    $(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
                }
                break;
            case "ReqDays"://两个日期自动求天数
                if (mapExt.Tag1 == null || mapExt.Tag1 == "" ||
                    mapExt.Tag2 == null || mapExt.Tag2 == "")
                    break;
                if (isReadonly == true)
                    break;
                if ($('#TB_' + mapExt.AttrOfOper).val() == "" || $('#TB_' + mapExt.AttrOfOper).val() ==="0" )
                    ReqDays(mapExt);
                    $('#TB_' + mapExt.Tag1).data({ "ReqDay": mapExt })
                    $('#TB_' + mapExt.Tag2).data({ "ReqDay": mapExt });
               

                break;
            case "FieldNameLink":
                //大块文本帮助.
                if (mapExt.DoWay == 0)
                    break;
                $("#Lab_" + mapAttr.KeyOfEn).wrap($('<a href="javascript:void(0)" onclick="GetFieldNameLink(\'' + mapAttr.Name + '\',\'' + mapExt.MyPK + '\')"></a>'));
                break;
            case "RMBDaXie"://转金额大写
                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    return true;
                //动态加载转大写的js
                if (location.href.indexOf("CCForm") > 0)
                    Skip.addJs("../Data/JSLibData/CovertMoneyToDaXie.js");
                else if (location.href.indexOf("CCBill") > 0)
                    Skip.addJs("../Data/JSLibData/CovertMoneyToDaXie.js");
                else
                    Skip.addJs("Data/JSLibData/CovertMoneyToDaXie.js");

                //给大写的文本框赋值
                $('#TB_' + mapExt.Doc).val(Rmb2DaXie($('#TB_' + mapExt.AttrOfOper).val()));

                $('#TB_' + mapExt.AttrOfOper).bind("change", function () {
                    var expVal = $(this).val();//获取要转换的值
                    var min = $(this).attr("data-min");
                    var max = $(this).attr("data-max");
                    if (min && parseInt(expVal) < parseInt(min)) {
                        alert("值不能小于设置的最小值:" + min);
                        expVal = min;
                        $(this).val(expVal);
                    }
                    if (max && parseInt(expVal) > parseInt(max)) {
                        alert("值不能大于设置的最大值:" + max);
                        expVal = max;
                        $(this).val(expVal);
                    }

                    $('#TB_' + mapExt.Doc).val(Rmb2DaXie(expVal));//给大写的文本框赋值
                });
                $('#TB_' + mapExt.AttrOfOper).attr("data-daxie", mapExt.Doc);
                break;
            default:
                layer.alert(mapAttr.Name + "字段扩展属性" + mapExt.ExtType + "还未解析，请反馈给开发人员")
                break;
        }
    })
    $.each(detailExt, function (idx, obj) {
        var iframeKey = "#Frame_" + obj[0].DtlNo
        var iframeDtl = $(iframeKey);
        var statisticColumn = detailExt[obj[0].DtlNo]

        if (!statisticColumnsSet.has(statisticColumn)) {
            statisticColumnsSet.add(statisticColumn)
            statisticColumnsArr.push(...statisticColumn)
            console.log(statisticColumnsArr)
        }

        iframeDtl.load(function () {
            $(this).contents().find(":input[id=formExt]").val(JSON.stringify(statisticColumnsArr));
            if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                this.contentWindow.parentStatistics(statisticColumnsArr);
            }
        });

    });

}

function DynamicBind(mapExt, ctrlType) {

    if (ctrlType == "RB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            var doc = DealExp(mapExt.Doc);
            doc = doc.replace("this", this.id);
            DBAccess.RunFunctionReturnStr(doc);
        });
    } else if (ctrlType == "CB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            var doc = DealExp(mapExt.Doc);
            doc = doc.replace("this", this.id);
            DBAccess.RunFunctionReturnStr(doc);
        });
    }
    else {
        $('#' + ctrlType + mapExt.AttrOfOper).on(mapExt.Tag, function () {
            var doc = DealExp(mapExt.Doc);
            doc = doc.replace("this", this.id);
            DBAccess.RunFunctionReturnStr(doc);
        });
    }


}

/**
 * 自动计算两个日期的天数
 * @param {any} mapExt
 */
function ReqDays(mapExt) {
    var ResRDT = mapExt.AttrOfOper;//接收计算天数结果
    var startField = mapExt.Tag1;//开始日期
    var endField = mapExt.Tag2;//结束日期
    var RDTRadio = mapExt.Tag3;//是否包含节假日 0包含，1不包含
    //计算量日期天数
    var StarRDT = $('#TB_' + startField).val();
    var EndRDT = $('#TB_' + endField).val();
    if (StarRDT == "" || EndRDT == "") {
        $('#TB_' + ResRDT).val(0);
        return ;
    }
  
    StarRDT = new Date(StarRDT);
    EndRDT = new Date(EndRDT);
    var countH = parseInt((EndRDT - StarRDT) / 1000 / 60 / 60);//总共的小时数
    res = parseInt(countH / 24); //把相差的毫秒数转换为天数
    //var day = res;
    // var h = (countH - day * 24)/24;
    //res = day + h;
    //判断结束日期是否早于开始日期
    if (parseInt(EndRDT / 1000 / 60 / 60 / 24) < parseInt(StarRDT / 1000 / 60 / 60 / 24)) {
        layer.alert("结束日期不能早于开始日期");
        res = "";
    }
    else {
        //当不包含节假日的时候，需要排除节假日
        if (RDTRadio == 1) {
            var holidayEn = new Entity("BP.Sys.GloVar");
            holidayEn.No = "Holiday";
            if (holidayEn.RetrieveFromDBSources() == 1) {
                var holidays = holidayEn.Val.split(",");
                StarRDT = FormatDate(StarRDT, "MM-dd");
                EndRDT = FormatDate(EndRDT, "MM-dd");
                holidays.forEach(item => {
                    //判断节假日日期是否在时间范围内
                    if (StarRDT <= item && item <= EndRDT)
                        res = res - 1;
                })
                //检查计算的天数
                if (res <= 0) {
                    layer.alert("请假时间内均为节假日");
                    res = "";
                }
            }
        }
    }

    if (res === "" || res == "NaN" || Object.is(res, NaN)) {
        $('#TB_' + endField).val("");
        res = 0;
    }
    $('#TB_' + ResRDT).val(res);
}
/**
 * pop弹出框的处理
 * @param {any} mapAttr
 * @param {any} mapExt
 * @param {any} frmData
 */

function SetSelectExt(mapExts, mapAttr) {
    //判断下拉框，枚举下拉框选中后事件（级联下拉框，填充其他控件，绑定函数，联动其他控件）
    mapAttr.MapExt = mapExts;
    $("#DDL_" + mapAttr.KeyOfEn).data(mapAttr);
    layui.form.on('select(' + mapAttr.KeyOfEn + 'Event)', function (data) {
        var elemID = this.elem.substring(1);
        var mapAttr = $("#" + elemID).data();
        var mapExts = mapAttr.mapExt;
        if (mapAttr.UIIsEnable == 0 || isReadonly == true || $("#DDL_" + mapExt.AttrOfOper).length == 0)
            return true;
        $.each(mapExts, function (i, mapExt) {
            //填充其他控件
            var DDLFull = GetPara(mapAttr.AtPara, "IsFullData");
            if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
                DDLFullCtrl(data.value, elemID, mapExt.MyPK);
                return true;
            }
            //绑定函数
            if (mapExt.ExtType == "BindFunction")
                DynamicBind(mapExt, "DDL_");
            //级联下拉框
            if (mapExt.ExtType == "ActiveDDL") {
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlChild.length == 0)
                    return true;
                DDLAnsc(data.value, "DDL_" + mapExt.AttrsOfActive, mapExt.MyPK);
            }
            return true;
        });

        //联动其他控件
        InitFoolLink(mapAttr, 0);
        layui.form.render('select');
    });

}
/**
 * 文本字段Pop弹出框属性扩展
 * @param {any} popType
 * @param {any} mapAttr
 * @param {any} mapExt
 * @param {any} frmData
 */
var isHaveLoadPop = false;
var isHaveLoagMtags = false;
function PopMapExt(popType, mapAttr, mapExt, frmData, baseUrl, mapExts) {
    if (isHaveLoadPop == false) {
        Skip.addJs(baseUrl + "/JS/Pop2021.js");
        isHaveLoadPop = true;
    }
    switch (popType) {
        case "PopBranches": //树干简单模式.
            var showModel = GetPara(mapExt.AtPara, "ShowModel");
            showModel = showModel == null || showModel == undefined || showModel == "" ? 0 : showModel;
            if (showModel == "1")
                CommPop(popType, mapAttr, mapExt, frmData, mapExts);
            else {
                if (isHaveLoagMtags == false) {
                    Skip.addJs(baseUrl + "JS/mtags2021.js");
                    isHaveLoagMtags = true;
                }
                CommPopDialog(popType, mapAttr, mapExt, null, frmData, baseUrl, mapExts);
            }

            break;
        case "PopBranchesAndLeaf": //树干叶子模式.
        case "PopTableSearch": //表格查询.
        case "PopSelfUrl": //自定义url.
            if (isHaveLoagMtags == false) {
                Skip.addJs(baseUrl + "JS/mtags2021.js");
                isHaveLoagMtags = true;
            }
            CommPopDialog(popType, mapAttr, mapExt, null, frmData, baseUrl, mapExts);
            break;
        case "PopBindSFTable": //绑定字典表，外部数据源.
        case "PopBindEnum": //绑定枚举.
        case "PopTableList": //绑定实体表.
        case "PopGroupList": //分组模式.
            CommPop(popType, mapAttr, mapExt, frmData, mapExts);
            break;
        default: break;
    }
}

/**
 * 判断当前的数据是不是已经存在Sys_FrmEeleDB中
 * @param {any} keyVal
 * @param {any} selects
 */
function IsHaveSelect(keyVal, selects) {
    if (selects.length == 0)
        return false;
    var isHave = false
    $.each(selects, function (i, item) {
        if (item.Tag1 == keyVal) {
            isHave = true;
            return false;
        }
    });
    return isHave;
}


/**
 * 获取表单数据
 * @param {any} dataJson 表单数据JSON集合
 */
function getFormData(dataJson) {
    //处理特殊的表单字段
    //1.富文本编辑器
    if ($(".rich").length > 0 && richTextType == "tinymce") {
        $(".rich").each(function (i, item) {
            var edit = layui.tinymce.get('#' + item.id)
            var val = edit.getContent();
            dataJson[item.id] = val;
        })
    }
    //2.复选框多选
    var mcheckboxs = $(".mcheckbox");
    if (mcheckboxs.length > 0) {
        const sorted = groupBy(mcheckboxs, function (item) {
            return item.name;
        });
        for (var key in sorted) {
            var ids = [];
            $("input[name='" + key + "']:checked").each(function (index, item) {
                ids.push($(this).val())
            });
            dataJson[key] = ids.join(",");
        }
    }
    //3.获取复选框
    var checkboxs = $(".checkbox");
    if (checkboxs.length > 0) {
        $.each(checkboxs, function (idx, item) {
            var itemID = item.id.replace("DIV_", "CB_");
            if (dataJson[itemID] == undefined)
                dataJson[itemID] = 0;
        })

    }
    //4.获取外部数据源T的赋值
    var ddls = $(".ddl-ext");
    if (ddls.length > 0) {
        $.each(ddls, function (idx, ddl) {
            var ctrlID = ddl.id;

            var item = $("#" + ctrlID).children('option:checked').text();
            ctrlID = ctrlID.replace("DDL_", "TB_") + "T";
            if ($("#" + ctrlID).length == 1)
                dataJson[ctrlID] = item;
        })
    }
    //5.树形结构

    //6.获取URL上的值
    return dataJson;
}
/**
 * 表单字段中自动计算
 * @param {any} mapExt
 */
function calculator(mapExt) {
    if (!testExpression(mapExt.Doc)) {
        console.log("MyPk: " + mapExt.MyPK + ", 表达式: '" + mapExt.Doc + "'格式错误");
        return false;
    }
    var targets = [];
    var index = -1;
    for (var i = 0; i < mapExt.Doc.length; i++) {	// 对于复杂表达式需要重点测试
        var c = mapExt.Doc.charAt(i);
        if (c == "(") {
            index++;
        } else if (c == ")") {
            targets.push(mapExt.Doc.substring(index + 1, i));
            i++;
            index = i;
        } else if (/[\+\-|*\/]/.test(c)) {
            targets.push(mapExt.Doc.substring(index + 1, i));
            index = i;
        }
    }
    if (index + 1 < mapExt.Doc.length) {
        targets.push(mapExt.Doc.substring(index + 1, mapExt.Doc.length));
    }
    //
    var expression = {
        "judgement": [],
        "execute_judgement": [],
        "calculate": mapExt.Doc
    };
    $.each(targets, function (i, o) {
        if (o.indexOf("@") == -1)
            return true;
        var target = o.replace("@", "");
        var element = "$(':input[name=TB_" + target + "]')";
        expression.judgement.push(element + ".length==0");
        expression.execute_judgement.push("!isNaN(parseFloat(" + element + ".val().replace(/,/g,'')))");
        expression.calculate = expression.calculate.replace(o, "parseFloat(" + element + ".val().replace(/,/g,''))");
    });
    (function (targets, expression, resultTarget, pk, expDefined, fk_mapData) {
        $.each(targets, function (i, o) {
            if (o.indexOf("@") == -1)
                return true;
            var target = o.replace("@", "");

            $(":input[name=TB_" + target + "]").bind("change", function () {

                var evalExpression = "var result = '';";
                if (expression.judgement.length > 0) {
                    var str = "if(" + expression.judgement.join("||") + "){";
                    evalExpression += str.replace(/\s|\xA0/g, "");
                    evalExpression += "alert('MyPk:" + pk + ",表达式:[" + expDefined.replace(/\s|\xA0/g, "") + "]" + "中有对象在当前页面不存在');"

                    evalExpression += "} ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += "else if(" + expression.execute_judgement.join("&&").replace(/\s|\xA0/g, "") + "){";;
                }
                if (expression.calculate.length > 0) {
                    evalExpression += "result=" + expression.calculate.replace(/\s|\xA0/g, "") + "; ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += "} ";
                }
                var result = cceval(evalExpression);

                if (typeof result != "undefined") {
                    var mapAttr = $.grep(frmMapAttrs, function (item) {
                        return item.MyPK == fk_mapData + "_" + resultTarget;
                    });
                    if (mapAttr[0].MyDataType == 2)
                        result = parseInt(result);
                    else {
                        var defVal = mapAttr[0].DefVal;
                        var bit = null;
                        if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
                            bit = defVal.substring(defVal.indexOf(".") + 1).length;
                        if (bit == null || bit == undefined || bit == "")
                            bit = 2;
                        result = numberFormat(result, bit);
                    }

                } else {
                    result = 0;
                }


                $(":input[name=TB_" + resultTarget + "]").val(result);
                var daXie = $(":input[name=TB_" + resultTarget + "]").data("daxie");
                if (daXie != null && daXie != undefined && daXie != "")
                    $("#TB_" + daXie).val(Rmb2DaXie(result));
            });
            if (i == 0) {
                $(":input[name=TB_" + target + "]").trigger("change");
            }
        });
    })(targets, expression, mapExt.AttrOfOper, mapExt.MyPK, mapExt.Doc, mapExt.FK_MapData);
    $(":input[name=TB_" + mapExt.AttrOfOper + "]").attr("disabled", true);
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
        if (keyOfEn == ele) {
            result = frmData.MainTable[0][ele];
            if (result == null) {
                result = "";
            }
            break;
        }
    }

    return result = unescape(result);
}
/**
 * 星级评分事件
 * @param {any} isReadonly
 */
function setScore(isReadonly) {
    if (isReadonly == false) {
        var scoreDiv = $(".score-star");
        $.each(scoreDiv, function (idex, item) {
            var divId = $(item).attr("id");
            var KeyOfEn = divId.substring(3);//获取字段值
            $("#Star_" + KeyOfEn + " img").click(function () {
                var index = $(this).index() + 1;
                $("#Star_" + KeyOfEn + " img:lt(" + index + ")").attr("src", "Style/Img/star_2.png");
                $("#SP_" + KeyOfEn + " strong").html(index + "  分");
                $("#TB_" + KeyOfEn).val(index);//给评分的隐藏input赋值
                index = index - 1;
                $("#Star_" + KeyOfEn + " img:gt(" + index + ")").attr("src", "Style/Img/star_1.png");
            });
        });

    }
}
/**
 * 地图选择
 * @param {any} MapID
 * @param {any} UIIsEnable
 */
function figure_Template_Map(MapID, UIIsEnable) {
    //var mainTable = frmData.MainTable[0];
    //var AtPara = "";
    ////通过MAINTABLE返回的参数
    //for (var ele in mainTable) {
    //    if (ele == "AtPara" && mainTable != '') {
    //        AtPara = mainTable[ele];
    //        break;
    //    }
    //}
    //if (AtPara == "")
    AtPara = $("#TB_AtPara").val();
    var baseUrl = "./CCForm/";
    if (currentURL.indexOf("CCForm") != -1)
        baseUrl = "./";
    if (currentURL.indexOf("CCBill") != -1)
        baseUrl = "../CCForm/";
    var url = baseUrl + "Map.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + MapID + "&IsReadonly=" + isReadonly + "&Paras=" + AtPara;
    OpenLayuiDialog(url, "地图", window.innerWidth / 2, 80, "auto");
}

/**
 * 签字版
 * @param {any} HandWriteID 签字版字段ID
 */
function figure_Template_HandWrite(HandWriteID) {
    var url = "./CCForm/";
    if (GetHrefUrl().indexOf("CCForm") != -1)
        url = "./";
    if (GetHrefUrl().indexOf("CCBill") != -1)
        url = "../CCForm/";
    url += "HandWriting.htm?WorkID=" + pageData.WorkID + "&FK_Node=" + pageData.FK_Node + "&KeyOfEn=" + HandWriteID;
    OpenLayuiDialog(url, '签字板', window.innerWidth / 2 - 30, 50, "auto");
}
/**
 * 给签字版赋值
 * @param {any} HandWriteID 签字版字段ID
 * @param {any} imagePath 签字版图片路径
 * @param {any} type 类型 0 普通字段签字版  1审核组件签字版
 */
function setHandWriteSrc(HandWriteID, imagePath, type) {
    var src = "../";
    if (type == 0) {
        if (currentURL.indexOf("CCForm") != -1 || currentURL.indexOf("CCBill") != -1)
            src = "../../";
        if (currentURL.indexOf("AdminFrm.htm") != -1)
            src = "../../../";
        src = src + imagePath.substring(imagePath.indexOf("DataUser"));
        document.getElementById("Img" + HandWriteID).src = "";
        $("#Img" + HandWriteID).attr("src", src);
        $("#TB_" + HandWriteID).val(imagePath);
    }
    if (type == 1) {
        $("#Img_" + HandWriteID).attr("src", imagePath);
        if ("undefined" != typeof writeImg)
            writeImg = imagePath;
    }

    layer.close(layer.index);
}

/**
 * 使用卡片模式打开从表数据或者新增一条数据
 * @param {any} ensName
 * @param {any} refPKVal
 * @param {any} pkVal
 * @param {any} frmType
 * @param {any} InitPage
 */
function DtlFrm(ensName, refPKVal, pkVal, frmType, InitPage) {
    // model=1 自由表单, model=2傻瓜表单.
    var wWidth = window.innerWidth * 2 / 3;
    if (wWidth > 1200) {
        wWidth = 1000;
    }
    var isNew = 1;
    if (pkVal != "0")
        isNew = 0;
    var params = GetParentParams();
    var url = basePath + '/WF/CCForm/DtlFrm.htm?EnsName=' + ensName + '&RefPKVal=' + refPKVal + "&FrmType=" + frmType + '&OID=' + pkVal + "&IsNew=" + isNew + params;

    OpenLayuiDialog(url, '编辑', wWidth, null, "r", false, false, false, null, function () {
        var iframe = $(window.frames["dlg"]).find("iframe");
        if (iframe.length > 0) {
            iframe[0].contentWindow.dtlFrm_Delete();
        }
        if (typeof InitPage === "function") {
            InitPage.call();
        } else {
            alert("请手动刷新表单");
        }
    });
}

function GetParentParams() {
    var params = "";
    if (typeof parent.frmData == "undefined")
        return "";

    var mainAttrs = parent.frmData.Sys_MapAttr;
    $.each(mainAttrs, function (idx, attr) {
        if (attr.KeyOfEn == "OID" || attr.KeyOfEn == "WorkID" || attr.KeyOfEn == "PWorkID" || attr.KeyOfEn == "FID"
            || attr.KeyOfEn == "RDT" || attr.KeyOfEn == "Rec" || attr.KeyOfEn == "EnsName" || attr.KeyOfEn == "RefPkVal"
            || attr.KeyOfEn == "AtPara" || attr.KeyOfEn == "Title" || attr.KeyOfEn == "Emps" || attr.KeyOfEn == "FK_Dept" || attr.KeyOfEn == "FK_NY")
            return true;
        if (attr.TextModel == 2 || attr.TextModel == 3)
            return true;
        var ele = $("#TB_" + attr.KeyOfEn, parent.document);
        if (ele.length != 0) {
            params += "&" + attr.KeyOfEn + "=" + ele.val();
            return true;
        }
        ele = $("#DDL_" + attr.KeyOfEn, parent.document);
        if (ele.length != 0) {
            params += "&" + attr.KeyOfEn + "=" + ele.val();
            return true;
        }
        ele = $("#CB_" + attr.KeyOfEn, parent.document);
        if (ele.length != 0) {
            params += "&" + attr.KeyOfEn + "=" + ele.val();
            return true;
        }
        ele = $("input[name='RB_" + attr.KeyOfEn + "']", parent.document);
        if (ele.length != 0) {
            params += "&" + attr.KeyOfEn + "=" + $("input[name='RB_" + attr.KeyOfEn + "']:checked", parent.document).val();
            return true;
        }
        ele = $("input[name='CB_" + attr.KeyOfEn + "']", parent.document);
        if (ele.length != 0) {
            params += "&" + attr.KeyOfEn + "=" + $("input[name='CB_" + attr.KeyOfEn + "']:checked", parent.document).val();
            return true;
        }
    })
    return params;
}
/**
 * 检查上传附件的数量
 */
function checkAths() {
    //获取字段附件
    var msg = "";
    $.each(FiledFJ, function (i, item) {
        if ($("#athModel_" + item.key).length != 0) {
            var athNum = $("#athModel_" + item.key).find("a").length;
            var minNum = item.info.minNum;
            var maxNum = item.info.maxNum;
            if (athNum < minNum)
                msg += item.name + "上传附件数量不能小于" + minNum + ";";
            if (athNum > maxNum)
                msg += item.name + "您最多上传[" + maxNum + "]个附件" + ";";
        }
    })
    if (msg != "")
        return msg;

    // 不支持火狐浏览器。
    if ("undefined" != typeof AthParams && AthParams.AthInfo != undefined) {
        var aths = document.getElementsByName("Ath");
        for (var i = 0; i < aths.length; i++) {
            var athment = aths[i].id.replace("Div_", "");
            if (AthParams.AthInfo[athment] != undefined && AthParams.AthInfo[athment].length > 0) {
                var athInfo = AthParams.AthInfo[athment][0];
                var minNum = athInfo[0];
                var maxNum = athInfo[1];
                var athNum = $("#Div_" + athment + " table tbody .athInfo").length;
                if (athNum.length == 0)
                    athNum = $("#Div_" + athment + " .athInfo").length;

                if (athNum < minNum)
                    return athment + "上传附件数量不能小于" + minNum;;
                if (athNum > maxNum)
                    return athment + "您最多上传[" + maxNum + "]个附件";
            }
        }
    }
    return "";

}


//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获得所有的class=mustInput的元素.
    var lbs = $('.mustInput');
    var msg = "";
    $.each(lbs, function (i, obj) {

        if ($(obj).parent().css('display') != 'none' && (($(obj).parent().next().css('display')) != 'none' || ($(obj).siblings("textarea").css('display')) != 'none')) {
        } else {
            return;
        }

        var keyofen = $(obj).data().keyofen;
        if (keyofen == undefined)
            return;
        var ele = $("#TB_" + keyofen);
        if (ele.length == 0)
            ele = $("#DDL_" + keyofen);
        if (ele.length == 0)
            ele = $("#CB_" + keyofen);
        if (ele.length == 0) {
            ele = $("input[name='CB_" + keyofen + "']");
            if (ele.length != 0) {
                var val = $("input[name='CB_" + keyofen + "']:checked").val();
                if (val == -1 || val == undefined) {
                    checkBlankResult = false;
                    $("input[name$='CB_" + keyofen + "']").parent().addClass('errorInput');
                } else {
                    $("input[name$='CB_" + keyofen + "']").parent().removeClass('errorInput');
                }
                return;
            } else {
                var val = $("input[name='RB_" + keyofen + "']:checked").val();
                if (val == -1 || val == undefined) {
                    checkBlankResult = false;
                    $("input[name$='RB_" + keyofen + "']").parent().addClass('errorInput');
                } else {
                    $("input[name$='RB_" + keyofen + "']").parent().removeClass('errorInput');
                }

                return;
            }
        }
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
                if (ele.val() == null || ele.val() == "" || ele.val() == -1 || ele.children('option:checked').text() == "*请选择") {
                    checkBlankResult = false;
                    ele.parent().addClass('errorInput');
                } else {
                    ele.parent().removeClass('errorInput');
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

    });


    return checkBlankResult;
}

/**********************************************************其他通用的方法************************************************************************************/
function ReqDDL(ddlID) {
    var v = document.getElementById('DDL_' + ddlID).value;
    if (v == null) {
        alert('没有找到ID=' + ddlID + '的下拉框控件.');
        return;
    }
    return v;
}

// 获取TB值
function ReqTB(tbID) {
    var v = document.getElementById('TB_' + tbID).value;
    if (v == null) {
        alert('没有找到ID=' + tbID + '的文本框控件.');
        return;
    }
    return v;
}

// 获取CheckBox值
function ReqCB(cbID) {
    var v = document.getElementById('CB_' + cbID).value;
    if (v == null) {
        alert('没有找到ID=' + cbID + '的文本框控件.');
        return;
    }
    return v;
}

// 获取 单选按钮的 值.
function ReqRadio(keyofEn, enumIntVal) {
    var v = document.getElementById('RB_' + keyofEn + '' + enumIntVal);
    if (v == null) {
        alert('没 有找到字段名=' + keyofEn + '值=' + enumIntVal + '的控件.');
        return;
    }
    return v.checked;
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
        alert('没有找到ID=' + cbID + '的单选控件.');
    }
    return v;
}

//设置隐藏?
function SetDevelopCtrlHidden(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length == 0)
        ctrl = $("#DDL_" + key);
    if (ctrl.length == 0)
        ctrl = $("#CB_" + key);
    if (ctrl.length == 0)
        ctrl = $("#SR_" + key);
    if (ctrl.length == 0)
        ctrl = $("#SC_" + key);
    if (ctrl.length == 0) {
        layer.alert(key + "的类型判断不正确，请告知开发人员");
        return;
    }
    var parent = ctrl.parent()[0];
    if (parent.tagName.toLowerCase() != "td")
        parent = $(parent).parent()[0];
    if (parent.tagName.toLowerCase() == "td") {
        if (parent.id != "CCForm")
            $(parent).hide();
        //当前节点的兄弟节点，如果没有input，select,就隐藏
        var prev = $(parent).prev();
        if (prev.find("input").length == 0 && prev.find("select").length == 0)
            prev.hide();
    }


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
/**
 * 字段附件的解析
 * @param {any} mapAttr
 */
var FiledFJ = [];
function getFieldAth(mapAttr,aths) {
    var Obj = {};
    //获取上传附件列表的信息及权限信息
    var nodeID = pageData.FK_Node;
    var no = nodeID.toString().substring(nodeID.toString().length - 2);
    var IsStartNode = 0;
    if (no == "01")
        IsStartNode = 1;

    //创建附件描述信息. 
    var mypk = mapAttr.MyPK;

    //获取附件显示的格式
    var athShowModel = GetPara(mapAttr.AtPara, "AthShowModel")||"0";

    let ath = $.grep(aths, function (ath) { return ath.MyPK == mypk });
    ath = ath.length > 0 ? ath[0] : null;
    if (ath == null) {
        layer.alert("没有找到附件属性,请联系管理员");
        return;
    }
    var noOfObj = mypk.replace(mapAttr.FK_MapData + "_", "");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("WorkID", pageData.WorkID);
    handler.AddPara("FID", pageData.FID);
    handler.AddPara("FK_Node", nodeID);
    handler.AddPara("FK_Flow", pageData.FK_Flow);
    handler.AddPara("IsStartNode", IsStartNode);
    handler.AddPara("PKVal", pageData.WorkID);
    handler.AddPara("Ath", noOfObj);
    handler.AddPara("FK_MapData", mapAttr.FK_MapData);
    handler.AddPara("FromFrm", mapAttr.FK_MapData);
    handler.AddPara("FK_FrmAttachment", mypk);
    data = handler.DoMethodReturnString("Ath_Init");

    if (data.indexOf('err@') == 0) {
        layer.alert(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        var url = data.replace('url@', '');
        SetHref(url);
        return;
    }
    data = JSON.parse(data);
    var dbs = data["DBAths"];
    athDesc = data["AthDesc"][0];
    var css = "style='text-align:left;padding-left:10px;border:1px solid #eee'";
    var clickEvent = "";
    if (athDesc.IsUpload == 1 && isReadonly == false) {
        clickEvent = "<button type='button' class='layui-btn layui-btn-sm' style='margin:5px' onclick='OpenAth(\"" + mapAttr.Name + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.MyPK + "\",\"" + mapAttr.AtPara + "\",\"" + mapAttr.FK_MapData + "\",0,0)'><i class='layui-icon layui-icon-upload'></i>上传文件</button>";
        FiledFJ.push({
            key: mapAttr.KeyOfEn,
            name: mapAttr.Name,
            info: {
                minNum: athDesc.NumOfUpload,
                maxNum: athDesc.TopNumOfUpload
            }
        })

    } else {
        clickEvent = "<button type='button' class='layui-btn layui-btn-sm' style='margin:5px' onclick='OpenAth(\"" + mapAttr.Name + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.MyPK + "\",\"" + mapAttr.AtPara + "\",\"" + mapAttr.FK_MapData + "\",0,1)'><i class='layui-icon layui-icon-upload'></i>查看文件详情</button>";
        FiledFJ.push({
            key: mapAttr.KeyOfEn,
            name: mapAttr.Name,
            info: {
                minNum: athDesc.NumOfUpload,
                maxNum: athDesc.TopNumOfUpload
            }
        })
    }
    if (dbs.length == 0) {
        if (athDesc.IsUpload == 1 && isReadonly == false)
            return "<div " + css + " id='athModel_" + mapAttr.KeyOfEn + "'>" + clickEvent + "</div>";
        else
            return "<div " + css + "  id='athModel_" + mapAttr.KeyOfEn + "' class='athModel'><label>附件(0)</label></div>";
    }
    var eleHtml = "";
   
    if (athShowModel == "" || athShowModel == 0)
        return "<div " + css + "  id='athModel_" + mapAttr.KeyOfEn + "' data-type='0'><label >" + clickEvent + "附件(" + dbs.length + ")</label></div>";

    eleHtml = "<div " + css + "  id='athModel_" + mapAttr.KeyOfEn + "' data-type='1'>" + clickEvent + "<br/>";

    var workID = GetQueryString("WorkID");
    for (var i = 0; i < dbs.length; i++) {
        var db = dbs[i];
        var exts = db.FileExts;
        if (getIsImg(exts) == true) {
            var url = GetFileStream(db.MyPK, db.FK_FrmAttachment);
            eleHtml += "<a id='" + db.MyPK + "' class='image-item athInfo' style='background-image: url(&quot;" + url + "&quot;);' href='javaScript:void(0)' onclick='ShowBigPic(this)' title='" + db.FileName + "'></a>";
        } else {
            var url = "./Img/FileType/" + db.FileExts + ".gif";
            eleHtml += "<a class='image-item' style='font-size:12px;background-color: rgb(0 0 0 / 29%);background-image: url(&quot;" + url + "&quot;);'  href=\"javascript:Down2018('" + db.MyPK + "','" + workID + "')\" title='" + db.FileName + "'><div style='height:75px;background: rgba(255,255,255,0.6);padding-top: 20px;font-weight: bold;text-overflow: ellipsis;overflow: hidden;'>" + db.FileName + "</div></a>"

        }
    }
    eleHtml += "</div>";
    return eleHtml;
}

function getIsImg(ext) {
    switch (ext.toLowerCase()) {
        case "gif":
        case "jpg":
        case "jepg":
        case "bmp":
        case "png":
        case "tif":
        case "gsp":
            return true;
    }
    return false;
}
function ShowBigPic(obj) {
    var _this = $(obj); //将当前的pimg元素作为_this传入函数
    var src = _this.css("background-image").replace("url(\"", "").replace("\")", "")
    imgShow(this, src);
}
/**
 *图片附件预览
 * @param {any} obj
 */
function imgShow(obj, src) {
    if (src == null || src == undefined)
        src = obj.src;
    var img = new Image();
    img.src = src;
    img.onload = () => {
        var height = img.height + 50; //获取图片高度
        if (height > window.innerHeight - 150)
            height = window.innerHeight - 150;
        var width = img.width; //获取图片宽度
        var imgHtml = "<div style='text-align:center'><img src='" + src + "' /></div>";
        //弹出层
        window.parent.layer.open({
            type: 1,
            shade: 0.8,
            offset: 'auto',
            area: ['80%', '80%'],
            shadeClose: true,//点击外围关闭弹窗
            scrollbar: false,//不现实滚动条
            title: "",
            closeBtn: 1,
            content: imgHtml, //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响  
            cancel: function () {
                //layer.msg('捕获就是从页面已经存在的元素上，包裹layer的结构', { time: 5000, icon: 6 });  
            }
        });
    }

}

//树形结构
function findChildren(jsonArray, parentNo) {
    var appendToTree = function (treeToAppend, o) {
        $.each(treeToAppend, function (i, child) {
            if (o.id == child.ParentNo)
                o.children.push({
                    "id": child.No,
                    "text": child.Name,
                    "children": []
                });
        });

        $.each(o.children, function (i, o) {
            appendToTree(jsonArray, o);
        });

    };

    var jsonTree = [];
    var jsonchildTree = [];
    if (jsonArray.length > 0 && typeof parentNo !== "undefined") {
        $.each(jsonArray, function (i, o) {
            if (o.ParentNo == parentNo) {
                jsonchildTree.push(o);
                jsonTree.push({
                    "id": o.No,
                    "text": o.Name,
                    "children": []
                });
            }
        });

        $.each(jsonTree, function (i, o) {
            appendToTree(jsonArray, o);
        });

    }

    function _(treeArray) {
        $.each(treeArray, function (i, o) {
            if ($.isArray(o.children)) {
                if (o.children.length == 0) {
                    o.children = undefined;
                } else {
                    _(o.children);
                }
            }
        });
    }
    _(jsonTree);
    return jsonTree;
}

//修改公文正文组件的值,这里需要处理  todo:yln
function ChangeGovDocFilcceval(docWord) {

    if ($("#TB_GovDocFile").length == 1) {
        $("#TB_GovDocFile").val(docWord);
    }

    $('#bootStrapdlg').modal('hide');
}

function ChangeDocWordVal(docWord) {

    if ($("#TB_DocWord").length == 1) {
        $("#TB_DocWord").val(docWord);
    }

    $('#bootStrapdlg').modal('hide');
}
//此方法用于显示收文编号的值
function ChangeDocWordReceive(docWord) {

    if ($("#TB_DocWordReceive").length == 1) {
        $("#TB_DocWordReceive").val(docWord);
    }

    $('#bootStrapdlg').modal('hide');
}

/**
 * 跳转常用短语页面
 * @param {any} nodeID 当前节点ID
 * @param {any} GroupKey 所属短语类型 CYY,FlowBBS,WorkReturn
 * @param {any} elementID 选择短语后赋值元素
 */
function UsefulExpresFlow(attrKey, elementID) {
    var url = basePath + "/WF/WorkOpt/UsefulExpresFlow.htm?AttrKey=" + attrKey + "&ElementID=" + elementID + "&m=" + Math.random();
    var W = window.innerWidth / 2;
    OpenLayuiDialog(url, "常用短语", W, 70, "auto");
}



//弹出附件
function OpenAth(title, keyOfEn, athMyPK, atPara, FK_MapData, frmType, isRead) {
    var H = 70;
    var W = window.innerWidth / 2;
    var url = url = basePath + "/WF/CCForm/Ath.htm?1=1&AthPK=" + athMyPK + "&OID=" + pageData.WorkID + "&FK_MapData=" + FK_MapData + "&FK_Node=" + pageData.FK_Node + "&IsReadonly=" + isRead;
    if (isRead == 1) {
        OpenLayuiDialog(url, title, W, H, "auto", false);
        return;
    }
    OpenLayuiDialog(url, title, W, H, "auto", false, false, false, null, function () {
        //获取附件显示的格式
        var athShowModel = GetPara(atPara, "AthShowModel")||"0";

        var ath = new Entity("BP.Sys.FrmAttachment");
        ath.MyPK = athMyPK;
        if (ath.RetrieveFromDBSources() == 0) {
            layer.alert("没有找到附件属性,请联系管理员");
            return;
        }
        var data = Ath_Init(athMyPK, FK_MapData)

        if (data.indexOf('err@') == 0) {
            layer.alert(data);
            return;
        }

        if (data.indexOf('url@') == 0) {
            var url = data.replace('url@', '');
            SetHref(url);
            return;
        }
        data = JSON.parse(data);
        var dbs = data["DBAths"];
        var css = "style='text-align:left;padding-left:10px;border:1px solid #eee'";
        var clickEvent = "";
        if (athDesc == undefined) {
            var athDesc = data["AthDesc"][0];
        }
        if (athDesc.IsUpload == 1 && isReadonly == false)
            clickEvent = "<button type='button' class='layui-btn layui-btn-sm' style='margin:5px' onclick='OpenAth(\"" + title + "\",\"" + keyOfEn + "\",\"" + athMyPK + "\",\"" + atPara + "\",\"" + FK_MapData + "\"," + frmType + ")'><i class='layui-icon layui-icon-upload'></i>上传文件</button>";

        if (dbs.length == 0 && frmType != 8) {
            $("#athModel_" + keyOfEn).html(clickEvent);
            return;
        }

        var eleHtml = "";
        if (athShowModel == "" || athShowModel == 0) {
            $("#athModel_" + keyOfEn).html(clickEvent + "<label >附件(" + dbs.length + ")</label>");
            return;
        }

        var workID = GetQueryString("WorkID");
        var curUrl = GetHrefUrl();
        var isRoot = false;
        if (curUrl.indexOf("MyFlowGener.htm") != -1 || curUrl.indexOf("MyViewGener.htm"))
            isRoot = true;
        var url = "";
        for (var i = 0; i < dbs.length; i++) {
            var db = dbs[i];
            if (isRoot == true)
                url = "./Img/FileType/" + db.FileExts + ".gif";
            else
                url = "../Img/FileType/" + db.FileExts + ".gif";
            var exts = db.FileExts;
            if (getIsImg(exts) == true) {
                url = GetFileStream(db.MyPK, db.FK_FrmAttachment);
                eleHtml += "<a id='" + db.MyPK + "' class='image-item athInfo' style='background-image: url(&quot;" + url + "&quot;);' href='javaScript:void(0)' onclick='ShowBigPic(this)' title='" + db.FileName + "'></a>";
            } else {
                eleHtml += "<a class='image-item' style='font-size:12px;background-color: rgb(0 0 0 / 29%);background-image: url(&quot;" + url + "&quot;);'  href=\"javascript:Down2018('" + db.MyPK + "','" + workID + "')\" title='" + db.FileName + "'><div style='height:75px;background: rgba(255,255,255,0.6);padding-top: 20px;font-weight: bold;text-overflow: ellipsis;overflow: hidden;'>" + db.FileName + "</div></a>"

            }
        }
        if (frmType == 8)
            $("#athModel_" + keyOfEn).children().last().html(eleHtml);
        else
            $("#athModel_" + keyOfEn).html(clickEvent + "<br/>" + eleHtml);

    });


}

function Ath_Init(mypk, FK_MapData) {

    var IsStartNode = 0;
    var nodeID = pageData.FK_Node;
    if (nodeID == null || nodeID == undefined || nodeID == "")
        nodeID = 0;
    var no = nodeID.toString().substring(nodeID.toString().length - 2);
    if (no == "01")
        IsStartNode = 1;

    var noOfObj = mypk.replace(FK_MapData + "_", "");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("WorkID", pageData.WorkID);
    handler.AddPara("FID", pageData.FID);
    handler.AddPara("FK_Node", nodeID);
    handler.AddPara("FK_Flow", pageData.FK_Flow);
    handler.AddPara("IsStartNode", IsStartNode);
    handler.AddPara("PKVal", pageData.WorkID);
    handler.AddPara("Ath", noOfObj);
    handler.AddPara("FK_MapData", FK_MapData);
    handler.AddPara("FromFrm", FK_MapData);
    handler.AddPara("FK_FrmAttachment", mypk);
    data = handler.DoMethodReturnString("Ath_Init");
    return data;
}

/**
 * 傻瓜表单点击全屏时从表全屏显示
 * @param {any} dtlNo
 */
function WindowOpenDtl(dtlNo) {
    var iframeDtl = $("#Dtl_" + dtlNo);
    iframeDtl[0].contentWindow.WindowOpenDtl();
}

//正则表达式检查
function CheckRegInput(oInput, filter, tipInfo) {
    var mapExt = $('#' + oInput).data();
    var filter = mapExt.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}');
    var oInputVal = $("#" + oInput).val();
    var result = true;
    if (oInput != '') {
        var re = filter;
        if (typeof (filter) == "string") {
            if (filter.indexOf('/') == 0) {
                filter = filter.substr(1, filter.length - 2);
            }

            re = new RegExp(filter);
        } else {
            re = filter;
        }
        result = re.test(oInputVal);
    }
    if (!result) {
        $("#" + oInput).addClass('errorInput');
        var errorId = oInput + "error";
        if ($("#" + errorId).length == 0) {
            var span = $("<span id='" + errorId + "' style='color:red'></span>");
            $("#" + oInput).parent().append(span);
        }
        $("#" + errorId).html(tipInfo);

    } else {
        $("#" + oInput).removeClass('errorInput');
        var errorId = oInput + "error";
        if ($("#" + errorId).length != 0)
            $("#" + errorId).remove();
        //$("[name=" + oInput + ']').parent().removeChild($("#" + errorId));



    }
    return result;
}

function TBHelp(ObjId, MyPK) {
    var url = basePath + "/WF/CCForm/Pop/HelperOfTBEUIBS.htm?PKVal=" + MyPK + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
    var W = document.body.clientWidth / 2.5;
    OpenLayuiDialog(url, "词汇选择", W, 60, "auto", false);
}
function changeFastInt(ctrl, value) {
    $("#TB_" + ctrl).val(value);
    if ($('#eudlg').length > 0)
        $('#eudlg').window('close');
    if ($('#bootStrapdlg').length > 0)
        $('#bootStrapdlg').modal('hide');
}
function clearContent(ctrl) {
    $("#" + ctrl).val("");
}
