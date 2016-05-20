using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.WF.Data;
using System.Text;
using BP.En;
using System.Data;
using BP.DA;
using System.Collections;

namespace CCFlow.WF.CH1234
{
    public partial class FlowCH : System.Web.UI.Page
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(WebUser.No))//没有登录信息
                return;

            string method = string.Empty;

            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                //加载图形
                case "empChart":
                    s_responsetext = empChart();
                    break;
                case "deptChart":
                    s_responsetext = deptChart();
                    break;
                case "allDeptChart":
                    s_responsetext = allDeptChart();
                    break;

            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        /// <summary>
        /// 加载我的工作图形
        /// </summary>
        /// <returns></returns>
        private string empChart()
        {
            DateTime dTime = DateTime.Now.AddMonths(-1);
            int daysCount = DateTime.DaysInMonth(dTime.Year, dTime.Month);

            DateTime dtFirst = dTime.AddDays(-(dTime.Day) + 1);

            StringBuilder sb = new StringBuilder();

            sb.Append("<categories >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sb.Append("<category label='" + dtFirst.ToString("MM-dd") + "' />");
                }
                DateTime dtNextTime = dtFirst.AddDays(i);
                sb.Append("<category label='" + dtNextTime.ToString("MM-dd") + "' />");
            }
            sb.Append("</categories>");


            sb.Append("<dataset seriesName='按期完成数' color='F6BD0F' anchorBorderColor='F6BD0F' >");

            int maxValue = 0;
            string sql = "";
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                          "' and DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='1'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                         "' and DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='1'";

                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append("<dataset seriesName='逾期完成数' color='FF0000' anchorBorderColor='FF0000' >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                          "' and DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='2'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                         "' and DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='2'";

                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append(" </chart>\"]}");

            if (maxValue == 0)
            {
                maxValue = 100;
            }
            sb.Insert(0, "{set_XML:[\"<chart numdivlines='9' lineThickness='2' yAxisMaxValue='" + maxValue + "' showValues='0' numVDivLines='22' formatNumberScale='1'" + "labelDisplay='ROTATE' slantLabels='1' anchorRadius='2' anchorBgAlpha='50' showAlternateVGridColor='1' anchorAlpha='100' animation='1' "
           + "limitsDecimalPrecision='0' divLineDecimalPrecision='1'>");
            return sb.ToString();
        }
        /// <summary>
        /// 加载我部门图形
        /// </summary>
        /// <returns></returns>
        private string deptChart()
        {
            
            DateTime dTime = DateTime.Now.AddMonths(-1);
            int daysCount = DateTime.DaysInMonth(dTime.Year, dTime.Month);

            DateTime dtFirst = dTime.AddDays(-(dTime.Day) + 1);

            StringBuilder sb = new StringBuilder();


            sb.Append("<categories >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sb.Append("<category label='" + dtFirst.ToString("MM-dd") + "' />");
                }
                DateTime dtNextTime = dtFirst.AddDays(i);
                sb.Append("<category label='" + dtNextTime.ToString("MM-dd") + "' />");
            }
            sb.Append("</categories>");


            sb.Append("<dataset seriesName='按期完成数' color='F6BD0F' anchorBorderColor='F6BD0F' >");

            int maxValue = 0;
            string sql = "";
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                          "' and DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='1' and fk_dept='" + WebUser.FK_Dept + "'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                         "' and DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='1' and fk_dept='" + WebUser.FK_Dept + "'";
                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append("<dataset seriesName='逾期完成数' color='FF0000' anchorBorderColor='FF0000' >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                          "' and DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='2'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                         "' and DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='2'";

                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append(" </chart>\"]}");

            if (maxValue == 0)
            {
                maxValue = 100;
            }
            sb.Insert(0, "{set_XML:[\"<chart numdivlines='9' lineThickness='2' yAxisMaxValue='" + maxValue + "' showValues='0' numVDivLines='22' formatNumberScale='1'" + "labelDisplay='ROTATE' slantLabels='1' anchorRadius='2' anchorBgAlpha='50' showAlternateVGridColor='1' anchorAlpha='100' animation='1' "
           + "limitsDecimalPrecision='0' divLineDecimalPrecision='1'>");
            return sb.ToString();
        }
        /// <summary>
        /// 加载全单位图形
        /// </summary>
        /// <returns></returns>
        private string allDeptChart()
        {
            DateTime dTime = DateTime.Now.AddMonths(-1);
            int daysCount = DateTime.DaysInMonth(dTime.Year, dTime.Month);

            DateTime dtFirst = dTime.AddDays(-(dTime.Day) + 1);

            StringBuilder sb = new StringBuilder();

            sb.Append("<categories >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sb.Append("<category label='" + dtFirst.ToString("MM-dd") + "' />");
                }
                DateTime dtNextTime = dtFirst.AddDays(i);
                sb.Append("<category label='" + dtNextTime.ToString("MM-dd") + "' />");
            }
            sb.Append("</categories>");


            sb.Append("<dataset seriesName='按期完成数' color='F6BD0F' anchorBorderColor='F6BD0F' >");

            int maxValue = 0;
            string sql = "";
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='1'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where  DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='1'";

                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append("<dataset seriesName='逾期完成数' color='FF0000' anchorBorderColor='FF0000' >");
            for (int i = 1; i < daysCount; i++)
            {
                if (i == 1)
                {
                    sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                          "' and DTFrom like '" + dtFirst.ToString("yyyy-MM-dd") + "%' and chsta='2'";
                    sb.Append("<set value='" + DBAccess.RunSQLReturnCOUNT(sql) + "' />");
                }

                DateTime dtNextTime = dtFirst.AddDays(i);
                sql = "select * from wf_ch where fk_emp ='" + WebUser.No +
                         "' and DTFrom like '" + dtNextTime.ToString("yyyy-MM-dd") + "%' and chsta='2'";

                int rowsCount = DBAccess.RunSQLReturnCOUNT(sql);
                if (rowsCount > maxValue)
                {
                    maxValue = rowsCount;
                }
                sb.Append("<set value='" + rowsCount + "' />");
            }
            sb.Append("</dataset>");

            sb.Append(" </chart>\"]}");

            if (maxValue == 0)
            {
                maxValue = 100;
            }
            sb.Insert(0, "{set_XML:[\"<chart numdivlines='9' lineThickness='2' yAxisMaxValue='" + maxValue + "' showValues='0' numVDivLines='22' formatNumberScale='1'" + "labelDisplay='ROTATE' slantLabels='1' anchorRadius='2' anchorBgAlpha='50' showAlternateVGridColor='1' anchorAlpha='100' animation='1' "
           + "limitsDecimalPrecision='0' divLineDecimalPrecision='1'>");
            return sb.ToString();
        }
    }
}