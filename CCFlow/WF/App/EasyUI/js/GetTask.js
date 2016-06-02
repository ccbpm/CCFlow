function WinOpenIt(url,target) {
    window.showModalDialog(url,target , 'height=800px,width=950px,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
   
}
//打开流程图
function OpenFlowPicture(flowNo, flowName) {
    var pictureUrl = "../../WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + flowNo + "&DoType=Chart&T=" + strTimeKey;
    var win = $.ligerDialog.open({
        height: 500, width: 800, url: pictureUrl, showMax: true, isResize: true, modal: true, title: flowName + "流程图", slide: false, buttons: [{
            text: '关闭', onclick: function (item, Dialog, index) {
                win.hide();
            }
        }]
    });
}
//回调函数
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").ligerGrid({
            columns: [
                   { display: '流程名称', name: 'NAME', width: 420, align: 'left', render: function (rowdata, rowindex) {
                       var h = "../../GetTask.aspx?FK_Flow=" + rowdata.NO + "&FK_Node=" + parseInt(rowdata.NO) + "01&T=" + strTimeKey;
                       return "<a href='javascript:void(0);' onclick=WinOpenIt('" + h + "','_blank') >" + rowdata.NAME + "</a>";
                   }

                   },
                     { display: '流程图', render: function (rowdata, rowindex) {
                         var h = "../../WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + rowdata.NO + "&DoType=Chart&T=" + strTimeKey;
                         return "<a href='javascript:void(0);' onclick=OpenFlowPicture('" + rowdata.NO + "','" + rowdata.NAME + "')>打开</a>";
                        }
                      },
                     { display: '描述', name: 'NOTE' }
                   ],
            pageSize: 20,
            data: pushData,
            rownumbers: true,
            height: "99%",
            width: "99%",
            columnWidth: 120,
            onReload: LoadGrid,
            groupColumnName: 'FK_FLOWSORTTEXT',
            groupColumnDisplay: '流程类别',
            onDblClickRow: function (rowdata, rowindex) {
                WinOpenIt("../../MyFlow.aspx?FK_Flow=" + rowdata.FK_FLOW + "&FK_Node=" + rowdata.FK_NODE
                           + "&FID=" + rowdata.FID + "&IsRead=0&WorkID=" + rowdata.WORKID + "&T=" + strTimeKey, rowdata.WORKID, rowdata.FLOWNAME);
            }
        });
    }
    else {
        $.ligerDialog.warn('加载数据出错，请关闭后重试！');
    }
    $("#pageloading").hide();
}
//加载 取回审批 列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.getTask(callBack, this);
}
var strTimeKey = "";
$(function () {
    strTimeKey = "";
    var date = new Date();
    strTimeKey += date.getFullYear(); //年
    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
    strTimeKey += date.getDate(); //日
    strTimeKey += date.getHours(); //HH
    strTimeKey += date.getMinutes(); //MM
    strTimeKey += date.getSeconds(); //SS
    LoadGrid();
});