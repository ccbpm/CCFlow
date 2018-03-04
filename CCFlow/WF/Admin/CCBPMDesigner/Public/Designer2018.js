//全局变量

//流程属性.
function FlowProperty() {
    url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PK=" + flowNo + "&Lang=CH";
    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 1000, 550, "icon-property", true, null, null, null, function () {
        //window.location.href = window.location.href;
    });
}

//报表设计.
function FlowRpt() {

    var flowId = Number(flowNo);
    
    flowId = String(flowId);

    url = "../RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 990, 500, "icon-property", true, null, null, null, function () {

    });
}



//运行流程
function FlowRun() {
    var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    OpenEasyUiDialog(url, "eudlgframe", '流程测试运行', 900, 500, "icon-property", true, null, null, null, function () {
        //window.location.href = window.location.href;
    });
}

//旧版本.
function OldVer() {
    var url = "Designer.htm?FK_Flow=" + flowNo + "&Lang=CH";
    window.location.href = url;
}

function NewNode

/***********************  节点信息. ****************/

//节点属性
function NodeAttr(nodeID) {
    url = "../../Comm/En.htm?EnsName=BP.WF.Template.NodeExts&NodeID=" + nodeID + "&Lang=CH";
    WinOpen(url);
}


//节点方案
function NodeFrmSln(nodeID) {

    var url = "../AttrNode/NodeFromWorkModel.htm?FK_Node=" + nodeID;
    WinOpen(url);
   
    ajaxModal(url, function () {
        //  window.location.href = window.location.href;
    });
}

  
 