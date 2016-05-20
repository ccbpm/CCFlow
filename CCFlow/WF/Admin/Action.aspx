<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_Action"
    CodeBehind="Action.aspx.cs" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>事件</title>
    <link href="../Comm/Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var showMsg = <%=this.HaveMsg.ToString().ToLower() %>;

        $(document).ready(function () {
            var currEventGroup = '<%= this.CurrentEventGroup %>';

            $("#eventAccordion").accordion("select", currEventGroup);

            var urlParams = "?NodeID=<%=this.NodeID %>&MyPK=<%=this.MyPK %>&Event=<%=this.Event %>&FK_MapData=<%=this.FK_MapData %>&FK_Flow=<%=this.FK_Flow %>&tk=";
            $("#src1").attr("src","ActionEvent.aspx" + urlParams + Math.random());

            if(showMsg){
                $("#src2").attr("src","ActionPush2Curr.aspx" + urlParams + Math.random());
                $("#src3").attr("src","ActionPush2Spec.aspx" + urlParams + Math.random());
            }
        });
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center',title:'<%=this.Title %>',border:false">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'east',noheader:true" style="width: 200px;">
                <div id="eventAccordion" class="easyui-accordion" data-options="fit:true,border:false,animate:false">
                    <uc1:Pub ID="Pub1" runat="server" />
                </div>
            </div>
            <div data-options="region:'center',title:'<%=this.CurrentEvent %>'" style="padding: 5px;">
                <uc1:Pub ID="Pub2" runat="server" />
            </div>
    </div>
    </div>
    </form>
</body>
</html>
