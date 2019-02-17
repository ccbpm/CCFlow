
function InitBar(optionKey) {

    var html = "<b>发起前置导航</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=" + StartGuideWay.None + ">无,不设置(默认).</option>";
    html += "<option value=" + StartGuideWay.BySQLOne + ">按设置的SQL-单条模式  </option>";
    html += "<option value=" + StartGuideWay.FoolForm + ">从历史发起的流程Copy数据(查询历史记录)</option>";
    html += "<option value=" + StartGuideWay.BySelfUrl + ">按自定义的Url </option>";
    html += "<option value=" + StartGuideWay.ByParentFlowModel + ">父子流程模式</option>";

    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'>  按设置的SQL-多条模式(用于批量发起)</option>";
    html += "<option value=" + StartGuideWay.FoolForm + " disabled='disabled'>   子流程实例列表模式-多条 </option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";
    //    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}


function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}

function changeOption() {

    var flowNo = GetQueryString("FK_Flow");
    if (flowNo == null)
        flowNo = '001';

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);

    window.location.href = url + "?FK_Flow=" + flowNo;
}

function GetUrl(optionKey) {

    switch (parseInt(optionKey)) {
        case StartGuideWay.None:
            url = "0.None.htm";
            break;
        case StartGuideWay.BySQLOne:
            url = "1.BySQLOne.htm";
            break;
        case StartGuideWay.SubFlowGuide:
            url = "2.SubFlowGuide.htm";
            break;
        case StartGuideWay.BySelfUrl:
            url = "7.BySelfUrl.htm";
            break;
        case StartGuideWay.ByFrms:
            url = "8.ByFrms.htm";
            break;
        case StartGuideWay.ByParentFlowModel:
            url = "9.ByParentFlowModel.htm";
            break;
        default:
            url = "0.None.htm";
            break;
    }

    return url;
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