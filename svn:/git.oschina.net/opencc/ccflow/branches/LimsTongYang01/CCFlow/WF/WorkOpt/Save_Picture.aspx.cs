using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Web.Script.Services;
using System.Web.Services;


namespace TYApp.WF.PicSignature
{
[ScriptService]
    public partial class Save_Picture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    [WebMethod()]
    public static void UploadPic(string imageData)
    {
        string Pic_Path = HttpContext.Current.Server.MapPath("MyPicture.png");
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




