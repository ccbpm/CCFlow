/**
 *  数据视图组件. 属性在  MapAttrDataView 里面维护
 *  1. 配置SQL 查询语句，该语句里允许由变量比如：@FrmID, @WorkID, @WebUser.*等。
 *  2. 配置对应的列，比如 : @No=编号@Name=名称@Addr=地址
 *  3. 解析一个视图
 * */
function DataView(divID, dataViewID, frmPKVal) {

    //var nodeID = GetQueryString("FK_Node");
    //var workID = GetQueryString("WorkID");

    //创建组件.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCFormr_Components");
    handler.CopyURL();
    handler.AddPara("MyPKVal", frmPKVal);
    handler.AddPara("DataViewID", dataViewID);
    var data = handler.DoMethodReturnJSON("DataView_Init");

    var en = new Entity("BP.Sys.FrmUI.MapAttrDataView", dataViewID);

    var html = "<table>";
    html += "<tr>";
    for (var val in data) {
        html += "<td>" + val + "</td>";
    }
    html += "</tr>";

    for (var i = 0; i < data.length; i++) {
        var da = data[i];

        html += "<tr>";
        for (var val in data) {
            html += "<td>" + da[val] + "</td>";
        }
        html += "</tr>";
    }
    html += "</table>";

    $("#" + divID).html(html);

    //开始编写你的业务逻辑，实现公文的处理.
    // div.html("<a>打开公文</a>");
}
