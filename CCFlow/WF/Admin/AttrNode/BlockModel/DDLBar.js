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

    //调用公共类库的方法:执行批量主表赋值
    GenerFullAllCtrlsVal(node);

    $("#TB_Alert").val(node.BlockAlert);
    switch (parseInt(node.BlockModel)) {
        case 0:
            break;
        case 1:
            break;
        case 2:
            $("#TB_SpecSubFlow").val(node.BlockExp);
            break;
        case 3:
            $("#TB_SpecSubFlowNode").val(node.BlockExp);
            break;
        case 4:
            $("#TB_Exp").val(node.BlockExp);
            break;
        case 5:
            $("#TB_SQL").val(node.BlockExp);
            break;
        case 6:
            $("#TB_SameLevelSubFlow").val(node.BlockExp);
            break;
        default:
            break;
    }
    return;
}
function InitBar(optionKey) {

    var html = "发送阻塞规则:";
    html += "<select id='changBar' onchange='changeOption()'>";

    html += "<option value=null  disabled='disabled'>+阻塞规则</option>";
    html += "<option value=" + BlockModel.None + ">不阻塞</option>";
    html += "<option value=" + BlockModel.IncompleteBlock + ">当前节点有未完成的子流程时 </option>";
    html += "<option value=" + BlockModel.AppointmentBlock + ">按约定格式阻塞未完成子流程</option>";
    html += "<option value=" + BlockModel.ByParentBlock + ">是否启用为父流程时，子流程未运行到指定的节点 </option>";
    html += "<option value=" + BlockModel.ByChildBlock + ">是否启用为平级子流程时，子流程未运行到指定的节点</option>";

    html += "<option value=" + BlockModel.BySQLBlock + " >  按照SQL阻塞</option>";
    html += "<option value=" + BlockModel.ByExpressionBlock +"> 按照表达式阻塞</option>";
    html += "<option value=" + BlockModel.ByOtherBlock + ">  其他选项设置 </option>";

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
        case BlockModel.None:
            url = "0.None.htm";
            break;
        case BlockModel.IncompleteBlock:
            url = "1.IncompleteBlock.htm";
            break;
        case BlockModel.AppointmentBlock:
            url = "2.AppointmentBlock.htm";
            break;
        case BlockModel.ByParentBlock:
            url = "3.ByParentBlock.htm";
            break;
        case BlockModel.ByChildBlock:
            url = "4.ByChildBlock.htm";
            break;
        case BlockModel.BySQLBlock:
            url = "5.BySQLBlock.htm";
            break;
        case BlockModel.ByExpressionBlock:
            url = "6.ByExpressionBlock.htm";
            break;
        case BlockModel.ByOtherBlock:
            url = "7.ByOtherBlock.htm";
            break;
    }
    return url;
}