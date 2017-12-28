<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DTC.aspx.cs" Inherits="CCFlow.WF.WorkOpt.DTC" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流转自定义（动态生成配置Dom）</title>
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI15/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI15/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI15/jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI15/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../Comm/JS/Calendar/skin/WdatePicker.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" src="../Comm/JS/Calendar/WdatePicker.js" defer="defer"
        type="text/javascript"></script>
    <script type="text/javascript">
        var flowNo;
        var workid;

        function startFlow() {
            flowNo = $('#flows').combobox('getValue');

            if (!flowNo || flowNo.length == 0) {
                workid = 0;
                $('#workid').textbox('setValue', '');
                $.messager.alert('错误', '请选择流程！', 'error');
                return;
            }

            queryData({ method: 'startflow', flowNo: flowNo }, function (data) {
                var re = $.parseJSON(data);
                if (!re.success) {
                    $.messager.alert('错误', '发起流程出现错误：' + re.msg + '！', 'error');
                    return;
                }

                workid = parseInt(re.msg);
                $('#workid').textbox('setValue', workid);
            });
        }

        function setTransferConfig() {
            flowNo = $('#flows').combobox('getValue');

            if (!flowNo || flowNo.length == 0) {
                workid = 0;
                $('#workid').textbox('setValue', '');
                $.messager.alert('错误', '请选择流程！', 'error');
                return;
            }

            $('#aa').panel('refresh', "TransferCustomSimple.aspx?flowNo=" + flowNo);
            $('#bb').panel('refresh', "../Admin/CCBPMDesigner/truck/TruckSimple.aspx?FK_Flow=" + flowNo + "&WorkID=" + $('#workid').textbox('getValue') + "&FID=0");
            return;

            var v = $('#workid').textbox('getValue');
            if (!v || v.length == 0) {
                $.messager.alert('错误', '请输入workid！', 'error');
                return;
            }

            queryData({ method: 'settransfer', flowNo: flowNo, workId: v }, function (data) {
                $('#nodes tr').remove(':not(:eq(0))');
                var re = $.parseJSON(data);
                if (!re.success) {
                    $.messager.alert('错误', '发起流程出现错误：' + re.msg + '！', 'error');
                    $('#workid').textbox('setValue', '');
                    return;
                }

                workid = re.msg.workid;

                //生成处理人设置区域html
                var html;
                var node;
                var es;

                for (var i = 0; i < re.msg.nodes.length; i++) {
                    node = re.msg.nodes[i];
                    html = "<tr><td style='text-align:center;'>" + node.id + "</td>"
                        + "<td>" + node.name + "</td>"
                        + "<td>";

                    if (node.isPass) {
                        //开始节点、或已经处理过的节点
                        html += node.empNames;
                    }
                    else {
                        es = "[";

                        $.each(node.empNos.split(','), function () {
                            if (this.length > 0) {
                                es += "'" + this + "',";
                            }
                        });

                        if (es.length > 1) {
                            es = es.substring(0, es.length - 1);
                        }

                        es += "]";
                        html += "<input id='emps_" + node.id + "' class='easyui-combobox' style='width: 220px;' "
                        + "data-options=\"url:'DTC.aspx?method=findemps&workId=" + v + "&nodeId=" + node.id
                        + "',method:'get',multiple:true,valueField:'no',textField:'name',groupField:'dept'";

                        if (es == "[]") {
                            html += "\" />";
                        }
                        else {
                            html += ",onLoadSuccess:function(){$(this).combobox('setValues', " + es + ");}\" />";
                        }
                    }

                    html += "</td><td>";

                    if (node.isPass) {
                        html += node.plan;
                    }
                    else {
                        html += '<input id="plan_' + node.id + '" type="text" class="Wdate" style="width:100px;" onfocus="WdatePicker()" value="' + node.plan + '"/>';
                    }

                    html += "</td>";

                    $('#nodes').append(html);
                    $.parser.parse('#nodes');
                }
            });
        }

        function saveConfig() {
            if ($('#nodes tr').length == 1) {
                $.messager.alert('错误', '请先进行流转配置！', 'error');
                return;
            }

            var nodeEmps = $("input[id*='emps_']");
            var d = { method: 'savecfg', data: '', workid: workid };
            var emp;
            var empName;
            var sdt;
            var nodeid;

            $.each(nodeEmps, function () {
                nodeid = this.id.split('_')[1];
                emp = $(this).combobox('getValue');
                empName = $(this).combobox('getText');
                sdt = $('#plan_' + nodeid).val();
                d.data += nodeid + "_" + (emp ? emp : '') + "_" + (empName ? empName : '') + "_" + (sdt ? sdt : '') + "|";
            });

            d.data = encodeURIComponent(d.data);

            queryData(d, function (data) {
                alert($.parseJSON(data).msg);
            });
        }

        //公共方法
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method, //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                traditional: true,
                contentType: "application/json; charset=utf-8",
                url: "DTC.aspx", //要访问的后台地址
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
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center'" title="流转自定义（动态生成配置Dom）Demo" style="padding: 5px;">
        <table class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 100%">
            <tr>
                <td class="GroupTitle" style="width: 140px;">
                    项目
                </td>
                <td class="GroupTitle">
                    值
                </td>
            </tr>
            <tr>
                <td class="GroupField">
                    流程：
                </td>
                <td>
                    <input id="flows" class="easyui-combobox" style="width: 280px;" data-options="
				        url: 'DTC.aspx?method=getflows',
				        method: 'get',
				        valueField:'NO',
				        textField:'NAME',
				        groupField:'SORT',
                        onSelect: function(item){
                            $('#workid').textbox('setValue', '');
                            $('#nodes tr').remove(':not(:eq(0))');
                        }
			        ">
                    &nbsp; <a href="javascript:void(0)" onclick="startFlow()" class="easyui-linkbutton">
                        发起流程</a> &nbsp; 流程WorkId:
                    <input id="workid" class="easyui-textbox" style="width: 100px" />&nbsp;<a href="javascript:void(0)"
                        onclick="setTransferConfig()" class="easyui-linkbutton"> 流转配置</a>
            </tr>
            <tr>
                <td class="GroupField">
                    流转配置：
                </td>
                <td style="padding: 5px;">
                    <div class="easyui-panel" id="aa" style="height:auto;width:100%;"></div>
                    <%--<table id="nodes" class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 100%;
                        line-height: 26px;">
                        <tr>
                            <td style="width: 80px; text-align: center;" class="GroupTitle">
                                ID
                            </td>
                            <td style="width: 140px" class="GroupTitle">
                                步骤
                            </td>
                            <td style="width: 240px" class="GroupTitle">
                                处理人
                            </td>
                            <td class="GroupTitle">
                                预计处理日期
                            </td>
                        </tr>
                    </table>--%>
                </td>
            </tr>
            <tr>
                <td class="GroupField">
                    流转进度：
                </td>
                <td style="padding: 5px;">
                    <div class="easyui-panel" id="bb" style="height:auto;width:100%;"></div>
                </td>
            </tr>
        </table>
        <br />
        &nbsp;&nbsp; <a href="javascript:void(0)" onclick="saveConfig()" class="easyui-linkbutton"
            data-options="iconCls:'icon-save'">保存</a>
    </div>
    </form>
</body>
</html>
