<%@ Page Title="组织结构数据初始化" Language="C#" AutoEventWireup="true" CodeBehind="InitOrg.aspx.cs" Inherits="CCFlow.WF.Comm.Sys.InitOrg" %>

<%@ Register Src="~/WF/Comm/UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>组织结构数据初始化</title>
    <link href="../Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',title:'组织结构数据初始化',collapsible:false" style="line-height: 30px; overflow: hidden;
        height: 70px; padding: 5px">
        组织结构Excel文件(*.xls)：<asp:FileUpload ID="file" runat="server" Width="400" />&nbsp;
        <asp:Button ID="btnImport" runat="server" Text="批量导入" onclick="btnImport_Click" />
        <asp:HyperLink ID="modelfile" runat="server">ccflow组织结构批量导入模板.xls</asp:HyperLink>
    </div>
    <div data-options="region:'center',title:'处理数据'">
        <div id="results" class="easyui-tabs" data-options="fit:true">
            <div title="岗位">
                <uc1:Pub ID="pub1" runat="server" />
            </div>
            <div title="部门">
                <uc1:Pub ID="pub2" runat="server" />
            </div>
            <div title="人员">
                <uc1:Pub ID="pub3" runat="server" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
