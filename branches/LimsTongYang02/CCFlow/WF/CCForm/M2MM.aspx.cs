using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web.Controls;
namespace CCFlow.WF.CCForm
{

    public partial class WF_M2MM : BP.Web.WebPage
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
        private int GetNum(M2Ms m2ms, string obj)
        {
            foreach (M2M m2m in m2ms)
            {
                if (m2m.DtlObj == obj)
                {
                    return m2m.NumSelected;
                }
            }
            return 0;
        }
        /// <summary>
        /// 操作对象
        /// </summary>
        public string OperObj = null;
        public bool BindLeft(MapM2M mapM2M)
        {
            BP.Sys.M2Ms m2ms = new BP.Sys.M2Ms();
            m2ms.Retrieve(M2MAttr.FK_MapData, this.FK_MapData,
                M2MAttr.M2MNo, this.NoOfObj, M2MAttr.EnOID, this.OID);
            DataTable dtList;
            if (mapM2M.DBOfLists.Substring(0, 1) == "@")
            {
                dtList = new DataTable();
                dtList.Columns.Add("No", typeof(string));
                dtList.Columns.Add("Name", typeof(string));
                string myNo = mapM2M.DBOfLists.Replace("@", "");
                BP.Sys.M2M m2m = new BP.Sys.M2M();
                m2m.MyPK = this.FK_MapData + "_" + myNo + "_" + this.OID + "_";
                if (m2m.RetrieveFromDBSources() == 0)
                    return false;

                string[] vals = m2m.ValsName.Split('@');
                this.Left.AddUL();
                foreach (string val in vals)
                {
                    if (string.IsNullOrEmpty(val))
                        continue;
                    string[] strs = val.Split(',');
                    if (this.OperObj == null)
                        this.OperObj = strs[0];

                    if (this.OperObj == strs[0])
                        this.Left.AddLi("<a href='M2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj + "&OID=" + this.OID + "&OperObj=" + strs[0] + "'><b>" + strs[1] + "(" + GetNum(m2ms, strs[0]) + ")</b></a><br>");
                    else
                        this.Left.AddLi("<a href='M2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj + "&OID=" + this.OID + "&OperObj=" + strs[0] + "'>" + strs[1] + "(" + GetNum(m2ms, strs[0]) + ")</a><br>");
                }
                this.Left.AddULEnd();
                return true;
            }
            else
            {
                dtList = DBAccess.RunSQLReturnTable(mapM2M.DBOfListsRun);
            }
            if (dtList.Rows.Count == 0)
                return false;

            this.Left.AddUL();
            foreach (DataRow dr in dtList.Rows)
            {
                if (this.OperObj == null)
                    this.OperObj = dr[0].ToString();

                if (this.OperObj == dr[0].ToString())
                    this.Left.AddLi("<a href='M2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj + "&OID=" + this.OID + "&OperObj=" + dr[0].ToString() + "'><b>" + dr[1].ToString() + "(" + GetNum(m2ms, dr[0].ToString()) + ")</b></a><br>");
                else
                    this.Left.AddLi("<a href='M2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj + "&OID=" + this.OID + "&OperObj=" + dr[0].ToString() + "'>" + dr[1].ToString() + "(" + GetNum(m2ms, dr[0].ToString()) + ")</a><br>");
            }
            this.Left.AddULEnd();
            return true;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
                "<link href='../Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            MapM2M mapM2M = new MapM2M(this.FK_MapData, this.NoOfObj);
            this.OperObj = this.Request.QueryString["OperObj"];

            if (string.IsNullOrEmpty(mapM2M.DBOfLists) == true)
            {
                this.Pub1.AddFieldSetYellow("提示");
                this.Pub1.Add("表单设计错误:没有设置列表数据源.");
                this.Pub1.AddFieldSetEnd();
                this.Button1.Enabled = false;
                return;
            }

            if (this.BindLeft(mapM2M) == false)
            {
                this.Pub1.AddFieldSetYellow("提示");
                this.Pub1.Add("列表数据源为空.");
                this.Pub1.AddFieldSetEnd();
                return;
            }

            BP.Sys.M2M m2m = new BP.Sys.M2M();
            m2m.MyPK = this.FK_MapData + "_" + this.NoOfObj + "_" + this.OID + "_" + this.OperObj;
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
                {
                    dr["Group"] = "01";
                }
            }
            bool isInsert = mapM2M.IsInsert;
            bool isDelete = mapM2M.IsDelete;

            if (isDelete == false && isInsert == false)
                this.Button1.Enabled = false;

            if ((isDelete || isInsert) && string.IsNullOrEmpty(this.IsOpen) == false)
                this.Button1.Visible = true;


            this.Pub1.Add("<Table style='border:none;' >");
            foreach (DataRow drGroup in dtGroup.Rows)
            {
                string ctlIDs = "";
                string groupNo = drGroup[0].ToString();
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
                    if (group != groupNo)
                        continue;

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Attributes["onclick"] = "isChange=true;";
                    cb.Text = name;
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
                cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
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
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }

            #region 处理未分组的情况.
            bool isHaveUnGroup = false;
            if (dtObj.Columns.Count > 2)
            {
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
                    cb.Text = name;
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
            m2m.DtlObj = this.OperObj;

            DataTable dtObj = BP.DA.DBAccess.RunSQLReturnTable(mapM2M.DBOfObjs);
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
                strT += "@" + id + cb.Text;
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