//初始化函数
$(function () {

    //webUser = new WebUser();

});


function Search() {
    var url = "Search.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function Group() {
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function DraftBox() {
    var url = "Draft.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function RefBill() {
    alert('尚未完成.');
}


function StartFlow() {
    alert('尚未完成.');
}

function PrintHtml() {
    alert('尚未完成-PrintHtml');
    return;
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function PrintPDF() {
    alert('尚未完成-PrintPDF');
    return;
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function PrintRTF() {
    alert('尚未完成 - PrintRTF');
    return;
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function PrintCCWord() {
    alert('尚未完成 - PrintCCWord');
    return;
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function ExpToZip() {
    alert('尚未完成 - ExpToZip');
    return;
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}
