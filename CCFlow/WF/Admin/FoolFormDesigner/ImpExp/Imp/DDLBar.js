
function InitBar(optionKey) {

    var html = "导入导出:";
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

        { "No": "A", "Name": "从文件中导入" },
        { "No": "B", "Name": "从本机其他表单" },
        { "No": "C", "Name": "从数据库表导入" },
        { "No": "D", "Name": "从共享模板库导入" },
        { "No": "E", "Name": "导出xml表单模板" },

    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": "LocalhostXml", "Name": "本地xml表单模版导入", "GroupNo": "A", "Url": "LocalhostXml.htm" },
        { "No": "LocalhostWord", "Name": "本地Word表单文件", "GroupNo": "A", "Url": "LocalhostWord.htm" },
        { "No": "LocalhostExcel", "Name": "本地Excel表单文件", "GroupNo": "A", "Url": "LocalhostExcel.htm" },
        { "No": "LocalhostHtml", "Name": "本地Html表单文件", "GroupNo": "A", "Url": "LocalhostHtml.htm" },

        { "No": "NodeFrmImp", "Name": "从流程节点表单导入", "GroupNo": "B", "Url": "NodeFrmImp.htm" },
        { "No": "FlowFrmImp", "Name": "从其他流程导入", "GroupNo": "B", "Url": "FlowFrmImp.htm" },
        { "No": "FrmLibraryImp", "Name": "从表单库导入", "GroupNo": "B", "Url": "FrmLibraryImp.htm" },
        { "No": "FrmEnsName", "Name": "从类的实体类导入", "GroupNo": "B", "Url": "FrmEnsName.htm" },


        { "No": "ExternalDataSourseImp", "Name": "数据源表导入", "GroupNo": "C", "Url": "ExternalDataSourseImp.htm" },
        { "No": "WebAPIImp", "Name": "WebAPI接口导入", "GroupNo": "C", "Url": "WebAPIImp.htm" },

        { "No": "ClouldXml", "Name": "共享的xml文件", "GroupNo": "D", "Url": "ClouldXml.htm" },
        { "No": "ClouldHtml", "Name": "共享的html文件", "GroupNo": "D", "Url": "ClouldHtml.htm" },

        { "No": "ExpXmlFrm", "Name": "导出Xml", "GroupNo": "E", "Url": "ExpXmlFrm.htm" }

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
    SetHref(url + "?FK_Node=" + nodeID + "&FK_Flow=" + GetQueryString("FK_Flow") + "&FrmID=" + GetQueryString("FrmID"));
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

    loadScript(basePath + "/WF/Admin/Admin.js");
       
});