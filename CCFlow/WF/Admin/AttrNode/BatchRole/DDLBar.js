
function InitBar(optionKey) {

    var html = "批处理模式:";
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
    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save' type=button onclick='Save()' value='保存' >保存</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "批处理规则" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [
        { "No": 0, "Name": "不处理", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "审核组件模式", "GroupNo": "A", "Url": "1.WorkCheck.htm" },
        { "No": 2, "Name": "审核字段分组模式", "GroupNo": "A", "Url": "2.GroupFieldCheck.htm" },
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
    SetHref( url + "?FK_Node=" + nodeID +"&FK_Flow=" + GetQueryString("FK_Flow"));
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