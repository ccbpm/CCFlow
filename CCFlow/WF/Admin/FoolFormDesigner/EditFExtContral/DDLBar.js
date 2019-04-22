
function InitBar(optionKey) {

    var html = "<b>扩展控件</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='0' >无,不设置(默认).</option>";
    html += "<option value='10' >文本</option>";
    html += "<option value='9' >超链接</option>";
    html += "<option value='8' >手写签名版</option>";
    html += "<option value='6' >设置为附件展示字段</option>";
    html += "<option value='4' >地图控件</option>";
    html += "<option value='7' >手机拍照控件</option>";
    html += "<option value='5' >手机录音控件</option>";
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

    var fk_MapData = GetQueryString("FK_MapData");
    var KeyOfEn = GetQueryString("KeyOfEn");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_MapData=" + fk_MapData + "&KeyOfEn=" + KeyOfEn;
}

function GetUrl(extModel) {


    switch (extModel) {
        case "0":
            url = "0.None.htm";
            break;
        case "10":
            url = "10.Text.htm";
            break;
        case "9":
            url = "9.HyperLink.htm";
            break;
        case "8":
            url = "8.HandWriting.htm";
            break;
        case "6":
            url = "6.AthShow.htm";
            break;
        case "4":
            url = "4.MapPin.htm";
            break;
        case "7":
            url = "7.MobilePhoto.htm";
            break;
        case "5":
            url = "5.MicHot.htm";
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