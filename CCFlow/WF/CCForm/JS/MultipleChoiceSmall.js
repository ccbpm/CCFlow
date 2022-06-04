//小范围的多选,不需要搜索.
function MultipleChoiceSmall(mapExt, mapAttr, frmData, tbID, rowIndex, OID) {
    if (tbID == null || tbID == undefined) {
        tbID = "TB_" + mapExt.AttrOfOper;
    }
    var cbxID = mapExt.AttrOfOper + "_combobox";
    if (rowIndex != null && rowIndex != undefined)
        cbxID = mapExt.AttrOfOper + "_combobox_" + rowIndex;
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
        case 2: //枚举.
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
        case 3: //外键表.
            if (frmData != null && frmData != undefined) {

                data = frmData[mapExt.Tag3];
                if (data == undefined) {
                    var en = new Entity("BP.Sys.SFTable", mapExt.Tag3);
                    data = en.DoMethodReturnJSON("GenerDataOfJson");
                    if (data.length > 400)
                    {
                        alert("数据量太大，请检查配置是否有逻辑问题，或者您可以使用搜索选择或者pop弹出窗选择:" + mapExt.Tag3);
                        return;
                    }

                    frmData[mapExt.Tag3] = data;
                }
            } else {
                var en = new Entity("BP.Sys.SFTable", mapExt.Tag3);
                data = en.DoMethodReturnJSON("GenerDataOfJson");

                if (data.length > 400) {
                    alert("数据量太大，请检查配置是否有逻辑问题，或者您可以使用搜索选择或者pop弹出窗选择:" + mapExt.Tag3);
                    return;
                }
            }
            break;
        case 4:
            var tag4SQL = mapExt.Tag4;

            tag4SQL = DealExp(tag4SQL, webUser);
            if (tag4SQL.indexOf('@') == 0) {
                alert('约定的变量错误:' + tag4SQL + ", 没有替换下来.");
                return;
            }
            tag4SQL = tag4SQL.replace(/~/g, "'");

            data = DBAccess.RunSQLReturnTable(tag4SQL);
            if (data.length > 400) {
                alert("数据量太大，请检查配置是否有逻辑问题，或者您可以使用搜索选择或者pop弹出窗选择:" + mapExt.Tag3);
                return;
            }
            break;
        default:
            alert("未判断的模式");
            break;
    }

    (function (AttrOfOper, data, FK_MapData) {

        //如果是checkbox 多选.
        if (mapExt.Tag == "1" || mapExt.Tag == "2") {
            return MakeCheckBoxsModel(mapExt, data, mapAttr, tbID);
        }

        var tb = $("#" + tbID);
        var w = "100%"; //tb.outerWidth();
        var h = tb.outerHeight();
        tb.hide();

        var cbx = $('<input type="text" />');
        cbx.attr("id", cbxID);
        cbx.attr("name", AttrOfOper + "_combobox");
        tb.before(cbx);

        cbx.attr("class", "easyui-combobox");
        cbx.css("width", w);
        cbx.css("height", h);

        cbx.combobox({
            "editable": false,
            "valueField": valueField,
            "textField": textField,

            "multiple": true,
            "onSelect": function (p) {
                $("#" + tbID).val(cbx.combobox("getValues"));
                $("#" + tbTextID).val(cbx.combobox("getText"));
                //保存选择的值.
            },
            "onUnselect": function (p) {
                $("#" + tbID).val(cbx.combobox("getValues"));
                (function unsel(n, KeyOfEn) {

                    //删除选择的值.
                    Delete(KeyOfEn, n, OID);

                })(p[valueField], AttrOfOper);
            }
        });


        cbx.combobox("loadData", data);
        $(".textbox-text").css("width", w);
        $(".easyui-fluid").css("width", w);

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
function MakeCheckBoxsModel(mapExt, data, mapAttr, tbID) {
    var textbox = $("#" + tbID);
    textbox.hide();
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

        var cb = $("<input " + enableAttr + " type='checkbox' id='" + id + "' name='" + name + "' value='" + keyValue + "'onclick='changeValue(\"" + tbID + "\",\"" + name + "\")'  />");


        if (tbVal.indexOf(keyValue + ',') != -1)
            cb.attr("checked", true);
        else
            cb.attr("checked", false);

        //开始绑定事件.

        //end 绑定checkbox事件. @解相宇 绑定取消选择事件.

        textbox.before(cb);

        if (mapExt.Tag == "1")
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "&nbsp;&nbsp;</label>");
        else
            var lab = $("<label class='labRb align_cbl' for='" + id + "'>&nbsp;" + en.Name + "&nbsp;&nbsp;</label><br>");

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
function Delete(keyOfEn, val, oid) {
    if (oid == null || oid == undefined)
        oid = (pageData.WorkID || pageData.OID || "");
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.Delete();
}

//设置值.
function Savcceval(fk_mapdata, keyOfEn, val, name, oid) {

    if (oid == null || oid == undefined) {
        if (GetQueryString("WorkID") == null || GetQueryString("WorkID") == undefined)
            oid = GetQueryString("OID");
        else
            oid = GetQueryString("WorkID");
    }

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


function MultipleInputSearch(mapExt, defaultVal, tbID,oid) {
    if (tbID == null || tbID == undefined) {
        tbID = "TB_" + mapExt.AttrOfOper;
    }
    var tb = $("#" + tbID);
    var width = tb.width();
    var height = tb.height();
    tb.hide();
    if (oid == null || oid == undefined)
        oid = GetPKVal();
    //获取当前元素是否在P标签内
    var parent;
    var container;
    if (tb.parent().length == 1 && tb.parent()[0].tagName.toUpperCase() == "P") {
        parent = tb.parent()[0];
        var ptext = $(parent).text();
        if (ptext.indexOf(tb.attr("data-name")) != -1) {
            var _html = $(parent).html();
            $(parent).html(_html.replace(ptext, ""));
            tb = $("#" + tbID);
            tb.before($("<div style='float:left'>" + ptext + "</div>"));

            container = $("<div style='float:left'></div>");
            tb.before(container);
            tb.before($("<div style='clear:both'></div>"));

        } else {
            container = $("<div></div>");
            tb.before(container);
        }


    } else {
        container = $("<div></div>");
        tb.before(container);
    }

    container.attr("id", mapExt.AttrOfOper + "_comboTree");

    container.addClass("select-tree-wrap");

    var dbSrc = mapExt.Doc; //搜索数据源
    //处理sql，url参数.
    dbSrc = dbSrc.replace(/~/g, "'");
    dbSrc = DealExp(dbSrc);

    var listSrc = mapExt.Tag1;//列表数据源
    listSrc = listSrc.replace(/~/g, "'");
    listSrc = DealExp(listSrc);

    var isShowSignature = mapExt.Tag == "1" ? true : false;
    var valArray = [];
    var frmEleDBs = getVals(mapExt.FK_MapData,mapExt.AttrOfOper,oid);
    $.each(frmEleDBs,function(i,item){
        //valArray.push(item.Tag2);
        valArray.push({No:item.Tag1,Name:item.Tag2});
    });
   /* if (defaultVal != null && defaultVal != undefined) {
        defaultVal = defaultVal.replace(new RegExp("[[]", "gm"), "").replace(/]/g, ",");
        defaultVal = defaultVal.substr(0, defaultVal.length - 1);
        valArray = defaultVal.split(",");
    }*/

    $('#' + mapExt.AttrOfOper + "_comboTree").comboTree({
        source: dbSrc,
        listSource: listSrc,
        isMultiple: true,
        isFirstClassSelectable: false, //第一级是否可选
        cascadeSelect: true,
        selectedlength: 30,//最多可选
        keyOfEn: mapExt.AttrOfOper,
        selected: valArray,
        isShowSignature: isShowSignature,
        refPK:oid,
        FK_MapData:mapExt.FK_MapData
    });
}
