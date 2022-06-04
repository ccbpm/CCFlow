

function InitBar(optionKey) {

    var html = "<b>列表数据源</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    var groups = GetDBGroup();
    var dtls = GetDBDtl();

    for (var i = 0; i < groups.length; i++) {

        var group = groups[i];
        html += "<option value=null  disabled='disabled'>+" + group.Name + "</option>";

        for (var idx = 0; idx < dtls.length; idx++) {
            var dtl = dtls[idx];
            if (dtl.GroupNo != group.No) continue;
            html += "<option value=" + dtl.No + ">&nbsp;&nbsp;" + dtl.Name + "</option>";
        }
    }
    html += "</select >";

    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save'  onclick='Save()' >保存</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [
        { "No": "A", "Name": "SQL模式" },
        { "No": "B", "Name": "Url模式" },
        { "No": "C", "Name": "Javascript函数模式" }

    ];
    return json;
}

function GetDBDtl() {

    var json = [
        { "No": "0", "Name": "SQL数据源", "GroupNo": "A", "Url": "SQL.htm" },
        { "No": "2", "Name": "前台分页（开发中）", "GroupNo": "B", "Url": "Back.htm" },
        { "No": "3", "Name": "后台分页（开发中）", "GroupNo": "B", "Url": "Back.htm" },
        { "No": "4", "Name": "前台分页（开发中）", "GroupNo": "C", "Url": "Back.htm" },
        { "No": "5", "Name": "后台分页（开发中）", "GroupNo": "C", "Url": "Back.htm" }
    ];
    return json;
}
function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }

    return "0.None.htm";
}


function changeOption() {

    var frmID = GetQueryString("FrmID");
    if (frmID == null)
        frmID = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    SetHref(url + "?FrmID=" + frmID);
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
