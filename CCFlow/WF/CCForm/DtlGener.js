
/**
 * 获得控件的值，不管是cb,tb,ddl 都可以获取到.
 * @param {any} ctrlID
 */

//获得控件的值.
function ReqDtlCtrlVal(ctrlID) {

    var ctrl = ReqDtlCtrl(ctrlID);
    if (ctrl == null || ctrl == undefined || ctrl.length == 0) {
        alert("列名错误:" + ctrlID);
        return "";
    }
    var val = ctrl.val();
    return val;
}

/**
 * 获得控件, 不需要加前缀, 不需要idx字段。
 * @param {控件ID,比如:XingMing } ctrlID
 */
//获得控件.
function ReqDtlCtrl(ctrlID) {

    var ctrl = $("#TB_" + ctrlID + "_" + curRowIndex);

    if (ctrl.length == 0)
        ctrl = $("#DDL_" + ctrlID + "_" + curRowIndex);
    else
        return ctrl;

    if (ctrl.length == 0)
        ctrl = $("#CB_" + ctrlID + "_" + curRowIndex);
    else
        return ctrl;

    return ctrl;
}

/**
 * 
 * @param {控件ID,比如:XingMing } ctrlID
 * @param {any} val
 */
//设置控件的值.
function SetDtlCtrlVal(ctrlID, val) {

    var ctrl = $("#TB_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length != 0) {
        $("#TB_" + ctrlID + "_" + curRowIndex).val(val);
        return;
    }

    ctrl = $("#DDL_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length != 0) {
        $("#DDL_" + ctrlID + "_" + curRowIndex).val(val);
        return;
    }

    ctrl = $("#CB_" + ctrlID + "_" + curRowIndex);
    if (ctrl.length == 0) {
        alert("执行方法： SetCtrlVal， 列名：" + ctrlID + " 不存在, val=" + val + ". 请F12检查是否正确.");
        return;
    }

    if (val >= 1 || val == true)
        ctrl.prop('checked', true);
    else
        ctrl.prop('checked', false);
    return;
}