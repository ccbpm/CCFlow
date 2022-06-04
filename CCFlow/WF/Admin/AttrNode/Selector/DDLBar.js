$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            //alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});


var optionKey = 0;
function InitBar(key) {

    optionKey = key;

    var webUser = new WebUser();
    var nodeID = GetQueryString("FK_Node");
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";
    var html = "<div style='padding:5px' >接受人可以选择的范围限定: ";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+按组织结构限定范围</option>";

    html += "<option value=" + SelectorModel.Station + ">&nbsp;&nbsp;&nbsp;&nbsp;按照岗位</option>";
    html += "<option value=" + SelectorModel.ByStationAI + ">&nbsp;&nbsp;&nbsp;&nbsp;按照岗位智能计算</option>";
    html += "<option value=" + SelectorModel.Dept + " >&nbsp;&nbsp;&nbsp;&nbsp;按指定的部门人员计算</option>";
    html += "<option value=" + SelectorModel.ByMyDeptEmps + " >&nbsp;&nbsp;&nbsp;&nbsp;按本部门人员计算</option>";

    html += "<option value=" + SelectorModel.Emp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的人员计算</option>";
    html += "<option value=" + SelectorModel.SQL + " >&nbsp;&nbsp;&nbsp;&nbsp;按SQL计算</option>";
    html += "<option value=" + SelectorModel.SQLTemplate + " >&nbsp;&nbsp;&nbsp;&nbsp;按SQL模板计算</option>";
    html += "<option value=" + SelectorModel.GenerUserSelecter + " >&nbsp;&nbsp;&nbsp;&nbsp;使用通用人员选择器</option>";
    html += "<option value=" + SelectorModel.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按部门与岗位的交集</option>";

    if (webUser.CCBPMRunModel == 1) {
        html += "<option value=" + SelectorModel.TeamOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(全集团)</option>";
        html += "<option value=" + SelectorModel.TeamOrgOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本组织人员)</option>";
        html += "<option value=" + SelectorModel.TeamDeptOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本部门人员)</option>";
    }

    html += "<option value=null  disabled='disabled'>+其他</option>";
    html += "<option value=" + SelectorModel.Url + ">&nbsp;&nbsp;&nbsp;&nbsp;自定义URL</option>";
    html += "<option value=" + SelectorModel.ByWebAPI + ">&nbsp;&nbsp;&nbsp;&nbsp;按照WebAPI计算</option>";
    html += "<option value=" + SelectorModel.AccepterOfDeptStationEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;使用通用部门岗位人员选择器（开发中）</option>";
    html += "<option value=" + SelectorModel.AccepterOfDeptStationEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;按岗位智能计算(操作员所在部门)（开发中）</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_Back' type=button onclick='Back()' value='返回' />";
    //    html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //   html += "<input type=button onclick='Help()' value='帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");


}
function Back() {
    url = "../AccepterRole/Default.htm?FK_Node=" + GetQueryString("FK_Node") + "&FK_Flow=" + GetQueryString("FK_Flow");
    SetHref(url);
}

/*
* 获取节点绑定的岗位
*/
function getStas() {

    var ens = new Entities("BP.WF.Template.NodeStations");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
/*
* 获取节点绑定的部门
*/
function getDepts(orgNo) {


    var ens = new Entities("BP.WF.Template.NodeDepts");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

    // var handler = new HttpHandler();


    //var ens = new Entities("BP.WF.Template.NodeDepts");
    //ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
/*
* 获取节点绑定的用户组
*/
function getGroups() {

    var ens = new Entities("BP.WF.Template.NodeTeams");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });

    return ens;

}
function OldVer() {

    var nodeID = GetQueryString("FK_Node");
    var flowNo = GetQueryString("FK_Flow");

    var url = '../NodeAccepterRole.aspx?FK_Flow=' + flowNo + '&FK_Node=' + nodeID;
    SetHref(url);
}
function Help() {

    var url = "";
    switch (optionKey) {
        case SelectorModel.Station:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        case SelectorModel.Dept:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        default:
            url = "http://ccbpm.mydoc.io/?v=5404&t=17906";
            break;
    }

    window.open(url);
}

//通用的设置岗位的方法。for admin.

function OpenDot2DotStations() {

    var nodeID = GetQueryString("FK_Node");

    var url = "../../../Comm/RefFunc/Dot2Dot.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeStations";
    url += "&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.Port.Stations";
    url += "&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;

    OpenEasyUiDialogExt(url, '设置岗位', 800, 500, true);
}

function GenerUrlByOptionKey(optionKey) {
    var roleName = "";
    switch (parseInt(optionKey)) {
        case SelectorModel.Station:
            roleName = "0.Station.htm";
            break;
        case SelectorModel.Dept:
            roleName = "1.Dept.htm";
            break;
        case SelectorModel.Emp:
            roleName = "2.Emp.htm";
            break;
        case SelectorModel.SQL:
            roleName = "3.SQL.htm";
            break;
        case SelectorModel.SQLTemplate:
            roleName = "4.SQLTemplate.htm";
            break;
        case SelectorModel.GenerUserSelecter:
            roleName = "5.GenerUserSelecter.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "6.DeptAndStation.htm";
            break;
        case SelectorModel.Url:
            roleName = "7.Url.htm";
            break;
        case SelectorModel.BySpecNodeEmp:
            roleName = "8.AccepterOfDeptStationEmp.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "9.AccepterOfDeptStationOfCurrentOper.htm";
            break;
        case SelectorModel.TeamOnly:
            roleName = "10.TeamOnly.htm";
            break;
        case SelectorModel.TeamOrgOnly:
            roleName = "11.TeamOrgOnly.htm";
            break;
        case SelectorModel.TeamDeptOnly:
            roleName = "12.TeamDeptOnly.htm";
            break;
        case SelectorModel.ByStationAI:
            roleName = "13.ByStationAI.htm";
            break;
        case SelectorModel.ByWebAPI:
            roleName = "14.ByWebAPI.htm";
            break;
        case SelectorModel.ByMyDeptEmps:
            roleName = "15.ByMyDeptEmps.htm";
            break;
        default:
            roleName = "0.Station.htm";
            break;
    }
    return roleName;
}


function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 1) {
        optionKey = sele[index].value
    }

    var roleName = GenerUrlByOptionKey(optionKey);
    SetHref( roleName + "?FK_Node=" + nodeID + "&FK_Flow=" + GetQueryString("FK_Flow"));
}
function SaveAndClose() {
    Save();
    window.close();
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            Reload();
        }
    });
}

//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "AdvSetting.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}

//设置岗位-左右结构.
function OpenBranchesAndLeafStations() {

    var w = 300;
    var nodeID = GetQueryString("FK_Node");
    var url = "../../../Comm/RefFunc/BranchesAndLeaf.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeStations&Dot2DotEnName=BP.WF.Template.NodeStation&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.Port.Stations&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;

    OpenEasyUiDialogExt(url, '设置岗位', 800, 800, true);

    //OpenEasyUiDialogExtCloseFunc(url, '设置岗位', w * 1.5, h, function () {
    //    Baseinfo.stas = getStas();
    //});
}