<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectMustEnter.aspx.cs"
    Inherits="CCFlow.AppDemoLigerUI.ProjectMustEnter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Comm/JS/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Comm/JS/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Comm/JS/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Comm/JS/EasyUI/locale/easyui-lang-zh_CN.js" type="text/javascript"
        charset="UTF-8"></script>
    <script language="javascript" type="text/javascript">
        //加载grid数据
        function LoadDataGrid(gridData, scope) {
            if (gridData) {
                
                if (gridData == "") gridData = "[]";
                var pushData = eval('(' + gridData + ')');
                var fitColumns = true;
                if (scope.columns.length > 7) {
                    fitColumns = false;
                }

                $('#ensGrid').datagrid({
                    columns: [scope.columns],
                    data: pushData,
                    width: 'auto',
                    height: 'auto',
                    striped: true,
                    rownumbers: true,
                    singleSelect: true,
                    pagination: false,
                    remoteSort: false,
                    fitColumns: fitColumns,
                    onDblClickCell: function (index, field, value) {
                        ToolBarClick("editdata");
                    },
                    loadMsg: '数据加载中......'
                });
            }
            else {
                ShowHomePage();
                $.messager.alert('提示', '没有获取到数据！', 'info');
            }
        }
        //获取应报数据
        function LoadMustEnterData() {
            this.columns = [{ title: '年月', field: 'XMNY', width: 80 },
            { title: '日报应报数量', field: 'YBRB', width: 100 },
            { title: '商务经理月报应报数量', field: 'YBSWJLYB', width: 100 },
            { title: '项目经理月报应报数量', field: 'YBXMJLYB', width: 100 },
            { title: '备注', field: 'MEMO', width: 120}];

            var year = document.getElementById("DDL_Year").value;
            var params = {
                method: "getmustenterdata",
                ProjNo: getArgsFromHref("id"),
                NYear: year
            };
            queryData(params, LoadDataGrid, this);
        }
        //工具按钮事件
        function ToolBarClick(item) {
            //查询列表
            if (item == "doquery") {
                LoadMustEnterData();
            }
            //修改数据
            if (item == "editdata") {
                var row = $('#ensGrid').datagrid('getSelected');
                if (row) {
                    var rRb = row.YBRB;
                    var rSWJL = row.YBSWJLYB;
                    var rXMJL = row.YBXMJLYB;

                    $("#XMBH").val(row.XMBH);
                    $("#RBSL").val(row.YBRB);
                    $("#SWJLYBSL").val(row.YBSWJLYB);
                    $("#XMJLYBSL").val(row.YBXMJLYB);
                    $("#TB_Doc").val(row.MEMO);

                    //弹出窗体
                    $('#infoPanel').dialog({
                        title: "编辑上报数量",
                        width: 600,
                        height: 280,
                        closed: false,
                        modal: true,
                        iconCls: 'icon-save',
                        resizable: true,
                        onClose: function () {
                            ClearText();
                        },
                        buttons: [{
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                var rb = $("#RBSL").val();
                                var swjl = $("#SWJLYBSL").val();
                                var xmjl = $("#XMJLYBSL").val();
                                var remark = $("#TB_Doc").val();
                                var id = row.ID;
                                if (rb == "") {
                                    $.messager.alert("提示", "日报应报数量不能为空!");
                                    return;
                                }
                                if (swjl == "") {
                                    $.messager.alert("提示", "商务经理应报数量不能为空!");
                                    return;
                                }
                                if (xmjl == "") {
                                    $.messager.alert("提示", "项目经理应报数量不能为空!");
                                    return;
                                }
                                //日报数量限制
                                if (rb < 0 || rb > 31) {
                                    $.messager.alert("提示", "日报数量限制在0-31之间!");
                                    return;
                                }
                                //商务经理数量限制
                                if (swjl < 0 || swjl > 1) {
                                    $.messager.alert("提示", "商务经理月报数量只能为0或1");
                                    return;
                                }
                                //项目经理数量限制
                                if (xmjl < 0 || xmjl > 1) {
                                    $.messager.alert("提示", "项目经理月报数量只能为0或1");
                                    return;
                                }
                                //内容发生变化备注不能为空
                                if (rRb != rb || rSWJL != swjl || rXMJL != xmjl || row.MEMO != remark) {
                                    if (remark == "") {
                                        $.messager.alert("提示", "内容发生变化，请填写备注说明原因!");
                                        return;
                                    }
                                    //去除空格
                                    remark = $.trim(remark);
                                    if (remark == "") {
                                        $.messager.alert("提示", "内容发生变化，请填写备注说明原因!");
                                        return;
                                    }

                                    //参数
                                    var params = {
                                        method: "editmustenter",
                                        ID: id,
                                        RBSL: rb,
                                        SWJLYBSL: swjl,
                                        XMJLYBSL: xmjl,
                                        Remark: encodeURI(remark)
                                    };
                                    queryData(params, function (js, scope) {
                                        if (js == "true") {
                                            LoadMustEnterData();
                                            $('#infoPanel').dialog("close");
                                        } else {
                                            $.messager.alert("提示", "更新数据失败!");
                                        }
                                    }, this);

                                } else {
                                    $('#infoPanel').dialog("close");
                                }
                            }
                        }, {
                            text: '取消',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                $('#infoPanel').dialog("close");
                            }
                        }]
                    });
                } else {
                    $.messager.alert("提示", "您没有选中数据!");
                }
            }
        }

        //初始化
        $(function () {
            LoadMustEnterData();
        });
        function getArgsFromHref(sArgName) {
            var sHref = window.location.href;
            var args = sHref.split("?");
            var retval = "";
            if (args[0] == sHref) /*参数为空*/
            {
                return retval; /*无需做任何处理*/
            }

            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1) continue;
                if (arg[0] == sArgName) retval = arg[1];
            }
            while (retval.indexOf('#') >= 0) {
                retval = retval.replace('#', '');
            }
            return retval;
        }
        //清空文本
        function ClearText() {
            $("#XMBH").val('');
            $("#RBSL").val('');
            $("#SWJLYBSL").val('');
            $("#XMJLYBSL").val('');
            $("#TB_Doc").val('');
        }
        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "ProjectMustEnter.aspx", //要访问的后台地址
                data: param, //要发送的数据
                async: false,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {//msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
    </script>
    <style type="text/css">
        body, html
        {
            height: 100%;
        }
        body
        {
            padding: 0px;
            margin: 0;
            overflow: hidden;
        }
    </style>
</head>
<body class="easyui-layout" id="index_layout">
    <form id="Form1" runat="server">
    <div data-options="region:'center',border:false" style="margin: 0; padding: 0; overflow: hidden;">
        <div id="tb">
            年份：<asp:DropDownList runat="server" ID="DDL_Year" Width="80px">
            </asp:DropDownList>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-insert'"
                onclick="ToolBarClick('doquery')">查询</a> 
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-edit'"
                onclick="ToolBarClick('editdata')">修改</a> 
            &nbsp;&nbsp;&nbsp;&nbsp;<span id="Lab_XMMC" runat="server">
            </span>
        </div>
        <table id="ensGrid" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    <div id="infoPanel">
        <table style="width: 500px; margin-top: 10px;">
            <tr>
                <td style="float: right; width: 70px;">项 目 编 号：</td>
                <td><input type="text" id="XMBH" readonly="readonly" /></td>
                <td style="float: right; width: 90px;">日报应报数量：</td>
                <td><input type="text" id="RBSL" onkeypress="return (/[\d.]/.test(String.fromCharCode(event.keyCode)))" /></td>
            </tr>
            <tr>
                <td style="float: right; width: 110px;">商务经理月报数量：</td>
                <td><input type="text" id="SWJLYBSL" onkeypress="return (/[\d.]/.test(String.fromCharCode(event.keyCode)))" /></td>
                <td style="float: right; width: 110px;">项目经理月报数量：</td>
                <td><input type="text" id="XMJLYBSL" onkeypress="return (/[\d.]/.test(String.fromCharCode(event.keyCode)))" /></td>
            </tr>
            <tr>
                <td style="float: right;">备注：</td>
                <td colspan="3"><textarea cols="50" rows="5" id="TB_Doc"></textarea></td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
