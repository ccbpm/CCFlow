
function InitBar(optionKey) {
    
    var html = "请选择访问规则:";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+按组织结构绑定</option>";
    html += "<option value=0 >&nbsp;&nbsp;&nbsp;&nbsp;按照岗位智能计算</option>";
    html += "<option value=1 >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的部门计算</option>";
    html += "<option value=3 >&nbsp;&nbsp;&nbsp;&nbsp;按节点绑定的人员计算</option>";
    html += "<option value=9 >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位与部门交集计算</option>";
    html += "<option value=10 >&nbsp;&nbsp;&nbsp;&nbsp;按绑定的岗位计算并且以绑定的部门集合为纬度</option>";
    html += "<option value=11 >&nbsp;&nbsp;&nbsp;&nbsp;按指定节点的人员岗位计算</option>";
    html += "<option value=14 >&nbsp;&nbsp;&nbsp;&nbsp;仅按绑定的岗位计算</option>";
    html += "<option value=16 >&nbsp;&nbsp;&nbsp;&nbsp;按绑定部门计算，该部门一人处理标识该工作结束(子线程)</option>";

    html += "<option value=null disabled='disabled' >+按访问规则选项</option>";
    html += "<option value=7 >&nbsp;&nbsp;&nbsp;&nbsp;与开始节点处理人相同</option>";
    html += "<option value=6 >&nbsp;&nbsp;&nbsp;&nbsp;与上一节点处理人相同</option>";
    html += "<option value=8 >&nbsp;&nbsp;&nbsp;&nbsp;与指定节点处理人相同</option>";

    html += "<option value=null disabled='disabled' >+按自定义SQL查询</option>";
    html += "<option value=2 >&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQL获取接受人计算</option>";
    html += "<option value=17 >&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQLTempate获取接受人计算</option>";
    html += "<option value=12 >&nbsp;&nbsp;&nbsp;&nbsp;按SQL确定子线程接受人与数据源</option>";

    html += "<option value=null disabled='disabled' >+其他方式</option>";
    html += "<option value=4 >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点发送人通过“人员选择器”选择接受人</option>";
    html += "<option value=5 >&nbsp;&nbsp;&nbsp;&nbsp;按上一节点表单指定的字段值作为本步骤的接受人</option>";
    html += "<option value=13 >&nbsp;&nbsp;&nbsp;&nbsp;由上一节点的明细表来决定子线程的接受人</option>";
    html += "<option value=15 >&nbsp;&nbsp;&nbsp;&nbsp;由FEE来决定</option>";
    html += "<option value=‘RB_ByFromEmpToEmp’ >&nbsp;&nbsp;&nbsp;&nbsp;按照配置的人员路由列表计算</option>";
    html += "<option value=100 >&nbsp;&nbsp;&nbsp;&nbsp;按ccBPM的BPM模式处理</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input type=button onclick='SaveAndClose()' value='保存&关闭' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected"); 
}
function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = "1";
    if (index > 1) {
        optionKey = sele[index].value
    }
    var roleName = "";
    switch (optionKey) {
        case "0":
            roleName = "0.ByStation.htm";
            break;
        case "1":
            roleName = "1.ByDept.htm";
            break;
        case "2":
            roleName = "2.BySQL.htm";
            break;
        case "3":
            roleName = "3.ByBindEmp.htm";
            break;
        case "4":
            roleName = "4.BySelected.htm";
            break;
        case "5":
            roleName = "5.ByPreviousNodeFormEmpsField.htm";
            break;
        case "6":
            roleName = "6.ByPreviousNodeEmp.htm";
            break;
        case "7":
            roleName = "7.ByStarter.htm";
            break;
        case "8":
            roleName = "8.BySpecNodeEmp.htm";
            break;
        case "9":
            roleName = "9.ByDeptAndStation.htm";
            break;
        case "10":
            roleName = "10.ByStationAndEmpDept.htm";
            break;
        case "11":
            roleName = "11.BySpecNodeEmpStation.htm";
            break;
        case "12":
            roleName = "12.BySQLAsSubThreadEmpsAndData.htm";
            break;
        case "13":
            roleName = "13.ByDtlAsSubThreadEmps.htm";
            break;
        case "14":
            roleName = "14.ByStationOnly.htm";
            break;
        case "15":
            roleName = "15.ByFEEp.htm";
            break;
        case "16":
            roleName = "16.BySetDeptAsSubthread.htm";
            break;        
        case "17":
            roleName = "17.BySQLTemplate.htm";
            break;
        case "18":
            roleName = "18.ByFromEmpToEmp.htm";
            break;
        case "100":
            roleName = "100.ByCCFlowBPM.htm";
            break;
        default:
            roleName = "0.ByStation.htm";
            break;
    }
    window.location.href =   roleName + "?FK_Node=" + nodeID;
}
function SaveAndClose() {

    Save();
    window.close();
}