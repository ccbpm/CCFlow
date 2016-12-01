var querySta = "unread";
var dateNow = "";
//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").ligerGrid({
            columns: [
                   { display: '标题', name: 'TITLE', width: 380, align: 'left', render: function (rowdata, rowindex) {
                       var title = "<a href=javascript:WinOpenIt('" + rowdata.MYPK + "','" + rowdata.FK_FLOW + "','" + rowdata.FK_NODE
                       + "','" + rowdata.WORKID + "','" + rowdata.FID + "','" + rowdata.STA + "');><img align=middle src='Img/Menu/CCSta/" + rowdata.STA
                       + ".png' id=" + rowdata.MYPK + ">" + rowdata.TITLE + "</a>日期:" + rowdata.RDT;
                       return title;
                   }
                   },
                   { display: '流程名称', name: 'FLOWNAME' },
                   { display: '当前节点', name: 'NODENAME' },
                   { display: '内容', name: 'DOC', width: 200 },
                   { display: '抄送人', name: 'REC' },
                   { display: '抄送日期', name: 'RDT' },
                   { display: '读取日期', name: 'CDT' }
                   ],
            pageSize: 20,
            data: pushData,
            rownumbers: true,
            height: "99%",
            width: "99%",
            columnWidth: 100,
            onReload: LoadGrid,
            onDblClickRow: function (rowdata, rowindex) {
                WinOpenIt(rowdata.MyPK, rowdata.FK_Flow, rowdata.FK_Node, rowdata.WorkID, rowdata.FID, rowdata.Sta);
            }
        });
    }
    else {
        $.ligerDialog.warn('加载数据出错，请关闭后重试！');
    }
    $("#pageloading").hide();
}
//加载抄送列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.getCCFlowList(querySta, callBack, this);
}
//打开窗体
function WinOpenIt(ccid, fk_flow, fk_node, workid, fid, sta) {
    var url = '';
    if (sta == '0') {
        url = '../../Do.aspx?DoType=DoOpenCC&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid + '&Sta=' + sta + '&MyPK=' + ccid + "&T=" + dateNow;
    }
    else {
        url = '../../WorkOpt/OneWork/Track.aspx?FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid + '&Sta=' + sta + '&MyPK=' + ccid + "&T=" + dateNow;
    }
    //window.parent.f_addTab("cc" + fk_flow + workid, "抄送" + fk_flow + workid, url);
    var newWindow = window.open(url, 'z');
    newWindow.focus();
    if (querySta == "all" || querySta == "unread")

        LoadGrid();
}
//工具栏事件
function itemclick(item) {
    if (item.text == "全部") {
        querySta = "all";
    }
    else if (item.text == "未读") {
        querySta = "unread";
    }
    else if (item.text == "已读") {
        querySta = "isread";
    }
    else if (item.text == "已删除") {
        querySta = "delete";
    }
    LoadGrid();
}

$(function () {

    dateNow = "";
    var date = new Date();
    dateNow += date.getFullYear(); //年
    dateNow += date.getMonth() + 1; //月 月比实际月份要少1
    dateNow += date.getDate(); //日
    dateNow += date.getHours(); //HH
    dateNow += date.getMinutes(); //MM
    dateNow += date.getSeconds(); //SS

    var toolBarManager = $("#toptoolbar").ligerToolBar({ items: [
                { text: '全部', click: itemclick, icon: 'database' },
                { line: true },
                { text: '未读', click: itemclick, icon: 'outbox' },
                { line: true },
                { text: '已读', click: itemclick, icon: 'mailbox' },
                { line: true },
                { text: '已删除', click: itemclick, icon: 'attibutes' }
            ]
    });
    LoadGrid();
});