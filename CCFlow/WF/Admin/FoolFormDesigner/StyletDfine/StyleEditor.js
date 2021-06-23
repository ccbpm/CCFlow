
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

function AddEventExt() {
    //文字大小
    $("#TBPara_font-size_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();
            var thispropertyval = $("#TBPara_font-size_Temp").val();
            if (changeobj) {
                var cssText = $("." + changeobj).attr("style") + "font-size:" + thispropertyval + $("#TBPara_font-size-unit_Temp").val() + " !important";
                $("." + changeobj).css("cssText", cssText);
         
            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
        }
    })

    //整体宽度
    $("#TBPara_selfbody-width_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();//要改变的对象
            var thispropertyval = $("#TBPara_selfbody-width_Temp").val();//文字大小的值
            if (changeobj) {//判断是否有选中对象
                var cssText = $("." + changeobj).attr("style") + "width:" + thispropertyval + $("#TBPara_selfbody-width-unit_Temp").val() + " !important";
                $("." + changeobj).css("cssText", cssText);

            } else {
                return false;
            }

        } else {
            $(this).val('');
            alert("请输入数字");
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
                var cssText = $("." + changeobj).attr("style") + "height:" + thispropertyval + $("#TBPara_selfbody-hight-unit_Temp").val() + " !important";
                $("." + changeobj).css("cssText", cssText);
            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
        }

    })
    
    //字体行高
    $("#TBPara_font-height_Temp").keyup(function () {
        var thisval = $(this).val();
        if (!isNaN(thisval)) {
            var changeobj = $('#DDL_Style').val();
            var thispropertyval = $("#TBPara_font-height_Temp").val();

            if (changeobj) {
                var cssText = $("." + changeobj).attr("style") + "line-height:" + thispropertyval + $("#TBPara_font-height-unit_Temp").val() + " !important";
                $("." + changeobj).css("cssText", cssText);

            } else {
                return false;
            }
        } else {
            $(this).val('');
            alert("请输入数字");
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
                var cssText = $("." + changeobj).attr("style");
                if (borderselect == "all") {
                    cssText += "border-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
                } else {
                    cssText += "border-" + borderselect + "-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
                }
                $("." + changeobj).css("cssText", cssText);
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
            var cssText = $("." + changeobj).attr("style");
            if (borderselect == "all") {
                cssText += "border-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
            } else {
                cssText += "border-" + borderselect + "-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
            }
            $("." + changeobj).css("cssText", cssText);
        } else {
            return false;
        }
    });
    $("#TBPara_border-which").change(function () {
        var changeobj = $('#DDL_Style').val();
        var thispropertyval = $("#TBPara_border-width_Temp").val();
        var borderselect = $('#TBPara_border-which').val();
        if (changeobj) {
            var cssText = $("." + changeobj).attr("style");
            if (borderselect == "all") {
                cssText += "border-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
            } else {
                cssText += "border-" + borderselect + "-width:" + thispropertyval + $("#TBPara_border-width-unit_Temp").val() + " !important";
            }
            $("." + changeobj).css("cssText", cssText);
        } else {
            return false;
        }
    });

  
}