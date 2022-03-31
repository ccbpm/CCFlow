//生成左边的icon.
function GenerLeftIcon(track) {
    //左边的日期点.
    var left = "<center>";
    left = track.RDT.substring(5, 16);
    left = left.replace('-', '月');
    left = left.replace(' ', '日');
    left = left.replace(':', '时');

    left = left + "分";
    left += "<br><img src='../../../DataUser/UserIcon/" + track.EmpFrom + ".png'  onerror=\"src='../../../DataUser/UserIcon/Default.png'\" style='width:60px;' />";
    left += "<br>" + track.EmpFromT + "&nbsp;&nbsp;&nbsp;";
    left += "</center>";
    return left;
}
function GenerDataFormat(dt) {
    //左边的日期点.
    var left = "<center>";
    left = dt.substring(5, 16);
    left = left.replace('-', '月');
    left = left.replace(' ', '日');
    left = left.replace(':', '时');
    left = left + "分";
    return left;
}

function GetSpanTime(date1, date2) {
    ///<summary>计算date2-date1的时间差，返回使用“x天x小时x分x秒”形式的字符串表示</summary>
    var date3 = date2.getTime() - date1.getTime();  //时间差秒
    var str = '';
    //计算出相差天数
    var days = Math.floor(date3 / (24 * 3600 * 1000));
    if (days > 0) {
        str += days + '天';
    }

    //计算出小时数
    var leave1 = date3 % (24 * 3600 * 1000);   //计算天数后剩余的毫秒数
    var hours = Math.floor(leave1 / (3600 * 1000));
    if (hours > 0) {
        str += hours + '小时';
    }

    //计算相差分钟数
    var leave2 = leave1 % (3600 * 1000);         //计算小时数后剩余的毫秒数
    var minutes = Math.floor(leave2 / (60 * 1000));
    if (minutes > 0) {
        str += minutes + '分';
    }

    if (str.length == 0) {
        var leave3 = leave2 % (60 * 1000);
        var seconds = Math.floor(leave3 / 1000);

        str += seconds + '秒';

        if (seconds == NaN)
            return date1 + ","+date2;
        return str;
    }

    return str;
}

/**Begin - Form Controls ***************/
var ActionType = {
    /// <summary>
    /// 发起
    /// </summary>
    Start: 0,
    /// <summary>
    /// 前进(发送)
    /// </summary>
    Forward: 1,
    /// <summary>
    /// 退回
    /// </summary>
    Return: 2,
    /// <summary>
    /// 退回并原路返回.
    /// </summary>
    ReturnAndBackWay: 201,
    /// <summary>
    /// 移交
    /// </summary>
    Shift: 3,
    /// <summary>
    /// 撤消移交
    /// </summary>
    UnShift: 4,
    /// <summary>
    /// 撤消发送
    /// </summary>
    UnSend: 5,
    /// <summary>
    /// 分流前进
    /// </summary>
    ForwardFL: 6,
    /// <summary>
    /// 合流前进
    /// </summary>
    ForwardHL: 7,
    /// <summary>
    /// 流程正常结束
    /// </summary>
    FlowOver: 8,
    /// <summary>
    /// 调用起子流程
    /// </summary>
    CallChildenFlow: 9,
    /// <summary>
    /// 启动子流程
    /// </summary>
    StartChildenFlow: 10,
    /// <summary>
    /// 子线程前进
    /// </summary>
    SubFlowForward: 11,
    /// <summary>
    /// 取回
    /// </summary>
    Tackback: 12,
    /// <summary>
    /// 恢复已完成的流程
    /// </summary>
    RebackOverFlow: 13,
    /// <summary>
    /// 强制终止流程 For lijian:2012-10-24.
    /// </summary>
    FlowOverByCoercion: 14,
    /// <summary>
    /// 挂起
    /// </summary>
    Hungup: 15,
    /// <summary>
    /// 取消挂起
    /// </summary>
    UnHungup: 16,
    /// <summary>
    /// 强制移交
    /// </summary>
    ShiftByCoercion: 17,
    /// <summary>
    /// 催办
    /// </summary>
    Press: 18,
    /// <summary>
    /// 逻辑删除流程(撤销流程)
    /// </summary>
    DeleteFlowByFlag: 19,
    /// <summary>
    /// 恢复删除流程(撤销流程)
    /// </summary>
    UnDeleteFlowByFlag: 20,
    /// <summary>
    /// 抄送
    /// </summary>
    CC: 21,
    /// <summary>
    /// 工作审核(日志)
    /// </summary>
    WorkCheck: 22,
    /// <summary>
    /// 删除子线程
    /// </summary>
    DeleteSubThread: 23,
    /// <summary>
    /// 请求加签
    /// </summary>
    AskforHelp: 24,
    /// <summary>
    /// 加签向下发送
    /// </summary>
    ForwardAskfor: 25,
    /// <summary>
    /// 自动条转的方式向下发送
    /// </summary>
    Skip: 26,
    /// <summary>
    /// 队列发送
    /// </summary>
    Order: 27,
    /// <summary>
    /// 协作发送
    /// </summary>
    TeampUp: 28,
    /// <summary>
    /// 流程评论
    /// </summary>
    FlowBBS: 29,
    ///会签.
    HuiQian:30,
    /// <summary>
    /// 信息
    /// </summary>
    Info: 100
};

function ActionTypeStr(at) {

    switch (at) {
        case ActionType.Start:
            return "../../Img/Action/Start.png";
        case ActionType.Forward:
            return "../../Img/Action/Forward.png";
        case ActionType.Return:
            return "../../Img/Action/Return.png";
        case ActionType.ReturnAndBackWay:
            return "../../Img/Action/ReturnAndBackWay.png";
        case ActionType.Shift:
            return "../../Img/Action/Shift.png";
        case ActionType.UnShift:
            return "../../Img/Action/UnShift.png";
        case ActionType.UnSend:
            return "../../Img/Action/UnSend.png";
        case ActionType.ForwardFL:
            return "../../Img/Action/ForwardFL.png";
        case ActionType.ForwardHL:
            return "../../Img/Action/ForwardHL.png";
        case ActionType.CallChildenFlow:
            return "../../Img/Action/CallChildenFlow.png";
        case ActionType.StartChildenFlow:
            return "../../Img/Action/StartChildenFlow.png";
        case ActionType.SubFlowForward:
            return "../../Img/Action/SubFlowForward.png";
        case ActionType.RebackOverFlow:
            return "../../Img/Action/RebackOverFlow.png";
        case ActionType.FlowOverByCoercion:
            return "../../Img/Action/FlowOverByCoercion.png";
        case ActionType.Hungup:
            return "../../Img/Action/Hungup.png";
        case ActionType.UnHungup:
            return "../../Img/Action/UnHungup.png";
        case ActionType.ShiftByCoercion:
            return "../../Img/Action/ShiftByCoercion.png";
        case ActionType.Press:
            return "../../Img/Action/Press.png";
        case ActionType.DeleteFlowByFlag:
            return "../../Img/Action/DeleteFlowByFlag.png";
        case ActionType.UnDeleteFlowByFlag:
            return "../../Img/Action/UnDeleteFlowByFlag.png";
        case ActionType.CC:
            return "../../Img/Action/CC.png";
        case ActionType.WorkCheck:
            return "../../Img/Action/WorkCheck.png";
        case ActionType.AskforHelp:
            return "../../Img/Action/AskforHelp.png";
        case ActionType.Skip:
            return "../../Img/Action/Skip.png";
        case ActionType.Order:
            return "../../Img/Action/Order.png";
        case ActionType.TeampUp:
            return "../../Img/Action/TeampUp.png";
        case ActionType.FlowBBS:
            return "../../Img/Action/FlowBBS.png";
        case ActionType.Info:
            return "../../Img/Action/Info.png";
        default:
            return "../../Img/Dot.png";
    }
}
