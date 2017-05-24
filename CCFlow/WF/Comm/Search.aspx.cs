using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Web.Comm;
using BP;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace CCFlow.Web.Comm
{
    /// <summary>
    /// ��ѯͨ�ý���
    /// </summary>
    public partial class Search : BP.Web.WebPage
    {
        #region ����.
        public int PageIdxOfSeach
        {
            get
            {
                if (ViewState["PageIdxOfSeach"] == null)
                    return this.PageIdx;
                else
                    return 1;
            }
            set
            {
                ViewState["PageIdxOfSeach"] = value;
            }
        }
        public new Entities HisEns
        {
            get
            {
                if (this.EnsName != null)
                {
                    if (this._HisEns == null)
                        _HisEns = BP.En.ClassFactory.GetEns(this.EnsName);
                }
                return _HisEns;
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
                    str = this.Request.QueryString["EnsName"];
                if (str == null)
                    throw new Exception("������Ч��");
                return str;
            }
        }
        /// <summary>
        /// _HisEns
        /// </summary>
        public new Entities _HisEns = null;
        private Entity _HisEn = null;
        public new Entity HisEn
        {
            get
            {
                if (_HisEn == null)
                    _HisEn = this.HisEns.GetNewEntity;
                return _HisEn;
            }
        }
        /// <summary>
        /// ��ʾ��ʽ.
        /// </summary>
        public ShowWay ShowWay
        {
            get
            {
                if (Session["ShowWay"] == null)
                {
                    if (this.Request.QueryString["ShowWay"] == null)
                    {
                        Session["ShowWay"] = "2";
                    }
                    else
                    {
                        Session["ShowWay"] = this.Request.QueryString["ShowWay"];
                    }
                }
                return (ShowWay)int.Parse(Session["ShowWay"].ToString());
            }
            set
            {
                Session["ShowWay"] = (int)value;
            }
        }

        public bool IsReadonly
        {
            get
            {
                string i = this.Request.QueryString["IsReadonly"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }

        public TB TB_Key
        {
            get
            {
                return this.ToolBar1.GetTBByID("TB_Key");
            }
        }

        /// <summary>
        /// ��ǰѡ��de En
        /// </summary>
        public Entity CurrentSelectEnDa
        {
            get
            {
                Entity en = this.HisEn;
                en.Retrieve();
                return en;
            }
        }
        public bool IsShowGroup
        {
            get
            {
                if (this.Request.QueryString["IsShowGroup"] == null)
                {
                    return true;
                }
                else
                {
                    if (this.Request.QueryString["IsShowGroup"] == "0")
                        return false;
                    else
                        return true;
                }
            }
        }
        public bool IsShowToolBar1
        {
            get
            {
                string str = this.Request.QueryString["IsShowToolBar1"];
                if (str == null || str == "1")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// �����ֶ�
        /// </summary>
        public string SortBy
        {
            get
            {
                return Request.QueryString["SortBy"] ?? "";
            }
        }
        /// <summary>
        /// ����ʽ��ASC/DESC
        /// </summary>
        public string SortType
        {
            get
            {
                var t = Request.QueryString["SortType"] ?? "ASC";
                return t.ToUpper();
            }
        }
        /// <summary>
        /// URL�й��������ƴ���ַ���
        /// </summary>
        public string SortString
        {
            get
            {
                return "&SortBy=" + SortBy + "&SortType=" + SortType;
            }
        }
        #endregion ����.

        #region װ�ط���. Page_Load
        protected void Page_Load(object sender, System.EventArgs e)
        {
            UAC uac = this.HisEn.HisUAC;
            if (uac.IsView == false)
                throw new Exception("��û�в鿴[" + this.HisEn.EnDesc + "]���ݵ�Ȩ��.");

            if (this.IsReadonly)
            {
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
            }

            if (this.Request.QueryString["PageIdx"] == null)
                this.PageIdx = 1;
            else
                this.PageIdx = int.Parse(this.Request.QueryString["PageIdx"]);

            Entity en = this.HisEn;
            UIConfig cfg = new UIConfig(en);

            // edit by stone : ������ʵʩ�Ļ�ȡmap, �������û���̬�����ò�ѯ����.
            Map map = en.EnMapInTime;
            this.ShowWay = ShowWay.Dtl;
            if (uac.IsView == false)
                throw new Exception("@�Բ�����û�в鿴��Ȩ�ޣ�");

            #region ����toolbar2 �� contral  ���ò�Ѱ����.
            this.ToolBar1.InitByMapV2(map, 1);

            bool isEdit = true;
            if (this.IsReadonly)
                isEdit = false;
            if (uac.IsInsert == false)
                isEdit = false;

            string js = "ShowEn('./RefFunc/UIEn.aspx?EnsName=" + this.EnsName + "&inlayer=1','cd','" + cfg.WinCardH + "' , '" + cfg.WinCardW + "');";
            if (isEdit)
                this.ToolBar1.AddLinkBtn(NamesOfBtn.New, "�½�", js);

            js = "OpenAttrs('" + this.EnsName + "');";

            if (WebUser.No == "admin")
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Setting, "����", js);

            if (uac.IsExp)
            {
                js = "DoExp();";
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Excel, "����", js);
            }

            if (uac.IsImp)
            {
                js = "OpenDialogAndCloseRefresh('./Sys/EnsDataIO.aspx?EnsName=" + this.EnsName + "&Step=1&t=' + Math.random(), '��������', 720, 500, 'icon-insert');";
                this.ToolBar1.AddLinkBtn("Btn_Import", "����", js);
                this.ToolBar1.GetLinkBtnByID("Btn_Import").SetDataOption("iconCls", "'icon-insert'");
            }

            #endregion

            #region ����ѡ��� Ĭ��ֵ
            AttrSearchs searchs = map.SearchAttrs;
            bool isChange = false;
            foreach (AttrSearch attr in searchs)
            {
                string mykey = this.Request.QueryString[attr.Key];
                if (mykey == "" || mykey == null)
                    continue;
                else
                {
                    this.ToolBar1.GetDDLByKey("DDL_" + attr.Key).SetSelectItem(mykey, attr.HisAttr);
                    isChange = true;
                }
            }

            if (this.Request.QueryString["Key"] != null && this.DoType != "Exp")
            {
                this.ToolBar1.GetTBByID("TB_Key").Text = this.Request.QueryString["Key"];
                isChange = true;
            }

            if (isChange == true)
            {
                /*����Ǳ�����*/
                this.ToolBar1.SaveSearchState(this.EnsName, null);
            }
            #endregion

            this.SetDGData();
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Search).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            // this.Label1.Text = this.GenerCaption(this.HisEn.EnMap.EnDesc + "" + this.HisEn.EnMap.TitleExt);

            //��ʱ�ļ���
            this.expFileName.Value = this.HisEns.GetNewEntity.EnDesc + "���ݵ���" + "_" + BP.DA.DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";
        }
        #endregion װ�ط���. Page_Load

        #region ����
        public Entities SetDGData()
        {
            return this.SetDGData(this.PageIdx);
        }
        public Entities SetDGData(int pageIdx)
        {
            Entities ens = this.HisEns;
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;
            QueryObject qo = new QueryObject(ens);
            qo = this.ToolBar1.GetnQueryObject(ens, en);

            if (this.DoType == "Exp")
            {
                if (en.HisUAC.IsExp == false)
                {
                    this.WinCloseWithMsg("��û�����ݵ���Ȩ�ޣ�����ϵ����Ա�����");
                    return null;
                }

                #region //������excel
                string name = en.EnDesc + "���ݵ���" + "_" + DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";
                string filepath = BP.Sys.SystemConfig.PathOfTemp;
                string filename = null;
                long len = 0;
                DataTable dt = qo.DoQueryToTable();

                if (Directory.Exists(filepath) == false)
                    Directory.CreateDirectory(filepath);

                filename = filepath + name;

                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ISheet sheet = wb.CreateSheet("Sheet1");
                    sheet.DefaultRowHeightInPoints = 20;
                    IRow row = sheet.CreateRow(1);
                    IRow frow = sheet.CreateRow(0);
                    IRow lrow = sheet.CreateRow(2 + dt.Rows.Count);
                    ICell cell = null;
                    IFont font = null;
                    int r = 0;
                    int c = 0;
                    IDataFormat fmt = wb.CreateDataFormat();
                    Attrs attrs = en.EnMap.Attrs;
                    Attrs selectedAttrs = null;
                    UIConfig cfg = new UIConfig(en);
                    float charWidth = 0;

                    //�б��ⵥԪ����ʽ�趨
                    ICellStyle titleStyle = wb.CreateCellStyle();
                    titleStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;
                    font = wb.CreateFont();
                    font.IsBold = true;
                    titleStyle.SetFont(font);

                    //�ļ����ⵥԪ����ʽ�趨
                    ICellStyle headerStyle = wb.CreateCellStyle();
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;
                    font = wb.CreateFont();
                    font.FontHeightInPoints = 12;
                    font.IsBold = true;
                    headerStyle.SetFont(font);

                    //�Ʊ��˵�Ԫ����ʽ�趨
                    ICellStyle userStyle = wb.CreateCellStyle();
                    userStyle.Alignment = HorizontalAlignment.Right;
                    userStyle.VerticalAlignment = VerticalAlignment.Center;

                    //��Ԫ����ʽ�趨
                    ICellStyle cellStyle = wb.CreateCellStyle();
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    //���ڵ�Ԫ����ʽ�趨
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.CloneStyleFrom(cellStyle);
                    dateCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d;@");

                    //����ʱ�䵥Ԫ����ʽ�趨
                    ICellStyle timeCellStyle = wb.CreateCellStyle();
                    timeCellStyle.CloneStyleFrom(cellStyle);
                    timeCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d h:mm;@");

                    //һ���ַ������ؿ�ȣ���Arial��10����i���в���
                    using (Bitmap bmp = new Bitmap(10, 10))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            charWidth = g.MeasureString("i", new Font("Arial", 10)).Width;
                        }
                    }

                    //Ҫ��ʾ����
                    if (cfg.ShowColumns.Length == 0)
                    {
                        selectedAttrs = attrs;
                    }
                    else
                    {
                        selectedAttrs = new Attrs();

                        foreach (Attr attr in attrs)
                        {
                            bool contain = false;

                            foreach (string col in cfg.ShowColumns)
                            {
                                if (col == attr.Key)
                                {
                                    contain = true;
                                    break;
                                }
                            }

                            if (contain)
                                selectedAttrs.Add(attr);
                        }
                    }
                    //����б���ͷ
                    row.HeightInPoints = 20;

                    foreach (Attr attr in selectedAttrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.Key == "MyNum")
                            continue;

                        cell = row.CreateCell(c++);
                        cell.SetCellValue(attr.Desc);
                        cell.CellStyle = titleStyle;
                        sheet.SetColumnWidth(c - 1, (int)(Math.Ceiling(attr.UIWidthInt / charWidth) + 0.72) * 256);

                        frow.CreateCell(c - 1);
                        lrow.CreateCell(c - 1);
                    }
                    //����ļ�����ͷ
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, c - 1));
                    cell = frow.GetCell(0);
                    cell.SetCellValue(en.EnDesc);
                    cell.CellStyle = headerStyle;
                    frow.HeightInPoints = 26;
                    //����Ʊ���
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2 + dt.Rows.Count, 2 + dt.Rows.Count, 0, c - 1));
                    cell = lrow.GetCell(0);
                    cell.SetCellValue("�Ʊ��ˣ�" + WebUser.Name);
                    cell.CellStyle = userStyle;
                    lrow.HeightInPoints = 20;

                    r = 2;
                    //�����ѯ���
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = sheet.CreateRow(r++);
                        row.HeightInPoints = 20;
                        c = 0;

                        foreach (Attr attr in selectedAttrs)
                        {
                            if (attr.UIVisible == false)
                                continue;

                            if (attr.Key == "MyNum")
                                continue;

                            cell = row.CreateCell(c++);

                            if (attr.IsFKorEnum)
                            {
                                cell.CellStyle = cellStyle;
                                cell.SetCellValue(dr[attr.Key + "Text"] as string);
                            }
                            else
                            {
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppBoolean:
                                        cell.CellStyle = cellStyle;
                                        cell.SetCellValue(dr[attr.Key].Equals(1) ? "��" : "��");
                                        break;
                                    case DataType.AppDate:
                                        cell.SetCellValue(dr[attr.Key] as string);
                                        cell.CellStyle = dateCellStyle;
                                        break;
                                    case DataType.AppDateTime:
                                        cell.SetCellValue(dr[attr.Key] as string);
                                        cell.CellStyle = timeCellStyle;
                                        break;
                                    case DataType.AppString:
                                        cell.CellStyle = cellStyle;
                                        cell.SetCellValue(dr[attr.Key] as string);
                                        break;
                                    case DataType.AppInt:
                                        cell.CellStyle = cellStyle;
                                        cell.SetCellValue((int)dr[attr.Key]);
                                        break;
                                    case DataType.AppFloat:
                                    case DataType.AppMoney:
                                        cell.CellStyle = cellStyle;
                                        cell.SetCellValue((double)dr[attr.Key]);
                                        break;
                                }
                            }
                        }
                    }

                    wb.Write(fs);
                    len = fs.Length;
                }

                if (!"firefox".Contains(Request.Browser.Browser.ToLower()))
                    name = HttpUtility.UrlEncode(name);
                //��������
                Response.AddHeader("Content-Length", len.ToString());
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
                Response.WriteFile(filename);
                Response.End();
                Response.Close();
                #endregion //������excel

                return null;
            }

            int maxPageNum = 0;
            try
            {
                this.UCSys2.Clear();
                maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(),
                    SystemConfig.PageSize, pageIdx, "Search.aspx?EnsName=" + this.EnsName + SortString);
                if (maxPageNum > 1)
                    this.UCSys2.Add("��ҳ��:�� �� PageUp PageDown");
            }
            catch
            {
                try
                {
                    en.CheckPhysicsTable();
                }
                catch (Exception wx)
                {
                    BP.DA.Log.DefaultLogWriteLineError(wx.Message);
                }
                maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(), SystemConfig.PageSize, pageIdx, "Search.aspx?EnsName=" + this.EnsName + SortString);
            }

            qo.ClearOrderBy();

            if (!string.IsNullOrWhiteSpace(SortBy))
            {
                string[] sortbys = SortBy.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (SortType == "DESC")
                {
                    if (sortbys.Length > 1)
                        qo.addOrderByDesc(sortbys[0], sortbys[1]);
                    else
                        qo.addOrderByDesc(sortbys[0]);
                }
                else
                {
                    if (sortbys.Length > 1)
                        qo.addOrderBy(sortbys[0], sortbys[1]);
                    else
                        qo.addOrderBy(SortBy);
                }
            }

            qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx, string.IsNullOrWhiteSpace(SortBy) ? en.PK : SortBy, SortType == "DESC");

            if (map.IsShowSearchKey)
            {
                string keyVal = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();
                if (keyVal.Length >= 1)
                {
                    Attrs attrs = map.Attrs;
                    foreach (Entity myen in ens)
                    {
                        foreach (Attr attr in attrs)
                        {
                            if (attr.IsFKorEnum)
                                continue;

                            if (attr.IsPK)
                                continue;

                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                case DataType.AppInt:
                                case DataType.AppFloat:
                                case DataType.AppDouble:
                                case DataType.AppBoolean:
                                    continue;
                                default:
                                    break;
                            }
                            myen.SetValByKey(attr.Key, myen.GetValStrByKey(attr.Key).Replace(keyVal, "<font color=red>" + keyVal + "</font>"));
                        }
                    }
                }
            }

            this.UCSys1.DataPanelDtl(ens, null);

            int ToPageIdx = this.PageIdx + 1;
            int PPageIdx = this.PageIdx - 1;

            this.UCSys1.Add("<SCRIPT language=javascript>");
            this.UCSys1.Add("\t\n document.onkeydown = chang_page;");
            this.UCSys1.Add("\t\n function chang_page() { ");
            //  this.UCSys3.Add("\t\n  alert(event.keyCode); ");
            if (this.PageIdx == 1)
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 37 || event.keyCode == 33) alert('�Ѿ��ǵ�һҳ');");
            }
            else
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 37  || event.keyCode == 38 || event.keyCode == 33) ");
                this.UCSys1.Add("\t\n     location='Search.aspx?EnsName=" + this.EnsName + SortString + "&PageIdx=" + PPageIdx + "';");
            }

            if (this.PageIdx == maxPageNum)
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) alert('�Ѿ������һҳ');");
            }
            else
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) ");
                this.UCSys1.Add("\t\n     location='Search.aspx?EnsName=" + this.EnsName + SortString + "&PageIdx=" + ToPageIdx + "';");
            }

            this.UCSys1.Add("\t\n } ");
            this.UCSys1.Add("</SCRIPT>");
            return ens;
        }

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        #endregion

        #region �¼�.
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            try
            {
                var btn = sender as LinkBtn;
                switch (btn.ID)
                {
                    case NamesOfBtn.Insert: //���ݵ���
                        this.Response.Redirect("UIEn.aspx?EnName=" + this.HisEn.ToString(), true);
                        return;
                    case NamesOfBtn.Excel: //���ݵ���
                        Entities ens = this.HisEns;
                        Entity en = ens.GetNewEntity;
                        QueryObject qo = new QueryObject(ens);
                        qo = this.ToolBar1.GetnQueryObject(ens, en);
                        qo.DoQuery();
                        string file = "";

                        try
                        {
                            file = this.ExportDGToExcel(ens.ToDataTableDesc(), this.HisEn.EnDesc);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                file = this.ExportDGToExcel(ens.ToDataTableDescField(), this.HisEn.EnDesc);
                            }
                            catch
                            {
                                throw new Exception("����û����ȷ�������ܵ�ԭ��֮һ��:ϵͳ����Աû��ȷ�İ�װExcel�������֪ͨ�����ο���װ˵��������@ϵͳ�쳣��Ϣ��" + ex.Message);
                            }
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Excel_S: //���ݵ���.
                        Entities ens1 = this.SetDGData();
                        try
                        {
                            this.ExportDGToExcel(ens1.ToDataTableDesc(), this.HisEn.EnDesc);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("����û����ȷ�������ܵ�ԭ��֮һ��:ϵͳ����Աû��ȷ�İ�װExcel�������֪ͨ�����ο���װ˵��������@ϵͳ�쳣��Ϣ��" + ex.Message);
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Xml: //���ݵ���
                        return;
                    case "Btn_Print":  //���ݵ���.
                        return;
                    default:
                        this.PageIdx = 1;
                        this.SetDGData(1);
                        this.ToolBar1.SaveSearchState(this.EnsName, null);
                        return;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    this.ResponseWriteRedMsg(ex);
                }
            }
        }
        private bool Btn_New_ButtonClick(object sender, EventArgs e)
        {
            this.WinOpen("./RefFunc/UIEn.aspx?EnName=" + this.HisEn.ToString());
            this.SetDGData();
            return false;
        }
        #endregion �¼�.
    }
}
