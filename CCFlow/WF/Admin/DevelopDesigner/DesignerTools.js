//预览.
function PreviewForm()
{
    alert('在实现中..');
}

//导入表单模版.
function ImpFrmTemplate() {
    alert('在实现中..');
}

//导入现有字段.
function ImpFrmFields() {

    var url = "Fields.html?FrmID=" + GetQueryString("FrmID");
    window.open(url);
   // alert('在实现中..');
}
 