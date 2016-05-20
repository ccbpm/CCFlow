<%@ Page Language="C#" AutoEventWireup="true" 
CodeBehind="PriFlow.aspx.cs" 
Inherits="CCFlow.WF.Admin.Clound.PriFlow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<div id="LoadingBar" style="margin-left: auto; margin-right: auto; margin-top: 40%;
    width: 250px; height: 38px; line-height: 38px; padding-left: 50px; padding-right: 5px;
    background: url(../../Scripts/easyUI/themes/default/images/pagination_loading.gif) no-repeat scroll 5px 

10px; border: 2px solid #95B8E7; color: #696969; font-family: 'Microsoft YaHei'">
    正在连接到云服务器,请稍候…
</div>
<head id="Head1" runat="server">
    <title></title>
    <link href="../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="js/loading.js" type="text/javascript"></script>
    <script type="text/javascript">
        var met = ""; ;
        var dir = "";
        $(function () {
            $("#pageloading").show();
            loadTreeData();
            met = "getRecentlyPriFlowTemp";
            LoadGridData(1, 20);
        });
        function loadTreeData() {
            var params = {
                method: "getPriFlowDir"
            };
            queryData(params, function (js) {
                $("#pageloading").hide();
                if (js == "") js = "[]";
                if (js.status && js.status == 500) {
                    $("body").html("<b>访问页面出错，请联系管理员。<b>");
                    return;
                }
                var pushData = eval('(' + js + ')');

                $("#tt").tree({
                    idField: 'id',
                    data: pushData,
                    animate: true,
                    lines: true,
                    dnd: true,
                    onClick: function (node) {
                        var nodes;
                        try {
                            nodes = $('#tt').tree('getChildren', node.target);
                        } catch (e) {
                            nodes = node;
                        }

                        var ids = [];
                        ids.push(node.id);
                        for (var i = 0; i < nodes.length; i++) {
                            ids.push(nodes[i].id);
                        }

                        met = "getPriFlowTempByDir";
                        dir = ids.join(',');
                        $('.layout-panel-center  .panel-title').html(node.text);
                        document.getElementById("TB_KeyWords").value = "";
                        LoadGridData(1, 20);
                    }
                });
            }, this);
        }
        function LoadGridData(pageNumber, pageSize) {
            this.pageNumber = pageNumber;
            this.pageSize = pageSize;
            var keyWords = replaceTrim(document.getElementById("TB_KeyWords").value);

            if (!met)
                met = "getRecentlyPriFlowTemp";

            var params = {
                method: met,
                keyWords: encodeURI(keyWords),
                pageNumber: pageNumber,
                pageSize: pageSize,
                dir: dir
            };
            queryData(params, LoadDataGridCallBack, this);
        }
        function rowDetail(no, name) {
            window.parent.closeTab('流程信息');
            window.parent.addTab('PriFlowTemOne', '流程信息', '../../../WF/Admin/Clound/PriFlowTemOne.aspx?GUID=' + no, '');
        }
        function LoadDataGridCallBack(js, scorp) {
            $("#pageloading").hide();
            if (js == "") js = "[]";

            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }

            var pushData = eval('(' + js + ')');
            $('#newsGrid').datagrid({
                columns: [[
                    { field: 'NAME', title: '流程', width: 200, align: 'left', formatter: function (value, row, index) {
                        return "<a href='#' onclick='rowDetail(\"" + row.NO + "\",\"" + row.NAME + "\")'>" + row.NAME + ".xml</a>";
                    }
                    },
                    { field: 'DIRNAME', title: '类别', width: 50, align: 'center' },
                    { field: 'NODENUM', title: '节点数', width: 80, align: 'center' }
                ]],
                idField: 'NO',
                selectOnCheck: false,
                checkOnSelect: true,
                singleSelect: true,
                data: pushData,
                width: 'auto',
                height: 'auto',
                striped: true,
                rownumbers: true,
                pagination: true,
                remoteSort: false,
                fitColumns: true,
                pageNumber: scorp.pageNumber,
                pageSize: scorp.pageSize,
                pageList: [20, 30, 40, 50],
                loadMsg: '数据加载中......'
            });
            //分页
            var pg = $("#newsGrid").datagrid("getPager");
            if (pg) {
                $(pg).pagination({
                    onRefresh: function (pageNumber, pageSize) {
                        LoadGridData(pageNumber, pageSize);
                    },
                    onSelectPage: function (pageNumber, pageSize) {
                        LoadGridData(pageNumber, pageSize);
                    }
                });
            }
        }
        function searchData() {
            $('.layout-panel-center  .panel-title').html("流程查询");
            met = "getSearchPriData";
            LoadGridData(1, 20);
        }
        function RefreshGrid() {
            var grid = $('#newsGrid');
            var options = grid.datagrid('getPager').data("pagination").options;
            var curPage = options.pageNumber;
            var pageSize = options.pageSize;
            LoadGridData(curPage, pageSize);
        }
        function replaceTrim(val) {
            return val.replace(/[ ]/g, "");
        }
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "PriFlow.aspx", //要访问的后台地址
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
        //私有云流程类别的操作
        function addNode() {
            var parentNode = $('#tt').tree('getSelected');
            var parentNodeId = "";

            if (parentNode) {
                parentNodeId = parentNode.id;
            }

            if (parentNodeId) {
                $.messager.prompt('添加流程类别', '请输入流程类别', function (r) {
                    if (r) {
                        var params = {
                            method: "addPriFlowDir",
                            parentNo: parentNodeId,
                            nodeName: encodeURI(replaceTrim(r))
                        };
                        queryData(params, function (js) {
                            if (js == "true") {
                                loadTreeData();
                            } else
                                $.messager.alert("提示", "操作失败", "error");
                        }, this);
                    }
                });
            } else {
                $.messager.alert("提示", "没有选中节点", "info");
            }
        }
        function editNode() {
            var parentNode = $('#tt').tree('getSelected');
            var parentNodeId = "";

            if (parentNode) {
                parentNodeId = parentNode.id;
            }
            if (parentNodeId) {
                $.messager.prompt('编辑流程类别', '请输入流程类别', function (r) {
                    if (r) {
                        var params = {
                            method: "editPriFlowDir",
                            no: parentNodeId,
                            nodeName: encodeURI(replaceTrim(r))
                        };
                        queryData(params, function (js) {
                            if (js == "true") {
                                loadTreeData();
                            } else
                                $.messager.alert("提示", "操作失败", "error");
                        }, this);
                    }
                });
            }
            else {
                $.messager.alert("提示", "没有选中节点", "info");
            }
        }
        function deleteNode() {
            var parentNode = $('#tt').tree('getSelected');
            var parentNodeId = "";

            if (parentNode) {
                parentNodeId = parentNode.id;
            }
            if (parentNodeId) {
                var nodes = "";
                try {
                    nodes = $('#tt').tree('getChildren', parentNode.target)
                } catch (e) {
                    nodes = "";
                }
                if (nodes.length > 0) {
                    $.messager.alert("提示", "目录【" + parentNode.text + "】下有子目录,禁止删除.", "error");
                    return;
                }

                $.messager.confirm('确认对话框', '确定删除吗?目录【' + parentNode.text + '】下的流程也将被删除!', function (r) {
                    if (r) {
                        var params = {
                            method: "deletePriFlowDir",
                            no: parentNodeId
                        };
                        queryData(params, function (js) {
                            if (js == "true") {
                                loadTreeData();
                            } else
                                $.messager.alert("提示", "操作失败", "error");
                        }, this);
                    }
                });
            }
            else {
                $.messager.alert("提示", "没有选中节点", "info");
            }
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="ff" method="post" runat="server">
    <div id="pageloading">
    </div>
    <div data-options="region:'west',title:'流程模板类别',split:true" style="width: 250px;">
        <div style="padding: 5px; height: auto; background-color: #E0ECFF; padding-left: auto;
            padding-right: auto;">
            &nbsp;&nbsp; <a href="#" class="easyui-linkbutton" onclick="addNode();" data-options="iconCls:'icon-add',plain:true">
                新增</a> <a href="#" class="easyui-linkbutton" onclick="editNode();" data-options="iconCls:'icon-edit',plain:true">
                    编辑</a> <a href="#" class="easyui-linkbutton" onclick="deleteNode();" data-options="iconCls:'icon-delete',plain:true">
                        删除</a>
        </div>
        <ul class="easyui-tree" id="tt" style="margin-left: 10px;">
        </ul>
    </div>
    <div data-options="region:'center',title:'最新上传模板'" style="background: #eee;">
        <div id="tb" style="padding: 5px; height: auto; background-color: #E0ECFF;">
            关键字&nbsp;&nbsp;<input id="TB_KeyWords" type="text" style="width: 250px; border-style: solid;
                border-color: #aaaaaa;" />
            <a href="#" class="easyui-linkbutton" onclick="searchData();" data-options="iconCls:'icon-search',plain:true">
                查询</a>
        </div>
        <table id="newsGrid" fit="true" toolbar='#tb' fitcolumns="true" class="easyui-datagrid">
        </table>
    </div>
    </form>
</body>
</html>
