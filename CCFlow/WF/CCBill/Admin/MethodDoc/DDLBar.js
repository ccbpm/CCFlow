

function InitBar(optionKey) {

    var html = "";
    // html += " <button  id='Btn_Attrs' class='cc-btn-tab btn-save'  onclick='FrmAttrs()' >表单字段</button>";
    // html += " <button  id='Btn_Paras' class='cc-btn-tab btn-save'  onclick='Paras()' >参数</button>";


    html += "<b>&nbsp;&nbsp选择脚本类型</b>:";

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

    html += "</select>";

    /* html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
     html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
     html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";*/


    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save'  style='margin-right:50px;' onclick='Save()' >保存</button>";

    //html += "更换主题：";
    //html += " <select onchange=selectTheme() id=select>";
    //html += " <option>eclipse</option>";
    //html += " <option>elegant</option>";
    //html += "  <option selected=selected>erlang-dark</option>";
    //html += "  <option>idea</option>";
    //html += " </select>";

    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    // html += "<button  id='Btn_Help'class='cc-btn-tab btn-hlep' onclick='HelpOnline()'>在线帮助</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [
        { "No": "A", "Name": "选择业务表达载体" }
    ];
    return json;
}
function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "执行SQL模式", "GroupNo": "A", "Url": "0.SQL.htm" },
        { "No": 1, "Name": "执行javascript脚本", "GroupNo": "A", "Url": "1.JS.htm" },
        { "No": 2, "Name": "执行Url", "GroupNo": "A", "Url": "2.Url.htm" },
        { "No": 3, "Name": "执行后台方法", "GroupNo": "A", "Url": "3.DTS.htm" }

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

function Paras() {

    var url = "MethodParas.htm?No=" + GetQueryString("No");
    SetHref(url);
    // OpenEasyUiDialogExt(url, '参数', 600, 400, false);
}

function FrmAttrs() {

    var no = GetQueryString("No");
    var en = new Entity("BP.CCBill.Template.MethodFunc", no);

    var url = "Attrs.htm?No=" + GetQueryString("No") + "&FrmID=" + en.FrmID;
    window.open(url);
    // SetHref(url);
    // OpenEasyUiDialogExt(url, '参数', 600, 400, false);
}


function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("No");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    SetHref(url + "?No=" + flowNo);
}
function ToMethodParas() {
    SetHref("MethodParas.htm?No=" + GetQueryString("No"));
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