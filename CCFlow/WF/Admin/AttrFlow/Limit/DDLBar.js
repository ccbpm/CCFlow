
function InitBar(optionKey) {

    var html = "<b>流程计划时间计算</b>:";

    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value='0'>&nbsp;&nbsp;&nbsp;&nbsp;不限制  </option>";
    html += "<option value='1'>&nbsp;&nbsp;&nbsp;&nbsp;按时间规则计算  </option>";
    html += "<option value='6'>&nbsp;&nbsp;&nbsp;&nbsp;按照发起字段不能重复规则  </option>";
    html += "<option value='7'>&nbsp;&nbsp;&nbsp;&nbsp;按SQL规则  </option>";
    html += "<option value='9'>&nbsp;&nbsp;&nbsp;&nbsp;为子流程时仅仅只能被调用1次.  </option>";

    html += "</select >";

    //html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
   // html += "<input  id='Btn_Help' type=button onclick='HelpOnline()' value='在线帮助' />";
    html += "<button  id='Btn_Save' class='cc-btn-tab btn-save'  onclick='Save()' value='保存' >保存</button>";
    //html += "<input  id='Btn_Help' type=button onclick='Adv()' value='高级设置' />";
    html += "<button  id='Btn_Help'class='cc-btn-tab btn-hlep'  onclick='HelpOnline()' value='在线帮助' >在线帮助</button>";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

//function GetDBGroup() {

//    var json = [

//        { "No": "A", "Name": "流程计划时间计算" },
//    ];
//    return json;
//}

//function GetDBDtl() {

//    var json = [

//        { "No": 0, "Name": "不限制（默认）", "GroupNo": "A", "Url": "0.None.htm" },

//        { "No": 1, "Name": " 按时间规则计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },
//        { "No": 2, "Name": " 按时间规则计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },
//        { "No": 3, "Name": " 按时间规则计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },
//        { "No": 4, "Name": " 按时间规则计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },
//        { "No": 5, "Name": " 按时间规则计算", "GroupNo": "A", "Url": "1.NodeFrmDT.htm" },

//        { "No": 6, "Name": "按照发起字段不能重复规则", "GroupNo": "A", "Url": "6.ColNotExit.htm" },

//        { "No": 7, "Name": "按SQL规则", "GroupNo": "A", "Url": "7.SQLDT.htm" },
//        { "No": 8, "Name": "按SQL规则", "GroupNo": "A", "Url": "7.SQLDT.htm" },

//        { "No": 9, "Name": "为子流程时的规则", "GroupNo": "A", "Url": "9.OnlyOneSubFlow.htm" },
//    ];
//    return json;
//}

function Adv()
{
    var url = "Adv.htm?FK_Flow=" + GetQueryString("FK_Flow");
    OpenEasyUiDialogExt(url, '高级设置', 600, 400, false);
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
        case StartLimitRole.None:
            url = "0.None.htm";
            break;
        case StartLimitRole.Day:
        case StartLimitRole.Week:
        case StartLimitRole.Month:
        case StartLimitRole.JD:
        case StartLimitRole.Year:
            url = "1.TimeDT.htm";
            break;
        case StartLimitRole.ColNotExit:
            url = "6.ColNotExit.htm";
            break;
        case StartLimitRole.ResultIsZero:
        case StartLimitRole.ResultIsNotZero:
            url = "7.BySQL.htm";
            break;
        case StartLimitRole.OnlyOneSubFlow:
            url = "9.OnlyOneSubFlow.htm";
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
