$(function () {

    loadScript(basePath + "/WF/Admin/Admin.js");
   
});
var nodeID = GetQueryString("FK_Node");
if (nodeID == undefined || nodeID == 0)
    nodeID = GetQueryString("NodeID");
var frmID = GetQueryString("FrmID");
if (frmID == null || frmID == undefined || frmID == "")
    frmID = GetQueryString("FK_MapData");
function InitBar(optionKey) {

    var html = "模板文件格式:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var groups = GetDBGroup();

    for (var i = 0; i < groups.length; i++) {

        var group = groups[i];
        html += "<option value="+group.No+">" + group.Name + "</option>";
    }
    html += "</select >";
    html += "<button  id='Btn_Save' type='button'class='cc-btn-tab btn-save' onclick='Btn_Save_Click()' value='保存' >保存</button>";
    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "0", "Name": "RTF模板","Url":"0.Rtf.htm"},
        { "No": "1", "Name": "VSTOForWord模板", "Url": "1.VSTOForWord.htm" },
        { "No": "2", "Name": "VSTOForExcel模板", "Url": "2.VSTOForExcel.htm" },
        { "No": "3", "Name": "WPS模板", "Url": "3.Wps.htm" },
    ];
    return json;
}
function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}
function changeOption() {
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    SetHref( url + "?FK_Node=" + nodeID+"&NodeID="+nodeID+"&FrmID="+frmID+"&FK_MapData="+frmID);
}

function GetUrl(optionKey) {
    var json = GetDBGroup();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No == optionKey)
            return en.Url;
    }

    return "0.Rtf.htm";
}

//处理显示模板列表的数据
function SetTableList() {

    var ens = new Entities("BP.WF.Template.Frm.FrmPrintTemplates");
    ens.Retrieve("FrmID", frmID);

    var fk_FrmPrintTemplate = GetQueryString("FK_FrmPrintTemplate");
    var currBill = null;
    //增加处理.
    for (var i = 0; i < ens.length; i++) {

        var en = ens[i];

        if (fk_FrmPrintTemplate == en.No) {
            currBill = en;
        }

        var newRow = "";
        newRow = "<tr ><td class=Idx>" + i + "</td>";
        newRow += "<td>" + en.MyPK + "</td>";
        newRow += "<td>" + en.Name + "</td>";
        newRow += "<td>" + en.TemplateFileModelText + "</td>";
        newRow += "<td>" + en.QRModelText + "</td>";

        newRow += "<td><a href='javaScript:void(0)' onclick='window.open(\"../../../../DataUser/CyclostyleFile/" + en.TempFilePath + "\")'>" + en.TempFilePath + "</td>";

        newRow += "<td>";
        newRow += "<a href=\"javascript:Delete('" + en.MyPK + "')\"><img src='" + basePath + "/WF/Img/Btn/Delete.gif' border=0 />删除</a>";
        newRow += "</td>";

        newRow += "</tr>";

        $("#Table1 tr:last").after(newRow);
    }

    return currBill;

}
/**
 * 删除模板
 * @param {any} mypk 模板的主键
 */
function Delete(mypk) {

    if (window.confirm('您确定要删除[' + mypk + ']吗？') == false)
        return;

    var en = new Entity("BP.WF.Template.Frm.FrmPrintTemplate");
    en.SetPKVal(mypk);
    en.Delete();

    Reload();
}
/**
 * 上传文件名自动显示到模板名称中
 **/
function show() {
    var path = document.getElementById("bill").value;
    var pos1 = path.lastIndexOf("\\");
    var pos2 = path.lastIndexOf(".");
    var TB_Name = path.substring(pos1 + 1, pos2);
    document.getElementById("TB_Name").value = TB_Name;
    //模板文件格式显示对应格式
    var hz = path.substr(pos2 + 1, 4).toLowerCase();
    if (hz == "rtf") {
        $("#DDL_TemplateFileModel").val(0);
    } else if (hz == "doc" || hz == "docx") {
        $("#DDL_TemplateFileModel").val(1);
    } else if (hz == "xls" || hz == "xlsx") {
        $("#DDL_TemplateFileModel").val(2);
    } else {
        alert("格式不正确！");
    }
}
