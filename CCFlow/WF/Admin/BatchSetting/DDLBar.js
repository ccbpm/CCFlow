
function InitBar(optionKey) {

    var html = "选择项目:";
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
    //    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save' type=button onclick='Save()' value='保存' >保存</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "节点" },
        { "No": "B", "Name": "表单" }

    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": "Accepter", "Name": "接收人规则", "GroupNo": "A", "Url": "0.Accepter.htm" },
        //  { "No": "Btns", "Name": "按钮属性", "GroupNo": "A", "Url": "1.Btns.htm" },


        { "No": "WorkCheck", "Name": "审核组件", "GroupNo": "B", "Url": "3.WorkCheck.htm" },
        //{ "No": "WorkC2heck", "Name": "批量设置字段", "GroupNo": "B", "Url": "1.WorkCheck.htm" },

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
    SetHref(url + "?FK_Node=" + nodeID + "&FlowNo=" + GetQueryString("FlowNo"));
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