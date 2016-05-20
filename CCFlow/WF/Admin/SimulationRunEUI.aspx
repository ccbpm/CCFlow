<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimulationRunEUI.aspx.cs"
    Inherits="CCFlow.WF.Admin.SimulationRunEUI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程模拟器</title>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script type="text/javascript">
        //加载grid后回调函数
        var selectId;
        var whichTaps;
        var getTaps = 0;
        var getText;

        function LoadDataCallBack(js, scorp) {
            $("#pageloading").hide();
            if (js == "") js = "[]";

            if (js.status && js.status == 500) {
                $("body").html("<b>访问页面出错，请联系管理员。<b>");
                return;
            }
            getSelectedTabInd();
            var pushData = eval('(' + js + ')');
            $(whichTaps).tree({
                idField: 'id',
                iconCls: 'tree-folder',
                data: pushData,
                checkbox: true,
                collapsed: true,
                animate: true,
                width: 300,
                height: 400,
                lines: true,
                onContextMenu: function (e, row) {
                    e.preventDefault();
                    $(whichTaps).tree("select", row.id);
                    $('#whatToDo').menu('show', {
                        left: e.pageX,
                        top: e.pageY
                    });
                    selectId = row.id;
                },
                onExpand: function (node) {
                    if (node) {
                    }
                },
                onClick: function (node) {
                    if (node) {
                        $(whichTaps).tree("check", node.target);
                    }
                }
            });
        }

        function LoadData() {
            var params = {
                method: "getMyData",
                getTaps: getTaps
            };
            queryData(params, LoadDataCallBack, this);
        }
        setTimeout("closeCurDlg()", 5000);

        function closeCurDlg() {
            $('#Curdlg').dialog('close');
        }
        var getEmps;
        //初始化
        $(function () {
            $('#dlg').dialog('close');
            $('#Curdlg').dialog('close');
            //自动增加Panel
            var urlEmps = Application.common.getArgsFromHref("IDs");
            getEmps = urlEmps.split(',');
            for (var i = 0; i < getEmps.length; i++) {
                if (getEmps[i] != '') {
                    index++;
                    $('#tt').tabs('add', {
                        title: '发起人编号---' + getEmps[i],
                        content: '<div style="padding:10px">Content' + index + '</div>',
                        closable: true
                    });
                }
            }


        });

        function bindMet(resetStatus) {
            if (resetStatus.className == 'model') {
                if (resetStatus.id == 'modelOne') {
                    document.getElementById("modelTwo").checked = false;
                } else {
                    document.getElementById("modelOne").checked = false;
                }
            }
            else {
                if (resetStatus.id == 'TimeSOne') {
                    document.getElementById("TimeSTwo").checked = false;
                } else {
                    document.getElementById("TimeSOne").checked = false;
                }
            }
        }

        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: "SimulationRunEUI.aspx", //要访问的后台地址
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
        //操作tabs
        var index = 0;
        function addPanel() {
            $("#dlg").dialog({
                iconCls: 'icon-user',
                buttons: [{
                    text: '确定',
                    iconCls: 'icon-ok'
                }, {
                    text: '取消',
                    handler: function () {
                        $('#dlg').dialog('close');
                    }
                }]
            });
            $('#dlg').dialog('open');
        }
        function removePanel() {
            var tab = $('#tt').tabs('getSelected');
            if (tab) {
                var index = $('#tt').tabs('getTabIndex', tab);
                $('#tt').tabs('close', index);
            }
        }
    </script>
</head>
<body>
    <div id="tt" class="easyui-tabs" data-options="tools:'#tab-tools'" style="width: auto;
        height: 600px">
        <div title="关于流程模拟器" style="padding: 10px; width: 700px; height: 250px">
        </div>
    </div>
    <div id="tab-tools">
        <a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-config'"
            onclick="addPanel()"></a><a href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-delete'"
                onclick="removePanel()"></a>
    </div>
    <div id="dlg" class="easyui-dialog" title="模拟器参数设置" style="width: 400px; height: 200px;
        padding: 10px">
        <div>
            <div>
                运行模式</div>
            <input type="radio" id="modelOne" class="model" onclick="bindMet(this)" checked="checked" />串行
            <input type="radio" id="modelTwo" class="model" onclick="bindMet(this)" />并行</div>
        <div style="margin-top: 20px;">
            <div>
                时间间隔</div>
            <input type="radio" id="TimeSOne" class="TimeS" onclick="bindMet(this)" />极速模式
            <input type="radio" id="TimeSTwo" class="TimeS" onclick="bindMet(this)" checked="checked" />间隔三秒</div>
    </div>
    <div id="CurDlg" class="easyui-dialog" title="当前节点" style="width: 400px; height: 200px;
        padding: 10px">
    </div>
</body>
</html>
