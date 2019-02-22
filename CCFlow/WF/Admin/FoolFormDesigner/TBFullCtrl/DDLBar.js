
function InitBar(optionKey) {

    var html = "<b>文本框自动完成</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='None' >无,不设置(默认).</option>";
    html += "<option value='Simple' >简洁模式</option>";
    html += "<option value='Table' >表格模式</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_FullData' type=button onclick='FullData()' value='填充设置' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

function FullData() {
 
    var myPK = "TBFullCtrl_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn");
    var url = "../FullData/Main.htm?FK_MapData=" + this.GetQueryString("FK_MapData") + "&RefPK=" + myPK + "&KeyOfEn=" + GetQueryString("KeyOfEn");

    window.location.href = url;

}


function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function changeOption() {

    var fk_MapData = GetQueryString("FK_MapData");
    var KeyOfEn = GetQueryString("KeyOfEn");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_MapData=" + fk_MapData + "&KeyOfEn=" + KeyOfEn;
}

function GetUrl(popModel) {


    switch (popModel) {
        case "None":
            url = "0.None.htm";
            break;
        case "Simple":
            url = "1.Simple.htm";
            break;
        case "Table":
            url = "2.Table.htm";
            break;
        default:
            url = "0.None.htm";
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
            window.location.href = window.location.href;
        }
    });
}