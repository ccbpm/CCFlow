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

    (function (AttrOfOper, sql) {
        var mselector = $("#" + AttrOfOper + "_mselector");
        mselector.mselector({
            "fit": true,
            "filter": false,
            "sql": sql,
            "onSelect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getValue"));
                //mssel(record);
            },
            "onUnselect": function (record) {
                $("#TB_" + AttrOfOper).val(mselector.mselector("getValue"));
                //msunsel(record);
            }
        });
    })(mapExt.AttrOfOper, mapExt.Tag1);
}