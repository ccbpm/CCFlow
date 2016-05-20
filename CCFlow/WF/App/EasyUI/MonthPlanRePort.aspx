<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthPlanRePort.aspx.cs"
    Inherits="CCFlow.AppDemoLigerUI.MonthPlanRePort" %>

<%@ Register Src="UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript" type="text/javascript">
         function init() {
             var LODOP; //声明为全局变量     
             function PrintOneURL() {
                 LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));
                 LODOP.PRINT_INIT("打印月计划");
                 LODOP.ADD_PRINT_TABLE(10, 10, "90%", "100%", document.getElementById("monthReport").innerHTML);
                 LODOP.SET_PRINT_STYLEA(0, "HOrient", 3);
                 LODOP.SET_PRINT_STYLEA(0, "VOrient", 3);
                 LODOP.SET_SHOW_MODE("HIDE_PAPER_BOARD", 1);
                 LODOP.SET_SHOW_MODE("LANDSCAPE_DEFROTATED", 1); //横向时的正向显示
                 LODOP.ADD_PRINT_TEXT(580, 660, 165, 22, "第#页/共&页");
                 LODOP.SET_PRINT_STYLEA(0, "ItemType", 2);
                 LODOP.SET_PRINT_STYLEA(0, "Horient", 1);
                 LODOP.SET_PRINT_STYLEA(0, "Vorient", 1);
                 LODOP.SET_PRINT_STYLEA(0, "TableHeightScope", 2);
                 LODOP.PREVIEW();
             }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%;">
        <uc1:Pub ID="monthReport" runat="server" />
    </div>
    </form>
</body>
</html>
