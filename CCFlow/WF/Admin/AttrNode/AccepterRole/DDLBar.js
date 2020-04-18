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

//Vue.component('model-component', {
//    template: '\
//            <div>\
//                <button @click="handleIncrease">+1</button>\
//                <button @click="handleReduce">-1</button>\
//            </div> ',
//    data: function () {
//        return {
//            nodeID: GetQueryString("FK_Node"),
//        }
//    },
//    methods: {
//        handleIncrease: function () {
//            this.counter++;
//            this.$emit('input', this.counter);
//        },
//        handleReduce: function () {
//            this.counter--;
//            this.$emit('input', this.counter);
//        }
//    }

//});
var optionKey = 0;
var flowNo = null;
function InitBar(optionKey) {

    var nodeID = GetQueryString("FK_Node");
    var en = new Entity("BP.WF.Template.NodeSimple", nodeID);
    flowNo = en.FK_Flow;
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";

    var html = "<div style='padding:5px' >接受人规则: ";
    if (isSatrtNode == true)
        html = "<div style='padding:5px' >发起人范围限定规则: ";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+按组织结构绑定</option>";

    var webUser = new WebUser();

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门计算</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的人员计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";

        if (webUser.CCBPMRunModel == 1) {
            html += "<option value=" + DeliveryWay.ByTeamOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(全集团)</option>";
            html += "<option value=" + DeliveryWay.ByTeamOrgOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本组织人员)</option>";
            html += "<option value=" + DeliveryWay.ByTeamDeptOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本部门人员)</option>";
        }


    } else {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按岗位智能计算</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门计算</option>";
        if (webUser.CCBPMRunModel == 1) {
            html += "<option value=" + DeliveryWay.ByTeamOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(全集团)</option>";
            html += "<option value=" + DeliveryWay.ByTeamOrgOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本组织人员)</option>";
            html += "<option value=" + DeliveryWay.ByTeamDeptOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本部门人员)</option>";
        }

        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的人员计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";
        html += "<option value=" + DeliveryWay.ByStationAndEmpDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算并且以绑定的部门集合为纬度</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmpStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按指定节点的人员岗位计算</option>";
        html += "<option value=" + DeliveryWay.ByStationOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;仅按绑定的岗位计算</option>";
        html += "<option value=" + DeliveryWay.BySetDeptAsSubthread + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定部门计算，该部门一人处理标识该工作结束(子线程)</option>";

        html += "<option value=" + DeliveryWay.FindSpecDeptEmps + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门范围内的岗位集合里面的人员.</option>";
        html += "<option value=" + DeliveryWay.ByDeptLeader + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门的领导(负责人).</option>";

        // 与按照岗位智能计算不同的是，仅仅找本部门的人员.
    }


    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+按指定节点处理人</option>";
        html += "<option value=" + DeliveryWay.ByStarter + " >&nbsp;&nbsp;&nbsp;&nbsp;与开始节点处理人相同</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;与上一节点处理人相同</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;与指定节点处理人相同</option>";
    }

    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+按自定义SQL查询</option>";
        html += "<option value=" + DeliveryWay.BySQL + " >&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQL获取接受人计算</option>";
        html += "<option value=" + DeliveryWay.BySQLTemplate + " >&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQLTempate获取接受人计算</option>";
        html += "<option value=" + DeliveryWay.BySQLAsSubThreadEmpsAndData + " >&nbsp;&nbsp;&nbsp;&nbsp;按SQL确定子线程接受人与数据源</option>";
    }


    if (isSatrtNode == false) {
        //检查是否是项目类的流程如果
        var isPrjFlow = false;
        var node = new Entity("BP.WF.Node", nodeID);
        var flowNo = node.FK_Flow;
        var flow = new Entity("BP.WF.Flow", flowNo);
        if (flow.FlowAppType == 1) {
            html += "<option value=null disabled='disabled' >+项目类流程</option>";
            html += "<option value=" + DeliveryWay.ByStationForPrj + ">&nbsp;&nbsp;&nbsp;&nbsp;按项目组内的岗位计算</option>";
            html += "<option value=" + DeliveryWay.BySelectedForPrj + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“项目组人员选择器”选择接受人</option>";
        }
    }


    html += "<option value=null disabled='disabled' >+其他方式</option>";

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.BySelected_1 + ">&nbsp;&nbsp;&nbsp;&nbsp;所有的人员都可以发起.</option>";
        html += "<option value=" + DeliveryWay.BySelectedOrgs + ">&nbsp;&nbsp;&nbsp;&nbsp;指定的组织可以发起(对集团版有效).</option>";

    } else {
        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“人员选择器”选择接受人</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormEmpsField + " >&nbsp;&nbsp;&nbsp;&nbsp;按上一节点表单指定的字段值作为本步骤的接受人</option>";
        html += "<option value=" + DeliveryWay.ByDtlAsSubThreadEmps + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点的明细表来决定子线程的接受人</option>";
        html += "<option value=" + DeliveryWay.ByFEE + " >&nbsp;&nbsp;&nbsp;&nbsp;由FEE来决定</option>";
        html += "<option value=" + DeliveryWay.ByFromEmpToEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;按照配置的人员路由列表计算</option>";
        html += "<option value=" + DeliveryWay.ByCCFlowBPM + " >&nbsp;&nbsp;&nbsp;&nbsp;按ccBPM的BPM模式处理</option>";
    }
    html += "</select >";
    html += "<input  id='Btn_Save' type=button onclick='SaveRole()' value='保存' />";
    html += "<input id='Btn' type=button onclick='AdvSetting()' value='高级设置' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function SaveRole() {
    $("#Btn_Save").val("正在保存请稍后.");

    try {

        Save();

    } catch (e) {
        alert(e);
        return;
    }

    AccepterRole_ClearStartFlowsCash();

    $("#Btn_Save").val("保存成功");
    setTimeout(function () { $("#Btn_Save").val("保存"); }, 1000);
}
//清除缓存，本组织的.
function AccepterRole_ClearStartFlowsCash() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_AttrNode");
    var data = handler.DoMethodReturnString("AccepterRole_ClearStartFlowsCash");
}
//清除缓存，所有本组织的.
function AccepterRole_ClearAllOrgStartFlowsCash() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_AttrNode");
    var data = handler.DoMethodReturnString("AccepterRole_ClearAllOrgStartFlowsCash");
}

function OldVer() {

    var nodeID = GetQueryString("FK_Node");
    var flowNo = GetQueryString("FK_Flow");

    var url = '../NodeAccepterRole.aspx?FK_Flow=' + flowNo + '&FK_Node=' + nodeID;
    window.location.href = url;
}
function Help() {

    var url = "";
    switch (optionKey) {
        case DeliveryWay.ByStation:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        case DeliveryWay.ByDept:
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
    OpenEasyUiDialogExtCloseFunc(url, '设置岗位', 800, 500, function () {
        Baseinfo.stas = getStas();
    });

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
function getDepts() {
    var ens = new Entities("BP.WF.Template.NodeDepts");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
/*
 * 获取节点绑定的用户组@lz
 */
function getGroups() {

    var ens = new Entities("BP.WF.Template.NodeTeams");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));

    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });

    return ens;

}
/*
 * 获取节点绑定部门的负责人@lz
 */
function getDeptLeader() {
    var ens = getDepts();
    var depts = new Entities("BP.WF.Port.Depts");

    for (var i = 0; i < ens.length; i++) {
        var en = ens[i];
        depts.Retrieve("No", en.FK_Dept);
    }
    return depts;
}

/*
 * 获取节点绑定的组织@lz
 */
function getOrgs() {

    var ens = new Entities("BP.WF.Template.FlowOrgs");

    // ens.Retrieve("FlowNo", flowNo);
    ens.Retrieve("FlowNo", GetQueryString("FK_Flow"));

    // alert(ens.length);
    return ens;

    ens = $.grep(ens, function (obj, i) {
        return obj.FlowNo != undefined
    });
    return ens;

}
/*
 * 获取节点绑定的人员
 */
function getEmps() {
    var ens = new Entities("BP.WF.Template.NodeEmps");
    ens.Retrieve("FK_Node", GetQueryString("FK_Node"));
    ens = $.grep(ens, function (obj, i) {
        return obj.FK_Node != undefined
    });
    return ens;

}
function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var en = new Entity("BP.WF.Template.NodeSimple", nodeID);
    flowNo = en.FK_Flow;
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 1) {
        optionKey = sele[index].value
    }
    var roleName = "";
    switch (parseInt(optionKey)) {
        case DeliveryWay.ByStation:
            roleName = "0.ByStation.htm";
            break;
        case DeliveryWay.ByDept:
            roleName = "1.ByDept.htm";
            break;
        case DeliveryWay.BySQL:
            roleName = "2.BySQL.htm";
            break;
        case DeliveryWay.ByBindEmp:
            roleName = "3.ByBindEmp.htm";
            break;
        case DeliveryWay.BySelected:
            roleName = "4.BySelected.htm";
            break;
        case DeliveryWay.BySelected_1:
            roleName = "41.BySelected.htm";
            break;
        case DeliveryWay.ByPreviousNodeFormEmpsField:
            roleName = "5.ByPreviousNodeFormEmpsField.htm";
            break;
        case DeliveryWay.ByPreviousNodeEmp:
            roleName = "6.ByPreviousNodeEmp.htm";
            break;
        case DeliveryWay.ByStarter:
            roleName = "7.ByStarter.htm";
            break;
        case DeliveryWay.BySpecNodeEmp:
            roleName = "8.BySpecNodeEmp.htm";
            break;
        case DeliveryWay.ByDeptAndStation:
            roleName = "9.ByDeptAndStation.htm";
            break;
        case DeliveryWay.ByStationAndEmpDept:
            roleName = "10.ByStationAndEmpDept.htm";
            break;
        case DeliveryWay.BySpecNodeEmpStation:
            roleName = "11.BySpecNodeEmpStation.htm";
            break;
        case DeliveryWay.BySQLAsSubThreadEmpsAndData:
            roleName = "12.BySQLAsSubThreadEmpsAndData.htm";
            break;
        case DeliveryWay.ByDtlAsSubThreadEmps:
            roleName = "13.ByDtlAsSubThreadEmps.htm";
            break;
        case DeliveryWay.ByStationOnly:
            roleName = "14.ByStationOnly.htm";
            break;
        case DeliveryWay.ByFEE:
            roleName = "15.ByFEEp.htm";
            break;
        case DeliveryWay.BySetDeptAsSubthread:
            roleName = "16.BySetDeptAsSubthread.htm";
            break;
        case DeliveryWay.BySQLTemplate:
            roleName = "17.BySQLTemplate.htm";
            break;
        case DeliveryWay.ByFromEmpToEmp:
            roleName = "18.ByFromEmpToEmp.htm";
            break;
        case DeliveryWay.FindSpecDeptEmps:
            roleName = "19.FindSpecDeptEmpsInStationlist.htm";
            break;
        case DeliveryWay.ByStationForPrj:
            roleName = "20.ByStationForPrj.htm";
            break;
        case DeliveryWay.BySelectedForPrj:
            roleName = "21.BySelectedForPrj.htm";
            break;
        case DeliveryWay.ByDeptLeader:
            roleName = "23.ByDeptLeader.htm";
            break;
        case DeliveryWay.ByTeamOrgOnly:
            roleName = "24.ByTeamOrgOnly.htm";
            break;
        case DeliveryWay.ByTeamOnly:
            roleName = "25.ByTeamOnly.htm";
            break;
        case DeliveryWay.ByTeamDeptOnly:
            roleName = "26.ByTeamDeptOnly.htm";
            break;
        case DeliveryWay.BySelectedOrgs:
            roleName = "42.BySelectedOrgs.htm";
            break;
        case DeliveryWay.ByCCFlowBPM:
            roleName = "100.ByCCFlowBPM.htm";
            break;
        default:
            roleName = "0.ByStation.htm";
            break;
    }

    // alert(roleName);

    window.location.href = roleName + "?FK_Node=" + nodeID + "&FK_Flow=" + flowNo;
}
function SaveAndClose() {
    Save();
    window.close();
}

function SaveIt() {

    $("#Btn_Save").val("正在保存请稍后.");

    try {
        Save();
        AfterSave();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").val("保存成功");
    setTimeout(function () { $("#Btn_Save").val("保存."); }, 1000);
}
// 保存之后要做的事情.
function AfterSave() {
    //清除.
    DBAccess.RunSQL("UPDATE WF_Emp SET StartFlows=''");
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });
}

//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "AdvSetting.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}

