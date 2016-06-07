﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin
{
    public partial class SimulationRun : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 人员ID
        /// </summary>
        public string IDs
        {
            get
            {
                return this.Request.QueryString["IDs"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            string[] emps = this.IDs.Split(',');
            this.Pub1.AddTable();
            this.Pub1.AddCaption("流程模拟自动运行");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("模拟执行人员");
            this.Pub1.AddTDTitle("参数（可以为空)");
            this.Pub1.AddTREnd();

            int idx = 0;
            foreach (string empStr in emps)
            {
                if (string.IsNullOrEmpty(empStr))
                    continue;

                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);

                BP.Port.Emp emp = new BP.Port.Emp(empStr);
                CheckBox cb = new CheckBox();
                cb.Text = emp.No + "-" + emp.Name;
                cb.Checked = true;
                cb.ID = "CB_" + emp.No;
                this.Pub1.AddTD(cb);

                TextBox tb = new TextBox();
                tb.ID = "TB_" + emp.No;
                tb.Width = 900;
                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            this.Pub1.Add("格式为: @Para1=Value1@Para2=Value2）比如:@QingJiaTianShu=20");

            Button btn = new Button();
            btn.ID = "ss";
            btn.Text = "执行";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            string html = "";
            foreach (Control ctl in this.Pub1.Controls)
            {
                TextBox tb = ctl as TextBox;
                if (tb == null)
                    continue;

                string empid = ctl.ID.Replace("TB_", "");
                string paras = tb.Text;

                html += "<fieldset>";
                html += "<legend>" + empid + "</legend>";
                html += BP.WF.Glo.Simulation_RunOne(this.FK_Flow, empid, paras);
                html += "</fieldset>";
            }

            //输出执行结果.
            this.Response.Write(html);

            //没有.
            BP.WF.Dev2Interface.Port_Login("admin");
        }
    }
}