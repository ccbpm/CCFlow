
/*
1. 该页面,是被引用到 /WF/MyFlowGener.htm, /WF/CCForm/FrmGener.htm 里面的.
2. 这里方法大多是执行后，返回json ,可以被页面控件调用. 
*/
function funDemo() {
    alert("我被执行了。");
}

//FK_MapData,附件属性，RefPK,FK_Node
function afterDtlImp(FK_MapData, frmAth, newOID, FK_Node, oldOID) {
    //处理从表附件导入的事件
}
                  