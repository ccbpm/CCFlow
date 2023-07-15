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
var flowNo = null;
var w = window.innerWidth / 2;
var h = window.innerHeight - 40;
function InitBar(optionKey) {

    var nodeID = GetQueryString("FK_Node");
    var en = new Entity("BP.WF.Template.NodeSimple", nodeID);
    flowNo = en.FK_Flow;
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    //var html = "<div style='background-color:Silver' > 请选择访问规则: ";

    var html = "<div>接受人规则: ";
    if (isSatrtNode == true)
        html = "<div>发起人范围限定规则: ";

    html += "<select id='changBar' onchange='changeOption()'>";
    html += "<option value=null  disabled='disabled'>+按组织结构绑定</option>";

    var webUser = new WebUser();

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的人员计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";
        //不常用的放入到下面.
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndEmpField + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门人员选择器计算</option>";

        if (webUser.CCBPMRunModel == 1 || webUser.CCBPMRunModel == 2) {

            html += "<option value=null  disabled='disabled'>+按用户组计算</option>";
            html += "<option value=" + DeliveryWay.ByTeamDeptOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组</option>";
        }


    } else {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按岗位智能计算</option>";
        html += "<option value=" + DeliveryWay.ByStationOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;仅按绑定的岗位计算</option>";
      

        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的人员计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";
        // html += "<option value=" + DeliveryWay.ByStationAndEmpDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算并且以绑定的部门集合为纬度</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmpStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按指定节点的人员岗位计算</option>";
        html += "<option value=" + DeliveryWay.BySetDeptAsSubthread + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定部门计算，该部门一人处理标识该工作结束(子线程)</option>";

        html += "<option value=" + DeliveryWay.FindSpecDeptEmps + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门范围内的岗位集合里面的人员.</option>";
        html += "<option value=" + DeliveryWay.ByDeptLeader + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门的领导(主管,负责人).</option>";
        html += "<option value=" + DeliveryWay.ByEmpLeader + ">&nbsp;&nbsp;&nbsp;&nbsp;找指定节点的人员直属领导.</option>";
        //  html += "<option value=" + DeliveryWay.ByDeptShipLeader + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门的分管领导.</option>";
        // 与按照岗位智能计算不同的是，仅仅找本部门的人员.

        //不常用的放入到下面.
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndEmpField + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的部门人员选择器计算</option>";

        if (webUser.CCBPMRunModel == 1 || webUser.CCBPMRunModel == 2) {
            html += "<option value=null  disabled='disabled'>+按用户组计算</option>";
            if (webUser.CCBPMRunModel == 1)
                html += "<option value=" + DeliveryWay.ByTeamOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(全集团)</option>";

            html += "<option value=" + DeliveryWay.ByTeamOrgOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本组织人员)</option>";
            html += "<option value=" + DeliveryWay.ByTeamDeptOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的用户组(本部门人员)</option>";
         //   html += "<option value=" + DeliveryWay.ByBindTeamEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位用户组的人员计算</option>";
        }
    }



    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+按上一个节点的处理人身份</option>";
        html += "<option value=" + DeliveryWay.BySenderParentDeptLeader + ">&nbsp;&nbsp;&nbsp;&nbsp;发送人上级部门的负责人.</option>";
        html += "<option value=" + DeliveryWay.BySenderParentDeptStations + ">&nbsp;&nbsp;&nbsp;&nbsp;发送人上级部门岗位下的人员(需绑定岗位).</option>";
    }

    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+按指定节点处理人</option>";
        html += "<option value=" + DeliveryWay.ByStarter + " >&nbsp;&nbsp;&nbsp;&nbsp;与开始节点处理人相同</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;指定的节点相同</option>"; //@by zhoupeng  add 为啥去掉了？
        html += "<option value=" + DeliveryWay.ByPreviousNodeEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;与上一节点处理人相同</option>";
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

    if (isSatrtNode == false) {
        html += "<option value=null disabled='disabled' >+按节点表单的数据计算</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormEmpsField + " >&nbsp;&nbsp;&nbsp;&nbsp;字段是人员编号</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormDepts + " >&nbsp;&nbsp;&nbsp;&nbsp;字段是部门编号</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormStationsAI + " >&nbsp;&nbsp;&nbsp;&nbsp;字段是岗位编号(按岗位智能计算)</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormStationsOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;字段是岗位编号(仅按岗位计算)</option>";
        html += "<option value=" + DeliveryWay.ByDtlAsSubThreadEmps + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点的明细表来决定子线程的接受人</option>";
    }

    html += "<option value=null disabled='disabled' >+其他方式</option>";

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.BySelected_1 + ">&nbsp;&nbsp;&nbsp;&nbsp;所有的人员都可以发起.</option>";
        html += "<option value=" + DeliveryWay.ByGuest + ">&nbsp;&nbsp;&nbsp;&nbsp;仅外部用户可以发起.</option>";


        if (webUser.CCBPMRunModel == 1)
            html += "<option value=" + DeliveryWay.BySelectedOrgs + ">&nbsp;&nbsp;&nbsp;&nbsp;指定的组织可以发起(对集团版有效).</option>";


    } else {
        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“人员选择器”选择接受人</option>";

        if (webUser.CCBPMRunModel == 1) {
            html += "<option value=" + DeliveryWay.BySelectedEmpsOrgModel + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“人员选择器”选择接受人(集团模式)</option>";
            html += "<option value=" + DeliveryWay.BySelectEmpByOfficer + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人选择其他组织的联络员</option>";
        }

        html += "<option value=" + DeliveryWay.BySelfUrl + " >&nbsp;&nbsp;&nbsp;&nbsp;自定义人员选择器</option>";
        html += "<option value=" + DeliveryWay.ByAPIUrl + " >&nbsp;&nbsp;&nbsp;&nbsp;按照设置的WebAPI接口获取的数据计算</option>";

        html += "<option value=" + DeliveryWay.ByFEE + " >&nbsp;&nbsp;&nbsp;&nbsp;由FEE来决定</option>";
        html += "<option value=" + DeliveryWay.ByFromEmpToEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;按照配置的人员路由列表计算</option>";
        html += "<option value=" + DeliveryWay.ByCCFlowBPM + " >&nbsp;&nbsp;&nbsp;&nbsp;按ccBPM的BPM模式处理</option>";
    }

    html += "</select >";
    html += "<button  id='Btn_Save'type=button  onclick='SaveRole()' value='保存' />保存</button>";
    if (GetQueryString("FK_Node").substr(GetQueryString("FK_Node").length - 2) != "01")
        html += "<button id='Btn_Advanced' type=button onclick='AdvSetting()' />更多设置</button>";
    html += "<button id='Btn_Batch' type=button onclick='Batch()' value='批处理设置' />批处理设置</button>";
    html += "</div>";
    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

//批处理.
function Batch() {
    var url = "Batch.htm?NodeID=" + GetQueryString("FK_Node") + "&FK_Flow=" + GetQueryString("FK_Flow");
    SetHref(url);
}

function SaveRole() {

    $("#Btn_Save").html("正在保存请稍后.");

    try {

        Save();

    } catch (e) {
        alert(e);
        return;
    }

    AccepterRole_ClearStartFlowsCash();

    $("#Btn_Save").html("保存成功");
    setTimeout(function () { $("#Btn_Save").html("保存"); }, 1000);
}
//清除缓存，本组织的.
function AccepterRole_ClearStartFlowsCash() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_AttrNode_AccepterRole");
    var data = handler.DoMethodReturnString("AccepterRole_ClearStartFlowsCash");
}
//清除缓存，所有本组织的.
function AccepterRole_ClearAllOrgStartFlowsCash() {
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_AttrNode_AccepterRole");
    var data = handler.DoMethodReturnString("AccepterRole_ClearAllOrgStartFlowsCash");
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

//通用的设置岗位的方法。for admin. ***********************************************************
function OpenDot2DotStations() {
    var nodeID = GetQueryString("FK_Node");
    var url = "../../../Comm/RefFunc/Dot2Dot.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeStations";
    url += "&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.Port.Stations";
    url += "&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;
    OpenEasyUiDialogExtCloseFunc(url, '设置岗位', w, h, function () {
        Baseinfo.stas = getStas();
    });
}
//设置岗位-左右结构.
function OpenBranchesAndLeafStations() {

    var nodeID = GetQueryString("FK_Node");
    var url = "../../../Comm/RefFunc/BranchesAndLeaf.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeStations&Dot2DotEnName=BP.WF.Template.NodeStation&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.Port.Stations&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;
    OpenEasyUiDialogExtCloseFunc(url, '设置岗位', w * 1.5, h, function () {
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


//绑定部门 ************************************************
function BindDeptTree() {

    var nodeID = GetQueryString("FK_Node");
    var rootNo = 0;
    var webUser = new WebUser();
    if (webUser.CCBPMRunModel != 0)
        rootNo = webUser.OrgNo;

    var url = "../../../Comm/RefFunc/Branches.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeDepts&Dot2DotEnName=BP.WF.Template.NodeDept&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Dept&EnsOfM=BP.Port.Depts&DefaultGroupAttrKey=&RootNo=" + rootNo + "&NodeID=" + nodeID + "&PKVal=" + nodeID;

    OpenEasyUiDialogExtCloseFunc(url, '绑定部门', w, h, function () {
        Baseinfo.depts = getDepts();
    });
}

function BindDeptTreeGroup() {

    var nodeID = GetQueryString("FK_Node");
    var webUser = new WebUser();
    var rootNo = 0;
    if (webUser.CCBPMRunModel == 0 || webUser.CCBPMRunModel == 2)
        rootNo = webUser.OrgNo;
    if (webUser.CCBPMRunModel == 1) {
        var orgs = new Entities("BP.WF.Port.Admin2Group.Orgs");
        orgs.RetrieveCond("No", "=", "ParentNo");
        if (orgs.length != 0) {
            rootNo = orgs[0].No;
        }

    }

    var url = "../../../Comm/RefFunc/Branches.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeDepts&Dot2DotEnName=BP.WF.Template.NodeDept&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Dept&EnsOfM=BP.Port.Depts&DefaultGroupAttrKey=&RootNo=" + rootNo + "&NodeID=" + nodeID + "&PKVal=" + nodeID;

    OpenEasyUiDialogExtCloseFunc(url, '绑定部门', w, h, function () {
        Baseinfo.depts = getDepts();
    });
}




//绑定用户组: for admin. ***********************************************************
function OpenDot2DotTeams() {
    var nodeID = GetQueryString("FK_Node");
    var url = "../../../Comm/RefFunc/Dot2Dot.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeTeams";
    url += "&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Team&EnsOfM=BP.Port.Teams";
    url += "&DefaultGroupAttrKey=FK_TeamType&NodeID=" + nodeID + "&PKVal=" + nodeID;
    OpenEasyUiDialogExtCloseFunc(url, '设置用户组', w, h, function () {
        Baseinfo.stas = getStas();
    });
}
//设置岗位-左右结构.
function OpenBranchesAndLeafTeams() {

    var nodeID = GetQueryString("FK_Node");
    var url = "../../../Comm/RefFunc/BranchesAndLeaf.htm?EnName=BP.WF.Template.NodeSheet&Dot2DotEnsName=BP.WF.Template.NodeTeams&Dot2DotEnName=BP.WF.Template.NodeTeam&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Team&EnsOfM=BP.Port.Teams&DefaultGroupAttrKey=FK_TeamType&NodeID=" + nodeID + "&PKVal=" + nodeID;
    OpenEasyUiDialogExtCloseFunc(url, '设置用户组', w, h, function () {
        Baseinfo.stas = getStas();
    });
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
/*
 * 获取节点绑定部门的负责人
 */
function getDeptLeader() {
    var ens = getDepts();
    var depts = new Entities("BP.Port.Depts");

    for (var i = 0; i < ens.length; i++) {
        var en = ens[i];
        depts.Retrieve("No", en.FK_Dept);
    }
    return depts;
}

/*
 * 获取节点绑定的组织
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
/*
 * 获取节点绑定岗位的用户组人员
 */
function getTeamEmps() {
    var en = new Entity("BP.WF.Template.NodeTeam", GetQueryString("FK_Node"));
    var ens = new Entities("BP.Port.TeamEmps");

    ens.Retrieve("FK_Team", en.FK_Team);
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
        case DeliveryWay.ByPreviousNodeFormDepts:
            roleName = "52.ByPreviousNodeFormDepts.htm";
            break;
        case DeliveryWay.ByPreviousNodeFormStationsAI:
            roleName = "53.ByPreviousNodeFormStationsAI.htm";
            break;
        case DeliveryWay.ByPreviousNodeFormStationsOnly:
            roleName = "54.ByPreviousNodeFormStationsOnly.htm";
            break;
        case DeliveryWay.BySelectEmpByOfficer:
            roleName = "55.BySelectEmpByOfficer.htm";
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
        case DeliveryWay.ByDeptShipLeader:
            roleName = "28.ByDeptShipLeader.htm";
            break;
        case DeliveryWay.ByEmpLeader:
            roleName = "50.ByEmpLeader.htm";
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
        case DeliveryWay.ByBindTeamEmp:
            roleName = "27.SelectEmpsByTeamStation.htm";
            break;
        case DeliveryWay.BySelectedOrgs:
            roleName = "42.BySelectedOrgs.htm";
            break;
        case DeliveryWay.BySelectedEmpsOrgModel:
            roleName = "43.BySelectedEmpsOrgModel.htm";
            break;
        case DeliveryWay.BySelfUrl: //自定义url.
            roleName = "44.BySelfUrl.htm";
            break;
        case DeliveryWay.ByAPIUrl:
            roleName = "45.ByAPIUrl.htm";
            break;
        case DeliveryWay.BySenderParentDeptLeader:
            roleName = "46.BySenderParentDeptLeader.htm";
            break;
        case DeliveryWay.BySenderParentDeptStations:
            roleName = "47.BySenderParentDeptStations.htm";
            break;
        case DeliveryWay.ByCCFlowBPM:
            roleName = "100.ByCCFlowBPM.htm";
            break;
        case DeliveryWay.ByGuest:
            roleName = "51.ByGuest.htm";
            break;

        default:
            roleName = "0.ByStation.htm";
            break;
    }

    // alert(roleName);

    SetHref(roleName + "?FK_Node=" + nodeID + "&FK_Flow=" + flowNo);
}
function SaveAndClose() {
    Save();
    window.close();
}

function SaveIt() {

    $("#Btn_Save").html("正在保存请稍后.");

    try {
        Save();
        AfterSave();
    } catch (e) {
        alert(e);
        return;
    }

    $("#Btn_Save").html("保存成功");
    setTimeout(function () { $("#Btn_Save").html("保存."); }, 1000);
}
// 保存之后要做的事情.
function AfterSave() {
    //清除.
    AccepterRole_ClearAllOrgStartFlowsCash();
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

