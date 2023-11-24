
/* 内置的Pop自动返回值.
   使用方法: 
1, 如果要使用此函数，需要在 \datauser\Xml\popval.xml 里面按照范例的要求格式配置。
2, 把此文件引入您的页面中去。
*/
function PopVal(ctrlID, ctrlID1, popNameInXML) {
    var ctrl = document.getElementById(ctrlID);
    if (ctrl == null)
        ctrl = document.getElementsByName(ctrlID);

    if (ctrl == null) {
        ctrl = document.getElementById(ctrlID1);
    }
    if (ctrl == null) {
        ctrl = document.getElementsByName(ctrlID1);
    }

    if (ctrl == null) {
        alert('ERR:' + ctrlID + '没有找到');
        return;
    }

    var url = '/WF/CCForm/FrmPopVal.aspx?FK_MapExt=' + popNameInXML + '&CtrlVal=' + ctrl.value;
    var v = window.showModalDialog(url, 'opp', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    if (v == null || v == '' || v == 'NaN') {
        return;
    }
    ctrl.value = v;
    return;
}

function WorkReturn(fk_flow, fk_node, workid, fid) {
    var url = "/WF/ReturnWorkSmall.aspx?FK_Node=" + fk_node + "&FID=" + fid + "&WorkID=" + workid + "&FK_Flow=" + fk_flow + "&s=2233";
    var v = window.showModalDialog(url, 'opp', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    return v;
}



