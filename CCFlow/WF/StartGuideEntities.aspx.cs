﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using System.Collections;
namespace CCFlow.WF
{
    public partial class StartGuideEntities : BP.Web.WebPage
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
        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType != null)
                return;

            Flow fl = new Flow(this.FK_Flow);
            this.Title = fl.Name;
            this.Label1.Text = fl.Name;

            this.Pub1.Add("&nbsp;&nbsp;请输入关键字:");

            TextBox tb = new TextBox();
            tb.ID = "TB_Key";
            tb.Text = this.SKey;
            this.Pub1.Add(tb);
         
            Button btn = new Button();
            btn.ID = "Btn1";
            btn.Text = "查询";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            if (fl.StartGuideLink.Length > 10)
            {
                string url = "<div style='float:right'><a href=\"javascript:WinOpen('" + fl.StartGuideLink + "')\" >" + fl.StartGuideLab + "</a></div>";
                this.Pub1.Add(url);
            }

            
            #region 显示数据.
            string key = this.Pub1.GetTextBoxByID("TB_Key").Text.Trim();
            string sql = "";
            if (this.SKey == null)
            {
                sql = fl.StartGuidePara2.Clone() as string;
                sql = sql.Replace("~", "'");
            }
            else
            {
                sql = fl.StartGuidePara1.Clone() as string;
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
                case StartGuideWay.BySQLMulti:
                    this.BindTableMulti(dt);
                    //增加启动流程.
                    Button bton = new Button();
                    bton.ID = "Btn_Sav2";
                    bton.Text = "批量发起";
                    bton.Click += new EventHandler(bton_Start_Click);
                    this.Pub1.Add(bton);
                    break;
                default:
                    //绑定多个.
                    this.BindTableMulti(dt);
                    //增加启动流程.
                    Button button = new Button();
                    button.ID = "Btn_Sav2";
                    button.Text = "启动流程";
                    button.Click += new EventHandler(btn_Start_Click);
                    this.Pub1.Add(button);
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
            this.Response.Redirect("StartGuideEntities.aspx?FK_Flow=" + this.FK_Flow + "&SKey=" + key+"&WorkID="+this.WorkID, true);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void BindTableOne(DataTable dt)
        {
            this.Pub2.AddTable("width='100%'");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("IDX");
            this.Pub2.AddTDTitle("Name");
            foreach (DataColumn dc in dt.Columns)
            {
                switch (dc.ColumnName.ToLower())
                {
                    case "pflowno":
                    case "pworkid":
                    case "no":
                    case "name":
                        continue;
                    default:
                        break;
                }
                this.Pub2.AddTDTitle(dc.ColumnName);
            }
            this.Pub2.AddTREnd();

            string url = "MyFlow.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID="+this.WorkID+"&IsCheckGuide=1";
            // 输出数据.
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);

                string paras = url + "&No=" + dr["No"];
                //foreach (DataColumn dc in dt.Columns)
                //{
                //    string str = dr[dc.ColumnName] as string;
                //    if (string.IsNullOrEmpty(str) == true)
                //        continue;

                //    if (str.Contains("<"))
                //    {
                //        /*如果包含特殊标记, 就去掉它.*/
                //        str = BP.DA.DataType.ParseHtmlToText(str);
                //    }
                //    paras += "&" + System.Web.HttpUtility.HtmlEncode(dc.ColumnName) + "=" + System.Web.HttpUtility.HtmlEncode(dr[dc.ColumnName]);
                //}

                //输出名称列
                this.Pub2.AddTD("<a href='" + paras + "' >" +dr["No"]+" - "+ dr["Name"] + "</a>");
                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.ColumnName.ToLower())
                    {
                        case "pflowno":
                        case "pworkid":
                        case "no":
                        case "name":
                            continue;
                        default:
                            break;
                    }

                    string val = dr[dc.ColumnName].ToString();
                    if (val == null)
                        val = "";

                    //i++;
                    ////输出连接.
                    //if (i == 1)
                    //    this.Pub2.AddTD("<a href='" + paras + "' >" + val + "</a>");
                    //else
                    this.Pub2.AddTD(val);
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
            if (string.IsNullOrEmpty(cWorkID) == true)
            {
                BP.Sys.PubClass.Alert("您没有选择项目.");
                return;
            }

            Flow fl = new Flow(this.FK_Flow);
            string url = "MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID=0&IsCheckGuide=1";

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
        void bton_Start_Click(object sender, EventArgs e)
        {
            string no = "";
            foreach (Control ctl in this.Pub2.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID.Contains("CB_") == false)
                    continue;

                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                no = no + "'" + ctl.ID.Replace("CB_", "") + "',";
            }
            if (string.IsNullOrEmpty(no) == true)
            {
                BP.Sys.PubClass.Alert("您没有选择项目.");
                return;
            }

            //获取设置的数据源
            Flow fl = new Flow(this.FK_Flow);
            string key = this.Pub1.GetTextBoxByID("TB_Key").Text.Trim();
            string sql = "";
            if (this.SKey == null)
            {
                sql = fl.StartGuidePara2.Clone() as string;
                sql = sql.Replace("~", "'");
            }
            else
            {
                sql = fl.StartGuidePara1.Clone() as string;
                sql = sql.Replace("@Key", key);
                sql = sql.Replace("~", "'");
            }
            //替换变量
            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //获取选中的数据源
            DataRow[] drArr = dt.Select("No in(" + no.TrimEnd(',') + ")");

            //获取ht并发送
            string sendMsg = "";
            for (int i = 0; i < drArr.Length; i++)
            {
                DataRow row = drArr[i];
                Hashtable ht = new Hashtable();
                //生成workid
                long workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow);

                //生成表单数据
                for (int k = 0; k < row.Table.Columns.Count; k++)
                {
                    ht.Add(row.Table.Columns[k].ColumnName, row[k]);
                }
                //执行发送
                try
                {
                    sendMsg += BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, workid, ht).ToMsgOfHtml();
                }
                catch (Exception ex)
                {
                    BP.Sys.PubClass.Alert("发送失败！" + ex.Message.ToString());
                    return;
                }
            }
            this.ToMsg(sendMsg, "info");

        }
        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + int.Parse(this.FK_Flow) + "01&WorkID=" + this.WorkID, false);
        }
    }
}