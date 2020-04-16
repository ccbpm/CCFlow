$(function () {

    jQuery.getScript(basePath + "/WF/Admin/Admin.js")
        .done(function () {
            /* 耶，没有问题，这里可以干点什么 */
             //alert('ok');
        })
        .fail(function () {
            /* 靠，马上执行挽救操作 */
            //alert('err');
        });
    InitPage();
});
//初始化数据.
function InitPage() {

    var fk_node = GetQueryString("FK_Node");
    var node = new Entity("BP.WF.Node", fk_node);
    InitBar(node.TurnToDeal);
    //调用公共类库的方法:执行批量主表赋值
    GenerFullAllCtrlsVal(node);
   
    switch (parseInt(node.TurnToDeal)) {
        case 0:
            break;
        case 1:
            $("#TB_SpecMsg").val(node.TurnToDealDoc);
            break;
        case 2:
            $("#TB_SpecURL").val(node.TurnToDealDoc);
            break;
        case 3:
            break;
        default:
            break;
    }
    return;
}
function InitBar(optionKey) {

    var html = "发送后转向:";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+转向规则</option>";
    html += "<option value=" + TurntoWay.TurntoDefault + ">提示CCFlow默认信息</option>";
    html += "<option value=" + TurntoWay.TurntoMessage + ">提示指定信息 </option>";
    html += "<option value=" + TurntoWay.TurntoUrl + ">转向指定的URL</option>";
    html += "<option value=" + TurntoWay.TurntoClose + ">发送完成立即关闭 </option>";
    html += "</select >";

    html += "<input  id='Btn_Save' type=button onclick='Save()' value='保存' />";
    //   html += "<input  id='Btn_SaveAndClose' type=button onclick='SaveAndClose()' value='保存并关闭' />";

    //  html += "<input type=button onclick='OldVer()' value='使用旧版本' />";

    //  html += "<input  id='Btn_Help' type=button onclick='Help()' value='视频帮助' />";
    
    html += "<input id='Btn' type=button onclick='AdvSetting()' value='高级设置' />";


    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}



function HelpOnline() {
    var url = "http://ccbpm.mydoc.io";
    window.open(url);
}
function changeOption() {
    var nodeID = GetQueryString("FK_Node");
    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    
    window.location.href = url + "?FK_Node=" + nodeID;
}
//高级设置.
function AdvSetting() {

    var nodeID = GetQueryString("FK_Node");
    var url = "7.ByOtherBlock.htm?FK_Node=" + nodeID + "&M=" + Math.random();
    OpenEasyUiDialogExt(url, "高级设置", 600, 500, false);
}
function GetUrl(optionKey) {
    switch (parseInt(optionKey)) {
        case TurntoWay.TurntoDefault:
            url = "0.TurntoDefault.htm";
            break;
        case TurntoWay.TurntoMessage:
            url = "1.TurntoMessage.htm";
            break;
        case TurntoWay.TurntoUrl:
            url = "2.TurntoUrl.htm";
            break;
        case TurntoWay.TurntoClose:
            url = "3.TurntoClose.htm";
            
    }
    return url;
}