<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectCheck.aspx.cs" Inherits="CCFlow.AppDemoLigerUI.ProjectCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/WF/Comm/JS/EasyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="/WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="/WF/Comm/JS/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="/WF/Comm/JS/EasyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="/WF/Comm/JS/EasyUI/locale/easyui-lang-zh_CN.js" type="text/javascript"
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
            var FK_Flow = getArgsFromHref("FK_Flow");
            this.columns = [{ title: '标题', field: 'Title', width: 240, align: 'left', formatter: function (rowindex, rowdata) {
                var titleText = "../WF/WFRpt.aspx?WorkID=" + rowdata.OID + "&FK_Flow=" + FK_Flow + "&FK_Node=" + FK_Flow + "01&DoType=View";
                titleText = "<a href=javascript:winOpen('" + titleText + "');>" + rowdata.Title + "</a>";
                return titleText;
            }
            },
            { title: '项目名称', field: 'XMMC', width: 200 },
            { title: '子(分)公司名称', field: 'FGSMC', width: 160 },
            { title: '发布部门', field: 'DeptName', width: 120 },
            { title: '上传人', field: 'UserName', width: 100 },
            { title: '上传时间', field: 'FlowStartRDT', width: 100}];

            if (FK_Flow == "") {
                FK_Flow = "999";
            }
            //关键字
            var keyWord = $("#TB_KeyWord").val();
            var params = {
                method: "loadoverflowdata",
                FK_Flow: FK_Flow,
                ProjNo: getArgsFromHref("id"),
                keyWords: encodeURI(keyWord)
            };
            queryData(params, LoadDataGrid, this);
        }
        //工具按钮事件
        function ToolBarClick(item) {
            //发起
            if (item == "start") {
                var fk_flow = getArgsFromHref("FK_Flow");
                var url = "../../Flow/WF/MyFlow.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_flow + "01&ProjNo=" + getArgsFromHref("id");
                var params = {
                    method: "iscanstartthisflow",
                    FK_Flow: getArgsFromHref("FK_Flow")
                };
                queryData(params, function (js) {
                    try {
                        var pushData = eval('(' + js + ')');
                        if (pushData.IsCan == "true") {
                            if (window.parent && window.parent.addNewTab) {
                                window.parent.addNewTab(pushData.FlowName + "-发起", url);
                            }
                        } else {
                            $.messager.alert('提示', '您没有发起此业务的权限！');
                        }
                    } catch (e) {
                        $.messager.alert('提示', '您没有发起此业务的权限！');
                    }
                }, this);
            }
            //查询列表
            if (item == "sendover") {
                LoadOverFowData();
            }
        }

        function IsCanStartThisFlow() {
            var params = {
                method: "iscanstartthisflow",
                FK_Flow: getArgsFromHref("FK_Flow")
            };
            queryData(params, function (js) {
                var pushData = eval('(' + js + ')');
                if (pushData.IsCan == "true") {
                    alert(document.getElementById("LinkBtn_Start").style);
                    $('#LinkBtn_Start').linkbutton('enable');
                }
            }, this);
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
                url: "ProjectCheck.aspx", //要访问的后台地址
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
    <div data-options="region:'center',border:false" style="margin: 0; padding: 0; overflow: auto;">
        <div id="tb">
            <a href="#" id="LinkBtn_Start" style="float: left;" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-add'"
                onclick="ToolBarClick('start')">发起</a>
            <div class="datagrid-btn-separator">
            </div>
            关键字：<input type="text" id="TB_KeyWord" style="width: 150px;" />
            <a href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-insert'"
                onclick="ToolBarClick('sendover')">查询</a>
        </div>
        <table id="ensGrid" toolbar="#tb" class="easyui-datagrid">
        </table>
    </div>
</body>
</html>
