
function InitBar(optionKey) {

    var html = "多人处理规则:";
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

    html += "<button  id='Btn_Save' type=button class='cc-btn-tab btn-save' onclick='Save()'>保存</button>";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "多人处理" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "抢办模式", "GroupNo": "A", "Url": "0.QiangBan.htm" },
        { "No": 1, "Name": "协作模式", "GroupNo": "A", "Url": "1.Teamup.htm" },
        { "No": 2, "Name": "队列模式", "GroupNo": "A", "Url": "2.Order.htm" },
        { "No": 3, "Name": "共享模式", "GroupNo": "A", "Url": "3.Sharing.htm" },
        { "No": 4, "Name": "协作组长模式", "GroupNo": "A", "Url": "4.TeamupGroupLeader.htm" },
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
    
    return "0.QiangBan.htm";
}