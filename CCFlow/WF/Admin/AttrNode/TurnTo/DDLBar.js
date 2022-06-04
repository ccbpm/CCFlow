
function InitBar(optionKey) {

    var html = "发送后转向:";
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

        { "No": "A", "Name": "转向规则" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "提示CCFlow默认信息", "GroupNo": "A", "Url": "0.CCFlowMsg.htm" },
        { "No": 1, "Name": "提示指定信息", "GroupNo": "A", "Url": "1.SpecMsg.htm" },
        { "No": 2, "Name": "转向指定的URL", "GroupNo": "A", "Url": "2.SpecUrl.htm" },
        { "No": 3, "Name": "发送完成立即关闭", "GroupNo": "A", "Url": "3.TurntoClose.htm" },
        { "No": 4, "Name": "按设置的方向条件转向", "GroupNo": "A", "Url": "4.TurnToByCond.htm" },
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
 
function GetUrl(optionKey) {
    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }

    return "0.CCFlowMsg.htm";
}