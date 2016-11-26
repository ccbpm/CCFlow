<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectDelInfo.aspx.cs"
    Inherits="CCFlow.AppDemoLigerUI.ProjectDelInfo" %>

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
        function ReturnFlowData(FK_Flow, WorkID) {
            //弹出窗体
            $('#infoPanel').dialog({
                title: "回滚流程-原因",
                width: 438,
                height: 200,
                closed: false,
                modal: true,
                iconCls: 'icon-save',
                resizable: false,
                onClose: function () {
                    $("#TB_Doc").val("");
                },
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok',
                    handler: function () {
                        var remark = $("#TB_Doc").val();
                        if (remark == "") {
                            $.messager.alert("提示", "回滚原因不能为空，请填写说明原因!");
                            return;
                        }
                        //去除空格
                        remark = $.trim(remark);
                        if (remark == "") {
                            $.messager.alert("提示", "回滚原因不能为空，请填写说明原因!");
                            return;
                        }

                        //参数
                        var params = {
                            method: "returnflowdata",
                            FK_Flow: FK_Flow,
                            WorkID: WorkID,
                            Remark: encodeURI(remark)
                        };
                        queryData(params, function (js, scope) {
                            $('#infoPanel').dialog("close");
                            $('#content').html(js);
                            //弹出窗体
                            $('#msgPanel').dialog({
                                title: "系统消息",
                                width: 450,
                                height: 260,
                                closed: false,
                                modal: true,
                                iconCls: 'icon-save',
                                resizable: true,
                                onClose: function () {
                                    $('#content').html("");
                                    LoadOverFowData();
                                },
                                buttons: [{
                                    text: '确定',
                                    iconCls: 'icon-ok',
                                    handler: function () {
                                        $('#msgPanel').dialog("close");
                                    }
                                }]
                            });
                        }, this);
                    }
                }, {
                    text: '取消',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        $('#infoPanel').dialog("close");
                    }
                }]
            });
        }
        function DelFlowData(FK_Flow, WorkID) {
            $.messager.confirm('确认对话框', '删除后将不能恢复，您确定执行删除吗？', function (r) {
                if (r) {
                    var url = "../WF/admin/FlowDB/FlowDB.aspx?DoType=DelIt&WorkID=" + WorkID + "&FK_Flow=" + FK_Flow;
                    $("<div id='dialogEnPanel'></div>").append($("<iframe width='100' height='10' frameborder=0 src='" + url + "'/>")).dialog({
                        title: "请稍后......",
                        width: 150,
                        height: 60,
                        autoOpen: true,
                        modal: true,
                        resizable: true,
                        onClose: function () {
                            $("#dialogEnPanel").remove();
                        }
                    });
                    //定是关闭
                    setTimeout(function () {
                        LoadOverFowData();
                        $('#dialogEnPanel').dialog("close");
                    }, 3000);
                }
            });
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
            this.columns = [{ title: '操作', field: 'del', align: 'center', width: 80, formatter: function (rowindex, rowdata) {
                var titleText = "";
                //结束流程才允许回滚
                if (rowdata.WFState == "3")
                    titleText = "<a href=javascript:ReturnFlowData('" + rowdata.FK_Flow + "','" + rowdata.OID + "');>回滚</a>&nbsp;&nbsp;|&nbsp;&nbsp;";
                titleText += "<a href=javascript:DelFlowData('" + rowdata.FK_Flow + "','" + rowdata.OID + "');>删除</a>";
                return titleText;
            }
            },
            { title: '标题', field: 'Title', width: 240, align: 'left', sortable: true, formatter: function (rowindex, rowdata) {
                var titleText = "../WF/WorkOpt/OneWork/Track.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + rowdata.FK_Flow;
                titleText = "<a href=javascript:winOpen('" + titleText + "');>" + rowdata.Title + "</a>";
                return titleText;
            }
            },
            { title: '部门', field: 'FK_DeptText', sortable: true, width: 120 },
            { title: '流程名称', field: 'FlowName', sortable: true, width: 200 },
            { title: '发起人', field: 'StarterName', sortable: true, width: 100 },
            { title: '发起时间', field: 'FlowStartRDT', sortable: true, width: 100 },
            { title: '流程状态', field: 'WFStateText', sortable: true, width: 100 },
            { title: '最后处理时间', field: 'FlowEnderRDT', sortable: true, width: 100}];

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
    <form id="Form1" runat="server">
    <div data-options="region:'center',border:false" style="margin: 0; padding: 0; overflow: auto;">
        <div id="tb" style="padding-top: 5px;">
            <div style="float: left;">
                关键字：<input type="text" id="TB_KeyWords" style="width: 120px;" />
            </div>
            <div style="float: left; padding-left: 5px;">
                <asp:DropDownList runat="server" ID="DDL_WFState" Width="120px">
                </asp:DropDownList>
            </div>
            <div style="float: left; padding-left: 5px;">
                <asp:DropDownList runat="server" ID="DDL_Flow" Width="160">
                </asp:DropDownList>
            </div>
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-search'"
                onclick="LoadOverFowData()">查询</a>
        </div>
        <table id="ensGrid" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
    <div id="infoPanel">
        <textarea cols="50" rows="8" id="TB_Doc"></textarea>
    </div>
    <div id="msgPanel">
        <div id="content" style="height: 150px; overflow: auto; padding: 10px 10px 10px 10px;">
        </div>
    </div>
    </form>
</body>
</html>
