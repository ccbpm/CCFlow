/**
 * 扩展字段属性的解析
 * @param {any} tr 行信息
 * @param {any} rowIndex 当前字段所在的行号
 */
var WorkId = GetQueryString("WorkId");
var isReadonly  = GetQueryString("IsReadonly");
var isFirstTBFull = true;
var isHaveLoadMapExt = false;
var isHaveEnableJs = false;
var mapExts = workNodeData.Sys_MapExt; // 扩展信息
//根据字段的主键分组
var mapKeyExts = GetMapExtsGroup(mapExts);
//debugger
//获取扩展属性的MapAttr
var mapAttrs = workNodeData.Sys_MapAttr;
var mapExtAttrs = $.grep(mapAttrs, function (mapAttr) {
    var isHave = false;
    for (var key in mapKeyExts) {
        if (mapAttr.MyPK == key) {
            isHave = true;
            break;
        }
    }
    return mapAttr;
})
function AfterBindEn_DealMapExt(tr, rowIndex) {
    var workNode = workNodeData;
      
    //表示从表还没有数据
    var OID = 0;
    if ($(tr).data().data == undefined)
        return;
    else
        OID = $(tr).data().data.OID;
    if (OID == 0)//数据还未保存
        OID = WorkId + "_" + rowIndex;
	 layui.config({
		base: '../Scripts/layui/ext/'
	});
    $.each(mapExtAttrs, function (idx, mapAttr) {
        //字段不可见
        if (mapAttr.UIVisible == 0)
            return true;

        //证件类扩展
        if (mapAttr.UIContralType == 13)
            return true;

        if (isHaveLoadMapExt == false) {
            Skip.addJs("./MapExt2021.js");
            isHaveLoadMapExt = true;
        }

        //获取当前字段的ID
        var tbAuto = $(tr).find("[name=TB_" + mapAttr.KeyOfEn + ']');
        var targetID = tbAuto.attr('id').replace("TB_", "");

        //如果是枚举、下拉框、复选框判断是否有选项联动其他控件
        if (mapAttr.LGType == 1 && (mapAttr.UIContralType == 1 || mapAttr.UIContralType == 3)
            || (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1)
            || (mapAttr.LGType == "2" && mapAttr.MyDataType == "1")
            || mapAttr.MyDataType == "4") {

            
            if (mapKeyExts[mapAttr.MyPK] == undefined || mapKeyExts[mapAttr.MyPK].length == 0)
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

            SetRadioSelectMapExt(mapKeyExts[mapAttr.MyPK], mapAttr, selectVal, model, false);

            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                return;

            if (model == "radio") {
                layui.form.on('radio(' + mapAttr.KeyOfEn + ')', function (element) {
                    var data = $(this).data();
                    SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value,  "radio", true);
                });
            } else if (model == "select") {
                layui.form.on('select(' + mapAttr.KeyOfEn + ')', function (element) {
                    SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value, "select", true);
                });
            } else if (model == "checkbox") {
                var obj = $("#CB_" + mapAttr.KeyOfEn);
                var sky = obj.attr("lay-skin");
                sky = sky == null || sky == undefined ? "" : sky;
                if (sky == "switch")
                    layui.form.on('switch(' + mapAttr.KeyOfEn + ')', function (element) {
                        SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value,"select", true);
                    });
                else
                    layui.form.on('checkbox(' + mapAttr.KeyOfEn + ')', function (element) {
                        SetRadioSelectMapExt(data.mapExts, data.mapAttr, element.value,"select", true);
                    });

            }
            var data = {
                mapAttr: mapAttr,
                mapExts: mapKeyExts[mapAttr.MyPK]
              
            };

            $("input[name=RB_" + mapAttr.KeyOfEn + "]").data(data);
            $("#CB_" + mapAttr.KeyOfEn).data(data);
            $("#DDL_" + mapAttr.KeyOfEn).data(data);
            return true;
        }

        //没有扩展属性
        if (mapKeyExts[mapAttr.MyPK] == undefined || mapKeyExts[mapAttr.MyPK].length == 0)
            return true;

        //如果是日期型或者时间型
        if (mapAttr.MyDataType == 6 || mapAttr.MyDataType == 7) {
            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                return true;
            SetDateExt(mapKeyExts[mapAttr.MyPK], mapAttr, targetID);
            return true;
        }

        //如果是整数，浮点型，金额类型的扩展属性
        if (mapAttr.MyDataType == 2 || mapAttr.MyDataType == 3 || mapAttr.MyDataType == 5 || mapAttr.MyDataType == 8) {
            if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                return true;

            SetNumberMapExt(mapKeyExts[mapAttr.MyPK], mapAttr);
            return true;
        }

        //文本字段扩展属性
        var tbAuto = $(tr).find("[name=TB_" + mapAttr.KeyOfEn + ']');
        var targetID = tbAuto.attr('id').replace("TB_","");
        $.each(mapKeyExts[mapAttr.MyPK], function (k, mapExt1) {
            var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
            mapExt.MyPK = mapExt1.MyPK;
            //处理Pop弹出框的问题
            var PopModel = GetPara(mapAttr.AtPara, "PopModel");

            if (PopModel != undefined && PopModel != "" && PopModel != "None") {
                if (mapExt.ExtType != PopModel)
                    return true;
                if (mapAttr.UIIsEnable == 0 || isReadonly == true || $("#TB_" + targetID).length == 0)
                    return true;
                PopMapExt(PopModel, mapAttr, mapExt, workNode, mapKeyExts,OID);
                return true;
            }
            //处理文本自动填充
            var TBModel = GetPara(mapAttr.AtPara, "TBFullCtrl");
            if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
                if (mapAttr.UIIsEnable == 0 || isReadonly == true || $("#TB_" + targetID).length == 0)
                    return true;
                if (TBModel == "Simple") {
                    if (isFirstTBFull == true) {
                        layui.config({
                            base: '../Scripts/layui/ext/'
                        });
                        isFirstTBFull = false;
                    }
                    //判断时简洁模式还是表格模式
                    layui.use('autocomplete', function () {
                        var autocomplete = layui.autocomplete;
                        autocomplete.render({
                            elem: "#TB_" + targetID,
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
                    var obj = $("#TB_" + targetID);
                    obj.attr("onkeyup", "showDataGrid(\'TB_" + targetID + "\',this.value, \'" + mapExt.MyPK + "\');");
                    //showDataGrid("TB_" + mapAttr.KeyOfEn, $("#TB_" + mapAttr.KeyOfEn).val(), mapExt);
                }


            }

            switch (mapExt.ExtType) {
                case "MultipleChoiceSmall"://小范围多选
                case "SingleChoiceSmall"://小范围单选
                    if (mapExt.DoWay == 0)//不设置
                        break;
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true) {
                        //只显示
                        $("#TB_" + targetID).hide();
                        var val = frmData.MainTable[0][mapAttr.KeyOfEn + "T"];
                        $("#TB_" + targetID).after("<div style='border:1px solid #eee;line-height:36px;width:100%;height:36px'>" + val + "</div>");
                        break;
                    }
                  
                    var data = GetDataTableOfTBChoice(mapExt, frmData, $("#TB_" + targetID).val());
                    data = data == null ? [] : data;
                    $("#TB_" + targetID).hide();
                    $("#TB_" + targetID).after("<div id='mapExt_" + targetID + "'style='width:99%'></div>")
                    layui.use('xmSelect', function () {
                        var xmSelect = layui.xmSelect;
                        xmSelect.render({
                            el: "#mapExt_" + targetID,
                            pkval: OID,
                            id:mapExt.AttrOfOper,
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

                case "MultipleChoiceSearch"://搜索多选
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                        break;

                  //  debugger
                    var isLoad = true;
                    $("#TB_" + targetID).hide();
                    $("#TB_" + targetID).after("<div id='mapExt_" + targetID + "' style='width:99%'></div>");
                    var data = {
                        pkval: OID,
                        keyOfEn: mapExt.AttrOfOper,
                    }
                    $("#mapExt_" + targetID).data(data);
                    //单选还是多选
                    var selectType = mapExt.GetPara("SelectType");
                    selectType = selectType == null || selectType == undefined || selectType == "" ? 1 : selectType;
                    layui.use('xmSelect', function () {
                        var xmSelect = layui.xmSelect;
                        xmSelect.render({
                            el: "#mapExt_" + targetID,
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
                                var dataInfo = $(data.el).data();
                                SaveFrmEleDBs(arr, dataInfo.keyOfEn, mapExt, dataInfo.pkval);
                            },
                            remoteMethod: function (val, cb, show) {
                                //这里如果val为空, 则不触发搜索
                                /*if (!val) {
                                    return cb([]);
                                }*/
                                var mapExt = new Entity("BP.Sys.MapExt", this.mapExt);
                                //选中的值
                                var selects = new Entities("BP.Sys.FrmEleDBs");
                                selects.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", OID);
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
                case "MultipleInputSearch"://高级快速录入

                    break;
                case "BindFunction"://绑定函数(现在只处理文本，其他的单独处理了)
                    if (mapAttr.UIIsEnable == 0 || isReadonly == true)
                        break;
                    if ($('#TB_' + targetID).length == 1) {
                        $('#TB_' + targetID).bind(DynamicBind(mapExt, "TB_"));
                        break;
                    }
                    break;
                case "FullData"://POP返回值的处理，放在了POP2021.js

                    break;
                case "RegularExpression":
                    $('#TB_' + targetID).data(mapExt);
                    $('#TB_' + targetID).on(mapExt.Tag.substring(2), function () {
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
function SetRadioSelectMapExt(mapExts, mapAttr, selectVal, model,tag) {
    //联动其他控件
    /*if (isEnableJS == true && (selectVal != null && selectVal != undefined && selectVal != "")) {
        if (model == "radio" && selectVal == -1) {
        } else {
            cleanAll(mapAttr.KeyOfEn, frmType);
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, selectVal, frmType);
        }

    }*/
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
                if (tag == true)
                    DBAccess.RunFunctionReturnStr(mapExt.Doc);
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
 */
function SetDateExt(mapExts, mapAttr,targetID) {
    var funcDoc = "";
    var roleExt = null;
    $.each(mapExts, function (k, mapExt1) {
        var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
        mapExt.MyPK = mapExt1.MyPK;
        if (mapExt.ExtType == "BindFunction")
            funcDoc = mapExt.Doc;
        if (mapExt.ExtType == "DataFieldInputRole" && mapExt.DoWay == 1) {
            roleExt = mapExt;
        }
    });
    var format = $("#TB_" + targetID).attr("data-info");
    var type = $("#TB_" + targetID).attr("data-type");
    var dateOper = "";
    if (roleExt != null) {
        if (roleExt.Tag1 == 1) {//不能选择历史时间
            dateOper = {
                elem: '#TB_' + targetID,
                format: format, //可任意组合
                type: type,
                min: 0,
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
                    $(this.elem).val(value);
                    if (funcDoc != "")
                        DBAccess.RunFunctionReturnStr(funcDoc);
                    var data = $(this.elem).data();
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            }
        }
        if (roleExt.Tag2 == 1) {
            //根据选择的条件进行日期限制
            var isHaveOper = $("#TB_" + roleExt.Tag4).is(".ccdate");
            var startOper = "";
            startOper = {
                elem: '#TB_' + roleExt.Tag4,
                format: format, //可任意组合
                type: type,
                operKey: mapAttr.KeyOfEn,
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
                    var msg = true;
                    switch (oper) {
                        case "dayu":
                            if (value >= operVal && operVal != "")
                                msg = "所选日期不能大于等于" + this.operKey + "对应的日期时间";
                            break;
                        case "dayudengyu":
                            if (value > operVal && operVal != "")
                                msg = "所选日期不能大于" + this.operKey + "对应的日期时间";
                            break;
                        case "xiaoyu":
                            if (value <= operVal && operVal != "")
                                msg = "所选日期不能小于等于" + this.operKey + "对应的日期时间";
                            break;
                        case "xiaoyudengyu":
                            if (value < operVal && operVal != "")
                                msg = "所选日期不能小于" + this.operKey + "对应的日期时间";
                            break;
                        case "budengyu":
                            if (value == operVal && operVal != "")
                                msg = "所选日期不能等于" + this.operKey + "对应的日期时间";
                            break;
                    }
                    if (msg != "")
                        value = "";
                    $(this.elem).val(value);
                    if (msg != "")
                        layer.alert(msg);


                }
            }
            dateOper = {
                elem: '#TB_' + mapAttr.KeyOfEn,
                format: format, //可任意组合
                type: type,
                operKey: roleExt.Tag4,
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
                    switch (oper) {
                        case "dayu":
                            if (value <= operVal && operVal != "") {
                                layer.alert("所选日期不能小于等于" + this.operKey + "对应的日期时间")
                                $(this.elem).val("");
                                return;
                            }
                            break;
                        case "dayudengyu":
                            if (value < operVal && operVal != "") {
                                layer.alert("所选日期不能小于" + this.operKey + "对应的日期时间")
                                $(this.elem).val("");
                                return;
                            }
                            break;
                        case "xiaoyu":
                            if (value >= operVal && operVal != "") {
                                layer.alert("所选日期不能大于等于" + this.operKey + "对应的日期时间")
                                $(this.elem).val("");
                                return;
                            }
                            break;
                        case "xiaoyudengyu":
                            if (value > operVal && operVal != "") {
                                layer.alert("所选日期不能大于" + this.operKey + "对应的日期时间")
                                $(this.elem).val("");
                                return;
                            }
                            break;
                        case "budengyu":
                            if (value == operVal && operVal != "") {
                                layer.alert("所选日期不能等于" + this.operKey + "对应的日期时间")
                                $(this.elem).val("");
                                return;
                            }
                            break;
                    }

                    $(this.elem).val(value);
                    if (funcDoc != "")
                        DBAccess.RunFunctionReturnStr(funcDoc);
                    var data = $(this.elem).data();
                    if (data && data.ReqDay != null && data.ReqDay != undefined)
                        ReqDays(data.ReqDay);
                }
            }

            if (isHaveOper == true && startOper != "")
                layui.laydate.render(startOper);
        }

    } else {
        dateOper = {
            elem: '#TB_' + mapAttr.KeyOfEn,
            format: format, //可任意组合
            type: type,
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
                $(this.elem).val(value);
                if (funcDoc != "")
                    DBAccess.RunFunctionReturnStr(funcDoc);
                var data = $(this.elem).data();
                if (data && data.ReqDay != null && data.ReqDay != undefined)
                    ReqDays(data.ReqDay);
            }
        }
    }

    layui.laydate.render(dateOper);
    $("#TB_" + mapAttr.KeyOfEn).removeClass(".ccdate");
}
/**
 * 整数，浮点型，金额型扩展属性的解析
 * @param {any} mapExts
 * @param {any} mapAttr
 */
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
                if ($('#TB_' + mapExt.AttrOfOper).val() == "0") {
                    ReqDays(mapExt);
                    $('#TB_' + mapExt.Tag1).data({ "ReqDay": mapExt })
                    $('#TB_' + mapExt.Tag2).data({ "ReqDay": mapExt });
                }

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
        var iframeDtl = $("#Frame_" + obj[0].DtlNo);
        iframeDtl.load(function () {
            $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[obj[0].DtlNo]));
            if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                this.contentWindow.parentStatistics(detailExt[obj[0].DtlNo]);
            }
        });

    });

}

function DynamicBind(mapExt, ctrlType) {

    if (ctrlType == "RB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
        });
    } else if (ctrlType == "CB_") {
        $('input[name="' + ctrlType + mapExt.AttrOfOper + '"]').on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
        });
    }
    else {
        $('#' + ctrlType + mapExt.AttrOfOper).on(mapExt.Tag, function () {
            DBAccess.RunFunctionReturnStr(mapExt.Doc);
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
    var demoRDT;
    demoRDT = StarRDT.split("-");
    StarRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);  //转换为yyyy-MM-dd格式
    demoRDT = EndRDT.split("-");
    EndRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);
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
function PopMapExt(popType, mapAttr, mapExt, frmData, mapExts,targerId,pkval) {
    if (isHaveLoadPop == false) {
        Skip.addJs("./JS/Pop2021.js");
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
                CommPopDialog(popType, mapAttr, mapExt, pkval, frmData, "./", mapExts, targerId);
            }

            break;
        case "PopBranchesAndLeaf": //树干叶子模式.

        case "PopTableSearch": //表格查询.
        case "PopSelfUrl": //自定义url.
            if (isHaveLoagMtags == false) {
                Skip.addJs("./JS/mtags2021.js");
                isHaveLoagMtags = true;
            }
            CommPopDialog(popType, mapAttr, mapExt, pkval, frmData, "./", mapExts, targerId);
            break;
        case "PopBindSFTable": //绑定字典表，外部数据源.
        case "PopBindEnum": //绑定枚举.
        case "PopTableList": //绑定实体表.
        case "PopGroupList": //分组模式.
            CommPop(popType, mapAttr, mapExt, frmData, mapExts, targerId,pkval);
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
 * 保存EleDB
 * @param {any} rows
 */
function SaveFrmEleDBs(rows, keyOfEn, mapExt, pkval) {
    pkval = pkval == null || pkval == undefined || pkval == 0 ? pageData.OID : pkval;
    //删除
    var ens = new Entities("BP.Sys.FrmEleDBs");
    ens.Delete("FK_MapData", mapExt.FK_MapData, "EleID", keyOfEn, "RefPKVal", pkval);
    //保存
    $.each(rows, function (i, row) {
        var frmEleDB = new Entity("BP.Sys.FrmEleDB");
        frmEleDB.MyPK = keyOfEn + "_" + pkval + "_" + row.No;
        frmEleDB.FK_MapData = mapExt.FK_MapData;
        frmEleDB.EleID = keyOfEn;
        frmEleDB.RefPKVal = pkval;
        frmEleDB.Tag1 = row.No;
        frmEleDB.Tag2 = row.Name;
        frmEleDB.Insert();
    })
}
