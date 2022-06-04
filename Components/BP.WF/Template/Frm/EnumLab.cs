
namespace BP.WF.Template.Frm
{
    /// <summary>
    /// 模版类型
    /// </summary>
    public enum TemplateFileModel
    {
        /// <summary>
        /// 旧版本的rtf模版格式
        /// </summary>
        RTF,
        /// <summary>
        /// Word模版格式
        /// </summary>
        VSTOForWord,
        /// <summary>
        /// Excel模版格式
        /// </summary>
        VSTOForExcel,
        /// <summary>
        /// WPS模板格式(只适应于专业版)
        /// </summary>
        WPS
    }
    /// <summary>
    /// 二维码生成方式
    /// </summary>
    public enum QRModel
    {
        /// <summary>
        /// 不生成
        /// </summary>
        None,
        /// <summary>
        /// 生成
        /// </summary>
        Gener
    }
    /// <summary>
    /// 生成的类型
    /// </summary>
    public enum PrintFileType
    {
        /// <summary>
        /// Word
        /// </summary>
        Word = 0,
        PDF = 1,
        Excel = 2,
        Html = 3,
        RuiLang = 5
    }
    /// <summary>
    /// 生成的文件打开方式
    /// </summary>
    public enum PrintOpenModel
    {
        /// <summary>
        /// 下载保存
        /// </summary>
        DownLoad = 0,
        /// <summary>
        /// 在线WebOffice打开
        /// </summary>
        WebOffice = 1
    }
}
