
//插入html.
function InsertHtmlToEditor(dataType)
{
    var _Html = "";
    //文本
    if (dataType == "Text")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //int型
    if (dataType == "Int")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "'  data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //Float型
    if (dataType == "Float")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //金额
    if (dataType == "Money")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //日期
    if (dataType == "Date")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' onfocus='WdatePicker({dateFmt:yyyy-MM-dd})'  leipiplugins='text' style='width:120px'/>";
    //时间
    if (dataType == "DateTime")
        _Html = "<input type='text' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' onfocus='WdatePicker({dateFmt:yyyy-MM-dd HH:mm:ss})' leipiplugins='text' style='width:160px'/>";
    //复选框
    if (dataType == "CheckBox")
        _Html = "<input type='CheckBox' value= '' id='TB_" + KeyOfEn + "' name='TB_" + KeyOfEn + "' data-key='" + KeyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:100px'/>" + name;

    editor.execCommand('insertHtml', _Html);
}

function InsertHtmlToEditorByMapAttr(mapAttr)
{
    InsertHtmlToEditor(GetDataType(mapAttr));
}

function GetDataType(mapAttr) {

    var ctType = "";
    if (mapAttr.UIContralType == 0) {
        //控件数据类型
        if (mapAttr.MyDataType == "1") {
            ctType = "Text";
        } else if (mapAttr.MyDataType == "2") {
            ctType = "Int";
        } else if (mapAttr.MyDataType == "3") {
            return "Float";
        } else if (mapAttr.MyDataType == "4") {
            return "Money";
        } else if (mapAttr.MyDataType == "5") {
            return "Double";
        } else if (mapAttr.MyDataType == "6") {
            return "Date";
        } else if (mapAttr.MyDataType == "7") {
            return "DateTime";
        } else if (mapAttr.MyDataType == "8") {
            return "Money";
        }
    } else if (mapAttr.UIContralType == 1) {
        //枚举下拉框
        if (mapAttr.LGType == 1) {
            return "枚举下拉框";
        } //外键下拉框
        else if (mapAttr.LGType == 2) {
            return "外键下拉框";
        }
        //外部数据源
        else if (mapAttr.LGType == 0) {
            return "外部数据源";
        }
    } else if (mapAttr.UIContralType == 2) {//复选框
        return "复选框";
    } else if (mapAttr.UIContralType == 3) {//单选框
        return "";
    }

}