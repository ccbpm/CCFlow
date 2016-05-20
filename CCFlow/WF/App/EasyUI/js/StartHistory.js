//获取从上一个url传递来的参数列表
function QueryString() {
    var name, value, i;

    var str = location.href;
    var num = str.indexOf("?")

    str = str.substr(num + 1);
    var arrtmp = str.split("&");

    for (i = 0; i < arrtmp.length; i++) {
        num = arrtmp[i].indexOf("=");
        if (num > 0) {
            name = arrtmp[i].substring(0, num);
            value = arrtmp[i].substr(num + 1);
            this[name] = value;
        }
    }
}

var fk_flow; //流程编号
var name; //流程名称
var dateNow; //日期
$(function () {
    dateNow = "";
    var date = new Date();
    dateNow += date.getFullYear(); //年
    dateNow += date.getMonth() + 1; //月 月比实际月份要少1
    dateNow += date.getDate(); //日
    dateNow += date.getHours(); //HH
    dateNow += date.getMinutes(); //MM
    dateNow += date.getSeconds(); //SS

    var Request = new QueryString();
    fk_flow = Request["FK_Flow"];
    name = Request["Name"];
    strAuth = Request["Auth"];
        //工具事件
        var toolBarManager = $("#toptoolbar").ligerToolBar({ items: [
                    { text: '发起流程', click: itemclick, icon: 'plus' }
                ]
        });
    LoadGrid();
});

function itemclick() {
    var h;

    h = "../../MyFlow.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_flow + "01&T=" + dateNow;
    WinOpenIt(fk_flow, '发起' + name + '流程', h);
}
//加载列表
function LoadGrid() {
    $("#pageloading").show();
    Application.data.getStoryHistory(fk_flow, callBack, this);
}

//回调函数
function callBack(jsonData, scope) {
    if (jsonData.length) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $("#maingrid").ligerGrid({
            columns: [
                   { display: '标题', name: 'Title', width: 380, align: 'left', render: function (rowdata, rowindex) {
                       //                       var title = "<a href=javascript:WinOpenIt('" + rowdata.MyPK + "','" + rowdata.FK_Flow + "','" + rowdata.FK_Node
                       //                       + "','" + rowdata.WorkID + "','" + rowdata.FID + "','" + rowdata.Sta + "');><img align=middle src='Img/Menu/CCSta/" + rowdata.Sta
                       //                       + ".png' id=" + rowdata.MyPK + ">" + rowdata.Title + "</a>日期:" + rowdata.RDT;
                       //                       return Title;   

                       var h = "../../WFRpt.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + fk_flow + "&FID=" + rowdata.FID + "&T=" + dateNow;
                       return "<a href='javascript:void(0);' onclick=WinOpenWindow('" + h + "')>" + rowdata.Title + "</a>";
                                   
                       return rowdata.Title;
                   }
                   },
                   { display: '发起日期', name: 'FlowStartRDT' },
                   { display: '结束节点', name: 'FlowEndNode' },
                   { display: '状态', name: 'WFState', render: function (rowdata, rowindex) {
                       switch (rowdata.WFState) {
                           case 0:
                                return "空白";
                                //return "<img src='../WF/Img/WFState/Cancel.png'   class='imgWFstate' />" + "   空白";
                               break;
                           case 1:
                              // return "草稿";
                               return "<img src='../../Img/WFState/Draft.png'   class='imgWFstate' />" + "   草稿";
                               break;
                           case 2:
                               //return "运行中"; 
                               return "<img src='../../Img/WFState/Runing.png'   class='imgWFstate' />" + "   运行中";
                               break;
                           case 3:
                               return "已完成";
                               //return "<img src='../WF/Img/WFState/RebackOverFlow.png'   class='imgWFstate' />" + "    已完成";
                               break;
                           case 4:
                              // return "挂起";
                               return "<img src='../../Img/WFState/HungUp.png'   class='imgWFstate' />" + "   挂起";
                               break;
                           case 5:
                               //return "退回"; 
                               return "<img src='../../Img/WFState/RebackOverFlow.png'  class='imgWFstate' />" + "   退回";
                               break;
                           case 6:
                               return "转发";
                               break;
                           case 7:
                               //return "删除";
                               return "<img src='../../Img/WFState/Cancel.png'   class='imgWFstate' />" + "   删除";
                               break;
                           default:
                               return "";
                               break;
                       }
                   }
                   }
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
//打开窗体
function WinOpenIt(tabid, text, url) {
    if (ccflow.config.IsWinOpenStartWork == 1) {
        window.parent.f_addTab(tabid + dateNow, text, url);
    } else {
        var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        newWindow.focus();
    }
}
function WinOpenWindow(url) {
    var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
    newWindow.focus();
}