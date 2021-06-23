function Init() {

    var workID = GetQueryString("WorkID");
    var frmID = GetQueryString("FrmID");

    //查询出来数据.
    var ens = new Entities("BP.CCBill.FrmBBSs");
    ens.Retrieve("WorkID", workID, "RDT");

    for (var i = 0; i < ens.length; i++) {

        var en = ens[i];
        var rdt = en.RDT;
        var name = en.Name;
        var parnt = en.ParentNo;
    }

    //给头部赋值.
    var dictEn = new Entity(frmID, workID);
    $("#TB_No").html(dictEn.BillNo);
    $("#TB_Name").html(dictEn.Title);
}

function Save() {

    var input = $("#reply-input")
    var en = new Entity("BP.CCBill.FrmBBS");
    en.Name = input.val();
    en.WorkID = GetQueryString("WorkID");
    en.FrmID = GetQueryString("FrmID");
    en.Insert();

    input.val('');
    layer.msg("回复成功");

    // var hanler = new HttpHandler();
    //hanler.AddFileData();

}
///答复.
function SaveAsReply(parentNo) {

    var en = new Entity("BP.CCBill.FrmBBS");
    en.Name = $("TB_Doc").val();
    en.ParentNo = parentNo;
    en.WorkID = GetQueryString("WorkID");
    en.FrmID = GetQueryString("FrmID");
    en.Insert();

}

function Delete(id) {
    var en = new Entity("BP.CCBill.FrmBBS", id);
    en.Delete();
}

function replayItem() {
    $('#reply-tips').html("回复给：周朋");
}

(function getCurrentDate() {
    Init()
    $("#r-date").html(new Date().toLocaleDateString())
})();
