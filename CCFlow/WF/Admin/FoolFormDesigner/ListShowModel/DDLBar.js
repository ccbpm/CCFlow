
function InitBar(optionKey) {

    var html = "<b>展示模式</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+常规模式</option>";
    html += "<option value=" + ListShowModel.Table + ">&nbsp;&nbsp;&nbsp;&nbsp;表格(默认)</option>";
    html += "<option value=" + ListShowModel.Card + ">&nbsp;&nbsp;&nbsp;&nbsp;卡片模式</option>";
    html += "<option value=" + ListShowModel.Self + ">&nbsp;&nbsp;&nbsp;&nbsp;自定义URL</option>";

    html += "<option value=null  disabled='disabled'>+报表模式</option>";
    html += "<option value=" + ListShowModel.TwoD + ">&nbsp;&nbsp;&nbsp;&nbsp;2维表</option>";
    html += "<option value=" + ListShowModel.ThreeDL + ">&nbsp;&nbsp;&nbsp;&nbsp;3维表(左)</option>";
    html += "<option value=" + ListShowModel.ThreeDT + ">&nbsp;&nbsp;&nbsp;&nbsp;3维表(上)</option>";

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

    var frmID = GetQueryString("FK_MapData");
    
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    SetHref(url + "?FK_MapData=" + frmID);
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case ListShowModel.Table:
            url = "0.Table.htm";
            break;
        case ListShowModel.Card:
            url = "1.Card.htm";
            break;
        case ListShowModel.Self:
            url = "2.Self.htm";
            break;
        case ListShowModel.TwoD:
            url = "3.TwoD.htm";
            break;
        case ListShowModel.ThreeDL:
            url = "4.ThreeDL.htm";
            break;
        case ListShowModel.ThreeDT:
            url = "5.ThreeDT.htm";
            break;
        default:
            url = "0.Table.htm";
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