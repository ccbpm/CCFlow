/*大范围，查询模式的多选. */
function MultipleChoiceSearch(mapExt, mapAttr, tbID, rowIndex, OID) {

    mapExt = new Entity("BP.Sys.MapExt", mapExt);

    if (tbID == null || tbID == undefined) {
        tbID = "TB_" + mapExt.AttrOfOper;
    }
    var tb = $("#" + tbID);
	var width = tb.width();
	var height = tb.height();
	tb.hide();

	var container = $("<div></div>");
	tb.before(container);
    container.attr("id", tbID.replace("TB_","") + "_mselector");
	container.width(width);
	container.height(height);

	var tip = mapExt.GetPara("SearchTip"); //   undefined;

	var dbSrc = mapExt.Doc;
     

    (function (FK_MapData, AttrOfOper, oid, tip, dbSrc, tbID) {
        var objID = tbID.replace("TB_", "");
        var mselector = $("#" + objID + "_mselector");
        mselector.mselector({
            "fit": true,
            "filter": false,
			"tip" : tip,
			"dbSrc": dbSrc,
            "onSelect": function (record) {
                $("#TB_" + objID).val(mselector.mselector("getText"));
				msSavcceval(FK_MapData, AttrOfOper, oid, record.No, record.Name);
            },
            "onUnselect": function (record) {
                $("#TB_" + objID).val(mselector.mselector("getText"));
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
    })(mapExt.FK_MapData, mapExt.AttrOfOper, (pageData.WorkID || pageData.OID || ""), tip, dbSrc, tbID);
}

//删除数据.
function msDelete(keyOfEn, oid, val) {
    var frmEleDB = new Entity("BP.Sys.FrmEleDB");
    frmEleDB.MyPK = keyOfEn + "_" + oid + "_" + val;
    frmEleDB.Delete();
}

//设置值.
function msSavcceval(fk_mapdata, keyOfEn, oid, val1, val2) {
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

function parseOptions(target, properties) {
    var t = $(target);
    var options = {};

    var s = $.trim(t.attr('data-options'));
    if (s) {
        if (s.substring(0, 1) != '{') {
            s = '{ ' + s + ' } ';
        }
        options = (new Function('return ' + s))();
    }
    $.map(['width', 'height', 'left', 'top', 'minWidth', 'maxWidth', 'minHeight', 'maxHeight'], function (p) {
        var pv = $.trim(target.style[p] || '');
        if (pv) {
            if (pv.indexOf('%') == -1) {
                pv = parseInt(pv) || undefined;
            }
            options[p] = pv;
        }
    });

    if (properties) {
        var opts = {};
        for (var i = 0; i < properties.length; i++) {
            var pp = properties[i];
            if (typeof pp == 'string') {
                opts[pp] = t.attr(pp);
            } else {
                for (var name in pp) {
                    var type = pp[name];
                    if (type == 'boolean') {
                        opts[name] = t.attr(name) ? (t.attr(name) == 'true') : undefined;
                    } else if (type == 'number') {
                        opts[name] = t.attr(name) == '0' ? 0 : parseFloat(t.attr(name)) || undefined;
                    }
                }
            }
        }
        $.extend(options, opts);
    }
    return options;
}