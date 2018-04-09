using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;

namespace CCFlow.WF
{
    public partial class StartGuide : BP.Web.WebPage
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string SKey
        {
            get
            {
                return this.Request.QueryString["SKey"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType != null)
                return;

            this.Pub1.Add("<br><b>&nbsp;&nbsp;请输入关键字:&nbsp;</b>");
            TextBox tb = new TextBox();
            tb.ID = "TB_Key";
            tb.Text = this.SKey;
            this.Pub1.Add(tb);

            Flow fl = new Flow(this.FK_Flow);

            if (fl.StartGuideLink.Length > 10)
            {
                string url = "<div style='float:right'><a href=\"javascript:WinOpen('" + fl.StartGuideLink + "')\" >" + fl.StartGuideLab + "</a></div>";
                this.Pub1.Add(url);
            }
            
            this.Pub1.AddTD();

            //ImageButton imgbtn = new ImageButton();
            //imgbtn.ID = "imgbtn1";
            //imgbtn.ImageUrl = "./Img/Search.gif";
            //imgbtn.Click += new ImageClickEventHandler(btn_Click);
            //this.Pub1.Add(imgbtn);

            Button btn = new Button();
            btn.ID = "Btn1";
            btn.Text = "查询";
         //   btn.Attributes.Add("CssClass", "Img");
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            this.Pub1.AddTD();

            if (fl.StartGuideWay == StartGuideWay.SubFlowGuide)
            {
                Button button = new Button();
                button.ID = "Btn_Sav2";
                button.Text = "启动流程";
                button.Click += new EventHandler(btn_Start_Click);
                this.Pub1.Add(button);
            }

            //ImageButton imgbtnsav = new ImageButton();
            //imgbtnsav.ID = "imgbtn2";
            //imgbtn.ImageUrl = "./Img/Start.gif";
            //imgbtnsav.Click += new ImageClickEventHandler(btn_Start_Click);
            //this.Pub1.Add(imgbtnsav);

            #region 显示数据.
            string key = this.Pub1.GetTextBoxByID("TB_Key").Text.Trim();
            string sql = "";
            if (this.SKey == null)
            {
                sql = fl.StartGuidePara1.Clone() as string;
                sql = sql.Replace("~", "'");
            }
            else
            {
                sql = fl.StartGuidePara2.Clone() as string;
                sql = sql.Replace("@Key", key);
                sql = sql.Replace("~", "'");
            }

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            switch (fl.StartGuideWay)
            {
                case StartGuideWay.BySQLOne:
                case StartGuideWay.BySystemUrlOneEntity:
                      this.BindTableOne(dt);
                    break;
                case StartGuideWay.ByHistoryUrl: //历史数据.
                    if (dt.Rows.Count == 0)
                    {
                        string url = "MyFlow.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID=0&IsCheckGuide=1";
                        this.Response.Redirect(url, true);
                        //this.BindTableOne(dt);
                    }
                    else
                    {

                        this.BindTableOne(dt);
                    }
                    break;
                case StartGuideWay.SubFlowGuide: //子父流程.
                    this.BindTableMulti(dt);
                    break;
                default:
                    this.BindTableMulti(dt);
                    break;
            }
            #endregion 显示数据.
        }
        /// <summary>
        /// 执行查询.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Click(object sender, EventArgs e)
        {
            string key = this.Pub1.GetTextBoxByID("TB_Key").Text.Trim();
            this.Response.Redirect("StartGuide.aspx?FK_Flow=" + this.FK_Flow + "&SKey=" + key, true);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void BindTableOne(DataTable dt)
        {
            this.Pub2.AddTable("width='100%'");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("IDX");
            foreach (DataColumn dc in dt.Columns)
            {
                switch (dc.ColumnName.ToLower())
                {
                    case "pflowno":
                    case "pworkID":
                    case "no":
                    case "name":
                        continue;
                    default:
                        break;
                }
                this.Pub2.AddTDTitle(dc.ColumnName);
            }
            this.Pub2.AddTREnd();

            Flow fl=new Flow(this.FK_Flow);

            string url = "MyFlow.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID=0&IsCheckGuide=1";
            // 输出数据.
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);

                string paras = url + "";
                foreach (DataColumn dc in dt.Columns)
                {
                    string val = dr[dc.ColumnName] as string;
                    if (DataType.IsNullOrEmpty(val) == true)
                        continue;

                    paras += "&" + dc.ColumnName + "=" + val;
                }

                int i = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.ColumnName.ToLower())
                    {
                        case "pflowno":
                        case "pworkID":
                        case "no":
                        case "name":
                            continue;
                        default:
                            break;
                    }

                    string val = dr[dc.ColumnName].ToString();
                    if (val == null)
                        val = "";

                    i++;
                    //输出连接.
                    if (i == 1)
                    {
                        if (fl.StartGuideWay == StartGuideWay.ByHistoryUrl)
                            this.Pub2.AddTD("<a href='" + url + "&CopyFormWorkID=" + dr["OID"] + "&CopyFormNode=" + int.Parse(this.FK_Flow) + "01' >" + val + "</a>");
                        else
                            this.Pub2.AddTD("<a href='" + paras + "' >" + val + "</a>");
                    }
                    else
                    {
                        this.Pub2.AddTD(val);
                    }
                }
                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void BindTableMulti(DataTable dt)
        {
            //if (dt.Columns.Contains("CTitle") == false || dt.Columns.Contains("CWorkID") == false)
            //{
            //    this.Pub2.AddFieldSetRed("导航参数设置错误", "缺少CFlowNo,CWorkID列.");
            //    return;
            //}

            string pksVal = "no";
            string pksLab = "name";

            this.Pub2.AddTable("width='100%'");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("IDX");
            this.Pub2.AddTDTitle("选择全部");
            foreach (DataColumn dc in dt.Columns)
            {
                switch (dc.ColumnName.ToLower())
                {
                    case "ctitle":
                    case "cworkID":
                        pksVal = "CWorkID";
                        pksLab = "CTitle";
                        continue;
                    case "no":
                    case "name":
                        pksVal = "no";
                        pksLab = "name";
                        continue;
                    default:
                        break;
                }
                this.Pub2.AddTDTitle(dc.ColumnName);
            }
            this.Pub2.AddTREnd();


            // 输出数据.
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);

                //加入选择.
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + dr[pksVal].ToString();
                cb.Text = dr[pksLab].ToString();
                this.Pub2.AddTD(cb);

                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.ColumnName.ToLower())
                    {
                        case "ctitle":
                        case "ctorkID":
                        case "no":
                        case "name":
                            continue;
                        default:
                            break;
                    }

                    string val = dr[dc.ColumnName].ToString();
                    if (val == null)
                        val = "";
                    this.Pub2.AddTD(val);
                }
                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();


        }

        void btn_Start_Click(object sender, EventArgs e)
        {
            string cWorkID = "";
            foreach (Control ctl in this.Pub2.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID.Contains("CB_") == false)
                    continue;

                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                cWorkID += ctl.ID.Replace("CB_", "") + ",";
            }
            if (DataType.IsNullOrEmpty(cWorkID) == true)
            {
                BP.Sys.PubClass.Alert("您没有选择项目.");
                return;
            }

            cWorkID = cWorkID.Substring(0, cWorkID.Length - 1);

            Flow fl = new Flow(this.FK_Flow);
            string url = "MyFlow.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID=0&IsCheckGuide=1";

            //必要的系统约定参数.
            switch (fl.StartGuideWay)
            {
                case StartGuideWay.SubFlowGuide:
                case StartGuideWay.BySQLOne:
                    url += "&DoFunc=SetParentFlow&WorkIDs=" + cWorkID + "&CFlowNo=" + fl.StartGuidePara3;
                    break;
                case StartGuideWay.SubFlowGuideEntity:
                case StartGuideWay.BySystemUrlOneEntity:
                    url += "&DoFunc=" + fl.StartGuideWay.ToString() + "&Nos=" + cWorkID + "&StartGuidePara3=" + fl.StartGuidePara3;
                    break;
                default:
                    break;
            }
            this.Response.Redirect(url, true);

        }
    }
}