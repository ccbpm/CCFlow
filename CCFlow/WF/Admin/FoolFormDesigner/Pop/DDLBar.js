
function InitBar(optionKey) {

    var html = "<b>设置Pop返回值模式</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='None' >无,不设置(默认).</option>";
    html += "<option value='Branches' >树干叶子模式</option>";
    html += "<option value=2 >Excel文件模式</option>";
    html += "<option value=3 >单据模式</option>";
    html += "<option value=4 >表格查询模式（简洁）</option>";

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

function GetUrl(popModel) {

    switch (popModel) {
        case "None":
            url = "0.None.htm";
            break;
        case "Branches":
            url = "1.Branches.htm";
            break;
        case 2:
            url = "2.ExcelFile.htm";
            break;
        case 3:
            url = "3.BillModel.htm";
            break;
        case 4:
            url = "4.TableSimple.htm";
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