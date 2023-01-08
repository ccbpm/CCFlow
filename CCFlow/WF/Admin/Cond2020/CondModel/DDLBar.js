
function InitBar(optionKey) {

    var html = "<b>转向规则</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+主观选择</option>";
    html += "<option value=" + DirCondModel.ByDDLSelected + ">&nbsp;&nbsp;&nbsp;&nbsp;下拉框模式</option>";
    html += "<option value=" + DirCondModel.ByButtonSelected + ">&nbsp;&nbsp;&nbsp;&nbsp;按钮模式</option>";
    html += "<option value=" + DirCondModel.ByPopSelect + ">&nbsp;&nbsp;&nbsp;&nbsp;发送后手工选择到达节点与接受人</option>";

    html += "<option value=null  disabled='disabled'>+自动计算</option>";
    html += "<option value=" + DirCondModel.ByLineCond + ">&nbsp;&nbsp;&nbsp;&nbsp;由连接线设置的条件控制</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='帮助' />";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}

function changeOption() {

    var nodeNo = GetQueryString("FK_Node");
    if (nodeNo == null)
        nodeNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    SetHref(url + "?FK_Node=" + nodeNo);
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case DirCondModel.ByLineCond:
            url = "0.ByLineCond.htm";
            break;
        case DirCondModel.ByDDLSelected:
            url = "1.ByDDLSelected.htm";
            break;
        case DirCondModel.ByPopSelect:
            url = "2.ByPopSelect.htm";
            break;
        case DirCondModel.ByButtonSelected:
            url = "3.ByButtonSelected.htm";
            break;
        default:
            url = "0.ByLineCond.htm";
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