
function InitBar(optionKey) {

    var html = "<div style='float:left'>一条记录上要执行的相关操作</div>  选择方法类型:";
    html += "<select id='changBar' onchange='changeOption()'>";

    var groups = GetDBGroup();
    var dtls = GetDBDtl();

    for (var i = 0; i < groups.length; i++) {

        var group = groups[i];
        html += "<option value=null  disabled='disabled'>+" + group.Name + "</option>";

        for (var idx = 0; idx < dtls.length; idx++) {
            var dtl = dtls[idx];
            if (dtl.GroupNo != group.No)
                continue;
            html += "<option value=" + dtl.No + ">&nbsp;&nbsp;" + dtl.Name + "</option>";
        }
    }

    html += "</select>";

    html += "<button  id='Btn_Save' type=button class='cc-btn-tab btn-save' onclick='Save()'>保存</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

    document.getElementById("bar").innerHTML = html;
    $("#changBar option[value='" + optionKey + "']").attr("selected", "selected");
}

function GetDBGroup() {

    var json = [

        { "No": "A", "Name": "常规组件" },
        { "No": "B", "Name": "实体组件" },
        //{ "No": "C", "Name": "打印组件" },
        { "No": "D", "Name": "流程类" }

        /*  { "No": "C", "Name": "工具栏按钮" },*/
        /*  { "No": "D", "Name": "流程组件" },*/
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        { "No": MethodModel.Link, "Name": "自定义URL菜单", "GroupNo": "A", "Url": "Link.htm" },
        { "No": MethodModel.Func, "Name": "方法:(包含有参数与无参数的方法)", "GroupNo": "A", "Url": "Func.htm" },
        { "No": MethodModel.Bill, "Name": "单据:缴费单、维修单、报销单不需流程审批.", "GroupNo": "A", "Url": "Bill.htm" },


        { "No": MethodModel.FrmBBS, "Name": "评论/审核/日志组件", "GroupNo": "B", "Url": "FrmBBS.htm" },
        { "No": MethodModel.DataVer, "Name": "数据快照", "GroupNo": "B", "Url": "DataVer.htm" },
        { "No": MethodModel.DictLog, "Name": "操作日志", "GroupNo": "B", "Url": "DictLog.htm" },
        { "No": MethodModel.QRCode, "Name": "二维码(扫码手机上查看)", "GroupNo": "B", "Url": "QRCode.htm" },
        { "No": "Toolbar", "Name": "工具栏按钮组件(打印、导出)", "GroupNo": "B", "Url": "Toolbar.htm" },
        { "No": MethodModel.ImpFromFile, "Name": "从文件里导入数据", "GroupNo": "B", "Url": "ImpFromFile.htm" },

        { "No": "PrintRTF", "Name": "RTF模板打印", "GroupNo": "C", "Url": "PrintRTF.htm" },
        { "No": "PrintHtml", "Name": "html打印", "GroupNo": "C", "Url": "PrintHtml.htm" },
        { "No": "PrintPDF", "Name": "PDF打印", "GroupNo": "C", "Url": "PrintRTF.htm" },
        { "No": "PrintZip", "Name": "打包下载", "GroupNo": "C", "Url": "PrintZip.htm" },


        { "No": MethodModel.FlowBaseData, "Name": "(新建)修改基础数据流程", "GroupNo": "D", "Url": "FlowBaseData.htm" },
        { "No": MethodModel.FlowEtc, "Name": "(新建)其他业务流程", "GroupNo": "D", "Url": "FlowEtc.htm" },
        //{ "No": MethodModel.FlowNewEntity, "Name": "(新建)实体(注册)流程", "GroupNo": "D", "Url": "FlowNewEntity.htm" },
        //{ "No": MethodModel.FlowEntityBatchStart, "Name": "(新建)批量发起", "GroupNo": "D", "Url": "FlowEntityBatchStart.htm" },
        { "No": MethodModel.SingleDictGenerWorkFlows, "Name": "实体流程汇总列表(综合流程列表)", "GroupNo": "D", "Url": "SingleDictGenerWorkFlows.htm" }
         


    ];
    return json;
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}
function changeOption() {
    var groupID = GetQueryString("GroupID");
    var frmID = GetQueryString("FrmID");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    SetHref(url + "?GroupID=" + groupID + "&FrmID=" + frmID + "&ModuleNo=" + GetQueryString("ModuleNo"));

}
function GetUrl(optionKey) {

    var json = GetDBDtl();
    for (var i = 0; i < json.length; i++) {
        var en = json[i];
        if (en.No === optionKey)
            return en.Url;
    }

    alert(optionKey);

    return "0.QiangBan.htm";
}