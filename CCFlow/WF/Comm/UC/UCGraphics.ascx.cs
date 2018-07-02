namespace BP.Web.UC
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Data.SqlClient;
	using System.Data.Odbc ;
	using System.Drawing;
	using System.Web;
	using System.Web.SessionState;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using BP.En;
	using BP.DA;
    using BP.Sys;
    //using Microsoft.Office;
    //using Microsoft.Office.Interop;
    //using Microsoft.Web.UI.WebControls;
  //  using Microsoft.Office.Interop.Owc11;
    //using OWC10;

	/// <summary>
	///		UCGraphics 的摘要说明。
	/// </summary>
    public partial class UCGraphics : UCBase
    {

        #region 3 d
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colOfGroupField"></param>
        /// <param name="colOfGroupName"></param>
        /// <param name="colOfNumField"></param>
        /// <param name="colOfNumName"></param>
        /// <param name="title"></param>
        /// <param name="chartHeight"></param>
        /// <param name="chartWidth"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static string GenerChart(DataTable dt, string colOfGroupField, string colOfGroupName,
            string colOfNumField, string colOfNumName, string title, int chartHeight, int chartWidth, ChartType ct)
        {
            //string strCategory = ""; // "1" + '\t' + "2" + '\t' + "3" + '\t'+"4" + '\t' + "5" + '\t' + "6" + '\t';
            //string strValue = ""; // "9" + '\t' + "8" + '\t' + "4" + '\t'+"10" + '\t' + "12" + '\t' + "6" + '\t';
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
            //        ThisChChart.PlotArea.Interior.Color = "red";
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

            //导出图像文件
            //ThisChart.ExportPicture("G:\\chart.gif","gif",600,350);

            string fileName = ct.ToString() + PubClass.GenerTempFileName("GIF");
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
        /// 产生2纬度的表
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colOfGroupField1"></param>
        /// <param name="colOfGroupName1"></param>
        /// <param name="colOfGroupField2"></param>
        /// <param name="colOfGroupName2"></param>
        /// <param name="colOfNumField"></param>
        /// <param name="colOfNumName"></param>
        /// <param name="title"></param>
        /// <param name="chartHeight"></param>
        /// <param name="chartWidth"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static string GenerChart2D(DataTable dt, string colOfGroupField1, string colOfGroupName1,
            string colOfGroupField2, string colOfGroupName2,
            string colOfNumField, string colOfNumName, string title, int chartHeight, int chartWidth, ChartType ct)
        {
            //string strCategory = ""; // "1" + '\t' + "2" + '\t' + "3" + '\t'+"4" + '\t' + "5" + '\t' + "6" + '\t';
            //string strValue = ""; // "9" + '\t' + "8" + '\t' + "4" + '\t'+"10" + '\t' + "12" + '\t' + "6" + '\t';

            ////声明对象
            //ChartSpace ThisChart = new ChartSpaceClass();
            //ChChart ThisChChart = ThisChart.Charts.Add(0);
            ////ChSeries ThisChSeries = ThisChChart.SeriesCollection.Add(0);

            ////显示图例
            //ThisChChart.HasLegend = true;
            ////标题
            //ThisChChart.HasTitle = true;
            //ThisChChart.Title.Caption = title;

            ////给定x,y轴图示说明
            //ThisChChart.Axes[0].HasTitle = true;
            //ThisChChart.Axes[1].HasTitle = true;

            ////			ThisChChart.Axes[0].Title.Caption = colOfGroupName1;
            ////			ThisChChart.Axes[1].Title.Caption = colOfNumName;

            //switch (ct)
            //{
            //    case ChartType.Histogram:
            //        ThisChChart.Type = ChartChartTypeEnum.chChartTypeColumnClustered;
            //        DataTable dtC = dt.Clone();
            //        int j = -1;
            //        foreach (DataRow dr1 in dtC.Rows)
            //        {
            //            j++;
            //            ChSeries ThisChSeries = ThisChChart.SeriesCollection.Add(j);
            //            ThisChChart.SeriesCollection[j].DataLabelsCollection.Add();
            //            //给定series的名字
            //            ThisChChart.SeriesCollection[j].SetData(ChartDimensionsEnum.chDimSeriesNames,
            //                (int)ChartSpecialDataSourcesEnum.chDataLiteral, dr1[colOfGroupField1].ToString());

            //            strCategory = "";
            //            strValue = "";
            //            foreach (DataRow dr in dt.Rows)
            //            {
            //                if (dr1[colOfGroupField1].Equals(dr[colOfGroupField1]) == false)
            //                    continue;

            //                strCategory += dr[colOfGroupField1].ToString() + '\t' + dr[colOfGroupField2].ToString() + '\t';
            //                strValue += dr[colOfNumField].ToString() + '\t';
            //            }

            //            //给定分类
            //            ThisChSeries.SetData(ChartDimensionsEnum.chDimCategories,
            //                (int)ChartSpecialDataSourcesEnum.chDataLiteral, strCategory);
            //            //给定值
            //            ThisChSeries.SetData
            //                (ChartDimensionsEnum.chDimValues,
            //                (int)ChartSpecialDataSourcesEnum.chDataLiteral, strValue);
            //        }
            //        //					ThisChChart.Overlap = 50;
            //        //					//旋转
            //        //					ThisChChart.Rotation  = 360;
            //        //					ThisChChart.Inclination = 10;
            //        //					//背景颜色
            //        //					ThisChChart.PlotArea.Interior.Color = "red";
            //        //					//底色
            //        //					ThisChChart.PlotArea.Floor.Interior.Color = "green";
            //        ////给定series的名字
            //        //ThisChSeries.SetData(ChartDimensionsEnum.chDimSeriesNames,ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),"ssdd" );
            //        //给定分类
            //        //ThisChSeries.SetData(ChartDimensionsEnum.chDimCategories,ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strCategory);
            //        //给定值
            //        //ThisChSeries.SetData(ChartDimensionsEnum.chDimValues,ChartSpecialDataSourcesEnum.chDataLiteral.GetHashCode(),strValue);
            //        break;
            //    case ChartType.Pie:
            //        // 产生数据
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            strCategory += dr[colOfGroupField1].ToString() + '\t';
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
            //            strCategory += dr[colOfGroupField1].ToString() + '\t';
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


            //导出图像文件
            //ThisChart.ExportPicture("G:\\chart.gif","gif",600,350);

            string fileName = ct.ToString() + PubClass.GenerTempFileName("GIF");
            string strAbsolutePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\Temp\\" + fileName;

#warning 注释掉了。
            //try
            //{
            //    ThisChart.ExportPicture(strAbsolutePath, "GIF", chartWidth, chartHeight);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("@不能创建文件,可能是权限的问题，请把该目录设置为任何人都可以修改。" + strAbsolutePath + " Exception:" + ex.Message);
            //}
            return fileName;
            //
            //
            //			//创建GIF文件的相对路径. 
            //			string strRelativePath = "./Temp/"+fileName;
            //
            //			//把图片添加到placeholder.  onmousedown=\"CellDown('Cell')\"
            //			//string strImageTag = "<IMG SRC='../../Temp/" + fileName + "'  />"; 
            //			return strRelativePath ;
        }
        #endregion

        #region 2纬度图形
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
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
