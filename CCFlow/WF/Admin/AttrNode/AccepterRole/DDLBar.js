$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            // alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});


var optionKey = 0;
function InitBar(key) {

    optionKey = key;

    var nodeID = GetQueryString("FK_Node");
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";
    var html = "<div style='padding:5px' >访问规则: ";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+按组织结构绑定</option>";

    if (isSatrtNode == true) {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按照岗位智能计算发起人</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的部门计算发起人</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的人员计算发起人</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算发起人</option>";

    } else {

        html += "<option value=" + DeliveryWay.ByStation + ">&nbsp;&nbsp;&nbsp;&nbsp;按照岗位智能计算</option>";
        html += "<option value=" + DeliveryWay.ByDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的部门计算</option>";
        html += "<option value=" + DeliveryWay.ByBindEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的人员计算</option>";
        html += "<option value=" + DeliveryWay.ByDeptAndStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";
        html += "<option value=" + DeliveryWay.ByStationAndEmpDept + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算并且以绑定的部门集合为纬度</option>";
        html += "<option value=" + DeliveryWay.BySpecNodeEmpStation + " >&nbsp;&nbsp;&nbsp;&nbsp;按指定节点的人员岗位计算</option>";
        html += "<option value=" + DeliveryWay.ByStationOnly + " >&nbsp;&nbsp;&nbsp;&nbsp;仅按绑定的岗位计算</option>";
        html += "<option value=" + DeliveryWay.BySetDeptAsSubthread + " >&nbsp;&nbsp;&nbsp;&nbsp;按绑定部门计算，该部门一人处理标识该工作结束(子线程)</option>";

        html += "<option value=" + DeliveryWay.FindSpecDeptEmps + ">&nbsp;&nbsp;&nbsp;&nbsp;找本部门范围内的岗位集合里面的人员.</option>";
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

        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;所有的人员都可以发起.</option>";

    } else {
        html += "<option value=" + DeliveryWay.BySelected + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“人员选择器”选择接受人</option>";
        html += "<option value=" + DeliveryWay.ByPreviousNodeFormEmpsField + " >&nbsp;&nbsp;&nbsp;&nbsp;按上一节点表单指定的字段值作为本步骤的接受人</option>";
        html += "<option value=" + DeliveryWay.ByDtlAsSubThreadEmps + " >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点的明细表来决定子线程的接受人</option>";
        html += "<option value=" + DeliveryWay.ByFEE + " >&nbsp;&nbsp;&nbsp;&nbsp;由FEE来决定</option>";
        html += "<option value=" + DeliveryWay.ByFromEmpToEmp + ">&nbsp;&nbsp;&nbsp;&nbsp;按照配置的人员路由列表计算</option>";
        html += "<option value=" + DeliveryWay.ByCCFlowBPM + " >&nbsp;&nbsp;&nbsp;&nbsp;按ccBPM的BPM模式处理</option>";
    }
    html += "</select >";
    html += "<input  id='Btn_Save' type=button onclick='SaveIt()' value='保存' />";
    html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //  html += "<input type=button onclick='Help()' value='我需要帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
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
    url += "&AttrOfOneInMM=FK_Node&AttrOfMInMM=FK_Station&EnsOfM=BP.WF.Port.Stations";
    url += "&DefaultGroupAttrKey=FK_StationType&NodeID=" + nodeID + "&PKVal=" + nodeID;

    OpenEasyUiDialogExt(url, '设置岗位', 800, 500, true);
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
        case DeliveryWay.ByStationForPrj:
            roleName = "20.ByStationForPrj.htm";
            break;
        case DeliveryWay.BySelectedForPrj:
            roleName = "21.BySelectedForPrj.htm";
            break;
        case DeliveryWay.ByCCFlowBPM:
            roleName = "100.ByCCFlowBPM.htm";
            break;
        default:
            roleName = "0.ByStation.htm";
            break;
    }

    // alert(roleName);

    window.location.href = roleName + "?FK_Node=" + nodeID;
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