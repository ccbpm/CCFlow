var currYear = (new Date()).getFullYear();
//初始化日期控件
var optDate = {
    preset: 'date', //日期
    theme: 'default', //皮肤样式
    display: 'modal', //显示方式 
    mode: 'scroller', //日期选择模式
    dateFormat: 'yy-mm-dd', // 日期格式
    setText: '确定', //确认按钮名称
    cancelText: '取消', //取消按钮名籍我
    dateOrder: 'yymmdd', //面板中日期排列格式
    dayText: '日', monthText: '月', yearText: '年', //面板中年月日文字
    endYear: currYear + 10, //结束年份
    showNow: true,
    nowText: "今天"
};

//初始化时间日期控件
var optDateTime = {
    preset: 'datetime', //日期，可选：date\datetime\time\tree_list\image_text\select
    theme: 'default', //皮肤样式，可选：default\android\android-ics light\android-ics\ios\jqm\sense-ui\wp light\wp
    display: 'modal', //显示方式 ，可选：modal\inline\bubble\top\bottom
    mode: 'scroller', //日期选择模式，可选：scroller\clickpick\mixed
    dateFormat: 'yy-mm-dd', // 日期格式
    setText: '确定', //确认按钮名称
    cancelText: '取消', //取消按钮名籍我
    dateOrder: 'yymmdd', //面板中日期排列格式
    hourText: '时', minuteText: '分',
    dayText: '日', monthText: '月', yearText: '年', //面板中年月日文字
    timeWheels: 'HHii',
    endYear: currYear + 10, //结束年份
    timeFormat: 'HH:ii',
    showNow: true,
    ampm: false,
    seconds: false,
    nowText: "今天"
};