

function InitBar(optionKey) {

    var html = "<b>自动发起</b>:";

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
    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save'  onclick='RunIt()' >手工运行</button>";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<button  id='Btn_Help'class='cc-btn-tab btn-hlep' onclick='HelpOnline()'>在线帮助</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [
        { "No": "A", "Name": "配置模式" },
        { "No": "B", "Name": "开发者模式" }

    ];
    return json;
}
function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "手工启动（默认）", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "指定人员按时启动", "GroupNo": "A", "Url": "1.ByDesignee.htm" },
        { "No": 2, "Name": "数据集按时启动", "GroupNo": "A", "Url": "2.ByTimeData.htm" },
        { "No": 4, "Name": "指定人员集合按时启动", "GroupNo": "A", "Url": "4.ByDesigneeAdv.htm" },
        { "No": 5, "Name": "以admin启动发送给指定人员集合", "GroupNo": "A", "Url": "5.ByDesigneeAdminSendTo02Node.htm" },
        { "No": 3, "Name": "触发试启动", "GroupNo": "B", "Url": "3.ByTrigger.htm" }
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

function Adv() {
    var url = "Adv.htm?FK_Flow=" + GetQueryString("FK_Flow");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
}
function RunIt() {
    var url = basePath + "/DataUser/AppCoder/Default.htm";
    window.open(url);
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
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

    SetHref( url + "?FK_Flow=" + flowNo);
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