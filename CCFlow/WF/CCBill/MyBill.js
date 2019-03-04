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
