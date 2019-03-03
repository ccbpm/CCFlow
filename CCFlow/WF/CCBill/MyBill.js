//初始化函数
$(function () {

    //webUser = new WebUser();


});


function New() {
    alert('尚未完成.');
}

function DeleteIt() {
    if (window.confirm('您确定要删除吗？') == false)
        return;
}

function Search() {
    var url = "Search.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function Group() {
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}


function RefBill() {
    alert('尚未完成.');
}


function StartFlow() {
    alert('尚未完成.');
}
