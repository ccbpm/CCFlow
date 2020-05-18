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
        if (data[i].Rec == webUser.No)
            isHaveMySelf = true;
        if (str.indexOf('@' + data[i].DeptNo + '@') == -1)
            str += '@' + data[i].DeptNo + '@';
        strT += '@' + data[i].DeptName + '@';
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
    if (paramData.IsReadonly == "1" && isHaveMySelf == false && GetQueryString("CCSta") == "1") {
        var en = new Entity("BP.Sys.GloVar");
        en.SetPKVal("ND" + paramData.FK_Node + "_Comment");
        var DuanYu = "";
        if (en.RetrieveFromDBSources() == 0) {
            DuanYu = en.Val;
        }
        if (DuanYu != null && DuanYu != undefined && DuanYu != "") {

            var NewDuanYu = DuanYu.split("@");
        } else {
            var NewDuanYu = "";
        }

        _Html += "</select>";
        _Html += "<div style='line-height: 1px;border-top: 2px solid #ddd;margin-top: 4px;margin-bottom: 4px;margin-left: -6px;margin-right: -6px;'></div>";
        _Html += "<div>";
        _Html += "<textarea rows='5' id='TB_Msg' name='TB_Msg' cols='60'></textarea>";
        _Html += "<br/>";
        //加入常用短语.
        _Html += "<select id='DuanYu' onchange='SetDocVal()'>";
        _Html += "<option value=''>常用短语</option>";
        if (NewDuanYu.length > 0) {
            for (var i = 0; i < NewDuanYu.length; i++) {
                if (NewDuanYu[i] == "") {
                    continue;
                }
                _Html += "<option value='" + NewDuanYu[i] + "'>" + NewDuanYu[i] + "</option>";
            }
        } else {
            _Html += "<option value='已阅'>已阅</option>";
        }
        _Html += "</select>";
        _Html += "<a onclick='AddDuanYu(\"" + paramData.FK_Node + "\",\"Comment\");'> <img alt='编辑常用评论语言.' src='../../WF/Img/Btn/Edit.gif' /></a>";
        _Html += "<input type='button' id='Btn_BBSSave' name='Btn_BBsSave' value='提交评论'style='float:right' onclick='BBSSubmit();' />";
        _Html += "</div>";

    }

    $("#FlowBBS").html(_Html);
}

/**
 * 评论组件的提交
 */
function BBSSubmit() {

    if ($("#TB_Msg").val() == null || $("#TB_Msg").val() == "" || $("#TB_Msg").val().trim().length == 0) {
        alert("请填写评论内容!");
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
    alert("提交评论成功");

    //获取所有的评论内容
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
    handler.AddUrlData();
    var data = handler.DoMethodReturnString("FlowBBSList");
    if (data.indexOf('err@') == 0) {
        alert(data);
        console.log(data);
    }
    ShowFlowBBS(JSON.parse(data));

}
