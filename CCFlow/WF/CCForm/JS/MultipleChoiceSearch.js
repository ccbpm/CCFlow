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

	var tip = undefined;
	try {
		tip = GetAtPara(mapExt.AtPara, "SearchTip");
	} catch (e) {
	}
	
	var _sql;
	var _url;
	var _json;
	switch (mapExt.DBType) {
		case 0:
			_sql = mapExt.Doc;
			break;
		case 1:
			_url = mapExt.Doc;
			break;
		case 2:
			_json = mapExt.Doc;
	}

    (function (FK_MapData, AttrOfOper, oid, tip, _sql, _url, _json) {
        var mselector = $("#" + AttrOfOper + "_mselector");
        mselector.mselector({
            "fit": true,
            "filter": false,
			"tip" : tip,
            "sql": _sql,
			"url" : _url,
			"data" : _json,
            "onSelect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getText"));
				msSaveVal(FK_MapData, AttrOfOper, oid, record.No, record.Name);
            },
            "onUnselect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getText"));
                msDelete(AttrOfOper, oid, record.No);
            }
        });
		// init
		var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
		frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", AttrOfOper, "RefPKVal", oid);
		var initJsonData = [];
		$.each(frmEleDBs, function (i, o) {
			initJsonData.push({
				"No" : o.Tag1,
				"Name" : o.Tag2
			});
		});
		mselector.mselector("loadData", initJsonData);
		//
    })(mapExt.FK_MapData, mapExt.AttrOfOper, (pageData.WorkID || pageData.OID || ""), tip, _sql, _url, _json);
}

//删除数据.
function msDelete(keyOfEn, oid, val) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.Delete();
}

//设置值.
function msSaveVal(fk_mapdata, keyOfEn, oid, val1, val2) {
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