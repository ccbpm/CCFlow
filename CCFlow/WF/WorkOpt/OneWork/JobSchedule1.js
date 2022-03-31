var step = 0;
var gwf;
$(function () {

    var workid = GetQueryString("WorkID");
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddPara("WorkID", workid);
    var ds = handler.DoMethodReturnJSON("JobSchedule_Init");

    gwf= ds["WF_GenerWorkFlow"][0]; //工作记录.
    var tempNodes = ds["WF_Node"]; //节点.
    var tempDirs = ds["WF_Direction"]; //连接线.
    var tracks = ds["Track"]; //历史记录.
    var transf = ds["WF_TransferCustom"];//游离态的节点

    var nodes = {};
    for (var i = 0; i < tempNodes.length; i++) {
        var nodeID = tempNodes[i].NodeID;
        if (!$.isArray(nodes[nodeID])) {
            nodes[nodeID] = [];
        }
        nodes[nodeID].push(tempNodes[i]);
    }
    var dirs = {};
    //每个节点的连线
    for (var i = 0; i < tempNodes.length; i++) {
        var nodeDir = [];
        var nodeID = tempNodes[i].NodeID;
        for (var k = 0; k < tempDirs.length; k++) {
            if (tempDirs[k].Node == nodeID)
                nodeDir.push({ ToNodeID: tempDirs[k].ToNode });
            else
                continue;
        }
        if (nodeDir.length == 0)
            continue;
        if (!$.isArray(dirs[nodeID])) {
            dirs[nodeID] = [];
        }
        dirs[nodeID].push(nodeDir);
    }
    var step = 0;
    //获取走过的节点, 生成唯一的节点连续字符串比如 101,102,103
    var nds = "";
    for (var i = 0; i < tracks.length; i++) {
        var tk = tracks[i];
        if (nds.indexOf(tk.FK_Node) != -1)
            continue;
        nds += "," + tk.FK_Node;
    }

    //把节点转化为数组.
    var nds = nds.split(",");

    for (var i = 0; i < nds.length; i++) {
        var nodeID = nds[i];
        if (nodeID == "")
            continue;
        var currTrack = getTracksByNodeID(tracks,nodeID);
        if (currTrack.length != 0) {//$(".DashbCon")
            $("#JobSchedule").append('<div class="alone '+ (currTrack[0].IsPass == 0?"gray":currTrack[0].IsPass ==1?"green":"red") +'"><div class="circleDot finishType"><div class="circle"><div></div>'+ (currTrack[0].NodeName?currTrack[0].NodeName:'') +'</div></div><div class="figure"></div><div class="contentCon"><p>'+ (currTrack[0].EmpName == null?'':currTrack[0].EmpName == 0?'':currTrack[0].EmpName)+'</p><p>'+ (currTrack[0].SDT == null?'':currTrack[0].SDT == 0?'':currTrack[0].SDT) +'</p></div></div>')
        }

    }
    //流程未完成的状态, 输出没有经过的节点。
    if (gwf.WFState != 3) {

        //当前停留的节点.
        var currNodeID = gwf.FK_Node;// $(".DashbCon")
       $("#JobSchedule").append('<div class="alone gray"><div class="circleDot finishType"><div class="circle"><div></div>' + nodes[currNodeID][0].Name + '</div></div><div class="figure"></div><div class="contentCon"></div>');
        //递归获取未运行到的节点
        ShowNextNode_DiGui(currNodeID, dirs, transf, nodes, gwf);

    }

    return;
});


//根据tracks、nodeID获取当前节点的运行信息
function getTracksByNodeID(tracks,nodeID){
    var curTrack=[];
    for(var i =0;i<tracks.length;i++){
        var tk = tracks[i];
        if(tk.FK_Node != nodeID)
            continue;
        else
            curTrack.push(tk) ;
    }
    return curTrack;
}


//根据当前节点获得下一个节点.
function GetNextNodeID(nodeID, dirs) {

    var toNodeID = [];
    var nodeDirs = dirs[nodeID];
    if (nodeDirs == undefined)
        return toNodeID;
    for (var i = 0; i < nodeDirs.length; i++) {
        
        toNodeID += "," + nodeDirs[i].ToNodeID;
    }
    toNodeID = toNodeID.substr(1,toNodeID.length);
    toNodeID =toNodeID.split(",");
    return toNodeID;
}

//游离态的计算
function GetNextYouLiTaiNodeID(nodeID, transfs, dirs,nodes) {
    var toNodeID = [];
    var isMeet = false;
    //获取到他下一个自定义节点
    for (var i = 0; i < transfs.length; i++) {
        var transf = transfs[i];
        if (transf.FK_Node == nodeID) {
            isMeet = true;
            continue;
          
        }
        if (isMeet == true && transf.IsEnable == true) {
            toNodeID[0] = transf.FK_Node;
            return toNodeID;
        }
       
    }
    var currNode = nodes[nodeID][0]; //当前节点不是游离态
    if (currNode.AtPara.indexOf("IsYouLiTai=0") != -1 && gwf.IsAutoRun == 0) {
        for (var i = 0; i < transfs.length; i++) {
            var transf = transfs[i];
            if (transf.IsEnable == true && transf.FK_Node != nodeID) {
                toNodeID[0] = transf.FK_Node;
                return toNodeID;
            }
               
        }
    }

    //该节点是最后一个自定义节点，下一个节点
    if (isMeet == true) {
        var nodeDirs = dirs[nodeID][0];
        if (nodeDirs.length == 0) //流程最后一个节点
            return "";
        for (var i = 0; i < nodeDirs.length; i++) {
            var nextNode = nodes[nodeDirs[i].ToNodeID][0];
            if (nextNode.AtPara.indexOf("IsYouLiTai=1") != -1)
                continue;
            else {
                toNodeID[0] = nodeDirs[i].ToNodeID;
                gwf.IsAutoRun = 1;
                return toNodeID;
            }
               
        }

    }
    return toNodeID;

}

//根据NodeID 获取Node信息
function GetNodeByNodeID(nodes,nodeID){
    for(var i =0 ;i<nodes.length;i++){
        if(nodes[i].NodeID == nodeID)
            return nodes[i];
    }
}
var haveMsNode="";
function ShowNextNode_DiGui(currNodeID, dirs,transf,nodes,gwf){
    for (var i = 0; i < 100; i++) {
        var currNode = nodes[currNodeID][0];
        var nextNodes = "";
        if (currNode.AtPara.indexOf("IsYouLiTai=1") != -1)
            nextNodes = GetNextYouLiTaiNodeID(currNodeID, transf, dirs, nodes);
        else
            if (gwf.AtPara.indexOf("IsAutoRun==0"))
                nextNodes = GetNextYouLiTaiNodeID(currNodeID, transf, dirs, nodes);
            else
                nextNodes = GetNextNodeID(currNodeID, dirs);

        
        if (nextNodes.length == 0) //最后一个节点
            break;
        //拼接其余节点
        if(nextNodes.length==1){
            haveMsNode += ","+nextNodes[0];
            var node = nodes[nextNodes[0]][0];
            if (node != undefined)
                $(".DashbCon").append('<div class="alone gray"><div class="circleDot finishType"><div class="circle"><div></div>'+ node.Name +'</div></div><div class="figure"></div><div class="contentCon"></div>');
            currNodeID = nextNodes[0];
        }else {
                haveMsNode += ","+nextNodes[0];
            $(".DashbCon").append('<div class="multiple"><div class="figure" style="background: gray"></div><div>');
            for (var idx = 0; idx < nextNodes.length; idx++) {
                var node = nodes[nextNodes[idx]][0];
                $(".multiple").append('<div class="daughter gray"><div class="roundDot finishType"><div class="circle"><div></div>'+ node.Name +'</div><span class="name"></span><div class="data"> </div>');
                    //ShowNextNode_DiGui(nextNodes[idx],dirs,nodes);
            }
        }

    }
}

//游离态节点的递归
function ShowNextYouLiTai_DiGui(currNodeID, transf, dirs, nodes) {
    for (var i = 0; i < 100; i++) {
        var currNode = nodes[currNodeID][0];
        var nextNodes = "";
        if(currNode.AtPara.indexOf("IsYouLiTai")!=-1)
            nextNodes = GetNextYouLiTaiNodeID(currNodeID, transf, dirs, nodes);
        else
            nextNodes = GetNextNodeID()

        if (nextNodes.length == 0) //最后一个节点
            break;
        //拼接其余节点
        if (nextNodes.length == 1) {
            haveMsNode += "," + nextNodes[0];
            var node = GetNodeByNodeID(nodes, nextNodes[0]);
            if (node != undefined)
                $(".DashbCon").append('<div class="alone gray"><div class="circleDot finishType"><div class="circle"><div></div>' + node.Name + '</div></div><div class="figure"></div><div class="contentCon"></div>');
            currNodeID = nextNodes[0];
        } else {
            haveMsNode += "," + nextNodes[0];
            $(".DashbCon").append('<div class="multiple"><div class="figure" style="background: gray"></div><div>');
            for (var idx = 0; idx < nextNodes.length; idx++) {
                var node = GetNodeByNodeID(nodes, nextNodes[idx]);
                $(".multiple").append('<div class="daughter gray"><div class="roundDot finishType"><div class="circle"><div></div>' + node.Name + '</div><span class="name"></span><div class="data"> </div>');
                //ShowNextNode_DiGui(nextNodes[idx],dirs,nodes);
            }
        }

    }
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
