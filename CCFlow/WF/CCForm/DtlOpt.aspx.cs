using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.WF.XML;
using BP.Web;
using BP.Sys;
using System.IO;
namespace CCFlow.WF.CCForm
{
    public partial class WF_DtlOpt : BP.Web.WebPage
    {
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
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public string FK_MapDtl
        {
            get
            {
                return this.Request.QueryString["FK_MapDtl"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "明细选项";

            WorkOptDtlXmls xmls = new WorkOptDtlXmls();
            xmls.RetrieveAll();
            MapDtl dtl = new MapDtl(this.FK_MapDtl);

            this.Pub1.Add("\t\n<div id='tabsJ'  align='center'>");
            this.Pub1.Add("\t\n<ul>");
            foreach (WorkOptDtlXml item in xmls)
            {
                switch (item.No)
                {
                    case "UnPass":
                        if (dtl.IsEnablePass == false)
                            continue;
                        break;
                    case "SelectItems":
                        if (dtl.IsEnableSelectImp == false)
                            continue;
                        break;
                    default:
                        break;
                }
                string url = item.URL + "?DoType=" + item.No + "&WorkID=" + this.WorkID + "&FK_MapDtl=" + this.FK_MapDtl + "&FID=" + this.FID;
                this.Pub1.AddLi("<a href=\"" + url + "\" ><span>" + item.Name + "</span></a>");
            }
            this.Pub1.Add("\t\n</ul>");
            this.Pub1.Add("\t\n</div>");

            switch (this.DoType)
            {
                case "UnPass":
                    this.BindUnPass();
                    break;
                case "ExpImp":
                default:
                    this.BindExpImp();
                    break;
            }
        }
        private void BindExpImp()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);

            if (this.Request.QueryString["Flag"] == "ExpTemplete")
            {
                string file = this.Request.PhysicalApplicationPath + "DataUser\\DtlTemplete\\" + this.FK_MapDtl + ".xlsx";
                if (System.IO.File.Exists(file))
                {
                    BP.Sys.PubClass.OpenExcel(file, dtl.Name + ".xlsx");
                    this.WinClose();
                    return;
                }

                file = this.Request.PhysicalApplicationPath + "DataUser\\DtlTemplete\\" + this.FK_MapDtl + ".xls";
                if (System.IO.File.Exists(file))
                {
                    BP.Sys.PubClass.OpenExcel(file, dtl.Name + ".xls");
                    this.WinClose();
                    return;
                }

                this.WinCloseWithMsg("设计错误：流程设计人员没有把该导入的从表模版放入" + file);
                return;
            }

            if (this.Request.QueryString["Flag"] == "ExpData")
            {
                GEDtls dtls = new GEDtls(this.FK_MapDtl);
                dtls.Retrieve(GEDtlAttr.RefPK, this.WorkID);
                this.ExportDGToExcelV2(dtls, dtl.No + ".xls");
                this.WinClose();
                return;
            }

            if (dtl.IsExp)
            {
                this.Pub1.AddFieldSet("数据导出");
                this.Pub1.Add("点下面的连接进行本从表的导出，您可以根据列的需要增减列。");
                string urlExp = "DtlOpt.aspx?DoType=" + this.DoType + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_MapDtl=" + this.FK_MapDtl + "&Flag=ExpData";
                this.Pub1.Add("<a href='" + urlExp + "' target=_blank ><img src='../Img/FileType/xls.gif' border=0 /><b>导出数据</b></a>");
                this.Pub1.AddFieldSetEnd();
            }

            if (dtl.IsImp)
            {
                this.Pub1.AddFieldSet("通过Excel导入:" + dtl.Name);
                this.Pub1.Add("下载数据模版:利用数据模板导出一个数据模板，您可以在此基础上进行数据编辑，把编辑好的信息<br>在通过下面的功能导入进来，以提高工作效率。");
                string url = "DtlOpt.aspx?DoType=" + this.DoType + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_MapDtl=" + this.FK_MapDtl + "&Flag=ExpTemplete";
                this.Pub1.Add("<a href='" + url + "' target=_blank ><img src='../Img/FileType/xls.gif' border=0 />数据模版</a>");
                this.Pub1.Add("<br>");

                this.Pub1.Add("格式数据文件:");
                System.Web.UI.WebControls.FileUpload fu = new System.Web.UI.WebControls.FileUpload();
                fu.ID = "fup";
                this.Pub1.Add(fu);

                BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
                ddl.Items.Add(new ListItem("选择导入方式", "all"));
                ddl.Items.Add(new ListItem("清空方式", "0"));
                ddl.Items.Add(new ListItem("追加方式", "1"));
                ddl.ID = "DDL_ImpWay";
                this.Pub1.Add(ddl);

                Button btn = new Button();
                btn.Text = "导入";
                btn.CssClass = "Btn";
                btn.ID = "Btn_" + dtl.No;
                btn.Click += new EventHandler(btn_Click);
                this.Pub1.Add(btn);
                this.Pub1.AddFieldSetEnd();
            }

            if (dtl.IsEnableSelectImp)
            {
                this.Pub1.AddFieldSet("从数据源导入:" + dtl.Name);
                this.Pub1.Add("进入列表，选择一个或者多个记录，然后点确定按钮，执行导入。");
                string url = "DtlOpSelectItems.aspx?DoType=" + this.DoType + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_MapDtl=" + this.FK_MapDtl + "&Flag=ExpTemplete";
                this.Pub1.Add("<a href='" + url + "' target=_self ><img src='../Img/Table.gif' border=0 /><b>进入....</b></a>");
                this.Pub1.AddFieldSetEnd();
            }
        }
        void btn_Exp_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string id = btn.ID.Replace("Btn_Exp", "");

            MapDtl dtl = new MapDtl(id);
            GEDtls dtls = new GEDtls(id);
            this.ExportDGToExcelV2(dtls, dtl.Name + ".xls");
            return;
        }
        /// <summary>
        /// edited by qin 无用代码删除  逻辑修改  支持excel列名中/英文 混合  16.6.21
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                BP.Web.Controls.DDL DDL_ImpWay = (BP.Web.Controls.DDL)this.Pub1.FindControl("DDL_ImpWay");
                System.Web.UI.WebControls.FileUpload fuit = (System.Web.UI.WebControls.FileUpload)this.Pub1.FindControl("fup");
                if (DDL_ImpWay.SelectedIndex == 0)
                {
                    this.Alert("请选择导入方式.");
                    return;
                }

                string tempPath = this.Request.PhysicalApplicationPath + "Temp\\";
                if (System.IO.Directory.Exists(tempPath) == false)
                    System.IO.Directory.CreateDirectory(tempPath);

                MapDtl dtl = new MapDtl(this.FK_MapDtl);

                //求出扩展名.
                string fileName = fuit.FileName.ToLower();
                if (fileName.Contains(".xls") == false)
                {
                    this.Alert("上传的文件必须是excel文件.");
                    return;
                }
                string ext = ".xls";
                if (fileName.Contains(".xlsx"))
                    ext = ".xlsx";

                //保存临时文件.
                string file = tempPath + WebUser.No + ext;
                fuit.SaveAs(file);

                GEDtls dtls = new GEDtls(this.FK_MapDtl);
                System.Data.DataTable dt = BP.DA.DBLoad.GetTableByExt(file);

                #region 检查两个文件是否一致。 生成要导入的属性
                BP.En.Attrs attrs = dtls.GetNewEntity.EnMap.Attrs;
                BP.En.Attrs attrsExp = new BP.En.Attrs();

                bool isHave = false;
                foreach (DataColumn dc in dt.Columns)
                {
                    foreach (BP.En.Attr attr in attrs)
                    {
                        if (dc.ColumnName == attr.Desc)
                        {
                            isHave = true;
                            attrsExp.Add(attr);
                            continue;
                        }

                        if (dc.ColumnName.ToLower() == attr.Key.ToLower())
                        {
                            isHave = true;
                            attrsExp.Add(attr);
                            dc.ColumnName = attr.Desc;
                        }
                    }
                }
                if (isHave == false)
                    throw new Exception("@您导入的excel文件不符合系统要求的格式，请下载模版文件重新填入。");
                #endregion

                #region 执行导入数据.
                if (DDL_ImpWay.SelectedIndex == 1)
                    BP.DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK='" + this.WorkID + "'");

                int i = 0;
                Int64 oid = BP.DA.DBAccess.GenerOID("Dtl", dt.Rows.Count);
                string rdt = BP.DA.DataType.CurrentData;

                string errMsg = "";
                foreach (DataRow dr in dt.Rows)
                {
                    GEDtl dtlEn = dtls.GetNewEntity as GEDtl;
                    dtlEn.ResetDefaultVal();

                    foreach (BP.En.Attr attr in attrsExp)
                    {
                        if (attr.UIVisible == false || dr[attr.Desc] == DBNull.Value)
                            continue;
                        string val = dr[attr.Desc].ToString();
                        if (val == null)
                            continue;
                        val = val.Trim();
                        switch (attr.MyFieldType)
                        {
                            case FieldType.Enum:
                            case FieldType.PKEnum:
                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                bool isHavel = false;
                                foreach (SysEnum se in ses)
                                {
                                    if (val == se.Lab)
                                    {
                                        val = se.IntKey.ToString();
                                        isHavel = true;
                                        break;
                                    }
                                }
                                if (isHavel == false)
                                {
                                    errMsg += "@数据格式不规范,第(" + i + ")行，列(" + attr.Desc + ")，数据(" + val + ")不符合格式,改值没有在枚举列表里.";
                                    val = attr.DefaultVal.ToString();
                                }
                                break;
                            case FieldType.FK:
                            case FieldType.PKFK:
                                Entities ens = null;
                                if (attr.UIBindKey.Contains("."))
                                    ens = BP.En.ClassFactory.GetEns(attr.UIBindKey);
                                else
                                    ens = new GENoNames(attr.UIBindKey, "desc");

                                ens.RetrieveAll();
                                bool isHavelIt = false;
                                foreach (Entity en in ens)
                                {
                                    if (val == en.GetValStrByKey("Name"))
                                    {
                                        val = en.GetValStrByKey("No");
                                        isHavelIt = true;
                                        break;
                                    }
                                }
                                if (isHavelIt == false)
                                    errMsg += "@数据格式不规范,第(" + i + ")行，列(" + attr.Desc + ")，数据(" + val + ")不符合格式,改值没有在外键数据列表里.";
                                break;
                            default:
                                break;
                        }

                        if (attr.MyDataType == BP.DA.DataType.AppBoolean)
                        {
                            if (val.Trim() == "是" || val.Trim().ToLower() == "yes")
                                val = "1";

                            if (val.Trim() == "否" || val.Trim().ToLower() == "no")
                                val = "0";
                        }

                        dtlEn.SetValByKey(attr.Key, val);
                    }
                    dtlEn.RefPKInt = (int)this.WorkID;
                    dtlEn.SetValByKey("RDT", rdt);
                    dtlEn.SetValByKey("Rec", WebUser.No);
                    i++;

                    dtlEn.InsertAsOID(oid);
                    oid++;
                }
                #endregion 执行导入数据.

                if (string.IsNullOrEmpty(errMsg) == true)
                    this.Alert("共有(" + i + ")条数据导入成功。");
                else
                    this.Alert("共有(" + i + ")条数据导入成功，但是出现如下错误:" + errMsg);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Replace("'", "‘");
                this.Alert(msg);
            }
        }
        private void BindUnPass()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            Node nd = new Node(dtl.FK_MapData);
            MapData md = new MapData(dtl.FK_MapData);

            string starter = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
            starter = BP.DA.DBAccess.RunSQLReturnString(starter);

            GEDtls geDtls = new GEDtls(dtl.No);
            geDtls.Retrieve(GEDtlAttr.Rec, starter, "IsPass", "0");

            MapAttrs attrs = new MapAttrs(dtl.No);
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");

            if (geDtls.Count > 0)
            {
                string str1 = "<INPUT id='checkedAll' onclick='selectAll()' type='checkbox' name='checkedAll'>";
                this.Pub1.AddTDTitle(str1);
            }
            else
            {
                this.Pub1.AddTDTitle();
            }

            string spField = ",Checker,Check_RDT,Check_Note,";

            foreach (MapAttr attr in attrs)
            {
                if (attr.UIVisible == false
                    && spField.Contains("," + attr.KeyOfEn + ",") == false)
                    continue;

                this.Pub1.AddTDTitle(attr.Name);
            }
            this.Pub1.AddTREnd();
            int idx = 0;
            foreach (GEDtl item in geDtls)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + item.OID;
                this.Pub1.AddTD(cb);
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false
                        && spField.Contains("," + attr.KeyOfEn + ",") == false)
                        continue;

                    if (attr.MyDataType == BP.DA.DataType.AppBoolean)
                    {
                        this.Pub1.AddTD(item.GetValBoolStrByKey(attr.KeyOfEn));
                        continue;
                    }

                    switch (attr.UIContralType)
                    {
                        case UIContralType.DDL:
                            this.Pub1.AddTD(item.GetValRefTextByKey(attr.KeyOfEn));
                            continue;
                        default:
                            this.Pub1.AddTD(item.GetValStrByKey(attr.KeyOfEn));
                            continue;
                    }
                }
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEndWithHR();

            if (geDtls.Count == 0)
                return;

            if (nd.IsStartNode == false)
                return;

            Button btn = new Button();
            btn.ID = "Btn_Delete";
            btn.CssClass = "Btn";
            btn.Text = "批量删除";
            btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
            btn.Click += new EventHandler(btn_DelUnPass_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_Imp";
            btn.CssClass = "Btn";
            btn.Text = "导入并重新编辑(追加方式)";
            btn.Click += new EventHandler(btn_Imp_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_ImpClear";
            btn.CssClass = "Btn";
            btn.Text = "导入并重新编辑(清空方式)";
            btn.Click += new EventHandler(btn_Imp_Click);
            this.Pub1.Add(btn);
        }

        void btn_DelUnPass_Click(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            Node nd = new Node(dtl.FK_MapData);
            MapData md = new MapData(dtl.FK_MapData);

            string starter = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
            starter = BP.DA.DBAccess.RunSQLReturnString(starter);
            GEDtls geDtls = new GEDtls(this.FK_MapDtl);
            geDtls.Retrieve(GEDtlAttr.Rec, starter, "IsPass", "0");
            foreach (GEDtl item in geDtls)
            {
                if (this.Pub1.GetCBByID("CB_" + item.OID).Checked == false)
                    continue;
                item.Delete();
            }
            this.Response.Redirect(this.Request.RawUrl, true);
        }
        void btn_Imp_Click(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            Button btn = sender as Button;
            if (btn.ID.Contains("ImpClear"))
            {
                /*如果是清空方式导入。*/
                BP.DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK='" + this.WorkID + "'");
            }

            Node nd = new Node(dtl.FK_MapData);
            MapData md = new MapData(dtl.FK_MapData);

            string starter = "SELECT Rec FROM " + md.PTable + " WHERE OID=" + this.WorkID;
            starter = BP.DA.DBAccess.RunSQLReturnString(starter);
            GEDtls geDtls = new GEDtls(this.FK_MapDtl);
            geDtls.Retrieve(GEDtlAttr.Rec, starter, "IsPass", "0");

            string strs = "";
            foreach (GEDtl item in geDtls)
            {
                if (this.Pub1.GetCBByID("CB_" + item.OID).Checked == false)
                    continue;
                strs += ",'" + item.OID + "'";
            }
            if (strs == "")
            {
                this.Alert("请选择要执行的数据。");
                return;
            }
            strs = strs.Substring(1);
            BP.DA.DBAccess.RunSQL("UPDATE  " + dtl.PTable + " SET RefPK='" + this.WorkID + "',BatchID=0,Check_Note='',Check_RDT='" + BP.DA.DataType.CurrentDataTime + "', Checker='',IsPass=1  WHERE OID IN (" + strs + ")");
            this.WinClose();
        }
    }
}