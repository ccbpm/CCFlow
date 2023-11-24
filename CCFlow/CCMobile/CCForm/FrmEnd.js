//@du-table add this file
var frmMapAttrs = [];
var numLimit = [];
function LoadFrmDataAndChangeEleStyle(workNode) {

    //加入隐藏控件.
    var mapAttrs = workNode.Sys_MapAttr;
	var node = frmData.WF_Node == undefined ? null : frmData.WF_Node[0];

    frmMapAttrs = mapAttrs;
    var html = "";
    for (var mapAttr in mapAttrs) {
        if (mapAttr.UIVisable == 0) {
            var defval = ConvertDefVal(workNode, mapAttr.DefVal, mapAttr.KeyOfEn);
            html += "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' value='" + defval + "' />";
            html = $(html);
            $('#CCFrom').append(html);
        }
    }

    //设置默认值
    for (var j = 0; j < mapAttrs.length; j++) {

        var mapAttr = mapAttrs[j];

        //添加 label
        //如果是整行的需要添加  style='clear:both'.
        var defValue = ConvertDefVal(frmData, mapAttr.DefVal, mapAttr.KeyOfEn);
        //if (mapAttr.DefValType == 0 && mapAttr.DefVal == "10002" && (defValue == "10002" || defValue == "10002.0000"))
        //    defValue = "";

        if ($('#TB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1") {
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == false)
                    $("#SW_" + mapAttr.KeyOfEn).addClass("mui-active");
            } else {
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == true)
                    $("#SW_" + mapAttr.KeyOfEn).removeClass("mui-active");
            }
            $('#TB_' + mapAttr.KeyOfEn).val(defValue);

            //大文本和富文本都放到div中
            if (mapAttr.IsRichText == 0) {
                $('#TB_' + mapAttr.KeyOfEn).text(defValue.replace(/"white-space: nowrap;"/g, ""));//只读大文本放到div里
            } else {
                $('#TB_' + mapAttr.KeyOfEn).html(defValue.replace(/"white-space: nowrap;"/g, ""));//只读大文本放到div里
            }
            if (mapAttr.MyDataType == FormDataType.AppDate || mapAttr.MyDataType == FormDataType.AppDateTime)
                $('#LAB_' + mapAttr.KeyOfEn).html(defValue);

            if ((mapAttr.MyDataType == 2 || mapAttr.MyDataType == 3 || mapAttr.MyDataType == 5 || mapAttr.MyDataType == 8)
                && (mapAttr.UIIsEnable == "1" && pageData.IsReadonly != 1)) {
                var isEnableNumEnterLimit = GetPara(mapAttr.AtPara, "IsEnableNumEnterLimit");
                if (isEnableNumEnterLimit != null && isEnableNumEnterLimit != undefined && isEnableNumEnterLimit == "1") {
                    var minVal = GetPara(mapAttr.AtPara, "MinNum");
                    var maxVal = GetPara(mapAttr.AtPara, "MaxNum");
                    //$('#TB_' + mapAttr.KeyOfEn).attr("data-min", minVal);
                    //$('#TB_' + mapAttr.KeyOfEn).attr("data-max", maxVal);
                    var target = $('#TB_' + mapAttr.KeyOfEn).parent();
                    if (target.length != 0 && target.hasClass("mui-numbox") == true) {
                        target.attr("data-numbox-min", minVal);
                        target.attr("data-numbox-max", maxVal);
                    }
                    numLimit.push(mapAttr.KeyOfEn);
                }
            }
        }

        if ($('#DDL_' + mapAttr.KeyOfEn).length == 1) {
            // 判断下拉框是否有对应option, 若没有则追加
            if ($("option[value='" + defValue + "']", '#DDL_' + mapAttr.KeyOfEn).length == 0) {
                var mainTable = frmData.MainTable[0];
                var selectText = mainTable[mapAttr.KeyOfEn + "Text"];
                //@浙商银行
                if (selectText == undefined)
                    selectText = mainTable[mapAttr.KeyOfEn + "T"];
                if (selectText == undefined)
                    selectText = "";
                $('#DDL_' + mapAttr.KeyOfEn).append("<option value='" + defValue + "'>" + selectText + "</option>");
            }
            //
            $('#DDL_' + mapAttr.KeyOfEn).val(defValue);
        }

        if ($('#CB_' + mapAttr.KeyOfEn).length == 1) {
            if (defValue == "1") {
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", true);
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == false)
                    $("#SW_" + mapAttr.KeyOfEn).addClass("mui-active");
            } else {
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", false);
                //判断是否存在mui-active这个类
                if ($("#SW_" + mapAttr.KeyOfEn).hasClass("mui-active") == true)
                    $("#SW_" + mapAttr.KeyOfEn).removeClass("mui-active");
            }
        }

        if (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0) {

            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
        }
		
		if (mapAttr.UIContralType == 14 && node.FWCSta == 0) { //隐藏签批组件
            $("#TB_" + mapAttr.KeyOfEn).hide();
        }
    }

    //设置为只读的字段.
    for (var i = 0; i < mapAttrs.length; i++) {
        var mapAttr = mapAttrs[i];
        //设置文本框只读.
        if (mapAttr.UIIsEnable == false || mapAttr.UIIsEnable == 0) {
            var tb = $('#TB_' + mapAttr.KeyOfEn);
            $('#TB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#CB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#RB_' + mapAttr.KeyOfEn).attr('disabled', true);
            $('#DDL_' + mapAttr.KeyOfEn).attr('disabled', true);
        }
    }
    if (pageData.IsReadonly == 1)
        OptionLinkOthers(workNode);


}

function OptionLinkOthers(frmData) {
    var mapAttrs = frmData.Sys_MapAttr;
    //解析设置表单字段联动显示与隐藏.
    for (var i = 0; i < mapAttrs.length; i++) {

        var mapAttr = mapAttrs[i];
        if (mapAttr.UIVisible == 0)
            continue;

        if (mapAttr.LGType != 1)
            continue;

        // if (mapAttr.UIIsEnable == 0)
        //   continue;


        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
            if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                if (mapAttr.UIContralType == 1) {
                    /*启用了显示与隐藏.*/
                    var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                    //初始化页面的值
                    var nowKey = ddl.val();


                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
                if (mapAttr.UIContralType == 3) {
                    /*启用了显示与隐藏.*/
                    var rb = $("#RB_" + mapAttr.KeyOfEn);
                    var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                    setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey);

                }
            }
        }

    }
}

//@浙商银行
//处理流程绑定表单字段权限的问题
function SetFieldsAuth(flowData, FK_Node, FK_Flow, FK_MapData) {
    var frmSlns = new Entities("BP.WF.Template.FrmFields", "FK_Node", FK_Node, "FK_MapData", FK_MapData);
    if (frmSlns == null || frmSlns.length == 0)
        return;
    for (var i = 0; i < frmSlns.length; i++) {
        var frmSln = frmSlns[i];
        var keyOfEn = frmSln.KeyOfEn;
        var myPk = FK_MapData + "_" + keyOfEn;
        var mapAttr = new Entity("BP.Sys.MapAttr", myPk);
        var defval = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn);
        if (defval == null || defval == "")
            defval = frmSln.DefVal;
        //获取值
        if (mapAttr.MyDataType == 4) {
            dealField(frmSln, $("#CB_" + keyOfEn), keyOfEn, mapAttr);

            if (val == "1")
                $('#CB_' + mapAttr.KeyOfEn).attr("checked", "true");
            if (defval != null && defval == "1")
                $("#CB_" + keyOfEn).checked = true;
            else
                $("#CB_" + keyOfEn).checked = false;

            continue;
        }

        //外部数据源
        if (mapAttr.MyDataType == 1 && mapAttr.LGType == 0 && mapAttr.UIContralType == 1) {
            dealField(frmSln, $("#DDL_" + keyOfEn), keyOfEn, mapAttr);
            if (defval != null && defval != "")
                $("#DDL_" + keyOfEn).val(defval);
            continue;
        }

        //外键
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 2) {
            dealField(frmSln, $("#DDL_" + keyOfEn), keyOfEn, mapAttr);
            if (defval != null && defval != "")
                $("#DDL_" + keyOfEn).val(defval);
            continue;
        }
        //枚举
        if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
            //单选按钮
            if (mapAttr.UIControlType == 3) {
                //是否可见
                frmSln.UIVisible == 0 ? $('input[name=RB_' + keyOfEn + ']').hide() : $('input[name=RB_' + keyOfEn + ']').show();
                //是否可编辑
                if (frmSln.UIIsEnable == 0) {
                    $('input[name=RB_' + keyOfEn + ']').attr("disabled", "disabled");
                } else {
                    $('input[name=RB_' + keyOfEn + ']').removeAttr("disabled");
                }

                if (defval != null && defval != "") {
                    var obj = document.getElementById("RB_" + keyOfEn + "_" + defval);
                    if (obj != null && obj != undefined)
                        obj.checked = true;
                }

            } else {
                dealField(frmSln, $("#DDL_" + keyOfEn), keyOfEn, mapAttr);
                if (defval != null && defval != "")
                    $("#DDL_" + keyOfEn).val(defval);
            }
            continue;
        }

        //文本值显示
        dealField(frmSln, $("#TB_" + keyOfEn), keyOfEn, mapAttr);
        if (defval != null && defval != "")
            $("#TB_" + keyOfEn).val(defval);
        //正则表达式
        if (frmSln.RegularExp != null && frmSln.RegularExp != "")
            $("#TB_" + keyOfEn).attr("onblur", frmSln.RegularExp);
    }

}

function dealField(frmSln, FieldID, keyOfEn, mapAttr) {
    //是否可见
    frmSln.UIVisible == 0 ? FieldID.hide() : FieldID.show();
    //是否可编辑
    if (frmSln.UIIsEnable == 0) {
        FieldID.attr("disabled", "disabled");
        FieldID.attr("placeholder", "");
    } else {
        FieldID.removeAttr("disabled");
    }
    //除去设置的不可见或可编辑的
    if (!(frmSln.UIVisible == 1 && frmSln.UIIsEnable == 1)) {
        //移除mtags
        var mtags = $("#" + keyOfEn + "_mtags");
        if (mtags.length > 0) {
            var parentTarget = FieldID.parent();
            parentTarget.children("a").remove();
            var divcontainer = mtags.find(".ccflow-input-span-container");
            var spans = divcontainer.children("span");
            $.each(spans, function (i, span) {
                span.innerHTML = span.innerText;
            });
            $("#TB_" + mapAttr.KeyOfEn).attr("placeholder", "");
        }

    }

    //是否 必填
    if (frmSln.IsNotNull == 1) {
        var showSpan = '<span style="color:red" class="mustInput" data-keyofen="' + keyOfEn + '">*</span>';
        FieldID.after(showSpan);
    }
}
//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
// 主表扩展(统计从表)
var detailExt = {};
var isLoadJs = {
    isLoadMapExt: false,
    isLoadPop: false,
    isLoadBranchesAndLeaf: false,
    isLoadBranches: false,
    isLoadTableSearch: false,
    isLoadTBFullCtrl: false,
    isLoadMultipleChoiceSearch: false,
    isLoadMultipleChoiceSmall: false,
    isLoadmtags: false
}

var urlPrefix = "./CCForm/";
if (location.href.indexOf("CCForm") > 0)
    urlPrefix = "./";
if (location.href.indexOf("CCBill") > 0)
    urlPrefix = "../CCForm/";
var frmEleDBs = null;
function AfterBindEn_DealMapExt(workNode) {
    var mapExts = workNode.Sys_MapExt;
    var mapAttrs = workNode.Sys_MapAttr;
    //根据字段的主键分组
    var reMapExts = GetMapExtsGroup(mapExts);
    var dateString = [];
    for (var i = 0; i < mapExts.length; i++) {
        var mapExt1 = mapExts[i];

        //一起转成entity.
        var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
        mapExt.MyPK = mapExt1.MyPK;
        if (mapExt.ExtType == 'PageLoadFull' || mapExt.ExtType == 'StartFlow') {
            continue;
        }
        var mapAttr1;
        if (mapExt.ExtType == "PageLoadFull")
            continue;
        if (mapExt.ExtType == "FullDataDtl")
            continue;

        for (var j = 0; j < mapAttrs.length; j++) {
            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                mapAttr1 = mapAttrs[j];
                break;
            }
        }
        if (mapAttr1 == null) {
            continue;
        }

        if (pageData.IsReadonly != 1 && mapAttr1.UIIsEnable == 1
            && (mapAttr1.MyDataType == 6 || mapAttr1.MyDataType == 7)) {
            var result = dateString.some(item => {
                if (item.MyPK == mapAttr1.MyPK) {
                    return true
                }
            })
            if (result == false)
                dateString.push({
                    MyPK: mapAttr1.MyPK,
                    data: mapAttr1
                });
            continue;
        }

        var mapAttr = new Entity("BP.Sys.MapAttr", mapAttr1);
        mapAttr.MyPK = mapAttr1.MyPK;

        if (isLoadJs.isLoadPop == false) {
            isLoadJs.isLoadPop = true;
            Skip.addJs(urlPrefix + "JS/Pop.js?t=" + Math.random());
            var oid = GetPKVal();
            frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
            frmEleDBs.Retrieve("FK_MapData", mapAttr.FK_MapData, "RefPKVal", oid);
        }

        if (isLoadJs.isLoadMapExt == false) {
            isLoadJs.isLoadMapExt = true;
            Skip.addJs(urlPrefix + "MapExt2016.js?t=" + Math.random());
        }
        if (isLoadJs.isLoadmtags == false) {
            isLoadJs.isLoadmtags = true;
            $('head').append('<link href="' + urlPrefix + 'JS/mselector.css" rel="Stylesheet"/>');
            Skip.addJs(urlPrefix + "JS/mselector.js?t=" + Math.random());
            Skip.addJs(urlPrefix + "JS/mtags.js?t=" + Math.random());

        }


        //处理Pop弹出框
        var PopModel = mapAttr.GetPara("PopModel");

        if (PopModel && PopModel == "None" && mapExt.ExtType.indexOf("Pop") >= 0)
            continue;

        if (PopModel != undefined && PopModel != "" && PopModel != "None") {
            if (mapExt.ExtType != PopModel)
                continue;
            if (mapAttr.UIIsEnable == 0 || pageData.IsReadonly == 1 || $("#TB_" + mapAttr.KeyOfEn).length == 0)
                continue;
            PopMapExt(mapAttr, mapExt, frmData, 0);
            continue;
        }


        //处理文本自动填充
        var TBModel = mapAttr.GetPara("TBFullCtrl");
        if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
            if (mapAttr.UIIsEnable == "0")
                continue;
            var tbAuto = $("#TB_" + mapExt.AttrOfOper);
            if (tbAuto == null)
                continue;
            if (isLoadJs.isLoadTBFullCtrl == false) {
                isLoadJs.isLoadTBFullCtrl = true;
                loadScript(urlPrefix + "TBFullCtrl.js?t=" + Math.random());
            }
            TBFullCtrl(mapExt, mapAttr, null, 0);
            continue

        }

        //下拉框填充其他控件
        var DDLFull = mapAttr.GetPara("IsFullData");
        if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
            /*  //枚举类型
              if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
                  var ddlOper = $('input:radio[name="RB_' + mapExt.AttrOfOper + '"]');
                  if (ddlOper.length == 0)
                      continue;
  
                  var enName = frmData.Sys_MapData[0].No;
  
                  ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");
  
                  //初始化填充数据
                  var val = $('input:radio[name="RB_' + mapExt.AttrOfOper + '"]:checked').val();
                  DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK);
  
  
                  continue;
              }
  */
            //外键类型
            var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
            if (ddlOper.length == 0)
                continue;

            var enName = frmData.Sys_MapData[0].No;

            ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\',0)");
            //初始化填充数据
            var val = ddlOper.val();
            if (val != "" && val != undefined)
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper, mapExt.MyPK, 0);
            continue;
        }


        switch (mapExt.ExtType) {
            case "MultipleChoiceSmall":
                if (isLoadJs.isLoadMultipleChoiceSmall == false) {
                    isLoadJs.isLoadMultipleChoiceSmall = true;
                    Skip.addJs(urlPrefix + "JS/MultipleChoiceSmall.js?t=" + Math.random());
                }

                MultipleChoiceSmall(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "MultipleChoiceSearch":
                if (isLoadJs.isLoadMultipleChoiceSearch == false) {
                    isLoadJs.isLoadMultipleChoiceSearch = true;
                    Skip.addJs(urlPrefix + "JS/MultipleChoiceSearch.js?t=" + Math.random());
                }
                MultipleChoiceSearch(mapExt); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
                break;
            case "DataFieldInputRole": //时间限制
                if (pageData.IsReadonly == 1)
                    break;
                if (bindDataString.indexOf(mapExt.AttrOfOper) != -1)
                    break;
                if (mapExt.DoWay == 1) {
                    var tag1 = mapExt.Tag1;
                    if (tag1 == 1) {
                        bindDataString += "@" + mapExt.AttrOfOper;

                        document.getElementById('LAB_' + mapExt.AttrOfOper).addEventListener('tap', function () {
                            var _self = this;
                            var optionsJson = this.getAttribute('data-options') || '{}';

                            var options = JSON.parse(optionsJson);
                            if (options.type == 0) {
                                options.type = "date";//yyyy-MM-dd
                            } else if (options.type == 1) {
                                options.type = "date-time";//yyyy-MM-dd HH:mm
                            } else if (options.type == 2) {
                                options.type = "datetime";//yyyy-MM-dd HH:mm:ss
                            } else if (options.type == 3) {
                                options.type = "month";//yyyy-MM
                            } else if (options.type == 4) {
                                options.type = "time-min";//HH:mm
                            } else if (options.type == 5) {
                                options.type = "time";//HH:mm:ss
                            } else if (options.type == 6) {
                                options.type = "month-day";//MM-dd
                            } else if (options.type == 7) {
                                options.type = "year";//yyyy
                            }
                            var id = this.getAttribute('id');
                            /*
                             * 首次显示时实例化组件
                             * 示例为了简洁，将 options 放在了按钮的 dom 上
                             * 也可以直接通过代码声明 optinos 用于实例化 DtPicker
                             */
                            _self.picker = new mui.DtPicker(options);
                            _self.picker.show(function (rs) {
                                /*
                                * rs.value 拼合后的 value
                                * rs.text 拼合后的 text
                                * rs.y 年，可以通过 rs.y.vaue 和 rs.y.text 获取值和文本
                                * rs.m 月，用法同年
                                * rs.d 日，用法同年
                                * rs.h 时，用法同年
                                * rs.i 分（minutes 的第二个字母），用法同年
                                */

                                /* 
                                * 返回 false 可以阻止选择框的关闭
                                * return false;
                                */
                                /*
                                * 释放组件资源，释放后将将不能再操作组件
                                * 通常情况下，不需要示放组件，new DtPicker(options) 后，可以一直使用。
                                * 当前示例，因为内容较多，如不进行资原释放，在某些设备上会较慢。
                                * 所以每次用完便立即调用 dispose 进行释放，下次用时再创建新实例。
                                */
                                $("#" + id).html(rs.text);
                                $("#TB_" + id.substr(4)).val(rs.text);
                                _self.picker.dispose();
                                _self.picker = null;
                                var datemapExt = new Entity("BP.Sys.MapExt");
                                datemapExt.SetPKVal("BindFunction_" + mapExt.FK_MapData + "_" + mapExt.AttrOfOper + "_change");
                                var count = datemapExt.RetrieveFromDBSources();
                                var doc = "";
                                if (count != 0)
                                    doc = datemapExt.Doc;
                                if (doc != "")
                                    DBAccess.RunFunctionReturnStr(doc);
                                var data = $("#" + id.replace("LAB_", "TB_")).data();
                                if (data && data.ReqDay != null && data.ReqDay != undefined)
                                    ReqDays(data.ReqDay);
                            });
                        }, false);
                    }

                }
                break;
            case "BindFunction": //控件绑定函数.
                if (pageData.IsReadonly == 1 || mapAttr.UIIsEnable == 0)
                    break;

                if ($('#TB_' + mapExt.AttrOfOper).length == 1) {
                    if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
                        if (bindDataString.indexOf(mapExt.AttrOfOper) != -1)
                            break;
                        bindDataString += "@" + mapExt.AttrOfOper;

                        var doc = mapExt.Doc;



                        document.getElementById('LAB_' + mapExt.AttrOfOper).addEventListener('tap', function () {
                            var _self = this;
                            var optionsJson = this.getAttribute('data-options') || '{}';

                            var year = new Date().getFullYear();
                            var month = new Date().getMonth() + 1;
                            var day = new Date().getDate();

                            var options = JSON.parse(optionsJson);
                            if (options.type == 0) {
                                options.type = "date";//yyyy-MM-dd
                            } else if (options.type == 1) {
                                options.type = "date-time";//yyyy-MM-dd HH:mm
                            } else if (options.type == 2) {
                                options.type = "datetime";//yyyy-MM-dd HH:mm:ss
                            } else if (options.type == 3) {
                                options.type = "month";//yyyy-MM
                            } else if (options.type == 4) {
                                options.type = "time-min";//HH:mm
                            } else if (options.type == 5) {
                                options.type = "time";//HH:mm:ss
                            } else if (options.type == 6) {
                                options.type = "month-day";//MM-dd
                            } else if (options.type == 7) {
                                options.type = "year";//MM-dd
                            }

                            //获取有无绑定时间限制
                            var datemapExt = new Entity("BP.Sys.MapExt");
                            datemapExt.SetPKVal("DataFieldInputRole_" + mapExt.FK_MapData + "_" + mapExt.AttrOfOper);
                            var count = datemapExt.RetrieveFromDBSources();
                            if (count != 0) {
                                options.beginYear = year;
                                options.beginMonth = month;
                                options.beginDay = day;
                            }
                            var id = this.getAttribute('id');
                            /*
                            * 首次显示时实例化组件
                            * 示例为了简洁，将 options 放在了按钮的 dom 上
                            * 也可以直接通过代码声明 optinos 用于实例化 DtPicker
                            */
                            _self.picker = new mui.DtPicker(options);
                            _self.picker.show(function (rs) {
                                /*
                                * rs.value 拼合后的 value
                                * rs.text 拼合后的 text
                                * rs.y 年，可以通过 rs.y.vaue 和 rs.y.text 获取值和文本
                                * rs.m 月，用法同年
                                * rs.d 日，用法同年
                                * rs.h 时，用法同年
                                * rs.i 分（minutes 的第二个字母），用法同年
                                */

                                /* 
                                * 返回 false 可以阻止选择框的关闭
                                * return false;
                                */
                                /*
                                * 释放组件资源，释放后将将不能再操作组件
                                * 通常情况下，不需要示放组件，new DtPicker(options) 后，可以一直使用。
                                * 当前示例，因为内容较多，如不进行资原释放，在某些设备上会较慢。
                                * 所以每次用完便立即调用 dispose 进行释放，下次用时再创建新实例。
                                */
                                $("#" + id).html(rs.text);
                                $("#TB_" + id.substr(4)).val(rs.text);
                                _self.picker.dispose();
                                _self.picker = null;

                                var datemapExt = new Entity("BP.Sys.MapExt");
                                var pkval = "BindFunction_" + mapExt.FK_MapData + "_" + id.substr(4) + "_change";
                                datemapExt.SetPKVal(pkval);
                                var count = datemapExt.RetrieveFromDBSources();
                                var doc = "";
                                if (count != 0)
                                    doc = datemapExt.Doc;
                                if (doc != "")
                                    DBAccess.RunFunctionReturnStr(doc);

                                var data = $("#" + id.replace("LAB_", "TB_")).data();
                                if (data && data.ReqDay != null && data.ReqDay != undefined)
                                    ReqDays(data.ReqDay);
                            });
                        }, false);
                    } else {
                        $('#TB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "TB_"));
                    }
                    break;
                }
                if ($('#DDL_' + mapExt.AttrOfOper).length == 1) {
                    $('#DDL_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "DDL_"));
                    break;
                }
                if ($('#CB_' + mapExt.AttrOfOper).length == 1) {
                    $('#CB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "CB_"));
                    break;
                }
                if ($('#RB_' + mapExt.AttrOfOper).length == 1) {
                    $('#RB_' + mapExt.AttrOfOper).bind(DynamicBind(mapExt, "RB_"));
                    break;
                }
                break;
            case "RegularExpression": //正则表达式  统一在保存和提交时检查

                var tb = $('#TB_' + mapExt.AttrOfOper);

                if (tb.attr('class') != undefined && tb.attr('class').indexOf('CheckRegInput') > 0) {
                    break;
                } else {
                    tb.addClass("CheckRegInput");
                    tb.data(mapExt)
                    tb.attr(mapExt.Tag, "CheckRegInput('" + tb.attr('name') + "','','" + mapExt.Tag1 + "')");

                }
                break;
            case "InputCheck": //输入检查
                break;
            case "TBFullCtrl": //自动填充
                if (pageData.IsReadonly == 1)
                    break;
                TBFullCtrl(mapExt, mapAttr);
                break;

            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive);
                if (ddlPerant == null || ddlChild == null)
                    continue;

                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

                var valClient = ConvertDefVal(frmData, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //初始化页面时方法加载

                DDLAnsc($("#DDL_" + mapExt.AttrOfOper).val(), "DDL_" + mapExt.AttrsOfActive, mapExt.MyPK, dbSrc, mapExt.DBType);

                //ddlChild.select(valClient);  未写
                break;
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
            case "AutoFull": //自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    continue;
                calculator(mapExt);
                break;
            case "ReqDays"://两个日期自动求天数
                if (mapExt.Tag1 == null || mapExt.Tag1 == "" ||
                    mapExt.Tag2 == null || mapExt.Tag2 == "")
                    break;
                if (pageData.IsReadonly == 1)
                    break;
                ReqDays(mapExt);
                $('#TB_' + mapExt.Tag1).data({ "ReqDay": mapExt })
                $('#TB_' + mapExt.Tag2).data({ "ReqDay": mapExt });
                break;
            case "RMBDaXie": //RMB转换成大写
                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    continue;
                //动态加载转大写的js
                if (location.href.indexOf("CCForm") > 0) {

                    Skip.addJs("../../DataUser/JSLibData/CovertMoneyToDaXie.js");
                } else if (location.href.indexOf("CCBill") > 0) {

                    Skip.addJs("../../DataUser/JSLibData/CovertMoneyToDaXie.js");
                } else {

                    Skip.addJs("../DataUser/JSLibData/CovertMoneyToDaXie.js");
                }
                var tbDoc = $('#TB_' + mapExt.AttrOfOper);
                var tb = $('#TB_' + mapExt.Doc);
                tbDoc.bind("change", function () {
                    var expVal = $("#" + this.id).val();//获取要转换的值
                    tb.val(Rmb2DaXie(expVal));//给大写的文本框赋值
                });
                var expVal = tbDoc.val();//获取要转换的值
                tb.val(Rmb2DaXie(expVal));//给大写的文本框赋值
                break;
            case "AutoFullDtlField": //主表扩展(统计从表)
                var docs = mapExt.Doc.split("\.");
                if (docs.length == 3) {
                    var ext = {
                        "DtlNo": docs[0],
                        "FK_MapData": mapExt.FK_MapData,
                        "AttrOfOper": mapExt.AttrOfOper,
                        "Doc": mapExt.Doc,
                        "DtlColumn": docs[1],
                        "exp": docs[2]
                    };
                    if (!$.isArray(detailExt[ext.DtlNo])) {
                        detailExt[ext.DtlNo] = [];
                    }
                    detailExt[ext.DtlNo].push(ext);
                    //var iframeDtl = $("#F" + ext.DtlNo);
                    //iframeDtl.load(function () {
                    //    $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[ext.DtlNo]));
                    //    if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                    //        this.contentWindow.parentStatistics(detailExt[ext.DtlNo]);
                    //    }
                    //});
                    $(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
                }
                break;
            case "DDLFullCtrl": // 自动填充其他的控件..

                var ddlOper = $("#DDL_" + mapExt.AttrOfOper);
                if (ddlOper.length == 0)
                    continue;

                var enName = frmData.Sys_MapData[0].No;
                var dbSrc = mapExt.Doc;

                //SQL 数据源获取
                if (mapExt.DBType == 0)
                    dbSrc = "";

                ddlOper.attr("onchange", "Change('" + enName + "');DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "\', \'" + mapExt.MyPK + "\')");


                if (mapExt.Tag != null && mapExt.Tag != "") {
                    /* 下拉框填充范围. */
                    var strs = mapExt.Tag.split('$');
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

                        var sql = myCtl[1].replace(/~/g, "'");
                        sql = sql.replace("@Key", ddlOper.val());
                        var operations = '';

                        ddlC1.children().remove();
                        ddlC1.html(operations);
                        //ddlC1.SetSelectItem(valC1);
                    }
                }
                break;
        }
    }

    //判断时间字段是否存在扩展属性
    if (dateString.length > 0) {
        $.each(dateString, function (idx, item) {
            var mapAttr = item.data;
            var mypk = item.MyPK;
            SetDateExt(reMapExts[mypk], mapAttr);
        });
    }



    //数值类型
    //if (numLimit.length != 0) {
    //    for (var i = 0; i < numLimit.length; i++) {
    //        var keyOfEn = numLimit[i];
    //        if (reMapExts[keyOfEn] == undefined) {
    //            $("#TB_" + keyOfEn).bind("change", function () {
    //                NumEnterLimit($("#" +this.id));
    //            })
    //        }
    //    }
    //}
}

function SetDateExt(mapExts, mapAttr) {

    bindDataString += "@" + mapAttr.KeyOfEn;
    var data = $("#TB_" + mapAttr.KeyOfEn).data();
    if (data != null && data.ReqDay != null)
        mapExts.push(data.ReqDay);
    var isFullData = GetPara(mapAttr.AtPara, "IsFullData");
    isFullData = isFullData == undefined || isFullData == "" || isFullData != "1" ? false : true;
    if (isFullData == true) {
        var mapExt = $.grep(mapExts, function (item) {
            return item.ExtType == "FullData";
        });
        if (mapExt.length != 0)
            DDLFullCtrl($("#TB_" + mapAttr.KeyOfEn).val(), "TB_" + mapAttr.KeyOfEn, mapExt[0].MyPK, 0);
    } else {
        //需要在MapExts中移除
        mapExts = $.grep(mapExts, function (item) {
            return item.ExtType != "FullData";
        });
    }

    $("#TB_" + mapAttr.KeyOfEn).data(mapExts);

    document.getElementById('LAB_' + mapAttr.KeyOfEn).addEventListener('tap', function () {
        var _self = this;
        var id = this.getAttribute('id');
        var keyOfEn = id.substr(4);
        var optionsJson = this.getAttribute('data-options') || '{}';
        var options = JSON.parse(optionsJson);
        if (options.type == 0) {
            options.type = "date";//yyyy-MM-dd
        } else if (options.type == 1) {
            options.type = "date-time";//yyyy-MM-dd HH:mm
        } else if (options.type == 2) {
            options.type = "datetime";//yyyy-MM-dd HH:mm:ss
        } else if (options.type == 3) {
            options.type = "month";//yyyy-MM
        } else if (options.type == 4) {
            options.type = "time-min";//HH:mm
        } else if (options.type == 5) {
            options.type = "time";//HH:mm:ss
        } else if (options.type == 6) {
            options.type = "month-day";//MM-dd
        } else if (options.type == 7) {
            options.type = "year";//MM-dd
        }

        var mapExts = $("#TB_" + keyOfEn).data();
        var funcDoc = "";
        var roleExt = null;
        var FullExt = null;
        var ReqDay = null;
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
            if (mapExt.ExtType == "ReqDay") {
                ReqDay = mapExt;
            }
        });


        //不能选择历史时间
        if (roleExt != null && roleExt.DoWay == 1 && roleExt.Tag1 == "1") {
            options.beginDate = new Date();
        }
        if (roleExt != null && roleExt.DoWay == 1 && roleExt.Tag2 == "1") {
            var date = $("#TB_" + roleExt.Tag4).val();
            if (date != undefined && date != "")
                switch (roleExt.Tag3) {
                    case "dayu":
                    case "dayudengyu":
                        options.beginDate = new Date(date);
                        break;
                    case "xiaoyu":
                    case "xiaoyudengyu":
                        options.endDate = new Date(date);
                        break;
                    case "budengyu":

                        break;
                }

        }

        /*
        * 首次显示时实例化组件
        * 示例为了简洁，将 options 放在了按钮的 dom 上
        * 也可以直接通过代码声明 optinos 用于实例化 DtPicker
        */
        _self.picker = new mui.DtPicker(options);
        _self.picker.show(function (rs) {
            /*
            * rs.value 拼合后的 value
            * rs.text 拼合后的 text
            * rs.y 年，可以通过 rs.y.vaue 和 rs.y.text 获取值和文本
            * rs.m 月，用法同年
            * rs.d 日，用法同年
            * rs.h 时，用法同年
            * rs.i 分（minutes 的第二个字母），用法同年
            */

            /* 
            * 返回 false 可以阻止选择框的关闭
            * return false;
            */
            /*
            * 释放组件资源，释放后将将不能再操作组件
            * 通常情况下，不需要示放组件，new DtPicker(options) 后，可以一直使用。
            * 当前示例，因为内容较多，如不进行资原释放，在某些设备上会较慢。
            * 所以每次用完便立即调用 dispose 进行释放，下次用时再创建新实例。
            */
            $("#" + id).html(rs.text);
            $("#TB_" + keyOfEn).val(rs.text);
            _self.picker.dispose();
            _self.picker = null;

            //计算差值
            if (ReqDay != null)
                ReqDays(ReqDay);

            //执行函数
            if (funcDoc != "")
                DBAccess.RunFunctionReturnStr(funcDoc);

            //执行填充
            if (FullExt != null) {
                DDLFullCtrl(rs.text, "TB_" + FullExt.AttrOfOper, FullExt.MyPK, 0);
            }

        });
    }, false);


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
    var demoRDT;
    demoRDT = StarRDT.split("-");
    StarRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);  //转换为yyyy-MM-dd格式
    demoRDT = EndRDT.split("-");
    EndRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);
    res = parseInt((EndRDT - StarRDT) / 1000 / 60 / 60 / 24); //把相差的毫秒数转换为天数
    //判断结束日期是否早于开始日期
    if (parseInt(EndRDT / 1000 / 60 / 60 / 24) < parseInt(StarRDT / 1000 / 60 / 60 / 24)) {
        layer.alert("结束日期不能早于开始日期");
        res = "";
    }
    else {
        //当包含节假日的时候
        if (RDTRadio == 0) {
            var holidayEn = new Entity("BP.Sys.GloVar");
            holidayEn.No = "Holiday";
            if (holidayEn.RetrieveFromDBSources() == 1) {
                var holidays = holidayEn.Val.split(",");
                res = res - (holidays.length - 1);
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
        $("#LAB_" + endField).html("");
        res = 0;
    }
    $('#TB_' + ResRDT).val(res);
}


/**Pop弹出框的处理**/
function PopMapExt(mapAttr, mapExt, frmData) {


    switch (mapAttr.GetPara("PopModel")) {

        case "PopBranchesAndLeaf": //树干叶子模式.
            if (isLoadJs.isLoadBranchesAndLeaf == false) {
                isLoadJs.isLoadBranchesAndLeaf = true;
                loadScript(urlPrefix + "BranchesAndLeaf.js?t=" + Math.random());

            }
            PopBranchesAndLeaf(mapExt, mapAttr); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopBranches": //树干简单模式. 
            if (isLoadJs.isLoadBranches == false) {
                isLoadJs.isLoadBranches = false;
                loadScript(urlPrefix + "Branches.js?t=" + Math.random());
            }

            // PopBranches(mapExt, mapAttr); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopGroupList": //分组模式.

            PopGroupList(mapExt, mapAttr); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopSelfUrl": //自定义url.
            SelfUrl(mapExt, mapAttr); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
            break;
        case "PopTableSearch": //表格查询.
            if (isLoadJs.isLoadTableSearch == false) {
                isLoadJs.isLoadTableSearch = true;
                loadScript(urlPrefix + "TableSearch.js?t=" + Math.random());
            }
            PopTableSearch(mapExt, mapAttr); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        default: break;
    }
}


//为从表添加
function DynamicBind(mapExt, ctrlType, oid) {
    var tag = mapExt.Tag;
    if (tag == 'click') {
        tag = 'tap';
    }

    var id = '#' + ctrlType + mapExt.AttrOfOper + (oid == undefined ? "" : "_" + oid);

    $(id).on(tag, function () {
        //if (tag == 'change') {
        //    NumEnterLimit($("#"+this.id));
        //}
        mapExt.Doc = mapExt.Doc.replace("this", this.id);
        DBAccess.RunFunctionReturnStr(mapExt.Doc);
    });
}

/**
* 表单计算(包括普通表单以及从表弹出页表单)
*/
function calculator(o, lid) {
    if (!testExpression(o.Doc)) {
        console.log("MyPk: " + o.MyPK + ", 表达式: '" + o.Doc + "'格式错误");
        return false;
    }
    var targets = [];
    var index = -1;
    for (var i = 0; i < o.Doc.length; i++) {	// 对于复杂表达式需要重点测试
        var c = o.Doc.charAt(i);
        if (c == "(") {
            index++;
        } else if (c == ")") {
            targets.push(o.Doc.substring(index + 1, i));
            i++;
            index = i;
        } else if (/[\+\-|*\/]/.test(c)) {
            targets.push(o.Doc.substring(index + 1, i));
            index = i;
        }
    }
    if (index + 1 < o.Doc.length) {
        targets.push(o.Doc.substring(index + 1, o.Doc.length));
    }

    var expression = {
        "judgement": [],
        "execute_judgement": [],
        "calculate": o.Doc
    };
    $.each(targets, function (i, o) {
        if (o.indexOf("@") == -1)
            return true;
        var target = o.replace("@", "");
        if (lid != null && lid != undefined && lid != "")
            target = target + "_" + lid;
        var element = "$(':input[name=TB_" + target + "]')";
        expression.judgement.push(element + ".length == 0");
        expression.execute_judgement.push("!isNaN(parseFloat(" + element + ".val()))");
        expression.calculate = expression.calculate.replace(o, "parseFloat(" + element + ".val())");
    });
    (function (targets, expression, resultTarget, pk, expDefined) {
        if (lid != null && lid != undefined && lid != "")
            resultTarget = resultTarget + "_" + lid;
        $.each(targets, function (i, o) {
            if (o.indexOf("@") == -1)
                return true;
            var target = o.replace("@", "");
            if (lid != null && lid != undefined && lid != "")
                target = target + "_" + lid;
            $(":input[name=TB_" + target + "]").bind("change", function () {
                var evalExpression = " var result = ''; ";
                if (expression.judgement.length > 0) {
                    evalExpression += " if ( " + expression.judgement.join(" || ") + " ) { ";
                    evalExpression += " 	console.log(\"MyPk: " + pk + ", 表达式: '" + expDefined + "' " + "中有对象在当前页面不存在\");"
                    evalExpression += " } ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += " else if ( " + expression.execute_judgement.join(" && ") + " ) { ";
                }
                if (expression.calculate.length > 0) {
                    evalExpression += " 	result = " + expression.calculate + "; ";
                }
                if (expression.execute_judgement.length > 0) {
                    evalExpression += " } ";
                }
                var resultVal = cceval(evalExpression);
                //解决自动计算精度丢失
                resultVal = Strip(resultVal);
                $(":input[name=TB_" + resultTarget + "]").val(typeof resultVal == "undefined" ? "" : resultVal);
            });
            if (i == 0) {
                $(":input[name=TB_" + target + "]").trigger("change");
            }
        });
    })(targets, expression, o.AttrOfOper, o.MyPK, o.Doc);
    if (lid != null && lid != undefined && lid != "")
        $(":input[name=TB_" + o.AttrOfOper + "_" + lid + "]").attr("disabled", true);
    else
        $(":input[name=TB_" + o.AttrOfOper + "]").attr("disabled", true);
}

function Strip(num, precision = 12) {
    return +parseFloat(num.toPrecision(precision));
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
//获取本窗体值.
function getCtrlVal(ctrlID) {
    var ctrl = document.getElementById('TB_' + ctrlID);
    if (ctrl) {
        return ctrl.value
    }

    ctrl = document.getElementById('DDL_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }

    ctrl = document.getElementById('CB_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }
    return null;
}

//获取父窗体值.
function getParentCtrlVal(ctrlID) {
    if (!window.parent)
        return null;
    var ctrl = window.parent.document.getElementById('TB_' + ctrlID);
    if (ctrl) {
        return ctrl.value
    }

    ctrl = window.parent.document.getElementById('DDL_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }

    ctrl = window.parent.document.getElementById('CB_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }
    return null;
}

//获取父窗的父窗体值.
function getPParentCtrlVal(ctrlID) {
    if (!window.parent.parent)
        return null;
    var ctrl = window.parent.parent.document.getElementById('TB_' + ctrlID);
    if (ctrl) {
        return ctrl.value
    }

    ctrl = window.parent.parent.document.getElementById('DDL_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }

    ctrl = window.parent.parent.document.getElementById('CB_' + ctrlID);
    if (ctrl) {
        return ctrl.value;
    }
    return null;
}

//记录改变字段样式 不可编辑，不可见
var mapAttrs = {};
function changeEnable(obj, FK_MapData, KeyOfEn, AtPara) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll(KeyOfEn);
        setEnable(FK_MapData, KeyOfEn, selecedval);
    }
}
function clickEnable(obj, FK_MapData, KeyOfEn, AtPara) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selectVal = $(obj).val();
        cleanAll(KeyOfEn);
        setEnable(FK_MapData, KeyOfEn, selectVal);
    }
}

//清空所有的设置
function cleanAll(KeyOfEn) {
    if (mapAttrs.length == 0)
        return;

    //获取他的值
    if (mapAttrs[KeyOfEn] != undefined && mapAttrs[KeyOfEn].length > 0) {
        var FKMapAttrs = mapAttrs[KeyOfEn][0];
        for (var i = 0; i < FKMapAttrs.length; i++) {
            SetCtrlShow(FKMapAttrs[i]);
            SetCtrlEnable(FKMapAttrs[i]);
            CleanCtrlVal(FKMapAttrs[i]);
            SetCtrlUnMustInput(FKMapAttrs[i]);
        }
    }
}
//启用了显示与隐藏.
function setEnable(FK_MapData, KeyOfEn, selectVal) {
    var NDMapAttrs = [];
    var pkval = FK_MapData + "_" + KeyOfEn + "_" + selectVal;
    var frmRB = new Entity("BP.Sys.FrmRB");
    frmRB.SetPKVal(pkval);
    if (frmRB.RetrieveFromDBSources() == 0)
        return;

    //解决字段隐藏显示.
    var cfgs = frmRB.FieldsCfg;

    //解决为其他字段设置值.
    var setVal = frmRB.SetVal;
    if (setVal) {
        var strs = setVal.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            var kv = str.split('=');

            var key = kv[0];
            var value = kv[1];
            SetCtrlVal(key, value);
            NDMapAttrs.push(key);

        }
    }
    //@Title=3@OID=2@RDT=1@FID=3@CDT=2@Rec=1@Emps=3@FK_Dept=2@FK_NY=3
    if (cfgs) {

        var strs = cfgs.split('@');

        for (var i = 0; i < strs.length; i++) {

            var str = strs[i];
            var kv = str.split('=');

            var key = kv[0];
            var sta = kv[1];

            if (sta == 0)
                continue; //什么都不设置.


            if (sta == 1) {  //要设置为可编辑.
                SetCtrlShow(key);
                SetCtrlEnable(key);
            }

            if (sta == 2) { //要设置为不可编辑.
                SetCtrlShow(key);
                SetCtrlUnEnable(key);
                NDMapAttrs.push(key);
            }

            if (sta == 3) { //不可见.
                SetCtrlHidden(key);
                NDMapAttrs.push(key);
            }
            if (sta == 4) { //不可见.
                SetCtrlMustInput(key);
                NDMapAttrs.push(key);
            }

        }


    }

    if (!$.isArray(mapAttrs[KeyOfEn])) {
        mapAttrs[KeyOfEn] = [];
    }
    mapAttrs[KeyOfEn] = [];

    if (NDMapAttrs.length > 0) {
        mapAttrs[KeyOfEn].push(NDMapAttrs);
    }


}

//设置是否可以用?
function SetCtrlEnable(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
    }

    ctr = document.getElementsByName('RB_' + key);
    if (ctrl != null) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", key);
        for (var i = 0; i < ses.length; i++)
            $("#RB_" + key + "_" + ses[i].IntKey).removeAttr("disabled");
    }
}
function SetCtrlUnEnable(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "true");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.attr("disabled", "disabled");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {

        ctrl.attr("disabled", "disabled");
    }

    ctrl = $("#RB_" + key);
    if (ctrl != null) {
        $('input[name=RB_' + key + ']').attr("disabled", "disabled");
        //ctrl.attr("disabled", "disabled");
    }
}

//设置是否可用且必填?
function SetCtrlMustInput(key) {

    SetCtrlEnable(key);
    $("label[for$='_" + key + "']").find("p").append('<span style="color:red;display: inline-block;" class="mustInput" data-keyofen="' + key + '">*</span>');
}
//设置可不必填
function SetCtrlUnMustInput(key) {
    $("label[for$='_" + key + "']").find("p .mustInput").remove();
}

//设置隐藏?
function SetCtrlHidden(key) {
    ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        if (ctrl.is(":hidden") && ctrl.parent('div')[0].id == "CCForm")
            return;
        ctrl.parent('div').hide();
        if ($("#" + key + "_mtags").length > 0)
            $("#" + key + "_mtags").hide();
    }
    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').hide();
    }


}
//设置显示?
function SetCtrlShow(key) {
    var ctrl = $("#Td_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('tr').show();

    }

    ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.parent('div').show();
        if ($("#" + key + "_mtags").length > 0)
            $("#" + key + "_mtags").show();
    }


}

//设置值?
function SetCtrlVal(key, value) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        // ctrl.attr("value",value);
        //$("#DDL_"+key+" option[value='"+value+"']").attr("selected", "selected");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        ctrl.attr('checked', true);
    }

    ctrl = $("#RB_" + key + "_" + value);
    if (ctrl.length > 0) {
        var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
        document.getElementById("RB_" + key + "_" + checkVal).checked = false;
        document.getElementById("RB_" + key + "_" + value).checked = true;
        // ctrl.attr('checked', 'checked');
    }
}

//清空值?
function CleanCtrlVal(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val('');
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        //ctrl.attr("value",'');
        ctrl.val('');
        // $("#DDL_"+key+" option:first").attr('selected','selected');
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr('checked', false);;
    }

    ctrl = $("#RB_" + key + "_" + 0);
    if (ctrl.length > 0) {
        ctrl.attr('checked', true);
    }
}

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
            || mapExt.ExtType == 'AutoFullDLLSearchCond')
            return true;

        mypk = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;
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
function Initcheckbox(frmData, mapAttr) {
    let operations = '';
    let data = frmData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = frmData[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            let enums = frmData.Sys_Enum;
            let mainTable = frmData.MainTable[0];
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });
            let val = mainTable[mapAttr.KeyOfEn] + ",";
            $.each(enums, function (i, obj) {
                //operations += '<div><input type="checkbox" class="checkBox-input" name="' + obj.EnumKey + '" value="' + obj.Lab + '" ' + (GZFLarr.indexOf(obj.IntKey + '') >= 0 ? 'checked' : '') + '/>' + obj.Lab + '</div>';
                operations += '<div class="mui-input-row mui-checkbox">';
                operations += '<label>' + obj.Lab + '</label>'
                operations += '<input name="CB_' + obj.EnumKey + '" id="CB_' + obj.EnumKey + '_' + obj.IntKey + '" value="' + obj.IntKey + '" type="checkbox" ' + (val.indexOf(obj.IntKey + ',') >= 0 ? 'checked ' : '') + (mapAttr.UIIsEnable == 0 ? 'disabled ' : '') + '/>'
                operations += '</div>'
            });
        }
        operations += '</div>'
        return operations;
    }
    return operations;
}

//获取表单数据
function getFormData(isCotainTextArea, isCotainUrlParam, formID, isDtl) {
    if (window.editor) {
        $("textarea[name='" + editor.srcElement.attr("name") + "']").val(editor.html());
    }

    var formss = $('#' + formID).serialize();
    if (formss == "")
        return {};

    var formArr = "\"" + formss.replace(/=/g, "\":\"");
    var stringObj = "{" + formArr.replace(/&/g, "\",\"") + "\"}";
    var formArrResult = JSON.parse(stringObj);
    haseExistStr = "";
    //获取CHECKBOX的值
    for (var key in formArrResult) {
        var attrName = key.replace("CB_", "");
        if ($("#SW_" + attrName).hasClass("mui-active") == true) {
            formArrResult["CB_" + attrName] = 1;
            continue;
        }

        if (key.indexOf("CB_") == 0) {
            //可能是复选框
            var ckboxs = $("input[name='" + key + "']");
            if (ckboxs.length == 1) {
                if ($('#' + key + ':checked').length == 1) {
                    formArrResult[key] = 1;
                } else {
                    formArrResult[key] = 0;
                }
            } else {
                var vals = [];
                $.each($("input[name='" + key + "']:checked"), function (i, item) {
                    vals.push($(item).val());
                });
                formArrResult[key] = vals.join(",");
            }

            continue;
        }


        if (key.indexOf('DDL_') == 0) {
            var item = $("#" + key).children('option:checked').text();
            var mystr = '';
            //如果是从表，需要获取后缀
            if (isDtl == true) {
                var before = key.substring(0, key.lastIndexOf("_"));
                var after = key.substring(key.lastIndexOf("_"));
                var keyT = before.replace("DDL_", "TB_") + 'T' + after;
                mystr = keyT + "=" + item;
                formArrResult[keyT] = item;
                //formArrResult.push(ele);
                haseExistStr += keyT + ",";
            } else {
                //mystr = key.replace("DDL_", "TB_") + 'T=' + item;
                var keyT = key.replace("DDL_", "TB_") + 'T'
                formArrResult[keyT] = item;
                //formArrResult.push(ele);
                haseExistStr += keyT + ",";
            }

        }


    }

    //复选框checkbox未选中时序列化时不包含的添加
    var checkBoxs = $('input[type=checkbox]');
    $.each(checkBoxs, function (i, checkBox) {
        //@浙商银行
        var name = $(checkBox).attr("name");
        if ($("input[name='" + name + "']:checked").length == 0) {
            formArrResult[name] = 0;
        }
    });

    //获取表单中禁用的表单元素的值
    var disabledEles = $('#' + formID + ' :disabled');
    $.each(disabledEles, function (i, disabledEle) {
        var name = $(disabledEle).attr('name');
        if (name == null || name == undefined || name == "")
            return true;
        switch (disabledEle.tagName.toUpperCase()) {
            case "INPUT":
                switch (disabledEle.type.toUpperCase()) {
                    case "CHECKBOX": //复选框
                        formArrResult[name] = encodeURIComponent($(disabledEle).is(':checked') ? 1 : 0);
                        break;
                    case "TEXT": //文本框
                    case "NUMBER":
                        if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                            formArrResult[name] = encodeURIComponent($(disabledEle).val());
                        }
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
                var tbID = name.replace("DDL_", "TB_") + 'T';
                if ($("#" + tbID).length == 1) {
                    if (haseExistStr.indexOf("," + tbID + ",") == -1) {
                        formArrResult[tbID] = $(disabledEle).children('option:checked').text();
                        haseExistStr += tbID + ",";
                    }
                }
                formArrResult[name] = $(disabledEle).children('option:checked').val();
                break;
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
            formArrResult[name + "T"] = data.text;
        }
    });

    return formArrResult;
}
//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput

    var lbs = $('.mustInput');
    $.each(lbs, function (i, obj) {
        var parentObj = $(obj).parent().parent();
        if (parentObj && parentObj.css('display') != 'none') {
            var keyOfEn = $(obj).attr("data-keyofen");
            if (keyOfEn != null) {
                var item = $("#TB_" + keyOfEn);
                if (item.length != 0) {
                    var val = item.val();
                    if ($("#" + keyOfEn + "_mtags").length != 0) {
                        var count = $("#" + keyOfEn + "_mtags").find(".ccflow-tag");
                        if (count == 0) {
                            checkBlankResult = false;
                            item.addClass('errorInput');
                        } else {
                            item.removeClass('errorInput');
                        }
                    } else {
                        if (item.val() == "") {
                            checkBlankResult = false;
                            item.addClass('errorInput');
                        } else {
                            item.removeClass('errorInput');
                        }
                    }

                    return true;
                }

                item = $("#DDL_" + keyOfEn);
                if (item.length != 0) {
                    if (item.val() == "" || item.val() == -1 || item.children('option:checked').text() == "*请选择") {
                        checkBlankResult = false;
                        item.addClass('errorInput');
                    } else {
                        item.removeClass('errorInput');
                    }
                    return true;
                }

            }

        }
    });

    return checkBlankResult;
}