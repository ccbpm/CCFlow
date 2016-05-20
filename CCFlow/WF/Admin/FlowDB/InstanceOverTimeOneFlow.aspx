<%@ Page Title="逾期实例" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="InstanceOverTimeOneFlow.aspx.cs" Inherits="CCFlow.WF.Admin.FlowDB.InstanceOverTimeOneFlow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href=" ../../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src=" ../../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src=" ../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src=" ../../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
        }
        input
        {
            vertical-align: text-top;
            margin-top: 0;
        }
    </style>
    <script type="text/javascript">
        var fk_flow;
       
        function loadFlowUSLChartType() {
            var str1 = $('input:radio[name=anaTime]:checked').val();
            window.location.href = "InstanceOverTimeOneFlow.aspx?anaTime=" + str1 + "&FK_Flow=" + fk_flow + "&";
        }
        $(function () {
            fk_flow = Application.common.getArgsFromHref("FK_Flow");
            $("#" + Application.common.getArgsFromHref("anaTime")).attr("checked", true);
        });
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <% 
        string fk_flow = this.Request.QueryString["FK_Flow"]; 
    %>
    <table style="width: 100%;">
        <caption>
            逾期分析</caption>
        <tr>
            <td valign="top">
                <!-- 总数分析..... -->
                <fieldset>
                    <legend>逾期流程实例统计 </legend>
                    <table style="width: 80%;">
                        <tr>
                            <td>
                                流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(distinct WorkID) FROM WF_CH  where   FK_Flow=" + fk_flow + " ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState !=" + (int)BP.WF.WFState.Complete + " ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState =" + (int)BP.WF.WFState.Complete + " ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                退回数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.ReturnSta + "   ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.Delete + " ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE   FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFSta=" + (int)BP.WF.WFSta.Etc + " ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <!-- 上个月分析..... -->
                <fieldset>
                    <legend>上月逾期流程分析 </legend>
                    <%
                        //获得上月的月份.

                        string FK_NY = BP.DA.DataType.CurrentNYOfPrevious;
                        string ShangFK_NY = BP.DA.DataType.ShangCurrentNYOfPrevious;
                        int YQ_SYF = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and FK_NY='" + FK_NY + "' ");

                        int YQ_SSYF = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and FK_NY='" + ShangFK_NY + "' ");
                        int YQ_TBZZS = YQ_SYF - YQ_SSYF;
                        string YQ_TBZZL = "×";
                        if (YQ_SYF == 0 || YQ_SSYF == 0)
                        {
                            YQ_TBZZL = "×";
                        }
                        else
                        {
                            YQ_TBZZL = (((decimal)YQ_SYF - (decimal)YQ_SSYF) / (decimal)YQ_SSYF * 100).ToString("0.00");
                        }
                    %>
                    <table style="width: 80%;">
                        <tr>
                            <td>
                                发起流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and FK_NY='" + FK_NY + "' ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState!=" + (int)BP.WF.WFState.Complete + " AND  FK_NY='" + FK_NY + "' ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.Complete + " AND  FK_NY='" + FK_NY + "' ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                退回数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.ReturnSta + " AND  FK_NY='" + FK_NY + "'  ")%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                同比增长数
                            </td>
                            <td>
                                <%=YQ_TBZZS%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                同比增长率
                            </td>
                            <td>
                                <%=YQ_TBZZL%>
                            </td>
                            <td>
                                %
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td valign="top">
                <!-- 上个周分析..... -->
                <fieldset>
                    <legend>上周逾期流程分析 </legend>
                    <%
                        //获得上周
                        int priWeek = BP.DA.DataType.CurrentWeek - 1;
                        //获取上上周
                        int ShangpriWeek = BP.DA.DataType.CurrentWeek - 2;

                        int YQ_SZZS = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WeekNum=" + priWeek);
                        int YQ_SSZZS = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WeekNum=" + ShangpriWeek);
                        string YQ_ZTBZZL = "×";
                        //同比增长
                        int YQ_ZTBZZS = YQ_SZZS - YQ_SSZZS;
                        //同比增长率
                        if (YQ_SZZS == 0 || YQ_SSZZS == 0)
                        {
                            YQ_ZTBZZL = "×";
                        }
                        else
                        {
                            YQ_ZTBZZL = (((decimal)YQ_SZZS - (decimal)YQ_SSZZS) / (decimal)YQ_SSZZS * 100).ToString("0.00");
                        }
        
       
                    %>
                    <table style="width: 80%;">
                        <tr>
                            <td>
                                发起流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WeekNum=" + priWeek)%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState!=" + (int)BP.WF.WFState.Complete + " AND WeekNum=" + priWeek)%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.Complete + " AND    WeekNum=" + priWeek)%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                退回数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) and WFState=" + (int)BP.WF.WFState.ReturnSta + " AND   WeekNum=" + priWeek)%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                同比增长数
                            </td>
                            <td>
                                <%=YQ_ZTBZZS%>
                            </td>
                            <td>
                                个
                            </td>
                        </tr>
                        <tr>
                            <td>
                                同比增长率
                            </td>
                            <td>
                                <%=YQ_ZTBZZL%>
                            </td>
                            <td>
                                %
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <table style="width: 100%;">
        <tr>
           <td valign="top" colspan="3" style="padding-left: 10px; padding-right: 40px; border-bottom: 1px solid white;">
                流程图表分析:
                <input style="margin-left: 52px;" type="radio" id="mouth" name="anaTime" onclick="loadFlowUSLChartType();"
                    value="mouth" checked="checked" />按月
                <input type="radio" id="week" name="anaTime" onclick="loadFlowUSLChartType();" value="week" />按周
                <input type="radio" id="day" name="anaTime" onclick="loadFlowUSLChartType();" value="day" />按日
            </td>
        </tr>
        <tr>
            <td>
                <div id="chartDiv">
                </div>
                <%
                    DateTime dTime = DateTime.Now;

                    string anaTime = this.Request.Params["anaTime"];

                    string sql = "";
                    switch (anaTime)
                    {
                        case "mouth":
                            sql = "select  COUNT(DeptName) Num, DeptName from WF_GenerWorkFlow" +
                                  "  where  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) AND FK_NY='" + dTime.ToString("yyyy-MM") + "' group  by DeptName";
                            break;
                        case "week":


                            sql = "select  COUNT(DeptName) Num, DeptName from WF_GenerWorkFlow" +
                                  "  where  FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) AND  WeekNum= " +
                                  " " + BP.DA.DataType.CurrentWeek + "  group  by DeptName";
                            break;
                        case "day":
                            sql = "select  COUNT(DeptName) Num, DeptName from WF_GenerWorkFlow" +
                                "  where   FK_Flow=" + fk_flow + "   AND WorkID  in(SELECT distinct WorkID FROM WF_CH) AND  RDT LIKE'" + dTime.ToString("yyyy-MM-dd") + "%'  group  by DeptName";
                            break;
                        default:
                            break;
                    }


                    System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    StringBuilder sBuilder = new StringBuilder();

                    int yAxisMaxValue = 0;
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        int setValue = int.Parse(dr["Num"].ToString());
                        if (setValue > yAxisMaxValue)
                            yAxisMaxValue = setValue;

                        sBuilder.Append("<set label='" + dr["DeptName"].ToString() +
                            "' value='" + setValue + "' />");
                    }
                    sBuilder.Append("</chart>");

                    yAxisMaxValue += 10;
                    string exportHeader = "exportEnabled='1' exportAtClient='0'" +
                       " exportAction='download' " +
                       "exportHandler='../../Comm/Charts/FCExport.aspx' " +
                       "exportDialogMessage='正在生成,请稍候...'  " +
                       " exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件'";

                    sBuilder.Insert(0, "<chart " + exportHeader + " subcaption='柱状图' formatNumberScale='0'"
                    + " divLineAlpha='20' divLineColor='CC3300'"
                    + " alternateHGridColor='CC3300' shadowAlpha='40' numvdivlines='9'"
                    + "  bgColor='FFFFFF,CC3300' bgAngle='270'"
                    + " bgAlpha='10,10' alternateHGridAlpha='5' yAxisMaxValue ='" + yAxisMaxValue + "'>");

                    this.HiddenField1.Value = sBuilder.ToString();
                %>
                <asp:HiddenField ID="HiddenField1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                分析项目： 逾期次数， 逾期部门
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        $(function () {
            var chartData = $("#<%=HiddenField1.ClientID%>").val();

            var w = $("#chartDiv").css("width"); //当前宽度
            w = w.replace("px", "") - 20;
            var chart = new FusionCharts("../../Comm/Charts/swf/Column3D.swf", "CharZ", w, '350', '0', '0');
            chart.setDataXML(chartData);
            chart.render("chartDiv");
        });
    </script>
</asp:Content>
