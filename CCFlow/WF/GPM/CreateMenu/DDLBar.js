
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
        { "No": "A", "Name": "通用功能" },
        { "No": "B", "Name": "实体单据" },
        { "No": "C", "Name": "字典表" }
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": MenuModel.SelfUrl, "Name": "自定义URL菜单", "GroupNo": "A", "Url": "SelfUrl.htm" },
        { "No": MenuModel.FlowUrl, "Name": "内置流程菜单", "GroupNo": "A", "Url": "FlowUrl.htm" },
        { "No": MenuModel.Func, "Name": "独立功能(方法)页", "GroupNo": "A", "Url": "Func.htm" },
        { "No": "Windows", "Name": "信息窗/大屏(统计分析图表)", "GroupNo": "A", "Url": "Windows.htm" },
        { "No": "Tabs", "Name": "Tabs页面容器", "GroupNo": "A", "Url": "Tabs.htm" },

        { "No": MenuModel.StandAloneFlow, "Name": "创建独立运行的流程", "GroupNo": "A", "Url": "StandAloneFlow.htm" },

        { "No": MenuModel.Dict, "Name": "创建实体", "GroupNo": "B", "Url": "Dict.htm" },
        { "No": "DBList", "Name": "数据源实体", "GroupNo": "B", "Url": "DBList.htm" },

        /*    { "No": "DictQRCode", "Name": "表单填报二维码", "GroupNo": "B", "Url": "DictQRCode.htm" },*/
        { "No": MenuModel.DictCopy, "Name": "复制实体", "GroupNo": "B", "Url": "DictCopy.htm" },
        { "No": MenuModel.Bill, "Name": "创建单据", "GroupNo": "B", "Url": "Bill.htm" },
        { "No": MenuModel.DictRef, "Name": "引入实体", "GroupNo": "B", "Url": "DictRef.htm" },

        /* { "No": MenuModel.BillRef, "Name": "引入单据", "GroupNo": "B", "Url": "BillRef.htm" },*/

        { "No": MenuModel.DictTable, "Name": "创建字典表", "GroupNo": "C", "Url": "DictTable.htm" }
        /*     { "No": MenuModel.DictTableSpecNo, "Name": "引入字典表", "GroupNo": "C", "Url": "DictTableSpecNo.htm" }*/

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
    var url = "Adv.htm?ModuleNo=" + GetQueryString("ModuleNo");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("ModuleNo");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    window.location.href = url + "?ModuleNo=" + flowNo;
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