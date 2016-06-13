var strTimeKey = "";
//发起流程
function StartFlow(url) {

}
//发起工作流
function StartListUrl(url) {
    var v = window.showModalDialog(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    if (v == null || v == "")
        return;
}
//打开窗体
function WinOpenIt(tabid, text, url) {
    if (ccflow.config.IsWinOpenStartWork == 1) {
        window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
    } else {
        var winWidth = 850;
        var winHeight = 680;
        if (screen && screen.availWidth) {
            winWidth = screen.availWidth;
            winHeight = screen.availHeight;
        }
        //var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        window.showModalDialog(url, "", "scrollbars=yes;resizable=yes;center=yes;dialogWidth=" + winWidth + ";dialogHeight=" + winHeight + ";dialogTop=50px;dialogLeft=50px;");
    }
}
function WinOpenWindow(url) {
    var newWindow = window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
    newWindow.focus();
}
function callBack(jsonData, scope) {
    if (jsonData) {
        var pushData = eval('(' + jsonData + ')');
        var grid = $('#maingrid').treegrid({
            data: pushData,
            idField: 'NO',
            treeField: 'NAME',
            animate: true,
            fitColumns: true,
            loadMsg: '数据加载中......',
            columns: [[
                    { field: 'NAME', title: '名称', width: 380, align: 'left', formatter: function (value, rec) {
                        if (rec.FK_FLOWSORT == "")
                            return value;
                        var h = "";
                        if (rec.STARTLISTURL) {
                            h = rec.STARTLISTURL + "?FK_Flow=" + rec.NO + "&FK_Node=" + rec.NO + "01&T=" + strTimeKey;
                            return "<a href='javascript:void(0);' onclick=StartListUrl('" + h + "')>" + rec.NAME + "</a>";
                        }
                        h = "../../MyFlow.aspx?FK_Flow=" + rec.NO + "&FK_Node=" + rec.NO + "01&T=" + strTimeKey;
                        return "<a href='javascript:void(0);' onclick=ShowEasyUiTitleDiv('" + rec.NO + "','" + rec.NAME + "','" + h + "')>" + rec.NAME + "</a>";
                    }
                    },
                    { field: 'IsBatchStart', title: '批量发起', formatter: function (value, rec) {
                        if (rec.FK_FLOWSORT == "")
                            return value;
                        var h = "";
                        if (rec.ISBATCHSTART == "1") {
                            h = "../../BatchStart.aspx?FK_Flow=" + rec.NO;
                            h = "<a href='javascript:void(0);' onclick=StartListUrl('" + h + "')>批量发起</a>";
                        }
                        return h;
                    }
                    },
                    { field: 'RoleType', title: '流程图', formatter: function (value, rec) {
                        if (rec.FK_FLOWSORT == "")
                            return value;
                        return "<a href='javascript:void(0);' onclick=OpenEasyUiFlowPicture('" + rec.NO + "','" + rec.NAME + "')>打开</a>";
                    }
                    },
                    { field: 'HistoryFlow', title: '历史发起', width: 180, formatter: function (value, rec) {
                        if (rec.FK_FLOWSORT == "")
                            return value;
                        return "<a href='javascript:void(0);' onclick=ShowEasyUiHistoryData('" + rec.NO + "','" + rec.NAME + "')>查看</a>";
                    }
                    },
                    { field: 'Note', title: '描述', width: 280, formatter: function (value, rec) {
                        if (rec.FK_FLOWSORT == "")
                            return value;
                        if (rec.NOTE == null || rec.NOTE == "") {
                            return "无";
                        }
                        return rec.Note;
                    }
                    }
                ]],
            onLoadSuccess: function (row, data) {

            }

        });
    }
    else {
        $.messager.alert('提示', '加载数据出错，请关闭后重试！');
    }
    $("#pageloading").hide();
}

//加载历史发起数据
function LoadEasyUiHistoryGrid(flowNo) {
    $("#pageloading").show();
    Application.data.getHistoryStartFlow(flowNo, function (json) {
        if (json) {
            var grid = $("#historyGrid").datagrid({
                pagination: true,
                nowrap: true,
                fitColumns: true,
                autoRowHeight: false,
                rownumbers: true,
                striped: true,
                collapsible: false,
                url: '/WF/App/EasyUI/Base/DataService.aspx?method=historystartflow&FK_Flow=' + flowNo,
                columns: [[
                        { title: '标题', field: 'Title', width: 320, align: 'left', formatter: function (value, rec) {
                            var h = "../../WFRpt.aspx?WorkID=" + rec.OID + "&FK_Flow=" + flowNo + "&FID=" + rec.FID + "&T=" + strTimeKey;
                            return "<a href='javascript:void(0);' onclick=WinOpenWindow('" + h + "')>" + rec.Title + "</a>";
                        }
                        },
                        { title: '发起时间', field: 'FlowStartRDT' },
                        { title: '参与人', field: 'FlowEmps', width: 300 }
                    ]]
            });
        }
        $("#pageloading").hide();
    }, this)
}
//打开流程图
function OpenEasyUiFlowPicture(flowNo, flowName) {
    var pictureUrl = "../../../DataUser/FlowDesc/" + flowNo + "." + flowName + "/Flow.png";

    document.getElementById("FlowPic").src = pictureUrl;
    $("#flowPicDiv").dialog({
        height: 500,
        width: 800,
        showMax: true,
        isResize: true,
        modal: true,
        title: flowName + "流程图",
        slide: false,
        buttons: [{ text: '关闭', handler: function () {
            $('#flowPicDiv').dialog('close');
        }
        }]
    });
}
//显示历史发起
function ShowEasyUiHistoryData(flowNo, flowName) {
    //打开层
    $("#showHistory").dialog({
        title: flowName + '-历史发起列表',
        width: 810,
        height: 480,
        modal: true,
        buttons: [{ text: '关闭', handler: function () {
            $('#showHistory').dialog('close');
        }
        }]
    });
    LoadEasyUiHistoryGrid(flowNo);
}
//输入标题框
function ShowEasyUiTitleDiv(tabid, text, url) {
    //执行命令
    Application.data.createEmptyCase(tabid, "", function (js, scope) {
        if (js == "addform") {
            if (ccflow.config.IsWinOpenStartWork == 0) {
                WinOpenIt(tabid, text, url);
            } else {
                if (window.parent && window.parent.f_addTab) {
                    window.parent.f_addTab(tabid + strTimeKey, text, url);
                } else {
                    WinOpenIt(tabid, text, url);
                }
            }
        } else {
            //打开层
            $('#divTitle').show();
            $("#divTitle").dialog({
                title: '新建- ' + text + "流程",
                width: 510,
                height: 180,
                resizable: true,
                buttons: [{ text: '确定', handler: function () {
                    var title = $("#TB_Title").val();

                    if (title == "") {
                        $.messager.alert('提示', '标题不允许为空！');
                        return;
                    }
                    //执行命令
                    Application.data.createEmptyCase(tabid, title, function (js, scope) {
                        $("#TB_Title").val("");
                        $('#divTitle').dialog('close');
                        WinOpenIt(tabid, text, js);
                    });
                }
                }, { text: '取消', handler: function (i, d) {
                    $("#TB_Title").val("");
                    //d.hide();
                    $('#divTitle').dialog('close');
                }
                }]
            });
        }
    }, this);
}
//加载发起流程列表
function LoadGrid() {
    $("#pageloading").show();
    $("#divTitle").hide();
    Application.data.getStartFlowTree(callBack, this);
}

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