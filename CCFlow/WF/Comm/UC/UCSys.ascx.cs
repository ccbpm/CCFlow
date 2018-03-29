//NOTE:修改说明
//2015.1.6 liuxc,解决1个人员处于不同部门的情况下，如果分组显示则报错的问题，每个Checkbox的ID再+分组的值来区分，保存的时候，同1个人员只保存一次；修改涉及本类及Dot2Dot.ascx.cs
namespace CCFlow.WF.Comm.UC
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.Odbc;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Linq;
    using BP.DA;
    using BP.En;
    using BP.Sys;
    using BP.Web;
    using BP.Web.Controls;
    using BP.Web.UC;
    using BP.Sys.XML;
    using BP.Port;
    using BP.Web.Comm;
    using BP;
    using CCFlow.Web.Comm;
    using System.Collections.Generic;
    using System.Text;

    // using OWC10;
    // using Microsoft.Office.Interop.Owc11;
    /// <summary>
    ///		UCSys 的摘要说明。
    /// </summary>
    public partial class UCSys : BP.Web.UC.UCBase
    {
        public static string FilesViewStr(string enName, object pk)
        {
            string url = "../Comm/FileManager.aspx?EnsName=" + enName + "&PK=" + pk.ToString();

            //string strs="<a href=\"javascript:WinOpen("") \" >附件</>";
            //string strs="<a href=\"javascript:WinOpen('"+url+"') \" >编辑附件</>";
            string strs = "";
            SysFileManagers ens = new SysFileManagers(enName, pk.ToString());
            string path = System.Web.HttpContext.Current.Request.ApplicationPath;
            foreach (SysFileManager file in ens)
            {
                strs += "<img src='../Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='" + path + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a>&nbsp;";
                if (file.Rec == WebUser.No)
                {
                    strs += "<a title='打开它' href=\"javascript:DoAction('../Comm/Do.aspx?ActionType=" + (int)ActionType.DeleteFile + "&OID=" + file.OID + "&EnsName=" + enName + "&PK=" + pk + "','删除文件《" + file.MyFileName + file.MyFileExt + "》')\" ><img src='" + path + "../Img/Btn/delete.gif' border=0 alt='删除此附件' /></a>&nbsp;";
                }
            }
            return strs;
        }

        public static string FilesViewStr1(string enName, object pk)
        {
            string url = "../Comm/FileManager.aspx?EnsName=" + enName + "&PK=" + pk.ToString();

            //string strs="<a href=\"javascript:WinOpen("") \" >附件</>";
            string strs = "<a href=\"javascript:WinOpen('" + url + "') \" >编辑附件</>";
            SysFileManagers ens = new SysFileManagers(enName, pk.ToString());
            foreach (SysFileManager file in ens)
            {
                strs += "<img src='../Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='../" + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a>&nbsp;";
            }
            return strs;
        }

        public string GenerIt()
        {
            ////创建一个图形容器对象
            //OWC11.ChartSpace objCSpace = new OWC11.ChartSpaceClass();
            ////在图形容器中增加一个图形对象
            //OWC11.ChChart objChart = objCSpace.Charts.Add(0);
            ////将图形的类型设置为柱状图的一种
            //objChart.Type = OWC11.ChartChartTypeEnum.chChartTypeColumnStacked;
            ////将图形容器的边框颜色设置为白色
            //objCSpace.Border.Color = "White";

            ////显示标题
            //objChart.HasTitle = true;
            ////设置标题内容
            //objChart.Title.Caption = "统计图测试";
            ////设置标题字体的大小
            //objChart.Title.Font.Size = 10;
            ////设置标题为粗体
            //objChart.Title.Font.Bold = true;
            ////设置标题颜色为红色
            //objChart.Title.Font.Color = "Red";

            ////显示图例
            //objChart.HasLegend = true;
            ////设置图例字体大小
            //objChart.Legend.Font.Size = 10;
            ////设置图例位置为底端
            //objChart.Legend.Position = OWC11.ChartLegendPositionEnum.chLegendPositionBottom;

            ////在图形对象中添加一个系列
            //objChart.SeriesCollection.Add(0);
            ////给定系列的名字
            //objChart.SeriesCollection[0].SetData(OWC11.ChartDimensionsEnum.chDimSeriesNames,
            //    +(int)OWC11.ChartSpecialDataSourcesEnum.chDataLiteral, "指标");
            ////给定值
            //objChart.SeriesCollection[0].SetData(OWC11.ChartDimensionsEnum.chDimValues,
            //    +(int)OWC11.ChartSpecialDataSourcesEnum.chDataLiteral, "10\t40\t58\t55\t44");

            ////显示数据，创建GIF文件的相对路径.
            //string FileName = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + ".gif";
            //objCSpace.ExportPicture(@"E:\Projects\Study\OwcImg\ChartDetail.gif", "GIF", 450, 300);


            //return FileName;
            //Image1.ImageUrl = "Http://localhost/Study/OwcImg/ChartDetail.gif";
            return null;
        }

        public static string GenerChart(DataTable dt, string colOfGroupField, string colOfGroupName,
            string colOfNumField, string colOfNumName, string title, int chartHeight, int chartWidth, ChartType ct)
        {
            string fileName = "";
            //string strCategory = "";
            //string strValue = "";
            ////声明对象
            //ChartSpace ThisChart = new ChartSpaceClass();
            //ChChart ThisChChart = ThisChart.Charts.Add(0);
            //ChSeries ThisChSeries = ThisChChart.SeriesCollection.Add(0);

            ////显示图例
            //ThisChChart.HasLegend = true;
            ////标题
            //ThisChChart.HasTitle = true;
            //ThisChChart.Title.Caption = title;

            ////给定x,y轴图示说明
            //ThisChChart.Axes[0].HasTitle = true;
            //ThisChChart.Axes[1].HasTitle = true;

            //ThisChChart.Axes[0].Title.Caption = colOfGroupName;
            //ThisChChart.Axes[1].Title.Caption = colOfNumName;

            //switch (ct)
            //{
            //    case ChartType.Histogram:
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField].ToString() + '\t';
            //            strValue += dr[colOfNumField].ToString() + '\t';
            //        }
            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
            //        ThisChChart.Overlap = 50;
            //        //旋转
            //        ThisChChart.Rotation = 360;
            //        ThisChChart.Inclination = 10;
            //        //背景颜色
            //        ThisChChart.PlotArea.Interior.Color = "white";
            //        //底色
            //        ThisChChart.PlotArea.Floor.Interior.Color = "green";
            //        ////给定series的名字
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), colOfGroupName);
            //        //给定分类
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimCategories,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), strCategory);
            //        //给定值
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimValues,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), strValue);
            //        break;
            //    case ChartType.Pie:
            //        // 产生数据
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField].ToString() + '\t';
            //            strValue += dr[colOfNumField].ToString() + '\t';
            //        }

            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypePie3D;
            //        ThisChChart.SeriesCollection.Add(0);
            //        //在图表上显示数据
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection.Add();
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position = ChartDataLabelPositionEnum.chLabelPositionAutomatic;
            //        ThisChChart.SeriesCollection[0].Marker.Style = ChartMarkerStyleEnum.chMarkerStyleCircle;

            //        //给定该组图表数据的名字 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, "strSeriesName");

            //        //给定数据分类 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, strCategory);

            //        //给定值 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
            //            (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
            //        break;
            //    case ChartType.Line:
            //        // 产生数据
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField].ToString() + '\t';
            //            strValue += dr[colOfNumField].ToString() + '\t';
            //        }
            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypeLineStacked;
            //        ThisChChart.SeriesCollection.Add(0);
            //        //在图表上显示数据
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection.Add();
            //        //ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position=ChartDataLabelPositionEnum.chLabelPositionAutomatic;
            //        //ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position=ChartDataLabelPositionEnum.chLabelPositionOutsideBase;

            //        ThisChChart.SeriesCollection[0].Marker.Style = ChartMarkerStyleEnum.chMarkerStyleCircle;

            //        //给定该组图表数据的名字 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, "strSeriesName");

            //        //给定数据分类 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, strCategory);

            //        //给定值 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
            //            (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
            //        break;
            //}

            ////导出图像文件
            ////ThisChart.ExportPicture("G:\\chart.gif","gif",600,350);

            //string fileName = ct.ToString() + WebUser.No + ".gif";
            //string strAbsolutePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + fileName;
            //try
            //{
            //    ThisChart.ExportPicture(strAbsolutePath, "GIF", chartWidth, chartHeight);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("@不能创建文件,可能是权限的问题，请把该目录设置为任何人都可以修改。" + strAbsolutePath + " Exception:" + ex.Message);
            //}
            return fileName;
        }

        /// <summary>
        /// 生成MS Chart柱状图
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="xfield">X轴要显示的字段</param>
        /// <param name="xdesc">X轴要显示的字段文本</param>
        /// <param name="yfields">Y轴要分析的项目
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.绑定的字段</para>
        /// <para>Value.绑定的字段显示名称</para>
        /// </remarks></param>
        /// <param name="title">图表标题</param>
        /// <param name="chartWidth">单个图表的宽度</param>
        /// <param name="chartHeight">单个图表的高度</param>
        public void GenerateColumnChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            StringBuilder columnStrB = new StringBuilder();

            columnStrB.Append(" <chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' decimalPrecision='4'  hoverCapBgColor='ffffff' bgColor='B8D288,FFFFFF' outCnvBaseFontSize='12' anchorBgColor='008ED6' caption='" + title + "'");
            columnStrB.Append("AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' >");
            columnStrB.Append("numberPrefix='' showNames='1' showValues='1'  showAlternateHGridColor='1' ");
            columnStrB.Append(" decimalPrecision='0' formatNumberScale='0' ");
            columnStrB.Append(" yAxisName='hkhkhk' ");  // 底部显示的数据.
            columnStrB.Append(" xAxisName='adadasd' ");     // 左边显示的数据.

            columnStrB.Append(" <categories font='Arial' fontSize='11' fontColor='000000' >");
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                columnStrB.Append("<category  label='" + dr[xfield] + "' />");
            }
            columnStrB.Append("</categories>");
            foreach (var yfield in yfields)
            {
                if (string.IsNullOrEmpty(yfield.Value) == true)
                    continue;
                columnStrB.Append("<dataset seriesname='" + yfield.Value + "'  >");
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    columnStrB.Append(" <set value='" + dr[yfield.Key] + "' />");
                }
                columnStrB.Append(" </dataset>");
            }
            columnStrB.Append(" </chart>");
            string q = columnStrB.ToString();

            this.Add("<script type='text/javascript'>"
             + "  var chart = new FusionCharts(\"/WF/Comm/Charts/swf/MSColumn3D.swf\", \"ChartLine\", '" + chartWidth + "', '" + chartHeight + "', '0', '0');"
             + "  chart.setDataXML(\"" + q + "\");"
             + "  chart.render(\"column_chart_div\");"
             + "</script>"
             );


            //var legendName = "MainLegend_";
            //var seriesName = "Series_";

            //var chart = new Chart();
            //chart.ID = "Chart_" + xfield;
            //chart.Width = new Unit(chartWidth);
            //chart.Height = new Unit(chartHeight);
            //chart.Palette = ChartColorPalette.BrightPastel;
            //chart.BackColor = Color.FromArgb(224, 236, 255);//Color.FromArgb(243, 223, 193);
            //chart.BorderlineDashStyle = ChartDashStyle.Solid;
            //chart.BackGradientStyle = GradientStyle.TopBottom;
            //chart.BorderlineWidth = 1;
            //chart.BorderlineColor = Color.FromArgb(149, 184, 231);// Color.FromArgb(181, 64, 1);
            //chart.BorderSkin = new BorderSkin { SkinStyle = BorderSkinStyle.Emboss };

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - 柱状图");
            //chartTitle.Font = new Font(new FontFamily("宋体"), 10.5f, FontStyle.Bold);
            //chartTitle.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //chartTitle.ShadowOffset = 3;
            //chartTitle.ForeColor = Color.FromArgb(26, 59, 105);

            //var legend = chart.Legends.Add(legendName + xfield);
            //legend.Docking = Docking.Bottom;
            //legend.TitleFont = new Font(new FontFamily("Microsoft Sans Serif"), 8, FontStyle.Bold);
            //legend.BackColor = Color.Transparent;
            //legend.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //legend.IsTextAutoFit = false;
            //legend.LegendStyle = LegendStyle.Table;

            //var chartArea = chart.ChartAreas.Add("ChartArea_" + xfield);
            //chartArea.BorderColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.BackSecondaryColor = Color.White;
            //chartArea.BackColor = Color.OldLace;
            //chartArea.ShadowColor = Color.Transparent;
            //chartArea.BackGradientStyle = GradientStyle.TopBottom;
            //chartArea.Area3DStyle.Rotation = 10;
            //chartArea.Area3DStyle.Perspective = 10;
            //chartArea.Area3DStyle.Inclination = 15;
            //chartArea.Area3DStyle.IsRightAngleAxes = false;
            //chartArea.Area3DStyle.WallWidth = 0;
            //chartArea.Area3DStyle.IsClustered = false;
            //chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisY.LabelAutoFitMaxFontSize = 8;
            //chartArea.AxisY.LabelStyle.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //chartArea.AxisY.LabelStyle.IsEndLabelVisible = false;
            //chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.Title = xdesc;
            //chartArea.AxisX.TitleAlignment = StringAlignment.Far;
            //chartArea.AxisX.TitleFont = new Font(new FontFamily("宋体"), 9f, FontStyle.Bold);

            //Series series = null;
            //foreach (var yfield in yfields)
            //{
            //    series = chart.Series.Add(seriesName + xfield + "_" + yfield.Key);
            //    series.Legend = legend.Name;
            //    series.LegendText = yfield.Value;
            //    series.ChartArea = chartArea.Name;
            //    series.ChartType = SeriesChartType.Column;
            //    series.BorderColor = Color.FromArgb(180, 26, 59, 105);
            //    series.XValueMember = xfield;
            //    series.YValueMembers = yfield.Key;
            //    series.IsValueShownAsLabel = true;
            //    series.CustomProperties = "LabelStyle=Bottom";
            //}

            //if (yfields.Count == 1)
            //{
            //    chartArea.AxisY.Title = yfields.Values.First();
            //    chartArea.AxisY.TitleAlignment = StringAlignment.Far;
            //    chartArea.AxisY.TitleFont = new Font(new FontFamily("宋体"), 9f, FontStyle.Bold);
            //}

            //chart.DataSource = dt;
            //chart.DataBind();

            //this.Add(chart);
        }

        /// <summary>
        /// 生成MS Chart饼图
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="xfield">X轴要显示的字段</param>
        /// <param name="xdesc">X轴要显示的字段文本</param>
        /// <param name="yfields">Y轴要分析的项目
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.绑定的字段</para>
        /// <para>Value.绑定的字段显示名称</para>
        /// </remarks></param>
        /// <param name="title">图表标题</param>
        /// <param name="chartWidth">单个图表的宽度</param>
        /// <param name="chartHeight">单个图表的高度</param>
        public void GeneratePieChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            var firstKey = yfields.First();
            StringBuilder pieStrB = new StringBuilder();

            pieStrB.Append("<chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' " +
                            "exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' caption='" + title + "-" + firstKey.Value + "' outCnvBaseFontColor='FFFFFF' " +
                            "hoverCapBgColor='2E4A89' basefontcolor='FFFFFF' basefontsize='14' bgColor='2E4A89, 90B1DE' palette='2' animation='1' formatNumberScale='0' pieSliceDepth='30' startingAngle='125'>");



            foreach (DataRow dr in dt.Rows)
            {
                pieStrB.Append(" <set  value='" + dr[firstKey.Key] + "' label='" + dr[xfield] + "'/>");
            }
            pieStrB.Append("</chart>");

            string q = pieStrB.ToString();

            this.Add("<script type='text/javascript'>"
                + "  var chart = new FusionCharts(\"/WF/Comm/Charts/swf/Pie3D.swf\", \"ChartIdP\", '" + chartWidth + "', '" + chartHeight + "', '0', '0');"
                + "  chart.setDataXML(\"" + q + "\");"
                + "  chart.render(\"pie_chart_div\");"
                + "</script>"
                );

            //var legendName = "MainLegend_";
            //var seriesName = "Series_";

            //var chart = new Chart();
            //chart.ID = "Chart_" + xfield;
            //chart.Width = new Unit(chartWidth);
            //chart.Height = new Unit(chartHeight);
            //chart.Palette = ChartColorPalette.BrightPastel;
            //chart.BackColor = Color.FromArgb(224, 236, 255);//Color.FromArgb(243, 223, 193);
            //chart.BorderlineDashStyle = ChartDashStyle.Solid;
            //chart.BackGradientStyle = GradientStyle.TopBottom;
            //chart.BorderlineWidth = 1;
            //chart.BorderlineColor = Color.FromArgb(149, 184, 231);//Color.FromArgb(181, 64, 1);
            //chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - 饼状图");
            //chartTitle.Font = new Font(new FontFamily("宋体"), 10.5f, FontStyle.Bold);
            //chartTitle.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //chartTitle.ShadowOffset = 3;
            //chartTitle.ForeColor = Color.FromArgb(26, 59, 105);

            //var legend = chart.Legends.Add(legendName + xfield);
            //legend.Docking = Docking.Bottom;
            //legend.TitleFont = new Font(new FontFamily("Microsoft Sans Serif"), 8, FontStyle.Bold);
            //legend.BackColor = Color.Transparent;
            //legend.Alignment = StringAlignment.Center;
            //legend.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //legend.IsTextAutoFit = false;
            //legend.LegendStyle = LegendStyle.Table;

            //Series series = null;
            //ChartArea chartArea = null;
            //Legend tempLegend = null;

            //foreach (var yfield in yfields)
            //{
            //    chartArea = chart.ChartAreas.Add("ChartArea_" + yfield.Key);
            //    chartArea.BorderColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.BackSecondaryColor = Color.Transparent;
            //    chartArea.BackColor = Color.Transparent;
            //    chartArea.ShadowColor = Color.Transparent;
            //    chartArea.BorderWidth = 0;
            //    chartArea.Area3DStyle.Rotation = 0;
            //    chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.AxisY.LabelStyle.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //    chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.AxisX.LabelStyle.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //    chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);

            //    series = chart.Series.Add(seriesName + xfield + "_" + yfield.Key);
            //    series.Legend = legend.Name;
            //    series["PieLabelStyle"] = "Inside";
            //    series["PieDrawingStyle"] = "SoftEdge";
            //    series.Label = "#PERCENT{P1}";
            //    series.ChartArea = chartArea.Name;
            //    series.ChartType = SeriesChartType.Pie;
            //    series.BorderColor = Color.FromArgb(180, 26, 59, 105);
            //    series.Color = Color.FromArgb(220, 65, 140, 240);

            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        series.Points.Add(new DataPoint
            //                              {
            //                                  IsVisibleInLegend = chart.Series.Count < 2,
            //                                  LegendText = dr[xfield].ToString(),
            //                                  YValues = new[] { Convert.ToDouble(dr[yfield.Key]) }
            //                              });
            //    }

            //    tempLegend = chart.Legends.Add(legendName + yfield.Key);
            //    tempLegend.BackColor = Color.Transparent;
            //    tempLegend.BorderColor = Color.Transparent;
            //    tempLegend.Font = new Font(new FontFamily("宋体"), 9f, FontStyle.Bold);
            //    tempLegend.DockedToChartArea = chartArea.Name;
            //    tempLegend.Docking = Docking.Left;
            //    tempLegend.CustomItems.Add(Color.Transparent, yfield.Value);
            //}

            //this.Add(chart);
        }

        /// <summary>
        /// 生成MS Chart折线图
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="xfield">X轴要显示的字段</param>
        /// <param name="xdesc">X轴要显示的字段文本</param>
        /// <param name="yfields">Y轴要分析的项目
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.绑定的字段</para>
        /// <para>Value.绑定的字段显示名称</para>
        /// </remarks></param>
        /// <param name="title">图表标题</param>
        /// <param name="chartWidth">单个图表的宽度</param>
        /// <param name="chartHeight">单个图表的高度</param>
        public void GenerateLineChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            StringBuilder lineStrB = new StringBuilder();

            lineStrB.Append(" <chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='正在生成,请稍候...' exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件' decimalPrecision='4'  hoverCapBgColor='AFD8F8' bgColor='2E4A89, 90B1DE' outCnvBaseFontSize='12' anchorBgColor='008ED6' caption='" + title + "'");
            lineStrB.Append("AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' >");
            lineStrB.Append("numberPrefix='' showNames='1' showValues='1'  showAlternateHGridColor='1' ");
            lineStrB.Append(" decimalPrecision='0' formatNumberScale='0' ");
            lineStrB.Append(" yAxisName='hkhkhk' ");  // 底部显示的数据.
            lineStrB.Append(" xAxisName='adadasd' ");     // 左边显示的数据.


            lineStrB.Append(" <categories font='Arial' fontSize='11' fontColor='000000' >");
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                lineStrB.Append("<category  label='" + dr[xfield] + "' />");
            }
            lineStrB.Append("</categories>");
            foreach (var yfield in yfields)
            {
                if (string.IsNullOrEmpty(yfield.Value) == true)
                    continue;
                lineStrB.Append("<dataset seriesname='" + yfield.Value + "'  >");
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    lineStrB.Append(" <set value='" + dr[yfield.Key] + "' />");
                }
                lineStrB.Append(" </dataset>");
            }
            lineStrB.Append(" </chart>");
            string q = lineStrB.ToString();

            this.Add("<script type='text/javascript'>"
             + "  var chart = new FusionCharts(\"/WF/Comm/Charts/swf/MSLine.swf\", \"ChartLine\", '" + chartWidth + "', '" + chartHeight + "', '0', '0');"
             + "  chart.setDataXML(\"" + q + "\");"
             + "  chart.render(\"line_chart_div\");"
             + "</script>"
             );
            //----------------------------------------以下方法不动---------------------------------------------
            //var legendName = "MainLegend_";
            //var seriesName = "Series_";

            //var chart = new Chart();
            //chart.ID = "Chart_" + xfield;
            //chart.Width = new Unit(chartWidth);
            //chart.Height = new Unit(chartHeight);
            //chart.Palette = ChartColorPalette.BrightPastel;
            //chart.BackColor = Color.FromArgb(224, 236, 255);// Color.FromArgb(243, 223, 193);
            //chart.BorderlineDashStyle = ChartDashStyle.Solid;
            //chart.BackGradientStyle = GradientStyle.TopBottom;
            //chart.BorderlineWidth = 1;
            //chart.BorderlineColor = Color.FromArgb(149, 184, 231);//Color.FromArgb(181, 64, 1);
            //chart.BorderSkin = new BorderSkin { SkinStyle = BorderSkinStyle.Emboss };

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - 折线图");
            //chartTitle.Font = new Font(new FontFamily("宋体"), 10.5f, FontStyle.Bold);
            //chartTitle.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            //chartTitle.ShadowOffset = 3;
            //chartTitle.ForeColor = Color.FromArgb(26, 59, 105);
            //var legend = chart.Legends.Add(legendName + xfield);
            //legend.Docking = Docking.Bottom;
            //legend.TitleFont = new Font(new FontFamily("Microsoft Sans Serif"), 8, FontStyle.Bold);
            //legend.BackColor = Color.Transparent;
            //legend.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //legend.IsTextAutoFit = true;

            //Series series = null;
            //ChartArea chartArea = null;
            //var groups = new Dictionary<string, List<DataRow>>();

            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (groups.ContainsKey(dr[xfield].ToString()) == false)
            //    {
            //        groups.Add(dr[xfield].ToString(), new List<DataRow> { dr });
            //    }
            //    else
            //    {
            //        groups[dr[xfield].ToString()].Add(dr);
            //    }
            //}

            //foreach (var yfield in yfields)
            //{
            //    chartArea = chart.ChartAreas.Add("ChartArea_" + yfield.Key);
            //    chartArea.BorderColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.BackSecondaryColor = Color.White;
            //    chartArea.BackColor = Color.OldLace;
            //    chartArea.ShadowColor = Color.Transparent;
            //    chartArea.BackGradientStyle = GradientStyle.TopBottom;
            //    chartArea.Area3DStyle.Rotation = 10;
            //    chartArea.Area3DStyle.Perspective = 10;
            //    chartArea.Area3DStyle.Inclination = 15;
            //    chartArea.Area3DStyle.IsRightAngleAxes = false;
            //    chartArea.Area3DStyle.WallWidth = 0;
            //    chartArea.Area3DStyle.IsClustered = false;
            //    chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            //    chartArea.AxisY.LabelAutoFitMaxFontSize = 8;
            //    chartArea.AxisY.LabelStyle.Font = new Font(new FontFamily("Trebuchet MS"), 8.25f, FontStyle.Bold);
            //    chartArea.AxisY.LabelStyle.IsEndLabelVisible = false;
            //    chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //    //chartArea.AxisX.Title = xdesc;
            //    //chartArea.AxisX.TitleAlignment = StringAlignment.Far;
            //    //chartArea.AxisX.TitleFont = new Font(new FontFamily("宋体"), 9f, FontStyle.Bold);
            //    chartArea.AxisY.Title = yfield.Value;
            //    chartArea.AxisY.TitleAlignment = StringAlignment.Far;
            //    chartArea.AxisY.TitleFont = new Font(new FontFamily("宋体"), 9f, FontStyle.Bold);

            //    foreach (var de in groups)
            //    {
            //        series = chart.Series.Add(seriesName + yfield.Key + "_" + de.Key);
            //        series.Legend = legend.Name;
            //        series.LegendText = de.Key;
            //        series.IsVisibleInLegend = chart.ChartAreas.Count < 2;
            //        series.ChartArea = chartArea.Name;
            //        series.ChartType = SeriesChartType.Line;
            //        series.BorderColor = Color.FromArgb(180, 26, 59, 105);
            //        series.IsValueShownAsLabel = true;
            //        series.MarkerStyle = MarkerStyle.Circle;

            //        foreach (var dr in de.Value)
            //        {
            //            series.Points.AddY(dr[yfield.Key]);
            //        }
            //    }
            //}

            //this.Add(chart);
        }

        public void BindMenu_Small(string enumKey, string url, string selecVal, bool IsShowAll)
        {
            SysEnums ses = new SysEnums(enumKey);
            this.Add("<Table >");
            this.AddTR();
            if (IsShowAll)
            {
                if (selecVal == "all")
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  > <b>全部</A> </TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' >全部</A> </TD>");
            }

            foreach (SysEnum se in ses)
            {
                if (se.IntKey.ToString() == selecVal)
                    this.Add("<TD style='font-size:12px; font-weight:bolder;  '  background=Enum.gif width='126' height='36' align=center ><b>" + se.Lab + "</b></TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, se.IntKey.ToString()) + "' >" + se.Lab + "</A> </TD>");
            }

            this.AddTREnd();
            this.AddTableEnd();
        }
        public void BindMenu(string enumKey, string url, string selecVal, bool IsShowAll, string imgPath, string newStr)
        {
            SysEnums ses = new SysEnums(enumKey);
            this.Add("<Table >");
            this.AddTR();
            if (newStr != null)
            {
                this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  >" + newStr + "</TD>");
            }

            if (IsShowAll)
            {
                if (selecVal == "all")
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><img src='" + imgPath + "all.gif' border=0 />全部</TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' ><img src='" + imgPath + "all.gif' border=0 />全部</A> </TD>");
            }

            foreach (SysEnum se in ses)
            {
                if (se.IntKey.ToString() == selecVal)
                    this.Add("<TD style='font-size:12px; font-weight:bolder;  '  background=Enum.gif width='126' height='36' align=center ><b><img src='" + imgPath + se.IntKey + ".gif' border=0 />" + se.Lab + "</b></TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, se.IntKey.ToString()) + "' ><img src='" + imgPath + se.IntKey + ".gif' border=0 />" + se.Lab + "</A> </TD>");
            }

            this.AddTREnd();
            this.AddTableEnd();
        }

        public void BindMenu(string enumKey, string url, string selecVal, bool IsShowAll)
        {
            SysEnums ses = new SysEnums(enumKey);
            this.Add("<Table >");
            this.AddTR();
            if (IsShowAll)
            {
                if (selecVal == "all")
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  > <b>全部</A> </TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' >全部</A> </TD>");
            }

            foreach (SysEnum se in ses)
            {
                if (se.IntKey.ToString() == selecVal)
                    this.Add("<TD style='font-size:12px; font-weight:bolder;  '  background=Enum.gif width='126' height='36' align=center ><b>" + se.Lab + "</b></TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, se.IntKey.ToString()) + "' >" + se.Lab + "</A> </TD>");
            }

            this.AddTREnd();
            this.AddTableEnd();
        }

        public void BindMenuList(string enumKey, string url, string selecVal)
        {
            SysEnums ses = new SysEnums(enumKey);
            this.Add("<Table >");
            foreach (SysEnum se in ses)
            {
                this.AddTR();
                if (se.IntKey.ToString() == selecVal)
                    this.Add("<TD style='font-size:12px; font-weight:bolder;'  background=Enum.gif width='126' height='36' align=center ><b>" + se.Lab + "</b></TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder;' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, se.IntKey.ToString()) + "' >" + se.Lab + "</A> </TD>");
                this.AddTREnd();
            }
            this.AddTableEnd();
        }

        //		public void BindXmlEns(XmlEns ens)
        public void BindXmlEns(XmlEns ens)
        {
            this.Clear();
            this.AddTable();

            XmlEn myen = ens[0];
            this.Add("<TR>");
            foreach (string key in myen.Row.Keys)
                this.Add("<TD class='Title' >" + key + "</TD>");
            this.AddTREnd();

            foreach (XmlEn en in ens)
            {
                this.Add("<TR onmouseover='TROver(this)' onmouseout='TROut(this)' >");
                foreach (string key in en.Row.Keys)
                    this.AddTD(en.GetValStringByKey(key));
                this.AddTREnd();
            }
            this.Add("</Table>");

        }
        // public void GenerOutlookMenuV2(string cate)
        public void GenerOutlookMenuV2(string cate)
        {
            if (cate == null)
                cate = "01";

            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "Menu.xml");
            DataTable dt = ds.Tables[0];
            DataTable dtl = dt.Clone();
            DataTable dtCate = dt.Clone();

            //DataTable dtl = dt.Clone();
            foreach (DataRow dr in dt.Rows)
            {
                string ForUser = dr["ForUser"].ToString().Trim();
                switch (ForUser)
                {
                    case "SysAdmin":
                        //if (WebUser.HisUserType != UserType.SysAdmin)
                        //    continue;
                        break;
                    case "AppAdmin":
                        //if (WebUser.HisUserType == UserType.AppAdmin
                        //    || WebUser.HisUserType == UserType.SysAdmin)
                        //{
                        //}
                        //else
                        //    continue;
                        break;
                    default:
                        break;
                }
                string no = dr["No"].ToString().Trim();
                if (no.Trim().Length == 2)
                {
                    DataRow dr2 = dtCate.NewRow();
                    dr2["No"] = dr["No"];
                    dr2["Name"] = dr[BP.Web.WebUser.SysLang];
                    dr2["Url"] = dr["Url"];
                    dr2["Desc"] = dr["Desc"];
                    dr2["Img"] = dr["Img"];
                    dtCate.Rows.Add(dr2);
                    continue;
                }

                if (no.Substring(0, 2) == cate)
                {
                    DataRow dr1 = dtl.NewRow();
                    dr1["No"] = dr["No"];
                    dr1["Name"] = dr[BP.Web.WebUser.SysLang];
                    dr1["Url"] = dr["Url"];
                    dr1["Desc"] = dr["Desc"];
                    dr1["Img"] = dr["Img"];
                    dtl.Rows.Add(dr1);
                }
            }


            this.Add("<TABLE   class='MainTable'  >");

            int i = 0;
            foreach (DataRow dr in dtCate.Rows)
            {
                i++;
                string no = dr["No"].ToString();
                string name = dr[BP.Web.WebUser.SysLang].ToString();
                string url = dr["Url"].ToString();
                string img = dr["Img"].ToString();
                string desc = dr["Desc"].ToString(); //描述数据

                if (img.Trim().Length != 5)
                    name = "<img src='" + img + "' border=0 />" + name;

                string srcp = "window.location.href='LeftOutlook.aspx?cate=" + no + "'";
                /*他是目录数据。*/
                if (cate == no)
                {
                    /* 当前要选择他。*/
                    this.Add("<TR  >");
                    this.Add("<TD class='TDM_Selected' nowrap=true title='" + dr["DESC"].ToString() + "' ><b>" + name + "</b></TD>");
                    this.AddTREnd();

                    /*如果遇到了当前要选择的菜单。*/
                    this.Add("<TR height='100%' >");
                    this.Add("<TD calss='TDItemTable'  height='100%'  >");
                    this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                    foreach (DataRow itemdr in dtl.Rows)
                    {
                        string no1 = itemdr["No"].ToString();
                        string name1 = itemdr[BP.Web.WebUser.SysLang].ToString();
                        string url1 = itemdr["Url"].ToString();
                        string img1 = itemdr["Img"].ToString();
                        string desc1 = itemdr["Desc"].ToString(); //描述数据

                        if (img1.Trim().Length != 5)
                            name1 = "<img src='" + img1 + "' border=0 />" + name1;

                        this.Add("<TR  >");
                        this.Add("<TD onclick=\"Javascript:WinOpen('" + url1 + "','mainfrm' )\" onmouseover=\"javascript:ItemOver(this);\" onmouseout=\"javascript:ItemOut(this);\" class='Item' title='" + desc1 + "'  >");
                        this.Add(name1);
                        this.Add("</TD>");
                        this.AddTREnd();
                    }

                    this.Add("</Table>");
                    this.Add("</TD>");
                    this.AddTREnd();
                }
                else
                {
                    this.Add("<TR >");
                    this.Add("<TD class='TDM' nowrap=true title='" + dr["DESC"].ToString() + "' onclick=\"" + srcp + "\" >" + name + "</TD>");
                    this.AddTREnd();
                }
            }

            this.Add("</TABLE>");
        }
        //		public void ClearViewState()
        public void ClearViewState()
        {
            this.ViewState.Clear();
        }
        //		public void GenerOutlookMenuV2()
        public void GenerOutlookMenuV2()
        {
            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "MenuMain.xml");
            DataTable dt = ds.Tables[0];

            this.Add("<TABLE border=-1 class='MainTable'  >");
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string id = "Img" + i.ToString();

                string file = dr["File"].ToString();
                string ImgOut = dr["Out"].ToString();
                string ImgOn = dr["On"].ToString();
                string Name = "&nbsp;" + dr["Name"].ToString();


                string srcp = "window.location.href='LeftOutlook.aspx?xml=" + file + "'";



                this.Add("<TR   >");
                //this.Add("<TD class='TDL'  ><Img src='./ImgOutlook/panel_left_r.gif' border=0 width=1% > </TD>");
                this.Add("<TD class='TDM' nowrap=true title='" + dr["DESC"].ToString() + "' onclick=\"" + srcp + "\" >" + Name + "</TD>");
                //this.Add("<TD class='TDR' > </TD>");
                this.AddTREnd();



                /*如果遇到了当前要选择的菜单。*/
                this.Add("<TR  >");
                //this.Add("<TD ></TD>");
                this.Add("<TD calss='TDItemTable' >");

                this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                ds.Tables.Clear();
                ds.ReadXml(SystemConfig.PathOfXML + file);
                DataTable items = ds.Tables["Item"];
                foreach (DataRow itemdr in items.Rows)
                {
                    string itemUrl = itemdr["URL"].ToString();
                    string itemName = itemdr["Name"].ToString();
                    string ICON = itemdr["ICON"].ToString();
                    string Desc = itemdr["Desc"].ToString();


                    this.Add("<TR  >");
                    //this.Add("<TD  nowrap=true title='"+itemdr["DESC"].ToString()+"'  >");

                    this.Add("<TD onclick=\"Javascript:WinOpen('" + itemUrl + "','mainfrm' )\" onmouseover=\"javascript:ItemOver(this);\" onmouseout=\"javascript:ItemOut(this);\" class='Item' title='" + itemdr["DESC"].ToString() + "'  >");
                    this.Add(itemName);
                    //this.Add("<img src='"+ImgOn+"' id='"+id+"' onclick=\"javascript:"+id+".src='"+ImgOut+"'; TDClick( '"+this.Request.ApplicationPath+"','"+file+"', '"+ ImgOn +"'); \"  onmouseover=\"javascript:"+id+".src='"+ImgOut+"';\"  onmouseout=\"javascript: "+id+".src='"+ImgOn+"'; \" />" );
                    this.Add("</TD>");
                    this.AddTREnd();

                }
                this.Add("</Table>");

                this.Add("</TD>");
                //this.Add("<TD  ></TD>");
                this.AddTREnd();

            }


            this.Add("</TABLE>");

        }

        //		public void  ShowTableGroupEns( DataTable dt, Map map, int top,string url,bool isShowNoCol)
        public void ShowTableGroupEns(DataTable dt, Map map, int top, string url, bool isShowNoCol)
        {
            string str = "";
            str += "<Table style='border-collapse: collapse' bordercolor='#111111' >";
            str += "<TR>";
            str += "  <TD warp=false class='Title' nowrap >";
            str += "ID";
            str += "  </TD>";
            foreach (Attr attr in map.Attrs)
            {
                if (attr.Field == null && (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum))
                    continue;

                if (attr.MyFieldType == FieldType.RefText || attr.MyFieldType == FieldType.Normal)
                {
                    str += "  <TD warp=false class='Title' nowrap >";
                    str += attr.Desc;
                    str += "  </TD>";
                }
                else
                {
                    if (isShowNoCol)
                    {
                        str += "  <TD warp=false class='Title' nowrap >";
                        str += attr.Desc;
                        str += "  </TD>";
                    }
                }

            }
            str += "</TR>";

            int idx = 0;
            string myurl = "";
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                str += "<TR class='TR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
                str += "  <TD class='Idx' nowrap >";
                str += idx.ToString();
                str += "  </TD>";
                myurl = "";
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.Field == null && (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum))
                        continue;

                    if (attr.MyFieldType == FieldType.Normal)
                    {
                        str += "  <TD class='TDNum' nowrap >";
                        str += "<a href=\"javascript:WinOpen('" + url + myurl + "')\"  >" + dr[attr.Field] + "</a>";
                        str += "  </TD>";
                    }
                    else
                    {
                        if (attr.MyFieldType == FieldType.RefText)
                        {
                            str += "  <TD class='TD' nowrap >";
                            str += dr[attr.Key];
                            str += "  </TD>";
                        }
                        else
                        {
                            myurl += "&" + attr.Key + "=" + dr[attr.Field];
                            if (isShowNoCol)
                            {
                                str += "  <TD class='TD' nowrap >";
                                str += dr[attr.Field];
                                str += "  </TD>";
                            }
                        }
                    }
                }
                str += "</TR>";

                if (idx == top)
                    break;
            }

            str += "</Table>";
            this.Add(str);

        }


        //		public void  ShowTable( DataTable dt, Map map)
        public void ShowTable(DataTable dt, Map map)
        {
            string str = "";
            str += "<Table class='Table'  >";
            str += "<TR>";
            str += "  <TD warp=false class='Title' nowrap >";
            str += "ID";
            str += "  </TD>";
            foreach (Attr attr in map.Attrs)
            {
                if (attr.Field == null)
                    continue;

                str += "  <TD warp=false class='Title' nowrap >";
                str += attr.Desc;
                str += "  </TD>";
            }
            str += "</TR>";

            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;

                str += "<TR class='TR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
                str += "  <TD class='TDLeft' nowrap >";
                str += idx.ToString();
                str += "  </TD>";
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.UIContralType == UIContralType.DDL)
                        continue;

                    str += "  <TD class='TD' nowrap >";
                    if (attr.MyFieldType == FieldType.RefText)
                        str += dr[attr.Key];
                    else
                        str += dr[attr.Field];

                    str += "  </TD>";
                }
                str += "</TR>";
            }

            str += "</Table>";
            this.Add(str);

        }
        public void ShowHidenMsg(string id, string title, string msg, bool isShowHelpIcon)
        {

            string appPath = this.Request.ApplicationPath;
            if (isShowHelpIcon)
                title = "<img src='../Img/btn/help.gif' border=0 />" + title;


            msg = "<table class=Table id='t" + id + "' border=0 ><TR Class=TR ><TD class=TD  bgcolor=#FFFFFF >" + msg + "</TD></TR></Table>";

            string str = "<A onclick='show" + id + "();' style='cursor:hand' > <FONT color='#008000' style='font-size:12px'  ><b>" + title + "</b><img src='../Img/downUp.gif' id=Img" + id + "' ></FONT></A><span id='" + id + "'></span>";

            string script = "\n <script language='javascript'> var mode; mode=1; ";
            script += "\n function show" + id + "() {";

            script += "\n  if (mode==0) ";
            script += "\n  {  \n";
            script += id + ".innerHTML='' \n";
            //script += "Img"+id + ".Src='/imgages/Up.gif' \n";

            script += "   mode=1 \n";

            script += "  }else{ \n";

            script += id + ".innerHTML=' " + msg + "'\n";
            // script += "Img" + id + ".Src='/imgages/Down.gif' \n";

            script += "   mode=0 \n";
            script += "  }\n";
            script += "}\n";
            script += "</script>\n";

            this.Add(str);
            this.Add(script);

        }

        public void ShowTable(string title, DataTable dt, DataTable sDT, string color, string refF)
        {

            this.AddTable();
            if (title != null)
                this.AddCaptionLeft(title);

            this.AddTR();
            this.AddTDTitle("序");
            foreach (DataColumn dc in dt.Columns)
                this.AddTDTitle(dc.ColumnName);
            this.AddTREnd();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string bg = "";
                foreach (DataRow mydr in sDT.Rows)
                {
                    if (mydr[refF].ToString() == dr[refF].ToString())
                    {
                        bg = "bgcolor=" + color;
                        break;
                    }
                }

                this.AddTR(bg);

                this.AddTDIdx(i);
                foreach (DataColumn dc in dt.Columns)
                {
                    this.AddTD(dr[dc.ColumnName].ToString());
                }
                this.AddTREnd();
            }
            this.AddTableEnd();
        }

        //		public void  ShowTable( DataTable dt)
        public void ShowTable(string title, DataTable dt, bool is_TR_TX)
        {

            this.AddTable();
            if (title != null)
                this.AddCaptionLeft(title);

            this.AddTR();
            this.AddTDTitle("序");
            foreach (DataColumn dc in dt.Columns)
                this.AddTDTitle(dc.ColumnName);
            this.AddTREnd();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                if (is_TR_TX)
                    this.AddTRTX();
                else
                    this.AddTR();

                this.AddTDIdx(i);
                foreach (DataColumn dc in dt.Columns)
                {
                    this.AddTD(dr[dc.ColumnName].ToString());
                }
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
        //		public void GenerOutlookMenu(string xmlFile)
        public void GenerOutlookMenu(string xmlFile)
        {
            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "MenuMain.xml");
            DataTable dt = ds.Tables[0];


            if (xmlFile == null || xmlFile == "ss")  //如果没有找到它，就设置第一个。
                this.Add("<TABLE border=-1 class='MainTable'  >");
            else
                this.Add("<TABLE border=-1 class='MainTable'  height=100%  >");


            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string id = "Img" + i.ToString();

                string file = dr["File"].ToString();
                string ImgOut = dr["Out"].ToString();
                string ImgOn = dr["On"].ToString();
                string Name = "&nbsp;" + dr["Name"].ToString();

                string srcp = "window.location.href='LeftOutlook.aspx?xml=" + file + "'";
                if (file == xmlFile)
                {
                    this.Add("<TR   >");
                    this.Add("<TD class='TDM_Selected' nowrap=true title='" + dr["DESC"].ToString() + "' ><b>" + Name + "</b></TD>");
                    this.AddTREnd();
                }
                else
                {
                    this.Add("<TR   >");
                    this.Add("<TD class='TDM' nowrap=true title='" + dr["DESC"].ToString() + "' onclick=\"" + srcp + "\" >" + Name + "</TD>");
                    this.AddTREnd();
                }


                if (xmlFile == "RptTemplate" && file == "RptTemplate")
                {
                    /*如果遇到了当前要选择的菜单。*/
                    this.Add("<TR  >");
                    this.Add("<TD calss='TDItemTable' >");
                    this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                    this.Add("</Table>");
                    this.Add("</TD>");
                    this.AddTREnd();
                }
                else if (file == xmlFile)
                {
                    /*如果遇到了当前要选择的菜单。*/
                    this.Add("<TR  >");
                    this.Add("<TD calss='TDItemTable' >");
                    this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                    ds.Tables.Clear();
                    ds.ReadXml(SystemConfig.PathOfXML + file);
                    DataTable items = ds.Tables["Item"];
                    foreach (DataRow itemdr in items.Rows)
                    {
                        string itemUrl = itemdr["URL"].ToString();
                        string itemName = itemdr["Name"].ToString();
                        string ICON = itemdr["ICON"].ToString();
                        string Desc = itemdr["Desc"].ToString();

                        this.Add("<TR  >");
                        //this.Add("<TD  nowrap=true title='"+itemdr["DESC"].ToString()+"'  >");

                        this.Add("<TD onclick=\"Javascript:WinOpen('" + itemUrl + "','mainfrm' )\" onmouseover=\"javascript:ItemOver(this);\" onmouseout=\"javascript:ItemOut(this);\" class='Item' title='" + itemdr["DESC"].ToString() + "'  >");
                        this.Add(itemName);
                        //this.Add("<img src='"+ImgOn+"' id='"+id+"' onclick=\"javascript:"+id+".src='"+ImgOut+"'; TDClick( '"+this.Request.ApplicationPath+"','"+file+"', '"+ ImgOn +"'); \"  onmouseover=\"javascript:"+id+".src='"+ImgOut+"';\"  onmouseout=\"javascript: "+id+".src='"+ImgOn+"'; \" />" );
                        this.Add("</TD>");
                        this.AddTREnd();
                    }


                    this.Add("</Table>");
                    this.Add("</TD>");
                    //this.Add("<TD  ></TD>");
                    this.AddTREnd();
                }
            }


            this.Add("</TABLE>");

        }
        //		public void GenerOutlookMenu()
        public void GenerOutlookMenu()
        {
            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "MenuMain.xml");
            DataTable dt = ds.Tables[0];

            this.Add("<TABLE border=-1 class='MainTable'  >");
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string id = "Img" + i.ToString();

                string file = dr["File"].ToString();
                string ImgOut = dr["Out"].ToString();
                string ImgOn = dr["On"].ToString();
                string Name = "&nbsp;" + dr["Name"].ToString();


                string srcp = "window.location.href='LeftOutlook.aspx?xml=" + file + "'";



                this.Add("<TR   >");
                //this.Add("<TD class='TDL'  ><Img src='./ImgOutlook/panel_left_r.gif' border=0 width=1% > </TD>");
                this.Add("<TD class='TDM' nowrap=true title='" + dr["DESC"].ToString() + "' onclick=\"" + srcp + "\" >" + Name + "</TD>");
                //this.Add("<TD class='TDR' > </TD>");
                this.AddTREnd();



                /*如果遇到了当前要选择的菜单。*/
                this.Add("<TR  >");
                //this.Add("<TD ></TD>");
                this.Add("<TD calss='TDItemTable' >");

                this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                ds.Tables.Clear();
                ds.ReadXml(SystemConfig.PathOfXML + file);
                DataTable items = ds.Tables["Item"];
                foreach (DataRow itemdr in items.Rows)
                {
                    string itemUrl = itemdr["URL"].ToString();
                    string itemName = itemdr["Name"].ToString();
                    string ICON = itemdr["ICON"].ToString();
                    string Desc = itemdr["Desc"].ToString();


                    this.Add("<TR  >");
                    //this.Add("<TD  nowrap=true title='"+itemdr["DESC"].ToString()+"'  >");

                    this.Add("<TD onclick=\"Javascript:WinOpen('" + itemUrl + "','mainfrm' )\" onmouseover=\"javascript:ItemOver(this);\" onmouseout=\"javascript:ItemOut(this);\" class='Item' title='" + itemdr["DESC"].ToString() + "'  >");
                    this.Add(itemName);
                    //this.Add("<img src='"+ImgOn+"' id='"+id+"' onclick=\"javascript:"+id+".src='"+ImgOut+"'; TDClick( '"+this.Request.ApplicationPath+"','"+file+"', '"+ ImgOn +"'); \"  onmouseover=\"javascript:"+id+".src='"+ImgOut+"';\"  onmouseout=\"javascript: "+id+".src='"+ImgOn+"'; \" />" );
                    this.Add("</TD>");
                    this.AddTREnd();

                }
                this.Add("</Table>");

                this.Add("</TD>");
                //this.Add("<TD  ></TD>");
                this.AddTREnd();

            }


            this.Add("</TABLE>");

        }
        //		public void GenerOutlookMenu_Img(string xmlFile)
        public void GenerOutlookMenu_Img(string xmlFile)
        {
            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "MenuMain.xml");
            DataTable dt = ds.Tables[0];
            if (xmlFile == null || xmlFile == "")  //如果没有找到它，就设置第一个。
                xmlFile = dt.Rows[0]["File"].ToString();



            this.Add("<TABLE border=0 class='MainTable' >");

            //e.Item.Attributes.Add("onmouseover","DGTROn"+WebUser.Style+"(this)");
            //e.Item.Attributes.Add("onmouseout","DGTROut"+WebUser.Style+"(this)");

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string id = "Img" + i.ToString();

                string file = dr["File"].ToString();
                string ImgOut = dr["Out"].ToString();
                string ImgOn = dr["On"].ToString();

                // window.location.href='MyDay.aspx?RefDate='+date;

                if (file == xmlFile)
                {
                    this.Add("<TR>");
                    this.Add("<TD   nowrap=true title='" + dr["DESC"].ToString() + "'  >");
                    this.Add("<img src='" + ImgOn + "' id='" + id + "' />");
                    this.Add("</TD>");
                    this.AddTREnd();
                }
                else
                {
                    string srcp = "window.location.href='LeftOutlook.aspx?xml=" + file + "'";

                    this.Add("<TR>");
                    this.Add("<TD   nowrap=true title='" + dr["DESC"].ToString() + "'  >");
                    this.Add("<img src='" + ImgOn + "' id='" + id + "' onclick=\"javascript:" + id + ".src='" + ImgOut + "'; " + srcp + " ; ; \"  onmouseover=\"javascript:" + id + ".src='" + ImgOut + "';\"  onmouseout=\"javascript: " + id + ".src='" + ImgOn + "'; \" />");
                    this.Add("</TD>");
                    this.AddTREnd();
                }

                if (file == xmlFile)
                {
                    /*如果遇到了当前要选择的菜单。*/
                    this.Add("<TR>");
                    this.Add("<TD>");

                    this.Add("<Table border=0  class='ItemTable' >");
                    ds.Tables.Clear();
                    ds.ReadXml(SystemConfig.PathOfXML + file);
                    DataTable items = ds.Tables["Item"];
                    foreach (DataRow itemdr in items.Rows)
                    {
                        string itemUrl = itemdr["URL"].ToString();
                        string itemName = itemdr["Name"].ToString();
                        string ICON = itemdr["ICON"].ToString();
                        string Desc = itemdr["Desc"].ToString();

                        this.Add("<TR>");
                        this.Add("<TD   nowrap=true title='" + itemdr["DESC"].ToString() + "'  >");
                        this.Add("<a href='" + itemUrl + "' target='mainfrm' class='Link' >" + itemName + "</a>");
                        //this.Add("<img src='"+ImgOn+"' id='"+id+"' onclick=\"javascript:"+id+".src='"+ImgOut+"'; TDClick( '"+this.Request.ApplicationPath+"','"+file+"', '"+ ImgOn +"'); \"  onmouseover=\"javascript:"+id+".src='"+ImgOut+"';\"  onmouseout=\"javascript: "+id+".src='"+ImgOn+"'; \" />" );
                        this.Add("</TD>");
                        this.AddTREnd();
                    }
                    this.Add("</Table>");

                    this.Add("</TD>");
                    this.AddTREnd();
                }
            }
            this.Add("</TABLE>");
        }
        //		public void BindSystems()
        public void BindSystems()
        {
            this.AddTable();
            this.Add("<TR>");
            this.AddTDTitle("系统编号");
            this.AddTDTitle("名称");
            this.AddTDTitle("版本");
            this.AddTDTitle("发布日期");
            this.AddTREnd();
            //BPSystems ens = new BPSystems();
            //ens.RetrieveAll();
            //foreach (BPSystem en in ens)
            //{
            //    this.Add("<TR  onmouseover='TROver(this)' onmouseout='TROut(this)' >");
            //    this.AddTD(en.No);
            //    if (en.IsOk && SystemConfig.SysNo != en.No)
            //        this.AddTD("<a href='" + en.URL + "&Token=" + WebUser.Token + "&No=" + WebUser.No + "' target='_parent' >" + en.Name + "</a> ");
            //    else
            //        this.AddTD(en.Name);
            //    this.AddTD(en.Ver);
            //    this.AddTD(en.IssueDate);
            //    this.AddTREnd();
            //}
            this.Add("</Table>\n");
        }
        //		public void BindWel()
        public void BindWel()
        {
            this.Controls.Clear();
            //this.Add("<font color='#000000' size=2 >欢迎您："+WebUser.Name+"，部门："+WebUser.HisEmp.FK_DeptText+"，岗位："+WebUser.HisEmp.FK_StationText+"。</font>");
        }
        //		public void BindMsgInfo(string msg)
        public void BindMsgInfo(string msg)
        {
            this.Controls.Clear();
            this.Add("<Table  border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
            this.Add("<Caption align=left ><b>提示信息</b></Caption>");
            this.Add("<TR>");
            this.Add("<TD  bgcolor='#FFFF00' >" + msg + "</TD>");
            this.AddTREnd();
            this.Add("</Table>");
        }
        //		public void BindMsgWarning(string msg)
        public void BindMsgWarning(string msg)
        {
            this.Controls.Clear();
            this.Add("<font color='#000000' size=40 >" + msg + "</font>");
        }
        //		public void GenerMenuMain()
        public void GenerMenuMain()
        {
            this.Controls.Clear();
            DataSet ds = new DataSet();
            ds.ReadXml(SystemConfig.PathOfXML + "MenuMain.xml");
            DataTable dt = ds.Tables[0];

            this.Add("<TABLE border=0>");
            this.Add("<TR>");
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                string id = "Img" + i.ToString();

                string file = dr["File"].ToString();
                string ImgOut = dr["Out"].ToString();
                string ImgOn = dr["On"].ToString();


                this.Add("<TD   nowrap=true title='" + dr["DESC"].ToString() + "'  >");

                this.Add("<img src='" + ImgOn + "' id='" + id + "' onclick=\"javascript:" + id + ".src='" + ImgOut + "'; TDClick( '" + this.Request.ApplicationPath + "','" + file + "', '" + ImgOn + "'); \"  onmouseover=\"javascript:" + id + ".src='" + ImgOut + "';\"  onmouseout=\"javascript: " + id + ".src='" + ImgOn + "'; \" />");

                this.Add("</TD>");
            }
            this.AddTREnd();


            this.Add("</TABLE>");


        }
        //		public void DataPanel(Entities ens, string ctrlId, string key, ShowWay sh)
        public void DataPanel(Entities ens, string ctrlId, string key, ShowWay sh)
        {
            switch (sh)
            {
                case ShowWay.Cards:
                    this.DataPanelCards(ens, ctrlId, key, true);
                    break;
                case ShowWay.List:
                    this.DataPanelCards(ens, ctrlId, key, false);
                    break;
                case ShowWay.Dtl:
                    this.DataPanelDtl(ens, ctrlId, key);
                    break;
            }

        }
        //		public void DataPanelDtl(Entities ens, string ctrlId , string colName, string urlAttrKey, string colUrl  )
        public void DataPanelDtl(Entities ens, string ctrlId, string colName, string urlAttrKey, string colUrl)
        {
            this.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            Attrs selectedAttrs = myen.EnMap.GetChoseAttrs(ens);

            string appPath = this.Request.ApplicationPath;
            // 生成标题
            this.Add("<TABLE  style='border-collapse: collapse' bordercolor='#111111' >");
            this.Add("<TR >");
            this.Add("<TH  nowrap >序</TH>");
            this.Add("<TH nowrap >" + colName + "</TH>");

            foreach (Attr attrT in selectedAttrs)
            {
                if (attrT.UIVisible == false)
                    continue;

                this.Add("<TD  nowrap >" + attrT.Desc + "</TD>");
            }
            this.AddTREnd();

            int idx = 0;
            string style = WebUser.Style;
            foreach (Entity en in ens)
            {
                #region 处理keys
                string url = "";
                foreach (Attr attr in attrs)
                {
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.IsPK)
                                url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                            break;
                        case UIContralType.DDL:
                            url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                            break;
                    }
                }
                #endregion

                this.Add("<TR  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');\" onmouseout='TROut(this)' ondblclick=\" WinOpen('../Comm/En.htm?EnName=" + en.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\" >");
                idx++;
                this.Add("<TD  class='Idx' nowrap >" + idx + "</TD>");
                this.Add("<TD  class='No'  nowrap ><a href='" + colUrl + en.GetValStringByKey(urlAttrKey) + "' target='_blank'> " + colName + "</a></TD>");

                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.UIContralType == UIContralType.DDL)
                        this.Add("<TD  nowrap >" + en.GetValRefTextByKey(attr.Key) + "&nbsp;</TD>");
                    else
                    {
                        string str = en.GetValStringByKey(attr.Key);
                        switch (attr.MyDataType)
                        {
                            case DataType.AppBoolean:
                                if (str == "1")
                                    this.AddTD("是&nbsp;");
                                else
                                    this.AddTD("否&nbsp;");
                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:
                                this.AddTD(str);
                                break;
                            case DataType.AppString:
                                if (attr.UIHeight != 0)
                                    this.AddTDDoc(str, str);
                                else
                                    this.AddTD(str);
                                break;
                            case DataType.AppDouble:
                            case DataType.AppFloat:
                            case DataType.AppMoney:
                                this.AddTDNum(str);
                                break;
                            default:
                                throw new Exception("sdfasdfsd");
                        }
                    }
                }
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }
        public void DataPanelDtlCheckBox(Entities ens)
        {
            this.Controls.Clear();

            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            Attrs selectedAttrs = myen.EnMap.GetChoseAttrs(ens);

            BP.Sys.XML.Searchs cfgs = new BP.Sys.XML.Searchs();
            cfgs.RetrieveBy(BP.Sys.XML.SearchAttr.For, ens.ToString());

            // 生成标题
            this.Add("<table  style=\"width:30%\" >");
            this.AddTR();

            // CheckBox cb = new CheckBox();
            // cb.Text = "序";
            // cb.ID = "CB_Idx";
            //  cb.Attributes["CheckedChanged"] = "javascript:CheckIt(this)";
            // cb.Attributes["CheckedChanged"] = "javascrip:CheckIt(this)";
            //cb.CheckedChanged ["CheckedChanged"] = "javascrip:CheckIt(this)";

            if (ens.Count > 0)
            {
                string str1 = "<INPUT id='checkedAll' onclick='selectAll()' type='checkbox' name='checkedAll'>";
                this.AddTDTitle(str1);
            }
            else
            {
                this.AddTDTitle();
            }

            foreach (Attr attrT in selectedAttrs)
            {
                if (attrT.UIVisible == false)
                    continue;

                if (attrT.Key == "MyNum")
                    continue;

                if (attrT.IsNum && attrT.IsEnum == false && attrT.MyDataType == DataType.AppBoolean == false)
                    this.AddTDTitle("<a href=\"javascript:WinOpen('Group.aspx?EnsName=" + ens.ToString() + "&NumKey=" + attrT.Key + "','sd','800','700');\" >" + attrT.Desc + "</a>");
                else
                    this.AddTDTitle(attrT.Desc);
            }
            this.AddTDTitle();
            this.AddTREnd();

            bool isRefFunc = true;
            int pageidx = this.PageIdx - 1;
            int idx = SystemConfig.PageSize * pageidx;

            BP.Sys.UIConfig cfg = new UIConfig(myen);

            bool is1 = false;
            string urlExt = "";
            foreach (Entity en in ens)
            {
                idx++;

                #region 处理keys
                string style = WebUser.Style;
                string url = this.GenerEnUrl(en, attrs);
                #endregion


                urlExt = "\"javascript:ShowEn('./RefFunc/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "', 'cd','" + cfg.WinCardH + "','" + cfg.WinCardW + "');\"";


                switch ((UIRowStyleGlo)cfg.UIRowStyleGlo)
                {
                    case UIRowStyleGlo.None:
                        if (cfg.IsEnableDouclickGlo)
                            this.AddTR("ondblclick=" + urlExt);
                        else
                            this.AddTR();
                        break;
                    case UIRowStyleGlo.Mouse:
                        if (cfg.IsEnableDouclickGlo)
                            this.AddTRTX("ondblclick=" + urlExt);
                        else
                            this.AddTRTX();
                        break;
                    case UIRowStyleGlo.Alternately:
                    case UIRowStyleGlo.MouseAndAlternately:
                        if (cfg.IsEnableDouclickGlo)
                            is1 = this.AddTR(is1, "ondblclick=" + urlExt);
                        else
                            is1 = this.AddTR(is1);
                        break;
                    default:
                        throw new Exception("@目前还没有提供。");
                }


                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + en.PKVal;
                cb.Text = idx.ToString();
                //cb.Attributes["PK"] = en.PKVal;
                this.AddTDIdx(cb);
                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "MyNum")
                        continue;

                    this.DataPanelDtlAdd(en, attr, cfgs, url, urlExt, cfg.FocusField);
                }

                this.AddTD();


                //if (isRefFunc && cfg.IsEnableRefFunc)
                //{
                //    string str = "";

                //    #region 加入他门的 方法
                //    RefMethods myreffuncs = en.EnMap.HisRefMethods;
                //    foreach (RefMethod func in myreffuncs)
                //    {
                //        if (func.Visable == false || func.IsForEns == false)
                //            continue;

                //        //myurl="../Comm/RefMethod.htm?Index="+func.Index+"&EnsName="+ens.ToString() ;
                //        str += "<A onclick=\"javascript:RefMethod1('" + this.Request.ApplicationPath + "', '" + func.Index + "', '" + func.Warning + "', '" + func.Target + "', '" + ens.ToString() + "','" + url + "') \"  > " + func.GetIcon(this.Request.ApplicationPath) + " " + func.Title + "</A>";
                //        // str += "<A onclick=\"javascript:RefMethod1('" + this.Request.ApplicationPath + "', '" + func.Index + "', '" + func.Warning + "', '" + func.Target + "', '" + ens.ToString() + "','" + url + "') \"  > " + func.GetIcon(this.Request.ApplicationPath) + "<font color=blue >" + func.Title + "</font></A>";
                //        //this.AddItem(func.Title, "RefMethod('"+func.Index+"', '"+func.Warning+"', '"+func.Target+"', '"+this.EnsName+"')", func.Icon);
                //    }
                //    #endregion

                //    #region 加入他的明细
                //    EnDtls enDtls = en.EnMap.Dtls;
                //    foreach (EnDtl enDtl in enDtls)
                //    {
                //        str += "[<A onclick=\"javascript:EditDtl1('" + this.Request.ApplicationPath + "', '" + myen.ToString() + "',  '" + enDtl.EnsName + "', '" + enDtl.RefKey + "', '" + url + "&IsShowSum=1')\" >" + enDtl.Desc + "</A>]";
                //    }
                //    #endregion

                //    #region 加入一对多的实体编辑
                //    //AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
                //    //foreach (AttrOfOneVSM vsM in oneVsM)
                //    //{
                //    //    str += "[<A onclick=\"javascript:EditOneVsM1('" + this.Request.ApplicationPath + "','" + en.ToString() + "','" + vsM.EnsOfMM.ToString() + "s','" + vsM.EnsOfMM + "&dt=" + DateTime.Now.ToString("hhss") + "','" + myen.ToString() + "','" + url + "'); return; \" >" + vsM.Desc + "</A>]";
                //    //}
                //    #endregion

                //    if (cfg.IsEnableOpenICON)
                //        this.Add("<TD class='TD' style='cursor:hand;' nowrap=true  >" + str + " </TD>");
                //    else
                //        this.Add("<TD class='TD' style='cursor:hand;' nowrap=true  >" + str + " </TD>");
                //    // this.Add("<TD class='TD' style='cursor:hand;' nowrap=true  >" + str + " <a href=\"" + urlExt + "\" ><img src='../Img/Btn/Open.gif' border=0/></a></TD>");

                //}
                //else
                //{
                //    if (cfg.IsEnableOpenICON)
                //        this.AddTD();
                //    // this.Add("<TD class='TD' style='cursor:hand;' nowrap=true><a href=\"" + urlExt + "\" ><img src='../Img/Btn/Open.gif' border=0/></a></TD>");
                //    else
                //        this.AddTD();
                //}

                this.AddTREnd();
            }
            this.AddTableEnd();
        }
        //		public void DataPanelDtl(Entities ens, string ctrlId )
        public void DataPanelDtl(Entities ens, string ctrlId)
        {
            this.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();

            //   Map map = myen.EnMapInTime;
            Attrs attrs = myen.EnMap.Attrs;
            IEnumerable<Attr> selectedAttrs = null;
            BP.Sys.UIConfig cfg = new UIConfig(myen);

            if (cfg.ShowColumns.Length == 0)
                selectedAttrs = attrs.Cast<Attr>();
            else
                selectedAttrs = attrs.Cast<Attr>().Where(a => cfg.ShowColumns.Contains(a.Key));

            BP.Sys.XML.Searchs cfgs = new BP.Sys.XML.Searchs();
            cfgs.RetrieveBy(BP.Sys.XML.SearchAttr.For, ens.ToString());

            // 生成标题
            //this.Add("<Table border='0' width='100%' align=left cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#C0C0C0'>");
            this.Add("<Table class='Table' border='0' width='100%' cellpadding='0' cellspacing='0' style=\"border-style: none; width:100%\">");
            // this.AddTable("");
            this.AddTR();
            this.AddTDGroupTitle("style='text-align:center; width:40px;'", "序");
            foreach (Attr attrT in selectedAttrs)
            {
                if (attrT.UIVisible == false)
                    continue;
                if (attrT.Key == "MyNum")
                    continue;
                this.AddTDGroupTitle(attrT.Desc);
            }

            bool isRefFunc = false;

            isRefFunc = true;

            int pageidx = this.PageIdx - 1;
            //重新查询时，置为第1页
            if (IsPostBack)
                pageidx = 0;

            int idx = SystemConfig.PageSize * pageidx;
            bool is1 = false;

            #region 用户界面属性设置

            bool isAddTitle = false;  //是否显示相关功能列。
            if (cfg.IsEnableOpenICON)
                isAddTitle = true;
            if (cfg.IsEnableRefFunc)
                isAddTitle = true;
            #endregion 用户界面属性设置

            if (isAddTitle)
                this.AddTDGroupTitle();

            this.AddTREnd();

            string urlExt = "";
            foreach (Entity en in ens)
            {
                #region 处理keys
                string style = WebUser.Style;
                string url = this.GenerEnUrl(en, attrs);
                #endregion

                urlExt = "\"javascript:ShowEn('../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "&inlayer=1', 'cd','" + cfg.WinCardH + "','" + cfg.WinCardW + "');\"";
                switch ((UIRowStyleGlo)cfg.UIRowStyleGlo)
                {
                    case UIRowStyleGlo.None:
                        if (cfg.IsEnableDouclickGlo)
                            this.AddTR("ondblclick=" + urlExt);
                        else
                            this.AddTR();
                        break;
                    case UIRowStyleGlo.Mouse:
                        if (cfg.IsEnableDouclickGlo)
                            this.AddTRTX("ondblclick=" + urlExt);
                        else
                            this.AddTRTX();
                        break;
                    case UIRowStyleGlo.Alternately:
                    case UIRowStyleGlo.MouseAndAlternately:
                        if (cfg.IsEnableDouclickGlo)
                            is1 = this.AddTR(is1, "ondblclick=" + urlExt);
                        else
                            is1 = this.AddTR(is1);
                        break;
                    default:
                        throw new Exception("@目前还没有提供。");
                }

                idx++;
                this.AddTDIdx(idx);
                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "MyNum")
                        continue;

                    this.DataPanelDtlAdd(en, attr, cfgs, url, urlExt, cfg.IsEnableFocusField ? cfg.FocusField : string.Empty);
                }

                if (cfg.IsEnableRefFunc && isRefFunc)
                {
                    string str = "";

                    #region 加入他门的方法
                    RefMethods myreffuncs = en.EnMap.HisRefMethods;
                    foreach (RefMethod func in myreffuncs)
                    {
                        if (func.Visable == false || func.IsForEns == false || func.IsCanBatch == true || func.RefAttrKey != null)
                            continue;
                        
                        str += "<A style='cursor:hand;' nowrap=true onclick=\"javascript:RefMethod1('" + this.Request.ApplicationPath + "', '" + func.Index + "', '" + func.Warning + "', '" + func.Target + "', '" + ens.ToString() + "','" + url + "') \"  > " + func.GetIcon(this.Request.ApplicationPath) + "" + func.Title + "</A>";
                    }
                    #endregion

                    #region 加入他的明细
                    EnDtls enDtls = en.EnMap.Dtls;
                    foreach (EnDtl enDtl in enDtls)
                    {
                        str += "[<A onclick=\"javascript:EditDtl1('" + this.Request.ApplicationPath + "', '" + myen.ToString() + "',  '" + en.ToString() + "',  '" + enDtl.EnsName + "', '" + enDtl.RefKey + "', '" + url + "&IsShowSum=1')\" >" + enDtl.Desc + "</A>]";
                    }
                    #endregion

                    #region 加入一对多的实体编辑
                    AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
                    foreach (AttrOfOneVSM vsM in oneVsM)
                    {
                        //  str += "[<A onclick=\"javascript:EditOneVsM1('" + this.Request.ApplicationPath + "','" + en.ToString() + "','" + vsM.EnsOfMM.ToString() + "s','" + vsM.EnsOfMM + "&dt=" + DateTime.Now.ToString("hhss") + "','" + myen.ToString() + "','" + url + "'); return; \" >" + vsM.Desc + "</A>]";
                    }
                    #endregion

                    //if (isShowOpenICON)
                    //    this.Add("<TD class='TD' style='cursor:hand;' nowrap=true  >" + str + " <a href=" + urlExt + " ><img src='../Img/Btn/Open.gif' border=0/></a></TD>");
                    //else
                    this.Add("<TD class='TD' style='cursor:hand;' nowrap=true  >" + str + "</TD>");

                }
                else
                {
                    if (cfg.IsEnableOpenICON)
                        this.Add("<TD class='TD' style='cursor:hand;' nowrap=true><a href=" + urlExt + " ><img src='../Img/Btn/Open.gif' border=0/></a></TD>");
                }
                this.AddTREnd();
            }

            #region  求合计代码写在这里。
            string NoShowSum = SystemConfig.GetConfigXmlEns("NoShowSum", ens.ToString());
            if (NoShowSum == null)
                NoShowSum = "";

            bool IsHJ = false;
            foreach (Attr attr in selectedAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIContralType == UIContralType.DDL)
                    continue;

                if (NoShowSum.IndexOf("@" + attr.Key + "@") != -1)
                    continue;

                if (attr.Key == "OID" || attr.Key == "MID" || attr.Key.ToUpper() == "WORKID")
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppMoney:
                        IsHJ = true;
                        break;
                    default:
                        break;
                }
            }

            IsHJ = false;
            //if (ens.Count > 1 )
            //    IsHJ = true;
            //foreach (Attr attr in attrs)
            //{
            //    if (attr.IsNum  )
            //    {
            //        IsHJ = true;
            //    }
            //}



            if (IsHJ)
            {
                // 找出配置是不显示合计的列。

                if (NoShowSum == null)
                    NoShowSum = "";

                this.Add("<TR class='TRSum' >");
                this.AddTD("合计");
                foreach (Attr attr in selectedAttrs)
                {

                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "MyNum")
                        continue;


                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        this.AddTD();
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        this.AddTD();
                        continue;
                    }
                    if (attr.Key == "OID" || attr.Key == "MID" || attr.Key.ToUpper() == "WORKID")
                    {
                        this.AddTD();
                        continue;
                    }


                    if (NoShowSum.IndexOf("@" + attr.Key + "@") != -1)
                    {
                        /*不需要显示它他们的合计。*/
                        this.AddTD();
                        continue;
                    }



                    switch (attr.MyDataType)
                    {
                        case DataType.AppDouble:
                            this.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                            this.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            this.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.AddTDJE(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        default:
                            this.AddTD();
                            break;
                    }
                }
                this.AddTREnd();
            }
            #endregion
            this.AddTableEnd();
        }
        /// <summary>
        /// DataPanelDtl
        /// </summary>
        /// <param name="ens">要bind ens</param>
        /// <param name="ctrlId">webmenu id </param>
        /// <param name="groupkey">groupkey</param>
        //		public void DataPanelDtl(Entities ens, string ctrlId, string groupkey)
        public void DataPanelDtl(Entities ens, string ctrlId, string groupkey)
        {
            if (groupkey == "None")
            {
                this.DataPanelDtl(ens, ctrlId);
                return;
            }

            BP.Sys.XML.Searchs cfgs = new BP.Sys.XML.Searchs();
            cfgs.RetrieveBy(BP.Sys.XML.SearchAttr.For, ens.ToString());
            //   string cfgurl = "";

            this.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            Attrs selectedAttrs = myen.EnMap.GetChoseAttrs(ens);
            Attr groupAttr = myen.EnMap.GetAttrByKey(groupkey);
            if (groupAttr.MyFieldType == FieldType.Enum
                || groupAttr.MyFieldType == FieldType.PKEnum)
            {
                SysEnums ses = new SysEnums(groupAttr.Key);
                this.AddTable();
                this.AddTR();
                int num = 0;
                foreach (Attr attrT in selectedAttrs)
                {
                    if (attrT.UIVisible == false || attrT.Key == groupAttr.Key)
                        continue;
                    this.AddTDTitle(attrT.Desc);
                    num++;
                }
                this.AddTREnd();

                foreach (SysEnum se in ses)
                {
                    int gval = se.IntKey;

                    int i = 0;
                    foreach (Entity en in ens)
                    {
                        if (en.GetValIntByKey(groupAttr.Key) != gval)
                            continue;
                        i++;
                    }
                    if (i == 0)
                        continue;

                    this.AddTR();
                    this.Add("<TD colspan=" + num + " class='Bar' >&nbsp;" + se.Lab + "&nbsp;(共" + i + "项)</TD>");
                    this.AddTREnd();

                    foreach (Entity en in ens)
                    {
                        if (en.GetValIntByKey(groupAttr.Key) != gval)
                            continue;

                        #region 处理 keys
                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);
                        #endregion

                        this.AddTRTXHand(" ondblclick=\"WinOpen('../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\"   onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" ");
                        foreach (Attr attr in selectedAttrs)
                        {
                            if (attr.UIVisible == false || attr.Key == groupAttr.Key)
                                continue;
                            // this.DataPanelDtlAdd(en, attr, cfgs, url);
                        }
                        this.AddTREnd();
                    }
                }
                this.AddTableEnd();
            }
            else
            {
                Entities ensG = BP.En.ClassFactory.GetEns(groupAttr.UIBindKey);
                ensG.RetrieveAll();
                this.AddTable(); //("<TABLE  class='Table' id='tb1' >");
                this.AddTR();
                int num = 0;
                foreach (Attr attrT in selectedAttrs)
                {
                    if (attrT.UIVisible == false || attrT.Key == groupAttr.Key)
                        continue;
                    this.AddTDTitle(attrT.Desc);
                    num++;
                }
                this.AddTREnd();

                foreach (Entity enG in ensG)
                {
                    string gval = enG.GetValStringByKey(groupAttr.UIRefKeyValue);

                    int i = 0;
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval)
                            continue;
                        i++;
                    }
                    if (i == 0)
                        continue;

                    this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(共" + i + "项)</TD></TR>");

                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval)
                            continue;

                        #region 处理 keys
                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);

                        #endregion

                        this.AddTRTXHand(" ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" ");
                        foreach (Attr attr in selectedAttrs)
                        {
                            if (attr.UIVisible == false || attr.Key == groupAttr.Key)
                                continue;
                            //   this.DataPanelDtlAdd(en, attr, cfgs, url);
                        }
                        this.AddTREnd();
                    }
                }
                this.AddTableEnd(); //("</TABLE>");
            }
        }
        /// <summary>
        /// DataPanelDtl
        /// </summary>
        /// <param name="ens">要bind ens</param>
        /// <param name="ctrlId">webmenu id </param>
        /// <param name="groupkey">groupkey</param>
        //		public void DataPanelDtl(Entities ens, string ctrlId, string groupkey, string groupkey2)
        public void DataPanelDtl(Entities ens, string ctrlId, string groupkey, string groupkey2)
        {
            if (groupkey2 == "None" || groupkey == groupkey2)
            {
                this.DataPanelDtl(ens, ctrlId, groupkey);
                return;
            }

            Entities ensG2 = new Emps();

            this.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            Attr groupAttr = myen.EnMap.GetAttrByKey(groupkey);
            Attr groupAttr2 = myen.EnMap.GetAttrByKey(groupkey2);

            BP.Sys.XML.Searchs cfgs = new BP.Sys.XML.Searchs();
            cfgs.RetrieveBy(BP.Sys.XML.SearchAttr.For, ens.ToString());
            // string cfgurl = "";

            #region 增加标题
            this.AddTable();
            this.AddTR();
            int num = 0;
            foreach (Attr attrT in myen.EnMap.Attrs)
            {
                if (attrT.UIVisible == false || attrT.Key == groupAttr.Key || attrT.Key == groupAttr2.Key)
                    continue;

                this.AddTDTitle(attrT.Desc);
                num++;
            }
            this.AddTREnd();
            #endregion

            if (groupAttr.MyFieldType == FieldType.Enum || groupAttr.MyFieldType == FieldType.PKEnum)
            {
                /* 如果第一个分组是枚举类型。*/
                SysEnums ses = new SysEnums(groupAttr.Key);
                if (groupAttr2.MyFieldType == FieldType.Enum || groupAttr2.MyFieldType == FieldType.PKEnum)
                {
                    /* 条件1 与条件2 都是枚举类型 */
                    SysEnums ses2 = new SysEnums(groupAttr2.Key);
                    foreach (SysEnum se in ses)
                    {
                        string gval = se.IntKey.ToString();
                        int i = 0;
                        foreach (Entity en in ens)
                        {
                            if (en.GetValStringByKey(groupAttr.Key) != gval)
                                continue;
                            i++;
                        }
                        if (i == 0)
                            continue;

                        this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + se.Lab + "&nbsp;(共" + i + "项)</TD></TR>");

                        // 开始便利2分组。
                        foreach (SysEnum se2 in ses2)
                        {
                            string gval2 = se2.IntKey.ToString();  //.GetValStringByKey(groupAttr2.UIRefKeyValue);
                            i = 0;
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;
                                i++;
                            }
                            if (i == 0)
                                continue;

                            this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + se2.Lab + "&nbsp;(共" + i + "项)</TD></TR>");
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;

                                string style = WebUser.Style;
                                string url = this.GenerEnUrl(en, attrs);
                                this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseover='TROver(this);OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');' onmouseout='TROut(this)' >");
                                foreach (Attr attr in attrs)
                                {
                                    if (attr.UIVisible == false || attr.Key == groupAttr2.Key || attr.Key == groupAttr.Key)
                                        continue;
                                    //this.DataPanelDtlAdd(en, attr, cfgs, url);
                                }
                                this.AddTREnd();
                            }
                        }
                    }
                }
                else  /* 分组第一个条件是枚举，第二个条件是entities. */
                {
                    ensG2 = BP.En.ClassFactory.GetEns(groupAttr2.UIBindKey);
                    ensG2.RetrieveAll();

                    foreach (SysEnum se in ses)
                    {
                        string gval = se.IntKey.ToString();
                        int i = 0;
                        foreach (Entity en in ens)
                        {
                            if (en.GetValStringByKey(groupAttr.Key) != gval)
                                continue;
                            i++;
                        }
                        if (i == 0)
                            continue;

                        this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + se.Lab + "&nbsp;(共" + i + "项)</TD></TR>");

                        // 开始便利2分组。
                        foreach (Entity enG2 in ensG2)
                        {
                            string gval2 = enG2.GetValStringByKey(groupAttr2.UIRefKeyValue);
                            i = 0;
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;
                                i++;
                            }
                            if (i == 0)
                                continue;

                            this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + enG2.GetValByKey(groupAttr2.UIRefKeyText) + "&nbsp;(共" + i + "项)</TD></TR>");
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;

                                string style = WebUser.Style;
                                string url = this.GenerEnUrl(en, attrs);
                                this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\"  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');\" onmouseout='TROut(this)' >");
                                foreach (Attr attr in attrs)
                                {
                                    if (attr.UIVisible == false || attr.Key == groupAttr2.Key || attr.Key == groupAttr.Key)
                                        continue;
                                    //this.DataPanelDtlAdd(en, attr, cfgs, url);
                                }
                                this.AddTREnd();
                            }
                        }
                    }

                    this.AddTableEnd();
                    return;
                }

            } /* 结束判断第一级标题是枚举类型的情况。 */


            Entities ensG = BP.En.ClassFactory.GetEns(groupAttr.UIBindKey);
            ensG.RetrieveAll();

            if (groupAttr2.MyFieldType == FieldType.Enum || groupAttr2.MyFieldType == FieldType.PKEnum)
            {
                /*如果 2 级别 是枚举类型*/
                SysEnums ses = new SysEnums(groupAttr2.Key);
                foreach (Entity enG in ensG)
                {
                    string gval = enG.GetValStringByKey(groupAttr.UIRefKeyValue);
                    int i = 0;
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval)
                            continue;
                        i++;
                    }
                    if (i == 0)
                        continue;

                    this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(共" + i + "项)</TD></TR>");

                    // 开始便利2分组。
                    foreach (SysEnum se in ses)
                    {
                        string gval2 = se.IntKey.ToString();  //.GetValStringByKey(groupAttr2.UIRefKeyValue);
                        i = 0;
                        foreach (Entity en in ens)
                        {
                            if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                continue;
                            i++;
                        }
                        if (i == 0)
                            continue;

                        this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + se.Lab + "&nbsp;(共" + i + "项)</TD></TR>");
                        foreach (Entity en in ens)
                        {
                            if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                continue;

                            string style = WebUser.Style;
                            string url = this.GenerEnUrl(en, attrs);
                            this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\"  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseout='TROut(this)' >");
                            foreach (Attr attr in attrs)
                            {
                                if (attr.UIVisible == false || attr.Key == groupAttr2.Key || attr.Key == groupAttr.Key)
                                    continue;
                                //this.DataPanelDtlAdd(en, attr, cfgs, url);
                            }
                            this.AddTREnd();
                        }
                    }
                }
                return;
            }

            ensG2 = BP.En.ClassFactory.GetEns(groupAttr2.UIBindKey);
            ensG2.RetrieveAll();
            foreach (Entity enG in ensG)
            {
                string gval = enG.GetValStringByKey(groupAttr.UIRefKeyValue);
                int i = 0;
                foreach (Entity en in ens)
                {
                    if (en.GetValStringByKey(groupAttr.Key) != gval)
                        continue;
                    i++;
                }
                if (i == 0)
                    continue;

                this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(共" + i + "项)</TD></TR>");

                // 开始便利2分组。
                foreach (Entity enG2 in ensG2)
                {
                    string gval2 = enG2.GetValStringByKey(groupAttr2.UIRefKeyValue);
                    i = 0;
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval)
                            continue;
                        if (en.GetValStringByKey(groupAttr2.Key) != gval2)
                            continue;
                        i++;
                    }
                    if (i == 0)
                        continue;

                    this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + enG2.GetValByKey(groupAttr2.UIRefKeyText) + "&nbsp;(共" + i + "项)</TD></TR>");
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                            continue;

                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);
                        this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + url + "')\" onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseout='TROut(this)' >");
                        foreach (Attr attr in attrs)
                        {
                            if (attr.UIVisible == false || attr.Key == groupAttr2.Key || attr.Key == groupAttr.Key)
                                continue;
                            // this.DataPanelDtlAdd(en, attr, cfgs, url);
                        }
                        this.AddTREnd();
                    }
                }
            }
            this.AddTableEnd(); //("</TABLE>");
        }
        private string GenerEnUrl(Entity en, Attrs attrs)
        {
            string url = "";
            foreach (Attr attr in attrs)
            {
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        if (attr.IsPK)
                            url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                    case UIContralType.DDL:
                        url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                }
            }
            return url;
        }

        private void DataPanelDtlAdd(Entity en, Attr attr, BP.Sys.XML.Searchs cfgs, string url, string cardUrl, string focusField)
        {
            string cfgurl = "";
            if (attr.UIContralType == UIContralType.DDL)
            {
                this.AddTD(en.GetValRefTextByKey(attr.Key));
                return;
            }
            if (attr.UIHeight != 0)
            {
                this.AddTDDoc("...", "...");
                return;
            }
            string str = en.GetValStrByKey(attr.Key);

            if (focusField == attr.Key)
                str = "<a href=" + cardUrl + ">" + str + "</a>";

            switch (attr.MyDataType)
            {
                case DataType.AppDate:
                case DataType.AppDateTime:
                    if (str == "" || str == null)
                        str = "&nbsp;";
                    this.AddTD(str);
                    break;
                case DataType.AppString:
                    if (str == "" || str == null)
                        str = "&nbsp;";

                    if (attr.UIHeight != 0)
                    {
                        this.AddTDDoc(str, str);
                    }
                    else
                    {
                        if (attr.Key.IndexOf("ail") == -1)
                            this.AddTD(str);
                        else
                            this.AddTD("<a href=\"javascript:mailto:" + str + "\"' >" + str + "</a>");
                    }
                    break;
                case DataType.AppBoolean:
                    if (str == "1")
                        this.AddTD("是");
                    else
                        this.AddTD("否");
                    break;
                case DataType.AppFloat:
                case DataType.AppInt:
                case DataType.AppDouble:
                    foreach (BP.Sys.XML.Search pe in cfgs)
                    {
                        if (pe.Attr == attr.Key)
                        {
                            cfgurl = pe.URL;
                            Attrs attrs = en.EnMap.Attrs;
                            foreach (Attr attr1 in attrs)
                                cfgurl = cfgurl.Replace("@" + attr1.Key, en.GetValStringByKey(attr1.Key));

                            break;
                        }
                    }
                    if (cfgurl == "")
                    {
                        this.AddTDNum(str);
                    }
                    else
                    {
                        cfgurl = cfgurl.Replace("@Keys", url);
                        this.AddTDNum("<a href=\"javascript:WinOpen('" + cfgurl + "','dtl1');\" >" + str + "</a>");
                    }
                    break;
                case DataType.AppMoney:
                    cfgurl = "";
                    foreach (BP.Sys.XML.Search pe in cfgs)
                    {
                        if (pe.Attr == attr.Key)
                        {
                            cfgurl = pe.URL;
                            Attrs attrs = en.EnMap.Attrs;
                            foreach (Attr attr2 in attrs)
                                cfgurl = cfgurl.Replace("@" + attr2.Key, en.GetValStringByKey(attr2.Key));
                            break;
                        }
                    }
                    if (cfgurl == "")
                    {
                        this.AddTDNum(decimal.Parse(str).ToString("0.00"));
                    }
                    else
                    {
                        cfgurl = cfgurl.Replace("@Keys", url);
                        this.AddTDNum("<a href=\"javascript:WinOpen('" + cfgurl + "','dtl1');\" >" + decimal.Parse(str).ToString("0.00") + "</a>");
                    }
                    break;
                default:
                    throw new Exception("no this case ...");
            }
        }
        //		public void UIEn1ToMGroupKey(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string groupKey)
        //edited by liuxc,2016-12-23,filterKeyWord:检索关键字
        public void UIEn1ToMGroupKey(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string groupKey, string filterKeyWord)
        {
            this.EnableViewState = true;
            this.Controls.Clear();
            this.Add("<TABLE class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'>");
            //this.AddTable(); // ("<TABLE class='Table' cellSpacing='1' cellPadding='1'  border='1'>");

            string val = string.Empty;
            Entity seEn = null;
            Attr attr = ens.GetNewEntity.EnMap.GetAttrByKey(groupKey);
            string text = null;

            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) // 检查是否是 enum 类型。
            {
                BP.Sys.SysEnums eens = new BP.Sys.SysEnums(attr.Key);

                //added by liuxc,2016-3-3,如果当前枚举查不出数据，则自动增加一条名称为“无分组数据”的组，此组下方显示所有数据
                bool noEnum = false;
                if(eens.Count == 0)
                {
                    noEnum = true;
                    eens.AddEntity(new SysEnum{EnumKey = "NoneEnum", Lang = "CH", IntKey = 0, Lab = "无分组数据"});
                }

                foreach (SysEnum se in eens)
                {
                    this.Add("<TR>");
                    this.Add("<TD class='GroupTitle' colspan=3 >");

                    CheckBox cb1 = new System.Web.UI.WebControls.CheckBox();
                    cb1.Text = se.Lab;
                    cb1.ID = "CB_SE_" + se.IntKey;
                    this.Add(cb1);
                    this.Add("</TD>");
                    this.AddTREnd();

                    int i = 0;
                    bool is1 = false;
                    string ctlIDs = "";
                    foreach (Entity en in ens)
                    {
                        if (en.GetValIntByKey(attr.Key) != se.IntKey && !noEnum)
                            continue;

                        text = en.GetValStringByKey(showText);

                        if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                            continue;

                        i++;
                        if (i == 4)
                            i = 1;
                        if (i == 1)
                            is1 = this.AddTR(is1);
                        //this.Add("<TR>");
                        CheckBox cb = new CheckBox();
                        val = en.GetValStringByKey(showVal);
                        cb.ID = "CB_" + val + "_" + se.IntKey;    //edited by liuxc,2015.1.6
                        ctlIDs += cb.ID + ",";

                        seEn = selectedEns.GetEntityByKey(selecteVal, val);
                        if (seEn != null)
                            cb.Checked = true;

                        cb.Text = text;
                        cb.AccessKey = se.IntKey.ToString();

                        this.AddTD(cb);
                        if (i == 3)
                            this.AddTREnd();
                    }
                    cb1.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    // add blank
                    switch (i)
                    {
                        case 1:
                            this.Add("<TD>&nbsp;</TD>");
                            this.Add("<TD>&nbsp;</TD>");
                            this.AddTREnd();
                            break;
                        case 2:
                            this.Add("<TD>&nbsp;</TD>");
                            this.AddTREnd();
                            break;
                        default:
                            break;
                    }
                }

            }
            else
            {
                Entities groupEns = ClassFactory.GetEns(attr.UIBindKey);
                groupEns.RetrieveAll();
                
                //added by liuxc,2016-3-3,如果当前分组类查不出数据，则自动增加一个名称为“无分组数据”的分组对象，此分组对象下方显示所有数据
                bool noGroup = false;
                if (groupEns.Count == 0)
                {
                    noGroup = true;

                    //创建分组对象实体
                    Entity en = groupEns.GetNewEntity;
                    en.SetValByKey(attr.UIRefKeyText, "无分组数据");
                    switch(en.EnMap.GetAttrByKey(attr.UIRefKeyValue).MyDataType)
                    {
                        case BP.DA.DataType.AppDate:
                        case BP.DA.DataType.AppDateTime:
                            en.SetValByKey(attr.UIRefKeyValue, BP.DA.DataType.CurrentData);
                            break;
                        case BP.DA.DataType.AppDouble:
                            en.SetValByKey(attr.UIRefKeyValue, (double)0);
                            break;
                        case BP.DA.DataType.AppFloat:
                            en.SetValByKey(attr.UIRefKeyValue, (float)0);
                            break;
                        case BP.DA.DataType.AppInt:
                            en.SetValByKey(attr.UIRefKeyValue, 0);
                            break;
                        case BP.DA.DataType.AppMoney:
                            en.SetValByKey(attr.UIRefKeyValue, (decimal)0);
                            break;
                        case BP.DA.DataType.AppString:
                            en.SetValByKey(attr.UIRefKeyValue, "0");
                            break;
                        case BP.DA.DataType.AppBoolean:
                            en.SetValByKey(attr.UIRefKeyValue, true);
                            break;
                        default:
                            en.SetValByKey(attr.UIRefKeyValue, "0");
                            break;
                    }

                    groupEns.AddEntity(en);
                }

                foreach (Entity group in groupEns)
                {
                    this.Add("<TR>");
                    this.Add("<TD class='GroupTitle' colspan=3>");

                    CheckBox cb1 = new System.Web.UI.WebControls.CheckBox();
                    cb1.Text = group.GetValStrByKey(attr.UIRefKeyText);
                    cb1.ID = "CB_EN_" + group.GetValStrByKey(attr.UIRefKeyValue);
                    // cb1.Attributes["onclick"] = "SetSelected(this,'" + group.GetValStringByKey(attr.UIRefKeyValue) + "')";
                    this.Add(cb1);
                    this.Add("</TD>");
                    this.AddTREnd();

                    string ctlIDs = "";
                    int i = 0;
                    string gVal = group.GetValStrByKey(attr.UIRefKeyValue);
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStrByKey(attr.Key) != gVal && !noGroup)
                            continue;

                        text = en.GetValStrByKey(showText);

                        if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                            continue;

                        i++;
                        if (i == 4)
                            i = 1;
                        if (i == 1)
                            this.Add("<TR>");


                        CheckBox cb = new CheckBox();
                        val = en.GetValStringByKey(showVal);
                        cb.ID = "CB_" + val + "_" + gVal; //edited by liuxc,2015.1.6

                        seEn = selectedEns.GetEntityByKey(selecteVal, val);
                        if (seEn != null)
                            cb.Checked = true;

                        cb.Text = text;
                        this.AddTD(cb);

                        ctlIDs += cb.ID + ",";
                        if (i == 3)
                            this.AddTREnd();
                    }

                    // add blank
                    switch (i)
                    {
                        case 1:
                            this.Add("<TD>&nbsp;</TD>");
                            this.Add("<TD>&nbsp;</TD>");
                            this.AddTREnd();
                            break;
                        case 2:
                            this.Add("<TD>&nbsp;</TD>");
                            this.AddTREnd();
                            break;
                        default:
                            break;
                    }
                    cb1.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
                }
            }

            this.AddTableEnd();

            // edited by liuxc,2015.1.6
            // 此处屏掉，选择已经选中的操作放于上面添加的逻辑中，此处不需要了
            // 设置选择的 ens .
            //foreach (Entity en in selectedEns)
            //{
            //    string key = en.GetValStringByKey(selecteVal);
            //    CheckBox bp = (CheckBox)this.FindControl("CB_" + key);
            //    if (bp == null)
            //        continue;

            //    bp.Checked = true;
            //}

        }
        //		public void UIEn1ToMGroupKey_Line(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string groupKey)
        //edited by liuxc,2016-12-23,filterKeyWord:检索关键字
        public void UIEn1ToMGroupKey_Line(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string groupKey, string filterKeyWord)
        {
            this.EnableViewState = true;
            this.Controls.Clear();
            this.Add("<TABLE class='Table' cellSpacing='1' cellPadding='1'  border='1' width='100%' >");

            Attr attr = ens.GetNewEntity.EnMap.GetAttrByKey(groupKey);
            var val = string.Empty;
            Entity seEn = null;
            string text = null;

            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) // 检查是否是 enum 类型。
            {
                BP.Sys.SysEnums eens = new BP.Sys.SysEnums(attr.Key);
                foreach (SysEnum se in eens)
                {
                    this.Add("<TR>");
                    this.Add("<TD class='GroupTitle' >" + se.Lab + "</TD>");
                    this.AddTREnd();
                    foreach (Entity en in ens)
                    {
                        if (en.GetValIntByKey(attr.Key) != se.IntKey)
                            continue;

                        text = en.GetValStrByKey(showText);

                        if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                            continue;

                        this.AddTR();
                        CheckBox cb = new CheckBox();
                        val = en.GetValStrByKey(showVal);
                        cb.ID = "CB_" + val + "_" + se.IntKey;    //edited by liuxc,2015.1.6

                        seEn = selectedEns.GetEntityByKey(selecteVal, val);
                        if (seEn != null)
                            cb.Checked = true;

                        cb.Text = text;
                        this.AddTD(cb);
                        this.AddTREnd();
                    }
                }
            }
            else
            {
                Entities groupEns = ClassFactory.GetEns(attr.UIBindKey);
                groupEns.RetrieveAll();
                var gVal = string.Empty;
                foreach (Entity group in groupEns)
                {
                    gVal = group.GetValStringByKey(attr.UIRefKeyValue);
                    this.Add("<TR>");
                    this.Add("<TD class='GroupTitle' >" + group.GetValStringByKey(attr.UIRefKeyText) + "</TD>");
                    this.AddTREnd();

                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(attr.Key) != gVal)
                            continue;

                        text = en.GetValStringByKey(showText);

                        if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                            continue;

                        this.Add("<TR>");

                        CheckBox cb = new CheckBox();
                        val = en.GetValStringByKey(showVal);
                        cb.ID = "CB_" + val + "_" + gVal;  //edited by liuxc,2015.1.6
                        cb.Text = text;

                        seEn = selectedEns.GetEntityByKey(selecteVal, val);
                        if (seEn != null)
                            cb.Checked = true;

                        this.Add("<TD >");
                        this.Add(cb);
                        this.Add("</TD>");
                        this.AddTREnd();
                    }
                }
            }

            this.AddTableEnd();

            // edited by liuxc,2015.1.6
            // 此处屏掉，选择已经选中的操作放于上面添加的逻辑中，此处不需要了
            // 设置选择的 ens .
            //foreach (Entity en in selectedEns)
            //{
            //    string key = en.GetValStringByKey(selecteVal);
            //    CheckBox bp = (CheckBox)this.FindControl("CB_" + key);
            //    if (bp == null)
            //        continue;

            //    bp.Checked = true;
            //}

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="groupKey"></param>
        //		public void UIEn1ToM(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal)
        //edited by liuxc,2016-12-23,filterKeyWord:检索关键字
        public void UIEn1ToM(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string filterKeyWord)
        {
            this.Controls.Clear();
            this.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            int i = 0;
            bool is1 = false;
            string text = null;

            foreach (Entity en in ens)
            {
                text = en.GetValStringByKey(showText);

                if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                    continue;

                i++;
                if (i == 4)
                    i = 1;

                if (i == 1)
                {
                    is1 = this.AddTR(is1);
                }

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + en.GetValStringByKey(showVal);
                cb.Text = text;
                this.AddTD(cb);
                if (i == 3)
                    this.AddTREnd();
            }

            switch (i)
            {
                case 1:
                    this.AddTD();
                    this.AddTD();//"<TD>&nbsp;</TD>");
                    this.AddTREnd();//("</TR>");
                    break;
                case 2:
                    this.AddTD();
                    this.AddTREnd();
                    break;
                default:
                    break;
            }
            this.AddTableEnd();

            // 设置选择的 ens .
            foreach (Entity en in selectedEns)
            {
                string key = en.GetValStringByKey(selecteVal);
                CheckBox bp = this.FindControl("CB_" + key) as CheckBox;

                if (bp == null)
                    continue;

                bp.Checked = true;               
            }
        }
        public void UIEn1ToM_Tree(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal)
        {
            this.Controls.Clear();
            //this.Add("<table border=0 width='500px'>");
            //this.AddTR();
            //this.AddTDBegin();
            // this.Add("<font size=12px >");
            string no = null;
            foreach (Entity en in ens)
            {
                no = en.GetValStrByKey(showVal);
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + no;
                cb.Text = no + "&nbsp;" + en.GetValStrByKey(showText);
                this.Add(DataType.GenerSpace(no.Length / 2));
                this.Add(cb);
                this.AddBR();
            }
            //this.Add("</font>");
            //this.AddTDEnd();
            //this.AddTREnd();
            //this.AddTableEnd();

            // 设置选择的 ens .
            foreach (Entity en in selectedEns)
            {
                string key = en.GetValStringByKey(selecteVal);
                CheckBox bp = (CheckBox)this.FindControl("CB_" + key);
                bp.Checked = true;
            }
        }

        //./././ public void UIEn1ToM_OneLine(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal)
        //edited by liuxc,2016-12-23,filterKeyWord:检索关键字
        public void UIEn1ToM_OneLine(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string filterKeyWord)
        {
            this.Controls.Clear();
            this.Add("<table border=0 width='500px'>");
            bool is1 = false;
            string text = null;

            foreach (Entity en in ens)
            {
                text = en.GetValStringByKey(showText);

                if (!string.IsNullOrWhiteSpace(filterKeyWord) && text.IndexOf(filterKeyWord) == -1)
                    continue;

                is1 = this.AddTR(is1); //("<TR>");
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + en.GetValStrByKey(showVal);
                cb.Text = text;
                this.AddTD(cb);
                this.AddTREnd();
            }
            this.AddTableEnd();

            // 设置选择的 ens .
            foreach (Entity en in selectedEns)
            {
                string key = en.GetValStrByKey(selecteVal);
                CheckBox bp = this.FindControl("CB_" + key) as CheckBox;

                if (bp == null)
                    continue;

                bp.Checked = true;
            }
        }
        /// <summary>
        /// s
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="ctrlId"></param>
        /// <param name="showtext1"></param>
        /// <param name="showDtl"></param>
        private void DataPanelCards(Entities ens, string ctrlId, string showtext1, bool showDtl)
        {
            this.Controls.Clear();
            this.AddTable();
            int i = 0;
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string textName1 = myen.EnMap.GetAttrByKey(showtext1).Desc;
            //	string textName2=myen.EnMap.GetAttrByKey(showtext2).Desc;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;

            foreach (Entity en in ens)
            {
                if (i == 0)
                    this.AddTREnd();
                i++;

                #region 处理keys
                string style = WebUser.Style;
                string url = "";
                foreach (Attr attr in attrs)
                {
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.IsPK)
                                url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                            break;
                        case UIContralType.DDL:
                            url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                            break;
                    }
                }
                #endregion

                string context = "";
                if (showDtl)
                {
                    context = "<TABLE class='TableCard'  >";
                    foreach (Attr attr in attrs)
                    {
                        if (attr.Key == showtext1)
                            continue;

                        switch (attr.MyFieldType)
                        {
                            case FieldType.Normal:
                                if (attr.UIVisible == true)
                                {
                                    if (en.GetValStringByKey(attr.Key) == "")
                                        continue;
                                    context += "<TR><TD nowrap class='TDLeft' >" + attr.Desc + "</TD><TD   class='RightTD' >" + en.GetValStringByKey(attr.Key) + "</TD></TR>";
                                }
                                break;
                            case FieldType.RefText:
                                if (en.GetValStringByKey(attr.Key) == "")
                                    continue;
                                context += "<TR><TD nowrap class='TDLeft' >" + attr.Desc.Replace("名称", "") + "</TD><TD   class='RightTD' >" + en.GetValStringByKey(attr.Key) + "</TD></TR>";
                                //context+="<TR><TD nowrap >"+attr.Desc.Replace("名称","")+"</TD><TD nowrap >"+en.GetValStringByKey( attr.Key )+"</TD></TR>";
                                break;
                        }
                    }
                    context += "</TABLE>";
                }

                string img = "<img src='" + en.EnMap.Icon + "'/>";
                if (i == 3)
                {
                    i = 0;
                    this.Add("<TD   valign=top ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + en.ToString() + "&PKVal=" + en.GetValByKey(pk) + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\"  >" + img + "&nbsp;<b>" + en.GetValStringByKey(showtext1) + "</b>" + context + "</TD>");
                    this.AddTREnd();
                }
                else
                {
                    this.Add("<TD class='TD' valign=top ondblclick=\"WinOpen( '../Comm/En.htm?EnName=" + ens.ToString() + "&PKVal=" + en.GetValByKey(pk) + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\"  >" + img + "&nbsp;<b>" + en.GetValStringByKey(showtext1) + "</b>" + context + "</TD>");
                }
            }

            switch (i)
            {
                case 1:
                    this.Add("<TD class='TD' >&nbsp;</TD>");
                    this.Add("<TD   >&nbsp;</TD>");
                    this.AddTREnd();
                    break;
                case 2:
                    this.Add("<TD   >&nbsp;</TD>");
                    this.AddTREnd();
                    break;
            }
            this.Add("</TABLE>");
        }
        /// <summary>
        /// 查看文件
        /// </summary>
        /// <param name="en"></param>
        //		public void FilesView(string enName, string pk)
        public void FilesView(string enName, string pk)
        {
            this.Controls.Clear();
            SysFileManagers ens = new SysFileManagers(enName, pk);
            this.Add("<TABLE BORDER=1>");
            this.Add("<TR>");
            this.Add("<TD>编号</TD>");
            this.Add("<TD>文件名称</TD>");
            this.Add("<TD>上传人</TD>");
            this.Add("<TD>上传时间</TD>");
            this.Add("<TD>大小</TD>");
            this.Add("<TD>操作</TD>");
            this.AddTREnd();
            foreach (SysFileManager file in ens)
            {
                this.Add("<TR>");
                this.Add("<TD>" + file.OID + "</TD>");
                this.Add("<TD><img src='../Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='" + this.Request.ApplicationPath + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a></TD>");
                this.Add("<TD>" + file.RecText + "</TD>");
                this.Add("<TD>" + file.RDT + "</TD>");
                this.Add("<TD>" + file.MyFileSize + "</TD>");
                if (file.Rec == WebUser.No)
                {
                    this.Add("<TD><a href=\"javascript:DoAction('FileManager.aspx?OID=" + file.OID + "&EnsName=" + enName + "&PK=" + pk + "','将要删除 《" + file.MyFileName + "》')\" >删除</a></TD>");
                }
                else
                {
                    this.Add("<TD>无</TD>");
                }
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在此处放置用户代码以初始化页面
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		设计器支持所需的方法 - 不要使用代码编辑器
        ///		修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}

