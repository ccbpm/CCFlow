
function InitBar(optionKey) {

    var html = "<b>流程计划时间计算</b>:";

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

    html += "</select >";

   /* html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";*/

    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save'  onclick='Save()' >保存</button>";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<button  id='Btn_Help'class='cc-btn-tab btn-hlep' onclick='HelpOnline()'>在线帮助</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "流程计划时间计算" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "不使用", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "按照节点表单的日期计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },
        { "No": 2, "Name": "按照sql计算", "GroupNo": "A", "Url": "2.SQLDT.htm" },
        { "No": 3, "Name": "按照所有节点的时间之和计算", "GroupNo": "A", "Url": "3.NodeSumDT.htm" },
        { "No": 4, "Name": "按照规定的天数计算", "GroupNo": "A", "Url": "4.DaysDT.htm" },
    ];
    return json;
}

function Adv()
{
    var url = "Adv.htm?FK_Flow=" + GetQueryString("FK_Flow");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
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

function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }

    return "0.None.htm";
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