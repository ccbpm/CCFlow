<%@ Page Title="数据分析" Language="C#" MasterPageFile="~/WF/Comm/MasterPage.master"
    AutoEventWireup="true" CodeBehind="Rpt2DBA.aspx.cs" Inherits="CCFlow.WF.Comm.Rpt2DBA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="JScript.js" type="text/javascript"></script>
    <link href="../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <%-- <link href="Charts/css/Style.css" rel="stylesheet" type="text/css" />
    <script src="Charts/js/FusionCharts.js" type="text/javascript"></script>--%>
    <link href="Charts/css/style_3.css" rel="stylesheet" type="text/css" />
    <link href="Charts/css/prettify.css" rel="stylesheet" type="text/css" />
    <%-- <script src="Charts/js/jquery-1.4.2.min.js" type="text/javascript"></script>--%>
    <script src="Charts/js/prettify.js" type="text/javascript"></script>
    <script src="Charts/js/json2_3.js" type="text/javascript"></script>
    <script src="Charts/js/FusionCharts.js" type="text/javascript"></script>
    <script src="Charts/js/FusionChartsExportComponent.js" type="text/javascript"></script>
    <script src="JS/Calendar/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        ul li
        {
            list-style: none;
            background: url(Charts/imgs/li_icon.gif) no-repeat;
            padding-left: 20px;
            text-decoration: none;
            line-height: 6px;
            margin-top: 12px;
        }
        ul li a
        {
            text-decoration: none;
            color: blue;
        }
        ul li a:hover
        {
            text-decoration: underline;
            color: #D64646;
        }
        .anoCss
        {
            width: 950px;
            height: 550px;
        }
        .queryLi
        {
            /*background:#E0ECFF;*/
            margin-left: 0;
            list-style-type: none;
            list-style-image: none;
            list-style-position: outside;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $(".largerMet").bind('contextmenu', function (e) {
                e.preventDefault();
                $('#mm').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            });
            $("#largerBtn").bind('click', function () {
                var tab = $('#tt').tabs('getSelected');
                var index = $('#tt').tabs('getTabIndex', tab);

                switch (index) {
                    case 1:
                        var columnW = $('#column_chart_div object').css("width");
                        var columnH = $('#column_chart_div object').css("height");
                        var cw = parseInt(columnW.replace('px', '')) + 20;
                        var ch = parseInt(columnH.replace('px', '')) + 10;
                        $('#column_chart_div object').css({ "width": cw });
                        $('#column_chart_div object').css({ "height": ch });
                        break;
                    case 2:
                        var pieW = $('#pie_chart_div object').css("width");
                        var pieH = $('#pie_chart_div object').css("height");
                        var pw = parseInt(pieW.replace('px', '')) + 20;
                        var ph = parseInt(pieH.replace('px', '')) + 10;
                        $('#pie_chart_div object').css({ "width": pw });
                        $('#pie_chart_div object').css({ "height": ph });
                        break;
                    case 3:
                        var lineW = $('#line_chart_div object').css("width");
                        var lineH = $('#line_chart_div object').css("height");
                        var lw = parseInt(lineW.replace('px', '')) + 20;
                        var lh = parseInt(lineH.replace('px', '')) + 10;
                        $('#line_chart_div object').css({ "width": lw });
                        $('#line_chart_div object').css({ "height": lh });
                        break;
                    default:
                        break;
                }
            });

            var myExportComponent = new FusionChartsExportObject("fcExporter1", "Charts/FCExporter.swf");
            myExportComponent.componentAttributes.bgColor = 'E0ECFF';
            myExportComponent.componentAttributes.fontFace = 'Arial';
            myExportComponent.componentAttributes.fontColor = '0372AB';
            myExportComponent.componentAttributes.fontSize = '17';
            myExportComponent.componentAttributes.btnsavetitle = '保存图表'
            myExportComponent.componentAttributes.btndisabledtitle = '等待导出';
            myExportComponent.Render("fcexpDiv");


            //针对每日伤病
            var da = new Date;
            var y = da.getFullYear();
            var m = da.getMonth() + 1;
            var d = da.getDate();

            var myDate = new Date();
            myDate.setDate(myDate.getDate() - 6);
            var dateArray = [];
            var dateTemp;
            var flag = 1;
            for (var i = 0; i < 7; i++) {
                var iM = myDate.getMonth() + 1;
                if (iM < 10) {
                    iM = "0" + iM;
                }
                var iD = myDate.getDate();
                if (iD < 10) {
                    iD = "0" + iD;
                }
                dateTemp = myDate.getFullYear() + "-" + iM + "-" + iD;
                dateArray.push(dateTemp);
                myDate.setDate(myDate.getDate() + flag);
            }
            //初次加载默认时间
            if (m < 10) {
                m = "0" + m;
            }
            if (d < 10) {
                d = "0" + d;
            }
            $('#tosdate').val(y + "-" + m + "-" + d);
            $('#fromsdate').val(dateArray[0]);

        });
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%
        BP.Rpt.Rpt2Base rpt = BP.En.ClassFactory.GetRpt2Base(this.Request.QueryString["Rpt2Name"]);
        string rptName = this.Rpt2Name;
        //分析项目,如果取不到，就获取默认来取.
        string attrOfSelect = this.Request.QueryString["Idx"];
        if (DataType.IsNullOrEmpty(attrOfSelect))
            attrOfSelect = rpt.AttrDefSelected.ToString();
        
        //获得报表对象.
        BP.Rpt.Rpt2Attr myattr = rpt.AttrsOfGroup.GetD2(int.Parse(attrOfSelect));
    %>
    <div id="mm" class="easyui-menu" style="width: 80px;">
        <div id='largerBtn' data-options="name:'save',iconCls:'icon-search'">
            放大</div>
    </div>
    <div id="tt" class="easyui-tabs" data-options="fit:true,tools:'#tab-tools'">
        <div title="分组数据" id="table_info" data-options="iconCls:'icon-table'" style="padding: 5px;">
            <div class="easyui-layout" data-options="fit:true">
                <div data-options="region:'west',title:'<%=myattr.LeftMenuTitle??" " %>'" style="width: 250px;
                    padding: 5px">
                    <%=myattr.LeftMenu%>
                </div>
                <div data-options="region:'center'">
                    <!-- 内容显示区域 -->
                    <%
                        //获得数据源.

                        System.Data.DataTable dt = myattr.DBDataTable;
                        string leftHtml = "";
                        if (dt.Rows.Count == 0)
                        {
                            leftHtml += "<h2 style='text-align:center'>" + myattr.Title + "</h2><hr><p style='color:black;font-size:14px;font-weight:bold;text-align:center'>无数据...</p><br>";
                    %>
                    <%=leftHtml %>
                </div>
            </div>
        </div>
    </div>
    <% return;
                 } %>

    <table class="Table" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
        <tr>
            <td class="GroupTitle" style="text-align: center">
                序
            </td>
            <%
                string title = "";
                // 标题.
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.Contains("_"))
                        continue; /*约定的参数列,用于显示明细表.*/
                    title += "<td class='GroupTitle'>" + dc.ColumnName + "</td>";
                }

                title += "<td class='GroupTitle'>详细</td>";
            %>
            <%=title %>
        </tr>
        <%   //数据表.
            int idx = 0;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                string kvs = "";
                idx++;
        %>
        <tr>
            <td class="Idx">
                <%=idx %>
            </td>
            <% 
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName.Contains("_"))
                    {
                        kvs += "&" + dc.ColumnName + "=" + dr[dc.ColumnName];
                        continue; /*约定的参数列,用于显示明细表.*/
                    }
            %>
            <td>
                <%=dr[dc.ColumnName]%>
            </td>
            <%  }
                kvs = CheckUrlParams(kvs);
            %>
            <td>
                <a href="javascript:OpenEasyUiDialog('Rpt2DBADtl.aspx?Rpt2Name=<%=rptName%>&Idx=<%=attrOfSelect%><%=kvs

%>','eudlgframe','详细信息',760,470,'icon-sheet');">详细</a>
            </td>
        </tr>
        <%}%>
    </table>
    <!-- qin -->
    <!--qin-->
    <!-- end内容显示区域 -->
    <% if (DataType.IsNullOrEmpty(myattr.DESC) == false)
       { %>
    <br />
    <br />
    <%=myattr.DESC %>
    <%} %>
    </div> 
    </div> </div>
    <%
        string numFieldStrs = "";  //数值列-字符串,生成后用逗号分割.
        int numColCount = 0; // 数值列的数量.

        string categoryName = "";
        System.Data.DataTable displayDt = dt.Copy(); // 克隆一份数据,对数据进行处理.
        foreach (System.Data.DataColumn dc in dt.Columns)//判断每行数据是否包含多项需要展示的数据
        {
            if (dc.ColumnName.Contains("_"))
            {
                displayDt.Columns.Remove(dc.ColumnName);
                continue;
            }
            if (dc.DataType == typeof(string))
            {
                categoryName = dc.ColumnName;//字段名
                displayDt.Columns.Remove(dc.ColumnName);
                continue;
            }
            numColCount += 1; //数值列的数量 +1 
            numFieldStrs += dc.ColumnName + ",";//数值列-字符串,生成后用逗号分割
        }

        if (dt.Rows.Count == 0)
            throw new Exception("@数据源组织错误,没有行数据。");
        if (numColCount == 0)
            throw new Exception("@数据源组织错误,没有找到分析的数值的列,不能显示图形.");

        //求出最小最大值---郭要求整数,秦修改
        int minValue = int.Parse(myattr.MinValue.ToString());
        int maxValue = int.Parse(myattr.MaxValue.ToString());
        
        if (maxValue == 0)
        {
            /*说明：属性已经设置最大最小值.*/
            for (int row = 0; row < displayDt.Rows.Count; row++)
            {
                for (int column = 0; column < displayDt.Columns.Count; column++)
                {
                    minValue = Math.Min(minValue, Convert.ToInt32(displayDt.Rows[row][column]));
                    maxValue = Math.Max(maxValue, Convert.ToInt32(displayDt.Rows[row][column]));
                }
            }
            maxValue = maxValue + maxValue / 10; //把数值列的最大值加上 10% 目的为了，不让其顶满格.

            if (maxValue <= 10)
                maxValue = 10;
        }
        string[] colorArray = { "9D080D", "588526", "F6BD0F", "AFD8F8", "8BBA00", "FF8E46", "008E8E", "D64646", "8E468E", "B3AA00", "008ED6", "A186BE" };//颜色的集合
        StringBuilder columnStrB = new StringBuilder(); //柱图，数据格式.
        StringBuilder lineStrB = new StringBuilder();
        StringBuilder pieStrB = new StringBuilder();

        #region 取出数据定义.
        string chartData = ""; //定义数据，用于柱，折线图.
        string pieData = "";//用于pie图
        chartData += " <categories font='Arial' fontSize='11' fontColor='000000' >";
        foreach (System.Data.DataRow dr in dt.Rows)
        {
            chartData += "<category  label='" + dr[categoryName] + "' />";
        }
        chartData += "</categories>";

        // 遍历数值类型的列.
        string[] strs = numFieldStrs.Split(',');

        int columnAndLine = 0;
        int pie = 0;
        int pieAddFirstNum = 0;
        var kvs2 = "";
        foreach (string str in strs)
        {
            if (DataType.IsNullOrEmpty(str) == true)
                continue;
            chartData += "<dataset seriesname='" + str + "' color='" + colorArray[columnAndLine] + "' >";
            columnAndLine++;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                kvs2 = "";

                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.Contains("_"))
                    {
                        kvs2 += "&" + col.ColumnName + "=" + dr[col.ColumnName];
                        continue;
                    }
                }

                kvs2 = CheckUrlParams(kvs2);

                chartData += " <set link='JavaScript:OpenEasyUiDialog(\\\"Rpt2DBADtl.aspx?Rpt2Name=" + rptName + "&Idx=" +attrOfSelect + kvs2 + "\\\",\\\"eudlgframe\\\",\\\" \\\",760,470,\\\"icon-sheet\\\")' value='" + dr[str] + "' />";
                if (pieAddFirstNum == 0)
                {
                    pieData += "<set link='JavaScript:OpenEasyUiDialog(\\\"Rpt2DBADtl.aspx?Rpt2Name=" + rptName + "&Idx=" +attrOfSelect + kvs2 + "\\\",\\\"eudlgframe\\\",\\\" \\\",760,470,\\\"icon-sheet\\\")' value='" + dr[str] + "' label='" +dr[categoryName].ToString() + "' color='" + colorArray[pie] + "'/>";
                    pie++;

                    if (pie >= colorArray.Length)
                        pie = 0;
                }

            }
            if (columnAndLine >= colorArray.Length)
                columnAndLine = 0;

            pieAddFirstNum += 1;
            pie = 0;
            chartData += " </dataset>";
        }
        #endregion 取出数据定义.

        pieStrB.Insert(0, "<styles><definition><style name='CaptionFont' type='FONT' size='12' bold='1' /><style name='LabelFont' type='FONT' color='2E4A89' bgColor='FFFFFF' bold='1' />"
                        + "<style name='ToolTipFont' type='FONT' bgColor='2E4A89' borderColor='2E4A89' /></definition><application><apply toObject='CAPTION' styles='CaptionFont' />"
                         + "<apply toObject='DATALABELS' styles='LabelFont' /><apply toObject='TOOLTIP' styles='ToolTIpFont' /></application>)</styles>");

        pieStrB.Insert(0, "<chart exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' decimalPrecision='4' caption='" + myattr.Title + "' baseFontColor='FFFFFF' bgColor='2E4A89, 90B1DE' pieYScale='30'  pieSliceDepth='8' smartLineColor='FFFFFF'>");

        columnStrB.Append(chartData);
        lineStrB.Append(chartData);


        columnStrB.Insert(0, " rotateNames='0' yAxisMaxValue='" + maxValue + "'  divLineColor='CCCCCC' divLineAlpha='80' decimalPrecision='0'> ");
        columnStrB.Insert(0, " AlternateHGridAlpha='50' ");
        columnStrB.Insert(0, " yAxisName='" + myattr.yAxisName + "'");  // 底部显示的数据.
        columnStrB.Insert(0, " xAxisName=' " + myattr.xAxisName + "'");     // 左边显示的数据.
        columnStrB.Insert(0, " subcaption='" + myattr.Title + "' ");
        columnStrB.Insert(0, " canvasBgColor='" + myattr.canvasBgColor + "'  ");
        columnStrB.Insert(0, " canvasBaseColor='" + myattr.canvasBaseColor + "'  ");
        columnStrB.Insert(0, " canvasBaseDepth='" + myattr.canvasBaseDepth + "'  ");
        columnStrB.Insert(0, " canvasBgDepth='" + myattr.canvasBgDepth + "'  ");
        columnStrB.Insert(0, " showCanvasBg='" + myattr.showCanvasBg + "'  ");
        columnStrB.Insert(0, " showCanvasBase='" + myattr.showCanvasBase + "'  ");
        columnStrB.Insert(0, "<chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' formatNumberScale='0' decimalPrecision='4' showValues='1' baseFontColor='FFFFFF' outCnvBaseFontColor='2E4A89' hoverCapBgColor='2E4A89' baseFontSize='12' bgColor='B8D288,FFFFFF'  showhovercap='1' outCnvBaseFontSize='18' numDivLines='" + myattr.numDivLines + "'");

        lineStrB.Insert(0, "AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' >");
        lineStrB.Insert(0, "numberPrefix='' showNames='1' showValues='1'  showAlternateHGridColor='1' ");
        lineStrB.Insert(0, "yAxisMinValue='" + minValue + "'  yAxisMaxValue='" + maxValue + "' decimalPrecision='0' formatNumberScale='0' ");
        lineStrB.Insert(0, " yAxisName='" + myattr.yAxisName + "' ");  // 底部显示的数据.
        lineStrB.Insert(0, " xAxisName='" + myattr.xAxisName + "' ");     // 左边显示的数据.
        lineStrB.Insert(0, " <chart   exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' decimalPrecision='4'  hoverCapBgColor='AFD8F8' bgColor='2E4A89, 90B1DE' outCnvBaseFontSize='12' anchorBgColor='008ED6' caption='" + myattr.Title + "'");

        string columnStr = columnStrB.ToString() + "</chart>";
        string lineStr = lineStrB.ToString() + "</chart>";
        string pieStr = pieStrB.ToString() + pieData;

        //针对每周伤病动态分析-----取数字连续上升3次为要报告的项.ing
        string warningInfo = "";
        int riseCount = 0;
        if (rptName == "BP.YueBing.DBASBEveryWeek")
        {
            for (int c = 0; c < displayDt.Columns.Count; c++)
            {
                for (int i = 0; i < displayDt.Rows.Count; i++)
                {
                    if (i + 1 == displayDt.Rows.Count)
                    {
                        continue;
                    }
                    if (int.Parse(displayDt.Rows[i][c].ToString()) <= int.Parse(displayDt.Rows[i + 1][c].ToString()))
                    {
                        riseCount += 1;
                    }
                    else
                    {
                        riseCount = 0;
                    }
                    if (riseCount == 2)
                    {
                        riseCount = 0;
                        if (displayDt.Columns[c].ColumnName == "呼吸道疾病")
                        {
                            warningInfo += displayDt.Columns[c].ColumnName + "---连续3天呈现上升趋势,请注意预防!</br></br>";
                        }
                        break;
                    }
                }
                riseCount = 0;
            }
        }
        //读取webConfig配置的底部信息
        string FootInformation = BP.Sys.SystemConfig.AppSettings["FootInformation"];
    %>
    <div title="柱状图" id='column_info' data-options="iconCls:'icon-columnchart'" style="padding: 5px;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" id="column_chart_div" class="largerMet" align="center"
                style="background-color: rgb(232,241,251); padding-top: 20px;">
                系统组织数据错误,请联系管理员.
            </div>
        </div>
    </div>
    <div title="饼状图" id='pie_info' data-options="iconCls:'icon-piechart'" style="padding: 5px;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" id="pie_chart_div" class="largerMet" align="center"
                style="background-color: rgb(232,241,251); padding-top: 20px;">
                系统组织数据错误,请联系管理员.
            </div>
        </div>
    </div>
    <div title="折线图" id='line_info' data-options="iconCls:'icon-linechart'" style="padding: 5px;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'center'" id="line_chart_div" class="largerMet" align="center"
                style="background-color: rgb(232,241,251); padding-top: 20px;">
                系统组织数据错误,请联系管理员.
            </div>
        </div>
    </div>
    </div>
    <div id="tab-tools">
        <div href="#" id="fcexpDiv" style="margin-top: -10px;">
        </div>
    </div>
    <script type="text/javascript">
        window.onload = function () {
            var againVal = 0;
            var curDate = new Date();
            var warningInfo = "<span style='color:red; font-size:17px;'><%=warningInfo %></span>";

            if(warningInfo.length > 48){
                $.messager.show({
				    title:'警告',
				    msg:warningInfo,
				    timeout:0,
				    showType:'fade'
			    });
            }

            $('#tt').tabs({
                onSelect: function (title, index) {
                    if (index != 0) {
                        var chart;
                        againVal += 1;
                        switch (index) {
                            case 1:
                                if ("<%=myattr.ColumnChartShowType %>"=="ShuXiang") 
                                chart = new FusionCharts('Charts/MSBar3D.swf', 'ChartIdStack',  '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');
                                if ("<%=myattr.ColumnChartShowType %>"=="ShuXiangAdd") //OK
                                chart = new FusionCharts('Charts/StackedBar3D.swf', 'ChartId', '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');
                                if ("<%=myattr.ColumnChartShowType %>"=="HengXiang") 
                                chart = new FusionCharts('Charts/MSColumn3D.swf', 'ChartId', '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');
                                if ("<%=myattr.ColumnChartShowType %>"=="HengXiangAdd") //ok
                                chart = new FusionCharts('Charts/StackedColumn3D.swf', 'ChartId', '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');

                                chart.setDataXML("<%=columnStr %>");
                                chart.render('column_chart_div');
                                break;
                            case 2:
                               chart = new FusionCharts("Charts/Pie3D.swf", "ChartIdP", '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');
                               var curPieStr= "<%=pieStr %>" +"</chart>";
                               chart.setDataXML(curPieStr);
                               chart.render("pie_chart_div");
                              FusionCharts("ChartIdP").configureLink({
                                width : '100%',
                                height: '100%',
                                id: 'linked-chart',
                                renderAt : "linkedchart-container",
                                overlayButton: { show : false }
                            }, 0);
							 

                            FusionCharts("ChartIdP").addEventListener('BeforeLinkedItemOpen',
                            function() {
                                dialog.dialog('open');
                            });
                                break;
                            case 3:
                                chart = new FusionCharts("Charts/MSLine.swf", "ChartIdZ", '<%=myattr.W %>', '<%=myattr.H %>', '0', '0');
                                chart.setDataXML("<%=lineStr%>");
                                chart.render("line_chart_div");
                                break;
                            default:
                        }
                    }
                }
            });
           $('#tt').tabs('select', <%= (int)myattr.DefaultShowChartType %>);
           if ("<%=myattr.IsEnableColumn %>"=="False") {
               $('#tt').tabs('disableTab', 1);
           }
           if ("<%=myattr.IsEnableLine %>"=="False") {
               $('#tt').tabs('disableTab', 2);
           }
           if ("<%=myattr.IsEnablePie %>"=="False") {
               $('#tt').tabs('disableTab', 3);
           }
        };
    </script>
</asp:Content>
