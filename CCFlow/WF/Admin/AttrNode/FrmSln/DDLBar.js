﻿
function InitBar(optionKey) {

    var html = "表单方案:";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+内置表单</option>";
    html += "<option value=" + StartGuideWay.FoolForm + ">&nbsp;&nbsp;内置傻瓜表单(默认)</option>";
    html += "<option value=" + StartGuideWay.FreeForm + ">&nbsp;&nbsp;内置自由表单</option>";
    html += "<option value=" + StartGuideWay.FoolTruck + " >&nbsp;&nbsp;内置累加模式表单</option>";
    html += "<option value=" + StartGuideWay.WebOffice + "  >&nbsp;&nbsp;公文表单(weboffice)</option>";

    html += "<option value=null  disabled='disabled'>+自定义表单</option>";
    html += "<option value=" + StartGuideWay.SelfForm + " >&nbsp;&nbsp;嵌入式表单</option>";
    html += "<option value=" + StartGuideWay.SDKForm + " >&nbsp;&nbsp;SDK表单(我自定义的表单)</option>";

    html += "<option value=null  disabled='disabled'>+绑定表单库里的表单</option>";
    html += "<option value=" + StartGuideWay.RefOneFrmTree + " >&nbsp;&nbsp;绑定表单库的表单</option>";
    html += "<option value=" + StartGuideWay.SheetTree + " >&nbsp;&nbsp;绑定多表单(表单树)</option>";

    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";

    //  html += "<input type=button onclick='OldVer()' value='使用旧版本' />";

    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function OldVer() {

    var nodeID = GetQueryString("FK_Node");
    var flowNo = GetQueryString("FK_Flow");
    var url = '../NodeFromWorkModel.htm?FK_Flow=' + flowNo + '&FK_Node=' + nodeID;
    window.location.href = url;
}


//打开傻瓜表单设计器.
function DFoolFrm() {

    var nodeID = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", nodeID);
    var url = '../../FoolFormDesigner/Designer.htm?FK_Flow=' + node.FK_Flow + '&FK_Node=' + nodeID + "&FK_MapData=ND" + nodeID;
    window.open(url);

    //OpenEasyUiDialogExt(url, '傻瓜表单设计器', 800, 500, false);
}

//打开自由表单设计器.
function DFreeFrm() {
    var nodeID = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", nodeID);
    var url = '../../CCFormDesigner/FormDesigner.htm?FK_Flow=' + node.FK_Flow + '&FK_Node=' + nodeID + "&FK_MapData=ND" + nodeID;
    OpenEasyUiDialogExt(url, '傻瓜表单设计器', 1100, 600, false);
}

function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}

function Help() {

    var url = window.location.href;

    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case StartGuideWay.FoolForm:
            url = "0.FoolForm.htm";
            break;
        case StartGuideWay.FreeForm:
            url = "1.FreeForm.htm";
            break;
        case StartGuideWay.SelfForm:
            url = "2.SelfForm.htm";
            break;
        case StartGuideWay.SDKForm:
            url = "3.SDKForm.htm";
            break;
        case StartGuideWay.SLForm:
            url = "4.SLForm.htm";
            break;
        case StartGuideWay.SheetTree:
            url = "5.SheetTree.htm";
            break;
        case StartGuideWay.SheetAutoTree:
            url = "6.SheetAutoTree.htm";
            break;
        case StartGuideWay.WebOffice:
            url = "7.WebOffice.htm";
            break;
        case StartGuideWay.ExcelForm:
            url = "8.ExcelForm.htm";
            break;
        case StartGuideWay.WordForm:
            url = "9.WordForm.htm";
            break;
        case StartGuideWay.FoolTruck:
            url = "10.FoolTruck.htm";
            break;
        case StartGuideWay.RefOneFrmTree:
            alert('该视频尚未提供');
            return;
            //  url = "11.RefOneFrmTree.htm";
            break;
        case StartGuideWay.DisableIt:
            url = "100.DisableIt.htm";
            break;
        default:
            url = "0.FoolForm.htm";
            break;
    }

    //if (url.indexOf
    //var url = "http://ccbpm.mydoc.io";
    //window.open(url);
}


function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case StartGuideWay.FoolForm:
            url = "0.FoolForm.htm";
            break;
        case StartGuideWay.FreeForm:
            url = "1.FreeForm.htm";
            break;
        case StartGuideWay.SelfForm:
            url = "2.SelfForm.htm";
            break;
        case StartGuideWay.SDKForm:
            url = "3.SDKForm.htm";
            break;
        case StartGuideWay.SLForm:
            url = "4.SLForm.htm";
            break;
        case StartGuideWay.SheetTree:
            url = "5.SheetTree.htm";
            break;
        case StartGuideWay.SheetAutoTree:
            url = "6.SheetAutoTree.htm";
            break;
        case StartGuideWay.WebOffice:
            url = "7.WebOffice.htm";
            break;
        case StartGuideWay.ExcelForm:
            url = "8.ExcelForm.htm";
            break;
        case StartGuideWay.WordForm:
            url = "9.WordForm.htm";
            break;
        case StartGuideWay.FoolTruck:
            url = "10.FoolTruck.htm";
            break;
        case StartGuideWay.RefOneFrmTree:
            url = "11.RefOneFrmTree.htm";
            break;
        case StartGuideWay.DisableIt:
            url = "100.DisableIt.htm";
            break;
        default:
            url = "0.FoolForm.htm";
            break;
    }
    window.location.href = url + "?FK_Node=" + nodeID;
}


function CheckFlow(flowNo) {
    var flow = new Entity('BP.WF.Flow', flowNo);
    flow.DoMethodReturnString("DoCheck"); //重置密码:不带参数的方法. 
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