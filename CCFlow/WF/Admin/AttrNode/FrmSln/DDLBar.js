
function InitBar(optionKey) {

    var html = "请选择表单方案:";
    html += "<select id='changBar' onchange='changeOption()'>";
    html += "<option value=" + FormType.FoolForm + ">&nbsp;&nbsp;内置傻瓜表单</option>";
    html += "<option value=" + FormType.FreeForm + ">&nbsp;&nbsp;内置自由表单</option>";
    html += "<option value=" + FormType.SelfForm + " >&nbsp;&nbsp;嵌入式表单</option>";
    html += "<option value=" + FormType.RefOneFrmTree + " >&nbsp;&nbsp;绑定一个表单库的表单</option>";
    html += "<option value=" + FormType.SheetTree + " >&nbsp;&nbsp;绑定多表单</option>";
    html += "<option value=" + FormType.FoolTruck + " >&nbsp;&nbsp;软通动力（傻瓜轨迹表单）</option>";
   
    html += "<option value=" + FormType.SDKForm + " >&nbsp;&nbsp;使用SDK表单</option>";
    html += "<option value=" + FormType.WebOffice + " >&nbsp;&nbsp;绑定公文表单</option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<hr/>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function changeOption() {
    var nodeID = GetQueryString("FK_Node");
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
    window.location.href = url + "?FK_Node=" + nodeID;
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