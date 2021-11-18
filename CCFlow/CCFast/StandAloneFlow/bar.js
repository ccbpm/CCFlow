//定义全局通用的系统字段变量
var sysFiels = ",OID,FID,ParentFlowNo,PWorkID,AtPara,Title,RDT,CDT,Rec,Emps,PNodeID,PrjName,PrjNo,AtPara";
sysFiels += ",BillNo,PEmp,GUID,WFSta,FlowEnderRDT,FlowEnder,FlowEndNode,FlowStartRDT,FlowDaySpan,FlowEmps,PFlowNo";
sysFiels += ",FlowStarter,FlowEnder,WFState,FK_Dept,FK_NY,FlowNote,";

$(function () {

    var fk_flow = GetQueryString("FK_Flow");
    var pageId = GetQueryString("PageID");
    // 最近的，我参与的流程.
    var html = "<div style='padding:15px 0px 0px 20px;float:left'>";

    html += "<a href='javascript:Start()' class='layui-btn layui-btn-sm'>发起</a>  ";

    var ens = new Entities("BP.WF.Template.FlowTabs");
    ens.Retrieve("FK_Flow", fk_flow, "Idx");

    for (var i = 0; i < ens.length; i++) {
        var en = ens[i];
        //console.log(en)
        if (pageId == en.MyPK)
            html += "<a href='" + en.UrlExt + "&PageID=" + en.MyPK + "' class='layui-btn layui-btn-danger layui-btn-sm'>" + en.Name + "</a>  ";
        else
            html += "<a href='" + en.UrlExt + "&PageID=" + en.MyPK + "' class='layui-btn layui-btn-sm'>" + en.Name + "</a>  ";
    }
    html += '</div>';

    //html += "<div  style='padding: 15px 0px 0px 20px; float: right'>";
    //html += "<a href='javascript:Setting();' class='layui-btn layui-btn-sm'>设置</a>";
    //html += '</div>';


    $("#toolbar").html(html);
});

function Setting() {
    var url = './Admin/Default.htm?FlowNo=' + GetQueryString("FlowNo");
    window.location.href = url;
}

function GetDBGroup() {

    var json = [
        { "No": "A", "Name": "限制规则" }
    ];
    return json;
}
function GetDBDtl() {

    var json = [

        { "No": "Default", "Name": "主页", "GroupNo": "A", "Url": "Default.htm" },
        { "No": "Todolist", "Name": "我的待办", "GroupNo": "A", "Url": "Todolist.htm" },
        { "No": "Runing", "Name": "在途", "GroupNo": "A", "Url": "Runing.htm" },
        { "No": "Complate", "Name": "办结", "GroupNo": "A", "Url": "Complate.htm" },
        { "No": "CC", "Name": "抄送", "GroupNo": "A", "Url": "CC.htm" },
        { "No": "Draf", "Name": "草稿", "GroupNo": "A", "Url": "Draft.htm" },
        /* { "No": "Chart", "Name": "图表", "GroupNo": "A", "Url": "Chart.htm" }*/
        { "No": "Nums", "Name": "统计", "GroupNo": "A", "Url": "Nums.htm" }

    ];
    return json;
}
function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }
    return "Default.htm";
}

function Start() {
    var flowNo = GetQueryString("FK_Flow");
    window.location.href = "../../WF/MyFlow.htm?FK_Flow=" + flowNo;
}


/**
 * 打开表单， 如果是仅仅传入的是FlowNo就是启动流程.
 * @param {any} flowNo
 * @param {any} nodeID
 * @param {any} workid
 * @param {any} fid
 * @param {any} paras
 */
function OpenForm(flowNo, nodeID, workid, fid, paras) {

    url = basePath + "/WF/MyFlow.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&WorkID=" + workid;
    window.open(url);

    // var url = GenerFrmUrl(flowNo, nodeID, workid, fid, paras);
    //打开工作处理器.
    //OpenMyFlow(url);
    //   window.open(url);
}

/**
 * 获得表单的 URL.
 * 该表单的URL存储在开始节点表单方案里.
 * @param {流程编号} flowNo
 * @param {节点ID默认为0} nodeID
 * @param {实例的ID} workid
 * @param {默认为:0} fid
 * @param {参数:格式为 &KeHuBianHao=001&KeHuMingCheng=新疆天业} paras
 */
function GenerFrmUrl(flowNo, nodeID = 0, workid = 0, fid = 0, paras = "") {

    // ccbpmHostDevelopAPI 变量是定义在 /config.js 的服务地址. 访问必须两个参数DoWhat,SID.
    //首先获得表单的URL.
    var myUrl = ccbpmHostDevelopAPI + "?DoType=GenerFrmUrl&Token=" + GetToken() + "&WorkID=" + workid + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&FID=" + fid;
    var frmUrl = RunUrlReturnString(myUrl);
    frmUrl += paras;

    //如果包含了通用的工作处理器.
    if (frmUrl.indexOf("WF/MyFlow.htm") >= 0) {
        frmUrl = host + frmUrl;
    }
    return frmUrl;
}