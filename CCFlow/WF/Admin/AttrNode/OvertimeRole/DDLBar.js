$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            //alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});


var optionKey = 0;
function InitBar(key) {

    optionKey = key;

    var webUser = new WebUser();
    var nodeID = GetQueryString("FK_Node");
    var str = nodeID.substr(nodeID.length - 2);
    var isSatrtNode = false;
    if (str == "01")
        isSatrtNode = true;

    // var html = "<div style='background-color:Silver' > 请选择访问规则: ";
    var html = "<div style='padding:5px' >超时处理规则: ";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=" + OvertimeRole.None + ">&nbsp;&nbsp;&nbsp;&nbsp;不处理</option>";
    html += "<option value=" + OvertimeRole.AutoDown + " >&nbsp;&nbsp;&nbsp;&nbsp;自动向下运动</option>";
    html += "<option value=" + OvertimeRole.JumpToNode + " >&nbsp;&nbsp;&nbsp;&nbsp;跳转到指定节点</option>";
    html += "<option value=" + OvertimeRole.TurnToEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;移交给指定的人员</option>";
    html += "<option value=" + OvertimeRole.SendMessageToEmp + " >&nbsp;&nbsp;&nbsp;&nbsp;给指定的人员发送消息</option>";
    html += "<option value=" + OvertimeRole.DeleteFlow + " >&nbsp;&nbsp;&nbsp;&nbsp;删除流程</option>";
    html += "<option value=" + OvertimeRole.RunSql + ">&nbsp;&nbsp;&nbsp;&nbsp;执行SQL</option>";
    html += "</select >";

    html += "<button  id='Btn_Save' onclick='Save()'/>保存</button>";
    //html += "<input  id='Btn_Back' type=button onclick='Back()' value='返回' />";
    //html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //   html += "<input type=button onclick='Help()' value='帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");


}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "超时处理规则" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "不处理", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "审核组件模式", "GroupNo": "A", "Url": "1.WorkCheck.htm" },
        { "No": 2, "Name": "审核字段分组模式URL", "GroupNo": "A", "Url": "2.GroupFieldCheck.htm" },
    ];
    return json;
}

function Back() {
    url = "../AccepterRole/Default.htm?FK_Node=" + GetQueryString("FK_Node") + "&FK_Flow=" + GetQueryString("FK_Flow");
    SetHref(url);
}

function Help() {

    var url = "";
    switch (optionKey) {
        case SelectorModel.Station:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        case SelectorModel.Dept:
            url = 'http://bbs.ccflow.org/showtopic-131376.aspx';
            break;
        default:
            url = "http://ccbpm.mydoc.io/?v=5404&t=17906";
            break;
    }

    window.open(url);
}

function GenerUrlByOptionKey(optionKey) {
    var roleName = "";
    switch (parseInt(optionKey)) {
        case OvertimeRole.None:
            roleName = "0.None.htm";
            break;
        case OvertimeRole.AutoDown:
            roleName = "1.AutoDown.htm";
            break;
        case OvertimeRole.JumpToNode:
            roleName = "2.JumpToNode.htm";
            break;
        case OvertimeRole.TurnToEmp:
            roleName = "3.TurnToEmp.htm";
            break;
        case OvertimeRole.SendMessageToEmp:
            roleName = "4.SendMessageToEmp.htm";
            break;
        case OvertimeRole.DeleteFlow:
            roleName = "5.DeleteFlow.htm";
            break;
        case OvertimeRole.RunSql:
            roleName = "6.RunSql.htm";
            break;
        default:
            roleName = "0.None.htm";
            break;
    }
    return roleName;
}


function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 0) {
        optionKey = sele[index].value
    }
    var roleName = GenerUrlByOptionKey(optionKey);
    SetHref( roleName + "?FK_Node=" + nodeID + "&FK_Flow=" + GetQueryString("FK_Flow"));
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

//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "AdvSetting.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}