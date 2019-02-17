
function InitBar(optionKey) {

    var html = "表单方案:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=" + StartGuideWay.None + ">无，不设置(默认)。</option>";
    html += "<option value=" + StartGuideWay.BySQLOne + "> 按设置的SQL-单条模式  </option>";
    html += "<option value=" + StartGuideWay.FoolForm + ">从历史发起的流程Copy数据(查询历史记录)</option>";
    html += "<option value=" + StartGuideWay.BySelfUrl + "> 按自定义的Url </option>";
    html += "<option value=" + StartGuideWay.ByParentFlowModel + "> 父子流程模式  </option>";

    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'>  按设置的SQL-多条模式(用于批量发起)</option>";
    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'>   子流程实例列表模式-多条 </option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";
    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function OldVer() {

    var nodeID = GetQueryString("FK_Flow");
    var flowNo = GetQueryString("FK_Flow");
    var url = '../NodeFromWorkModel.htm?FK_Flow=' + flowNo + '&FK_Flow=' + nodeID;
    window.location.href = url;
}


//打开傻瓜表单设计器.
function DFoolFrm() {

    var nodeID = GetQueryString("FK_Flow");
    var node = new Entity("BP.WF.Flow", nodeID);
    var url = '../../FoolFormDesigner/Designer.htm?FK_Flow=' + node.FK_Flow + '&FK_Flow=' + nodeID + "&FK_MapData=ND" + nodeID;
    window.open(url);
}

//打开自由表单设计器.
function DFreeFrm() {
    var nodeID = GetQueryString("FK_Flow");
    var node = new Entity("BP.WF.Flow", nodeID);
    var url = '../../CCFormDesigner/FormDesigner.htm?FK_Flow=' + node.FK_Flow + '&FK_Flow=' + nodeID + "&FK_MapData=ND" + nodeID;
    OpenEasyUiDialogExt(url, '傻瓜表单设计器', 1100, 600, false);
}

function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}

function Help() {

    var url = window.location.href;

    var nodeID = GetQueryString("FK_Flow");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var roleName = "";
    switch (parseInt(optionKey)) {
        case StartGuideWay.None:
            url = "0.None.htm";
            break;
        case StartGuideWay.BySQLOne:
            url = "1.BySQLOne.htm";
            break;
        case StartGuideWay.BySelfUrl:
            url = "7.BySelfUrl.htm";
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
}

function changeOption() {

    var nodeID = GetQueryString("FK_Flow");
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
    window.location.href = url + "?FK_Flow=" + nodeID;
}


function CheckFlow(flowNo)
{
	  var flow=new Entity('BP.WF.Flow', flowNo);
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