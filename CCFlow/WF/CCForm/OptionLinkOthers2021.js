
/**
 * 傻瓜表单、累加表单联动枚举，外键，复选框其它控件
 * @param {any} mapAttr
 * @param {any} frmType
 */
function InitFoolLink(mapAttr, frmType) {
    var AtPara = mapAttr.AtPara;
    if (AtPara == "" || AtPara == null || AtPara == undefined || AtPara.indexOf('@IsEnableJS=1') == -1)
        return;

    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1 && mapAttr.UIIsEnable != 0) {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll(mapAttr.KeyOfEn, frmType);
        setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, selecedval, "");

    }
    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll(mapAttr.KeyOfEn, frmType);
        setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, selecedval, "");

    }

    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            if (mapAttr.UIContralType == 1) {
                /*启用了显示与隐藏.*/
                var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#Td_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                //初始化页面的值
                var nowKey = ddl.val();
                if (nowKey == null || nowKey == undefined || nowKey == "" || nowKey == -1)
                    return;

                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
            if (mapAttr.UIContralType == 3) {
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#Td_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                if (nowKey == null || nowKey == undefined || nowKey == "" || nowKey == -1)
                    return;
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
        }
    }

    //复选框
    if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
        //获取复选框的值
        if ($("#CB_" + mapAttr.KeyOfEn).checked == true)
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 1, frmType);
        else
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 0, frmType);
    }

}

/**
 * 开发者联动枚举，外键，复选框其它控件
 * @param {any} mapAttr
 * @param {any} frmType
 */
function InitDevelopLink(mapAttr, frmType) {
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {  // AppInt Enum
        if (mapAttr.AtPara && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
            if (mapAttr.UIContralType == 1) {
                /*启用了显示与隐藏.*/
                var ddl = $("#DDL_" + mapAttr.KeyOfEn);
                //如果现在是隐藏状态就不可以设置
                if (ddl.length > 0) {
                    if (ddl.css('display') == "none")
                        return;
                }
                //初始化页面的值
                var nowKey = ddl.val();
                if (nowKey == null || nowKey == undefined || nowKey == "")
                    return;

                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);

            }
            if (mapAttr.UIContralType == 3) {
                //如果现在是隐藏状态就不可以设置
                var ctrl = $("#SR_" + mapAttr.KeyOfEn);
                if (ctrl.length > 0) {
                    if (ctrl.parent('tr').css('display') == "none")
                        return;
                }

                var nowKey = $('input[name="RB_' + mapAttr.KeyOfEn + '"]:checked').val();
                if (nowKey == null || nowKey == undefined || nowKey == "" || nowKey == -1)
                    return;
                setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, nowKey, frmType);
            }
        }
    }

    //复选框
    if (mapAttr.MyDataType == 4 && mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
        //获取复选框的值
        if ($("#CB_" + mapAttr.KeyOfEn).checked == true)
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 1, frmType);
        else
            setEnable(mapAttr.FK_MapData, mapAttr.KeyOfEn, 0, frmType);
    }

}


//记录改变字段样式 不可编辑，不可见
var mapAttrs = {};

/**
 * 下拉框改变事件
 * @param {any} obj
 * @param {any} FK_MapData
 * @param {any} KeyOfEn
 * @param {any} AtPara
 * @param {any} frmType
 */
function changeEnable(obj, FK_MapData, KeyOfEn, AtPara, frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selecedval = $(obj).children('option:selected').val();  //弹出select的值.
        cleanAll(KeyOfEn);
        setEnable(FK_MapData, KeyOfEn, selecedval, frmType);
    }
}
/**
 * 枚举改变事件
 * @param {any} obj
 * @param {any} FK_MapData
 * @param {any} KeyOfEn
 * @param {any} AtPara
 * @param {any} frmType
 */
function clickEnable(obj, FK_MapData, KeyOfEn, AtPara, frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        var selectVal = $(obj).val();
        cleanAll(KeyOfEn);
        setEnable(FK_MapData, KeyOfEn, selectVal, frmType);
    }
}

/**
 * 复选框改变事件
 * @param {any} obj
 * @param {any} FK_MapData
 * @param {any} KeyOfEn
 * @param {any} AtPara
 * @param {any} frmType
 */
function changeCBEnable(obj, FK_MapData, KeyOfEn, AtPara, frmType) {
    if (AtPara.indexOf('@IsEnableJS=1') >= 0) {
        cleanAll(KeyOfEn);
        if (obj.checked == true)
            setEnable(FK_MapData, KeyOfEn, 1, frmType);
        else
            setEnable(FK_MapData, KeyOfEn, 0, frmType);
    }
}

/**
 * 清空设置
 * @param {any} KeyOfEn
 * @param {any} frmType
 */
function cleanAll(KeyOfEn, frmType) {
    var trs = $("#CCForm  div .FoolFrmGroupBar"); //如果隐藏就显示
    $.each(trs, function (i, obj) {
        if ($(obj).parent().is(":hidden") == true)
            $(obj).parent().show();

    });

    if (mapAttrs.length == 0)
        return;

    //获取他的值
    if (mapAttrs[KeyOfEn] != undefined && mapAttrs[KeyOfEn].length > 0) {
        var FKMapAttrs = mapAttrs[KeyOfEn][0];
        for (var i = 0; i < FKMapAttrs.length; i++) {
            if (frmType != null && frmType !== undefined && frmType == 8)
                SetDevelopCtrlShow(FKMapAttrs[i]);
            else
                SetCtrlShow(FKMapAttrs[i]);
            SetCtrlEnable(FKMapAttrs[i]);
            SetCtrlUnMustInput(FKMapAttrs[i]);
            CleanCtrlVal(FKMapAttrs[i]);
        }
        layui.form.render();
    }




}
//启用了显示与隐藏.
function setEnable(FK_MapData, KeyOfEn, selectVal, frmType) {
    var NDMapAttrs = [];
    var pkval = FK_MapData + "_" + KeyOfEn + "_" + selectVal;
    var frmRB = new Entity("BP.Sys.FrmRB");
    frmRB.SetPKVal(pkval);
    if (frmRB.RetrieveFromDBSources() == 0)
        return;

    var Script = frmRB.Script;
    //解析执行js脚本
    if (Script != null && Script != "" && Script != undefined)
        DBAccess.RunDBSrc(Script, 2);

    //提示信息未解析
    //解决字段隐藏显示.
    var cfgs = frmRB.FieldsCfg;

    //解决为其他字段设置值.
    var setVal = frmRB.SetVal;
    if (setVal) {
        var strs = setVal.split('@');

        for (var i = 0; i < strs.length; i++) {
            var str = strs[i];
            if (str == "")
                continue;
            var kv = str.split('=');

            var key = kv[0];
            var value = kv[1];
            SetCtrlVal(key, value);
            NDMapAttrs.push(key);

        }
        layui.form.render();
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
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlShow(key);
                else
                    SetCtrlShow(key);
                SetCtrlEnable(key);
                NDMapAttrs.push(key);
            }

            if (sta == 2) { //要设置为不可编辑.
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlShow(key);
                else
                    SetCtrlShow(key);
                SetCtrlUnEnable(key);
                NDMapAttrs.push(key);
            }

            if (sta == 3) { //不可见.
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlHidden(key);
                else
                    SetCtrlHidden(key);
                NDMapAttrs.push(key);
            }

            if (sta == 4) {  //要设置为可编辑且必填.
                if (frmType != null && frmType != undefined && frmType == 8)
                    SetDevelopCtrlShow(key);
                else
                    SetCtrlShow(key);
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

    //设置是否隐藏分组、获取字段分组所有的tr
    var trs = $("#CCForm  div .FoolFrmGroupBar");
    var isHidden = false;
    $.each(trs, function (i, obj) {
        //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
        var sibles = $(obj).nextAll();
        if (sibles.length == 0) {
            $(obj).hide();
            return true;
        }
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

    //$.each(trs, function (i, obj) {
    //    //获取所有跟随的同胞元素，其中有不隐藏的tr,就跳出循环
    //    var sibles = $(obj).parent().nextAll();
    //    for (var k = 0; k < sibles.length; k++) {
    //        var sible = $(sibles[k]);
    //        if (sible.find(".FoolFrmGroupBar").length > 0 || sible.find(".form-unit").length > 0)
    //            break;
    //        if (sible.is(":hidden") == false) {
    //            isHidden = false;
    //            break;
    //        }
    //        isHidden = true;
    //    }
    //    if (isHidden == true)
    //        $(obj).parent().hide();

    //});
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

//设置是否可用必填?
function SetCtrlMustInput(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        var layVerify = ctrl.attr("lay-verify")
        if (layVerify == undefined || layVerify == "")
            ctrl.attr("lay-verify", "required")
        else
            ctrl.attr("lay-verify", layVerify + "|required");
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        if (layVerify == undefined || layVerify == "")
            ctrl.attr("lay-verify", "required")
        else
            ctrl.attr("lay-verify", layVerify + "|required");
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.removeAttr("disabled");
        if (layVerify == undefined || layVerify == "")
            ctrl.attr("lay-verify", "required")
        else
            ctrl.attr("lay-verify", layVerify + "|required");
    }

    ctr = document.getElementsByName('RB_' + key);
    if (ctrl != null) {
        var ses = new Entities("BP.Sys.SysEnums");
        ses.Retrieve("EnumKey", key);
        for (var i = 0; i < ses.length; i++) {
            $("#RB_" + key + "_" + ses[i].IntKey).removeAttr("disabled");
            if (layVerify == undefined || layVerify == "")
                $("#RB_" + key + "_" + ses[i].IntKey).attr("lay-verify", "required")
            else
                $("#RB_" + key + "_" + ses[i].IntKey).attr("lay-verify", layVerify + "|required");
        }
           
    }
    if ($("#Lab_" + key).length == 0)
        $("#Span_" + key).append("<font color=red>*</font>");
    else
        $("#Lab_" + key).before("<span style='color: red' class='mustInput' data-keyofen='" + key + "'>*</span>");
    
}

//设置可不必填?
function SetCtrlUnMustInput(key) {

    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        var layVerify = ctrl.attr("lay-verify")
        if (layVerify != undefined && layVerify != "") {
            layVerify = layVerify.replace("|required", "").replace("required", "");
            ctrl.attr("lay-verify", layVerify)
        }
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        var layVerify = ctrl.attr("lay-verify")
        if (layVerify != undefined && layVerify != "") {
            layVerify = layVerify.replace("|required", "").replace("required", "");
            ctrl.attr("lay-verify", layVerify)
        }
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        var layVerify = ctrl.attr("lay-verify")
        if (layVerify != undefined && layVerify != "") {
            layVerify = layVerify.replace("|required", "").replace("required", "");
            ctrl.attr("lay-verify", layVerify)
        }
    }

    ctrl = $("#RB_" + key);
    if (ctrl != null) {
        var layVerify = $('input[name=RB_' + key + ']').attr("lay-verify")
        if (layVerify != undefined && layVerify != "") {
            layVerify = layVerify.replace("|required", "").replace("required", "");
            $('input[name=RB_' + key + ']').attr("lay-verify", layVerify)
        }
    }
    if ($("#Lab_" + key).prev().length>0)
        $("#Lab_" + key).prev().remove();
}


//设置隐藏?
function SetCtrlHidden(key) {
    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0)
        ctrl.parent().parent('.layui-row').css("display", "none");

    var ctrl = $("#TD_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "none");
    }

    //从表隐藏
    var ctrl = $("#Dtl_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "none");
        ctrl.parent().parent('.layui-row').prev().css("display", "none");
      
    }

    //附件隐藏
    var ctrl = $("#Div_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "none");
        ctrl.parent().parent('.layui-row').prev().css("display", "none");
    }
}
//设置显示?
function SetCtrlShow(key) {


    var ctrl = $("#TD_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "block");
    }

    ctrl = $("#Lab_" + key);
    if (ctrl.length > 0)
        ctrl.parent().parent('.layui-row').css("display", "block");

    //从表隐藏
    var ctrl = $("#Dtl_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "block");
        ctrl.parent().parent('.layui-row').prev().css("display", "block");
    }

    //附件隐藏
    var ctrl = $("#Div_" + key);
    if (ctrl.length > 0) {
        ctrl.parent().parent('.layui-row').css("display", "block");
        ctrl.parent().parent('.layui-row').prev().css("display", "block");
    }

}

//设置隐藏?
function SetDevelopCtrlHidden(key) {
    //从表隐藏
    var ctrl = $("#Fd" + key);
    if (ctrl.length > 0) {
        ctrl.hide();
        $("#Lab_" + key).hide();
        return;
    }
    //附件隐藏
    ctrl = $("#Div_" + key);
    if (ctrl.length > 0) {
        $("#Div_" + key).hide();
        $("#Lab_" + key).hide();
        return;
    }

    ctrl = $("#TB_" + key);
    if (ctrl.length == 0) {
        ctrl = $("#SS_FDBZQ");
        if (ctrl.length == 0)
            ctrl = $("#DDL_" + key);
    }
       
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
//设置显示?
function SetDevelopCtrlShow(key) {
    //从表显示
    var ctrl = $("#Fd" + key);
    if (ctrl.length > 0) {
        ctrl.show();
        $("#Lab_" + key).show();
        return;
    }
    //附件显示
    ctrl = $("#Div_" + key);
    if (ctrl.length > 0) {
        $("#Div_" + key).show();
        $("#Lab_" + key).show();
        return;
    }

    ctrl = $("#TB_" + key);
    if (ctrl.length == 0) {
        ctrl = $("#SS_FDBZQ");
        if (ctrl.length == 0)
            ctrl = $("#DDL_" + key);
    }
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
    $(parent).show();
    ctrl.show();

    //当前节点的兄弟节点，如果没有input，select,就隐藏
    var prev = $(parent).prev();
    if (prev.find("input").length == 0 && prev.find("select").length == 0)
        prev.show();

   
}


//设置值?
function SetCtrlVal(key, value) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        return;
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        ctrl.val(value);
        return;
    }


    ctrl = $("input[name='CB_" + key + "']");
    if (ctrl.length == 1) {
        if (parseInt(value) <= 0)
            ctrl.attr('checked', false);
        else {
            ctrl.attr('checked', true);
            document.getElementById("CB_" + key).checked = true;
        }

        return;
    }
    if (ctrl.length > 1) {
        var checkBoxArray = value.split(",");
        ctrl.attr("checked", false);

        for (var k = 0; k < checkBoxArray.length; k++) {
            if (checkBoxArray[k] == "")
                continue;
            document.getElementById("CB_" + key + "_" + checkBoxArray[k]).checked = true;
        }
        return;
    }
    ctrl = document.getElementsByName("RB_" + key);
    if (ctrl.length > 0) {
        for (var i = 0; i < ctrl.length; i++) {
            if (value == ctrl[i].value) {
                $(ctrl[i]).next().click();
            }

        }
    }
    //ctrl = $('input:radio[name=RB_' + key + ']');
    //if (ctrl.length > 0) {
    //    var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
    //    if (checkVal != null && checkVal != undefined)
    //        document.getElementById("RB_" + key + "_" + checkVal).checked = false;
    //    if ($("#RB_" + key + "_" + value).length == 1)
    //        document.getElementById("RB_" + key + "_" + value).checked = true;
    //    return;
    //}
}

//清空值?
function CleanCtrlVal(key) {
    var ctrl = $("#TB_" + key);
    if (ctrl.length > 0) {
        ctrl.val('');
        return;
    }

    ctrl = $("#DDL_" + key);
    if (ctrl.length > 0) {
        //ctrl.attr("value",'');
        ctrl.val('');
        return;
    }

    ctrl = $("#CB_" + key);
    if (ctrl.length > 0) {
        ctrl.attr('checked', false);
        return;
    }

    ctrl = $("#RB_" + key + "_" + 0);
    if (ctrl.length > 0) {
        var checkVal = $('input:radio[name=RB_' + key + ']:checked').val();
        if (checkVal != null && checkVal != undefined)
            document.getElementById("RB_" + key + "_" + checkVal).checked = false;
        return;
    }
}
