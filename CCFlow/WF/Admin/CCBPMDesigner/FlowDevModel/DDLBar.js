var webUser = new WebUser();

function InitBar(optionKey) {

    var html = "流程设计模式:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var groups = GetDBGroup();
    var dtls = GetDBDtl();

    for (var i = 0; i < groups.length; i++) {

        var group = groups[i];
        html += "<option value=null  disabled='disabled'>+" + group.Name + "</option>";

        for (var idx = 0; idx < dtls.length; idx++) {
            var dtl = dtls[idx];
            if (dtl.GroupNo != group.No)
                continue;
            html += "<option value=" + dtl.No + ">&nbsp;&nbsp;" + dtl.Name + "</option>";
        }
    }
    html += "</select>";

    var sorts = new Entities("BP.WF.Admin.FlowSorts");
    var webUser = new WebUser();
    if (webUser.CCBPMRunModel == 0)
        sorts.RetrieveAll();
    else
        sorts.Retrieve("OrgNo", WebUser.OrgNo);
    html += "&nbsp;存放目录:";
    html += "<select id=DDL_FlowSort >";
    var sortNo = GetQueryString("SortNo");
    for (var i = 0; i < sorts.length; i++) {
        var sort = sorts[i];

        if (sortNo == sort.No)
            html += "<option selected=true value=" + sort.No + ">" + sort.Name + "</option>";
        else
            html += "<option  value=" + sort.No + ">" + sort.Name + "</option>";
    }
    html += "</select>";

    var from = GetQueryString("From");

    if (from == "Flows.htm" || from=="FlowTree.htm")
        html += "<input  id='Btn_Save'class='cc-btn-tab' type=button onclick='Save()' value='创建流程' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");



}

//创建流程.
function Save() {

    var newFlowInfo = getNewFlowInfo();
    if ((newFlowInfo.FlowFrmModel == FlowDevModel.RefOneFrmTree
        || newFlowInfo.FlowFrmModel == FlowDevModel.FrmTree)&& newFlowInfo.FrmID == "") {
        alert("请选择绑定的表单");
        return;
    }
    $("#Btn_Save").val("正在创建,请稍后");
    setTimeout(function () {

        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_CCBPMDesigner_FlowDevModel");
        handler.AddPara("SortNo", newFlowInfo.FlowSort);
        handler.AddPara("FlowName", newFlowInfo.FlowName);
        handler.AddPara("FlowDevModel", newFlowInfo.FlowFrmModel);
        handler.AddPara("FrmUrl", newFlowInfo.FrmUrl);
        handler.AddPara("FrmID", newFlowInfo.FrmID);
        var data = handler.DoMethodReturnString("FlowDevModel_Save");

        if (data.indexOf('err@') == 0) {
            alert(data);
            return;
        }
        //流程列表增加表单节点
        if (typeof window.parent.AppendFlowToFlowSort != "undefined") {
            window.parent.AppendFlowToFlowSort(newFlowInfo.FlowSort, data, newFlowInfo.FlowName);
        }
        var webUser = new WebUser();
        var url = "../Designer.htm?FK_Flow=" + data + "&OrgNo=" + webUser.OrgNo + "&Token=" + webUser.Token + "&UserNo=" + webUser.No + "&From=Ver2021";
        if (window.parent && window.parent.layer)
            window.parent.layer.close(window.parent.layer.index);
        WinOpenFull(url, data);
    }, 1000);

}

function GenerName() {
    var name = prompt('请输入流程名称');
    if (name == null || name == undefined)
        return null;
    return name;
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "内置表单模式" },
        //{ "No": "B", "Name": "服务的模式" },
        { "No": "C", "Name": "绑定表单库模式" },
        { "No": "D", "Name": "自定义表单模式" },
        { "No": "E", "Name": "物联网流程模式" }
        /*{ "No": "F", "Name": "敏捷应用" }*/

    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "专业模式", "GroupNo": "A", "Url": "0.Prefessional.htm" },
        { "No": 1, "Name": "极简模式", "GroupNo": "A", "Url": "1.JiJian.htm" },
        { "No": 2, "Name": "累加表单", "GroupNo": "A", "Url": "2.FoolTruck.htm" },

        { "No": 3, "Name": "绑表单库的表单", "GroupNo": "C", "Url": "3.RefOneFrmTree.htm" },
        { "No": 4, "Name": "表单树", "GroupNo": "C", "Url": "4.FrmTree.htm" },

        { "No": 5, "Name": "SDK表单", "GroupNo": "D", "Url": "5.SDKFrm.htm" },
        { "No": 6, "Name": "嵌入式表单", "GroupNo": "D", "Url": "6.SelfFrm.htm" },
        { "No": 7, "Name": "物联网流程", "GroupNo": "E", "Url": "7.InternetOfThings.htm" },
        { "No": 8, "Name": "决策树模式", "GroupNo": "E", "Url": "8.Tree.htm" }
        //{ "No": 9, "Name": "实体(车辆、固定资产)", "GroupNo": "F", "Url": "9.Dict.htm" },
        //{ "No": 10, "Name": "单据(车辆维修记录、固资使用记录)", "GroupNo": "F", "Url": "10.Bill.htm" }

    ];
    return json;
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}
function changeOption() {
    //获得流程类别.
    var sortNo = $("#DDL_FlowSort").val(); // GetQueryString("SortNo");
    var from = GetQueryString("From");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    SetHref(url + "?SortNo=" + sortNo + "&From=" + from);
}
//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "7.ByOtherBlock.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}
function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }
    return "1.JiJian.htm";
}
//要返回的json.
function getNewFlowInfo() {
    //流程名称
    var flowName = $("#TB_Name").val();
    //流程分类
    var flowSort = GetQueryString("SortNo");
    //表单方案
    var flowFrmType = $('#changBar option:selected').val();
    //Url
    var frmUrl = $("#TB_Url").val();
    //选择的表单库中表单的ID
    var frmID = $("#TB_Frm").val();
    return {
        FlowName: flowName,
        FlowSort: flowSort,
        FlowFrmModel: flowFrmType,
        FrmUrl: frmUrl,
        FrmID: frmID
    };
}