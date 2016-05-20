<%@ Page Title="实例增长分析" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master"
    AutoEventWireup="true" CodeBehind="InstanceGrow.aspx.cs" Inherits="CCFlow.WF.Admin.FlowDB.InstanceGrow" %>

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
        function loadFlowUSLChartType() {
            var str1 = $('input:radio[name=anaTime]:checked').val();

            window.location.href = "InstanceGrow.aspx?anaTime=" + str1 + "&";
        }
        $(function () {
            $("#" + Application.common.getArgsFromHref("anaTime")).attr("checked", true);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table id="tableContent" style="width: 100%;">
        <caption>
            实例增长分析</caption>
        <tr>
            <td valign="top">
                <!-- 总数分析..... -->
                <fieldset>
                    <legend>流程实例统计 </legend>
                    <table style="width: 80%;">
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" + (int)BP.WF.WFState.Complete + " " ) %>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete + "  " ) %>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta  + "   " ) %>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Delete + " ") %>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFSta=" + (int)BP.WF.WFSta.Etc + " ") %>
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
                    <legend>上月发起流程分析 </legend>
                    <%
                        //获得上月的月份.

                        string FK_NY = BP.DA.DataType.CurrentNYOfPrevious;
                        string ShangFK_NY = BP.DA.DataType.ShangCurrentNYOfPrevious;
                        int SYF = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_NY='" + FK_NY + "' ");

                        int SSYF = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_NY='" + ShangFK_NY + "' ");
                        int TBZZS = SYF - SSYF;
                        string TBZZL = "×";
                        if (SYF == 0 || SSYF == 0)
                        {
                            TBZZL = "×";
                        }
                        else
                        {
                            TBZZL = (((decimal)SYF - (decimal)SSYF) / (decimal)SSYF * 100).ToString("0.00");
                        }
       
                    %>
                    <table style="width: 80%;">
                        <tr>
                            <td>
                                发起流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE FK_NY='"+FK_NY+"' ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" + (int)BP.WF.WFState.Complete + " AND  FK_NY='" + FK_NY + "' ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete + " AND  FK_NY='" + FK_NY + "' ")%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta + " AND  FK_NY='" + FK_NY + "'  ")%>
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
                                <%=TBZZS%>
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
                                <%=TBZZL%>
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
                    <legend>上周发起流程分析 </legend>
                    <%
                        //获得上周
                        int priWeek = BP.DA.DataType.CurrentWeek - 1;
                        //获取上上周
                        int ShangpriWeek = BP.DA.DataType.CurrentWeek - 2;

                        int SZZS = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WeekNum=" + priWeek);
                        int SSZZS = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WeekNum=" + ShangpriWeek);
                        string ZTBZZL = "×";
                        //同比增长
                        int ZTBZZS = SZZS - SSZZS;
                        //同比增长率
                        if (SZZS == 0 || SSZZS == 0)
                        {
                            TBZZL = "×";
                        }
                        else
                        {
                            ZTBZZL = (((decimal)SZZS - (decimal)SSZZS) / (decimal)SSZZS * 100).ToString("0.00");
                        }
        
       
                    %>
                    <table style="width: 80%;">
                        <tr>
                            <td>
                                发起流程总数
                            </td>
                            <td>
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WeekNum=" + priWeek )%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState!=" + (int)BP.WF.WFState.Complete + " AND WeekNum=" + priWeek )%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.Complete + " AND    WeekNum=" + priWeek)%>
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
                                <%=BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE WFState=" + (int)BP.WF.WFState.ReturnSta + " AND   WeekNum=" + priWeek)%>
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
                                <%=ZTBZZS%>
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
                                <%=ZTBZZL%>
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
                                  "  where FK_NY='" + dTime.ToString("yyyy-MM") + "' group  by DeptName";
                            break;
                        case "week":
                           

                            sql = "select  COUNT(DeptName) Num, DeptName from WF_GenerWorkFlow" +
                                  "  where  WeekNum= " +
                                  " " + BP.DA.DataType.CurrentWeek + "  group  by DeptName";
                            break;
                        case "day":
                            sql = "select  COUNT(DeptName) Num, DeptName from WF_GenerWorkFlow" +
                                "  where  RDT LIKE'" + dTime.ToString("yyyy-MM-dd") + "%'  group  by DeptName";
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
                分析项目： 发起流程， 发起部门
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
