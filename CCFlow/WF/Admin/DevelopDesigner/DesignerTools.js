//预览.
function PreviewForm() {
    //保存表单设计内容
    SaveForm();
    //清空MapData的缓存
    var en = new Entity("BP.Sys.MapData", pageParam.fk_mapdata);
    en.SetPKVal(pageParam.fk_mapdata);
    en.DoMethodReturnString("ClearCash");
    var frmID = GetQueryString("FK_MapData");
    var url = "../../CCForm/Frm.htm?FrmID=" + pageParam.fk_mapdata + "&FK_MapData=" + pageParam.fk_mapdata;
    window.open(url);
}

//另存为.
function SaveAs() {
    alert('在实现中..');
}


//导入表单模版.
function ImpFrmTemplate() {
    alert('在实现中..');
}

//导入现有字段.
function ImpFrmFields() {
    var frmID = GetQueryString("FK_MapData");
    var url = "Fields.html?FrmID=" + frmID;
    window.open(url);
    // alert('在实现中..');
}
