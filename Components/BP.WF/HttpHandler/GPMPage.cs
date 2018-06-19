using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class GPMPage : DirectoryPageBase
    {
        #region 签名.
        public string Siganture_Save()
        {
            HttpPostedFile f = context.Request.Files[0];

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            }

            try
            {
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                f.SaveAs(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg");
            // f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
            return "上传成功！";
        }
        #endregion



        public string StationToDeptEmp_Init()
        {
            return "";
        }

        /// <summary>
        /// 处理系统编辑菜单.
        /// </summary>
        /// <returns></returns>
        public string AppMenu_Init()
        {
            //BP.GPM.App app = new BP.GPM.App();
            //app.No = "CCFlowBPM";
            //if (app.RetrieveFromDBSources() == 0)
            //{
            //    BP.GPM.App.InitBPMMenu();
            //    app.Retrieve();
            //}
            return "";
        }

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public GPMPage(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region xxx 界面 .
        #endregion xxx 界面方法.

    }
}
