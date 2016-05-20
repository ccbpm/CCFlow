<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmpAppMenu.aspx.cs" Inherits="GMP2.GPM.EmpAppMenu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户在该系统中的菜单</title>
    <link id="appstyle" href="themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="jquery/jquery.min.js" type="text/javascript"></script>
    <script src="jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="javascript/AppData.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        var appName = "";
        var empNo = "";
        function LoadGrid() {
            Application.data.getMenuByEmpNo(empNo, appName, function (js, scope) {
                if (js) {
                    if (js == "") js = [];
                    var pushData = eval('(' + js + ')');
                    $('#menuGrid').datagrid({
                        data: pushData,
                        width: 'auto', 
                        striped: true,
                        rownumbers: true,
                        singleSelect: true,
                        loadMsg: '数据加载中......',
                        columns: [[
                       { field: 'Name', title: '菜单名', width: 180 },
                       { field: 'No', title: '编号', width: 100 },
                       { field: 'Url', title: 'Url', width: 260, align: 'left' },
                       { field: 'MyFileName', title: '图标名称', width: 80 },
                       { field: 'MyFilePath', title: '图标路径', width: 80 }
                       ]]
                    });
                }
            }, this);

        }
        //初始页面
        $(function () {
            appName = Application.common.getArgsFromHref("FK_App");
            empNo = Application.common.getArgsFromHref("FK_Emp");
            LoadGrid();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%= title%></div>
    <table id="menuGrid" class="easyui-datagrid">
    </table>
    </form>
</body>
</html>
