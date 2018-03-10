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
    /// 查询通用界面
    /// </summary>
    public partial class Search : BP.Web.WebPage
    {
        #region 属性.
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
                    throw new Exception("类名无效。");
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
        /// 显示方式.
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
        /// 当前选择de En
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
        /// 排序字段
        /// </summary>
        public string SortBy
        {
            get
            {
                return Request.QueryString["SortBy"] ?? "";
            }
        }
        /// <summary>
        /// 排序方式，ASC/DESC
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
        /// URL中关于排序的拼接字符串
        /// </summary>
        public string SortString
        {
            get
            {
                return "&SortBy=" + SortBy + "&SortType=" + SortType;
            }
        }
        #endregion 属性.

        #region 装载方法. Page_Load
        protected void Page_Load(object sender, System.EventArgs e)
        {
            UAC uac = this.HisEn.HisUAC;
            if (uac.IsView == false)
                throw new Exception("您没有查看[" + this.HisEn.EnDesc + "]数据的权限.");

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

            // edit by stone : 增加了实施的获取map, 可以让用户动态的设置查询条件.
            Map map = en.EnMapInTime;
            this.ShowWay = ShowWay.Dtl;
            if (uac.IsView == false)
                throw new Exception("@对不起，您没有查看的权限！");

            #region 设置toolbar2 的 contral  设置查寻功能.
            this.ToolBar1.InitByMapV2(map, 1);

            bool isEdit = true;
            if (this.IsReadonly)
                isEdit = false;
            if (uac.IsInsert == false)
                isEdit = false;

            string js = "ShowEn('./RefFunc/UIEn.aspx?EnsName=" + this.EnsName + "&inlayer=1','cd','" + cfg.WinCardH + "' , '" + cfg.WinCardW + "');";
            if (isEdit)
                this.ToolBar1.AddLinkBtn(NamesOfBtn.New, "新建", js);

            js = "OpenAttrs('" + this.EnsName + "');";

            if (WebUser.No == "admin")
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Setting, "设置", js);

            if (uac.IsExp)
            {
                js = "DoExp();";
                this.ToolBar1.AddLinkBtn(NamesOfBtn.Excel, "导出", js);
            }

            if (uac.IsImp)
            {
                js = "OpenDialogAndCloseRefresh('./Sys/EnsDataIO.aspx?EnsName=" + this.EnsName + "&Step=1&t=' + Math.random(), '导入数据', 720, 500, 'icon-insert');";
                this.ToolBar1.AddLinkBtn("Btn_Import", "导入", js);
                this.ToolBar1.GetLinkBtnByID("Btn_Import").SetDataOption("iconCls", "'icon-insert'");
            }

            #endregion

            #region 设置选择的 默认值
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
                /*如果是保存了*/
                this.ToolBar1.SaveSearchState(this.EnsName, null);
            }
            #endregion

            this.SetDGData();
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Search).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            // this.Label1.Text = this.GenerCaption(this.HisEn.EnMap.EnDesc + "" + this.HisEn.EnMap.TitleExt);

            //临时文件名
            this.expFileName.Value = this.HisEns.GetNewEntity.EnDesc + "数据导出" + "_" + BP.DA.DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";
        }
        #endregion 装载方法. Page_Load

        #region 方法
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

            #region 导出.
            if (this.DoType == "Exp")
            {
                if (en.HisUAC.IsExp == false)
                {
                    this.WinCloseWithMsg("您没有数据导出权限，请联系管理员解决！");
                    return null;
                }

                #region //导出到excel
                string name = en.EnDesc + "数据导出" + "_" + DataType.CurrentDataCNOfLong + "_" + WebUser.Name + ".xls";
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

                    //列标题单元格样式设定
                    ICellStyle titleStyle = wb.CreateCellStyle();
                    titleStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    titleStyle.VerticalAlignment = VerticalAlignment.Center;
                    font = wb.CreateFont();
                    font.IsBold = true;
                    titleStyle.SetFont(font);

                    //文件标题单元格样式设定
                    ICellStyle headerStyle = wb.CreateCellStyle();
                    headerStyle.Alignment = HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = VerticalAlignment.Center;
                    font = wb.CreateFont();
                    font.FontHeightInPoints = 12;
                    font.IsBold = true;
                    headerStyle.SetFont(font);

                    //制表人单元格样式设定
                    ICellStyle userStyle = wb.CreateCellStyle();
                    userStyle.Alignment = HorizontalAlignment.Right;
                    userStyle.VerticalAlignment = VerticalAlignment.Center;

                    //单元格样式设定
                    ICellStyle cellStyle = wb.CreateCellStyle();
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.VerticalAlignment = VerticalAlignment.Center;

                    //日期单元格样式设定
                    ICellStyle dateCellStyle = wb.CreateCellStyle();
                    dateCellStyle.CloneStyleFrom(cellStyle);
                    dateCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d;@");

                    //日期时间单元格样式设定
                    ICellStyle timeCellStyle = wb.CreateCellStyle();
                    timeCellStyle.CloneStyleFrom(cellStyle);
                    timeCellStyle.DataFormat = fmt.GetFormat("yyyy-m-d h:mm;@");

                    //一个字符的像素宽度，以Arial，10磅，i进行测算
                    using (Bitmap bmp = new Bitmap(10, 10))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            charWidth = g.MeasureString("i", new Font("Arial", 10)).Width;
                        }
                    }

                    //要显示的列
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
                    //输出列标题头
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
                    //输出文件标题头
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, c - 1));
                    cell = frow.GetCell(0);
                    cell.SetCellValue(en.EnDesc);
                    cell.CellStyle = headerStyle;
                    frow.HeightInPoints = 26;
                    //输出制表人
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(2 + dt.Rows.Count, 2 + dt.Rows.Count, 0, c - 1));
                    cell = lrow.GetCell(0);
                    cell.SetCellValue("制表人：" + WebUser.Name);
                    cell.CellStyle = userStyle;
                    lrow.HeightInPoints = 20;

                    r = 2;
                    //输出查询结果
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
                                        cell.SetCellValue(dr[attr.Key].Equals(1) ? "是" : "否");
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
                //弹出下载
                Response.AddHeader("Content-Length", len.ToString());
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
                Response.WriteFile(filename);
                Response.End();
                Response.Close();
                #endregion //导出到excel

                return null;
            }
            #endregion 导出.


            int maxPageNum = 0;
            try
            {
                this.UCSys2.Clear();
                maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(),
                    SystemConfig.PageSize, pageIdx, "Search.aspx?EnsName=" + this.EnsName + SortString);
                if (maxPageNum > 1)
                    this.UCSys2.Add("翻页键:← → PageUp PageDown");
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

            //qo.ClearOrderBy();
            //if (!string.IsNullOrWhiteSpace(SortBy))
            //{
            //    string[] sortbys = SortBy.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //    if (SortType == "DESC")
            //    {
            //        if (sortbys.Length > 1)
            //            qo.addOrderByDesc(sortbys[0], sortbys[1]);
            //        else
            //            qo.addOrderByDesc(sortbys[0]);
            //    }
            //    else
            //    {
            //        if (sortbys.Length > 1)
            //            qo.addOrderBy(sortbys[0], sortbys[1]);
            //        else
            //            qo.addOrderBy(SortBy);
            //    }
            //}
            //qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx, string.IsNullOrWhiteSpace(SortBy) ? en.PK : SortBy, SortType == "DESC");

            qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx);


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
                this.UCSys1.Add("\t\n if (event.keyCode == 37 || event.keyCode == 33) alert('已经是第一页');");
            }
            else
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 37  || event.keyCode == 38 || event.keyCode == 33) ");
                this.UCSys1.Add("\t\n     location='Search.aspx?EnsName=" + this.EnsName + SortString + "&PageIdx=" + PPageIdx + "';");
            }

            if (this.PageIdx == maxPageNum)
            {
                this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) alert('已经是最后一页');");
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

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        #endregion

        #region 事件.
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            try
            {
                var btn = sender as LinkBtn;
                switch (btn.ID)
                {
                    case NamesOfBtn.Insert: //数据导出
                        this.Response.Redirect("UIEn.aspx?EnName=" + this.HisEn.ToString(), true);
                        return;
                    case NamesOfBtn.Excel: //数据导出
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
                                throw new Exception("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message);
                            }
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Excel_S: //数据导出.
                        Entities ens1 = this.SetDGData();
                        try
                        {
                            this.ExportDGToExcel(ens1.ToDataTableDesc(), this.HisEn.EnDesc);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message);
                        }
                        this.SetDGData();
                        return;
                    case NamesOfBtn.Xml: //数据导出
                        return;
                    case "Btn_Print":  //数据导出.
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
        #endregion 事件.
    }
}
