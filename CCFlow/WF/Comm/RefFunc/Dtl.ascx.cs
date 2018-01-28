using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP;
namespace CCFlow.WF.Comm.RefFunc
{
    public partial class DtlUC : BP.Web.UC.UCBase3
    {
        #region 属性
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    return "BP.Edu.BTypes";
                return str;
            }
        }
        public string MainEnsName
        {
            get
            {
                return this.Request.QueryString["MainEnsName"];
            }
        }
        public string RefKey
        {
            get
            {
                return this.Request.QueryString["RefKey"];
            }
        }
        public string RefVal
        {
            get
            {
                string s = this.Request.QueryString["RefVal"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["PK"];
                if (s != null)
                    return s;


                s = this.Request.QueryString["No"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["OID"];
                if (s != null)
                    return s;

                s = this.Request.QueryString["MyPK"];
                if (s != null)
                    return s;

                return s;
            }
        }
        public  Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        public   Entities HisEns
        {
            get
            {
                Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
                return ens;
            }
        }
        public int PageSize
        {
            get
            {
                return 12;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ToolBar1.AddLinkBtn(NamesOfBtn.Save);
            this.ToolBar1.AddLinkBtn(NamesOfBtn.Delete);

            if (this.ToolBar1.IsExit(NamesOfBtn.Save))
                this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Save).Click += new EventHandler(ToolBar1_ButtonClick);

            //if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndClose))
            //    this.ToolBar1.GetLinkBtnByID(NamesOfBtn.SaveAndClose).Click += new EventHandler(ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Delete))
                this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Delete).Click += new EventHandler(ToolBar1_ButtonClick);

            if (this.ToolBar1.IsExit(NamesOfBtn.Excel))
                this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Excel).Click += new EventHandler(ToolBar1_ButtonClick);

            this.Bind();
        }

        public void Bind()
        {
            #region 生成标题
            Entity en = this.HisEn;
            en.SetValByKey(this.RefKey, this.RefVal);

            Map map = this.HisEn.EnMap;
            Attrs attrs = map.Attrs;

            //是否打开附件？
            bool isFJ = false;
            if (attrs.Contains("MyFileName"))
                isFJ = true;

            //是否打开卡片.
            bool isOpenCard = false;
            if (attrs.Count > 12)
                isOpenCard = true;

            this.ucsys1.AddTable();
            this.ucsys1.AddTR();
            this.ucsys1.AddTDTitle();

            string str1 = "<INPUT id='checkedAll' onclick='SelectAll(this);' type='checkbox' name='checkedAll'>";
            this.ucsys1.AddTDTitle(str1);
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.ucsys1.AddTDTitle(attr.Desc);
            }
            if (isFJ)
                this.ucsys1.AddTDTitle();
            if (isOpenCard)
                this.ucsys1.AddTDTitle();

            this.ucsys1.AddTREnd();
            #endregion 生成标题

            this.Page.Title = en.EnDesc;

            Entities dtls = this.HisEns;
            QueryObject qo = new QueryObject(dtls);
            qo.AddWhere(this.RefKey, this.RefVal);


            #region 生成翻页
            this.ucsys2.Clear();
            try
            {
                this.ucsys2.BindPageIdx(qo.GetCount(), BP.Sys.SystemConfig.PageSize, this.PageIdx, "Dtl.aspx?EnName=" + this.EnName + "&PK=" + this.RefVal + "&EnsName=" + this.EnsName + "&RefVal=" + this.RefVal + "&RefKey=" + this.RefKey + "&MainEnsName=" + this.MainEnsName);
                qo.DoQuery(en.PK, this.PageSize, this.PageIdx, false);
            }
            catch
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                //   this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
                return;
            }
            #endregion 生成翻页

            UAC uac = en.HisUAC;
            if (uac.IsDelete == false)
                this.ToolBar1.GetBtnByID(NamesOfBtn.Delete).Enabled = false;

            if (uac.IsInsert)
            {
                en.PKVal = "0";
                dtls.AddEntity(en);
            }

            DDL ddl = new DDL();
            CheckBox cb = new CheckBox();
            bool is1 = false;

            #region 生成数据
            int i = 0;
            foreach (Entity dtl in dtls)
            {
                i++;
                if (dtl.PKVal.ToString() == "0" || dtl.PKVal.ToString() == "")
                {
                    this.ucsys1.AddTRSum();
                    this.ucsys1.AddTD("colspan=2", "<b>*</B>");
                }
                else
                {
                    //  is1 = this.ucsys1.AddTR(is1, "ondblclick=\"WinOpen( 'UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + dtl.PKVal + "', 'cd' )\"");
                    is1 = this.ucsys1.AddTR(is1);

                    //  is1 = this.ucsys1.AddTR(is1);
                    this.ucsys1.AddTDIdx(i);
                    cb = new CheckBox();
                    cb.ID = "CB_" + dtl.PKVal;
                    this.ucsys1.AddTD(cb);
                }

                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "OID")
                        continue;

                    string val = dtl.GetValByKey(attr.Key).ToString();
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            TB tb = new TB();
                            this.ucsys1.AddTD(tb);
                            tb.LoadMapAttr(attr);
                            tb.ID = "TB_" + attr.Key + "_" + dtl.PKVal;
                            tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                    tb.TextExtMoney = decimal.Parse(val);
                                    break;

                                case DataType.AppDate:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Date;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case DataType.AppDateTime:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.DateTime;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                    break;
                                default:
                                    tb.Text = val;
                                    break;
                            }

                            if (attr.IsNum && attr.IsFKorEnum == false)
                            {
                                if (tb.Enabled)
                                {
                                    // OnKeyPress="javascript:return VirtyNum(this);"
                                    //  tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";
                                    // tb.Attributes["onkeyup"] += "javascript:C" + dtl.PKVal + "();C" + attr.Key + "();";
                                    tb.Attributes["class"] = "TBNum";
                                }
                                else
                                {
                                    //   tb.Attributes["onpropertychange"] += "C" + attr.Key + "();";
                                    tb.Attributes["class"] = "TBNumReadonly";
                                }
                            }
                            break;
                        case UIContralType.DDL:
                            ddl = new DDL();
                            ddl.ID = "DDL_" + attr.Key + "_" + dtl.PKVal;
                            if (attr.UIIsReadonly == false)
                            {
                                ddl.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.Key), val));
                                ddl.Enabled = false;
                            }
                            else
                            {
                                if (attr.IsEnum)
                                {
                                    ddl.BindSysEnum(attr.UIBindKey);
                                }
                                else
                                {
                                    ddl.BindEntities(attr.HisFKEns, attr.UIRefKeyValue, attr.UIRefKeyText);
                                }
                            }
                            this.ucsys1.AddTD(ddl);
                            ddl.SetSelectItem(val);
                            break;
                        case UIContralType.CheckBok:
                            cb = new CheckBox();
                            cb.ID = "CB_" + attr.Key + "_" + dtl.PKVal;
                            cb.Text = attr.Desc;
                            if (val == "1")
                                cb.Checked = true;
                            else
                                cb.Checked = false;
                            this.ucsys1.AddTD("nowarp=true", cb);
                            break;
                        default:
                            break;
                    }
                }

                if (isFJ)
                {
                    string ext = dtl.GetValStrByKey("MyFileExt");
                    if (ext != "")
                        this.ucsys1.AddTD("<img src='../Images/FileType/" + ext + ".gif' border=0/>" + dtl.GetValStrByKey("MyFileName"));
                }

                if (isOpenCard)
                    this.ucsys1.AddTD("<a href=\"javascript:WinOpen('/WF/Comm/En.htm?EnName=BP.LI.ZhiBiaoFXFF&PK=" + dtl.PKVal + "')\" >打开</a>");

                this.ucsys1.AddTREnd();
            }

            #region 生成合计
            //this.ucsys1.AddTRSum();
            //this.ucsys1.AddTD("colspan=2", "合计");
            //foreach (Attr attr in attrs)
            //{
            //    if (attr.UIVisible == false)
            //        continue;
            //    if (attr.IsNum && attr.IsFKorEnum == false)
            //    {
            //        TB tb = new TB();
            //        tb.ID = "TB_" + attr.Key;
            //        tb.Text = attr.DefaultVal.ToString();
            //        tb.ShowType = attr.HisTBType;
            //        tb.ReadOnly = true;
            //        tb.Font.Bold = true;
            //        tb.BackColor = System.Drawing.Color.FromName("infobackground");

            //        switch (attr.MyDataType)
            //        {
            //            case DataType.AppRate:
            //            case DataType.AppMoney:
            //                tb.TextExtMoney = dtls.GetSumDecimalByKey(attr.Key);
            //                break;
            //            case DataType.AppInt:
            //                tb.TextExtInt = dtls.GetSumIntByKey(attr.Key);
            //                break;
            //            case DataType.AppFloat:
            //                tb.TextExtFloat = dtls.GetSumFloatByKey(attr.Key);
            //                break;
            //            default:
            //                break;
            //        }
            //        this.ucsys1.AddTD(tb);
            //    }
            //    else
            //    {
            //        this.ucsys1.AddTD();
            //    }
            //}
            //this.ucsys1.AddTD();
            //this.ucsys1.AddTREnd();
            #endregion 生成合计

            #endregion 生成数据

            this.ucsys1.AddTableEnd();
        }
        public void Save(bool isclose)
        {
            Entities dtls = BP.En.ClassFactory.GetEns(this.EnsName);
            Entity en = dtls.GetNewEntity;
            QueryObject qo = new QueryObject(dtls);
            qo.AddWhere(this.RefKey, this.RefVal);
            int num = qo.DoQuery(en.PK, this.PageSize, this.PageIdx, false);

            // qo.DoQuery(en.PK, 12, this.PageIdx, false);
            Map map = dtls.GetNewEntity.EnMap;

            int idx = 0;
            string msg = "";
            foreach (Entity dtl in dtls)
            {
                try
                {
                    idx++;

                    this.ucsys1.Copy(dtl, dtl.PKVal.ToString(), map);
                    dtl.SetValByKey(this.RefKey, this.RefVal);
                    dtl.Update();

                }
                catch (Exception ex)
                {
                    msg += "@在保存(" + idx + ")行出现错误：" + ex.Message;
                }
            }

            en = this.ucsys1.Copy(en, "0", map);
            en.PKVal = "";
            bool isInsert = false;
            if (en.IsBlank == false)
            {
                if (en.IsNoEntity)
                {
                    if (en.EnMap.IsAutoGenerNo)
                        en.SetValByKey("No", en.GenerNewNoByKey("No"));
                }

                en.SetValByKey(this.RefKey, this.RefVal);
                try
                {
                    en.Insert();
                    isInsert = true;
                }
                catch (Exception ex)
                {
                    msg += "@在插入新行时出现错误：" + ex.Message;
                }
            }

            if (msg.Length > 2)
            {
                throw new Exception(msg);
            }

            if (isclose)
            {
                this.WinClose();
                return;
            }

            int pageIdx = this.PageIdx;
            if (isInsert == true && num == this.PageSize)
                pageIdx++;

            this.Response.Redirect("Dtl.aspx?EnName=" + this.EnName + "&PK=" + this.RefVal + "&EnsName=" + this.EnsName + "&RefVal=" + this.RefVal + "&RefKey=" + this.RefKey + "&PageIdx=" + pageIdx + "&MainEnsName=" + this.MainEnsName, true);
        }
        public void ExpExcel()
        {
            //Entity mdtl = this.HisEn;
            //this.GenerLabel(this.Label1, mdtl.EnDesc);
            //this.Title = mdtl.Name;

            //GEDtls dtls = this.HisEns;
            //QueryObject qo = null;
            //qo = new QueryObject(dtls);
            //qo.DoQuery();

            ////DataTable dt = dtls.ToDataTableDesc();
            //// this.GenerExcel(dtls.ToDataTableDesc(), mdtl.Name + ".xls");
            //this.GenerExcel_pri_Text(dtls.ToDataTableDesc(), mdtl.Name + "@" + WebUser.No + "@" + DataType.CurrentData + ".xls");

            //this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //dtls.GetNewEntity.CheckPhysicsTable();
            //this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
        }
        void ToolBar1_ButtonClick(object sender, EventArgs e)
        {
            var btn = sender as LinkBtn;
            try
            {
                switch (btn.ID)
                {
                    case NamesOfBtn.New:
                    case NamesOfBtn.Save:
                    case NamesOfBtn.SaveAndNew:
                        this.Save(false);
                        break;
                    case NamesOfBtn.SaveAndClose:
                        this.Save(true);
                        break;
                    case NamesOfBtn.Delete:
                        Entities dtls = BP.En.ClassFactory.GetEns(this.EnsName);
                        QueryObject qo = new QueryObject(dtls);
                        qo.AddWhere(this.RefKey, this.RefVal);
                        qo.DoQuery("OID", BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
                        foreach (Entity dtl in dtls)
                        {
                            CheckBox cb = this.ucsys1.GetCBByID("CB_" + dtl.PKVal);
                            if (cb == null)
                                continue;

                            if (cb.Checked)
                                dtl.Delete();
                        }
                        this.ucsys1.Clear();
                        this.Bind();
                        break;
                    case NamesOfBtn.Excel:
                        this.ExpExcel();
                        break;
                    default:
                        BP.Sys.PubClass.Alert("当前版本不支持此功能。");
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        /// <summary>
        /// 生成列的计算
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string GenerAutoFull(string pk, MapAttrs attrs, MapAttr attr)
        {
            return "";
            //string left = "\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value = ";
            //string right = attr.AutoFullDoc;
            //foreach (MapAttr mattr in attrs)
            //{
            //    string tbID = "TB_" + mattr.Key + "_" + pk;
            //    TB tb = this.ucsys1.GetTBByID(tbID);
            //    if (tb == null)
            //        continue;
            //    right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
            //    right = right.Replace("@" + mattr.Key, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
            //}

            //string s = left + right;
            //s += "\t\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value ) ;";
            //return s += " C" + attr.Key + "();";
        }
        public string GenerSum(MapAttr mattr, GEDtls dtls)
        {
            return "";
        }
    }
}