
var frmID = null;
var flowNo = null;
var nodeID = null;



function InitBar(optionKey) {

    //补充一些变量信息.
    frmID = GetQueryString("FK_MapData");
    if (frmID == null || frmID == undefined)
        frmID = GetQueryString("FrmID");

    flowNo = GetQueryString("FK_Flow");
    if (flowNo == null || flowNo == undefined)
        flowNo = GetQueryString("FlowNo");

    nodeID = GetQueryString("FK_Node");
    if (nodeID == null || nodeID == undefined)
        nodeID = GetQueryString("NodeID");


    var html = "<b>请选择导入/导出模式</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+导入表单</option>";
    html += "<option value=" + Imp.localhostImp + ">&nbsp;&nbsp;&nbsp;&nbsp;本地xml表单模版导入</option>";
    html += "<option value=" + Imp.NodeFrmImp + ">&nbsp;&nbsp;&nbsp;&nbsp;本流程的节点表单导入</option>";
    html += "<option value=" + Imp.FlowFrmImp + ">&nbsp;&nbsp;&nbsp;&nbsp;其他流程导入</option>";
    html += "<option value=" + Imp.FrmLibraryImp + ">&nbsp;&nbsp;&nbsp;&nbsp;表单库导入</option>";

    html += "<option value=null  disabled='disabled'>+从表结构导入生成表单</option>";
    html += "<option value=" + Imp.ExternalDataSourseImp + ">&nbsp;&nbsp;&nbsp;&nbsp;外部数据源导入</option>";
    html += "<option value=" + Imp.WebAPIImp + ">&nbsp;&nbsp;&nbsp;&nbsp;WebAPI接口导入</option>";

    html += "<option value=null  disabled='disabled'>+导出</option>";
    html += "<option value=" + Imp.ExportFrm + ">&nbsp;&nbsp;&nbsp;&nbsp;导出表单模板</option>";

    html += "</select >";

    //html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    ////html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    //html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}

function changeOption() {

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    window.location.href = url + '?FK_MapData=' + GetQueryString("FK_MapData") + "&DoType=FunList&FK_Flow=" + GetQueryString("FK_Flow") + "&FK_Node=" + GetQueryString("FK_Node");
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case Imp.localhostImp:
            url = "localhostImp.htm";
            break;
        case Imp.NodeFrmImp:
            url = "NodeFrmImp.htm";
            break;
        case Imp.FlowFrmImp:
            url = "FlowFrmImp.htm";
            break;
        case Imp.FrmLibraryImp:
            url = "FrmLibraryImp.htm";
            break;
        case Imp.ExternalDataSourseImp:
            url = "ExternalDataSourseImp.htm";
            break;
        case Imp.ExportFrm:
            url = "ExportFrm.htm";
            break;
        case Imp.WebAPIImp:
            url = "WebAPIImp.htm";
            break;
        default:
            url = "localhostImp.htm";
            break;
    }

    return url;
}

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