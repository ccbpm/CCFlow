
if ("undefined" == typeof IsShowDevelopFieldLab) {
    IsShowDevelopFieldLab =true;
}

//插入html.
function InsertHtmlToEditor(dataType, keyOfEn, name,uiBindKey,mapAttr,uicontrolType)
{
    var _Html = "";
    var fieldName = "";
    if (IsShowDevelopFieldLab == true)
        fieldName = name
    //文本
    if (dataType == "Text")
        _Html = "<input type='text' value= ''  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>" + fieldName;
    if (dataType == "Textarea") {
        if (uicontrolType == 14)
            _Html = "<textarea id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='SignCheck'  leipiplugins='textarea' value='' orgrich='0' orgfontsize='12' orgwidth='600' orgheight='80' style='font-size: 12px; width: 100%; height: 59px; margin: 0px;' placeholder='签批组件'></textarea>" + fieldName;
        else if (uicontrolType == 15)
            _Html = "<textarea id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='FlowBBS'  leipiplugins='textarea' value='' orgrich='0' orgfontsize='12' orgwidth='600' orgheight='80' style='font-size: 12px; width: 528px; height: 59px; margin: 0px;' placeholder='评论组件'></textarea>" + fieldName;
        else
            _Html = "<textarea id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  leipiplugins='textarea' value='' orgrich='0' orgfontsize='12' orgwidth='600' orgheight='80' style='font-size: 12px; width: 100%; height: 59px; margin: 0px;'></textarea>" + fieldName;
    }
    //int型
    if (dataType == "Int")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "'  data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>" + fieldName;
    //Float型
    if (dataType == "Float")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>" + fieldName;
    //浮点型
    if (dataType == "Double")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>" + fieldName;
    //金额
    if (dataType == "Money")
        _Html = "<input type='text' value= '' id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  class='form-control' leipiplugins='text' style='width:120px'/>" + fieldName;
    //日期
    if (dataType == "Date")
        _Html = '<input type="text" value= ""  id="TB_' + keyOfEn + '" name="TB_' + keyOfEn + '" data-key="' + keyOfEn + '" data-name="' + name + '" data-type="' + dataType + '"  class="form-control Wdate"   leipiplugins="text" style="width:120px;display:inline"/>' + fieldName;
    //时间
    if (dataType == "DateTime")
        _Html = '<input type="text" value= ""  id="TB_' + keyOfEn + '" name="TB_' + keyOfEn + '" data-key="' + keyOfEn + '" data-name="' + name + '" data-type="' + dataType + '"  class="form-control Wdate"  leipiplugins="text" style="width:160px;display:inline"/>' + fieldName;
    //复选框
    if (dataType == "CheckBox")
        _Html = "<input type='CheckBox' value= '' id='CB_" + keyOfEn + "' name='CB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'  leipiplugins='text' style='margin: 4px 0px;vertical-align:middle'/>&nbsp;" + name + " " + fieldName;

    if (dataType == "Radio") {
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            return;
        _Html += "<span leipiplugins='enum' id='SR_" + keyOfEn + "' title='单选' name='leipiNewField'  data-key='" + keyOfEn + "' data-type='" + dataType + "'>";
        for (var i = 0; i < enums.length; i++) {
            _Html += "<label><input type='radio' value= '" + enums[i].IntKey + "' id='RB_" + keyOfEn + "_" + enums[i].IntKey + "' name='RB_" + keyOfEn + "' data-key='" + keyOfEn + "'  data-type='" + dataType + "'   data-bindKey='" + uiBindKey + "'  style='width:15px;height:15px;'/>" + enums[i].Lab + "</label>&nbsp;&nbsp;";
        }
        _Html += "</span>";
    }
    if (dataType == "EnumCheckBox") {
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            return;
        _Html += "<span leipiplugins='enum' id='SC_" + keyOfEn + "' title='复选框' name='leipiNewField' data-type='EnumCheckBox'  data-key='" + keyOfEn + "'   data-bindKey='" + uiBindKey + "'>";
        for (var i = 0; i < enums.length; i++) {
                _Html += "<label><input type='checkbox' value= '" + enums[i].IntKey + "' id='CB_" + keyOfEn + "_" + enums[i].IntKey + "' name='CB_" + keyOfEn + "' data-key='" + keyOfEn + "'  data-type='" + dataType + "'   data-bindKey='" + uiBindKey + "'   style='width:15px;height:15px;'/>" + enums[i].Lab  + "&nbsp;&nbsp;</label>";
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
        if (srcType == 7)
            srcType = "SFTable";
        if (srcType == 8)
            srcType = "WebApi";
        _Html += "<span leipiplugins='select' id='SS_" + keyOfEn + "' title='下拉框' name='leipiNewField'   data-sfTable='" + uiBindKey + "'>";
        _Html += "<select id='DDL_" + keyOfEn + "' name='DDL_" + keyOfEn + "' data-type='" + srcType + "'  data-key='" + keyOfEn + "'   class='form-control'  style='width:80%'>";
        _Html += "<option value=''>" + keyOfEn + "</option>";
        _Html += "</select>";
        _Html += "</span>";
        _Html += name;
    }
    if (dataType == "EnumSelect") {
        //获取枚举值
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", uiBindKey);
        if (enums.length == 0)
            return;
        _Html += "<span leipiplugins='enum' id='SS_" + keyOfEn + "' title='下拉框' name='leipiNewField' data-type='EnumSelect'   data-bindKey='" + uiBindKey + "'>";
        _Html += "<select id='DDL_" + keyOfEn + "' name='DDL_" + keyOfEn + "' data-type='EnumSelect' data-key='" + keyOfEn + "' class='form-control'  style='width:80%'>";
        for (var i = 0; i < enums.length; i++) {
                _Html += "<option value='" + enums[i].IntKey + "'>" + enums[i].Lab + "</option>";
        }
        _Html += "</select>";
        _Html += "</span>";
        _Html += name;
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
        _Html = "<span class='score-star' style='text-align:left;padding-left:0px'   data-key='" + mapAttr.MyPK + "' id='SC_" + mapAttr.KeyOfEn + "'>";
        _Html += "<span class='simplestar' data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "' id='Star_" + mapAttr.KeyOfEn + "'>";

        var num = mapAttr.Tag2;
        for (var i = 0; i < num; i++) {

            _Html += "<img src='../../Style/Img/star_1.png'  data-type='Score'  leipiplugins='component'  data-key='" + mapAttr.MyPK + "'/>";
        }
        _Html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
        _Html += "</span></span>";
        _Html +="<input type='text' id='TB_"+keyOfEn+"' name='TB_"+keyOfEn+"' style='display:none'/>"
    }
    if (dataType == "SignCheck")
        _Html = "<input type='text'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'   leipiplugins='text' style='width:98%' placeholder='签批组件'/>";

    if (dataType == "FlowBBS")
        _Html = "<input type='text'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'   leipiplugins='text' style='width:98%' placeholder='评论组件'/>";

    if (dataType == "GovDocFile")
        _Html = "<input type='text'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'   leipiplugins='component' style='width:98%' placeholder='公文正文组件'/>";

    if (dataType == "DocWord")
        _Html = "<input type='text'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'   leipiplugins='component' style='width:98%' placeholder='发文字号'/>";

    if (dataType == "DocWordReceive")
        _Html = "<input type='text'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='" + dataType + "'   leipiplugins='component' style='width:98%' placeholder='收文字号'/>";

    if (dataType == "Btn")
        _Html = "<input type='button'  id='TB_" + keyOfEn + "' name='TB_" + keyOfEn + "' data-key='" + keyOfEn + "' data-name='" + name + "' data-type='Btn' value='" + name + "'   leipiplugins='component' class='Btn'/>";

    editor.execCommand('insertHtml', _Html);
}

function InsertHtmlToEditorByMapAttr(mapAttr)
{
    InsertHtmlToEditor(GetDataType(mapAttr));
}

function GetDataType(mapAttr) {

    var ctType = "";
    if (mapAttr.UIContralType == 2 && mapAttr.MyDataType == "4")
        return "CheckBox";
    if (mapAttr.UIContralType == 0) {
        //控件数据类型
        if (mapAttr.MyDataType == "1") {
            if (mapAttr.IsSupperText == 1)
                return "Textarea";

            return"Text";
        } else if (mapAttr.MyDataType == "2") {
            return "Int";
        } else if (mapAttr.MyDataType == "3") {
            return "Float";
        } else if (mapAttr.MyDataType == "4") {
            return "CheckBox";
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
        return "Radio";
    }
    if (mapAttr.MyDataType == "1" && mapAttr.UIContralType == 2) //枚举复选框
        return "EnumCheckBox";

    if (mapAttr.MyDataType == 1) {
        if (mapAttr.UIContralType == 8)//手写签字版
            return "HandWriting";
        if (mapAttr.UIContralType == 11)//图片
            return "Img";
        if (mapAttr.UIContralType == 4)//地图
            return "Map";
        if (mapAttr.UIContralType == 101)//评分
            return "Score";

        if (mapAttr.UIContralType == 14)//签批组件
            return "SignCheck";

        if (mapAttr.UIContralType == 15)//评论组件
            return "FlowBBS";
        if (mapAttr.UIContralType == 16)//评论组件
            return "Fixed";

        if (mapAttr.UIContralType == 110) //公文正文组件.
            return "GovDocFile";

        if (mapAttr.UIContralType == 17) //发文字号
            return "DocWord";
        

        if (mapAttr.UIContralType == 170) //收文字号
            return "DocWordReceive";

        if (mapAttr.UIContralType == 18)//按钮
            return "Btn";
    }

}