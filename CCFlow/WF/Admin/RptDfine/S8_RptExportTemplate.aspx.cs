using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Rpt;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Data;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class S8_RptExportTemplate : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
                //return "002";
            }
        }
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];
                //return "ND2MyRpt";
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 报表定义实体
        /// </summary>
        public MapData MData { get; set; }
        /// <summary>
        /// 模板存放路径
        /// </summary>
        public string TmpDir { get; set; }
        /// <summary>
        /// 模板文件允许的扩展名
        /// </summary>
        public string Exts = ".xls.xlsx";
        #endregion 属性.

        //获取所有字段信息
        private string selectSql = "SELECT sma.FK_MAPDATA,smd.NAME FK_MAPDATANAME, sma.KEYOFEN,sma.NAME,sma.LGTYPE,sma.UIBINDKEY,sma.UIREFKEY,sma.UIREFKEYTEXT,sma.GROUPID,sgf.LAB GROUPNAME FROM Sys_MapAttr sma"
                                         +
                                         " INNER JOIN (SELECT wfn.FK_Frm FK_MAPDATA FROM WF_FrmNode wfn WHERE wfn.FK_Flow = '{0}' AND wfn.IsEnable = 1 GROUP BY wfn.FK_Frm UNION SELECT smd2.No FK_MAPDATA FROM Sys_MapData smd2 WHERE smd2.No = 'ND{1}Rpt') t ON t.FK_MAPDATA = sma.FK_MapData"
                                         + " INNER JOIN Sys_MapData smd ON smd.No = sma.FK_MapData"
                                         + " LEFT JOIN Sys_GroupField AS sgf ON sgf.OID = sma.GroupID"
                                         + " ORDER BY sma.GroupID,sma.Idx";
        //获取所有明细表信息
        private string selectDtlsSql = "SELECT wfn.FK_NODE,wn.Name FK_NODENAME,smd.NO FK_MAPDATA,smd.NAME FK_MAPDATANAME,sma.KEYOFEN,sma.NAME FROM Sys_MapAttr sma"
                                       + " INNER JOIN Sys_MapDtl smd ON smd.No = sma.FK_MapData"
                                       +
                                       " INNER JOIN WF_FrmNode wfn ON wfn.FK_Frm = smd.FK_MapData AND wfn.FK_Flow = '{0}' AND wfn.IsEnable = 1"
                                       + " INNER JOIN WF_Node wn ON wn.NodeID = wfn.FK_Node"
                                       + " ORDER BY wn.Step";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            MData = new MapData(this.RptNo);
            TmpDir = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + RptNo;

            if (!Directory.Exists(TmpDir))
                Directory.CreateDirectory(TmpDir);

            if (IsPostBack)
            {
                if (fileUpload.HasFile)
                {
                    string file = fileUpload.PostedFile.FileName;

                    if (Exts.IndexOf(Path.GetExtension(file)) == -1)
                    {
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "alert('上传的模板文件格式仅限Microsoft Excel文档 （*.xls, *.xlsx）！');", true);
                    }
                    else
                    {
                        string savefile = GetCorrectFileName(TmpDir, Path.GetFileName(file));
                        fileUpload.PostedFile.SaveAs(savefile);

                        try
                        {
                            //自动生成配置XML
                            GenerAutoFieldBinding(fileUpload.PostedFile.InputStream, fileUpload.PostedFile.FileName);
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "msg",
                                                                   "alert('解析模板时出现错误，请确保模板格式正确！错误信息：" + ex.Message +
                                                                   "');", true);
                        }
                    }
                }
            }
            else
            {
                string method = Request.QueryString["method"];
                string tmpName = HttpUtility.UrlDecode(Request.QueryString["tmp"] ?? "");
                string filename = TmpDir + "\\" + tmpName;
                string checkMsg = string.Empty;
                string resultString = string.Empty;

                switch (method)
                {
                    case "down":    //下载模板文件
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "alert('" + checkMsg + "');self.close();", true);
                            return;
                        }

                        Response.Clear();

                        FileStream fs = new FileStream(filename, FileMode.Open);
                        byte[] bs = new byte[fs.Length];
                        fs.Read(bs, 0, bs.Length);
                        fs.Close();
                        fs.Dispose();

                        Response.ContentType = "application/octet-steam";
                        Response.AddHeader("Content-Disposition",
                                           "attachment;filename=" + HttpUtility.UrlEncode(tmpName, System.Text.Encoding.UTF8));
                        Response.BinaryWrite(bs);
                        Response.Flush();
                        Response.End();
                        break;
                    case "del": //删除模板文件
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            resultString = ReturnJson(false, checkMsg);
                        }
                        else
                        {
                            try
                            {
                                File.Delete(filename);
                                File.Delete(Path.GetDirectoryName(filename) + "\\" +
                                            Path.GetFileNameWithoutExtension(filename) + ".xml");
                                resultString = ReturnJson(true, "删除成功！");
                            }
                            catch (Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message);
                            }
                        }
                        break;
                    case "set": //配置模板
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            resultString = ReturnJson(false, checkMsg);
                        }
                        else
                        {
                            try
                            {
                                DataTable dtAttrs = BP.DA.DBAccess.RunSQLReturnTable(string.Format(selectSql, FK_Flow, int.Parse(FK_Flow)));
                                DataTable dtDtlsAttrs = BP.DA.DBAccess.RunSQLReturnTable(string.Format(selectDtlsSql, FK_Flow));

                                //处理GROUPID为null的情况
                                DataRow[] drs = dtAttrs.Select("GROUPID IS NULL");

                                foreach (DataRow r in drs)
                                    r["GROUPID"] = 0;

                                resultString = "{\"success\": true, \"attrs\": " + BP.Tools.Json.ToJson(dtAttrs) +
                                               ",\"dtlattrs\": " + BP.Tools.Json.ToJson(dtDtlsAttrs) +
                                               ", \"setinfo\": \"" + GetTemplateSetInfos(filename, dtAttrs, dtDtlsAttrs) +
                                               "\", \"menu\":\"" + GetMapAttrsMenu(dtAttrs) +
                                               "\", \"dtlmenu\":\"" + GetDtlsMapAttrsMenu(dtDtlsAttrs) + "\"}";
                            }
                            catch (Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message);
                            }
                        }

                        break;
                    case "save":    //保存配置
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            resultString = ReturnJson(false, checkMsg);
                        }
                        else
                        {
                            string data = HttpUtility.UrlDecode(Request.QueryString["data"] ?? "");
                            string[] items = data.Split('`');

                            if (items.Length < 2)
                            {
                                resultString = ReturnJson(false, "保存数据格式不正确！");
                            }
                            else
                            {
                                var isBeginIdx = int.Parse(items[0]);
                                var beginIdx = int.Parse(items[1]);
                                string xml = TmpDir + "\\" + Path.GetFileNameWithoutExtension(filename) + ".xml";
                                RptExportTemplate tmp = RptExportTemplate.FromXml(xml);

                                if (isBeginIdx == 1)
                                {
                                    tmp.Direction = FillDirection.Vertical;
                                }
                                else if (isBeginIdx == 2)
                                {
                                    tmp.Direction = FillDirection.Horizontal;
                                }

                                tmp.BeginIdx = beginIdx;
                                tmp.LastModify = DateTime.Now;
                                tmp.Cells.Clear();

                                if (items.Length > 2)
                                {
                                    string[] subItems = null;
                                    for (var i = 2; i < items.Length; i++)
                                    {
                                        subItems = items[i].Split('^');

                                        if (subItems.Length != 6)
                                            continue;

                                        tmp.Cells.Add(new RptExportTemplateCell
                                                          {
                                                              RowIdx = int.Parse(subItems[0]),
                                                              ColumnIdx = int.Parse(subItems[1]),
                                                              FK_MapData = subItems[2],
                                                              KeyOfEn = subItems[3],
                                                              FK_DtlMapData = subItems[4],
                                                              DtlKeyOfEn = subItems[5]
                                                          });
                                    }
                                }

                                tmp.SaveXml(xml);
                                resultString = ReturnJson(true, "保存成功！");
                            }
                        }
                        break;
                    case "rename":  //重命名模板文件
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            resultString = ReturnJson(false, checkMsg);
                        }
                        else
                        {
                            string newTmpName = HttpUtility.UrlDecode(Request.QueryString["newTmp"] ?? "");
                            string newFileName = GetCorrectFileName(TmpDir, newTmpName + Path.GetExtension(filename));

                            try
                            {
                                File.Move(filename, newFileName);

                                //修改对应的配置XML文件
                                string xml = TmpDir + "\\" + Path.GetFileNameWithoutExtension(filename) + ".xml";
                                string newXml = TmpDir + "\\" + Path.GetFileNameWithoutExtension(newFileName) + ".xml";

                                if (File.Exists(xml))
                                    File.Move(xml, newXml);

                                //判断如果当前改名的模板正在被使用，则将mapdata中的name也改成将改的模板名称
                                if (Equals(MData.Name, Path.GetFileNameWithoutExtension(tmpName)))
                                {
                                    MData.Name = Path.GetFileNameWithoutExtension(newFileName);
                                    MData.Update();
                                }

                                resultString = ReturnJson(true, Path.GetFileName(newFileName)); //返回重命名后的新模板文件名称
                            }
                            catch (Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message);
                            }
                        }
                        break;
                    case "use": //使用此模板
                        checkMsg = Check(tmpName, filename);
                        if (!string.IsNullOrWhiteSpace(checkMsg))
                        {
                            resultString = ReturnJson(false, checkMsg);
                        }
                        else
                        {
                            try
                            {
                                string oldName = MData.Name;

                                MData.Name = Path.GetFileNameWithoutExtension(filename);
                                MData.Update();

                                //查找旧的模板的扩展名
                                FileInfo[] oldFiles = new DirectoryInfo(TmpDir).GetFiles(oldName + ".xls*");

                                resultString = ReturnJson(true, oldName + (oldFiles.Length > 0 ? oldFiles[0].Extension : ".xls"));
                                //返回旧的正在使用的报表导出模板文件名
                            }
                            catch (Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message);
                            }
                        }
                        break;
                    default:
                        return;
                }

                Response.Charset = "UTF-8";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "text/html";
                Response.Expires = 0;
                Response.Write(resultString);
                Response.End();
            }
        }

        /// <summary>
        /// 自动生成模板文件的字段与单元格对应关系，保存
        /// </summary>
        /// <param name="steam">模板文件流</param>
        /// <param name="tmpFile">模板文件名</param>
        private void GenerAutoFieldBinding(Stream steam, string tmpFile)
        {
            DataTable dtAttrs = BP.DA.DBAccess.RunSQLReturnTable(string.Format(selectSql, FK_Flow, int.Parse(FK_Flow)));
            string xml = TmpDir + "\\" + Path.GetFileNameWithoutExtension(tmpFile) + ".xml";

            if (File.Exists(xml))
                File.Delete(xml);

            RptExportTemplate tmp = RptExportTemplate.FromXml(xml);
            string ext = Path.GetExtension(tmpFile).ToLower();
            IWorkbook wb = null;
            IRow row = null;
            ICell cell = null;
            DataRow[] rows = null;

            if (ext.Equals(".xls"))
                wb = new HSSFWorkbook(steam);
            else
                wb = new XSSFWorkbook(steam);

            ISheet sheet = wb.GetSheetAt(0);

            for (var r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
            {
                row = sheet.GetRow(r);

                if (row == null) continue;

                for (var c = row.FirstCellNum; c < row.LastCellNum; c++)
                {
                    cell = row.GetCell(c);
                    if (cell == null) continue;

                    rows = dtAttrs.Select(string.Format("NAME='{0}' AND NAME <> ''", GetCellValue(cell, cell.CellType)), "GROUPID ASC");

                    if (rows.Length > 0)
                    {
                        tmp.Cells.Add(new RptExportTemplateCell
                                          {
                                              RowIdx = r,
                                              ColumnIdx = c,
                                              FK_MapData = rows[0]["FK_MAPDATA"].ToString(),
                                              KeyOfEn = rows[0]["KEYOFEN"].ToString()
                                          });
                    }
                }
            }

            tmp.SaveXml(xml);
            wb.Close();
        }

        /// <summary>
        /// 根据字段集合生成字段选择菜单
        /// </summary>
        /// <param name="dtAttrs">字段集合</param>
        /// <returns>返回easyui-menu的html代码</returns>
        private string GetMapAttrsMenu(DataTable dtAttrs)
        {
            int groupid = 0;
            StringBuilder s = new StringBuilder();
            Dictionary<string, List<DataRow>> mapdatas = new Dictionary<string, List<DataRow>>(); //fk_mapdata,attrs
            Dictionary<int, List<DataRow>> groups = null;
            s.Append("<div id='mAttrs' class='easyui-menu' style='width:120px'>");
            s.Append("<div name='deleteField' iconCls='icon-delete'>删除字段</div>");

            //将字段按照fk_mapdata分组
            foreach (DataRow row in dtAttrs.Rows)
            {
                if (!mapdatas.ContainsKey(row["FK_MAPDATA"].ToString()))
                    mapdatas.Add(row["FK_MAPDATA"].ToString(), new List<DataRow> { row });
                else
                    mapdatas[row["FK_MAPDATA"].ToString()].Add(row);
            }

            foreach (KeyValuePair<string, List<DataRow>> ke in mapdatas)
            {
                s.Append(string.Format("<div name='{0}'><span>{1}[{0}]</span>", ke.Key, ke.Value[0]["FK_MAPDATANAME"]));
                s.Append("<div>");

                groups = new Dictionary<int, List<DataRow>>();

                //在fk_mapdata分组的字段集合中，再按照字段所处group进行分组
                foreach (DataRow row in ke.Value)
                {
                    groupid = row["GROUPID"] == null || row["GROUPID"] == DBNull.Value ? 0 : (int) row["GROUPID"];

                    if (!groups.ContainsKey(groupid))
                        groups.Add(groupid, new List<DataRow> { row });
                    else
                        groups[groupid].Add(row);
                }

                foreach (KeyValuePair<int, List<DataRow>> ke2 in groups)
                {
                    s.Append(string.Format("<div name='{0}'><span>{1}</span>", ke2.Key, ke2.Value[0]["GROUPNAME"] == null || ke2.Value[0]["GROUPNAME"] == DBNull.Value ? "未分组" : ke2.Value[0]["GROUPNAME"].ToString()));
                    s.Append("<div>");

                    foreach (DataRow row2 in ke2.Value)
                    {
                        s.Append(string.Format("<div name='{0}.{1}'>{1}[{2}]</div>", ke.Key, row2["KEYOFEN"], row2["NAME"]));
                    }

                    s.Append("</div>");
                    s.Append("</div>");
                }

                s.Append("</div>");
                s.Append("</div>");
            }

            s.Append("</div>");
            return s.ToString();
        }

        /// <summary>
        /// 根据明细表字段集合生成明细表字段选择菜单
        /// </summary>
        /// <param name="dtDtlsAttrs">明细表字段集合</param>
        /// <returns>返回easyui-menu的html代码</returns>
        private string GetDtlsMapAttrsMenu(DataTable dtDtlsAttrs)
        {
            StringBuilder s = new StringBuilder();
            Dictionary<string, List<DataRow>> mapdatas = new Dictionary<string, List<DataRow>>(); //fk_mapdata,attrs
            s.Append("<div id='mDtlAttrs' class='easyui-menu' style='width:120px'>");
            s.Append("<div name='deleteDtlField' iconCls='icon-delete'>删除明细表字段</div>");

            //将字段按照fk_mapdata分组
            foreach (DataRow row in dtDtlsAttrs.Rows)
            {
                if (!mapdatas.ContainsKey(row["FK_MAPDATA"].ToString()))
                    mapdatas.Add(row["FK_MAPDATA"].ToString(), new List<DataRow> { row });
                else
                    mapdatas[row["FK_MAPDATA"].ToString()].Add(row);
            }

            foreach (KeyValuePair<string, List<DataRow>> ke in mapdatas)
            {
                s.Append(string.Format("<div name='{0}'><span>{1}[{0}]</span>", ke.Key, ke.Value[0]["FK_MAPDATANAME"]));
                s.Append("<div>");

                foreach (DataRow row in ke.Value)
                {
                    s.Append(string.Format("<div name='{0}.{1}'>{1}[{2}]</div>", ke.Key, row["KEYOFEN"], row["NAME"]));
                }

                s.Append("</div>");
                s.Append("</div>");
            }

            s.Append("</div>");
            return s.ToString();
        }

        /// <summary>
        /// 获取指定单元格的字段绑定信息，存储在td中的data-field属性中
        /// </summary>
        /// <param name="rowidx">单元格行号</param>
        /// <param name="colidx">单元格列号</param>
        /// <param name="cellValue">单元格值</param>
        /// <param name="dtAttrs">字段集合</param>
        /// <param name="dtDtlAttrs">明细表字段集合</param>
        /// <param name="tmp">模板对象</param>
        /// <returns>返回data-field,data-tooltip属性的拼接字符串</returns>
        private string GetAutoFieldBinding(int rowidx, int colidx, string cellValue, DataTable dtAttrs, DataTable dtDtlAttrs, RptExportTemplate tmp)
        {
            DataRow[] rows = null;
            DataRow[] dtlRows = null;
            RptExportTemplateCell cell = null;

            if (tmp.Cells.Count > 0)
            {
                foreach (RptExportTemplateCell c in tmp.Cells)
                {
                    if (c.RowIdx == rowidx && c.ColumnIdx == colidx)
                    {
                        cell = c;
                        rows = dtAttrs.Select(string.Format("FK_MAPDATA='{0}' AND KEYOFEN='{1}'", c.FK_MapData, c.KeyOfEn));
                        break;
                    }
                }
            }
            else
            {
                rows = dtAttrs.Select(string.Format("NAME='{0}' AND NAME <> ''", cellValue), "GROUPID ASC");
            }

            if (rows != null && rows.Length > 0)
            {
                if (cell != null && !string.IsNullOrWhiteSpace(cell.DtlKeyOfEn))
                    dtlRows =
                        dtDtlAttrs.Select(string.Format("FK_MAPDATA='{0}' AND KEYOFEN='{1}'", cell.FK_DtlMapData,
                                                        cell.DtlKeyOfEn));

                return string.Format(" data-field='{0}`{1}`{2}`{3}' data-dtlField='{5}' data-tooltip='{4}{2}[{3}]{6}'",
                                     rows[0]["FK_MAPDATA"],
                                     rows[0]["FK_MAPDATANAME"], rows[0]["KEYOFEN"], rows[0]["NAME"],
                                     Equals(rows[0]["FK_MAPDATA"], "ND" + int.Parse(FK_Flow) + "Rpt")
                                         ? ""
                                         : (rows[0]["FK_MAPDATA"] + "[" + rows[0]["FK_MAPDATANAME"] + "] "),
                                     dtlRows == null || dtlRows.Length == 0
                                         ? ""
                                         : string.Format("{0}`{1}`{2}`{3}", dtlRows[0]["FK_MAPDATA"],
                                                         dtlRows[0]["FK_MAPDATANAME"], dtlRows[0]["KEYOFEN"],
                                                         dtlRows[0]["NAME"]),
                                     dtlRows == null || dtlRows.Length == 0
                                         ? ""
                                         : string.Format("<br />明细表：{0}[{1}] {2}[{3}]", dtlRows[0]["FK_MAPDATA"],
                                                         dtlRows[0]["FK_MAPDATANAME"], dtlRows[0]["KEYOFEN"],
                                                         dtlRows[0]["NAME"]));
            }

            return " data-field='' data-dtlField='' data-tooltip=''";
        }

        /// <summary>
        /// 解析excel模板，获取配置信息
        /// </summary>
        /// <param name="tmpFile">模板文件路径</param>
        /// <param name="dtAttrs">字段集合</param>
        /// <param name="dtDtlAttrs">明细表字段集合</param>
        /// <returns>返回table的html代码</returns>
        private string GetTemplateSetInfos(string tmpFile, DataTable dtAttrs, DataTable dtDtlAttrs)
        {
            string xml = TmpDir + "\\" + Path.GetFileNameWithoutExtension(tmpFile) + ".xml";
            RptExportTemplate tmp = RptExportTemplate.FromXml(xml);
            string ext = Path.GetExtension(tmpFile).ToLower();
            StringBuilder s = null;
            IWorkbook wb = null;
            IRow row = null;
            ICell cell = null;
            List<CellRangeAddress> mergedRanges = new List<CellRangeAddress>();
            CellRangeAddress range = null;
            List<string> existedMergedRanges = new List<string>();
            string cellValue = string.Empty;

            using (FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
            {
                if (ext.Equals(".xls"))
                    wb = new HSSFWorkbook(fs);
                else
                    wb = new XSSFWorkbook(fs);

                ISheet sheet = wb.GetSheetAt(0);
                s =
                    new StringBuilder(
                        string.Format(
                            "<table id='excel' cellspacing='0' cellpadding='0' border='0' data-cols='{0}' data-rows='{1}' data-direction='{2}' data-beginidx='{3}'>",
                            sheet.GetRow(0).LastCellNum, sheet.LastRowNum + 1, (int)tmp.Direction, tmp.BeginIdx));

                for (var i = 0; i < sheet.NumMergedRegions; i++)
                    mergedRanges.Add(sheet.GetMergedRegion(i));

                for (var r = sheet.FirstRowNum; r <= sheet.LastRowNum; r++)
                {
                    row = sheet.GetRow(r);

                    if (row == null) continue;

                    s.Append(string.Format("<tr style='height:{0}pt;' data-rowid='{1}'>", row.HeightInPoints, r));
                    for (var c = row.FirstCellNum; c < row.LastCellNum; c++)
                    {
                        cell = row.GetCell(c);

                        if (cell == null) continue;

                        range = GetMergedRegion(cell, mergedRanges);
                        cellValue = GetCellValue(cell, cell.CellType);

                        if (range != null)
                        {
                            if (!existedMergedRanges.Contains(range.ToString()))
                            {
                                existedMergedRanges.Add(range.ToString());
                                s.Append(string.Format("<td{0} data-rowid='{1}' data-colid='{2}' data-name='{3}'{4}", GetCellStyle(cell), r, c, RptExportTemplateCell.GetCellName(c, r), GetAutoFieldBinding(r, c, cellValue, dtAttrs, dtDtlAttrs, tmp)));

                                if (range.LastColumn - range.FirstColumn > 0)
                                    s.Append(string.Format(" colspan='{0}'", range.LastColumn - range.FirstColumn + 1));

                                if (range.LastRow - range.FirstRow > 0)
                                    s.Append(string.Format(" rowspan='{0}'", range.LastRow - range.FirstRow + 1));

                                s.Append(">");
                                s.Append(cellValue);
                                s.Append("</td>");
                            }
                        }
                        else
                        {
                            s.Append(string.Format("<td{0} data-rowid='{1}' data-colid='{2}' data-name='{3}'{4}>{5}</td>", GetCellStyle(cell), r, c, RptExportTemplateCell.GetCellName(c, r), GetAutoFieldBinding(r, c, cellValue, dtAttrs, dtDtlAttrs, tmp), cellValue));
                        }
                    }
                    s.Append("</tr>");
                }

                s.Append("</table>");
                wb.Close();
            }

            return s.ToString();
        }

        /// <summary>
        /// 获取单元格的样式
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns></returns>
        private string GetCellStyle(ICell cell)
        {
            //样式依次为：0.字体名称、1.字体大小、2.字体粗体比重、3.文本水平对齐方式、4.文本垂直对齐方式、5.单元格宽度、6、左边框、7.上边框、8.右边框、9.下边框
            string style = " style='font-family:{0};font-size:{1}pt;font-weight:{2};text-align:{3};vertical-align:{4};{5}{6}{7}{8}{9}'";
            ICellStyle st = cell.CellStyle;

            IFont font = st.GetFont(cell.Sheet.Workbook);

            return string.Format(style,
                                 font.FontName,
                                 font.FontHeightInPoints,
                                 font.Boldweight,
                                 st.Alignment == NPOI.SS.UserModel.HorizontalAlignment.Center
                                     ? "center"
                                     : st.Alignment == NPOI.SS.UserModel.HorizontalAlignment.Right ? "right" : "left",
                                 st.VerticalAlignment == VerticalAlignment.Center
                                     ? "middle"
                                     : st.VerticalAlignment == VerticalAlignment.Bottom ? "bottom" : "top",
                                 cell.IsMergedCell
                                     ? ""
                                     : string.Format("width:{0}px;", cell.Sheet.GetColumnWidthInPixels(cell.ColumnIndex)),
                                 st.BorderLeft != NPOI.SS.UserModel.BorderStyle.None
                                     ? "border-left:1px solid #ccc;"
                                     : "",
                                 st.BorderTop != NPOI.SS.UserModel.BorderStyle.None
                                     ? "border-top:1px solid #ccc;"
                                     : "",
                                 st.BorderRight != NPOI.SS.UserModel.BorderStyle.None
                                     ? "border-right:1px solid #ccc;"
                                     : "",
                                 st.BorderBottom != NPOI.SS.UserModel.BorderStyle.None
                                     ? "border-bottom:1px solid #ccc;"
                                     : "");
        }

        /// <summary>
        /// 获取指定单元格所在的合并单元格区域对象
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="ranges">合并单元格区域集合</param>
        /// <returns></returns>
        private CellRangeAddress GetMergedRegion(ICell cell, List<CellRangeAddress> ranges)
        {
            if (!cell.IsMergedCell)
                return null;

            foreach (CellRangeAddress rng in ranges)
            {
                if (cell.RowIndex >= rng.FirstRow && cell.RowIndex <= rng.LastRow && cell.ColumnIndex >= rng.FirstColumn && cell.ColumnIndex <= rng.LastColumn)
                    return rng;
            }

            return null;
        }

        /// <summary>
        /// 获取单元格值的字符串形式
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="cellType">单元格值类型</param>
        /// <returns></returns>
        private string GetCellValue(ICell cell, CellType cellType)
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
                    s = "ERROR";
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
                    s = "UNKNOWN";
                    break;
            }

            return s;
        }

        /// <summary>
        /// 检验参数
        /// </summary>
        /// <param name="tmpName">模板文件名称</param>
        /// <param name="filename">模板文件全路径</param>
        /// <returns>校验错误信息</returns>
        private string Check(string tmpName, string filename)
        {
            if (string.IsNullOrWhiteSpace(tmpName))
            {
                return "参数传输错误！";
            }

            if (!File.Exists(filename))
            {
                return tmpName + " 模板文件不存在！";
            }

            return null;
        }

        /// <summary>
        /// 获取正确的保存文件名
        /// </summary>
        /// <param name="dir">保存文件夹路径</param>
        /// <param name="filename">文件基名称，带有扩展名</param>
        /// <returns>返回保存文件名的全路径</returns>
        private string GetCorrectFileName(string dir, string filename)
        {
            string ext = Path.GetExtension(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string fullname = dir + "\\" + filename;
            int i = 1;

            while (File.Exists(fullname))
            {
                fullname = dir + "\\" + name + i++ + ext;
            }

            return fullname;
        }

        /// <summary>
        /// 生成返给前台页面的JSON字符串信息
        /// </summary>
        /// <param name="success">是否操作成功</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        private string ReturnJson(bool success, string msg)
        {
            return "{\"success\":" + success.ToString().ToLower() + ",\"msg\":\"" + msg.Replace("\"", "'") +
                   "\"}";
        }
    }
}