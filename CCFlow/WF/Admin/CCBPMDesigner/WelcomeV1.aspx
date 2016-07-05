<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WelcomeV1.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.Welcome" %>

<%@ Register Src="UC/ChartWorks.ascx" TagName="ChartWorks" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>主页</title>
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
        .red
        {
            font-color: red;
        }
    </style>
    <script type="text/javascript">
        function OpenTab(id, name, url, icon) {
            if (window.parent != null && window.parent.closeTab != null) {
                window.parent.closeTab(name);
                window.parent.addTab(id, name, url, icon);
            }
        }
        function OpenNewFlowPanel() {
            if (window.parent && window.parent.newFlow) {
                window.parent.newFlow();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <h3>
        欢迎:
        <%=BP.Web.WebUser.Name %></h3>
    <table style="width: 100%">
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
                        <li><a href="">
                            <img src="./Img/NewFlow.png" />新建数据源</a></li>
                        <li><a href="">
                            <img src="./Img/NewFlow.png" />管理数据源</a></li>
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
        <tr>
            <td colspan="3">
                <div style="width: 100%;">
                    <uc1:ChartWorks ID="ChartWorks1" runat="server" />
                </div>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>流程模版统计</legend>
                    <%
                        int flowNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow ");
                        int flowRunNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow WHERE IsCanStart=1  ");
                        int nodeNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node ");
                        decimal avgNum = 0;
                        if (flowNum == 0)
                            avgNum = 0;
                        else
                            avgNum = nodeNum / flowNum;
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                流程总数
                            </td>
                            <td>
                                <%=flowNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                启用的流程数
                            </td>
                            <td>
                                <%=flowRunNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                节点数
                            </td>
                            <td>
                                <%=nodeNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                平均流程节点数
                            </td>
                            <td>
                                <%=avgNum.ToString("0.00")%>
                            </td>
                            <td>个</td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>表单模版统计</legend>
                    <%
                        int frmNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Sys_MapData WHERE No NOT LIKE 'ND%' ");
                        int fieldNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(MyPK) FROM Sys_MapAttr WHERE FK_MapData NOT LIKE 'ND%' ");
                        decimal avgFrmNum = 0;  // fieldNum / frmNum;
                        if (frmNum == 0)
                            avgFrmNum = 0;
                        else
                            avgFrmNum = fieldNum / frmNum;

                        int numOfRef = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(DISTINCT FK_Frm) FROM WF_FrmNode");
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                表单总数
                            </td>
                            <td>
                                <%=frmNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                字段总数
                            </td>
                            <td>
                                <%=fieldNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                平均表单字段数
                            </td>
                            <td>
                                <%=avgFrmNum.ToString("0.00")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                被流程引擎引用数
                            </td>
                            <td>
                                <%=numOfRef%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>组织结构统计</legend>
                    <%
                        int empNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Emp ");
                        int deptNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Dept ");
                        int stationNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Station ");

                        int noStationEmps = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Emp WHERE No Not IN (SELECT FK_Emp FROM Port_EmpStation ) ");

                        int noDeptEmps = 0;
                        if (BP.Sys.SystemConfig.OSModel == BP.Sys.OSModel.OneOne)
                          noDeptEmps = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Emp WHERE FK_Dept Not IN (SELECT No FROM Port_Dept) ");
                        else
                            noDeptEmps = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Port_Emp WHERE FK_Dept Not IN (SELECT No FROM Port_Dept) ");
                    %>
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                操作员数
                            </td>
                            <td>
                                <%=empNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                部门数
                            </td>
                            <td>
                                <%=deptNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                岗位数
                            </td>
                            <td>
                                <%=stationNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                无岗位数（ <font color="Red"><b>
                                    <%=noStationEmps%></b></font>） 无部门人员（<font color="Red"><b><%=noDeptEmps%></b></font>）
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>流程实例分析(合计)- <a href="../CCBPMDesigner/App/Welcome.aspx">进入流程监控</a></legend>
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                正在运行数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" +  (int)BP.WF.WFState.Complete)+"  "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                完成数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                退回中数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                删除数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Delete) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                其他运行数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFSta=" + (int)BP.WF.WFSta.Etc) + "  "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>考核信息</legend>
                    <%
                        //OverMinutes小于0表明提前 
                        string sql = "SELECT SUM(OverMinutes) FROM WF_CH WHERE  OverMinutes <0";
                        int beforeOver = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);

                        //OverMinutes大于0表明逾期
                        sql = "SELECT SUM(OverMinutes) FROM WF_CH WHERE OverMinutes >0 ";
                        int afterOver = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);


                        sql = "SELECT SUM (ASNum) AS ASNum , SUM(CSNum) CSNum ,SUM(AllNum) AllNum  FROM V_TOTALCH  ";
                        System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                        int inTimeOverCount = 0;//按时
                        int afterOverCount = 0;//超时
                        int totalCount = 0;
                        if (dt.Rows.Count == 1)
                        {
                            inTimeOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["ASNum"].ToString()) ? "0" : dt.Rows[0]["ASNum"].ToString());
                            afterOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["CSNum"].ToString()) ? "0" : dt.Rows[0]["ASNum"].ToString());
                            totalCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["AllNum"].ToString()) ? "0" : dt.Rows[0]["ASNum"].ToString());
                        }

                        //求按时办结率.
                        decimal asRate = 0;
                        if (totalCount != 0)
                            asRate = (decimal)inTimeOverCount / (decimal)totalCount * 100;

                        //在运行的逾期.
                        sql = "SELECT COUNT(WorkID) as Num  FROM WF_GenerWorkFlow WHERE SDTOfNode >='2015-07-06 10:43' AND WFState NOT IN (0,3)";
                        int runningFlowOverTime = BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    %>
                    <table>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">1、</font>提前完成分钟数
                            </td>
                            <td>
                                <%=beforeOver%>
                            </td>
                            <td>
                                分钟
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font ">2、</font>逾期分钟数
                            </td>
                            <td class="font_red">
                                <%=afterOver%>
                            </td>
                            <td>
                                分钟
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">3、</font>按时完成
                            </td>
                            <td>
                                <%=inTimeOverCount%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">4、</font>超时完成
                            </td>
                            <td class="font_red">
                                <%=afterOverCount%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">5、</font>按时办结率
                            </td>
                            <td>
                                <%=asRate.ToString("0.00") %><font class="flow_font flow_font_big">%</font>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">6、</font>在运行的逾期
                            </td>
                            <td>
                                <font class="font_red">
                                    <%=runningFlowOverTime%></font>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <fieldset>
                    <legend>表单云 </legend>
                    <table>
                        <tr>
                            <td>
                                模版总数
                            </td>
                            <td>
                                N
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                我贡献
                            </td>
                            <td>
                                N
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                最新贡献数
                            </td>
                            <td>
                                N
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <div>
    </div>
    </form>
</body>
</html>
