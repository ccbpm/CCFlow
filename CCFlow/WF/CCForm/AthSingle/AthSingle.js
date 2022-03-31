/**
 *  加载该表单的单附件
 *  1. 这个js可以被加载的sdk表单上,如果在sdk表单上.
 *  
 * */
function AthSingle(frmID, pkval) {

    // 首先获得：div Name=AthSingle 的元素.
    var athDivs = $("div[name=AthSingle]"); //这个语法不一定对.

    if (athDivs.length == 0)
        return; // 如果元素为0 则执行 return.

    for (var i = 0; i < athDivs.length; i++) {
        var athDiv = athDivs[i];


    }

    //var nodeID = GetQueryString("FK_Node");
    //var workID = GetQueryString("WorkID");
    //var node = new Entity("BP.WF.Template.NodeExt", nodeID);

    ////状态  0=不启用,1=可编辑,2=不可编辑.
    //var sta = node.OfficeBtnEnable;
    //if (sta == 0)
    //    return; 
    //var div = $("#GovDocFile");
    ////开始编写你的业务逻辑，实现公文的处理.
    //div.html("<a>打开公文</a>");
}
