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
   /* var val = window.confirm('您确定要格式化吗？');
    if (val == false)
        return;
    debugger
    //首先执行保存.
    SaveForm();
    var rels = /style="[^=>]*"([(\s+\w+=)|>])/g
    var ssss = /style\s*?=\s*?([‘"])[\s\S]*?\1/
    var dsd = /style=\"(.*?)\"/g
    var newHtml = ''

    newHtml = formeditor.replace(rels, '');
    newHtml = newHtml.replace(ssss, '');
    newHtml = newHtml.replace(dsd, '');
    //执行保存.
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_DevelopDesigner");
    handler.AddPara("FK_MapData", pageParam.fk_mapdata);
    handler.AddPara("HtmlCode", encodeURIComponent(newHtml));
    var data = handler.DoMethodReturnString("SaveForm");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }

    leipiEditor.getContent(newHtml);
    leipiEditor.sync();       //同步内容*/
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
    var url = "../MobileFrmDesigner/Default.htm?FK_Flow=" + flowNo + "&FK_Node=" + nodeid + "&FK_MapData=" + frmID;
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




