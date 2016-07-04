<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SFDBSrcNewGuide.aspx.cs" Inherits="CCFlow.WF.Comm.Sys.SFDBSrcUI" %>
<%@ Register Src="~/WF/Comm/Sys/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>数据源维护向导</title>
    <link href="../Style/CommStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'center'" title="<%=this.Page.Title %>" style="padding: 5px;">
        <uc1:Pub ID="pub1" runat="server" />
    </div>
    </form>
</body>
</html>