<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowFormTree.aspx.cs" Inherits="CCFlow.WF.Admin.FlowFormTree" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Style/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/default/datagrid.css" rel="stylesheet" type="text/css" />
    <link href="../Style/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        //sArgName表示要获取哪个参数的值
        this.getArgsFromHref = function (sArgName) {
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
            return retval;
        }
        //公共方法
        function queryData(url, param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: url, //要访问的后台地址
                data: param, //要发送的数据
                async: true,
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
        //回调函数
        function LoadFormTreeCallBack(js, scope) {
            if (js == "") js = [];
            var pushData = eval('(' + js + ')');

            //加载系统目录
            $("#flowFormTree").tree({
                data: pushData,
                iconCls: 'tree-folder',
                collapsed: true,
                lines: true,
                onExpand: function (node) {
                    if (node) {
                        LoadMenuChildNodes(node, false);
                    }
                },
                onClick: function (node) {
                    if (node) {
                        LoadMenuChildNodes(node, true);
                    }
                }
            });
            $("#pageloading").hide();
        }
        //加载子节点
        function LoadMenuChildNodes(node, expand) {
            var childNodes = $('#menuTree').tree('getChildren', node.target);
            if (childNodes && childNodes.length > 0 && childNodes[0].text == '加载中...') {
                $('#menuTree').tree('remove', childNodes[0].target);
                Application.data.getMenusOfMenuForEmp(node.id, "true", function (js) {
                    if (js && js != '[]') {
                        var pushData = eval('(' + js + ')');
                        $('#menuTree').tree('append', { parent: node.target, data: pushData });
                        if (expand) $('#menuTree').tree('expand', node.target);
                    }
                    GetTemplatePanel(node);
                }, this);
            } else {
                GetTemplatePanel(node);
            }
        }
        //加载独立表单数据
        function LoadFlowFormTree() {
            var tUrl = "FlowFormTree.aspx";
            var flowId = getArgsFromHref("FK_Flow");
            var params = {
                method: "getflowformtree",
                flowId: flowId
            };
            queryData(tUrl, params, LoadFormTreeCallBack, this);
        }
        //加载节点表单数据
        function LoadNodeFormTree() {
            var tUrl = "FlowFormTree.aspx";
            var flowId = getArgsFromHref("FK_Flow");
            var nodeId = getArgsFromHref("FK_Node");
            var params = {
                method: "getnodeformtree",
                flowId: flowId,
                nodeId: nodeId
            };
            queryData(tUrl, params, LoadFormTreeCallBack, this);
        }
        
        //初始化页面
        $(function () {
            $("#pageloading").show();
            var nodePage = getArgsFromHref("FK_Node");
            if (nodePage == "") {
                LoadFlowFormTree();
            } else {
                LoadNodeFormTree();
            }
        });
    </script>
</head>
<body>
    <div id="pageloading" style="filter: alpha(opacity=80); opacity:0.80;">
    </div>
    <div style="width: 100%; height: 100%; text-align:center; overflow: auto;">
        <ul id="flowFormTree" class="easyui-tree-line" data-options="animate:false,dnd:false">
        </ul>
    </div>
</body>
</html>
