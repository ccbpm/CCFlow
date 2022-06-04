
function InitBar(optionKey) {

    var html = "<b>文本框自动完成</b>:";
    html += "<select id='changBar' onchange='changeOption()' >";
    html += " <option value='None' >不设置(默认).</option>";
    html += " <option value='Simple' >简洁模式</option>";
    html += " <option value='Table' >表格模式</option>";
    html += "</select>";

    html += "<input  id='Btn_Save' class='cc-btn-tab' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_FullData' class='cc-btn-tab' type=button onclick='FullData()' value='填充' />";
    html += "<input  id='Btn_Delete' class='cc-btn-tab'  type=button onclick='Delete()' value='删除' />";

    //html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function FullData() {

    var myPK = "TBFullCtrl_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn");
    var url = "../FullData/Main.htm?FK_MapData=" + this.GetQueryString("FK_MapData") + "&RefPK=" + myPK + "&KeyOfEn=" + GetQueryString("KeyOfEn");

    SetHref(url);
}

//生成主键.
function GenerMapExtPK() {
    return "TBFullCtrl_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn") + "_FullData";
}

function Delete() {

    if (window.confirm("您确定要删除吗?") == false)
        return;

    //更新节点表单类型.
    var frmID = GetQueryString("FK_MapData");
    var keyOfEn = GetQueryString("KeyOfEn");

    var en = new Entity("BP.Sys.MapAttr", frmID + "_" + keyOfEn);
    en.SetPara("TBFullCtrl", "None");
    en.Update();

    var myPK = "TBFullCtrl_" + frmID + "_" + keyOfEn;
    var en = new Entity("BP.Sys.MapExt");
    en.MyPK = GenerMapExtPK();
    var i = en.Delete();

    SetHref("0.None.htm?FK_MapData=" + GetQueryString("FK_MapData") + "&KeyOfEn=" + GetQueryString("KeyOfEn"));
}

function HelpOnline() {
    var url = "http://ccform.mydoc.io";
    window.open(url);
}

function changeOption() {

    var fk_MapData = GetQueryString("FK_MapData");
    var KeyOfEn = GetQueryString("KeyOfEn");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    SetHref(url + "?FK_MapData=" + fk_MapData + "&KeyOfEn=" + KeyOfEn);
}

function GetUrl(popModel) {


    switch (popModel) {
        case "None":
            url = "0.None.htm";
            break;
        case "Simple":
            url = "1.Simple.htm";
            break;
        case "Table":
            url = "2.Table.htm";
            break;
        default:
            url = "0.None.htm";
            break;
    }
    return url;
}

function SaveAndClose() {

    Save();
    window.close();
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            Reload();
        }
    });
}