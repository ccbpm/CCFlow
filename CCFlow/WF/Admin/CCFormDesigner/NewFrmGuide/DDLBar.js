
function InitBar(optionKey) {

    /*
    FrmType_CH_0	傻瓜表单	FrmType	0	CH
    FrmType_CH_1	自由表单	FrmType	1	CH
    FrmType_CH_11	累加表单	FrmType	11	CH
    FrmType_CH_3	嵌入式表单	FrmType	3	CH
    FrmType_CH_4	Word表单	FrmType	4	CH
    FrmType_CH_5	在线编辑模式Excel表单	FrmType	5	CH
    FrmType_CH_6	VSTO模式Excel表单	FrmType	6	CH
    FrmType_CH_7	实体类组件	FrmType	7	CH
        */

    var html = "选择要创建表单类型:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var frmWorkModel = GetQueryString("FormWorkMode");

    if (frmWorkModel == 1) {
        html += "<option value=null  disabled='disabled'>+单据模式</option>";
        html += "<option value=" + FormType.FoolForm + ">&nbsp;&nbsp;傻瓜模式(默认)</option>";
        html += "<option value=" + FormType.FreeForm + ">&nbsp;&nbsp;自由表单</option>";
        //html += "<option value=" + FormType.FoolTruck + " >&nbsp;&nbsp;内置累加模式表单</option>";
        //html += "<option value=" + FormType.WebOffice + "  >&nbsp;&nbsp;公文表单(weboffice)</option>";

        html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
        html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";

        html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
        html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";


        document.getElementById("bar").innerHTML = html;
        $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
        return;
    }

    html += "<option value=null  disabled='disabled'>+内置表单</option>";
    html += "<option value=" + FormType.FoolForm + ">&nbsp;&nbsp;内置傻瓜表单(默认)</option>";
    html += "<option value=" + FormType.FreeForm + ">&nbsp;&nbsp;内置自由表单</option>";
  //  html += "<option value=" + FormType.FoolTruck + " >&nbsp;&nbsp;内置累加模式表单</option>";
   // html += "<option value=" + FormType.WebOffice + "  >&nbsp;&nbsp;公文表单(weboffice)</option>";

    html += "<option value=null  disabled='disabled'>+自定义表单</option>";
    html += "<option value=" + FormType.SelfForm + " >&nbsp;&nbsp;嵌入式表单</option>";
  //  html += "<option value=" + FormType.SDKForm + " >&nbsp;&nbsp;SDK表单(我自定义的表单)</option>";

    html += "<option value=null  disabled='disabled'>+Office表单</option>";
    html += "<option value=" + FormType.RefOneFrmTree + " >&nbsp;&nbsp;VSTO模式Excel表单</option>";
    html += "<option value=" + FormType.SheetTree + " >&nbsp;&nbsp;Word表单</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='创建' />";
    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='创建并关闭' />";

    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

function changeOption() {
    var frmSort = GetQueryString("FK_FrmSort");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case FormType.FoolForm:
            url = "0.FoolForm.htm";
            break;
        case FormType.FreeForm:
            url = "1.FreeForm.htm";
            break;
        case FormType.SelfForm:
            url = "2.SelfForm.htm";
            break;
        case FormType.SDKForm:
            url = "3.SDKForm.htm";
            break;
        case FormType.SLForm:
            url = "4.SLForm.htm";
            break;
        case FormType.SheetTree:
            url = "5.SheetTree.htm";
            break;
        case FormType.SheetAutoTree:
            url = "6.SheetAutoTree.htm";
            break;
        case FormType.WebOffice:
            url = "7.WebOffice.htm";
            break;
        case FormType.ExcelForm:
            url = "8.ExcelForm.htm";
            break;
        case FormType.WordForm:
            url = "9.WordForm.htm";
            break;
        case FormType.FoolTruck:
            url = "10.FoolTruck.htm";
            break;
        case FormType.RefOneFrmTree:
            url = "11.RefOneFrmTree.htm";
            break;
        case FormType.DisableIt:
            url = "100.DisableIt.htm";
            break;
        default:
            url = "0.FoolForm.htm";
            break;
    }

    window.location.href = url + "?FK_FrmSort=" + frmSort + "&FormWorkMode=" + GetQueryString("FormWorkMode");
}

function SaveAndClose() {

    Save();
    window.close();
}

//打开窗体.
function OpenEasyUiDialogExt(url, title, w, h, isReload) {

    OpenEasyUiDialog(url, "eudlgframe", title, w, h, "icon-property", true, null, null, null, function () {
        if (isReload == true) {
            window.location.href = window.location.href;
        }
    });

}