using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.CCForm
{
    public partial class DtlOpSelectItems : BP.Web.WebPage
    {

        public string getUTF8ToString(string param)
        {
            return Server.UrlDecode(Request[param]);
        }

        #region 属性.
        public string SKey
        {
            get
            {
                return getUTF8ToString("SKey");
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (str == null)
                    return 0;
                return Int64.Parse(str);
            }
        }
        public string FK_MapDtl
        {
            get
            {
                return this.Request.QueryString["FK_MapDtl"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

            if (this.DoType != null)
                return;

            MapDtl dtl = new BP.Sys.MapDtl(this.FK_MapDtl);
            this.Title = dtl.Name;
            this.Label1.Text = dtl.Name;

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

            #region 显示数据.
            string key = this.Pub1.GetTextBoxByID("TB_Key").Text.Trim();
            string sql = "";
            if (this.SKey == null)
            {
                sql = dtl.ImpSQLInit.Clone() as string;
                sql = sql.Replace("~", "'");
            }
            else
            {
                sql = dtl.ImpSQLSearch.Clone() as string;
                sql = sql.Replace("@Key", key);
                sql = sql.Replace("~", "'");
            }

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            this.BindTableMulti(dt);
            //增加启动流程.
            Button button = new Button();
            button.ID = "Btn_Pub1";
            button.Text = "放入";
            button.Click += new EventHandler(btn_Pub_Click);
            this.Pub1.Add(button);

            button = new Button();
            button.ID = "Btn_Pub2";
            button.Text = "放入并关闭";
            button.Click += new EventHandler(btn_Pub_Click);
            this.Pub1.Add(button);

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
            key = Server.UrlEncode(key);
            this.Response.Redirect("DtlOpSelectItems.aspx?SKey=" + key + "&WorkID=" + this.WorkID + "&FK_MapDtl=" + this.FK_MapDtl, false);
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

        void btn_Pub_Click(object sender, EventArgs e)
        {
            #region 获得选择的ID
            string ids = "";
            foreach (Control ctl in this.Pub2.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID.Contains("CB_") == false)
                    continue;

                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                ids += ctl.ID.Replace("CB_", "") + ",";
            }
            if (string.IsNullOrEmpty(ids) == true)
            {
                BP.Sys.PubClass.Alert("您没有选择项目.");
                return;
            }
            this.Alert("成功将:" + ids + "放入了数据表...");
            #endregion 获得选择的ID

            #region 获得数据.
            MapDtl dtl = new BP.Sys.MapDtl(this.FK_MapDtl);
            string sql = "";
            DataTable dt = null;
            try
            {
                sql = dtl.ImpSQLFull.Clone() as string;
                ids = ids.Replace(",", "','");
                sql = sql.Replace("@Keys", ids.Substring(0, ids.Length - 3));
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //当ID是int类型是可能抛出异常.
            }
            catch
            {
                sql = dtl.ImpSQLFull.Clone() as string;
                sql = sql.Replace("@Keys", ids.Substring(0, ids.Length - 2));
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            #endregion 获得数据.

            #region 把数据放入明细表.
            GEDtl gedtl = new BP.Sys.GEDtl(this.FK_MapDtl);
            foreach (DataRow dr in dt.Rows)
            {
                gedtl.RefPK = this.WorkID.ToString();
                gedtl.FID = this.FID;
                gedtl.Rec = WebUser.No;
                foreach (DataColumn dc in dt.Columns)
                {
                    //赋值.
                    gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                }
                gedtl.InsertAsOID(BP.DA.DBAccess.GenerOID());
            }
            #endregion 把数据放入明细表.

            Button btn = sender as Button;
            if (btn.ID == "Btn_Pub1")
            {
                this.Alert("放入成功");
            }
            else
            {
                this.WinCloseWithMsg("放入成功");
            }

        }
    }
}