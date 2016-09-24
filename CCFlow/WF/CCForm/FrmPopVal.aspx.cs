using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.Sys;
using BP.En;
namespace CCFlow.WF.Web.CCForm
{
    public partial class WF_FrmPopVal : BP.Web.WebPage
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
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //string url = this.Request.RawUrl;
            //url = url.Replace("FrmPopVal.aspx", "FrmPopVal.htm");
            //this.Response.Redirect(url,true);
            //return;

            me = new MapExt();
            me.MyPK = this.FK_MapExt;
          
            if (me.RetrieveFromDBSources() == 0)
            {
                BP.Web.Controls.FrmPopVal pv = new BP.Web.Controls.FrmPopVal(this.FK_MapExt);
                me.Copy(pv);
            }

            if (me.PopValWorkModel == PopValWorkModel.GroupModel )
            {
                this.Response.Redirect("FrmPopValDir.aspx?a=2"+this.RequestParas,true);
                return;
            }
            
            string sqlGroup = me.Tag1;
            sqlGroup = sqlGroup.Replace("@WebUser.No", BP.Web.WebUser.No);
            sqlGroup = sqlGroup.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

            string sqlObjs = me.Tag2;
            sqlObjs = sqlObjs.Replace("@WebUser.No", BP.Web.WebUser.No);
            sqlObjs = sqlObjs.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            sqlObjs = sqlObjs.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

            bool isHaveUnGroup = true;
            DataTable dtGroup = new DataTable();
            if (sqlGroup.Length > 5)
            {
                isHaveUnGroup = false;
                dtGroup = BP.DA.DBAccess.RunSQLReturnTable(sqlGroup);
            }
            else
            {
                dtGroup.Columns.Add("No", typeof(string));
                dtGroup.Columns.Add("Name", typeof(string));
                DataRow dr = dtGroup.NewRow();
                dr["No"] = "01";
                dr["Name"] = "全部选择";
                dtGroup.Rows.Add(dr);
            }

            DataTable dtObj = BP.DA.DBAccess.RunSQLReturnTable(sqlObjs);
            if (dtObj.Columns.Count == 2)
            {
                dtObj.Columns.Add("Group", typeof(string));
                foreach (DataRow dr in dtObj.Rows)
                    dr["Group"] = "01";
            }

            int cols = 4;
            this.Pub1.AddTable("width=95% border=0");
            if (isHaveUnGroup == false)
            {
                foreach (DataRow drGroup in dtGroup.Rows)
                {
                    string ctlIDs = "";
                    string groupNo = drGroup[0].ToString();

                    //增加全部选择.
                    this.Pub1.AddTR();
                    CheckBox cbx = new CheckBox();
                    cbx.ID = "CBs_" + drGroup[0].ToString();
                    cbx.Text = drGroup[1].ToString();
                    this.Pub1.AddTDTitle("align=left", cbx);
                    this.Pub1.AddTREnd();

                    this.Pub1.AddTR();
                    this.Pub1.AddTDBegin("nowarp=false");

                    this.Pub1.AddTable("border=0");
                    int colIdx = -1;
                    foreach (DataRow drObj in dtObj.Rows)
                    {
                        string no = drObj[0].ToString();
                        string name = drObj[1].ToString();
                        string group = drObj[2].ToString();
                        if (group.Trim() != groupNo.Trim())
                            continue;

                        colIdx++;
                        if (colIdx == 0)
                            this.Pub1.AddTR();

                        CheckBox cb = new CheckBox();
                        cb.ID = "CB_" + no;
                        ctlIDs += cb.ID + ",";
                        cb.Attributes["onclick"] = "isChange=true;";
                        cb.Text = name;
                        cb.Checked = this.CtrlVal.Contains("," + no + ",");
                        if (cb.Checked)
                            cb.Text = "<font color=green>" + cb.Text + "</font>";
                        this.Pub1.AddTD(cb);
                        if (cols - 1 == colIdx)
                        {
                            this.Pub1.AddTREnd();
                            colIdx = -1;
                        }
                    }
                    cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    if (colIdx != -1)
                    {
                        while (colIdx != cols - 1)
                        {
                            colIdx++;
                            this.Pub1.AddTD();
                        }
                        this.Pub1.AddTREnd();
                    }
                    this.Pub1.AddTableEnd();
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }
            #region 处理未分组的情况.
            if (isHaveUnGroup == true)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDBigDocBegain(); // ("nowarp=true");
                this.Pub1.AddTable();
                int colIdx = -1;
                string ctlIDs = "";
                //增加全部选择.
                this.Pub1.AddTR();
                CheckBox cbx = new CheckBox();
                cbx.ID = "CBs_" + dtGroup.Rows[0][0].ToString();
                cbx.Text = dtGroup.Rows[0][1].ToString();

                this.Pub1.AddTDTitle("align=left colspan=4", cbx);
                this.Pub1.AddTREnd();

                foreach (DataRow drObj in dtObj.Rows)
                {
                    string group = drObj[2].ToString();
                    isHaveUnGroup = true;
                    foreach (DataRow drGroup in dtGroup.Rows)
                    {
                        string groupNo = drGroup[0].ToString();
                        if (group != groupNo)
                        {
                            isHaveUnGroup = true;
                            break;
                        }
                    }

                    if (isHaveUnGroup == false)
                        continue;

                    string no = drObj[0].ToString();
                    string name = drObj[1].ToString();

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Text = name + group;
                    cb.Checked = this.CtrlVal.Contains("," + no + ",");
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";

                    this.Pub1.AddTD(cb);

                    if (cols - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                if (colIdx != -1)
                {
                    while (colIdx != cols - 1)
                    {
                        colIdx++;
                        this.Pub1.AddTD();
                    }
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            #endregion 处理未分组的情况.

            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "s";
            btn.Text = " OK ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Cancel";
            btn.Text = " Cancel ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ID == "Cancel")
            {
                this.WinClose();
                return;
            }

            // MapExt me = new MapExt(this.FK_MapExt);

            int popValFormat = (int)me.PopValFormat;
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
                        val +=  cb.ID.Replace("CB_", "") + "," + text+";";
                        break;
                    default:
                        break;
                }
            }
            val = val.Replace("<font color=green>", "");
            val = val.Replace("</font>", "");

            if (val.Length > 2 && popValFormat != 2)
                val = val.Substring(1);
            this.WinClose(val);
        }
    }
}