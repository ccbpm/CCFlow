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
    var html = "<div style='padding:5px' >考核规则: ";

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

    html += "<button  id='Btn_Save' type=button onclick='Save()' value='保存' >保存</button>";
    //html += "<input  id='Btn_Back' type=button onclick='Back()' value='返回' />";
    //html += "<input type=button onclick='AdvSetting()' value='高级设置' />";
    //   html += "<input type=button onclick='Help()' value='帮助' />";
    html += "</div>";

    document.getElementById("bar").innerHTML = html;

    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");


}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "考核规则" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "不考核", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "按照时效考核", "GroupNo": "A", "Url": "1.ByTime.htm" },
        { "No": 2, "Name": "按工作量考核", "GroupNo": "A", "Url": "2.ByWorkNum.htm" },
        { "No": 3, "Name": "是否是考核质量点", "GroupNo": "A", "Url": "3.IsQuality.htm" },
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
    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }
    return "0.None.htm";

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