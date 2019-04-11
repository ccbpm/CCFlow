//小范围的多选,不需要搜索.
function MultipleChoiceSmall(mapExt, mapAttr) {
    var webUser = new WebUser();
    var data = [];
    var valueField = "No";
    var textField = "Name";
    switch (mapExt.DoWay) {
        case 1:
            var tag1 = mapExt.Tag1;
            tag1 = tag1.replace(/;/g, ',');


            $.each(tag1.split(","), function (i, o) {
                data.push({ No: i, Name: o })
            });
            break;
        case 2:
            valueField = "IntKey"
            textField = "Lab";
            var enums = new Entities("BP.Sys.SysEnums");
            enums.Retrieve("EnumKey", mapExt.Tag2);
            $.each(enums, function (i, o) {
                data.push({ No: o.EnumKey, Name: o.Lab, IntKey: o.IntKey })
            });
            //data = enums;
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

        //如果是checkbox 多选.
        if (mapExt.Tag == "1" || mapExt.Tag == "2") {
            return MakeCheckBoxsModel(mapExt, data, mapAttr);
        }

        var tb = $("#TB_" + AttrOfOper);
        //tb.attr("visible", true); //把他隐藏起来.
        tb.css("visibility", "hidden");

        var cbx = $('<input type="text" />');
        cbx.attr("id", AttrOfOper + "_combobox");
        cbx.attr("name", AttrOfOper + "_combobox");
        tb.before(cbx);
        var w = tb.outerWidth() - 120;
        var h = tb.outerHeight() + 5;
        cbx.attr("class", "easyui-combobox");
        cbx.css("width", w);
        cbx.css("height", h);

        cbx.combobox({
            "editable": false,
            "valueField": valueField,
            "textField": textField,

            "multiple": true,
            "onSelect": function (p) {
                $("#TB_" + AttrOfOper).val(cbx.combobox("getValues"));
                //保存选择的值.
                SaveVal(FK_MapData, AttrOfOper, p.No, p.Name);

                //                (function sel(n, KeyOfEn, FK_MapData) {
                //                    //保存选择的值.
                //                    SaveVal(FK_MapData, KeyOfEn, n);

                //                })(p[valueField], AttrOfOper, FK_MapData);
            },
            "onUnselect": function (p) {
                $("#TB_" + AttrOfOper).val(cbx.combobox("getValues"));
                (function unsel(n, KeyOfEn) {

                    //删除选择的值.
                    Delete(KeyOfEn, n);

                })(p[valueField], AttrOfOper);
            }
        });


        cbx.combobox("loadData", data);
        $(".textbox-text").css("width", "100%");

        if (mapAttr != null && mapAttr.UIIsEnable != 1) {
            cbx.combobox('disable');
        }
        var tbVal = tb.val();
        if (tbVal != "") {
            var tbVals = tbVal.split(',');
            for (var index = 0; index < tbVals.length; index++) {
                cbx.combobox('select', tbVals[index]);
            }
        }


    })(mapExt.AttrOfOper, data, mapExt.FK_MapData);
}


//checkbox 模式.
function MakeCheckBoxsModel(mapExt, data, mapAttr) {
    var textboxId = "TB_" + mapExt.AttrOfOper
    var textbox = $("#" + textboxId);
    textbox.css("visibility", "hidden");
    var tbVal = textbox.val();
    if (tbVal == null) tbVal = "";
    for (var i = 0; i < data.length; i++) {

        var en = data[i];

        var eleHtml = "";
        var name;
        var id;
        var keyValue;
        if (mapExt.DoWay == 2) {
            name = "CB_" + mapExt.AttrOfOper + "_" + en.No;
            id = name + "_" + en.IntKey
            keyValue = en.IntKey;
        } else {
            name = "CB_" + mapExt.AttrOfOper + "_" + mapExt.AttrOfOper;
            id = name + "_" + en.No;
            keyValue = en.No;
        }
        var enableAttr = '';
        if (mapAttr != null && mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        var cb = $("<input " + enableAttr + " type='checkbox' id='" + id + "' name='" + name + "' value='" + keyValue + "'onclick='changeValue(\"" + textboxId + "\",\"" + name + "\")'  />");


        if (tbVal.indexOf(keyValue + ',') != -1)
            cb.attr("checked", true);
        else
            cb.attr("checked", false);

        //开始绑定事件.

        //end 绑定checkbox事件. @解相宇 绑定取消选择事件.

        textbox.before(cb);

        if (mapExt.Tag == "1")
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "</label>");
        else
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "</label><br>");

        textbox.before(lab);
    }

}

function changeValue(changeIdV, getNameV) {
    var strgetSelectValue = "";
    var getSelectValueMenbers = $("input[name='" + getNameV + "']:checked").each(function (j) {

        if (j >= 0) {
            strgetSelectValue += $(this).val() + ",";
        }
    });

    $("#" + changeIdV).val(strgetSelectValue);

}

//删除数据.
function Delete(keyOfEn, val) {

    var oid = (pageData.WorkID || pageData.OID || "");
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.Delete();
}

//设置值.
function SaveVal(fk_mapdata, keyOfEn, val, name) {

    var oid = (pageData.WorkID || pageData.OID || "");

    var frmEleDB = new Entity("BP.Sys.FrmEleDB");

    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.FK_MapData = fk_mapdata;
    frmEleDB.EleID = keyOfEn;
    frmEleDB.RefPKVal = oid;
    frmEleDB.Tag1 = val;
    frmEleDB.Tag2 = name;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}

function getVals(fk_mapData, eleID, refPKVal) {
    var groupId = "";
    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", fk_mapData, "EleID", eleID, "RefPKVal", refPKVal);
    return frmEleDBs;

}

function DeptEmpModelAdv0(mapExt) {
    var target = $("#TB_" + mapExt.AttrOfOper);
    target.hide();

    var width = target.width();
    var height = target.height();
    var container = $("<div></div>");
    target.after(container);
    container.width(width);
    container.height(height);
    container.attr("id", mapExt.AttrOfOper + "_mtags");

    $("#" + mapExt.AttrOfOper + "_mtags").mtags({
        "fit": true,
        "onUnselect": function (record) {
            console.log("unselect: " + JSON.stringify(record));
        }
    });

    var width = mapExt.W;
    var height = mapExt.H;
    var iframeId = mapExt.MyPK + mapExt.FK_MapData;
    var title = GetAtPara(mapExt.AtPara, "Title");
    var oid = (pageData.WorkID || pageData.OID || "");

    var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
    frmEleDBs.Retrieve("FK_MapData", mapExt.FK_MapData, "EleID", mapExt.AttrOfOper, "RefPKVal", oid);
    var initJsonData = [];
    $.each(frmEleDBs, function (i, o) {
        initJsonData.push({
            "No": o.Tag1,
            "Name": o.Tag2
        });
    });
    $("#" + mapExt.AttrOfOper + "_mtags").mtags("loadData", initJsonData);
    //解项羽 这里需要相对路径.
    var url = "../CCForm/Pop/BranchesAndLeaf.htm?MyPK=" + mapExt.MyPK + "&oid=" + oid + "&m=" + Math.random();
    container.on("dblclick", function () {
        OpenEasyUiDialog(url, iframeId, title, width, height, undefined, true, function () {
            var iframe = document.getElementById(iframeId);
            if (iframe) {
                var selectedRows = iframe.contentWindow.selectedRows;
                if ($.isArray(selectedRows)) {
                    var mtags = $("#" + mapExt.AttrOfOper + "_mtags")
                    mtags.mtags("loadData", selectedRows);
                    $("#TB_" + mapExt.AttrOfOper).val(mtags.mtags("getText"));
                }
            }
            return true;
        });
    });
}
