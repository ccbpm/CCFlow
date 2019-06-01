$(function () {

    var workid = GetQueryString("WorkID");

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddPara("WorkID", workid);

    var ds = handler.DoMethodReturnJSON("JobSchedule_Init");

    var gwf = ds["WF_GenerWorkFlow"][0]; //工作记录.
    var nodes = ds["WF_Node"]; //节点.
    var dirs = ds["WF_Direction"]; //连接线.
    var tracks = ds["Track"]; //历史记录.

    var html = "<table style='width:100%;height:100px;'>";
    html += "<tr>";


    //循环历史记录.
    for (var i = 0; i < tracks.length; i++) {
        var tk = tracks[i];

        var info = "";
        if (tk.FK_Node == gwf.FK_Node)
            var info = "<img src='/WF/WorkOpt/OneWork/Img/DotGreen.png' />";
        else
            var info = "<img src='/WF/WorkOpt/OneWork/Img/DotBlue.png' />";

        info += "<br><b>" + tk.NodeName+"</b>";
        info += "<br>" + tk.EmpName;
        info += "<br>" + tk.RDT.substring(0, 16);
        html += "<td style='text-align:center'>" + info + "</td>";
    }
    debugger
    //流程未完成的状态.
    if (gwf.WFState != 3) {

        //当前停留的节点.
        var currNode = gwf.FK_Node;

        for (var i = 0; i < 100; i++) {

            var nextNode = GetNextNodeID(currNode, dirs);
            if (nextNode == 0)
                break;


            var info = "<img src='/WF/WorkOpt/OneWork/Img/DotGiay.png' />";
            var nodeName = "";
            for (var idx = 0; idx < nodes.length; idx++) {

                var nd = nodes[idx];
                if (nd.NodeID == nextNode) {
                    nodeName = nd.Name;
                    break;
                }
            }

            info += "<br>" + nodeName;

            html += "<td style='text-align:center'>" + info + "</td>";

           
            currNode = nextNode;
        }
    }

    var info = "<img src='/WF/WorkOpt/OneWork/Img/DotEnd.png' />";

    html += "<td style='text-align:center'>" + info + "<br>结束</td>";

    html += "</tr>";
    html += "</table>";

    //html += "<img src='./Admin/FoolFormDesigner/Img/JobSchedule.png' />";

    $("#JobSchedule").html(html);
    // alert('sss');
    return;
});
 

//根据当前节点获得下一个节点.
function GetNextNodeID(nodeID, dirs) {
    debugger

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
      
      
