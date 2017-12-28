<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow_Comm_Sys_EnsDataIO"
    Title="数据导入导出" CodeBehind="EnsDataIO.aspx.cs" %>

<%@ Register Src="../UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>数据导入导出</title>
    <link href="../Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showpro() {
            $("#pro").show();
        }
    </script>
    <base target="_self" />
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center'">
        <div style="width:100%;height:100%; font-weight:bold; font-size: 24px; text-align: center; padding-top:200px; position:absolute; z-index:99999; left:0; top:0; display:none;" id="pro">正在导入数据，请耐心等待...</div>
        <uc1:Pub ID="Pub1" runat="server" />
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
    </form>
</body>
</html>
