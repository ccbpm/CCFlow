
var webUser = new WebUser();

if (typeof FrmSubFlowSta == "undefined") {

    var FrmSubFlowSta = {}
    // 不可用
    FrmSubFlowSta.Disable = 0,
        // 可用
        FrmSubFlowSta.Enable = 1,
        // 只读
        FrmSubFlowSta.Readonly = 2

}
if (typeof SFShowCtrl == "undefined") {
    var SFShowCtrl = {}
    // 所有的子线程都可以看到
    SFShowCtrl.All = 0,
        // 仅仅查看我自己的
        SFShowCtrl.MySelf = 1

}

//传一个节点对象.
function SubFlow_Init(node) {
     
    var workID = GetQueryString("WorkID");
    if (workID == null || workID == undefined)
        workID = 0;

    var flowNo = GetQueryString("FK_Flow");
    var nodeID = node.NodeID; // GetQueryString("FK_Node");
    var currNodeID = GetQueryString("FK_Node");

    var pworkID = GetQueryString("WorkID");
    if (pworkID == null || pworkID == undefined)
        pworkID = 0;

    var _Html = "";
    var subFlows = new Entities("BP.WF.Template.SFlow.SubFlowHands");
    subFlows.Retrieve("FK_Node", nodeID, "SubFlowType", 0, "Idx");

    //处理累加表单问题，如果当前节点与，绑定子流程的节点不一致，就把他设置为只读.
    if (currNodeID != nodeID)
    {
        for (var i = 0; i < subFlows.length; i++) {
            var en = subFlows[i];
            en.SubFlowSta = 2;
        }
    }

    //查询出来所有子流程的数据.
    var fsf = new Entity("BP.WF.Template.SFlow.FrmSubFlow", nodeID);

    var subFlowGuids = $.grep(subFlows, function (subFlow) {

        return subFlow.SubFlowStartModel !=0;
    });
    //表示存在批量发起子流程
    if (subFlowGuids.length != 0) {
        return ShowBtnListSubFlow(subFlows, fsf, node, workID, pworkID, flowNo, nodeID);

    }
    return ShowTableSubFlow(subFlows, fsf, node, workID, pworkID, flowNo, nodeID);
}

//自定义展示子流程
function ShowBtnListSubFlow(subFlows, fsf, node, workID, pworkID, flowNo, nodeID) {
    var _Html = "";
    var basePath = "./";
    var currUrl = GetHrefUrl();

    if (currUrl.indexOf("Admin/FoolFormDesigner/Designer.htm") != -1)
        basePath = "../../";
    for (var i = 0; i < subFlows.length; i++) {
        var subFlow = subFlows[i];

        //如果子流程为启动模式
        /*if (fsf.SFSta == FrmSubFlowSta.Enable && GetQueryString("DoType") != "View") {
            //增加启动按钮
            if (subFlow.SubFlowModel == 0 || subFlow.SubFlowModel == null) { //下级子流程.
                _Html += "<div style='text-align:left'><input type='button' value='会签单位' onclick=\"javascript:SelectOpenIt(0,'" + subFlow.MyPK + "','" + subFlow.SubFlowNo + "'," + workID + "," + nodeID + ",'" + flowNo + "'," + GetQueryString("FID") + ")\"  /></div>";
            }

            if (subFlow.SubFlowModel == 1) { //平级子流程.

                if (gwf == null)
                    gwf = new Entity("BP.WF.GenerWorkFlow", workID);

                //如果当前的流程不是子流程，就不处理.
                if (gwf.PWorkID == 0) {
                    _Html += "<div style='text-align:left'><img src='./Img/Max.gif' />&nbsp;" + subFlow.SubFlowName + "</div> <div style='float:right'>为子流程的时候才能启动(" + subFlow.SubFlowName + ")]</style></div>";
                } else {
                    pworkID = gwf.PWorkID;
                    //传递启动该子流程的流程的信息 IsSameLevel = 1;SLWorkID=workId 
                    _Html += "<div style='text-align:left'><input type='button' value='会签单位' onclick=\"javascript:SelectOpenIt(1,'" + subFlow.MyPK + "','" + subFlow.SubFlowNo + "'," + gwf.PWorkID + "," + gwf.PNodeID + ",'" + gwf.PFlowNo + "'," + gwf.PFID + "," + workID + "," + nodeID + ",'" + flowNo + "')\"  /></div>";
                }
            }
        }*/
        //} else {
        //    _Html += "<div style='float:left'><span>" + subFlow.SubFlowName + "</span></div>";
        //}
        _Html += "<div>" + subFlow.SubFlowName + "</div>";
        var gwfs = new Entities("BP.WF.GenerWorkFlows");
        if (fsf.SFShowCtrl == SFShowCtrl.All)
            gwfs.Retrieve("PWorkID", pworkID, "FK_Flow", subFlow.SubFlowNo, "WFState"); //流程.          
        else
            gwfs.Retrieve("PWorkID", pworkID, "FK_Flow", subFlow.SubFlowNo, "Starter", webUser.No, "WFState"); //流程.
        _Html += "<div id='WFState_" + subFlow.SubFlowNo + "'>";
        for (var j = 0; j < gwfs.length; j++) {

            var item = gwfs[j];
            if (item.WFState == 1) {
                //只显示标题
                _Html += "<div style='line-height: 30px;padding-left: 6px;' id='" + item.WorkID + "'>" + GetPara(item.AtPara, "SubFlowGuideEnNameFiled") + "<span class='glyphicon glyphicon-remove' style='margin-left:3px' onclick='DeleteSubFlowDraf(" + item.WorkID + ",\"" + item.FK_Flow + "\")'></span></div>";
                continue;
            }
           
            var url = basePath+"MyView.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "&IsCheckGuide=1&Frms=" + item.Paras_Frms + "&FK_Node=" + item.FK_Node + "&PNodeID=" + item.PNodeID + "&PWorkID=" + item.PWorkID;
            _Html += "<div style='line-height: 30px;padding-left: 6px;' id='" + item.WorkID + "'>" + item.Title + "<span class='glyphicon glyphicon-folder-open' style='margin-left:3px' onclick='OpenIt(\"" + url + "\")'></span></div>";

        }
        _Html += "</div>";

    }
    return _Html;

}
function GetState(wfState) {

    switch (parseInt(wfState)) {
        case 1:
            return "草稿";
        case 2:
          
            return "运行中";
            break;
        case 3: //已完成.
            return "已完成";
            break;
        case 4:
            return "挂起";
        case 5:
            return "退回";
        case 6:
            return "转发";
        case 7:
            return "删除";
        case 8:
            return "加签";
        case 11:
            return "加签回复";
        default:
            return "其它";
    }
}

//表格模式展示子流程
function ShowTableSubFlow(subFlows, sf, node, workID, pworkID, flowNo, nodeID) {
    var _Html = "";

    _Html += "<table width='100%'>";
    _Html += "<tr>";
    _Html += "<th class='TitleExt'>发起人</th>";
    _Html += "<th class='TitleExt'>标题</th>";
    _Html += "<th class='TitleExt'>停留节点</th>";
    _Html += "<th class='TitleExt'>状态</th>";
    _Html += "<th class='TitleExt'>处理人</th>";
    _Html += "<th class='TitleExt'>处理时间</th>";
    _Html += "<th class='TitleExt'>信息</th>";
    _Html += "</tr>";

    //要兼容旧版本.
    if (subFlows.length == 0 && sf.SFDefInfo != "") {
        var strs = sf.SFDefInfo.split(',');
        for (var idx = 0; idx < strs.length; idx++) {
            var flowNo = strs[idx];
            if (flowNo == null || flowNo == "")
                continue;

            var en = new Entity("BP.WF.Template.SFlow.SubFlowHand");
            en.FK_Node = nodeID;
            en.SubFlowNo = flowNo;

            en.SetPKVal(flowNo + "_" + nodeID + "_0");
            en.Insert();
        }
    }
    var tdHtml = "";
    var imgbasePath = "./";
    var currUrl = GetHrefUrl();

    if (currUrl.indexOf("Admin/FoolFormDesigner/Designer.htm") != -1)
        imgbasePath = "../../";
    if (currUrl.indexOf("FrmGener.htm") != -1)
        imgbasePath = "../";
    for (var i = 0; i < subFlows.length; i++) {

        var subFlow = subFlows[i];

        if (subFlow.SubFlowSta == 0)
            continue; //如果是禁用. @0=禁用@1=启用@2=只读

        if (sf.SFSta == FrmSubFlowSta.Enable && subFlow.SubFlowSta == 1 && GetQueryString("DoType") != "View") {

            if (subFlow.SubFlowModel == 0 || subFlow.SubFlowModel == null) { //下级子流程.
                tdHtml = "<div style='float:left'><img src='" + imgbasePath + "Img/Max.gif' />&nbsp;" + subFlow.SubFlowName + "</div> <div style='float:right'>[<a href=\"javascript:OpenIt('" + imgbasePath +"MyFlow.htm?IsStartSameLevelFlow=0&FK_Flow=" + subFlow.SubFlowNo + "&PWorkID=" + workID + "&PNodeID=" + nodeID + "&PFlowNo=" + flowNo + "&PFID=" + GetQueryString("FID") + "')\"  >" + sf.SFCaption + "</a>]</style>";
            }

            if (subFlow.SubFlowModel == 1) { //平级子流程.

                if (gwf == null)
                    gwf = new Entity("BP.WF.GenerWorkFlow", workID);

                //如果当前的流程不是子流程，就不处理.
                if (gwf.PWorkID == 0) {
                    tdHtml = "<div style='float:left'><img src='" + imgbasePath +"Img/Max.gif' />&nbsp;" + subFlow.SubFlowName + "</div> <div style='float:right'>为子流程的时候才能启动(" + subFlow.SubFlowName + ")]</style>";
                } else {
                    pworkID = gwf.PWorkID;
                    //传递启动该子流程的流程的信息 IsSameLevel = 1;SLWorkID=workId 
                    tdHtml = "<div style='float:left'><img src='" + imgbasePath + "Img/Max.gif' />&nbsp;" + subFlow.SubFlowName + "</div> <div style='float:right'>[<a href=\"javascript:OpenIt('" + imgbasePath +"MyFlow.htm?FK_Flow=" + subFlow.SubFlowNo + "&PWorkID=" + gwf.PWorkID + "&PNodeID=" + gwf.PNodeID + "&PFlowNo=" + gwf.PFlowNo + "&PFID=" + gwf.PFID + "&IsStartSameLevelFlow=1&SLWorkID=" + workID + "&SLNodeID=" + nodeID + "&SLFlowNo=" + flowNo + "')\"  >" + sf.SFCaption + "</a>]</style>";
                }
            }
        }

        if (sf.SFSta == FrmSubFlowSta.Readonly || subFlow.SubFlowSta == 2 || GetQueryString("DoType") == "View")
            tdHtml = "<div style='float:left'><img src='" + imgbasePath +"Img/Max.gif' />&nbsp;" + subFlow.SubFlowName + "</div></style>";

        _Html += "<tr>";
        _Html += "<td class='TRSum' colspan=7 >" + tdHtml + "</td>";
        _Html += "</tr>";

        //该流程的子流程信息.
        var gwfs = new Entities("BP.WF.GenerWorkFlows");
        if (sf.SFShowCtrl == SFShowCtrl.All)
            gwfs.Retrieve("PWorkID", pworkID, "FK_Flow", subFlow.SubFlowNo); //流程.          
        else
            gwfs.Retrieve("PWorkID", pworkID, "FK_Flow", subFlow.SubFlowNo, "Starter", webUser.No); //流程.

        for (var j = 0; j < gwfs.length; j++) {

            var item = gwfs[j];
            if (item.WFState == 0) continue;

            //平级子流程，获取平级的workID
            var slWorkID = GetPara(item.AtPara, "SLWorkID");
            if (slWorkID != null && slWorkID != undefined && slWorkID != GetQueryString("WorkID"))
                continue;

            _Html += "<tr>";
            if (item.StarterName == null || item.StarterName == "")
                _Html += "<td nowrap>&nbsp;</td>";
            else
                _Html += "<td nowrap>" + item.StarterName + "</td>";

            if (item.TodoEmps.indexOf( webUser.No + "," + webUser.Name + ";" ) >= 0) {
                _Html += "<td  style='word-break:break-all;' title='" + item.Title + "'>";
                _Html += "<a href=\"javascript:OpenIt('" + imgbasePath + "MyView.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "&IsCheckGuide=1&Frms=" + item.Paras_Frms + "&FK_Node=" + item.FK_Node + "&PNodeID=" + item.PNodeID + "&PWorkID=" + item.PWorkID + "')\" ><img src='" + imgbasePath + "Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a></td>";
            } else {
                if (sf.SFOpenType == 0) {
                    _Html += "<td  style='word-break:break-all;' title='" + item.Title + "'>";
                    _Html += "<a href=\"javascript:OpenIt('" + imgbasePath + "WFRpt.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "&PWorkID=" + item.PWorkID + "&PFlowNo=" + item.PFlowNo + "&PNodeID=" + item.PNodeID + "')\" ><img src='" + imgbasePath + "Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a></td>";
                } else {
                    _Html += "<td style='word-break:break-all;' title='" + item.Title + "'>";
                    _Html += "<a href=\"javascript:OpenIt('" + imgbasePath + "MyView.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "')\" ><img src='" + imgbasePath + "Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a></td>";
                }
            }
            //到达节点名称.
            if (item.NodeName == null || item.NodeName == "")
                _Html += "<td nowrap>&nbsp;</td>";
            else
                _Html += "<td nowrap>" + item.NodeName + "</td>";
            //流程的状态 
           
                _Html += "<td nowrap>" + GetState(item.WFState) + "</td>";


            var emps = item.TodoEmps.split(';');
            var myemps = "";

            for (var idx = 0; idx < emps.length; idx++) {

                var empstrs = emps[idx];
                if (empstrs == null)
                    continue;

                if (empstrs == '' || empstrs.length == 0 || empstrs == null)
                    continue;

                empstrs = emps[idx].split(',');
                myemps += "" + empstrs[1] + ",";
            }


            //到达人员.
            _Html += "<td title='" + item.TodoEmps + "'>" + myemps + "</td>";

            //日期.
            if (item.RDT == null || item.RDT == "")
                _Html += "<td nowrap>&nbsp;</td>";
            else
                _Html += "<td nowrap>" + item.RDT + "</td>";

            //流程备注.
            //if (item.FlowNote == null)
            //    _Html += "<td title='" + item.FlowNote + "'></td>";
            //else
            //    _Html += "<td title='" + item.FlowNote + "'>" + item.FlowNote + "</td>";

            _Html += "</tr>";


        }
    }


    _Html += "</table>";
    return _Html;

}

//删除子流程
function DeleteSubFlowDraf(workid, flowNo) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("WorkID", workid);
    handler.AddPara("FK_Flow", flowNo);
    var data = handler.DoMethodReturnString("SubFlowGuid_DeleteSubFlowDraf");
    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }

    $("#" + workid).remove();
}

function InsertSubFlows(flowNo, fid, workid, layer, html) {

    //该流程的子流程信息, 并按照流程排序.
    var gwfs = new Entities("BP.WF.GenerWorkFlows");
    gwfs.Retrieve("PWorkID", workid, "FK_Flow"); //流程.
    if (gwfs.Count == 0)
        return;

    var currUrl = GetHrefUrl();

    if (currUrl.indexOf("Admin/FoolFormDesigner/Designer.htm") != -1)
        imgbasePath = "../../";

    var myFlowNo = "";
    var item = null;
    for (var i = 0; i < gwfs.length; i++) {
        item = gwfs[i];
        if (item.WFState == 0)
            continue;

        if (myFlowNo.indexOf(item.FK_Flow) == -1) {
            myFlowNo = myFlowNo + "," + item.FK_Flow;

            //输出流程.
            var fl = new Entity("BP.WF.Flow", item.FK_Flow);
            var tdhtml = "<div style='float:left'>" + GenerSpace(layer * 2) + "<img src='" + imgbasePath + "Img/Max.gif' />&nbsp;" + fl.Name + "</div>";
            html += "<tr>";
            html += "<td class='TRSum' colspan=6>" + tdhtml + "</td>";
            html += "</tr>";
        }

        html += "<tr>";
        html += "<td style='word-break:break-all;' title='" + item.Title + "'> ";
        html += GenerSpace(layer * 2) + "<a href=\"javascript:OpenIt('" + imgbasePath + "WFRpt.htm?WorkID=" + item.WorkID + "&FK_Flow=" + item.FK_Flow + "')\" ><img src='" + imgbasePath + "Img/Dot.png' width='9px' />&nbsp;" + item.Title + "</a></td>";

        //到达节点名称.
        if (item.NodeName == null || item.NodeName == "")
            html += "<td nowrap>&nbsp;</td>";
        else
            html += "<td nowrap>" + item.NodeName + "</td>";


        if (item.WFState == 3)
            html += "<td nowrap>已完成</td>";
        else
            html += "<td nowrap>未完成</td>";

        //到达人员.
        htm += "<td title='" + item.TodoEmps + "'>" + item.TodoEmps + "</td>";

        //日期.
        if (item.RDT == null || item.RDT == "")
            html += "<td nowrap>&nbsp;</td>";
        else
            html += "<td nowrap>" + item.RDT + "</td>";

        //流程备注.
       // htm += "<td title='" + item.FlowNote + "'>" + item.FlowNote + "</td>";

        html += "</tr>";
        //加载他下面的子流程.
        InsertSubFlows(item.FK_Flow, item.FK_Node, item.WorkID, layer + 1, html);
    }
    return html;
}

function GenerSpace(spaceNum) {
    if (spaceNum <= 0)
        return "";

    var strs = "";
    while (spaceNum != 0) {
        strs += "&nbsp;&nbsp;";
        spaceNum--;
    }
    return strs;
}

function RSize() {
    if (document.body.scrollWidth > (window.screen.availWidth - 100)) {
        window.dialogWidth = (window.screen.availWidth - 100).toString() + "px"
    } else {
        window.dialogWidth = (document.body.scrollWidth + 50).toString() + "px"
    }

    if (document.body.scrollHeight > (window.screen.availHeight - 70)) {
        window.dialogHeight = (window.screen.availHeight - 50).toString() + "px"
    } else {
        window.dialogHeight = (document.body.scrollHeight + 115).toString() + "px"
    }
    window.dialogLeft = ((window.screen.availWidth - document.body.clientWidth) / 2).toString() + "px"
    window.dialogTop = ((window.screen.availHeight - document.body.clientHeight) / 2).toString() + "px"
}

function NoSubmit(ev) {
    if (window.event.srcElement.tagName == "TEXTAREA")
        return true;

    if (ev.keyCode == 13) {
        window.event.keyCode = 9;
        ev.keyCode = 9;
        return true;
    }
    return true;
}

function OpenIt(url) {
    var newWindow = window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
    newWindow.focus();
    var loop = setInterval(function () {
        if (newWindow.closed) {
            clearInterval(loop);
            parent.location.reload();

        }

    }, 1);
    return;
}
