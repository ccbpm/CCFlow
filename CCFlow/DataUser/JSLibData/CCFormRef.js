
/*
1. 该页面,是被引用到 /WF/MyFlowGener.htm, /WF/CCForm/FrmGener.htm 里面的.
2. 这里方法大多是执行后，返回json ,可以被页面控件调用. 
*/
function funDemo() {
    alert("我被执行了。");
    return;
}


//FK_MapData,附件属性，RefPK,FK_Node
function afterDtlImp(FK_MapData, frmAth, newOID, FK_Node, oldOID,oldFK_MapData) {
    //处理从表附件导入的事件.

}

function CompareData() {
    if ($("#TB_StartTime").val() > $("#TB_EndTime").val()) {
        alert("开始时间不能大于结束时间");
        return false;
    }
    return true;
}

function HeJi() {
    var bmhj = $("#BMHJ").val();
    var jthj = $("#JTHJ").val();
    var bzhj = $("#BZHJ").val();
    $("#HeJj").val(bmhj + jthj + bzhj);


}

function GetShiJian(keyOfEn) {
    alert($("#TB_" + keyOfEn + "_0").val());
}


function IsSaveDtl() {
    var regInput = true;
    //获取页面的所有IFrame
    var frames = $("#divCCForm").find("iframe");
    var dtlFrames = $.grep(frames, function (frame,idx) {
        if (frame.id.indexOf("Dtl_") == 0)
            return frame;
    });

    //循环从表IFrame，如果有未填的返回false
    $.each(dtlFrames, function (idx, dtlFrame) {
        var mustInput = $(this).contents().find(".errorInput");

        if (mustInput.length > 0) {
            regInput = false;
            return;
        }
    });

return regInput;
}    
/***  计算两个请假天数 begin ***/
function DateDiffExt() {

    var d1 = $("#TB_QJSJQ").val();

    d1 = d1.substring(0, 10);
    var d2 = $("#TB_QJSJZ").val();
    d2 = d2.substring(0, 10);

    var days = DateDiff(d1, d2);
    //请假天数.
    $("#TB_QJTS").val(days);

}

function DateDiff(date1, date2) {

    var regexp = /^(\d{1,4})[-|\.]{1}(\d{1,2})[-|\.]{1}(\d{1,2})$/;
    var monthDays = [0, 3, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1];
    regexp.test(date1);
    var date1Year = RegExp.$1;
    var date1Month = RegExp.$2;
    var date1Day = RegExp.$3;

    regexp.test(date2);
    var date2Year = RegExp.$1;
    var date2Month = RegExp.$2;
    var date2Day = RegExp.$3;

    if (validatePeriod(date1Year, date1Month, date1Day, date2Year, date2Month, date2Day)) {

        firstDate = new Date(date1Year, date1Month, date1Day);
        secondDate = new Date(date2Year, date2Month, date2Day);

        result = Math.floor((secondDate.getTime() - firstDate.getTime()) / (1000 * 3600 * 24));
        for (j = date1Year; j <= date2Year; j++) {
            if (isLeapYear(j)) {
                monthDays[1] = 2;
            } else {
                monthDays[1] = 3;
            }
            for (i = date1Month - 1; i < date2Month; i++) {
                result = result - monthDays[i];
            }
        }
        return result;
    }

    alert('对不起第一个时间必须小于第二个时间，谢谢！');

}

//判断是否为闰年
function isLeapYear(year) {
    if (year % 4 == 0 && ((year % 100 != 0) || (year % 400 == 0))) {
        return true;
    }
    return false;
}
//判断前后两个日期
function validatePeriod(fyear, fmonth, fday, byear, bmonth, bday) {
    if (fyear < byear) {
        return true;
    } else if (fyear == byear) {
        if (fmonth < bmonth) {
            return true;
        } else if (fmonth == bmonth) {
            if (fday <= bday) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    } else {
        return false;
    }
}

/***  计算两个请假天数 end ***/