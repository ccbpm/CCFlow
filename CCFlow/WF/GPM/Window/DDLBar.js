
function InitBar(optionKey) {

    var html = "菜单类型:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var groups = GetDBGroup();
    var dtls = GetDBDtl();

    for (var i = 0; i < groups.length; i++) {

        var group = groups[i];
        html += "<option value=null  disabled='disabled'>+" + group.Name + "</option>";

        for (var idx = 0; idx < dtls.length; idx++) {
            var dtl = dtls[idx];
            if (dtl.GroupNo != group.No)
                continue;
            html += "<option value=" + dtl.No + ">&nbsp;&nbsp;" + dtl.Name + "</option>";
        }
    }

    html += "</select>";
    html += "<button  id='Btn_Save' type=button class='cc-btn-tab btn-save' onclick='Save()'>创建</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [
        { "No": "A", "Name": "文字类型" },
        { "No": "B", "Name": "图形" },
        { "No": "C", "Name": "列表" }
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": "Html", "Name": "Html文本", "GroupNo": "A", "Url": "Html.htm" },
        { "No": "HtmlVar", "Name": "变量文本", "GroupNo": "A", "Url": "HtmlVar.htm" },

        { "No": "ChartLine", "Name": "折线图", "GroupNo": "B", "Url": "ChartLine.htm" },
        { "No": "ChartPie", "Name": "饼图", "GroupNo": "B", "Url": "ChartPie.htm" },
        { "No": "ChartZZT", "Name": "柱状图", "GroupNo": "B", "Url": "ChartZZT.htm" },
         
        { "No": "Table", "Name": "简单表格", "GroupNo": "C", "Url": "Table.htm" }

    ];
    return json;
}


function Close() {
    window.close();
    window.Close();
    window.closed();
}

function Back() {
    var url = "../Menus.htm";
    window.location.href = url;
}

function Adv() {
    var url = "Adv.htm?MenuNo=" + GetQueryString("MenuNo");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("MenuNo");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    window.location.href = url + "?MenuNo=" + flowNo;
}

function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No === optionKey)
            return en.Url;
    }
    alert(optionKey);
    return "0.QiangBan.htm";
}


function SaveAndClose() {

    Save();
    window.close();
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