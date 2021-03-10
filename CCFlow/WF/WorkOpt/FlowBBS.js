/**
 * 评论组件
 */
$(function () {
    //初始化获取评论组件的信息
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FlowBBSList");
    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
    }
    ShowFlowBBS(JSON.parse(data));
});
/**
 * 加载评论组件的信息
 * @param {any} data 评论信息列表
 */
function ShowFlowBBS(data) {
    var isHaveMySelf = false;
    var _Html = "";
    var str = "";
    var strT = "";

    for (var i = 0; i < data.length; i++) {
        var webUser = new WebUser();
        if (data[i].Rec == webUser.No)
            isHaveMySelf = true;
        if (str.indexOf('@' + data[i].DeptNo + '@') == -1) {
            str += '@' + data[i].DeptNo + '@';
            strT += '@' + data[i].DeptName + '@';
        }
            
    }
    _Html += "<div>";
    var strs = str.split("@"); //生成数组.
    var strTs = strT.split("@");
    for (var idx = 0; idx < strs.length; idx++) {
        var dept = strs[idx];
        if (dept == "" || dept == null)
            continue;
        _Html += "<div class='row' style='margin-left:10px;margin-right:10px'>";
        _Html += "<label style='font-size:13px;font-weight:bold'>" + strTs[idx] + "</label>";
        for (var i = 0; i < data.length; i++) {
            var bbs = data[i];
            if (bbs.DeptNo != dept)
                continue;
            _Html += "<div class='row' style='margin-left:0px;margin-right:0px'>";
            _Html += "<div col-xs-12 style='margin-top:5px'><font color=green>" + bbs.Msg + "</font>";
            _Html += "</div>";
            _Html += "<div col-xs-8 style='text-align:right'>" + bbs.RecName + "&nbsp;&nbsp;" + bbs.RDT;
            _Html += "</div>";
            _Html += "</div>";
        }
        _Html += "</div>";
    }
    _Html += "</div>";
    //只读状态并且当前登陆人的的抄送列表还未发生评论
    if (pageData.IsReadonly == "1" && isHaveMySelf == false && GetQueryString("CCSta") == "1") {
        

        _Html += "</select>";
        _Html += "<div style='line-height: 1px;border-top: 2px solid #ddd;margin-top: 4px;margin-bottom: 4px;margin-left: -6px;margin-right: -6px;'></div>";
        _Html += "<div>";
        _Html += "<textarea rows='5' id='FlowBBS_Doc' name='FlowBBS_Doc' cols='60'></textarea>";
        _Html += "<br/>";
       
        _Html += "<a onclick='AddCommUseWord(\"" + pageData.FK_Node + "\",\"Comment\",\"FlowBBS_Doc\");'><span>常用短语</span> <img alt='编辑常用评论语言.' src='../../WF/Img/Btn/Edit.gif' /></a>";
        _Html += "<input type='button' id='Btn_BBSSave' name='Btn_BBsSave' value='发送'style='float:right' onclick='BBSSubmit();' />";
        _Html += "</div>";

    }

    $("#FlowBBS").html(_Html);
}

function SetFlowBBSVal() {

    var objS = document.getElementById("DuanYu");
    var val = objS.options[objS.selectedIndex].value;

    if (val == "")
        return;
    
    if ($("#FlowBBS_Doc").length == 1) {
        $("#FlowBBS_Doc").val(val);
    }

}

/**
 * 评论组件的提交
 */
function BBSSubmit() {

    if ($("#FlowBBS_Doc").val() == null || $("#FlowBBS_Doc").val() == "" || $("#FlowBBS_Doc").val().trim().length == 0) {
        alert("未填写意见!");
        return;
    }

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    handler.AddFormData();
    var data = handler.DoMethodReturnString("FlowBBS_Save");
    if (data.indexOf('err@') == 0) {
        alert(data);
        return;
    }
    alert("发送成功");

    //获取所有的评论内容
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FlowBBSList");
    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
        return;
    }

    if ( typeof ReadAndClose === "function") {
        ReadAndClose();
    }

}
