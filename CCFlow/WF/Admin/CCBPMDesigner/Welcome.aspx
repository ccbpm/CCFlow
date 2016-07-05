﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Wel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <div style=" text-align:left">
    <h4>感谢您选择驰骋工作流程引擎</h4>

  <table style="width:99%">
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>流程引擎常用功能</legend>
                    <ul style="list-style: none">
                        <li><a href="javascript:OpenNewFlowPanel()">
                            <img src="./Img/NewFlow.png" border="0" />&nbsp;新建流程</a></li>
                        <li><a href="javascript:OpenTab('TempleteFlowSearch','关键字查询流程模版','../CCBPMDesigner/SearchFlow.aspx','icon-search');">
                            <img src="./Img/SearchKey.png" alt="" border="0" />&nbsp;关键字查询流程模版</a></li>
                        <%--<li><a href="" href="../CCBPMDesigner/SearchFlow.aspx" ><img src="./Img/SearchKey.png" alt="" border="0" />&nbsp;关键字查询流程模版</a></li>--%>
                        <li><a href="javascript:OpenTab('TempleteFlowList','流程列表','../CCBPMDesigner/Flows.aspx','icon-flows');">
                            <img src="./Img/flows.png" border="0" />&nbsp;流程列表</a></li>
                        <li><a href="javascript:OpenTab('TempleteFlowControl','流程监控','../CCBPMDesigner/App/Welcome.aspx','icon-Monitor');">
                            <img src="./Img/Monitor.png" border="0" />&nbsp;流程监控</a></li>
                        <li><a href="javascript:OpenTab('TempleteFlowGloKeys','全文检索运行流程实例','../../../WF/KeySearch.aspx','icon-SearchKey');">
                            <img src="./Img/SearchKey.png" border="0" />&nbsp;全文检索运行流程实例</a></li>
                        <li><a href="javascript:OpenTab('TempleteGeneralSearch','综合查询','../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews','icon-Search');">
                            <img src="./Img/Group.png" border="0" />&nbsp;综合查询</a> -<a href="javascript:OpenTab('TempleteGeneralAnal','综合分析','../../Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews','icon-Group');">分析</a>
                        </li>
                    </ul>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>表单引擎常用功能</legend>
                    <ul style="list-style: none">
                        <li><a href="">
                            <img src="./Img/NewFlow.png" />新建表单</a></li>
                        <li><a href="../CCBPMDesigner/SearchFlow.aspx">
                            <img src="./Img/NewFlow.png" border="0" />关键字查询表单模版</a></li>
                        <li><a href="javascript:OpenTab('NewDbSrc','新建数据源','../../Comm/Sys/SFDBSrcNewGuide.aspx?DoType=New','icon-new');">
                            <img src="./Img/NewFlow.png" />新建数据源</a></li>
                        <li><a href="javascript:OpenTab('ManageDbSrcs','数据源管理','../../Comm/Sys/SFDBSrcNewGuide.aspx','icon-sheet');">
                            <img src="./Img/NewFlow.png" />数据源管理</a></li>
                        <li><a href="">
                            <img src="./Img/NewFlow.png" />枚举列表</a></li>
                        <li><a href="">
                            <img src="./Img/NewFlow.png" />字典表列表</a></li>
                    </ul>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset >
                    <legend>更多资源</legend>
                    <ul style="list-style: none" >
                        <li><a href="http://online.ccflow.org" target="_blank">BPM工程师培训认证网</a></li>
                        <li><a href="http://jflow.cn" target="_blank">JFlow官方网站</a></li>
                        <li><a href="http://ccflow.org" target="_blank">ccflow官方网站</a></li>
                        <li><a href="http://ccbpm.mydoc.io" target="_blank">ccbpm在线手册</a></li>
                        <li><a href="http://ccform.mydoc.io" target="_blank">ccform在线手册</a></li>
                        <li><a href="http://bbs.ccflow.org" target="_blank">驰骋工作流引擎论坛</a></li>
                    </ul>
                </fieldset>
            </td>
        </tr>

        </table>
    </form>
</body>
</html>
