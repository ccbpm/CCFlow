var StarStepNum = 0;
var step = 0;
$(function () {

    var v = $("#JobSchedule");
    if (v == null || v == undefined)
        return;

    var workid = GetQueryString("WorkID");
	var oid = GetQueryString("OID");
    if (workid==null) {
        workid = oid;
    }
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddPara("WorkID", workid);
    var ds = handler.DoMethodReturnJSON("JobSchedule_Init");
    var gwf = ds["WF_GenerWorkFlow"][0]; //工作记录.
    var nodes = ds["WF_Node"]; //节点.
    var dirs = ds["WF_Direction"]; //连接线.
    var tracks = ds["Track"]; //历史记录. 

    var html = "<div class='step'>";
    html += "<tr>";

    var step = 0;
    //循环历史记录, 生成唯一的节点连续字符串比如 101,102,103
    var nds = "";
    for (var i = 0; i < tracks.length; i++) {
        var tk = tracks[i];
        if (nds.indexOf(tk.FK_Node) != -1)
            continue;
        if (tk.ActionType == 2)
            continue;

        nds += "," + tk.FK_Node;
    }

    //把节点转化为数组.
    var nds = nds.split(",");

    for (var i = 0; i < nds.length; i++) {
        var nodeID = nds[i];
        if (nodeID == "")
            continue;

        var nodeNums = 0;
        for (var myidx = 0; myidx < tracks.length; myidx++) {
            var tk = tracks[myidx];
            if (tk.FK_Node == nodeID)
                nodeNums++;
        }

        //如果是单个节点.
        if (nodeNums == 1) {
            html += GenerSingerNode(tracks, nodeID, gwf);
            continue;
        }

        if (nodeNums > 1) { //多个人的节点.
            html += GenerMNode(tracks, nodeID, gwf);
            continue;
        }
    }

    //流程未完成的状态, 输出没有经过的节点。
    if (gwf.WFState != 3) {

        //当前停留的节点.
        var currNode = gwf.FK_Node;

        var nodeName = "";
        for (var i = 0; i < 100; i++) {

            var nextNode = GetNextNodeID(currNode, dirs);
            if (nextNode == 0)
                break;

            for (var idx = 0; idx < nodes.length; idx++) {
                var nd = nodes[idx];

                if (nd.NodeID == nextNode) {
                    nodeName = nd.Name;
                    break;
                }
            }

            var doc = "<b>-</b>";
            doc += "<br>";
            doc += "<br>";
            doc += "<br>";

            //  doc = "";

            step = step + 1;
            currNode = nextNode;

            if (nextNode == 0)
                var info = GenerIcon("DotEnd", step, doc, true, nodeName);
            else
                var info = GenerIcon("DotEnd", step + StarStepNum, doc, false, nodeName);

            html += "<td style='text-align:center;vertical-align:top;'>" + info + "</td>";

            if (nextNode == 0)
                break;
        }
    }


    html += "</tr>";
    html += "</table>";

    $("#JobSchedule").html(html);
    return;
});


//生成多个节点处理人. .
function GenerMNode(tracks, nodeID, gwf) {

    step = step + 1;
    var info = "<ul>";

    var emps = "";
    var track;
    for (var i = 0; i < tracks.length; i++) {

        var tk = tracks[i];
        if (tk.FK_Node != nodeID) continue;
        if (emps.indexOf(tk.EmpNo + ',') >= 0) continue; //已经出现的，就不处理了.
        track = tk;

        emps += tk.EmpNo + ",";

        if (tk.IsPass == 1) {
            info += "<ol><font color=blue><b>" + tk.DeptName + " " + tk.EmpName + "</b></font> " + tk.RDT.substring(5, 16) + "</ol>";
        }
        else {
            info += "<ol>" + tk.DeptName + " " + tk.EmpName + "</ol>";
        }
    }
    info += "</ul>";
    if(track != null && track != undefined) {
        if (track.FK_Node == gwf.FK_Node)
            info = GenerIcon("DotGreen", step, info, false, track.NodeName);
        else
            info = GenerIcon("DotBlue", step, info, false, track.NodeName);
    }
   

    return "<td style='text-align:center;vertical-align:top;'>" + info + "</td>";

}


//生成单个节点的样式风格.
function GenerSingerNode(tracks, nodeID, gwf) {

    for (var i = 0; i < tracks.length; i++) {

        var tk = tracks[i];
        if (tk.FK_Node != nodeID)
            continue;

        var doc = "";
        doc += "<br>" + tk.EmpName;
        doc += "<br>" + tk.RDT.substring(0, 16);

        step = i + 1;

        var info = "";
        if (tk.FK_Node == gwf.FK_Node)
            info = GenerIcon("DotGreen", step, doc, false, tk.NodeName);
        else
            info = GenerIcon("DotBlue", step, doc, false, tk.NodeName);
        StarStepNum = step;
        return "<td style='text-align:center;vertical-align:top;'>" + info + "</td>";
    }
}

function GenerIcon(icon, step, docs, isEndNode, nodeName) {

    var url = basePath + "/WF/WorkOpt/OneWork/Img/" + icon + "-" + step + ".png";


    var barUrlLeft = "";
    var barUrlRight = "";

    if (icon == 'DotGreen') {
        barUrlRight = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGreen.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
        barUrlLeft = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGreen.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
    }

    if (icon == "DotBlue") {

        barUrlRight = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGreen.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
        barUrlLeft = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGreen.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
    }

    if (icon == 'DotEnd') {
        barUrlRight = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGiay.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
        barUrlLeft = "<img src='" + basePath + "/WF/WorkOpt/OneWork/Img/BarGiay.png' style='width:100%;margin-right:0px;margin-left:0px;padding-left:0px;padding-right:0px;' />";
    }

    if (isEndNode == true)
        barUrlRight = "";

    var html = "";
    html += "<table style='height:100px;width: 100%; table-layout: fixed;border:none;margin:0px; padding:0px;'>";
    html += "<tr>";
    html += "<td style='border:none;margin:0px; padding:0px;width:40%;text-align:center;vertical-align:top;background-image: url('" + url + "'); background-repeat: no-repeat; background-attachment: fixed; background-position: center center'><table style='border:none;'><tr><td style='border:none;'><img src='" + url + "' style='width:18px;'/></td></tr><tr><td style='border:none;'><nobr>" + nodeName + "</nobr></td></tr><tr><td style='border:none;'>" + barUrlRight + "</td></tr><tr><td style='border:none;'>" + docs + "</td></tr></table></td>";
    html += "<tr>";
    html += "</table>";

    return html;
}

function GenerStart() {
    var str = "<div><img src='" + basePath + "/WF/WorkOpt/OneWork/Img/DotGreen1.png' /></div>";
}

//根据当前节点获得下一个节点.
function GetNextNodeID(nodeID, dirs) {
    var toNodeID = 0;
    for (var i = 0; i < dirs.length; i++) {
        var dir = dirs[i];
        if (dir.Node == nodeID) {
            toNodeID = dir.ToNode;
            break;
        }
    }

    var toNodeID2 = 0;
    for (var i = 0; i < dirs.length; i++) {
        var dir = dirs[i];
        if (dir.Node == nodeID) {
            toNodeID2 = dir.ToNode;
        }
    }

    //两次去的不一致，就有分支，有分支就reutrn 0 .
    if (toNodeID2 == toNodeID)
        return toNodeID;

    return 0;
}

function GetNextNodeIDExpSpecNode_del(nodeID, specToNode, dirs) {

    for (var i = 0; i < dirs.length; i++) {
        var dir = dirs[i];
        if (dir.Node == currNode) {

            if (dir.ToNode == specToNode)
                return 0;
        }
    }
    return 0;
} 
