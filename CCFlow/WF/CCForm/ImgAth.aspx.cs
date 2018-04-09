using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;


namespace CCFlow.WF.CCForm
{
    public partial class ImgAth : BP.Web.PageBase
    {
        #region 属性.
        string sourceFile = "";
        public int H
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["H"]);
                }
                catch
                {
                    return 120;
                }
            }
        }
        public int W
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["W"]);
                }
                catch
                {
                    return 100;
                }
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string ImgAths
        {
            get
            {
                return this.Request.QueryString["ImgAth"];
            }
        }

        public string MSG
        {
            get { return this.Request.QueryString["MSG"]; }
        }
        public string MSGWidth
        {
            get { return this.Request.QueryString["MSGWidth"]; }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack==false)
            {
                if (DataType.IsNullOrEmpty(MSG))
                {

                    sourceFile = BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Def.jpg";
                    string myPK = this.ImgAths + "_" + this.MyPK;
                    BP.Sys.FrmImgAthDB imgDB = new BP.Sys.FrmImgAthDB();
                    imgDB.MyPK = myPK;
                    imgDB.RetrieveFromDBSources();

                    if (imgDB != null && !DataType.IsNullOrEmpty(imgDB.FileFullName))
                    {
                        if (System.IO.File.Exists(Server.MapPath(imgDB.FileFullName)))
                            sourceFile = imgDB.FileFullName;
                    }
                    sourceImg.Value = sourceFile;
                    newImgUrl.Value = sourceFile;
                    Page.ClientScript.RegisterStartupScript(this.GetType(),
                        "js", "<script>ImageCut('" + sourceFile + "','" + W + "','" + H + "' )</script>");
                }
                else
                {
                    sourceImg.Value = MSG;
                    newImgUrl.Value = MSG;
                    Page.ClientScript.RegisterStartupScript(this.GetType(),
                        "js", "<script>ImageCut('" + MSG + "','" + W + "','" + H + "' )</script>");
                }
            }
        }
        //确定按钮
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            sourceFile = sourceImg.Value;
            string myName = this.ImgAths + "_" + this.MyPK;
            string type = sourceFile.Substring(sourceFile.LastIndexOf(".") + 1); //得到文件后缀名 
            FileInfo fileInfo = new FileInfo(Server.MapPath(sourceFile));
            float fileSize = 0;
            if (fileInfo.Exists)
                fileSize = float.Parse(fileInfo.Length.ToString());

            BP.Sys.FrmImgAthDB imgDB = new BP.Sys.FrmImgAthDB();
            imgDB.FK_MapData = this.FK_MapData;
            imgDB.FK_FrmImgAth = this.ImgAths;
            imgDB.RefPKVal = this.MyPK;
            imgDB.FileFullName = sourceFile;
            imgDB.FileName = myName;
            imgDB.FileExts = type;
            imgDB.FileSize = fileSize;
            imgDB.RDT = DateTime.Now.ToString("yyyy-MM-dd mm:HH");
            imgDB.Rec = BP.Web.WebUser.No;
            imgDB.RecName = BP.Web.WebUser.Name;

            try
            {
                imgDB.Save();
            }
            catch
            {
                imgDB.CheckPhysicsTable();
                imgDB.Save();
            }

            CopyFile(newImgUrl.Value, BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Data/" + myName + ".png");
            this.WinClose(newImgUrl.Value);
        }

        //复制文件
        public void CopyFile(string SourceFile, string ObjectFile)
        {
            string sourceFilePath = Server.MapPath(SourceFile);
            string objectFile = Server.MapPath(ObjectFile);
            if (System.IO.File.Exists(sourceFilePath))
            {
                System.IO.File.Copy(sourceFilePath, objectFile, true);
            }
        }

        protected void btnImaeSave_Click(object sender, EventArgs e)
        {
            if (fileToUpload.HasFile)
            {
                
                string msg = "";
             
                if (fileToUpload.FileBytes.Length==0)
                    this.Alert("文件长度为0!");
                else
                {
                    string newName = this.Request.QueryString["ImgAth"] + "_" + this.MyPK + "_" +
                                     DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                    string saveToPath = Server.MapPath(BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Upload");
                    if (!Directory.Exists(saveToPath))
                        Directory.CreateDirectory(saveToPath);
                    saveToPath = saveToPath + "\\" + newName;
                    fileToUpload.SaveAs(saveToPath);
                    msg = BP.WF.Glo.CCFlowAppPath + "DataUser/ImgAth/Upload/" + newName;
                }


                this.Response.Redirect("ImgAth.aspx?MyPK=" + this.MyPK + "&MSG=" + msg + "&MSGWidth=" + GetImage(msg, 120) + "&FK_MapData=" + this.FK_MapData + "&ImgAth="+this.ImgAths+"&W="+this.W+"&H="+this.H);
            }
            else
                this.Alert("文件为空!");
        }

        protected void refresh_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(),
            "js", "<script>ImageCut('" + sourceImg.Value + "','" + W + "','" + H + "' )</script>");
        }
        public int GetImage(string url, int widthSize)
        {
            if (File.Exists(System.Web.HttpContext.Current.Server.MapPath(url)))
            {
                System.Drawing.Image imgOutput = System.Drawing.Bitmap.FromFile(System.Web.HttpContext.Current.Server.MapPath(url));
                if (imgOutput.Width > widthSize)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}