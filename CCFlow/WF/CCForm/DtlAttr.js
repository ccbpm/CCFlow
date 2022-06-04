//升级表单元素 初始化文本框、日期、时间
function figure_MapAttr_Template(mapAttr) {
    var eleHtml = '';
    //获取初始值
    var defValue = ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn, mapAttr.MyDataType);

    //隐藏字段的处理
    if (mapAttr.UIVisible == 0) {
        defValue = defValue == null || defValue == undefined ? "" : defValue;
        eleHtml= "<input type='hidden' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'></input>";
        eleHtml = $(eleHtml);
        //赋初始值
        eleHtml.val(defValue);
        
        return eleHtml;
    }

    //字段附件不解析
    if (mapAttr.UIContralType == 6) {
        layer.alert("字段附件不解析，请使用从表附件上传的功能");
        return "";
    }

    switch (mapAttr.LGType) {
        case 0: //普通值
            switch (mapAttr.MyDataType) {
                case 1:
                    if (mapAttr.UIContralType == "1") {
                        //外部数据源
                        eleHtml += "<select name='DDL_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn) + "' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + " onChange='ChangeValue(this)'>" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
                        eleHtml += "<input type='hidden'  name='TB_" + mapAttr.KeyOfEn + "' value='" + ConvertDefVal(workNodeData, mapAttr.DefVal, mapAttr.KeyOfEn + "T") + "' />";
                        //大文本
                    } else if (mapAttr.IsSupperText == "1") {
                        eleHtml += "<textarea id='TB_" + mapAttr.KeyOfEn + "' maxlength=" + mapAttr.MaxLen + " style='height:" + mapAttr.UIHeight + "px;' name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>";
                        //input字段
                    } else {
                        eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  name='TB_" + mapAttr.KeyOfEn + "' type='text' " + (mapAttr.UIIsEnable ? '' : ' disabled="disabled"') + "/>";
                    }
                    break;
                case 2://AppInt
                    var enableAttr = mapAttr.UIIsEnable == 0 ? "disabled='disabled'":"";
                    eleHtml += "<input style='text-align:right;' onblur='valitationAfter(this, \"int\")' onkeydown='valitationBefore(this, \"int\")' onkeyup=" + '"' + "valitationAfter(this, 'int'); if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'int'); if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                    break;
                case 3://AppFloat
                case 5://AppDouble
                    var enableAttr = mapAttr.UIIsEnable == 0 ? "disabled='disabled'" : "";
                    //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
                    var defVal = mapAttr.DefVal;
                    var bit;
                    if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
                        bit = defVal.substring(defVal.indexOf(".") + 1).length;

                    eleHtml += "<input  class='form-control' style='text-align:right;' onblur='valitationAfter(this, \"float\")' onkeydown='valitationBefore(this, \"float\")' onkeyup=" + '"' + "valitationAfter(this, 'float'); if(!(value.indexOf('-')==0&&value.length==1)&&isNaN(value))execCommand('undo');limitLength(this," + bit + ");" + '"' + " onafterpaste=" + '"' + "valitationAfter(this, 'float'); if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "'/>";
                    break;
                case 4://复选框
                    eleHtml += "<div style='white-space: nowrap;align:center;'><input style='text-align:center;' type='checkbox' name='CB_" + mapAttr.KeyOfEn + "' /></div>";
                    break;
                case 6://日期
                case 7://日期时间
                    var enableAttr = '';
                    var frmDate = mapAttr.IsSupperText;//获取日期格式
                    var dateFmt = '';
                    var style = "";
                    if (mapAttr.MyDataType == 6) {
                        style = "style='width:110px'";
                        dateFmt = "yyyy-MM-dd";
                    }
                      
                    if (mapAttr.MyDataType == 7) {
                        style = "style='width:145px'";
                        dateFmt = "yyyy-MM-dd HH:mm";
                    }

                    if (frmDate == 0) 
                        dateFmt = "yyyy-MM-dd";
                    if (frmDate == 1)
                        dateFmt = "yyyy-MM-dd HH:mm";
                    if (frmDate == 2)
                        dateFmt = "yyyy-MM-dd HH:mm:ss"
                    if (frmDate == 3)
                        dateFmt = "yyyy-MM";
                    if (frmDate == 4) 
                        dateFmt = "HH:mm";
                    if (frmDate == 5)
                        dateFmt = "HH:mm:ss"
                     if (frmDate == 6) 
                        dateFmt = "MM-dd";
                    if (frmDate == 7)
                        dateFmt = "yyyy";
                    

                    if (mapAttr.UIIsEnable == 1) {
                        enableAttr = 'onfocus="' + 'WdatePicker({dateFmt:' + '\'' + dateFmt + '\'});SetChange(false);"  onchange="SetChange(true);" ' + style;
                    } else {
                        enableAttr = "disabled='disabled'";
                    }
                    eleHtml += "<input maxlength=" + mapAttr.MaxLen + "  type='text' " + enableAttr + " name='TB_" + mapAttr.KeyOfEn + "' class='Wdate'/>";

                    break;
                case 8:
                    //获取DefVal,根据默认的小数点位数来限制能输入的最多小数位数
                    var defVal = mapAttr.DefVal;
                    var bit;
                    if (defVal != null && defVal !== "" && defVal.indexOf(".") >= 0)
                        bit = defVal.substring(defVal.indexOf(".") + 1).length;
                    eleHtml += "<input value='" + defValue + "' style='text-align:right;' class='form-control' onfocus='removeplaceholder(this," + bit + ");' onblur='addplaceholder(this," + bit + ");numberFormat (this, " + bit + ") ' onkeyup=" + '"' +
                        "limitLength(this," + bit + ");" + '"' +
                        " onafterpaste=" + '"' + "valitationAfter(this, 'money');if(isNaN(value))execCommand('undo');" + '"' +
                        " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' name='TB_" + mapAttr.KeyOfEn + "' value='0.00' placeholder='" + (mapAttr.Tip || '') + "'/>";

                    break;
                default:
                    break;
            }
            //外部数据源
            
            break;
        case 1://枚举
            //下拉框
            if (mapAttr.UIContralType == 1) {
                eleHtml += "<select name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + ">" + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
            }
            //复选框、枚举
            if (mapAttr.UIContralType == 2 || mapAttr.UIContralType == 3) {
                var ses = workNodeData[mapAttr.KeyOfEn];
                if (ses == undefined)
                    ses = workNodeData[mapAttr.UIBindKey];
                if (ses == undefined) {
                    var sess = workNodeData.Sys_Enum;
                    ses = $.grep(sess, function (value) {
                        return value.EnumKey == mapAttr.UIBindKey;
                    });
                }
                if (ses == undefined) {
                    layer.alert(MapAttr.Name + "字段没有获取到关联的枚举[" + mapAttr.UIBindKey + "]的集合");
                    return "";
                }

                var enableAttr = "";
                if (mapAttr.UIIsEnable == 1)
                    enableAttr = "";
                else
                    enableAttr = "disabled='disabled'";

                //显示方式,默认为横向展示.
                var RBShowModel = 0;
                if (mapAttr.AtPara.indexOf('@RBShowModel=0') > 0)
                    RBShowModel = 1;
                if (mapAttr.UIContralType == 2) {
                    $.each(ses, function (i, se) {
                        var br = "";
                        if (RBShowModel == 1)
                            br = "<br>";
                        var checked = "";
                        if (("," + defValue + ",").indexOf("," + se.No + ",") != -1)
                            checked = " checked=true";

                        eleHtml += "<input type=checkbox name='CB_" + mapAttr.KeyOfEn + "' id='CB_" + mapAttr.KeyOfEn + "_" + se.No + "' value='" + se.No + "' " + checked + enableAttr + "/><span>" + se.Name + "&nbsp;</span>" + br;
                    });

                }

                if (mapAttr.UIContralType == 3) {
                    $.each(enums, function (i, obj) {
                        var onclickEvent = "";
                        if (mapAttr.AtPara.indexOf('@IsEnableJS=1') >= 0) {
                            onclickEvent = "onclick='clickEnable( this ,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'";
                        }
                        if (RBShowModel == 3)
                            eleHtml += "<input " + enableAttr + " " + (obj.No == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.No + "' value='" + obj.No + "' " + onclickEvent + " /><label>&nbsp;" + obj.Name + "</label>";
                        else
                            eleHtml += "<input " + enableAttr + " " + (obj.No == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.No + "' value='" + obj.No + "' " + onclickEvent + "/><label>&nbsp;" + obj.Name + "</label><br/>";
                    });
                }
                
            }
            break;
        case 2://外键
            eleHtml += "<select  name='DDL_" + mapAttr.KeyOfEn + "' " + (mapAttr.UIIsEnable ? '' : 'disabled="disabled"') + "> " + InitDDLOperation(workNodeData, mapAttr, defValue) + "</select>";
            break;
        default:
            layer.alert("LGType=" + mapAttr.LGType + "类型的字段还没有解析");
            beak;


    }
    //字段必填
    eleHtml +=mapAttr.UIIsInput == 1 ? '<span style="color:red" class="mustInput" data-keyofen="' + mapAttr.KeyOfEn + '"></span>' : "";
       
    eleHtml = $(eleHtml);

    if ((mapAttr.MyDataType == "1" && mapAttr.UIContralType == "2"
        || (mapAttr.MyDataType == "2" && mapAttr.UIContralType == "3")) && mapAttr.LGType == 1) {
        defValue = "," + defValue + ",";
        $.each(eleHtml, function () {
            if ($(this).nodeName == "INPUT") {
                if (defValue.indexOf("," + $(this).val() + ",") != -1)
                    $(this).attr("checked", true);
                else
                    $(this).attr("checked", false);
            }

        });
    } else {
        if (mapAttr.MyDataType != "4" && mapAttr.UIContralType != "2") {
            if (mapAttr.MyDataType != 6 && mapAttr.MyDataType != 7)
                eleHtml.css('width', mapAttr.UIWidth);
        }
        else
            eleHtml.css('width', "auto");
        eleHtml.val(defValue);
    }



    eleHtml.bind('focus', blurEvent);
    if (eleHtml.find('[name^=CB_]').length == 1) { //CHECKBOX 处理
        eleHtml.find('[name^=CB_]').bind('focus', function (obj) { blurEvent(obj); });
    }
    if (mapAttr.UIIsEnable == "0") {
        if (eleHtml.find('[name^=CB_]').length == 1) { //CHECKBOX 处理
            eleHtml.find('[name^=CB_]').attr('disabled', true);
        } else {
            eleHtml.attr('disabled', true);
        }
    }
    return eleHtml;
}
//初始化下拉列表框的OPERATION
function InitDDLOperation(workNodeData, mapAttr, defVal) {
    var operations = '';
    var data = workNodeData[mapAttr.KeyOfEn];
    if (data == undefined)
        data = workNodeData[mapAttr.UIBindKey];

    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = workNodeData.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });


            $.each(enums, function (i, obj) {
                if (obj.IntKey == -1)
                    return false;
                operations += "<option " + (obj.IntKey == mapAttr.DefVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
        }
        return operations;

    }


    if (mapAttr.UIIsInput == 0)
        if (mapAttr.LGType == 1)
            operations = "<option value='-1'>- 请选择 -</option>" + operations;
        else
            operations = "<option value=''>- 请选择 -</option>" + operations;

    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });

    return operations;

}

//填充默认数据
function ConvertDefVal(workNodeData, defVal, keyOfEn, myDataType) {

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
    for (var ele in workNodeData.DBDtl[0]) {
        if (keyOfEn == ele && workNodeData.DBDtl[0][ele] != '') {
            result = workNodeData.DBDtl[0][ele];
            break;
        }
    }

    //通过URL参数传过来的参数
    for (var pageParam in pageParamObj) {
        if (pageParam == keyOfEn) {
            result = pageParamObj[pageParam];
            break;
        }
    }

    if (result != undefined && typeof (result) == 'string') {
        //result = result.replace(/｛/g, "{").replace(/｝/g, "}").replace(/：/g, ":").replace(/，/g, ",").replace(/【/g, "[").replace(/】/g, "]").replace(/；/g, ";").replace(/~/g, "'").replace(/‘/g, "'").replace(/‘/g, "'");
    }

    if (myDataType == 8)
        if (!/\./.test(result))
            result += '.00';
    return result = unescape(result);
}