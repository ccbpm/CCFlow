
//插入html.
function InsertHtmlToEditor(dataType, keyOfEn, name,uiBindKey,mapAttr)
{
    var _Html = "";
    //文本
    if (dataType == "Text")
        _Html = "<input type='text' value= ''  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //int型
    if (dataType == "Int")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "'  data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //Float型
    if (dataType == "Float")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //浮点型型
    if (dataType == "Double")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //金额
    if (dataType == "Money")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>";
    //日期
    if (dataType == "Date")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control Wdate' onfocus='WdatePicker({dateFmt:yyyy-MM-dd})'  leipiplugins='text' style='width:120px'/>";
    //时间
    if (dataType == "DateTime")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control Wdate' onfocus='WdatePicker({dateFmt:yyyy-MM-dd HH:mm:ss})' leipiplugins='text' style='width:160px'/>";
    //复选框
    if (dataType == "CheckBox")
        _Html = "<input type='CheckBox' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:100px'/>" + name;

    if (dataType == "Enum") {
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            return;
        _Html += "<span leipiplugins='enum' id='SR_" + uiBindKey + "' title='单选' name='leipiNewField'  data-key='" + keyOfEn + "' data-type='" + dataType + "'>";
        for (var i = 0; i < enums.length; i++) {
            _Html += "<input type='radio' value= '" + enums[i].IntKey + "' id='RB_" + keyOfEn + "_" + enums[i].IntKey + "' name='RB_" + keyOfEn + "' data-key='" + keyOfEn + "'  data-type='" + dataType + "'   data-bindKey='" + uiBindKey + "' class='form-control'  style='width:15px;height:15px;'/>" + enums[i].Lab + "&nbsp;&nbsp;";
        }
        _Html += "</span>";
    }
    if (dataType == "Select") {
        var sfTable = new Entity("BP.Sys.SFTable", uiBindKey);
        var srcType = sfTable.SrcType;
        if (srcType == 0)
            srcType = "BPClass"; //BP类
        if (srcType == 1)
            srcType = "CreateTable"; //创建表
        if (srcType == 2)
            srcType = "TableOrView"; //表或者视图
        if (srcType == 3)
            srcType = "SQL"; //SQL
        if (srcType == 4)
            srcType = "WebServices";//微服务
        if (srcType == 5)
            srcType = "Handler"; //Handler
        if (srcType == 6)
            srcType = "JQuery";//JQuery
        _Html += "<span leipiplugins='select' id='SS_" + uiBindKey + "' title='下拉框' name='leipiNewField'   data-sfTable='" + uiBindKey + "'>";
        _Html += "<select id='DDL_" + keyOfEn + "' name='DDL_" + keyOfEn + "' data-type='" + srcType + "'  data-key='" + keyOfEn + "'   class='form-control'>";
        _Html += "<option value=''>" + keyOfEn + "</option>";
        _Html += "</select>";
        _Html += "</span>";
    }
    if (dataType == "EnumSelect") {
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            return;
        _Html += "<span leipiplugins='enum' id='SS_" + uiBindKey + "' title='下拉框' name='leipiNewField' data-type='EnumSelect'   data-bindKey='" + uiBindKey + "'>";
        _Html += "<select id='DDL_" + keyOfEn + "' name='DDL_" + keyOfEn + "' data-type='EnumSelect' data-key='" + keyOfEn + "' class='form-control' >";
        for (var i = 0; i < enums.length; i++) {
                _Html += "<option value='" + enums[i].IntKey + "'>" + enums[i].Lab + "</option>";
        }
        _Html += "</select>";
        _Html += "</span>";
    }
    if (dataType == "HandWriting")//手写签字版
         _Html = "<img src='../../../DataUser/Siganture/admin.jpg' onerror=\"this.src='../../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapAttr.UIHeight + "px;' id='Img" + mapAttr.KeyOfEn + "' data-type='HandWriting' data-key='" + mapAttr.MyPK + "'  leipiplugins='component'/>";

    if (dataType == "Img")//图片

         _Html = "<img src='../CCFormDesigner/Controls/basic/Img.png' style='width:" + mapAttr.UIWidth + "px;height:" + mapAttr.UIHeight + "px'  leipiplugins='component' data-key='" + mapAttr.MyPK + "' data-type='Img'/>"
    if (dataType == "Map") { //地图
        _Html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapAttr.KeyOfEn + "' data-type='Map' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _Html += "<input type='button' name='select' value='选择' style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
        _Html += "<input type = text style='width:200px' maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
        _Html += "</div>";
    }

    if (dataType == "Score") { //评分
        _Html= "<div style='text-align:left;padding-left:0px'  data-type='Score' data-key='" + mapAttr.MyPK + "' leipiplugins='component'>";
        _Html += "<span class='simplestar'>";

        var num = mapAttr.Tag2;
        for (var i = 0; i < num; i++) {

            _Html += "<img src='../../Style/Img/star_2.png' />";
        }
        _Html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
        _Html += "</span></div>";
    }
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
            return"Text";
        } else if (mapAttr.MyDataType == "2") {
            return "Int";
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
            return "EnumSelect";
        } //外键下拉框
        else if (mapAttr.LGType == 2) {
            return "Select";
        }
        //外部数据源
        else if (mapAttr.LGType == 0) {
            return "Select";
        }
    }  else if (mapAttr.UIContralType == 3) {//单选框
        return "Enum";
    }

    if (mapAttr.MyDataType == 1) {
        if (mapAttr.UIContralType == 8)//手写签字版
            return "HandWriting";
        if (mapAttr.UIContralType == 11)//图片
            return "Img";
        if (mapAttr.UIContralType == 4)//地图
            return "Map";
        if (mapAttr.UIContralType == 101)//评分
            return "Score";
    }

}