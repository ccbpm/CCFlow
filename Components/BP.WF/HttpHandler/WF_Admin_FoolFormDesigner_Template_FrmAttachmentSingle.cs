using System.Web;
using BP.DA;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_FoolFormDesigner_Template_FrmAttachmentSingle : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_FoolFormDesigner_Template_FrmAttachmentSingle()
        {

        }

        #region  界面 .
        /// <summary>
        /// 生成模板文件
        /// </summary>
        /// <returns></returns>
        public string UploadAthTemplateWPS_Init()
        {
            BP.Sys.FrmUI.FrmAttachmentSingle ath = new Sys.FrmUI.FrmAttachmentSingle(this.MyPK);
            string file =  BP.Difference.SystemConfig.PathOfTemp + "/" + this.MyPK + ".wps";
            DBAccess.GetFileFromDB(file, ath.EnMap.PhysicsTable, "MyPK", this.MyPK, "TemplateFile");
            return file;
        }
        /// <summary>
        /// 保存模板文件
        /// </summary>
        /// <returns></returns>
        public string UploadAthTemplateWPS_Save()
        {
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请上传文件模板";
            HttpPostedFile file = HttpContextHelper.RequestFiles(0);
            //保存文件到临时目录
            string path =  BP.Difference.SystemConfig.PathOfTemp + file.FileName;
            HttpContextHelper.UploadFile(file, path);
            BP.Sys.FrmUI.FrmAttachmentSingle ath = new Sys.FrmUI.FrmAttachmentSingle(this.MyPK);
            //存储到模板库里。
            DBAccess.SaveFileToDB(path, ath.EnMap.PhysicsTable, "MyPK", this.MyPK, "TemplateFile");
            return "保存成功";
        }
        #endregion 界面.

    }
}
