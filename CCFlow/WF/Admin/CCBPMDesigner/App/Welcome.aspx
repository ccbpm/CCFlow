<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.Welcome" %>

<%@ Register src="../UC/ChartWorks.ascx" tagname="ChartWorks" tagprefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>主页</title>
    <script src="../../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../../../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <script src="../../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        .flow_info
        {
            padding-left: 15px;
        }
        .flow_font
        {
            color: Blue;
        }
        .flow_font_big
        {
            font-size: 14px;
            margin-left: 4px;
        }
        .font_red
        {
            color: Red;
        }
        .chart_div
        {
            width: 800px;
            height: 330px;
            margin: 0px auto;
        }
        .chart_div_con
        {
            width: 800px;
            height: 300px;
            float: left;
        }
        .chart_div_footer
        {
            width: 100%;
            height: 30px;
            text-align: center;
            line-height: 30px;
        }
        input
        {
            vertical-align: text-top;
            margin-top: 0;
        }
    </style>
    <script type="text/javascript">
        function loadFlowUSLChartType() {
            var str1 = $('input:radio[name=anaTime]:checked').val();
            var str2 = $('input:radio[name=flowSort]:checked').val();

            window.location.href = "Welcome.aspx?anaTime=" + str1 + "&flowSort=" + str2 + "&";
        }
        $(function () {
            $("#" + Application.common.getArgsFromHref("anaTime")).attr("checked", true);
            $("#" + Application.common.getArgsFromHref("flowSort")).attr("checked", true);
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width: 100%; min-width: 750px;">
        <caption>
            欢迎:
            <%=BP.Web.WebUser.Name %>
        </caption>
        <tr>
            <td valign="top">
                <fieldset>
                    <legend>流程引擎信息</legend>
                    <%
                        int totalFlow = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow ");
                        int runFlowNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM WF_Flow WHERE IsCanStart=1  ");
                        int nodeNum = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(NodeID) FROM WF_Node ");

                        //平均每流程发起数量。
                        decimal avgNum = 0;
                        try
                        {
                            avgNum = (decimal)nodeNum / (decimal)totalFlow;
                        }
                        catch (Exception)
                        {
                        }

                        //流程启用比率。
                        decimal flowRate = 0;
                        try
                        {
                            flowRate = (decimal)runFlowNum / (decimal)totalFlow * 100;
                        }
                        catch (Exception)
                        {
                        } 
         
                    %>
                    <table style="width: 80%;">
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">1、</font>流程设计总数
                            </td>
                            <td>
                                <%=totalFlow%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">2、</font>节点总数
                            </td>
                            <td>
                                <%=nodeNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">3、</font>平均流程节点数
                            </td>
                            <td>
                                <%=avgNum.ToString("0.00")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">4、</font>启用的流程数
                            </td>
                            <td>
                                <%=runFlowNum%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">5、</font>流程启用比率
                            </td>
                            <td>
                                <%= flowRate.ToString("0.00")%><font class="flow_font flow_font_big">%</font>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">6、</font>平均每流程启动数
                            </td>
                            <td>
                                <%= flowRate.ToString("0")%>
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
                    <legend>流程实例分析 </legend>
                    <table style="width: 80%;">
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">1、</font>流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">2、</font>正在运行数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" +  (int)BP.WF.WFState.Complete)+"  "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">3、</font>完成数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font ">4、</font>退回中数
                            </td>
                            <td class="font_red">
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">5、</font>删除数
                            </td>
                            <td class="font_red">
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Delete) + " "%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td class="flow_info">
                                <font class="flow_font">6、</font>其他运行数
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


                        sql = "SELECT SUM (ASNum) AS ASNum , SUM(CSNum) CSNum ,SUM(AllNum) AllNum FROM V_TOTALCH  ";
                        System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                        int inTimeOverCount = 0;//按时
                        int afterOverCount = 0;//超时
                        int totalCount = 0;
                        if (dt.Rows.Count == 1)
                        {
                            inTimeOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["ASNum"].ToString()) ? "0" : dt.Rows[0]["ASNum"].ToString());
                            afterOverCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["CSNum"].ToString()) ? "0" : dt.Rows[0]["CSNum"].ToString());
                            totalCount = int.Parse(string.IsNullOrEmpty(dt.Rows[0]["AllNum"].ToString()) ? "0" : dt.Rows[0]["AllNum"].ToString());
                        }

                        //求按时办结率.
                        decimal asRate = 0;
                        if (totalCount == 0)
                            asRate = 0;
                        else
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
                            <td>分钟 </td>
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
        </tr>
        <tr>
            <td colspan="3">
                <div style="width: 100%;">
                    <uc2:ChartWorks ID="ChartWorks1" runat="server" />
                </div>
            </td>
        </tr>
    </table>
    <div>
    </div>
    </form>
</body>
</html>
