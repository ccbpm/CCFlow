<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChartWorks.ascx.cs"
    Inherits="CCFlow.WF.Admin.CCBPMDesigner.UC.ChartWorks" %>
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
<table style="margin: 0 auto; width: 100%;">
    <tr>
        <td valign="top" colspan="3" style="padding-left: 10px; padding-right: 40px; border-bottom: 1px solid white;">
            流程图表分析:
            <input style="margin-left: 32px;" type="radio" id="slMouth" name="anaTime" onclick="loadFlowUSLChartType();"
                value="slMouth" checked="checked" /><label for="slMouth">按月（最近12月）</label>
            <input type="radio" id="slWeek" name="anaTime" onclick="loadFlowUSLChartType();"
                value="slWeek" /><label for="slWeek">按周（最近15周）</label>
            <input type="radio" id="slDay" name="anaTime" onclick="loadFlowUSLChartType();" value="slDay" /><label
                for="slDay">按日（最近10天）</label>&nbsp;&nbsp;&nbsp;分析维度：
            <input type="radio" id="slFlow" name="flowSort" onclick="loadFlowUSLChartType();"
                value="slFlow" checked="checked" /><label for="slFlow">流程实例分析</label>
            <input type="radio" id="cqUnDoFlow" name="flowSort" onclick="loadFlowUSLChartType();"
                value="cqUnDoFlow" /><label for="cqUnDoFlow">超期完成流程分布</label>
            <%-- <div id="fcexpDiv" style="margin-top: -2px; float: right;">
            </div>--%>
        </td>
    </tr>
    <tr>
        <td valign="top" colspan="3" style="height: 300px;">
            <div class="chart_div">
                <%
                    StringBuilder sBuilder = null;

                    System.Data.DataTable dt = null;
                    string sql = "";

                    string sl1 = this.Request.Params["anaTime"];
                    string sl2 = this.Request.Params["flowSort"];

                    if (string.IsNullOrWhiteSpace(sl1))
                        sl1 = "slMouth";

                    if (string.IsNullOrWhiteSpace(sl2))
                        sl2 = "slFlow";

                    sBuilder = new StringBuilder();
                    string exportStr = "exportEnabled='1' exportAtClient='0'" +
                                       " exportAction='download' " +
                                       " exportHandler='" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Charts/FCExport.aspx' " +
                                       " exportDialogMessage='正在生成,请稍候...'  " +
                                       " exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件'";
                    string chartTitle = "";//图表title
                    DateTime dTime = DateTime.Now;

                    dt = new System.Data.DataTable();
                    switch (sl1)
                    {
                        #region     取最近一年
                        case "slMouth":
                            DateTime mouthTime = new DateTime();
                            sql = "SELECT COUNT(WorkID) NUM,FK_NY FROM  WF_GENERWORKFLOW" +
                                          " WHERE WFState!=0 GROUP BY FK_NY ORDER BY FK_NY";
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            bool isAppend = true;
                            switch (sl2)
                            {
                                case "slFlow":
                                    for (int i = -11; i <= 0; i++)
                                    {
                                        isAppend = true;
                                        mouthTime = dTime.AddMonths(i);

                                        foreach (System.Data.DataRow dr in dt.Rows)
                                        {
                                            if (dr["FK_NY"].ToString() == mouthTime.ToString("yyyy-MM"))
                                            {
                                                sBuilder.Append("<set label='" + mouthTime.ToString("yyyy-MM") + "' value='" + dr["Num"] + "' />");
                                                isAppend = false;
                                                break;
                                            }
                                        }

                                        if (isAppend)
                                            sBuilder.Append("<set label='" + mouthTime.ToString("yyyy-MM") + "' value='0' />");
                                    }
                                    
                                    chartTitle = dTime.AddMonths(-11).ToString("yyyy-MM") + "至" + dTime.ToString("yyyy-MM") + "流程实例分析";
                                    break;
                                case "cqUnDoFlow":
                                    sql = "  SELECT COUNT(DISTINCT FK_FLOW) NUM,FK_NY " +
                                        "FROM WF_CH  WHERE CHSTA='3'  GROUP BY FK_NY";
                                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                                    for (int i = -11; i <= 0; i++)
                                    {
                                        isAppend = true;
                                        mouthTime = dTime.AddMonths(i);

                                        foreach (System.Data.DataRow dr in dt.Rows)
                                        {
                                            if (dr["FK_NY"].ToString() == mouthTime.ToString("yyyy-MM"))
                                            {
                                                sBuilder.Append("<set label='" + mouthTime.ToString("yyyy-MM") + "' value='" + dr["Num"] + "' />");
                                                isAppend = false;
                                                break;
                                            }
                                        }

                                        if (isAppend)
                                            sBuilder.Append("<set label='" + mouthTime.ToString("yyyy-MM") +
                                                            "' value='0' />");
                                    }
                                    
                                    chartTitle = dTime.AddMonths(-11).ToString("yyyy-MM") + "至" + dTime.ToString("yyyy-MM") + "超期完成流程分析";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        #endregion

                        #region     15周
                        case "slWeek":
                            int spanCount = dTime.DayOfWeek - DayOfWeek.Monday;
                            if (spanCount == -1) spanCount = 6;
                            TimeSpan ts = new TimeSpan(spanCount, 0, 0, 0);
                            DateTime firstDay = dTime.Subtract(ts);

                            List<string> fifteenWeekList = new List<string>();

                            //数组存放15周的第一天
                            for (int span = -14; span <= 0; span++)
                            {
                                DateTime ddd = firstDay.AddDays(span * 7);
                                fifteenWeekList.Add(ddd.ToString("yyyy-MM-dd"));
                            }

                            DateTime nextDayTime = new DateTime();

                            switch (sl2)
                            {
                                case "slFlow":
                                    for (int i = 0; i < 15; i++)
                                    {
                                        nextDayTime = DateTime.Parse(fifteenWeekList[i]).AddDays(7);

                                        sBuilder.Append("<set label='" + fifteenWeekList[i] + " " + nextDayTime.ToString("yyyy-MM-dd") +
                                                        "' value='" + BP.DA.DBAccess.RunSQLReturnCOUNT("SELECT  WORKID FROM                                                               WF_GenerWorkFlow" +
                                                        " WF_GenerWorkFlow WHERE WFState!=0 AND RDT >= '" + fifteenWeekList[i]
                                                         + "' AND RDT<='" + nextDayTime.ToString("yyyy-MM-dd") +
                                                         "' ") + "' />");
                                    }
                                    
                                    chartTitle = "15周内流程实例分析";
                                    break;
                                case "cqUnDoFlow":
                                    for (int i = 0; i < 15; i++)
                                    {
                                        nextDayTime = DateTime.Parse(fifteenWeekList[i]).AddDays(7);

                                        sBuilder.Append("<set label='" + fifteenWeekList[i] + " " + nextDayTime.ToString("yyyy-MM-dd") +
                                                        "' value='" + BP.DA.DBAccess.RunSQLReturnCOUNT("SELECT distinct FK_Flow" +
                                                        " FROM WF_CH WHERE CHSta='3' AND DTFrom >='" + fifteenWeekList[i] +
                                                        "' AND DTTo <='" + nextDayTime.ToString("yyyy-MM-dd") + "'") + "' />");
                                    }
                                    
                                    chartTitle = "超期完成流程分析(15周内)";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        #endregion     周

                        #region     10天
                        case "slDay":
                            switch (sl2)
                            {
                                case "slFlow":
                                    sql = "SELECT COUNT(WorkID) NUM, "+BP.Sys.SystemConfig.AppCenterDBSubstringStr+"(RDT,0,11) RDT FROM" +
                                        " WF_GENERWORKFLOW WHERE WFSTATE!=0 " +
                                        " GROUP  BY " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(rdt,0,11) ORDER BY SUBSTRING(RDT,0,11)";
                                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                                    for (int i = -9; i <= 0; i++)
                                    {
                                        isAppend = true;
                                        nextDayTime = dTime.AddDays(i);
                                        foreach (System.Data.DataRow dr in dt.Rows)
                                        {
                                            if (nextDayTime.ToString("yyyy-MM-dd") == dr["RDT"].ToString().Trim())
                                            {
                                                isAppend = false;
                                                sBuilder.Append("<set label='" + nextDayTime.ToString("yyyy-MM-dd") + "' value='" + dr["Num"] + "' />");
                                                break;
                                            }
                                        }

                                        if (isAppend)
                                            sBuilder.Append("<set label='" + nextDayTime.ToString("yyyy-MM-dd") + "' value='0' />");
                                    }
                                    
                                    chartTitle = dTime.AddDays(-9).ToString("yyyy-MM-dd") + "至" + dTime.ToString("yyyy-MM-dd") + "流程实例分析";
                                    break;
                                case "cqUnDoFlow":
                                    for (int i = -9; i <= 0; i++)
                                    {
                                        nextDayTime = dTime.AddDays(i);
                                        sBuilder.Append("<set label='" + nextDayTime.ToString("yyyy-MM-dd") + "' value='" +
                                            BP.DA.DBAccess.RunSQLReturnCOUNT("SELECT distinct FK_Flow" +
                                                        " FROM WF_CH WHERE CHSta='3' AND DTFrom >='" + nextDayTime.ToString("yyyy-MM-dd") +
                                                        "' AND DTTo <='" + nextDayTime.ToString("yyyy-MM-dd") + "'") + "' />");

                                    }
                                    
                                    chartTitle = dTime.AddDays(-9).ToString("yyyy-MM-dd") + "至" + dTime.ToString("yyyy-MM-dd") + "超期完成流程分析";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        #endregion  天
                        default:
                            break;
                    }


                    sBuilder.Append("<styles>");
                    sBuilder.Append("<definition>");
                    sBuilder.Append("<style name='CaptionFont' type='font' size='12'/>");
                    sBuilder.Append("</definition>");
                    sBuilder.Append("<application>");
                    sBuilder.Append("<apply toObject='CAPTION' styles='CaptionFont' />");
                    sBuilder.Append("<apply toObject='SUBCAPTION' styles='CaptionFont' />");
                    sBuilder.Append("</application>");
                    sBuilder.Append("</styles>");
                    sBuilder.Append("</chart>");

                    sBuilder.Insert(0, "<chart " + exportStr + " caption='" + chartTitle +
                                       "' lineThickness='1'" +
                                       " showValues='1' formatNumberScale='0' anchorRadius='2'   divLineAlpha='20'" +
                                       " divLineColor='CC3300' divLineIsDashed='1' showAlternateHGridColor='1' " +
                                       " alternateHGridColor='CC3300' shadowAlpha='40' labelStep='2' numvdivlines='5'" +
                                       " chartRightMargin='35' bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10' " +
                                       " alternateHGridAlpha='5'  legendPosition ='RIGHT '>");

                    this.Literal1.Text = InfoSoftGlobal.FusionCharts.RenderChart(BP.WF.Glo.CCFlowAppPath + "WF/Comm/Charts/swf/Line.swf",
                        "", sBuilder.ToString(), "Line_3D", "800", "300", false, true);
                        
                %>
                <div class="chart_div_con" id="slChart">
                    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                </div>
                <%--    <div class="chart_div_footer">
                    分析维度：
                    <input type="radio" id="slFlow" name="flowSort" onclick="loadFlowUSLChartType();"
                        value="slFlow" checked="checked" /><label for="slFlow">流程实例分析</label>
                    <input type="radio" id="cqUnDoFlow" name="flowSort" onclick="loadFlowUSLChartType();"
                        value="cqUnDoFlow" /><label for="cqUnDoFlow">超期完成流程分布</label>
                </div>--%>
            </div>
        </td>
    </tr>
</table>
