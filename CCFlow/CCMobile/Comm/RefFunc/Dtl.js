/**
* 初始化按钮权限
**/
function InitToolbar(pageType) {
    var isHaveOper = false;
    var bottombar = $('#bottomToolBar');
    //权限控制.
    if (mapData.GetPara("IsInsert") == 1) {
        isHaveOper = true;
        bottombar.append("<a class='mui-tab-item' id='Btn_New' name='Btn_New' href='#' >新建</ a>");
    }

    if (mapData.GetPara("IsUpdate") == 1 && pageType!="dtlSearch") {
        isHaveOper = true;
        bottombar.append("<a class='mui-tab-item' id='Btn_Save' name='Btn_Save' href='#' >保存</ a>");
    }

    if (mapData.GetPara("IsExp") == 1) {
        isHaveOper = true;
        bottombar.append("<a class='mui-tab-item' id='Btn_Exp' name='Btn_Exp' href='#' >导出</ a>");
    }
    if (isHaveOper == false)
        $("#bottomToolBar").hide();

    $("#Btn_New").on("tap", function () {
        NewEn();

    });
    $("#Btn_Save").on("tap", function () {
        SaveEns();

    });
    //导入
    $("#Btn_Imp").on("tap", function () {
        ImpEns();

    });
    //导出
    $("#Btn_Exp").on("tap", function () {
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CommEntity");
        handler.AddPara("EnsName", GetQueryString("EnsName"));
        handler.AddPara("RefKey", GetQueryString("RefKey"));
        handler.AddPara("RefVal", GetQueryString("RefVal"));
        //查询集合
        var data = handler.DoMethodReturnString("Dtl_Exp");
        var url = "";
        if (data.indexOf('err@') == 0) {
            alert(data);
        }
        data = basePath + "/" + data;

        window.open(data);

    });
}
/**
 * 数据显示
 * @param {any} mapAttrs
 * @param {any} dtl
 */
function ShowDtlByMapAttr(mapAttrs, dtl) {
    var lab = "";
    var val = "";
    _html = "";
    $.each(mapAttrs, function (idx, mapAttr) {
        if (mapAttr.UIVisible == 0
            || mapAttr.KeyOfEn == "OID"
            || mapAttr.KeyOfEn == "WorkID"
            || mapAttr.KeyOfEn == "NodeID"
            || mapAttr.KeyOfEn == "MyNum"
            || mapAttr.KeyOfEn == "MyPK") {
            return true;
        }
        lab = mapAttr.Name;
        val = dtl[mapAttr.KeyOfEn]||"";
        if (mapAttr.UIContralType == 1) {
            val = dtl[mapAttr.KeyOfEn + "T"] || "";
            if (val == "")
                val = dtl[mapAttr.KeyOfEn + "Text"] || "";
        }
        if (mapAttr.UIContralType == 2) {
            if (val == "0") val = "否";
            if (val == "1") val = "是";
        }
        if (mapAttr.MyDataType == "6") {
            if (val != "")
                val = FormatDate(new Date(val), "yyyy-MM-dd");
        }
        if (mapAttr.IsRichText == "1") {
            _html += '<div class="mui-input-row">';
            _html += '<label><p>' + lab + '</p></label>';
            _html += '</div >';
            _html += '<div class="mui-input-row">';
            _html += '<textarea id="textarea" rows="5" readonly="readonly"  disabled="disabled">'+val+'</textarea>';
            _html += '</div >';
            return true;
        }
        _html += '<div class="mui-input-row">';
        _html += '<label><p>' + lab + '</p></label>';
        _html += '<input style="background-color:#fff " value="' + val + '" readonly="readonly" type="text"  disabled="disabled"/>';
        _html += '</div >';
    })
    return _html;
}