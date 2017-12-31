/*大范围，查询模式的多选. */
function MultipleChoiceSearch(mapExt) {
    if (mapExt.DoWay == 0)
        return;

    (function (AttrOfOper, sql) {
        var mselector = $("#" + AttrOfOper + "_mselector");
        var hiddenField = $('<input type="hidden" />');
        hiddenField.attr("id", "TB_" + AttrOfOper);
        hiddenField.attr("name", "TB_" + AttrOfOper);
        mselector.after(hiddenField);
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