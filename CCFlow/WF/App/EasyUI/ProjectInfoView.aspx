<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectInfoView.aspx.cs"
    Inherits="CCFlow.AppDemoLigerUI.ProjectInfoView" %>

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
        function winOpen(url) {
            window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
        }
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

                    },
                    loadMsg: '数据加载中......'
                });
            }
            else {
                ShowHomePage();
                $.messager.alert('提示', '没有获取到数据！', 'info');
            }
        }
        //办结流程
        function LoadOverFowData() {
            this.columns = [{ title: '标题', field: 'Title', width: 240, align: 'left', formatter: function (rowindex, rowdata) {
                var titleText = "../WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&WorkID=" + rowdata.OID + "&FK_Flow=" + rowdata.FK_Flow;
                titleText = "<a href=javascript:winOpen('" + titleText + "');>" + rowdata.Title + "</a>";
                return titleText;
            }
            },
            { title: '部门', field: 'FK_DeptText', width: 120 },
            { title: '流程名称', field: 'FlowName', width: 200 },
            { title: '发起人', field: 'StarterName', width: 100 },
            { title: '发起时间', field: 'FlowStartRDT', width: 100 },
            { title: '流程状态', field: 'WFStateText', width: 100 },
            { title: '最后处理时间', field: 'FlowEnderRDT', width: 100}];

            var keyWords = $("#TB_KeyWords").val();
            var FK_Flow = document.getElementById("DDL_Flow").value;
            if (FK_Flow == "0") {
                FK_Flow = getArgsFromHref("FK_Flow");
            }
            var WFState = document.getElementById("DDL_WFState").value;

            var params = {
                method: "loadoverflowdata",
                FK_Flow: FK_Flow,
                ProjNo: getArgsFromHref("id"),
                keyWords: encodeURI(keyWords),
                WFState: WFState
            };
            queryData(params, LoadDataGrid, this);
        }
        //初始化
        $(function () {
            LoadOverFowData();
            $("#tb").hide();
            var showToolBar = getArgsFromHref("toolbar");
            if (showToolBar == "1") {
                $("#tb").show();
            }
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
        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "ProjectInfo.aspx", //要访问的后台地址
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
    <form runat="server">
    <div data-options="region:'center',border:false" style="margin: 0; padding: 0; overflow:auto;">
        <div id="tb" style="padding-top: 5px;">
            <div style="float: left;">
                关键字：<input type="text" id="TB_KeyWords" style="width: 120px;" />
            </div>
            <div style="float: left; padding-left:5px;">
                <asp:DropDownList runat="server" ID="DDL_WFState" Width="120px">
                </asp:DropDownList>
            </div>
            <div style="float: left; padding-left:5px;">
                <asp:DropDownList runat="server" ID="DDL_Flow" Width="160">
                </asp:DropDownList>
            </div>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'"
                onclick="LoadOverFowData()">查询</a>
        </div>
        <table id="ensGrid" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    </form>
</body>
</html>
