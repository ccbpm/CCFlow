<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthPlanCollect.aspx.cs"
    Inherits="CCFlow.AppDemoLigerUI.MonthPlanCollect" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <script type="text/javascript" language="javascript" src="Lodop/LodopFuncs.js"></script>
    <object id="LODOP_OB" classid="clsid:2105C259-1E0C-4534-8141-A753534CB4CA" width="0"
        height="0">
        <embed id="LODOP_EM" type="application/x-print-lodop" width="0" height="0" pluginspage="install_lodop32.exe"></embed>
    </object>
    <link href="jquery/lib/ligerUI/skins/Aqua/css/ligerui-all.css" rel="stylesheet" type="text/css" />
    <link href="jquery/tablestyle.css" rel="stylesheet" type="text/css" />
    <link href="jquery/lib/ligerUI/skins/ligerui-icons.css" rel="stylesheet" type="text/css" />
    <script src="jquery/lib/jquery/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/core/base.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/ligerui.all.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerGrid.js" type="text/javascript"></script>
    <script src="jquery/lib/ligerUI/js/plugins/ligerDialog.js" type="text/javascript"></script>
    <script src="js/AppData.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var LODOP; //声明为全局变量     
        function PrintOneURL() {
            LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
            LODOP.PRINT_INIT("打印月计划");
            LODOP.ADD_PRINT_TBURL(30, 20, 730, "100%", 'MonthPlanRePort.aspx');
            LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
            LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
            LODOP.SET_SHOW_MODE("HIDE_PAPER_BOARD", 1);
            LODOP.SET_SHOW_MODE("LANDSCAPE_DEFROTATED", 1); //横向时的正向显示
            LODOP.ADD_PRINT_TEXT(580, 660, 165, 22, "第#页/共&页");
            LODOP.SET_PRINT_STYLEA(0, "ItemType", 2);
            LODOP.SET_PRINT_STYLEA(0, "Horient", 3);
            LODOP.SET_PRINT_STYLEA(0, "Vorient", 3);
            LODOP.SET_PRINT_STYLEA(0, "TableHeightScope", 2);
            LODOP.PREVIEW();
        }
        //工具栏事件
        function itemclick(item) {
            if (item.text == "新建计划") {
                Application.data.createMonthPlan(function (js, scope) {
                    if (js == "success") {
                        LoadGrid();
                    }
                }, this);
            } else if (item.text == "导出Excel") {
                $.ligerDialog.open({ title: '请稍后...', width: 200, content: '正在导出，请稍后.....', url: "MonthPlanRePort.aspx?exporttype=xls" });
                setTimeout(function () {
                    $.ligerDialog.hide();
                }, 3000);

            } else if (item.text == "导出Word") {
                $.ligerDialog.open({ title: '请稍后...', width: 200, content: '正在导出，请稍后.....', url: "MonthPlanRePort.aspx?exporttype=doc" });
                setTimeout(function () {
                    $.ligerDialog.hide();
                }, 3000);
            } else if (item.text == "打印") {
                PrintOneURL();
            } else {
                var grid = $("#maingrid").ligerGetGridManager();
                var rows = grid.getCheckedRows();

                //判断是否选中行
                if (rows.length == 0) {
                    $.ligerDialog.warn('请选择行后重试！');
                    return;
                }

                if (item.text == "发送") {

                    $.ligerDialog.confirm('您确定要执行发送吗？', function (yes) {
                        if (yes) {
                            var str = "";
                            $(rows).each(function () {
                                if (str != "") str += "^";
                                str += this.FK_Flow + "," + this.WorkID;
                            });
                            //执行发送方法
                            Application.data.workFlowManage("send", str, function (js, scope) {
                                if (js.statusText == null && js.statusText != "error") {
                                    LoadGrid();
                                    document.getElementById("msgDialog").innerHTML = js;
                                    ShowMsgDialog();
                                }
                            }, this);
                        }
                    });
                } else if (item.text == "修改计划") {

                    var topRow = rows[0];
                    var strTimeKey = "";
                    var date = new Date();
                    strTimeKey += date.getFullYear(); //年
                    strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
                    strTimeKey += date.getDate(); //日
                    strTimeKey += date.getHours(); //HH
                    strTimeKey += date.getMinutes(); //MM
                    strTimeKey += date.getSeconds(); //SS
                    WinOpenIt("../WF/MyFlow.aspx?FK_Flow=" + topRow.FK_Flow + "&FK_Node=" + topRow.FK_Node
                           + "&FID=" + topRow.FID + "&WorkID=" + topRow.WorkID + "&IsRead=0&T=" + strTimeKey, topRow.WorkID, topRow.FlowName);

                } else if (item.text == "删除计划") {

                    $.ligerDialog.confirm('您确定要删除所选项吗？', function (yes) {
                        if (yes) {
                            var str = "";
                            $(rows).each(function () {
                                if (str != "") str += "^";
                                str += this.FK_Flow + "," + this.WorkID;
                            });
                            //执行删除方法
                            Application.data.workFlowManage("delete", str, function (js, scope) {
                                if (js.statusText == null && js.statusText != "error") {
                                    LoadGrid();
                                    document.getElementById("msgDialog").innerHTML = js;
                                    ShowMsgDialog();
                                }
                            }, this);
                        }
                    });
                }
            }
        }
        //弹出消息框
        function ShowMsgDialog() {
            //打开层
            $.ligerDialog.open({
                target: $("#msgDialog"),
                title: '消息',
                width: 810,
                height: 500,
                isResize: true,
                modal: true,
                buttons: [{ text: '关闭', onclick: function (i, d) {
                    d.hide();
                }
                }]
            });
        }
        //打开窗体
        function WinOpenIt(url, workId, text) {
            var isReadImg = document.getElementById(workId);
            if (isReadImg) isReadImg.src = "Img/Menu/Mail_Read.png";
            if (ccflow.config.IsWinOpenEmpWorks.toUpperCase() == "TRUE") {
                var val = window.showModalDialog(url, "审批流程", "dialogWidth=900px;dialogHeight=650px;dialogTop=50px;dialogLeft=60px");
                LoadGrid();
            } else {
                window.parent.f_addTab(workId, text, url);
            }
        }
        //加载列表
        function LoadGridCallBack(jsonData, scope) {
            if (jsonData) {
                var strTimeKey = "";
                var date = new Date();
                strTimeKey += date.getFullYear(); //年
                strTimeKey += date.getMonth() + 1; //月 月比实际月份要少1
                strTimeKey += date.getDate(); //日
                strTimeKey += date.getHours(); //HH
                strTimeKey += date.getMinutes(); //MM
                strTimeKey += date.getSeconds(); //SS
                var month = date.getMonth() + 1;
                var dateNow = date.getFullYear() + "-" + month + "-" + date.getDate();

                var pushData = eval('(' + jsonData + ')');
                var grid = $("#maingrid").ligerGrid({
                    columns: [
                   { display: '标题', name: 'Title', width: 340, align: 'left', render: function (rowdata, rowindex) {
                       var title = "";
                       if (rowdata.IsRead == 0) {
                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlow.aspx?FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FK_Node
                           + "&FID=" + rowdata.FID + "&WorkID=" + rowdata.WorkID + "&IsRead=0&T=" + strTimeKey
                           + "','" + rowdata.WorkID + "','" + rowdata.FlowName + "');\" ><img align='middle' alt='' id='" + rowdata.WorkID
                           + "' src='Img/Menu/Mail_UnRead.png' border=0/>" + rowdata.Title + "</a>";
                       } else {
                           title = "<a href=\"javascript:WinOpenIt('../WF/MyFlow.aspx?FK_Flow=" + rowdata.FK_Flow + "&FK_Node=" + rowdata.FK_Node
                           + "&FID=" + rowdata.FID + "&T=" + strTimeKey + "&WorkID=" + rowdata.WorkID + "','" + rowdata.WorkID
                           + "','" + rowdata.FlowName + "');\"  ><img border=0 align='middle' id='" + rowdata.WorkID + "' alt='' src='Img/Menu/Mail_Read.png'/>" + rowdata.Title + "</a>";
                       }
                       return title;
                   }
                   },
                   { display: '工作项目', name: 'GongZuoXiangMu' },
                   { display: '当前节点', name: 'NodeName' },
                   { display: '发起人', name: 'JiHuaBianZhiRen' },
                   { display: '发起日期', name: 'RDT' },
                   { display: '接受日期', name: 'ADT' },
                   { display: '应完成日期', name: 'SDT' },
                   { display: '状态', name: 'FlowState', render: function (rowdata, rowindex) {
                       var datePattern = /^(\d{4})-(\d{1,2})-(\d{1,2})$/;
                       if (datePattern.test(rowdata.SDT)) {
                           var d1 = new Date(dateNow.replace(/-/g, "/"));
                           var d2 = new Date(rowdata.SDT.replace(/-/g, "/"));

                           if (Date.parse(d1) <= Date.parse(d2)) {
                               return "正常";
                           }
                           return "<font color=red>逾期</font>";
                       }
                   }
                   }
                   ],
                    data: pushData,
                    rownumbers: true,
                    checkbox: true,
                    selectRowButtonOnly: true,
                    height: "99%",
                    width: "99%",
                    columnWidth: 100,
                    pageSize: 50,
                    onReload: LoadGrid,
                    toolbar: { items: [
                    { text: '发送', click: itemclick, icon: 'msn' },
                    { line: true },
                    { text: '新建计划', click: itemclick, icon: 'add' },
                    { text: '修改计划', click: itemclick, icon: 'modify' },
                    { text: '删除计划', click: itemclick, icon: 'delete' },
                    { line: true },
                        //{ text: '导出Excel', click: itemclick, icon: 'excel' },
                    {text: '导出Word', click: itemclick, icon: 'Word' },
                    { text: '打印', click: itemclick, icon: 'print' }
                    ]
                    }
                });
            }
            else {
                $.ligerDialog.warn('加载数据出错，请关闭后重试！');
            }
            $("#pageloading").hide();
        }

        //加载列表
        function LoadGrid() {
            $("#pageloading").show();
            Application.data.monthPlanCollect(LoadGridCallBack, this);
        }
        //页面初始化
        $(function () {
            LoadGrid();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="pageloading">
    </div>
    <div id="maingrid" style="margin: 0; padding: 0;">
    </div>
    <div id="msgDialog" style="display: none">
    </div>
    </form>
</body>
</html>
