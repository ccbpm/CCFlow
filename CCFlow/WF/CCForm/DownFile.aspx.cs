using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.WF.Template;
using BP.WF;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;

namespace CCFlow.WF.CCForm
{
    public partial class WF_CCForm_DownFile : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// ath.
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        public string IsReadonly
        {
            get
            {
                return this.Request.QueryString["IsReadonly"];
            }
        }
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        public string FK_FrmAttachment
        {
            get
            {
                return this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_FrmAttachmentExt
        {
            get
            {
                return "ND" + this.FK_Node + "_DocMultiAth"; // this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int _fk_node = 0;
        public int FK_Node
        {
            get
            {
                if (_fk_node == 0 && !string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return int.Parse(this.Request.QueryString["FK_Node"]);

                return _fk_node;
            }
            set
            {
                _fk_node = value;
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = this.Request.QueryString["WorkID"];
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["OID"];

                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["PKVal"];
                return Int64.Parse(str);
            }
        }
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (string.IsNullOrEmpty(str))
                    return 0;
                return Int64.Parse(str);
            }
        }
        public string FK_MapData
        {
            get
            {
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrEmpty(fk_mapdata))
                    fk_mapdata = "ND" + FK_Node;
                return fk_mapdata;
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
            }
        }
        public string IsCC
        {
            get
            {
                string paras = this.Request.QueryString["Paras"];
                if (string.IsNullOrEmpty(paras) == false)
                    if (paras.Contains("IsCC=1") == true)
                        return "1";
                return "ssss";
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            this.Request.ContentEncoding = System.Text.UTF8Encoding.UTF8;


            if (this.DoType == "Down")
            {
                FrmAttachmentDB downDB = new FrmAttachmentDB();

                downDB.MyPK = this.DelPKVal == null ? this.MyPK : this.DelPKVal;
                downDB.Retrieve();
                FrmAttachment dbAtt = new FrmAttachment();
                dbAtt.MyPK = downDB.FK_FrmAttachment;
                dbAtt.Retrieve();
                if (dbAtt.AthSaveWay == AthSaveWay.IISServer)
                {
                    #region 解密下载
                    //1、先解密到本地
                    string filepath = downDB.FileFullName + ".tmp";
                    string tempName = downDB.FileFullName;

                    EncHelper.DecryptDES(downDB.FileFullName, filepath);
                    //PubClass.DownloadFile(filepath, tempName);
                    #region 文件下载（并删除临时明文文件）
                    if (!"firefox".Contains(HttpContext.Current.Request.Browser.Browser.ToLower()))
                        tempName = HttpUtility.UrlEncode(tempName);

                    HttpContext.Current.Response.Charset = "GB2312";
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + tempName);
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    HttpContext.Current.Response.ContentType = "application/octet-stream;charset=utf8";
                    //HttpContext.Current.Response.ContentType = "application/ms-msword";  //image/JPEG;text/HTML;image/GIF;application/ms-excel
                    //HttpContext.Current.EnableViewState =false;

                    HttpContext.Current.Response.WriteFile(filepath);
                    File.Delete(filepath);
                    HttpContext.Current.Response.End();
                    HttpContext.Current.Response.Close();
                    #endregion

                    #endregion
                }

                if (dbAtt.AthSaveWay == AthSaveWay.FTPServer)
                {
                    PubClass.DownloadFile(downDB.GenerTempFile(dbAtt.AthSaveWay), downDB.FileName);
                }

                if (dbAtt.AthSaveWay == AthSaveWay.DB)
                {
                    string downpath = GetRealPath(downDB.FileFullName);
                    BP.Sys.PubClass.DownloadFile(downpath, downDB.FileName);
                    PubClass.DownloadHttpFile(downDB.FileFullName, downDB.FileName);
                }
               
                this.WinClose();
                return;
            }
           
        }

        void btn_DownLoad_Zip(object sender, EventArgs e)
        {
            try
            {
                BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
                dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment,
                        FrmAttachmentDBAttr.RefPKVal, this.PKVal);

                if (dbs.Count == 0)
                {
                    this.Alert("文件不存在，不需打包下载。");
                    return;
                }

                string zipName = this.WorkID + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string basePath = Server.MapPath("//DataUser//Temp");
                string tempPath = basePath + "//" + WebUser.No;
                string zipPath = basePath + "//" + WebUser.No;
                string zipFile = zipPath + "//" + zipName + ".zip";

                //删除临时文件，保证一个用户只能存一份，减少磁盘占用空间
                if (System.IO.Directory.Exists(tempPath) == true)
                    System.IO.Directory.Delete(tempPath, true);
                //根据路径创建文件夹
                if (System.IO.Directory.Exists(zipPath) == false)
                    System.IO.Directory.CreateDirectory(zipPath);
                //copy文件临时文件夹
                tempPath = tempPath + "//" + this.WorkID;
                if (System.IO.Directory.Exists(tempPath) == false)
                    System.IO.Directory.CreateDirectory(tempPath);

                foreach (FrmAttachmentDB db in dbs)
                {
                    string copyToPath = tempPath;
                    if (!File.Exists(db.FileFullName)) continue;

                    if (!string.IsNullOrEmpty(db.Sort))
                    {
                        copyToPath = tempPath + "//" + db.Sort;
                        if (System.IO.Directory.Exists(copyToPath) == false)
                            System.IO.Directory.CreateDirectory(copyToPath);
                    }
                    //新文件目录
                    copyToPath = copyToPath + "//" + db.FileName;
                    File.Copy(db.FileFullName, copyToPath, true);
                }
                //执行压缩
                (new FastZip()).CreateZip(zipFile, tempPath, true, "");
                //删除临时文件夹
                System.IO.Directory.Delete(tempPath, true);
                //显示出下载超链接
               // BP.Web.Controls.BPHyperLink hLink = (BP.Web.Controls.BPHyperLink)this.Pub1.FindControl("H_LINK_Btn");
                
                //BP.PubClass.DownloadFile(zipFile, this.WorkID + ".zip");
            }
            catch (Exception ex)
            {
                //this.Alert(ex.Message);
            }
        }
      



        private string GetRealPath(string fileFullName)
        {
            bool isFile = false;
            string downpath = "";
            try
            {
                //如果相对路径获取不到可能存储的是绝对路径
                FileInfo downInfo = new FileInfo(Server.MapPath("~/" + fileFullName));
                isFile = true;
                downpath = Server.MapPath("~/" + fileFullName);
            }
            catch (Exception)
            {
                FileInfo downInfo = new FileInfo(fileFullName);
                isFile = true;
                downpath = fileFullName;
            }
            if (!isFile)
            {
                throw new Exception("没有找到下载的文件路径！");
            }

            return downpath;
        }

        
    }
    public static class EncHelper
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };//自定义密匙

        private static string encryptKey = "ccflow123";

        public static bool EncryptDES(string inFile, string outFile)
        {
            byte[] rgb = Keys;
            try
            {
                byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                FileStream inFs = new FileStream(inFile, FileMode.Open, FileAccess.Read);//读入流
                FileStream outFs = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);// 等待写入流
                outFs.SetLength(0);//帮助读写的变量
                byte[] byteIn = new byte[100];//放临时读入的流
                long readLen = 0;//读入流的长度
                long totalLen = inFs.Length;//读入流的总长度
                int everylen = 0;//每次读入流的长度
                DES des = new DESCryptoServiceProvider();//将inFile加密后放到outFile
                CryptoStream encStream = new CryptoStream(outFs, des.CreateEncryptor(rgb, rgbKeys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    everylen = inFs.Read(byteIn, 0, 100);
                    encStream.Write(byteIn, 0, everylen);
                    readLen = readLen + everylen;
                }
                encStream.Close();
                inFs.Close();
                outFs.Close();
                return true;//加密成功
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message.ToString());
                return false;//加密失败
            }
        }

        public static bool DecryptDES(string inFile, string outFile)
        {
            byte[] rgb = Keys;
            try
            {
                byte[] rgbKeys = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                FileStream inFs = new FileStream(inFile, FileMode.Open, FileAccess.Read);//读入流
                FileStream outFs = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);// 等待写入流
                outFs.SetLength(0);//帮助读写的变量
                byte[] byteIn = new byte[100];//放临时读入的流
                long readLen = 0;//读入流的长度
                long totalLen = inFs.Length;//读入流的总长度
                int everylen = 0;//每次读入流的长度
                DES des = new DESCryptoServiceProvider();//将inFile加密后放到outFile
                CryptoStream encStream = new CryptoStream(outFs, des.CreateDecryptor(rgb, rgbKeys), CryptoStreamMode.Write);
                while (readLen < totalLen)
                {
                    everylen = inFs.Read(byteIn, 0, 100);
                    encStream.Write(byteIn, 0, everylen);
                    readLen = readLen + everylen;
                }
                encStream.Close();
                inFs.Close();
                outFs.Close();
                return true;//加密成功
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message.ToString());
                return false;//加密失败
            }
        }

    }
}