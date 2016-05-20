<%@ Page Title="工作量分析" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
    CodeBehind="NodesDtlEmps0.aspx.cs" Inherits="CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow.NodesDtlEmps0" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--add by qin 14:53 2015/9/4--%>
    <table id='tab_0' style="width: 100%; min-width: 800px;">
        <tr>
            <th class="center" rowspan="2">
                序
            </th>
            <th class="center" rowspan="2">
                人员
            </th>
            <th class="center" colspan="3">
                工作分析
            </th>
            <th class="center" colspan="4">
                按月份分析
            </th>
            <th class="center" colspan="4">
                按周分析
            </th>
        </tr>
        <tr>
            <th class="center">
                工作总数
            </th>
            <th class="center">
                待处理
            </th>
            <th class="center">
                退回次数
            </th>
            <%
                BP.WF.Node node = new BP.WF.Node(this.Request.Params["FK_Node"]);
                BP.WF.Flow flow = new BP.WF.Flow(node.FK_Flow);
                string tkTable = "ND" + int.Parse(node.FK_Flow) + "Track";
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;


                string sql = "SELECT distinct EmpFrom,EmpFromT FROM ND" + int.Parse(flow.No) +
                    "Track WHERE NDFrom='" + node.NodeID + "'";

                System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);


                //改为上月与上上月之间的比较，周类似     周要求
                DateTime dTime = DateTime.Now;

                //上上月
                string beforeLastMouth = dTime.AddMonths(-2).ToString("yyyy-MM");
                string lastMouth = dTime.AddMonths(-1).ToString("yyyy-MM");

                //上上周的开始，截止日期
                string beforeLastWeekFDay = BP.DA.DataType.WeekOfMonday(BP.DA.DataType.WeekOfMonday(dTime).AddDays(-9)).ToString("yyyy-MM-dd");
                string beforeLastWeekEndDay = BP.DA.DataType.WeekOfMonday(BP.DA.DataType.WeekOfMonday(dTime).AddDays(-9)).AddDays(6).ToString("yyyy-MM-dd");

                //上周的开始，截止日期
                string lastWeekFDay = BP.DA.DataType.WeekOfMonday(BP.DA.DataType.WeekOfMonday(dTime).AddDays(-3)).ToString("yyyy-MM-dd");
                string lastWeekEndDay = BP.DA.DataType.WeekOfMonday(BP.DA.DataType.WeekOfMonday(dTime).AddDays(-3)).AddDays(6).ToString("yyyy-MM-dd");
            %>
            <th class="center">
                <%=beforeLastMouth%>
            </th>
            <th class="center">
                <%=lastMouth%>
            </th>
            <th class="center">
                同比
            </th>
            <th class="center">
                同比增长
            </th>
            <th class="center">
                上上周</br><%= beforeLastWeekFDay %></br><%= beforeLastWeekEndDay %>
            </th>
            <th class="center">
                上周</br><%= lastWeekFDay %></br><%= lastWeekEndDay %>
            </th>
            <th class="center">
                同比
            </th>
            <th class="center">
                同比增长
            </th>
        </tr>
        <%
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //处理总数
                BP.DA.Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr + "NDFrom";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);
                int sumCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //待处理
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  WF_GenerWorkerlist WHERE FK_Emp=" + dbstr +
                    "FK_Emp AND FK_Node=" + dbstr + "FK_Node AND IsPass=0  ";
                ps.Add(BP.WF.GenerWorkerListAttr.FK_Emp, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.GenerWorkerListAttr.FK_Node, node.NodeID);
                int todoNum = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);


                //  退回数  count WorkID
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                    "NDFrom AND ActionType=" + dbstr + "ActionType";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);
                ps.Add(BP.WF.TrackAttr.ActionType, (int)BP.WF.ActionType.Return);
                int returnCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
        %>
        <tr>
            <td class="Idx">
                <%=i + 1 %>
            </td>
            <td class="center">
                <%=dt.Rows[i]["EmpFromT"] %>
            </td>
            <td class="center">
                <%=sumCount %>
            </td>
            <td class="center">
                <%  if (todoNum == 0)
                    {%>
                <%=  todoNum%>
                <%}
                    else
                    {%>
                <font style="color: Green;"><b>
                    <%=todoNum%>
                </b></font>
                <%}%>
            </td>
            <td class="center">
                <%
                    if (returnCount == 0)
                    {%>
                <%=returnCount %>
                <%}
                    else
                    {%>
                <font style="color: Red;"><b>
                    <%= returnCount %></b></font>
                <%}
                %>
            </td>
            <%--上上月--%>
            <td class="center">
                <%
                    ps = new BP.DA.Paras();
                    ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                        " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                        "NDFrom AND RDT LIKE'%" + beforeLastMouth + "%'";
                    ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                    ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                    int llastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);  %>
                <%=llastCount %>
            </td>
            <%--上月--%>
            <td class="center">
                <%
                    ps = new BP.DA.Paras();
                    ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                        " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                        "NDFrom AND RDT LIKE'%" + lastMouth + "%'";
                    ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                    ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                    int lastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0); %>
                <%=lastCount %>
            </td>
            <%-- 按月  同比 红字标示小于零的列--%>
            <td class="center">
                <%
                    if (lastCount - llastCount <= 0)
                    {%>
                <font style="color: Red;"><b>
                    <%=lastCount - llastCount%>
                </b></font>
                <%}
                    else
                    {%>
                <font style="color: Green;"><b>
                    <%=lastCount - llastCount%>
                </b></font>
                <%}
                %>
                <%--按月百分比--%>
                <td class="center">
                    <%
                        double bl;//比率

                        if (lastCount == 0)
                        {%>
                    -
                    <%}
                        else
                        {
                            //比率
                            bl = (lastCount - llastCount) * 100.0 / lastCount;
                    %>
                    <%if (bl < 0)
                      {%>
                    <font style="color: Red;"><b>
                        <%= bl.ToString("0.00")%>%</b></font>
                    <%}%>
                    <%if (bl > 0)
                      {%>
                    <font style="color: Green;"><b>
                        <%= bl.ToString("0.00")%>%</b></font>
                    <%  }%>
                    <%if (lastCount == llastCount)
                      {%>-
                    <%}%>
                    <%}%>
                </td>
                <%--上上周--%>
                <%
                    ps = new BP.DA.Paras();
                    ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                        " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                        "NDFrom AND RDT >='" + beforeLastWeekFDay + "' AND RDT<='" +
                         beforeLastWeekEndDay + "'";
                    ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                    ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);
                    llastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                %>
                <td class="center">
                    <%=llastCount %>
                </td>
                <%--上周--%>
                <td class="center">
                    <%
                        ps = new BP.DA.Paras();
                        ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                            " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                            "NDFrom AND RDT >='" + lastWeekFDay + "' AND RDT<='" +
                           lastWeekEndDay + "'";
                        ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                        ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                        lastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                    %>
                    <%=lastCount %>
                </td>
                <%-- 按周  同比 红字标示小于零的列--%>
                <td class="center">
                    <%
                        if (lastCount - llastCount <= 0)
                        {%>
                    <font style="color: Red;"><b>
                        <%=(lastCount - llastCount)%>
                    </b></font>
                    <%}
                        else
                        {%>
                    <font style="color: Green;"><b>
                        <%=(lastCount - llastCount)%>
                    </b></font>
                    <%}%>
                </td>
                <%--按周百分比--%>
                <td class="center">
                    <%if (lastCount == 0)
                      {%>-
                    <%}
                      else
                      {
                          bl = (lastCount - llastCount) * 100.0 / lastCount;%>
                    <%  if (bl < 0)
                        {%>
                    <font style="color: Red;"><b>
                        <%= bl.ToString("0.00") %>%</b></font>
                    <%}
                        if (bl > 0)
                        {%>
                    <font style="color: Green;"><b>
                        <%= bl.ToString("0.00") %>%</b></font>
                    <%} if (lastCount == llastCount)
                        {%>-<%}
                      } %>
                </td>
        </tr>
        <%}//循环
        %>
        <tr>
            <td colspan='13' class='center td_chart'>
                工作总量统计图
            </td>
        </tr>
    </table>
    <%--生成chart--%>
    <%
        StringBuilder sBuilder = new StringBuilder();


        sql = "SELECT distinct EmpFrom,EmpFromT FROM ND" + int.Parse(flow.No) +
           "Track WHERE NDFrom='" + node.NodeID + "'";

        dt = BP.DA.DBAccess.RunSQLReturnTable(sql);


        int maxValue = 0;
        int setCount = 0;
        string chartTitle = "";
        string exportStr = "exportEnabled='1'  exportAtClient='0'" +
                                           " exportAction='download' " +
                                           " exportHandler='../../../../Comm/Charts/FCExport.aspx' " +
                                           " exportDialogMessage='正在生成,请稍候...'  " +
                                           " exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' ";

        if (dt.Rows.Count <= 4)//使用MSCOL图形 取最近一年的数据
        {
            dTime = DateTime.Now;

            List<string> listMouth = new List<string>();

            for (int i = -11; i <= 0; i++)
            {
                listMouth.Add(dTime.AddMonths(i).ToString("yyyy-MM"));
            }

            sBuilder.Append("<categories>");
            foreach (string lm in listMouth)
            {
                sBuilder.Append("<category label='" + lm + "' />");
            }
            sBuilder.Append("</categories>");


            foreach (System.Data.DataRow dr in dt.Rows)
            {
                sBuilder.Append("<dataset seriesName='" + dr["EmpFromT"] + "'>");

                foreach (string lm in listMouth)
                {
                    BP.DA.Paras ps = new BP.DA.Paras();
                    ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                        " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                        "NDFrom AND RDT LIKE'%" + lm + "%'";

                    ps.Add(BP.WF.TrackAttr.EmpFrom, dr["EmpFrom"].ToString());
                    ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                    setCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                    if (setCount > maxValue)
                        maxValue = setCount;

                    sBuilder.Append("<set value='" + setCount + "' />");
                }


                sBuilder.Append("</dataset>");
            }

            sBuilder.Append("</chart>");

            maxValue += 10;

            chartTitle = BP.DA.DataTableConvertJson.GetFilteredStrForJSON("节点:[" + node.NodeID + "]" + node.Name +
                         "-" + listMouth[0] + "至" + listMouth[11] + "统计");

            //exportStr += "exportFileName='" + chartTitle + "'";
            sBuilder.Insert(0, "<chart unescapeLinks='0'  " + exportStr + " baseFontSize='14'  subcaption='" + chartTitle +
                               "' formatNumberScale='0' divLineAlpha='20'" +
                               " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                               " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                               " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue + "'>");
        }
        else
        {
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                BP.DA.Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr + "NDFrom";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dr["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                setCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                if (setCount > maxValue)
                    maxValue = setCount;

                sBuilder.Append("<set label='" + dr["EmpFromT"] + "' value='" + setCount + "' />");
            }

            sBuilder.Append("</chart>");

            maxValue += 10;//加10  不置顶

            chartTitle = BP.DA.DataTableConvertJson.GetFilteredStrForJSON("节点:[" + node.NodeID + "]" + node.Name + "-总数统计");

            //exportStr += "exportFileName='" + chartTitle + "'";
            sBuilder.Insert(0, "<chart unescapeLinks='0'  " + exportStr + "  baseFontSize='12' subcaption='" + chartTitle +
                               "' formatNumberScale='0' divLineAlpha='20'" +
                               " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                               " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                               " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue + "'>");
        }


        this.HiddenField1.Value = "{rowsCount:\"" + dt.Rows.Count + "\",chartData:\"" + sBuilder.ToString() + "\"}";
    %>
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <div id="chartDiv">
    </div>
    <script type="text/javascript">
        $(function () {
            var chartData = $("#<%=HiddenField1.ClientID%>").val();

            var data = eval('(' + chartData + ')');
            var w = $("#tab_0").css("width"); //当前宽度
            var chart;
            if (data.rowsCount <= 4) {
                chart = new FusionCharts("../../../../Comm/Charts/swf/MSColumn3D.swf", "CharZ", w, '350', '0', '0');
                chart.setDataXML(data.chartData);
                chart.render("chartDiv");
            } else {
                var chart = new FusionCharts("../../../../Comm/Charts/swf/Column3D.swf", "CharZ", w, '350', '0', '0');
                chart.setDataXML(data.chartData);
                chart.render("chartDiv");
            }
        });
    </script>
</asp:Content>
