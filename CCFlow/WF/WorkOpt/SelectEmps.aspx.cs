using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace CCFlow.WF.WorkOpt
{
    public partial class WF_WorkOpt_SelectEmps : BP.Web.WebPage
    {
        #region 属性.
        public string DBOfGroups
        {
            get
            {
                return "SELECT No,Name FROM Port_Dept ";
            }
        }
        public string DBOfObjs
        {
            get
            {
                return "SELECT No,Name,FK_Dept FROM Port_Emp ";
            }
        }
        public int Cols
        {
            get
            {
                return 5;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
               "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            string ctrlVal = "," + this.Request.QueryString["CtrlVal"] + ",";
            ctrlVal = ctrlVal.Replace(";", ",");
            ctrlVal = ctrlVal.Replace(" ", "");
            ctrlVal = ctrlVal.Replace(" ", "");
            ctrlVal = ctrlVal.Replace(" ", "");
            List<string> empNos = new List<string>();
            DataTable dtGroup = new DataTable();
            if (this.DBOfGroups.Length > 5)
            {
                dtGroup = BP.DA.DBAccess.RunSQLReturnTable(this.DBOfGroups);
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

            DataTable dtObj = BP.DA.DBAccess.RunSQLReturnTable(this.DBOfObjs);
            if (dtObj.Columns.Count == 2)
            {
                dtObj.Columns.Add("Group", typeof(string));
                foreach (DataRow dr in dtObj.Rows)
                    dr["Group"] = "01";
            }

            this.Pub1.AddTable("width=100% border=0");
            this.Pub1.AddCaptionLeft("选择人员");
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

                this.Pub1.AddTable();
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
                    empNos.Add(no);
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Attributes["onclick"] = "isChange=true;";
                    cb.Text = name;
                    cb.Checked = ctrlVal.Contains("," + no + ",");
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";
                    this.Pub1.AddTD(cb);
                    if (this.Cols - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                if (colIdx != -1)
                {
                    while (colIdx != this.Cols - 1)
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

            #region 处理未分组的情况.
            bool isHaveUnGroup = false;
            foreach (DataRow drObj in dtObj.Rows)
            {
                string group = drObj[2].ToString();
                isHaveUnGroup = true;
                foreach (DataRow drGroup in dtGroup.Rows)
                {
                    string groupNo = drGroup[0].ToString();
                    if (group == groupNo)
                    {
                        isHaveUnGroup = false;
                        break;
                    }
                }
                if (isHaveUnGroup == false)
                    continue;
            }


            if (isHaveUnGroup == true)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDBigDocBegain(); // ("nowarp=true");
                this.Pub1.AddTable();
                int colIdx = -1;
                string ctlIDs = "";
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
                    if (empNos.Contains(no))
                        continue;

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Text = name + group;
                    cb.Checked = ctrlVal.Contains("," + no + ",");
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";
                    this.Pub1.AddTD(cb);
                    if (this.Cols - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                if (colIdx != -1)
                {
                    while (colIdx != this.Cols - 1)
                    {
                        colIdx++;
                        this.Pub1.AddTD();
                    }
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
                //cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            #endregion 处理未分组的情况.
            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_OK";
            btn.CssClass = "Btn";
            btn.Text = "确定";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }
        void btn_Click(object sender, EventArgs e)
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(this.DBOfObjs);
            string keys = "";
            foreach (DataRow dr in dt.Rows)
            {
                CheckBox cb = this.Pub1.GetCBByID("CB_" + dr[0].ToString());
                if (cb == null)
                    continue;
                if (cb.Checked == false)
                    continue;
                keys += dr[0].ToString() + ",";
            }

            keys = keys.Substring(0, keys.Length - 1);
            this.WinClose(keys);
        }
    }
}