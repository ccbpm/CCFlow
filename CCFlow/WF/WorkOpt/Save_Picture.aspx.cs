using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using BP.Sys;
using BP.Web;


namespace TYApp.WF.PicSignature
{
    [ScriptService]
    public partial class Save_Picture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod()]
        public static void UploadPic(string imageData, string WorkID, string FK_Node, string FK_Flow)
        {
            string basePath = SystemConfig.PathOfDataUser + "HandWritingImg";
            string ny = DateTime.Now.ToString("yyyy_MM");
            string tempPath = basePath + "\\" + ny + "\\" + FK_Flow;
            string tempName = WorkID + "_" + FK_Node + "_" + WebUser.No + ".png";
            //string tempName = WorkID + "_" + FK_Node + "_" + "12345678.png";
            if (System.IO.Directory.Exists(tempPath) == false)
                System.IO.Directory.CreateDirectory(tempPath);
            string Pic_Path = tempPath + "\\" + tempName;

            using (FileStream fs = new FileStream(Pic_Path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
            }
        }
    }
}




