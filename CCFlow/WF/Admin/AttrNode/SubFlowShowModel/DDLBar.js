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
    var html = "<div style='padding:5px' >启动子流程模式: ";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=" + SubFlowStartModel.Single + ">&nbsp;&nbsp;0.单条子流程启动模式</option>";
    html += "<option value=" + SubFlowStartModel.Simple + " >&nbsp;&nbsp;1.列表批量启动模式</option>";
    html += "<option value=" + SubFlowStartModel.Group + " >&nbsp;&nbsp;2.分组数据源列表启动模式</option>";
    html += "<option value=" + SubFlowStartModel.Tree + " >&nbsp;&nbsp;3.树结构启动模式</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //html += "<input  id='Btn_Back' type=button onclick='Back()' value='返回' />";
    //html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //   html += "<input type=button onclick='Help()' value='帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GenerUrlByOptionKey(optionKey) {
    var roleName = "";
    switch (parseInt(optionKey)) {
        case SubFlowStartModel.Single:
            roleName = "0.Single.htm";
            break;
        case SubFlowStartModel.Simple:
            roleName = "1.Simple.htm";
            break;
        case SubFlowStartModel.Group:
            roleName = "2.Group.htm";
            break;
        case SubFlowStartModel.Tree:
            roleName = "3.Tree.htm";
            break;
        default:
            roleName = "0.Single.htm";
            break;
    }
    return roleName;
}


function changeOption() {
    var mypk = GetQueryString("MyPK");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 0) {
        optionKey = sele[index].value
    }
    var roleName = GenerUrlByOptionKey(optionKey);
    SetHref( roleName + "?MyPK=" + mypk + "&FK_Flow=" + GetQueryString("FK_Flow");
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