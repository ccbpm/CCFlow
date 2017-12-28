using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InfoSoftGlobal;
using System.Text;
using System.Data;
using BP.DA;

namespace CCFlow.SDKFlowDemo.BPFramework.Chart
{
    public partial class DemoEmps : System.Web.UI.Page
    {
        private string exportHeader
        {
            get
            {
                return "exportEnabled='1' exportAtClient='0'" +
                       " exportAction='download' " +
                       "exportHandler='../../../WF/Comm/Charts/FCExport.aspx' " +
                       "exportDialogMessage='正在生成,请稍候...'  " +
                       " exportFormats='PNG=生成PNG图片|JPG=生成JPG图片|PDF=生成PDF文件'";
            }
        }
        private StringBuilder sBuilder = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            string chartPath = "";//需要什么组件swf
            sBuilder = new StringBuilder();
            string sql = "SELECT COUNT(a.FK_Dept)  Num,b.Name" +
                         " deptName FROM PORT_EMP a,Port_Dept b " +
                         "where a.FK_Dept=b.No GROUP BY B.Name";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //饼状图 3D
            sBuilder.Append("<chart " + exportHeader);
            sBuilder.Append(" caption='饼状图 3D' palette='2' animation='1' ");
            sBuilder.Append(" YAxisName='Sales Achieved' showValues='0'     ");
            sBuilder.Append(" numberPrefix='$' formatNumberScale='0' ");
            sBuilder.Append(" showPercentInToolTip='0' showLabels='0' showLegend='1'> ");

            foreach (DataRow dr in dt.Rows)
            {
                sBuilder.Append(" <set label='" + dr["deptName"].ToString() +
                    "' value='" + dr["Num"] + "' isSliced='0' /> ");
            }
            sBuilder.Append(" <styles> ");
            sBuilder.Append(" <definition> ");
            sBuilder.Append(" <style type='font' name='CaptionFont' color='666666' size='15' /> ");
            sBuilder.Append(" <style type='font' name='SubCaptionFont' bold='0' /> ");
            sBuilder.Append(" </definition> ");
            sBuilder.Append(" <application> ");
            sBuilder.Append(" <apply toObject='caption' styles='CaptionFont' /> ");
            sBuilder.Append(" <apply toObject='SubCaption' styles='SubCaptionFont' /> ");
            sBuilder.Append(" </application> ");
            sBuilder.Append(" </styles> ");
            sBuilder.Append(" </chart>");

            chartPath = "../../../WF/Comm/Charts/swf/Pie3D.swf";

            Literal1.Text = FusionCharts.RenderChart(chartPath, "", sBuilder.ToString(), "Pie_3D",
                "700", "500", false, true);

            //Line_3D
            sBuilder = new StringBuilder();
            int yAxisMaxValue = 0;

            foreach (DataRow dr in dt.Rows)
            {
                int setValue = int.Parse(dr["Num"].ToString());
                if (setValue > yAxisMaxValue)
                    yAxisMaxValue = setValue;


                sBuilder.Append("<set label='" + dr["deptName"].ToString() +
                    "' value='" + setValue + "' />");
            }

            sBuilder.Append(" <styles>");
            sBuilder.Append(" <definition>");
            sBuilder.Append(" <style name='Anim1' type='animation' param='_alpha' start='0' duration='1' />");
            sBuilder.Append(" </definition>");
            sBuilder.Append(" <application>");
            sBuilder.Append(" <apply toObject='TRENDLINES' styles='Anim1' />");
            sBuilder.Append(" </application>");
            sBuilder.Append(" </styles>");
            sBuilder.Append(" </chart> ");

            yAxisMaxValue += 10;
            sBuilder.Insert(0, " <chart " + exportHeader
                             + " caption='线形图_3D'  xAxisName='部门' yAxisMaxValue='"
                             +  yAxisMaxValue + "' bgColor='91AF46,FFFFFF' "
                             + " divLineColor='91AF46' divLineAlpha='30' alternateHGridAlpha='5' "
                             + " canvasBorderColor='666666'  "
                             + " baseFontColor='666666' lineColor='91AF46' numVDivlines='7'"
                             + " showAlternateVGridColor='1' anchorSides='3' anchorRadius='5' showValues='0'>");

            chartPath = "../../../WF/Comm/Charts/swf/Line.swf";
            Literal2.Text = FusionCharts.RenderChart(chartPath, "", sBuilder.ToString(), "Line_3D",
               "700", "500", false, true);


            //Col_3D
            sBuilder = new StringBuilder();

            yAxisMaxValue = 0;
            foreach (DataRow dr in dt.Rows)
            {
                int setValue = int.Parse(dr["Num"].ToString());
                if (setValue > yAxisMaxValue)
                    yAxisMaxValue = setValue;

                sBuilder.Append("<set label='" + dr["deptName"].ToString() +
                    "' value='" + setValue + "' />");
            }
            sBuilder.Append("</chart>");

            yAxisMaxValue += 10;

            sBuilder.Insert(0, "<chart " + exportHeader + " subcaption='柱状图_3D' formatNumberScale='0'"
            + " divLineAlpha='20' divLineColor='CC3300'"
            + " alternateHGridColor='CC3300' shadowAlpha='40' numvdivlines='9'"
            + "  bgColor='FFFFFF,CC3300' bgAngle='270'"
            + " bgAlpha='10,10' alternateHGridAlpha='5' yAxisMaxValue ='" + yAxisMaxValue + "'>");


            chartPath = "../../../WF/Comm/Charts/swf/Column3D.swf";

            Literal3.Text = FusionCharts.RenderChart(chartPath, "", sBuilder.ToString(), "Col_3D",
             "700", "500", false, true);
        }

    }
}