var myPK = "";

//获取从上一个url传递来的参数列表
function QueryString() {
    var name, value, i;

    var str = location.href;
    var num = str.indexOf("?")

    str = str.substr(num + 1);
    var arrtmp = str.split("&");

    for (i = 0; i < arrtmp.length; i++) {
        num = arrtmp[i].indexOf("=");
        if (num > 0) {
            name = arrtmp[i].substring(0, num);
            value = arrtmp[i].substr(num + 1);
            this[name] = value;
        }
    }
}

$(function () {

    var Request = new QueryString();
    myPK = Request["MyPK"];

    LoadGrid();
}
)

//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        if (jsonData.length > 17) {

            var pushData = eval('(' + jsonData + ')');
            var doc = pushData.Rows[0].Doc.replace(/~/g, "'");

            document.getElementById("tdTitle").innerHTML = pushData.Rows[0].Title;
            document.getElementById("lbSender").innerHTML = pushData.Rows[0].Sender;
            document.getElementById("lbRDT").innerHTML = pushData.Rows[0].RDT.toString();
            document.getElementById("lbSendTo").innerHTML = pushData.Rows[0].SendTo;
            document.getElementById("divDoc").innerHTML = doc;
        }
    }
  
    else {
        $.ligerDialog.warn('加载数据出错，请关闭后重试！');
    }
    //修改已读  状态
    Application.data.upMsgSta(myPK, upMsgSta, this);
}
//加载 详细信息
function LoadGrid() {
    Application.data.getDetailSms(myPK, callBack, this);
}
//修改数据状态  2013.05.23 H
function upMsgSta(my_PK, jsonData, scope) {
    if (jsonData) { }
}