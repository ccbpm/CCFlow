<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FlowCH.aspx.cs" Inherits="CCFlow.WF.CH1234.FlowCH" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流程考核</title>
    <link href="../Scripts/jquery/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/jquery/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <link href="../Comm/Charts/css/style_3.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Charts/css/prettify.css" rel="stylesheet" type="text/css" />
    <link href="../Comm/Charts/css/Style.css" rel="stylesheet" type="text/css" />
    <script src="../Comm/Charts/js/json2_3.js" type="text/javascript"></script>
    <script src="../Comm/Charts/js/FusionCharts.js" type="text/javascript"></script>
    <link href="css/flowch.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function queryData(param, callback, scope, method, showErrMsg) {
            if (!method) method = 'GET';
            $.ajax({
                type: method,
                dataType: "text",
                contentType: "application/json; charset=utf-8",
                url: "FlowCH.aspx",
                data: param,
                async: false,
                cache: false,
                complete: function () { },
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) {
                    var data = msg;
                    callback(data, scope);
                }
            });
        }
        $(function () {
            //加载图形
            loadEmpChart();
            loadDeptChart();
            loadAllDeptChart();
        });
        //加载我的工作第一个图形
        function loadEmpChart() {
            var params = {
                method: "empChart"
            };
            queryData(params, function (js, scope) {
                $("#pageloading").hide();
                if (js == "") js = "[]";
                if (js.status && js.status == 500) {
                    $("body").html("<b>访问页面出错，请联系管理员。<b>");
                    return;
                }

                var pushData = eval('(' + js + ')');
                var chart = new FusionCharts("../Comm/Charts/MSLine.swf", "CharZ", '850', '350', '0', '0');
                chart.setDataXML(pushData.set_XML[0]);
                chart.render("empChart");
            }, this);
        }
        //我部门
        function loadDeptChart() {
            var params = {
                method: "deptChart"
            };
            queryData(params, function (js, scope) {
                $("#pageloading").hide();
                if (js == "") js = "[]";
                if (js.status && js.status == 500) {
                    $("body").html("<b>访问页面出错，请联系管理员。<b>");
                    return;
                }

                var pushData = eval('(' + js + ')');
                var chart = new FusionCharts("../Comm/Charts/MSLine.swf", "CharZ", '850', '350', '0', '0');
                chart.setDataXML(pushData.set_XML[0]);
                chart.render("deptChart");
            }, this);
        }
        //全单位
        function loadAllDeptChart() {
            var params = {
                method: "allDeptChart"
            };
            queryData(params, function (js, scope) {
                $("#pageloading").hide();
                if (js == "") js = "[]";
                if (js.status && js.status == 500) {
                    $("body").html("<b>访问页面出错，请联系管理员。<b>");
                    return;
                }

                var pushData = eval('(' + js + ')');
                var chart = new FusionCharts("../Comm/Charts/MSLine.swf", "CharZ", '850', '350', '0', '0');
                chart.setDataXML(pushData.set_XML[0]);
                chart.render("allDeptChart");
            }, this);
        }
    </script>
</head>
<body>
    <div class="main">
        <div class="main_top_left">
            <div class="main_top_left_info">
                我的总体工作效率
            </div>
            <ul>
                <%
                    string sql = "";
                    System.Data.DataTable dt = new System.Data.DataTable();

                    sql = "SELECT * FROM V_TOTALCH WHERE FK_Emp='" + BP.Web.WebUser.No + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    int totalGzzs = 0;   //总数.
                    int totalAswc = 0;   //按时完成
                    int totalCswc = 0;   //超时完成(逾期和超期之和)
                    if (dt.Rows.Count != 0)
                    {
                        totalGzzs = int.Parse(dt.Rows[0]["AllNum"].ToString());
                        totalAswc = int.Parse(dt.Rows[0]["ASNum"].ToString());
                        totalCswc = int.Parse(dt.Rows[0]["CSNum"].ToString());
                    }

                    //求按时完率.
                    double totalAswcl = 0;
                    string totalAswclStr = "0";
                    if (totalGzzs != 0)
                    {
                        totalAswcl = (double)totalAswc / totalGzzs * 100;//按时完成率
                        totalAswclStr = totalAswcl.ToString("00.00");
                    }

                    sql = "select * from V_TOTALCH	 ORDER BY WCRate desc";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    //根据按时(及时和按期)完成率计算排名，考虑按时完成率相同
                    int myTotalPm = 0;//默认排名
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        myTotalPm += 1;
                        if (dr["FK_Emp"].ToString() == BP.Web.WebUser.No)
                            break;
                    }

                    //OverMinutes小于0表明提前 
                    sql = "select sum(OverMinutes) from wf_ch where fk_emp='" + BP.Web.WebUser.No + "' and OverMinutes <0";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    int totalZttq = 0;//总体提前
                    try
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                            totalZttq = -int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }

                    //OverMinutes大于0表明逾期
                    int totalZtyq = 0;
                    sql = "select sum(OverMinutes) from wf_ch where fk_emp='" + BP.Web.WebUser.No + "' and OverMinutes >0";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    try
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                            totalZtyq = int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }

                    string totalJswc = "0";
                    string totalAqwc = "0";
                    string totalYqwc = "0";
                    string totalCqwc = "0";

                    sql = "SELECT JiShi,AnQi,YuQi,ChaoQi FROM V_TOTALCH WHERE FK_EMP='" + BP.Web.WebUser.No + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    if (dt.Rows.Count != 0)
                    {
                        totalJswc = dt.Rows[0]["JiShi"].ToString();
                        totalAqwc = dt.Rows[0]["AnQi"].ToString();
                        totalYqwc = dt.Rows[0]["YuQi"].ToString();
                        totalCqwc = dt.Rows[0]["ChaoQi"].ToString();
                    }
                %>
                <li>工作总数<%=totalGzzs%>,按时完成<font class="greenFont"><%=totalAswc%></font>个,超时完成<font
                    class="redFont"><%=totalCswc%></font>个,按时完成率:<font class="redFont"><%=totalAswclStr%>%</font></li>
                </br>
                <li>总体排名:第<font class="redFont"><%=myTotalPm %></font>名</li>
                <li>总体提前:<font class="greenFont"><%=totalZttq %></font>分钟</li>
                <li>总体逾期:<font class="redFont"><%=totalZtyq%></font>分钟</li>
                <li>及时完成<%=totalJswc %>条</li>
                <li>按期完成<%=totalAqwc%>条</li>
                <li>逾期完成<%=totalYqwc%>条</li>
                <li>超期完成<font style="font-size: 25px; color: Red; font-family: Vijaya"><%=totalCqwc%></font>条</li>`
            </ul>
        </div>
        <div class="main_top_right">
            <div class="main_top_right_info">
                我的上周工作效率
            </div>
            <ul>
                <%
                    DateTime firstDayOflastWeek = DateTime.Now.AddDays(-7);//上周第一天
                    System.Globalization.CultureInfo myCI =
                  new System.Globalization.CultureInfo("zh-CN");
                    //上一星期在本年的第几周
                    int lastWeekIndex = myCI.Calendar.GetWeekOfYear(firstDayOflastWeek, System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);
                    //上一星期隶属的年月
                    string lastFK_NY = firstDayOflastWeek.ToString("yyyy-MM");

                    sql = "SELECT * FROM V_TotalCHWeek WHERE FK_NY='" + lastFK_NY +
                         "' AND FK_Emp='" + BP.Web.WebUser.No + "' AND WeekNum=" + lastWeekIndex;
                    dt = new System.Data.DataTable();

                    int lastWeekGzzs = 0; //上周工作总数
                    int lastWeekAswc = 0;//按时完成
                    int lastWeekCswc = 0;//超时完成
                    if (dt.Rows.Count != 0)
                    {
                        lastWeekGzzs = int.Parse(dt.Rows[0]["AllNum"].ToString());
                        lastWeekAswc = int.Parse(dt.Rows[0]["ASNum"].ToString());
                        lastWeekCswc = int.Parse(dt.Rows[0]["CSNum"].ToString());
                    }

                    double lastWeekAswcl = 0;//按时完成率
                    string lastWeekAswclStr = "0";
                    if (lastWeekGzzs != 0)
                    {
                        lastWeekAswcl = (double)lastWeekAswc / lastWeekGzzs * 100;
                        lastWeekAswclStr = lastWeekAswcl.ToString("00.00");
                    }

                    //根据按时(及时,按期之和)完成率计算排名，考虑按时完成率相同
                    int lastWeekMypm = 0;//默认排名
                    sql = "SELECT * FROM V_TotalCHWeek WHERE FK_NY='" + lastFK_NY +
                        "' AND  WeekNum=" + lastWeekIndex + " ORDER BY WCRate desc";

                    dt = new System.Data.DataTable();

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        lastWeekMypm += 1;
                        if (dr["FK_Emp"].ToString() == BP.Web.WebUser.No)
                            break;
                    }

                    int lastWeekZttq = 0; //上周总体提前
                    sql = "select  sum(OverMinutes) from WF_CH where FK_Emp='" + BP.Web.WebUser.No
                    + "' and WeekNum='" + lastWeekIndex + "' and OverMinutes<0 and FK_NY='" + lastFK_NY + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    try
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                            lastWeekZttq = -int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }


                    int lastWeekZtyq = 0;//上周总体逾期
                    sql = "select  sum(OverMinutes) from WF_CH where FK_Emp='" + BP.Web.WebUser.No
                   + "' and WeekNum='" + lastWeekIndex + "' and OverMinutes>0 and FK_NY='" + lastFK_NY + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    try
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                            lastWeekZtyq = int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }

                    sql = "SELECT JiShi,AnQi,YuQi,ChaoQi FROM V_TotalCHWeek WHERE FK_NY='" + lastFK_NY +
                        "' AND FK_Emp='" + BP.Web.WebUser.No + "' AND WeekNum=" + lastWeekIndex;
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    string lastWeekJswc = "0";//及时完成
                    string lastWeekAqwc = "0";//按期完成
                    string lastWeekYqwc = "0";//逾期完成
                    string lastWeekCqwc = "0";//超期完成

                    if (dt.Rows.Count != 0)
                    {
                        lastWeekJswc = dt.Rows[0]["JiShi"].ToString();
                        lastWeekAqwc = dt.Rows[0]["AnQi"].ToString();
                        lastWeekYqwc = dt.Rows[0]["YuQi"].ToString();
                        lastWeekCqwc = dt.Rows[0]["ChaoQi"].ToString();
                    }
                %>
                <li>工作总数<%=lastWeekGzzs%>,按时完成<font class="greenFont"><%=lastWeekAswc%></font>个 ,超时完成<font
                    class="redFont"><%=lastWeekCswc%></font>个,按时完成率:<font class="redFont"><%=lastWeekAswclStr%>%</font></li>
                </br>
                <li>总体排名:第<font class="redFont"><%=lastWeekMypm%></font>名</li>
                <li>提前完成时间:<font style="font-size: 25px; color: Green; font-family: Vijaya"><%=lastWeekZttq %></font>分钟</li>
                <li>逾期完成时间:<font style="font-size: 25px; color: Red; font-family: Vijaya"><%=lastWeekZtyq %></font>分钟</li>
                <li>及时完成<%=lastWeekJswc %>条</li>
                <li>按期完成<%=lastWeekAqwc%>条</li>
                <li>逾期完成<%=lastWeekYqwc%>条</li>
                <li>超期完成<font style="font-size: 25px; color: Red; font-family: Vijaya;"><%=lastWeekCqwc%></font>条</li>`
            </ul>
        </div>
        <div class="main_center_left">
            <div class="main_center_left_info">
                我的上月工作效率
            </div>
            <ul>
                <%
                    DateTime dTime = DateTime.Now;

                    //上个月最后一天  
                    DateTime lastMouthFirstDay = dTime.AddMonths(-1).AddDays(-dTime.Day + 1);//当前时间的上一月的第一天
                    lastFK_NY = lastMouthFirstDay.ToString("yyyy-MM");

                    sql = "SELECT * FROM V_TOTALCHYF WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND FK_NY='" + lastFK_NY + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    int lastMouthGzzs = 0;//上月总的工作
                    int lastMouthAswc = 0;//按时完成
                    int lastMouthCswc = 0;//超时完成
                    if (dt.Rows.Count != 0)
                    {
                        lastMouthGzzs = int.Parse(dt.Rows[0]["AllNum"].ToString());
                        lastMouthAswc = int.Parse(dt.Rows[0]["ASNum"].ToString());
                        lastMouthCswc = int.Parse(dt.Rows[0]["CSNum"].ToString());
                    }

                    double lastMouthAswcl = 0;
                    string lastMouthAswclStr = "0";
                    try
                    {
                        if (lastMouthGzzs != 0)
                        {
                            lastMouthAswcl = (double)lastMouthAswc / lastMouthGzzs * 100;
                            lastMouthAswclStr = lastMouthAswcl.ToString("00.00");
                        }
                    }
                    catch (Exception)
                    {
                    }


                    //计算总体排名
                    sql = "select  FK_Emp from V_TOTALCHYF where "
                      + " FK_NY='" + lastFK_NY + "' ORDER BY WCRate desc";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    int lastMouthMypm = 0;//默认为第一名

                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        lastMouthMypm += 1;
                        if (dr["FK_Emp"].ToString() == BP.Web.WebUser.No)
                            break;
                    }

                    sql = "select  sum(OverMinutes) from WF_CH where FK_Emp='" + BP.Web.WebUser.No
                        + "' and FK_NY='" + lastFK_NY + "' and OverMinutes<0";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    int lastMouthZttq = 0;//上月提前完成
                    try
                    {
                        lastMouthZttq = -int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }

                    sql = "select  sum(OverMinutes) from WF_CH where FK_Emp='" + BP.Web.WebUser.No
                       + "' and FK_NY='" + lastFK_NY + "' and OverMinutes>0";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    int lastMouthZtyq = 0; //上月逾期完成

                    try
                    {
                        lastMouthZtyq = int.Parse(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }

                    sql = "SELECT JiShi,AnQi,YuQi,ChaoQi FROM V_TOTALCHYF WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND FK_NY='" + lastFK_NY + "'";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    string lastMouthJswc = "0";//上月及时完成
                    string lastMouthAqwc = "0";//上月按期完成
                    string lastMouthYqwc = "0";//上月逾期完成
                    string lastMouthCqwc = "0";//上月超期完成

                    if (dt.Rows.Count != 0)
                    {
                        lastMouthJswc = dt.Rows[0]["JiShi"].ToString();
                        lastMouthAqwc = dt.Rows[0]["AnQi"].ToString();
                        lastMouthYqwc = dt.Rows[0]["YuQi"].ToString();
                        lastMouthCqwc = dt.Rows[0]["ChaoQi"].ToString();
                    }
                %>
                <li>工作总数<%=lastMouthGzzs %>,按时完成<font class="greenFont"><%=lastMouthAswc %></font>个,
                    超时完成<font class="redFont"><%=lastMouthCswc%></font>个, 按时完成率:<font class="redFont"><%=lastMouthAswclStr%>%</font>
                </li>
                </br>
                <li>总体排名:第<%=lastMouthMypm%>名</li>
                <li>提前完成时间:<font style="font-size: 25px; color: Green; font-family: Vijaya"><%=lastMouthZttq%></font>分钟</li>
                <li>逾期完成时间:<font style="font-size: 25px; color: Red; font-family: Vijaya"><%=lastMouthZtyq%></font>分钟</li>
                <li>及时完成<%=lastMouthJswc%>条</li>
                <li>按期完成<%=lastMouthAqwc%>条</li>
                <li>逾期完成<%=lastMouthYqwc%>条</li>
                <li>超期完成<font style="font-size: 25px; color: Red; font-family: Vijaya;"><%=lastMouthCqwc%></font>条</li>
            </ul>
        </div>
        <div class="main_center_right">
            <div class="main_center_right_info">
                按期完成率总体排名
            </div>
            <ul>
                <%
                    sql = "SELECT FK_Emp FROM V_TOTALCH ORDER BY WCRate DESC";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    string totalHtml = "";
                    int rowCount = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        BP.GPM.Emp e = new BP.GPM.Emp(dt.Rows[i]["FK_Emp"].ToString());

                        int num = i + 1;
                        if (i == 8)
                            break;
                        if (num <= 3)
                        {
                            totalHtml += "<li>第<font style='font-size:25px;color:Red;font-family:Vijaya;'>" + num + "</font>名:" + e.Name + "</li>";
                        }
                        else
                        {
                            totalHtml += "<li>第" + num + "名:" + e.Name + "</li>";
                        }
                        rowCount += 1;
                    }

                    for (int i = 1; i <= 8 - rowCount; i++)
                    {
                        totalHtml += "<li></li>";
                    }

                %>
                <%=totalHtml %>
            </ul>
        </div>
        <div class="main_bottom_left">
            <div class="main_bottom_left_info">
                按期完成率上周总体排名
            </div>
            <ul>
                <%
                    sql = "SELECT * FROM V_TotalCHWeek WHERE FK_NY='" + lastFK_NY +
                         "' AND  WeekNum=" + lastWeekIndex + " ORDER BY WCRate DESC";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    string llWeek = firstDayOflastWeek.AddDays(-7).ToString("yyyy-MM-dd");//上上周
                    string llFK_NY = DateTime.Parse(llWeek).ToString("yyyy-MM");

                    rowCount = 0;
                    string lastWeekHtml = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (rowCount == 8)
                            break;

                        BP.GPM.Emp e = new BP.GPM.Emp(dt.Rows[i]["FK_Emp"].ToString());

                        string str = "同比<img style='margin:0px 10px;' src='images/down.png' />";//下降


                        int llWeekIndex = myCI.Calendar.GetWeekOfYear(DateTime.Parse(llWeek), System.Globalization.CalendarWeekRule.FirstDay, System.DayOfWeek.Monday);
                       
                        sql = "SELECT * FROM V_TotalCHWeek WHERE FK_NY='" + llFK_NY +
                   "' AND  WeekNum=" + llWeekIndex + " AND FK_Emp='" + e.No + "'";

                        System.Data.DataTable llDt = new System.Data.DataTable();

                        llDt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                        float lWCRate = 0;
                        float tb = 0;
                        string tbStr = "";
                        try
                        {
                            lWCRate = float.Parse(dt.Rows[i]["WCRate"].ToString());
                            tb = (float.Parse(dt.Rows[i]["ASNum"].ToString()) - float.Parse(llDt.Rows[0]["ASNum"].ToString())) / float.Parse(llDt.Rows[0]["AllNum"].ToString()) * 100;
                        }
                        catch (Exception)
                        {
                        }
                        tbStr =  Math.Abs(tb).ToString() + "%";

                        if (tb > 0)//上升   
                        {
                            str = "同比<img style='margin:0px 10px;' src='images/up.png' />";
                            tbStr =  tb.ToString() + "%";
                        }

                        if (tb == 0)
                        {
                            str = "";
                            tbStr = "-幅度不变";
                        }

                        lastWeekHtml += "<li>" + e.Name + "-" + lWCRate + "%" + str + tbStr + "</li>";
                        rowCount += 1;
                    }

                    for (int i = 1; i <= 8 - rowCount; i++)
                    {
                        lastWeekHtml += "<li></li>";
                    }
                %>
                <%=lastWeekHtml%>
            </ul>
        </div>
        <div class="main_bottom_right">
            <div class="main_bottom_right_info">
                按期完成率上月总体排名
            </div>
            <ul>
                <%
                    dTime = DateTime.Now;
                    DateTime llDtime = dTime.AddMonths(-2);//上上一月
                    
                    sql = "SELECT * FROM V_TOTALCHYF WHERE FK_NY='" + lastFK_NY +
                              "'  ORDER BY WCRate DESC";
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                    rowCount = 0;
                    string lastMouthHtml = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (rowCount == 8)
                            break;

                        BP.GPM.Emp e = new BP.GPM.Emp(dt.Rows[i]["FK_Emp"].ToString());

                       
                        sql = "SELECT * FROM V_TotalCHWeek WHERE FK_NY='" + llDtime.ToString("yyyy-MM") +
                   "' AND  FK_Emp='" + e.No + "'";

                        System.Data.DataTable llDt = new System.Data.DataTable();

                        llDt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                        float lWCRate = 0;
                        float tb = 0;
                        string tbStr = "";
                        try
                        {
                            lWCRate = float.Parse(dt.Rows[i]["WCRate"].ToString());
                            tb = (float.Parse(dt.Rows[i]["ASNum"].ToString()) - float.Parse(llDt.Rows[0]["ASNum"].ToString())) /
                                float.Parse(llDt.Rows[0]["AllNum"].ToString()) * 100;
                        }
                        catch (Exception)
                        {
                        }

                        string str = "同比<img style='margin:0px 10px;' src='images/down.png' />";//下降

                        tbStr = Math.Abs(tb).ToString() + "%";
                        
                        if (tb > 0)//上升   
                        {
                            str = "同比<img style='margin:0px 10px;' src='images/up.png' />";
                            tbStr = tb.ToString() + "%";
                        }

                        if (tb == 0)
                        {
                            str = "";
                            tbStr = "-幅度不变";
                        }

                        lastMouthHtml += "<li>" + e.Name + "-" + lWCRate + "%" + str + tbStr + "</li>";
                        rowCount += 1;
                    }

                    for (int i = 1; i <= 8 - rowCount; i++)
                    {
                        lastMouthHtml += "<li></li>";
                    }
                %>
                <%=lastMouthHtml%>
            </ul>
        </div>
        <div class="myworkbgPho">
            <div class="myworkmaintitle">
                我的工作(最近一个月)
            </div>
        </div>
        <div class="mywork_top">
            <div class="mywork_top_info">
                趋势图(按数量)
            </div>
            <div class="mywork_top_chart" id="empChart">
            </div>
        </div>
        <div class="deptworkbgPho">
            <div class="deptworktitle">
                我部门(最近一个月)
            </div>
        </div>
        <div class="deptwork_top">
            <div class="deptwork_top_info">
                趋势图(按数量)
            </div>
            <div class="deptwork_top_chart" id="deptChart">
            </div>
        </div>
        <div class="alldeptbgPho">
            <div class="alldeptworktitle">
                全单位(最近一个月)
            </div>
        </div>
        <div class="alldeptwork_top">
            <div class="alldeptwork_top_info">
                趋势图(按数量)
            </div>
            <div class="alldeptwork_top_chart" id="allDeptChart">
            </div>
        </div>
    </div>
</body>
</html>
