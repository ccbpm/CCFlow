
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
//計算日期間隔
function CalculateRDT(StarRDT, EndRDT, RDTRadio) {

    var res = "";
    var demoRDT;
    demoRDT = StarRDT.split("-");
    StarRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);  //转换为yyyy-MM-dd格式
    demoRDT = EndRDT.split("-");
    EndRDT = new Date(demoRDT[0] + '-' + demoRDT[1] + '-' + demoRDT[2]);
    res = parseInt((EndRDT - StarRDT) / 1000 / 60 / 60 / 24); //把相差的毫秒数转换为天数
    res = res + 1;
    //判断结束日期是否早于开始日期
    if (parseInt(EndRDT / 1000 / 60 / 60 / 24) < parseInt(StarRDT / 1000 / 60 / 60 / 24)) {
        alert("结束日期不能早于开始日期");
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
                    alert("请假时间内均为节假日");
                    res = "";
                }
            }
        }
    }
    return res;

}

function GetMapExtsGroup(mapExts) {
    var map = {};
    var mypk = "";
    //对mapExt进行分组，根据AttrOfOper
    $.each(mapExts, function (i, mapExt) {
        //不是操作字段不解析
        if (mapExt.AttrOfOper == "")
            return true;
        if (mapExt.ExtType == "DtlImp"
            || mapExt.MyPK.indexOf(mapExt.FK_MapData + '_Table') >= 0
            || mapExt.MyPK.indexOf('PageLoadFull') >= 0
            || mapExt.ExtType == 'StartFlow'
            || mapExt.ExtType == 'AutoFullDLL'
            || mapExt.ExtType == 'ActiveDDLSearchCond'
            || mapExt.ExtType == 'AutoFullDLLSearchCond')
            return true;

        mypk = mapExt.FK_MapData + "_" + mapExt.AttrOfOper;

        /*if (isFirstXmSelect == true) {
            layui.config({
                base: laybase + 'Scripts/layui/ext/'
            });
            isFirstXmSelect = false;
        }*/
        if (!map[mypk])
            map[mypk] = [mapExt];
        else
            map[mypk].push(mapExt);
    });
    var res = [];
    Object.keys(map).forEach(key => {
        res.push({
            attrKey: key,
            data: map[key],
        })
    });
    console.log(res);
    return map;
}