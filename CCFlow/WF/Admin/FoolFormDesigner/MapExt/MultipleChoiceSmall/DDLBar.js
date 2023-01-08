
function InitBar(optionKey) {

    var html = "小范围多选:";
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

    html += "<input  id='Btn_Save' type=button onclick='Save()' class='cc-btn-tab' value='保存' />";
    //html += "<input id='Btn_Advanced' type=button onclick='AdvSetting()'class='cc-btn-tab' value='更多' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "基本设置" },
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": 0, "Name": "无,不设置", "GroupNo": "A", "Url": "0.None.htm" },
        { "No": 1, "Name": "按文本输入的值", "GroupNo": "A", "Url": "1.ByInputText.htm" },
        { "No": 2, "Name": "按照枚举值", "GroupNo": "A", "Url": "2.ByEnum.htm" },
        { "No": 3, "Name": "按照系统外键表计算", "GroupNo": "A", "Url": "3.BySysKey.htm" },
        { "No": 4, "Name": "按照SQL计算", "GroupNo": "A", "Url": "4.BySQL.htm" },
        { "No": 5, "Name": "按照指定的岗位计算", "GroupNo": "A", "Url": "5.ByStation.htm" }
    ];
    return json;
}

function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}
function changeOption() {
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    SetHref( url + "?FK_MapData=" + GetQueryString("FK_MapData") + "&KeyOfEn=" +GetQueryString("KeyOfEn"));
}
//高级设置.
function AdvSetting() {
    var url = "Adv.htm?FK_MapData=" + GetQueryString("FK_MapData") + "&KeyOfEn=" + GetQueryString("KeyOfEn");
    OpenEasyUiDialogExt(url, "高级设置", 400, 180, false);
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