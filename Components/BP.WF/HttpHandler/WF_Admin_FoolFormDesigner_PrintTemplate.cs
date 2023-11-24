using BP.DA;
using BP.WF.Template;
using BP.WF.Template.Frm;
using BP.Difference;
using System.Web;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_FoolFormDesigner_PrintTemplate : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner_PrintTemplate()
        {
        }

        #region  单据模版维护
        /// <summary>
        /// @李国文.
        /// </summary>
        /// <returns></returns>
        public string Bill_Save()
        {
            FrmPrintTemplate bt = new FrmPrintTemplate();

            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请上传模版.";
            //上传附件
            string filepath = "";
            //HttpPostedFile file = HttpContextHelper.RequestFiles(0);
            string fileName = HttpContextHelper.GetNameByIdx(0);
            fileName = fileName.Substring(fileName.IndexOf(this.GetRequestVal("TB_Name")));
            fileName = fileName.ToLower();

            filepath =  BP.Difference.SystemConfig.PathOfDataUser + "CyclostyleFile/" + fileName;
            //file.SaveAs(filepath);
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), filepath);

            bt.NodeID = this.NodeID;
            bt.FrmID = this.FrmID;
            bt.MyPK= this.GetRequestVal("TB_No");

            if (DataType.IsNullOrEmpty(bt.MyPK))
                bt.MyPK = DBAccess.GenerOID("Template").ToString();

            bt.Name = this.GetRequestVal("TB_Name");
            bt.TempFilePath = fileName; //文件.

            //打印的文件类型.
            bt.HisPrintFileType = (PrintFileType)this.GetRequestValInt("DDL_BillFileType");

            //打开模式.
            bt.PrintOpenModel = (PrintOpenModel)this.GetRequestValInt("DDL_BillOpenModel");

            //二维码模式.
            bt.QRModel = (QRModel)this.GetRequestValInt("DDL_QRModel");

            bt.TemplateFileModel = (TemplateFileModel)this.GetRequestValInt("TemplateFileModel");
           

            bt.Save();

            bt.SaveFileToDB("DBFile", filepath); //把文件保存到数据库里. 

            Cache.ClearCache(fileName);
            Cache.ClearCache(fileName+ "Para");

            return "保存成功.";
        }
        /// <summary>
        /// 下载文件.
        /// </summary>
        public void Bill_Download()
        {
            FrmPrintTemplate en = new FrmPrintTemplate(this.No);
            string MyFilePath = en.TempFilePath;
            //HttpResponse response = context.Response;
            //response.Clear();
            //response.Buffer = true;
            //response.Charset = "utf-8";
            //response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", en.TempFilePath.Substring(MyFilePath.LastIndexOf('/') + 1)));
            //response.ContentEncoding = System.Text.Encoding.UTF8;
            //response.BinaryWrite(System.IO.File.ReadAllBytes(MyFilePath));
            //response.End();

            HttpContextHelper.ResponseWrite("Charset");
            HttpContextHelper.ResponseWriteHeader("Content-Disposition", string.Format("attachment;filename={0}", en.TempFilePath.Substring(MyFilePath.LastIndexOf('/') + 1)));
            HttpContextHelper.Response.ContentType = "application/octet-stream;charset=utf-8";
            HttpContextHelper.ResponseWriteFile(MyFilePath);
        }
        #endregion

    }
}
