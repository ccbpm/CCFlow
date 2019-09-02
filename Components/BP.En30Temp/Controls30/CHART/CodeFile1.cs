using System;
using System.IO;//用于文件存取 
using System.Data;//用于数据访问 
using System.Drawing;//提供画GDI+图形的基本功能 
using System.Drawing.Text;//提供画GDI+图形的高级功能 
using System.Drawing.Drawing2D;//提供画高级二维，矢量图形功能 
using System.Drawing.Imaging;//提供画GDI+图形的高级功能 
using BP.Sys;

namespace BP.Web.Controls
{
    public class PieChart
    {
        public PieChart()
        {
        }
        public void Render(string title, string subTitle, int width, int height, DataTable dt)
        {
            const int SIDE_LENGTH = 400;
            const int PIE_DIAMETER = 200;

            //通过输入参数，取得饼图中的总基数 
            float sumData = 0;
            foreach (DataRow dr in dt.Rows)
            {
                sumData += Convert.ToSingle(dr[1]);
            }
            //产生一个image对象，并由此产生一个Graphics对象 
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            //设置对象g的属性 
            g.ScaleTransform((Convert.ToSingle(width)) / SIDE_LENGTH, (Convert.ToSingle(height)) / SIDE_LENGTH);
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //画布和边的设定 
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, 0, 0, SIDE_LENGTH - 1, SIDE_LENGTH - 1);
            //画饼图标题 
            g.DrawString(title, new Font("Tahoma", 24), Brushes.Black, new PointF(5, 5));
            //画饼图的图例 
            g.DrawString(subTitle, new Font("Tahoma", 14), Brushes.Black, new PointF(7, 35));
            //画饼图 
            float curAngle = 0;
            float totalAngle = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                curAngle = Convert.ToSingle(dt.Rows[i][1]) / sumData * 360;

                g.FillPie(new SolidBrush(ChartUtil.GetChartItemColor(i)), 100, 65, PIE_DIAMETER, PIE_DIAMETER, totalAngle, curAngle);
                g.DrawPie(Pens.Black, 100, 65, PIE_DIAMETER, PIE_DIAMETER, totalAngle, curAngle);
                totalAngle += curAngle;
            }
            //画图例框及其文字 
            g.DrawRectangle(Pens.Black, 200, 300, 199, 99);
            g.DrawString("Legend", new Font("Tahoma", 12, FontStyle.Bold), Brushes.Black, new PointF(200, 300));

            //画图例各项 
            PointF boxOrigin = new PointF(210, 330);
            PointF textOrigin = new PointF(235, 326);
            float percent = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                g.FillRectangle(new SolidBrush(ChartUtil.GetChartItemColor(i)), boxOrigin.X, boxOrigin.Y, 20, 10);
                g.DrawRectangle(Pens.Black, boxOrigin.X, boxOrigin.Y, 20, 10);
                percent = Convert.ToSingle(dt.Rows[i][1]) / sumData * 100;
                g.DrawString(dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1].ToString() + " (" + percent.ToString("0") + "%)", new Font("Tahoma", 10), Brushes.Black, textOrigin);
                boxOrigin.Y += 15;
                textOrigin.Y += 15;
            }
            //通过Response.OutputStream，将图形的内容发送到浏览器 
            string file = SystemConfig.PathOfTemp + "Pie" + Web.WebUser.No + ".gif";
            bm.Save(file, ImageFormat.Gif);
            //回收资源 
            bm.Dispose();
            g.Dispose();
        }
    }

    //画条形图 
    public class BarChart
    {
        public BarChart()
        {
        }
        public void Render(string title, string subTitle, int width, int height, DataTable dt, Stream target)
        {
            const int SIDE_LENGTH = 400;
            const int CHART_TOP = 75;
            const int CHART_HEIGHT = 200;
            const int CHART_LEFT = 50;
            const int CHART_WIDTH = 300;
           // DataTable dt = chartData.Tables[0];

            //计算最高的点 
            float highPoint = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (highPoint < Convert.ToSingle(dr[1]))
                {
                    highPoint = Convert.ToSingle(dr[1]);
                }
            }
            //建立一个Graphics对象实例 
            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);
            //设置条图图形和文字属性 
            g.ScaleTransform((Convert.ToSingle(width)) / SIDE_LENGTH, (Convert.ToSingle(height)) / SIDE_LENGTH);
            g.SmoothingMode = SmoothingMode.Default;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //设定画布和边 
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Black, 0, 0, SIDE_LENGTH - 1, SIDE_LENGTH - 1);
            //画大标题 
            g.DrawString(title, new Font("Tahoma", 24), Brushes.Black, new PointF(5, 5));
            //画小标题 
            g.DrawString(subTitle, new Font("Tahoma", 14), Brushes.Black, new PointF(7, 35));
            //画条形图 
            float barWidth = CHART_WIDTH / (dt.Rows.Count * 2);
            PointF barOrigin = new PointF(CHART_LEFT + (barWidth / 2), 0);
            float barHeight = dt.Rows.Count;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                barHeight = Convert.ToSingle(dt.Rows[i][1]) * 200 / highPoint;
                barOrigin.Y = CHART_TOP + CHART_HEIGHT - barHeight;
                g.FillRectangle(new SolidBrush(ChartUtil.GetChartItemColor(i)), barOrigin.X, barOrigin.Y, barWidth, barHeight);
                barOrigin.X = barOrigin.X + (barWidth * 2);
            }
            //设置边 
            g.DrawLine(new Pen(Color.Black, 2), new Point(CHART_LEFT, CHART_TOP), new Point(CHART_LEFT, CHART_TOP + CHART_HEIGHT));
            g.DrawLine(new Pen(Color.Black, 2), new Point(CHART_LEFT, CHART_TOP + CHART_HEIGHT), new Point(CHART_LEFT + CHART_WIDTH, CHART_TOP + CHART_HEIGHT));
            //画图例框和文字 
            g.DrawRectangle(new Pen(Color.Black, 1), 200, 300, 199, 99);
            g.DrawString("Legend", new Font("Tahoma", 12, FontStyle.Bold), Brushes.Black, new PointF(200, 300));

            //画图例 
            PointF boxOrigin = new PointF(210, 330);
            PointF textOrigin = new PointF(235, 326);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                g.FillRectangle(new SolidBrush(ChartUtil.GetChartItemColor(i)), boxOrigin.X, boxOrigin.Y, 20, 10);
                g.DrawRectangle(Pens.Black, boxOrigin.X, boxOrigin.Y, 20, 10);
                g.DrawString(dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1].ToString(), new Font("Tahoma", 10), Brushes.Black, textOrigin);
                boxOrigin.Y += 15;
                textOrigin.Y += 15;
            }
            //输出图形 
            bm.Save(BP.Sys.SystemConfig.PathOfTemp+"Bar"+BP.Web.WebUser.No+".gif", ImageFormat.Gif);

            //资源回收 
            bm.Dispose();
            g.Dispose();
        }
    }
    public class ChartUtil
    {
        public ChartUtil()
        {
        }
        public static Color GetChartItemColor(int itemIndex)
        {
            Color selectedColor;
            switch (itemIndex)
            {
                case 0:
                    selectedColor = Color.Blue;
                    break;
                case 1:
                    selectedColor = Color.Red;
                    break;
                case 2:
                    selectedColor = Color.Yellow;
                    break;
                case 3:
                    selectedColor = Color.Purple;
                    break;
                default:
                    selectedColor = Color.Green;
                    break;
            }
            return selectedColor;
        }
    }
}
