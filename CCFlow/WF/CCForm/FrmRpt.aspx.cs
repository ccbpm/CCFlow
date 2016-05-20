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
using BP.WF.XML;
using BP.Sys;  
using BP.Web.Controls;
namespace CCFlow.WF.CCForm
{
    public partial class Comm_FrmRpt : BP.Web.WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                if (string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return 0;

                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_MapExt
        {
            get
            {
                return this.Request.QueryString["FK_MapExt"];
            }
        }
        public new string Key
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    return "ND299Dtl";
                return str;
            }
        }
        /// <summary>
        /// 主表FK_MapData
        /// </summary>
        public string MainEnsName
        {
            get
            {
                string str = this.Request.QueryString["MainEnsName"];
                if (str == null)
                    return "ND299";
                return str;
            }
        }
        public int BlankNum
        {
            get
            {
                try
                {
                    return int.Parse(ViewState["BlankNum"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                ViewState["BlankNum"] = value;
            }
        }
        public new string RefPK
        {
            get
            {
                string str = this.Request.QueryString["RefPK"];
                return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.Request.QueryString["RefPKVal"];
                if (str == null)
                    return "1";
                return str;
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
        #endregion 属性

        public int DtlCount
        {
            get
            {
                return int.Parse(ViewState["DtlCount"].ToString());
            }
            set
            {
                ViewState["DtlCount"] = value;
            }
        }
        public int IsReadonly
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["IsReadonly"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
             "<link href='../Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            FrmRpt mdtl = new FrmRpt(this.EnsName);
            if (this.IsReadonly == 1)
            {
                mdtl._IsReadonly = 1;
                this.Button1.Enabled = false;
            }
          
            this.Bind(mdtl);
        }
        public int addRowNum
        {
            get
            {
                try
                {
                    int i = int.Parse(this.Request.QueryString["addRowNum"]);
                    if (this.Request.QueryString["IsCut"] == null)
                        return i;
                    else
                        return i;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int IsWap
        {
            get
            {
                if (this.Request.QueryString["IsWap"] == "1")
                    return 1;
                return 0;
            }
        }
        public string FK_MapData = null;
        public void Bind(FrmRpt rpt)
        {
            if (this.Request.QueryString["IsTest"] != null)
                BP.DA.Cash.SetMap(this.EnsName, null);

            this.FK_MapData = rpt.FK_MapData;

            #region 生成标题
            DataTable dtCol = BP.DA.DBAccess.RunSQLReturnTable(rpt.SQLOfColumn);
            DataTable dtRow = BP.DA.DBAccess.RunSQLReturnTable(rpt.SQLOfRow);
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序号");
            this.Pub1.AddTDTitle("项目");
            foreach (DataRow drCol in dtCol.Rows)
            {
                this.Pub1.AddTDTitle(drCol[1].ToString());
            }
            this.Pub1.AddTREnd();
            #endregion

            // 获得数据源数据.
            DataTable dt = null;
            try
            {
                string sql = "SELECT * FROM " + rpt.PTable + " WHERE RefOID=" + this.RefOID;
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            catch
            {
                BP.Sys.DataRpt rptTemplete = new DataRpt();
                rpt.CheckPhysicsTable();
                string sqlRename = "";
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        sqlRename = "EXEC SP_RENAME Sys_DataRpt, " + rpt.PTable;
                        break;

                    case DBType.Informix:
                        sqlRename = "RENAME TABLE Sys_DataRpt TO " + rpt.PTable;
                        break;

                    case DBType.Oracle:
                        sqlRename = "ALTER TABLE Sys_DataRpt rename to " + rpt.PTable;
                        break;

                    case DBType.MySQL:
                        sqlRename = "ALTER TABLE Sys_DataRpt rename to " + rpt.PTable;
                        break;
                    default:
                        throw new Exception("@未涉及到此类型.");
                }
                DBAccess.RunSQL(sqlRename);

                string sql = "SELECT * FROM " + rpt.PTable + " WHERE RefOID=" + this.RefOID;
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            }


            #region 生成单元格
            foreach (DataRow drCol in dtCol.Rows)
            {
                string noOfCol = drCol[0].ToString();
                this.Pub1.AddTR();
                foreach (DataRow drRow in dtRow.Rows)
                {
                    string noOfRow = drRow[0].ToString();
                    TextBox tb = new TextBox();
                    tb.ID = "TB_" + noOfCol + "" + noOfRow;

                    //求出此单元格数据.
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[BP.Sys.DataRptAttr.ColCount].ToString() != noOfCol)
                            continue;
                        if (dr[BP.Sys.DataRptAttr.RowCount].ToString() != noOfRow)
                            continue;

                        tb.Text = dr["Val"].ToString();
                        break;
                    }
                    this.Pub1.AddTD(tb);
                }
                this.Pub1.AddTREnd();
            }
            #endregion 生成单元格
            this.Pub1.AddTableEnd();
        }
        /// <summary>
        /// 执行保存.
        /// </summary>
        public void Save()
        {
            FrmRpt rpt = new FrmRpt(this.EnsName);
            DataTable dtCol = BP.DA.DBAccess.RunSQLReturnTable(rpt.SQLOfColumn);
            DataTable dtRow = BP.DA.DBAccess.RunSQLReturnTable(rpt.SQLOfRow);

            BP.DA.DBAccess.RunSQL("DELETE "+rpt.PTable+" WHERE RefOID="+this.RefPKVal);

            Paras ps = new Paras();
            ps.SQL = "INSERT INTO "+rpt.PTable+" ()VALUES()";
            foreach (DataRow drCol in dtCol.Rows)
            {
                string col=drCol[0].ToString();
                foreach (DataRow drRow in dtRow.Rows)
                {
                    ps.Clear();

                    string row = drRow[0].ToString();
                    string val = this.Pub1.GetTextBoxByID("TB_" + col + "" + row).Text;
                    ps.Add("Col", col);
                    ps.Add("Row", col);
                    ps.Add("Val", decimal.Parse(val));
                    ps.Add("MyPK", col + "_" + row + "_" + this.RefPKVal);
                    BP.DA.DBAccess.RunSQL(ps);
                }
            }
        }
        public void ExpExcel()
        {
        }
        void BPToolBar1_ButtonClick(object sender, EventArgs e)
        {
            //ToolbarBtn btn = sender as ToolbarBtn;
            //switch (btn.ID)
            //{
            //    case NamesOfBtn.New:
            //    case NamesOfBtn.Save:
            //    case NamesOfBtn.SaveAndNew:
            //        this.Save();
            //        break;
            //    case NamesOfBtn.SaveAndClose:
            //        this.Save();
            //        this.WinClose();
            //        break;
            //    case NamesOfBtn.Delete:
            //        GEDtls dtls = new GEDtls(this.EnsName);
            //        QueryObject qo = new QueryObject(dtls);
            //        qo.DoQuery("OID", BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
            //        foreach (GEDtl dtl in dtls)
            //        {
            //            CheckBox cb = this.Pub1.GetCBByID("CB_" + dtl.PKVal);
            //            if (cb == null)
            //                continue;

            //            if (cb.Checked)
            //                dtl.Delete();
            //        }
            //        this.Pub1.Clear();
            //        FrmRpt md = new FrmRpt(this.EnsName);
            //        this.Bind(md);
            //        break;
            //    case NamesOfBtn.Excel:
            //        this.ExpExcel();
            //        break;
            //    default:
            //        BP.Sys.PubClass.Alert("@当前版本不支持此功能。");
            //        break;
            //}
        }
        /// <summary>
        /// 生成列的计算
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string GenerAutoFull(string pk, MapAttrs attrs, MapExt ext)
        {
            string left = "\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value = ";
            string right = ext.Doc;
            foreach (MapAttr mattr in attrs)
            {
                //if (mattr.IsNum == false)
                //    continue;
                //if (mattr.LGType != FieldTypeS.Normal)
                //    continue;

                string tbID = "TB_" + mattr.KeyOfEn + "_" + pk;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");
            }
            string s = left + right;
            s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value ) ;";
            return s += " C" + ext.AttrOfOper + "();";
        }
        public string GenerSum(MapAttr mattr, GEDtls dtls)
        {
            if (dtls.Count <= 1)
                return "";

            string ClientID = "";

            try
            {
                ClientID = this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID;
            }
            catch
            {
                return "";
            }

            string left = "\n  document.forms[0]." + ClientID + ".value = ";
            string right = "";
            int i = 0;
            foreach (GEDtl dtl in dtls)
            {
                string tbID = "TB_" + mattr.KeyOfEn + "_" + dtl.OID;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                if (i == 0)
                    right += " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
                else
                    right += " +parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
                i++;
            }
            string s = left + right + " ;";
            switch (mattr.MyDataType)
            {
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppRate:
                    return s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value ) ;";
                default:
                    return s;
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            this.Save();
        }
    }

}