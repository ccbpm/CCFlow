
//生成拼音.
function ParsePinYin(str, model, textBoxId) {

    var data = SpecWords(str);
    if (data == null) {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
        handler.AddPara("name", str);
        handler.AddPara("flag", model);
        data = handler.DoMethodReturnString("ParseStringToPinyin");
    }

    if (textBoxId == undefined || textBoxId == null)
        return data;

    document.getElementById(textBoxId).value = data;
    return data;
}
