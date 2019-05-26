
/*
1. 该页面,是被引用到 /WF/MyFlowGener.htm, /WF/CCForm/FrmGener.htm 里面的.
2. 这里方法大多是执行后，返回json ,可以被页面控件调用. 
*/
function funDemo() {
    alert("我被执行了。");
}

//FK_MapData,附件属性，RefPK,FK_Node
function afterDtlImp(FK_MapData, frmAth, newOID, FK_Node, oldOID,oldFK_MapData) {
    //处理从表附件导入的事件
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