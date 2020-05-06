/*
1. 该JS文件被嵌入到了MyFlowGener.htm 的工作处理器中. 
2. 开发者可以重写该文件处理通用的应用,比如通用的函数.

*/

//外发公文.
function WaiFa() {

    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var nodeid = GetQueryString('NodeID');


    var url = "../App/WaiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&NodeID=" + nodeid;
    WinOpen(url);
    //window.open(url);
}

//转内发公文.
function NeiFa()
{
    var workid = GetQueryString('WorkID');
    var flowNo = GetQueryString('FK_Flow');
    var mypk = GetQueryString('MyPK');
    var pkVal = GetQueryString('PKVal');

    var url = "../App/NeiFa.htm?WorkID=" + workid + "&FK_Flow=" + flowNo + "&MyPK=" + pkVal;
    WinOpen(url);
}

function DZ() {

    alert('sss');
    var url = 'pop.htm';
    window.open(url);
}

/*

1. beforeSave、beforeSend、 beforeReturn、 beforeDelete 
2 .MyFlowGener、MyFlowTree的固定方法，禁止删除
3.主要写保存前、发送前、退回前、删除前事件
4.返回值为 true、false

*/

//保存前事件
function beforeSave() {
    return true;
}

//发生前事件
function beforeSend() {
    return true;
}

//退回前事件
function beforeReturn() {
    return true;
}

//删除前事件
function beforeDelete() {
    return true;
}


//抄送阅读页面增加关闭前事件
function beforeCCClose() {
    return true;
}

