
function InitBar(optionKey) {

    var html = "请选择访问规则:";

    html += "<select id='changBar' onchange='changeOption()'>";
    html += "<option value=0 >内置傻瓜表单</option>";
    html += "<option value=1 >内置自由表单</option>";
    html += "<option value=2 >嵌入式表单</option>";
    html += "<option value=3 >SDK表单</option>";
    html += "<option value=5 >表单树（多表单）</option>";
    //  html += "<option value=5 >表单树（多表单）</option>";
    html += "<option value=7 >公文表单</option>";
  //  html += "<option value=8 >Excel表单</option>";
   // html += "<option value=9 >Word表单</option>";
    html += "<option value=10 >软通动力（傻瓜轨迹表单）</option>";
    html += "<option value=11 >表单库表单</option>";
  //  html += "<option value=100 >禁用</option>";
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
            roleName = "15.ByFEE.htm";
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
    window.location.href = roleName + "?FK_Node=" + nodeID;
}
function SaveAndClose() {

    Save();
    window.close();
}