/**  
 * 解析字段通用方法
 * @param {any} mainTable  字段表单的数据
 * @param {any} mapAttr 字段属性
 * @param {any} dataSet 数据返回的结果集
 */

function InitAthMapAttrOfCtrlFool(mainTable, mapAttr,dataSet) {
    var defValue = "";
    if (mainTable!=null)
        defValue = GetPara(mapAttr.Name, mainTable)
    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        if (mapAttr.UIIsEnable == 0) {
            var ctrl = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type=hidden  class='form-control' type='text'/>";
            defValue = ConvertDefVal(mainTable, mapAttr.DefVal, mapAttr.KeyOfEn + "T");

            if (defValue == '' || defValue == null)
                defValue = '无';

            ctrl += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "Text'  value='" + defValue + "' disabled='disabled'   type='text'/>";
            return ctrl;
        }


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue,dataSet) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        var data = dataSet[mapAttr.UIBindKey];
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue,dataSet) + "</select>";
    }

    //添加文本框 ，日期控件等.
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
            return "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'   type='text'/>";

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容                
                eleHtml += "<div class='richText' style='width:99%;margin-right:2px'>" + defValue + "</div>";

            } else {
                document.BindEditorMapAttr = mapAttr; //存到全局备用

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                styleText += "height:" + mapAttr.UIHeight + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                eleHtml += "<script id='editor'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + "  style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input type='text' " + enableAttr + " value='" + defValue + "' style='width:120px;' class='form-control' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input  type='text'  value='" + defValue + "' style='width:140px;' class='form-control' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
    }

    // boolen 类型.
    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        return "<label ><input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> </label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";
        if (mapAttr.UIContralType == 1)
        //return "<select " + enableAttr + "  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control' >" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
            return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitAthDDLOperation(mapAttr, defValue, dataSet) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContent(mapAttr, defValue, RBShowModel, enableAttr);

        }
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;'class='form-control'  onkeyup=" + '"' + "if(isNaN(value)) execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        //alert(defValue);

        return "<input  value='0' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}

/**
 * 初始化下拉列表框的OPERATION
 * @param {any} mapAttr
 * @param {any} defVal
 * @param {any} dataSet
 */
function InitAthDDLOperation(mapAttr, defVal, dataSet) {

    var operations = '';
    var data = dataSet[mapAttr.KeyOfEn];
    if (data == undefined)
        data = dataSet[mapAttr.UIBindKey];
    if (data == undefined) {
        //枚举类型的.
        if (mapAttr.LGType == 1) {
            var enums = dataSet.Sys_Enum;
            enums = $.grep(enums, function (value) {
                return value.EnumKey == mapAttr.UIBindKey;
            });


            $.each(enums, function (i, obj) {
                operations += "<option " + (obj.IntKey == mapAttr.DefVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
            });
        }
        return operations;
    }
    $.each(data, function (i, obj) {
        operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
    });
    return operations;
}
/**
 * 解析单选按钮
 * @param {any} mapAttr
 * @param {any} defValue
 * @param {any} RBShowModel
 * @param {any} enableAttr
 */
function InitRBShowContent(mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = new Entities("BP.Sys.SysEnums");
    enums.Retrieve("EnumKey", mapAttr.UIBindKey);
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        if (RBShowModel == 3)
        //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'  />&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}