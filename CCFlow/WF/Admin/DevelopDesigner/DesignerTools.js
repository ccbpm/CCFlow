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

//移动表单
function FrmMobile() {
    var frmID = GetQueryString("FK_MapData");

    var frmID = GetQueryString("FK_MapData");
    var flowNo = GetQueryString("FK_Flow");
    var nodeID = GetQueryString("FK_Node");

    var url = "/Admin/AttrNode/SortingMapAttrs.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeid + "&FK_MapData=" + frmID;
    window.open(url);
}


//另存为.
function SaveAs() {
    alert('在实现中..');
}


//导入表单模版.
function ImpFrmTemplate() {

    var frmID = GetQueryString("FK_MapData");
    var flowNo = GetQueryString("FK_Flow");
    var nodeID = GetQueryString("FK_Node");

    var url = "../FoolFormDesigner/ImpExp/Imp.htm?FK_MapData=" + frmID + "&FrmID=" + frmID + "&DoType=FunList&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.open(url);
}


  

