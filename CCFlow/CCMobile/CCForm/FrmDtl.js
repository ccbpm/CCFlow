/**
 * 主表中只显示从表名称，点击详情跳转的页面
 */
var Form_ReadOnly = false;
var mainData;
var gfs; //明细表分组
var dtlSize = 0; //明细表条数
var dtlmapAttrs;
var dtlMapExt;
var dtl_No;
var dtls = {};
//加载明细表数据
function Load_DtlInit(dtlDiv, dtlNo) {
    dtlDiv = dtlDiv || "DtlContent";
    $("#" + dtlDiv).empty();
    dtl_No = dtlNo || $("#HD_CurDtl_No").val();
    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", dtl_No);
    hand.AddPara("RefPKVal", pageData.WorkID);
    hand.AddPara("FK_Node", pageData.FK_Node);
    hand.AddPara("IsReadonly", pageData.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");

    //获取真正含有的分组
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddPara("FrmID", dtl_No);
    gfs = handler.DoMethodReturnJSON("RetrieveFieldGroup");


    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    dtlmapAttrs = mainData["Sys_MapAttr"]; //从表字段.
    dtlMapExt = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.
    var mapDtls = mainData["MapDtls"]; //从表的从表集合.

    dtlSize = dbDtl.length;
    if (!$.isArray(dtls[dtl_No])) {
        dtls[dtl_No] = [];
    }
    dtls[dtl_No].push({
        No: dtl_No,
        Name: sys_MapDtl.Name,
        Count: dbDtl.length,
        NumOfDtl: sys_MapDtl.NumOfDtl
    })

    var _Html = "";
    //判断是否有数据
    if (dbDtl.length == 0) {
        _Html = "<div class='mui-indexed-list-inner empty'>";
        _Html += " <div class='mui-indexed-list-empty-alert'>没有数据</div>";
        _Html += "</div>";
    }
    Form_ReadOnly = pageData.IsReadonly == "1" ? true : false;

    if (Form_ReadOnly == false && sys_MapDtl.IsReadonly == "1")
        Form_ReadOnly = true;

    //加载表单元素\数据
    var dtl_Idx = 1;
    for (var j = 0; j < dbDtl.length; j++) {
        _Html += "<ul class='mui-table-view'>";
        _Html += "  <li class='mui-table-view-divider'>明细:" + dtl_Idx;
        if (sys_MapDtl.IsDelete == "1") {
            _Html += "   <div class='dtl_deleterow' id='" + dbDtl[j].OID + "' data-info='" + dtl_No + "' data-div='" + dtlDiv +"'>删除</div>";
        }
        _Html += "	</li>";
        //循环分组
        for (var g = 0; g < gfs.length; g++) {
            //收个分组标签不进行添加
            if (g != 0 && gfs[g].CtrlType != 'Ath') {
                _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
            }
            //明细表的控件.
            if (gfs[g].CtrlType == 'Dtl') {
                if (mapDtls) {
                    $.each(mapDtls, function (i, dtl) {
                        if (gfs[g].CtrlID == dtl.No) {
                            var func = "DtlChild_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\",\"" + dbDtl[j].OID + "\")";
                            _Html += "<div class='mui-table-view-cell'>";
                            _Html += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                                + "<span id='" + dtl.No + "_Count'></span></h5>";
                            _Html += "		<p>点击查看详细</p>";
                            _Html += "	</a>";
                            _Html += "</div>";
                        }
                    });
                }
                continue;
            }
            if (gfs[g].CtrlType == 'Ath') {

                _Html += InitAth(frmData, gfs[g],false, mainData, 1, dbDtl[j].OID);
                continue;
            }
            //字段生成
            var OID = dbDtl[j].OID;
            for (var k = 0; k < dtlmapAttrs.length; k++) {

                if (dtlmapAttrs[k].UIVisible == "0") {
                    _Html += "<input type='hidden' id='TB_" + dtlmapAttrs[k].KeyOfEn + "_" + OID + "' name='TB_" + dtlmapAttrs[k].KeyOfEn + "_" + OID + "' value='" + dbDtl[j][dtlmapAttrs[k].KeyOfEn] + "' />";
                    continue;
                }

                if (dtlmapAttrs[k].KeyOfEn == "OID") {
                    _Html += "<input type='hidden' id='TB_" + dtlmapAttrs[k].KeyOfEn + "_" + OID + "' name='TB_" + dtlmapAttrs[k].KeyOfEn + "_" + OID + "' value='" + dbDtl[j][dtlmapAttrs[k].KeyOfEn] + "' />";
                    continue;
                }


                var transControl = new TranseDtlControl(dtlmapAttrs[k], dbDtl[j]);
                _Html += transControl.To_Html();
            }
        }
        _Html += "</ul>";
        dtl_Idx++;
    }

    //启用新增按钮
    if (Form_ReadOnly == false && sys_MapDtl.IsInsert == "1") {
        _Html += "<ul class='mui-table-view' id='AddInfo'>";
        _Html += "<li class='mui-table-view-cell'>";
        _Html += " <div class='dtl_addpanel'>";
        _Html += "    <a data-info='" + dtl_No +"' data-div='"+dtlDiv+"'><b class='dtl_addpanel_b'>+</b> 增加" + sys_MapDtl.Name + "明细</a>";
        _Html += " </div>";
        _Html += "</li>";
        _Html += "</ul>";
    }
    //添加保存按钮
    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        $("#dtlDone").html("提交");
        $("#dtlDone").off("tap").on("tap", function () {
            if (checkDtlBlanks() == false) {
                mui.alert("请输入必填项！");
                return false;
            }
            $("input").blur();
            var isTrue = Dtl_SaveData(dtl_No,"form_Dtl");
            if (isTrue == false)
                return;
            viewApi.back();
            mui.toast("保存成功！");

        });
    }

    //更新明细表记录数量
    //$("#" + dtl_No + "_Count").html("(" + dbDtl.length + ")条记录");



    //生成页面
    $("#" + dtlDiv).append(_Html);

    if (mainData.Sys_FrmAttachment.length > 0) {
        try {
            var s = document.createElement('script');
            s.type = 'text/javascript';
            s.src = "/CCMobile/js/mui/js/feedback-page.js";
            var tmp = document.getElementsByTagName('script')[0];
            tmp.parentNode.insertBefore(s, tmp);
        }
        catch (err) {

        }
    }

    if (pageData.IsReadonly == "1") {
        setDtlFormEleDisabled((dtlDiv == "DtlContent" ? "form_Dtl" : dtlNo + "form_Dtl"))
    }

    //解析扩展设置,MapExt
    for (var y = 0; y < dbDtl.length; y++) {
        AfterBindDtl_DealMapExt(dbDtl[y].OID);
    }

    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        //日期控件
        mui(".mui-input-row").off("tap").on("tap", ".ccformdatedtl", function () {
            var dDate = new Date();
            var optionsJson = this.getAttribute('data-options') || '{}';
            var ctrID = this.getAttribute('id');
            var options = JSON.parse(optionsJson);
            var picker = new mui.DtPicker(options);
            picker.show(function (rs) {
                var timestr = rs.text;
                $("#" + ctrID).html(timestr);
                $("#TB_" + ctrID.substr(4)).val(timestr);
                picker.dispose();
            });
        });
       
    }
    //添加行事件
    $("#"+dtlDiv+" .dtl_addpanel a").on("click", function () {
        //先保存后新增行
        var dtlNo = $(this).attr("data-info");
        var dtlDivID = $(this).attr("data-div");
        var isTrue = Dtl_SaveData(dtlNo, (dtlDivID == "DtlContent" ? "form_Dtl" : dtlNo + "form_Dtl"));
        if (isTrue == false)
            return;
        Dtl_InsertRow(dtlNo);
        Load_DtlInit(dtlDivID, dtlNo);

    });
    //删除事件
    $("#" + dtlDiv +" .dtl_deleterow").on("click", function () {
        var target = $(this);
        var oid = target.attr("id");
        var dtlNo = $(this).attr("data-info");
        var dtlDivID = $(this).attr("data-div");
        //先保存后删除行
        var isTrue = Dtl_SaveData(dtlNo, (dtlDivID == "DtlContent" ? "form_Dtl" : dtlNo + "form_Dtl"));
        if (isTrue == false)
            return;
        Dtl_DeleteByOID(oid, dtlDivID, dtlNo);

    });
    mui(".mui-switch").switch();
    //监听开关事件
    var SW = $('.mui-switch');
    $.each(SW, function (i, obj) {
        var KeyOfEn = $(obj).attr("id");
        document.getElementById(KeyOfEn).addEventListener("toggle", function (event) {
            KeyOfEn = KeyOfEn.substring(3);
            if (event.detail.isActive) {
                $("#CB_" + KeyOfEn).val("1");
            } else {
                $("#CB_" + KeyOfEn).val("0");
            }
        })
    })

    //var numInputs = $(".minMax");
    //$.each(numInputs, function (i, obj) {
    //    $(obj).bind("input", function () {
    //        NumEnterLimit($(this));
    //    })
    //})
    parentStatistics(dtlNo);
    mui('.mui-numbox').numbox();
}

function setDtlFormEleDisabled(formID) {
    //文本框等设置为不可用
    $('#'+formID+' li textarea').attr('disabled', 'disabled');
    $('#'+ formID +' li select').attr('disabled', 'disabled');
    $('#'+ formID +' li input[type!=button]').attr('disabled', 'disabled');
}

//根据从表字段计算主表的和
function parentStatistics(dtlNo) {
    if (detailExt != undefined && $.isArray(detailExt[dtlNo])) {
        $.each(detailExt[dtlNo], function (i, extObj) {
            var exp = extObj.exp;
            var name = extObj.DtlColumn;
            var obj = $(":input[name^=TB_" + name + "_]");
            if (obj.length == 0)
                obj = $(":input[name^=RB_" + name + "_]");
            if (obj.length == 0)
                obj = $(":input[name^=DDL_" + name + "_]");
            if (obj.length == 0)
                obj = $(":input[name^=CB_" + name + "_]");

            var template = $("#TB_" + extObj.AttrOfOper);
            var DXTemplate = $("#TB_" + extObj.DaXieAttrOfOper);
            var expVal = 0;
            if (exp == "Sum") {	// 和
                var sum = 0;
                //判断值是否含有小数
                var flag = false;
                obj.each(function (i, e) {
                    var val = $(e).val();
                    val = val.replace(/,/g, "");
                    val = val.replace(/￥/g, "");
                    sum += parseFloat(val);
                    if ($(e).val().indexOf('.') >= 0)
                        flag = true;
                });
                if (flag) {
                    if (!/\./.test(sum))
                        sum += '.00';
                    //防止出现相加小数位数不正确的情况
                    parseFloat(sum).toFixed(2);
                }
                expVal = sum;


                //sum = formatNumber(sum, 2, ',');

                template.val(sum);
            } else if (exp == "Avg") {	// 平均数
                var sum = 0;
                var count = 0;
                obj.each(function (i, e) {
                    var val = $(e).val();
                    val = val.replace(/,/g, "");
                    val = val.replace(/￥/g, "");
                    sum += parseFloat(val);
                    count++;
                });
                if (count > 0) {
                    expVal = sum / count;

                    var avg = formatNumber(sum / count, 2, ',');

                    template.val(avg);
                }
            } else if (exp == "Max") {	// 最大
                var max = null;
                obj.each(function (i, e) {
                    var val = $(e).val();
                    val = val.replace(/,/g, "");
                    val = val.replace(/￥/g, "");

                    var value = parseFloat(val);
                    if (max == null) {
                        max = value;
                    } else if (value > max) {
                        max = value;
                    }
                });
                expVal = max;
                max = formatNumber(max, 2, ',');
                template.val(max);
            } else if (exp == "Min") {	// 最小
                var min = null;
                obj.each(function (i, e) {
                    var val = $(e).val();
                    val = val.replace(/,/g, "");
                    val = val.replace(/￥/g, "");

                    var value = parseFloat(val);
                    if (min == null) {
                        min = value;
                    } else if (value < min) {
                        min = value;
                    }
                });
                expVal = min;
                min = formatNumber(min, 2, ',');
                template.val(min);
            }

            template.trigger("change");
            if (DXTemplate.length == 1)
                DXTemplate.val(Rmb2DaXie(expVal));

            if (extObj.Tag == "1")
                DBAccess.RunFunctionReturnStr(extObj.Tag1);
        });

    }
}

function checkDtlBlanks(dtlNo) {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput

    var lbs = $('#' + dtlNo+' .mustInput');
    $.each(lbs, function (i, obj) {
        var parentObj = $(obj).parent().parent();
        if (parentObj && parentObj.css('display') != 'none') {
            var keyOfEn = obj.id;
            if (keyOfEn != null) {
                var item = $("#" + keyOfEn);
                if (item.length == 0)
                    return true;
                if (keyOfEn.indexOf("TB_") == 0) {
                    if (item.val() == "") {
                        checkBlankResult = false;
                        item.addClass('errorInput');
                    } else {
                        item.removeClass('errorInput');
                    }
                    return true;
                }

                if (keyOfEn.indexOf("DDL_") == 0) {
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

//保存数据
function Dtl_SaveData(dtlNo,formID) {
    if ($(".compareClass").length > 0)
        return false;
    //必填项和正则表达式检查.
    if (checkDtlBlanks(dtlNo) == false) {
        mui.alert("请输入必填项！");
        return false;
    }
    var urlExt = urlExtFrm();
    var args = new RequestArgs();
    //var dtl_No = $("#HD_CurDtl_No").val();

    var url = GetHrefUrl();
    if (url.indexOf('/jflow-web/') >= 0) {
        var index = url.indexOf('/jflow-web');
        url = url.substring(index);
    }
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
    handler.AddPara("EnsName", dtlNo);
    handler.AddPara("RefPKVal", pageData.WorkID);
    handler.AddUrlData();
    var params = getFormData(true, true, formID, true);

    handler.AddJson(params);
    var data = handler.DoMethodReturnString("Dtl_SaveRow");
    if (data.indexOf("err@") == 0) {
        mui.toast(data);
        return;
    }
    parentStatistics(dtlNo);
    return true;
}

//添加行
function Dtl_InsertRow(dtlNo) {
    var args = new RequestArgs();
    var dtl = new Entity(dtlNo);
    dtl.RefPK = args.WorkID;
    dtl.FID = args.FID;
    dtl = dtl.Insert();
    //$.each(dtlmapAttrs, function (item) {
    //    dtl[item.KeyOfEn] = GetValByDefVal(dtl[item.KeyOfEn], item);
    //});
    //dtl.Update();
    Load_DtlForm(dtl);
    parentStatistics(dtlNo);
}

/**
* 默认值的转化
* @param defVal 默认值
* @param attr 字段属性
*/
function GetValByDefVal(defVal, attr) {
    switch (defVal) {
        case "@WebUser.No":
        case "@CurrWorker":
            return webUser.No;
        case "@WebUser.Name":
            return webUser.Name;
        case "@WebUser.FK_Dept":
            return webUser.FK_Dept;
        case "@WebUser.FK_DeptName":
            return webUser.FK_DeptName;
        case "@WebUser.FK_DeptNameOfFull":
        case "@WebUser.FK_DeptFullName":
            return webUser.FK_DeptNameOfFull;
        case "@WebUser.OrgNo":
            return webUser.OrgNo;
        case "@WebUser.OrgName":
            return webUser.OrgName;
        case "@RDT":
            var dataFormat = "yyyy-MM-dd";
            switch (attr.IsSupperText) {
                case 0: break;
                case 1:
                    dataFormat = "yyyy-MM-dd HH:mm";
                    break;
                case 2:
                    dataFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case 3:
                    dataFormat = "yyyy-MM";
                    break;
                case 4:
                    dataFormat = "HH:mm";
                    break;
                case 5:
                    dataFormat = "HH:mm:ss";
                    break;
                case 6:
                    dataFormat = "MM-dd";
                    break;
                case 7:
                    dataFormat = "yyyy";
                    break;
                default:
                    alert("没有找到指定的时间类型");
                    return;
            }
            return FormatDate(new Date(), dataFormat);
        case "@FK_ND":
            return FormatDate(new Date(), "yyyy-MM");
        case "@yyyy年MM月dd日":
        case "@yyyy年MM月dd日HH时mm分":
        case "@yy年MM月dd日":
        case "@yy年MM月dd日HH时mm分":
        case "@yyyy-MM-dd":
            return FormatDate(new Date(), defVal.replace("@", ""));
        default:
            return defVal;
    }
}
function Load_DtlForm(dbDtl) {
    var _Html = "";
    var index = parseInt(dtlSize) + 1;
    _Html += "<ul class='mui-table-view'>";
    _Html += "  <li class='mui-table-view-divider'>明细:" + index;
    _Html += "	</li>";
    //循环分组
    for (var g = 0; g < gfs.length; g++) {
        //收个分组标签不进行添加
        if (g != 0) {
            _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
        }
        //明细表的控件.
        if (gfs[g].CtrlType == 'Dtl') {

            continue;
        }
        //字段生成
        for (var k = 0; k < dtlmapAttrs.length; k++) {
            //            if (sys_mapAttr[k].GroupID != gfs[g].OID)
            //                continue;
            if (dtlmapAttrs[k].UIVisible == "0")
                continue;
            if (dtlmapAttrs[k].KeyOfEn == "OID")
                continue;

            var transControl = new TranseDtlControl(dtlmapAttrs[k], dbDtl);
            _Html += transControl.To_Html();
        }
    }
    _Html += "</ul>";
    $(_Html).appendTo('#DtlContent');
    AfterBindDtl_DealMapExt(dbDtl.OID);
}

//删除记录通过主键OID
function Dtl_DeleteByOID(oid, dtlDiv,dtlNo) {
    var btnArray = ['否', '是'];
    mui.confirm('确定要删除所选记录吗？', '提示', btnArray, function (e) {
        if (e.index == 1) {
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
            handler.AddPara("FK_MapDtl", dtlNo);
            handler.AddPara("OID", oid);
            handler.DoMethodReturnString("Dtl_DeleteRow");
            Load_DtlInit(dtlDiv,dtlNo);
        }
    });
}

//打开明细表的明细表
function DtlChild_ShowPage(dtlNo, dtlName, OID) {
    $("#HD_CurChildDtl_No").val(dtlNo);
    $("#HD_CurDtl_OID").val(OID);
    $("#frmChildDtlTitle").html(dtlName);
    Load_ChildDtlForm();
    viewApi.go("#frmChildDtl");
}

//加载明细表的字表数据
function Load_ChildDtlForm() {
    var cdtl_No = $("#HD_CurChildDtl_No").val();
    $("#ChildDtlContent").empty();
    var OID = $("#HD_CurDtl_OID").val();
    var args = new RequestArgs();

    //获得mapdtl实体的基本信息.
    var hand = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    hand.AddPara("EnsName", cdtl_No);
    hand.AddPara("RefPKVal", OID);
    hand.AddPara("FK_Node", args.FK_Node);
    hand.AddPara("IsReadonly", args.IsReadonly);
    mainData = hand.DoMethodReturnJSON("Dtl_Init");

    //获取正真含有的分组
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_MyFlow");
    handler.AddPara("FK_Node", args.FK_Node);
    handler.AddPara("FK_MapData", cdtl_No);
    var gfs = handler.DoMethodReturnJSON("GetGroupFields");


    //主表数据，用于变量替换.
    var mainTable = mainData["MainTable"]; //主表数据.
    //从表信息.
    sys_MapDtl = mainData["Sys_MapDtl"][0]; //从表描述.
    var sys_mapAttr = mainData["Sys_MapAttr"]; //从表字段.
    var sys_mapExtDtl = mainData["Sys_MapExt"]; //扩展信息.
    var dbDtl = mainData["DBDtl"]; //从表数据.
    //var mapDtls = mainData["MapDtls"]; //从表的从表集合.

    var _Html = "";
    //判断是否有数据
    if (dbDtl.length == 0) {
        if (Form_ReadOnly == false && sys_MapDtl.Insert == "1") {
            Dtl_InsertRow();
            return;
        }
        _Html = "<div class='mui-indexed-list-inner empty'>";
        _Html += " <div class='mui-indexed-list-empty-alert'>没有数据</div>";
        _Html += "</div>";
    } else if (Form_ReadOnly == true || (sys_MapDtl.IsInsert == "0" && sys_MapDtl.IsUpdate == "0")) {
        //只读
        Form_ReadOnly = true;
    }

    //加载表单元素\数据
    var dtl_Idx = 1;
    for (var j = 0; j < dbDtl.length; j++) {
        _Html += "<ul class='mui-table-view'>";
        _Html += "  <li class='mui-table-view-divider'>明细:" + dtl_Idx;
        if (sys_MapDtl.IsDelete == "1") {
            _Html += "   <div class='dtlchild_deleterow' id='" + dbDtl[j].OID + "'>删除</div>";
        }
        _Html += "	</li>";
        //循环分组
        for (var g = 0; g < gfs.length; g++) {
            //收个分组标签不进行添加
            if (g != 0) {
                _Html += "<div class=\"mui-table-view-divider\"><h5 style='color:black;'>" + gfs[g].Lab + "</h5></div>";
            }
            //明细表的控件.
            if (gfs[g].CtrlType == 'Dtl') {
                if (mapDtls) {
                    $.each(mapDtls, function (i, dtl) {
                        if (gfs[g].CtrlID == dtl.No) {
                            var func = "DtlChild_ShowPage(\"" + dtl.No + "\",\"" + dtl.Name + "\")";
                            _Html += "<div class='mui-table-view-cell'>";
                            _Html += "	<a class='mui-navigate-right' data-title-type='native' href='javascript:" + func + "'><h5>" + dtl.Name
                                + "<span id='" + dtl.No + "_Count'></span></h5>";
                            _Html += "		<p>点击查看详细</p>";
                            _Html += "	</a>";
                            _Html += "</div>";
                        }
                    });
                }
                continue;
            }
            //字段生成
            for (var k = 0; k < sys_mapAttr.length; k++) {
                if (sys_mapAttr[k].GroupID != gfs[g].OID)
                    continue;
                if (sys_mapAttr[k].UIVisible == "0")
                    continue;
                if (sys_mapAttr[k].KeyOfEn == "OID")
                    continue;
                var transControl = new TranseDtlControl(sys_mapAttr[k], null);
                _Html += transControl.To_Html();
            }
        }
        _Html += "</ul>";

    }



    //生成页面
    $("#AddInfo").before(_Html);

    if (Form_ReadOnly == false && dbDtl.length > 0 && (sys_MapDtl.IsInsert == "1" || sys_MapDtl.IsUpdate == "1")) {
        //日期控件
        mui(".mui-input-row").off("tap").on("tap", ".ccformdatedtl", function () {
            var dDate = new Date();
            var optionsJson = this.getAttribute('data-options') || '{}';
            var ctrID = this.getAttribute('id');
            var options = JSON.parse(optionsJson);
            var picker = new mui.DtPicker(options);
            picker.show(function (rs) {
                var timestr = rs.text;
                $("#" + ctrID).html(timestr);
                $("#TB_" + ctrID.substr(4)).val(timestr);
                picker.dispose();
            });
        });
    }

}

//保存子表数据
function DtlChild_SaveData(CallBack) {
    var urlExt = urlExtFrm();
    var args = new RequestArgs();
    var dtl_No = $("#HD_CurChildDtl_No").val();
    var revpk = $("#HD_CurDtl_OID").val();

    var url = GetHrefUrl();
    if (url.indexOf('/jflow-web/') >= 0) {
        var index = url.indexOf('/jflow-web');
        url = url.substring(index);
    }
    var ccmobile = url.substring(0, url.lastIndexOf('/') + 1) + "CCForm/ProcessRequest.do";

    $("#form_ChildDtl").ajaxSubmit({
        type: 'post',
        url: ccmobile + "?DoType=SaveDtl&EnsName=" + dtl_No + "&RefPKVal=" + revpk,
        success: function (data) {
            CallBack(data);
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) {
            mui.toast("保存失败，请检查表单！");
        }
    });
}

//子表添加行
function DtlChild_InsertRow() {
    var dtl_No = $("#HD_CurChildDtl_No").val();
    var revpk = $("#HD_CurDtl_OID").val();
    var dtl = new Entity(dtl_No);
    dtl.RefPK = revpk;
    dtl.FID = 0;
    dtl = dtl.Insert();
    Load_ChildDtlForm();
}

//删除记录通过主键OID
function DtlChild_DeleteByOID(oid) {
    var btnArray = ['否', '是'];
    mui.confirm('确定要删除所选记录吗？', '提示', btnArray, function (e) {
        if (e.index == 1) {
            var dtl_No = $("#HD_CurChildDtl_No").val();
            var dtl = new Entity(dtl_No);
            dtl.OID = oid;
            dtl.SetPKVal(oid);
            var dtl = dtl.Delete();
            Load_ChildDtlForm();
        }
    });
}

//转控件
function TranseDtlControl(dtlColumn, DataRow) {
    this.control = dtlColumn;
    this.DataRow = DataRow;
    this.Ctrl_Class = "";
    //控件是否可用
    this.Enable = true;
}
//控件属性
TranseDtlControl.prototype = {
    To_Html: function () {
        var _html = "";
        this.Ctrl_Class = "";
        this.Enable = true;
        //判断控件是否可用
        if (this.control.UIIsEnable == "0" || Form_ReadOnly == true) {
            this.Enable = false;
            this.Ctrl_Class = "disabled = \"disabled\" ";
        }
        if (this.control.MyDataType == FormDataType.AppString && this.control.IsSupperText == 1)
            _html = "";
        else {
            _html = "<li class='mui-input-row'>";
            //标签
            var mustInputDtl = this.control.UIIsInput == 1 ? '<span style="color:red;display: inline-block;" class="mustInputDtl" data-keyofen="' + this.control.KeyOfEn + '" >*</span>' : "";
            _html += "	<label><p>" + this.control.Name + mustInputDtl + "</P></label>";
        }

        //图片签名
        if (this.control.IsSigan == "1") {
            _html += this.CreateSignPicture();
            _html += "</li>"
            return _html;
        }
        var Ctrl_Val = "";
        if (this.DataRow != null)
            Ctrl_Val = this.DataRow[this.control.KeyOfEn];

        if (Ctrl_Val == undefined)
            Ctrl_Val = "";
        //if (this.control.DefValType == 0 && this.control.DefVal == "10002" && (Ctrl_Val == "10002" || Ctrl_Val == "10002.0000"))
        //    Ctrl_Val = "";
        //加载其他数据控件
        switch (this.control.LGType) {
            case FieldTypeS.Normal: //输出普通类型字段
                if (this.control.UIContralType == UIContralType.DDL) {
                    //判断外部数据或WS类型.
                    if (this.Enable == false) {
                        this.Ctrl_Class = "disabled = \"disabled\" ";
                    }
                    _html += this.CreateDDLPK(Ctrl_Val);
                    break;
                }
                switch (this.control.MyDataType) {
                    case FormDataType.AppString:
                        _html += this.CreateTBString(Ctrl_Val);
                        break;
                    case FormDataType.AppInt:
                        _html += this.CreateTBInt(Ctrl_Val);
                        break;
                    case FormDataType.AppFloat:
                    case FormDataType.AppDouble:
                    case FormDataType.AppMoney:
                        _html += this.CreateTBFloat(Ctrl_Val);
                        break;
                    case FormDataType.AppDate:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateTBDate(Ctrl_Val);
                        break;
                    case FormDataType.AppDateTime:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateTBDateTime(Ctrl_Val);
                        break;
                    case FormDataType.AppBoolean:
                        if (this.Enable == false) {
                            this.Ctrl_Class = "readonly = \"readonly\" ";
                        }
                        _html += this.CreateCBBoolean(Ctrl_Val);
                        break;
                }
                break;
            case FieldTypeS.Enum: //枚举值下拉框
                if (this.Enable == false) {
                    this.Ctrl_Class = "disabled = \"disabled\" ";
                }
                _html += this.CreateDDLEnum(Ctrl_Val);
                break;
            case FieldTypeS.FK: //外键表下拉框    
                if (this.Enable == false) {
                    this.Ctrl_Class = "disabled = \"disabled\" ";
                }
                _html += this.CreateDDLPK(Ctrl_Val);
                break;
        }
        _html += "</li>"
        return _html;
    },
    CreateSignPicture: function (Ctrl_Val) {
        //图片签名
        var sign_id = this.control.KeyOfEn;
        var val = this.DataRow[sign_id];
        var errorVal = this.DataRow["ImgErrorValue"];
        return "<img name=\"Sign_Dtl_" + sign_id + "\" id=\"Sign_Dtl_" + sign_id + "\" src='" + val + "' border=0 onerror=\"this.src='" + errorVal + "'\"/>";
    },
    CreateTBString: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //多行文本
        if (this.control.IsSupperText == 1) {
            var html_string = "<div class='' style='padding:11px 15px;line-height:1.1;'>";
            html_string += "<label for=\"" + Ctrl_Id + "\"><p>" + this.control.Name + "</p></label>";
            if (this.control.UIIsEnable == "0" || Form_ReadOnly == false)
                html_string += "<div name='" + Ctrl_Id + "' id='" + Ctrl_Id + "' style='padding:5px;border:1px solid #d6dde6;font-size: 14px;line-height:22px;'>" + Ctrl_Val + "</div>";
            else
                html_string += "<textarea wrap='virtual' onpropertychange= 'this.style.posHeight=this.scrollHeight' cols='40' style='overflow:visible;font-size:14px;width:100%;border:solid 1px gray;' rows=\"5\"  name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\">" + Ctrl_Val + "</textarea>";
            return html_string;

        }
        //单行文本
        return "<input " + this.Ctrl_Class + " type=\"text\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
    },
    CreateTBInt: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        this.Ctrl_Class = " class='mui-numbox-input'";
        var disabledStr = "";
        if (this.control.UIIsEnable == "0" || Form_ReadOnly == true)
            disabledStr = " disabled=disabled";
        var event = 'onblur="valitationAfter(this, \'int\')" onkeydown="valitationBefore(this, \'int\')" onkeyup="valitationAfter(this, \'int\'); if(isNaN(value) || (value%1 !== 0))execCommand(\'undo\')"  onafterpaste="valitationAfter(this, \'int\'); if(isNaN(value) || (value%1 !== 0))execCommand(\'undo\')"';
        var minNum = GetPara(this.control.AtPara, "NumMin")||"";
        var maxNum = GetPara(this.control.AtPara, "NumMax") || "";
        var dataInfo = "";
        if (minNum!="")
            dataInfo = " data-numbox-min='" + minNum + "'";
        if (maxNum != "")
            dataInfo += " data-numbox-max='" + maxNum + "'";
        var inputHtml = "";
        if (Form_ReadOnly == false && this.control.UIIsEnable == 1) {
            var step = GetPara(this.control.AtPara, "NumStepLength");
            step = step == null || step == undefined ? 1 : parseInt(step) == 0 ? 1 : parseInt(step);
            inputHtml += "<div class='mui-numbox' data-numbox-step='"+step+"'  data-numbox-bit='0' " +dataInfo+"style='width:200px'>";
            inputHtml += "<button class='mui-btn mui-btn-numbox-minus' type='button'>-</button>";
        }
        inputHtml += "<input " + this.Ctrl_Class + disabledStr + " type=\"number\" " + event;

        if (this.control.DefValType == 0)
            inputHtml += " name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        else
            inputHtml += " name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" placeholder=\"0\" value=\"" + Ctrl_Val + "\" />";
        if (Form_ReadOnly == false && this.control.UIIsEnable == 1) {
            inputHtml += "<button class='mui-btn mui-btn-numbox-plus' type='button'>+</button>";
            inputHtml += "</div>";
        }
        return inputHtml;
    },
    CreateTBFloat: function (Ctrl_Val) {
        var dataInfo = "";
        var minNum = GetPara(this.control.AtPara, "NumMin") || "";
        var maxNum = GetPara(this.control.AtPara, "NumMax") || "";
        var dataInfo = "";
        if (minNum != "")
            dataInfo = " data-numbox-min='" + minNum + "'";
        if (maxNum != "")
            dataInfo += " data-numbox-max='" + maxNum + "'";
        var bit;
        var defVal = this.control.DefValType;
        if (defVal != null && defVal !== "" && defVal.toString().indexOf(".") >= 0)
            bit = attrdefVal.substring(defVal.indexOf(".") + 1).length;
        else
            bit = 2;

        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var inputHtml = "";
        if (Form_ReadOnly == false && this.control.UIIsEnable == 1) {
            var step = GetPara(this.control.AtPara, "NumStepLength");
            step = step == null || step == undefined ? 0.1 : parseFloat(step);
            inputHtml += "<div class='mui-numbox' data-numbox-step='"+step+"' data-numbox-bit='" + bit + "' "+dataInfo+" style='width:200px'>";
            inputHtml += "<button class='mui-btn mui-btn-numbox-minus' type='button'>-</button>";
            this.Ctrl_Class = " class='mui-numbox-input'";
        }
        if (defVal == 0)
            inputHtml += "<input " + this.Ctrl_Class + " type=\"number\"  name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        else
            inputHtml += "<input " + this.Ctrl_Class + " type=\"number\"  name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" placeholder=\"0.00\" value=\"" + Ctrl_Val + "\" />"
        if (Form_ReadOnly == false && this.control.UIIsEnable == 1) {
            inputHtml += "<button class='mui-btn mui-btn-numbox-plus' type='button'>+</button>";
            inputHtml += "</div>";
        }

        return inputHtml;
    },
    CreateTBDate: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var LAB_Id = "LAB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var Ctrl_Text = Ctrl_Val;
        if (this.Enable == false) {
            return "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datebox\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        }
        var inputHtml = "";
        if (Ctrl_Val == "") {
            Ctrl_Text = "<p>请选择日期</p>";
        }
        inputHtml += "<a class='mui-navigate-right'>";
        inputHtml += "  <span name=\"" + LAB_Id + "\" id=\"" + LAB_Id + "\" data-options='{\"type\":\"date\"}' class='mui-pull-right ccformdatedtl' style='margin-right:30px;margin-top:10px;min-width:160px;text-align:right;'>" + Ctrl_Text + "</span>";
        inputHtml += "</a>";
        inputHtml += "<input  type='hidden' name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value='" + Ctrl_Val + "' />";
        return inputHtml;
    },
    CreateTBDateTime: function (Ctrl_Val) {
        var isSupperText = this.control.IsSupperText;
        var optionType = "datetime";
        if (isSupperText == 0) {
            optionType = "date";//yyyy-MM-dd
        } else if (isSupperText == 1 || isSupperText == 5) {
            optionType = "date-time";//yyyy-MM-dd HH:mm
        } else if (isSupperText == 2) {
            optionType = "datetime";//yyyy-MM-dd HH:mm:ss
        } else if (isSupperText == 3) {
            optionType = "month";//yyyy-MM
        } else if (isSupperText == 4) {
            optionType = "time-min";//HH:mm
        } else if (isSupperText == 5) {
            optionType = "time";//HH:mm:ss
        } else if (isSupperText == 6) {
            optionType = "month-day";//MM-dd
        } else if (isSupperText == 7) {
            options.type = "year";//MM-dd
        }
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var LAB_Id = "LAB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Text = Ctrl_Val;
        if (this.Enable == false) {
            return "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datetimebox\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        }
        var inputHtml = "";
        if (Ctrl_Val == "") {
            Ctrl_Text = "<p>请选择时间</p>";
        }
        inputHtml += "<a class='mui-navigate-right'>";
        inputHtml += "  <span name=\"" + LAB_Id + "\" id=\"" + LAB_Id + "\" data-options='{\"type\":\"" + optionType + "\"}' class='mui-pull-right ccformdatedtl' style='margin-right:30px;margin-top:10px;min-width:160px;text-align:right;'>" + Ctrl_Text + "</span>";
        inputHtml += "</a>";
        inputHtml += "<input  type='hidden' name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value='" + Ctrl_Val + "' />";
        return inputHtml;
    },
    CreateTBTime: function (Ctrl_Val) {
        var Ctrl_Id = "TB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var LAB_Id = "LAB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        //var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Text = Ctrl_Val;
        if (this.Enable == false) {
            return "<input " + this.Ctrl_Class + " type=\"text\" data-role=\"datetimebox\" name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value=\"" + Ctrl_Val + "\" />";
        }
        var inputHtml = "";
        if (Ctrl_Val == "") {
            Ctrl_Text = "<p>请选择时间</p>";
        }
        inputHtml += "<a class='mui-navigate-right'>";
        inputHtml += "  <span name=\"" + LAB_Id + "\" id=\"" + LAB_Id + "\" data-options='{\"type\":\"time\"}' class='mui-pull-right ccformdatedtl' style='margin-right:30px;margin-top:10px;min-width:160px;text-align:right;'>" + Ctrl_Text + "</span>";
        inputHtml += "</a>";
        inputHtml += "<input  type='hidden' name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" value='" + Ctrl_Val + "' />";
        return inputHtml;
    },
    CreateCBBoolean: function (Ctrl_Val) {
        var checkBoxVal = "";
        var keyOfEn = this.control.KeyOfEn;
        var Ctrl_Id = "CB_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var CB_Html = "";
        CB_Html += "  <input type='hidden'  id='" + Ctrl_Id + "' name='" + Ctrl_Id + "' value='" + checkBoxVal + "'/>";
        var classVal = "";
        if (Ctrl_Val == 1)
            classVal = " mui-active";
        if (this.Enable == false )
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini  mui-disabled" + classVal + "' id='SW_" + keyOfEn + "_" + this.DataRow.OID + "'>";
        else
            CB_Html += "  <div class='mui-switch mui-switch-blue mui-switch-mini" + classVal + "' id='SW_" + keyOfEn + "_" + this.DataRow.OID + "'>";

        CB_Html += "      <div class='mui-switch-handle'></div>";
        CB_Html += "  </div>";

        return CB_Html;
    },
    CreateDDLEnum: function (Ctrl_Val) {
        //下拉框和单选都使用下拉框实现

        var Ctrl_Id = "RB_" + this.control.KeyOfEn + "_" + this.DataRow.OID
        if (this.control.UIContralType == UIContralType.DDL) {
            Ctrl_Id = "DDL_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        }

        //获取枚举数据
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", this.control.UIBindKey);

        var html_Select = "";
        html_Select += "<select name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" " + this.Ctrl_Class + ">";

        if (this.control.UIIsInput == 0)
            html_Select += "<option value='-1'>- 请选择 -</option>";

        for (var i = 0; i < enums.length; i++) {
            if (Ctrl_Val == enums[i].IntKey) {
                html_Select += "<option value=\"" + enums[i].IntKey + "\" selected='selected'>" + enums[i].Lab + "</option>";
            } else {
                html_Select += "<option value=\"" + enums[i].IntKey + "\">" + enums[i].Lab + "</option>";
            }
        }
        html_Select += "</select>";
        return html_Select;
    },
    CreateDDLPK: function (Ctrl_Val) {
        var args = new RequestArgs();
        // var Ctrl_Val = this.DataRow[this.control.KeyOfEn];
        var Ctrl_Id = "DDL_" + this.control.KeyOfEn + "_" + this.DataRow.OID;
        var dtl_OID = this.DataRow.OID;
        var isEnable = this.Enable == true ? 1 : 0;
        var WorkID = args.WorkID;
        var html_Select = "";

        html_Select += "<select name=\"" + Ctrl_Id + "\" id=\"" + Ctrl_Id + "\" " + this.Ctrl_Class + ">";
        if (isEnable == 0) {
            if (this.DataRow[this.control.KeyOfEn + "Text"] != null && this.DataRow[this.control.KeyOfEn + "Text"] != undefined)
                html_Select += "<option value=\"" + this.DataRow[this.control.KeyOfEn] + "\">" + this.DataRow[this.control.KeyOfEn + "Text"] + "</option>";
            else
                html_Select += "<option value=\"" + this.DataRow[this.control.KeyOfEn] + "\">" + this.DataRow[this.control.KeyOfEn + "T"] + "</option>";
        }
        var pushData = mainData[this.control.UIBindKey];
        //获取外键表数据,不存在表及sql方法
        var sfTable = new Entity("BP.Sys.SFTable", this.control.UIBindKey);
        if (sfTable != null && sfTable != "") {
            var selectStatement = sfTable.SelectStatement;
            var srcType = sfTable.SrcType;
            //Handler 获取外部数据源
            if (srcType == 5)
                pushData = DBAccess.RunDBSrc(selectStatement, 1);
            //JavaScript获取外部数据源
            if (srcType == 6)
                pushData = DBAccess.RunDBSrc(sfTable.FK_Val, 2);

        }
        if (pushData != null) {
            if (this.control.UIIsInput == 0)
                if (this.control.LGType == 1)
                    html_Select += "<option value='-1'>- 请选择 -</option>";
                else
                    html_Select += "<option value=''>- 请选择 -</option>";

            for (var i = 0; i < pushData.length; i++) {
                if (Ctrl_Val == pushData[i].No) {
                    html_Select += "<option value=\"" + pushData[i].No + "\" selected='selected'>" + pushData[i].Name + "</option>";
                } else {
                    html_Select += "<option value=\"" + pushData[i].No + "\">" + pushData[i].Name + "</option>";
                }
            }
        }
        html_Select += "</select>";
        return html_Select;
    }
}
//必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
function checkDtlBlanks() {
    var checkBlankResult = true;
    //获取所有的列名 找到带* 的LABEL mustInput

    var lbs = $('.mustInputDtl');
    $.each(lbs, function (i, obj) {
        //if ($(obj).parent().css('display') != 'none') {

        var ele = $(obj).parent().parent().siblings("input");
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
                case "A":

                    break;
            }
        }
        //}
    });

    return checkBlankResult;
}

//处理 MapExt 的扩展. 工作处理器，独立表单都要调用他.
function AfterBindDtl_DealMapExt(oid) {
    var mapExts = dtlMapExt;
    var mapAttrs = dtlmapAttrs;
    // 主表扩展(统计从表)
    var detailExt = {};
    wxh: for (var i = 0; i < mapExts.length; i++) {
        var mapExt1 = mapExts[i];

        //一起转成entity.
        var mapExt = new Entity("BP.Sys.MapExt", mapExt1);
        mapExt.MyPK = mapExt1.MyPK;
        if (mapExt.ExtType == "FullDataDtl")
            continue;
        var mapAttr1 = null;
        for (var j = 0; j < mapAttrs.length; j++) {

            if (mapAttrs[j].FK_MapData == mapExt.FK_MapData && mapAttrs[j].KeyOfEn == mapExt.AttrOfOper) {
                if (mapAttrs[j].UIIsEnable == 0 && (mapExt.ExtType == 'PopBranchesAndLeaf' || mapExt.ExtType == 'PopBranches' || mapExt.ExtType == 'PopTableSearch' || mapExt.ExtType == 'PopGroupList'))
                    continue wxh;//如果控件只读，并且是pop弹出形式，就continue到外循环
                mapAttr1 = mapAttrs[j];
                break;
            }
        }
        if (mapAttr1 == null) {
            continue;
        }

        if (isLoadJs.isLoadPop == false) {

            isLoadJs.isLoadPop = true;
            Skip.addJs(urlPrefix + "JS/Pop.js?t=" + Math.random());

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

        var mapAttr = new Entity("BP.Sys.MapAttr", mapAttr1);
        mapAttr.MyPK = mapAttr1.MyPK;

        //处理Pop弹出框
        var PopModel = mapAttr.GetPara("PopModel");

        if (PopModel != "" && mapAttr.GetPara("PopModel") == "None" && mapExt.ExtType.indexOf("Pop") >= 0)
            continue;

        if (PopModel != undefined && PopModel != "" && mapExt.ExtType == mapAttr.GetPara("PopModel")) {
            PopMapDtlExt(mapAttr, mapExt, oid);
            continue;
        }


        //处理文本自动填充
        var TBModel = mapAttr.GetPara("TBFullCtrl");
        if (TBModel != undefined && TBModel != "" && TBModel != "None" && (mapExt.ExtType == "FullData")) {
            var tbAuto = $("#TB_" + mapExt.AttrOfOper + "_" + oid);
            if (tbAuto.length == 0)
                continue;
            if (isLoadJs.isLoadTBFullCtrl == false) {
                isLoadJs.isLoadTBFullCtrl = true;
                loadScript(urlPrefix + "TBFullCtrl.js?t=" + Math.random());
            }
            TBFullCtrl(mapExt, mapAttr, "TB_" + mapExt.AttrOfOper + "_" + oid, 1);
            continue

        }

        //下拉框填充其他控件
        var DDLFull = mapAttr.GetPara("IsFullData");
        if (DDLFull != undefined && DDLFull != "" && DDLFull == "1" && (mapExt.MyPK.indexOf("DDLFullCtrl") != -1)) {
            /* //枚举类型
             if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
                 var ddlOper = $('input:radio[name="RB_' + mapExt.AttrOfOper + "_" + oid + '"]');
                 if (ddlOper.length == 0)
                     continue;
 
 
                 ddlOper.attr("onchange", "DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "_" + oid + "\', \'" + mapExt.MyPK + "\')");
 
                 //初始化填充数据
                 var val = $('input:radio[name="RB_' + mapExt.AttrOfOper + "_" + oid + '"]:checked').val();
                 DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper + "_" + oid, mapExt.MyPK);
 
 
                 continue;
             }*/

            //外键类型
            var ddlOper = $("#DDL_" + mapExt.AttrOfOper + "_" + oid);
            if (ddlOper.length == 0)
                continue;

            ddlOper.attr("onchange", "DDLFullCtrl(this.value,\'" + "DDL_" + mapExt.AttrOfOper + "_" + oid + "\', \'" + mapExt.MyPK + "\',1)");
            //初始化填充数据
            var val = ddlOper.val();
            if (val != "" && val != undefined)
                DDLFullCtrl(val, "DDL_" + mapExt.AttrOfOper + "_" + oid, mapExt.MyPK, 1);
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

            case "BindFunction": //控件绑定函数.

                if ($('#TB_' + mapExt.AttrOfOper + '_' + oid).length == 1) {
                    $('#TB_' + mapExt.AttrOfOper + '_' + oid).bind(DynamicBind(mapExt, "TB_", oid));
                    break;
                }
                if ($('#DDL_' + mapExt.AttrOfOper + '_' + oid).length == 1) {
                    $('#DDL_' + mapExt.AttrOfOper + '_' + oid).bind(DynamicBind(mapExt, "DDL_"));
                    break;
                }
                if ($('#CB_' + mapExt.AttrOfOper + '_' + oid).length == 1) {
                    $('#CB_' + mapExt.AttrOfOper + '_' + oid).bind(DynamicBind(mapExt, "CB_"));
                    break;
                }
                if ($('#RB_' + mapExt.AttrOfOper + '_' + oid).length == 1) {
                    $('#RB_' + mapExt.AttrOfOper + '_' + oid).bind(DynamicBind(mapExt, "RB_"));
                    break;
                }
                break;
            case "RegularExpression": //正则表达式  统一在保存和提交时检查

                var tb = $('#TB_' + mapExt.AttrOfOper + '_' + oid);

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

            case "ActiveDDL": /*自动初始化ddl的下拉框数据. 下拉框的级联操作 已经 OK*/
                var ddlPerant = $("#DDL_" + mapExt.AttrOfOper + "_" + oid);
                var ddlChild = $("#DDL_" + mapExt.AttrsOfActive + "_" + oid);
                if (ddlPerant == null || ddlChild == null)
                    continue;

                ddlPerant.attr("onchange", "DDLAnsc(this.value,\'" + "DDL_" + mapExt.AttrsOfActive + "_" + oid + "\', \'" + mapExt.MyPK + "\',\'" + ddlPerant.val() + "\')");

                var valClient = ConvertDefVal(frmData, '', mapExt.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                //初始化页面时方法加载

                DDLAnsc(ddlPerant.val(), "DDL_" + mapExt.AttrsOfActive + "_" + oid, mapExt.MyPK);

                break;
            case "AutoFullDLL": // 自动填充下拉框.
                continue; //已经处理了。
            case "AutoFull": //自动填充  //a+b=c DOC='@DanJia*@ShuLiang'  等待后续优化
                //循环  KEYOFEN
                //替换@变量
                //处理 +-*%

                if (mapExt.Doc == undefined || mapExt.Doc == '')
                    continue;
                calculator(mapExt, oid);
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
                        "exp": docs[2],
                        "Tag": mapExt.Tag,
                        "Tag1": mapExt.Tag1
                    };
                    if (!$.isArray(detailExt[ext.DtlNo])) {
                        detailExt[ext.DtlNo] = [];
                    }
                    detailExt[ext.DtlNo].push(ext);
                    var iframeDtl = $("#F" + ext.DtlNo);
                    iframeDtl.load(function () {
                        $(this).contents().find(":input[id=formExt]").val(JSON.stringify(detailExt[ext.DtlNo]));
                        if (this.contentWindow && typeof this.contentWindow.parentStatistics === "function") {
                            this.contentWindow.parentStatistics(detailExt[ext.DtlNo]);
                        }
                    });
                    $(":input[name=TB_" + ext.AttrOfOper + "]").attr("disabled", true);
                }
                break;

        }
    }
}

/**Pop弹出框的处理**/
function PopMapDtlExt(mapAttr, mapExt, oid) {
    switch (mapAttr.GetPara("PopModel")) {

        case "PopBranchesAndLeaf": //树干叶子模式.
            if (isLoadJs.isLoadBranchesAndLeaf == false) {
                isLoadJs.isLoadBranchesAndLeaf = true;
                loadScript(urlPrefix + "BranchesAndLeaf.js?t=" + Math.random());

            }
            Dtl_PopBranchesAndLeaf(mapExt, mapAttr, oid, 1); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopBranches": //树干简单模式. 
            if (isLoadJs.isLoadBranches == false) {
                isLoadJs.isLoadBranches = false;
                loadScript(urlPrefix + "Branches.js?t=" + Math.random());
            }

            Dtl_PopBranches(mapExt, mapAttr, oid, 1); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopGroupList": //分组模式.
            PopGroupList(mapExt, mapAttr, oid, 1); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        case "PopSelfUrl": //自定义url.
            SelfUrl(mapExt, mapAttr, oid, 1); //调用 /CCForm/JS/MultipleChoiceSmall.js 的方法来完成.
            break;
        case "PopTableSearch": //表格查询.
            Dtl_PopTableSearch(mapExt, mapAttr, oid, 1); //调用 /CCForm/JS/Pop.js 的方法来完成.
            break;
        default:
            break;
    }
}

/******************************************  树干枝叶模式 **********************************/
function Dtl_PopBranchesAndLeaf(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;
    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();

    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#branchesAndLeaf");
            initBranchesLPage(mapExt, OID, attrOfOper, 1);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }

    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");

    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(attrOfOper, OID, record.No);
            var mtags = $("#" + ctrlID + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType);
            }
            console.log("unselect: " + JSON.stringify(record));
        }
    });

    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, OID, mapAttr);
    return;
}

/******************************************  树干模式 **********************************/
function Dtl_PopBranches(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;

    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();
    var oid = GetPKVal();
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#branches");
            initBranchesPage(mapExt, OID, attrOfOper, 1);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");

    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
            DeleteFrmEleDB(attrOfOper, OID, record.No);

            var mtags = $("#" + attrOfOper + "_mtags i");
            var len = mtags.length;
            var RemoveFunc = mapExt.GetPara("RemoveFunc");

            if (RemoveFunc) {
                if (RemoveFunc.indexOf("(") == -1) {
                    RemoveFunc = RemoveFunc + "('" + record.No + "','" + len + "')";
                } else {
                    var para = record.No + "','" + len;
                    RemoveFunc = replaceAll(RemoveFunc, "Key", para);
                    RemoveFunc = replaceAll(RemoveFunc, "~", "'");
                }
                //调用移除函数
                DBAccess.RunDBSrc(RemoveFunc, mapExt.DBType);
            }
        }
    });
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, OID, mapAttr);
}

/******************************************  表格查询 **********************************/
function Dtl_PopTableSearch(mapExt, mapAttr, OID) {
    if (mapAttr.UIVisible == 0) {
        return;
    }
    var attrOfOper = mapExt.AttrOfOper + "_" + OID;
    var ctrlID = "TB_" + mapExt.AttrOfOper + "_" + OID;

    var target = $("#" + ctrlID);
    target.hide();
    var parentTarget = target.parent();
    var oid = OID;
    if (mapAttr.UIIsEnable != 0 && mapAttr.UIVisible != 0) {
        //增加a标签
        var aLink = $('<a class="mui-navigate-right"></a>');
        aLink.on('tap', function () {
            viewApi.go("#tableSearch");
            initTableSPage(mapExt, oid, attrOfOper, 1);
        });
        aLink.append($('<p style="margin-right:30px;margin-top:10px;min-width:160px;text-align:right;">请选择</p>'));
        parentTarget.append(aLink);
    }
    //增加显示选择结果行
    var width = target.width();
    var height = target.height();
    var container = $("<div class='mui-table-view-cell'></div>");
    parentTarget.after(container);
    container.attr("id", attrOfOper + "_mtags");


    $("#" + attrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            DeleteFrmEleDB(attrOfOper, oid, record.No);
        }
    });
    //初始加载
    Refresh_Mtags(mapExt.FK_MapData, attrOfOper, oid, mapAttr);

}

function GetNumberMinMax(mapAttr) {
    var isEnableNumEnterLimit = GetPara(mapAttr.AtPara, "IsEnableNumEnterLimit");
    isEnableNumEnterLimit = isEnableNumEnterLimit == null || isEnableNumEnterLimit == "" || isEnableNumEnterLimit == undefined || isEnableNumEnterLimit == "0" ? false : true;
    if (isEnableNumEnterLimit == false)
        return [];

    var min = 0, max = 0;
    if (isEnableNumEnterLimit == true) {
        min = GetPara(mapAttr.AtPara, "MinNum");
        if (min == undefined || min == "")
            min = 0;
        max = GetPara(mapAttr.AtPara, "MaxNum");
        if (max == undefined || max == "")
            max = 100000;
        return [min, max];
    }
}