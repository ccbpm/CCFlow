/**
 *  每个节点都需要配置，公文组价启动模式.
 *  配置路径： 节点属性=》按钮权限(最下部.)
 * */
function LoadGovDocFile() {

    var nodeID = GetQueryString("FK_Node");
    var workID = GetQueryString("WorkID");
    var node = new Entity("BP.WF.Template.NodeExt", nodeID);

    //状态  0=不启用,1=可编辑,2=不可编辑.
    var sta = node.OfficeBtnEnable;
    if (sta == 0)
        return; 

    var div = $("#GovDocFile");

    //开始编写你的业务逻辑，实现公文的处理.
    div.html("<a>打开公文</a>");
}
