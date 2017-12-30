//小范围的多选,不需要搜索.
function MultipleChoiceSmall(mapExt) {
    var data = [];
    var valueField = "No";
    var textField = "Name";
    switch (mapExt.DoWay) {
        case 1:
            var tag1 = mapExt.Tag1;
            tag1 = tag1.replace(';', ',');

            $.each(tag1.split(","), function (i, o) {
                data.push({ No: i, Name: o })
            });
            break;
        case 2:
            valueField = "IntKey"
            textField = "Lab";
            var enums = new Entities("BP.Sys.SysEnums");
            enums.Retrieve("EnumKey", mapExt.Tag2);
            data = enums;
            break;
        case 3:
            var en = new Entity("BP.Sys.SFTable", mapExt.Tag3);
            data = en.DoMethodReturnJSON("GenerDataOfJson");
            break;
        case 4:
            var tag4SQL = mapExt.Tag4;
            tag4SQL = tag4SQL.replace('@WebUser.No', webUser.No);
            tag4SQL = tag4SQL.replace('@WebUser.Name', webUser.Name);
            tag4SQL = tag4SQL.replace('@WebUser.FK_Dept', webUser.FK_Dept);
            tag4SQL = tag4SQL.replace('@WebUser.FK_DeptName', webUser.FK_DeptName);
            if (tag4SQL.indexOf('@') == 0) {
                alert('约定的变量错误:' + tag4SQL + ", 没有替换下来.");
                return;
            }
            data = DBAccess.RunSQLReturnTable(tag4SQL);
            break;
    }
    (function (AttrOfOper, data, FK_MapData) {
        var cbx = $("#" + AttrOfOper + "_combobox");
        var hiddenField = $('<input type="hidden" />');
        hiddenField.attr("id", "TB_" + AttrOfOper);
        hiddenField.attr("name", "TB_" + AttrOfOper);
        cbx.after(hiddenField);
        cbx.attr("class", "easyui-combobox");
        cbx.combobox({
            "editable": false,
            "valueField": valueField,
            "textField": textField,
            "multiple": true,
            "onSelect": function (p) {
                $("#TB_" + AttrOfOper).val(cbx.combobox("getText"));
                (function sel(n, KeyOfEn, FK_MapData) {
                    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
                    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
                    frmEleDB.FK_MapData = FK_MapData;
                    frmEleDB.EleID = KeyOfEn;
                    frmEleDB.RefPKVal = (pageData.WorkID || pageData.OID || "");
                    frmEleDB.Tag1 = n;
                    if (frmEleDB.Update() == 0) {
                        frmEleDB.Insert();
                    }
                })(p[valueField], AttrOfOper, FK_MapData);
            },
            "onUnselect": function (p) {
                $("#TB_" + AttrOfOper).val(cbx.combobox("getText"));
                (function unsel(n, KeyOfEn) {
                    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
                    frmEleDB.MyPK = KeyOfEn + "_" + (pageData.WorkID || pageData.OID || "") + "_" + n;
                    frmEleDB.Delete();
                })(p[valueField], AttrOfOper);
            }
        });
        cbx.combobox("loadData", data);
    })(mapExt.AttrOfOper, data, mapExt.FK_MapData);
}