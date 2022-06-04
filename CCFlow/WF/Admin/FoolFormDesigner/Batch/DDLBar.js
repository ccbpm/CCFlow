
function InitBar(optionKey) {


    if (optionKey == null) {
        // http://111.111.11:/xxx.htm?x=111;
        var url = GetHrefUrl();
        optionKey = "111.htm"; //怎么求？
    }


    var html = "批量字段编辑:";
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
    //  html += "<button  id='Btn_Save' type='button'class='cc-btn-tab btn-save' onclick='Save()' value='保存' >保存</button>";
    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "批量修改" }

    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": "KeyOfEn", "Name": "修改字段名", "GroupNo": "A", "Url": "KeyOfEn.htm" },
        { "No": "Length", "Name": "修改String类型字段长度", "GroupNo": "A", "Url": "Length.htm" },
        { "No": "DataType", "Name": "修改数据类型", "GroupNo": "A", "Url": "DataType.htm" },
        { "No": "Tip", "Name": "输入提示", "GroupNo": "A", "Url": "Tip.htm" },

        { "No": "Regular", "Name": "设置验证", "GroupNo": "A", "Url": "Regular.htm" }
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
    SetHref(url + "?FrmID=" + GetQueryString("FrmID"));
}

function GetUrl(optionKey) {

    var nodeID = GetQueryString("FK_Node");
    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;// + "?FK_Node=" + nodeID + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FrmID=" + GetQueryString("FrmID");
    }
    return "LocalhostXml.htm";
    // return "?FK_Node=" + nodeID + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FrmID=" + GetQueryString("FrmID");
}


$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
            // alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
});