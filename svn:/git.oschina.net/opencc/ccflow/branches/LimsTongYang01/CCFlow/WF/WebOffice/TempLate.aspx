<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TempLate.aspx.cs" Inherits="CCFlow.WF.WebOffice.TempLate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/jBox/jquery.jBox-2.3.min.js" type="text/javascript"></script>
    <link href="../Scripts/jBox/Skins/Blue/jbox.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            loadGrid();
        });

        function loadGrid() {
            $("#maingrid").datagrid({
                nowrap: true,
                fitColumns: true,
                fit: true,
                singleSelect: true,
                autoRowHeight: false,
                striped: true,
                collapsible: false,
                url: location.href + "&load=true",
                rownumbers: true,
                columns: [[
                   { title: '名称 ', field: 'Name', width: 160, align: 'left', formatter: function (value, rec) {
                       return rec.Name;
                   }
                   },
                   { title: '类型', field: 'Type' },
                   { title: '大小(KB)', field: 'Size' },
                   { title: 'realName', field: 'RealName', hidden: 'true' }
                   ]]
            });
        }
        function getSelected() {
            var row = $('#maingrid').datagrid('getSelected');
            return row;
        }


        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }
        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');
        }

    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',iconCls:'icon-ok'">
        <div id='maingrid'>
        </div>
    </div>
    </form>
</body>
</html>
