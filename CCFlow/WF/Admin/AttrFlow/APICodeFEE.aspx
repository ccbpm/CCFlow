<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="APICodeFEE.aspx.cs" Inherits="CCFlow.WF.Admin.AttrFlow.APICodeFEE" %>
<%@ Register TagPrefix="uc1" TagName="Pub" Src="../Pub.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=Title %>自定义事件代码生成</title>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <link href="../../Scripts/SyntaxHighlighter/Styles/shCoreDefault.css" rel="stylesheet"
        type="text/css" />
    <script src="../../Scripts/SyntaxHighlighter/shCore.js" type="text/javascript"></script>
    <script src="../../Scripts/SyntaxHighlighter/shBrushCSharp.js" type="text/javascript"></script>
</head>
<body class="easyui-layout" data-options="border:false">
    <form id="form1" runat="server">
        <div data-options="region:'center',border:false,title:'<%=Title %>自定义事件代码生成'," style="padding: 5px">        
            <uc1:Pub ID="Pub1" runat="server" />
    </div>
    </form>
</body>
</html>
