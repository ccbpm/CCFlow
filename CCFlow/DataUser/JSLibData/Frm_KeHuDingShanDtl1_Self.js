
//当前选择的行.
var idx = 0;

function GetNow() {
    //取今天的日期.
    var myDate = new Date;
    var year = myDate.getFullYear();        //获取当前年
    var month = myDate.getMonth() + 1;   //获取当前月
    var date = myDate.getDate();            //获取当前日
    var dtNow = year + "-" + month + "-" + date;
    return dtNow;
}

// 客户约定交期 - 文本框内容变化.
function KeHuYueDingJiaoQi() {

    //获得料号.
    var liaohao = ReqDtlCtrlVal("LiaoHao");

    if (liaohao == null || liaohao == undefined || liaohao == "") {
        alert("请输入料号");
        return;
    }

    //取的输入的约定交期
    var dtTo = ReqDtlCtrlVal("KeHuYueDingJiaoQi");

    //获得当前日期.
    var dtNow = GetNow();

    //获得两个时间差.
    var intDingDanZhouqi = datedifference(dtNow, dtTo);

    //订单周期天数，字段赋值.
    SetDtlCtrlVal("DingShanZhouQiTianSh", intDingDanZhouqi);

    //取基础信息表的交期，天数字段.
    var intJiaoQi = ReqDtlCtrlVal("JiaoHuoZhouQi");

    if (intJiaoQi == 0) {
        alert("请输入料号,并且该料号的交货周期不能为0");
        return;
    }

    //alert(intJiaoQi + "  " + intDingDanZhouqi);
    var ctrlCB = ReqDtlCtrl("ShiFouGouJiaoQi");

    if (parseInt(intJiaoQi) <= parseInt(intDingDanZhouqi)) {

        //    alert("够交货周期.");
        //如果交期大于等于订单周期. 设置交期是否够.
        SetDtlCtrlVal("ShiFouGouJiaoQi", 1);

        // ctrlCB.css("color", "green");
        $(ctrlCB.parent().parent()).css("background-color", "white");

    } else {

        //  alert("不够交货周期.");
        SetDtlCtrlVal("ShiFouGouJiaoQi", 0);
        $(ctrlCB.parent().parent()).css("background-color", "red");
    }
}

$(function () {

    var trs = $('table tbody tr');
    if (trs.length == 1)
        return;

    for (var i = 0; i < trs.length; i++) {
        curRowIndex = i;
        KeHuYueDingJiaoQi();
        DingShanShuLiang();
    }
});

//检查是否超开订单.
function CheckIsChaoKaiDingDan() {
    var nodeID = GetQueryString("FK_Node");
    if (nodeID == 2901) {
        alert("您不能填写此栏目.");
        SetDtlCtrlVal("ChaoKaiDingShan", 0);
        return;
    }

    var isDBJPL = ReqDtlCtrlVal("IsDBJPL");

    if (isDBJPL == "on" || isDBJPL == 1 || isDBJPL == "1") {

    } else {
        alert("不符合超开订单的条件，已经达到了报价批量..");
        SetDtlCtrlVal("ChaoKaiDingShan", 0);
    }
}

function CheckBlank() {
    var nodeID = GetQueryString("FK_Node");
    if (nodeID == 2901) {
        alert("您不能填写 开单数量.");
        SetDtlCtrlVal("KaiShanShuLiang", 0);
        return;
    }
}


//订单数 - 变化后要处理的事件.
function DingShanShuLiang() {

    //获得料号.
    var liaohao = ReqDtlCtrlVal("LiaoHao");
    if (liaohao == null || liaohao == "") {
        alert("请输入料号");
        return;
    }


    //获得输入的订单数.
    var intDingShanShuLiang = ReqDtlCtrlVal("DingShanShuLiang");

    //获得报价批量.
    var intBaoJiaPiLiang = ReqDtlCtrlVal("BaoJiaPiLiang");

    //获得是否达标价批量.
    var ctrlCB = ReqDtlCtrl("IsDBJPL");

    //   alert("intBaoJiaPiLiang : " + intDingShanShuLiang + "  intBaoJiaPiLiang:" + intBaoJiaPiLiang);

    //如果订单数小于报价批量.
    if (parseInt(intDingShanShuLiang) < parseInt(intBaoJiaPiLiang)) {
        SetDtlCtrlVal("IsDBJPL", 0);
        $(ctrlCB.parent().parent()).css("background-color", "red");

    } else {
        SetDtlCtrlVal("IsDBJPL", 1);
        $(ctrlCB.parent().parent()).css("background-color", "white");
    }
}


function datedifference(sDate1, sDate2) {    //sDate1和sDate2是2006-12-18格式  
    var dateSpan,
        tempDate,
        iDays;
    sDate1 = Date.parse(sDate1);
    sDate2 = Date.parse(sDate2);
    dateSpan = sDate2 - sDate1;
    dateSpan = Math.abs(dateSpan);
    iDays = Math.floor(dateSpan / (24 * 3600 * 1000));
    return iDays
}

//计算两个时间点的日期.
function SetQingJiaTianShu() {

    var dtFrom = $("#TB_QingJiaRiJiCong").val();
    var dtTo = $("#TB_RiJiDao").val();

    var days = datedifference(dtFrom, dtTo);
    if (days == undefined || days == "NaN" || days == NaN)
        days = 0;

    $("#TB_QingJiaTianShu").val(days);
}


// 处理一些控件是否可用.
function WhenClickCheckBoxClick(cb) {

    //使用ccform的内置函数根据字段名获得控件，然后给它的属性赋值.
    ReqTBObj('JiaTingZhuZhi').disabled = !cb.checked;
    ReqTBObj('JiaTingDianHua').disabled = !cb.checked;
    ReqDDLObj('FK_SF').disabled = !cb.checked;
    ReqDDLObj('XingBie').disabled = !cb.checked;
    ReqCBObj('HunFou').disabled = !cb.checked;

    //让控件变化背景颜色。
    var color = 'InfoBackground';
    if (cb.checked) {
        color = 'white';
    }
    ReqTBObj('JiaTingZhuZhi').style.backgroundColor = color;
    ReqTBObj('JiaTingDianHua').style.backgroundColor = color;
    ReqDDLObj('FK_SF').style.backgroundColor = color;
    ReqDDLObj('XingBie').style.backgroundColor = color;
    ReqCBObj('HunFou').style.backgroundColor = color;
    return;
}
