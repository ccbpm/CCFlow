
function PrintHtml() {
    alert('在开发中..');
    return;
}

/**
 * 一个表单里仅仅有一个RTF组件.
 * */
function PrintRTF() {

    var frmID = GetQueryString("FK_MapData");
    if (frmID == null)
        frmID = GetQueryString("FrmID");

    var en = new Entity("BP.Sys.MapAttr");
    en.SetPKVal(frmID + "_RTF");
    if (en.RetrieveFromDBSources() == 1) {
        alert("该表单 RTF 字段已经存在。");
        return;
    }

    //alert('将要生成rtf打印组件，您需要刷新页面才');

    var mypk = frmID+ "_RTF";
    var mapAttr = new Entity("BP.Sys.MapAttr");
    mapAttr.UIContralType = 111; //模板打印.
    mapAttr.MyPK = mypk;
    mapAttr.FK_MapData = frmID;
    mapAttr.KeyOfEn = "RTF";
    mapAttr.Name = "Rtf模板打印";
    mapAttr.MyDataType = 1;
    mapAttr.LGType = 0;
    mapAttr.ColSpan = 1; // 
    mapAttr.UIWidth = 150;
    mapAttr.UIHeight = 23;
    mapAttr.Insert(); //插入字段.
    mapAttr.Retrieve();
    var url = "../../Comm/En.htm?EnName=BP.Sys.FrmUI.MapAttrPrint&MyPK=" + mapAttr.MyPK;
    OpenLayuiDialog(url, 'Rtf模板打印', innerWidth / 2, 0, "r", false, false, false, null, function () {

        var _Html = "<input type='button' value='" + mapAttr.Name + "'  id='Btn_RTF' name='Btn_RTF' data-key='RTF' data-name='rtf模板打印' data-type='RTF'   leipiplugins='component' style='width:98%'  placeholder='rtf模板打印'/>";
        leipiEditor.execCommand('insertHtml', _Html);
        var url = GetHrefUrl();
        if (url.indexOf('FoolFormDesigner') > 0)
        {
            Reload();
        }
    });
}