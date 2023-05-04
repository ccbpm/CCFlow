
function InitBar(optionKey) {

    var html = "<b>发起前置导航</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";


    //流程数据获取模式.

    html += "<option value=null  disabled='disabled'>+数据源配置模式</option>";
    html += "<option value=" + StartGuideWay.BySQLOne + ">&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQL-单条模式  </option>";
    html += "<option value=" + StartGuideWay.BySQLMulti + ">&nbsp;&nbsp;&nbsp;&nbsp;按设置的SQL-多条模式(用于批量发起)</option>";


    //流程数据获取模式.
    html += "<option value=null  disabled='disabled'>+流程数据获取模式</option>";
    html += "<option value=" + StartGuideWay.SubFlowGuide + ">&nbsp;&nbsp;&nbsp;&nbsp;从历史发起的流程Copy数据(查询历史记录)</option>";
    html += "<option value=" + StartGuideWay.ByParentFlowModel + ">&nbsp;&nbsp;&nbsp;&nbsp;父子流程模式</option>";
    html += "<option value=" + StartGuideWay.ByChildFlowModel + ">&nbsp;&nbsp;&nbsp;&nbsp;子流程实例列表模式-多条 </option>";

    html += "<option value=null  disabled='disabled'>+其他模式</option>";
    html += "<option value=" + StartGuideWay.ByFrms + ">&nbsp;&nbsp;&nbsp;&nbsp;开始节点绑定的独立表单列表 </option>";
    html += "<option value=" + StartGuideWay.BySelfUrl + ">&nbsp;&nbsp;&nbsp;&nbsp;按自定义的Url </option>";
    html += "<option value=" + StartGuideWay.None + ">&nbsp;&nbsp;&nbsp;&nbsp;无,不设置(默认).</option>";

    html += "</select >";

    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save' onclick='Save()' value='保存' />保存</button>";
    //    html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";
    //    html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    if (optionKey != StartGuideWay.None)
    html += "<button  id='Btn_Help' class='cc-btn-tab btn-setting' onclick='Adv()' value='高级设置' />高级设置</button>";
    html += "<button  id='Btn_Help' class='cc-btn-tab btn-save' onclick='HelpOnline()' value='在线帮助' />在线帮助</button>";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function Adv() {
    var url = "Adv.htm?FK_Flow=" + GetQueryString("FK_Flow");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
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

    SetHref( url + "?FK_Flow=" + flowNo);
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
        case StartGuideWay.BySQLMulti:
            url = "6.BySQLMulti.htm";
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
        case StartGuideWay.ByChildFlowModel:
            url = "10.ByChildFlowModel.htm";
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
            Reload();
        }
    });
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