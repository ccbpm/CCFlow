
function InitPage() {

    $("JobSchedule").html("开发中.");
    return;

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

    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }

    if (data == "[]")
        return;

    data = eval('(' + data + ')');

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
    for (var i = 0; i < tracks.length; i++) {
        var Msg = "";
        var startTime = "";
        var endTime = "";
        var passTime = "";
        var actionType = "";
        var track = tracks[i];
        if (track.FID != 0)
            continue;

        if (track.ActionType == ActionType.FlowBBS)
            continue;
        if (track.ActionType == ActionType.WorkCheck)
            continue;

        //是否显示审批意见？
        var isShowCheckMsg = true;
        if (fwc.FWCMsgShow == "1" && track.NDFrom == GetQueryString("FK_Node") && webUser.No != track.EmpTo) {
            continue;
            //isShowCheckMsg = false;
        }

        //内容.

        actionType = track.ActionTypeText;
        var at = track.ActionType;

        //        if (at == ActionType.Return) {
        //            doc += "<p><span>退回到:</span><font color=green>" + track.NDToT + "</font><span>退回给:</span><font color=green>" + track.EmpToT + "</font></p>";
        //            doc += "<p><span>退回意见如下</span>  </p>";
        //        }

        if (at == ActionType.Forward || at == ActionType.FlowOver) {


            //找到该节点，该人员的审核track, 如果没有，就输出Msg, 可能是焦点字段。
            for (var myIdx = 0; myIdx < tracks.length; myIdx++) {

                var checkTrack = tracks[myIdx];
                if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
                    Msg = checkTrack.Msg;
                }
            }
        }
        //协作发送.
        if (at == ActionType.TeampUp) {

            for (var myIdx = 0; myIdx < tracks.length; myIdx++) {

                var checkTrack = tracks[myIdx];
                if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
                    Msg = checkTrack.Msg;
                }
            }
        }
        //输出备注信息.
        var tag = track.Tag;
        if (tag != null)
            tag = tag.replace("~", "'");

        msg = track.Msg;
        if (msg == "0")
            msg = "";

        if (msg != "") {

            var reg = new RegExp('\t\n', "g")

            msg = msg.replace(reg, '<br>');

            // Msg = Msg.replace(/t\n/g, '<br>');

            msg = msg.replace('null', '');

            if (msg == "" || Msg == undefined)
                msg = "无";

        };
        //获取轨迹中上一个节点的时间
        if (i == 0) {
            startTime = track.RDT;
            endTime = track.RDT;
        } else {
            //上一节点的到达时间就是本节点的开始时间
            var track1 = tracks[i - 1];
            startTime = track1.RDT;
            endTime = track.RDT;
        }
        //求得历时时间差
        var sdt = startTime.replace(/\-/g, "/");
        sdt = new Date(Date.parse(sdt.replace(/-/g, "/")));
        var edt = endTime.replace(/\-/g, "/");
        edt = new Date(Date.parse(edt.replace(/-/g, "/")));

        passTime = GetSpanTime(sdt, edt);
        if (passTime == '')
            passTime = '0秒';

        //输出row
        var newRow = "";
        newRow = "<tr>";
        newRow += "<td >" + idx + "</td>";
        newRow += "<td >" + track.NDFromT + "</td>";

        newRow += "<td >" + '' + "</td>";

        newRow += "<td >" + msg + "</td>";
        newRow += "<td >" + track.ActionTypeText + "</td>";
        newRow += "<td >" + track.EmpFromT + "</td>";
        newRow += "<td >" + startTime + "</td>";
        newRow += "<td >" + endTime + "</td>";
        newRow += "<td >" + passTime + "</td>";
        newRow += "</tr>";
        $("tbody tr:last").after(newRow);

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

            for (var i = 0; i < gwls.length; i++) {
                var html = "";

                var gwl = gwls[i];
                if (gwl.IsPass == 1)
                    continue;

                var newRow = "";
                newRow = "<tr>";
                newRow += "<td >" + idx + "</td>";
                newRow += "<td >" + gwl.FK_NodeText + "</td>";
                newRow += "<td >" + '' + "</td>";

                if (gwl.IsRead == "1") {
                    newRow += "<td ><span><font color=green>已阅读.</font></span></td>";
                } else {
                    newRow += "<td ><span><font color=green>尚未阅读.</font></span></td>";
                }
                newRow += "<td >" + '等待审批' + "</td>";
                newRow += "<td >" + gwl.FK_EmpText + "</td>";
                newRow += "<td >" + gwl.RDT + "</td>";
                newRow += "<td ></td>";
                newRow += "<td ></td>";
                newRow += "</tr>";
                $("tbody tr:last").after(newRow);
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
