using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Port;
using BP.Sys;

namespace BP.Web.Controls
{
    public class WebFile
    {
        /// <summary>
        /// 要给值的实体
        /// </summary>
        /// <param name="en">要给值的实体</param>
        /// <param name="file"></param>
        /// <param name="saveAsPath"></param>
        public WebFile(BP.En.Entity en, HtmlInputFile file, string saveAsPath)
        {
            if (file != null && file.Value.IndexOf(":") != -1)
            {
                /* 如果包含这二个字段。*/
                string fileName = file.PostedFile.FileName;
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                this.FileName = fileName;

                string ext = "";
                if (fileName.IndexOf(".") != -1)
                    ext = fileName.Substring(fileName.LastIndexOf(".") + 1);
                this.FileExt = ext;

                if (saveAsPath != null)
                {
                    string fullFile = saveAsPath + "." + this.FileExt;
                    file.PostedFile.SaveAs(fullFile);
                    if (BP.DA.DataType.IsImgExt(ext))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(fullFile);
                        this.ImgW = img.Width;
                        this.ImgH = img.Height;
                        img.Dispose();
                    }
                }

                if (en != null)
                {
                    en.SetValByKey("MyFileName", this.FileName);
                    en.SetValByKey("MyFileExt", this.FileExt);
                    en.SetValByKey("MyFileH", this.ImgH);
                    en.SetValByKey("MyFileW", this.ImgW);
                }
            }
        }

        public string FileExt = null;
        public string FileName = null;
        public string FileFullName = null;

        /// <summary>
        /// 图片高度
        /// </summary>
        public int ImgH = 0;
        /// <summary>
        /// 图片宽度
        /// </summary>
        public int ImgW = 0;
        /// <summary>
        /// 大小
        /// </summary>
        public int FileSize = 0;
    }

}
