
//处理字段值相加, 对于参数类型的数据有效.
function UnitCtrl(hidCtrl) {

    var f = $("#" + hidCtrl + "_Temp").val();
    var n = $("#" + hidCtrl + "-unit_Temp").val();

    $("#" + hidCtrl).val(f + " " + n);

    var val = $("#" + hidCtrl).val();
    // alert(val + " " + hidCtrl);

    var changeobj = $('#DDL_Style').val();//要改变的对象
    var thispropertyval = $("#TBPara_selfbody-width").val();//文字大小的值
    if (changeobj) {//判断是否有选中对象
        $("." + changeobj).css("width", thispropertyval + $("#TBPara_selfbody-width-unit").val());
    } else {
        return false;
    }
}

function DealHidCtrl() {

    UnitCtrl("TBPara_selfbody-width");
    UnitCtrl("TBPara_selfbody-hight");

    UnitCtrl("TBPara_font-height");
    UnitCtrl("TBPara_font-size");
    UnitCtrl("TBPara_border-width");
}
function AddEvent() {

    //绑定背景颜色.
    AddColorEvent("background-color");

    //绑定字体颜色.
    AddColorEvent("color", "color");

    //增加特殊的事件.
    AddEventExt();

}

function AddColorEvent(ctrlID, cssID) {

    if (cssID == undefined)
        cssID = ctrlID;

    //背景色控件
    $("#TBPara_" + ctrlID).colorpicker({
        fillcolor: true,
        success: function (o, color) {
            var changeobj = $('#DDL_Style').val();//要改变的对象.
            if (changeobj) { //判断是否有选中对象
                $("." + changeobj).css(cssID, color);
                //   $("." + changeobj).css("backgroundColor", color);
            }
        }
    });

}

function AddEventExt() {

    //边框颜色控件
    $("#TBPara_border-color").colorpicker({

        fillcolor: true,

        success: function (o, color) {
            var changeobj = $('#DDL_Style').val();//要改变的对象
            var borderselect = $('#TBPara_border-which').val();
            if (changeobj) { //判断是否有选中对象
                if (borderselect == 'all') {
                    $("." + changeobj).css("borderColor", color);
                } else {
                    $("." + changeobj).css("border-" + borderselect + "-color", color);
                }
            } else {
                return false;
            }
        }
    });

    // var thisproperty = $(this).attr('id').substring(7);//当前属性
    //文字大小
    $("#TBPara_font-size_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();
            var thispropertyval = $("#TBPara_font-size_Temp").val();
            if (changeobj) {
                $("." + changeobj).css("fontSize", thispropertyval + $("#TBPara_font-size-unit_Temp").val());
            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
        }
    })

    $("#TBPara_font-size-unit_Temp").change(function () {
        var changeobj = $('#DDL_Style').val();//要改变的对象
        var thispropertyval = $("#TBPara_font-size_Temp").val();//文字大小的值
        if (changeobj) {//判断是否有选中对象
            $("." + changeobj).css("fontSize", thispropertyval + $("#TBPara_font-size-unit_Temp").val());
        } else {
            return false;
        }
    })

    //整体宽度
    $("#TBPara_selfbody-width_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();//要改变的对象
            var thispropertyval = $("#TBPara_selfbody-width_Temp").val();//文字大小的值
            if (changeobj) {//判断是否有选中对象
                $("." + changeobj).css("width", thispropertyval + $("#TBPara_selfbody-width-unit_Temp").val());
            } else {
                return false;
            }

        } else {
            $(this).val('');
            alert("请输入数字");
        }
    })

    $("#TBPara_selfbody-width-unit_Temp").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_selfbody-width_Temp").val();
        if (changeobj) {
            $("." + changeobj).css("width", thispropertyval + $("#TBPara_selfbody-width-unit_Temp").val());
        } else {
            return false;
        }
    })
    //整体高度
    $("#TBPara_selfbody-hight_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();//要改变的对象
            var thispropertyval = $("#TBPara_selfbody-hight_Temp").val();//文字大小的值
            console.log($("#TBPara_selfbody-hight-unit_Temp").val());
            if (changeobj) {//判断是否有选中对象
                $("." + changeobj).css("height", thispropertyval + $("#TBPara_selfbody-hight-unit_Temp").val());
            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
        }

    })

    $("#TBPara_selfbody-hight-unit_Temp").change(function () {
        var changeobj = $('#DDL_Style').val();//要改变的对象
        var thispropertyval = $("#TBPara_selfbody-hight_Temp").val();//文字大小的值
        if (changeobj) { //判断是否有选中对象
            $("." + changeobj).css("height", thispropertyval + $("#TBPara_selfbody-hight-unit_Temp").val());
        } else {
            return false;
        }
    })
    //字体粗细
    $("#TBPara_font-weight").change(function () {
        var changeobj = $('#DDL_Style').val();//要改变的对象
        var thispropertyval = $("#TBPara_font-weight").val();//文字大小的值
        if (changeobj) {
            $("." + changeobj).css("fontWeight", thispropertyval);
        } else {
            return false;
        }
    })
    //字体样式
    $("#TBPara_font-style").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_font-style").val();
        if (changeobj) {
            $("." + changeobj).css("fontStyle", thispropertyval);
        } else {
            return false;
        }
    })
    //字体行高
    $("#TBPara_font-height_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();
            var thispropertyval = $("#TBPara_font-height_Temp").val();

            if (changeobj) {
                $("." + changeobj).css("line-height", thispropertyval + $("#TBPara_font-height-unit_Temp").val());
            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
        }

    })

    $("#TBPara_font-height-unit_Temp").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_font-height_Temp").val();
        if (changeobj) {
            $("." + changeobj).css("lineHeight", thispropertyval + $("#TBPara_font-height-unit_Temp").val());
        } else {
            return false;
        }
    })

    //字体款式
    $("#TBPara_font-family").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_font-family").val();
        if (changeobj) {
            $("." + changeobj).css("fontFamily", thispropertyval);
        } else {
            return false;
        }
    })

    //边框宽度
    $("#TBPara_border-width_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();
            var thispropertyval = $("#TBPara_border-width_Temp").val();
            var borderselect = $('#TBPara_border-which').val();
            if (changeobj) {
                if (borderselect == "all") {
                    $("." + changeobj).css("borderWidth", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
                } else {
                    $("." + changeobj).css("border-" + borderselect + "-width", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
                }

            } else {
                return false;
            }

        } else {
            $(this).val('');
            alert("请输入数字");
        }

    });

    $("#TBPara_border-width-unit_Temp").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_border-width_Temp").val();
        var borderselect = $('#TBPara_border-which').val();
        if (changeobj) {
            if (borderselect == "all") {
                $("." + changeobj).css("borderWidth", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
            } else {
                $("." + changeobj).css("border-" + borderselect + "-width", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
            }
        } else {
            return false;
        }
    });
    $("#TBPara_border-which").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_border-width_Temp").val();
        var borderselect = $('#TBPara_border-which').val();
        if (changeobj) {
            if (borderselect == "all") {
                $("." + changeobj).css("borderWidth", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
            } else {
                $("." + changeobj).css("border-" + borderselect + "-width", thispropertyval + $("#TBPara_border-width-unit_Temp").val());
            }
        } else {
            return false;
        }
    });

    //边框样式
    $("#DDLPara_border-style").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#DDLPara_border-style").val();
        if (changeobj) {
            console.log(thispropertyval);
            $("." + changeobj).css("borderStyle", thispropertyval);
        } else {
            return false;
        }
    })
}