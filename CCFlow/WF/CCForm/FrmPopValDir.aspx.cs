using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.Sys;
using BP.En;
using BP.DA;

namespace CCFlow.WF.CCForm
{
    public partial class FrmPopValDir : BP.Web.WebPage
    {
        #region 属性.
        private string _CtrlVal = null;
        public string CtrlVal
        {
            get
            {
                if (_CtrlVal == null)
                    _CtrlVal = "," + this.Request.QueryString["CtrlVal"] + ",";
                return _CtrlVal;
            }
        }
        public string FK_MapExt
        {
            get
            {
                return this.Request.QueryString["FK_MapExt"];
            }
        }
        public MapExt me = null;

        public string GroupVal
        {
            get
            {
                return this.Request.QueryString["GroupVal"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            me = new MapExt();
            me.MyPK = this.FK_MapExt;

            if (me.RetrieveFromDBSources() == 0)
            {
                BP.Web.Controls.FrmPopVal pv = new BP.Web.Controls.FrmPopVal(this.FK_MapExt);
                me.Copy(pv);
            }

            string sqlGroup = me.Tag1;
            sqlGroup = sqlGroup.Replace("@WebUser.No", BP.Web.WebUser.No);
            sqlGroup = sqlGroup.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);

            if (dtGroup.Rows.Count == 0)
            {
                this.Pub1.AddFieldSet("配置错误","分组数据源为空:"+sqlGroup);
                return;
            }

            this.Left.AddUL();
            foreach (DataRow dr in dtGroup.Rows)
            {
                string no = dr[0].ToString();
                string name = dr[1].ToString();
                this.Left.AddLi("<a href='FrmPopValDir.aspx?GroupVal=" + no + "&FK_MapExt=" + this.FK_MapExt + "&RefPK=" + this.RefPK + "&CtrlVal=" +this.CtrlVal + "' >" + dr[1].ToString() + "</a>");
            }
            this.Left.AddULEnd();


            string gVal = this.GroupVal;
            if (string.IsNullOrEmpty(gVal) )
                gVal = dtGroup.Rows[0][0].ToString();
            string sqlObjs = me.Tag2;
            sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
            sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            sqlObjs = sqlObjs.Replace("@GroupVal", gVal);
            DataTable dtObjs = DBAccess.RunSQLReturnTable(sqlObjs);

            bool isCheckbox = false;
            if (me.PopValSelectModel == 1)
                isCheckbox = true;

            foreach (DataRow dr in dtObjs.Rows)
            {
                string no = dr[0].ToString();
                string name = dr[1].ToString();

                if (isCheckbox == true)
                {
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    cb.Text = name;
                    this.Pub1.Add(cb);
                }
                else
                {
                    RadioButton rb = new RadioButton();
                    rb.ID = "RB_" + no;
                    rb.Text = name;
                    rb.GroupName = "ss";
                    this.Pub1.Add(rb);
                }
            }

            if (dtObjs.Rows.Count == 0)
                this.Pub1.AddFieldSet("配置或者数据源错误", "查询的entity是空的:" + sqlObjs);
        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            me = new MapExt();
            me.MyPK = this.FK_MapExt;

            if (me.RetrieveFromDBSources() == 0)
            {
                BP.Web.Controls.FrmPopVal pv = new BP.Web.Controls.FrmPopVal(this.FK_MapExt);
                me.Copy(pv);
            }
  
            int popValFormat = me.PopValFormat;
            string val = "";
            foreach (Control ctl in this.Pub1.Controls)
            {
                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;
                if (cb.ID.Contains("CBs_"))
                    continue;
                if (cb.Checked == false)
                    continue;
                string text = cb.Text.Replace("<font color=green>", "");
                text = cb.Text.Replace("</font>", "");
                switch (popValFormat)
                {
                    case 0:  //仅仅编号
                        val += "," + cb.ID.Replace("CB_", "");
                        break;
                    case 1: // 仅名称
                        val += "," + text;
                        break;
                    case 2: // 编号与名称
                        val += "," + cb.ID.Replace("CB_", "") + "," + text;
                        break;
                    default:
                        break;
                }
            }
            val = val.Replace("<font color=green>", "");
            val = val.Replace("</font>", "");

            if (val.Length > 2)
                val = val.Substring(1);
            this.WinClose(val);
        }
    }
}