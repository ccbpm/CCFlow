using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Text;
using System.Data;
using BP.DA;

namespace CCFlow.WF.Admin.CCBPMDesigner.App.OneFlow
{
    public partial class NodesDtlEmps : System.Web.UI.Page
    {
        #region
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 节点
        /// </summary>
        private Node node
        {
            get
            {
                BP.WF.Node nd = new BP.WF.Node(this.getUTF8ToString("FK_Node"));
                return nd;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        private Flow flow
        {
            get
            {
                BP.WF.Flow fl = new BP.WF.Flow(node.FK_Flow);
                return fl;
            }
        }
        private string tabIndex
        {
            get
            {
                return getUTF8ToString("tabIndex");
            }
        }
        private string tkTable
        {
            get
            {
                return "ND" + int.Parse(node.FK_Flow) + "Track";
            }
        }
        private string dbstr
        {
            get
            {
                return BP.Sys.SystemConfig.AppCenterDBVarStr;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;

            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();

            switch (method)
            {
                case "loadCaption":
                    s_responsetext = LoadCaption();
                    break;
                case "loadTabData":
                    s_responsetext = LoadTabData();
                    break;
                case "loadFirstTabColChart":
                    s_responsetext = GetFirstTabColChartData();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        private StringBuilder sBuilder = null;
        private string LoadCaption()
        {
            return "流程：" + flow.Name + " 节点：" + node.Name + "";
        }
        private string LoadTabData()
        {
            switch (tabIndex)
            {
                case "0":
                    return TabIndex_0();
                case "1":
                    break;
                case "2":
                    break;
                default:
                    break;
            }
            return "";
        }
        /// <summary>
        /// 加载第一个面板二维表
        /// </summary>
        /// <returns></returns>
        private string TabIndex_0()
        {
            this.sBuilder = new StringBuilder();

            string sql = "";
            DataTable dt = new DataTable();

            sql = "SELECT distinct EmpFrom,EmpFromT FROM ND" + int.Parse(flow.No) +
                "Track WHERE NDFrom='" + node.NodeID + "'";

            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            this.sBuilder.Append("<table id='tab_0' style=\"width: 100%; min-width: 800px;\">");

            this.sBuilder.Append("<tr>");
            this.sBuilder.Append("<th class=\"center\" rowspan=\"2\">");
            this.sBuilder.Append("序");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\" rowspan=\"2\">");
            this.sBuilder.Append("人员");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\" colspan=\"3\">");
            this.sBuilder.Append("工作分析");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\" colspan=\"4\">");
            this.sBuilder.Append("按月份分析");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\" colspan=\"4\">");
            this.sBuilder.Append("按周分析");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("</tr>");
            this.sBuilder.Append("<tr>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("工作总数");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("待处理");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("退回次数");
            this.sBuilder.Append("</th>");

            //改为上月与上上月之间的比较，周类似     周要求

            DateTime dTime = DateTime.Now;

            //上上月
            string beforeLastMouth = dTime.AddMonths(-2).ToString("yyyy-MM");
            string lastMouth = dTime.AddMonths(-1).ToString("yyyy-MM");


            //上上周的开始，截止日期
            string beforeLastWeekFDay = DataType.WeekOfMonday(DataType.WeekOfMonday(dTime).AddDays(-9)).ToString("yyyy-MM-dd");
            string beforeLastWeekEndDay = DataType.WeekOfMonday(DataType.WeekOfMonday(dTime).AddDays(-9)).AddDays(6).ToString("yyyy-MM-dd");

            //上周的开始，截止日期
            string lastWeekFDay = DataType.WeekOfMonday(DataType.WeekOfMonday(dTime).AddDays(-3)).ToString("yyyy-MM-dd");
            string lastWeekEndDay = DataType.WeekOfMonday(DataType.WeekOfMonday(dTime).AddDays(-3)).AddDays(6).ToString("yyyy-MM-dd");

            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append(beforeLastMouth);
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append(lastMouth);
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("同比");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("同比增长");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("上上周</br>" + beforeLastWeekFDay + "</br>" + beforeLastWeekEndDay);
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("上周</br>" + lastWeekFDay + "</br>" + lastWeekEndDay);
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("同比");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("<th class=\"center\">");
            this.sBuilder.Append("同比增长");
            this.sBuilder.Append("</th>");
            this.sBuilder.Append("</tr>");

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

                this.sBuilder.Append("<tr>");
                this.sBuilder.Append("<td class=\"Idx\">");
                this.sBuilder.Append(i + 1);
                this.sBuilder.Append("</td>");
                this.sBuilder.Append("<td class=\"center\">");
                this.sBuilder.Append(dt.Rows[i]["EmpFromT"]);
                this.sBuilder.Append("</td>");
                this.sBuilder.Append("<td class=\"center\">");
                this.sBuilder.Append(sumCount);
                this.sBuilder.Append("</td>");
                this.sBuilder.Append("<td class=\"center\">");

                if (todoNum == 0)
                    this.sBuilder.Append(todoNum);
                else
                    this.sBuilder.Append("<font style=\"color: Green;\"><b>" + todoNum + "</b></font>");

                this.sBuilder.Append("</td>");
                this.sBuilder.Append("<td class=\"center\">");
                if (returnCount == 0)
                    this.sBuilder.Append(returnCount);
                else
                    this.sBuilder.Append("<font style=\"color: Red;\"><b>" + returnCount + "</b></font>");

                this.sBuilder.Append("</td>");


                //上上月
                this.sBuilder.Append("<td class=\"center\">");

                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                    "NDFrom AND RDT LIKE'%" + beforeLastMouth + "%'";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                int llastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //if (llastCount == 0)
                //    this.sBuilder.Append("<font style=\"color: Red;\"><b>" + llastCount + "</b></font>");
                //else
                this.sBuilder.Append(llastCount);

                this.sBuilder.Append("</td>");

                //上月
                this.sBuilder.Append("<td class=\"center\">");

                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                    "NDFrom AND RDT LIKE'%" + lastMouth + "%'";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                int lastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                //if (lastCount == 0)
                //    this.sBuilder.Append("<font style=\"color: Red;\"><b>" + lastCount + "</b></font>");
                //else
                this.sBuilder.Append(lastCount);

                this.sBuilder.Append("</td>");

                //按月  同比 红字标示小于零的列
                this.sBuilder.Append("<td class=\"center\">");

                if (lastCount - llastCount <= 0)
                    this.sBuilder.Append("<font style=\"color: Red;\"><b>" + (lastCount - llastCount) + "</b></font>");
                else
                    this.sBuilder.Append("<font style=\"color: Green;\"><b>" + (lastCount - llastCount) + "</b></font>");

                this.sBuilder.Append("</td>");

                //按月百分比
                this.sBuilder.Append("<td class=\"center\">");

                double bl;//比率

                if (lastCount == 0)//
                    this.sBuilder.Append("-");
                else
                {
                    //比率
                    bl = (lastCount - llastCount) * 100.0 / lastCount;

                    if (bl < 0)
                        this.sBuilder.Append("<font style=\"color: Red;\"><b>" + bl.ToString("0.00") + "%</b></font>");
                    if (bl > 0)
                        this.sBuilder.Append("<font style=\"color: Green;\"><b>" + bl.ToString("0.00") + "%</b></font>");

                    if (lastCount == llastCount)
                        this.sBuilder.Append("-");
                }
                this.sBuilder.Append("</td>");

                //上上周
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                    "NDFrom AND RDT >='" + beforeLastWeekFDay + "' AND RDT<='" +
                     beforeLastWeekEndDay + "'";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);
                llastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);

                this.sBuilder.Append("<td class=\"center\">");
                this.sBuilder.Append(llastCount);
                this.sBuilder.Append("</td>");

                //上周
                this.sBuilder.Append("<td class=\"center\">");
                ps = new BP.DA.Paras();
                ps.SQL = "SELECT COUNT(distinct WorkID)  FROM  " + tkTable +
                    " WHERE EmpFrom=" + dbstr + "EmpFrom AND NDFrom=" + dbstr +
                    "NDFrom AND RDT >='" + lastWeekFDay + "' AND RDT<='" +
                   lastWeekEndDay + "'";
                ps.Add(BP.WF.TrackAttr.EmpFrom, dt.Rows[i]["EmpFrom"].ToString());
                ps.Add(BP.WF.TrackAttr.NDFrom, node.NodeID);

                lastCount = BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                this.sBuilder.Append(lastCount);
                this.sBuilder.Append("</td>");

                //按周  同比 红字标示小于零的列
                this.sBuilder.Append("<td class=\"center\">");
                if (lastCount - llastCount <= 0)
                    this.sBuilder.Append("<font style=\"color: Red;\"><b>" + (lastCount - llastCount) + "</b></font>");
                else
                    this.sBuilder.Append("<font style=\"color: Green;\"><b>" + (lastCount - llastCount) + "</b></font>");

                this.sBuilder.Append("</td>");

                //按周百分比
                this.sBuilder.Append("<td class=\"center\">");

                if (lastCount == 0)//
                    this.sBuilder.Append("-");
                else
                {
                    bl = (lastCount - llastCount) * 100.0 / lastCount;

                    if (bl < 0)
                        this.sBuilder.Append("<font style=\"color: Red;\"><b>" + bl.ToString("0.00") + "%</b></font>");
                    if (bl > 0)
                        this.sBuilder.Append("<font style=\"color: Green;\"><b>" + bl.ToString("0.00") + "%</b></font>");
                    if (lastCount == llastCount)
                        this.sBuilder.Append("-");
                }
                this.sBuilder.Append("</td>");
                this.sBuilder.Append("</tr>");
            }//循环

            this.sBuilder.Append("<tr><td colspan='13' class='center td_chart'>工作总量统计图</td></tr>");
            this.sBuilder.Append("</table>");
            return this.sBuilder.ToString();
        }
        /// <summary>
        /// 加载一个tab chart
        /// </summary>
        /// <returns></returns>
        private string GetFirstTabColChartData()
        {
            sBuilder = new StringBuilder();

            string sql = "SELECT distinct EmpFrom,EmpFromT FROM ND" + int.Parse(flow.No) +
                "Track WHERE NDFrom='" + node.NodeID + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            Paras ps;
            int maxValue = 0;
            int setCount = 0;
            if (dt.Rows.Count <= 4)//使用MSCOL图形 取最近一年的数据
            {
                DateTime dTime = DateTime.Now;

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


                foreach (DataRow dr in dt.Rows)
                {
                    sBuilder.Append("<dataset seriesName='" + dr["EmpFromT"] + "'>");

                    foreach (string lm in listMouth)
                    {
                        ps = new BP.DA.Paras();
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
                sBuilder.Insert(0, "<chart baseFontSize='14'  subcaption='" + "节点:[" + node.NodeID + "]" + node.Name +
                                  "-" + listMouth[0] + "至" + listMouth[11] + "统计" + "' formatNumberScale='0' divLineAlpha='20'" +
                                  " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                                  " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                                  " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue + "'>");
            }
            else
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ps = new BP.DA.Paras();
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
                sBuilder.Insert(0, "<chart baseFontSize='12' subcaption='" + "节点:[" + node.NodeID + "]" + node.Name +
                                   "-总数统计" + "' formatNumberScale='0' divLineAlpha='20'" +
                                   " divLineColor='CC3300' alternateHGridColor='CC3300' shadowAlpha='40'" +
                                   " numvdivlines='9'  bgColor='FFFFFF,CC3300' bgAngle='270' bgAlpha='10,10'" +
                                   " alternateHGridAlpha='5'   yAxisMaxValue ='" + maxValue + "'>");
            }

            return "{rowsCount:\"" + dt.Rows.Count + "\",chartData:\"" + sBuilder.ToString() + "\"}";
        }
    }
}