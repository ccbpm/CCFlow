
//转拼音，并去除开头为数字的字符
function ParseStringToPinYin(parseString, parseModel, prix) {

    parseString = $.trim(parseString);
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    handler.AddPara("name", parseString);
    handler.AddPara("flag", parseModel);
    var data = handler.DoMethodReturnString("ParseStringToPinyin");

    if (prix == "")
        document.getElementById("TB_No").value = data;
    else
        document.getElementById("TB_No").value = prix + "_" + data;

}