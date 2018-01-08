/*大范围，查询模式的多选. */
function MultipleChoiceSearch(mapExt) {

    if (mapExt.DoWay == 0)
        return;

	var tb = $("#TB_" + mapExt.AttrOfOper);
	var width = tb.width();
	var height = tb.height();
	tb.hide();

	var container = $("<div></div>");
	tb.before(container);
	container.attr("id", mapExt.AttrOfOper + "_mselector");
	container.width(width);
	container.height(height);

    (function (FK_MapData, AttrOfOper, sql) {
        var mselector = $("#" + AttrOfOper + "_mselector");
        mselector.mselector({
            "fit": true,
            "filter": false,
            "sql": sql,
            "onSelect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getValue"));
				msSaveVal(FK_MapData, AttrOfOper, record.No, record.Name);
            },
            "onUnselect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getValue"));
                msDelete(AttrOfOper, record.No);
            }
        });
		//mselector.mselector("loadData", [{ No : "admin", Name : "admin"}]);
    })(mapExt.FK_MapData, mapExt.AttrOfOper, mapExt.Tag1);
}

//删除数据.
function msDelete(keyOfEn, val) {
    var oid = (pageData.WorkID || pageData.OID || "");
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.Delete();
}

//设置值.
function msSaveVal(fk_mapdata, keyOfEn, val1, val2) {
    var oid = (pageData.WorkID || pageData.OID || "");
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val1;
    frmEleDB.FK_MapData = fk_mapdata;
    frmEleDB.EleID = keyOfEn;
    frmEleDB.RefPKVal = oid;
    frmEleDB.Tag1 = val1;
	frmEleDB.Tag2 = val2;
    if (frmEleDB.Update() == 0) {
        frmEleDB.Insert();
    }
}