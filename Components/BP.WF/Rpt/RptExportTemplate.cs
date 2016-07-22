using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BP.WF.Rpt
{
    /// <summary>
    /// 报表导出模板
    /// </summary>
    public class RptExportTemplate
    {
        /// <summary>
        /// 模板最后修改时间
        /// </summary>
        public DateTime LastModify { get; set; }

        /// <summary>
        /// 导出填充方向
        /// </summary>
        public FillDirection Direction { get; set; }

        /// <summary>
        /// 导出开始填充的行/列号
        /// </summary>
        public int BeginIdx { get; set; }

        /// <summary>
        /// 字段与单元格绑定信息集合
        /// </summary>
        public List<RptExportTemplateCell> Cells { get; set; }

        /// <summary>
        /// 是否有单元格绑定了指定的表单中的字段
        /// </summary>
        /// <param name="fk_mapdata">表单对应FK_MapData</param>
        /// <returns></returns>
        public bool HaveCellInMapData(string fk_mapdata)
        {
            foreach(RptExportTemplateCell cell in Cells)
            {
                if (cell.FK_MapData == fk_mapdata)
                    return true;
            }

            return false;
        }

        public RptExportTemplateCell GetBeginHeaderCell(FillDirection direction)
        {
            if (Cells == null || Cells.Count == 0)
                return null;

            RptExportTemplateCell cell = Cells[0];

            if(direction == FillDirection.Vertical)
            {
                for(int i = 1;i<Cells.Count;i++)
                {
                    if (Cells[i].ColumnIdx < cell.ColumnIdx)
                        cell = Cells[i];
                }

                return cell;
            }
            
            for(int i = 1;i<Cells.Count;i++)
            {
                if (Cells[i].RowIdx < cell.RowIdx)
                    cell = Cells[i];
            }

            return cell;
        }

        /// <summary>
        /// 保存到xml文件中
        /// </summary>
        /// <param name="fileName">xml文件路径</param>
        /// <returns></returns>
        public bool SaveXml(string fileName)
        {
            try
            {
                using (var sw = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    new XmlSerializer(typeof(RptExportTemplate)).Serialize(sw, this);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取定义的填充明细表NO
        /// </summary>
        /// <returns></returns>
        public string GetDtl()
        {
            foreach(RptExportTemplateCell cell in Cells)
            {
                if (!string.IsNullOrWhiteSpace(cell.DtlKeyOfEn))
                    return cell.FK_DtlMapData;
            }

            return null;
        }

        /// <summary>
        /// 从xml文件加载报表导出模板信息对象
        /// </summary>
        /// <param name="fileName">xml文件路径</param>
        /// <returns></returns>
        public static RptExportTemplate FromXml(string fileName)
        {
            RptExportTemplate t;

            if(!File.Exists(fileName))
            {
                t = new RptExportTemplate
                        {
                            LastModify = DateTime.Now,
                            Direction = FillDirection.Vertical,
                            Cells = new List<RptExportTemplateCell>()
                        };

                t.SaveXml(fileName);
                return t;
            }

            try
            {
                using (var sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    t = new XmlSerializer(typeof(RptExportTemplate)).Deserialize(sr) as RptExportTemplate;
                }
            }
            catch
            {
                t = new RptExportTemplate
                        {
                            LastModify = DateTime.Now,
                            Direction = FillDirection.Vertical,
                            Cells = new List<RptExportTemplateCell>()
                        };
            }

            return t;
        }
    }

    /// <summary>
    /// 报表导出模板字段与单元格绑定信息对象
    /// </summary>
    public class RptExportTemplateCell
    {
        [XmlIgnore]
        private string _cellName;

        /// <summary>
        /// 单元格行号
        /// </summary>
        public int RowIdx { get; set; }

        /// <summary>
        /// 单元格列号
        /// </summary>
        public int ColumnIdx { get; set; }

        /// <summary>
        /// 获取单元格名称
        /// </summary>
        [XmlIgnore]
        public string CellName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_cellName))
                    _cellName = GetCellName(ColumnIdx, RowIdx);

                return _cellName;
            }
        }

        /// <summary>
        ///  单元格所属sheet表名
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 字段所属fk_mapdata
        /// </summary>
        public string FK_MapData { get; set; }

        /// <summary>
        /// 字段英文名
        /// </summary>
        public string KeyOfEn { get; set; }

        /// <summary>
        /// 明细表字段所属fk_mapdata
        /// </summary>
        public string FK_DtlMapData { get; set; }

        /// <summary>
        /// 明细表字段英文名
        /// </summary>
        public string DtlKeyOfEn { get; set; }

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

            var alphaCount = maxs.Where(m => m < col).Count() + 1;

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

    public enum FillDirection
    {
        Vertical = 1,

        Horizontal = 2
    }
}
