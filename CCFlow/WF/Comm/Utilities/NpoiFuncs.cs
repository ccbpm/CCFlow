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