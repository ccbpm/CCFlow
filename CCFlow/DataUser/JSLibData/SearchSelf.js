
function FlowTemplateNew() {
    var url = basePath + "/WF/Admin/CCBPMDesigner/FlowDevModel/Default.htm";
    window.open(url);
    return;
}

function FlowTemplateImp() {
    var url = basePath + "/WF/Admin/CCBPMDesigner/FlowDevModel/Default.htm";
    window.open(url);
    return;
}


function DealFlowEmps(str) {
    return str;
}


/**
 * 新建流程模板
 * */
function NewFlowTemplate() {
    var url = "/WF/Admin/CCBPMDesigner/FlowDevModel/Default.htm?SortNo=100&From=Flows.htm";
    SetHref(url);
    return;

    //  vm.open(url);
    // return;
    // var url = "/App/FlowDesigner/NewFlow.htm?EnsName=BP.Cloud.Template.FlowExts";
    SetHref(url);
}

function ImpFlowTemplate() {

    //alert();
    //var url = "/App/FlowDesigner/NewFlow/Default.htm";
    var url = "/WF/Comm/Search.htm?EnsName=BP.Cloud.Template.FlowExts";
    var url = "/App/FlowDesigner/Template.htm?EnsName=BP.Cloud.Template.FlowExts";
    SetHref(url);
}
function DelTodoEmps(str) {
    return str;
}
