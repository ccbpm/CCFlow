<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptTree.aspx.cs" Inherits="CCFlow.WF.Comm.Port.DeptTree" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script type="text/javascript">
        //加载grid后回调函数
        function LoadDataGridCallBack(js, scorp) {
            $("#pageloading").hide();
            if (js == "") js = "[]";

            //系统错误
            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }

            var pushData = eval('(' + js + ')');

            $("#tt").tree({
                idField: 'id',
                iconCls: 'tree-folder',
                data: pushData,
                checkbox: true,
                collapsed: true,
                animate: true,
                width: 300,
                height: 400,
                lines: true,
                onExpand: function (node) {
                    if (node) {
                    }
                },
                onClick: function (node) {
                    if (node) {
                    }
                }
            });
        }
        //加载DeptTree
        function LoadDeptTreeData() {
            var FK_Node = Application.common.getArgsFromHref("FK_Node");
            var params = {
                method: "getTreeDateMet",
                FK_Node: FK_Node
            };
            queryData(params, LoadDataGridCallBack, this);
        }

        //初始化
        $(function () {
            LoadDeptTreeData();
        });

        //获取选中项id
        function getChecked() {
            var nodes = $('#tt').tree('getChecked');
            var getId = '';
            for (var i = 0; i < nodes.length; i++) {
                if (getId != '') getId += ',';
                getId += nodes[i].id;
            }

            // 获取URL传的值
            var FK_Node = Application.common.getArgsFromHref("FK_Node");
            var params = {
                method: "insertMet",
                getId: getId,
                FK_Node: FK_Node
            };
            queryData(params, function (js, scope) {
                if (js == "true")
                    $.messager.alert("提示", "保存成功!", "info");
            }, this);
        }
        //关闭
        function cancelMet() {
            window.close();
        }

        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "DeptTree.aspx", //要访问的后台地址
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
</head>
<body class="easyui-layout" style="overflow-y: hidden" fit="true">
    <div region="north" split="false" border="false" title="节点部门选择" iconcls='icon-department'
        style="height: 53px;">
        <div id="tb" style="background-color: #fafafa; height: 25px;">
            <a id="isOk" style="float: left; background-color: #fafafa; margin-left: 10px;" href="#"
                class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-save'" onclick="getChecked()">
                保存</a>&nbsp; &nbsp;<a id="delMeeting" href="#" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-cancel'"
                    onclick="cancelMet()">关闭</a>
        </div>
    </div>
    <div region="center" border="true" style="margin: 0; padding: 0; overflow: auto;">
        <ul class="easyui-tree" id="tt" style="margin-left:10px;">
        </ul>
    </div>
</body>
</html>
