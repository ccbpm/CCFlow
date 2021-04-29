
function InitBar(optionKey) {

    var html = "<b>自动发起</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+配置模式</option>";

    html += "<option value=" + AutoStart.None + ">&nbsp;&nbsp;&nbsp;&nbsp;手工启动（默认）</option>";
    html += "<option value=" + AutoStart.ByDesignee + ">&nbsp;&nbsp;&nbsp;&nbsp;指定人员按时启动</option>";
    html += "<option value=" + AutoStart.ByTineData + ">&nbsp;&nbsp;&nbsp;&nbsp;数据集按时启动</option>";

    html += "<option value=null  disabled='disabled'>+开发者模式</option>";
    html += "<option value=" + AutoStart.ByTrigger + ">&nbsp;&nbsp;&nbsp;&nbsp;触发试启动</option>";

    html += "</select >";

    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save' type=button onclick='Save()' value='保存' >保存</button>";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<button  id='Btn_Help'class='cc-btn-tab btn-hlep' type=button onclick='HelpOnline()' value='在线帮助' >在线帮助</button>";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function Adv()
{
    var url = "Adv.htm?FK_Flow=" + GetQueryString("FK_Flow");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
}

function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("FK_Flow");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;
    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_Flow=" + flowNo;
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case AutoStart.None:
            url = "0.None.htm";
            break;
        case AutoStart.ByDesignee:
            url = "1.ByDesignee.htm";
            break;
        case AutoStart.ByTineData:
            url = "2.ByTimeData.htm";
            break;
        case AutoStart.ByTrigger:
            url = "3.ByTrigger.htm";
            break;
        default:
            url = "0.None.htm";
            break;
    }

    return url;
}

function CheckFlow(flowNo) {
    var flow = new Entity('BP.WF.Flow', flowNo);
    flow.DoMethodReturnString("DoCheck"); //重置密码:不带参数的方法. 
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