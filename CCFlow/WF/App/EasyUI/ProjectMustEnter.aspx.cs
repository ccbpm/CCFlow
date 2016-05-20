using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BP.DA;
using BP.WF;
using BP.Web;
using BP.Tools;
using System.Text;

namespace CCFlow.AppDemoLigerUI
{
    public partial class ProjectMustEnter : System.Web.UI.Page
    {
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
            {
                InitProjectMustEnter();
                return;
            }
            method = Request["method"].ToString();
            switch (method)
            {
                case "getmustenterdata"://获取列表数据
                    s_responsetext = GetMustEnterData();
                    break;
                case "editmustenter"://编辑数量
                    s_responsetext = EditMustEnter();
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

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        private string GetMustEnterData()
        {
            string projNo = getUTF8ToString("ProjNo");
            string curYear = string.IsNullOrEmpty(getUTF8ToString("NYear")) ? DateTime.Now.Year.ToString() : getUTF8ToString("NYear");
            string sql = string.Format("select * from TJ_SBL where XMID='{0}' and NYEAR='{1}'", projNo, curYear);
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            
            return ReplaceIllgalChart(BP.DA.DataTableConvertJson.DataTable2Json(dt, dt.Rows.Count));
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        private string EditMustEnter()
        {
            string id = getUTF8ToString("ID");
            string RBSL = getUTF8ToString("RBSL");
            string SWJLYBSL = getUTF8ToString("SWJLYBSL");
            string XMJLYBSL = getUTF8ToString("XMJLYBSL");
            string Remark = getUTF8ToString("Remark");
            
            //备注添加最后修改人
            Remark = Remark.Replace("最后修改人", "修改人") + " 最后修改人：" + WebUser.Name;
            string sql = "UPDATE TJ_SBL SET YBRB={0},YBSWJLYB={1},YBXMJLYB={2},MEMO='{3}' where ID={4}";
            sql = string.Format(sql, RBSL, SWJLYBSL, XMJLYBSL, Remark, id);
            int i = DBAccess.RunSQL(sql);
            if (i > 0)
                return "true";
            return "false";
        }
        /// <summary>
        /// 初始化数量
        /// </summary>
        private void InitProjectMustEnter()
        {
            string projNo = getUTF8ToString("id");
            int currentYear = DateTime.Now.Year;
            string sql = "";
            //判断编号是否存在
            if (string.IsNullOrEmpty(projNo))
                return;

            sql = "select * from TJ_SBL where XMID='" + projNo + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //判断记录是否存在，不存在进行初始化
            if (dt == null || dt.Rows.Count > 0)
            {
                Lab_XMMC.InnerText = "(项目名称：" + dt.Rows[0]["XNMC"].ToString() + ")";
                sql = "select distinct NYEAR from TJ_SBL where XMID='" + projNo + "' order by NYEAR";
                dt = DBAccess.RunSQLReturnTable(sql);
                DDL_Year.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem();
                    item.Value = row["NYEAR"].ToString();
                    item.Text = row["NYEAR"].ToString();
                    if (currentYear.ToString() == item.Value)
                        item.Selected = true;
                    DDL_Year.Items.Add(item);
                }
                return;
            }

            //创建记录
            sql = "select * from XM where XMID='" + projNo + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count > 0)
            {
                string xmbh = dt.Rows[0]["BM"].ToString();
                string xmmc = dt.Rows[0]["MC"].ToString();
                Lab_XMMC.InnerText = "(项目名称：" + xmmc + ")";
                //根据当前年，向上追1年，向下追5年包括今年
                DDL_Year.Items.Clear();
                for (int iYear = currentYear - 1, j = currentYear + 5; iYear < j; iYear++)
                {
                    ListItem item = new ListItem();
                    item.Value = iYear.ToString();
                    item.Text = iYear.ToString();
                    if (currentYear == iYear)
                        item.Selected = true;
                    DDL_Year.Items.Add(item);
                    //循环12个月
                    for (int iMonth = 1; iMonth <= 12; iMonth++)
                    {
                        //天数
                        int daymumber = DateTime.DaysInMonth(iYear, iMonth);
                        string xmny = iYear + (iMonth < 10 ? "0" + iMonth.ToString() : iMonth.ToString());
                        string formSql = "insert into TJ_SBL(XMID,XMBH,XNMC,XMNY,YBRB,YBSWJLYB,YBXMJLYB,NYEAR,NMONTH) values(@XMID,@XMBH,@XNMC,@XMNY,@YBRB,@YBSWJLYB,@YBXMJLYB,@NYEAR,@NMONTH)";
                        Paras paras = new Paras();
                        paras.Add("XMID", projNo);
                        paras.Add("XMBH", xmbh);
                        paras.Add("XNMC", xmmc);
                        paras.Add("XMNY", xmny);
                        paras.Add("YBRB", daymumber);
                        paras.Add("YBSWJLYB", "1");
                        paras.Add("YBXMJLYB", "1");
                        paras.Add("NYEAR", iYear);
                        paras.Add("NMONTH", iMonth);
                        DBAccess.RunSQL(formSql, paras);
                    }
                }
            }
        }

        /// <summary>
        /// 去除特殊字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string ReplaceIllgalChart(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0, j = s.Length; i < j; i++)
            {

                char c = s[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '/':
                        sb.Append("\\/");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}