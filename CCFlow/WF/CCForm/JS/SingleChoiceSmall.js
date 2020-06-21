//小范围的多选,不需要搜索.
function SingleChoiceSmall(mapExt, mapAttr, frmData, tbID, rowIndex, OID) {
    if (tbID == null || tbID == undefined) {
        tbID = "TB_" + mapExt.AttrOfOper;
    }
    var cbxID = mapExt.AttrOfOper + "_select";
    if (rowIndex != null && rowIndex != undefined)
        cbxID = mapExt.AttrOfOper + "_select_" + rowIndex;
    var tbTextID = tbID + "T";

    var webUser = new WebUser();
    var data = [];
    var valueField = "No";
    var textField = "Name";
    switch (parseInt(mapExt.DoWay)) {
        case 1:
            var tag1 = mapExt.Tag1;
            tag1 = tag1.replace(/;/g, ',');


            $.each(tag1.split(","), function (i, o) {
                data.push({ No: i, Name: o })
            });
            break;
        case 2:
            var enums = new Entities("BP.Sys.SysEnums");
            enums.Retrieve("EnumKey", mapExt.Tag2);
            if (mapExt.Tag == "1" || mapExt.Tag == "2")
                $.each(enums, function (i, o) {
                    data.push({ No: o.EnumKey, Name: o.Lab, IntKey: o.IntKey })
                });
            else
                $.each(enums, function (i, o) {
                    data.push({ No: o.IntKey, Name: o.Lab })
                });
            //data = enums;
            break;
        case 3:
            if (frmData != null && frmData != undefined) {

                data = frmData[mapExt.Tag3];
                if (data == undefined) {
                    var en = new Entity("BP.Sys.SFTable", mapExt.Tag3);
                    data = en.DoMethodReturnJSON("GenerDataOfJson");
                    frmData[mapExt.Tag3] = data;
                }
            } else {
                var en = new Entity("BP.Sys.SFTable", mapExt.Tag3);
                data = en.DoMethodReturnJSON("GenerDataOfJson");
            }
            break;
        case 4:
            var tag4SQL = mapExt.Tag4;
            tag4SQL = tag4SQL.replace('@WebUser.No', webUser.No);
            tag4SQL = tag4SQL.replace('@WebUser.Name', webUser.Name);
            tag4SQL = tag4SQL.replace('@WebUser.FK_DeptName', webUser.FK_DeptName);
            tag4SQL = tag4SQL.replace('@WebUser.FK_Dept', webUser.FK_Dept);

            if (tag4SQL.indexOf('@') == 0) {
                alert('约定的变量错误:' + tag4SQL + ", 没有替换下来.");
                return;
            }
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCFrom");
            handler.AddPara("SQL", tag4SQL);
            data = handler.DoMethodReturnString("RunSQL_Init");
            break;
    }

    (function (AttrOfOper, data, FK_MapData) {

        //如果是单选按钮
        if (mapExt.Tag == "1" || mapExt.Tag == "2") {
            return MakRadioBoxsModel(mapExt, data, mapAttr, tbID);
        }

        var textbox = $("#" + tbID);
        textbox.hide();
        var tbVal = textbox.val();
        if (tbVal == null) tbVal = "";

        var enableAttr = '';
        if (mapAttr != null && mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        var name = "DDL_" + mapExt.AttrOfOper
       
        var select = $("<select " + enableAttr + "  id='" + name + "' name='" + name + "'onchange='changeValue(\"" + tbID + "\",\"" + name + "\")'  />");

        var options = "";
        var textVal;
        for (var i = 0; i < data.length; i++) {

            var en = data[i];

           
            var keyValue = en.No;

            if (tbVal.indexOf(keyValue + ',') != -1)
                textVal = en.Name;
         
            options += "<option" + (tbVal.indexOf(keyValue + ',') != -1 ? " selected='selected' " : "") + " value='" + keyValue + "'>" + en.Name + "</option>";

        }
        textbox.before(select);
        select.append(options);
        textbox.val(select.val());
        $("#" + tbID + "T").val(textVal);

    })(mapExt.AttrOfOper, data, mapExt.FK_MapData);
}


//单选 模式.
function MakRadioBoxsModel(mapExt, data, mapAttr, tbID) {

    var textbox = $("#" + tbID);
    textbox.hide();
    var tbVal = textbox.val();
    if (tbVal == null) tbVal = "";
    for (var i = 0; i < data.length; i++) {

        var en = data[i];

        var name;
        var id;
        var keyValue;
        if (mapExt.DoWay == 2) {
            name = "RB_" + mapExt.AttrOfOper + "_" + en.No;
            id = name + "_" + en.IntKey
            keyValue = en.IntKey;
        } else {
            name = "RB_" + mapExt.AttrOfOper + "_" + mapExt.AttrOfOper;
            id = name + "_" + en.No;
            keyValue = en.No;
        }
        var enableAttr = '';
        if (mapAttr != null && mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        var rb = $("<input " + enableAttr + " type='radio' id='" + id + "' name='" + name + "' value='" + keyValue + "'onclick='changeValue(\"" + tbID + "\",\"" + name + "\")'  />");


        if (tbVal.indexOf(keyValue + ',') != -1) {
            rb.attr("checked", true);
            textbox.val(keyValue);
            $("#" + tbID + "T").val(en.Name);
        } 
        else
            rb.attr("checked", false);

        //开始绑定事件.

        //end 绑定checkbox事件. @解相宇 绑定取消选择事件.

        textbox.before(rb);

        if (mapExt.Tag == "1")
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "&nbsp;&nbsp;</label>");
        else
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "&nbsp;&nbsp;</label><br>");

        textbox.before(lab);
    }

}

function changeValue(changeIdV, getNameV) {
    if (getNameV.indexOf("DDL_") != -1) {
        $("#" + changeIdV).val($("#" + getNameV).val());
        $("#" + changeIdV + "T").val($("#" + getNameV).find("option:selected").text());
    }
    else {
        $("#" + changeIdV).val($("input[name='" + getNameV + "']:checked").val());
        $("#" + changeIdV+"T").val($("input[name='" + getNameV + "']:checked").next().text());
    }
       
}




