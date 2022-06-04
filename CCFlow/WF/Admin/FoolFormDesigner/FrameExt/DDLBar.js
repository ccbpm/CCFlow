
function InitBar(optionKey) {

    var html = "<b>框架预制功能</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+基本设置</option>";
    html += "<option value='Self' >自定义URL.</option>";
    html += "<option value='Map' disabled='disabled' >地图(开发中)</option>";

    html += "<option value=null  disabled='disabled'>+流程相关</option>";
    html += "<option value='FlowTable' >流程轨迹表</option>";
    html += "<option value='FlowChart' >流程轨迹图</option>";
    html += "<option value='FlowJobSchedule' >简易进度图(进展中)</option>";
    html += "<option value='FlowGanttChart' >甘特图(进展中)</option>";


    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function changeOption() {

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    SetHref(url + "?MyPK=" + GetQueryString("MyPK"));
}

function GetUrl(popModel) {

    switch (popModel) {
        case "Self":
        case "0":
        case 0:
            url = "0.Self.htm";
            break;
        case "Map":
        case "1":
        case 1:
            url = "1.Map.htm";
            break;
        case "FlowTable": //流程表.
        case "2":
        case 2:
            url = "2.FlowTable.htm";
            break;
        case "FlowChart":
        case "3": //流程轨迹图.
        case 3:
            url = "3.FlowChart.htm";
            break;
        case "FlowJobSchedule":  //工作进度图.
        case "4":
        case 4:
            url = "4.FlowJobSchedule.htm";
            break;
        case "FlowGanttChart":  //甘特图.
        case "5":
        case 5:
            url = "5.FlowGanttChart.htm";
            break;
        default:
            url = "0.Self.htm";
            break;
    }
    return url;
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