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
    if (days == undefined || days == "NaN" || days ==NaN )
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
