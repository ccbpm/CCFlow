// 监听键盘按下事件
document.onkeydown = function () {

    //判断 Ctrl+S
    if (event.ctrlKey == true && event.keyCode == 83) {

        SaveForm();

        // alert('触发ctrl+s');
        event.preventDefault(); // 或者 return false;
    }
}

//格式化
function FormatHtml() {
    var val = window.confirm('您确定要格式化吗？');
    if (val == false)
        return;

    //首先执行保存.
    SaveForm();

    //数据库中查找
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
    htmlContent = handler.DoMethodReturnString("Designer_FormatHtml");

    //获得table.
    //  $("table");

    //var handler = new XMLHttpRequest("BP");

    // handler.DmDesigner_FormatHtml

    //获得该table的 tr 集合。 

    //执行删除tr 的内容.


    //获得table 的 td集合.

    //执行删除tr 的修饰内容.

    //执行保存.

}


//预览.
function PreviewForm() {
    //保存表单设计内容
    SaveForm();

    //  var en = new Entity(pageParam.fk_mapdata);
    // var da = en.DoMethodReturnString("CheckPhysicsTable");
    //alert(da);


    var frmID = GetQueryString("FK_MapData");
    var url = "../../CCForm/Frm.htm?FrmID=" + pageParam.fk_mapdata + "&FK_MapData=" + pageParam.fk_mapdata;
    window.open(url);
}

//表单属性.
function FrmAttr() {
    var frmID = GetQueryString("FK_MapData");
    //var mapdata = new Entity("BP.WF.Template.MapFrmFool", frmID);

    var url = "../../Comm/RefFunc/En.htm?EnName=BP.WF.Template.MapFrmFool&PKVal=" + frmID;
    window.open(url);

}

//表单属性.
function OpenFoolFrm() {

    var frmID = GetQueryString("FK_MapData");
    var nodeID = GetQueryString("NodeID");
    var flowNo = GetQueryString("FK_Flow");

    var url = "../FoolFormDesigner/Designer.htm?FK_MapData=" + frmID + "&FK_Flow=" + flowNo + "&FK_Node=" + nodeID;
    window.open(url);

}


//移动表单
function FrmMobile() {

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




