
function NewFlow(flowNo) {
    var url = "../MyFlow.htm?FK_Flow=" + flowNo;
    window.open(url);
    return;
}

function DealFlowEmps(str) {
    return str;
}

/* 
使用说明:
1. 该文件被自动的载入到 /WF/Comm/Search.htm 中.
2. 你可以在这里写入自己的方法函数被工具栏上的按钮调用, 来完成高级的js操作.

 */

function NewFlowTemplate() {
    //   alert();
   //var url = "/App/FlowDesigner/NewFlow/Default.htm";
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
