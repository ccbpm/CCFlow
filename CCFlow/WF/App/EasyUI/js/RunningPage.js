//撤销发送
function UnSend(fkFlow, workId) {
    $.messager.confirm('提示', '您确定要撤销本次发送吗？', function (yes) {
        if (yes) {
            Application.data.unSend(fkFlow, workId, function (js) {
                if (js) {
                    //                    var msg = eval('(' + js + ')');
                    //$.ligerDialog.alert(msg.message);
                    $.messager.alert('提示', js);
                    LoadGrid();
                }
            });
        }
    });
}
//催办
function Press(url) {
    window.showModalDialog(url, 'sd', 'dialogHeight: 200px; dialogWidth: 400px;center: yes; help: no;');
}
function winOpen(url) {
    // window.showModalDialog(url, '_blank', 'dialogHeight: 500px; dialogWidth: 850px;center: yes; help: no;scroll:no');
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');

}

//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").datagrid({
            title: '催办列表',
            height: "auto",
            width: "auto",
            nowrap: true,
            fitColumns: true,
            singleSelect: true,
            autoRowHeight: false,
            striped: true,
            idField: 'Title',
            collapsible: false,
            data: pushData,
            pagination: true,
            rownumbers: true,
            columns: [[
                   { title: '标题', field: 'TITLE', width: 340, align: 'left', formatter: function (value, rec) {

                       var h = "../../WFRpt.aspx?WorkID=" + rec.WORKID + "&FK_Flow=" + rec.FK_FLOW + "&FID=" + rec.FID + "&T=" + dateNow;
                       return "<a href='javascript:void(0);' onclick=winOpen('" + h + "')><img align='middle' border=0 width='20' height='20' src='Img/Menu/Runing.png'/>" + rec.TITLE + "</a>";

                   }
                   },
                   { title: '当前节点', field: 'NODENAME' },
                   { title: '发起人', field: 'STARTERNAME' },
                   { title: '发起日期', field: 'RDT' },
                   { title: '操作', field: 'OPT', width: 200,
                       formatter: function (value, rec) {

                           var h2 = "../../WorkOpt/Press.htm?FID=" + rec.FID + '&WorkID=' + rec.WORKID + '&FK_Flow=' + rec.FK_FLOW + "&T=" + dateNow;

                           return "<a href='javascript:void(0);' onclick=UnSend('" + rec.FK_FLOW + "','" + rec.WORKID + "') ><img align='middle' width='20' height='20' src='../../Img/Action/UnSend.png' border=0 />撤消发送</a>&nbsp;&nbsp;&nbsp;<a href='javascript:void(0);' onclick=Press('" + h2 + "')><img width='20' height='20' align='middle' src='../../Img/Action/Press.png' border=0 />催办</a>";

                       }
                   }]]
        });
        var p = $('#maingrid').datagrid('getPager');
        $(p).pagination({
            pageSize: 20,
            pageList: [20, 50, 100],
            beforePageText: '第',
            afterPageText: '页    共 {pages} 页',
            displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
        });
        $('#maingrid').datagrid('unselectAll');
    }
    else {
        $.messager.alert('提示', '<p class="warn">加载失败！ </p>');
    }
    $("#pageloading").hide();
}
//加载在途列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.getRunning(callBack, this);

}
var dateNow = "";
$(function () {
    dateNow = "";
    var date = new Date();
    dateNow += date.getFullYear(); //年
    dateNow += date.getMonth() + 1; //月 月比实际月份要少1
    dateNow += date.getDate(); //日
    dateNow += date.getHours(); //HH
    dateNow += date.getMinutes(); //MM
    dateNow += date.getSeconds(); //SS
    LoadGrid();
});