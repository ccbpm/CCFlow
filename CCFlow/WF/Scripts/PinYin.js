
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

function StrToPinYin(str) {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_FoolFormDesigner");
    handler.AddPara("name", str);
    handler.AddPara("flag", "false");
    data = handler.DoMethodReturnString("ParseStringToPinyin");
    return data;
}

//特别词汇.
function SpecWords(str) {

    if (str == '单价') return 'DanJia';
    if (str == '单位') return 'DanWei';

    if (str == '名称') return 'MingCheng';
    if (str == '项目编号') return 'PrjNo';
    if (str == '项目名称') return 'PrjName';
    if (str == '电话') return 'Tel';
    if (str == '地址') return 'Addr';
    if (str == '邮件') return 'Email';
    if (str == '手机') return 'Mobile';
    if (str == '合计') return 'HeJi';
   // if (str.indexOf('编号') != -1) return 'BillNo';
   // if (str.indexOf('单据') != -1) return 'BillNo';

 //   str = str.replace('单', 'Dan');
 //   str = str.replace('称', 'Cheng');

    return null;
}