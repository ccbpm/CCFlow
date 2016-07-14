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
        public DateTime LastModify { get; set; }

        public FillDirection Direction { get; set; }

        public int BeginIdx { get; set; }

        public List<RptExportTemplateCell> Cells { get; set; }

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

    public class RptExportTemplateCell
    {
        [XmlIgnore]
        private string _cellName;

        public int RowIdx { get; set; }

        public int ColumnIdx { get; set; }

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

        public string SheetName { get; set; }

        public string FK_MapData { get; set; }

        public string KeyOfEn { get; set; }

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
