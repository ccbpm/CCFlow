<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Integration.aspx.cs" Inherits="CCFlow.WF.Admin.Org.Default" %>

<%@ Register Src="~/WF/Comm/UC/Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>集成设置</title>
    <link href="../../Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <link href="../../Scripts/easyUI/themes/gray/easyui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
</head>
<body class="easyui-layout" data-options="title:'组织机构集成',border:false">
    <form id="form1" runat="server">
    <div data-options="region:'west',border:false" style="width: 300px; padding: 5px">
        <fieldset>
            <legend>
                <img src='/WF/Img/Setting.png' border="0" />当前定义内容</legend>
            <uc1:Pub ID="Pub3" runat="server" />
<%--            <ul>
                <li>组织机构模式：<%=BP.Sys.SystemConfig.OSModel %> </li>
                <li>组织机构维护方式：<%=BP.Sys.SystemConfig.OSModel%> </li>
                <li>组织机构维护方式：<%=BP.Sys.SystemConfig.OSModel%> </li>
                <li>组织机构来源：<%=BP.Sys.SystemConfig.OSModel%> </li>
            </ul>--%>
        </fieldset>
        <fieldset>
            <legend><a href="http://ccbpm.mydoc.io" target="_blank">
                <%=BP.WF.Glo.GenerHelpCCForm("帮助",null,null)%></legend>
            <ul>
                <li>组织结构就是整个应用系统的操作员，部门、岗位、以及三者之间的关系。</li>
                <li>在ccbpm里，我们根据不用的应用环境需要，支持了一个用户一个部门，多个岗位的模式，与一个用户多个部门，多岗位的模式。</li>
                <li>在前一个模式里，我们支持</li>
            </ul>
        </fieldset>
    </div>
    <div data-options="region:'center',border:false" style="padding: 5px">
        <fieldset style="padding: 10px">
            <legend>
                <img src='/WF/Img/Guide.png' width="16" border="0" /><%=StepTitle %></legend>
            <uc1:Pub ID="Pub1" runat="server" />
        </fieldset>
    </div>
    <div class="easyui-window" id="datawin" data-options="closed:true,modal:true" title="数据(前100条)"
        style="width: 760px; height: 470px">
        <uc1:Pub ID="Pub2" runat="server" />
    </div>
    </form>
</body>
</html>
