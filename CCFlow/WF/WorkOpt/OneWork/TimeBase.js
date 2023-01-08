var subFlows = [];
function InitPage() {

    var isMobile = GetQueryString('IsMobile');
    if (isMobile == null || isMobile == undefined || isMobile == "")
        isMobile = 0;

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("TimeBase_Init");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    if (data == "[]")
        return;

    data = cceval('(' + data + ')');

    //日志列表.
    var tracks = data["Track"];

    //工作列表.
    var timeDay = "";
    var checkStr = "";
    var dotColor = -1;
    var idx = 1;

    //获得流程引擎注册表信息.
    var gwf = data["WF_GenerWorkFlow"][0];

    //审核组件信息.
    var fwc = data["FrmWorkCheck"][0];

    //获得工作人员列表.
    var gwls = data["WF_GenerWorkerList"];

    //该流程启动的子流程数据
    subFlows = data["WF_SubFlow"];

    //前进的 track. 用于获取当前节点的上一个节点的track.
    var trackDotOfForward = null;

    var webUser = new WebUser();

    //是否有审核组件?
    var isHaveCheck = false;
    for (var i = 0; i < tracks.length; i++) {

        var track = tracks[i];

        // 记录审核节点。
        if (track.ActionType == ActionType.WorkCheck) {
            isHaveCheck = true;
            break;
        }
    }

    //输出列表. zhoupeng 2017-12-19 修改算法，所有的审核动作都依靠发送来显示.
    var isHaveThread = false; //是否有子线程
    var firstTrack;
    var isHaveSubFlow = false; //是否有子流程
    var threadTrMyPK = "";
    for (var i = 0; i < tracks.length; i++) {

        var track = tracks[i];
        if (track.FID != 0) {
            if (isHaveThread == false)
                firstTrack = track;
            isHaveThread = true;
            if (track.ActionType != ActionType.WorkCheck)
                threadTrMyPK += track.MyPK + ",";
            continue;
        }
        var at = track.ActionType;
        if (at == ActionType.ForwardFL && track.NDFrom!=parseInt(GetQueryString("FK_Flow"))+"01")
            continue;
        //增加一个子线程的节点
        if (isHaveThread == true) {
            var newRow = "";
            firstTrack.EmpFrom = "";
            firstTrack.EmpFromT = "";
            newRow = "<tr  title='子线程前进 ' >";
            newRow += "<td class='TDTime' >" + GenerLeftIcon(firstTrack) + "</td>";
            newRow += "<td class='TDBase' ></td>";
            newRow += "<td class='TDDoc' ><img src = '../../Img/Action/ForwardFL.png' width = '10px;' class='ImgOfAC' alt = '子线程前进' />子线程前进<p><a href='javaScript:OpenSubThreadTime(" + firstTrack.FID + ",\"" + threadTrMyPK + "\")'>查看子线程时间轴</a></p></td>";
            newRow += "</tr>";

            $("#Table1 tr:last").after(newRow);
        }
        isHaveThread = false;
        threadTrMyPK = "";

        if (track.ActionType == ActionType.FlowBBS)
            continue;
        if (track.ActionType == ActionType.WorkCheck)
            continue;

        //时间轴.
        var timeBase = "";
        var img = ActionTypeStr(track.ActionType);
        img = "<img src='" + img + "' width='10px;' class='ImgOfAC' alt='" + track.ActionTypeText + "'  />";

        //是否显示审批意见？
        var isShowCheckMsg = true;
        if (fwc.FWCMsgShow == "1" && track.NDFrom == GetQueryString("FK_Node") && webUser.No != track.EmpTo) {
            continue;
            //isShowCheckMsg = false;
        }

        //内容.
        var doc = "";
        doc += img + track.NDFromT + " - " + track.ActionTypeText;

        if (at == ActionType.Return) {
            doc += "<p><span>退回到:</span><font color=green>" + track.NDToT + "</font><span>退回给:</span><font color=green>" + track.EmpToT + "</font></p>";
            doc += "<p><span>退回意见如下</span>  </p>";
        }

        var isHaveCheck = false;
        var nodeSubFlows = $.grep(subFlows, function (subFlow) {
            return subFlow.FK_Node == track.NDFrom && subFlow.SubFlowSta==1;
        });

        if (at == ActionType.Forward || at == ActionType.FlowOver) {
            doc += "<p><span>到达节点:</span><font color=green>" + track.NDToT + "</font><span>到达人员:</span><font color=green>" + track.EmpToT + "</font> </p>";

            //判断是否隐藏
            if (Hide_IsOpenFrm == true) {
                doc += "<p><span><a href=\"javascript:OpenFrm('" + workid + "','" + track.NDFrom + "','" + fk_flow + "','" + fid + "','" + track.NDFrom + "','" + track.MyPK + "')\">查看表单</a></span></p>";
            }
            if (nodeSubFlows.length!=0)
                doc += "<p><span><a href=\"javascript:void(0);\" onclick=\"OpenSubFlowTable(this,'" + workid + "','" + track.NDFrom + "')\">查看子流程</a></span></p>";
            //说明审核组件采用的是2019版本
            if (track.Msg != null && track.Msg != undefined && track.Msg.indexOf("WorkCheck@") != -1) {
                var val = track.Msg.split("WorkCheck@");
                if (val.length == 2) {
                    track.Msg = val[1];
                    isHaveCheck = true;
                    doc += "<p><span>审批意见：</span><font color=green>" + track.Msg + "</font> </p>";
                }
            } else {
                //查找关联的审核意见
                //找到该节点，该人员的审核track, 如果没有，就输出Msg, 可能是焦点字段。
                for (var myIdx = 0; myIdx < tracks.length; myIdx++) {
                    var checkTrack = tracks[myIdx];
                    if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
                        isHaveCheck = true;
                        doc += "<p><span>审批意见：</span><font color=green>" + checkTrack.Msg + "</font> </p>";
                        break;
                    }
                }
            }



        }

        //协作发送.
        if (at == ActionType.TeampUp) {

            for (var myIdx = 0; myIdx < tracks.length; myIdx++) {

                var checkTrack = tracks[myIdx];
                if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
                    var val = track.Msg.replace('null', '').split("WorkCheck@");
                    if (val.length == 2)
                        track.Msg = val[1];
                    doc += "<p><span>会签意见：</span><font color=green>" + track.Msg.replace('null', '') + "</font> </p>";
                }
            }
        }

        //输出备注信息.
        var tag = track.Tag;
        if (tag != null)
            tag = tag.replace("~", "'");

        var msg = track.Msg;
        if (msg.indexOf("WorkCheck@") != -1) {
            var val = track.Msg.replace('null', '').split("WorkCheck@");
            if (val.length == 2)
                msg = val[1];
        }
        if (msg == "0")
            msg = "";

        if (msg != "" && isHaveCheck == false) {

            while (msg.indexOf('\t\n') >= 0) {
                msg = msg.replace('\t\n', '<br>');
            }

            msg = msg.replace('null', '');

            if (msg == "" || msg == undefined)
                msg = "无";


            doc += "<p>";
            doc += "<font color=green>" + msg + "</font>";
            doc += "</p>";
        }

        //输出row
        var newRow = "";
        newRow = "<tr  title='" + track.ActionTypeText + "' >";
        newRow += "<td class='TDTime' >" + GenerLeftIcon(track) + "</td>";
        newRow += "<td class='TDBase' ></td>";
        newRow += "<td class='TDDoc' >" + doc + "</td>";
        newRow += "</tr>";

        $("#Table1 tr:last").after(newRow);

        idx++;
    }


    //增加等待审核的人员, 在所有的人员循环以后.
    if (gwls) {
        var isHaveNoChecker = false;
        var gwl = null;
        for (var i = 0; i < gwls.length; i++) {

            var gwl = gwls[i];
            if (gwl.IsPass == 1)
                continue;

            isHaveNoChecker = true;
        }

        //如果有尚未审核的人员，就输出.
        if (isHaveNoChecker == true) {

            var rowDay = "<tr>";
            rowDay += "<td colspan=3 class=TDDay ><span>等待审批</span><b>" + gwf.NodeName + "</b></td>";
            rowDay += "</tr>";
            $("#Table1 tr:last").after(rowDay);


            for (var i = 0; i < gwls.length; i++) {
                var html = "";

                var gwl = gwls[i];
                if (gwl.IsPass == 1)
                    continue;

                if (gwl.IsPass == 3 || gwl.IsPass < 0)
                    continue;

                var doc = "";
                if (Hide_IsRead == true) {

                    doc += "<div class='fright'>";
                    if (gwl.IsRead == "1")
                        doc += "<span class='yd'>已阅</span></div>";
                    else
                        doc += "<span class='wd'>未阅</span></div>";


                }

                doc += "<p><span>审批人:</span><strong>";
                doc += gwl.FK_EmpText;
                doc += "</strong></p>";
                //判断是否隐藏
               
                doc += "<p>";
                doc += "<span class='fright'>应完成日期:";
                doc += gwl.SDT +'</span>';

                doc += "<span>工作到达日期:</span>";
                doc += gwl.RDT;
               
               
                doc += "</p>";
                //到达时间.
                var toTime = gwl.RDT;
                var toTimeDot = toTime.replace(/\-/g, "/");
                toTimeDot = new Date(Date.parse(toTimeDot.replace(/-/g, "/")));

                //当前发生日期.
                timeDot = new Date();


                doc += "<p>";
                doc += "<span class='fright'>已经耗时:<font color=red>";
                doc += GetSpanTime(toTimeDot, timeDot) +'</font></span>';

               


                //应该完成日期.
                toTime = gwl.SDT;
                toTimeDot = toTime.replace(/\-/g, "/");
                toTimeDot = new Date(Date.parse(toTimeDot.replace(/-/g, "/")));

                //当前发生日期.
                timeDot = new Date();

                var timeLeft = GetSpanTime(timeDot, toTimeDot);

                if (timeLeft != 'NaN秒') {                  
                    if (timeLeft.startsWith('-')) {
                        doc += "<span>已超时:<font color=red>";
                        doc += timeLeft.substring(1, timeLeft.length) + '</font></span>'
                    } else {
                        doc += "<span>还剩余:<font color=green>";
                        doc += timeLeft + '</font></span>'
                    };
                }
                doc += "</p>";
                var nodeSubFlows = $.grep(subFlows, function (subFlow) {
                    return subFlow.FK_Node == gwl.FK_Node && subFlow.SubFlowSta == 1;
                });
                if (nodeSubFlows.length != 0)
                    doc += "<p><span><a href=\"javascript:void(0);\" onclick=\"OpenSubFlowTable(this,'" + gwl.WorkID + "','" + gwl.FK_Node + "')\">查看子流程</a></span></p>";


                var left = "<div class='usernameInfo'>";
                left += "<span class='uimg'><img src='../../../DataUser/UserIcon/" + gwl.FK_Emp + ".png'  onerror=\"src='../../../DataUser/UserIcon/Default.png'\" style='width:60px;' /></span><div class='ut'><span class='uname'>";
                left += "" + gwl.FK_EmpText;      
                left += "</span></div></div>";
               

                var newRow = "";
                newRow = "<tr  title='等待审批人员' >";
                newRow += "<td class='TDTime' >" + left + "</td>";
                newRow += "<td class='TDBase' ></td>";
                newRow += "<td class='TDDoc' >" + doc + "</td>";
                newRow += "</tr>";

                $("#Table1 tr:last").after(newRow);
            }
        }
    }

    //调整大小.
    if (window.screen) {
        var w = screen.availWidth;
        var h = screen.availHeight;
        window.moveTo(0, 0);
        window.resizeTo(w, h);
    }
}

//子线程，子流程的时间轴轨迹
function OpenSubThreadTime(workID, mypks) {

    OpenBootStrapModal("./TimeSubThread.htm?MyPks=" + mypks + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node"), "SubThread", "子线程", 500, 600);
}

/**
 * 父流程启动的子流程
 * @param {any} workID
 */
function OpenSubFlowTable(obj,workid,fk_node) {
    var nodeSubFlows = $.grep(subFlows, function (subFlow) {
        return subFlow.FK_Node == fk_node && subFlow.SubFlowSta == 1;
    });
    //显示一个table,包含启动的子流程名称和启动的子流程实例
    var _html = "";
    var gwfs = new Entities("BP.WF.GenerWorkFlows");
    if (workid == null || workid == undefined)
        workid = 0;

    gwfs.Retrieve("PWorkID", workid, "PNodeID", fk_node);
    nodeSubFlows.forEach(function (subFlow) {
        _html += subFlow.SubFlowName + "<br>";
        _html += "<div style='display: block;height: 1px;width: 95%;margin: 8px 0;background-color: #dcdfe6;position: relative;'></div>"
        $.each(gwfs, function (i, gwf) {
            if (gwf.FK_Flow == subFlow.SubFlowNo) {
                _html +="<a href=\"javaScript:OpenSubFlow("+gwf.WorkID+",'"+gwf.FK_Flow+"',"+gwf.FK_Node+","+gwf.PWorkID+")\">"+gwf.Title + "<br>";
            }
        })
    });
    var p = $(obj).offset();
    $('#subFlowDiv').html(_html);
    $('#subFlowinfo').offset({ top: p.top + 20 - 2, left: p.left+30 });
    $('#subFlowinfo').show();

}

/* 打开子流程表单. */
function OpenSubFlow(workid, flowNo, nodeID, pworkid) {
    var url = "../../MyView.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID+ "&PWorkID=" + pworkid;
    window.open(url);
    //window.location.url = url;
    return;
}

//生成左边的icon.
function GenerLeftIcon(track) {
    //左边的日期点.
    var left = "<div class='usernameInfo'>";
    days = track.RDT.substring(5, 16);
    days = days.replace('-', '月');
    days = days.replace(' ', '日');
    days = days.replace(':', '时');

   
    left += "<span class='uimg'><img src='../../../DataUser/UserIcon/" + track.EmpFrom + ".png'  onerror=\"src='../../../DataUser/UserIcon/Default.png'\" style='width:60px;' /></span><div class='ut'><span class='uname'>";
    left += track.EmpFromT;
    left += "</span><span class='utime'>" + days + "分";
    left += "</span></div></div>";

    return left;
}

function GetSpanTime(date1, date2) {
    ///<summary>计算date2-date1的时间差，返回使用“x天x小时x分x秒”形式的字符串表示</summary>
    var date3 = date2.getTime() - date1.getTime();  //时间差秒
    if (date1.getTime() > date2.getTime())
        date3 = date1.getTime() - date2.getTime();

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


    var leave3 = leave2 % (60 * 1000);
    var seconds = Math.floor(leave3 / 1000);
    if (seconds > 0)
        str += seconds + '秒';
    if (date1.getTime() > date2.getTime())
        return "-" + str;
    return str;

    if (seconds == NaN)
        return date1 + "," + date2;
    return str;


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
            return "../../Img/dot.png";
    }
}
