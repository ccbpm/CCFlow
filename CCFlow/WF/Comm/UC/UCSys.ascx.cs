//NOTE:�޸�˵��
//2015.1.6 liuxc,���1����Ա���ڲ�ͬ���ŵ�����£����������ʾ�򱨴�����⣬ÿ��Checkbox��ID��+�����ֵ�����֣������ʱ��ͬ1����Աֻ����һ�Σ��޸��漰���༰Dot2Dot.ascx.cs
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
    ///		UCSys ��ժҪ˵����
    /// </summary>
    public partial class UCSys : BP.Web.UC.UCBase
    {
        public static string FilesViewStr(string enName, object pk)
        {
            string url = "../Comm/FileManager.aspx?EnsName=" + enName + "&PK=" + pk.ToString();

            //string strs="<a href=\"javascript:WinOpen("") \" >����</>";
            //string strs="<a href=\"javascript:WinOpen('"+url+"') \" >�༭����</>";
            string strs = "";
            SysFileManagers ens = new SysFileManagers(enName, pk.ToString());
            string path = System.Web.HttpContext.Current.Request.ApplicationPath;
            foreach (SysFileManager file in ens)
            {
                strs += "<img src='../Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='" + path + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a>&nbsp;";
                if (file.Rec == WebUser.No)
                {
                    strs += "<a title='����' href=\"javascript:DoAction('../Comm/Do.aspx?ActionType=" + (int)ActionType.DeleteFile + "&OID=" + file.OID + "&EnsName=" + enName + "&PK=" + pk + "','ɾ���ļ���" + file.MyFileName + file.MyFileExt + "��')\" ><img src='" + path + "../Img/Btn/delete.gif' border=0 alt='ɾ���˸���' /></a>&nbsp;";
                }
            }
            return strs;
        }

        public static string FilesViewStr1(string enName, object pk)
        {
            string url = "../Comm/FileManager.aspx?EnsName=" + enName + "&PK=" + pk.ToString();

            //string strs="<a href=\"javascript:WinOpen("") \" >����</>";
            string strs = "<a href=\"javascript:WinOpen('" + url + "') \" >�༭����</>";
            SysFileManagers ens = new SysFileManagers(enName, pk.ToString());
            foreach (SysFileManager file in ens)
            {
                strs += "<img src='../Img/FileType/" + file.MyFileExt.Replace(".", "") + ".gif' border=0 /><a href='../" + file.MyFilePath + "' target='_blank' >" + file.MyFileName + file.MyFileExt + "</a>&nbsp;";
            }
            return strs;
        }

        public string GenerIt()
        {
            ////����һ��ͼ����������
            //OWC11.ChartSpace objCSpace = new OWC11.ChartSpaceClass();
            ////��ͼ������������һ��ͼ�ζ���
            //OWC11.ChChart objChart = objCSpace.Charts.Add(0);
            ////��ͼ�ε���������Ϊ��״ͼ��һ��
            //objChart.Type = OWC11.ChartChartTypeEnum.chChartTypeColumnStacked;
            ////��ͼ�������ı߿���ɫ����Ϊ��ɫ
            //objCSpace.Border.Color = "White";

            ////��ʾ����
            //objChart.HasTitle = true;
            ////���ñ�������
            //objChart.Title.Caption = "ͳ��ͼ����";
            ////���ñ�������Ĵ�С
            //objChart.Title.Font.Size = 10;
            ////���ñ���Ϊ����
            //objChart.Title.Font.Bold = true;
            ////���ñ�����ɫΪ��ɫ
            //objChart.Title.Font.Color = "Red";

            ////��ʾͼ��
            //objChart.HasLegend = true;
            ////����ͼ�������С
            //objChart.Legend.Font.Size = 10;
            ////����ͼ��λ��Ϊ�׶�
            //objChart.Legend.Position = OWC11.ChartLegendPositionEnum.chLegendPositionBottom;

            ////��ͼ�ζ��������һ��ϵ��
            //objChart.SeriesCollection.Add(0);
            ////����ϵ�е�����
            //objChart.SeriesCollection[0].SetData(OWC11.ChartDimensionsEnum.chDimSeriesNames,
            //    +(int)OWC11.ChartSpecialDataSourcesEnum.chDataLiteral, "ָ��");
            ////����ֵ
            //objChart.SeriesCollection[0].SetData(OWC11.ChartDimensionsEnum.chDimValues,
            //    +(int)OWC11.ChartSpecialDataSourcesEnum.chDataLiteral, "10\t40\t58\t55\t44");

            ////��ʾ���ݣ�����GIF�ļ������·��.
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
            ////��������
            //ChartSpace ThisChart = new ChartSpaceClass();
            //ChChart ThisChChart = ThisChart.Charts.Add(0);
            //ChSeries ThisChSeries = ThisChChart.SeriesCollection.Add(0);

            ////��ʾͼ��
            //ThisChChart.HasLegend = true;
            ////����
            //ThisChChart.HasTitle = true;
            //ThisChChart.Title.Caption = title;

            ////����x,y��ͼʾ˵��
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
            //        //��ת
            //        ThisChChart.Rotation = 360;
            //        ThisChChart.Inclination = 10;
            //        //������ɫ
            //        ThisChChart.PlotArea.Interior.Color = "white";
            //        //��ɫ
            //        ThisChChart.PlotArea.Floor.Interior.Color = "green";
            //        ////����series������
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), colOfGroupName);
            //        //��������
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimCategories,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), strCategory);
            //        //����ֵ
            //        ThisChSeries.SetData(ChartDimensionsEnum.chDimValues,
            //            ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(), strValue);
            //        break;
            //    case ChartType.Pie:
            //        // ��������
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField].ToString() + '\t';
            //            strValue += dr[colOfNumField].ToString() + '\t';
            //        }

            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypePie3D;
            //        ThisChChart.SeriesCollection.Add(0);
            //        //��ͼ������ʾ����
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection.Add();
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position = ChartDataLabelPositionEnum.chLabelPositionAutomatic;
            //        ThisChChart.SeriesCollection[0].Marker.Style = ChartMarkerStyleEnum.chMarkerStyleCircle;

            //        //��������ͼ�����ݵ����� 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, "strSeriesName");

            //        //�������ݷ��� 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, strCategory);

            //        //����ֵ 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
            //            (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
            //        break;
            //    case ChartType.Line:
            //        // ��������
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField].ToString() + '\t';
            //            strValue += dr[colOfNumField].ToString() + '\t';
            //        }
            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypeLineStacked;
            //        ThisChChart.SeriesCollection.Add(0);
            //        //��ͼ������ʾ����
            //        ThisChChart.SeriesCollection[0].DataLabelsCollection.Add();
            //        //ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position=ChartDataLabelPositionEnum.chLabelPositionAutomatic;
            //        //ThisChChart.SeriesCollection[0].DataLabelsCollection[0].Position=ChartDataLabelPositionEnum.chLabelPositionOutsideBase;

            //        ThisChChart.SeriesCollection[0].Marker.Style = ChartMarkerStyleEnum.chMarkerStyleCircle;

            //        //��������ͼ�����ݵ����� 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimSeriesNames,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, "strSeriesName");

            //        //�������ݷ��� 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimCategories,
            //            +(int)ChartSpecialDataSourcesEnum.chDataLiteral, strCategory);

            //        //����ֵ 
            //        ThisChChart.SeriesCollection[0].SetData(ChartDimensionsEnum.chDimValues,
            //            (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
            //        break;
            //}

            ////����ͼ���ļ�
            ////ThisChart.ExportPicture("G:\\chart.gif","gif",600,350);

            //string fileName = ct.ToString() + WebUser.No + ".gif";
            //string strAbsolutePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + fileName;
            //try
            //{
            //    ThisChart.ExportPicture(strAbsolutePath, "GIF", chartWidth, chartHeight);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("@���ܴ����ļ�,������Ȩ�޵����⣬��Ѹ�Ŀ¼����Ϊ�κ��˶������޸ġ�" + strAbsolutePath + " Exception:" + ex.Message);
            //}
            return fileName;
        }

        /// <summary>
        /// ����MS Chart��״ͼ
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="xfield">X��Ҫ��ʾ���ֶ�</param>
        /// <param name="xdesc">X��Ҫ��ʾ���ֶ��ı�</param>
        /// <param name="yfields">Y��Ҫ��������Ŀ
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.�󶨵��ֶ�</para>
        /// <para>Value.�󶨵��ֶ���ʾ����</para>
        /// </remarks></param>
        /// <param name="title">ͼ�����</param>
        /// <param name="chartWidth">����ͼ��Ŀ��</param>
        /// <param name="chartHeight">����ͼ��ĸ߶�</param>
        public void GenerateColumnChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            StringBuilder columnStrB = new StringBuilder();

            columnStrB.Append(" <chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='��������,���Ժ�...' exportFormats='PNG=����PNGͼƬ|JPG=����JPGͼƬ|PDF=����PDF�ļ�' decimalPrecision='4'  hoverCapBgColor='ffffff' bgColor='B8D288,FFFFFF' outCnvBaseFontSize='12' anchorBgColor='008ED6' caption='" + title + "'");
            columnStrB.Append("AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' >");
            columnStrB.Append("numberPrefix='' showNames='1' showValues='1'  showAlternateHGridColor='1' ");
            columnStrB.Append(" decimalPrecision='0' formatNumberScale='0' ");
            columnStrB.Append(" yAxisName='hkhkhk' ");  // �ײ���ʾ������.
            columnStrB.Append(" xAxisName='adadasd' ");     // �����ʾ������.

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

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - ��״ͼ");
            //chartTitle.Font = new Font(new FontFamily("����"), 10.5f, FontStyle.Bold);
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
            //chartArea.AxisX.TitleFont = new Font(new FontFamily("����"), 9f, FontStyle.Bold);

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
            //    chartArea.AxisY.TitleFont = new Font(new FontFamily("����"), 9f, FontStyle.Bold);
            //}

            //chart.DataSource = dt;
            //chart.DataBind();

            //this.Add(chart);
        }

        /// <summary>
        /// ����MS Chart��ͼ
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="xfield">X��Ҫ��ʾ���ֶ�</param>
        /// <param name="xdesc">X��Ҫ��ʾ���ֶ��ı�</param>
        /// <param name="yfields">Y��Ҫ��������Ŀ
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.�󶨵��ֶ�</para>
        /// <para>Value.�󶨵��ֶ���ʾ����</para>
        /// </remarks></param>
        /// <param name="title">ͼ�����</param>
        /// <param name="chartWidth">����ͼ��Ŀ��</param>
        /// <param name="chartHeight">����ͼ��ĸ߶�</param>
        public void GeneratePieChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            var firstKey = yfields.First();
            StringBuilder pieStrB = new StringBuilder();

            pieStrB.Append("<chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='��������,���Ժ�...' " +
                            "exportFormats='PNG=����PNGͼƬ|JPG=����JPGͼƬ|PDF=����PDF�ļ�' caption='" + title + "-" + firstKey.Value + "' outCnvBaseFontColor='FFFFFF' " +
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

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - ��״ͼ");
            //chartTitle.Font = new Font(new FontFamily("����"), 10.5f, FontStyle.Bold);
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
            //    tempLegend.Font = new Font(new FontFamily("����"), 9f, FontStyle.Bold);
            //    tempLegend.DockedToChartArea = chartArea.Name;
            //    tempLegend.Docking = Docking.Left;
            //    tempLegend.CustomItems.Add(Color.Transparent, yfield.Value);
            //}

            //this.Add(chart);
        }

        /// <summary>
        /// ����MS Chart����ͼ
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="xfield">X��Ҫ��ʾ���ֶ�</param>
        /// <param name="xdesc">X��Ҫ��ʾ���ֶ��ı�</param>
        /// <param name="yfields">Y��Ҫ��������Ŀ
        /// <remarks>Dictionary&lt;string, string&gt;
        /// <para>Key.�󶨵��ֶ�</para>
        /// <para>Value.�󶨵��ֶ���ʾ����</para>
        /// </remarks></param>
        /// <param name="title">ͼ�����</param>
        /// <param name="chartWidth">����ͼ��Ŀ��</param>
        /// <param name="chartHeight">����ͼ��ĸ߶�</param>
        public void GenerateLineChart(DataTable dt, string xfield, string xdesc, Dictionary<string, string> yfields, string title, int chartWidth = 300, int chartHeight = 300)
        {
            StringBuilder lineStrB = new StringBuilder();

            lineStrB.Append(" <chart  exportEnabled='1' exportAtClient='1' exportHandler='fcExporter1' exportDialogMessage='��������,���Ժ�...' exportFormats='PNG=����PNGͼƬ|JPG=����JPGͼƬ|PDF=����PDF�ļ�' decimalPrecision='4'  hoverCapBgColor='AFD8F8' bgColor='2E4A89, 90B1DE' outCnvBaseFontSize='12' anchorBgColor='008ED6' caption='" + title + "'");
            lineStrB.Append("AlternateHGridColor='ff5904' divLineColor='ff5904' divLineAlpha='20' alternateHGridAlpha='5' >");
            lineStrB.Append("numberPrefix='' showNames='1' showValues='1'  showAlternateHGridColor='1' ");
            lineStrB.Append(" decimalPrecision='0' formatNumberScale='0' ");
            lineStrB.Append(" yAxisName='hkhkhk' ");  // �ײ���ʾ������.
            lineStrB.Append(" xAxisName='adadasd' ");     // �����ʾ������.


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
            //----------------------------------------���·�������---------------------------------------------
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

            //var chartTitle = chart.Titles.Add(title + " - " + xdesc + " - ����ͼ");
            //chartTitle.Font = new Font(new FontFamily("����"), 10.5f, FontStyle.Bold);
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
            //    //chartArea.AxisX.TitleFont = new Font(new FontFamily("����"), 9f, FontStyle.Bold);
            //    chartArea.AxisY.Title = yfield.Value;
            //    chartArea.AxisY.TitleAlignment = StringAlignment.Far;
            //    chartArea.AxisY.TitleFont = new Font(new FontFamily("����"), 9f, FontStyle.Bold);

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
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  > <b>ȫ��</A> </TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' >ȫ��</A> </TD>");
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
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><img src='" + imgPath + "all.gif' border=0 />ȫ��</TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' ><img src='" + imgPath + "all.gif' border=0 />ȫ��</A> </TD>");
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
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  > <b>ȫ��</A> </TD>");
                else
                    this.Add("<TD style='font-size:12px; font-weight:bolder; ' background=Enum.gif  width='126' height='36' align=center  ><A href='" + url.Replace("@" + enumKey, "all") + "' >ȫ��</A> </TD>");
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
                string desc = dr["Desc"].ToString(); //��������

                if (img.Trim().Length != 5)
                    name = "<img src='" + img + "' border=0 />" + name;

                string srcp = "window.location.href='LeftOutlook.aspx?cate=" + no + "'";
                /*����Ŀ¼���ݡ�*/
                if (cate == no)
                {
                    /* ��ǰҪѡ������*/
                    this.Add("<TR  >");
                    this.Add("<TD class='TDM_Selected' nowrap=true title='" + dr["DESC"].ToString() + "' ><b>" + name + "</b></TD>");
                    this.AddTREnd();

                    /*��������˵�ǰҪѡ��Ĳ˵���*/
                    this.Add("<TR height='100%' >");
                    this.Add("<TD calss='TDItemTable'  height='100%'  >");
                    this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                    foreach (DataRow itemdr in dtl.Rows)
                    {
                        string no1 = itemdr["No"].ToString();
                        string name1 = itemdr[BP.Web.WebUser.SysLang].ToString();
                        string url1 = itemdr["Url"].ToString();
                        string img1 = itemdr["Img"].ToString();
                        string desc1 = itemdr["Desc"].ToString(); //��������

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



                /*��������˵�ǰҪѡ��Ĳ˵���*/
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
            this.AddTDTitle("��");
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
            this.AddTDTitle("��");
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


            if (xmlFile == null || xmlFile == "ss")  //���û���ҵ����������õ�һ����
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
                    /*��������˵�ǰҪѡ��Ĳ˵���*/
                    this.Add("<TR  >");
                    this.Add("<TD calss='TDItemTable' >");
                    this.Add("<Table   class='ItemTable'  cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
                    this.Add("</Table>");
                    this.Add("</TD>");
                    this.AddTREnd();
                }
                else if (file == xmlFile)
                {
                    /*��������˵�ǰҪѡ��Ĳ˵���*/
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



                /*��������˵�ǰҪѡ��Ĳ˵���*/
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
            if (xmlFile == null || xmlFile == "")  //���û���ҵ����������õ�һ����
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
                    /*��������˵�ǰҪѡ��Ĳ˵���*/
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
            this.AddTDTitle("ϵͳ���");
            this.AddTDTitle("����");
            this.AddTDTitle("�汾");
            this.AddTDTitle("��������");
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
            //this.Add("<font color='#000000' size=2 >��ӭ����"+WebUser.Name+"�����ţ�"+WebUser.HisEmp.FK_DeptText+"����λ��"+WebUser.HisEmp.FK_StationText+"��</font>");
        }
        //		public void BindMsgInfo(string msg)
        public void BindMsgInfo(string msg)
        {
            this.Controls.Clear();
            this.Add("<Table  border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' >");
            this.Add("<Caption align=left ><b>��ʾ��Ϣ</b></Caption>");
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
            // ���ɱ���
            this.Add("<TABLE  style='border-collapse: collapse' bordercolor='#111111' >");
            this.Add("<TR >");
            this.Add("<TH  nowrap >��</TH>");
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
                #region ����keys
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

                this.Add("<TR  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');\" onmouseout='TROut(this)' ondblclick=\" WinOpen('../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\" >");
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
                                    this.AddTD("��&nbsp;");
                                else
                                    this.AddTD("��&nbsp;");
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

            // ���ɱ���
            this.Add("<table  style=\"width:30%\" >");
            this.AddTR();

            // CheckBox cb = new CheckBox();
            // cb.Text = "��";
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

                #region ����keys
                string style = WebUser.Style;
                string url = this.GenerEnUrl(en, attrs);
                #endregion


                urlExt = "\"javascript:ShowEn('./RefFunc/UIEn.aspx?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "', 'cd','" + cfg.WinCardH + "','" + cfg.WinCardW + "');\"";

                // urlExt = "javascript:ShowEn('./RefFunc/UIEn.aspx?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "', 'cd');";

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
                        throw new Exception("@Ŀǰ��û���ṩ��");
                }


                // this.Add("<TR onmouseover=\"TROver(this);\" onmouseout='TROut(this)' ondblclick=\"WinOpen( 'UIEn.aspx?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "', 'cd' )\"   >");
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

                //    #region �������ŵ� ����
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

                //    #region ����������ϸ
                //    EnDtls enDtls = en.EnMap.Dtls;
                //    foreach (EnDtl enDtl in enDtls)
                //    {
                //        str += "[<A onclick=\"javascript:EditDtl1('" + this.Request.ApplicationPath + "', '" + myen.ToString() + "',  '" + enDtl.EnsName + "', '" + enDtl.RefKey + "', '" + url + "&IsShowSum=1')\" >" + enDtl.Desc + "</A>]";
                //    }
                //    #endregion

                //    #region ����һ�Զ��ʵ��༭
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

            // ���ɱ���
            //this.Add("<Table border='0' width='100%' align=left cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#C0C0C0'>");
            this.Add("<Table class='Table' border='0' width='100%' cellpadding='0' cellspacing='0' style=\"border-style: none; width:100%\">");
            // this.AddTable("");
            this.AddTR();
            this.AddTDGroupTitle("style='text-align:center; width:40px;'", "��");
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
            //���²�ѯʱ����Ϊ��1ҳ
            if (IsPostBack)
                pageidx = 0;

            int idx = SystemConfig.PageSize * pageidx;
            bool is1 = false;

            #region �û�������������

            bool isAddTitle = false;  //�Ƿ���ʾ��ع����С�
            if (cfg.IsEnableOpenICON)
                isAddTitle = true;
            if (cfg.IsEnableRefFunc)
                isAddTitle = true;
            #endregion �û�������������

            if (isAddTitle)
                this.AddTDGroupTitle();

            this.AddTREnd();

            string urlExt = "";
            foreach (Entity en in ens)
            {
                #region ����keys
                string style = WebUser.Style;
                string url = this.GenerEnUrl(en, attrs);
                #endregion

                urlExt = "\"javascript:ShowEn('../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "&inlayer=1', 'cd','" + cfg.WinCardH + "','" + cfg.WinCardW + "');\"";
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
                        throw new Exception("@Ŀǰ��û���ṩ��");
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
                    // string str = "<a href=\"javascript:WinOpen('../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "', 'cd')\" >��</a>";
                    //<a href=\"javascript:WinOpen('../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "','cd','400','600')\" >��</A>

                    #region �������ŵķ���
                    RefMethods myreffuncs = en.EnMap.HisRefMethods;
                    foreach (RefMethod func in myreffuncs)
                    {
                        if (func.Visable == false || func.IsForEns == false || func.IsCanBatch == true || func.RefAttrKey != null)
                            continue;
                        
                        str += "<A style='cursor:hand;' nowrap=true onclick=\"javascript:RefMethod1('" + this.Request.ApplicationPath + "', '" + func.Index + "', '" + func.Warning + "', '" + func.Target + "', '" + ens.ToString() + "','" + url + "') \"  > " + func.GetIcon(this.Request.ApplicationPath) + "" + func.Title + "</A>";
                    }
                    #endregion

                    #region ����������ϸ
                    EnDtls enDtls = en.EnMap.Dtls;
                    foreach (EnDtl enDtl in enDtls)
                    {
                        str += "[<A onclick=\"javascript:EditDtl1('" + this.Request.ApplicationPath + "', '" + myen.ToString() + "',  '" + en.ToString() + "',  '" + enDtl.EnsName + "', '" + enDtl.RefKey + "', '" + url + "&IsShowSum=1')\" >" + enDtl.Desc + "</A>]";
                    }
                    #endregion

                    #region ����һ�Զ��ʵ��༭
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

            #region  ��ϼƴ���д�����
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
                // �ҳ������ǲ���ʾ�ϼƵ��С�

                if (NoShowSum == null)
                    NoShowSum = "";

                this.Add("<TR class='TRSum' >");
                this.AddTD("�ϼ�");
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
                        /*����Ҫ��ʾ�����ǵĺϼơ�*/
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
        /// <param name="ens">Ҫbind ens</param>
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
                    this.Add("<TD colspan=" + num + " class='Bar' >&nbsp;" + se.Lab + "&nbsp;(��" + i + "��)</TD>");
                    this.AddTREnd();

                    foreach (Entity en in ens)
                    {
                        if (en.GetValIntByKey(groupAttr.Key) != gval)
                            continue;

                        #region ���� keys
                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);
                        #endregion

                        this.AddTRTXHand(" ondblclick=\"WinOpen('../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\"   onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" ");
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

                    this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(��" + i + "��)</TD></TR>");

                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval)
                            continue;

                        #region ���� keys
                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);

                        #endregion

                        this.AddTRTXHand(" ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" ");
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
        /// <param name="ens">Ҫbind ens</param>
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

            #region ���ӱ���
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
                /* �����һ��������ö�����͡�*/
                SysEnums ses = new SysEnums(groupAttr.Key);
                if (groupAttr2.MyFieldType == FieldType.Enum || groupAttr2.MyFieldType == FieldType.PKEnum)
                {
                    /* ����1 ������2 ����ö������ */
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

                        this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + se.Lab + "&nbsp;(��" + i + "��)</TD></TR>");

                        // ��ʼ����2���顣
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

                            this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + se2.Lab + "&nbsp;(��" + i + "��)</TD></TR>");
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;

                                string style = WebUser.Style;
                                string url = this.GenerEnUrl(en, attrs);
                                this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseover='TROver(this);OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');' onmouseout='TROut(this)' >");
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
                else  /* �����һ��������ö�٣��ڶ���������entities. */
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

                        this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + se.Lab + "&nbsp;(��" + i + "��)</TD></TR>");

                        // ��ʼ����2���顣
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

                            this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + enG2.GetValByKey(groupAttr2.UIRefKeyText) + "&nbsp;(��" + i + "��)</TD></TR>");
                            foreach (Entity en in ens)
                            {
                                if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                    continue;

                                string style = WebUser.Style;
                                string url = this.GenerEnUrl(en, attrs);
                                this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\"  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "');\" onmouseout='TROut(this)' >");
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

            } /* �����жϵ�һ��������ö�����͵������ */


            Entities ensG = BP.En.ClassFactory.GetEns(groupAttr.UIBindKey);
            ensG.RetrieveAll();

            if (groupAttr2.MyFieldType == FieldType.Enum || groupAttr2.MyFieldType == FieldType.PKEnum)
            {
                /*��� 2 ���� ��ö������*/
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

                    this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(��" + i + "��)</TD></TR>");

                    // ��ʼ����2���顣
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

                        this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + se.Lab + "&nbsp;(��" + i + "��)</TD></TR>");
                        foreach (Entity en in ens)
                        {
                            if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                                continue;

                            string style = WebUser.Style;
                            string url = this.GenerEnUrl(en, attrs);
                            this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\"  onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseout='TROut(this)' >");
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

                this.Add("<TR ><TD colspan=" + num + " class='Bar' >" + groupAttr.Desc + ":" + enG.GetValByKey(groupAttr.UIRefKeyText) + "&nbsp;(��" + i + "��)</TD></TR>");

                // ��ʼ����2���顣
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

                    this.Add("<TR><TD colspan=" + num + " class='Bar' >&nbsp;&nbsp;" + groupAttr2.Desc + ":" + enG2.GetValByKey(groupAttr2.UIRefKeyText) + "&nbsp;(��" + i + "��)</TD></TR>");
                    foreach (Entity en in ens)
                    {
                        if (en.GetValStringByKey(groupAttr.Key) != gval || en.GetValStringByKey(groupAttr2.Key) != gval2)
                            continue;

                        string style = WebUser.Style;
                        string url = this.GenerEnUrl(en, attrs);
                        this.Add("<TR class='TR' ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + url + "')\" onmouseover=\"TROver(this,'" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\" onmouseout='TROut(this)' >");
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
                        this.AddTD("��");
                    else
                        this.AddTD("��");
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
        //edited by liuxc,2016-12-23,filterKeyWord:�����ؼ���
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

            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) // ����Ƿ��� enum ���͡�
            {
                BP.Sys.SysEnums eens = new BP.Sys.SysEnums(attr.Key);

                //added by liuxc,2016-3-3,�����ǰö�ٲ鲻�����ݣ����Զ�����һ������Ϊ���޷������ݡ����飬�����·���ʾ��������
                bool noEnum = false;
                if(eens.Count == 0)
                {
                    noEnum = true;
                    eens.AddEntity(new SysEnum{EnumKey = "NoneEnum", Lang = "CH", IntKey = 0, Lab = "�޷�������"});
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
                
                //added by liuxc,2016-3-3,�����ǰ������鲻�����ݣ����Զ�����һ������Ϊ���޷������ݡ��ķ�����󣬴˷�������·���ʾ��������
                bool noGroup = false;
                if (groupEns.Count == 0)
                {
                    noGroup = true;

                    //�����������ʵ��
                    Entity en = groupEns.GetNewEntity;
                    en.SetValByKey(attr.UIRefKeyText, "�޷�������");
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
            // �˴�������ѡ���Ѿ�ѡ�еĲ�������������ӵ��߼��У��˴�����Ҫ��
            // ����ѡ��� ens .
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
        //edited by liuxc,2016-12-23,filterKeyWord:�����ؼ���
        public void UIEn1ToMGroupKey_Line(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal, string groupKey, string filterKeyWord)
        {
            this.EnableViewState = true;
            this.Controls.Clear();
            this.Add("<TABLE class='Table' cellSpacing='1' cellPadding='1'  border='1' width='100%' >");

            Attr attr = ens.GetNewEntity.EnMap.GetAttrByKey(groupKey);
            var val = string.Empty;
            Entity seEn = null;
            string text = null;

            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum) // ����Ƿ��� enum ���͡�
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
            // �˴�������ѡ���Ѿ�ѡ�еĲ�������������ӵ��߼��У��˴�����Ҫ��
            // ����ѡ��� ens .
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
        //edited by liuxc,2016-12-23,filterKeyWord:�����ؼ���
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

            // ����ѡ��� ens .
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

            // ����ѡ��� ens .
            foreach (Entity en in selectedEns)
            {
                string key = en.GetValStringByKey(selecteVal);
                CheckBox bp = (CheckBox)this.FindControl("CB_" + key);
                bp.Checked = true;
            }
        }

        //./././ public void UIEn1ToM_OneLine(Entities ens, string showVal, string showText, Entities selectedEns, string selecteVal)
        //edited by liuxc,2016-12-23,filterKeyWord:�����ؼ���
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

            // ����ѡ��� ens .
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

                #region ����keys
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
                                context += "<TR><TD nowrap class='TDLeft' >" + attr.Desc.Replace("����", "") + "</TD><TD   class='RightTD' >" + en.GetValStringByKey(attr.Key) + "</TD></TR>";
                                //context+="<TR><TD nowrap >"+attr.Desc.Replace("����","")+"</TD><TD nowrap >"+en.GetValStringByKey( attr.Key )+"</TD></TR>";
                                break;
                        }
                    }
                    context += "</TABLE>";
                }

                string img = "<img src='" + en.EnMap.Icon + "'/>";
                if (i == 3)
                {
                    i = 0;
                    this.Add("<TD   valign=top ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\"  >" + img + "&nbsp;<b>" + en.GetValStringByKey(showtext1) + "</b>" + context + "</TD>");
                    this.AddTREnd();
                }
                else
                {
                    this.Add("<TD class='TD' valign=top ondblclick=\"WinOpen( '../Comm/En.htm?EnsName=" + ens.ToString() + "&PK=" + en.GetValByKey(pk) + "')\"  onmousedown=\"OnDGMousedown('" + this.Page.Request.ApplicationPath + "','" + ctrlId + "', '" + clName + "', '" + url + "')\"  >" + img + "&nbsp;<b>" + en.GetValStringByKey(showtext1) + "</b>" + context + "</TD>");
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
        /// �鿴�ļ�
        /// </summary>
        /// <param name="en"></param>
        //		public void FilesView(string enName, string pk)
        public void FilesView(string enName, string pk)
        {
            this.Controls.Clear();
            SysFileManagers ens = new SysFileManagers(enName, pk);
            this.Add("<TABLE BORDER=1>");
            this.Add("<TR>");
            this.Add("<TD>���</TD>");
            this.Add("<TD>�ļ�����</TD>");
            this.Add("<TD>�ϴ���</TD>");
            this.Add("<TD>�ϴ�ʱ��</TD>");
            this.Add("<TD>��С</TD>");
            this.Add("<TD>����</TD>");
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
                    this.Add("<TD><a href=\"javascript:DoAction('FileManager.aspx?OID=" + file.OID + "&EnsName=" + enName + "&PK=" + pk + "','��Ҫɾ�� ��" + file.MyFileName + "��')\" >ɾ��</a></TD>");
                }
                else
                {
                    this.Add("<TD>��</TD>");
                }
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // �ڴ˴������û������Գ�ʼ��ҳ��
        }

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		�����֧������ķ��� - ��Ҫʹ�ô���༭��
        ///		�޸Ĵ˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}

