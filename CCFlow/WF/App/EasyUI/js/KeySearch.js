
function OpenIt(url) {
    window.open(url, 'card', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
}

//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").ligerGrid({
            columns: [
                   { display: '标题', name: 'Title', width: 380, align: 'left', render: function (rowdata, rowindex) {
                       var h = "../../WFRpt.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FlowEndNode+"&T="+dateNow;
                       return "<a href='javascript:void(0);' onclick=OpenIt('" + h + "') >" + rowdata.Title + "</a>";
                   }
                   },
                     { display: '发起人', name: 'FlowStarter' },
                     { display: '发起日期', name: 'FlowStartRDT' },
                       { display: '状态', name: 'WFState', render: function (rowdata, rowindex) {
                           if (rowdata.WFState == "0") {
                               return "未完成";
                           }
                           else if (rowdata.WFState == "1") {
                               return "已完成";
                           }
                           else {
                               return "未知";
                           }
                       }
                       },
                    { display: '参与人', name: 'FlowEmps',width:300 }
                   ],
            pageSize: 20,
            data: pushData,
            rownumbers: true,
            height: "99%",
            width: "99%",
            columnWidth: 120,
            groupColumnName: 'FlowName',
            groupColumnDisplay: '流程类型',
            onDblClickRow: function (rowdata, rowindex) {
                OpenIt("../../WFRpt.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FlowEndNode+"&T="+dateNow);
            }
        });
        $("#pageloading").hide();
    }
    else {
        $.ligerDialog.warn('加载数据出错，请关闭后重试！');
    }
}
//加载数据
function LoadGrid() {
    $("#pageloading").show();
    Application.data.keySearch(false, 9, 'all', callBack, this);
}
//工具菜单事件
function itemclick(item) {
    if (item.text == "按工作ID查") {
        queryType = "workid";
    } else if (item.text == "按流程标题字段关键字查") {
        queryType = "title";
    }
    else if (item.text == "全部字段关键字查") {
        queryType = "all";
    }
    $.ligerDialog.open({
        target: $("#showKey"),
        title: '输入查询内容',
        width: 380,
        height: 100,
        isResize: true,
        modal: true,
        buttons: [{ text: '确定', onclick: function (i, d) {
            ckbOwner = $("#cbkQueryType");
            ckbOwner = ckbOwner[0].checked;
            contentKey = $("#txtKey").val();
            $("#pageloading").show();
            Application.data.keySearch(ckbOwner, contentKey, queryType, callBack, this);
            d.hide();
        }
        }, { text: '关闭', onclick: function (i, d) {
            d.hide();
        }
        }]
    });
}
//变量
var ckbOwner = false;
var contentKey = "";
var queryType = "all";
var dateNow = "";
//初始
$(function () {
    dateNow = "";
    var date = new Date();
    dateNow += date.getFullYear(); //年
    dateNow += date.getMonth() + 1; //月 月比实际月份要少1
    dateNow += date.getDate(); //日
    dateNow += date.getHours(); //HH
    dateNow += date.getMinutes(); //MM
    dateNow += date.getSeconds(); //SS
    //工具事件
    var toolBarManager = $("#toptoolbar").ligerToolBar({ items: [
                { text: '按工作ID查', click: itemclick, icon: 'search2' },
                { line: true },
                { text: '按流程标题字段关键字查', click: itemclick, icon: 'search2' }
//                { line: true },
//                { text: '全部字段关键字查', click: itemclick, icon: 'search2' }
            ]
    });
    LoadGrid();
});