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


function InitBar(optionKey) {

    var html = "<b>选择Pop模式</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+树形模式</option>";
    html += "<option value='PopBranchesAndLeaf' >&nbsp;&nbsp;&nbsp;&nbsp;树干叶子模式</option>";
    //html += "<option value='PopBranchesAndLeafLazyLoad' >树干叶子模式-懒加载</option>";
    html += "<option value='PopBranches' >&nbsp;&nbsp;&nbsp;&nbsp;树干模式(简单)</option>";
    //html += "<option value='PopBranchesLazyLoad' >树干模式(简单)-懒加载</option>";

    html += "<option value=null  disabled='disabled'>+分组模式</option>";
    html += "<option value='PopGroupList' >&nbsp;&nbsp;&nbsp;&nbsp;分组列表平铺</option>";
    html += "<option value='PopTableList' >&nbsp;&nbsp;&nbsp;&nbsp;单实体平铺</option>";
    html += "<option value='PopBindSFTable' >&nbsp;&nbsp;&nbsp;&nbsp;绑定外键(字典表)表</option>";
    html += "<option value='PopBindEnum' >&nbsp;&nbsp;&nbsp;&nbsp;绑定枚举</option>";

    html += "<option value=null  disabled='disabled'>+其他模式</option>";
    html += "<option value='PopTableSearch' >&nbsp;&nbsp;&nbsp;&nbsp;表格条件查询</option>";
    html += "<option value='PopSelfUrl' >&nbsp;&nbsp;&nbsp;&nbsp;自定义URL</option>";
    html += "<option value='None' >&nbsp;&nbsp;&nbsp;&nbsp;无,不设置(默认).</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_Gener' type=button class='cc-btn-tab' onclick=Adv('" + optionKey + "') value='通用' />";
    html += "<input type='button' value='删除' id='Btn_Delete' name='Btn_Delete' onclick='return Delete()' />"
    html += "<input id='Btn_FullData' type=button onclick='FullData()' value='填充' />";

    //html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");

}

//高级设置.
function Adv(PopModel) {
    //这里弹窗设置最好
    var keyOfEn = GetQueryString("KeyOfEn");
    var optionKey = $("#changBar").val();
    var myPK = optionKey + "_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn");

    var url = "Adv.htm?FK_MapData=" + this.GetQueryString("FK_MapData") + "&RefPK=" + myPK + "&KeyOfEn=" + keyOfEn + "&PopModel=" + PopModel;
    var w = window.innerWidth*4/5;
    var h = window.innerHeight*4/5;
    OpenEasyUiDialogExt(url, "通用设置", w, h, false);
    //window.location.href = filterXSS(url);

    //WinOpen(url);



}

function FullData() {

    var keyOfEn = GetQueryString("KeyOfEn");
    var optionKey = $("#changBar").val();
    var myPK = optionKey + "_" + GetQueryString("FK_MapData") + "_" + GetQueryString("KeyOfEn");
    var url = "../FullData/Default.htm?FK_MapData=" + this.GetQueryString("FK_MapData") + "&RefPK=" + myPK + "&KeyOfEn=" + keyOfEn;
    SetHref(url);

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
        case "PopBranchesAndLeaf":
            url = "1.BranchesAndLeaf.htm";
            break;
        case "PopBranchesAndLeafLazyLoad":
            url = "2.BranchesAndLeafLazyLoad.htm";
            break;
        case "PopBranches":
            url = "3.Branches.htm";
            break;
        case "PopBranchesLazyLoad":
            url = "4.BranchesLazyLoad.htm";
            break;
        case "PopGroupList":
            url = "5.GroupList.htm";
            break;
        case "PopTableList":
            url = "6.TableList.htm";
            break;
        case "PopTableSearch":
            url = "7.TableSearch.htm";
            break;
        case "PopSelfUrl":
            url = "8.SelfUrl.htm";
            break;
        case "PopBindEnum":
            url = "9.BindEnum.htm";
            break;
        case "PopBindSFTable":
            url = "10.BindSFTable.htm";
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