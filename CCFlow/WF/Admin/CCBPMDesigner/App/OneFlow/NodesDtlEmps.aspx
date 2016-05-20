<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NodesDtlEmps.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.NodesDtlEmps" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../../../Scripts/jquery/themes/default/easyui.css" rel="stylesheet"
        type="text/css" />
    <link href="../../../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../../../../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <link href="../../../../Comm/Style/Table.css" rel="stylesheet" type="text/css" />
    <link href="../../../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var fk_node;
        $(function () {
            fk_node = $('.hiddenTag').attr("id");
            $("#tabIndex_0").load("NodesDtlEmps0.aspx?FK_Node=" + fk_node);
            $('#tt').tabs({
                onSelect: function (title, index) {
                    $("#tabIndex_" + index).load("NodesDtlEmps" + index + ".aspx?FK_Node=" + fk_node);
                }
            });
        });
        var resizeTimer = null;

        window.onresize = function () {
            resizeTimer = resizeTimer ? null : setTimeout(doResize, 0);
        }

        function doResize() {
            var tab = $('#tt').tabs('getSelected');
            var index = $('#tt').tabs('getTabIndex', tab);

            $("#tabIndex_" + index).load("NodesDtlEmps" + index + ".aspx?FK_Node=" + fk_node);
        }
    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .center
        {
            text-align: center;
        }
        th
        {
            text-align: center;
            border: 1px dotted #CCCCCC;
        }
        #tabIndex_0_chart
        {
            width: auto;
            height: 350px;
        }
        .td_chart
        {
            border-bottom: none;
            background-color: #eee;
            font-size: 12px;
            font-weight: bold;
        }
        .iframeStyle
        {
            margin: 0px;
            padding: 0px;
            width: 100%;
            border: none;
            height: auto;
        }
    </style>
</head>
<body class="easyui-layout" >
    <%
        BP.WF.Node node = new BP.WF.Node(this.Request.Params["FK_Node"]);
        BP.WF.Flow flow = new BP.WF.Flow(node.FK_Flow);
        string captionContent = "流程：" + flow.Name + " 节点：" + node.Name + "";
    %>
    <div data-options="region:'north',split:false" style="height: 32px;">
        <table style="width: 100%;">
            <caption id="capTion">
                <%=captionContent%>
            </caption>
        </table>
    </div>
    <div data-options="region:'center'" class="layoutCenter" style="">
        
        <div id="tt" class="easyui-tabs">
            <div id="tabIndex_0" title="工作总量分析">
            </div>
            <div id="tabIndex_1" title="工作时长分析" data-options="" style="overflow: auto;">
            </div>
            <div id="tabIndex_2" title="工作考核分析" data-options="" style="">
            </div>
        </div>
    </div>
    <input id="<%=node.NodeID %>" class="hiddenTag" type="hidden" />
</body>
</html>
