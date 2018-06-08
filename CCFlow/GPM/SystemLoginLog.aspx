<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemLoginLog.aspx.cs"
    Inherits="GMP2.GPM.SystemLoginLogPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link id="appstyle" href="themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link href="themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="jquery/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="javascript/CC.MessageLib.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        if ($.fn.pagination) {
            $.fn.pagination.defaults.beforePageText = '第';
            $.fn.pagination.defaults.afterPageText = '共{pages}页';
            $.fn.pagination.defaults.displayMsg = '显示{from}到{to},共{total}记录';
        }
        if ($.fn.datagrid) {
            $.fn.datagrid.defaults.loadMsg = '正在处理，请稍待。。。';
        }
        if ($.fn.treegrid && $.fn.datagrid) {
            $.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
        }
        if ($.messager) {
            $.messager.defaults.ok = '确定';
            $.messager.defaults.cancel = '取消';
        }
        //系统日志查询
        function LoadGrid(date1, date2) {
            Application.data.getSystemLoginLogs(function (js, scope) {
                if (js) {
                    if (js == "") js = "[]";
                    var pushData = eval('(' + js + ')');
                    $('#logGrid').datagrid({
                        data: pushData,
                        width: 'auto',
                        striped: true,
                        singleSelect: true,
                        loadMsg: '数据加载中......',
                        columns: [[
                       { field: 'OID', title: '编号', width: 100 },
                       { field: 'FK_EMP', title: '登录帐号', width: 100, align: 'left' },
                       { field: 'EMP_Name', title: '用户名', width: 100, align: 'left' },
                       { field: 'Dept_Name', title: '部门名称', width: 100, align: 'left' },
                       { field: 'Sys_Name', title: '系统名称', width: 200, align: 'left' },
                       { field: 'LoginDateTime', title: '登录时间', width: 160, align: 'center' },
                       { field: 'IP', title: 'IP地址', width: 200, align: 'left' },
                       ]]
                    });
                    $("#rowCount").html("查询共：" + pushData.total + " 条记录");
                }
            }, this, date1, date2);
        }

        $(function () {
            var date = new Date();
            var month = date.getMonth() + 1;
            var dateNow = date.getFullYear() + "-" + month + "-" + date.getDate();
            LoadGrid(dateNow, dateNow);
        });
        //查询
        function btnSearch_onclick() {
            var startDate = $("#editDate1").datebox('getValue');
            var endDate = $("#editDate2").datebox('getValue');
            if (startDate == "" || endDate == "") {
                CC.Message.showError("系统提示", "开始时间和结束时间都不能为空！");
                return;
            }
            LoadGrid(startDate, endDate);
        }
    </script>
</head>
<body>
    <div style="background-color: #b5cbf7; height: 45px;">
        <div id="divEditDate" style="margin-left: 5px; margin-bottom: 5px; vertical-align: middle;">
            <br />
            <div style="float: left;">
                开始时间:<input type="text" name="mdate" size="20" id="editDate1" class="easyui-datetimebox" data-options="required:true" />
            </div>
            <div style="float: left;">
                结束时间:
                <input type="text" name="mdate" size="20" id="editDate2" class="easyui-datetimebox" data-options="required:true" />
            </div>
            <div style="float: left;">
                <input type="button" id="btnSearch" value="查  询" onclick="btnSearch_onclick();" style="float: left;
                    display: inline;" />
            </div>
            <div style="float: right; font-family:arial,宋体,sans-serif; font-size:12px; color:Red;">&nbsp;&nbsp;<span id="rowCount"></span></div>
        </div>
    </div>
    <table id="logGrid" class="easyui-datagrid">
    </table>
</body>
</html>
