//预览.
function PreviewForm() {
    var frmID = GetQueryString("FK_MapData");
    var url = "FrmView.htm?FrmID=" + frmID;
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
