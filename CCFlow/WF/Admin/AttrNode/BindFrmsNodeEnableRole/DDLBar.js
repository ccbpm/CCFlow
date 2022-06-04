
function InitBar(optionKey) {

    var html = "表单启用规则:";
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
    html += "<button  id='Btn_Save' type='button'class='cc-btn-tab btn-save' onclick='Save()' value='保存' >保存</button>";
    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "表单启用规则" },

    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "始终启用（默认）", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "有数据时启用", "GroupNo": "A", "Url": "1.ByData.htm" },
        { "No": 2, "Name": "有参数时启用", "GroupNo": "A", "Url": "2.ByPara.htm" },
        { "No": 3, "Name": "按表单的字段表达式", "GroupNo": "A", "Url": "3.ByExp.htm" },
        { "No": 4, "Name": "按SQL表达式", "GroupNo": "A", "Url": "4.BySQL.htm" },
        { "No": 5, "Name": "按选择的岗位", "GroupNo": "A", "Url": "5.Stations.htm" },
        { "No": 6, "Name": "按选择的部门", "GroupNo": "A", "Url": "6.Depts.htm" },
        { "No": 7, "Name": "不启用(禁用) ", "GroupNo": "A", "Url": "7.Disable.htm" },
    ];
    return json;
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}
function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    SetHref( url + "?FK_Node=" + nodeID);
}
//高级设置.
function AdvSetting() {
    var nodeID = GetQueryString("FK_Node");
    var url = "7.ByOtherBlock.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}
function GetUrl(optionKey) {
    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }
    return "0.CCFlowMsg.htm";
}