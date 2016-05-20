using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web.Controls;
namespace CCFlow.WF.CCForm
{
    public partial class Comm_M2M : BP.Web.WebPage
    {
        public Int64 OID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["OID"]);
            }
        }
        public string IsOpen
        {
            get
            {
                return this.Request.QueryString["IsOpen"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        public string IsEdit
        {
            get
            {
                return this.Request.QueryString["IsEdit"];
            }
        }
        public string FK_MapExt
        {
            get
            {
                return this.Request.QueryString["FK_MapExt"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Page.RegisterClientScriptBlock("s",
            //    "<link href='../Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");
            this.Pub1.Clear();
            MapM2M mapM2M = new MapM2M(this.FK_MapData, this.NoOfObj);
            if (mapM2M.HisM2MType == M2MType.M2MM)
            {
                this.Response.Redirect("M2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj + "&IsOpen=" + this.IsOpen + "&OID=" + this.OID, true);
                return;
            }

            BP.Sys.M2M m2m = new BP.Sys.M2M();
            m2m.MyPK = this.FK_MapData + "_" + this.NoOfObj + "_" + this.OID + "_";
            m2m.RetrieveFromDBSources();
            DataTable dtGroup = new DataTable();
            if (mapM2M.DBOfGroups.Length > 5)
            {
                dtGroup = BP.DA.DBAccess.RunSQLReturnTable(mapM2M.DBOfGroupsRun);
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

            DataTable dtObj = BP.DA.DBAccess.RunSQLReturnTable(mapM2M.DBOfObjsRun);
            if (dtObj.Columns.Count == 2)
            {
                dtObj.Columns.Add("Group", typeof(string));
                foreach (DataRow dr in dtObj.Rows)
                    dr["Group"] = "01";
            }

            bool isInsert = mapM2M.IsInsert;
            bool isDelete = mapM2M.IsDelete;

            if ((isDelete || isInsert) && string.IsNullOrEmpty(this.IsOpen) == false)
                this.Button1.Visible = true;

            this.Pub1.AddTable(" width='100%' border=0 ");
            foreach (DataRow drGroup in dtGroup.Rows)
            {
                string ctlIDs = "";
                string groupNo = drGroup[0].ToString();
                string minOrMax = this.IsPostBack ? "Min" : "Max";

                string cbsID = "CBs_" + drGroup[0].ToString();
                //增加全部选择.
                this.Pub1.AddTR();
                CheckBox cbx = null;
                if (mapM2M.IsCheckAll == true)
                {
                    cbx = new CheckBox();
                    cbx.ID = cbsID;
                    cbx.Text = drGroup[1].ToString();
                    this.Pub1.Add("<TD class=Title onDblClick=\"GroupBarClick('" + cbx.ID + "')\">");
                    this.Pub1.Add("<div class=ckbgroup style='float:left'>");
                    this.Pub1.Add(cbx);
                    this.Pub1.Add("</div>");
                }
                else
                {
                    this.Pub1.Add("<TD class=Title >");
                    this.Pub1.Add("<div class=ckbgroup style='float:left'>");
                    this.Pub1.Add("<img onclick=\"GroupBarClick('" + cbsID + "')\" src='../Img/" + minOrMax + ".gif' id='I" + cbsID + "' alt = '" + minOrMax + "' />" + drGroup[1].ToString());
                    this.Pub1.Add("</div>");
                }


                this.Pub1.Add("<div style='float:right'><img onclick=\"GroupBarClick('" + cbsID + "')\" src='../Img/" + minOrMax + ".gif' id='I" + cbsID + "' alt = '" + minOrMax + "' /></div>");
                this.Pub1.Add("</TD>");
                this.Pub1.AddTREnd();

                this.Pub1.AddTR("ID='TR" + cbsID + "' style='display:none'");

                this.Pub1.AddTDBegin("nowarp=false");
                this.Pub1.Add("<table width=100% border=0 >");
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
                        this.Pub1.Add("<TR>");

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Attributes["onclick"] = "isChange=true;";
                    cb.Text = name;
                    cb.Checked = m2m.Vals.Contains("," + no + ",");
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";
                    this.Pub1.Add("<TD>");
                    this.Pub1.Add(cb);
                    this.Pub1.Add("</TD>");

                    if (mapM2M.Cols - 1 == colIdx)
                    {
                        this.Pub1.Add("</TR>");
                        colIdx = -1;
                    }
                }
                if (mapM2M.IsCheckAll == true)
                    cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                if (colIdx != -1)
                {
                    while (colIdx != mapM2M.Cols - 1)
                    {
                        colIdx++;
                        this.Pub1.Add("<TD ></TD>");
                    }
                    this.Pub1.Add("</TR>");
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

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Text = name + group;
                    cb.Checked = m2m.Vals.Contains("," + no + ",");
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";

                    this.Pub1.AddTD(cb);

                    if (mapM2M.Cols - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                if (colIdx != -1)
                {
                    while (colIdx != mapM2M.Cols - 1)
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
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (this.OID == 0)
                return;

            MapM2M mapM2M = new MapM2M(this.FK_MapData, this.NoOfObj);

            BP.Sys.M2M m2m = new BP.Sys.M2M();
            m2m.FK_MapData = this.FK_MapData;
            m2m.EnOID = this.OID;
            m2m.M2MNo = this.NoOfObj;

            DataTable dtObj = BP.DA.DBAccess.RunSQLReturnTable(mapM2M.DBOfObjsRun);
            string str = ",";
            string strT = "";
            int numOfselected = 0;
            foreach (DataRow dr in dtObj.Rows)
            {
                string id = dr[0].ToString();
                CheckBox cb = this.Pub1.GetCBByID("CB_" + id);
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                str += id + ",";
                strT += "@" + id + "," + cb.Text;
                numOfselected++;
            }
            m2m.Vals = str;
            m2m.ValsName = strT;
            m2m.InitMyPK();
            m2m.NumSelected = numOfselected;
            m2m.Save();
        }
    }

}