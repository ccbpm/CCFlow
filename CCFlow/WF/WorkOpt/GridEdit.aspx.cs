using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.WorkOpt
{
    public partial class GridEdit : System.Web.UI.Page
    {
        #region

        public string FileName
        {
            get
            {
                string grfName = Request.QueryString["grf"];

                string[] nameData = grfName.Split('\\');


                if (nameData.Length > 2)
                {
                    return nameData[nameData.Length - 1];
                }
                else
                {
                    return grfName;
                }


            }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request.QueryString["method"];
            if (!string.IsNullOrEmpty(method))
            {

                switch (method)
                {
                    case "save":
                        byte[] FormData = Request.BinaryRead(Request.TotalBytes);

                        //写入上传数据，最后保存到文件
                        string strPathFile = Server.MapPath("~") + "DataUser\\CyclostyleFile\\" + Request.QueryString["grf"];
                        BinaryWriter bw = new BinaryWriter(File.Create(strPathFile));
                        bw.Write(FormData);
                        bw.Close();
                        break;
                    case "load":
                        string loadPathFile = Server.MapPath("~") + "DataUser\\CyclostyleFile\\" + Request.QueryString["grf"];
                        Response.Clear();
                             Response.WriteFile(loadPathFile);
                        break;
                    default:
                        break;
                }


             }

        }
    }
}