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
function InitBar(optionKey) {

    var webUser = new WebUser();

    var html = "<div style='padding:5px' >表单组件: ";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+通用组件</option>";

    html += "<option value=" + FrmComponents.Station + ">&nbsp;&nbsp;&nbsp;&nbsp;装饰类图片</option>";
    html += "<option value=" + FrmComponents.Dept + " >&nbsp;&nbsp;&nbsp;&nbsp;图片附件 </option>";
    html += "<option value=" + FrmComponents.Emp + " >&nbsp;&nbsp;&nbsp;&nbsp;身份证 </option>";
    html += "<option value=" + FrmComponents.SQL + " >&nbsp;&nbsp;&nbsp;&nbsp;多附件</option>";
    html += "<option value=" + FrmComponents.SQLTemplate + " >&nbsp;&nbsp;&nbsp;&nbsp;超链接 </option>";
    html += "<option value=" + FrmComponents.GenerUserSelecter + " >&nbsp;&nbsp;&nbsp;&nbsp;写字板</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;评分控件</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;大块Html说明文字引入</option>";

    html += "<option value=null  disabled='disabled'>+流程组件</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;签批组件</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;评论（抄送）组件</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;公文字号</option>";

    html += "<option value=null  disabled='disabled'>+移动端控件</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;系统定位</option>";
    html += "<option value=" + FrmComponents.DeptAndStation + ">&nbsp;&nbsp;&nbsp;&nbsp;系统定位</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //html += "<input  id='Btn_Save' type=button onclick='Back()' value='返回' />";
    //    html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //   html += "<input type=button onclick='Help()' value='我需要帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    //  $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}
function Back() {
    url = "../AccepterRole/Default.htm?FK_Node=" + GetQueryString("FK_Node");
    window.location.href = url;
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
        case SelectorModel.Station:
            roleName = "0.Station.htm";
            break;
        case SelectorModel.Dept:
            roleName = "1.Dept.htm";
            break;
        case SelectorModel.Emp:
            roleName = "2.Emp.htm";
            break;
        case SelectorModel.SQL:
            roleName = "3.SQL.htm";
            break;
        case SelectorModel.SQLTemplate:
            roleName = "4.SQLTemplate.htm";
            break;
        case SelectorModel.GenerUserSelecter:
            roleName = "5.GenerUserSelecter.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "6.DeptAndStation.htm";
            break;
        case SelectorModel.Url:
            roleName = "7.Url.htm";
            break;
        case SelectorModel.BySpecNodeEmp:
            roleName = "8.AccepterOfDeptStationEmp.htm";
            break;
        case SelectorModel.DeptAndStation:
            roleName = "9.AccepterOfDeptStationOfCurrentOper.htm";
            break;
        case SelectorModel.Group:
            roleName = "10.Group.htm";
            break;
        case SelectorModel.GroupOnly:
            roleName = "11.GroupOnly.htm";
            break;
        default:

            roleName = "0.Station.htm";
            break;
    }
    return roleName;
}

//下拉框变化的事件.
function changeOption() {

    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = 0;
    if (index > 1) {
        optionKey = sele[index].value
    }

    //根据key获取url.
    var url = GenerUrlByOptionKey(optionKey);

    //转到这个url.
    window.location.href = url + "?FK_Node=" + nodeID;
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

//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "AdvSetting.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}
