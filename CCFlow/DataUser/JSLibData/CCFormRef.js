
function GenerNextStepEmp() {
    var qingjiaren = $("TB_QingJiaRen").val();

    var url = "xxx.aspx?QingJiaRen=" + qingjiaren;

    $("TB_DiYiJiShenPiRen").val("zhangsna");

    return "";
}


//表单结束之后执行的方法., 该方法放在了 AfterBindEn_DealMapExt 里面.
function frmLoadEnd() {


    var flowNo = GetQueryString("FK_Flow");
    if (flowNo == '008') {

        var webUser = new WebUser();

        var jxs = $("#TB_JingXiaoShang").val();
        if (jxs == null || jxs == '')
            jxs = "xxx";

        var title = "<b>[" + webUser.FK_DeptName + "]事业部[" + jxs + "]经销商-月核销单.</b>";
        $("#FrmTitle").html(title);
    }
}

/*
1. 该页面,是被引用到 /WF/MyFlowGener.htm, /WF/CCForm/FrmGener.htm 里面的.
2. 这里方法大多是执行后，返回json ,可以被页面控件调用. 
*/
function funDemo() {
    alert("我被执行了。");
}

function IsSelectAccount() {
    var val = $('input[name="RB_SFYHJZT"]:checked').val();
    //若是本级办理，就不做处理
    if ($("#DDL_SFBJBL option:selected").val() == 0)
        return;
    if (val == 0) {
        if (GetQueryString("NodeID") == "202") {
            //只显示会计主体子流程
            $("#DDL_ToNode option").eq(0).show();
            $("#DDL_ToNode option").eq(1).hide();
            //选择会计主体子流程
            $("#DDL_ToNode").val("302");
        }
    }
    else {
        if (GetQueryString("NodeID") == "202") {
            //隐藏会计主体子流程
            $("#DDL_ToNode option").eq(0).hide();
            $("#DDL_ToNode option").eq(1).show();
            //选择非会计主体子流程
            $("#DDL_ToNode").val("402");
        }
    }
}

function IsLoaclOperation() {

    var val = $("#DDL_SFBJBL option:selected").val();

    if (val == 0) {
        if (GetQueryString("NodeID") == "202") {
            //启动子流程的选项设置为不可见
            $("#DDL_ToNode option").eq(0).hide();
            $("#DDL_ToNode option").eq(1).hide();
            $("#DDL_ToNode option").eq(2).show();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("203");
        }
        if (GetQueryString("NodeID") == "302") {
            //启动子流程的选项设置为不可见
            $("#DDL_ToNode option").eq(0).hide();
            $("#DDL_ToNode option").eq(1).show();
            $("#DDL_ToNode option").eq(2).show();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("303");
        }
        if (GetQueryString("NodeID") == "402") {
            //启动子流程的选项设置为不可见
            $("#DDL_ToNode option").eq(0).hide();
            $("#DDL_ToNode option").eq(1).hide();
            $("#DDL_ToNode option").eq(2).show();
            $("#DDL_ToNode option").eq(3).show();
            $("#DDL_ToNode option").eq(4).show();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("403");
        }
        if (GetQueryString("NodeID") == "502") {
            //启动子流程的选项设置为不可见
            $("#DDL_ToNode option").eq(0).hide();
            $("#DDL_ToNode option").eq(1).show();
            $("#DDL_ToNode option").eq(2).show();
            $("#DDL_ToNode option").eq(3).show();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("503");
        }

    }
    else {
        if (GetQueryString("NodeID") == "202") {
            //显示启用启动子流程的选项
            $("#DDL_ToNode option").eq(0).show();
            $("#DDL_ToNode option").eq(1).show();
            $("#DDL_ToNode option").eq(2).hide();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("302");
        }
        if (GetQueryString("NodeID") == "302") {
            //显示启用启动子流程的选项
            $("#DDL_ToNode option").eq(0).show();
            $("#DDL_ToNode option").eq(1).hide();
            $("#DDL_ToNode option").eq(2).hide();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("401");
        }
        if (GetQueryString("NodeID") == "402") {
            //显示启用启动子流程的选项
            $("#DDL_ToNode option").eq(0).show();
            $("#DDL_ToNode option").eq(1).show();
            $("#DDL_ToNode option").eq(2).hide();
            $("#DDL_ToNode option").eq(3).hide();
            $("#DDL_ToNode option").eq(4).hide();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("502");
        }
        if (GetQueryString("NodeID") == "502") {
            //显示启用启动子流程的选项
            $("#DDL_ToNode option").eq(0).show();
            $("#DDL_ToNode option").eq(1).hide();
            $("#DDL_ToNode option").eq(2).hide();
            $("#DDL_ToNode option").eq(3).hide();
            //将Bar上的发送节点设置为结办节点
            $("#DDL_ToNode").val("621");
        }


    }
}
//FK_MapData,附件属性，RefPK,FK_Node
function afterDtlImp(FK_MapData, frmAth, newOID, FK_Node, oldOID, oldFK_MapData) {
    //处理从表附件导入的事件.

}

function CompareData() {
    if ($("#TB_StartTime").val() > $("#TB_EndTime").val()) {
        alert("开始时间不能大于结束时间");
        return false;
    }
    return true;
}

function HeJi() {
    var bmhj = $("#BMHJ").val();
    var jthj = $("#JTHJ").val();
    var bzhj = $("#BZHJ").val();
    $("#HeJj").val(bmhj + jthj + bzhj);
}

function GetShiJian(keyOfEn) {
    alert($("#TB_" + keyOfEn + "_0").val());
}


function IsSaveDtl() {
    var regInput = true;
    //获取页面的所有IFrame
    var frames = $("#divCCForm").find("iframe");
    var dtlFrames = $.grep(frames, function (frame, idx) {
        if (frame.id.indexOf("Dtl_") == 0)
            return frame;
    });

    //循环从表IFrame，如果有未填的返回false
    $.each(dtlFrames, function (idx, dtlFrame) {
        var mustInput = $(this).contents().find(".errorInput");

        if (mustInput.length > 0) {
            regInput = false;
            return;
        }
    });

    return regInput;
}    