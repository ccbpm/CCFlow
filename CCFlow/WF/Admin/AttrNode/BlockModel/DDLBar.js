
function InitBar(optionKey) {

    var html = "发送阻塞规则:";
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

    html += "<button  id='Btn_Save' type=button class='cc-btn-tab btn-save'  onclick='Save()' >保存</button>";

    html += "<button id='Btn_Advanced' type=button class='cc-btn-tab btn-advanced' onclick='AdvSetting()'> 高级设置</button>";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "通用规则" },
        { "No": "B", "Name": "父子流程规则" }
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "不阻塞", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "当前节点的有未完成的子线程", "GroupNo": "B", "Url": "1.CurrNodeAll.htm" },
        { "No": 2, "Name": "按约定格式阻塞未完成子流程", "GroupNo": "B", "Url": "2.SpecSubFlow.htm" },
        { "No": 3, "Name": "按照SQL阻塞", "GroupNo": "A", "Url": "3.BySQL.htm" },
        { "No": 4, "Name": "按照表达式阻塞", "GroupNo": "A", "Url": "4.ByExp.htm" },
        { "No": 5, "Name": "是否启用为父流程时，子流程未运行到指定的节点", "GroupNo": "B", "Url": "5.SpecSubFlowNode.htm" },
        { "No": 6, "Name": "是否启用为平级子流程时，子流程未运行到指定的节点", "GroupNo": "B", "Url": "6.SameLevelSubFlow.htm" },
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
    
    return "0.None.htm";
}