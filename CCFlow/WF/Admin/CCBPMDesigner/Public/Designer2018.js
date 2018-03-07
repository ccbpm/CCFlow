﻿//全局变量

function WinOpen(url) {
    window.open(url);
}

//流程属性.
function FlowProperty() {
    url = "../../Comm/En.htm?EnName=BP.WF.Template.FlowExt&PK=" + flowNo + "&Lang=CH";
    WinOpen(url);

    //    OpenEasyUiDialog(url, "eudlgframe", '流程属性', 1000, 550, "icon-property", true, null, null, null, function () {
    //        //window.location.href = window.location.href;
    //    });
}

//报表设计.
function FlowRpt() {

    var flowId = Number(flowNo);
    flowId = String(flowId);
    url = "../RptDfine/Default.htm?FK_Flow=" + flowNo + "&FK_MapData=ND" + flowId + "MyRpt";
    WinOpen(url);
}

//运行流程
function FlowRun() {
    var url = "../TestFlow.htm?FK_Flow=" + flowNo + "&Lang=CH";
    WinOpen(url);
}

//旧版本.
function OldVer() {
    var url = "Designer.htm?FK_Flow=" + flowNo + "&Lang=CH";
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
    var url = "../../Comm/RefFunc/EnV2.htm?EnName=BP.WF.Template.NodeExt&NodeID=" + nodeID + "&Lang=CH";
    WinOpen(url);
}
//节点属性
function NodeAttrOld(nodeID) {
    var url = "../../Comm/En.htm?EnsName=BP.WF.Template.NodeExts&NodeID=" + nodeID + "&Lang=CH";
    WinOpen(url);
}

//节点方案
function NodeFrmSln(nodeID) {
    //表单方案.
    var url = "../AttrNode/NodeFromWorkModel.htm?FK_Node=" + nodeID;
    WinOpen(url);
}

function NodeFrmFool(nodeID) {

    //表单方案.
    var url = "../FoolFormDesigner/Designer.htm?FK_MapData=ND203&FK_Flow="+flowNo+"&FK_Node=" + nodeID;
    WinOpen(url);
}

function NodeFrmFree(nodeID) {

    //表单方案.
    var url = "../CCFormDesigner/FormDesigner.htm?FK_MapData=ND"+nodeID+"&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;

    ///CCFormDesigner/FormDesigner.htm?FK_Node=9502&FK_MapData=ND9502&FK_Flow=095&UserNo=admin&SID=c3466cb7-edbe-4cdc-92df-674482182d01
    WinOpen(url);
}

//接受人规则.
function NodeAccepterRole(nodeID) {
    //表单方案.
    var url = "../AttrNode/AccepterRole/Default.htm?FK_MapData=ND" + nodeID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    WinOpen(url);
}

 