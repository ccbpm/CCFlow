
function InitBar(optionKey) {

    var html = "<div style='float:left'>列表操作</div>  选择方法类型:";
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

        { "No": "A", "Name": "无需集合支持" },
        { "No": "B", "Name": "需要集合支持" }

        /*  { "No": "C", "Name": "工具栏按钮" },*/
        /*  { "No": "D", "Name": "流程组件" },*/
    ];
    return json;
}

function GetDBDtl() {

    var json = [

        //无需集合支持.
        //{ "No": "New", "Name": "新建:创建实体按钮", "GroupNo": "A", "Url": "New.htm" },
        { "No": "SearchCond", "Name": "查询条件设置", "GroupNo": "A", "Url": "SearchCond.htm" },
        //{ "No": "Toolbar", "Name": "导出Excel", "GroupNo": "A", "Url": "Toolbar.htm" },
        //{ "No": "Toolbar", "Name": "导出固定的模板", "GroupNo": "A", "Url": "Toolbar.htm" },
        { "No": "Link", "Name": "自定义按钮", "GroupNo": "A", "Url": "Link.htm" },
        { "No": "QRCodeAddDict", "Name": "扫码填报", "GroupNo": "A", "Url": "QRCodeAddDict.htm" },
        { "No": "FlowNewEntity", "Name": "注册/新增实体类流程", "GroupNo": "A", "Url": "FlowNewEntity.htm" },

        //需要集合支持.
        //{ "No": "Delete", "Name": "删除:(批量删除选中的多条实体)", "GroupNo": "B", "Url": "Delete.htm" },
        { "No": "Func", "Name": "方法:(包含有参数与无参数的方法)", "GroupNo": "B", "Url": "Func.htm" },
        { "No": "Bill", "Name": "单据:批量发起.", "GroupNo": "B", "Url": "Bill.htm" },
        { "No": "LinkCollection", "Name": "自定义按钮", "GroupNo": "B", "Url": "LinkCollection.htm" },
        { "No": MethodModel.FlowEntityBatchStart, "Name": "实体批量发起流程", "GroupNo": "B", "Url": "FlowEntityBatchStart.htm" }

    ];
    return json;
}

function HelpOnline() {
    var url = "http://doc.ccbpm.cn";
    window.open(url);
}
function changeOption() {
    var frmID = GetQueryString("FrmID");

    var obj = document.getElementById("changBar");
    var sele = obj.options;
    var index = obj.selectedIndex;
    var optionKey = optionKey = sele[index].value;

    var url = GetUrl(optionKey);
    SetHref(url + "?FrmID=" + frmID + "&ModuleNo=" + GetQueryString("ModuleNo"));

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