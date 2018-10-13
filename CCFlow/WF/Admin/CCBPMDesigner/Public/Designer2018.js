//全局变量
function WinOpen(url) {
    window.open(url);
}

//流程属性.
function FlowProperty() {
    url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PKVal=" + flowNo + "&Lang=CH";

    //OpenEasyUiDialogExt(url, "流程属性", 900, 500, false);
    window.parent.addTab(flowNo, "流程属性" + flowNo, url);

    //  WinOpen(url);
    //    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 1000, 550, "icon-property", true, null, null, null, function () {
    //        //window.location.href = window.location.href;
    //    });
}

//报表设计.
function FlowRpt() {

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";

    //OpenEasyUiDialogExt(url, "报表设计", 900, 500, false);
    window.parent.addTab(flowNo + "_BBSJ", "报表设计" + flowNo, url);
}

//检查流程.
function FlowCheck() {

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../AttrFlow/CheckFlow.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    // WinOpen(url);
    OpenEasyUiDialog(url, "FlowCheck" + flowNo, "检查流程" + flowNo, 600, 500, "icon - library", false);
    //window.parent.addTab(flowNo + "_JCLC", "检查流程" + flowNo, url);
}

//运行流程
function FlowRun() {

    //执行流程检查.
    var flow = new Entity("BP.WF.Flow", flowNo);
    flow.DoMethodReturnString("ClearCash");

    var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    //WinOpen(url);
    window.parent.addTab(flowNo + "_YXLH", "运行流程" + flowNo, url);
}
//运行流程
function FlowRunAdmin() {

    //执行流程检查.
    var flow = new Entity("BP.WF.Flow", flowNo);
    flow.DoMethodReturnString("ClearCash");

    //var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    var webUser = new WebUser();
    var url = "../TestFlow.htm?DoType=TestFlow_ReturnToUser&DoWhat=StartClassic&UserNo=" + webUser.No + "&FK_Flow=" + flowNo;
    //  var url = "../../MyFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    WinOpen(url);
    //window.parent.addTab(flowNo + "_YXLH", "运行流程" + flowNo, url);
}

//旧版本.
function OldVer() {
    var url = "Designer2016.htm?FK_Flow=" + flowNo + "&Lang=CH&&Flow_V=1";
    window.location.href = url;
}

function Help() {

    var msg = "<ul>";
    msg += "<li>开发者:济南驰骋信息技术有限公司.</li>";
    msg += "<li>官方网站: <a href='http://www.ccflow.org' target=_blank>http://ccflow.org</a></li>";
    msg += "<li>商务联系:0531-82374939, 微信:18660153393 QQ:793719823</li>";
    msg += "<li>地址:济南是高新区齐鲁软件大厦A座408室.</li>";
    msg += "<li>公众帐号<img src='' border=0/></li>";
    msg += "</ul>";
    mAlert(msg, 20000);
}

/***********************  节点信息. ******************************************/

//节点属性
function NodeAttr(nodeID) {

    //var url = "../../Comm/RefFunc/EnV2.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    var html = "";

    //var html = "<a href=\"javascript:OpenEasyUiDialogExt('" + url + "','';\" >主页</a> - ";
    window.parent.addTab(nodeID, "节点属性" + nodeID, url);
    //OpenEasyUiDialogExt(url, html+"属性", 900, 500, false);
}
//节点属性
function NodeAttrOld(nodeID) {
    var url = "../../Comm/En.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    window.parent.addTab(nodeID, "节点属性" + nodeID, url);
    //OpenEasyUiDialogExt(url, "节点属性", 800, 500, false);
}

//表单方案
function NodeFrmSln(nodeID) {
    //表单方案.
    var url = "../AttrNode/FrmSln/Default.htm?FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_JDFA", "表单方案" + nodeID, url);
    // OpenEasyUiDialogExt(url, "表单方案", 800, 500, false);
}


//设计表单
function NodeFrmD(nodeID) {

    var node = new Entity("BP.WF.Node", nodeID);
    if (node.FormType == 1) {
        NodeFrmFree(nodeID);
        return;
    }

    NodeFrmFool(nodeID);
}

function NodeFrmFool(nodeID) {
    //傻瓜表单.
    var url = "../FoolFormDesigner/Designer.htm?FK_MapData=ND" + nodeID + "&IsFirst=1&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    //WinOpen(url);
    window.parent.addTab(nodeID + "_Fool", "设计表单" + nodeID, url);
}

function NodeFrmFree(nodeID) {

    //自由表单.
    var url = "../CCFormDesigner/FormDesigner.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_Free", "设计表单" + nodeID, url);
    ///CCFormDesigner/FormDesigner.htm?FK_Node=9502&FK_MapData=ND9502&FK_Flow=095&UserNo=admin&SID=c3466cb7-edbe-4cdc-92df-674482182d01
    //WinOpen(url);
}

//接受人规则.
function NodeAccepterRole(nodeID) {
    //接受人规则.
    var url = "../AttrNode/AccepterRole/Default.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.parent.addTab(nodeID + "_JSGZ", "接受人规则" + nodeID, url);
    //OpenEasyUiDialogExt(url, "接受人规则", 800, 500, false);
}

function Reload() {
    window.location.href = window.location.href;
}


//打开.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });
}