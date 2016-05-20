<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUploadEUI.aspx.cs"
    Inherits="CCFlow.WF.FileUploadEUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>文件上传</title>
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
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
            var loadType = $('#LoadType').combobox('getValue');
            $("#maingrid").datagrid({
                nowrap: true,
                title: '文件列表',
                fitColumns: true,
                fit: true,
                singleSelect: true,
                autoRowHeight: false,
                striped: true,
                toolbar: '#tb',
                collapsible: false,
                url: location.href + "?type=load&LoadType=" + loadType,
                rownumbers: true,
                onDblClickRow: function (idx, rowData) {
                    if (rowData.Name != '') {
                        if (rowData.Type == 'doc' || rowData.Type == 'docx' || rowData.Type == 'xlsx') {
                            window.open("WebOffice/OfficeView.aspx?IsEdit=1&Path=" + encodeURI("/DataUser/") + $("#LoadType").combobox('getValue') + "/" + rowData.Name);

                        }

                    }
                },
                columns: [[
                   { title: '名称 ', field: 'Name', width: 160, align: 'left', formatter: function (value, rec) {
                       return rec.Name;
                   }
                   },
                   { title: '类型', field: 'Type' },
                   { title: '大小(KB)', field: 'Size' }
                   ]]
            });
        }

        function remove() {
            var row = $('#maingrid').datagrid('getSelected');
            if (row == null) {
                $.messager.alert('提示', '请选择要删除的文件!');
                return;
            }
            pageLoadding('文件删除中!');
            var loadType = $('#LoadType').combobox('getValue');
            $.ajax({
                type: "post", //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: location.href + "?type=delete&LoadType=" + loadType + "&name=" + row.Name, //要访问的后台地址
                data: "", //要发送的数据
                async: true,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    loaddingOut("删除失败!");
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    if (msg == "true") {
                        loaddingOut("删除成功!");
                        loadGrid();
                    } else {

                        loaddingOut("删除失败!");
                        $.messager.alert('提示', msg);
                    }
                }
            });


        }
        function add() {
            $('#addWin').window('open');
            $('#addWin').parent().appendTo($("form:first"));
        }

        function checkForm() {
            if ($('#fileUpload').val() == '') {
                $.messager.alert('提示', '请选则文件!');
                return false;
            }
            pageLoadding('保存中...');
            return true;
        }

        function pageLoadding(msg) {
            $.jBox.tip(msg, 'loading');
        }
        function loaddingOut(msg) {
            $.jBox.tip(msg, 'success');
        }
    </script>
    <style type="text/css">
        .btn
        {
            border: 0;
            background: #4D77A7;
            color: #FFF;
            font-size: 12px;
            padding: 6px 10px;
            margin: 5px 0;
        }
    </style>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center'">
        <div id="maingrid" style="margin: 0; padding: 0;">
        </div>
    </div>
    <div id="tb" style="padding: 5px; height: auto">
        <a onclick="add();" class="easyui-linkbutton" iconcls="icon-add" plain="true"></a>
        <a onclick="remove()" class="easyui-linkbutton" iconcls="icon-remove" plain="true">
        </a>
        <asp:DropDownList runat="server" class="easyui-combobox" name="LoadType" data-options="onSelect: function(rec){loadGrid();},panelHeight: 'auto'"
            ID="LoadType" Style="width: 100px;">
            <asp:ListItem Text="套红文件" Value="OfficeOverTemplate"></asp:ListItem>
            <asp:ListItem Text="公文模板" Value="OfficeTemplate"></asp:ListItem>
            <asp:ListItem Text="公文签章" Value="OfficeSeal"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div id="addWin" class="easyui-window" title="添加" data-options="modal:true,closed:true,iconCls:'icon-add',maximizable:false,minimizable:false"
        style="width: 500px; height: 200px; padding: 10px;">
        文件:
        <br />
        <asp:FileUpload ID="fileUpload" runat="server" />
        <br />
        <asp:Button ID="btnConfirm" runat="server" class="btn" OnClientClick="return checkForm()"
            Text="确定" OnClick="btnConfirm_Click" />
    </div>
    </form>
</body>
</html>
