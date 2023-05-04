var flowNo = null;
var nodeID = null;
var token = null;
var userNo = null;
$(function () {

    //if (window.screenLeft == 0 && window.document.body.clientWidth == window.screen.width) {
    //    alert("full");
    //}
    //alert(window.opener);
    //alert(window.parent);

    if ($("#Guide") == null || $("#Guide").length == 0) return;
    if (GetQueryString("IsShowHideGuide") != "1" && GetCurrFuncID() == "Frm") return;

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == null || flowNo == undefined || flowNo == "")
        flowNo = GetQueryString("FlowNo");
    if (flowNo == null || flowNo == undefined || flowNo == "")
        return;

    token = GetQueryString("Token");
    if(token==null || token==undefined || token=="null" ||token=="")
        token = GetQueryString("Token");

    var webUser = new WebUser();
    userNo = GetQueryString("UserNo");
    if (userNo == null) {
        userNo = webUser.No;
        token = webUser.Token;
    }

    if (userNo != webUser.No) {

        //访问后台，获得一个工作ID.
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
        handler.AddPara("Token", token);
        handler.AddPara("UserNo", userNo);
        
        var data = handler.DoMethodReturnString("Default_LetAdminerLogin");

        if (data.indexOf('err@token失效') >= 0)
        {
            alert(data);

        }

        // alert("userNo:" + userNo + " - webUser:" + webUser.No + " data:" + data);

        Reload();
        return;
    }

    nodeID = GetNodeID();

    var Guide = $("#Guide");
    var str = '';
    str = "<style>.Guide{position:fixed;right:0px;top:0px;z-index:1040;width:40px;height:100%;background:#444851;}.Guide-list{position:absolute;top:15%;width:100%}.Guide .guideb i{color:#fff;font-size:18px;}.Guide .guideb{text-align:center;position:relative;font-family:'Microsoft YaHei';font-size:18px;color:#fff;cursor:pointer;padding:15px 0px;}.Guide .guideb:before{content:'';height:1px;width:30px;position:absolute;background:#6f737a;bottom:-1px;left:5px;}.Guide .guideb:last-child:before{content:'';height:1px;width:30px;position:absolute;background:none;bottom:-1px;left:5px;}.Guide .guideb:hover{background:#ed145b}.Guide .guideb:hover:before{content:'';height:1px;width:30px;position:absolute;background:none;bottom:0px;left:5px;}.Guide .guideb b{width:16px;font-size:13px;margin:5px 0 0 12px;display:block}.guideicon{line-height:50px;text-align:center;color:#fff}.guideicon i{line-height:50px;cursor:pointer;font-size:18px;}.iconBtn{position:fixed;bottom:10px;right:25px;z-index:999;display:none;width:50px;height:50px;line-height:50px;}.iconBtn i{line-height:50px;cursor:pointer;font-size:36px;}</style>"
    str += "<div class='Guide'><div class='Guide-list'>"
    /* flowNo = GetQueryString("FlowNo");
     if (flowNo == null)
         flowNo = GetQueryString("FK_Flow");*/

    var dbs = GetDBDtl();

    var currFuncID = GetCurrFuncID(); //当前的功能ID.

    // console.log(dbs.length)
    for (i = 0; i < dbs.length; i++) {

        var node = dbs[i];
        if (node.No == currFuncID)
            str += "<div class=guideb onclick=\"GetUrl('" + node.No + "')\" ><i class='iconfont " + node.ICON + "' ></i><b>(" + (i + 1) + ")" + node.Name + "</b></div>";
        else
            str += "<div class=guideb onclick=\"GetUrl('" + node.No + "')\" ><i class='iconfont " + node.ICON + "' ></i> <b>(" + (i + 1) + ")" + node.Name + "</b></div>";
    }

   /* str += '<div class=guideicon  onclick=\"ShowHidden()\"><i class="iconfont icon-gaodu"></i></div>'
    str += '</div>';
    str += '<div class=iconBtn onclick=\"HiddenShow()\"><i class="iconfont icon-kuandu"></i></div>';*/
    str += '</div></div>';
    Guide.html(str);

    resizeWindow();
    return;
});
function GetCurrFuncID() {

    //确定当前的功能ID.
    var url = window.location.href;

    if (url.indexOf('TestFlow2020') > 1)
        return "TestingContainer";

    if (url.indexOf('CCBPMDesigner') > 1)
        return "Flow";

    if (url.indexOf('FormDesigner') > 1)
        return "Frm";

    if (url.indexOf('NodeFrm') > 1)
        return "Frm";

    if (url.indexOf('GuideAccepter') > 1)
        return "Accepter";

    alert("没有判断funid");

}
function resizeWindow() {
    return;

    if (window.screen) {//判断浏览器是否支持window.screen判断浏览器是否支持screen    
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽    
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高    
        window.moveTo(0, 0);           //把window放在左上脚    
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh    
    }
}

function GetNodeID() {
    flowNo = GetQueryString("FK_Flow");
    if (flowNo == null)
        flowNo = GetQueryString("FlowNo");
    if (flowNo == null)
        return;

    nodeID = GetQueryString("FK_Node");
    if (nodeID == null || "undefined" == nodeID || undefined == nodeID)
        nodeID = GetQueryString("NodeID");
    if (nodeID == null || "undefined" == nodeID || undefined == nodeID)
        nodeID = parseInt(flowNo + "01");


    return nodeID;
}


function GetDBDtl() {

    var json = [
        { "No": "Flow", "Name": "流程", "ICON": " icon-Track" },
        { "No": "Frm", "Name": "表单", "ICON": " icon-biaoge" },
        { "No": "Accepter", "Name": "接受人", "ICON": " icon-Shift" },
        { "No": "TestingContainer", "Name": "测试运行", "ICON": " icon-Send" }
    ];
    return json;
}

function GetUrl(funcID) {

    currFuncID = funcID;

    if (funcID == "Accepter") return Accepter();
    if (funcID == "Flow") return Flow();
    if (funcID == "Frm") return Frm();
    if (funcID == "TestingContainer") return TestingContainer();
    if (funcID == "Accepter") return Accepter();

}
function ShowHidden() {
    $('.Guide').hide();
    $('.iconBtn').show();
}
function HiddenShow() {
    $('.Guide').show();
    $('.iconBtn').hide();
}

function Frm() {

    var flow = new Entity("BP.WF.Flow", flowNo);

    var flowDevModel = flow.GetPara("FlowDevModel") || flow.FlowDevModel; //设计模式.
    if (flowDevModel == undefined || flowDevModel == null)
        flowDevModel = 0;

    flowDevModel = parseInt(flowDevModel);

    // 极简表单. 
    if (flowDevModel == FlowDevModel.JiJian) {

        var nodeID = parseInt(flowNo + "01");
        var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?1=1";
        url += "&FlowNo=" + flowNo;
        url += "&FK_Flow=" + flowNo;
        url += "&FK_MapData=ND" + nodeID;
        url += "&FrmID=ND" + nodeID;
        url += "&FK_Node=" + nodeID;
        url += "&UserNo=" + userNo;
        url += "&Token=" + token;
        url += "&IsShowHideGuide=1";
        //window.location.href = filterXSS(url);
        OpenLayuiDialog(filterXSS(url), "设计表单ND" + nodeID, window.innerWidth * 0.9);
        return;
    }

    //绑定单个表单.
    if (flowDevModel == FlowDevModel.RefOneFrmTree) {
        var frmID = flow.FrmUrl;
        var nodeID = parseInt(flowNo + "01");
        var url = basePath + "/WF/Admin/FoolFormDesigner/Designer.htm?FrmID=" + frmID + "&FK_Flow=" + flowNo + "&FK_MapData=" + frmID + "&FK_Node=" + nodeID;
        url += "&IsShowHideGuide=1";
        //window.location.href = filterXSS(url);
        OpenLayuiDialog(filterXSS(url), "设计表单ND" + nodeID, window.innerWidth * 0.9);
        return;
        //if (pageFrom == "") {
        //    window.parent.addTab(nodeID, "设计表单" + nodeID, url);
        //} else {
        //    window.top.vm.openTab("设计表单" + nodeID, url);
        //}
    }

    //自定义表单.
    if (flowDevModel == FlowDevModel.SDKFrm || flowDevModel == FlowDevModel.SelfFrm) {

        var flow = new Entity("BP.WF.Flow", flowNo);
        var url = flow.FrmUrl;
        alert("是自定义表单流程，请用对应的编辑器编辑：" + url);
        return;
        url = promptGener('请输入url', url);
        if (url == null || url == undefined)
            return;
        flow.FrmUrl = url;
        flow.Update();
        WinOpen(url);
        return;
    }

    var url = basePath + "/WF/Admin/BatchSetting/GuideNodeFrm.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeID + "&UserNo=" + userNo + "&Token=" + GetQueryString("Token");
    url += "&IsShowHideGuide=1";
    window.location.href = filterXSS(url);
    return;
}

function Flow() {
    var url = basePath + "/WF/Admin/CCBPMDesigner/Designer.htm?1=1";
    url += "&FlowNo=" + flowNo;
    url += "&FK_Flow=" + flowNo;
    url += "&FK_Node=" + nodeID;
    url += "&UserNo=" + userNo;
    url += "&Token=" + token;
    window.location.href = filterXSS(url);
}

function Accepter() {

    var url = basePath + "/WF/Admin/BatchSetting/GuideAccepter.htm?1=1";
    url += "&FlowNo=" + flowNo;
    url += "&FK_Flow=" + flowNo;
    url += "&FK_Node=" + nodeID;
    url += "&UserNo=" + userNo;
    url += "&Token=" + token;
    window.location.href = filterXSS(url);
}

/**
 * 测试运行.
 * */
function TestingContainer() {

    var flowNo = GetQueryString("FK_Flow");
    var url = basePath + "/WF/Admin/TestingContainer/TestFlow2020.htm?1=1";
    url += "&FlowNo=" + flowNo;
    url += "&FK_Flow=" + flowNo;
    url += "&UserNo=" + GetQueryString("UserNo");
    //url += "&Token=" + GetQueryString("Token");
    url += "&FK_Node=" + GetNodeID();
    window.location.href = filterXSS(url);
}


//流程设计模式.
if (typeof FlowDevModel == "undefined") {
    var FlowDevModel = {}
    /// <summary>
    /// 专业模式
    /// </summary>
    FlowDevModel.Prefessional = 0,
        /// <summary>
        /// 极简模式
        /// </summary>
        FlowDevModel.JiJian = 1,
        /// <summary>
        /// 累加模式
        /// </summary>
        FlowDevModel.FoolTruck = 2,
        /// <summary>
        /// 绑定单表单
        /// </summary>
        FlowDevModel.RefOneFrmTree = 3,
        /// <summary>
        /// 绑定多表单
        /// </summary>
        FlowDevModel.FrmTree = 4,
        /// <summary>
        /// SDK表单
        /// </summary>
        FlowDevModel.SDKFrm = 5,
        /// <summary>
        /// 嵌入式表单
        /// </summary>
        FlowDevModel.SelfFrm = 6,
        /// <summary>
        /// 物联网流程
        /// </summary>
        FlowDevModel.InternetOfThings = 7,
        /// <summary>
        /// 决策树流程
        /// </summary>
        FlowDevModel.Tree = 8
}