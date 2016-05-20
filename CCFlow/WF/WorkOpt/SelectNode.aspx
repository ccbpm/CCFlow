<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectNode.aspx.cs" Inherits="CCFlow.WF.WorkOpt.SelectNode" %>

<%@ Register Assembly="BP.Web.Controls" Namespace="BP.Web.Controls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>插入步骤/流程</title>
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var selectType;

        $(function () {
            $('#tab').tabs({
                onSelect: function (title) {
                    selectType = title == '步骤' ? 'node' : 'flow';
                }
            });
        });

        //获取选中的值
        //1.如果选中的是节点步骤，则val格式为：idx,nodeid,nodetext
        //2.如果选中的是流程，则val格式为：flowno,flowname
        function getSelectedValue() {
            return selectType == 'node' ? $('#<%=lbNodes.ClientID %>').val() : $('#<%=lbFlows.ClientID %>').val();
        }

        //获取当前选中的类型
        //选中节点步骤为node，流程为flow
        function getSelectedType() {
            return selectType;
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',border:false" style="padding: 5px">
        <div id="tab" class="easyui-tabs" data-options="fit:true">
            <div title="步骤" data-options="closed:<%=this.ShowFlowOnly.ToString().ToLower() %>">
                <cc1:LB ID="lbNodes" runat="server" style="width:100%;height:100%;border:0">
                </cc1:LB>
            </div>
            <div title="流程" data-options="closed:<%=this.ShowNodeOnly.ToString().ToLower() %>">
                <cc1:LB ID="lbFlows" runat="server" style="width:100%;height:100%;border:0">
                </cc1:LB>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
