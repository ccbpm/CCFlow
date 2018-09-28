using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Data;
using BP.En;
using BP.Sys;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace CCFlow.WF.Comm.Utilities
{
    public static class NpoiFuncs
    {
        /// <summary>
        /// 将DataTable输出到Excel文件
        /// <para>added by liuxc,2017-05-20</para>
        /// </summary>
        /// <param name="dt">DataTable数据
        /// <para></para>
        /// <para>DataColumn.ExtendedProperties中Key值定义如下：</para>
        /// <para>width:列宽（像素）</para>
        /// <para>sum:是否合计该列</para>
        /// <para>isdate:是否是日期格式（false为日期时间格式）</para>
        /// <para>k:此列使用千位分隔符（只对数值格式列有效）</para>
        /// <para>dots:该列显示的小数位数（只对数值格式列有效）</para>
        /// <para></para>
        /// </param>
        /// <param name="filename">导出Excel文件的保存路径，为本地绝对路径</param>
        /// <param name="header">文件标题，null则不显示文件标题行</param>
        /// <param name="creator">创建人，null则不显示创建人行</param>
        /// <param name="date">是否显示导出日期行</param>
        /// <param name="index">是否自动添加“序”号列</param>
        /// <param name="download">生成文件后，是否自动下载</param>
        /// <returns></returns>
        public static string DataTableToExcel(DataTable dt, string filename, string header = null,
            string creator = null, bool date = false, bool index = true, bool download = false)
        {
            string dir = Path.GetDirectoryName(filename);
            string name = Path.GetFileName(filename);
            long len = 0;
            IRow row = null, headerRow = null, dateRow = null, sumRow = null, creatorRow = null;
            ICell cell = null;
            int r = 0;
            int c = 0;
            int headerRowIndex = 0; //文件标题行序
            int dateRowIndex = 0;   //日期行序
            int titleRowIndex = 0;  //列标题行序
            int sumRowIndex = 0;    //合计行序
            int creatorRowIndex = 0;    //创建人行序
            float DEF_ROW_HEIGHT = 20;  //默认行高
            float charWidth = 0;    //单个字符宽度
            int columnWidth = 0;    //列宽，像素
            bool isDate;    //是否是日期格式，否则是日期时间格式
            int decimalPlaces = 2;  //小数位数
            bool qian;  //是否使用千位分隔符
            List<int> sumColumns = new List<int>();   //合计列序号集合
            
            string deleteSql = "DELETE FROM {0} WHERE {1} = '{2}'";

            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            //一个字符的像素宽度，以Arial，10磅，i进行测算
            using (Bitmap bmp = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    charWidth = g.MeasureString("i", new Font("Arial", 10)).Width;
                }
            }
            //序
            if (index && dt.Columns.Contains("序") == false)
            {
                dt.Columns.Add("序", typeof(string)).ExtendedProperties.Add("width", 50);
                dt.Columns["序"].SetOrdinal(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                    dt.Rows[i]["序"] = (i + 1).ToString();
            }
            //合计列
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ExtendedProperties.ContainsKey("sum") == false)
                    continue;

                sumColumns.Add(col.Ordinal);
            }

            headerRowIndex = string.IsNullOrWhiteSpace(header) ? -1 : 0;
            dateRowIndex = date ? (headerRowIndex + 1) : -1;
            titleRowIndex = date
                                        ? dateRowIndex + 1
                                        : headerRowIndex == -1 ? 0 : 1;
            sumRowIndex = sumColumns.Count == 0 ? -1 : titleRowIndex + dt.Rows.Count + 1;
            creatorRowIndex = string.IsNullOrWhiteSpace(creator)
                                  ? -1
                                  : sumRowIndex == -1 ? titleRowIndex + dt.Rows.Count + 1 : sumRowIndex + 1;

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    HSSFWorkbook wb = new HSSFWorkbook();
                    ISheet sheet = wb.CreateSheet("Sheet1");
                    sheet.DefaultRowHeightInPoints = DEF_ROW_HEIGHT;
                    IFont font = wb.CreateFont();
                    IDataFormat fmt = wb.CreateDataFormat();

                    if (headerRowIndex != -1)
                        headerRow = sheet.CreateRow(headerRowIndex);
                    if (date)
                        dateRow = sheet.CreateRow(dateRowIndex);
                    if (sumRowIndex != -1)
                        sumRow = sheet.CreateRow(sumRowIndex);
                    if (creatorRowIndex != -1)
                        creatorRow = sheet.CreateRow(creatorRowIndex);

                    //输出列标题
                    row = sheet.CreateRow(titleRowIndex);
                    row.HeightInPoints = DEF_ROW_HEIGHT;

                    foreach (DataColumn col in dt.Columns)
                    {
                        cell = row.CreateCell(c++);
                        cell.SetCellValue(col.ColumnName);
                        font.IsBold = true;
                        cell.CellStyle = wb.CreateCellStyle();
                        cell.CellStyle.BorderTop = BorderStyle.Thin;
                        cell.CellStyle.BorderBottom = BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = BorderStyle.Thin;
                        cell.CellStyle.BorderRight = BorderStyle.Thin;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell.CellStyle.SetFont(font);

                        if (col.ColumnName == "序")
                            cell.CellStyle.Alignment = HorizontalAlignment.Center;

                        columnWidth = col.ExtendedProperties.ContainsKey("width")
                                          ? (int)col.ExtendedProperties["width"]
                                          : 100;
                        sheet.SetColumnWidth(c - 1, (int)(Math.Ceiling(columnWidth / charWidth) + 0.72) * 256);

                        if (headerRow != null)
                            headerRow.CreateCell(c - 1);
                        if (dateRow != null)
                            dateRow.CreateCell(c - 1);
                        if (sumRow != null)
                            sumRow.CreateCell(c - 1);
                        if (creatorRow != null)
                            creatorRow.CreateCell(c - 1);
                    }
                    //输出文件标题
                    if (headerRow != null)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(headerRowIndex, headerRowIndex, 0,
                                                                                dt.Columns.Count - 1));
                        cell = headerRow.GetCell(0);
                        cell.SetCellValue(header);
                        cell.CellStyle = wb.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        font = wb.CreateFont();
                        font.FontHeightInPoints = 12;
                        font.IsBold = true;
                        cell.CellStyle.SetFont(font);
                        headerRow.HeightInPoints = 26;
                    }
                    //输出日期
                    if (dateRow != null)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(dateRowIndex, dateRowIndex, 0,
                                                                                dt.Columns.Count - 1));
                        cell = dateRow.GetCell(0);
                        cell.SetCellValue("日期：" + DateTime.Today.ToString("yyyy-MM-dd"));
                        cell.CellStyle = wb.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Right;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        dateRow.HeightInPoints = DEF_ROW_HEIGHT;
                    }
                    //输出制表人
                    if (creatorRow != null)
                    {
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(creatorRowIndex, creatorRowIndex, 0,
                                                                                dt.Columns.Count - 1));
                        cell = creatorRow.GetCell(0);
                        cell.SetCellValue("制表人：" + creator);
                        cell.CellStyle = wb.CreateCellStyle();
                        cell.CellStyle.Alignment = HorizontalAlignment.Right;
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        creatorRow.HeightInPoints = DEF_ROW_HEIGHT;
                    }

                    r = titleRowIndex + 1;
                    //输出查询结果
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = sheet.CreateRow(r++);
                        row.HeightInPoints = DEF_ROW_HEIGHT;
                        c = 0;

                        foreach (DataColumn col in dt.Columns)
                        {
                            cell = row.CreateCell(c++);
                            cell.CellStyle = wb.CreateCellStyle();
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;
                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            if (col.ColumnName == "序")
                                cell.CellStyle.Alignment = HorizontalAlignment.Center;

                            switch (col.DataType.Name)
                            {
                                case "Boolean":
                                    cell.SetCellValue(Equals(dr[col.ColumnName], true) ? "是" : "否");
                                    break;
                                case "DateTime":
                                    isDate = col.ExtendedProperties.ContainsKey("isdate")
                                                 ? (bool)col.ExtendedProperties["isdate"]
                                                 : false;

                                    cell.SetCellValue(dr[col.ColumnName] as string);
                                    cell.CellStyle.DataFormat = isDate
                                                                    ? fmt.GetFormat("yyyy-m-d;@")
                                                                    : fmt.GetFormat("yyyy-m-d h:mm;@");
                                    break;
                                case "Int16":
                                case "Int32":
                                case "Int64":
                                    qian = col.ExtendedProperties.ContainsKey("k")
                                               ? (bool)col.ExtendedProperties["k"]
                                               : false;

                                    cell.SetCellValue((int)dr[col.ColumnName]);

                                    if (qian)
                                        cell.CellStyle.DataFormat = fmt.GetFormat("#,##0_ ;@");
                                    break;
                                case "Single":
                                case "Double":
                                case "Decimal":
                                    decimalPlaces = col.ExtendedProperties.ContainsKey("dots")
                                                        ? (int)col.ExtendedProperties["dots"]
                                                        : 2;
                                    qian = col.ExtendedProperties.ContainsKey("k")
                                               ? (bool)col.ExtendedProperties["k"]
                                               : false;

                                    cell.SetCellValue(((double)dr[col.ColumnName]));

                                    if (decimalPlaces > 0 && !qian)
                                        cell.CellStyle.DataFormat =
                                            fmt.GetFormat("0." + string.Empty.PadLeft(decimalPlaces, '0') + "_ ;@");
                                    else if (decimalPlaces == 0 && qian)
                                        cell.CellStyle.DataFormat = fmt.GetFormat("#,##0_ ;@");
                                    else if (decimalPlaces > 0 && qian)
                                        cell.CellStyle.DataFormat =
                                            fmt.GetFormat("#,##0." + string.Empty.PadLeft(decimalPlaces, '0') + "_ ;@");
                                    break;
                                default:
                                    cell.SetCellValue(dr[col.ColumnName] as string);
                                    break;
                            }
                        }
                    }
                    //合计
                    if (sumRow != null)
                    {
                        sumRow.HeightInPoints = DEF_ROW_HEIGHT;

                        for (c = 0; c < dt.Columns.Count; c++)
                        {
                            cell = sumRow.GetCell(c);
                            cell.CellStyle = wb.CreateCellStyle();
                            cell.CellStyle.BorderTop = BorderStyle.Thin;
                            cell.CellStyle.BorderBottom = BorderStyle.Thin;
                            cell.CellStyle.BorderLeft = BorderStyle.Thin;
                            cell.CellStyle.BorderRight = BorderStyle.Thin;
                            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;

                            if (sumColumns.Contains(c) == false)
                                continue;

                            cell.SetCellFormula(string.Format("SUM({0}:{1})",
                                                              GetCellName(c, titleRowIndex + 1),
                                                              GetCellName(c, titleRowIndex + dt.Rows.Count)));
                        }
                    }

                    wb.Write(fs);
                    len = fs.Length;
                }
                //弹出下载
                if (download)
                {
                    if (!"firefox".Contains(HttpContext.Current.Request.Browser.Browser.ToLower()))
                        name = HttpUtility.UrlEncode(name);

                    HttpContext.Current.Response.AddHeader("Content-Length", len.ToString());
                    HttpContext.Current.Response.ContentType = "application/octet-stream";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + name);
                    HttpContext.Current.Response.WriteFile(filename);
                    HttpContext.Current.Response.End();
                    HttpContext.Current.Response.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return null;
        }

        /// <summary>
        /// 根据Excel数据模板，抓取Excel文件中数据存放数据表中（用于流程中Excel附件数据提取）
        /// </summary>
        /// <param name="xlsfile">Excel附件文件路径</param>
        /// <param name="fk_excelfile">Excel数据模板编号</param>
        /// <param name="fk_frmAttachmentDB">附件记录PK</param>
        /// <param name="en">流程工作实体对象</param>
        /// <returns></returns>
        public static string FetchExcelDataIntoTable(string xlsfile, string fk_excelfile, string fk_frmAttachmentDB, Entity en)
        {
            string ext = Path.GetExtension(xlsfile).ToLower();

            if (ext != ".xls" && ext != ".xlsx")
                return "文件必须是Microsoft Excel 文件（*.xls或*.xlsx）";

            ExcelFile efile = new ExcelFile(fk_excelfile);
            ExcelSheets esheets = new ExcelSheets(fk_excelfile);
            ExcelTables etables = new ExcelTables(fk_excelfile);
            ExcelFields efields = new ExcelFields(fk_excelfile);
            ExcelSheet esheet = null;
            ExcelField tfield = null;
            IWorkbook wb = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            Dictionary<string, DataTable> datas = new Dictionary<string, DataTable>();
            Dictionary<string, Dictionary<string, ExcelField>> dataCols =
                new Dictionary<string, Dictionary<string, ExcelField>>();
            Dictionary<string, bool> dataIsDtls = new Dictionary<string, bool>();
            Dictionary<string, DataTable> refSources = new Dictionary<string, DataTable>();
            Dictionary<string, DataTable> refEnums = new Dictionary<string, DataTable>();
            DataRow drow = null;
            Type t = null;
            object val = null;
            string value = null;
            string AthDbField = "FK_FrmAttachmentDB";
            string deleteSql = "DELETE FROM {0} WHERE {1} = '{2}'";

            using (FileStream fs = new FileStream(xlsfile, FileMode.Open, FileAccess.Read))
            {
                if (ext.Equals(".xls"))
                    wb = new HSSFWorkbook(fs);
                else
                    wb = new XSSFWorkbook(fs);

                foreach (ExcelTable etable in etables)
                {
                    datas.Add(etable.No, new DataTable());
                    dataCols.Add(etable.No, new Dictionary<string, ExcelField>());
                    dataIsDtls.Add(etable.No, etable.IsDtl);

                    tfield = new ExcelField();
                    tfield.No = Guid.NewGuid().ToString();
                    tfield.FK_ExcelTable = etable.No;
                    tfield.Field = AthDbField;
                    tfield.DataType = ExcelFieldDataType.String;
                    efields.AddEntity(tfield);

                    foreach (ExcelField efield in efields)
                    {
                        if (efield.FK_ExcelTable != etable.No)
                            continue;

                        switch (efield.DataType)
                        {
                            case ExcelFieldDataType.String:
                            case ExcelFieldDataType.Date:
                            case ExcelFieldDataType.DateTime:
                            case ExcelFieldDataType.ForeignKey:
                            case ExcelFieldDataType.Enum:
                                t = typeof(string);
                                break;
                            case ExcelFieldDataType.Int:
                                t = typeof(int);
                                break;
                            case ExcelFieldDataType.Float:
                                t = typeof(float);
                                break;
                        }

                        datas[etable.No].Columns.Add(efield.Field, t);
                        dataCols[etable.No].Add(efield.Field, efield);

                        if (efield.DataType == ExcelFieldDataType.ForeignKey ||
                            efield.DataType == ExcelFieldDataType.Enum)
                            datas[etable.No].Columns.Add("FK_" + efield.Field);
                    }
                }

                try
                {
                    //数据提取
                    foreach (KeyValuePair<string, Dictionary<string, ExcelField>> def in dataCols)
                    {
                        //主表
                        if (dataIsDtls[def.Key] == false)
                        {
                            drow = datas[def.Key].NewRow();

                            foreach (KeyValuePair<string, ExcelField> ef in def.Value)
                            {
                                //固定值字段
                                if (string.IsNullOrWhiteSpace(ef.Value.FK_ExcelSheet))
                                {
                                    if (ef.Key == AthDbField)
                                    {
                                        drow[ef.Key] = fk_frmAttachmentDB;
                                        continue;
                                    }

                                    if (!en.Row.ContainsKey(ef.Value.Field))
                                        continue;

                                    try
                                    {
                                        val = en.GetValByKey(ef.Value.Field);

                                        switch (ef.Value.DataType)
                                        {
                                            case ExcelFieldDataType.String:
                                                drow[ef.Key] = (val == null ? "" : val.ToString()).Trim();
                                                break;
                                            case ExcelFieldDataType.Int:
                                                drow[ef.Key] = int.Parse((val == null ? "0" : val.ToString()).Trim());
                                                break;
                                            case ExcelFieldDataType.Float:
                                                drow[ef.Key] = float.Parse((val == null ? "0" : val.ToString()).Trim());
                                                break;
                                            case ExcelFieldDataType.Date:
                                                drow[ef.Key] = val == null ? "" : DateTime.Parse(val.ToString().Trim()).ToString("yyyy-MM-dd");
                                                break;
                                            case ExcelFieldDataType.DateTime:
                                                drow[ef.Key] = val == null ? "" : DateTime.Parse(val.ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return string.Format("{0}[{1}] 字段的值格式不正确：" + ex.Message, ef.Value.Name,
                                                                    ef.Value.Field);
                                    }
                                    continue;
                                }

                                //单元格字段
                                esheet = esheets.GetEntityByKey(ExcelSheetAttr.No, ef.Value.FK_ExcelSheet) as ExcelSheet;
                                if (esheet == null)
                                {
                                    return string.Format("ExcelSheet.No={0}的记录不存在！", ef.Value.FK_ExcelSheet);
                                }

                                sheet = wb.GetSheet(esheet.Name);
                                if (sheet == null)
                                {
                                    return string.Format("文件中不存在名称为“{0}”的Sheet表！", esheet.Name);
                                }

                                switch (ef.Value.ConfirmKind)
                                {
                                    case ConfirmKind.Cell:
                                        if (string.IsNullOrWhiteSpace(ef.Value.ConfirmCellValue))
                                            cell = sheet.GetRow(ef.Value.CellRow).GetCell(ef.Value.CellColumn);
                                        else
                                            cell = FindCell(sheet, ef.Value.ConfirmCellValue,
                                                            ef.Value.ConfirmRepeatIndex);

                                        if (cell == null)
                                        {
                                            return string.Format("{0}[{1}] 字段的对应单元格值设置不正确，未检索到此单元格！",
                                                                        ef.Value.Name,
                                                                        ef.Value.Field);
                                        }
                                        break;
                                    default:
                                        if (string.IsNullOrWhiteSpace(ef.Value.ConfirmCellValue))
                                        {
                                            return string.Format("{0}[{1}] 字段的对应单元格值不能为空！", ef.Value.Name,
                                                                        ef.Value.Field);
                                        }

                                        cell = FindCell(sheet, ef.Value.ConfirmCellValue, ef.Value.ConfirmRepeatIndex);
                                        if (cell == null)
                                        {
                                            return string.Format("{0}[{1}] 字段的对应单元格值设置不正确，未检索到此单元格！",
                                                                        ef.Value.Name,
                                                                        ef.Value.Field);
                                        }

                                        cell = GetCell(cell, ef.Value.ConfirmKind, ef.Value.ConfirmCellCount);
                                        if (cell == null)
                                        {
                                            return string.Format("{0}[{1}] 字段的单元格确认设置不正确，未检索到此单元格！",
                                                                        ef.Value.Name,
                                                                        ef.Value.Field);
                                        }

                                        break;
                                }

                                value = GetCellValue(cell, cell.CellType);
                                if (value == "^ERROR$" || value == "^UNKNOWN$")
                                {
                                    return string.Format("{0}[{1}] 字段未获取到准确的值，请检查！", ef.Value.Name,
                                                                ef.Value.Field);
                                }

                                try
                                {
                                    switch (ef.Value.DataType)
                                    {
                                        case ExcelFieldDataType.String:
                                            drow[ef.Key] = value;
                                            break;
                                        case ExcelFieldDataType.Int:
                                            drow[ef.Key] = int.Parse(value);
                                            break;
                                        case ExcelFieldDataType.Float:
                                            drow[ef.Key] = float.Parse(value);
                                            break;
                                        case ExcelFieldDataType.Date:
                                            DateTime d;
                                            if (DateTime.TryParse(value, out d))
                                                drow[ef.Key] = d.ToString("yyyy-MM-dd");
                                            else
                                                drow[ef.Key] = new DateTime(1899, 12, 30)
                                                    .AddDays(int.Parse(value))
                                                    .ToString("yyyy-MM-dd");
                                            break;
                                        case ExcelFieldDataType.DateTime:
                                            if (DateTime.TryParse(value, out d))
                                                drow[ef.Key] = d.ToString("yyyy-MM-dd HH:mm:ss");
                                            else
                                                drow[ef.Key] = new DateTime(1899, 12, 30)
                                                    .AddDays(double.Parse(value))
                                                    .ToString("yyyy-MM-dd HH:mm:ss");
                                            break;
                                        case ExcelFieldDataType.ForeignKey:
                                            if (Equals(value, DBNull.Value))
                                            {
                                                drow[ef.Key] = DBNull.Value;
                                                drow["FK_" + ef.Key] = DBNull.Value;
                                                continue;
                                            }

                                            object key = GetForeignKey(refSources, ef.Value, value);
                                            if (key == null)
                                                throw new Exception(string.Format("在{0}外键表中未找到{1}={2}的记录！",
                                                                                  ef.Value.UIBindKey,
                                                                                  ef.Value.UIRefKeyText, value));

                                            drow[ef.Key] = value;
                                            drow["FK_" + ef.Key] = key;
                                            break;
                                        case ExcelFieldDataType.Enum:
                                            if (Equals(value, DBNull.Value))
                                            {
                                                drow[ef.Key] = DBNull.Value;
                                                drow["FK_" + ef.Key] = DBNull.Value;
                                                continue;
                                            }

                                            int intKey = GetEnumIntKey(refEnums, ef.Value, value);
                                            if (intKey == -1)
                                                throw new Exception(
                                                    string.Format("在Sys_Enum枚举表中未找到EnumKey='{0}' AND Lab='{1}'的记录！",
                                                                  ef.Value.UIBindKey, value));

                                            drow[ef.Key] = value;
                                            drow["FK_" + ef.Key] = intKey;
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return string.Format("{0}[{1}] 字段的值[{2}]格式不正确：" + ex.Message, ef.Value.Name,
                                                                ef.Value.Field,
                                                                NpoiFuncs.GetCellName(cell.ColumnIndex, cell.RowIndex));
                                }
                            }

                            datas[def.Key].Rows.Add(drow);
                            continue;
                        }

                        //明细表
                        Dictionary<string, KeyValuePair<int, int>> dtlHeaders =
                            new Dictionary<string, KeyValuePair<int, int>>();
                        Dictionary<string, int> dtlHeaderSpans = new Dictionary<string, int>();
                        Dictionary<string, object> fixedValues = new Dictionary<string, object>();
                        foreach (KeyValuePair<string, ExcelField> ef in def.Value)
                        {
                            //固定值字段
                            if (string.IsNullOrWhiteSpace(ef.Value.FK_ExcelSheet))
                            {
                                if(ef.Value.Field == AthDbField)
                                {
                                    fixedValues.Add(ef.Key, fk_frmAttachmentDB);
                                    continue;
                                }

                                if (!en.Row.ContainsKey(ef.Value.Field))
                                    continue;

                                try
                                {
                                    val = en.GetValByKey(ef.Value.Field);

                                    switch (ef.Value.DataType)
                                    {
                                        case ExcelFieldDataType.String:
                                            fixedValues.Add(ef.Key, (val == null ? "" : val.ToString()).Trim());
                                            break;
                                        case ExcelFieldDataType.Int:
                                            fixedValues.Add(ef.Key, int.Parse((val == null ? "0" : val.ToString()).Trim()));
                                            break;
                                        case ExcelFieldDataType.Float:
                                            fixedValues.Add(ef.Key, float.Parse((val == null ? "0" : val.ToString()).Trim()));
                                            break;
                                        case ExcelFieldDataType.Date:
                                            fixedValues.Add(ef.Key, val == null ? "" : DateTime.Parse(val.ToString().Trim()).ToString("yyyy-MM-dd"));
                                            break;
                                        case ExcelFieldDataType.DateTime:
                                            fixedValues.Add(ef.Key, val == null ? "" : DateTime.Parse(val.ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss"));
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return string.Format("{0}[{1}] 字段的值格式不正确：" + ex.Message, ef.Value.Name,
                                                                ef.Value.Field);
                                }
                                continue;
                            }

                            //单元格字段
                            esheet = esheets.GetEntityByKey(ExcelSheetAttr.No, ef.Value.FK_ExcelSheet) as ExcelSheet;
                            if (esheet == null)
                            {
                                return string.Format("ExcelSheet.No={0}的记录不存在！", ef.Value.FK_ExcelSheet);
                            }

                            sheet = wb.GetSheet(esheet.Name);
                            if (sheet == null)
                            {
                                return string.Format("文件中不存在名称为“{0}”的Sheet表！", esheet.Name);
                            }

                            switch (ef.Value.ConfirmKind)
                            {
                                case ConfirmKind.Cell:
                                    if (string.IsNullOrWhiteSpace(ef.Value.ConfirmCellValue))
                                        cell = sheet.GetRow(ef.Value.CellRow).GetCell(ef.Value.CellColumn);
                                    else
                                        cell = FindCell(sheet, ef.Value.ConfirmCellValue, ef.Value.ConfirmRepeatIndex);

                                    if (cell == null)
                                    {
                                        return string.Format("{0}[{1}] 字段的对应单元格值设置不正确，未检索到此单元格！", ef.Value.Name,
                                                                    ef.Value.Field);
                                    }
                                    break;
                                default:
                                    if (string.IsNullOrWhiteSpace(ef.Value.ConfirmCellValue))
                                    {
                                        return string.Format("{0}[{1}] 字段的对应单元格值不能为空！", ef.Value.Name,
                                                                    ef.Value.Field);
                                    }

                                    cell = FindCell(sheet, ef.Value.ConfirmCellValue, ef.Value.ConfirmRepeatIndex);
                                    if (cell == null)
                                    {
                                        return string.Format("{0}[{1}] 字段的对应单元格值设置不正确，未检索到此单元格！", ef.Value.Name,
                                                                    ef.Value.Field);
                                    }

                                    cell = GetCell(cell, ef.Value.ConfirmKind, ef.Value.ConfirmCellCount);
                                    if (cell == null)
                                    {
                                        return string.Format("{0}[{1}] 字段的单元格确认设置不正确，未检索到此单元格！", ef.Value.Name,
                                                                    ef.Value.Field);
                                    }

                                    break;
                            }

                            dtlHeaders.Add(ef.Key, new KeyValuePair<int, int>(cell.RowIndex, cell.ColumnIndex));
                        }

                        //确定明细表是水平加载还是垂直加载
                        //水平加载时，所有标题单元格处于一列中，考虑合并单元格行情况
                        bool isHorizontal = true;
                        int begin = -1;
                        foreach (KeyValuePair<string, KeyValuePair<int, int>> dh in dtlHeaders)
                        {
                            if (begin == -1)
                            {
                                begin = GetCellEndColumnIndex(sheet.GetRow(dh.Value.Key).GetCell(dh.Value.Value));
                                continue;
                            }

                            if (begin != GetCellEndColumnIndex(sheet.GetRow(dh.Value.Key).GetCell(dh.Value.Value)))
                            {
                                isHorizontal = false;
                                break;
                            }
                        }
                        //垂直加载时，所有标题单元格处于一行中，考虑合并单元格行情况
                        bool isVertical = true;
                        if (isHorizontal)
                        {
                            isVertical = false;
                        }
                        else
                        {
                            begin = -1;
                            foreach (KeyValuePair<string, KeyValuePair<int, int>> dh in dtlHeaders)
                            {
                                if (begin == -1)
                                {
                                    begin = GetCellEndRowIndex(sheet.GetRow(dh.Value.Key).GetCell(dh.Value.Value));
                                    continue;
                                }

                                if (begin != GetCellEndRowIndex(sheet.GetRow(dh.Value.Key).GetCell(dh.Value.Value)))
                                {
                                    isVertical = false;
                                    break;
                                }
                            }
                        }

                        if (!isHorizontal && !isVertical)
                        {
                            return string.Format("明细表“{0}”标题行设置不正确，不在同一行或同一列中！", esheet.Name);
                        }

                        //各标题单元格合并单元格数量，此数据以供判断明细表结束
                        foreach (KeyValuePair<string, KeyValuePair<int, int>> dh in dtlHeaders)
                        {
                            cell = sheet.GetRow(dh.Value.Key).GetCell(dh.Value.Value);
                            KeyValuePair<int, int> span = GetCellRegion(cell, isHorizontal);
                            dtlHeaderSpans.Add(dh.Key, span.Value - span.Key);
                        }

                        //开始行号/列号
                        begin++;

                        //计算最大列号
                        int maxColumnIndex = 0;
                        for (var r = sheet.FirstRowNum; r < sheet.LastRowNum; r++)
                        {
                            row = sheet.GetRow(r);
                            if (row == null)
                                continue;

                            maxColumnIndex = Math.Max(maxColumnIndex, (int)row.LastCellNum);
                        }

                        //开始提取明细表数据
                        for (var i = begin; i <= (isHorizontal ? maxColumnIndex : sheet.LastRowNum); i++)
                        {
                            drow = datas[def.Key].NewRow();
                            bool isLast = false;
                            bool isNull = true;
                            foreach (KeyValuePair<string, KeyValuePair<int, int>> dh in dtlHeaders)
                            {
                                cell =
                                    sheet.GetRow(isHorizontal ? dh.Value.Key : i).GetCell(isHorizontal
                                                                                              ? i
                                                                                              : dh.Value.Value);
                                if (cell == null || cell.CellType == CellType.Blank)
                                {
                                    drow[dh.Key] = DBNull.Value;
                                    continue;
                                }

                                if (dataCols[def.Key][dh.Key].SkipIsNull == false)
                                    isNull = isNull && false;

                                KeyValuePair<int, int> span = GetCellRegion(cell, isHorizontal);
                                if (span.Value - span.Key != dtlHeaderSpans[dh.Key])
                                {
                                    isLast = true;
                                    break;
                                }

                                value = NpoiFuncs.GetCellValue(cell, cell.CellType);
                                if (value == "^ERROR$" || value == "^UNKNOWN$")
                                {
                                    return string.Format("Sheet“{0}”中{1}单元格的值格式不正确！", esheet.Name,
                                                                NpoiFuncs.GetCellName(cell.RowIndex, cell.ColumnIndex));
                                }

                                try
                                {
                                    switch (dataCols[def.Key][dh.Key].DataType)
                                    {
                                        case ExcelFieldDataType.String:
                                            drow[dh.Key] = value;
                                            break;
                                        case ExcelFieldDataType.Int:
                                            drow[dh.Key] = int.Parse(value);
                                            break;
                                        case ExcelFieldDataType.Float:
                                            drow[dh.Key] = float.Parse(value);
                                            break;
                                        case ExcelFieldDataType.Date:
                                            DateTime d;
                                            if (DateTime.TryParse(value, out d))
                                                drow[dh.Key] = d.ToString("yyyy-MM-dd");
                                            else
                                                drow[dh.Key] = new DateTime(1899, 12, 30)
                                                    .AddDays(int.Parse(value))
                                                    .ToString("yyyy-MM-dd");
                                            break;
                                        case ExcelFieldDataType.DateTime:
                                            if (DateTime.TryParse(value, out d))
                                                drow[dh.Key] = d.ToString("yyyy-MM-dd HH:mm:ss");
                                            else
                                                drow[dh.Key] = new DateTime(1899, 12, 30)
                                                    .AddDays(double.Parse(value))
                                                    .ToString("yyyy-MM-dd HH:mm:ss");
                                            break;
                                        case ExcelFieldDataType.ForeignKey:
                                            if (Equals(value, DBNull.Value))
                                            {
                                                drow[dh.Key] = DBNull.Value;
                                                drow["FK_" + dh.Key] = DBNull.Value;
                                                continue;
                                            }

                                            object key = GetForeignKey(refSources, dataCols[def.Key][dh.Key], value);
                                            if (key == null)
                                                throw new Exception(string.Format("在{0}外键表中未找到{1}={2}的记录！",
                                                                                  dataCols[def.Key][dh.Key].UIBindKey,
                                                                                  dataCols[def.Key][dh.Key].UIRefKeyText,
                                                                                  value));

                                            drow[dh.Key] = value;
                                            drow["FK_" + dh.Key] = key;
                                            break;
                                        case ExcelFieldDataType.Enum:
                                            if (Equals(value, DBNull.Value))
                                            {
                                                drow[dh.Key] = DBNull.Value;
                                                drow["FK_" + dh.Key] = DBNull.Value;
                                                continue;
                                            }

                                            int intKey = GetEnumIntKey(refEnums, dataCols[def.Key][dh.Key], value);
                                            if (intKey == -1)
                                                throw new Exception(
                                                    string.Format("在Sys_Enum枚举表中未找到EnumKey='{0}' AND Lab='{1}'的记录！",
                                                                  dataCols[def.Key][dh.Key].UIBindKey, value));

                                            drow[dh.Key] = value;
                                            drow["FK_" + dh.Key] = intKey;
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return string.Format("{0}[{1}] 字段的值[{2}]格式不正确：" + ex.Message,
                                                                dataCols[def.Key][dh.Key].Name, dh.Key,
                                                                NpoiFuncs.GetCellName(cell.ColumnIndex, cell.RowIndex));
                                }
                            }

                            if (isLast || isNull)
                                break;

                            //再次检测是否为空，此次增加检测程度，即当单元格值为null、空字符串、全部字符空格时，视此单元格为null
                            //当该行所有单元格，都为null时，则视此行为结束行
                            isNull = true;
                            foreach (object item in drow.ItemArray)
                            {
                                foreach (KeyValuePair<string, KeyValuePair<int, int>> dh in dtlHeaders)
                                {
                                    if (dataCols[def.Key][dh.Key].SkipIsNull)
                                        continue;

                                    if (item != DBNull.Value && !string.IsNullOrWhiteSpace(item.ToString()))
                                    {
                                        isNull = false;
                                        break;
                                    }
                                }
                            }

                            if (isNull)
                                break;

                            foreach (KeyValuePair<string, object> fv in fixedValues)
                            {
                                drow[fv.Key] = fv.Value;
                            }

                            datas[def.Key].Rows.Add(drow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return "提取数据遇到未知错误：" + ex.Message;
                }
                //提取数据完成
            }

            //数据写入表
            string tableName = string.Empty;
            string fields = string.Empty;
            string values = string.Empty;
            string sql = "INSERT INTO {0}({1}) VALUES({2})";
            string tsql = string.Empty;

            try
            {
                foreach (KeyValuePair<string, DataTable> data in datas)
                {
                    tableName = data.Key;
                    fields = string.Empty;

                    //删除原有数据
                    BP.DA.DBAccess.RunSQL(string.Format(deleteSql, tableName, AthDbField, fk_frmAttachmentDB));

                    foreach (DataColumn col in data.Value.Columns)
                        fields += col.ColumnName + ",";

                    fields = fields.TrimEnd(',');

                    foreach (DataRow dr in data.Value.Rows)
                    {
                        values = string.Empty;

                        foreach (DataColumn col in data.Value.Columns)
                        {
                            if (dr[col.ColumnName] == null || dr[col.ColumnName] == DBNull.Value)
                            {
                                values += "NULL,";
                                continue;
                            }

                            switch (col.DataType.Name)
                            {
                                case "String":
                                    values += string.Format("'{0}',", dr[col.ColumnName]);
                                    break;
                                case "Int32":
                                case "Single":
                                    values += dr[col.ColumnName] + ",";
                                    break;
                            }
                        }

                        values = values.TrimEnd(',');
                        tsql = string.Format(sql, tableName, fields, values);

                        try
                        {
                            if (1 != BP.DA.DBAccess.RunSQL(tsql))
                                throw new Exception(string.Format("向{0}表插入数据失败：" + tsql));
                        }
                        catch (Exception ex)
                        {
                            return "数据提取后存储出现错误：" + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return "提取的数据写入数据库时出现错误：" + ex.Message;
            }

            wb = null;
            return null;
        }

        public static int GetEnumIntKey(Dictionary<string, DataTable> refEnums, ExcelField field, string valueName)
        {
            if (field.DataType != ExcelFieldDataType.Enum)
                return -1;

            if (refEnums.ContainsKey(field.UIBindKey) == false)
                refEnums.Add(field.UIBindKey, BP.DA.DBAccess.RunSQLReturnTable("SELECT IntKey,Lab FROM Sys_Enum"));

            DataRow[] rows = refEnums[field.UIBindKey].Select("Lab='" + valueName + "'");

            return rows.Length > 0 ? Convert.ToInt32(rows[0]["IntKey"]) : -1;
        }

        public static object GetForeignKey(Dictionary<string, DataTable> refSources, ExcelField field, string valueName)
        {
            if (field.DataType != ExcelFieldDataType.ForeignKey)
                return null;

            if (refSources.ContainsKey(field.UIBindKey) == false)
            {
                refSources.Add(field.UIBindKey,
                               BP.DA.DBAccess.RunSQLReturnTable(string.Format("SELECT {0} No,{1} Name FROM {2}",
                                                                              field.UIRefKey, field.UIRefKeyText,
                                                                              field.UIBindKey)));
            }

            DataRow[] rows = refSources[field.UIBindKey].Select("Name='" + valueName + "'");

            return rows.Length > 0 ? rows[0]["No"] : null;
        }

        public static KeyValuePair<int, int> GetCellRegion(ICell cell, bool isHorizontal)
        {
            ISheet sheet = cell.Sheet;
            CellRangeAddress mergedCell;
            int begin = isHorizontal ? cell.RowIndex : cell.ColumnIndex;
            int end = isHorizontal ? cell.RowIndex : cell.ColumnIndex;

            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                mergedCell = sheet.GetMergedRegion(i);
                if (mergedCell.IsInRange(cell.RowIndex, cell.ColumnIndex))
                {
                    begin = isHorizontal ? mergedCell.FirstRow : mergedCell.FirstColumn;
                    end = isHorizontal ? mergedCell.LastRow : mergedCell.LastColumn;
                    break;
                }
            }

            return new KeyValuePair<int, int>(begin, end);
        }

        public static CellRangeAddress GetMergedRange(ICell cell)
        {
            ISheet sheet = cell.Sheet;
            CellRangeAddress mergedCell;

            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                mergedCell = sheet.GetMergedRegion(i);
                if (mergedCell.IsInRange(cell.RowIndex, cell.ColumnIndex))
                {
                    return mergedCell;
                }
            }

            return null;
        }

        public static int GetCellEndRowIndex(ICell cell)
        {
            ISheet sheet = cell.Sheet;
            CellRangeAddress mergedCell;

            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                mergedCell = sheet.GetMergedRegion(i);
                if (mergedCell.IsInRange(cell.RowIndex, cell.ColumnIndex))
                    return mergedCell.LastRow;
            }

            return cell.RowIndex;
        }

        public static int GetCellEndColumnIndex(ICell cell)
        {
            ISheet sheet = cell.Sheet;
            CellRangeAddress mergedCell;

            for (var i = 0; i < sheet.NumMergedRegions; i++)
            {
                mergedCell = sheet.GetMergedRegion(i);
                if (mergedCell.IsInRange(cell.RowIndex, cell.ColumnIndex))
                    return mergedCell.LastColumn;
            }

            return cell.ColumnIndex;
        }

        public static ICell FindCell(ISheet sheet, string cellText, int findIndex = 0)
        {
            IRow row = null;
            ICell cell = null;
            string value = null;
            var idx = 0;

            for (var r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
            {
                row = sheet.GetRow(r);
                if (row == null)
                    continue;

                for (var c = row.FirstCellNum; c <= row.LastCellNum; c++)
                {
                    cell = row.GetCell(c);
                    if (cell == null)
                        continue;

                    value = NpoiFuncs.GetCellValue(cell, cell.CellType);

                    if (value == cellText)
                    {
                        if (idx == findIndex)
                            return cell;

                        idx++;
                    }
                }
            }

            return null;
        }

        public static ICell GetCell(ICell cell, ConfirmKind kind, int confirmCellCount)
        {
            ISheet sheet = cell.Sheet;
            ICell ncell = null;
            CellRangeAddress cellRng = null;
            IRow row = null;
            int idx = 0;

            switch (kind)
            {
                case ConfirmKind.Cell:
                    return cell;
                case ConfirmKind.LeftCell:
                    for (var c = cell.ColumnIndex + 1; c <= cell.Row.LastCellNum; c++)
                    {
                        ncell = cell.Row.GetCell(c);
                        if (ncell == null)
                            continue;

                        if (ncell.CellType == CellType.Blank && ncell.IsMergedCell)
                        {
                            cellRng = GetMergedRange(ncell);

                            if (cellRng == null)
                                continue;

                            if (cellRng.LastRow == cellRng.FirstRow)
                                continue;
                        }

                        idx++;
                        if (idx < confirmCellCount)
                            continue;

                        return ncell;
                    }
                    break;
                case ConfirmKind.TopCell:
                    for (var r = cell.RowIndex + 1; r <= sheet.LastRowNum; r++)
                    {
                        row = sheet.GetRow(r);
                        if (row == null || row.ZeroHeight)
                            continue;

                        ncell = row.GetCell(cell.ColumnIndex);
                        if (ncell == null || (ncell.CellType == CellType.Blank && ncell.IsMergedCell))
                            continue;

                        idx++;
                        if (idx < confirmCellCount)
                            continue;

                        return ncell;
                    }
                    break;
                case ConfirmKind.RightCell:
                    for (var c = cell.ColumnIndex - 1; c >= cell.Row.FirstCellNum; c--)
                    {
                        ncell = cell.Row.GetCell(c);
                        if (ncell == null)
                            continue;

                        if (ncell.CellType == CellType.Blank && ncell.IsMergedCell)
                        {
                            cellRng = GetMergedRange(ncell);

                            if (cellRng == null)
                                continue;

                            if (cellRng.LastRow == cellRng.FirstRow)
                                continue;
                        }

                        idx++;
                        if (idx < confirmCellCount)
                            continue;

                        return ncell;
                    }
                    break;
                case ConfirmKind.BottomCell:
                    for (var r = cell.RowIndex - 1; r >= sheet.FirstRowNum; r--)
                    {
                        row = sheet.GetRow(r);
                        if (row == null || row.ZeroHeight)
                            continue;

                        ncell = row.GetCell(cell.ColumnIndex);
                        if (ncell == null || (ncell.CellType == CellType.Blank && ncell.IsMergedCell))
                            continue;

                        idx++;
                        if (idx < confirmCellCount)
                            continue;

                        return ncell;
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// 获取单元格值的字符串形式
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="cellType">单元格值类型</param>
        /// <returns></returns>
        public static string GetCellValue(ICell cell, CellType cellType)
        {
            string s = string.Empty;

            switch (cellType)
            {
                case CellType.Blank:
                    s = string.Empty;
                    break;
                case CellType.Boolean:
                    s = cell.BooleanCellValue.ToString();
                    break;
                case CellType.Error:
                    s = "^ERROR$";
                    break;
                case CellType.Formula:
                    s = GetCellValue(cell, cell.CachedFormulaResultType);
                    break;
                case CellType.Numeric:
                    s = cell.NumericCellValue.ToString();
                    break;
                case CellType.String:
                    s = (cell.StringCellValue ?? string.Empty).Replace("\n", "");
                    break;
                case CellType.Unknown:
                    s = "^UNKNOWN$";
                    break;
            }

            return s;
        }

        /// <summary>
        /// 获取单元格的显示名称，格式如A1,B2
        /// </summary>
        /// <param name="columnIdx">单元格列号</param>
        /// <param name="rowIdx">单元格行号</param>
        /// <returns></returns>
        public static string GetCellName(int columnIdx, int rowIdx)
        {
            int[] maxs = new[] { 26, 26 * 26 + 26, 26 * 26 * 26 + (26 * 26 + 26) + 26 };
            int col = columnIdx + 1;
            int row = rowIdx + 1;

            if (col > maxs[2])
                throw new Exception("列序号不正确，超出最大值");

            int alphaCount = 1;

            foreach (int m in maxs)
            {
                if (m < col)
                    alphaCount++;
            }

            switch (alphaCount)
            {
                case 1:
                    return (char)(col + 64) + "" + row;
                case 2:
                    return (char)((col / 26) + 64) + "" + (char)((col % 26) + 64) + row;
                case 3:
                    return (char)((col / 26 / 26) + 64) + "" + (char)(((col - col / 26 / 26 * 26 * 26) / 26) + 64) + "" + (char)((col % 26) + 64) + row;
            }

            return "Unkown";
        }
    }

    public class NpoiDataTable : DataTable
    {
        public NpoiDataTable()
        {

        }

        public List<NpoiMergeCell> MergeCells = new List<NpoiMergeCell>();

        public T GetCellValue<T>(int rowIdx, int colIdx)
        {
            T t = default(T);



            return t;
        }

        public NpoiMergeCell GetMergeCell(int rowIdx, int colIdx)
        {
            foreach (NpoiMergeCell cell in this.MergeCells)
            {
                if (rowIdx >= cell.Row && rowIdx <= cell.EndRow && colIdx >= cell.Column && colIdx <= cell.EndColumn)
                    return cell;
            }

            return null;
        }
    }

    public class NpoiMergeCell
    {
        public string Name { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public int EndRow { get; set; }

        public int EndColumn { get; set; }
    }
}