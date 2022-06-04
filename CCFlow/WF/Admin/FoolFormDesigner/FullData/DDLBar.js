
function InitBar(optionKey) {

    var html = "<b>数据填充</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='Main' >填充主表</option>";
    html += "<option value='Dtls' >填充从表</option>";
    if (extType != "PageLoadFull")
        html += "<option value='DDLs' >填充下拉框</option>";
    html += "</select >";


    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input type='button' value='返回' onclick='Back()' id='Btn_Back' title='' />";
    html += "<input  id='Btn_Help' type=button onclick='EtcDBFull()' value='多数据源填充(列/字段)' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}
function DBTypeChange() {

    var val = $("#DDL_DBType").val();
    if (val == 0) {
        $("#DBSrc").show();
    } else {
        $("#DBSrc").hide();
    }
}


function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function Back() {

    //    var myPK = GetQueryString('MyPK');
    var refPK = GetQueryString("RefPK");
    //    var keyOfEn = refPK.split("_")[2];
    var extType = GetQueryString("ExtType");


    if (refPK.indexOf('TBFullCtrl') == 0)
        var url = '../TBFullCtrl/Default.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn");

    if (refPK.indexOf('DDLFullCtrl') == 0)
        var url = '../MapExt/DDLFullCtrl2019.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn") + "&ExtType=" + extType;

    if (refPK.indexOf('Pop') == 0)
        var url = '../Pop/Default.htm?FK_MapData=' + GetQueryString('FK_MapData') + "&KeyOfEn=" + GetQueryString("KeyOfEn");

    if (refPK.indexOf('PageLoadFull') == 0)
        var url = '../MapExt/PageLoadFull.htm?FK_MapData=' + GetQueryString('FK_MapData');


    SetHref(url);
    return;
}

function changeOption() {

    var refPK = GetQueryString("RefPK");
    var fk_mapData = GetQueryString("FK_MapData");


    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    SetHref(url + "?RefPK=" + refPK + "&FK_MapData=" + fk_mapData + "&KeyOfEn=" + GetQueryString('KeyOfEn') + "&ExtType=" + GetQueryString("ExtType"));
}

function GetUrl(popModel) {

    switch (popModel) {
        case "DDLs":
            url = "DDLs.htm";
            break;
        case "Dtls":
            url = "Dtls.htm";
            break;
        case "Main":
            url = "Main.htm";
            break;
        default:
            url = "Main.htm";
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