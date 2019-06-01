$(function () {

    var html = "";

    var handler = new HttpHandler("WF_WorkOpt_OneWork");
    handler.AddPara("WorkID", workid);
    var ds = handler.DoMethodReturnJSON("JobSchedule_Init");

    var gwf = ds["WF_GenerWorkFlow"]; //工作记录.
    var nodes = ds["WF_Node"]; //节点.
    var dirs = ds["WF_Direction"]; //连接线.
    var tracks = ds["Track"]; //历史记录.

    //循环历史记录.     
    for (var i = 0; i < tracks.length; i++) {
        var tk = tracks[i];
        html += "@已经完成的节点:" + tk.NodeName;
    }

    //流程未完成的状态.
    if (gwf.WFState != 3) {

        //当前停留的节点.
        var currNode = gwf.FK_Node;

        for (var i = 0; i < 100; i++) {

            var nextNode = GetNextNodeID(currNode, dirs);
            if (nextNode == 0)
                break;

            html += "@未完成的节点:" + nextNode;
            currNode = nextNode;
        }
    }
    //html += "<img src='./Admin/FoolFormDesigner/Img/JobSchedule.png' />";

    $("#JobSchedule").html(html);
    // alert('sss');
    return;
});

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
      
      
