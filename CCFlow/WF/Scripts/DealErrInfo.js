var msgFieldly = ""; //友好的提示.
var msgTech = "";    //技术信息错误.
var flag = 0;
//处理错误信息.
function DealErrMsg(msg) {

    msgFieldly = ""; //友好的提示.
    msgTech = "";    //技术信息错误.
    flag = 0;
    if (msg.indexOf('调用类') == 0 || msg.indexOf('err@调用类') == 0) {
        msgFieldly = "<img src='../../WF/img/info.png' height='20' width='20'/>抱歉,系统出现错误,请您把错误信息反馈给管理员.";
        msgTech = msg;
    }

    if (msg.indexOf('err@流程设计错误') == 0 || msg.indexOf('流程设计错误') >= 0) {
        msgFieldly = "<img src='../../WF/img/info.png' height='20' width='20'/>抱歉,流程设计出现错误,请您把错误信息反馈给管理员.";
        msgTech = msg;
    }

    if (msg.indexOf('保存错误') >= 0) {
        msgFieldly = "<img src='../../WF/img/info.png' height='20' width='20'/>保存错误.";
        msgTech = msg;
    }

    if (msg.indexOf('工作已经发送到下一个环节') > 0) {
        msgFieldly = "<img src='../../WF/img/info.png' height='20' width='20'/>该工作已经运行到下一个环节,执行保存失败.";
        msgTech = msg;
    }

    if (msgFieldly != "") {

        //友好提示.
        $(".divFieldly").html(msgFieldly);
        $(".divFieldly").css("color", "red");

        $(".divFieldly").show();

        var tech = "<br><a href='javascript: void(0)' onclick='msgTchClick()' ><img src='../../WF/img/Message24.png' height='20' width='20'/>技术信息</a>";
        $(".divTech").html(tech); //技术要显示的信息

        $(".divTech").show();

        msgTech = "<img src='../../WF/img/Help.png' height='20' width='20' />" + msgTech;
        $(".techMsg").html(msgTech); //技术要显示的信息

        $("#techMsg").html(msgTech); //技术要显示的信息
        $("#techMsg").hide();
    }
}
function msgTchClick() {


    if (flag == 0) {
        $(".techMsg").show();
        $("#techMsg").show();
        flag = 1;
    } else {
        $(".techMsg").hide();
        $("#techMsg").hide();
        flag = 0;
    }

}