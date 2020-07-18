//处理错误信息.
function DealErrMsg(msg) {

  

    var msgFieldly = ""; //友好的提示.
    var msgTech = "";    //技术信息错误.

    if (msg.indexOf('调用类') == 0) {
        msgFieldly = "抱歉,系统出现错误,请您把错误信息反馈给管理员.";
        msgTech = msg;
    }

    if (msg.indexOf('流程设计错误') == 0) {
        msgFieldly = "抱歉,系统出现错误,请您把错误信息反馈给管理员.";
        msgTech = msg;
    }

    if (msg.indexOf('流程设计错误') == 0) {
        msgFieldly = "抱歉,流程设计出现错误,请您把错误信息反馈给管理员.";
        msgTech = msg;
    }

    if (msg.indexOf('Node_SaveWork') == 0) {
        msgFieldly = "保存错误.";
        msgTech = msg;
    }

    if (msg.indexOf('Node_SaveWork') == 0 || msg.indexOf('工作已经发送到下一个环节') > 0)
    {
        msgFieldly = "该工作已经运行到下一个环节,执行保存失败..";
        msgTech = msg;
    }

    var divFieldly = $("#divFieldly"); //友好提示.
    var divTech = $("#divTech"); //技术要显示的信息.

}
