
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
