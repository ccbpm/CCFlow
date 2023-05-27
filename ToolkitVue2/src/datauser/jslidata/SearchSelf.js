
function NewFlow(flowNo) {
    var url = "../MyFlow.htm?FK_Flow=" + flowNo;
    window.open(url);
    return;
}

function DealFlowEmps(str) {
    return str;
}



function NewFlowTemplate() {
    var url = "/WF/Comm/Search.htm?EnsName=BP.Cloud.Template.FlowExts";
    var url = "/App/FlowDesigner/NewFlow.htm?EnsName=BP.Cloud.Template.FlowExts";

    window.location.href = url;
}

function ImpFlowTemplate() {

    //alert();
    //var url = "/App/FlowDesigner/NewFlow/Default.htm";
    var url = "/WF/Comm/Search.htm?EnsName=BP.Cloud.Template.FlowExts";
    var url = "/App/FlowDesigner/Template.htm?EnsName=BP.Cloud.Template.FlowExts";
    window.location.href = url;
}
function DelTodoEmps(str) {
    return str;
}
